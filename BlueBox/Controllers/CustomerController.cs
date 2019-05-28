using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlueBox.DataLayer.Models;
using BlueBox.DataLayer;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using BlueBox.Services;
using BlueBox.Helpers;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BlueBox.Controllers
{
    [Authorize]
    [Route("api/[customer]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly BBDbContext _context;
        private CustomerService _customerService;
        private readonly AppSettings _appSettings;

        public CustomerController(BBDbContext context, CustomerService service, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _customerService = service;
            _appSettings = appSettings.Value;
        }

        //allow user to login
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]Customer customer)
        {
            var cust = _customerService.Authenticate(customer.Email, customer.Password);

            if (cust == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, cust.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                Id = cust.Id,
                Username = cust.Email,
                FirstName = cust.FirstName,
                LastName = cust.LastName,
                Token = tokenString
            });
        }

        //GET: api/customers - return the full list of all customers regardless of genre
        [HttpGet]
        public IEnumerable<Customer> Getcustomers()
        {
            return _customerService.GetAll();
        }

        // GET: api/customers/5
        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomer(int id)
        {
            var customer = _customerService.GetById(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }
        // POST: api/customers - Add new customer to catalog, only usable by admin
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer item)
        {
            _context.Customers.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Getcustomers), new { id = item.FirstName }, item);
        }

        // DELETE: api/customers/5 - delete customer from catalog, only usable by admin
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletecustomer(long id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

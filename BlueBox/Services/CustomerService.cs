using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using BlueBox.DataLayer;
using BlueBox.DataLayer.Models;
using BlueBox.Helpers;

namespace BlueBox.Services
{
    public interface ICustomerService
    {
        Customer Authenticate(string username, string password);
        IEnumerable<Customer> GetAll();
        Customer GetById(int id);
        Customer Create(Customer customer, string password);
        void Update(Customer user, string password = null);
        void Delete(int id);
    }
    public class CustomerService : ICustomerService
    {
        private BBDbContext _context;

        public CustomerService(BBDbContext context)
        {
            _context = context;
        }

        public Customer Authenticate(string username, string password)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var customer = _context.Customers.SingleOrDefault(x => x.Email == username);

            if(customer == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, customer.PasswordHash, customer.PasswordSalt))
            {
                return null;
            }

            return customer;
        }

        public Customer Create(Customer customer, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Customers.Any(x => x.Email == customer.Email))
                throw new AppException("Username \"" + customer.Email + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            customer.PasswordHash = passwordHash;
            customer.PasswordSalt = passwordSalt;

            _context.Customers.Add(customer);
            _context.SaveChanges();

            return customer;
        }

        public void Delete(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers;
        }

        public Customer GetById(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null)
                return null;

            return customer;
        }

        public void Update(Customer customer, string password = null)
        {
            var cust = _context.Customers.Find(customer.Id);

            if (cust == null)
                throw new AppException("User not found");

            if (customer.Email != cust.Email)
            {
                // email as username has changed so check if the new username is already taken
                if (_context.Customers.Any(x => x.Email == customer.Email))
                    throw new AppException("Username " + customer.Email + " is already taken");
            }

            // update user properties
            cust.FirstName = customer.FirstName;
            cust.LastName = customer.LastName;
            cust.Email = customer.Email;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                cust.PasswordHash = passwordHash;
                cust.PasswordSalt = passwordSalt;
            }

            _context.Customers.Update(cust);
            _context.SaveChanges();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }

}

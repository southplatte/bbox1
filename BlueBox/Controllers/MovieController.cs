using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlueBox.DataLayer.Models;
using BlueBox.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace BlueBox.Controllers
{
    [Route("api/[movies]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly BBDbContext _context;

        public MovieController(BBDbContext context)
        {
            _context = context;
        }
        //GET: api/movies - return the full list of all movies regardless of genre
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movies.ToListAsync();
        }

        // GET: api/movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetTodoItem(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }
        // POST: api/movies - Add new movie to catalog, only usable by admin
        [HttpPost]
        public async Task<ActionResult<Movie>> PostTodoItem(Movie item)
        {
            _context.Movies.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovies), new { id = item.Id }, item);
        }

        // DELETE: api/movies/5 - delete movie from catalog, only usable by admin
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(long id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

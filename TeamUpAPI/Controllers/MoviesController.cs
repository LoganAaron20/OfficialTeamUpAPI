using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamUpAPI.Models;

namespace TeamUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieContext DB;

        public MoviesController(MovieContext db)
        {
            DB = db;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            if (DB.Movies == null)
            {
                return NotFound();
            }
            return await DB.Movies.ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            if (DB.Movies == null)
            {
                return NotFound();
            }
            var movie = await DB.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }
            return movie;
        }

        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            DB.Movies.Add(movie);
            await DB.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.ID }, movie);
        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.ID)
            {
                return BadRequest();
            }

            DB.Entry(movie).State = EntityState.Modified;

            try
            {
                await DB.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE: /api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (DB.Movies == null)
            {
                return NotFound();
            }

            var movie = await DB.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            DB.Movies.Remove(movie);
            await DB.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(long id)
        {
            return (DB.Movies?.Any(m => m.ID == id)).GetValueOrDefault();
        }
    }
}

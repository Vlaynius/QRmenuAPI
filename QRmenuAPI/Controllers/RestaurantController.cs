using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRmenuAPI.Data;
using QRmenuAPI.Models;

namespace QRmenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public RestaurantController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Restaurant
        [HttpGet]
        //[Authorize(Roles = "CompanyAdministrator")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurant()
        {
          if (_context.Restaurants == null)
          {
              return NotFound();
          }
            return await _context.Restaurants.ToListAsync();
        }

        // GET: api/Restaurant/5
        [HttpGet("{id}")]
        //[Authorize(Roles = ("RestaurantAdministrator,CompanyAdministrator"))]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
          if (_context.Restaurants == null)
          {
              return NotFound();
          }
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        // PUT: api/Restaurant/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestaurant(int id, Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return BadRequest();
            }

            _context.Entry(restaurant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
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

        // POST: api/Restaurant
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Restaurant>> PostRestaurant(Restaurant restaurant)
        {
          if (_context.Restaurants == null)
          {
              return Problem("Entity set 'ApplicationContext.Restaurant'  is null.");
          }
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRestaurant", new { id = restaurant.Id }, restaurant);
        }

        // DELETE: api/Restaurant/5
        [HttpDelete("{id}")]
        public ActionResult DeleteRestaurant(int id)
        {
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            Restaurant? restaurant = _context.Restaurants.Where(r => r.Id == id).Include(r => r.Categories)!.ThenInclude(r => r.Foods)!.FirstOrDefault();
            if (restaurant != null)
            {
                restaurant.StateId = 0;
                if(restaurant.Categories != null)
                {
                    foreach (Category cat in restaurant.Categories)
                    {
                        cat.StateId = 0;
                        if(cat.Foods != null)
                        {
                            foreach (Food food in cat.Foods)
                            {
                                food.StateId = 0;
                            }
                        }
                        
                    }
                }
                
            }
            _context.Restaurants.Update(restaurant);
            _context.SaveChanges();
            
            return NoContent();
        }

        private bool RestaurantExists(int id)
        {
            return (_context.Restaurants?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

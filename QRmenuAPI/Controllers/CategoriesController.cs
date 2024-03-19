using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRmenuAPI.Data;
using QRmenuAPI.Models;

namespace QRmenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public CategoriesController(ApplicationContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // GET: api/Categories
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);
           

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "CompanyAdministrator, RestaurantAdministrator")]
        //Claim --> Company Tüm restoranlarını , Restaurant kendi restaurant'larını editleyebilmili
        public ActionResult PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }
            ApplicationUser currentUser = _signInManager.UserManager.GetUserAsync(User).Result;
            if(User.HasClaim("RestaurantId", category.RestaurantId.ToString()))
            {

            }
            else
            {
                var restaurant = _context.Restaurants.Where(r => r.Id == category.RestaurantId).FirstOrDefault();
                if (User.HasClaim("CompanyId", restaurant.CompanyId.ToString()))
                {

                }
                else
                {
                    Unauthorized();
                }

            }
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                 _context.SaveChangesAsync().Wait();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        //Claim --> Company Tüm restoranlarını , Restaurant kendi restaurant'larını editleyebilmili
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationContext.Categories'  is null.");
            }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        //Claim --> Company Tüm restoranlarını , Restaurant kendi restaurant'larını editleyebilmili
        public ActionResult DeleteCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            Category? category = _context.Categories!.Where(c => c.Id == id).Include(c => c.Foods).FirstOrDefault();
            if (category != null)
            {
                category.StateId = 0;
                foreach (Food food in category.Foods!)
                {
                    food.StateId = 0;
                }
            }
            _context.Categories?.Update(category!);
            _context.SaveChanges();
            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

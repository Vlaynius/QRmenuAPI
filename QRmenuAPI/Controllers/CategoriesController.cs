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
        public ActionResult PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }
            if(User.HasClaim("RestaurantId", category.RestaurantId.ToString()) == false)
            {
                Restaurant? restaurant = _context.Restaurants!.Where(r => r.Id == category.RestaurantId).FirstOrDefault();
                if(User.HasClaim("CompanyId", restaurant!.CompanyId.ToString()) == false)
                {
                    return Unauthorized();
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
        [Authorize(Roles = "CompanyAdministrator , RestaurantAdministrator")]
        public  ActionResult<Category> PostCategory(Category category)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationContext.Categories'  is null.");
            }
            Restaurant? restaurant = _context.Restaurants!.FindAsync(category.RestaurantId).Result;
            if(restaurant == null)
            {
                return Problem("Restaurant not found");
            }
            if(User.HasClaim("CompAdmin", restaurant.CompanyId.ToString()) == false )
            {
                if (User.HasClaim("RestaurantId", category.RestaurantId.ToString()) == false)
                {
                    return Unauthorized();
                }
            }
            _context.Categories.Add(category);
            _context.SaveChangesAsync().Wait();
            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "CompanyAdministrator , RestaurantAdministrator")]
        public ActionResult DeleteCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            Category? category = _context.Categories!.Where(c => c.Id == id).FirstOrDefault();
            if (category != null)
            {
                return Problem("Category not found");
            }
            Restaurant? restaurant = _context.Restaurants!.Find(category!.RestaurantId);
            if (User.HasClaim("CompanyId", restaurant!.CompanyId.ToString()) == false)
            {
                if (User.HasClaim("RestaurantId", restaurant.Id.ToString()) == false)
                {
                    return Unauthorized();
                }
            }

            category.StateId = 0;
            List<Food> foods = _context.Foods!.Where(f => f.CategoryId == category.Id).ToList();
            if (foods != null)
            {
                foreach (Food food in foods)
                {
                    food.StateId = 0;
                }
            }
            _context.Categories?.Update(category);
            _context.SaveChanges();
            return Ok("Silme işlemi Başarılı");
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

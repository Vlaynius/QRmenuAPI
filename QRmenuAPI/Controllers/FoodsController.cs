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
    public class FoodsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public FoodsController(ApplicationContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // GET: api/Foods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetFoods()
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            return await _context.Foods.ToListAsync();
        }

        // GET: api/Foods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }
            return food;
        }

        // PUT: api/Foods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "CompanyAdministrator, RestaurantAdministrator")]
        public ActionResult PutFood(int id, Food food)
        {
            if (id != food.Id)
            {
                return BadRequest();
            }
            ApplicationUser currentUser = _signInManager.UserManager.GetUserAsync(User).Result;
            Category? category = _context.Categories!.Where(c=>c.Id == food.CategoryId).FirstOrDefault();
            if (User.HasClaim("RestaurantId", category!.RestaurantId.ToString()) == false)
            {
                Restaurant? restaurant = _context.Restaurants!.Where(r => r.Id == category.RestaurantId).FirstOrDefault();
                if (User.HasClaim("CompanyId", restaurant!.CompanyId.ToString()) == false)
                {
                    Unauthorized();
                }
            }
            _context.Entry(food).State = EntityState.Modified;
            try
            {
                 _context.SaveChangesAsync().Wait();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(id))
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

        // POST: api/Foods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "CompanyAdministrator, RestaurantAdministrator")]
        public  ActionResult<Food> PostFood(Food food)
        {
            if (_context.Foods == null)
            {
                return Problem("Entity set 'ApplicationContext.Foods'  is null.");
            }
            Category? category = _context.Categories!.Where(c => c.Id == food.CategoryId).FirstOrDefault();
            if(category == null)
            {
                return Problem("Category that food assigned net found.");
            }
            Restaurant? restaurant = _context.Restaurants!.Where(r => r.Id == category.RestaurantId).FirstOrDefault();
            if(User.HasClaim("CompanyId", restaurant!.CompanyId.ToString()) == false)
            {
                if(User.HasClaim("RestaurantId", category.RestaurantId.ToString()) == false)
                {
                    return Unauthorized();
                }
            }
            _context.Foods.Add(food);
            _context.SaveChangesAsync().Wait();
            return CreatedAtAction("GetFood", new { id = food.Id }, food);
        }

        // DELETE: api/Foods/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "CompanyAdministrator, RestaurantAdministrator")]
        public  ActionResult DeleteFood(int id)
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            Food? food =  _context.Foods.Find(id);
            Category? category = _context.Categories!.Where(c => c.Id == food!.CategoryId).FirstOrDefault();
            Restaurant? restaurant = _context.Restaurants!.Where(r => r.Id == category!.RestaurantId).FirstOrDefault();
            if (User.HasClaim("CompanyId", restaurant!.CompanyId.ToString()) == false)
            {
                if (User.HasClaim("RestaurantId", category!.RestaurantId.ToString()) == false)
                {
                    return Unauthorized();
                }
            }
            if ( food != null)
            {
                food.StateId = 0;
            }
            return NoContent();
        }

        private bool FoodExists(int id)
        {
            return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public RestaurantController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Restaurant
        [HttpGet]
        [Authorize]
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
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
            var restaurant = await _context.Restaurants!.FindAsync(id);
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            if (restaurant == null)
            {
                return NotFound();
            }
            return restaurant;
        }

        // PUT: api/Restaurant/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "CompanyAdministrator, RestaurantAdministrator")]
        public ActionResult PutRestaurant(int id, Restaurant restaurant)
        {
            if (User.HasClaim("RestaurantId", id.ToString()) == true)
            {
                _context.Entry(restaurant).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok("Güncelleme Başarılı");
            }
            if(User.HasClaim("CompanyId", restaurant.CompanyId.ToString())== false)
            {
                return Unauthorized();
            }
            _context.Entry(restaurant).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok("Güncelleme Başarılı");
        }

        // POST: api/Restaurant
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "CompanyAdministrator")]
        public string PostRestaurant(Restaurant restaurant)
        {
            Claim claim;
            ApplicationUser applicationUser = new ApplicationUser();
            _context.Restaurants!.Add(restaurant);
            _context.SaveChanges();
            applicationUser.CompanyId = restaurant.CompanyId;
            applicationUser.Email = "abc@def";
            applicationUser.Name =restaurant.Name + "Administrator";
            applicationUser.PhoneNumber = "11122233344";
            applicationUser.RegisterDate = DateTime.Today;
            applicationUser.StateId = 1;
            applicationUser.UserName = "RestaurantAdministrator" + restaurant.Id.ToString();
            _userManager.CreateAsync(applicationUser).Wait();
            _userManager.AddPasswordAsync(applicationUser, "Admin123!").Wait();
            _userManager.AddToRoleAsync(applicationUser, "RestaurantAdministrator").Wait(); //Restaurant admin'e claim ver
            claim = new Claim("RestaurantId", restaurant.Id.ToString());
            _userManager.AddClaimAsync(applicationUser, claim).Wait();
            var appUser = _context.Users.Where(c => c.CompanyId == restaurant.CompanyId).FirstOrDefault();
            _userManager.AddClaimAsync(appUser!, claim);    //Company admin'e claim ver
            string Info = "RestaurantId: " + restaurant.Id + "\nUserName: " + applicationUser.UserName  ;
            return Info;
        }

        // DELETE: api/Restaurant/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "CompAdmin")]
        public ActionResult DeleteRestaurant(int id)
        {
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            Restaurant? restaurant = _context.Restaurants.Where(r => r.Id == id).FirstOrDefault();
            if (restaurant == null)
            {
                return NotFound();
            }
            if (User.HasClaim("CompanyId", restaurant.CompanyId.ToString()) == false)
            {
                return Unauthorized();
            }
            restaurant.StateId = 0;
            List<Category>? categories = _context.Categories!.Where(c => c.RestaurantId == restaurant.Id).ToList();
            if (categories != null)
            {
                foreach (Category cat in categories)
                {
                    cat.StateId = 0;
                    List<Food> foods = _context.Foods!.Where(f => f.CategoryId == cat.Id).ToList();
                    if (foods != null)
                    {
                        foreach (Food food in foods)
                        {
                            food.StateId = 0;
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


        //[HttpGet("Menu")]
        //public ActionResult Menu(int restaurantId)
        //{
          
        //    Restaurant? restaurant = _context.Restaurants.Where(r => r.Id == restaurantId).Include(r=>r.).FirstOrDefault();
        //    if(restaurant == null && restaurant.StateId != 1)
        //    {
        //        return NotFound();
        //    }
        //    List<Category> categories = _context.Categories.Where(c => c.RestaurantId == restaurantId).ToList();
        //    foreach (Category cat in categories)
        //    {
        //        res.Add(cat);
        //    }



        //}

    }
}

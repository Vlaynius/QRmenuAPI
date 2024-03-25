using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRmenuAPI.Data;
using QRmenuAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace QRmenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CompaniesController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Companies
        [Authorize(Roles = "Administrator")]
        [HttpGet("GetCompanies")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            return await _context.Companies.ToListAsync();
        }

        // GET: api/Companies/5
        [Authorize]
        [HttpGet("GetCompany")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            ApplicationUser currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser.CompanyId != id)
            {
                return Unauthorized();
            }
            if (_context.Companies == null)
            {
                return NotFound();
            }
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }
            return company;
        }

        // PUT: api/Companies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutCompany")]
        [Authorize(Policy = "CompAdmin")]
        public ActionResult PutCompany(int id, Company company)
        {
            if (User.HasClaim("CompanyId", id.ToString()) == false)
            {
                return Unauthorized();
            }
            _context.Entry(company).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        // POST: api/Companies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Administrator")]
        [HttpPost("PostCompany")]
        public string PostCompany(Company company)
        {
            Claim Compclaim;
            ApplicationUser applicationUser = new ApplicationUser();
            _context.Companies!.Add(company);
            _context.SaveChanges();
            applicationUser.CompanyId = company.Id;
            applicationUser.Email = company.Email;
            applicationUser.Name = company.Name + "Administrator";
            applicationUser.PhoneNumber = company.Phone;
            applicationUser.RegisterDate = DateTime.Today;
            applicationUser.StateId = 1;
            applicationUser.UserName = "CompanyAdministrator" + company.Id.ToString();
            _userManager.CreateAsync(applicationUser).Wait();
            _userManager.AddToRoleAsync(applicationUser, "CompanyAdministrator").Wait();
            _userManager.AddPasswordAsync(applicationUser, "Admin123!").Wait();
            Compclaim = new Claim("CompanyId", applicationUser.CompanyId.ToString());
            _userManager.AddClaimAsync(applicationUser, Compclaim).Wait();
            string Info = "CompanyId: " + company.Id.ToString() + "\nUserName: " + applicationUser.UserName ;
            return Info;
        }

        // DELETE: api/Companies/5
        [HttpDelete("DeleteCompany")]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteCompany(int id)
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            Company? company = _context.Companies!.Where(c => c.Id == id).FirstOrDefault();
            List<ApplicationUser>? applicationUser = _userManager.Users.Where(u => u.CompanyId == company!.Id).ToList();
            List<Restaurant>? restaurants = _context.Restaurants!.Where(r => r.CompanyId == company!.Id).ToList();
            if (company != null)
            {
                company.StateId = 0;
                if (applicationUser != null)
                {
                    foreach (ApplicationUser user in applicationUser)
                    {
                        user.StateId = 0;
                    }
                }
                if (restaurants != null)
                {
                    foreach (Restaurant rest in restaurants)
                    {
                        rest.StateId = 0;
                        List<Category>? categories = _context.Categories!.Where(c => c.RestaurantId == rest.Id).ToList();
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
                    }
                }
                _context.Companies!.Update(company);
            }
            _context.SaveChanges();
            return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

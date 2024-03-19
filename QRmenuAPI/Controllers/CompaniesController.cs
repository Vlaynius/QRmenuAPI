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
        private readonly UserManager<ApplicationUser> _userManager ;
        public CompaniesController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Companies
        [Authorize(Roles = "Administrator")]
        [HttpGet]
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
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
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
        [HttpPut("{id}")]
        [Authorize(Policy = "CompAdmin")]
        public  ActionResult PutCompany(int id, Company company)
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
        [HttpPost]
        public int PostCompany(Company company)
        {
            Claim Compclaim ;
            ApplicationUser applicationUser = new ApplicationUser();
            _context.Companies!.Add(company);
            _context.SaveChanges();
            applicationUser.CompanyId = company.Id;
            applicationUser.Email = "abc@def";
            applicationUser.Name = company.Name +"Administrator";
            applicationUser.PhoneNumber = "11122233344";
            applicationUser.RegisterDate = DateTime.Today;
            applicationUser.StateId = 1;
            applicationUser.UserName = "CompanyAdministrator" + company.Id.ToString();
            _userManager.CreateAsync(applicationUser).Wait();
            _userManager.AddToRoleAsync(applicationUser, "CompanyAdministrator").Wait();
            Compclaim = new Claim("CompanyId", applicationUser.CompanyId.ToString());
            _userManager.AddClaimAsync(applicationUser, Compclaim).Wait();
            
            return company.Id;
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Administrator,CompanyAdministrator")]
        [Authorize(Roles ="Administrator")]
        public ActionResult DeleteCompany(int id)
        {
            
       
            if (_context.Companies == null)
            {
                return NotFound();
            }

            Company? company = _context.Companies!.Where(c => c.Id == id).Include(c => c.applicationUsers).Include(c => c.Restaurants)!.ThenInclude(c => c.Categories).FirstOrDefault();
            if (company != null)
            {
                company.StateId = 0;
                if(company.applicationUsers != null)
                {
                    foreach (ApplicationUser user in company.applicationUsers!)
                    {
                        user.StateId = 0;
                    }
                }

                if(company.Restaurants != null)
                {
                    foreach (Restaurant rest in company.Restaurants!)
                    {
                        rest.StateId = 0;
                        if(rest.Categories != null)
                        {
                            foreach (Category cat in rest.Categories!)
                            {
                                cat.StateId = 0;
                                if(cat.Foods != null)
                                {
                                    foreach (Food food in cat.Foods!)
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

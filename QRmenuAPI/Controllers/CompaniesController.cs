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
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (User.HasClaim("CompanyId", id.ToString()) == false)
            {
                return Unauthorized();
            }
            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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

        // POST: api/Companies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public int PostCompany(Company company)
        {
            Claim claim;
            ApplicationUser applicationUser = new ApplicationUser();
            _context.Companies!.Add(company);
            _context.SaveChanges();
            applicationUser.CompanyId = company.Id;
            applicationUser.Email = "abc@def";
            applicationUser.Name = "Administrator";
            applicationUser.PhoneNumber = "11122233344";
            applicationUser.RegisterDate = DateTime.Today;
            applicationUser.StateId = 1;
            applicationUser.UserName = "Administrator" + company.Id.ToString();
            _userManager.CreateAsync(applicationUser).Wait();
            _userManager.AddToRoleAsync(applicationUser, "CompanyAdministrator").Wait();
            claim = new Claim("CompanyId", applicationUser.CompanyId.ToString());
            _userManager.AddClaimAsync(applicationUser, claim).Wait();

            return company.Id;
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,CompanyAdministrator")]
        public async Task<IActionResult> DeleteCompany(int id)
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

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

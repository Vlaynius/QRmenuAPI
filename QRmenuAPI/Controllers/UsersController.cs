using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRmenuAPI.Data;
using QRmenuAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace QRmenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
           return await _userManager.Users.ToListAsync();
            
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetApplicationUser(string id)
        {
          
            var applicationUser = await _userManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return applicationUser;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationUser(string id, ApplicationUser applicationUser/*, string? Password = null , string? currentPassword = null?*/)
        {
            var ExistingApplicationUser = await _userManager.FindByIdAsync(id);
            ExistingApplicationUser.Name = applicationUser.Name;
            ExistingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            ExistingApplicationUser.Email = applicationUser.Email;
            ExistingApplicationUser.StateId = applicationUser.StateId;

            await _userManager.UpdateAsync(ExistingApplicationUser);

            //if(Password != null)
            //{
            //   await _userManager.ChangePasswordAsync(ExistingApplicationUser, currentPassword, Password);
            //}

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> PostApplicationUser(ApplicationUser applicationUser , string Password)
        {
            await _userManager.CreateAsync(applicationUser,Password);

            return CreatedAtAction("GetApplicationUser", new { id = applicationUser.Id }, applicationUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationUser(string id)
        {

            var applicationUser = await _userManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            //await _userManager.DeleteAsync(applicationUser);
            applicationUser.StateId = 0;
            await _userManager.UpdateAsync(applicationUser);
            
            return NoContent();
        }

        private bool ApplicationUserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(ApplicationContext context, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
           return await _signInManager.UserManager.Users.ToListAsync();
            
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetApplicationUser(string id)
        {
          
            var applicationUser = await _signInManager.UserManager.FindByIdAsync(id);

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
            var ExistingApplicationUser = await _signInManager.UserManager.FindByIdAsync(id);
            ExistingApplicationUser.Name = applicationUser.Name;
            ExistingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            ExistingApplicationUser.Email = applicationUser.Email;
            ExistingApplicationUser.StateId = applicationUser.StateId;

            await _signInManager.UserManager.UpdateAsync(ExistingApplicationUser);

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
            await _signInManager.UserManager.CreateAsync(applicationUser,Password);

            return CreatedAtAction("GetApplicationUser", new { id = applicationUser.Id }, applicationUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationUser(string id)
        {

            var applicationUser = await _signInManager.UserManager.FindByIdAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            //await _userManager.DeleteAsync(applicationUser);
            applicationUser.StateId = 0;
            await _signInManager.UserManager.UpdateAsync(applicationUser);
            
            return NoContent();
        }

        private bool ApplicationUserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //LogIn
        [HttpPost("LogIn")]
        public async Task<bool> LogIn(string userName, string password)
        {
          
            ApplicationUser user =  _signInManager.UserManager.FindByNameAsync(userName).Result;
            if (user == null)
            {
                return false; //false
            }
            Microsoft.AspNetCore.Identity.SignInResult signInResult =
                _signInManager.PasswordSignInAsync(user, password, false, false).Result;
            
            return  signInResult.Succeeded;
        }

        [HttpPost("ForgetPassword")]
        public  void ForgetPassword(string userName, string NewPassword)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            if(User == null)
            {
                return ;//Kullanıcı Yok
            }
            //var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(applicationUser);
            //var resetPasswordResult = await _signInManager.UserManager.ResetPasswordAsync(applicationUser, token, NewPassword);
            
            _signInManager.UserManager.RemovePasswordAsync(applicationUser).Wait();
            _signInManager.UserManager.AddPasswordAsync(applicationUser,NewPassword).Wait();

           

            //return resetPasswordResult.Succeeded;
        }

        [HttpPost("ChangePassword")]
        public async Task<bool> ChangePassword(string userName, string currentPassword, string NewPassword)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            if (User == null)
            {
                return false;//Kullanıcı Yok
            }

            var changePasswordResult = await _signInManager.UserManager.ChangePasswordAsync(applicationUser,currentPassword,NewPassword);
            return changePasswordResult.Succeeded;
        }

        [HttpPost("PasswordReset")]
        public string? PasswordReset(string userName)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            if (User == null)
            {
                return null;//Kullanıcı Yok
            }
            return _signInManager.UserManager.GeneratePasswordResetTokenAsync(applicationUser).Result;
        }
        [HttpPost("ValidateResetPassword")]
        public ActionResult<string> ValidateResetPassword(string UserName, string token, string newPassword)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(UserName).Result;
            if (User == null)
            {
                return NotFound();//Kullanıcı Yok
            }
            IdentityResult identityResult = _signInManager.UserManager.ResetPasswordAsync(applicationUser, token, newPassword).Result;

            if (!identityResult.Succeeded)
            {
                return identityResult.Errors.First().Description;
            }
            return Ok("Password Reset Successfull");
        }

        [HttpPost("AssignRole")]
        public void AssignRole(string userId , string roleId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            IdentityRole applicationRole = _roleManager.FindByIdAsync(roleId).Result;
            _signInManager.UserManager.AddToRoleAsync(applicationUser, applicationRole.Name);
        }
    }
}

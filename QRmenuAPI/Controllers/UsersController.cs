using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRmenuAPI.Data;
using QRmenuAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


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
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ApplicationUser>> GetApplicationUser(string id)
        {
          
            var applicationUser = await _signInManager.UserManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return applicationUser;
        }

        //[Authorize(Roles = "CompanyAdministrator")]
        [HttpPut("{id}")]
        //Claim --> Her kullanıcı kendisini ve altındakini güncelleyebilmeli
        public OkResult PutApplicationUser(ApplicationUser applicationUser)
        {
            ApplicationUser existingApplicationUser = _signInManager.UserManager.FindByIdAsync(applicationUser.Id).Result;

            existingApplicationUser.Email = applicationUser.Email;
            existingApplicationUser.Name = applicationUser.Name;
            existingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            existingApplicationUser.StateId = applicationUser.StateId;
            existingApplicationUser.UserName = applicationUser.UserName;
            _signInManager.UserManager.UpdateAsync(existingApplicationUser);
            return Ok();
        }

        [Authorize(Roles = "CompanyAdministrator")]
        [HttpPost]
        public string PostApplicationUser(ApplicationUser applicationUser, string passWord)
        {
            _signInManager.UserManager.CreateAsync(applicationUser, passWord).Wait();
            return applicationUser.Id;
        }

        [Authorize(Roles = "CompanyAdministrator")]
        [HttpDelete("{id}")]
        public ActionResult DeleteApplicationUser(string id)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;

            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.StateId = 0;
            _signInManager.UserManager.UpdateAsync(applicationUser);
            return Ok();
        }

        private bool ApplicationUserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //LogIn
        [HttpPost("LogIn")]
        public bool LogIn(string userName, string password)
        {
            
            ApplicationUser user =  _signInManager.UserManager.FindByNameAsync(userName).Result;
            if (user == null)
            {
                return false; //false
            }
            Microsoft.AspNetCore.Identity.SignInResult signInResult =
                _signInManager.PasswordSignInAsync(user, password, false, false).Result;
            
            return signInResult.Succeeded;
        }

        [Authorize(Roles = "Administrator")]
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
        [Authorize]
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
        //Admin (her kullanıcıya atama yapalilmeli), Company admin(ResAdmin'e atama yapabilmeli)***
        [HttpPost("AssignRole")]
        public void AssignRole(string userId , string roleId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            IdentityRole applicationRole = _roleManager.FindByIdAsync(roleId).Result;
            _signInManager.UserManager.AddToRoleAsync(applicationUser, applicationRole.Name);
        }
    }
}

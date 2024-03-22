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

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, CompanyAdministrator")]
        public ActionResult<string> PutApplicationUser(ApplicationUser applicationUser)
        {
            bool isAdmin = User.IsInRole("Administrator");
            ApplicationUser existingApplicationUser = _signInManager.UserManager.FindByIdAsync(applicationUser.Id).Result;
            if (isAdmin == true)
            {
                existingApplicationUser.Email = applicationUser.Email;
                existingApplicationUser.Name = applicationUser.Name;
                existingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
                existingApplicationUser.StateId = applicationUser.StateId;
                existingApplicationUser.UserName = applicationUser.UserName;
                _signInManager.UserManager.UpdateAsync(existingApplicationUser);
                return Ok("Successfull");
            }
            ApplicationUser currentUser = _signInManager.UserManager.GetUserAsync(User).Result;
            if (User.HasClaim("CompanyId", currentUser.CompanyId.ToString()) == false && currentUser.CompanyId != existingApplicationUser.CompanyId)
            {
                return Unauthorized();
            }
            existingApplicationUser.Email = applicationUser.Email;
            existingApplicationUser.Name = applicationUser.Name;
            existingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            existingApplicationUser.StateId = applicationUser.StateId;
            existingApplicationUser.UserName = applicationUser.UserName;
            _signInManager.UserManager.UpdateAsync(existingApplicationUser);
            return Ok("Successfull");
        }

        [Authorize(Roles = "CompanyAdministrator")]
        [HttpPost]
        public string PostApplicationUser(ApplicationUser applicationUser, string passWord)
        {
            _signInManager.UserManager.CreateAsync(applicationUser, passWord).Wait();
            return applicationUser.Id;
        }

        [Authorize(Roles = "Administrator,CompanyAdministrator")]
        [HttpDelete("{id}")]
        public ActionResult<string> DeleteApplicationUser(string id)
        {
            bool isAdmin = false;
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;

            if (applicationUser == null)
            {
                return NotFound();
            }
            isAdmin = User.IsInRole("Administrator");
            if(isAdmin == true)
            {
                applicationUser.StateId = 0;
                _signInManager.UserManager.UpdateAsync(applicationUser);
                return Ok("User Created");
            }
            ApplicationUser currentUser = _signInManager.UserManager.GetUserAsync(User).Result;
            if (User.HasClaim("CompanyId", currentUser.CompanyId.ToString()) == false || currentUser.CompanyId != applicationUser.CompanyId)
            {
                return Unauthorized();
            }
            applicationUser.StateId = 0;
            _signInManager.UserManager.UpdateAsync(applicationUser);
            return Ok("User Created");

        }

        private bool ApplicationUserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //LogIn
        [HttpPost("LogIn")]
        public ActionResult<string> LogIn(string userName, string password)
        {
            
            ApplicationUser user =  _signInManager.UserManager.FindByNameAsync(userName).Result;
            if (user == null || user.StateId != 1)
            {
                return Problem(); //Kullanıcı 
            }
            Microsoft.AspNetCore.Identity.SignInResult signInResult = _signInManager.PasswordSignInAsync(user, password, false, false).Result;
            bool sonuc = signInResult.Succeeded;
            if(sonuc != true)
            {
                return Problem("Invalid UserName or Password");
            }
            Activate(user);
            return Ok("Successfull");
        }

        public bool Activate(ApplicationUser user)
        {
            try
            {
                user.StateId = 1;
                _signInManager.UserManager.UpdateAsync(user);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("ForgetPassword")]
        public ActionResult<string> ForgetPassword(string userName, string NewPassword)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            if(applicationUser == null)
            {
                return Problem();//Kullanıcı Yok
            }
            try
            {
                _signInManager.UserManager.RemovePasswordAsync(applicationUser).Wait();
                _signInManager.UserManager.AddPasswordAsync(applicationUser, NewPassword).Wait();
            }
            catch (Exception)
            {
                return Ok("An Error Accured");
            }
            return Ok("Password Assigned Successfully");
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
        
        [HttpPut("ActivateUser")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<string> ActivateUser(string userId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            if(applicationUser == null)
            {
                return Problem("User Not Found");
            }

            _signInManager.UserManager.UpdateAsync(applicationUser);
            return Ok("User Set to Active");
        }

        [HttpPut("PassifyUser")]
        [Authorize]
        public ActionResult<string> PassifyUser(string password)
        {
            ApplicationUser currentUser = _signInManager.UserManager.GetUserAsync(User).Result;
            bool result = _signInManager.UserManager.CheckPasswordAsync(currentUser, password).Result;
            if (result != true)
            {
                return Unauthorized();
            }
            currentUser.StateId = 2;
            _signInManager.UserManager.UpdateAsync(currentUser);
            return Ok("User Set to Passive");
        }

        [HttpGet("LogOut")]
        [Authorize]
        public ActionResult<string> LogOut()
        {
            try
            {
                _signInManager.SignOutAsync().Wait();
            }
            catch (Exception)
            {
                return Problem();
            }
            return Ok("Logged Out");
        }
    }
}

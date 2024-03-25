using System;
using System.Security.Claims;
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
    public class RolesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public RolesController( RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationContext context)
        {
            _context = context;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // GET: api/Roles
        [Authorize(Roles = "Administrator")]
        [HttpGet("GetApplicationRoles")]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> GetApplicationRoles()
        {
            if (_roleManager == null)
            {
                return NotFound();
            }
            return await _roleManager.Roles.ToListAsync();
        }

        [Authorize]
        [HttpGet("UserRoles")]
        public IList<string> GetUserRoles()
        {
            ApplicationUser appUser = _signInManager.UserManager.GetUserAsync(User).Result;
            IList<string> Roles = _signInManager.UserManager.GetRolesAsync(appUser).Result;
            return Roles;
        }

        //// POST: api/Roles
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize(Roles = "Administrator")]
        //[HttpPost("PostApplicationRole")]
        //public void PostApplicationRole(string name)
        //{
        //    IdentityRole applicationRole = new IdentityRole(name);
        //    _roleManager.CreateAsync(applicationRole).Wait();
        //}

        [HttpPost("AssignRestaurantAdminRole")]
        [Authorize(Roles = "Administrator,CompanyAdministrator")]
        public ActionResult<string> AssignRestaurantAdminRole(string userId, int Companyid)
        {
            ApplicationUser currentUser = _signInManager.UserManager.GetUserAsync(User).Result;
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            if (applicationUser == null)
            {
                return Problem("User Not Found");
            }
            if (User.IsInRole("Administrator") == false)
            {
                if (User.HasClaim("CompanyId", Companyid.ToString()) == false && currentUser.CompanyId != applicationUser.CompanyId)
                {
                    return Unauthorized();
                }
            }
            try
            {
                _signInManager.UserManager.AddToRoleAsync(applicationUser, "RestaurantAdministrator");
            }
            catch (Exception)
            {
                return Ok("An Error Accured");
            }
            return Ok("RestaurantAdmin Role Assigned to User");
        }

        [HttpPost("AssignCompanyAdminRole")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<string> AssignCompanyAdminRole(string userId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            if (applicationUser == null)
            {
                return Problem("User Not Found");
            }
            try
            {
                _signInManager.UserManager.AddToRoleAsync(applicationUser, "CompanyAdministrator");
            }
            catch (Exception)
            {
                return Ok("AN ERROR ACCURED");
            }
            return Ok("CompanyAdmin Role Assigned to User Successfully.");
        }

        [HttpPost("AssignCompanyClaim")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<string> AssignCompanyClaim( string userId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            if (applicationUser == null)
            {
                return Problem("User Not Found");
            }
            Claim Compclaim = new Claim("CompanyId", applicationUser.CompanyId.ToString());
            _signInManager.UserManager.AddClaimAsync(applicationUser, Compclaim).Wait();
            return Ok("CompClaim Assign to User");
        }

        [HttpPost("AssignRestaurantClaim")]
        [Authorize(Roles = "Administrator, CompanyAdministrator")]
        public ActionResult<string> AssignRestaurantClaim(string userId, int restaurantId)
        {   
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            Restaurant? restaurant = _context.Restaurants!.Where(r => r.Id == restaurantId).FirstOrDefault();
            if(User.IsInRole("Administrator") == false)
            {
                if(User.HasClaim("CompanyId", restaurant!.CompanyId.ToString()) == false)
                {
                    return Unauthorized();
                }
            }
            if (applicationUser == null || restaurant == null)
            {
                return Problem();
            }
            if (restaurant.CompanyId != applicationUser.CompanyId)
            {
                return Unauthorized(); 
            }
            Claim Restclaim = new Claim("RestaurantId", restaurantId.ToString());
            _signInManager.UserManager.AddClaimAsync(applicationUser, Restclaim).Wait();
            return Ok("RestClaim Assign to User");
        }

        [HttpDelete("DeleteRole")]
        [Authorize(Roles = "Administrator, CompanyAdministrator")]
        public ActionResult<string> DeleteRole(string RoleId, string userId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            if (applicationUser == null)
            {
                return Problem("User Not Found");
            }
            IdentityRole? role = _roleManager.FindByIdAsync(RoleId).Result;
            if (_roleManager.RoleExistsAsync(role.Name).Result == false)
            {
                return Problem("Role Not Found");
            }
            if (User.IsInRole("Administrator") == false)
            {
                if (User.HasClaim("CompanyId", applicationUser.CompanyId.ToString()) == false || role.Name.Equals("RestaurantAdministrator") == false )
                {
                    return Unauthorized();
                }
            }
            try
            {
                _signInManager.UserManager.RemoveFromRoleAsync(applicationUser, role.Name).Wait();
            }
            catch (Exception)
            {
                return NoContent();
            }
            return Ok("Role Deleted From User");
        }

        [HttpDelete("DeleteClaim")]
        [Authorize(Roles = "Administrator, CompanyAdministrator")]
        public ActionResult<string> DeleteClaim(string ClaimType, string UserId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(UserId).Result;
            if (applicationUser == null)
            {
                return Problem("User Not Found");
            }
            Claim? claim = _signInManager.UserManager.GetClaimsAsync(applicationUser).Result.FirstOrDefault();
           
            if(claim == null)
            {
                return NotFound();
            }
            _signInManager.UserManager.RemoveClaimAsync(applicationUser, claim).Wait();
            return Ok("Claim Deleted");
        }
    }
}

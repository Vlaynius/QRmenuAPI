using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class RolesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(ApplicationContext context, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _context = context;
        }

        //// GET: api/Roles
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ApplicationRole>>> GetApplicationRole()
        //{
        //  if (_context.ApplicationRole == null)
        //  {
        //      return NotFound();
        //  }
        //    return await _context.ApplicationRole.ToListAsync();
        //}

        //// GET: api/Roles/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<ApplicationRole>> GetApplicationRole(string id)
        //{
        //  if (_context.ApplicationRole == null)
        //  {
        //      return NotFound();
        //  }
        //    var applicationRole = await _context.ApplicationRole.FindAsync(id);

        //    if (applicationRole == null)
        //    {
        //        return NotFound();
        //    }

        //    return applicationRole;
        //}

        //// PUT: api/Roles/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutApplicationRole(string id, ApplicationRole applicationRole)
        //{
        //    if (id != applicationRole.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(applicationRole).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ApplicationRoleExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754


        //[HttpPost]
        //public  Task<ActionResult<ApplicationRole>> PostApplicationRole(string name)
        //{
            
        //}

        //// DELETE: api/Roles/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteApplicationRole(string id)
        //{
        //    if (_context.ApplicationRole == null)
        //    {
        //        return NotFound();
        //    }
        //    var applicationRole = await _context.ApplicationRole.FindAsync(id);
        //    if (applicationRole == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.ApplicationRole.Remove(applicationRole);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool ApplicationRoleExists(string id)
        //{
        //    return (_context.ApplicationRole?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using QRmenuAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace QRmenuAPI.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{

		}
		public DbSet<Company>? Companies { get; set; }
		public DbSet<State>? States { get; set; }
		public DbSet<Restaurant>? Restaurants { get; set; }
		public DbSet<RestaurantUser>? RestaurantUsers { get; set; }
		public DbSet<Category>? Categories { get; set; }
        public DbSet<Food>? Foods { get; set; }



        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
			modelbuilder.Entity<ApplicationUser>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
			modelbuilder.Entity<Restaurant>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<Category>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<Food>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
			modelbuilder.Entity<RestaurantUser>().HasOne(r => r.Restaurant).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<RestaurantUser>().HasOne(r => r.ApplicationUser).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<RestaurantUser>().HasKey(ru => new { ru.UserApplicationUserId, ru.RestaurantId });
			modelbuilder.Entity<State>().HasData(
				new State { Id = 0, Name = "Deleted"},
                new State { Id = 1, Name = "Active" },
                new State { Id = 2, Name = "Passive" });
			base.OnModelCreating(modelbuilder);
        }


    }
}


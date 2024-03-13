using Microsoft.EntityFrameworkCore;
using QRmenuAPI.Models;
using QRmenuAPI.Data;
using Microsoft.AspNetCore.Identity;
namespace QRmenuAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders() ;

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        //Preset Inputs
        {
            ApplicationContext? Dbcontext = app.Services.CreateScope().ServiceProvider.GetService<ApplicationContext>();
            if (Dbcontext != null)
            {
                Dbcontext.Database.Migrate();
                State state;
                if(Dbcontext.States.Count() == 0)
                {
                    state = new State();
                    state.Id = 0;
                    state.Name = "Deleted";
                    Dbcontext.States.Add(state);

                    state = new State();
                    state.Id = 1;
                    state.Name = "Active";
                    Dbcontext.States.Add(state);

                    state = new State();
                    state.Id = 2;
                    state.Name = "Passive";
                    Dbcontext.States.Add(state);
                }
                if(Dbcontext.Companies.Count() == 0)
                {
                    Company company = new Company();
                    company.Id = 1;
                    company.Name = "AdminCompany";
                    company.AddressDetail = "Admin";
                    company.Email = "admin@admin";
                    company.Phone = "1234567890";
                    company.PostalCode = "00000";
                    company.RegisterDate = DateTime.Now;
                    company.StateId = 1;
                }
                Dbcontext.SaveChanges();
                RoleManager<IdentityRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<IdentityRole>>();
                if(roleManager != null)
                {
                    IdentityRole identityRole;
                    if (roleManager.Roles.Count() == 0)
                    {
                        identityRole = new IdentityRole("Administrator");
                        roleManager.CreateAsync(identityRole).Wait();

                        identityRole = new IdentityRole("CompanyAdministrator");
                        roleManager.CreateAsync(identityRole).Wait();

                    }
                }
                UserManager<ApplicationUser>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();
                if (userManager != null)
                {
                    ApplicationUser applicationUser;
                    if(userManager.Users.Count() == 0)
                    {
                        applicationUser = new ApplicationUser();
                        applicationUser.UserName = "Administrator";
                        applicationUser.CompanyId = 1;
                        applicationUser.Email = "admin@admin";
                        applicationUser.PhoneNumber = "1234567890";
                        applicationUser.RegisterDate = DateTime.Now;
                        applicationUser.StateId = 1;
                        userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                        userManager.AddToRoleAsync(applicationUser, "Administrator").Wait();
                    }
                }
            }
        }
        app.Run();
    }
}


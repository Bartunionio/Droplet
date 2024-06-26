using Droplet.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        //builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlServer(connectionString));
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
              options.UseMySql("Server=127.0.0.1;Database=Droplet;User=droplet_admin;Password=kropelka;", new MySqlServerVersion(new Version(8, 4, 0))));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();


        // Seed application user roles
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "Manager", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed admin user
        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string adminEmail = "admin@droplet.com";
            string adminPasswd = "zaq1@WSX";
            string adminLogin = "admin";

            if(await userManager.FindByLoginAsync(adminLogin, "Droplet") == null)
            {
                var user = new IdentityUser();
                user.Email = adminEmail;
                user.UserName = adminLogin;
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, adminPasswd);

                await userManager.AddToRoleAsync(user, "Admin");
            }

            string managerEmail = "manager@droplet.com";
            string managerPasswd = "zaq1@WSX";
            string managerLogin = "manager";

            if (await userManager.FindByLoginAsync(managerLogin, "Droplet") == null)
            {
                var user = new IdentityUser();
                user.Email = managerEmail;
                user.UserName = managerLogin;
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, managerPasswd);

                await userManager.AddToRoleAsync(user, "Manager");
            }

            string userEmail = "admin@droplet.com";
            string userPasswd = "zaq1@WSX";
            string userLogin = "admin";

            if (await userManager.FindByLoginAsync(userLogin, "Droplet") == null)
            {
                var user = new IdentityUser();
                user.Email = userEmail;
                user.UserName = userLogin;
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, userPasswd);

                await userManager.AddToRoleAsync(user, "User");
            }
        }

        app.Run();
    }
}

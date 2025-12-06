using Microsoft.AspNetCore.Identity;
using Assignment1TicketingSystem.Models;

public static class IdentitySeed
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleMgr = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = new[] {"Admin", "Organizer", "Attendee"};
        foreach (var r in roles)
            if (!await roleMgr.RoleExistsAsync(r))
                await roleMgr.CreateAsync(new IdentityRole(r));

        // create admin if not exists
        var adminEmail = "admin@local.test";
        var adminUser = await userMgr.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, FullName = "Admin" };
            await userMgr.CreateAsync(adminUser, "P@ssword123!");
            await userMgr.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

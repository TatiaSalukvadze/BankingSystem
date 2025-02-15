using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure.Identity
{
    public class DataSeeder
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task SeedDataAsync()
        {
            await SeedInitialDataAsync();
        }
        private async Task SeedInitialDataAsync()
        {
            var roles = new List<string>() { "Manager", "Operator", "User" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            await AddEmployeeAsync("Manager@gmail.com", roles[0], "Manager1*");
            await AddEmployeeAsync("Operator@gmail.com", roles[1], "Operator1*");

        }

        private async Task AddEmployeeAsync(string email, string role, string password)
        {
            if (await _userManager.FindByEmailAsync(email) is null)
            {
                var employee = new IdentityUser()
                {
                    UserName = email,
                    Email = email
                };
                var employeeAdded = await _userManager.CreateAsync(employee, password);
                if (employeeAdded.Succeeded)
                {
                    await _userManager.AddToRoleAsync(employee, role);
                }
            }

        }
    }
}

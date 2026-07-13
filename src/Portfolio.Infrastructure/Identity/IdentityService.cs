using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Portfolio.Application.Interfaces.Identity;

namespace Portfolio.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;

    public IdentityService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string?> GetUserEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.Email;
    }

    public async Task<List<string>> GetAdminsEmailsAsync()
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        return admins.Select(a => a.Email).Where(e => !string.IsNullOrEmpty(e)).Cast<string>().ToList();
    }
}

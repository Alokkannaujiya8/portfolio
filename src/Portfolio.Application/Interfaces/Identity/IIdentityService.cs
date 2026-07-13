using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portfolio.Application.Interfaces.Identity;

public interface IIdentityService
{
    Task<string?> GetUserEmailAsync(string userId);
    Task<List<string>> GetAdminsEmailsAsync();
}

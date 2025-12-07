using Microsoft.AspNetCore.Identity;

namespace Bookify.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
   

}
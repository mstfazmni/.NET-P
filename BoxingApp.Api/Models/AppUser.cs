using Microsoft.AspNetCore.Identity;

namespace BoxingApp.Api.Models
{
    public class AppUser : IdentityUser
    {
    }
}


// Why inherit from IdentityUser instead of writing your own from scratch:
// IdentityUser already includes Id, UserName, Email, and a securely-hashed PasswordHash field 
// — you're not reinventing password storage,
//  which is exactly the kind of thing you don't want to get wrong yourself.
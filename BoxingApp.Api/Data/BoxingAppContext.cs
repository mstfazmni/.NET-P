using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BoxingApp.Api.Models;

namespace BoxingApp.Api.Data
{
    public class BoxingAppContext : IdentityDbContext<AppUser>
    {
        public BoxingAppContext(DbContextOptions<BoxingAppContext> options) : base(options) { }

        public DbSet<Athlete> Athletes { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
    }
}

// Why IdentityDbContext<AppUser> instead of
//  plain DbContext: this automatically adds all the tables Identity needs (users, roles, etc.) 
//  to your migrations — you don't manually write CREATE TABLE Users.
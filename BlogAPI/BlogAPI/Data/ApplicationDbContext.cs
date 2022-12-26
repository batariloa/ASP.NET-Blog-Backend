
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;



    public class ApplicationDbContext : IdentityDbContext
    {
        

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options){


        }
        public DbSet<Post> Posts {get; set;}
        public DbSet<ApplicationUser> Users {get; set;}

  

    }

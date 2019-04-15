using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Models
{
    public class Assignment2DataContext : DbContext
    {
        public Assignment2DataContext(DbContextOptions<Assignment2DataContext> options)
            : base(options)
        { }
        /*Creating DBSet<T>’ collection for each of the tables in your model*/
      
            /*for the BlogPost*/
        public DbSet<BlogPost> BlogPosts { get; set; }

        /*for the Comment*/
        public DbSet<Comment> Comments { get; set; }

        /*for the Role*/
        public DbSet<Role> Roles { get; set; }

        /*for the User*/
        public DbSet<User> Users { get; set; }
        /*for the Photos*/
        public DbSet<Photo> Photos { get; set; }
        /*for the BadWords*/
        public DbSet<BadWord> BadWords { get; set; }
    }
}

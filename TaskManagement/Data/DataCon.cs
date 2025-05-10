using Microsoft.EntityFrameworkCore;
using System;
using TaskManagement.Model;
using TaskManagement.Model.Common;

namespace TaskManagement.Data
{
    public class DataCon : DbContext
    {
        public DataCon(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Task_User> Task_User { get; set; }
        public DbSet<Task_Master> Task_Master { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Task_User>().HasData(
                new Task_User { User_ID = 1,User_Name = "Admin",Password = "admin",User_Role = Convert.ToInt32(UserRole.Admin)},
                new Task_User { User_ID =2, User_Name = "User", Password = "user", User_Role = Convert.ToInt32(UserRole.User) }
                );
        }
    }
}

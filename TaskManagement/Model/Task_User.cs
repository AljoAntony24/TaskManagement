using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Model
{
    public class Task_User
    {
        [Key]
        public long User_ID { get; set; }
        public string User_Name { get; set; }
        public string Password { get; set; }
        public int User_Role { get; set; }
        public string? Token { get; set; }
        public DateTime? EnteredAt { get; set; }
        public string Phone { get; set; }
    }
}

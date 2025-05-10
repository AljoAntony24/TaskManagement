using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Model
{
    public class Task_Master
    {
        [Key]
        public long TaskID { get; set; }
        public string TaskName { get; set; }
        public string TaskDetails { get; set; }
        public long? AssignedTo { get; set; }
        public int? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public long EnteredBy { get; set; }
        public DateTime EnteredAt { get; set; }
    }
}

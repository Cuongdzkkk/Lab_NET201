using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab5.Models
{
    public class StudentDetails
    {
        public int StudentId { get; set; }
        public string Email { get; set; }
        public DateTime DateBirth { get; set; }

        // Navigation Property
        [ValidateNever]
        public Student Student { get; set; }
    }
}

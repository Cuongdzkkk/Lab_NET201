using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab5.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Navigation Properties
        [ValidateNever]
        public StudentDetails StudentDetails { get; set; }
        
        [ValidateNever]
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
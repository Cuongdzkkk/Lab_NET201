using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab5.Models
{
    public class StudentCourse
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        [ValidateNever]
        public Student Student { get; set; }
        
        [ValidateNever]
        public Course Course { get; set; }
    }
}

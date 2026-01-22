using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab5.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }

        [ValidateNever]
        public ICollection<StudentCourse> StudentCourses { get; set; }
        
        [ValidateNever]
        public CourseDetails CourseDetails { get; set; }
    }
}

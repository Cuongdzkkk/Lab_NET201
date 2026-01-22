using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab5.Models
{
    public class CourseDetails
    {
        [Key]
        public int CourseDetailsId { get; set; }
        
        public int CourseId { get; set; } // FK to Course (1-1 relationship)
        
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        
        [StringLength(2000)]
        public string Syllabus { get; set; }
        
        [StringLength(500)]
        public string Prerequisites { get; set; }
        
        [Range(1, 10)]
        public int Credits { get; set; }
        
        [Range(1, 500)]
        public int DurationHours { get; set; }
        
        [StringLength(200)]
        public string Instructor { get; set; }
        
        [StringLength(300)]
        public string Location { get; set; }
        
        // Navigation Property
        [ValidateNever]
        public Course Course { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab5.Models
{
    [Table("ThanNhans")] // Ép cứng tên bảng đúng
    public class ThanNhan
    {
        // QUAN TRỌNG: Đã xóa cột Id
        
        public int MaNV { get; set; } // Part of PK, also FK
        public string TenTN { get; set; } // Part of PK

        public string MoiQuanHe { get; set; }
        public string Phai { get; set; }
        public DateTime NgSinh { get; set; }
        
        // Navigation Property
        [ForeignKey("MaNV")]
        [ValidateNever]
        public virtual NhanVien NhanVien { get; set; }
    }
}
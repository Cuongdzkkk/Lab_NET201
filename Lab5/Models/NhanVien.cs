using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab5.Models
{
    [Table("NHANVIEN")]
    public class NhanVien
    {
        [Key]
        [Column("MANV")]
        public int MaNV { get; set; }

        public string TenLot { get; set; }
        public string HoNV { get; set; }
        public string TenNV { get; set; }
        public string DChi { get; set; }
        public string Phai { get; set; }
        public string Ma_NQL { get; set; }
        public decimal Luong { get; set; }
        public DateTime NgaySinh { get; set; }
        public int PHG { get; set; }

        [ValidateNever]
        public ICollection<ThanNhan> ThanNhans { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;

namespace ServiceLayer.DTO
{
    public class BookEditDto
    {
        [Required]
        public int Id { get; set; }   // شناسه کتاب برای ویرایش

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        [Required]
        [MaxLength(13)]
        public string ISBN { get; set; }

        [MaxLength(50)]
        public string? Barcode { get; set; }

        public BookStatus BookStatus { get; set; }

        [Required]
        public int CategoryId { get; set; }

    }
}

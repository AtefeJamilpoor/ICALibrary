using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;

namespace DimainLayer.Model
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author {  get; set; }

        //شماره ISBN یکتا
        [Required]
        [MaxLength(13)]
        public string ISBN {  get; set; }

        [MaxLength(50)]
        public string Barcode {  get; set; }

        [Required]
        public BookStatus BookStatus { get; set; } = BookStatus.Available;

        //کلید خارجی به Category
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        //ارتباط یک به چند با Reservation
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}

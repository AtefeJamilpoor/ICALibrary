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
    public  class MembershipCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string CardNumber {  get; set; }

        //تاریخ صدور کارت
        [Required]
        public DateTime IssueDate { get; set; } 

        public DateTime ? ExpiryDate { get; set; }

        public string PhotoUrl {  get; set; }

        [Required]
        public CardStatus Status { get; set; } = CardStatus.Active;

        //کلید خارجی User
        [ForeignKey("User")]
        public int UserId { get; set; }

        //Navigation Property
        public User User { get; set; }
    }
}

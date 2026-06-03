using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTO
{
    public class CreateUserDTOLayer
    {
        public string FullName { get; set; }
        public string NationalCode { get; set; }
        public string PhoneNumber { get; set; }
        // فقط مسیر فایل
        public string ImagePath { get; set; } 
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
    }
}

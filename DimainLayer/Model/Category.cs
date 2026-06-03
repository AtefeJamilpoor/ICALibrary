using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimainLayer.Model
{
    public  class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        //توضیحات دسته بندی - اختیاری
        [MaxLength(250)]
        public string ? Description { get; set; }

        //ارتباط یک به چند با کتاب
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}

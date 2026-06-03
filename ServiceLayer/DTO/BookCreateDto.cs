using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;
using DimainLayer.Model;

namespace ServiceLayer.DTO
{
    public class BookCreateDto
    {
            [Required(ErrorMessage ="عنوان کتاب را وارد نکردید")]
            [MaxLength(200,ErrorMessage ="عنوان کتاب بیش از حد طولانی است.")]
            public string Title { get; set; }

            [Required(ErrorMessage ="نویسنده را وارد کنید")]
            [MaxLength(100)]
            public string Author { get; set; }

            // دسته انتخابی کاربر
            [Required(ErrorMessage ="دسته بندی کتاب باید وارد شود. اگر دسته بندی کتاب در موارد وجود ندارد به بخش دسته بندی ها بروید و آن را اضافه کنید.")]
            public int? CategoryId { get; set; }

        // لیست دسته‌ها برای نمایش در DropDown
        
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        public string? CategoryName { get; set; }
    }

        public class CategoryDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    
}

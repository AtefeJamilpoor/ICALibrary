using DimainLayer.Model;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.BusinesLayer;

namespace ICALibrary.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryservice;
        public CategoryController(ICategoryService category)
        {
            _categoryservice = category;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryservice.GetAll();
            return View(categories); // مدل لیست دسته‌ها پاس داده می‌شود
        }

        // Partial: لیست دسته‌ها
        public async Task<IActionResult> GetList()
        {
            var categories = await _categoryservice.GetAll();
            return PartialView("_CategoryList", categories);
        }

        // GET: فرم ایجاد یا ویرایش
        public async Task<IActionResult> GetForm(int? id)
        {
            if (id == null)
                return  PartialView("Partial/_CategoryForm", new Category());

                var category = await _categoryservice.GetById(id.Value);
            if (category == null)
                return NotFound();
            return PartialView("Partial/_CategoryForm", category);
        }
   
        // POST: ذخیره دسته (ایجاد یا ویرایش)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    await _categoryservice.Create(category);
                }
                else
                {
                    await _categoryservice.Update(category);
                }

                // بعد از ذخیره دوباره به Index برگرد
                return RedirectToAction(nameof(Index));
            }

            // اگر خطا بود، دوباره فرم را با داده‌ها نمایش بده
            var categories = await _categoryservice.GetAll();
            ViewBag.Error = "اطلاعات وارد شده معتبر نیست.";
            return View("Index", categories);

        }

    }
}

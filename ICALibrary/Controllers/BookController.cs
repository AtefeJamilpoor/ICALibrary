using DimainLayer.Model;
using ICALibrary.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.BusinesLayer;
using ServiceLayer.DTO;
using Stimulsoft.Base;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace ICALibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;


        public BookController(IBookService bookService, ICategoryService categoryService)
        {
            StiLicense.LoadFromString("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHl2AD0gPVknKsaW0un+3PuM6TTcPMUAWEURKXNso0e5OJN40hxJjK5JbrxU+NrJ3E0OUAve6MDSIxK3504G4vSTqZezogz9ehm+xS8zUyh3tFhCWSvIoPFEEuqZTyO744uk+ezyGDj7C5jJQQjndNuSYeM+UdsAZVREEuyNFHLm7gD9OuR2dWjf8ldIO6Goh3h52+uMZxbUNal/0uomgpx5NklQZwVfjTBOg0xKBLJqZTDKbdtUrnFeTZLQXPhrQA5D+hCvqsj+DE0n6uAvCB2kNOvqlDealr9mE3y978bJuoq1l4UNE3EzDk+UqlPo8KwL1XM+o1oxqZAZWsRmNv4Rr2EXqg/RNUQId47/4JO0ymIF5V4UMeQcPXs9DicCBJO2qz1Y+MIpmMDbSETtJWksDF5ns6+B0R7BsNPX+rw8nvVtKI1OTJ2GmcYBeRkIyCB7f8VefTSOkq5ZeZkI8loPcLsR4fC4TXjJu2loGgy4avJVXk32bt4FFp9ikWocI9OQ7CakMKyAF6Zx7dJF1nZw");

            _bookService = bookService;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            var model = await showing();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateBook()
        {
            var categories =await  _categoryService.GetAll();
            var dto = new BookCreateDto
            {
                Categories = categories
           .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
           .ToList()
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(BookCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                // اگر اعتبارسنجی شکست خورد، دوباره لیست دسته‌ها رو پاس بده
                var categories = await _categoryService.GetAll();
                model.Categories = categories
                    .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                    .ToList();
                return View(model);
            }
            await _bookService.Create(model);
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(int id)
        {
            Book book = await _bookService.GetById(id);
            HttpContext.Session.SetInt32("bookId", id);
            var categories = await _categoryService.GetAll();
            var model = new BookCreateDto
            {
                Title = book.Title,
                Author= book.Author,
                CategoryId = book.CategoryId,
                Categories = categories
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                .ToList(),
                CategoryName= categories.SingleOrDefault(c=>c.Id==book.CategoryId)?.Name,
            }; 
            return View (model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook(BookCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int Id = HttpContext.Session.GetInt32("bookId").Value;
           
            Book book = await _bookService.GetById(Id);

            if (book == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.Title))
                book.Title = model.Title;

            if (!string.IsNullOrEmpty(model.Author))
                book.Author = model.Author;

            if (model.CategoryId!=null)
                book.CategoryId = model.CategoryId.Value;
            await _bookService.Update(book);
            return RedirectToAction("Index");
        }

           
        
        //برای لیست کتاب ها
        async Task<List<BookIndexViewModel>> showing()
        {
            var books = await _bookService.GetAll();
            var model = new List<BookIndexViewModel>();

            foreach (var b in books)
            {
                var categoryName = await _categoryService.GetCategoryName(b.CategoryId);
                model.Add(new BookIndexViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    CategoryName = categoryName,
                    ISBN = b.ISBN ?? "ندارد",
                    BookStatus = b.BookStatus.ToString()
                });
            }

            return model;
        }     

        public ActionResult PrintReportAllBooks()
        {
            return View("PrintReportAllBooks");
        }
        public async Task<IActionResult> ReportAllBooks()
        {
            StiReport report = new StiReport();
            report.Load(StiNetCoreHelper.MapPath(this, "wwwroot/Report/BooksListReport.mrt"));
            var model = await showing();
            report.RegData("dt", model);
            return StiNetCoreViewer.GetReportResult(this,report);
        }
        public ActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }
        public async Task<IActionResult> ReportByCategories()
        {
            var model = await _bookService.GetCategoryPivotReport();
            return View(model);
        }
        //گزارشگیری برای لیست دسته بندی ها 
        public async Task<IActionResult> PrintReportingByCategories()
        {
            StiReport report = new StiReport();
            report.Load(StiNetCoreHelper.MapPath(this, "wwwroot/Report/ReportingByCategories.mrt"));
            var model = await _bookService.GetCategoryPivotReport();
            report.RegData("dt", model);
            return StiNetCoreViewer.GetReportResult(this, report);
        }
        public ActionResult ShowingPrintReportingByCategories()
        {
            return View("PrintReportingByCategories");
        }
        public async Task<ActionResult> BookDetails(int id)
        {
            Book? model = await _bookService.GetById(id);
            return View(model);
        }
    }
}

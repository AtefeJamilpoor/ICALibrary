using DimainLayer.Enums;
using DimainLayer.Model;
using ICALibrary.Models.DTO;
using ICALibrary.Models.Enum;
using ICALibrary.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.BusinesLayer;
using ServiceLayer.DTO;
using Stimulsoft.Base;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace ICALibrary.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMemberShipCardService _membershipCard;
        private readonly IWebHostEnvironment _env;
        public UserController(IUserService userService, IWebHostEnvironment env,IMemberShipCardService memberShipCardService )
        {
            StiLicense.LoadFromString("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHl2AD0gPVknKsaW0un+3PuM6TTcPMUAWEURKXNso0e5OJN40hxJjK5JbrxU+NrJ3E0OUAve6MDSIxK3504G4vSTqZezogz9ehm+xS8zUyh3tFhCWSvIoPFEEuqZTyO744uk+ezyGDj7C5jJQQjndNuSYeM+UdsAZVREEuyNFHLm7gD9OuR2dWjf8ldIO6Goh3h52+uMZxbUNal/0uomgpx5NklQZwVfjTBOg0xKBLJqZTDKbdtUrnFeTZLQXPhrQA5D+hCvqsj+DE0n6uAvCB2kNOvqlDealr9mE3y978bJuoq1l4UNE3EzDk+UqlPo8KwL1XM+o1oxqZAZWsRmNv4Rr2EXqg/RNUQId47/4JO0ymIF5V4UMeQcPXs9DicCBJO2qz1Y+MIpmMDbSETtJWksDF5ns6+B0R7BsNPX+rw8nvVtKI1OTJ2GmcYBeRkIyCB7f8VefTSOkq5ZeZkI8loPcLsR4fC4TXjJu2loGgy4avJVXk32bt4FFp9ikWocI9OQ7CakMKyAF6Zx7dJF1nZw");
           
            _userService = userService;
            _env = env;
            _membershipCard = memberShipCardService;
        }
        public async  Task<IActionResult> Index()
        {
            var model =await  _userService.GetAllUsers();
            return View(model);
        }
        public async Task<IActionResult> CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                bool exists = await _userService.UserExistsByNationalCodeAsync(model.NationalCode);
                if (exists)
                {
                    ModelState.AddModelError("", "کاربری با این کد ملی قبلاً ثبت شده است.");
                    return View(model);
                }

                string imagePath = null;
                if (model.Image != null && model.Image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "image");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }

                    imagePath = "/Image/" + uniqueFileName;
                }

                // ساخت DTO برای سرویس
                var serviceDto = new CreateUserDTOLayer
                {
                    FullName = model.FullName,
                    NationalCode = model.NationalCode,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    BirthDate = model.BirthDate,
                    ImagePath = imagePath,
                };

                await _userService.CreateUser(serviceDto);

                return RedirectToAction("Index");

            }
        }

        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userService.GetById(id);
            var model = new EditUserDTO
            {
                FullName = user.FullName,
                PhoneNumber=user.PhoneNumber,
                Email=user.Email,
                BirthDate=user.BirthDate,
                //Status=user.Status==UserStatus.Active? UserEditStatus.Active : UserEditStatus.Blocked,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetById(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;

            if (model.BirthDate!=DateTime.MinValue)
                user.BirthDate = model.BirthDate;

            if (model.Image != null && model.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "image");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // حذف عکس قبلی
                if (!string.IsNullOrEmpty(user.PhotoUrl))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, "image", Path.GetFileName(user.PhotoUrl));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // ذخیره عکس جدید
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                var newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                user.PhotoUrl = "/image/" + uniqueFileName;
            }

            await _userService.Update(user);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var model = new UserDetailsViewModel()
            {
                User = await _userService.GetById(id),
                MembershipCard = await _membershipCard.GetByUserId(id),
            };
            return View(model);
        }
        public ActionResult PrintUserListReport()
        {
            return View("PrintUserListReport");
        }
        public IActionResult UserListReport()
        {
            StiReport report = new StiReport();
            report.Load(StiNetCoreHelper.MapPath(this,"wwwroot/Report/UserReport.mrt"));
            var usersList =  _userService.GetUsersReport();
            report.RegData("dt",usersList);
            report.Render();
            return  StiNetCoreViewer.GetReportResult(this,report);
        }
        public ActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }
    }
}

using System.Globalization;
using System.Security.Claims;
using DimainLayer.Enums;
using DimainLayer.Model;
using ICALibrary.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.BusinesLayer;
using ServiceLayer.DTO;
using ServiceLayer.ViewModel;
using Stimulsoft.Base;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace ICALibrary.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IMemberShipCardService _cardService;
        private readonly IReservationService _reservationService;
        private readonly IBookService _bookService;
        public ReservationController(IMemberShipCardService cardService,IReservationService reservationService,IBookService bookService)
        {
                _cardService = cardService;
            _reservationService = reservationService;
            _bookService = bookService;
            StiLicense.LoadFromString("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHl2AD0gPVknKsaW0un+3PuM6TTcPMUAWEURKXNso0e5OJN40hxJjK5JbrxU+NrJ3E0OUAve6MDSIxK3504G4vSTqZezogz9ehm+xS8zUyh3tFhCWSvIoPFEEuqZTyO744uk+ezyGDj7C5jJQQjndNuSYeM+UdsAZVREEuyNFHLm7gD9OuR2dWjf8ldIO6Goh3h52+uMZxbUNal/0uomgpx5NklQZwVfjTBOg0xKBLJqZTDKbdtUrnFeTZLQXPhrQA5D+hCvqsj+DE0n6uAvCB2kNOvqlDealr9mE3y978bJuoq1l4UNE3EzDk+UqlPo8KwL1XM+o1oxqZAZWsRmNv4Rr2EXqg/RNUQId47/4JO0ymIF5V4UMeQcPXs9DicCBJO2qz1Y+MIpmMDbSETtJWksDF5ns6+B0R7BsNPX+rw8nvVtKI1OTJ2GmcYBeRkIyCB7f8VefTSOkq5ZeZkI8loPcLsR4fC4TXjJu2loGgy4avJVXk32bt4FFp9ikWocI9OQ7CakMKyAF6Zx7dJF1nZw");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ReservationView()
        {
            return View();
        }

        public async Task<IActionResult> SearchByIsbn(string isbn)
        {
            BookInfoPartial bookInfo = await _reservationService.GetByISBN(isbn);
            if (bookInfo == null)
            {
                ModelState.AddModelError("", "isbn ناموجود");
            }
            int id = HttpContext.Session.GetInt32("userId").Value;
            var rInfo = await _reservationService.GetReservationInfo(id);
            UserReservationsViewModel model = new UserReservationsViewModel
            {
                hasAvalableCard = rInfo.hasAvalableCard,
                bookInfo = bookInfo,
                BorrowedBooks = rInfo.Borrowed,
                OverdueBooks = rInfo.Overdue,
                ReservedBooks = rInfo.Reserved,
            };
            return View("ReservationInfo", model);
        }

        [HttpPost]
        public async Task<IActionResult> Borrow(int bookId)
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("userId");
                if (userId == null)
                {
                    return BadRequest("invalid user");
                }
                bool isBorrowed = true;
                await _reservationService.Create(bookId, userId.Value, isBorrowed);
                return Json(new { redirectUrl = Url.Action("ReservationInfo", "Reservation", new { id = userId }) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(int bookId)
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("userId");
                if (userId == null)
                    return BadRequest("UserId is null in session");

                bool isBorrowed = false;
                await _reservationService.Create(bookId, userId.Value, isBorrowed);
                return Json(new { redirectUrl = Url.Action("ReservationInfo", "Reservation", new { id = userId }) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // پیام محدودیت
            }
            catch (Exception ex)
            {
                return Content(ex.ToString(), "text/plain");
            }
            ////int userId = HttpContext.Session.GetInt32("userId").Value;
            ////bool isBorrowed = false;
            ////_reservationService.Create(bookId, userId,isBorrowed);
            ////return Ok();


            //   List<Reservation> validReservations = reservation
            //      .Where(r => r.ReservationStatus == ReservationStatus.Active
            //  && DateTime.Now <= r.DueDate.AddDays(3))
            //.ToList();
        }

        [HttpPost]
        public async Task<IActionResult> ReserveToBorrow(int bookId)
        {
            try { 
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return BadRequest("UserId is null in session");
            await _reservationService.UpdateReserveToBorrow(bookId, userId.Value);
            int Id = userId.Value;
            return Json(new { redirectUrl = Url.Action("ReservationInfo", "Reservation",  new { id = Id }) });
            }
             catch (InvalidOperationException ex)
                   {
                       return BadRequest(ex.Message); // پیام محدودیت
             }
             catch (Exception ex)
             {
                        return Content(ex.ToString(), "text/plain");
             }
        }
        public IActionResult ReturnBook(int bookId)
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("userId");
                if (userId == null)
                    return BadRequest("UserId is null in session");
                 _reservationService.BorrowToReturn(bookId, userId.Value);
                return Json(new { redirectUrl = Url.Action("ReservationInfo", "Reservation", new { id = userId }) });
            }
            catch (Exception ex)
            {
                return Content(ex.ToString(), "text/plain");
            }
        }
        public async Task<IActionResult> CancelReservation(int bookId)
        {
            try {
                int? userId = HttpContext.Session.GetInt32("userId");
                if (userId == null)
                    return BadRequest("UserId is null in session");
                await _reservationService.CancelReservation(bookId, userId.Value);
                return Json(new { redirectUrl = Url.Action("ReservationInfo", "Reservation", new { id = userId }) });
            }
            catch (Exception ex) { return Content(ex.ToString(), "text/plain"); }
        }
        public async Task<IActionResult> ReservationInfo(int Id)
        {
            HttpContext.Session.SetInt32("userId",Id);

            //MembershipCard card = await _cardService.GetByUserId(Id);
           
            var rInfo = await _reservationService.GetReservationInfo(Id);

            UserReservationsViewModel model = new UserReservationsViewModel{
                    BorrowedBooks = rInfo.Borrowed,
                    OverdueBooks = rInfo.Overdue,
                    ReservedBooks = rInfo.Reserved,
                hasAvalableCard = rInfo.hasAvalableCard,
            };

            return View(model);
        }
        public async Task<IActionResult> ShowReservationsDate()
        {
            return View("ShowReservationsDate");
        }

        [HttpPost]
        public IActionResult ShowReservationsDate(DateTime dateTime)
        {
            try
            {
                PersianCalendar pc = new PersianCalendar();
                DateTime date = pc.ToDateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
                bool AnyReserva =  _reservationService.AnyReserva(date);
                if (!AnyReserva)
                {
                    // اگر هیچ کتابی رزرو نشده
                    return NotFound("در این تاریخ هیچ رزروی یافت نشد.");
                }
                else
                {
                    //ViewBag.Date = date;
                    return Ok(new { success = true, redirectUrl = Url.Action("ReportReservationsDate",date) });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // پیام محدودیت
            }
            catch (Exception ex)
            {
                return Content(ex.ToString(), "text/plain");
            }
        }

        public async Task<IActionResult> ReservationReport(DateTime date)
        {
                List<BookReservationViewModel> model = await _reservationService.ShowReservationsDate(date);
                StiReport report = new StiReport();
                report.Load(StiNetCoreHelper.MapPath(this, "wwwroot/Report/ReservationReport.mrt"));
                report.RegData("dt", model);
                return StiNetCoreViewer.GetReportResult(this, report);
        }

        public ActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }

        public ActionResult ReportReservationsDate(DateTime date)
        {
            return View(date);
        }
    }

}

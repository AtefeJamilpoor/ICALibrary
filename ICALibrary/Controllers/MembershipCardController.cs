using DimainLayer.Model;
using ICALibrary.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.BusinesLayer;

namespace ICALibrary.Controllers
{
    public class MembershipCardController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMemberShipCardService _memberShipCardService;
        IWebHostEnvironment _env;
        public MembershipCardController(IUserService userService,IMemberShipCardService memberShipCardService, IWebHostEnvironment env)
        {
                _userService = userService;
            _memberShipCardService = memberShipCardService;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GenerateCard(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null) return NotFound();

            var card = await _memberShipCardService.GenerateMembershipCardAsync(user);

            return RedirectToAction("ShowCard", "MembershipCard", new { id = user.Id });
        }
        public async Task<IActionResult> ShowCard(int id)
        {
            var user = await _userService.GetById(id);
            var card = await _memberShipCardService.GetByUserId(id);
            var vm = new MembershipCardViewModel
            {
                UserFullName = user.FullName,
                MembershipNumber = user.MembershipNumber,
                NationalCode = user.NationalCode,
                PhotoUrl = user.PhotoUrl,
                CardNumber = card.CardNumber,
                IssueDate = card.IssueDate,
                ExpiryDate = card.ExpiryDate,
                Status = card.Status,
            };
            return View(vm);
        }
        public async Task<IActionResult> RenewCard(int id)
        {
            var user = await _userService.GetById(id);
            var card = await _memberShipCardService.RenewCard(id);
            return RedirectToAction("ShowCard", "MembershipCard", new { id = user.Id });
        }
    }
}

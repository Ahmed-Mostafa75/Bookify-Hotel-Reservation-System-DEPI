using Bookify.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BookingsController : Controller
{
    private readonly IUnitOfWork _uow;
    public BookingsController(IUnitOfWork uow) { _uow = uow; }

    public async Task<IActionResult> Index()
    {
        var bookings = await _uow.Repository<Bookify.Data.Entities.Booking>().GetAllAsync(includeProperties: "Room,User");
        return View(bookings);
    }
}
using Bookify.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Bookify.Web.Controllers;

public class RoomsController : Controller
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<IActionResult> Index(DateTime? checkIn, DateTime? checkOut, int? roomTypeId)
    {
        if (checkIn.HasValue && checkOut.HasValue)
        {
            HttpContext.Session.SetString("LastCheckIn", checkIn.Value.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("LastCheckOut", checkOut.Value.ToString("yyyy-MM-dd"));
            var rooms = await _roomService.SearchAvailableAsync(checkIn.Value, checkOut.Value, roomTypeId);
            ViewBag.RoomTypes = await _roomService.GetRoomTypesAsync();
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            return View(rooms);
        }
        ViewBag.RoomTypes = await _roomService.GetRoomTypesAsync();
        return View(Enumerable.Empty<Bookify.Data.Entities.Room>());
    }
}

using Bookify.Data.Entities;
using Bookify.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RoomsController : Controller
{
    private readonly IUnitOfWork _uow;

    public RoomsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _uow.Repository<Room>().GetAllAsync(includeProperties: "RoomType");
        return View(rooms);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.RoomTypes = await _uow.Repository<RoomType>().GetAllAsync();
        return View(new Room());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Room room)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.RoomTypes = await _uow.Repository<RoomType>().GetAllAsync();
            return View(room);
        }
        await _uow.Repository<Room>().AddAsync(room);
        await _uow.SaveChangesAsync();
        TempData["toast"] = "Room created";
        return RedirectToAction("Index");
    }
}
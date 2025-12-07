using Bookify.Data.Entities;
using Bookify.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RoomTypesController : Controller
{
    private readonly IUnitOfWork _uow;

    public RoomTypesController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IActionResult> Index()
    {
        var types = await _uow.Repository<RoomType>().GetAllAsync();
        return View(types);
    }

    public IActionResult Create()
    {
        return View(new RoomType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoomType type)
    {
        if (!ModelState.IsValid) return View(type);
        await _uow.Repository<RoomType>().AddAsync(type);
        await _uow.SaveChangesAsync();
        TempData["toast"] = "Room type created";
        return RedirectToAction("Index");
    }
}
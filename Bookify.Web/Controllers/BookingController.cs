using Bookify.Services;
using Bookify.Web.Models;
using Bookify.Data;
using Bookify.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Claims;

namespace Bookify.Web.Controllers;

public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IUnitOfWork _uow;

    public BookingController(IBookingService bookingService, IUnitOfWork uow)
    {
        _bookingService = bookingService;
        _uow = uow;
    }
	[Authorize]

    [HttpPost]
    public IActionResult AddToCart(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cartKey = $"BookingCart:{userId}";
        if (checkIn == default || checkOut == default || checkIn.Date >= checkOut.Date)
        {
            var ciStr = HttpContext.Session.GetString("LastCheckIn");
            var coStr = HttpContext.Session.GetString("LastCheckOut");
            if (DateTime.TryParse(ciStr, out var ci) && DateTime.TryParse(coStr, out var co) && ci.Date < co.Date)
            {
                checkIn = ci;
                checkOut = co;
            }
            else
            {
                checkIn = DateTime.Today;
                checkOut = DateTime.Today.AddDays(1);
            }
        }
        var item = new ReservationCartItem { RoomId = roomId, CheckIn = checkIn, CheckOut = checkOut };
        var cart = HttpContext.Session.GetString(cartKey);
        var items = string.IsNullOrEmpty(cart) ? new List<ReservationCartItem>() : JsonSerializer.Deserialize<List<ReservationCartItem>>(cart)!;
        items.Add(item);
        HttpContext.Session.SetString(cartKey, JsonSerializer.Serialize(items));
        TempData["toast"] = "Room added to reservation cart";
        return RedirectToAction("Cart");
    }
	[Authorize]

    public async Task<IActionResult> Cart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cartKey = $"BookingCart:{userId}";
        var cart = HttpContext.Session.GetString(cartKey);
        var items = string.IsNullOrEmpty(cart) ? new List<ReservationCartItem>() : JsonSerializer.Deserialize<List<ReservationCartItem>>(cart)!;
        var roomIds = items.Select(i => i.RoomId).Distinct().ToArray();
        var rooms = roomIds.Length == 0
            ? Array.Empty<Room>()
            : (await _uow.Repository<Room>().GetAllAsync(r => roomIds.Contains(r.Id), includeProperties: "RoomType")).ToArray();
        ViewBag.RoomsById = rooms.ToDictionary(r => r.Id);
        return View(items);
    }

	[Authorize]
	[HttpPost]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cartKey = $"BookingCart:{userId}";
        var cart = HttpContext.Session.GetString(cartKey);
        var cartItems = string.IsNullOrEmpty(cart)
            ? new List<ReservationCartItem>()
            : JsonSerializer.Deserialize<List<ReservationCartItem>>(cart)!;

        if (!cartItems.Any())
            return RedirectToAction("Cart");

        var checkoutUrl = await _bookingService.CreateCheckoutSessionAsync(
            userId,
            cartItems.Select(ci => (ci.RoomId, ci.CheckIn, ci.CheckOut))
        );

        return Redirect(checkoutUrl); // Redirect ??????? ??? Stripe Checkout
    }

	[Authorize]
    public IActionResult Success()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cartKey = $"BookingCart:{userId}";
        HttpContext.Session.Remove(cartKey);
        return View("History");
    }

	[Authorize]
    [HttpPost]
    public async Task<IActionResult> Confirm([FromBody] PaymentConfirmRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ok = await _bookingService.MarkPaidAsync(request.PaymentIntentId, userId);
        if (!ok) return NotFound();
        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var bookings = await _bookingService.GetUserBookingsAsync(userId!);
        return View(bookings);
    }
	public record PaymentConfirmRequest(string PaymentIntentId);
}



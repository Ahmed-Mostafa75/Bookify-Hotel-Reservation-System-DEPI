using Bookify.Data;
using Bookify.Data.Entities;
using System.Security.Claims;

public static class DbSeeder
{
	public static async Task SeedBookingAsync(IUnitOfWork uow, string userId)
	{
		// Check if user already has bookings
		var existing = await uow.Bookings.GetUserBookingsAsync(userId);
		if (existing.Any()) return; // already has bookings

		// Get first available room
		var room = (await uow.Repository<Room>().GetAllAsync(r => r.IsAvailable)).FirstOrDefault();
		if (room == null) return; // no rooms available

		var days = 2;
		var total = (await uow.Repository<RoomType>().GetByIdAsync(room.RoomTypeId)).BasePricePerNight * days;

		var booking = new Booking
		{
			UserId = userId,
			RoomId = room.Id,
			CheckIn = DateTime.Today,
			CheckOut = DateTime.Today.AddDays(days),
			TotalCost = total,
			Status = "Pending",
			StripePaymentIntentId = null
		};

		await uow.Bookings.AddAsync(booking);
		await uow.SaveChangesAsync();
	}
}

using Bookify.Data.Entities;

namespace Bookify.Services
{
	public interface IBookingService
	{
		Task<(Booking booking, string clientSecret)> CreateBookingWithPaymentAsync(
			string userId,
			int roomId,
			DateTime checkIn,
			DateTime checkOut,
			string currency,
			string? promoCode = null);

		Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId);

		Task<bool> MarkPaidAsync(string paymentIntentId, string userId);

		Task<string> CreateCheckoutSessionAsync(
			string userId,
			IEnumerable<(int RoomId, DateTime CheckIn, DateTime CheckOut)> cartItems,
			string currency = "usd");
	}
}

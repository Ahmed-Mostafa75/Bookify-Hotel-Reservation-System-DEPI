using Bookify.Data;
using Bookify.Data.Entities;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 

namespace Bookify.Services
{
	public class BookingService : IBookingService
	{
		private readonly IUnitOfWork _uow;
		private readonly IConfiguration _config;

		public BookingService(IUnitOfWork uow, IConfiguration config)
		{
			_uow = uow;
			_config = config;

			var apiKey = _config["Stripe:SecretKey"];
			if (!string.IsNullOrWhiteSpace(apiKey))
			{
				StripeConfiguration.ApiKey = apiKey;
			}
		}

		public async Task<(Booking booking, string clientSecret)> CreateBookingWithPaymentAsync(
			string userId,
			int roomId,
			DateTime checkIn,
			DateTime checkOut,
			string currency,
			string? promoCode = null)
		{
			var room = await _uow.Repository<Room>().GetByIdAsync(roomId);
			if (room is null) throw new InvalidOperationException("Room not found");
			if (!room.IsAvailable) throw new InvalidOperationException("Room not available");

			var days = Math.Max(1, (checkOut.Date - checkIn.Date).Days);

			var roomType = (await _uow.Repository<RoomType>()
				.GetAllAsync(r => r.Id == room.RoomTypeId)).FirstOrDefault();

			if (roomType is null) throw new InvalidOperationException("RoomType not found");

			var total = roomType.BasePricePerNight * days;

			if (!string.IsNullOrWhiteSpace(promoCode))
			{
				total = Math.Round(total * 0.9m, 2); // 10% off
			}

			string clientSecret = string.Empty;
			string? paymentIntentId = null;

			try
			{
				var paymentIntentService = new PaymentIntentService();
				var intent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
				{
					Amount = (long)(total * 100), // Stripe expects minor units
					Currency = currency,
					PaymentMethodTypes = new List<string> { "card" },
					Metadata = new Dictionary<string, string>
					{
						["roomId"] = roomId.ToString(),
						["userId"] = userId
					}
				});

				clientSecret = intent.ClientSecret;
				paymentIntentId = intent.Id;
			}
			catch
			{
				clientSecret = string.Empty;
				paymentIntentId = null;
			}

			var booking = new Booking
			{
				UserId = userId,
				RoomId = roomId,
				CheckIn = checkIn,
				CheckOut = checkOut,
				TotalCost = total,
				StripePaymentIntentId = paymentIntentId,
				Status = "Pending"
			};

			await _uow.Repository<Booking>().AddAsync(booking);
			await _uow.SaveChangesAsync();

			return (booking, clientSecret);
		}

		public Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId)
		{
			return _uow.Bookings.GetUserBookingsAsync(userId);
		}

		public async Task<bool> MarkPaidAsync(string paymentIntentId, string userId)
		{
			var booking = (await _uow.Bookings.GetUserBookingsAsync(userId))
				.FirstOrDefault(b => b.StripePaymentIntentId == paymentIntentId);

			if (booking == null) return false;

			booking.Status = "Paid";
			_uow.Repository<Booking>().Update(booking);
			await _uow.SaveChangesAsync();
			return true;
		}
		public async Task<string> CreateCheckoutSessionAsync(string userId, IEnumerable<(int RoomId, DateTime CheckIn, DateTime CheckOut)> cartItems, string currency = "usd")
		{
			var lineItems = new List<SessionLineItemOptions>();
			var createdBookingIds = new List<int>();

			foreach (var item in cartItems)
			{
				var room = await _uow.Repository<Room>().GetByIdAsync(item.RoomId);
				if (room == null) throw new InvalidOperationException($"Room {item.RoomId} not found");

				var roomType = await _uow.Repository<RoomType>().GetByIdAsync(room.RoomTypeId);
				if (roomType == null) throw new InvalidOperationException($"RoomType {room.RoomTypeId} not found");

				var days = Math.Max(1, (item.CheckOut.Date - item.CheckIn.Date).Days);
				var amount = (long)(roomType.BasePricePerNight * days * 100); // in cents

				lineItems.Add(new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						Currency = currency,
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = $"Room {room.Number} - {roomType.Name}"
						},
						UnitAmount = amount
					},
					Quantity = 1
				});

				var booking = new Booking
				{
					UserId = userId,
					RoomId = item.RoomId,
					CheckIn = item.CheckIn,
					CheckOut = item.CheckOut,
					TotalCost = (decimal)amount / 100m,
					StripePaymentIntentId = null,
					Status = "paid"
				};
				await _uow.Repository<Booking>().AddAsync(booking);
				await _uow.SaveChangesAsync();
				createdBookingIds.Add(booking.Id);
			}

			var bookingIdsCsv = string.Join(',', createdBookingIds);
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string> { "card" },
				LineItems = lineItems,
				Mode = "payment",
				SuccessUrl = "https://localhost:7176/Booking/Success?session_id={CHECKOUT_SESSION_ID}",
				CancelUrl = "https://localhost:7176/Booking/Cart",
				Metadata = new Dictionary<string, string>
				{
					["userId"] = userId,
					["bookingIds"] = bookingIdsCsv
				}
			};

			var service = new SessionService();
			var session = await service.CreateAsync(options);

			return session.Url; // ??? ?????? ??????? ??? Redirect ?? ??? Controller
		}

	}
}

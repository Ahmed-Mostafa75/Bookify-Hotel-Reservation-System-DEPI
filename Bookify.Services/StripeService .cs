// Services/StripeService.cs
using Bookify.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bookify.Data.Entities;


namespace Bookify.Services
{
	public class StripeService : IStripeService
	{
		private readonly ApplicationDbContext _db;
		private readonly ILogger<StripeService> _logger;

		public StripeService(ApplicationDbContext db, ILogger<StripeService> logger)
		{
			_db = db;
			_logger = logger;
		}

		public async Task<bool> HandleEventAsync(Event stripeEvent, string rawJson)
		{
			try
			{
				switch (stripeEvent.Type)
				{
					case "payment_intent.succeeded":

						{
							var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
							if (paymentIntent is null) break;

							// idempotency: check if PaymentIntentId already exists
							var exists = await _db.Transactions
								.AsNoTracking()
								.AnyAsync(t => t.PaymentIntentId == paymentIntent.Id);

							if (exists)
							{
								_logger.LogInformation("PaymentIntent {Id} already stored.", paymentIntent.Id);
								return true;
							}

							var transaction = new Transaction
							{
								PaymentIntentId = paymentIntent.Id,
								Amount = ConvertFromMinorUnits(paymentIntent.AmountReceived, paymentIntent.Currency),
								Currency = paymentIntent.Currency,
								Status = paymentIntent.Status,
								RawPayload = rawJson,
								CreatedAt = DateTime.UtcNow
							};

							_db.Transactions.Add(transaction);
							await _db.SaveChangesAsync();
							_logger.LogInformation("Saved PaymentIntent {Id} to DB.", paymentIntent.Id);
							break;
						}

					case "checkout.session.completed":
						{
							var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
							if (session is null) break;

							// PaymentIntent ID في Checkout Session تكون في PaymentIntent فقط
							var piId = session.PaymentIntentId;


							// التأكد أن الـ session أو الـ payment_intent لم يتم تسجيلهم مسبقًا
							var exists = await _db.Transactions
								.AsNoTracking()
								.AnyAsync(t =>
									t.CheckoutSessionId == session.Id ||
									t.PaymentIntentId == piId
								);

							if (exists)
							{
								_logger.LogInformation(
									"Session {Id} or PI {Pi} already stored.",
									session.Id,
									piId
								);
								return true;
							}

							// amount_total داخل Stripe يكون بالصورة (cents)
							decimal amountMajor = 0m;
							if (session.AmountTotal.HasValue)
								amountMajor = ConvertFromMinorUnits(
									session.AmountTotal.Value,
									session.Currency ?? "usd"
								);

				var transaction = new Transaction
				{
					PaymentIntentId = piId ?? string.Empty,
					CheckoutSessionId = session.Id,
					Amount = amountMajor,
					Currency = session.Currency,
					Status = "completed",
					RawPayload = rawJson,
					CreatedAt = DateTime.UtcNow
				};

				_db.Transactions.Add(transaction);
				await _db.SaveChangesAsync();

				_logger.LogInformation(
					"Saved Checkout Session {Id} to DB.",
					session.Id
				);

				// Update related bookings as Paid
				var userId = session.Metadata != null && session.Metadata.TryGetValue("userId", out var uid) ? uid : null;
				var bookingIdsCsv = session.Metadata != null && session.Metadata.TryGetValue("bookingIds", out var bids) ? bids : null;
				if (!string.IsNullOrWhiteSpace(bookingIdsCsv))
				{
					var ids = bookingIdsCsv
						.Split(',', StringSplitOptions.RemoveEmptyEntries)
						.Select(s => int.TryParse(s, out var id) ? id : 0)
						.Where(id => id > 0)
						.ToArray();

					var bookings = await _db.Bookings
						.Where(b => ids.Contains(b.Id))
						.ToListAsync();

					foreach (var b in bookings)
					{
						b.StripePaymentIntentId = piId;
						b.Status = "Paid";
					}
					await _db.SaveChangesAsync();
					_logger.LogInformation("Marked {Count} bookings paid via session {SessionId}", bookings.Count, session.Id);
				}

				break;
			}


					// add other events you care about
					default:
						_logger.LogInformation("Unhandled stripe event type: {Type}", stripeEvent.Type);
						break;
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error handling stripe event {Type}", stripeEvent.Type);
				return false;
			}
		}

		private decimal ConvertFromMinorUnits(long amountMinor, string? currency)
		{
			// most currencies: 100 minor units = 1 major unit
			// For robust solution, use Stripe's currency metadata (like JPY has 0 decimals) — simple approach:
			int decimals = 2;
			if (!string.IsNullOrEmpty(currency))
			{
				var c = currency.ToUpper();
				if (c == "JPY" || c == "VND") decimals = 0;
			}

			decimal divisor = (decimal)Math.Pow(10, decimals);
			return Math.Round(amountMinor / divisor, decimals);
		}
	}
}

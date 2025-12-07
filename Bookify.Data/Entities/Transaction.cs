using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Data.Entities
{
	public class Transaction
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string PaymentIntentId { get; set; } = string.Empty;

		public string? CheckoutSessionId { get; set; }

		public decimal Amount { get; set; } // in major currency units (e.g. dollars)

		public string? Currency { get; set; }

		public string? Status { get; set; }

		public string? RawPayload { get; set; } // optional: raw JSON for audit

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}

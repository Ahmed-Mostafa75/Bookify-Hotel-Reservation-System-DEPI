using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bookify.Data.Entities;

namespace Bookify.Data.Repositories
{
	public interface ITransactionRepository : IGenericRepository<Transaction>
	{
		Task<Transaction?> GetByPaymentIntentIdAsync(string paymentIntentId);
		Task<Transaction?> GetByCheckoutSessionIdAsync(string sessionId);
	}

	public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
	{
		public TransactionRepository(ApplicationDbContext context) : base(context)
		{
		}

		public async Task<Transaction?> GetByPaymentIntentIdAsync(string paymentIntentId)
		{
			return await _context.Transactions
				.AsNoTracking()
				.FirstOrDefaultAsync(t => t.PaymentIntentId == paymentIntentId);
		}

		public async Task<Transaction?> GetByCheckoutSessionIdAsync(string sessionId)
		{
			return await _context.Transactions
				.AsNoTracking()
				.FirstOrDefaultAsync(t => t.CheckoutSessionId == sessionId);
		}
	}
}

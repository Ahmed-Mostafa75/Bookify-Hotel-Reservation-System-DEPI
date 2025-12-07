using Stripe;
using System.Threading.Tasks;

namespace Bookify.Services
{
	public interface IStripeService
	{
		Task<bool> HandleEventAsync(Event stripeEvent, string rawJson);
	}
}


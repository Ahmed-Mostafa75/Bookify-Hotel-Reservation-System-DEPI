using Bookify.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using Stripe;

namespace Bookify.Web.Controllers
{
	public class StripeWebhookController : Controller
	{
		private readonly IStripeService _stripeService;
		private readonly IConfiguration _config;

		public StripeWebhookController(IStripeService stripeService, IConfiguration config)
		{
			_stripeService = stripeService;
			_config = config;
		}

		[HttpPost]
		public async Task<IActionResult> Post()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			var secret = _config["Stripe:WebhookSecret"];

			try
			{
				var stripeEvent = EventUtility.ConstructEvent(
					json,
					Request.Headers["Stripe-Signature"].ToString(),
					secret
				);

				await _stripeService.HandleEventAsync(stripeEvent, json);
				return Ok();
			}
			catch (StripeException e)
			{
				return BadRequest(e.Message);
			}
		}
	}
}

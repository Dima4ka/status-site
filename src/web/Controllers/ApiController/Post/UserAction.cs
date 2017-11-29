using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatusMonitor.Web.ActionFilters;
using StatusMonitor.Shared.Extensions;
using StatusMonitor.Shared.Models.Entities;
using StatusMonitor.Web.ViewModels;

namespace StatusMonitor.Web.Controllers.Api
{
	public partial class ApiController
	{
		[HttpPost]
		[ServiceFilter(typeof(ApiKeyCheck))]
		[ServiceFilter(typeof(ModelValidation))]
		public async Task<IActionResult> UserAction(UserActionViewModel model)
		{
			// Retrieve requested metric
			var metric = await _metricService.GetOrCreateMetricAsync(Metrics.UserAction, model.Source);

			_context.Attach(metric);

			// Record data
			await _context
				.UserActionDataPoints
				.AddAsync(
					new UserActionDataPoint
					{
						Count = model.Count,
						Action = model.Action,
						Metric = metric
					}
				);

			// Submit changes to data provider
			await _context.SaveChangesAsync();

			return Ok("Data point has been recorded.");
		}
	}
}

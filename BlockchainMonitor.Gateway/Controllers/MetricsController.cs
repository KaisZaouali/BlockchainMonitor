using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainMonitor.Gateway.Controllers;

/// <summary>
/// Metrics controller for exposing application metrics.
/// Provides endpoints for retrieving real-time metrics from Redis or local storage.
/// Used by the monitoring dashboard to display performance and operational data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;

    public MetricsController(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMetrics()
    {
        try
        {
            var metrics = await _metricsService.GetMetrics();
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve metrics", message = ex.Message });
        }
    }
} 
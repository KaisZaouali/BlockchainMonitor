using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainMonitor.Gateway.Controllers;

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
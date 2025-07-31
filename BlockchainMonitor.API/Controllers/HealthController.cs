using Microsoft.AspNetCore.Mvc;
using BlockchainMonitor.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace BlockchainMonitor.API.Controllers;

/// <summary>
/// Health check controller for API monitoring.
/// Provides simple health status endpoints for load balancers and monitoring systems.
/// Returns basic application status and availability information.
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly IBlockchainService _blockchainService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IBlockchainService blockchainService, ILogger<HealthController> logger)
    {
        _blockchainService = blockchainService ?? throw new ArgumentNullException(nameof(blockchainService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            _logger.LogDebug("Health check requested");

            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "BlockchainMonitor.API",
                version = "1.0.0"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new
            {
                status = "unhealthy",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Detailed health check with database connectivity
    /// </summary>
    /// <returns>Detailed health status</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        try
        {
            _logger.LogDebug("Detailed health check requested");

            // Test database connectivity
            var totalRecords = await _blockchainService.GetTotalRecordsAsync();

            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "BlockchainMonitor.API",
                version = "1.0.0",
                database = new
                {
                    status = "connected",
                    totalRecords
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detailed health check failed");
            return StatusCode(500, new
            {
                status = "unhealthy",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }
}

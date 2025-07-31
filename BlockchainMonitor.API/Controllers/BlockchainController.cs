using BlockchainMonitor.Application.DTOs;
using BlockchainMonitor.Application.Exceptions;
using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Application.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BlockchainMonitor.API.Controllers;

/// <summary>
/// REST API controller for blockchain data operations.
/// Provides endpoints for retrieving blockchain history, latest data, and health checks.
/// Handles HTTP requests and responses with proper status codes and validation.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BlockchainController : ControllerBase
{
    private readonly IBlockchainService _blockchainService;

    public BlockchainController(IBlockchainService blockchainService)
    {
        _blockchainService = blockchainService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlockchainDataDto>>> GetAllBlockchainData()
    {
        var data = await _blockchainService.GetAllBlockchainDataAsync();
        return Ok(data);
    }

    [HttpGet("{blockchainName}")]
    public async Task<ActionResult<BlockchainDataDto>> GetLatestBlockchainData(
        [Required]
        [StringLength(20, MinimumLength = 1)]
        string blockchainName)
    {
        var data = await _blockchainService.GetLatestBlockchainDataAsync(blockchainName);
        if (data == null)
            throw new BlockchainNotFoundException(blockchainName);

        return Ok(data);
    }

    [HttpGet("{blockchainName}/history")]
    public async Task<ActionResult<IEnumerable<BlockchainDataDto>>> GetBlockchainHistory(
        [Required]
        [StringLength(20, MinimumLength = 1)]
        string blockchainName,
        [FromQuery]
        [Range(1, BlockchainConstants.MaxHistoryLimit)]
        int limit = BlockchainConstants.DefaultHistoryLimit)
    {
        var history = await _blockchainService.GetBlockchainHistoryAsync(blockchainName, limit);
        return Ok(history);
    }

    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<BlockchainDataDto>>> GetLatestData()
    {
        var data = await _blockchainService.GetLatestDataAsync();
        return Ok(data);
    }
}

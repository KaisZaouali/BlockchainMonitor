using BlockchainMonitor.Application.DTOs;
using BlockchainMonitor.Application.Exceptions;
using BlockchainMonitor.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainMonitor.API.Controllers;

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
    public async Task<ActionResult<BlockchainDataDto>> GetLatestBlockchainData(string blockchainName)
    {
        var data = await _blockchainService.GetLatestBlockchainDataAsync(blockchainName);
        if (data == null)
            throw new BlockchainNotFoundException(blockchainName);

        return Ok(data);
    }

    [HttpGet("{blockchainName}/history")]
    public async Task<ActionResult<IEnumerable<BlockchainDataDto>>> GetBlockchainHistory(
        string blockchainName, 
        [FromQuery] int limit = 100)
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
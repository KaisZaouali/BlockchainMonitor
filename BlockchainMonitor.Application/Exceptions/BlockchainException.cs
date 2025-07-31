namespace BlockchainMonitor.Application.Exceptions;

/// <summary>
/// Base exception class for blockchain-related errors.
/// Provides a consistent exception type for all blockchain operation failures.
/// Used for validation errors, data processing errors, and business logic exceptions.
/// </summary>
public class BlockchainException : Exception
{
    public BlockchainException(string message) : base(message)
    {
    }

    public BlockchainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class BlockchainNotFoundException : BlockchainException
{
    public BlockchainNotFoundException(string blockchainName) 
        : base($"Blockchain data for '{blockchainName}' not found")
    {
    }
}

public class InvalidBlockchainDataException : BlockchainException
{
    public InvalidBlockchainDataException(string message) : base(message)
    {
    }
} 
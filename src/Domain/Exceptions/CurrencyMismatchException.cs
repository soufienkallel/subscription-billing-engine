using Domain.ValueObjects;
namespace Domain.Exceptions;

public class CurrencyMismatchException : Exception
{
    public CurrencyMismatchException(Currency left, Currency right)
        : base($"Cannot operate on Money values with different currencies: {left} and {right}.")
    {
    }
}

using Domain.Exceptions;
using Domain.ValueObjects;

namespace UnitTests;

public class MoneyTests
{
    [Fact]
    public void Adding_Same_Currency_Succeeds()
    {
        var a = new Money(10.50m, Currency.USD);
        var b = new Money(4.30m, Currency.USD);

        var result = a + b;

        Assert.Equal(14.80m, result.Amount);
        Assert.Equal(Currency.USD, result.Currency);
    }

    [Fact]
    public void Subtracting_Same_Currency_Succeeds()
    {
        var a = new Money(10.50m, Currency.USD);
        var b = new Money(4.30m, Currency.USD);

        var result = a - b;

        Assert.Equal(6.20m, result.Amount);
        Assert.Equal(Currency.USD, result.Currency);
    }

    [Fact]
    public void Adding_Different_Currencies_Throws()
    {
        var a = new Money(10m, Currency.USD);
        var b = new Money(10m, Currency.EUR);

        Assert.Throws<CurrencyMismatchException>(() => a + b);
    }

    [Fact]
    public void Subtracting_Different_Currencies_Throws()
    {
        var a = new Money(10m, Currency.USD);
        var b = new Money(10m, Currency.EUR);

        Assert.Throws<CurrencyMismatchException>(() => a - b);
    }

    [Fact]
    public void Equal_Amount_And_Currency_Are_Equal()
    {
        var a = new Money(10m, Currency.USD);
        var b = new Money(10m, Currency.USD);

        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void Different_Amount_Are_Not_Equal()
    {
        var a = new Money(10m, Currency.USD);
        var b = new Money(20m, Currency.USD);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Different_Currency_Are_Not_Equal()
    {
        var a = new Money(10m, Currency.USD);
        var b = new Money(10m, Currency.EUR);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Rounds_Midpoint_Down_To_Nearest_Even_Digit()
    {
        var money = new Money(0.125m, Currency.USD);

        Assert.Equal(0.12m, money.Amount);
    }

    [Fact]
    public void Rounds_Midpoint_Up_To_Nearest_Even_Digit()
    {
        var money = new Money(0.135m, Currency.USD);

        Assert.Equal(0.14m, money.Amount);
    }
}

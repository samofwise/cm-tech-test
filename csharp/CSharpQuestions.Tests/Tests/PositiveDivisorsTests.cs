using CSharpQuestions.Api.Services;
using Xunit;

namespace CSharpQuestions.Tests.Tests;

public class PositiveDivisorsTests
{
  private readonly QuestionsService _service = new();

  [Theory]
  // Examples from spec
  [InlineData(60, new[] { 1, 2, 3, 4, 5, 6, 10, 12, 15, 20, 30, 60 })]
  [InlineData(42, new[] { 1, 2, 3, 6, 7, 14, 21, 42 })]
  // Small numbers
  [InlineData(1, new[] { 1 })]
  [InlineData(2, new[] { 1, 2 })]
  [InlineData(3, new[] { 1, 3 })]
  [InlineData(4, new[] { 1, 2, 4 })]
  // Prime numbers
  [InlineData(5, new[] { 1, 5 })]
  [InlineData(7, new[] { 1, 7 })]
  [InlineData(11, new[] { 1, 11 })]
  // Perfect squares
  [InlineData(9, new[] { 1, 3, 9 })]
  [InlineData(16, new[] { 1, 2, 4, 8, 16 })]
  [InlineData(25, new[] { 1, 5, 25 })]
  // Numbers with many divisors
  [InlineData(24, new[] { 1, 2, 3, 4, 6, 8, 12, 24 })]
  [InlineData(36, new[] { 1, 2, 3, 4, 6, 9, 12, 18, 36 })]
  public void GetPositiveDivisors_ValidInput_ReturnsCorrectDivisors(int number, int[] expected)
  {
    // Act
    var result = _service.GetPositiveDivisors(number);

    // Assert
    Assert.Equal(expected, result);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  [InlineData(-42)]
  public void GetPositiveDivisors_InvalidInput_ThrowsArgumentException(int number)
  {
    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() => _service.GetPositiveDivisors(number));
    Assert.Equal("Number must be greater than 0", exception.Message);
  }

  [Fact]
  public void GetPositiveDivisors_LargeNumber_ReturnsCorrectDivisors()
  {
    // Arrange
    var number = 1000;
    var expected = new[] { 1, 2, 4, 5, 8, 10, 20, 25, 40, 50, 100, 125, 200, 250, 500, 1000 };

    // Act
    var result = _service.GetPositiveDivisors(number);

    // Assert
    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetPositiveDivisors_PrimeSquare_ReturnsCorrectDivisors()
  {
    // Arrange
    var number = 49;
    var expected = new[] { 1, 7, 49 };

    // Act
    var result = _service.GetPositiveDivisors(number);

    // Assert
    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetPositiveDivisors_ProductOfTwoPrimes_ReturnsCorrectDivisors()
  {
    // Arrange
    var number = 35;
    var expected = new[] { 1, 5, 7, 35 };

    // Act
    var result = _service.GetPositiveDivisors(number);

    // Assert
    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetPositiveDivisors_PowerOfTwo_ReturnsCorrectDivisors()
  {
    // Arrange
    var number = 32;
    var expected = new[] { 1, 2, 4, 8, 16, 32 };

    // Act
    var result = _service.GetPositiveDivisors(number);

    // Assert
    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetPositiveDivisors_NumberWithManyFactors_ReturnsCorrectDivisors()
  {
    // Arrange
    var number = 120;
    var expected = new[] { 1, 2, 3, 4, 5, 6, 8, 10, 12, 15, 20, 24, 30, 40, 60, 120 };

    // Act
    var result = _service.GetPositiveDivisors(number);

    // Assert
    Assert.Equal(expected, result);
  }
}
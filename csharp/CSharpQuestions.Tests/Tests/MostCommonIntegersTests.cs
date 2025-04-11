using CSharpQuestions.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpQuestions.Tests.Tests;

public class MostCommonIntegersTests
{
  private readonly QuestionsService _service = new();

  [Theory]
  // Examples from Spec
  [InlineData(new[] { 5, 4, 3, 2, 4, 5, 1, 6, 1, 2, 5, 4 }, new[] { 5, 4 })]
  [InlineData(new[] { 1, 2, 3, 4, 5, 1, 6, 7 }, new[] { 1 })]
  [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7 }, new[] { 1, 2, 3, 4, 5, 6, 7 })]
  // Additional
  [InlineData(new[] { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4 }, new[] { 4 })] // Single number
  [InlineData(new[] { 1, 1, 2, 2, 3 }, new[] { 1, 2 })] // Two numbers 
  [InlineData(new[] { 1, 1, 1, 2, 2, 2, 3, 3, 3 }, new[] { 1, 2, 3 })] // Three numbers
  public void GetMostCommonIntegers_ValidInput_ReturnsCorrectResults(int[] input, int[] expected)
  {
    // Act
    var result = _service.GetMostCommonIntegers(input);

    // Assert
    Assert.Equal(expected.OrderBy(x => x), result.OrderBy(x => x));
  }

  [Fact]
  public void GetMostCommonIntegers_EmptyArray_ReturnsEmptyArray()
  {
    // Arrange
    var input = Array.Empty<int>();

    // Act
    var result = _service.GetMostCommonIntegers(input);

    // Assert
    Assert.Empty(result);
  }

  [Fact]
  public void GetMostCommonIntegers_SingleElement_ReturnsSingleElement()
  {
    // Arrange
    var input = new[] { 42 };

    // Act
    var result = _service.GetMostCommonIntegers(input);

    // Assert
    Assert.Equal([42], result);
  }

  [Theory]

  [InlineData(new[] { -1, -1, -2, -2, -3 }, new[] { -1, -2 })] // Negatives
  [InlineData(new[] { 0, 0, 1, 1, 1 }, new[] { 1 })] // Zero
  [InlineData(new[] { -1, 1, -1, 1, 0 }, new[] { -1, 1 })] // Positives and negatives
  public void GetMostCommonIntegers_SpecialNumbers_ReturnsCorrectResults(int[] input, int[] expected)
  {
    // Act
    var result = _service.GetMostCommonIntegers(input);

    // Assert
    Assert.Equal(expected.OrderBy(x => x), result.OrderBy(x => x));
  }

  [Fact]
  public void GetMostCommonIntegers_LargeArray_ReturnsCorrectResults()
  {
    // Arrange
    var input = Enumerable.Range(0, 1000) // 0 to 999
        .Concat(Enumerable.Repeat(42, 1000)) // 42 repeated
        .Concat(Enumerable.Repeat(99, 1000)) // 99 repeated
        .ToArray();

    // Act
    var result = _service.GetMostCommonIntegers(input);

    // Assert
    Assert.Equal([42, 99], result.OrderBy(x => x));
  }

  [Fact]
  public void GetMostCommonIntegers_AllSameNumber_ReturnsSingleNumber()
  {
    // Arrange
    var input = Enumerable.Repeat(7, 100).ToArray();

    // Act
    var result = _service.GetMostCommonIntegers(input);

    // Assert
    Assert.Equal(new int[] { 7 }, result);
  }

  [Fact]
  public void GetMostCommonIntegers_AlternatingNumbers_ReturnsAllNumbers()
  {
    // Arrange
    var input = new[] { 1, 2, 1, 2, 1, 2 };

    // Act
    var result = _service.GetMostCommonIntegers(input);

    // Assert
    Assert.Equal([1, 2], result.OrderBy(x => x));
  }

  [Theory]
  [InlineData(new[] { 1, 2, 3, 1, 2, 3, 1, 2, 3 }, new[] { 1, 2, 3 })] // Consecutive numbers
  [InlineData(new[] { 10, 20, 30, 10, 20, 30, 10 }, new[] { 10 })] // Non-consecutive numbers
  public void GetMostCommonIntegers_VariousPatterns_ReturnsCorrectResults(int[] input, int[] expected)
  {
    // Act
    var result = _service.GetMostCommonIntegers(input);

    // Assert
    Assert.Equal(expected.OrderBy(x => x), result.OrderBy(x => x));
  }
}
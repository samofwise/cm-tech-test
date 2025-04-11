using CSharpQuestions.Api.Services;
using CSharpQuestions.Api.Exceptions;

namespace CSharpQuestions.Tests.Tests;

public class TriangleAreaTests
{
  private readonly TriangleAreaService _service = new();

  [Theory]
  [InlineData(3, 4, 5, 6)] // Right-angled triangle
  [InlineData(5, 5, 5, 10.825317547305483)] // Equilateral triangle
  [InlineData(5, 5, 6, 12)] // Isosceles triangle
  [InlineData(7, 8, 9, 26.832815729997478)] // Scalene triangle
  public void CalculateTriangleArea_ValidTriangles_ReturnsCorrectArea(int first, int second, int third, double expectedArea)
  {
    // Act
    var result = _service.CalculateTriangleArea(first, second, third);

    // Assert
    Assert.Equal(expectedArea, result, 10);
  }

  [Theory]
  [InlineData(0, 1, 1)] // Zero side
  [InlineData(1, 0, 1)]
  [InlineData(1, 1, 0)]
  [InlineData(-1, 1, 1)] // Negative side
  [InlineData(1, -1, 1)]
  [InlineData(1, 1, -1)]
  public void CalculateTriangleArea_NonPositiveSides_ThrowsInvalidTriangleException(int first, int second, int third)
  {
    // Act & Assert
    var exception = Assert.Throws<InvalidTriangleException>(() => _service.CalculateTriangleArea(first, second, third));
    Assert.Contains("All sides must be greater than 0", exception.Message);
  }

  [Theory]
  [InlineData(1, 1, 3)] 
  [InlineData(1, 3, 1)]
  [InlineData(3, 1, 1)]
  [InlineData(1, 2, 3)] 
  [InlineData(1, 3, 2)]
  [InlineData(3, 1, 2)]
  public void CalculateTriangleArea_ViolatesTriangleInequality_ThrowsInvalidTriangleException(int first, int second, int third)
  {
    // Act & Assert
    var exception = Assert.Throws<InvalidTriangleException>(() => _service.CalculateTriangleArea(first, second, third));
    Assert.Contains("Sum of any two sides must exceed the third", exception.Message);
  }

  [Theory]
  [InlineData(int.MaxValue, int.MaxValue, int.MaxValue)] 
  [InlineData(1, 1, 1)]
  public void CalculateTriangleArea_ExtremeValues_DoesNotThrow(int first, int second, int third)
  {
    // Act & Assert
    var exception = Record.Exception(() => _service.CalculateTriangleArea(first, second, third));
    Assert.Null(exception);
  }

  [Fact]
  public void CalculateTriangleArea_OrderOfSidesDoesNotMatter_ReturnsSameArea()
  {
    // Arrange
    const int first = 7;
    const int second = 8;
    const int third = 9;
    const double expectedArea = 26.83281573;

    // Act
    var result1 = _service.CalculateTriangleArea(first, second, third);
    var result2 = _service.CalculateTriangleArea(second, third, first);
    var result3 = _service.CalculateTriangleArea(third, first, second);

    // Assert
    Assert.Equal(expectedArea, result1, 10);
    Assert.Equal(expectedArea, result2, 10);
    Assert.Equal(expectedArea, result3, 10);
  }
}
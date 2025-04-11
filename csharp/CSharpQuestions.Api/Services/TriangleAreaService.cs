using CSharpQuestions.Api.Exceptions;

namespace CSharpQuestions.Api.Services;

public class TriangleAreaService
{

  /// <summary>
  /// Calculate the area of a triangle using Heron's formula
  /// Source: https://www.cuemath.com/measurement/area-of-triangle-with-3-sides
  /// </summary>
  /// <param name="first">The first side of the triangle</param>
  /// <param name="second">The second side of the triangle</param>
  /// <param name="third">The third side of the triangle</param>
  /// <returns>The area of the triangle</returns>
  public double CalculateTriangleArea(int first, int second, int third)
  {
    ValidateTriangleSides(first, second, third);
    ValidateTriangleInequality(first, second, third);

    //Get area of triangle using Heron's formula
    var s = ((double)first + second + third) / 2.0;
    return Math.Sqrt(s * (s - first) * (s - second) * (s - third));
  }

  private static void ValidateTriangleSides(int first, int second, int third)
  {
    var negativeSides = new[] { ("first", first), ("second", second), ("third", third) }.Where(s => s.Item2 <= 0).ToList();
    if (negativeSides.Count != 0)
      throw new InvalidTriangleException(GetNegativeSidesMessage(negativeSides));
  }

  private static string GetNegativeSidesMessage(List<(string, int)> negativeSides)
    => $"All sides must be greater than 0. Negative sides: {string.Join(", ", negativeSides.Select(s => $"{s.Item1}: {s.Item2}"))}";

  private static void ValidateTriangleInequality(int first, int second, int third)
  {
    var triangleInequalityChecks = new[] {
      ($"first({first}) + second({second}) is not greater than third({third})", !((double)first + second > third)),
      ($"first({first}) + third({third}) is not greater than second({second})", !((double)first + third > second)),
      ($"second({second}) + third({third}) is not greater than first({first})", !((double)second + third > first))
    }.Where(c => c.Item2).ToList();

    if (triangleInequalityChecks.Count != 0)
      throw new InvalidTriangleException(GetTriangleInequalityMessage([.. triangleInequalityChecks.Select(c => c.Item1)]));
  }

  private static string GetTriangleInequalityMessage(List<string> invalidMessages)
    => $"Sum of any two sides must exceed the third. {string.Join(", ", invalidMessages)}";
}
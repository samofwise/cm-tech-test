using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CSharpQuestions.Api.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;

namespace CSharpQuestions.Api.Services;

public class QuestionsService
{
  public List<int> GetPositiveDivisors(int number)
  {
    if (number <= 0)
      throw new ArgumentException("Number must be greater than 0");

    if (number == 1)
      return [1];

    if (number == 2)
      return [1, 2];

    var limit = (int)Math.Sqrt(number);

    var bucketCount = Math.Min(Environment.ProcessorCount, limit);
    var bucketSize = (int)Math.Ceiling((double)limit / bucketCount);

    var bucketResults = new ConcurrentDictionary<int, (List<int> lowDivisors, List<int> highDivisors)>();

    Parallel.For(0, bucketCount, bucketIndex =>
    {
      // Skip 1 as it is included at the bottom
      var start = bucketIndex * bucketSize + 2;
      var end = Math.Min(start + bucketSize - 1, limit);

      var result = GetDivisors(number, start, end);
      var didAdd = bucketResults.TryAdd(bucketIndex, result);
      if (!didAdd)
        throw new InvalidOperationException("Index already exists in bucket");
    });

    var lowDivisors = new List<int>();
    var highDivisors = new List<int>();

    foreach (var bucketResult in bucketResults.Values)
    {
      lowDivisors.AddRange(bucketResult.lowDivisors);
      highDivisors.AddRange(bucketResult.highDivisors);
    }

    return [1, .. lowDivisors, .. highDivisors, number];
  }

  private static (List<int> lowDivisors, List<int> highDivisors) GetDivisors(int number, int start, int end)
  {
    var lowDivisors = new List<int>();
    var highDivisors = new List<int>();

    for (int i = start; i <= end; i++)
    {
      if (number % i == 0)
      {
        lowDivisors.Add(i);

        var highDivisor = number / i;
        if (highDivisor != i) // Avoid adding square root twice
          highDivisors.Insert(0, highDivisor);
      }
    }

    return (lowDivisors, highDivisors);
  }

  public int[] GetMostCommonIntegers(int[] numbers)
  {
    if (numbers == null || numbers.Length == 0)
      return [];

    var frequency = numbers
        .GroupBy(n => n)
        .ToDictionary(g => g.Key, g => g.Count());

    var maxFrequency = frequency.Values.Max();
    return [.. frequency
        .Where(kvp => kvp.Value == maxFrequency)
        .Select(kvp => kvp.Key)];
  }
}

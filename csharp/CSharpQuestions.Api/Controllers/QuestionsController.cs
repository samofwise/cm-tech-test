using Microsoft.AspNetCore.Mvc;
using CSharpQuestions.Api.Services;
using CSharpQuestions.Api.Exceptions;

namespace CSharpQuestions.Api.Controllers
{
  [ApiController]
  [Route("questions")]
  public class QuestionsController(TriangleAreaService _triangleAreaService, QuestionsService _questionsService, LinkCheckerService _linkCheckerService) : ControllerBase
  {

    [HttpGet("positive-divisors")]
    public IActionResult GetPositiveDivisors([FromQuery] int number)
    {
      try
      {
        var divisors = _questionsService.GetPositiveDivisors(number);
        return Ok(divisors);
      }
      catch (ArgumentException ex)
      {
        return BadRequest(new { error = "ArgumentException", message = ex.Message });
      }
    }


    [HttpGet("triangle-area")]
    public IActionResult Question3_CalculateTriangleArea([FromQuery] int first, [FromQuery] int second, [FromQuery] int third)
    {
      try
      {
        var area = _triangleAreaService.CalculateTriangleArea(first, second, third);
        return Ok(new { area });
      }
      catch (InvalidTriangleException ex)
      {
        return BadRequest(new { error = "InvalidTriangleException", message = ex.Message });
      }
    }

    [HttpGet("most-common-integers")]
    public IActionResult GetMostCommonIntegers([FromBody] int[] numbers)
    {
      if (numbers == null || numbers.Length == 0)
        return BadRequest("Numbers array cannot be null or empty");

      var mostCommon = _questionsService.GetMostCommonIntegers(numbers);
      return Ok(mostCommon);
    }

    [HttpGet("link-checker")]
    public async Task<IActionResult> CheckLinks([FromBody] string htmlContent)
    {
      var results = await _linkCheckerService.CheckLinks(htmlContent);
      return Ok(results);
    }
  }
}
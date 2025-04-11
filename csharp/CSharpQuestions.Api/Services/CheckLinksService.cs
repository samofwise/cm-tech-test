using System.Net.Http;
using System.Text.RegularExpressions;
using CSharpQuestions.Api.Exceptions;

namespace CSharpQuestions.Api.Services;

public class CheckLinksService(IHttpClientFactory httpClientFactory)
{
  private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

 
}
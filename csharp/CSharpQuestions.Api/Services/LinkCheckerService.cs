using System.Collections.Concurrent;
using System.Net.Http;
using System.Text.RegularExpressions;
using CSharpQuestions.Api.Exceptions;
using CSharpQuestions.Api.Models;
using System.Net;

namespace CSharpQuestions.Api.Services;

public class LinkCheckerService(IHttpClientFactory httpClientFactory)
{
  private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
  private static readonly Regex _linkRegex = new(@"<a\s+[^>]*href=[""'](?<url>https?://[^""']+)[""'][^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

  public async Task<List<Link>> CheckLinks(string text)
  {
    if (string.IsNullOrWhiteSpace(text))
      return [];

    var links = new ConcurrentDictionary<string, bool>();
    var tasks = new List<Task>();

    var match = _linkRegex.Match(text);
    while (match.Success)
    {
      if (!links.ContainsKey(match.Value))
      {
        var url = match.Groups["url"].Value;
        tasks.Add(Task.Run(async () =>
        {
          var isValid = await VerifyLinkAsync(url);
          links.TryAdd(url, isValid);
        }));
      }
      match = match.NextMatch();
    }

    await Task.WhenAll(tasks);
    return [.. links.Select(l => new Link { Url = l.Key, IsValid = l.Value })];
  }

  private async Task<bool> VerifyLinkAsync(string url)
  {
    try
    {
      var response = await _httpClient.GetAsync(url);
      return response.IsSuccessStatusCode;
    }
    catch (HttpRequestException)
    {
      return false;
    }
  }
}
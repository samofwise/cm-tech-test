using System.Net;
using CSharpQuestions.Api.Services;
using Moq;
using Moq.Protected;

namespace CSharpQuestions.Tests.Tests;

public class LinkCheckerTests
{
  private readonly LinkCheckerService _service;
  private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
  private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
  public LinkCheckerTests()
  {
    _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    _mockHttpClientFactory = new Mock<IHttpClientFactory>();
    var _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

    _mockHttpClientFactory
      .Setup(x => x.CreateClient(It.IsAny<string>()))
      .Returns(_httpClient);

    _service = new LinkCheckerService(_mockHttpClientFactory.Object);
  }

  [Fact]
  public async Task CheckLinks_EmptyText_ReturnsEmptyList()
  {
    // Act
    var result = await _service.CheckLinks("");

    // Assert
    Assert.Empty(result);
  }

  [Fact]
  public async Task CheckLinks_NullText_ReturnsEmptyList()
  {
    // Act
    var result = await _service.CheckLinks(null!);

    // Assert
    Assert.Empty(result);
  }

  [Fact]
  public async Task CheckLinks_WhitespaceText_ReturnsEmptyList()
  {
    // Act
    var result = await _service.CheckLinks("   ");

    // Assert
    Assert.Empty(result);
  }

  [Fact]
  public async Task CheckLinks_NoLinks_ReturnsEmptyList()
  {
    // Arrange
    var html = "<p>No links here</p>";

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Empty(result);
  }

  [Fact]
  public async Task CheckLinks_ValidLinks_ReturnsCorrectResults()
  {
    // Arrange
    var html = File.ReadAllText("TestData/TestHtml.html");

    // Setup mock responses for all URLs in the test HTML
    SetupMockResponse("https://valid-link.com/", HttpStatusCode.OK);
    SetupMockResponse("https://another-valid.com/", HttpStatusCode.OK);
    SetupMockResponse("https://invalid-link.com/", HttpStatusCode.NotFound);
    SetupMockResponse("https://table-link.com/", HttpStatusCode.OK);
    SetupMockResponse("https://duplicate-link.com/", HttpStatusCode.OK);
    SetupMockResponse("https://different-case.com/", HttpStatusCode.OK);

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Equal(6, result.Count);
    Assert.All(result.Where(r => r.Url != "https://invalid-link.com"), r => Assert.True(r.IsValid));
    Assert.All(result.Where(r => r.Url == "https://invalid-link.com"), r => Assert.False(r.IsValid));
  }

  [Fact]
  public async Task CheckLinks_DuplicateLinks_ReturnsUniqueResults()
  {
    // Arrange
    var html = "<a href='https://duplicate.com'>Link 1</a><a href='https://duplicate.com'>Link 2</a>";
    SetupMockResponse("https://duplicate.com/", HttpStatusCode.OK);

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Single(result);
    Assert.Equal("https://duplicate.com", result[0].Url);
    Assert.True(result[0].IsValid);
  }

  [Fact]
  public async Task CheckLinks_InvalidLinks_ReturnsCorrectResults()
  {
    // Arrange
    var html = """
            <a>No href</a>
            <a href="">Empty href</a>
            <a href="not-a-url">Not a URL</a>
            <a href="ftp://ftp.example.com">Not HTTP(S)</a>
            """;

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Empty(result);
  }

  [Fact]
  public async Task CheckLinks_NetworkError_ReturnsInvalidLink()
  {
    // Arrange
    var html = "<a href='https://error.com'>Error Link</a>";
    SetupMockResponse("https://error.com/", HttpStatusCode.NotFound);

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Single(result);
    Assert.Equal("https://error.com", result[0].Url);
    Assert.False(result[0].IsValid);
  }

  [Fact]
  public async Task CheckLinks_MultipleLinks_ProcessesConcurrently()
  {
    // Arrange
    var html = string.Join("", Enumerable.Range(0, 10)
        .Select(i => $"<a href='https://link{i}.com'>Link {i}</a>"));

    var responses = Enumerable.Range(0, 10)
        .ToDictionary(i => $"https://link{i}.com/", i => HttpStatusCode.OK);

    foreach (var (url, status) in responses)
    {
      SetupMockResponse(url, status);
    }

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Equal(10, result.Count);
    Assert.All(result, r => Assert.True(r.IsValid));
  }

  [Fact]
  public async Task CheckLinks_RedirectLinks_ReturnsCorrectResults()
  {
    // Arrange
    var html = @"
      <a href='https://redirect1.com'>Permanent Redirect</a>
      <a href='https://redirect2.com'>Temporary Redirect</a>
      <a href='https://redirect3.com'>Multiple Redirects</a>";

    // Setup mock responses for redirects
    SetupMockResponse("https://redirect1.com/", HttpStatusCode.MovedPermanently);
    SetupMockResponse("https://redirect2.com/", HttpStatusCode.Found);
    SetupMockResponse("https://redirect3.com/", HttpStatusCode.TemporaryRedirect);

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Equal(3, result.Count);
    Assert.All(result, r => Assert.False(r.IsValid));
  }

  [Fact]
  public async Task CheckLinks_MalformedHtml_ReturnsCorrectResults()
  {
    // Arrange
    var malformedHtml = @"
      <a href='https://valid1.com'>Valid 1</a>
      <a href='https://valid2.com' >Valid 2</a>
      <a href='https://valid3.com' target='_blank'>Valid 3</a>
      <a href='https://valid4.com' rel='noopener'>Valid 4</a>
      <a href='https://valid5.com' class='link'>Valid 5</a>
      <a href='https://valid6.com' data-test='test'>Valid 6</a>
      <a href='https://valid7.com' style='color: blue'>Valid 7</a>
      <a href='https://valid8.com' onclick='alert()'>Valid 8</a>";

    // Setup mock responses for all valid links
    for (int i = 1; i <= 8; i++)
    {
      SetupMockResponse($"https://valid{i}.com/", HttpStatusCode.OK);
    }

    // Act
    var result = await _service.CheckLinks(malformedHtml);

    // Assert
    Assert.Equal(8, result.Count);
    Assert.All(result, r => Assert.True(r.IsValid));
  }

  [Fact]
  public async Task CheckLinks_LargeNumberOfLinks_ProcessesConcurrently()
  {
    // Arrange
    var links = Enumerable.Range(0, 150)
      .Select(i => $"<a href='https://link{i}.com/'>Link {i}</a>");
    var html = string.Join("\n", links);

    // Setup mock responses for all links
    for (int i = 0; i < 150; i++)
    {
      SetupMockResponse($"https://link{i}.com/", HttpStatusCode.OK);
    }

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Equal(150, result.Count);
    Assert.All(result, r => Assert.True(r.IsValid));
  }

  [Fact]
  public async Task CheckLinks_ComplexUrls_ReturnsCorrectResults()
  {
    // Arrange
    var html = @"
      <a href='https://example.com/path?param=value'>Query Param</a>
      <a href='https://example.com/path#section'>Fragment</a>
      <a href='https://example.com:8080/path'>Port</a>
      <a href='https://example.com/path%20with%20spaces'>Encoded</a>
      <a href='https://example.com/path?param=value&param2=value2'>Multiple Params</a>";

    // Setup mock responses
    SetupMockResponse("https://example.com/path?param=value", HttpStatusCode.OK);
    SetupMockResponse("https://example.com/path#section", HttpStatusCode.OK);
    SetupMockResponse("https://example.com:8080/path", HttpStatusCode.OK);
    SetupMockResponse("https://example.com/path with spaces", HttpStatusCode.OK);
    SetupMockResponse("https://example.com/path?param=value&param2=value2", HttpStatusCode.OK);

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Equal(5, result.Count);
    Assert.All(result, r => Assert.True(r.IsValid));
  }

  [Fact]
  public async Task CheckLinks_ErrorResponses_ReturnsCorrectResults()
  {
    // Arrange
    var html = @"
      <a href='https://timeout.com'>Timeout</a>
      <a href='https://servererror.com'>Server Error</a>
      <a href='https://ratelimit.com'>Rate Limit</a>";

    // Setup mock responses for different error scenarios
    SetupMockResponse("https://timeout.com/", HttpStatusCode.RequestTimeout);
    SetupMockResponse("https://servererror.com/", HttpStatusCode.InternalServerError);
    SetupMockResponse("https://ratelimit.com/", (HttpStatusCode)429); // Too Many Requests

    // Act
    var result = await _service.CheckLinks(html);

    // Assert
    Assert.Equal(3, result.Count);
    Assert.All(result, r => Assert.False(r.IsValid));
  }

  private void SetupMockResponse(string url, HttpStatusCode statusCode)
  {
    _mockHttpMessageHandler
      .Protected()
      .Setup<Task<HttpResponseMessage>>(
        "SendAsync",
        ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString() == url),
        ItExpr.IsAny<CancellationToken>()
      )
      .ReturnsAsync(new HttpResponseMessage(statusCode))
      .Verifiable();
  }
}
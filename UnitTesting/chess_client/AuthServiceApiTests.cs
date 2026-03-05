using System.Net;
using System.Text.Json;
using chess_client.Services;
using Shared;
using Shared.Dtos;
using UnitTesting.Mock;

namespace UnitTesting.chess_client;

/// <summary>
/// Tests for the AuthService API calls to verify that the correct HTTP requests
/// are sent to the server with the expected URL, method, headers, and body content.
/// </summary>
[TestClass]
public class AuthServiceApiTests
{
    private MockHttpMessageHandler _mockHandler = null!;
    private HttpClient _httpClient = null!;
    private AuthService _authService = null!;
    private readonly JsonParser _jsonParser = new();

    [TestInitialize]
    public void Setup()
    {
        _mockHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHandler);
        _authService = new AuthService(_httpClient);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _httpClient.Dispose();
        _mockHandler.Dispose();
    }

    // =====================================================================
    // Login Tests
    // =====================================================================

    [TestMethod]
    public async Task Login_SendsPostRequest_ToCorrectUrl()
    {
        var expectedUserId = Guid.NewGuid();
        var responseJson = JsonSerializer.Serialize(new { userId = expectedUserId });
        _mockHandler.EnqueueResponse(HttpStatusCode.OK, responseJson);

        await _authService.Login("user", "password");

        Assert.AreEqual(1, _mockHandler.RecordedRequests.Count);
        var request = _mockHandler.GetRequest(0);
        Assert.AreEqual(HttpMethod.Post, request.Method);
        Assert.AreEqual("http://localhost:8080/api/user/login", request.RequestUri?.ToString());
    }

    [TestMethod]
    public async Task Login_SendsCorrectJsonBody_WithUsernameAndPassword()
    {
        var expectedUserId = Guid.NewGuid();
        var responseJson = JsonSerializer.Serialize(new { userId = expectedUserId });
        _mockHandler.EnqueueResponse(HttpStatusCode.OK, responseJson);

        await _authService.Login("user", "password");

        var request = _mockHandler.GetRequest(0);
        Assert.IsNotNull(request.Body);

        var sentDto = _jsonParser.DeserializeJson<UserDto>(request.Body);
        Assert.IsNotNull(sentDto);
        Assert.AreEqual("user", sentDto.Username);
        Assert.AreEqual("password", sentDto.Password);
    }

    [TestMethod]
    public async Task Login_SendsJsonContentType()
    {
        var responseJson = JsonSerializer.Serialize(new { userId = Guid.NewGuid() });
        _mockHandler.EnqueueResponse(HttpStatusCode.OK, responseJson);

        await _authService.Login("user", "password");

        var request = _mockHandler.GetRequest(0);
        Assert.AreEqual("application/json", request.ContentType);
    }

    [TestMethod]
    public async Task Login_ReturnsUserId_OnSuccess()
    {
        var expectedUserId = Guid.NewGuid();
        var responseJson = JsonSerializer.Serialize(new { userId = expectedUserId });
        _mockHandler.EnqueueResponse(HttpStatusCode.OK, responseJson);

        var result = await _authService.Login("user", "password");

        Assert.AreEqual(expectedUserId, result);
    }

    [TestMethod]
    public async Task Login_ThrowsException_OnServerError()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.InternalServerError);

        await Assert.ThrowsExactlyAsync<Exception>(() => _authService.Login("user", "password"));
    }

    [TestMethod]
    public async Task Login_ThrowsException_OnUnauthorized()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.Unauthorized);

        await Assert.ThrowsExactlyAsync<Exception>(() => _authService.Login("user", "wrongPassword"));
    }

    [TestMethod]
    public async Task Login_ThrowsException_OnInvalidResponseBody()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.OK, "not-valid-json");

        await Assert.ThrowsExactlyAsync<JsonException>(() => _authService.Login("user", "password"));
    }

    // =====================================================================
    // Register Tests
    // =====================================================================

    [TestMethod]
    public async Task Register_SendsPostRequest_ToCorrectUrl()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.OK);

        await _authService.Register("user", "password");

        Assert.AreEqual(1, _mockHandler.RecordedRequests.Count);
        var request = _mockHandler.GetRequest(0);
        Assert.AreEqual(HttpMethod.Post, request.Method);
        Assert.AreEqual("http://localhost:8080/api/user/register", request.RequestUri?.ToString());
    }

    [TestMethod]
    public async Task Register_SendsCorrectJsonBody_WithUsernameAndPassword()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.OK);

        await _authService.Register("user", "password");

        var request = _mockHandler.GetRequest(0);
        Assert.IsNotNull(request.Body);

        var sentDto = _jsonParser.DeserializeJson<UserDto>(request.Body);
        Assert.IsNotNull(sentDto);
        Assert.AreEqual("user", sentDto.Username);
        Assert.AreEqual("password", sentDto.Password);
    }

    [TestMethod]
    public async Task Register_SendsJsonContentType()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.OK);

        await _authService.Register("user", "password");

        var request = _mockHandler.GetRequest(0);
        Assert.AreEqual("application/json", request.ContentType);
    }

    [TestMethod]
    public async Task Register_CompletesSuccessfully_OnOkResponse()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.OK);

        await _authService.Register("user", "password");
        Assert.AreEqual(1, _mockHandler.RecordedRequests.Count);
    }

    [TestMethod]
    public async Task Register_ThrowsException_OnServerError()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.InternalServerError);

        await Assert.ThrowsExactlyAsync<Exception>(() => _authService.Register("user", "password"));
    }

    [TestMethod]
    public async Task Register_ThrowsException_OnConflict()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.Conflict);

        await Assert.ThrowsExactlyAsync<Exception>(() => _authService.Register("existingUser", "password"));
    }

    // =====================================================================
    // MockHttpMessageHandler Tests
    // =====================================================================

    [TestMethod]
    public async Task MockHandler_RecordsMultipleRequests()
    {
        var responseJson = JsonSerializer.Serialize(new { userId = Guid.NewGuid() });
        _mockHandler.EnqueueResponse(HttpStatusCode.OK, responseJson);
        _mockHandler.EnqueueResponse(HttpStatusCode.OK);

        await _authService.Login("user1", "pass1");
        await _authService.Register("user2", "pass2");

        Assert.AreEqual(2, _mockHandler.RecordedRequests.Count);
        Assert.AreEqual("http://localhost:8080/api/user/login", _mockHandler.GetRequest(0).RequestUri?.ToString());
        Assert.AreEqual("http://localhost:8080/api/user/register", _mockHandler.GetRequest(1).RequestUri?.ToString());
    }

    [TestMethod]
    public void MockHandler_Clear_ClearsRecordedRequests()
    {
        _mockHandler.EnqueueResponse(HttpStatusCode.OK);

        _mockHandler.Clear();

        Assert.IsEmpty(_mockHandler.RecordedRequests);
    }

    [TestMethod]
    public async Task MockHandler_ThrowsException_WhenNoResponseEnqueued()
    {
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _httpClient.GetAsync("http://localhost:8080/api/test"));
    }
}
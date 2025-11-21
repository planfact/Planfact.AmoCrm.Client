using System.Net;
using System.Text;
using System.Text.Json;

using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Exceptions;
using Planfact.AmoCrm.Client.Leads;

namespace Planfact.AmoCrm.Client.Tests;

public class AmoCrmHttpResponseHandlerTests
{
    private readonly AmoCrmHttpResponseHandler _handler;
    private readonly Mock<ILogger<AmoCrmHttpResponseHandler>> _loggerMock;

    public AmoCrmHttpResponseHandlerTests()
    {
        _loggerMock = new Mock<ILogger<AmoCrmHttpResponseHandler>>();
        _handler = new AmoCrmHttpResponseHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_SuccessfulResponse_ReturnsEntitiesResponse()
    {
        // Arrange
        var expectedResponse = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = [new Lead { Id = 1, Name = "Lead 1" }] }
        };

        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
        };

        // Act
        EntitiesResponse result = await _handler.HandleAsync<EntitiesResponse>(httpResponse);

        // Assert
        result.Should().NotBeNull();
        result.Embedded.Should().NotBeNull();
        result.Embedded?.Leads.Should().NotBeNull().And.HaveCount(1);
        result.Embedded?.Leads?[0].Id.Should().Be(1);
        result.Embedded?.Leads?[0].Name.Should().Be("Lead 1");
    }

    [Fact]
    public async Task HandleAsync_UnauthorizedHttpStatus_ThrowsAmoCrmAuthorizationException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("Unauthorized", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await FluentActions
            .Invoking(async () => await _handler.HandleAsync<EntitiesResponse>(httpResponse).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>()
            .WithMessage("*Проверьте корректность заполнения и срок действия Access Token.*");
    }

    [Fact]
    public async Task HandleAsync_ForbiddenHttpStatus_ThrowsAmoCrmAuthorizationException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent("Forbidden", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await FluentActions
            .Invoking(async () => await _handler.HandleAsync<EntitiesResponse>(httpResponse).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>()
            .WithMessage("*Доступ запрещен*");
    }

    [Fact]
    public async Task HandleAsync_BadRequestHttpStatus_ThrowsAmoCrmValidationException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Bad Request", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await FluentActions
            .Invoking(async () => await _handler.HandleAsync<EntitiesResponse>(httpResponse).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>()
            .WithMessage("*Ошибка валидации*");
    }

    [Fact]
    public async Task HandleAsync_NoContentHttpStatus_ReturnsDefault()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        EntitiesResponse result = await _handler.HandleAsync<EntitiesResponse>(httpResponse);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_EmptyResponse_ThrowsAmoCrmHttpException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await FluentActions
            .Invoking(async () => await _handler.HandleAsync<EntitiesResponse>(httpResponse).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>()
            .WithMessage("*пустой ответ*");
    }

    [Fact]
    public async Task HandleAsync_InvalidJson_ThrowsAmoCrmHttpException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid json", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await FluentActions
            .Invoking(async () => await _handler.HandleAsync<EntitiesResponse>(httpResponse).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>()
            .WithMessage("*обработке ответа*");
    }

    [Fact]
    public async Task HandleAsync_NullResponse_ThrowsAmoCrmHttpException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await FluentActions
            .Invoking(async () => await _handler.HandleAsync<EntitiesResponse>(httpResponse).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>()
            .WithMessage("*null ответ*");
    }
}

using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Moq;

using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Exceptions;
using Planfact.AmoCrm.Client.Leads;

using Task = System.Threading.Tasks.Task;

namespace Planfact.AmoCrm.Client.Tests;

public class AmoCrmHttpResponseHandlerTests
{
    private AmoCrmHttpResponseHandler _handler;
    private Mock<ILogger<AmoCrmHttpResponseHandler>> _loggerMock;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<AmoCrmHttpResponseHandler>>();
        _handler = new AmoCrmHttpResponseHandler(_loggerMock.Object);
    }

    [Test]
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
        var result = await _handler.HandleAsync<EntitiesResponse>(httpResponse);

        // Assert

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Embedded!.Leads!.Count, Is.EqualTo(1));
            Assert.That(result.Embedded!.Leads![0].Id, Is.EqualTo(1));
            Assert.That(result.Embedded!.Leads[0].Name, Is.EqualTo("Lead 1"));
        });
    }

    [Test]
    public async Task HandleAsync_UnauthorizedHttpStatus_ThrowsAmoCrmAuthenticationException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("Unauthorized", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.HandleAsync<EntitiesResponse>(httpResponse))
            .Should().ThrowAsync<AmoCrmAuthenticationException>()
            .WithMessage("*Проверьте корректность заполнения и срок действия Access Token.*");
    }

    [Test]
    public async Task HandleAsync_ForbiddenHttpStatus_ThrowsAmoCrmAuthenticationException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent("Forbidden", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.HandleAsync<EntitiesResponse>(httpResponse))
            .Should().ThrowAsync<AmoCrmAuthenticationException>()
            .WithMessage("*Доступ запрещен*");
    }

    [Test]
    public async Task HandleAsync_BadRequestHttpStatus_ThrowsAmoCrmValidationException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Bad Request", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.HandleAsync<EntitiesResponse>(httpResponse))
            .Should().ThrowAsync<AmoCrmValidationException>()
            .WithMessage("*Ошибка валидации*");
    }

    [Test]
    public async Task HandleAsync_NoContentHttpStatus_ReturnsDefault()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        EntitiesResponse result = await _handler.HandleAsync<EntitiesResponse>(httpResponse);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task HandleAsync_EmptyResponse_ThrowsAmoCrmHttpException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.HandleAsync<EntitiesResponse>(httpResponse))
            .Should().ThrowAsync<AmoCrmHttpException>()
            .WithMessage("*пустой ответ*");
    }

    [Test]
    public async Task HandleAsync_InvalidJson_ThrowsAmoCrmHttpException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid json", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.HandleAsync<EntitiesResponse>(httpResponse))
            .Should().ThrowAsync<AmoCrmHttpException>()
            .WithMessage("*обработке ответа*");
    }

    [Test]
    public async Task HandleAsync_NullResponse_ThrowsAmoCrmHttpException()
    {
        // Arrange
        using var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await FluentActions.Invoking(() => _handler.HandleAsync<EntitiesResponse>(httpResponse))
            .Should().ThrowAsync<AmoCrmHttpException>()
            .WithMessage("*null ответ*");
    }
}

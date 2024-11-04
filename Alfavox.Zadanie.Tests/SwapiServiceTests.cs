using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;

namespace Alfavox.Zadanie.Tests;

public class SwapiServiceTests
{
    [Fact]
    public async Task GetLukeSkywalkerDataAsync_ShouldReturnValidData()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"name\": \"Luke Skywalker\", " +
                "\"films\": [\"Film1\"], " +
                "\"vehicles\": [\"Vehicle1\"], " +
                "\"starships\": [\"Starship1\"] }")
            });

        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new SwapiService(httpClient);

        // Act
        var data = await service.GetLukeSkywalkerDataAsync();

        // Assert
        data.Should().NotBeNull();
        data.Name.Should().Be("Luke Skywalker");
        data.Films.Should().HaveCount(1);
        data.Vehicles.Should().HaveCount(1);
        data.Starships.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetNameFromUrlAsync_ShouldReturnCorrectName()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == "https://swapi.py4e.com/api/films/1/"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"title\": \"A New Hope\"}")
            });

        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new SwapiService(httpClient);

        // Act
        var result = await service.GetNameFromUrlAsync("https://swapi.py4e.com/api/films/1/");

        // Assert
        result.Should().Be("A New Hope");
    }

    [Fact]
    public async Task GetNamesFromUrlsAsync_ShouldReturnListOfNames()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == "https://swapi.py4e.com/api/films/1/"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"title\": \"A New Hope\"}")
            });

        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == "https://swapi.py4e.com/api/films/2/"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"title\": \"The Empire Strikes Back\"}")
            });

        var httpClient = new HttpClient(mockHttpHandler.Object);
        var service = new SwapiService(httpClient);
        var urls = new List<string>
        {
            "https://swapi.py4e.com/api/films/1/",
            "https://swapi.py4e.com/api/films/2/"
        };

        // Act
        var result = await service.GetNamesFromUrlsAsync(urls);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("A New Hope");
        result.Should().Contain("The Empire Strikes Back");
    }
}

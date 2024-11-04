using FluentAssertions;
using Moq;
using Newtonsoft.Json;

namespace Alfavox.Refactor.Tests;

public class SwapiServiceTests
{
    private readonly Mock<IHttpClientService> _mockHttpClientService;
    private readonly SwapiService _swapiService;

    public SwapiServiceTests()
    {
        _mockHttpClientService = new Mock<IHttpClientService>();
        _swapiService = new SwapiService(_mockHttpClientService.Object);
    }

    [Fact]
    public async Task GetLukeSkywalkerDataAsync_ShouldReturnCorrectData_WhenApiResponseIsValid()
    {
        // Arrange
        var lukeApiResponse = new
        {
            name = "Luke Skywalker",
            films = new List<string>
                {
                    "https://swapi.py4e.com/api/films/1/",
                    "https://swapi.py4e.com/api/films/2/"
                },
            vehicles = new List<string>
                {
                    "https://swapi.py4e.com/api/vehicles/14/"
                },
            starships = new List<string>
                {
                    "https://swapi.py4e.com/api/starships/12/"
                }
        };

        var filmResponse1 = new { title = "A New Hope" };
        var filmResponse2 = new { title = "The Empire Strikes Back" };
        var vehicleResponse = new { name = "Snowspeeder" };
        var starshipResponse = new { name = "X-wing" };

        _mockHttpClientService.SetupSequence(x => x.GetStringAsync(It.IsAny<string>()))
           .ReturnsAsync(JsonConvert.SerializeObject(lukeApiResponse))
            .ReturnsAsync(JsonConvert.SerializeObject(filmResponse1))
            .ReturnsAsync(JsonConvert.SerializeObject(filmResponse2))
            .ReturnsAsync(JsonConvert.SerializeObject(vehicleResponse))
            .ReturnsAsync(JsonConvert.SerializeObject(starshipResponse));


        // Act
        var result = await _swapiService.GetLukeSkywalkerDataAsync();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Luke Skywalker");
        result.FilmNames.Should().BeEquivalentTo(new List<string> { "A New Hope", "The Empire Strikes Back" });
        result.VehicleNames.Should().BeEquivalentTo(new List<string> { "Snowspeeder" });
        result.StarshipNames.Should().BeEquivalentTo(new List<string> { "X-wing" });
    }

    [Fact]
    public async Task GetLukeSkywalkerDataAsync_ShouldThrowException_WhenApiResponseFails()
    {
        // Arrange
        _mockHttpClientService.Setup(x => x.GetStringAsync(It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("API error"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _swapiService.GetLukeSkywalkerDataAsync());
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using Xunit;

namespace RaceDay.Api.Tests;

public class EventsControllerTests
{
    private static RaceDayContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RaceDayContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new RaceDayContext(options);
    }

    [Fact]
    public async Task GetEvents_ReturnsOk()
    {
        using var context = CreateContext();
        var controller = new EventsController(context);

        var result = await controller.GetEvents();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetEvent_WhenEventDoesNotExist_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new EventsController(context);

        var result = await controller.GetEvent(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}

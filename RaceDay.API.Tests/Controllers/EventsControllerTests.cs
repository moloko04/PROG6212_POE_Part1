using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Tests.Helpers;
using System.Security.Claims;

namespace RaceDay.Api.Tests.Controllers;

public class EventsControllerTests
{
    private static EventsController CreateControllerWithUser(AppDbContext context, int userId, string role)
    {
        var controller = new EventsController(context);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        }, "TestAuth"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        return controller;
    }

    [Fact]
    public async Task GetAllEvents_ReturnsOkWithList()
    {
        var context = TestDbContextFactory.Create();
        context.Events.Add(new Event
        {
            EventName = "Soweto Marathon",
            EventDate = new DateTime(2026, 12, 15),
            Location = "Soweto",
            Description = "Annual marathon",
            Status = "Upcoming",
            OrganiserID = 1,
            CreatedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 1, "Organiser");
        var result = await controller.GetAllEvents();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task CreateEvent_AsOrganiser_ReturnsOk()
    {
        var context = TestDbContextFactory.Create();
        var controller = CreateControllerWithUser(context, 1, "Organiser");

        var newEvent = new Event
        {
            EventName = "Cape Town Cycle Tour",
            EventDate = new DateTime(2026, 3, 8),
            Location = "Cape Town",
            Description = "Cycle tour",
            Status = "Upcoming",
            OrganiserID = 1,
            CreatedDate = DateTime.Now
        };

        var result = await controller.CreateEvent(newEvent);

        result.Should().BeOfType<OkObjectResult>();
        context.Events.Count().Should().Be(1);
    }

    [Fact]
    public async Task DeleteEvent_AsNonOwnerOrganiser_ReturnsForbid()
    {
        var context = TestDbContextFactory.Create();
        context.Events.Add(new Event
        {
            EventID = 1,
            EventName = "Soweto Marathon",
            EventDate = new DateTime(2026, 12, 15),
            Location = "Soweto",
            Description = "Annual marathon",
            Status = "Upcoming",
            OrganiserID = 1,
            CreatedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 2, "Organiser");
        var result = await controller.DeleteEvent(1);

        result.Should().BeOfType<ForbidResult>();
    }
}
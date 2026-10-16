using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Tests.Helpers;
using System.Security.Claims;

namespace RaceDay.Api.Tests.Controllers;

public class EnrolmentsControllerTests
{
    private static EnrolmentsController CreateControllerWithUser(AppDbContext context, int userId, string role)
    {
        var controller = new EnrolmentsController(context);
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

    private static void SeedData(AppDbContext context)
    {
        context.Users.Add(new User
        {
            UserID = 1,
            Username = "moloko_organiser",
            FullName = "Moloko Mothemela",
            PasswordHash = "hash",
            Email = "moloko@raceday.com",
            Role = "Organiser",
            CreatedDate = DateTime.Now
        });
        context.Users.Add(new User
        {
            UserID = 2,
            Username = "sharon_participant",
            FullName = "Sharon Matlou",
            PasswordHash = "hash",
            Email = "sharon@email.com",
            Role = "Participant",
            CreatedDate = DateTime.Now
        });
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
    }

    [Fact]
    public async Task EnrolInEvent_AsParticipant_ReturnsOk()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 2, "Participant");
        var enrolment = new Enrolment { EventID = 1 };

        var result = await controller.EnrolInEvent(1, enrolment);

        result.Should().BeOfType<OkObjectResult>();
        context.Enrolments.Count().Should().Be(1);
    }

    [Fact]
    public async Task EnrolInEvent_WhenAlreadyEnrolled_ReturnsBadRequest()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        context.Enrolments.Add(new Enrolment
        {
            EnrolmentID = 1,
            EventID = 1,
            ParticipantID = 2,
            EnrolmentDate = DateTime.Now,
            Status = "Confirmed"
        });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 2, "Participant");
        var enrolment = new Enrolment { EventID = 1 };

        var result = await controller.EnrolInEvent(1, enrolment);

        result.Should().BeOfType<BadRequestObjectResult>();
        context.Enrolments.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetEnrolmentsForEvent_AsOwnerOrganiser_ReturnsOk()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 1, "Organiser");

        var result = await controller.GetEnrolmentsForEvent(1);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetEnrolmentsForEvent_AsNonOwnerOrganiser_ReturnsForbid()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        context.Users.Add(new User
        {
            UserID = 3,
            Username = "another_organiser",
            FullName = "Another Organiser",
            PasswordHash = "hash",
            Email = "another@raceday.com",
            Role = "Organiser",
            CreatedDate = DateTime.Now
        });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 3, "Organiser");
        var result = await controller.GetEnrolmentsForEvent(1);

        result.Should().BeOfType<ForbidResult>();
    }
}
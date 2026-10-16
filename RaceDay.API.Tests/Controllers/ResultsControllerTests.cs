using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Tests.Helpers;
using System.Security.Claims;

namespace RaceDay.Api.Tests.Controllers;

public class ResultsControllerTests
{
    private static ResultsController CreateControllerWithUser(AppDbContext context, int userId, string role)
    {
        var controller = new ResultsController(context);
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
            Status = "Completed",
            OrganiserID = 1,
            CreatedDate = DateTime.Now
        });
        context.Enrolments.Add(new Enrolment
        {
            EnrolmentID = 1,
            EventID = 1,
            ParticipantID = 2,
            EnrolmentDate = DateTime.Now,
            Status = "Confirmed"
        });
    }

    [Fact]
    public async Task AddResult_AsOwnerOrganiser_ReturnsOk()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 1, "Organiser");
        var result = new Result
        {
            FinishTime = TimeSpan.FromHours(2.5),
            Position = 5,
            PointsEarned = 95,
            IsDisqualified = false
        };

        var response = await controller.AddResult(1, result);

        response.Should().BeOfType<OkObjectResult>();
        context.Results.Count().Should().Be(1);
    }

    [Fact]
    public async Task AddResult_AsNonOwnerOrganiser_ReturnsForbid()
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
        var result = new Result
        {
            FinishTime = TimeSpan.FromHours(2.5),
            Position = 5,
            PointsEarned = 95,
            IsDisqualified = false
        };

        var response = await controller.AddResult(1, result);

        response.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task AddResult_WhenResultAlreadyExists_ReturnsBadRequest()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        context.Results.Add(new Result
        {
            ResultID = 1,
            EnrolmentID = 1,
            FinishTime = TimeSpan.FromHours(2.5),
            Position = 5,
            PointsEarned = 95,
            IsDisqualified = false
        });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 1, "Organiser");
        var result = new Result
        {
            FinishTime = TimeSpan.FromHours(3),
            Position = 6,
            PointsEarned = 90,
            IsDisqualified = false
        };

        var response = await controller.AddResult(1, result);

        response.Should().BeOfType<BadRequestObjectResult>();
        context.Results.Count().Should().Be(1);
    }
}
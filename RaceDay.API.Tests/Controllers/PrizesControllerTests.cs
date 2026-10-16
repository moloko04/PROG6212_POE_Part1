using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Tests.Helpers;
using System.Security.Claims;

namespace RaceDay.Api.Tests.Controllers;

public class PrizesControllerTests
{
    private static PrizesController CreateControllerWithUser(AppDbContext context, int userId, string role)
    {
        var controller = new PrizesController(context);
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
        context.Categories.Add(new Category
        {
            CategoryID = 1,
            CategoryName = "Full Marathon",
            Description = "42km",
            EventID = 1
        });
    }

    [Fact]
    public async Task AddPrize_AsOwnerOrganiser_ReturnsOk()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 1, "Organiser");
        var prize = new Prize
        {
            EventID = 1,
            CategoryID = 1,
            PrizeName = "Gold Medal",
            PrizeValue = 5000,
            PositionAwarded = 1
        };

        var result = await controller.AddPrize(1, prize);

        result.Should().BeOfType<OkObjectResult>();
        context.Prizes.Count().Should().Be(1);
    }

    [Fact]
    public async Task AddPrize_WithInvalidCategory_ReturnsBadRequest()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 1, "Organiser");
        var prize = new Prize
        {
            EventID = 1,
            CategoryID = 99, 
            PrizeName = "Gold Medal",
            PrizeValue = 5000,
            PositionAwarded = 1
        };

        var result = await controller.AddPrize(1, prize);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetPrizesForEvent_ReturnsOkWithList()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        context.Prizes.Add(new Prize
        {
            PrizeID = 1,
            EventID = 1,
            CategoryID = 1,
            PrizeName = "Gold Medal",
            PrizeValue = 5000,
            PositionAwarded = 1
        });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 1, "Organiser");
        var result = await controller.GetPrizesForEvent(1);

        result.Should().BeOfType<OkObjectResult>();
    }
}
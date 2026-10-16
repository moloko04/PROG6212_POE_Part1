using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Tests.Helpers;
using System.Security.Claims;

namespace RaceDay.Api.Tests.Controllers;

public class CategoriesControllerTests
{
    private static CategoriesController CreateControllerWithUser(AppDbContext context, int userId, string role)
    {
        var controller = new CategoriesController(context);
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
    }

    [Fact]
    public async Task AddCategory_AsOwnerOrganiser_ReturnsOk()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 1, "Organiser");
        var category = new Category
        {
            CategoryName = "Full Marathon (42km)",
            Description = "Standard full marathon distance",
            EventID = 1
        };

        var result = await controller.AddCategory(1, category);

        result.Should().BeOfType<OkObjectResult>();
        context.Categories.Count().Should().Be(1);
    }

    [Fact]
    public async Task AddCategory_AsNonOwnerOrganiser_ReturnsForbid()
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
        var category = new Category
        {
            CategoryName = "Half Marathon (21km)",
            Description = "Half marathon distance",
            EventID = 1
        };

        var result = await controller.AddCategory(1, category);

        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task GetCategoriesForEvent_ReturnsOkWithList()
    {
        var context = TestDbContextFactory.Create();
        SeedData(context);
        context.Categories.Add(new Category
        {
            CategoryID = 1,
            CategoryName = "Full Marathon (42km)",
            Description = "Standard full marathon distance",
            EventID = 1
        });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithUser(context, 1, "Organiser");
        var result = await controller.GetCategoriesForEvent(1);

        result.Should().BeOfType<OkObjectResult>();
    }
}
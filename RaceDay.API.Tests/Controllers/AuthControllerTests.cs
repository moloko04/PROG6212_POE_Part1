using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.Models;
using RaceDay.Api.Tests.Helpers;


namespace RaceDay.Api.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        var context = TestDbContextFactory.Create();
        var config = TestConfig.GetConfiguration();
        var controller = new AuthController(context, config);

        var newUser = new User
        {
            Username = "moloko_organiser",
            PasswordHash = "mothemela123",
            FullName = "Moloko Mothemela",
            Email = "moloko@raceday.com",
            Role = "Organiser"
        };

        var result = await controller.Register(newUser);

        result.Should().BeOfType<OkObjectResult>();
        context.Users.Count().Should().Be(1);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
    {
        var context = TestDbContextFactory.Create();
        var config = TestConfig.GetConfiguration();
        var controller = new AuthController(context, config);

        var user1 = new User
        {
            Username = "moloko_organiser",
            PasswordHash = "password123",
            FullName = "Moloko Mothemela",
            Email = "moloko@raceday.com",
            Role = "Organiser"
        };
        await controller.Register(user1);

        var user2 = new User
        {
            Username = "moloko_organiser", 
            PasswordHash = "password456",
            FullName = "Moloko Mothemela",
            Email = "mlk@raceday.com", 
            Role = "Organiser"
        };

        var result = await controller.Register(user2);

        result.Should().BeOfType<BadRequestObjectResult>();
        context.Users.Count().Should().Be(1);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var context = TestDbContextFactory.Create();
        var config = TestConfig.GetConfiguration();
        var controller = new AuthController(context, config);

        var newUser = new User
        {
            Username = "sharon_participant",
            PasswordHash = "matlou123",
            FullName = "Sharon Matlou",
            Email = "sharon@email.com",
            Role = "Participant"
        };
        await controller.Register(newUser);

        var loginRequest = new LoginRequest
        {
            Username = "sharon_participant",
            Password = "matlou123"
        };

        var result = await controller.Login(loginRequest);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var context = TestDbContextFactory.Create();
        var config = TestConfig.GetConfiguration();
        var controller = new AuthController(context, config);

        var newUser = new User
        {
            Username = "sharon_participant",
            PasswordHash = "matlou123",
            FullName = "Sharon Matlou",
            Email = "sharon@email.com",
            Role = "Participant"
        };
        await controller.Register(newUser);

        var loginRequest = new LoginRequest
        {
            Username = "sharon_participant",
            Password = "wrongpassword"
        };

        var result = await controller.Login(loginRequest);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
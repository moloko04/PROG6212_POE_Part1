using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Text;
using System.Text.Json;

namespace RaceDay.Web.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("RaceDayApi");

        var response = await client.PostAsJsonAsync("api/auth/login", new
        {
            username = model.Username,
            password = model.Password
        });

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(content).RootElement;

            HttpContext.Session.SetString("JwtToken", data.GetProperty("token").GetString()!);
            HttpContext.Session.SetString("UserRole", data.GetProperty("role").GetString()!);
            HttpContext.Session.SetString("Username", data.GetProperty("username").GetString()!);

            if (data.GetProperty("role").GetString() == "Organiser")
                return RedirectToAction("Index", "Organiser");
            else
                return RedirectToAction("Index", "Participant");
        }

        ModelState.AddModelError("", "Invalid username or password");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("RaceDayApi");

        var response = await client.PostAsJsonAsync("api/auth/register", new
        {
            username = model.Username,
            passwordHash = model.Password, 
            fullName = model.FullName,
            email = model.Email,
            role = model.Role
        });

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "Registration failed. Username or email may already exist.");
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
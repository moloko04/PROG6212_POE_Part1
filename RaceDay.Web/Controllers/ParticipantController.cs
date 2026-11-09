using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RaceDay.Web.Controllers;

public class ParticipantController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ParticipantController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

   
    private HttpClient GetAuthenticatedClient()
    {
        var client = _httpClientFactory.CreateClient("RaceDayApi");
        var token = HttpContext.Session.GetString("JwtToken");
        if (token != null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserRole") != "Participant")
            return RedirectToAction("Login", "Account");

        var client = GetAuthenticatedClient();
        var response = await client.GetAsync("api/events");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<List<EventViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(events);
        }

        ViewBag.Error = $"Could not load events: {response.StatusCode}";
        return View(new List<EventViewModel>());
    }

    public async Task<IActionResult> Details(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "Participant")
            return RedirectToAction("Login", "Account");

        var client = GetAuthenticatedClient();

        try
        {
            var response = await client.GetAsync($"api/events/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ev = JsonSerializer.Deserialize<EventViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(ev);
            }
            else
            {
                ViewBag.Error = $"API Error: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Exception: {ex.Message}";
        }

        return View(new EventViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Participant")
            return RedirectToAction("Login", "Account");

        var client = GetAuthenticatedClient();

        var response = await client.PostAsJsonAsync($"api/events/{eventId}/enrol", new { EventID = eventId });

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Successfully enrolled!";
        }
        else
        {
            TempData["Error"] = "You are already enrolled or an error occurred.";
        }

        return RedirectToAction("Details", new { id = eventId });
    }

    public async Task<IActionResult> MyEnrolments()
    {
        if (HttpContext.Session.GetString("UserRole") != "Participant")
            return RedirectToAction("Login", "Account");

        var client = GetAuthenticatedClient();
        var response = await client.GetAsync("api/users/me/enrolments");

        var enrolments = new List<EnrolmentViewModel>();

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            enrolments = JsonSerializer.Deserialize<List<EnrolmentViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        else
        {
            ViewBag.Error = $"API Error: {response.StatusCode}";
        }

        return View(enrolments);
    }
}


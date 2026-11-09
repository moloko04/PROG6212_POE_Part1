using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Text;
using System.Text.Json;

namespace RaceDay.Web.Controllers;

public class OrganiserController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OrganiserController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient GetAuthenticatedClient()
    {
        var client = _httpClientFactory.CreateClient("RaceDayApi");
        var token = HttpContext.Session.GetString("JwtToken");
        if (token != null)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Login", "Account");

        var client = GetAuthenticatedClient();
        var response = await client.GetAsync("api/events");

        var events = new List<EventViewModel>();
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            events = JsonSerializer.Deserialize<List<EventViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        }

        return View(events);
    }

    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(EventViewModel model, IFormFile imageFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = GetAuthenticatedClient();

        var response = await client.PostAsJsonAsync("api/events", new
        {
            eventName = model.EventName,
            eventDate = model.EventDate,
            location = model.Location,
            description = model.Description,
            status = "Upcoming",
            organiserId = 1
        });

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var json = System.Text.Json.JsonDocument.Parse(content);
            var newEventId = json.RootElement.GetProperty("eventId").GetInt32();

            if (imageFile != null && imageFile.Length > 0)
            {
                using var contentStream = new MultipartFormDataContent();
                var fileContent = new StreamContent(imageFile.OpenReadStream());
                contentStream.Add(fileContent, "file", imageFile.FileName);

                var uploadResponse = await client.PostAsync($"api/events/{newEventId}/upload-image", contentStream);

                if (!uploadResponse.IsSuccessStatusCode)
                {
                    var uploadError = await uploadResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Event created, but image failed: {uploadError}");
                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to create event. API says: {errorContent}");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(int eventId, string categoryName, string description)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser") return RedirectToAction("Login", "Account");

        var client = GetAuthenticatedClient();
        var response = await client.PostAsJsonAsync($"api/events/{eventId}/categories", new
        {
            categoryName = categoryName,
            description = description,
            eventId = eventId
        });

        return RedirectToAction("ManageCategories", new { id = eventId });
    }

    public async Task<IActionResult> Enrolments(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser") return RedirectToAction("Login", "Account");

        var client = GetAuthenticatedClient();
        var response = await client.GetAsync($"api/events/{id}/enrolments");

        var enrolments = new List<EnrolmentViewModel>();
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            enrolments = JsonSerializer.Deserialize<List<EnrolmentViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        ViewBag.EventId = id;
        return View(enrolments);
    }

    [HttpPost]
    public async Task<IActionResult> CaptureResult(int enrolmentId, string finishTime, int position, int eventId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser") return RedirectToAction("Login", "Account");

        var client = GetAuthenticatedClient();

        
        TimeSpan time = TimeSpan.Parse(finishTime);

        var response = await client.PostAsJsonAsync($"api/enrolments/{enrolmentId}/result", new
        {
            finishTime = time,
            position = position,
            pointsEarned = 100 
        });

        return RedirectToAction("Enrolments", new { id = eventId });
    }
}
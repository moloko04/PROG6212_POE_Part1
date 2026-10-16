using RaceDay.Api.Models;

namespace RaceDay.Api.Models;

public class Event
{
    public int EventID { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Upcoming";
    public int OrganiserID { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public User? Organiser { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    public ICollection<Prize> Prizes { get; set; } = new List<Prize>();
}
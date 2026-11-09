namespace RaceDay.Web.Models;

public class EventViewModel
{
    public int EventID { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; } = DateTime.Now;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Upcoming";
    public string? ImageUrl {  get; set; }
    public List<CategoryViewModel> Categories { get; set; } = new();
}

public class CategoryViewModel
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; 
}

public class EnrolmentViewModel
{
    public int EnrolmentID { get; set; }
    public int EventID { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public int ParticipantID { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
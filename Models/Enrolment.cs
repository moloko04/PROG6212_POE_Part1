using RaceDay.Api.Models;

namespace RaceDay.Api.Models;

public class Enrolment
{
    public int EnrolmentID { get; set; }
    public int EventID { get; set; }
    public int ParticipantID { get; set; }
    public DateTime EnrolmentDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Pending";

    public Event? Event { get; set; }
    public User? Participant { get; set; }
    public Result? Result { get; set; }
}
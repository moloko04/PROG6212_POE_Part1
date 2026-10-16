namespace RaceDay.Api.Models;

public class Result
{
    public int ResultID { get; set; }
    public int EnrolmentID { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public int? PointsEarned { get; set; }
    public bool IsDisqualified { get; set; } = false;

    public Enrolment? Enrolment { get; set; }
}
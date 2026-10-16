namespace RaceDay.Api.Models;

public class Prize
{
    public int PrizeID { get; set; }
    public int EventID { get; set; }
    public int CategoryID { get; set; }
    public string PrizeName { get; set; } = string.Empty;
    public decimal? PrizeValue { get; set; }
    public int? PositionAwarded { get; set; }

    public Event? Event { get; set; }
    public Category? Category { get; set; }
}
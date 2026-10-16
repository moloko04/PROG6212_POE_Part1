using RaceDay.Api.Models;

namespace RaceDay.Api.Models;

public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EventID { get; set; }

    public Event? Event { get; set; }
    public ICollection<Prize> Prizes { get; set; } = new List<Prize>();
}
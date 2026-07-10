namespace AmendoasDreams.Api.Models;

public class Dream
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly DreamDate { get; set; }
    public TimeOnly? SleepTime { get; set; }
    public TimeOnly? WakeTime { get; set; }
    public string? Mood { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<Tag> Tags { get; set; } = [];
}

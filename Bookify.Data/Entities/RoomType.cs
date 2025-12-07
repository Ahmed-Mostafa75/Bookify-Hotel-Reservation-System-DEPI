namespace Bookify.Data.Entities;

public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePricePerNight { get; set; }
    public int Capacity { get; set; }
    public string? ImageUrl { get; set; }
    public string? Amenities { get; set; }
}
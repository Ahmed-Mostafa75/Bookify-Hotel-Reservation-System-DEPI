namespace Bookify.Data.Entities;

public class Booking
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal TotalCost { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Paid, Cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
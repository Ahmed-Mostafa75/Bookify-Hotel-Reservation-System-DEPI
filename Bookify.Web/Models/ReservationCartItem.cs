namespace Bookify.Web.Models;

public class ReservationCartItem
{
    public int RoomId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}

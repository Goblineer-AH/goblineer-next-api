namespace GoblineerNextApi.Models
{
    public class Auction
    {
        public int Bid { get; set; }
        public long Price { get; set; }
        public int Quantity { get; set; }
        public string TimeLeft { get; set; } = "";
    }
}
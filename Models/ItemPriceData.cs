using System.Collections.Generic;

namespace GoblineerNextApi.Models
{
    public class ItemPriceData
    {
        public Item Item { get; set; } = new Item();
        public int Quantity { get; set; }
        public double Marketvalue { get; set; }
        public List<Auction> Auctions { get; set; } = new List<Auction>();
    }
}
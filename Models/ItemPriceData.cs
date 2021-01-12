using System.Collections.Generic;

namespace GoblineerNextApi.Models
{
    public record ItemPriceData
    {
        public Item Item { get; init; } = new ();
        public int Quantity { get; init; }
        public double Marketvalue { get; init; }
        public IEnumerable<Auction> Auctions { get; init; } = new List<Auction>();
    }
}
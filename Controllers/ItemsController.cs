using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoblineerNextApi.Models;
using GoblineerNextApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GoblineerNextApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private DbService DbService;
        public ItemsController(DbService dbService)
        {
            DbService = dbService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsById(int id)
        {
            var items = await DbService.GetItemsById(id);

            return Ok(items);
        }
        
        [HttpGet]
        [Route("prices/{itemId}")]
        public async Task<ActionResult<IEnumerable<ItemPriceData>>> GetItemPricesById(int itemId, int serverId)
        {
            var items = await DbService.GetItemsById(itemId);

            List<ItemPriceData> prices = new();
            foreach(Item item in items)
            {
                try {
                    var marketvalue = await DbService.GetMarketvalueByInternalItemId(serverId, item.InternalId);
                    var auctions = await DbService.GetAuctionsByInternalItemId(serverId, item.InternalId);
                    prices.Add(new ItemPriceData {
                        Item = item,
                        Quantity = marketvalue.Quantity,
                        Marketvalue = marketvalue.Marketvalue,
                        Auctions = auctions,
                    });
                } catch(ItemNotFoundException) {
                    // Do nothing, if the item is not present on that server it will not have a price
                }
            }
            
            return Ok(prices);
        }

        [HttpGet]
        [Route("glance_price/{itemId}")]
        public async Task<ActionResult<ItemPriceData>> GetItemGlanceById(int itemId, int serverId)
        {
            // using the first item as a glance target
            var items = await DbService.GetItemsById(itemId);
            foreach(var item in items)
            {
                try {
                    (int quantity, double marketvalue) = await DbService.GetMarketvalueByInternalItemId(serverId, item.InternalId);

                    return Ok(new 
                    {
                        Quantity = quantity,
                        Marketvalue = marketvalue,
                    });
                } catch(ItemNotFoundException) {
                    // This item was not found on this server, trying the next item
                }
            }

            return BadRequest($"Item '{itemId}' not found.");
        }
    }
}

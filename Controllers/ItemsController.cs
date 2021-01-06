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
        [Route("prices/{internalItemId}")]
        public async Task<ActionResult<ItemPriceData>> GetItemPricesById(int internalItemId, int serverId)
        {
            var item = await DbService.GetItemByInternalId(internalItemId);
            (int quantity, double marketvalue) = await DbService.GetMarketvalueByInternalItemId(serverId, internalItemId);
            var auctions = await DbService.GetAuctionsByItemId(serverId, internalItemId);

            return Ok(new ItemPriceData 
            {
                Item = item,
                Quantity = quantity,
                Marketvalue = marketvalue,
                Auctions = auctions,
            });
        }

        [HttpGet]
        [Route("glance_price/{itemId}")]
        public async Task<ActionResult<ItemPriceData>> GetItemGlanceById(int itemId, int serverId)
        {
            (int quantity, double marketvalue) = await DbService.GetMarketvalueByItemId(serverId, itemId);

            return Ok(new 
            {
                Quantity = quantity,
                Marketvalue = marketvalue,
            });
        }
    }
}

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
    public class ServersController : ControllerBase
    {
        private DbService DbService;
        public ServersController(DbService dbService)
        {
            DbService = dbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetServers()
        {
            var servers = await DbService.GetServers();

            return Ok(servers);
        }
    }
}

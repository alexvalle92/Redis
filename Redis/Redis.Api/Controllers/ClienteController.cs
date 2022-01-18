using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {

        private readonly ILogger<ClienteController> _logger;
        private readonly IDistributedCache _distributedCache;

        public ClienteController(ILogger<ClienteController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            string keyCache = "keyRedis1";
            string result = string.Empty;
            DateTime dataAtual = DateTime.Now;

            var valueRedis = await _distributedCache.GetAsync(keyCache);
            if (valueRedis != null)
            {
                result = Encoding.UTF8.GetString(valueRedis);
            }
            else
            {
                await Task.Delay(2000);

                result = dataAtual.ToShortTimeString();

                DistributedCacheEntryOptions optionCache = new DistributedCacheEntryOptions().SetAbsoluteExpiration(dataAtual.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));

                valueRedis = Encoding.UTF8.GetBytes(result);

                await _distributedCache.SetAsync(keyCache, valueRedis, optionCache);
            }


            return Ok(result);
        }
    }
}
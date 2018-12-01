using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TheCountBot.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ErrorController : Controller
    {
        private readonly Settings _settings;

        public ErrorController( IOptions<Settings> settings )
        {
            _settings = settings.Value;
        }

        [HttpGet, Route("isup")]
        public string Get()
        {
            return $"[Is Debug: {_settings.IsDebug}] - OK";
        }
    }
}

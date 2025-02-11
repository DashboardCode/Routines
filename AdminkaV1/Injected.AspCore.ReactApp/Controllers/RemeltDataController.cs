using Microsoft.AspNetCore.Mvc;

namespace RemeltLevel2.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RemeltDataController : ControllerBase
    {

        private readonly ILogger<RemeltDataController> _logger;

        public RemeltDataController(ILogger<RemeltDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetRemeltData")]
        public IEnumerable<RemeltData> Get()
        {
            var startDateTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var data = new List<RemeltData>();

            for (int i = 0; i < 60; i++)
            {
                var dateTime = startDateTime.AddMinutes(i);
                data.Add(new RemeltData
                {
                    DateTime = dateTime,
                    TemperatureC = 1500 + Random.Shared.Next(0, 100),
                    Voltage = 220 + Random.Shared.Next(0, 20)
                });
            }

            return data;
        }
    }
}

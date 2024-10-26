using MicroServiceMicrocontrollerManager.Models;
using MicroServiceMicrocontrollerManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace MicroServiceMicrocontrollerManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonitorController(ILogger<MonitorController> logger, MqttService mqttService) : ControllerBase
    {

        private readonly ILogger<MonitorController> _logger = logger;
        
        [HttpPost("PumpSwitcherRequest")]
        public async Task<IActionResult> PumpSwitcher([FromBody] PumpSwitcherRequest request)
        {
            if (request.RequestId == "string")
            {
                request.RequestId = Guid.NewGuid().ToString();
            }

            var result =
                await mqttService.SendRequestAndWaitForResponse<PumpSwitcherRequest, PumpSwitcherResponse>(
                    "control/pump/", "status/pump/", request);
            if (result == null)
            {
                return NotFound("No response from MQTT.");
            }

            return Ok(result);
        }
        
        [HttpPost("GetTemperatureHumidity")]
        public async Task<IActionResult> GetSoilMoisture([FromBody] GetTemperatureHumidityRequest request)
        {
            if (request.RequestId == "string")
            {
                request.RequestId = Guid.NewGuid().ToString();
            }

            var result =
                await mqttService.SendRequestAndWaitForResponse<GetTemperatureHumidityRequest, GetTemperatureHumidityResponse>(
                    "control/dht/", "status/dht/", request);
            if (result == null)
            {
                return NotFound("No response from MQTT.");
            }

            return Ok(result);
        }
        
        [HttpPost("GetSoilMoistureRequest")]
        public async Task<IActionResult> GetSoilMoisture([FromBody] GetSoilMoistureRequest request)
        {
            if (request.RequestId == "string")
            {
                request.RequestId = Guid.NewGuid().ToString();
            }

            var result =
                await mqttService.SendRequestAndWaitForResponse<GetSoilMoistureRequest, GetSoilMoistureResponse>(
                    "control/soil-moisture/", "status/soil-moisture/", request);
            if (result == null)
            {
                return NotFound("No response from MQTT.");
            }

            return Ok(result);
        }
    }
}

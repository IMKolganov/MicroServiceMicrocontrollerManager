using MicroServiceMicrocontrollerManager.Models;
using MicroServiceMicrocontrollerManager.Services;
using Microsoft.AspNetCore.Mvc;
using SharedRequests.SmartGarden.Models.Requests;
using SharedRequests.SmartGarden.Models.Responses;

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
        public async Task<IActionResult> GetTemperatureHumidity([FromBody] TemperatureHumidityRequest request)
        {
            var result =
                await mqttService.SendRequestAndWaitForResponse<TemperatureHumidityRequest, TemperatureHumidityResponse>(
                    "control/dht/", "status/dht/", request);
            if (result == null)
            {
                return NotFound("No response from MQTT.");
            }

            return Ok(result);
        }
        
        [HttpPost("GetSoilMoistureRequest")]
        public async Task<IActionResult> GetSoilMoisture([FromBody] SoilMoistureRequest request)
        {
            var result =
                await mqttService.SendRequestAndWaitForResponse<SoilMoistureRequest, SoilMoistureResponse>(
                    "control/soil-moisture/", "status/soil-moisture/", request);
            if (result == null)
            {
                return NotFound("No response from MQTT.");
            }

            return Ok(result);
        }
    }
}

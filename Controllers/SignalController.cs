using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalMonitoring.API.Hubs;
using SignalMonitoring.API.Models;
using SignalMonitoring.API.Services;
using System;
using System.Threading.Tasks;

namespace SignalMonitoring.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SignalController : ControllerBase
    {
        private readonly ISignalService _signalService;
        private readonly IHubContext<SignalHub> _hubContext;
        public SignalController(ISignalService signalService, IHubContext<SignalHub> hubContext)
        {
            _signalService = signalService;
            _hubContext = hubContext;
        }

        [HttpPost("deliverypoint")]
        public async Task<IActionResult> SignalArrived(SignalInputModel inputModel)
        {

            //you can validate input here
            //then if the inputmodel is valid then you can save the signal
            var saveResult = await _signalService.SaveSignalAsync(inputModel);

            //if you can save the signal you can notify all clients by using SignalHub
            if (saveResult)
            {
                //you might think use a mapping tool?
                SignalViewModel signalViewModel = new SignalViewModel
                {
                    Description = inputModel.Description,
                    CustomerName = inputModel.CustomerName,
                    Area = inputModel.Area,
                    Zone = inputModel.Zone,
                    SignalStamp = Guid.NewGuid().ToString()
                };

                await _hubContext.Clients.All.SendAsync("SignalMessageReceived", signalViewModel);
            }

            return StatusCode(200, saveResult);

        }

    }
}
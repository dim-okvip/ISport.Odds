using ISport.Odds.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ISport.Odds.Controllers
{
    [ApiController]
    [Route("api")]
    public class OddsController : ControllerBase
    {
        private readonly IHubContext<OddsHub> _oddsHub;
        private readonly TimerControl _timerControl;

        public OddsController(IHubContext<OddsHub> oddsHub, TimerControl timerControl)
        {
            _oddsHub = oddsHub;
            _timerControl = timerControl;
        }

        /// <summary>
        ///  This method is responsible to return the Odds information.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Return a object BookingResponse with all booking properties</returns>
        /// <response code="200">Odds</response>
        /// <response code="404">The Odds with the parameter informed does not exist.</response> 
        /// <response code="500">Internal error from Server.</response> 
        [ProducesResponseType(typeof(Models.PreMatchAndInPlayOddsMain), 200)]
        [HttpGet]
        [Route("odds/main")]
        public IActionResult Get([FromQuery] string connectionId, string matchId, string? companyId)
        {
            //if (!_timerControl.IsTimerStarted)
            //    _timerControl.ScheduleTimer(async () => await _oddsHub.Clients.Client(connectionId: connectionId).SendAsync(
            //        $"ReceiveMessage",
            //        await _oddsService.GetPreMatchAndInPlayMainOddsAsync(Utils.PreMatchAndInPlayOddsMainId, matchId, companyId)), 5000);
            return Ok(new { Message = "Synchronized" });
        }
    }
}

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
        private readonly OddsService _oddsService;

        public OddsController(IHubContext<OddsHub> oddsHub, TimerControl timerControl, OddsService oddsService)
        {
            _oddsHub = oddsHub;
            _timerControl = timerControl;
            _oddsService = oddsService;
        }

        [HttpGet]
        public async Task<List<Models.Odds>> Get() =>
            await _oddsService.GetAsync();

        /// <summary>
        ///  This method is responsible to return the Odds information.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Return a object BookingResponse with all booking properties</returns>
        /// <response code="200">Odds</response>
        /// <response code="404">The Odds with the parameter informed does not exist.</response> 
        /// <response code="500">Internal error from Server.</response> 
        [ProducesResponseType(typeof(Models.Odds), 200)]
        [HttpGet]
        [Route("odds/main")]
        public IActionResult Get([FromQuery] string matchId, string? companyId)
        {
            if (!_timerControl.IsTimerStarted)
                _timerControl.ScheduleTimer(async () => await _oddsHub.Clients.All.SendAsync(
                    "ReceiveMessage",
                    await _oddsService.GetByMatchIdAsync(Utils.PreMatchAndInPlayOddsMainId, matchId, companyId)), 2000);
            return Ok(new { Message = "Synchronized" });
        }

        [HttpPost]
        public async Task<IActionResult> Post(Models.Odds newOdds)
        {
            await _oddsService.CreateAsync(newOdds);

            return CreatedAtAction(nameof(Get), new { id = newOdds.Id }, newOdds);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Models.Odds updatedOdds)
        {
            var book = await _oddsService.GetAsync(id);

            if (book is null)
                return NotFound();

            updatedOdds.Id = book.Id;

            await _oddsService.UpdateAsync(id, updatedOdds);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _oddsService.GetAsync(id);

            if (book is null)
                return NotFound();

            await _oddsService.RemoveAsync(id);

            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ISport.Odds.Controllers
{
    [ApiController]
    [Route("api")]
    public class OddsController : ControllerBase
    {
        private readonly IPreMatchAndInPlayOddsMainService _preMatchAndInPlayOddsMainService;
        private readonly ITotalCornersService _totalCornersService;

        public OddsController(IPreMatchAndInPlayOddsMainService preMatchAndInPlayOddsMainService, ITotalCornersService totalCornersService)
        {
            _preMatchAndInPlayOddsMainService = preMatchAndInPlayOddsMainService;
            _totalCornersService = totalCornersService;
        }

        [ProducesResponseType(typeof(PreMatchAndInPlayOddsMain), 200)]
        [HttpGet]
        [Route("odds/main")]
        public async Task<IActionResult> GetPreMatchAndInPlayOddsMain([FromQuery] Source source, string? matchId, string? companyId)
        {
            PreMatchAndInPlayOddsMain preMatchAndInPlayOddsMain = await _preMatchAndInPlayOddsMainService.GetByMatchIdAsync(source, Utils.PreMatchAndInPlayOddsMainId, matchId, companyId);
            return Ok(preMatchAndInPlayOddsMain);
        }

        [ProducesResponseType(typeof(TotalCorners), 200)]
        [HttpGet]
        [Route("odds/cornerstotal/prematch")]
        public async Task<IActionResult> GetTotalCornersPreMatch([FromQuery] Source source, string? matchId, string? companyId)
        {
            TotalCorners totalCorners = await _totalCornersService.GetByMatchIdAsync(source, Utils.TotalCornersPreMatchId, matchId, companyId);
            return Ok(totalCorners);
        }

        [ProducesResponseType(typeof(TotalCorners), 200)]
        [HttpGet]
        [Route("odds/cornerstotal/inplay")]
        public async Task<IActionResult> GetTotalCornersInPlay([FromQuery] Source source, string? matchId, string? companyId)
        {
            TotalCorners totalCorners = await _totalCornersService.GetByMatchIdAsync(source, Utils.TotalCornersInPlayId, matchId, companyId);
            return Ok(totalCorners);
        }

        [HttpPut]
        [Route("odds/cornerstotal/prematch/{matchId}")]
        public async Task<IActionResult> FakeUpdateOddsCorner(string matchId)
        {
            TotalCorners cornerPreMatchMongoDB = _totalCornersService.GetByIdAsync(Utils.TotalCornersPreMatchId).Result;
            foreach (var itemMongoDB in cornerPreMatchMongoDB.Data)
            {
                if (itemMongoDB.MatchId == matchId)
                {
                    TotalCornersOddsDetail oddsMongoDB = itemMongoDB.Odds;
                    oddsMongoDB.TotalCorners = "10";
                    oddsMongoDB.Over = "10";
                    oddsMongoDB.Under = "10";
                }
            }
            _totalCornersService.UpdateAsync(Utils.TotalCornersPreMatchId, cornerPreMatchMongoDB).Wait();
            return Ok();
        }
    }
}

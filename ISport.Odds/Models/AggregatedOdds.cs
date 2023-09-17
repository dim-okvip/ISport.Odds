namespace ISport.Odds.Models
{
    public class AggregatedOdds
    {
        public PreMatchAndInPlayOddsMain PreMatchAndInPlayOddsMain { get; set; }
        public TotalCorners TotalCornersPreMatch { get; set; }
        public TotalCorners TotalCornersInPlay { get; set; }
    }
}

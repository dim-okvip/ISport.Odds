namespace ISport.Odds.Models
{
    public class PreMatchAndInPlayOddsMain
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public OddsType Data { get; set; }
    }

    public class OddsType
    {
        public List<string> Handicap { get; set; }
        public List<string> EuropeOdds { get; set; }
        public List<string> OverUnder { get; set; }
        public List<string> HandicapHalf { get; set; }
        public List<string> OverUnderHalf { get; set; }
    }
}

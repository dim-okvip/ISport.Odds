namespace ISport.Odds.Models
{
    public class TotalCorners
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public List<TotalCornersDetail> Data { get; set; } = new();
    }

    public class TotalCornersDetail
    {
        public string MatchId { get; set; }
        public string CompanyId { get; set; }
        public TotalCornersOddsDetail Odds { get; set; } = new();
        public long ChangeTime { get; set; }
    }

    public class TotalCornersOddsDetail
    {
        public string TotalCorners { get; set; }
        public string Over { get; set; }
        public string Under { get; set; }
    }
}

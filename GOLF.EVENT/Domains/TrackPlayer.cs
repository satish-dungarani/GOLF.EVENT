namespace GOLF.EVENT.Domains
{
    public class TrackPlayerPosition
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string? CurrentHole { get; set; }
        public DateTime? TrackedDateTime { get; set; }
    }
}

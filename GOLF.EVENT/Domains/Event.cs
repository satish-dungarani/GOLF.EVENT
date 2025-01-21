namespace GOLF.EVENT.Domains
{
    public class Event
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    } 
}

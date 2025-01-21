using GOLF.EVENT.Domains;
using GOLF.EVENT.Models;

namespace GOLF.EVENT.Services
{
    public interface IEventPlayerService
    {
        Task<Event> GetEventByIdAsync(int id);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> AddOrUpdateEventAsync(Event eventEntity);

        Task<Player> GetPlayerByIdAsync(int id);
        Task<IEnumerable<Player>> GetAllPlayersAsync();
        Task<Player> AddOrUpdatePlayerAsync(Player playerEntity);

        Task<TrackPlayerPosition> GetTrackPlayerPositionByIdAsync(int id);
        Task<IEnumerable<TrackPlayerPosition>> GetAllTrackPlayerPositionsAsync();
        Task<TrackPlayerPosition> AddOrUpdateTrackPlayerPositionAsync(TrackPlayerPosition trackPlayerPositionEntity);


        Task<IEnumerable<EventModel>> GetEventsFromApiAsync();
    }
}

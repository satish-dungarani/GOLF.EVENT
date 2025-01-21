using GOLF.EVENT.DatabaseContext;
using GOLF.EVENT.Domains;
using GOLF.EVENT.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Net.Http;
using System.Text;
using GOLF.EVENT.Models;

public class EventPlayerService : IEventPlayerService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly string _apiTokenUrl;
    private readonly string _apiClientId;
    private readonly string _apiClientSecret;
    private readonly string _apiBaseUrl;


    public EventPlayerService(ApplicationDbContext context, HttpClient httpClient, IConfiguration configuration)
    {
        _context = context;
        _httpClient = httpClient;
        _apiTokenUrl = configuration["OAuthConfig:TokenUrl"];
        _apiClientId = configuration["OAuthConfig:ClientId"];
        _apiClientSecret = configuration["OAuthConfig:ClientSecret"];
        _apiBaseUrl = configuration["OAuthConfig:StreamUrl"];

    }

    #region Event
    public async Task<Event> GetEventByIdAsync(int id)
    {
        return await _context.Events.FindAsync(id);
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task<Event> AddOrUpdateEventAsync(Event eventEntity)
    {
        if (eventEntity.Id == 0) // Insert
        {
            _context.Events.Add(eventEntity);
        }
        else // Update
        {
            _context.Events.Update(eventEntity);
        }
        await _context.SaveChangesAsync();
        return eventEntity;
    }
    #endregion


    #region Players
    public async Task<Player> GetPlayerByIdAsync(int id)
    {
        return await _context.Players.FindAsync(id);
    }

    public async Task<IEnumerable<Player>> GetAllPlayersAsync()
    {
        return await _context.Players.ToListAsync();
    }

    public async Task<Player> AddOrUpdatePlayerAsync(Player playerEntity)
    {
        if (playerEntity.Id == 0) // Insert
        {
            _context.Players.Add(playerEntity);
        }
        else // Update
        {
            _context.Players.Update(playerEntity);
        }
        await _context.SaveChangesAsync();
        return playerEntity;
    }
    #endregion


    #region TrackPlayerPosition     
    public async Task<TrackPlayerPosition> GetTrackPlayerPositionByIdAsync(int id)
    {
        return await _context.TrackPlayerPositions.FindAsync(id);
    }

    public async Task<IEnumerable<TrackPlayerPosition>> GetAllTrackPlayerPositionsAsync()
    {
        return await _context.TrackPlayerPositions.ToListAsync();
    }

    public async Task<TrackPlayerPosition> AddOrUpdateTrackPlayerPositionAsync(TrackPlayerPosition trackPlayerPositionEntity)
    {
        if (trackPlayerPositionEntity.Id == 0)
        {
            _context.TrackPlayerPositions.Add(trackPlayerPositionEntity);
        }
        else // Update
        {
            _context.TrackPlayerPositions.Update(trackPlayerPositionEntity);
        }
        await _context.SaveChangesAsync();
        return trackPlayerPositionEntity;
    }
    #endregion

    private async Task<string> GetAccessTokenAsync()
    {

        var request = new HttpRequestMessage(HttpMethod.Post, _apiTokenUrl);
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("client_id", _apiClientId));
        collection.Add(new("client_secret", _apiClientSecret));
        collection.Add(new("grant_type", "client_credentials"));
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to fetch access token: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResult = JsonConvert.DeserializeObject<dynamic>(responseContent);

        return tokenResult.access_token;
    }

    public async Task<IEnumerable<EventModel>> GetEventsFromApiAsync()
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, _apiBaseUrl);
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            //var response = await _httpClient.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            var response = await _httpClient.GetAsync(_apiBaseUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch events from API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var rootObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
            var events = rootObject.data;
            var eventsList = new List<EventModel>();

            foreach (var eventItem in events)
            {
                eventsList.Add(new EventModel
                {
                    EventId = eventItem.liv_event_id != null ? (int)eventItem.liv_event_id : 0,
                    EventName = eventItem.event_name != null ? eventItem.event_name.ToString() : string.Empty,
                    StartDate = eventItem.start_date != null ? DateTime.Parse(eventItem.start_date.ToString()) : (DateTime?)null,
                    EndDate = eventItem.end_date != null ? DateTime.Parse(eventItem.end_date.ToString()) : (DateTime?)null
                });
            }
            return eventsList;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}

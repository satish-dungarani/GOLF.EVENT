using GOLF.EVENT.Domains;
using GOLF.EVENT.Services;
using Microsoft.AspNetCore.Mvc;

namespace GOLF.EVENT.Controllers
{
    public class GolfEventController : Controller
    {
        private readonly IEventPlayerService _eventPlayerService;

        public GolfEventController(IEventPlayerService eventPlayerService)
        {
            _eventPlayerService = eventPlayerService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _eventPlayerService.GetEventsFromApiAsync();
                return View(events);
            }
            catch (Exception ex)
            {
                return View(new List<Event>());
            }
        }

    }
}

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaviUcakInterviewTask.Data;
using MaviUcakInterviewTask.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<EventsController> _logger;
    public EventsController(ApplicationDbContext context, IMemoryCache cache, ILogger<EventsController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }


    public async Task<IActionResult> Index()
    {
        const string cacheKey = "eventsList";

        
        if (!_cache.TryGetValue(cacheKey, out List<Event> events))
        {
           
            events = await _context.Events.ToListAsync();

            _logger.LogInformation("Veriler veritabanından alındı."); 
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(cacheKey, events, cacheEntryOptions);
        }
        else
        {
            _logger.LogInformation("Veriler cache'den alındı."); 
        }

        return View(events);
    }

    
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var eventItem = await _context.Events
            .FirstOrDefaultAsync(m => m.Id == id);
        if (eventItem == null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    
    public IActionResult Create()
    {
        return View();
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Location,Time,IsPaid,Price,Description,ImageUrl,EventType")] Event eventItem)
    {
        if (ModelState.IsValid)
        {
            _context.Add(eventItem);
            await _context.SaveChangesAsync();
            _cache.Remove("eventsList");
            return RedirectToAction(nameof(Index));
        }
        return View(eventItem);
    }

   
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null)
        {
            return NotFound();
        }
        return View(eventItem);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Location,Time,IsPaid,Price,Description,ImageUrl,EventType")] Event eventItem)
    {
        if (id != eventItem.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(eventItem);
                await _context.SaveChangesAsync();
                _cache.Remove("eventsList");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(eventItem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(eventItem);
    }

    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var eventItem = await _context.Events
            .FirstOrDefaultAsync(m => m.Id == id);
        if (eventItem == null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem != null)
        {
            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
            _cache.Remove("eventsList");
        }
        return RedirectToAction(nameof(Index));
    }

    private bool EventExists(int id)
    {
        return _context.Events.Any(e => e.Id == id);
    }
}

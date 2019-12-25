using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using crm.service.database;
using amocrm.library;
using System.Net.Http;
using office.fitness_pro.ru.Models;

namespace office.fitness_pro.ru.Areas.Crm.Controllers
{
    [Area("Crm")]
    public class EventLinksController : Controller
    {
        private readonly CrmDataBaseContext _context;

        public EventLinksController(CrmDataBaseContext context)
        {
            _context = context;
        }

        // GET: Crm/EventLinks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Crm/EventLinks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventLink = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventLink == null)
            {
                return NotFound();
            }

            return View(eventLink);
        }

        // GET: Crm/EventLinks/Create
        public async Task<IActionResult> Create()
        {
            var amoCrm = new CrmManager(account: "apfitness", login: "kloder@fitness-pro.ru", pass: "99aad176302f7ea9213c307e1e6ab8fc");
            var task = await amoCrm.CustomFields.ConfigureAwait(false);
            var seminars = task.Lead[66349].Enums;
            var progs = task.Lead[227457].Enums;
            var result = seminars.Concat(progs);


            var client = new HttpClient();
            var request = await client.GetAsync(@"https://lc.fitness-pro.ru/Program/ProgramAnnotation").ConfigureAwait(false);

            var lcProgs = await request.Content.ReadAsAsync<IEnumerable<EventsDto>>();

            var events = await _context.Events.ToListAsync();

            var amoKeys = result.Select(k => k.Key);
            var dbKeys = events.Select(k => k.CrmKey);

            var amodiff = amoKeys.Except(dbKeys);
            var amoResult = result.Where(x => amodiff.Any(y => y == x.Key));


            var lCKeys = lcProgs.Select(k => k.Guid);          
            var dbGuids = events.Select(k => k.lCKey);

            var lcDiff = lCKeys.Except(dbGuids);

            var lcResult = lcProgs.Where(x => lcDiff.Any(y => y == x.Guid));

            return View( new CreateVM { Amo = amoResult, lC = lcResult });
        }

        public class CreateVM
        {
            public IEnumerable< KeyValuePair<int, string>> Amo { get; set; }
            public IEnumerable<EventsDto> lC { get; set; }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVMResult eventLink)
        {
            if (ModelState.IsValid)
            {
                var client = new HttpClient();
                var request = await client.GetAsync(@"https://lc.fitness-pro.ru/Program/ProgramAnnotation").ConfigureAwait(false);
                var lcProgs = await request.Content.ReadAsAsync<IEnumerable<EventsDto>>();


                var @event = lcProgs.First(x => x.Guid == eventLink.lC);

                var dto = new EventLink
                {
                    CrmKey = eventLink.Amo,
                    lCKey = eventLink.lC,
                    PartTime = @event.EventPartTime.Title,
                    Title = @event.Title,
                    Department = @event.Department.Title,
                    Type = @event.EventForm.Title
                };


                _context.Add(dto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventLink);
        }

        public class CreateVMResult
        { 
            public int Amo { get; set; }
            public Guid lC { get; set; }
        }

        // POST: Crm/EventLinks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,CrmKey,lCKey,Title,Type,Department,PartTime")] EventLink eventLink)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(eventLink);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(eventLink);
        //}

        // GET: Crm/EventLinks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventLink = await _context.Events.FindAsync(id);
            if (eventLink == null)
            {
                return NotFound();
            }
            return View(eventLink);
        }

        // POST: Crm/EventLinks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CrmKey,lCKey,Title,Type,Department,PartTime")] EventLink eventLink)
        {
            if (id != eventLink.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventLink);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventLinkExists(eventLink.Id))
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
            return View(eventLink);
        }

        // GET: Crm/EventLinks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventLink = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventLink == null)
            {
                return NotFound();
            }

            return View(eventLink);
        }

        // POST: Crm/EventLinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventLink = await _context.Events.FindAsync(id);
            _context.Events.Remove(eventLink);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventLinkExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}

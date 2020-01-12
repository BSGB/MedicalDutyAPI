using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicalDutyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDutyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulerController : Controller
    {
        [HttpGet]
        public ActionResult<IEnumerable<SchedulerEvent>> Get()
        {
            using var db = new DutyingContext();

            var schedulerEvents = db.SchedulerEvents
                .ToList();

            return Ok(schedulerEvents);
        }

        [HttpGet("{id}")]
        public ActionResult<SchedulerEvent> Get([FromRoute]int eventId)
        {
            using var db = new DutyingContext();

            var _event = db.SchedulerEvents
                .FirstOrDefault(e => e.Id == eventId);

            if (_event is null) return NotFound();

            return Ok(_event);
        }

        [HttpGet("userId/{userId}")]
        public ActionResult<SchedulerEvent> GetByUserId([FromRoute]int userId)
        {
            using var db = new DutyingContext();

            var _events = db.Users
                .Include(user => user.SchedulerEvents)
                .Where(user => user.Id == userId)
                .Select(user => user.SchedulerEvents)
                .ToList();

            if (_events is null) return NotFound();

            return Ok(_events);
        }

        [HttpPost]
        public ActionResult Post([FromBody]SchedulerEvent schedulerEvent)
        {
            using var db = new DutyingContext();

            var user = db.Users
                .Include(user => user.SchedulerEvents)
                .FirstOrDefault(user => user.Id == schedulerEvent.UserId);

            if (user is null) NotFound();

            if (user.SchedulerEvents is null) user.SchedulerEvents = new List<SchedulerEvent>();

            user.SchedulerEvents.Add(schedulerEvent);

            db.Users.Update(user);
            db.SaveChanges();

            return Created("", schedulerEvent);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute]int eventId)
        {
            using var db = new DutyingContext();

            var _event = db.SchedulerEvents
                .FirstOrDefault(e => e.Id == eventId);

            if (_event is null) return NotFound();

            db.SchedulerEvents.Remove(_event);
            db.SaveChanges();

            return Ok();
        }

        [HttpPut]
        public ActionResult Put([FromBody]SchedulerEvent schedulerEvent)
        {
            using var db = new DutyingContext();

            var user = db.Users
                .Include(user => user.SchedulerEvents)
                .FirstOrDefault(user => user.Id == schedulerEvent.UserId);

            if (user is null) NotFound();

            user.SchedulerEvents
                .Where(e => e.Id == schedulerEvent.Id)
                .Select(e =>
                {
                    e.StartsAt = e.StartsAt != schedulerEvent.StartsAt ? schedulerEvent.StartsAt : e.StartsAt;
                    e.EndsAt = e.EndsAt != schedulerEvent.EndsAt ? schedulerEvent.EndsAt : e.EndsAt;
                    e.Comment = e.Comment != schedulerEvent.Comment ? schedulerEvent.Comment : e.Comment;
                    return e;
                });

            db.Users.Update(user);
            db.SaveChanges();

            return Ok();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicalDutyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MedicalDutyAPI.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        [HttpGet]
        [Authorize(Roles = "headmaster, doctor")]
        public ActionResult<IEnumerable<User>> Get()
        {
            using var db = new DutyingContext();

            var users = db.Users
                .Include(user => user.UserRoles)
                    .ThenInclude(userRole => userRole.Role)
                .ToList();

            return Ok(users);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "headmaster, doctor")]
        public ActionResult<User> Get([FromRoute]int userId)
        {
            using var db = new DutyingContext();

            var user = db.Users
                .Where(user => user.Id == userId)
                .Include(user => user.UserRoles)
                    .ThenInclude(userRole => userRole.Role)
                .FirstOrDefault();

            if (user is null) return NotFound();

            return Ok(user);
        }

        [HttpPut]
        [Authorize(Roles = "headmaster, doctor")]
        public ActionResult Put([FromBody]User user)
        {
            using var db = new DutyingContext();

            if (!db.Users.Any(u => u.Id == user.Id)) return NotFound();

            var dbUser = db.Users
                .Include(user => user.UserRoles)
                    .ThenInclude(uRoles => uRoles.Role)
                .FirstOrDefault(u => u.Id == user.Id);

            if (dbUser.FirstName != user.FirstName) dbUser.FirstName = user.FirstName;
            if (dbUser.LastName != user.LastName) dbUser.LastName = user.LastName;
            if (dbUser.Email != user.Email) dbUser.Email = user.Email;

            var newRoles = user.UserRoles
                .Where(userRole => !dbUser.UserRoles.Any(dbUserRole => dbUserRole.Id == userRole.Id))
                .ToList();

            dbUser.UserRoles.AddRange(newRoles);

            if (!string.IsNullOrEmpty(user.Password) && dbUser.Password != user.Password)
            {
                var salt = RegisterController.GenerateSalt();
                var hashedPassword = RegisterController.HashPasswordPbkdf2(user.Password, salt);

                dbUser.Password = hashedPassword;
                dbUser.Salt = Convert.ToBase64String(salt);
            }

            db.Users.Update(dbUser);
            db.SaveChanges();

            return Ok(dbUser);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "headmaster, administrator")]
        public ActionResult Delete([FromRoute]int userId)
        {
            using var db = new DutyingContext();

            var user = db.Users
                .Include(user => user.UserRoles)
                .Include(user => user.SchedulerEvents)
                .FirstOrDefault(user => user.Id == userId);

            if (user is null) return NotFound();

            db.Users.Remove(user);
            db.UserRoles.RemoveRange(user.UserRoles);
            if (user.SchedulerEvents != null) db.SchedulerEvents.RemoveRange(user.SchedulerEvents);
            db.SaveChanges();

            return Ok();
        }
    }
}

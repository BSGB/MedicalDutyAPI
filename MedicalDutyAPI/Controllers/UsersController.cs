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
        [Authorize(Roles = "headmaster, doctor, administrator")]
        public ActionResult<IEnumerable<User>> Get()
        {
            using var db = new DutyingContext();

            var users = db.Users
                .Include(user => user.UserRoles)
                    .ThenInclude(userRole => userRole.Role)
                .ToList();

            return Ok(users);
        }

        [HttpGet("userId/{userId}")]
        //[Authorize(Roles = "headmaster, doctor, administrator")]
        [AllowAnonymous]
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

        [HttpGet("wardId/{wardId}")]
        //[Authorize(Roles = "headmaster, doctor, administrator")]
        [AllowAnonymous]
        public ActionResult<User> GetByWardId([FromRoute]int wardId)
        {
            using var db = new DutyingContext();

            var users = db.Wards
                .Include(ward => ward.Users)
                .Where(ward => ward.Id == wardId)
                .Select(ward => ward.Users)
                .ToList();

            if (users is null) return NotFound();

            return Ok(users);
        }

        //TODO
        [HttpPut]
        //[Authorize(Roles = "headmaster, doctor, administrator")]
        [AllowAnonymous]
        public ActionResult<User> Put([FromBody]User user)
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

            dbUser.UserRoles.RemoveAll(dbUr => !user.UserRoles.Any(ur => dbUr.RoleId == ur.RoleId));
            user.UserRoles.RemoveAll(ur => dbUser.UserRoles.Any(dbUr => dbUr.RoleId == ur.RoleId));

            if (user.UserRoles.Count > 0) dbUser.UserRoles.AddRange(user.UserRoles);

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

        //TODO
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

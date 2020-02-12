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
        public ActionResult<IEnumerable<User>> Get([FromHeader(Name = "Paging-PageNo")] int pageNo, [FromHeader(Name = "Paging-PageSize")] int pageSize)
        {
            int skipRecords = (pageNo - 1) * pageSize;
            
            using var db = new DutyingContext();

            int totalRecords = db.Users.Count();
            int pageCount = totalRecords > 0 ? (int) Math.Ceiling(totalRecords / (double) pageSize) : 0;

            var users = db.Users
                .OrderBy(user => user.LastName)
                .Skip(skipRecords)
                .Take(pageSize)
                .Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.Role)
                .ToList();

            Response.Headers.Add("Paging-PageNo", pageNo.ToString());
            Response.Headers.Add("Paging-PageSize", pageSize.ToString());
            Response.Headers.Add("Paging-PageCount", pageCount.ToString());
            Response.Headers.Add("Paging-TotalRecordsCount", totalRecords.ToString());
            
            return Ok(users);
        }

        [HttpGet("userId/{userId}")]
        [Authorize(Roles = "headmaster, doctor, administrator")]
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
        [Authorize(Roles = "headmaster, doctor, administrator")]
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

        [HttpPut]
        [Authorize(Roles = "headmaster, doctor, administrator")]
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
            if (dbUser.WardId != user.WardId) dbUser.WardId = user.WardId;

            dbUser.UserRoles.RemoveAll(dbUr => !user.UserRoles.Any(ur => dbUr.RoleId == ur.RoleId));
            user.UserRoles.RemoveAll(ur => dbUser.UserRoles.Any(dbUr => dbUr.RoleId == ur.RoleId));

            if (user.UserRoles.Count > 0) dbUser.UserRoles.AddRange(user.UserRoles);

            if (!string.IsNullOrEmpty(user.Password) && dbUser.Password != RegisterController.HashPasswordPbkdf2(user.Password, Convert.FromBase64String(dbUser.Salt)))
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
                .FirstOrDefault(user => user.Id == userId);

            if (user is null) return NotFound();

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok();
        }
    }
}

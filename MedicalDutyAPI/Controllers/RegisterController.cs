using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MedicalDutyAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MedicalDutyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        private static readonly int numBytesSalt = 128 / 8;
        private static readonly int iterCount = 10000;
        private static readonly int numBytesReq = 256 / 8;

        [HttpGet]
        public ActionResult Get()
        {
            return Unauthorized("Register endpoint...");
        }

        [HttpPost]
        //[Authorize(Roles = "headmaster, administrator")]
        [AllowAnonymous]
        public ActionResult<User> Post([FromBody]User user)
        {
            using var db = new DutyingContext();

            if (db.Users.Any(u => u.Email == user.Email)) return Problem(title: "User with given email already exists!", statusCode: StatusCodes.Status409Conflict);

            var ward = db.Wards
                .Include(ward => ward.Users)
                    .ThenInclude(users => user.UserRoles)
                .FirstOrDefault(ward => ward.Id == user.WardId);

            if (ward.Users is null) ward.Users = new List<User>();

            var salt = GenerateSalt();
            var hashedPassword = HashPasswordPbkdf2(user.Password, salt);

            user.Password = hashedPassword;
            user.Salt = Convert.ToBase64String(salt);
            user.CreatedAt = DateTime.Now;

            try
            {
                var role = db.Roles
                    .Where(role => role.Symbol == (int)RoleEnum.Doctor)
                    .First();

                user.UserRoles = new List<UserRole>()
                {
                    new UserRole() { Role = role }
                };

                ward.Users.Add(user);

                db.Wards.Update(ward);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Database communication error!",
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: ex.Message);
            }

            return Created("User created", user);
        }

        public static string HashPasswordPbkdf2(string password, byte[] salt)
        {
            var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: iterCount,
                numBytesRequested: numBytesReq
            ));

            return hashedPassword;
        }

        public static byte[] GenerateSalt()
        {
            var salt = new byte[numBytesSalt];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
    }
}

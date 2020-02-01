using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MedicalDutyAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

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
        public ActionResult<User> Post(string firstName, string lastName, string password, string email)
        {
            var salt = GenerateSalt();
            var hashedPassword = HashPasswordPbkdf2(password, salt);

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Password = hashedPassword,
                Salt = Convert.ToBase64String(salt),
                Email = email,
                CreatedAt = DateTime.Now
            };

            using var db = new DutyingContext();

            try
            {
                var role = db.Roles
                    .Where(role => role.Symbol == (int)RoleEnum.Doctor)
                    .First();

                user.UserRoles = new List<UserRole>()
                {
                    new UserRole() { Role = role }
                };

                db.Add(user);

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

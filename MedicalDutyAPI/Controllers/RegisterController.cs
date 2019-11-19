using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MedicalDutyAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MedicalDutyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        public static readonly List<User> usersMockup = new List<User>();

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
                Id = usersMockup.Count,
                FirstName = firstName,
                LastName = lastName,
                Password = hashedPassword,
                Salt = Convert.ToBase64String(salt),
                Email = email,
                Roles = new List<RoleEnum> { RoleEnum.Doctor },
                CreatedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            usersMockup.Add(user);

            return Ok(user);
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

        private static byte[] GenerateSalt()
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

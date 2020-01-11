using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using MedicalDutyAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MedicalDutyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private static readonly string secretKey = "Tdt7J0zZfObeuL156DKhvcNGX3IslxEW";
        private static readonly string tokenIssuer = "server";
        private static readonly string tokenAudience = "server";
        private static readonly int tokenValidMinutes = 15;

        [HttpGet]
        public ActionResult Get()
        {
            return Unauthorized("Login endpoint...");
        }

        [HttpPost]
        public async Task<ActionResult> Post(string email, string password)
        {
            using var db = new DutyingContext();

            if (!db.Users.Any(user => user.Email == email)) return NotFound("No user found!");

            var user = db.Users
                .Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.Role)
                .Where(user => user.Email == email)
                .First();

            var salt = Convert.FromBase64String(user.Salt);
            var hashedPassword = RegisterController.HashPasswordPbkdf2(password, salt);

            if (user.Password != hashedPassword) return BadRequest("Wrong password!");

            var token = await CreateTokenAsync(user);

            return Ok(token);
        }

        private static async Task<string> CreateTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsIdentity = await CreateClaimsIdentityAsync(user);

            var token = tokenHandler.CreateJwtSecurityToken
                (
                issuer: tokenIssuer,
                audience: tokenAudience,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(tokenValidMinutes),
                signingCredentials: new SigningCredentials
                (
                    new SymmetricSecurityKey
                    (
                        Encoding.Default.GetBytes(secretKey)
                    ),
                    SecurityAlgorithms.HmacSha256Signature)
                );

            return tokenHandler.WriteToken(token);
        }

        private static Task<ClaimsIdentity> CreateClaimsIdentityAsync(User user)
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

            foreach (var role in user.UserRoles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Role.Name));
            }

            return Task.FromResult(claimsIdentity);
        }
    }
}

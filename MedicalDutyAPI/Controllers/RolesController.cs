using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicalDutyAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDutyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "headmaster, administrator")]
        public ActionResult<IEnumerable<Role>> Get()
        {
            using var db = new DutyingContext();

            var dbRoles = db.Roles
                .ToList();

            return Ok(dbRoles);
        }

        [HttpGet("id/{roleId}")]
        [Authorize(Roles = "headmaster, administrator")]
        public ActionResult<Role> GetById([FromRoute]int roleId)
        {
            using var db = new DutyingContext();

            var role = db.Roles
                .Where(role => role.Id == roleId)
                .FirstOrDefault();

            if (role is null) return NotFound();

            return Ok(role);
        }

        [HttpGet("symbol/{roleSymbol}")]
        [Authorize(Roles = "headmaster, administrator")]
        public ActionResult<Role> GetBySymbol([FromRoute]int roleSymbol)
        {
            using var db = new DutyingContext();

            var role = db.Roles
                .Where(role => role.Symbol == roleSymbol)
                .FirstOrDefault();

            if (role is null) return NotFound();

            return Ok(role);
        }
    }
}
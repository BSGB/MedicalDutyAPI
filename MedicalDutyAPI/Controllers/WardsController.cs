using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicalDutyAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDutyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WardsController : ControllerBase
    {
        [HttpGet("{hospitalId}")]
        [Authorize(Roles = "headmaster, administrator")]
        public ActionResult<IEnumerable<Ward>> Get([FromRoute]int hospitalId)
        {
            using var db = new DutyingContext();

            var wards = db.Hospitals
                .Include(hospital => hospital.Wards)
                .Where(hospital => hospital.Id == hospitalId)
                .Select(hospital => hospital.Wards)
                .ToList();

            if (wards is null) return NotFound();

            return Ok(wards);
        }

        [HttpPost]
        public ActionResult<Ward> Post([FromBody]Ward ward)
        {
            using var db = new DutyingContext();

            var hospital = db.Hospitals
                .Include(hospital => hospital.Wards)
                .FirstOrDefault(hospital => hospital.Id == ward.HospitalId);

            if (hospital is null) return NotFound();

            if (hospital.Wards is null) hospital.Wards = new List<Ward>();

            hospital.Wards.Add(ward);

            db.Hospitals.Update(hospital);
            db.SaveChanges();

            return Created("", ward);
        }

        [HttpDelete("{wardId}")]
        [Authorize(Roles = "headmaster, administrator")]
        public ActionResult Delete([FromRoute]int wardId)
        {
            using var db = new DutyingContext();

            var ward = db.Wards
                .FirstOrDefault(ward => ward.Id == wardId);

            if (ward is null) return NotFound();

            db.Wards.Remove(ward);
            db.SaveChanges();

            return Ok();
        }
    }
}
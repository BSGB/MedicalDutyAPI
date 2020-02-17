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
    public class HospitalsController : ControllerBase
    {
        [HttpGet("search/{searchPhrase?}")]
        [Authorize(Roles = "headmaster, administrator")]
        public ActionResult<IEnumerable<Hospital>> Get([FromRoute] string searchPhrase,
            [FromHeader(Name = "Paging-PageNo")] int pageNo,
            [FromHeader(Name = "Paging-PageSize")] int pageSize)
        {
            int skipRecords = (pageNo - 1) * pageSize;
            
            using var db = new DutyingContext();

            var query = db.Hospitals.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchPhrase))
            {
                query = query.Where(hospital =>
                    hospital.Name.ToLower().Contains(searchPhrase.ToLower()));
            }
            
            int totalRecords = query.Count();
            
            int pageCount = totalRecords > 0 ? (int) Math.Ceiling(totalRecords / (double) pageSize) : 0;

            var hospitals = query.OrderBy(hospital => hospital.Name)
                .Skip(skipRecords)
                .Take(pageSize)
                .ToList();
            
            pageNo = Math.Min(pageCount, pageNo);

            Response.Headers.Add("Paging-PageNo", pageNo.ToString());
            Response.Headers.Add("Paging-PageSize", pageSize.ToString());
            Response.Headers.Add("Paging-PageCount", pageCount.ToString());
            Response.Headers.Add("Paging-TotalRecordsCount", totalRecords.ToString());

            return Ok(hospitals);
        }
        
        

        [HttpGet("{hospitalId}")]
        [Authorize(Roles = "headmaster, administrator")]
        public ActionResult<IEnumerable<Hospital>> Get([FromRoute]int hospitalId)
        {
            using var db = new DutyingContext();

            var hospital = db.Hospitals
                .Where(hospital => hospital.Id == hospitalId)
                .FirstOrDefault();

            if (hospital is null) return NotFound();

            return Ok(hospital);
        }
        
        [HttpGet("wardId/{wardId}")]
        [Authorize(Roles = "headmaster, doctor, administrator")]
        public ActionResult<Hospital> GetByWardId([FromRoute]int wardId)
        {
            using var db = new DutyingContext();

            var hospitals = db.Hospitals
                .Include(hospital => hospital.Wards)
                .Where(hospital => hospital.Wards.Any(ward => ward.Id == wardId))
                .ToList();

            if (hospitals is null) return NotFound();

            return Ok(hospitals);
        }

        [HttpPost]
        [Authorize(Roles = "administrator")]
        public ActionResult<Hospital> Post(string street, string zip, string city, string district, string name)
        {
            using var db = new DutyingContext();

            var hospital = new Hospital()
            {
                Street = street,
                Zip = zip,
                City = city,
                District = district,
                Name = name,
            };

            try
            {
                db.Add(hospital);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Database communication error!",
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: ex.Message);
            }

            return Created("Hospital created", hospital);
        }

        [HttpPut]
        [Authorize(Roles = "administrator")]
        public ActionResult<Hospital> Put([FromBody]Hospital hospital)
        {
            using var db = new DutyingContext();

            if (!db.Hospitals.Any(h => h.Id == hospital.Id)) return NotFound();

            var dbHospital = db.Hospitals
                .FirstOrDefault(h => h.Id == hospital.Id);

            if (dbHospital.Street != hospital.Street) dbHospital.Street = hospital.Street;
            if (dbHospital.Zip != hospital.Zip) dbHospital.Zip = hospital.Zip;
            if (dbHospital.City != hospital.City) dbHospital.City = hospital.City;
            if (dbHospital.District != hospital.District) dbHospital.District = hospital.District;
            if (dbHospital.Name != hospital.Name) dbHospital.Name = hospital.Name;

            db.Hospitals.Update(dbHospital);
            db.SaveChanges();


            return Ok(dbHospital);
        }

        [HttpDelete("{hospitalId}")]
        [Authorize(Roles = "administrator")]
        public ActionResult Delete([FromRoute]int hospitalId)
        {
            using var db = new DutyingContext();

            var hospital = db.Hospitals
                .FirstOrDefault(hospital => hospital.Id == hospitalId);

            if (hospital is null) return NotFound();

            db.Hospitals.Remove(hospital);
            db.SaveChanges();

            return Ok();
        }
    }
}
﻿using System;
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
    public class HospitalsController : ControllerBase
    {
        [HttpGet]
        //[Authorize(Roles = "headmaster, administrator")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Hospital>> Get()
        {
            using var db = new DutyingContext();

            var hospitals = db.Hospitals
                .ToList();

            return Ok(hospitals);
        }

        [HttpGet("{hospitalId}")]
        //[Authorize(Roles = "headmaster, administrator")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Hospital>> Get([FromRoute]int hospitalId)
        {
            using var db = new DutyingContext();

            var hospital = db.Hospitals
                .Where(hospital => hospital.Id == hospitalId)
                .FirstOrDefault();

            if (hospital is null) return NotFound();

            return Ok(hospital);
        }

        [HttpPost]
        //[Authorize(Roles = "administrator")]
        [AllowAnonymous]
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
        //[Authorize(Roles = "administrator")]
        [AllowAnonymous]
        public ActionResult<Hospital> Put([FromBody]Hospital hospital)
        {
            using var db = new DutyingContext();

            if (db.Hospitals.Any(h => h.Id == hospital.Id)) return NotFound();

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
    }
}
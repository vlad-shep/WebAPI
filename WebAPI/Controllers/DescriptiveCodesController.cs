using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Services;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DescriptiveCodesController : Controller
    {
        private readonly IConfiguration _configuration;
        public DescriptiveCodesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult GetAllDescriptiveCodes()
        {
            DescriptiveCodesService descriptiveCodesService = new DescriptiveCodesService(_configuration);

            return new JsonResult(descriptiveCodesService.GetAllDescriptiveCodes());
        }

        [HttpPost]
        public JsonResult GetInsertDescriptiveCode(DescriptiveCodes descriptiveCodes)
        {
            DescriptiveCodesService descriptionPartsService = new DescriptiveCodesService(_configuration);
            descriptionPartsService.GetInsertDescriptiveCode(descriptiveCodes);
            return new JsonResult(descriptiveCodes);
        }

        [HttpPut("{descriptiveCodeID:int}")]
        public JsonResult GetUpdateDescriptveCode(DescriptiveCodes descriptiveCodes)
        {
            DescriptiveCodesService descriptiveCodesService = new DescriptiveCodesService(_configuration);
            descriptiveCodesService.GetUpdateDescriptiveCode(descriptiveCodes);
            return new JsonResult("Updated Successfully");
        }

        [HttpDelete("{descriptiveCodeID:int}")]
        public JsonResult GetDeleteDescriptiveCode(int descriptiveCodeID)
        {
            DescriptiveCodesService descriptiveCodesService = new DescriptiveCodesService(_configuration);
            descriptiveCodesService.GetDeleteDescriptiveCode(descriptiveCodeID);
            return new JsonResult("Delete Successfully");
        }
    }
}

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
    public class DescriptionPartsController : Controller
    {
        private readonly IConfiguration _configuration;
        public DescriptionPartsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult GetAllDescriptionParts()
        {
            DescriptionPartsService descriptionPartsService = new DescriptionPartsService(_configuration);

            return new JsonResult(descriptionPartsService.GetAllDescriptionParts());
        }

        [HttpPost]
        public JsonResult GetInsertDescriptionParts(DescriptionParts descriptionParts)
        {
            DescriptionPartsService descriptionPartsService = new DescriptionPartsService(_configuration);
            return new JsonResult(descriptionPartsService.GetInsertDescriptionParts(descriptionParts));
        }

        [HttpPut("{descriptionPartID:int}")]
        public JsonResult GetUpdateDescriptionParts(DescriptionParts descriptionParts)
        {
            DescriptionPartsService descriptionPartsService = new DescriptionPartsService(_configuration);
            descriptionPartsService.GetUpdateDescriptionParts(descriptionParts);
            return new JsonResult("Updated Successfully");
        }

        [HttpDelete("{descriptionPartID:int}")]
        public JsonResult GetDeleteDescriptionParts(int descriptionPartID)
        {
            DescriptionPartsService descriptionPartsService = new DescriptionPartsService(_configuration);
            descriptionPartsService.GetDeleteDescriptionParts(descriptionPartID);
            return new JsonResult("Delete Successfully");
        }
    }
}

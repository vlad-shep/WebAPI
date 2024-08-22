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
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class MasksController : Controller
    {
        private readonly IConfiguration _configuration;
        public MasksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult GetAllMasks()
        {
            MaskService maskService = new MaskService(_configuration);
            
            return new JsonResult(maskService.GetAllMasks());
        }
        [HttpPost]
        public JsonResult GetInsertMask(Masks mask)
        {
            MaskService maskService = new MaskService(_configuration);
            maskService.GetInsertMask(mask);
            return new JsonResult(mask);
        }
        
        [HttpPut("{maskID:int}")]
        public JsonResult GetUpdateMask(Masks mask)
        {
            MaskService maskService = new MaskService(_configuration);
            maskService.GetUpdateMask(mask);
            return new JsonResult("Updated Successfully");
        }

        [HttpDelete("{maskID:int}")]
        public JsonResult GetDeleteMasks(int maskID)
        {
            MaskService maskService = new MaskService(_configuration);
            maskService.GetDeleteMask(maskID);
            return new JsonResult("Delete Successfully");
        }
    }
}

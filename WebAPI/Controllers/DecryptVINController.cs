using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Configuration;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecryptVINController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDescriptionPartsService _descriptionPartsService;
        private readonly IMaskService _maskService;
        private readonly IDependenciesService _dependenciesService;

        public DecryptVINController(IConfiguration configuration, IDescriptionPartsService descriptionPartsService,
            IMaskService maskService, IDependenciesService dependenciesService)
        {
            _configuration = configuration;
            _descriptionPartsService = descriptionPartsService;
            _dependenciesService = dependenciesService;
            _maskService = maskService;
        }
   

        [HttpGet]
        public JsonResult GetDecryptVINCode(string vinCode)
        {
            vinCode = Request.Query.FirstOrDefault(p => p.Key == "vin").Value;
            DecryptVinCodeService decryptVinCode = new DecryptVinCodeService(_configuration, _descriptionPartsService, _dependenciesService, _maskService);
           
            return new JsonResult(decryptVinCode.DecryptVin(vinCode.ToUpper()));
        }
    }
}
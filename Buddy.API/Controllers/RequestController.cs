using Buddy.API.Infrastructure;
using Buddy.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Buddy.API.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        readonly Router router;

        /// <summary>
        /// use Dependency Injection to force instantiate Router and IConfiguration
        /// </summary>
        /// <param name="iConfig"></param>
        public RequestController(IConfiguration iConfig)
        {
            router = new Router(iConfig);
        }

        /// <summary>
        /// to handle generic request with POST method
        /// 1. get service settings data from request object
        /// 2. execute service by passing service settings and parameters object
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Generic([FromBody] Request request)
        {
            Response response;
            try
            {
                if (string.IsNullOrEmpty(request.ServiceCode))
                    return new NotFoundObjectResult("Service code is not provided");

                string serviceConfigurationError = "";                
                ServiceMetaData serviceData = router.GetServiceData(request.ServiceCode, ref serviceConfigurationError);
                if (string.IsNullOrEmpty(serviceConfigurationError) && serviceData != null)
                    response = router.Execute(serviceData, request?.ServiceParam ?? null);
                else
                    return new BadRequestObjectResult(serviceConfigurationError);
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex.ToString());
            }
            return Ok(response);
        }

        /// <summary>
        /// this is just for testing api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Welcome()
        {
            return Ok("Welcome to BUDDY generic request handler...");
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Responses;
using TutorMioAPI1.Domain;
using TutorMioAPI1.Interfaces;

namespace TutorMioAPI1.Controllers
{
    [Route("api/plans")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private IPlansService _service = null;
        public PlansController(IPlansService service)
        {
            _service = service;
        }

        //GET ALL PLANS
        [HttpGet]
        public ActionResult<ItemsResponse<Plan>> GetPlans()
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                List<Plan> list = _service.GetAllPlans();
                if (list == null)
                {
                    response = new ErrorResponse("App resource not found.");
                    code = 404;
                }
                else
                    response = new ItemsResponse<Plan> { Items = list };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }   
        
        [HttpGet("language")]
        public ActionResult<ItemsResponse<Plan>> GetPlansByLanguage(string language)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                List<Plan> list = _service.GetAllPlansByLang(language);
                if (list == null)
                {
                    response = new ErrorResponse("App resource not found.");
                    code = 404;
                }
                else
                    response = new ItemsResponse<Plan> { Items = list };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }
        //[dbo].[Plans_Select_ByLanguage]

    }
}

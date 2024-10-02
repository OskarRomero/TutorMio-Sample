using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Responses;
using System.Data;
using System.Xml.Linq;
using TutorMioAPI1.Domain;
using TutorMioAPI1.Interfaces;
using TutorMioAPI1.Requests;


namespace TutorMioAPI1.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private ICoursesService _service = null;
        public CoursesController(ICoursesService service) 
        {
            _service = service;
        }

        //GET TOP 6 COURSES 
        [Authorize]
        [HttpGet("top")]
        public ActionResult<ItemsResponse<Course>> GetTopCourses()
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                List<Course> list = _service.GetTop6();
                if(list== null)
                {
                    response = new ErrorResponse("App resource not found..");
                    code = 404;
                }
                else
                    response = new ItemsResponse<Course> { Items = list };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        //GET BY ID
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Course>> GetCourseById(int id)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                Course course = _service.GetById(id);
                if (course == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }

                else
                {
                    response = new ItemResponse<Course> { Item = course };
                }

            }
            catch (SqlException sqlEx)
            {
                iCode = 500;
                response = new ErrorResponse($"SqlException Error: ${sqlEx.Message}");
            }
            return StatusCode(iCode, response);
        }

        //GET COURSE DETAILS
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<ItemsResponse<CourseDetails>> GetCoursesWithClassDetails()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<CourseDetails> list = _service.GetCourseDetails();

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemsResponse<CourseDetails> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        //GET COURSE BY LANGUAGE
        [HttpGet("language")]
        [AllowAnonymous]
        public ActionResult<ItemsResponse<CourseDetails>> GetCourseByLanguage(string language)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<CourseDetails> list = _service.GetCourseByLang(language);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found.");
                }
                else
                {
                    response = new ItemsResponse<CourseDetails> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        //ADD NEW COURSE
        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(CourseAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int id = _service.Add(model);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created("", response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        //UPDATE COURSE
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateCourse(CourseUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
                try
                {
                _service.Update(model);
                response = new SuccessResponse();
                }
                catch (Exception ex)
                {
                    code =500;
                    response= new ErrorResponse(ex.Message);
                }
            return StatusCode(code, response);

        }

        //DELETE COURSE
        [HttpDelete("{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateStatus(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }
    }  
}

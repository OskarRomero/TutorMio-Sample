using TutorMioAPI1.Domain;
using TutorMioAPI1.Requests;

namespace TutorMioAPI1.Interfaces
{
    public interface ICoursesService
    {
        int Add(CourseAddRequest model);
        void Delete(int id);
        Course GetById(int id);
        List<Course> GetTop6();
        List<CourseDetails> GetCourseDetails();
        List<CourseDetails> GetCourseByLang(string language);
        void Update(CourseUpdateRequest model);
    }
}
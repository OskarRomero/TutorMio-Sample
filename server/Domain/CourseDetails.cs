using TutormioAPI1.Domain;

namespace TutorMioAPI1.Domain
{
    public class CourseDetails
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImgUrl { get; set; }
        public string? Description { get; set; }
        public List<CourseClass>? CourseClasses { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

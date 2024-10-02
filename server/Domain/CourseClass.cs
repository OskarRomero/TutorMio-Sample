namespace TutormioAPI1.Domain
{
    public class CourseClass
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public float Hours { get; set; }
        public string? Description { get; set; }
        public string? ImgUrl { get; set; }
        public List<Feature>? Features { get; set; }

    }
}

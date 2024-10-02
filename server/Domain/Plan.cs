namespace TutorMioAPI1.Domain
{
    public class Plan
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public int Price { get; set; }
        public string? Duration { get; set; }
        public string? Features { get; set; }
        public bool IsRecommended { get; set; }
        public string? Language { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

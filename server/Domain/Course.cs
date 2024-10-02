using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorMioAPI1.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImgUrl { get; set; }
        public string? Description { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

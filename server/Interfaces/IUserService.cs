using TutorMioAPI1.Domain;
using TutorMioAPI1.Requests;

namespace TutorMioAPI1.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<string> GenerateJwtToken(User user);
        public int Register(RegisterRequest request);
    }
}
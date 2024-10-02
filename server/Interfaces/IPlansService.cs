using TutorMioAPI1.Domain;

namespace TutorMioAPI1.Interfaces
{
    public interface IPlansService
    {
        List<Plan> GetAllPlans();
        List<Plan> GetAllPlansByLang(string language);
    }
}
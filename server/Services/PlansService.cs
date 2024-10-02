using Microsoft.Data.SqlClient;
using System.Data;
using TutorMioAPI1.Domain;
using TutorMioAPI1.Extensions;
using TutorMioAPI1.Interfaces;

namespace TutorMioAPI1.Services
{
    public class PlansService : IPlansService
    {
        IDataProvider _data = null;
        public PlansService(IDataProvider provider)
        {
            _data = provider;
        }

        public List<Plan> GetAllPlans()
        {
            List<Plan> list = null;
            string procName = "[dbo].[Plans_Select_All]";
            Plan plan = null;

            _data.ExecuteCmd(procName, inputParamMapper: null
                , singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    Plan plan = MapSinglePlan(reader, ref startingIndex);
                    if (list == null)
                    {
                        list = new List<Plan>();
                    }
                    list.Add(plan);
                });

            return list;
        }
        public List<Plan> GetAllPlansByLang(string language)
        {
            List<Plan> list = null;
            string procName = "[dbo].[Plans_Select_ByLanguage]";
            Plan plan = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Language", language);
            }, delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    Plan plan = MapSinglePlan(reader, ref startingIndex);
                    if (list == null)
                    {
                        list = new List<Plan>();
                    }
                    list.Add(plan);
                });

            return list;
        }

        private static Plan MapSinglePlan(IDataReader reader, ref int startingIndex)
        {
            Plan plan = new Plan();
            plan.Id = reader.GetSafeInt32(startingIndex++);
            plan.Name = reader.GetString(startingIndex++);
            plan.Icon = reader.GetString(startingIndex++);
            plan.Price = reader.GetSafeInt32(startingIndex++);
            plan.Duration = reader.GetString(startingIndex++);
            plan.Features = reader.GetString(startingIndex++);
            plan.IsRecommended = reader.GetSafeBool(startingIndex++);
            plan.Language = reader.GetString(startingIndex++);
            plan.DateModified = reader.GetSafeDateTime(startingIndex++);
            plan.DateCreated = reader.GetSafeDateTime(startingIndex++);
            return plan;
        }
    }
}

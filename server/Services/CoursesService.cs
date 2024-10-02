
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutormioAPI1.Domain;
using TutorMioAPI1.Domain;
using TutorMioAPI1.Extensions;
using TutorMioAPI1.Interfaces;
using TutorMioAPI1.Requests;

namespace TutorMioAPI1.Services
{
    public class CoursesService : ICoursesService
    {
        IDataProvider _data = null;
        public CoursesService(IDataProvider provider) 
        {
            _data = provider;
        }
        public Course GetById(int id)
        {
            string procName = "[dbo].[Courses_Select_ById]";
            Course? course = null;
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                course = MapSingleCourse(reader, ref startingIndex );
            });

            return course;
        }

        public List<Course> GetTop6()
        {
            List<Course> list = null;
            string procName = "[dbo].[Courses_Select_Top6]";
            Course? course = null;

            _data.ExecuteCmd(procName, inputParamMapper: null
                , singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    Course? course = MapSingleCourse(reader,ref startingIndex);
                    if (list == null)
                    {
                        list = new List<Course>();
                    }
                    list.Add(course);
                });

            return list;
        }

        public List<CourseDetails> GetCourseDetails()
        {
            List<CourseDetails> list = null;
            string procname = "[dbo].[Course_Select_Detailed]";

            _data.ExecuteCmd(procname, delegate (SqlParameterCollection paramCollection)
            {
               
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                CourseDetails? courseItem = MapSingleCourseWithDetails(reader, ref startingIndex);
                if (list == null)
                {
                    list = new List<CourseDetails>();
                }
                list.Add(courseItem);
            });
            return list;
        }   
        public List<CourseDetails> GetCourseByLang(string language)
        {
            List<CourseDetails> list = null;
            string procname = "[dbo].[Course_Select_ByLanguage]";

            _data.ExecuteCmd(procname, delegate (SqlParameterCollection paramCollection)
            {
               paramCollection.AddWithValue("@Language",language);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                CourseDetails? courseItem = MapSingleCourseWithDetails(reader, ref startingIndex);
                if (list == null)
                {
                    list = new List<CourseDetails>();
                }
                list.Add(courseItem);
            });
            return list;
        }
        private static CourseDetails MapSingleCourseWithDetails(IDataReader reader, ref int startingIndex)
        {
            CourseDetails? course = new CourseDetails();
            course.Id = reader.GetSafeInt32(startingIndex++);
            course.Name = reader.GetString(startingIndex++);
            course.ImgUrl = reader.GetString(startingIndex++);
            course.Description = reader.GetString(startingIndex++);
            course.CourseClasses = reader.DeserializeObject<List<CourseClass>>(startingIndex++);
            course.DateModified = reader.GetDateTime(startingIndex++);
            course.DateCreated = reader.GetDateTime(startingIndex++);
            return course;
        }

        public int Add(CourseAddRequest model)
        {
            int id = 0;
            string procName = "[dbo].[Courses_Insert]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out id);

            });

            return id;
        }

        public void Update(CourseUpdateRequest model)
        {
            string procName = "[dbo].[Courses_Update]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                col.AddWithValue("@Id", model.Id);

            }, returnParameters: null);
        }

        public void Delete(int id)
        {
            string procName = "[dbo].[Courses_Delete_ById]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);

            }, returnParameters: null
            );
        }

        private static Course MapSingleCourse(IDataReader reader, ref int startingIndex)
        {
            Course course;
            
            course = new Course();
            course.Id = reader.GetSafeInt32(startingIndex++);
            course.Name = reader.GetString(startingIndex++);
            course.ImgUrl = reader.GetString(startingIndex++);
            course.Description = reader.GetString(startingIndex++);
            course.DateModified = reader.GetDateTime(startingIndex++);
            course.DateCreated = reader.GetDateTime(startingIndex++);
            return course;
        }

      

        private static void AddCommonParams(CourseAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@Name", model.Name);
            col.AddWithValue("@ImgUrl", model.ImgUrl);
            col.AddWithValue("@Description", model.Description);
        }
    }
}

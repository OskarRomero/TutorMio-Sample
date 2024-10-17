
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

        public int AddWithClasses(CourseAddRequest model)
        {
            int id = 0;
            DataTable myParamValue = null;
            if (model.Classes != null)
            {
                myParamValue = MapClassesToTable(model.Classes);
            }
            _data.ExecuteNonQuery("[dbo].[Courses_Insert_DetailedV2]",
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Name", model.Name);
                    col.AddWithValue("@ImgUrl", model.ImgUrl);
                    col.AddWithValue("@Description", model.Description);
                    col.AddWithValue("@Language", model.Language);
                    col.AddWithValue("@Classes", myParamValue);
                    SqlParameter idOut = new SqlParameter("@CourseId", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    col.Add(idOut);
                },  returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@CourseId"].Value;
                    int.TryParse(oId.ToString(), out id);
                });
            return id;

        }
        private DataTable MapClassesToTable(List<ClassAddRequest> classes)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ClassTitle", typeof(string));
            dt.Columns.Add("ClassHours", typeof(float));
            dt.Columns.Add("ClassDescription",typeof(string));
            dt.Columns.Add("ClassImgUrl",typeof (string));
            dt.Columns.Add("FeatureOne", typeof(string));
            dt.Columns.Add("FeatureTwo", typeof(string));
            dt.Columns.Add("FeatureThree", typeof(string));
            dt.Columns.Add("FeatureFour", typeof(string));
            
            if (classes != null)
            {
                foreach(ClassAddRequest c in classes)
                {
                    DataRow dr = dt.NewRow();
                    int idx = 0;
                    dr.SetField(idx++, c.Title);
                    dr.SetField(idx++, c.Hours);
                    dr.SetField(idx++, c.Description);
                    dr.SetField(idx++, c.ImgUrl);
                    dr.SetField(idx++, c.FeatureOne);
                    dr.SetField(idx++, c.FeatureTwo);
                    dr.SetField(idx++, c.FeatureThree);
                    dr.SetField(idx++, c.FeatureFour);
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        public void Update(CourseUpdateRequest model)
        {
            DataTable myParamValue = null;
            if (model.Classes != null)
            {
                myParamValue = MapClassesToTable(model.Classes);
            }
            _data.ExecuteNonQuery("[dbo].[Courses_Update_Detailed]",
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@CourseId", model.Id);
                    col.AddWithValue("@Name", model.Name);
                    col.AddWithValue("@ImgUrl", model.ImgUrl);
                    col.AddWithValue("@Description", model.Description);
                    col.AddWithValue("@Language", model.Language);
                    col.AddWithValue("@Classes", myParamValue);
                }, returnParameters: null);
        }

        public void Delete(int id)
        {
            string procName = "[dbo].[Courses_DeleteDetails_ById]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@CourseId", id);

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

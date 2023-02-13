using DpimProject.Models.Data;
using DpimProject.Models.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace DpimProject.Models.Department
{
    public class Department
    {
        //public dynamic DepartmentReadList(ref string errorMsg)
        public dynamic DepartmentReadList(int? user_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var get_role = db.user.Where(w => w.id == user_id).Select(s => s.role_id).FirstOrDefault();
                    var data = db.department
                        .Where(w => (get_role == 3 ? w.id != null : db.group_department.Where(ww => ww.user_id == user_id.ToString() && ww.depart_id == w.id).Count() > 0))
                        .OrderByDescending(o => o.update_dt)
                        .Select(s => new {
                        s.id,
                        s.department_name,
                        s.create_by,
                        s.create_dt,
                        s.update_by,
                        s.update_dt,
                        s.department_name_short,
                        enable_delete = db.course.Where(w => w.is_deleted == 0 && w.department_id == s.id).Count() > 0 ? false : true
                    }).ToList();
                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message; ErrorList(ex);
                }
            }
            return output;
        }
        
        public dynamic DepartmentManage(department d, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.department.Where(x => x.id == d.id).FirstOrDefault();
                    if (data != null)
                    {
                        data.department_name = d.department_name;
                        data.department_name_short = d.department_name_short;
                        data.update_by = auth.user_id;
                        data.update_dt = DateTime.Now;

                        db.SaveChanges();
                    }
                    else
                    {
                        department department = new department();
                        department.id = d.id;
                        department.department_name = d.department_name;
                        department.department_name_short = d.department_name_short;
                        department.create_by = auth.user_id;
                        department.create_dt = DateTime.Now;
                        department.update_by = auth.user_id;
                        department.update_dt = DateTime.Now;
                        

                        db.department.Add(department);
                        db.SaveChanges();
                    }

                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message; ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic RemoveDepartment(string id, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.department.Where(x => x.id == id).FirstOrDefault();
                    if (data == null)
                    {
                        errorMsg = "Id Invalid";
                        return false;
                    }

                    db.department.Remove(data);
                    db.SaveChanges();

                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message; ErrorList(ex);
                }
            }
            return output;
        }

        private HttpResponseException ErrorList(Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();                // Get the top stack frame
            string stackIndent = ex.StackTrace;
            var errorException = new
            {
                ex.Message,
                FileName = frame.GetFileName(),
                line = frame.GetFileLineNumber(),
                Method = frame.GetMethod()
            };
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                ReasonPhrase = ex.Message
            };


            throw new HttpResponseException(resp);
        }
    }
}
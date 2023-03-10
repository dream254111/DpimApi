using DpimProject.Models.DataTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DpimProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DepartmentController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Department.Department department;
        private DataTools dtl;
        private Models.Student student;

        public DepartmentController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            department = new Models.Department.Department();
        }

        private void GetToken(ref string errorMsg)
        {
            string token = "";
            try
            {
                var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
                if (cookie != null)
                {
                    token = cookie?.ToString();
                    auth.GetAuthentication(token);

                }
                else
                {
                    throw new Exception("Token Not Found");
                }
            }
            catch (Exception ex)
            {

                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();                // Get the top stack frame
                string stackIndent = ex.StackTrace;
                var error = new
                {
                    success = string.IsNullOrEmpty(ex.Message),
                    ex.Message,
                    FileName = frame.GetFileName(),
                    line = frame.GetFileLineNumber(),
                    Method = frame.GetMethod()
                };
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new ObjectContent<object>(error, new JsonMediaTypeFormatter()),
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(resp);
            }
        }

        [ActionName("DepartmentReadList")]
        [HttpGet]
        public dynamic DepartmentReadList()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);
            //var data = (string.IsNullOrEmpty(error)) ? department.DepartmentReadList(ref error) : null;
            var data = (string.IsNullOrEmpty(error)) ? department.DepartmentReadList(auth.user_id, ref error) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }
        [ActionName("DepartmentManage")]
        [HttpPost]
        public dynamic DepartmentManage()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);

            dynamic output = null;

            var m = new Models.Data.DataModels.department();
            m = Dtl.json_to_object(Dtl.json_request(), m);
            var data = (string.IsNullOrEmpty(error)) ? department.DepartmentManage(m, auth, ref error) : null;
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }

        [ActionName("RemoveDepartment")]
        [HttpPut]
        public dynamic RemoveDepartment()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);

            dynamic output = null;

            var model = new
            {
                id = ""
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            
            var data = department.RemoveDepartment(get_body.id, auth, ref error);
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }
    }
}

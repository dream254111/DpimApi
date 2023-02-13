using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using DpimProject.Models.DataTools;
namespace DpimProject.Controllers
{
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        private Models.Authentication.Authentication auth;
        private string token = "";
        private Models.DataTools.DataTools dtl;
        public SuperAdminController()
        {
            auth = new Models.Authentication.Authentication();
            dtl = new DataTools();
        }
        private void GetToken(ref string errorMsg)
        {
            string token = "";

            var cookie = System.Web.HttpContext.Current.Request.Cookies["dpim_auth"]?.Value?.Trim();
            if (cookie != null)
            {
                token = cookie?.ToString();
                auth.GetAuthentication(token);
                auth.IsAdmin = true;

            }
            else
            {
                throw new Exception("Token Not Found");
            }





        }

        public ActionResult Login()
        {
            string error = "";

            ViewBag.userAllow = true;


            var f = new Models.Data.DataModels.users();


            var m = new
            {
                username = "",
                password = ""

            };
            m = Dtl.json_to_object(Dtl.json_request(), m);
            string token_text = "";
            auth.LogIn(m.username, m.password, false,ref token_text, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error
            };

            return Content(JsonConvert.SerializeObject(output), "application/json");

        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SuperAdmin()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BehaviorsOfConcern.Domain.RepoServices;
using BehaviorsOfConcern.Domain.RepoServices.Abstract;
using BehaviorsOfConcern.Web.Models;

namespace BehaviorsOfConcern.Web.Controllers {
    public class IncidentController : Controller {
        private const string _submitter = "principalX";
        private ISchoolRepository _schoolRepository;

        public IncidentController(ISchoolRepository schoolRepository = null) {
            this._schoolRepository = schoolRepository;
            this._schoolRepository = new SchoolRepository("server=bvsd.infinitecampus.org,7771;database=boulder_valley_sandbox;Uid=Reporting;Pwd=R3p0rt!nG;");
        }



        public ActionResult List() {
            IncidentListViewModel vm = new IncidentListViewModel();
            vm.SchoolList = _schoolRepository.ReadSchools();
            return View(vm);
        }

        public ActionResult Edit() {
            return View("Edit_revC");
        }

        //identity info from IC comes encrypted inside the 'campus' query string parameter
        public ActionResult Authenticate(string campus, string authURI) {
            //ViewBag.ServiceURI = @"http://localhost:52549/api/incidents/3";
            //ViewBag.ServiceURI = @"http://localhost:52549/api/auth";
            //ViewBag.ServiceURI = @"http://localhost:56743/api/auth";  //TODO: get uri from config file? or from url query string?
            //pass campus token on to View, display status msg, and make async REST call there
            ViewBag.ICToken = campus;
            ViewBag.AuthURI = authURI;
            return View();
        }

        public JsonResult sampleData(string _) {
            string sample = OrthogonalSample(HttpContext.Server.MapPath(@"..\_sampleData\orthogonal_working.txt"))
                .Replace("[draw_value]", HttpContext.Request.Form["draw"]);

            var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            var obj = jss.Deserialize<object>(sample);

            var payload = (Dictionary<string, object>)obj;
            payload["data"] = ((object[])payload["data"])
                .Skip(int.Parse(HttpContext.Request.Form["start"]))
                .Take(int.Parse(HttpContext.Request.Form["length"]))
                .ToArray();

            return Json(obj);
        }

        private string OrthogonalSample(string sampleFilePath) {
            return System.IO.File
                .ReadAllText(sampleFilePath)
                .Replace("\r", "")
                .Replace("\n", "");
        }
    }
}

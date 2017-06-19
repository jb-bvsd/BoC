using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BehaviorsOfConcern.Web.Controllers {
    public class IncidentController : Controller {
        public ActionResult List() {
            return View();
        }

        public ActionResult Edit() {
            return View("Edit_revB");
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

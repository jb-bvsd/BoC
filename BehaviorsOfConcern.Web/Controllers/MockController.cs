using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BehaviorsOfConcern.Web.Controllers {
    public class MockController : Controller {
        public ActionResult Index() {
            return View("MockIC");
        }

        public ActionResult NewIncident() {
            return View("MockExternalEmployee_NewIncident");
        }
    }
}

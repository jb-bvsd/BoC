using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BehaviorsOfConcern.Web.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MockIC() {
            HttpUtility.ParseQueryString("");
            /*
                <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2017.2.504/styles/kendo.common-bootstrap.min.css" />
                <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2017.2.504/styles/kendo.bootstrap.min.css" />

                <link rel="stylesheet" href="http://kendo.cdn.telerik.com/2017.2.504/styles/kendo.default.min.css">
                <link rel="stylesheet" href="http://kendo.cdn.telerik.com/2017.2.504/styles/kendo.mobile.all.min.css">

                <script src="http://code.jquery.com/jquery-1.12.3.min.js"></script>
                <script src="http://kendo.cdn.telerik.com/2017.2.504/js/kendo.all.min.js"></script>
            */
            return View();
        }
    }
}

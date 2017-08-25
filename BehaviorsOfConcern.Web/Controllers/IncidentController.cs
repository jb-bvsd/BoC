using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BehaviorsOfConcern.Domain.DomainServices;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using BehaviorsOfConcern.Web.Models;

namespace BehaviorsOfConcern.Web.Controllers {
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class IncidentController : Controller {
        private ISchoolRepository _schoolRepository;
        private ILookupCodeCache _lookupCodeCache;
        private string _clearAuthenticationPrefix;

        public IncidentController(ISchoolRepository schoolRepository, ILookupCodeCache lookupCodeCache, string clearAuthenticationPrefix) {
            this._schoolRepository = schoolRepository;
            this._lookupCodeCache = lookupCodeCache;
            this._clearAuthenticationPrefix = clearAuthenticationPrefix;
        }


        public ActionResult List() {
            IncidentListViewModel vm = new IncidentListViewModel();
            vm.RecencyIntervalLookups = _lookupCodeCache.CategoryDictionary[LookupCategories.RecencyInterval];
            vm.IncidentCategoryLookups = _lookupCodeCache.CategoryDictionary[LookupCategories.IncidentCategory];
            vm.SourceLookups = _lookupCodeCache.CategoryDictionary[LookupCategories.Source];
            vm.StatusLookups = _lookupCodeCache.CategoryDictionary[LookupCategories.Status];
            vm.OutcomeLookups = _lookupCodeCache.CategoryDictionary[LookupCategories.Outcome];
            vm.SchoolList = _schoolRepository.ReadSchools().OrderBy(s => s.Name);
            return View(vm);
        }


        public ActionResult Edit() {
            IncidentListViewModel vm = new IncidentListViewModel();
            vm.IncidentCategoryLookups = _lookupCodeCache.CategoryDictionary[LookupCategories.IncidentCategory];
            vm.SourceLookups = _lookupCodeCache.CategoryDictionary[LookupCategories.Source];
            vm.StatusLookups = _lookupCodeCache.CategoryDictionary[LookupCategories.Status];
            vm.OutcomeLookups = _lookupCodeCache.CategoryDictionary[LookupCategories.Outcome];
            vm.SchoolList = _schoolRepository.ReadSchools().OrderBy(s => s.Name);
            return View(vm);
        }


        public ActionResult About() {
            return View();
        }


        public ActionResult UnknownIncident() {
            return View();
        }


        [Route("authenticate")]
        //identity info from IC comes encrypted inside the 'campus' query string parameter.
        //bypassCampus (coupled with proper web.config setting) overrides any campus argument - allows for easy troubleshooting:
        //  - web.config must have a non-null appSettings["clearAuthenticationPrefix"] entry.
        //  - it must match an identical setting in the webAPI service.
        //  - the http request must have non-null query string entries for personID and userID.
        //  - an http request query string entry for calendarID should be present for school admins.
        public ActionResult Authenticate(string bocApiUri, string campus, AuthOverrideViewModel bypassCampus) {
            //pass campus token on to View, display status msg, and make async REST call there
            ViewBag.ICToken = (bypassCampus == null) ? campus : bypassCampus.BuildToken(_clearAuthenticationPrefix) ?? campus;
            ViewBag.BoCApiUri = bocApiUri;
            return View();
        }
    }
}

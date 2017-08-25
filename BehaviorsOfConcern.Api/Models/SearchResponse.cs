using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace BehaviorsOfConcern.Api.Models {
    public class SearchResponse<T> {
        [JsonProperty(PropertyName = "draw")]  //DataTables.net expects lowercase property names
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "recordsTotal")]  //DataTables.net expects lowercase property names
        public int RecordsTotal { get; set; }

        [JsonProperty(PropertyName = "recordsFiltered")]  //DataTables.net expects lowercase property names
        public int RecordsFiltered { get; set; }

        [JsonProperty(PropertyName = "data")]  //DataTables.net expects lowercase property names
        public IList<T> Data { get; set; }
    }
}

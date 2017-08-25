using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace BehaviorsOfConcern.Api.Models {
    //extended DTO, augmented to receive custom BoC filtering :
    public class SearchRequest : DataTablesSearchRequest {
        public FilterRequestItem[] FilterStatus { get; set; }
        public FilterRequestItem[] FilterCategory { get; set; }
        public FilterRequestItem[] FilterOutcome { get; set; }
        public int? FilterRecency { get; set; }
        public int? FilterSchool { get; set; }
    }

    public class FilterRequestItem {
        public string Name { get; set; }
        public int Value { get; set; }
    }


    //DTO(s) structured to match out-of-the-box request from DataTables.net,  (see  ht tps://datatables.net/manual/server-side) :
    public class DataTablesSearchRequest {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public ColumnRequestItem[] Columns { get; set; }
        public OrderRequestItem[] Order { get; set; }
        public SearchRequestItem Search { get; set; }
    }

    public class ColumnRequestItem {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public SearchRequestItem Search { get; set; }
    }

    public class OrderRequestItem {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public class SearchRequestItem {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }
}
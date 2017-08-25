using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using Dapper;

namespace BehaviorsOfConcern.Domain.DomainServices {
    public class LookupCodeCache : RepositoryBase, ILookupCodeCache {
        public LookupCodeCache(string connectionString) {
            _connString = connectionString;
        }

        private Dictionary<int, LookupCode> _codeDictionary = new Dictionary<int, LookupCode>();
        public IDictionary<int, LookupCode> CodeDictionary {
            get { return _codeDictionary; }
        }

        private Dictionary<string, int> _codeEnumDictionary = new Dictionary<string, int>();
        public IDictionary<string, int> CodeEnumDictionary {
            get { return _codeEnumDictionary; }
        }

        private Dictionary<LookupCategories, IList<LookupCode>> _categoryDictionary = new Dictionary<LookupCategories, IList<LookupCode>>();
        public IDictionary<LookupCategories, IList<LookupCode>> CategoryDictionary {
            get { return _categoryDictionary; }
        }

        public void Refresh() {
            IEnumerable<LookupCode> lookupList;
            _codeDictionary.Clear();
            _codeEnumDictionary.Clear();
            _categoryDictionary.Clear();

            try {
                using (SqlConnection conn = new SqlConnection(_connString)) {
                    conn.Open();
                    lookupList = conn.Query<LookupCode>(@"
                        select CD as Value, CategoryCD as Category, Label, Description, DefaultYN, ActiveYN, SortOrder, ProgrammaticEnum
                          from BVSD_BoC_CodeSet");
                }

                var activeCodes = from code in lookupList
                                  where code.ActiveYN
                                  orderby code.Category, code.SortOrder, code.Value
                                  select code;

                foreach (var code in activeCodes) {
                    _codeDictionary.Add(code.Value, code);
                    if (!string.IsNullOrWhiteSpace(code.ProgrammaticEnum)) _codeEnumDictionary[code.ProgrammaticEnum] = code.Value;  //add/overwrite item to/in dictionary
                    if (!_categoryDictionary.ContainsKey(code.Category)) _categoryDictionary.Add(code.Category, new List<LookupCode>());
                    _categoryDictionary[code.Category].Add(code);
                }
            } catch (SqlException ex) {
                //TODO:  Log error details here
                //wrap sql exception to keep Data.SqlClient namespace out of domain tier(s)
                throw new Invalid​Operation​Exception("Problem accessing LookupCode storage", ex);
            }
        }
    }
}
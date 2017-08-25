using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorsOfConcern.Domain.DomainServices.Abstract {
    public interface ILookupCodeCache {
        IDictionary<int, LookupCode> CodeDictionary { get; }
        IDictionary<string, int> CodeEnumDictionary { get; }
        IDictionary<LookupCategories, IList<LookupCode>> CategoryDictionary { get; }
        void Refresh();
    }
}

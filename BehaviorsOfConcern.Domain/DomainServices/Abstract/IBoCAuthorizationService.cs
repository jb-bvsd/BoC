using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.Entities;

namespace BehaviorsOfConcern.Domain.DomainServices.Abstract {
    public interface IBoCAuthorizationService {
        BVSDAdmin ExtractExternalUser(string externalToken);
        BVSDAdmin ExtractUser(string bocToken);
        string BuildToken(BVSDAdmin sessionUser);
    }
}

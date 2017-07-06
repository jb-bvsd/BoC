﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.Entities;

namespace BehaviorsOfConcern.Domain.RepoServices.Abstract {
    public interface ISchoolRepository {
        School ReadSchool(int schoolID);
        IEnumerable<School> ReadSchools();
    }
}

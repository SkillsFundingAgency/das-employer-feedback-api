﻿using System.Collections.Generic;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetAttributes
{
    public class GetAttributesQueryResult
    {
        public List<Attribute> Attributes { get; set; }
    }
}

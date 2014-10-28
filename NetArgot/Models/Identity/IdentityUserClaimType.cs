using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;

namespace NetArgot.Models.Identity
{
    public class IdentityUserClaimType
    {
        public int TypeId { get; set; }
        public string ClaimTypeCode { get; set; }
        public string ClaimTypeDescription { get; set; }
    }
}

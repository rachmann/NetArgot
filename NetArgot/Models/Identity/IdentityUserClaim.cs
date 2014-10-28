using System;
using Microsoft.AspNet.Identity;
using Dapper;
using DapperExtensions;

namespace NetArgot.Models.Identity
{
    public class IdentityUserClaim
    {
        public int ClaimId { get; set; }
        public int UserId { get; set; }

        public int ClaimTypeId { get; set; }
        public string ClaimValue { get; set; }
        public string ClaimValueType { get; set; }
        public string Issuer { get; set; }
    }
 
}

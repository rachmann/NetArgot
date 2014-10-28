using System;
using Dapper;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using DapperExtensions;
using NetArgot.Identity;

namespace NetArgot.Models.Identity
{
    public class IdentityUser : IUser<int>
    {
 
        public IdentityUser()
        {
          //  Id = Guid.NewGuid().ToString();
        }
        public IdentityUser(string userName)
            : this()
        {
            UserName = userName;
        }
        public int Id { get; set; }                        
        public string Description { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string ApplicationName { get; set; }

    }
}

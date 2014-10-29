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
            Description = string.Empty;
            PhoneNumber = string.Empty;
            ApplicationName = string.Empty;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


        public IdentityUser(IdentityUser user)
        {
            this.Id = user.Id;
            this.Description = user.Description;
            this.UserName = user.UserName;
            this.Email = user.Email;
            this.EmailConfirmed = user.EmailConfirmed;
            this.PasswordHash = user.PasswordHash;
            this.SecurityStamp = user.SecurityStamp;
            this.PhoneNumber = user.PhoneNumber;
            this.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            this.TwoFactorEnabled = user.TwoFactorEnabled;
            this.LockoutEndDateUtc = user.LockoutEndDateUtc;
            this.LockoutEnabled = user.LockoutEnabled;
            this.AccessFailedCount = user.AccessFailedCount;
            this.ApplicationName = user.ApplicationName;
        }
        public IdentityUser(string userName)
            : this()
        {
            UserName = userName;
        }
        public int          Id                      { get; set; }                        
        public string       Description             { get; set; }
        public string       UserName                { get; set; }
        public string       Email                   { get; set; }
        public bool         EmailConfirmed          { get; set; }
        public string       PasswordHash            { get; set; }
        public string       SecurityStamp           { get; set; }
        public string       PhoneNumber             { get; set; }
        public bool         PhoneNumberConfirmed    { get; set; }
        public bool         TwoFactorEnabled        { get; set; }
        public DateTime?    LockoutEndDateUtc       { get; set; }
        public bool         LockoutEnabled          { get; set; }
        public int          AccessFailedCount       { get; set; }
        public string       ApplicationName         { get; set; }
    }

}

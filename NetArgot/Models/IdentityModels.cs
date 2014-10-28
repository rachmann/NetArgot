using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Dapper;
using NetArgot.Models;
using NetArgot.Models.Identity;
using NetArgot.Identity;
using NetArgot.DataStore;
namespace NetArgot.Models
{
    //public class ApplicationUserLogin : IdentityUserLogin<int> { }
    //public class ApplicationUserClaim : IdentityUserClaim<int> { }
    //public class ApplicationUserRole : IdentityUserRole<int> { }

    public class ApplicationRole : IdentityRole, IRole<int>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }

        public ApplicationRole(string name, string description)
            : this(name)
        {
            this.Description = description;
        }
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser, IUser<int>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext()
            : base()
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }


    public class ApplicationUserStore : IdentityUserStore<ApplicationUser>, IDisposable
    {
        public ApplicationUserStore()
            : this((DbConnection)(new ApplicationDbContext().Connection))
        {
            
        }

        public ApplicationUserStore(DbConnection connection)
            : base(connection)
        {
        }
    }


    public class ApplicationRoleStore :
        IdentityRoleStore<ApplicationRole>,
        IQueryableRoleStore<ApplicationRole, int>,
        IRoleStore<ApplicationRole, int>, IDisposable
    {
        public ApplicationRoleStore()
            : base()
        {
        }
        public ApplicationRoleStore(IdentityDbContext context)
            : base(context)
        {
        }
        public ApplicationRoleStore(DbConnection context)
            : base(context)
        {
        }
    }
}
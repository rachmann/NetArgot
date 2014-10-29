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


    public class ApplicationUserStore : IdentityUserStore<IdentityUser>, IDisposable
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
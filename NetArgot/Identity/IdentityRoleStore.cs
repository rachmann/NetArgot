using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using NetArgot.DataStore;
using NetArgot.Models;
using NetArgot.Models.Identity;

namespace NetArgot.Identity
{
    public class IdentityRoleStore<TRole> : IQueryableRoleStore<TRole, int>
     where TRole : IdentityRole
    {

        public IdentityDbContext Database { get; private set; }
        public IQueryable<TRole> Roles
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Default constructor that initializes a new database
        /// instance using the Default Connection string
        /// </summary>
        public IdentityRoleStore()
        {
            new IdentityRoleStore<TRole>(new IdentityDbContext());
        }

        /// <summary>
        /// Constructor that takes a connection string as argument 
        /// </summary>
        /// <param name="connection"></param>
        public IdentityRoleStore(string connection)
        {
            Database = new IdentityDbContext(connection);
        }

        /// <summary>
        /// Constructor that takes a IdentityDbContext as argument 
        /// </summary>
        /// <param name="database"></param>
        public IdentityRoleStore(IdentityDbContext database)
        {
            Database = database;
        }

        public IdentityRoleStore(DbConnection context)
        {
            Database = new IdentityDbContext(context);
        }

        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            //TODO Add Db stuff

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            //TODO Add Db stuff

            return Task.FromResult<Object>(null);
        }

        public Task<TRole> FindByIdAsync(int roleId)
        {
            //TRole result = roleTable.GetRoleById(roleId) as TRole;
            //TODO Add Db stuff

            TRole result = null;
            return Task.FromResult<TRole>(result);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            //TODO Add Db stuff

            TRole result = null;

            return Task.FromResult<TRole>(result);
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            //TODO Add Db stuff

            return Task.FromResult<Object>(null);
        }

        public void Dispose()
        {
            if (Database != null)
            {
                Database.Dispose();
                Database = null;
            }
        }

    }
}
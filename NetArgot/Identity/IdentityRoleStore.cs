using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using DapperExtensions;
using Microsoft.AspNet.Identity;
using NetArgot.DataStore;
using NetArgot.Models;
using NetArgot.Models.Identity;

namespace NetArgot.Identity
{
    public class IdentityRoleStore<TRole> :
        IQueryableRoleStore<TRole, int>
        where TRole : IdentityRole
    {

        public IdentityDbContext Database { get; private set; }
        public IQueryable<TRole> Roles
        {
            get
            {
                return Database.Connection.GetList<TRole>().AsQueryable();
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

            return Task.Factory.StartNew(() => Database.Connection.Insert<IdentityRole>(role));

        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return Task.Factory.StartNew(() => Database.Connection.Delete<IdentityRole>(role));
        }

        public Task<TRole> FindByIdAsync(int roleId)
        {
            return Task.Factory.StartNew(() => Database.Connection.Get<TRole>(roleId));
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            return Task.Factory.StartNew(() => Database.Connection.Query<TRole>("SELECT * FROM IdentityRole WHERE Name = @roleName", new { roleName }).FirstOrDefault());
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return Task.Factory.StartNew(() => Database.Connection.Update<IdentityRole>(role));
        }

        public void Dispose()
        {
            if (Database != null)
            {
                if (Database.Connection != null && Database.Connection.State == ConnectionState.Open)
                    Database.Connection.Close();
                Database.Dispose();
                Database = null;
            }
        }

    }
}
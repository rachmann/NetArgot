using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
//using NetArgot.Identity;
using NetArgot.Models;
using NetArgot.Models.Identity;
using System.Data.Common;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace NetArgot.Identity
{
 
    /// <summary>
    /// A simple database connection manager
    /// </summary>
    public class IdentityDbContext : IDisposable
    {
        private IDbConnection _conn { get; set; }

        /// <summary>
        /// Return open connection
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                if (_conn.State == ConnectionState.Closed)
                    _conn.Open();

                return _conn;
            }
        }

        /// <summary>
        /// Create a new Sql database connection
        /// </summary>
        /// <param name="connection">The database connection</param>
        public IdentityDbContext(DbConnection connection)
        {
            if (connection == null)
            {
                var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                
                Debug.WriteLine(connString);
                connection = new SqlConnection(connString);
            }
            _conn = connection;
        }

        /// <summary>
        /// Create a new Sql database connection
        /// </summary>
        /// <param name="connString">The name of the connection string</param>
        public IdentityDbContext(string connString = "")
        {
            // Use first?
            if (connString == "")
            {
                connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }

            Debug.WriteLine(connString);
            _conn = new SqlConnection(connString);
        }

        /// <summary>
        /// Close and dispose of the database connection
        /// </summary>
        public void Dispose()
        {
            if (_conn != null)
            {
                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                    _conn.Dispose();
                }
                _conn = null;
            }
        }
    }

}
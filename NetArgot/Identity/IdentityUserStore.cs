using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using DapperExtensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using NetArgot.Models;
using NetArgot.Models.Identity;

namespace NetArgot.Identity
{
    public class IdentityUserStore<TUser> :
        IUserLoginStore<TUser, int>,
        IUserClaimStore<TUser, int>,
        IUserPasswordStore<TUser, int>,
        IUserRoleStore<TUser, int>,
        IUserSecurityStampStore<TUser, int>,
        IUserPhoneNumberStore<TUser, int>,
        IUserEmailStore<TUser, int>,
        IUserTwoFactorStore<TUser, int>
        where TUser : IdentityUser //IQueryableUserStore<TUser, int>,
    {

        public DbConnection Connection { get; private set; }

        public IdentityUserStore(ApplicationDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Connection = (DbConnection)context.Connection;
        }

        public IdentityUserStore(DbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            Connection = connection;
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }
                Connection.Dispose();
            }
        }

        public Task CreateAsync(TUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                if (user != null)
                {
                    Connection.Insert<TUser>(user);
                }
            });
        }

        public Task UpdateAsync(TUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                if (user != null)
                {
                        Connection.Update<TUser>(user);
                }
            });
        }

        public Task DeleteAsync(TUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                if (user != null)
                {
                    Connection.Delete<TUser>(user);
                }
            });
        }

        public Task<TUser> FindByIdAsync(int userId)
        {
            return Task.Factory.StartNew(() => (Connection.Get<TUser>(userId)));
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            var predicate = Predicates.Field<TUser>(f => f.UserName, Operator.Eq, userName);
            return Task.Factory.StartNew(() => Connection.GetList<TUser>(predicate).FirstOrDefault());

            //return Task.Factory.StartNew(() => Connection.Query<TUser>("select * from IdentityUser where UserName = @userName", new { userName }).FirstOrDefault());
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this user exist?
                var userItem = Connection.Get<TUser>(user.Id);
                if (userItem != null)
                {
                    //var userLoginItem = Connection.Query<IdentityUserLogin>("SELECT * FROM IdentityUserLogin WHERE UserId = @Id AND ProviderKey = @ProviderKey", new { user.Id, login.ProviderKey}).FirstOrDefault();
                    
                    var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pg.Predicates.Add(Predicates.Field<IdentityUserLogin>(f => f.UserId, Operator.Eq, user.Id));
                    pg.Predicates.Add(Predicates.Field<IdentityUserLogin>(f => f.ProviderKey, Operator.Eq, login.ProviderKey));
                    var userLoginItem = Connection.GetList<IdentityUserLogin>(pg).FirstOrDefault();
                    
                    if (userLoginItem == null)
                    {
                        Connection.Insert(
                            new IdentityUserLogin
                            {
                                UserId = user.Id,
                                LoginProvider = login.LoginProvider,
                                ProviderKey = login.ProviderKey
                            });
                    }
                }
            });
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this user exist?
                var userItem = Connection.Get<TUser>(user.Id);
                if (userItem != null)
                {
                    //var userLoginItem = Connection.Query<IdentityUserLogin>("SELECT * FROM IdentityUserLogin WHERE UserId = @Id AND ProviderKey = @ProviderKey", new { user.Id, login.ProviderKey }).FirstOrDefault();

                    var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pg.Predicates.Add(Predicates.Field<IdentityUserLogin>(f => f.UserId, Operator.Eq, user.Id));
                    pg.Predicates.Add(Predicates.Field<IdentityUserLogin>(f => f.ProviderKey, Operator.Eq, login.ProviderKey));

                    var userLoginItem = Connection.GetList<IdentityUserLogin>(pg).FirstOrDefault();
                    if (userLoginItem != null)
                    {
                        Connection.Delete(userLoginItem);
                    }
                }
            });
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                List<UserLoginInfo> logins = null;
                var userItem = Connection.Get<TUser>(user.Id);
                if (userItem != null)
                {
                    // logins = Connection.Query<IdentityUserLogin>("SELECT * FROM IdentityUserLogin WHERE UserId = @Id", new { user.Id }).Select(culi => new UserLoginInfo(culi.LoginProvider, culi.ProviderKey)).ToList();
                    var predicate = Predicates.Field<IdentityUserLogin>(f => f.UserId, Operator.Eq, user.Id);
                    logins = Connection.GetList<IdentityUserLogin>(predicate).Select(culi => new UserLoginInfo(culi.LoginProvider, culi.ProviderKey)).ToList();
                }

                return (IList<UserLoginInfo>)logins;
            });
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            return Task.Factory.StartNew(() =>
            {
                TUser user = null;
                // var userLoginItem = Connection.Query<IdentityUserLogin>("SELECT * FROM IdentityUserLogin WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey", new { login.LoginProvider, login.ProviderKey }).FirstOrDefault();

                var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                pg.Predicates.Add(Predicates.Field<IdentityUserLogin>(f => f.LoginProvider, Operator.Eq, login.LoginProvider));
                pg.Predicates.Add(Predicates.Field<IdentityUserLogin>(f => f.ProviderKey, Operator.Eq, login.ProviderKey));

                var userLoginItem = Connection.GetList<IdentityUserLogin>(pg).FirstOrDefault();
                if (userLoginItem != null)
                {
                    user = Connection.Get<TUser>(userLoginItem.UserId);
                }

                return user;
            });
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this user exist?
                List<Claim> claims = null;
                var userItem = Connection.Get<TUser>(user.Id);
                if (userItem != null)
                {
                    claims = Connection.Query<IdentityUserClaimJoined>("SELECT cuc.*, cuct.ClaimTypeCode FROM IdentityUserClaim cuc INNER JOIN IdentityUserClaimType cuct ON cuc.ClaimTypeId = cuct.TypeId WHERE UserId = @Id;",
                    new { user.Id }).Select(cuc =>
                        new Claim(cuc.ClaimTypeCode, cuc.ClaimValue, cuc.ClaimValueType, cuc.Issuer))
                           .ToList();
                }

                return (IList<Claim>)claims;
            });
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this user exist?
                var userItem = Connection.Get<TUser>(user.Id);
                if (userItem != null)
                {
                    var oldClaim =
                        Connection.Query<IdentityUserClaim>(
                            "SELECT cuc.* FROM IdentityUserClaim cuc " +
                            "INNER JOIN IdentityUserClaimType cuct ON cuc.ClaimTypeId = cuct.TypeId " +
                               "WHERE cuc.UserId = @Id AND " +
                               "cuct.ClaimTypeCode = @Type AND " +
                               "cuc.ClaimValue = @Value AND " +
                               "cuc.Issuer = @Issuer;",
                            new { user.Id, claim.Type, claim.Value, claim.Issuer })
                            .FirstOrDefault();
                    if (oldClaim == null)
                    {

                        var predicate = Predicates.Field<IdentityUserClaimType>(f => f.ClaimTypeCode, Operator.Eq, claim.Type );
                        var theClaimType = Connection.GetList<IdentityUserClaimType>(predicate).FirstOrDefault();
                        //var theClaimType = Connection.Query<IdentityUserClaimType>("SELECT * FROM IdentityUserClaimType WHERE cuct.IdentityUserClaimTypeCode = @Type;",new { claim.Type }).FirstOrDefault();

                        if (theClaimType != null)
                        {
                            Connection.Insert(new IdentityUserClaim
                            {
                                UserId = user.Id,
                                ClaimValueType = claim.ValueType,
                                ClaimTypeId = theClaimType.TypeId,
                                ClaimValue = claim.ValueType,
                                Issuer = claim.Issuer
                            });
                        }
                    }
                }
                return Task.FromResult(0);
            });
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this user exist?
                var userItem = Connection.Get<TUser>(user.Id);
                var predicate = Predicates.Field<IdentityUserClaimType>(f => f.ClaimTypeCode, Operator.Eq, claim.Type);
                var theClaimType = Connection.GetList<IdentityUserClaimType>(predicate).FirstOrDefault();

                //var theClaimType = Connection.Query<IdentityUserClaimType>("SELECT * FROM IdentityUserClaimType WHERE cuct.ClaimTypeCode = @Type;",new { claim.Type }).FirstOrDefault();

                if (userItem != null && theClaimType != null)
                {
                    //var oldClaim = Connection.Query<IdentityUserClaim>("SELECT * FROM [dbo].[IdentityUserClaim] WHERE UserId = @Id AND ClaimTypeId = @TypeId AND ClaimValue = @Value; AND CloudUserClaimDesc = @Issuer;",
                    //    new { user.Id, theClaimType.TypeId, claim.Value, claim.Issuer }).FirstOrDefault();
                    var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pg.Predicates.Add(Predicates.Field<IdentityUserClaim>(f => f.UserId, Operator.Eq, user.Id));
                    pg.Predicates.Add(Predicates.Field<IdentityUserClaim>(f => f.ClaimTypeId, Operator.Eq, theClaimType.TypeId));
                    pg.Predicates.Add(Predicates.Field<IdentityUserClaim>(f => f.ClaimValue, Operator.Eq, claim.Value));
                    pg.Predicates.Add(Predicates.Field<IdentityUserClaim>(f => f.Issuer, Operator.Eq, claim.Issuer));
                    var oldClaim = Connection.GetList<IdentityUserClaim>(pg).FirstOrDefault();

                    if (oldClaim != null)
                    {
                        Connection.Delete(oldClaim);
                    }
                }
            });
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            return Task.FromResult(user.PasswordHash = passwordHash);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this role exist?
                var predicateRole = Predicates.Field<IdentityRole>(f => f.Name, Operator.Eq, roleName);
                var roleItem = Connection.Get<IdentityRole>(predicateRole);
                // var roleItem = Connection.Query<IdentityRole>("SELECT * FROM IdentityRole WHERE Name = @roleName", new { roleName }).FirstOrDefault();

                if (roleItem != null)
                {
                    //does this user & role combo already exist?
                    // var roleUserItem = Connection.Query<IdentityUserRole>("SELECT * FROM IdentityUserRole WHERE UserId = @Id AND RoleId = @RoleId", new { user.Id, RoleId = roleItem.Id }).FirstOrDefault();
                    
                    var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pg.Predicates.Add(Predicates.Field<IdentityUserRole>(f => f.UserId, Operator.Eq, user.Id));
                    pg.Predicates.Add(Predicates.Field<IdentityUserRole>(f => f.RoleId, Operator.Eq, roleItem.Id));
                    var roleUserItem = Connection.GetList<IdentityUserRole>(pg).FirstOrDefault();
                    if (roleUserItem == null)
                    {
                        // no - so add
                        Connection.Insert(new IdentityUserRole { UserId = user.Id, RoleId = roleItem.Id });
                    }
                }
            });
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this role exist?
                var predicateRole = Predicates.Field<IdentityRole>(f => f.Name, Operator.Eq, roleName);
                var roleItem = Connection.Get<IdentityRole>(predicateRole);
                //var roleItem = Connection.Query<IdentityRole>("SELECT * FROM IdentityRole WHERE Name = @roleName", new { roleName }).FirstOrDefault();

                if (roleItem != null)
                {
                    //does this user & role combo already exist?
                    //var roleUserItem = Connection.Query<IdentityUserRole>("SELECT * FROM IdentityUserRole WHERE UserId = @Id AND RoleId = @RoleId", new { user.Id, RoleId = roleItem.Id }).FirstOrDefault();
                    var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pg.Predicates.Add(Predicates.Field<IdentityUserRole>(f => f.UserId, Operator.Eq, user.Id));
                    pg.Predicates.Add(Predicates.Field<IdentityUserRole>(f => f.RoleId, Operator.Eq, roleItem.Id));
                    var roleUserItem = Connection.GetList<IdentityUserRole>(pg).FirstOrDefault();

                    if (roleUserItem != null)
                    {
                        // yes - so delete
                        Connection.Delete(roleUserItem);
                    }

                }
            });
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this role exist?
                //does this user & role combo already exist?
                var results = Connection.Query<IdentityRole>(@"SELECT IdentityRole.* FROM IdentityUserRole ur
                             INNER JOIN IdentityRole on IdentityRole.Id = ur.RoleId WHERE ur.UserId = @Id", new { user.Id }).ToList();

                var retList = results.Select(r => r.Name).ToList();
                return (IList<string>)retList;
            });
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this role exist?
                var result = false;
                //var predicateRole = Predicates.Field<IdentityRole>(f => f.Name, Operator.Eq, roleName);
                //var roleItem = Connection.Get<IdentityRole>(predicateRole);

                var roleItem = Connection.Query<IdentityRole>("SELECT * FROM IdentityRole WHERE Name = @roleName", new { roleName }).FirstOrDefault();
                if (roleItem != null)
                {
                    //does this user & role combo already exist?
                    // var roleUserItem = Connection.Query<IdentityUserRole>("SELECT * FROM IdentityUserRole WHERE UserId = @Id AND RoleId = @RoleId", new { user.Id, RoleId = roleItem.Id }).FirstOrDefault();
                    var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pg.Predicates.Add(Predicates.Field<IdentityUserRole>(f => f.UserId, Operator.Eq, user.Id));
                    pg.Predicates.Add(Predicates.Field<IdentityUserRole>(f => f.RoleId, Operator.Eq, roleItem.Id));
                    var roleUserItem = Connection.GetList<IdentityUserRole>(pg);

                    if (roleUserItem != null)
                    {
                        result = true;
                    }
                }
                return result;
            });
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            return Task.Factory.StartNew(() => user.SecurityStamp = stamp);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            return Task.Factory.StartNew(() => user.PhoneNumber = phoneNumber);
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            return Task.Factory.StartNew(() => user.PhoneNumberConfirmed = confirmed);
        }

        //public IQueryable<TUser> Users {
        //    get
        //    {
        //        return Connection.GetList<TUser>().AsQueryable();   
        //    }
        //    private set
        //    {
                
        //    }}

        public Task SetEmailAsync(TUser user, string email)
        {
            return Task.Factory.StartNew(() => user.Email = email);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            return Task.Factory.StartNew(() => user.EmailConfirmed = confirmed);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            var predicateUser = Predicates.Field<TUser>(f => f.Email, Operator.Eq, email);
            return Task.Factory.StartNew(() => Connection.GetList<TUser>(predicateUser).FirstOrDefault());
            // return Task.Factory.StartNew(() => Connection.Query<TUser>("SELECT * FROM IdentityUser WHERE Email = @email", new { email }).FirstOrDefault());
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            return Task.Factory.StartNew(() => user.TwoFactorEnabled = enabled);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
    }
}
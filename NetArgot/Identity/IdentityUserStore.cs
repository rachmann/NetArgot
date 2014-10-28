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
        IQueryableUserStore<TUser, int>,
        IUserEmailStore<TUser, int>,
        IUserTwoFactorStore<TUser, int>
        where TUser : IdentityUser
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
                    Connection.Insert(user);
                }
            });
        }

        public Task UpdateAsync(TUser user)
        {

            return Task.Factory.StartNew(() =>
            {
                if (user != null)
                {
                    Connection.Update(user);
                }
            });
        }

        public Task DeleteAsync(TUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                if (user != null)
                {
                    Connection.Delete(user);
                }
            });
        }

        public Task<TUser> FindByIdAsync(int userId)
        {
            return Task.Factory.StartNew(() => Connection.Get<TUser>(userId));
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            return Task.Factory.StartNew(() => Connection.Query<TUser>("SELECT * FROM IdentityUser WHERE UserName = @userName", new { userName }).FirstOrDefault());
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this user exist?
                var userItem = Connection.Get<TUser>(user.Id);
                if (userItem != null)
                {
                    var userLoginItem =
                        Connection.Query<IdentityUserLogin>(
                            "SELECT * FROM IdentityUserLogin WHERE UserId = @Id AND ProviderKey = @ProviderKey",
                            new { user.Id, login.ProviderKey }).FirstOrDefault();
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
                var userItem = Connection.Get<IdentityUser>(user.Id);
                if (userItem != null)
                {
                    var userLoginItem = Connection.Query<IdentityUserLogin>("SELECT * FROM IdentityUserLogin WHERE UserId = @Id AND ProviderKey = @ProviderKey", new { user.Id, login.ProviderKey }).FirstOrDefault();

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
                //does this user exist?
                List<UserLoginInfo> logins = null;
                var userItem = Connection.Get<IdentityUser>(user.Id);
                if (userItem != null)
                {
                    logins = Connection.Query<IdentityUserLogin>("SELECT * FROM [dbo].[IdentityUserLogin] WHERE UserId = @Id;", new { user.Id }).Select(culi => new UserLoginInfo(culi.LoginProvider, culi.ProviderKey)).ToList();
                }

                return (IList<UserLoginInfo>)logins;
            });
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this user exist?
                IdentityUser user = null;
                var userLoginItem = Connection.Query<IdentityUserLogin>("SELECT * FROM [dbo].[IdentityUserLogin] WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey;", new { login.LoginProvider, login.ProviderKey }).FirstOrDefault();
                if (userLoginItem != null)
                {
                    user = Connection.Get<IdentityUser>(userLoginItem.UserId);
                }

                return (TUser)user;
            });
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this user exist?
                List<Claim> claims = null;
                var userItem = Connection.Get<IdentityUser>(user.Id);
                if (userItem != null)
                {

                    claims = Connection.Query<IdentityUserClaimJoined>("SELECT cuc.*, cuct.ClaimTypeCode FROM IdentityUserClaim cuc INNER JOIN IdentityUserClaimType cuct ON cuc.ClaimTypeId = cuct.TypeId WHERE UserId = @Id;",
                    new { user.Id }).Select(cuc =>
                        new Claim(cuc.ClaimTypeCode, cuc.ClaimValue, cuc.ClaimValueType, cuc.Issuer))
                           .ToList();
                    //var xclaims = _sqlConn.Query<IdentityUserClaim>("select * from [dbo].[IdentityUserClaim] where IdentityUserId = @IdentityUserId;",  new { user.IdentityUserId}).ToList();
                    //if (xclaims.Count > 0)
                    //{
                    //    claims = new List<Claim>();
                    //    foreach (var cuc in xclaims)
                    //    {
                    //        var ct = types.FirstOrDefault(t => t.IdentityUserClaimTypeId == cuc.IdentityUserClaimTypeId);
                    //        claims.Add(new Claim(ct.IdentityUserClaimTypeCode, cuc.IdentityUserClaimValue, cuc.IdentityUserClaimCode, cuc.IdentityUserClaimDesc));
                    //    }
                    //}


                }

                return (IList<Claim>)claims;
            });
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            return Task.Factory.StartNew(() =>
            {
                //does this user exist?
                var userItem = Connection.Get<IdentityUser>(user.Id);
                if (userItem != null)
                {
                    // IdentityUserClaimType  = Claim.Type
                    // IdentityUserClaimValue = Claim.Value
                    // IdentityUserClaimCode  = Claim.ValueType
                    // IdentityUserClaimDesc =  Claim.Subject  

                    var oldClaim =
                        Connection.Query<IdentityUserClaim>(
                            "select cuc.* from IdentityUserClaim cuc " +
                            "inner join IdentityUserClaimType cuct on cuc.ClaimTypeId = cuct.TypeId " +
                               "where cuc.UserId = @Id AND " +
                               "cuct.ClaimTypeCode = @Type AND " +
                               "cuc.ClaimValue = @Value AND " +
                               "cuc.Issuer = @Issuer;",
                            new { user.Id, claim.Type, claim.Value, claim.Issuer })
                            .FirstOrDefault();
                    if (oldClaim == null)
                    {
                        var theClaimType =
                       Connection.Query<IdentityUserClaimType>(
                           "select * from IdentityUserClaimType where cuct.IdentityUserClaimTypeCode = @Type;",
                           new { claim.Type })
                           .FirstOrDefault();

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
                var theClaimType = Connection.Query<IdentityUserClaimType>(
                       "select * from IdentityUserClaimType where cuct.ClaimTypeCode = @Type;",
                       new { claim.Type })
                       .FirstOrDefault();

                if (userItem != null && theClaimType != null)
                {
                    var oldClaim =
                        Connection.Query<IdentityUserClaim>(
                            "select * from [dbo].[IdentityUserClaim] where UserId = @Id AND ClaimTypeId = @TypeId  AND ClaimValue = @Value; AND CloudUserClaimDesc = @Issuer;",
                            new { user.Id, theClaimType.TypeId, claim.Value, claim.Issuer })
                            .FirstOrDefault();
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
                var roleItem = Connection.Query<IdentityRole>("SELECT * FROM IdentityRole WHERE Name = @roleName", new { roleName }).FirstOrDefault();
                if (roleItem != null)
                {
                    //does this user & role combo already exist?
                    var roleUserItem = Connection.Query<IdentityUserRole>("SELECT * FROM IdentityUserRole WHERE UserId = @Id AND RoleId = @RoleId", new { user.Id, RoleId = roleItem.Id }).FirstOrDefault();
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
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TUser> Users { get; private set; }
        public Task SetEmailAsync(TUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
        }
    }
}
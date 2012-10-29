using System;
using System.Collections.Generic;
using System.Linq;
using Deployd.Core.Security;
using log4net;

namespace Deployd.Agent.Services.Authentication
{
    public class InMemoryCredentialStore  : ICredentialStore
    {
        private static readonly List<UserCredentials> CredentialStore = new List<UserCredentials>();
        private ILog log = LogManager.GetLogger(typeof (InMemoryCredentialStore));

        public InMemoryCredentialStore()
        {
            log.DebugFormat("new inmemory credential store");
            // create a default admin account for the time being
            if (CredentialStore.Count==0)
                CredentialStore.Add(new UserCredentials(){Username="admin", HashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123")});
        }

        public void AddUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void ResetPassword(string passwordResetToken, string newPassword)
        {
            throw new NotImplementedException();
        }

        public string CreatePasswordResetToken(string username)
        {
            throw new NotImplementedException();
        }

        public bool ValidateCredentials(string username, string password)
        {
            return CredentialStore.Any(uc => uc.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                                              && BCrypt.Net.BCrypt.Verify(password, uc.HashedPassword));
        }

        public UserCredentials GetByAccessToken(Guid guid)
        {
            log.DebugFormat("Get user by access token {0}", guid);
            return CredentialStore.SingleOrDefault(
                uc => uc.AccessToken.Equals(guid));
        }

        public void SetAccessToken(string username, Guid guid)
        {
            var user =
                CredentialStore.SingleOrDefault(uc => uc.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                throw new ArgumentOutOfRangeException("user not found");

            user.AccessToken = guid;
            log.DebugFormat("Set user access token {0}", guid);
        }

        public UserCredentials GetByUsername(string username)
        {
            return CredentialStore.SingleOrDefault(
                uc=>uc.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));        
        }
    }
}
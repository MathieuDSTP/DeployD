using System;
using System.Collections.Generic;
using System.Linq;
using Deployd.Core.Security;

namespace Deployd.Agent.Services.Authentication
{
    public class InMemoryCredentialStore  : ICredentialStore
    {
        private List<UserCredentials> _credentialStore = new List<UserCredentials>();

        public InMemoryCredentialStore()
        {
            // create a default admin account for the time being
            _credentialStore.Add(new UserCredentials(){Username="admin", HashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123")});
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
            return _credentialStore.Any(uc => uc.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                                              && BCrypt.Net.BCrypt.Verify(password, uc.HashedPassword));
        }
    }
}
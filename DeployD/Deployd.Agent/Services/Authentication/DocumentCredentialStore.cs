using System;
using System.Linq;
using Deployd.Core.Security;
using Raven.Client;
using Raven.Client.Linq;

namespace Deployd.Agent.Services.Authentication
{
    public class DocumentCredentialStore : ICredentialStore
    {
        private readonly IDocumentSession _session;

        public DocumentCredentialStore(IDocumentSession session)
        {
            _session = session;
        }

        public void AddUser(string username, string password)
        {
            var existing = _session.Load<UserCredentials>(username);
            if (existing != null)
            {
                throw new InvalidOperationException("Username has been taken");
            }

            var user = new UserCredentials()
                {Id = username, HashedPassword = BCrypt.Net.BCrypt.HashPassword(password)};
            _session.Store(user);
        }

        public void DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void ResetPassword(string compressedToken, string newPassword)
        {
            Guid tokenAsGuid = Guid.Parse(compressedToken);
            var resetToken = _session.Load<PasswordResetToken>(tokenAsGuid.ToString());
            if (resetToken != null)
            {
                var credentials = GetByUsername(resetToken.Username);
                if (credentials != null)
                {
                    credentials.HashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                }
                else
                {
                    throw new InvalidOperationException("No such user to reset password for");
                }
                _session.Delete(resetToken);
            }
            else
            {
                throw new ArgumentOutOfRangeException("compressedToken", "Reset token not found");
            }
        }

        public string CreatePasswordResetToken(string username)
        {
            var resetToken = Guid.NewGuid();
            var existingResetHolder =
                _session.Query<PasswordResetToken>().SingleOrDefault(r => r.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (existingResetHolder != null)
            {
                existingResetHolder.Id = resetToken;
                existingResetHolder.Expiry = DateTime.Now.AddHours(1);
            }
            else
            {
                _session.Store(new PasswordResetToken() { Username = username, Id = resetToken, Expiry = DateTime.Now.AddHours(1) });
            }

            return resetToken.ToString();
        }

        public bool ValidateCredentials(string username, string password)
        {
            var user = _session.Load<UserCredentials>(username);
            if (user == null)
                return false;

            return user.HashedPassword.Equals(password, StringComparison.CurrentCultureIgnoreCase);
        }

        public UserCredentials GetByAccessToken(Guid guid)
        {
            var users = _session.Query<UserCredentials>()
                .Where(uc => uc.AccessToken == guid);

            if (users.Count() != 1)
            {
                throw new ArgumentOutOfRangeException("Zero or multiple user accounts found");
            }

            return users.Single();
        }

        public void SetAccessToken(string username, Guid guid)
        {
            var user = _session.Load<UserCredentials>(username);
            user.AccessToken = guid;
        }

        public UserCredentials GetByUsername(string username)
        {
            return _session.Load<UserCredentials>(username);
        }
    }
}
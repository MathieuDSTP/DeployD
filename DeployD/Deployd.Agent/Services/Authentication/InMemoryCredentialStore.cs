using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deployd.Core.Security;
using Nancy.Helpers;
using log4net;

namespace Deployd.Agent.Services.Authentication
{
    public class InMemoryCredentialStore  : ICredentialStore
    {
        private static readonly List<UserCredentials> CredentialStore = new List<UserCredentials>();
        private static readonly List<PasswordResetToken> PasswordResetTokens = new List<PasswordResetToken>();
        private ILog log = LogManager.GetLogger(typeof (InMemoryCredentialStore));

        public InMemoryCredentialStore()
        {
        }

        public void AddUser(string username, string password)
        {
            CredentialStore.Add(new UserCredentials(){Username=username, HashedPassword = BCrypt.Net.BCrypt.HashPassword(password)});
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
            Guid tokenAsGuid=ExpandGuid(compressedToken);
            var resetToken = PasswordResetTokens.SingleOrDefault(x => x.ResetToken == tokenAsGuid);
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
                PasswordResetTokens.Remove(resetToken);
            }
            else
            {
                throw new ArgumentOutOfRangeException("compressedToken", "Reset token not found");
            }
        }

        private string CompressGuid(Guid guid)
        {
            try
            {
                var compressGuid = new Ascii85().Encode(guid.ToByteArray());
                return compressGuid;
            } catch
            {
                throw new ArgumentException("Not a valid guid");
            }
        }

        private Guid ExpandGuid(string compressed)
        {

            return new Guid(new Ascii85().Decode(HttpUtility.UrlDecode(compressed)));
        }

        public string CreatePasswordResetToken(string username)
        {
            var resetToken = Guid.NewGuid();
            var existingResetHolder =
                PasswordResetTokens.SingleOrDefault(r => r.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (existingResetHolder != null)
            {
                existingResetHolder.ResetToken = resetToken;
                existingResetHolder.Expiry = DateTime.Now.AddHours(1);
            }
            else
            {
                PasswordResetTokens.Add(new PasswordResetToken()
                    {Username = username, ResetToken = resetToken, Expiry = DateTime.Now.AddHours(1)});
            }

            return CompressGuid(resetToken);
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

    internal class PasswordResetToken
    {
        public string Username { get; set; }
        public Guid ResetToken { get; set; }
        public DateTime Expiry { get; set; }
    }
}
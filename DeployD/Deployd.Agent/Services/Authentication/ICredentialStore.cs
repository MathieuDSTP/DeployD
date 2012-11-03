using System;
using Deployd.Core.Security;

namespace Deployd.Agent.Services.Authentication
{
    public interface ICredentialStore
    {
        void AddUser(string username, string password);
        void DeleteUser(string username);
        void ChangePassword(string username, string oldPassword, string newPassword);
        void ResetPassword(string compressedToken, string newPassword);
        string CreatePasswordResetToken(string username);
        bool ValidateCredentials(string username, string password, bool preHashed);
        UserCredentials GetByAccessToken(Guid guid);
        void SetAccessToken(string username, Guid guid);
        UserCredentials GetByUsername(string username);
        bool VerifyPasswordResetToken(string token);
    }
}
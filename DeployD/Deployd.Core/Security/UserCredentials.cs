using System;

namespace Deployd.Core.Security
{
    public class UserCredentials
    {
        public string Id { get; set; }
        public string HashedPassword { get; set; }
        public Guid AccessToken { get; set; }
    }
}
﻿using System;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace Deployd.Agent.Authentication
{
    public class DefaultUserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return new DeploydUserIdentity(){UserName="andrew", Claims=new[]{"admin"}};
        }
    }
}

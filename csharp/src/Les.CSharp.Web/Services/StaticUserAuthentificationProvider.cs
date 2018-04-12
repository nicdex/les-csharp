using System;
using Les.CSharp.Web.Controllers;

namespace Les.CSharp.Web.Services
{
    public class StaticUserAuthentificationProvider : IAuthentificationProvider
    {
        private static readonly Guid StaticUserId = Guid.NewGuid();

        public Guid GetUserId()
        {
            return StaticUserId;
        }
    }
}

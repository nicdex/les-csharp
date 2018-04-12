using System;

namespace Les.CSharp.Web.Controllers
{
    public interface IAuthentificationProvider
    {
        Guid GetUserId();
    }
}
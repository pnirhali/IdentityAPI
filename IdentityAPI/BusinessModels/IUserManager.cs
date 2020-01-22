using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.BusinessModels
{
    public interface IUserManager<TUser> : IDisposable where TUser : class
    {
    }
}

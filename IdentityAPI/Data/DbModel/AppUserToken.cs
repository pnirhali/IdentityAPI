using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace IdentityAPI.Data.DbModel
{
    public class AppUserToken : IdentityUserToken<long>
    {
    }
}

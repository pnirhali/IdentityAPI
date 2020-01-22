using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.DTO
{
    public class ErrorDTO
    {
        public string ErrMessages { get; set; }

        public int Code { get; set; }
    }
}

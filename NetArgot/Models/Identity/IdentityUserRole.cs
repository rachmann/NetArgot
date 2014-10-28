using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;

namespace NetArgot.Models.Identity
{
    public class IdentityUserRole
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User : BaseEntity<int>
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin or Customer

    }
}

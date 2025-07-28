using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace ServicesAbstractions
{
    public interface IAuthService
    {
        Task RegisterAsync(User user);
        Task<string?> LoginAsync(string username, string password);
    }


}

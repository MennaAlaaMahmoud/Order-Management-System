using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Shared;

namespace ServicesAbstractions
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task DeleteUserAsync(int id);
        Task UpdateUserRoleAsync(int id, string role);
    }


}

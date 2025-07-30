using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface IPaymobServices
    {
        Task<string> PayWithCardAsync(decimal amount, string email, string phone);
        Task<string> PayWithWalletAsync(decimal amount, string phone);
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Shared;

namespace ServicesAbstractions
{
    public interface IInvoiceService
    {
        Task<List<InvoiceResponseDto>> GetAllInvoicesAsync();
        Task<InvoiceResponseDto> GetInvoiceByIdAsync(int id);
    }

}

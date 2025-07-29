using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Models;
using Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //  1. Order → OrderResponseDto
            CreateMap<Order, OrderResponseDto>();

            //  2. OrderItem → OrderItemResponseDto
            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            //  3. Product → ProductResponseDto (ذهاب وعودة)
            CreateMap<Product, ProductResponseDto>().ReverseMap();

            //  4. ProductDto → Product (من Dto إلى كيان فقط)
            CreateMap<ProductDto, Product>();

            CreateMap<User, UserDto>();
            CreateMap<Customer, CustomerDto>();


            // 5. Invoice → InvoiceResponseDto
            CreateMap<Invoice, InvoiceResponseDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src => src.InvoiceDate));
        }
    }
}

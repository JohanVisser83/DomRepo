using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Core.Mapper
{
    public class StorefrontStoreMapper:Profile
    {
        public StorefrontStoreMapper() {

            CreateMap<CustomerStore, StorefrontDTO>().ReverseMap(); 
        }  
    }
}

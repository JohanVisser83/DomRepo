﻿using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class DriverMapper : Profile
    {
        public DriverMapper()
        {
            CreateMap<Driver,DriverDTO>().ReverseMap();
        }
    }
}

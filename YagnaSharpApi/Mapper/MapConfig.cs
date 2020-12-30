using AutoMapper;
using Golem.PaymentApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Mapper
{
    public class MapConfig
    {
        public static MapperConfiguration Config { get; set; }

        public static void Init()
        {
            Config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Allocation, AllocationEntity>();
            });
        }

    }
}

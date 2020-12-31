using AutoMapper;
using Golem.MarketApi.Client.Model;
using Golem.PaymentApi.Client.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Utils;

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

                cfg.CreateMap<Proposal, ProposalEntity>()
                    .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => (src.Properties as JObject).ToDictionary()));

                cfg.CreateMap<ProposalEvent, ProposalEventEntity>();
                cfg.CreateMap<PropertyQueryEvent, PropertyQueryEventEntity>();
                cfg.CreateMap<ProposalRejectedEvent, ProposalRejectedEventEntity>();

                cfg.CreateMap<Event, EventEntity>()
                    .Include<ProposalEvent, ProposalEventEntity>()
                    .Include<PropertyQueryEvent, PropertyQueryEventEntity>()
                    .Include<ProposalRejectedEvent, ProposalRejectedEventEntity>();
            });
        }

    }
}

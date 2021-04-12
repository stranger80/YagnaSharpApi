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

                cfg.CreateMap<DemandOfferBase, DemandOfferBaseEntity>();
                cfg.CreateMap<Demand, DemandEntity>()
                    .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => (src.Properties as JObject).ToDictionary()));
                cfg.CreateMap<Offer, OfferEntity>()
                    .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => (src.Properties as JObject).ToDictionary()));

                cfg.CreateMap<ReasonEntity, Reason>();

                cfg.CreateMap<Agreement, AgreementEntity>();

                // Agreement event hierarchy
                cfg.CreateMap<AgreementEvent, AgreementEventEntity>();
                cfg.CreateMap<AgreementTerminatedEvent, AgreementTerminatedEventEntity>();
                cfg.CreateMap<AgreementApprovedEvent, AgreementApprovedEventEntity>();

                // Proposal event hierarchy

                cfg.CreateMap<ProposalEvent, ProposalEventEntity>();
                cfg.CreateMap<PropertyQueryEvent, PropertyQueryEventEntity>();
                cfg.CreateMap<ProposalRejectedEvent, ProposalRejectedEventEntity>();

                cfg.CreateMap<Event, EventEntity>()
                    .Include<ProposalEvent, ProposalEventEntity>()
                    .Include<PropertyQueryEvent, PropertyQueryEventEntity>()
                    .Include<ProposalRejectedEvent, ProposalRejectedEventEntity>();
                    //.Include<AgreementEvent, AgreementEventEntity>()
                    //.Include<AgreementApprovedEvent, AgreementApprovedEventEntity>()
                    //.Include<AgreementTerminatedEvent, AgreementTerminatedEventEntity>()

                // Invoice events hierarchy

                cfg.CreateMap<InvoiceReceivedEvent, InvoiceReceivedEventEntity>();

                cfg.CreateMap<InvoiceEvent, InvoiceEventEntity>()
                    .Include<InvoiceReceivedEvent, InvoiceReceivedEventEntity>();

                cfg.CreateMap<Invoice, InvoiceEntity>();

                cfg.CreateMap<Account, AccountEntity>();

            });

        }

    }
}

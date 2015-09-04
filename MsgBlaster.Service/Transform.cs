using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;


namespace MsgBlaster.Service
{
    public static class Transform
    {
         #region CreditRequest
         public static CreditRequest CreditRequestToDomain(CreditRequestDTO CreditRequestDTO)
        {
            if (CreditRequestDTO == null) return null;
            Mapper.CreateMap<CreditRequestDTO, CreditRequest>();
            CreditRequest CreditRequest = Mapper.Map<CreditRequest>(CreditRequestDTO);
            CreditRequest._clientName = ClientService.GetById(CreditRequest.ClientId).Company;
            CreditRequest._userName = UserService.GetById(CreditRequest.RequestedBy).Name;
            return CreditRequest;
        }

         public static CreditRequestDTO CreditRequestToDTO(CreditRequest CreditRequest)
        {
            if (CreditRequest == null) return null;
            Mapper.CreateMap<CreditRequest, CreditRequestDTO>();
            CreditRequestDTO CreditRequestDTO = Mapper.Map<CreditRequestDTO>(CreditRequest);
            return CreditRequestDTO;
        }
        #endregion
         
         #region Campaign
         public static Campaign CampaignToDomain(CampaignDTO CampaignDTO)
         {
             if (CampaignDTO == null) return null;
             Mapper.CreateMap<CampaignDTO, Campaign>();
             Campaign Campaign = Mapper.Map<Campaign>(CampaignDTO);
             return Campaign;
         }

         public static CampaignDTO CampaignToDTO(Campaign Campaign)
         {
             if (Campaign == null) return null;
             Mapper.CreateMap<Campaign, CampaignDTO>();
             CampaignDTO CampaignDTO = Mapper.Map<CampaignDTO>(Campaign);
             return CampaignDTO;
         }
         #endregion

         #region Plan
         public static Plan PlanToDomain(PlanDTO PlanDTO)
         {
             if (PlanDTO == null) return null;
             Mapper.CreateMap<PlanDTO, Plan>();
             Plan Plan = Mapper.Map<Plan>(PlanDTO);
             return Plan;
         }

         public static PlanDTO PlanToDTO(Plan Plan)
         {
             if (Plan == null) return null;
             Mapper.CreateMap<Plan, PlanDTO>();
             PlanDTO PlanDTO = Mapper.Map<PlanDTO>(Plan);
             return PlanDTO;
         }
         #endregion

         #region Client
         public static Client ClientToDomain(ClientDTO ClientDTO)
         {
             if (ClientDTO == null) return null;
             Mapper.CreateMap<ClientDTO, Client>();
             Client Client = Mapper.Map<Client>(ClientDTO);
             return Client;
         }

         public static ClientDTO ClientToDTO(Client Client)
         {
             if (Client == null) return null;
             Mapper.CreateMap<Client, ClientDTO>();
             ClientDTO ClientDTO = Mapper.Map<ClientDTO>(Client);
             return ClientDTO;
         }
         #endregion

         #region ClientUser
         public static User UserToDomain(UserDTO UserDTO)
         {
             if (UserDTO == null) return null;
             Mapper.CreateMap<UserDTO,User>();
             User User = Mapper.Map<User>(UserDTO);
             return User;
         }

         public static UserDTO UserToDTO(User User)
         {
             if (User == null) return null;
             Mapper.CreateMap<User, UserDTO>();
             UserDTO UserDTO = Mapper.Map<UserDTO>(User);
             return UserDTO;
         }
         #endregion

         #region Coupon
         public static Coupon CouponToDomain(CouponDTO CouponDTO)
         {
             if (CouponDTO == null) return null;
             Mapper.CreateMap<CouponDTO, Coupon>();
             Coupon Coupon = Mapper.Map<Coupon>(CouponDTO);
             return Coupon;
         }

         public static CouponDTO CouponToDTO(Coupon Coupon)
         {
             if (Coupon == null) return null;
             Mapper.CreateMap<Coupon, CouponDTO>();
             CouponDTO CouponDTO = Mapper.Map<CouponDTO>(Coupon);
             return CouponDTO;
         }
         #endregion

         #region EcouponCampaign
         public static EcouponCampaign EcouponCampaignToDomain(EcouponCampaignDTO EcouponCampaignDTO)
         {
             if (EcouponCampaignDTO == null) return null;
             Mapper.CreateMap<EcouponCampaignDTO, EcouponCampaign>();
             EcouponCampaign EcouponCampaign = Mapper.Map<EcouponCampaign>(EcouponCampaignDTO);
             return EcouponCampaign;
         }

         public static EcouponCampaignDTO EcouponCampaignToDTO(EcouponCampaign EcouponCampaign)
         {
             if (EcouponCampaign == null) return null;
             Mapper.CreateMap<EcouponCampaign, EcouponCampaignDTO>();
             EcouponCampaignDTO EcouponCampaignDTO = Mapper.Map<EcouponCampaignDTO>(EcouponCampaign);
             return EcouponCampaignDTO;
         }
         #endregion

         #region Group
         public static Group GroupToDomain(GroupDTO GroupDTO)
         {
             if (GroupDTO == null) return null;
             Mapper.CreateMap<GroupDTO, Group>();
             Mapper.CreateMap<ContactDTO, Contact>();
             Group Group = Mapper.Map<Group>(GroupDTO);
             return Group;
         }

         public static GroupDTO GroupToDTO(Group Group)
         {
             if (Group == null) return null;
             Mapper.CreateMap<Group, GroupDTO>();
             Mapper.CreateMap<Contact, ContactDTO>();
             GroupDTO GroupDTO = Mapper.Map<GroupDTO>(Group);
             //ContactDTO ContactDTO = Mapper.Map<ContactDTO>(Group.Contacts);
             //var GroupDTO = AutoMapper.Mapper.Map<GroupDTO>(Group);
             return GroupDTO;
         }
         #endregion

         #region Partner
         public static Partner PartnerToDomain(PartnerDTO PartnerDTO)
         {
             if (PartnerDTO == null) return null;
             Mapper.CreateMap<PartnerDTO, Partner>();
             Partner Partner = Mapper.Map<Partner>(PartnerDTO);
             return Partner;
         }

         public static PartnerDTO PartnerToDTO(Partner Partner)
         {
             if (Partner == null) return null;
             Mapper.CreateMap<Partner, PartnerDTO>();
             PartnerDTO PartnerDTO = Mapper.Map<PartnerDTO>(Partner);
             return PartnerDTO;
         }
         #endregion

         #region Receipt
         public static Receipt ReceiptToDomain(ReceiptDTO ReceiptDTO)
         {
             if (ReceiptDTO == null) return null;
             Mapper.CreateMap<ReceiptDTO, Receipt>();
             Receipt Receipt = Mapper.Map<Receipt>(ReceiptDTO);
             return Receipt;
         }

         public static ReceiptDTO ReceiptToDTO(Receipt Receipt)
         {
             if (Receipt == null) return null;
             Mapper.CreateMap<Receipt, ReceiptDTO>();
             ReceiptDTO ReceiptDTO = Mapper.Map<ReceiptDTO>(Receipt);
             return ReceiptDTO;
         }
         #endregion

         #region Setting
         public static Setting SettingToDomain(SettingDTO SettingDTO)
         {
             if (SettingDTO == null) return null;
             Mapper.CreateMap<SettingDTO, Setting>();
             Setting Setting = Mapper.Map<Setting>(SettingDTO);
             return Setting;
         }

         public static SettingDTO SettingToDTO(Setting Setting)
         {
             if (Setting == null) return null;
             Mapper.CreateMap<Setting, SettingDTO>();
             SettingDTO SettingDTO = Mapper.Map<SettingDTO>(Setting);
             return SettingDTO;
         }
         #endregion

         #region Template
         public static Template TemplateToDomain(TemplateDTO TemplateDTO)
         {
             if (TemplateDTO == null) return null;
             Mapper.CreateMap<TemplateDTO, Template>();
             Template Template = Mapper.Map<Template>(TemplateDTO);
             return Template;
         }

         public static TemplateDTO TemplateToDTO(Template Template)
         {
             if (Template == null) return null;
             Mapper.CreateMap<Template, TemplateDTO>();
             TemplateDTO TemplateDTO = Mapper.Map<TemplateDTO>(Template);
             return TemplateDTO;
         }
         #endregion
         
         #region Contact
         public static Contact ContactToDomain(ContactDTO ContactDTO)
         {
             if (ContactDTO == null) return null;
             Mapper.CreateMap<ContactDTO, Contact>();
             Mapper.CreateMap<GroupDTO, Group>();
             Contact Contact = Mapper.Map<Contact>(ContactDTO);            
             return Contact;
         }

         public static ContactDTO ContactToDTO(Contact Contact)
         {
             if (Contact == null) return null;
             Mapper.CreateMap<Contact, ContactDTO>();
             Mapper.CreateMap<Group, GroupDTO>();
             ContactDTO ContactDTO = Mapper.Map<ContactDTO>(Contact);
             //GroupDTO GroupDTO = Mapper.Map<GroupDTO>(Contact.Groups);
             //var ContactDTO = AutoMapper.Mapper.Map<ContactDTO>(Contact);
             //ContactDTO.Name = Contact.FirstName + " " + Contact.LastName;
             return ContactDTO;
         }
         #endregion
                 
         #region RedeemedCount
         public static RedeemedCount RedeemedCountToDomain(RedeemedCountDTO RedeemedCountDTO)
         {
             if (RedeemedCountDTO == null) return null;
             Mapper.CreateMap<RedeemedCountDTO, RedeemedCount>();
             RedeemedCount RedeemedCount = Mapper.Map<RedeemedCount>(RedeemedCountDTO);
             return RedeemedCount;
         }

         public static RedeemedCountDTO RedeemedCountToDTO(RedeemedCount RedeemedCount)
         {
             if (RedeemedCount == null) return null;
             Mapper.CreateMap<RedeemedCount, RedeemedCountDTO>();
             RedeemedCountDTO RedeemedCountDTO = Mapper.Map<RedeemedCountDTO>(RedeemedCount);
             return RedeemedCountDTO;
         }
         #endregion
         
         #region Document
         public static Document DocumentToDomain(DocumentDTO DocumentDTO)
         {
             if (DocumentDTO == null) return null;
             Mapper.CreateMap<DocumentDTO, Document>();
             Document Document = Mapper.Map<Document>(DocumentDTO);
             return Document;
         }

         public static DocumentDTO DocumentToDTO(Document Document)
         {
             if (Document == null) return null;
             Mapper.CreateMap<Document, DocumentDTO>();
             DocumentDTO DocumentDTO = Mapper.Map<DocumentDTO>(Document);
             return DocumentDTO;
         }
         #endregion

         #region ActivityLog
         public static ActivityLog ActivityLogToDomain(ActivityLogDTO ActivityLogDTO)
         {
             if (ActivityLogDTO == null) return null;
             Mapper.CreateMap<ActivityLogDTO, ActivityLog>();
             ActivityLog ActivityLog = Mapper.Map<ActivityLog>(ActivityLogDTO);
             return ActivityLog;
         }

         public static ActivityLogDTO ActivityLogToDTO(ActivityLog ActivityLog)
         {
             if (ActivityLog == null) return null;
             Mapper.CreateMap<ActivityLog, ActivityLogDTO>();
             ActivityLogDTO ActivityLogDTO = Mapper.Map<ActivityLogDTO>(ActivityLog);
             return ActivityLogDTO;
         }
         #endregion
  
         #region Location
         public static Location LocationToDomain(LocationDTO LocationDTO)
         {
             if (LocationDTO == null) return null;
             Mapper.CreateMap<LocationDTO, Location>();
             Location Location = Mapper.Map<Location>(LocationDTO);
             return Location;
         }

         public static LocationDTO LocationToDTO(Location Location)
         {
             if (Location == null) return null;
             Mapper.CreateMap<Location, LocationDTO>();
             LocationDTO LocationDTO = Mapper.Map<LocationDTO>(Location);
             return LocationDTO;
         }
         #endregion

         #region CampaignLogXMLDTO
         public static CampaignLogXML CampaignLogXMLToDomain(CampaignLogXMLDTO CampaignLogXMLDTO)
         {
             if (CampaignLogXMLDTO == null) return null;
             Mapper.CreateMap<CampaignLogXMLDTO, CampaignLogXML>();
             CampaignLogXML CampaignLogXML = Mapper.Map<CampaignLogXML>(CampaignLogXMLDTO);
             return CampaignLogXML;
         }

         public static CampaignLogXMLDTO CampaignLogXMLToDTO(CampaignLogXML CampaignLogXML)
         {
             if (CampaignLogXML == null) return null;
             Mapper.CreateMap<CampaignLogXML, CampaignLogXMLDTO>();
             CampaignLogXMLDTO CampaignLogXMLDTO = Mapper.Map<CampaignLogXMLDTO>(CampaignLogXML);
             return CampaignLogXMLDTO;
         }
         #endregion

         #region GroupContactDTO
         public static GroupContact GroupContactNewToDomain(GroupContactDTO GroupContactDTO)
         {
             if (GroupContactDTO == null) return null;
             Mapper.CreateMap<GroupContactDTO, GroupContact>();
             GroupContact GroupContact = Mapper.Map<GroupContact>(GroupContactDTO);
             return GroupContact;
         }

         public static GroupContactDTO GroupContactNewToDTO(GroupContact GroupContact)
         {
             if (GroupContact == null) return null;
             Mapper.CreateMap<GroupContact, GroupContactDTO>();
             GroupContactDTO GroupContactDTO = Mapper.Map<GroupContactDTO>(GroupContact);
             return GroupContactDTO;
         }
         #endregion
         
         #region "Unwanted Code"

        //#region GroupContact
        //public static Group GroupContactToDomain(GroupContactDTO GroupContactDTO)
        //{
        //    if (GroupContactDTO == null) return null;
        //    Mapper.CreateMap<ContactDTO, Contact>();
        //    Mapper.CreateMap<GroupDTO, Group>();
        //    Group Group = Mapper.Map<Group>(GroupContactDTO);
        //    return Group;
        //}

        //public static GroupContactDTO GroupContactToDTO(Group Group)
        //{
        //    if (Group == null) return null;
        //    Mapper.CreateMap<Contact, ContactDTO>();
        //    Mapper.CreateMap<Group, GroupContactDTO>();
        //    GroupContactDTO GroupContactDTO = Mapper.Map<GroupContactDTO>(Group);
        //    //GroupDTO GroupDTO = Mapper.Map<GroupDTO>(Contact.Groups);
        //    //var ContactDTO = AutoMapper.Mapper.Map<ContactDTO>(Contact);

        //    return GroupContactDTO;
        //}
        //#endregion


        //#region CampaignLogDTO
        //public static CampaignLog CampaignLogToDomain(CampaignLogDTO CampaignLogDTO)
        //{
        //    if (CampaignLogDTO == null) return null;
        //    Mapper.CreateMap<CampaignLogDTO, CampaignLog>();
        //    CampaignLog CampaignLog = Mapper.Map<CampaignLog>(CampaignLogDTO);
        //    return CampaignLog;
        //}

        //public static CampaignLogDTO CampaignLogToDTO(CampaignLog CampaignLog)
        //{
        //    if (CampaignLog == null) return null;
        //    Mapper.CreateMap<CampaignLog, CampaignLogDTO>();
        //    CampaignLogDTO CampaignLogDTO = Mapper.Map<CampaignLogDTO>(CampaignLog);
        //    return CampaignLogDTO;
        //}
        //#endregion

        //#region SMSGateway
        //public static SMSGateway SMSGatewayToDomain(SMSGatewayDTO SMSGatewayDTO)
        //{
        //    if (SMSGatewayDTO == null) return null;
        //    Mapper.CreateMap<SMSGatewayDTO, SMSGateway>();
        //    SMSGateway SMSGateway = Mapper.Map<SMSGateway>(SMSGatewayDTO);
        //    return SMSGateway;
        //}

        //public static SMSGatewayDTO SMSGatewayToDTO(SMSGateway SMSGateway)
        //{
        //    if (SMSGateway == null) return null;
        //    Mapper.CreateMap<SMSGateway, SMSGatewayDTO>();
        //    SMSGatewayDTO SMSGatewayDTO = Mapper.Map<SMSGatewayDTO>(SMSGateway);
        //    return SMSGatewayDTO;
        //}
        //#endregion

        #endregion
        
    }
}

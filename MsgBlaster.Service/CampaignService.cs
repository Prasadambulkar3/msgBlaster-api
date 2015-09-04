using MsgBlaster.Repo;
using MsgBlaster.Domain;
using MsgBlaster.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Collections;


namespace MsgBlaster.Service
{
    public class CampaignService
    {

        #region "CRUD Functionality"

        //Create Camoaign
        public static int Create(CampaignDTO campaignDTO)
        {

            try
            {
                GlobalSettings.LoggedInClientId = campaignDTO.ClientId;
                GlobalSettings.LoggedInUserId = campaignDTO.CreatedBy;
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(campaignDTO.ClientId);
                int PartnerId = ClientDTO.PartnerId;// ClientService.GetById(campaignDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                //If SMS Credit balance is low then should not create campaign
                if (ClientDTO.SMSCredit < campaignDTO.ConsumedCredits)
                { 
                    return 0;
                }
                var campaign = new Campaign();
                SettingDTO SettingDTO = new SettingDTO();
                SettingDTO = SettingService.GetById(1);
                UnitOfWork uow = new UnitOfWork();
                if (campaignDTO.RecipientsNumber != null && campaignDTO.RecipientsNumber != "")
                {
                    campaignDTO.RecipientsNumber = CommonService.RemoveDuplicateMobile(campaignDTO.RecipientsNumber);
                }

                //Get Message Count
                if (campaignDTO.IsUnicode != true)
                {
                    campaignDTO.MessageCount = CommonService.GetMessageCount(campaignDTO.Message);
                }
                else
                {
                    campaignDTO.MessageCount = CommonService.GetUnicodeMessageCount(campaignDTO.Message);
                }
                campaignDTO.IPAddress = CommonService.GetIP();
                if (campaignDTO.GroupId == null)
                {
                    campaignDTO.RecipientsCount = CommonService.GetRecipientsCount(campaignDTO.RecipientsNumber);
                    campaignDTO.RequiredCredits = (campaignDTO.RecipientsCount * campaignDTO.MessageCount) * SettingDTO.NationalCampaignSMSCount;
                    campaignDTO.CreditsDiffrence = campaignDTO.ConsumedCredits - campaignDTO.RequiredCredits;
                }

                if (campaignDTO.ForAllContact == true)
                {
                    campaignDTO.RecipientsNumber = ContactService.GetAllReceipentNumberByClientId(campaignDTO.ClientId);
                    campaignDTO.RecipientsCount = CommonService.GetRecipientsCount(campaignDTO.RecipientsNumber);
                    campaignDTO.RequiredCredits = (campaignDTO.RecipientsCount * campaignDTO.MessageCount) * SettingDTO.NationalCampaignSMSCount;
                    campaignDTO.GroupId = null;
                    campaignDTO.RecipientsNumber = "";
                    campaignDTO.CreditsDiffrence = campaignDTO.ConsumedCredits - campaignDTO.RequiredCredits;
                }

                if (campaignDTO.GroupId > 0)
                {
                    campaignDTO.RequiredCredits = (campaignDTO.RecipientsCount * campaignDTO.MessageCount) * SettingDTO.NationalCampaignSMSCount;
                }

                campaignDTO.IsReconcile = false;
                campaignDTO.ReconcileDate = System.DateTime.Now.Date;
                campaign = Transform.CampaignToDomain(campaignDTO);
                uow.CampaignRepo.Insert(campaign);
                uow.SaveChanges();

                

                // Deduct SMS credit balance 
                campaignDTO.Id = campaign.Id;
                

                ClientDTO.SMSCredit = ClientDTO.SMSCredit - campaignDTO.ConsumedCredits;
                ClientService.Edit(ClientDTO);


                return campaignDTO.Id;

            }

            catch (Exception)
            {
                throw;
            }
        }

        //Edit Campaign
        public static void Edit(CampaignDTO campaignDTO)
        {
            try
            {

                //Get previous consumed count and restore clients credits 
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(campaignDTO.ClientId);
                CampaignDTO CampaignDTORestore = new CampaignDTO();
                CampaignDTORestore = GetById(campaignDTO.Id);
                ClientDTO.SMSCredit = ClientDTO.SMSCredit + CampaignDTORestore.ConsumedCredits;
                ClientService.Edit(ClientDTO);


                //If SMS Credit balance is greater or equal to ConsumededCredits then create campaign
                if (ClientDTO.SMSCredit >= campaignDTO.ConsumedCredits)
                {
                GlobalSettings.LoggedInClientId = campaignDTO.ClientId;
                GlobalSettings.LoggedInUserId = campaignDTO.CreatedBy;
                int PartnerId = ClientService.GetById(campaignDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                SettingDTO SettingDTO = new SettingDTO();
                SettingDTO = SettingService.GetById(1);

                UnitOfWork uow = new UnitOfWork();

                if (campaignDTO.RecipientsNumber != null && campaignDTO.RecipientsNumber != "")
                {
                    campaignDTO.RecipientsNumber = CommonService.RemoveDuplicateMobile(campaignDTO.RecipientsNumber);
                }

                campaignDTO.MessageCount = CommonService.GetMessageCount(campaignDTO.Message);
                if (campaignDTO.GroupId == null)
                {
                    campaignDTO.RecipientsCount = CommonService.GetRecipientsCount(campaignDTO.RecipientsNumber);
                    campaignDTO.RequiredCredits = (campaignDTO.RecipientsCount * campaignDTO.MessageCount) * SettingDTO.NationalCampaignSMSCount;
                    campaignDTO.CreditsDiffrence = campaignDTO.ConsumedCredits - campaignDTO.RequiredCredits;
                }
                campaignDTO.IPAddress = CommonService.GetIP();

                if (campaignDTO.ForAllContact == true)
                {
                    campaignDTO.RecipientsNumber = ContactService.GetAllReceipentNumberByClientId(campaignDTO.ClientId);
                    campaignDTO.RecipientsCount = CommonService.GetRecipientsCount(campaignDTO.RecipientsNumber);
                    campaignDTO.RequiredCredits = (campaignDTO.RecipientsCount * campaignDTO.MessageCount) * SettingDTO.NationalCampaignSMSCount;
                    campaignDTO.GroupId = null;
                    campaignDTO.GroupName = null;
                    campaignDTO.RecipientsNumber = "";
                    campaignDTO.CreditsDiffrence = campaignDTO.ConsumedCredits - campaignDTO.RequiredCredits;
                }

                if (campaignDTO.GroupId > 0)
                {
                    campaignDTO.RequiredCredits = (campaignDTO.RecipientsCount * campaignDTO.MessageCount) * SettingDTO.NationalCampaignSMSCount;
                }

                campaignDTO.IsReconcile = false;
                campaignDTO.ReconcileDate = System.DateTime.Now.Date;
                Campaign Campaign = Transform.CampaignToDomain(campaignDTO);
                uow.CampaignRepo.Update(Campaign);
                uow.SaveChanges();                            

                //Deduct clients Credit
                ClientDTO.SMSCredit = ClientDTO.SMSCredit - campaignDTO.ConsumedCredits;
                ClientService.Edit(ClientDTO);
                
                }
            }
            catch
            {
                throw;
            }
        }

        //Edit Campaign from backend process
        public static void EditCampaignFromBackend(CampaignDTO campaignDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = campaignDTO.ClientId;
                GlobalSettings.LoggedInUserId = campaignDTO.CreatedBy;
                int PartnerId = ClientService.GetById(campaignDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                UnitOfWork uow = new UnitOfWork();
                Campaign Campaign = Transform.CampaignToDomain(campaignDTO);
                uow.CampaignRepo.Update(Campaign);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Create Campaign for Backend process
        public static int CreateCampaignFromBackend(CampaignDTO campaignDTO)
        {

            try
            {

                GlobalSettings.LoggedInClientId = campaignDTO.ClientId;
                GlobalSettings.LoggedInUserId = campaignDTO.CreatedBy;

                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(campaignDTO.ClientId);

                int PartnerId = ClientDTO.PartnerId;// ClientService.GetById(campaignDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                //If SMS Credit balance is low then should not create campaign
                if (ClientDTO.SMSCredit < campaignDTO.ConsumedCredits)
                {
                    return 0;
                }

                var campaign = new Campaign();
                SettingDTO SettingDTO = new SettingDTO();
                SettingDTO = SettingService.GetById(1);
                UnitOfWork uow = new UnitOfWork();
                if (campaignDTO.RecipientsNumber != null)
                {
                    campaignDTO.RecipientsNumber = CommonService.RemoveDuplicateMobile(campaignDTO.RecipientsNumber);
                }
                //Get Message Count
                if (campaignDTO.IsUnicode != true)
                {
                    campaignDTO.MessageCount = CommonService.GetMessageCount(campaignDTO.Message);
                }
                else
                {
                    campaignDTO.MessageCount = CommonService.GetUnicodeMessageCount(campaignDTO.Message);
                }

                if (campaignDTO.GroupId == null)
                {
                    campaignDTO.RecipientsCount = CommonService.GetRecipientsCount(campaignDTO.RecipientsNumber);
                    campaignDTO.RequiredCredits = (campaignDTO.RecipientsCount * campaignDTO.MessageCount) * SettingDTO.NationalCampaignSMSCount;
                }

                campaignDTO.IsReconcile = false;
                campaignDTO.ReconcileDate = System.DateTime.Now.Date;

                //Calculate consumed credits
                double ConsumedCreditPerOneMsg = CommonService.GetConsumedCreditsForOneMessage(campaignDTO.Message, false);
                int RecepientsCount = CommonService.GetRecipientsCount(campaignDTO.RecipientsNumber);
                campaignDTO.ConsumedCredits = RecepientsCount * ConsumedCreditPerOneMsg;

                campaign = Transform.CampaignToDomain(campaignDTO);
                uow.CampaignRepo.Insert(campaign);
                uow.SaveChanges();
                campaignDTO.Id = campaign.Id;
                
                //Deduct clients balance
              
                ClientDTO.SMSCredit = ClientDTO.SMSCredit - campaignDTO.ConsumedCredits;
                ClientService.Edit(ClientDTO);

                return campaignDTO.Id;
            }

            catch (Exception)
            {
                throw;
            }
        }

        //Delete campaign
        public static void Delete(int id)
        {
            try
            {
                CampaignDTO CampaignDTO = new CampaignDTO();
                CampaignDTO = CampaignService.GetById(id);
                //Add Consumed Credits to clients
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(CampaignDTO.ClientId);
                ClientDTO.SMSCredit = ClientDTO.SMSCredit + CampaignDTO.ConsumedCredits;
                ClientService.Edit(ClientDTO);

                UnitOfWork uow = new UnitOfWork();
                uow.CampaignRepo.Delete(id);
                uow.SaveChanges();

              

            }
            catch
            {
                throw;
            }
        }

          //Edit Campaign
        public static void CancelCampaign(CampaignDTO campaignDTO)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();

                //Add Consumed Credits to clients
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(campaignDTO.ClientId);
                ClientDTO.SMSCredit = ClientDTO.SMSCredit + campaignDTO.ConsumedCredits;
                ClientService.Edit(ClientDTO);

                campaignDTO.ConsumedCredits = 0;
                campaignDTO.Status = "Cancelled";
                CampaignService.Edit(campaignDTO);

                Campaign Campaign = Transform.CampaignToDomain(campaignDTO);
                uow.CampaignRepo.Update(Campaign);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public static CampaignDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Campaign Campaign = uow.CampaignRepo.GetById(Id);
                CampaignDTO CampaignDTO = Transform.CampaignToDTO(Campaign);

                //TemplateDTO TemplateDTO = new TemplateDTO();
                CampaignDTO.TemplateDTO = TemplateService.GetTemplateByMessage(CampaignDTO.Message);


                //List<GroupDTO> GroupDTOList = new List<GroupDTO>();
                //GroupDTOList = CommonService.GetGroupList(CampaignDTO.Groups);
                //CampaignDTO.GroupDTOList = GroupDTOList;
                if(CampaignDTO.Status == "Unsend")// (CampaignDTO.IsSent == false)
                {
                    if (CampaignDTO.GroupId != null || CampaignDTO.GroupId > 0)
                    {
                        //List<GroupDTO> GroupDTOList = new List<GroupDTO>();
                        GroupDTO GroupDTO = new GroupDTO();
                        //GroupDTOList = CommonService.Get(EcouponCampaignDTO.Groups);
                        GroupDTO = GroupService.GetByIdWithContactCount(Convert.ToInt32(CampaignDTO.GroupId));
                        CampaignDTO.GroupName = GroupDTO.Name;
                        CampaignDTO.GroupContactCount = GroupDTO.ContactCount;
                    }

                    if (CampaignDTO.ForAllContact == true)
                    {
                        CampaignDTO.GroupId = 0;
                        CampaignDTO.GroupContactCount = uow.ContactRepo.GetAll().Where(e => e.ClientId == CampaignDTO.ClientId).Count();
                        CampaignDTO.GroupName = "All Contacts";
                    }
                }

                if (CampaignDTO.ForAllContact == true && CampaignDTO.Status == "Sent")// CampaignDTO.IsSent == true)
                {
                    CampaignDTO.GroupId = 0;
                    CampaignDTO.GroupContactCount = CampaignDTO.RecipientsCount;
                    CampaignDTO.GroupName = "All Contacts";
                }

                if (CampaignDTO.Status == "Sent" && CampaignDTO.GroupId != null || CampaignDTO.GroupId > 0) // CampaignDTO.IsSent == true
                {
                    //List<GroupDTO> GroupDTOList = new List<GroupDTO>();
                    GroupDTO GroupDTO = new GroupDTO();
                    //GroupDTOList = CommonService.Get(EcouponCampaignDTO.Groups);
                    if (CampaignDTO.GroupId > 0)
                    {
                        GroupDTO = GroupService.GetByIdWithContactCount(Convert.ToInt32(CampaignDTO.GroupId));
                        CampaignDTO.GroupName = GroupDTO.Name;
                    }

                }

                return CampaignDTO;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region "List Functionality"

        // Returns Full list which IsSent status is false
        public static List<CampaignDTO> GetCampaignNotSentList()
        {

            List<CampaignDTO> CampaignDTOList = new List<CampaignDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e =>e.Status == CampaignStatus.Unsend );//e.IsSent == false
                    if (Campaign != null)
                    {
                        foreach (var item in Campaign)
                        {
                            CampaignDTOList.Add(Transform.CampaignToDTO(item));
                        }
                    }
                }

                return CampaignDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        // Returns Client wise list
        public static List<CampaignDTO> GetCampaignListByClientId(int ClientId)
        {

            List<CampaignDTO> CampaignDTOList = new List<CampaignDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId).OrderByDescending(e => e.CreatedDate).ToList();
                    if (Campaign != null)
                    {
                        foreach (var item in Campaign)
                        {
                            CampaignDTOList.Add(Transform.CampaignToDTO(item));
                        }
                    }
                }

                return CampaignDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        // Returns Client wise list
        public static List<CampaignDTO> GetCampaignListByFilters(int ClientId, string Name, DateTime CreatedDate, DateTime ScheduledDate)
        {

            List<CampaignDTO> CampaignDTOList = new List<CampaignDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.Name.ToLower().Contains(Name.ToLower()) || (e.CreatedDate >= CreatedDate && e.CreatedDate < CreatedDate.AddDays(1) || e.ScheduledDate >= ScheduledDate && e.ScheduledDate < ScheduledDate.AddDays(1))).OrderByDescending(e => e.CreatedDate).ToList();
                    if (Campaign != null)
                    {
                        foreach (var item in Campaign)
                        {
                            CampaignDTOList.Add(Transform.CampaignToDTO(item));
                        }
                    }
                }

                return CampaignDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Returns Campaign list as per client and search 
        public static List<CampaignDTO> GetCampaignsbyClientId(int ClientId, string search, PagingInfo pagingInfo)
        {

            List<CampaignDTO> CampaignDTO = new List<CampaignDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();

                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                IQueryable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId).OrderByDescending(e => e.CreatedDate).AsQueryable();// .Skip(skip).Take(take).ToList();

                Campaign = PagingService.Sorting<Campaign>(Campaign, pagingInfo.SortBy, pagingInfo.Reverse);
                Campaign = Campaign.Skip(skip).Take(take);


                if (Campaign != null)
                {
                    if (search != "" && search != null)
                    {

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {
                            // string search
                            IQueryable<Campaign> Campaignsearch = uow.CampaignRepo.GetAll().Where(e => (e.Name.ToLower().Contains(search.ToLower()) || e.RecipientsNumber.Contains(search) || e.RequiredCredits.ToString() == (search) || (e.ScheduledDate.ToString() != null ? (Convert.ToDateTime(e.ScheduledDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)) && (e.ClientId == ClientId)).AsQueryable();//.OrderByDescending(e => e.CreatedDate).Skip(skip).Take(take);

                            Campaignsearch = PagingService.Sorting<Campaign>(Campaignsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Campaignsearch = Campaignsearch.Skip(skip).Take(take);

                            if (Campaignsearch != null)
                            {
                                foreach (var item in Campaignsearch)
                                {

                                    CampaignDTO.Add(Transform.CampaignToDTO(item));
                                }
                            }
                            return CampaignDTO;
                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(search);
                            IQueryable<Campaign> Campaignsearch = uow.CampaignRepo.GetAll().Where(e => (e.CreatedDate >= date && e.CreatedDate < date.AddDays(1) || e.ScheduledDate >= date && e.ScheduledDate < date.AddDays(1)) && (e.ClientId == ClientId)).AsQueryable();// .OrderByDescending(e => e.CreatedDate).Skip(skip).Take(take);
                            Campaignsearch = PagingService.Sorting<Campaign>(Campaignsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Campaignsearch = Campaignsearch.Skip(skip).Take(take);

                            if (Campaignsearch != null)
                            {
                                foreach (var item in Campaignsearch)
                                {
                                    CampaignDTO.Add(Transform.CampaignToDTO(item));
                                }
                            }
                            return CampaignDTO;

                        }

                    }
                    else
                    {


                        foreach (var item in Campaign)
                        {
                            CampaignDTO.Add(Transform.CampaignToDTO(item));
                        }
                    }
                }

                return CampaignDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Returns Campaign paged list as per client and search with pagingInfo obect
        public static PageData<CampaignDTO> GetCampaignPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            List<CampaignDTO> CampaignDTOList = new List<CampaignDTO>();
            PageData<CampaignDTO> pageList = new PageData<CampaignDTO>();

            if (pagingInfo == null)
            {
                PagingInfo PagingInfoCreated = new PagingInfo();
                PagingInfoCreated.Page = 1;
                PagingInfoCreated.Reverse = false;
                PagingInfoCreated.ItemsPerPage = 1;
                PagingInfoCreated.Search = "";
                PagingInfoCreated.TotalItem = 0;

                pagingInfo = PagingInfoCreated;
            }
            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "CreatedDate";
            }

            CampaignDTOList = GetCampaignsbyClientId(ClientId, pagingInfo.Search, pagingInfo);
            IQueryable<CampaignDTO> CampaignDTOPagedList = CampaignDTOList.AsQueryable();
            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.CampaignRepo.GetAll().Where(e => e.Name.ToLower().Contains(pagingInfo.Search.ToLower()) || e.RecipientsNumber.Contains(pagingInfo.Search) || e.RequiredCredits.ToString() == (pagingInfo.Search) || (e.ScheduledDate.ToString() != null ? (Convert.ToDateTime(e.ScheduledDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).OrderByDescending(e => e.CreatedDate).Count();
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.CampaignRepo.GetAll().Where(e => e.CreatedDate >= date && e.CreatedDate < date.AddDays(1) || e.ScheduledDate >= date && e.ScheduledDate < date.AddDays(1)).OrderByDescending(e => e.CreatedDate).Count();

                }

            }
            else
            {
                count = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId).Count();
            }

            ////Sorting
            //CampaignDTOPagedList = PagingService.Sorting<CampaignDTO>(CampaignDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CampaignDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CampaignDTO>(CampaignDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// CampaignDTOPagedList.Count();

                List<CampaignDTO> pagedCampaignDTOList = new List<CampaignDTO>();
                foreach (var item in CampaignDTOPagedList)
                {
                    if (item.Status == "Unsend" && item.GroupId != null)//item.IsSent == false
                    {
                        GroupContactDTO GroupDTO = new GroupContactDTO();
                        GroupDTO GroupDTOTemp = new GroupDTO();
                        GroupDTOTemp = GroupService.GetById(Convert.ToInt32(item.GroupId));
                        //GroupDTO = GroupService.GetGroupContactById(GroupDTOTemp.Id);

                        //List<ContactDTO> ContactDTOList = new List<ContactDTO>();
                        //ContactDTOList = GroupContactService.GetGroupIdWiseContacts(GroupDTOTemp.Id);

                        int CotactCount = GroupContactService.GetGroupIdWiseContactsCount(GroupDTOTemp.Id, "");

                        item.GroupName = GroupDTOTemp.Name;// GroupDTO.Name;
                        item.GroupContactCount = item.RecipientsCount;

                        if (item.RecipientsCount != CotactCount) // GroupDTO.Contacts.Count()
                        {
                            item.RecipientsCount = CotactCount;// GroupDTO.Contacts.Count();
                            item.GroupContactCount = item.RecipientsCount;
                            item.RequiredCredits = item.RecipientsCount * item.MessageCount;
                            CampaignService.Edit(item);
                        }

                        if (CotactCount == 1) //GroupDTO.Contacts.Count()
                        {
                            item.GroupContactCount = null;
                            List<ContactDTO> ContactDTOList = GroupContactService.GetGroupIdWiseContacts(GroupDTOTemp.Id).ToList();
                            item.RecipientsNumber = ContactDTOList[0].MobileNumber;// GroupDTO.Contacts[0].MobileNumber;
                            item.RecipientsCount = 1;
                        }
                    }
                    else if (item.Status == "Unsend" && item.ForAllContact == true)//item.IsSent == false
                    {
                        int CotactCount = uow.ContactRepo.GetAll().Where(e => e.ClientId == item.ClientId).Count();
                        item.GroupContactCount = CotactCount;
                        if (item.RecipientsCount != CotactCount) // GroupDTO.Contacts.Count()
                        {
                            item.GroupContactCount = CotactCount;
                            item.RecipientsCount = CotactCount;
                            item.RequiredCredits = item.RecipientsCount * item.MessageCount;
                            CampaignService.Edit(item);
                        }

                        if (CotactCount == 1) //GroupDTO.Contacts.Count()
                        {
                            item.GroupContactCount = null;
                            List<ContactDTO> ContactDTOList = ContactService.GetListByClientId(item.ClientId).ToList();
                            item.RecipientsNumber = ContactDTOList[0].MobileNumber;// GroupDTO.Contacts[0].MobileNumber;
                            item.RecipientsCount = 1;
                        }

                    }

                    pagedCampaignDTOList.Add(item);
                }
                pageList.Data = pagedCampaignDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Returns Campaign list as per partner and search
        public static List<CampaignDTO> GetCampaignsbyPartnerId(int PartnerId, string search, PagingInfo pagingInfo)
        {

            List<CampaignDTO> CampaignDTO = new List<CampaignDTO>();
            List<Client> Clients = new List<Client>();

            int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
            int take = pagingInfo.ItemsPerPage;

            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId).ToList();

                if (Client != null)
                {
                    foreach (var itemClient in Client)
                    {
                        PartnerDTO PartnerDTO = new PartnerDTO();
                        PartnerDTO = PartnerService.GetById(PartnerId);
                        itemClient.Partner = Transform.PartnerToDomain(PartnerDTO);
                        Clients.Add(itemClient);
                    }

                    List<CampaignDTO> CampaignsDTO = new List<CampaignDTO>();
                    if (Clients != null)
                    {
                        foreach (var itemCampaign in Clients)
                        {
                            CampaignsDTO = GetCampaignListByClientId(itemCampaign.Id);
                        }



                        var Campaign = uow.CampaignRepo.GetAll().OrderByDescending(e => e.CreatedDate).ToList().Skip(skip).Take(take);// uow.CampaignRepo.GetAll().Where(e => e.Client.PartnerId == PartnerId).ToList();


                        if (Campaign != null)
                        {
                            if (search != "" && search != null)
                            {

                                bool IsDate = CommonService.IsDate(search);
                                if (IsDate != true)
                                {
                                    // string search
                                    var Campaignsearch = uow.CampaignRepo.GetAll().Where(e => e.Name.ToLower().Contains(search.ToLower()) || e.RecipientsNumber.Contains(search) || e.RequiredCredits.ToString() == (search) || (e.ScheduledDate.ToString() != null ? (Convert.ToDateTime(e.ScheduledDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).OrderByDescending(e => e.CreatedDate).Skip(skip).Take(take);

                                    if (Campaignsearch != null)
                                    {
                                        foreach (var item in Campaignsearch)
                                        {

                                            CampaignDTO.Add(Transform.CampaignToDTO(item));
                                        }
                                    }
                                    return CampaignDTO;
                                }
                                else
                                {
                                    //date wise search
                                    DateTime date = Convert.ToDateTime(search);
                                    var Campaignsearch = uow.CampaignRepo.GetAll().Where(e => e.CreatedDate >= date && e.CreatedDate < date.AddDays(1) || e.ScheduledDate >= date && e.ScheduledDate < date.AddDays(1)).OrderByDescending(e => e.CreatedDate).Skip(skip).Take(take);

                                    if (Campaignsearch != null)
                                    {
                                        foreach (var item in Campaignsearch)
                                        {
                                            CampaignDTO.Add(Transform.CampaignToDTO(item));
                                        }
                                    }
                                    return CampaignDTO;

                                }

                            }
                            else
                            {


                                foreach (var item in Campaign)
                                {
                                    CampaignDTO.Add(Transform.CampaignToDTO(item));
                                }
                            }
                        }

                    }

                }

                return CampaignDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Returns Campaign paged list as per partner and search
        public static PageData<CampaignDTO> GetCampaignPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        {
            List<CampaignDTO> CampaignDTOList = new List<CampaignDTO>();
            PageData<CampaignDTO> pageList = new PageData<CampaignDTO>();

            if (pagingInfo == null)
            {
                PagingInfo PagingInfoCreated = new PagingInfo();
                PagingInfoCreated.Page = 1;
                PagingInfoCreated.Reverse = false;
                PagingInfoCreated.ItemsPerPage = 1;
                PagingInfoCreated.Search = "";
                PagingInfoCreated.TotalItem = 0;

                pagingInfo = PagingInfoCreated;
            }

            CampaignDTOList = GetCampaignsbyPartnerId(PartnerId, pagingInfo.Search, pagingInfo);
            IQueryable<CampaignDTO> CampaignDTOPagedList = CampaignDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.CampaignRepo.GetAll().Where(e => e.Name.ToLower().Contains(pagingInfo.Search.ToLower()) || e.RecipientsNumber.Contains(pagingInfo.Search) || e.RequiredCredits.ToString() == (pagingInfo.Search) || (e.ScheduledDate.ToString() != null ? (Convert.ToDateTime(e.ScheduledDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).Count();
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.CampaignRepo.GetAll().Where(e => e.CreatedDate >= date && e.CreatedDate < date.AddDays(1) || e.ScheduledDate >= date && e.ScheduledDate < date.AddDays(1)).Count();

                }

            }
            else
            {
                count = uow.CampaignRepo.GetAll().Count();
            }

            ////Sorting
            //CampaignDTOPagedList = PagingService.Sorting<CampaignDTO>(CampaignDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CampaignDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CampaignDTO>(CampaignDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// CampaignDTOPagedList.Count();

                List<CampaignDTO> pagedCampaignDTOList = new List<CampaignDTO>();
                foreach (var item in CampaignDTOPagedList)
                {
                    pagedCampaignDTOList.Add(item);
                }
                pageList.Data = pagedCampaignDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Returns Campaign recent blast as per client and top 'n' values 
        public static List<CampaignDTO> GetCampaignRecentBlastByClientId(int ClientId, int Top)
        {

            List<CampaignDTO> CampaignDTOList = new List<CampaignDTO>();

            try
            {
                int count = 0;
                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId).OrderByDescending(e => e.Id).ToList();
                    if (Campaign != null)
                    {
                        foreach (var item in Campaign)
                        {
                            count = count + 1;

                            CampaignDTOList.Add(Transform.CampaignToDTO(item));
                            if (count == Top)
                            {
                                return CampaignDTOList;
                            }
                        }
                    }
                }

                return CampaignDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Returns day wise Campaign and coupon message as per client 
        public static List<MessageSentCountDTO> GetDayWiseCampaignAndCouponMsssageSentCount(int ClientId)
        {
            int From = 0;
            int To = 30;
            int MaxTo = 360;
            int CampaignSMSCount = 0;
            int EcouponCampaignSMSCount = 0;

            List<MessageSentCountDTO> MessageSentCountDTOList = new List<MessageSentCountDTO>();
            try
            {
                for (To = 30; To <= MaxTo; To = To + 30)
                {
                    int Fromdays = From;
                    if (From != 0)
                    {
                        Fromdays = -From;
                    }

                    int Todays = -To;



                    UnitOfWork uow = new UnitOfWork();
                    IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.Status == CampaignStatus.Sent && e.CreatedDate > System.DateTime.Now.Date.AddDays(Todays) && e.CreatedDate < System.DateTime.Now.Date.AddDays(Fromdays)).OrderByDescending(e => e.Id); //e.IsSent == true|| e.ScheduledDate > System.DateTime.Now.AddDays(-Todays) && e.ScheduledDate < System.DateTime.Now.AddDays(Fromdays)

                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsSent == true && e.SendOn > System.DateTime.Now.Date.AddDays(Todays) && e.SendOn < System.DateTime.Now.Date.AddDays(Fromdays));


                    MessageSentCountDTO MessageSentCountDTO = new MessageSentCountDTO();
                    MessageSentCountDTO.Days = From + " To " + To;
                    foreach (var item in Campaign)
                    {

                        CampaignSMSCount = CampaignSMSCount + item.RecipientsCount;
                        MessageSentCountDTO.CampaignCount = CampaignSMSCount;
                    }

                    foreach (var item in EcouponCampaign)
                    {

                        EcouponCampaignSMSCount = EcouponCampaignSMSCount + item.RecipientsCount;
                        MessageSentCountDTO.EcouponCampaignCount = EcouponCampaignSMSCount;
                    }



                    MessageSentCountDTO.TotalCount = MessageSentCountDTO.EcouponCampaignCount + MessageSentCountDTO.CampaignCount;

                    MessageSentCountDTOList.Add(MessageSentCountDTO);

                    EcouponCampaignSMSCount = 0;
                    CampaignSMSCount = 0;
                    if (To == 30)
                    {
                        From = From + To + 1;
                    }
                    else
                    {
                        From = From + 30;
                    }

                }



                return MessageSentCountDTOList;

            }
            catch
            {
                throw;
            }

        }

        //Returns month wise Campaign and coupon message as per client
        public static List<MessageSentCountDTO> GetMonthWiseCampaignAndCouponMsssageSentCount(int ClientId)
        {
            string From = CommonService.FirstDayOfMonth(System.DateTime.Now.Date);
            string To = CommonService.LastDayOfMonth(Convert.ToDateTime(From));
            int MaxTo = 6;
            int CampaignSMSCount = 0;
            int EcouponCampaignSMSCount = 0;

            List<MessageSentCountDTO> MessageSentCountDTOList = new List<MessageSentCountDTO>();
            try
            {
                for (MaxTo = 1; MaxTo <= 6; MaxTo++)
                {

                    UnitOfWork uow = new UnitOfWork();
                    IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.Status == CampaignStatus.Sent && (e.CreatedDate >= Convert.ToDateTime(From) && e.CreatedDate <= Convert.ToDateTime(To)) && (e.ScheduledDate >= Convert.ToDateTime(From) && e.ScheduledDate <= Convert.ToDateTime(To))); //e.IsSent == true

                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsSent == true && (e.SendOn >= Convert.ToDateTime(From) && e.SendOn <= Convert.ToDateTime(To)) && (e.CreatedDate >= Convert.ToDateTime(From) && e.CreatedDate <= Convert.ToDateTime(To)));


                    MessageSentCountDTO MessageSentCountDTO = new MessageSentCountDTO();
                    if (MaxTo == 1)
                    {
                        MessageSentCountDTO.Days = "Current Month";
                    }
                    else
                    {
                        DateTime month = Convert.ToDateTime(From);
                        //MessageSentCountDTO.Days = Convert.ToDateTime(From).Month.ToString();
                        MessageSentCountDTO.Days = month.ToString("MMMMMMM");
                    }

                    foreach (var item in Campaign)
                    {

                        CampaignSMSCount = CampaignSMSCount + item.RecipientsCount;
                        MessageSentCountDTO.CampaignCount = CampaignSMSCount;
                    }

                    foreach (var item in EcouponCampaign)
                    {

                        EcouponCampaignSMSCount = EcouponCampaignSMSCount + item.RecipientsCount;
                        MessageSentCountDTO.EcouponCampaignCount = EcouponCampaignSMSCount;
                    }



                    MessageSentCountDTO.TotalCount = MessageSentCountDTO.EcouponCampaignCount + MessageSentCountDTO.CampaignCount;

                    MessageSentCountDTOList.Add(MessageSentCountDTO);

                    EcouponCampaignSMSCount = 0;
                    CampaignSMSCount = 0;

                    From = Convert.ToDateTime(From).AddMonths(-1).ToString();
                    To = Convert.ToDateTime(To).AddMonths(-1).ToString();
                    From = CommonService.FirstDayOfMonth(Convert.ToDateTime(From));
                    To = CommonService.LastDayOfMonth(Convert.ToDateTime(From));
                }



                return MessageSentCountDTOList;

            }
            catch
            {
                throw;
            }

        }

        //Returns Contact paged list as per campaign and client with search criteria
        public static PageData<ContactDTO> GetContactsByCampaignId(PagingInfo pagingInfo, int CampaignId, int ClientId)
        {
            UnitOfWork uow = new UnitOfWork();
            CampaignDTO CampaignDTO = new CampaignDTO();
            PageData<ContactDTO> pageList = new PageData<ContactDTO>();

            CampaignDTO = GetById(CampaignId);

            if (CampaignDTO != null)
            {
                string[] array;
                if (CampaignDTO.RecipientsNumber != "")
                {
                    string value = CampaignDTO.RecipientsNumber;
                    array = value.Replace(" ", "").Split(',');

                    List<ContactDTO> ContactDTOList = new List<ContactDTO>();
                    Contact Contact = new Contact();
                    ContactDTO ContactDTO = new ContactDTO();

                    int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                    int take = pagingInfo.ItemsPerPage;

                    var newlist = array.Skip(skip).Take(take).ToArray();

                    if (pagingInfo.Search != "")
                    {
                        ContactDTOList = SearchContacts(pagingInfo.Search, array, CampaignId, ClientId);
                        pageList.Data = ContactDTOList;
                        pageList.Count = ContactDTOList.Count;
                    }
                    else
                    {
                        for (int i = 0; i < newlist.Length; i++)
                        {
                            Contact = uow.ContactRepo.GetAll().Where(e => e.MobileNumber == newlist[i] && e.ClientId == ClientId).FirstOrDefault();
                            if (Contact == null)
                            {
                                Contact = new Contact();
                                Contact.MobileNumber = newlist.ToList()[i];
                                Contact.Gender = null;
                                Contact.Id = 0;
                                Contact.ClientId = 0;
                                //Contact.Name = null;
                            }

                            ContactDTO = Transform.ContactToDTO(Contact);

                            ContactDTOList.Add(ContactDTO);
                        }
                        pageList.Data = ContactDTOList;
                        pageList.Count = array.Length;
                    }
                }
                else if (CampaignDTO.GroupId != null)
                {
                    // GroupContactDTO GroupContactDTO =  GroupService.GetGroupContactById((int)CampaignDTO.GroupId);
                    pagingInfo.GroupId = (int)CampaignDTO.GroupId;
                    pageList = ContactService.GetGroupIdWisePresentContactsPagedListByClientId(pagingInfo, ClientId);
                }


            }
            else
            {
                pageList.Data = null;
            }

            return pageList;
        }

        //Returns Contact list as per receipent numbers, campaign and client with search criteria
        public static List<ContactDTO> SearchContacts(string search, string[] receipients, int CampaignId, int ClientId)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            List<Contact> Contacts = new List<Contact>();
            UnitOfWork uow = new UnitOfWork();
            Contact Contact = new Contact();

            if (receipients.Contains(search))
            {
                int count = 0;
                Contacts = uow.ContactRepo.GetAll().Where(e => e.MobileNumber == search && e.ClientId == ClientId).ToList();

                if (Contacts.Count != 0)
                {
                    foreach (var item in Contacts)
                    {
                        count = count + 1;
                        ContactDTOList.Add(Transform.ContactToDTO(item));
                    }
                }
                else
                {
                    Contact = new Contact();
                    ContactDTO ContactDTO = new ContactDTO();

                    Contact.MobileNumber = search;
                    Contact.Gender = null;
                    Contact.Id = 0;
                    Contact.ClientId = 0;
                    //Contact.Name = null;

                    ContactDTO = Transform.ContactToDTO(Contact);
                    ContactDTOList.Add(ContactDTO);
                }
            }
            else
            {
                int count = 0;
                Contacts = uow.ContactRepo.GetAll().Where(e => e.Name.ToLower().Equals(search.ToLower()) || (e.Email != null ? (e.Email.ToLower().Contains(search.ToLower())) : false) && e.ClientId == ClientId).ToList();

                if (Contacts != null)
                {
                    foreach (var item in Contacts)
                    {
                        count = count + 1;
                        ContactDTOList.Add(Transform.ContactToDTO(item));
                    }
                }
            }
            return ContactDTOList;
        }

        #endregion

        #region "Other Functionality"
        // Returns count of todays message sent 
        public static int GetTodaysCampaignMsssageSentCount(int ClientId)
        {
            try
            {
                int TodaysSMSCount = 0;
                UnitOfWork uow = new UnitOfWork();

                IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.Status == CampaignStatus.Sent && (e.ScheduledDate >= System.DateTime.Now.Date && e.ScheduledDate < System.DateTime.Now.Date.AddDays(1)));//e.IsSent == true
                foreach (var item in Campaign)
                {
                    TodaysSMSCount = TodaysSMSCount + item.RecipientsCount;
                }

                return TodaysSMSCount;

            }
            catch
            {
                throw;
            }

        }



        #endregion

    }

}

using MsgBlaster.Repo;
using MsgBlaster.Domain;
using MsgBlaster.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MsgBlaster.Service
{
    public class EcouponCampaignService
    {
        #region "CRUD Functionality"

        //Create ecoupon campaign
        public static int Create(EcouponCampaignDTO EcouponCampaignDTO)
        {

            try
            {
                var EcouponCampaign = new EcouponCampaign();

                GlobalSettings.LoggedInClientId = EcouponCampaignDTO.ClientId;
                GlobalSettings.LoggedInUserId = EcouponCampaignDTO.CreatedBy;

                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(EcouponCampaignDTO.ClientId);
                int PartnerId = ClientDTO.PartnerId;// ClientService.GetById(EcouponCampaignDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                //If SMS Credit balance is low then should not create campaign
                if (ClientDTO.SMSCredit < EcouponCampaignDTO.ConsumedCredits)
                {
                    return 0;
                }

                UnitOfWork uow = new UnitOfWork();
                if (EcouponCampaignDTO.ReceipentNumber != null && EcouponCampaignDTO.ReceipentNumber != "")
                {
                    EcouponCampaignDTO.ReceipentNumber = CommonService.RemoveDuplicateMobile(EcouponCampaignDTO.ReceipentNumber);
                }
                if (EcouponCampaignDTO.GroupId == null)
                {
                    EcouponCampaignDTO.RecipientsCount = CommonService.GetRecipientsCount(EcouponCampaignDTO.ReceipentNumber);
                }

                EcouponCampaignDTO.IPAddress = CommonService.GetIP();

                if (EcouponCampaignDTO.ForAllContact == true)
                {
                    EcouponCampaignDTO.ReceipentNumber = ContactService.GetAllReceipentNumberByClientId(EcouponCampaignDTO.ClientId);
                    EcouponCampaignDTO.RecipientsCount = CommonService.GetRecipientsCount(EcouponCampaignDTO.ReceipentNumber);
                    EcouponCampaignDTO.GroupId = null;
                    EcouponCampaignDTO.ReceipentNumber = "";
                }

                EcouponCampaignDTO.IsReconcile = false;
                EcouponCampaignDTO.ReconcileDate = System.DateTime.Now.Date;

                EcouponCampaign = Transform.EcouponCampaignToDomain(EcouponCampaignDTO);
                uow.EcouponCampaignRepo.Insert(EcouponCampaign);
                uow.SaveChanges();
                EcouponCampaignDTO.Id = EcouponCampaign.Id;

                // Deduct SMS credit balance                 
              
                ClientDTO.SMSCredit = ClientDTO.SMSCredit - EcouponCampaignDTO.ConsumedCredits;
                ClientService.Edit(ClientDTO);

                return EcouponCampaignDTO.Id;

            }
            catch (Exception)
            {
                throw;
            }
        }

        //Edit ecoupon campaign
        public static void Edit(EcouponCampaignDTO EcouponCampaignDTO)
        {
            try
            {

                //Get previous ecoupon balance and restore the client balance 
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(EcouponCampaignDTO.ClientId);
                EcouponCampaignDTO EcouponCampaignDTODRestore = new EcouponCampaignDTO();
                EcouponCampaignDTODRestore = GetById(EcouponCampaignDTO.Id);
                ClientDTO.SMSCredit = ClientDTO.SMSCredit + EcouponCampaignDTODRestore.ConsumedCredits;
                ClientService.Edit(ClientDTO);



                GlobalSettings.LoggedInClientId = EcouponCampaignDTO.ClientId;
                GlobalSettings.LoggedInUserId = EcouponCampaignDTO.CreatedBy;
                int PartnerId = ClientService.GetById(EcouponCampaignDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                //If SMS Credit balance is greater or equal to ConsumededCredits then create campaign
                if (ClientDTO.SMSCredit >= EcouponCampaignDTO.ConsumedCredits)              
                {
                        UnitOfWork uow = new UnitOfWork();
                        //EcouponCampaignDTO.MessageCount = CommonService.GetMessageCount(EcouponCampaignDTO.Message);
                        if (EcouponCampaignDTO.ReceipentNumber != null && EcouponCampaignDTO.ReceipentNumber != "")
                        {
                            EcouponCampaignDTO.ReceipentNumber = CommonService.RemoveDuplicateMobile(EcouponCampaignDTO.ReceipentNumber);
                        }

                        if (EcouponCampaignDTO.GroupId == null)
                        {
                            EcouponCampaignDTO.RecipientsCount = CommonService.GetRecipientsCount(EcouponCampaignDTO.ReceipentNumber);
                        }
                        EcouponCampaignDTO.IPAddress = CommonService.GetIP();

                        if (EcouponCampaignDTO.ForAllContact == true)
                        {

                            EcouponCampaignDTO.ReceipentNumber = ContactService.GetAllReceipentNumberByClientId(EcouponCampaignDTO.ClientId);
                            EcouponCampaignDTO.RecipientsCount = CommonService.GetRecipientsCount(EcouponCampaignDTO.ReceipentNumber);
                            EcouponCampaignDTO.GroupId = null;
                            EcouponCampaignDTO.Group = null;
                            EcouponCampaignDTO.ReceipentNumber = "";
                        }
                        if (EcouponCampaignDTO.GroupId > 0)
                        {
                            EcouponCampaignDTO.Group = null;
                        }

                        EcouponCampaignDTO.IsReconcile = false;
                        EcouponCampaignDTO.ReconcileDate = System.DateTime.Now.Date;

                        EcouponCampaign EcouponCampaign = Transform.EcouponCampaignToDomain(EcouponCampaignDTO);
                        uow.EcouponCampaignRepo.Update(EcouponCampaign);
                        uow.SaveChanges();


                       //Deduct client balance
                        ClientDTO.SMSCredit = ClientDTO.SMSCredit - EcouponCampaignDTO.ConsumedCredits;
                        ClientService.Edit(ClientDTO);                
                }
            }
            catch
            {
                throw;
            }
        }

        //Edit for ecoupon resend
        public static void EditForEcouponResend(EcouponCampaignDTO EcouponCampaignDTO)
        {
            GlobalSettings.LoggedInClientId = EcouponCampaignDTO.ClientId;
            GlobalSettings.LoggedInUserId = EcouponCampaignDTO.CreatedBy;
            int PartnerId = ClientService.GetById(EcouponCampaignDTO.ClientId).PartnerId;
            GlobalSettings.LoggedInPartnerId = PartnerId;

            UnitOfWork uow = new UnitOfWork();
            if (EcouponCampaignDTO.GroupId > 0)
            {
                EcouponCampaignDTO.Group = null;
            }
            EcouponCampaign EcouponCampaign = Transform.EcouponCampaignToDomain(EcouponCampaignDTO);
            uow.EcouponCampaignRepo.Update(EcouponCampaign);
            uow.SaveChanges();
        }


        //Edit ecoupon campaign from backend process
        public static void EditEcouponCampaignFromBackend(EcouponCampaignDTO EcouponCampaignDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = EcouponCampaignDTO.ClientId;
                GlobalSettings.LoggedInUserId = EcouponCampaignDTO.CreatedBy;
                int PartnerId = ClientService.GetById(EcouponCampaignDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                UnitOfWork uow = new UnitOfWork();
                //EcouponCampaignDTO.MessageCount = CommonService.GetMessageCount(EcouponCampaignDTO.Message);        
                if (EcouponCampaignDTO.GroupId > 0)
                {
                    EcouponCampaignDTO.Group = null;
                }
                EcouponCampaign EcouponCampaign = Transform.EcouponCampaignToDomain(EcouponCampaignDTO);
                uow.EcouponCampaignRepo.Update(EcouponCampaign);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Create ecoupon campaign backend
        public static int CreateEcouponCampaignBackend(EcouponCampaignDTO EcouponCampaignDTO)
        {

            try
            {
                GlobalSettings.LoggedInClientId = EcouponCampaignDTO.ClientId;
                GlobalSettings.LoggedInUserId = EcouponCampaignDTO.CreatedBy;

                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(EcouponCampaignDTO.ClientId);
                int PartnerId = ClientDTO.PartnerId;// ClientService.GetById(EcouponCampaignDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                //If SMS Credit balance is low then should not create campaign
                if (ClientDTO.SMSCredit < EcouponCampaignDTO.ConsumedCredits)
                {
                    return 0;
                }

                var EcouponCampaign = new EcouponCampaign();

                UnitOfWork uow = new UnitOfWork();
                if (EcouponCampaignDTO.ReceipentNumber != null && EcouponCampaignDTO.ReceipentNumber != "")
                {
                    EcouponCampaignDTO.ReceipentNumber = CommonService.RemoveDuplicateMobile(EcouponCampaignDTO.ReceipentNumber);
                }

                if (EcouponCampaignDTO.GroupId == null)
                {
                    EcouponCampaignDTO.RecipientsCount = CommonService.GetRecipientsCount(EcouponCampaignDTO.ReceipentNumber);
                }

                EcouponCampaignDTO.IsReconcile = false;
                EcouponCampaignDTO.ReconcileDate = System.DateTime.Now.Date;

                //Calculate consumed credits
                double ConsumedCreditPerOneMsg = CommonService.GetConsumedCreditsForOneMessage(EcouponCampaignDTO.Message, true);
                int RecepientsCount = CommonService.GetRecipientsCount(EcouponCampaignDTO.ReceipentNumber);
                EcouponCampaignDTO.ConsumedCredits = RecepientsCount * ConsumedCreditPerOneMsg;


                EcouponCampaign = Transform.EcouponCampaignToDomain(EcouponCampaignDTO);
                uow.EcouponCampaignRepo.Insert(EcouponCampaign);
                uow.SaveChanges();
                EcouponCampaignDTO.Id = EcouponCampaign.Id;

                //Deduct clients balance
               
                ClientDTO.SMSCredit = ClientDTO.SMSCredit - EcouponCampaignDTO.ConsumedCredits;
                ClientService.Edit(ClientDTO);

                return EcouponCampaignDTO.Id;
            }

            catch (Exception)
            {
                throw;
            }
        }

        //Delete ecoupon campaign
        public static void Delete(int id)
        {
            try
            {

                EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                EcouponCampaignDTO = EcouponCampaignService.GetById(id);
                //Add Consumed Credits to clients
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(EcouponCampaignDTO.ClientId);
                ClientDTO.SMSCredit = ClientDTO.SMSCredit + EcouponCampaignDTO.ConsumedCredits;
                ClientService.Edit(ClientDTO);

                UnitOfWork uow = new UnitOfWork();
                uow.EcouponCampaignRepo.Delete(id);
                uow.SaveChanges();

             

            }
            catch
            {
                throw;
            }
        }

        //Get ecoupon campaign by id
        public static EcouponCampaignDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                EcouponCampaign EcouponCampaign = uow.EcouponCampaignRepo.GetById(Id);
                EcouponCampaignDTO EcouponCampaignDTO = Transform.EcouponCampaignToDTO(EcouponCampaign);

                EcouponCampaignDTO.TemplateDTO = TemplateService.GetTemplateByMessage(EcouponCampaignDTO.Message);
                if (EcouponCampaignDTO.IsSent == false)
                {
                    if (EcouponCampaignDTO.GroupId != null || EcouponCampaignDTO.GroupId > 0)
                    {
                        //List<GroupDTO> GroupDTOList = new List<GroupDTO>();
                        GroupDTO GroupDTO = new GroupDTO();
                        //GroupDTOList = CommonService.Get(EcouponCampaignDTO.Groups);
                        GroupDTO = GroupService.GetByIdWithContactCount(Convert.ToInt32(EcouponCampaignDTO.GroupId));
                        EcouponCampaignDTO.Group = GroupDTO.Name;
                        EcouponCampaignDTO.GroupContactCount = GroupDTO.ContactCount;
                    }

                    if (EcouponCampaignDTO.ForAllContact == true)
                    {
                        EcouponCampaignDTO.GroupId = 0;
                        EcouponCampaignDTO.GroupContactCount = uow.ContactRepo.GetAll().Where(e => e.ClientId == EcouponCampaignDTO.ClientId).Count();
                        EcouponCampaignDTO.Group = "All Contacts";
                    }
                }
                else if (EcouponCampaignDTO.IsSent == true)
                {
                    if (EcouponCampaignDTO.ForAllContact == true)
                    {
                        EcouponCampaignDTO.GroupId = 0;
                        EcouponCampaignDTO.GroupContactCount = EcouponCampaignDTO.RecipientsCount;
                        EcouponCampaignDTO.Group = "All Contacts";
                    }

                    if (EcouponCampaignDTO.GroupId != null && EcouponCampaignDTO.GroupId > 0)
                    {
                        EcouponCampaignDTO.Group = GroupService.GetById(Convert.ToInt32(EcouponCampaignDTO.GroupId)).Name;
                    }
                }



                return EcouponCampaignDTO;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "List Functionality"

        // Returns Full list which IsSent status is false
        public static List<EcouponCampaignDTO> GetEcouponCampaignNotSentList()
        {

            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.IsSent == false);
                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            EcouponCampaignDTOList.Add(Transform.EcouponCampaignToDTO(item));
                        }
                    }
                }

                return EcouponCampaignDTOList;
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
        public static List<EcouponCampaignDTO> GetEcouponCampaignListByClientId(int ClientId)
        {

            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId).OrderByDescending(e => e.CreatedDate).ToList();
                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            EcouponCampaignDTOList.Add(Transform.EcouponCampaignToDTO(item));
                        }
                    }
                }

                return EcouponCampaignDTOList;
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
        public static List<EcouponCampaignDTO> GetEcouponCampaignListByFilters(int ClientId, string Name, DateTime CreatedDate, DateTime ScheduledDate, DateTime ExpiryDate)
        {

            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.Title.ToLower().Contains(Name.ToLower()) || (e.CreatedDate >= CreatedDate && e.CreatedDate < CreatedDate.AddDays(1) || e.SendOn >= ScheduledDate && e.SendOn < ScheduledDate.AddDays(1) || e.ExpiresOn >= ExpiryDate && e.ExpiresOn < ExpiryDate.AddDays(1))).OrderByDescending(e => e.CreatedDate).ToList();
                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            EcouponCampaignDTOList.Add(Transform.EcouponCampaignToDTO(item));
                        }
                    }
                }

                return EcouponCampaignDTOList;
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

        //Get ecoupon campaign list with search criteria and client id
        public static List<EcouponCampaignDTO> GetEcouponCampaignListSearchByClientId(int ClientId, string search, PagingInfo pagingInfo)
        {

            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();
            int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
            int take = pagingInfo.ItemsPerPage;


            try
            {

                using (var uow = new UnitOfWork())
                {
                    IQueryable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId).OrderByDescending(e => e.CreatedDate).AsQueryable();//.Skip(skip).Take(take).ToList();
                    EcouponCampaign = PagingService.Sorting<EcouponCampaign>(EcouponCampaign, pagingInfo.SortBy, pagingInfo.Reverse);
                    EcouponCampaign = EcouponCampaign.Skip(skip).Take(take);


                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            EcouponCampaignDTOList.Add(Transform.EcouponCampaignToDTO(item));
                        }

                        if (search != "" && search != null)
                        {
                            List<EcouponCampaignDTO> EcouponCampaignDTO = new List<EcouponCampaignDTO>();

                            bool IsDate = CommonService.IsDate(search);
                            if (IsDate != true)
                            {
                                // string search
                                IQueryable<EcouponCampaign> EcouponCampaignsearch = EcouponCampaign.Where(e => e.Title.ToLower().Contains(search.ToLower()) || e.Message.ToLower().Contains(search.ToLower()) || (e.ReceipentNumber != null ? (e.ReceipentNumber.Contains(search)) : false) || (e.ScheduleTime != null ? (e.ScheduleTime.Contains(search)) : false) || (e.IPAddress != null ? (e.IPAddress.Contains(search)) : false) || (e.SendOn.ToString() != null ? (Convert.ToDateTime(e.SendOn).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.ExpiresOn.ToString() != null ? (Convert.ToDateTime(e.ExpiresOn).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable();//.OrderByDescending(e => e.CreatedDate).Skip(skip).Take(take);
                                EcouponCampaignsearch = PagingService.Sorting<EcouponCampaign>(EcouponCampaignsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                EcouponCampaignsearch = EcouponCampaignsearch.Skip(skip).Take(take);

                                if (EcouponCampaignsearch != null)
                                {
                                    foreach (var item in EcouponCampaignsearch)
                                    {

                                        EcouponCampaignDTO.Add(Transform.EcouponCampaignToDTO(item));
                                    }
                                }
                                return EcouponCampaignDTO;
                            }
                            else
                            {
                                //date wise search
                                DateTime date = Convert.ToDateTime(search);
                                IQueryable<EcouponCampaign> EcouponCampaignsearch = EcouponCampaign.Where(e => e.CreatedDate >= date && e.CreatedDate < date.AddDays(1) || e.ExpiresOn >= date && e.ExpiresOn < date.AddDays(1) || e.SendOn >= date && e.SendOn < date.AddDays(1)).AsQueryable();//  .OrderByDescending(e => e.CreatedDate).Skip(skip).Take(take);
                                EcouponCampaignsearch = PagingService.Sorting<EcouponCampaign>(EcouponCampaignsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                EcouponCampaignsearch = EcouponCampaignsearch.Skip(skip).Take(take);
                                if (EcouponCampaignsearch != null)
                                {
                                    foreach (var item in EcouponCampaignsearch)
                                    {
                                        EcouponCampaignDTO.Add(Transform.EcouponCampaignToDTO(item));
                                    }
                                }
                                return EcouponCampaignDTO;

                            }

                        }



                    }
                }

                return EcouponCampaignDTOList;
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

        //Get ecoupon campaign paged list with search criteria and client id
        public static PageData<EcouponCampaignDTO> GetEcouponCampaignPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();
            PageData<EcouponCampaignDTO> pageList = new PageData<EcouponCampaignDTO>();

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


            EcouponCampaignDTOList = GetEcouponCampaignListSearchByClientId(ClientId, pagingInfo.Search, pagingInfo);
            IQueryable<EcouponCampaignDTO> EcouponCampaignDTOPagedList = EcouponCampaignDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.EcouponCampaignRepo.GetAll().Where(e => e.Title.ToLower().Contains(pagingInfo.Search.ToLower()) || e.Message.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.ReceipentNumber != null ? (e.ReceipentNumber.Contains(pagingInfo.Search)) : false) || (e.ScheduleTime != null ? (e.ScheduleTime.Contains(pagingInfo.Search)) : false) || (e.IPAddress != null ? (e.IPAddress.Contains(pagingInfo.Search)) : false) || (e.SendOn.ToString() != null ? (Convert.ToDateTime(e.SendOn).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.ExpiresOn.ToString() != null ? (Convert.ToDateTime(e.ExpiresOn).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).Count();
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.EcouponCampaignRepo.GetAll().Where(e => e.CreatedDate >= date && e.CreatedDate < date.AddDays(1) || e.ExpiresOn >= date && e.ExpiresOn < date.AddDays(1) || e.SendOn >= date && e.SendOn < date.AddDays(1)).Count();

                }

            }
            else
            {
                count = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId).Count();
            }

            ////Sorting
            //EcouponCampaignDTOPagedList = PagingService.Sorting<EcouponCampaignDTO>(EcouponCampaignDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (EcouponCampaignDTOPagedList.Count() > 0)
            {
                // var ContacDTOPerPage = PagingService.Paging<EcouponCampaignDTO>(EcouponCampaignDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count; //EcouponCampaignDTOPagedList.Count();

                List<EcouponCampaignDTO> pagedEcouponCampaignDTOList = new List<EcouponCampaignDTO>();
                foreach (var item in EcouponCampaignDTOPagedList)
                {
                    if (item.IsSent == false && item.GroupId != null)
                    {
                        GroupContactDTO GroupDTO = new GroupContactDTO();
                        GroupDTO GroupDTOTemp = new GroupDTO();
                        GroupDTOTemp = GroupService.GetById(Convert.ToInt32(item.GroupId));
                        //GroupDTO = GroupService.GetGroupContactById(GroupDTOTemp.Id);

                        //List<ContactDTO> ContactDTOList = new List<ContactDTO>();
                        //ContactDTOList = GroupContactService.GetGroupIdWiseContacts(GroupDTOTemp.Id);
                        int ContactCount = GroupContactService.GetGroupIdWiseContactsCount(GroupDTOTemp.Id, "");

                        item.GroupContactCount = item.RecipientsCount;
                        if (item.RecipientsCount != ContactCount) // GroupDTO.Contacts.Count()
                        {
                            item.RecipientsCount = ContactCount;// GroupDTO.Contacts.Count();
                            item.GroupContactCount = item.RecipientsCount;
                            EcouponCampaignService.Edit(item);
                        }

                        if (ContactCount == 1)//GroupDTO.Contacts.Count()
                        {
                            item.GroupContactCount = null;
                            List<ContactDTO> ContactDTOList = GroupContactService.GetGroupIdWiseContacts(GroupDTOTemp.Id);
                            item.ReceipentNumber = ContactDTOList[0].MobileNumber;// GroupDTO.Contacts[0].MobileNumber;
                            item.RecipientsCount = 1;
                        }
                    }

                    else if (item.IsSent == false && item.ForAllContact == true)
                    {
                        int CotactCount = uow.ContactRepo.GetAll().Where(e => e.ClientId == item.ClientId).Count();
                        item.GroupContactCount = CotactCount;
                        if (item.RecipientsCount != CotactCount) // GroupDTO.Contacts.Count()
                        {
                            item.GroupContactCount = CotactCount;
                            item.RecipientsCount = CotactCount;
                            //item.RequiredCredits = item.RecipientsCount * item.MessageCount;
                            EcouponCampaignService.Edit(item);
                        }

                        if (CotactCount == 1) //GroupDTO.Contacts.Count()
                        {
                            item.GroupContactCount = null;
                            List<ContactDTO> ContactDTOList = ContactService.GetListByClientId(item.ClientId).ToList();
                            item.ReceipentNumber = ContactDTOList[0].MobileNumber;// GroupDTO.Contacts[0].MobileNumber;
                            item.RecipientsCount = 1;
                        }

                    }

                    pagedEcouponCampaignDTOList.Add(item);
                }
                pageList.Data = pagedEcouponCampaignDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Get ecoupon campaign count list by client id
        public static List<EcouponCampaignDTO> GetEcouponCampaignCountByClientId(int ClientId)
        {

            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();
            List<EcouponCampaignDTO> EcouponCampaignDTOListNew = new List<EcouponCampaignDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.ExpiresOn >= System.DateTime.Now.Date || e.ExpiresOn == null && e.IsSent == true).ToList();
                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            //EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                            //EcouponCampaignDTO.ReedeemedCount = RedeemedCouponsCountByEcouponCampaignId(item.Id);
                            //EcouponCampaignDTO.TotalCount = TotalCouponsCountByEcouponCampaignId(item.Id);
                            EcouponCampaignDTOList.Add(Transform.EcouponCampaignToDTO(item));
                        }
                        if (EcouponCampaignDTOList != null)
                        {
                            foreach (var item in EcouponCampaignDTOList)
                            {
                                item.ReedeemedCount = RedeemedCouponsCountByEcouponCampaignId(item.Id);
                                item.TotalCount = TotalCouponsCountByEcouponCampaignId(item.Id);
                                EcouponCampaignDTOListNew.Add(item);
                            }
                        }


                    }
                }

                return EcouponCampaignDTOList;
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

        //Get ecoupon campaign list with search criteria and partner id
        public static List<EcouponCampaignDTO> GetEcouponCampaignListSearchByPartnerId(int PartnerId, string search)
        {

            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();
            List<ClientDTO> ClientDTOList = new List<ClientDTO>();
            try
            {
                using (var uow = new UnitOfWork())
                {

                    IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId).ToList();
                    if (Client != null)
                    {
                        foreach (var item in Client)
                        {
                            ClientDTOList.Add(Transform.ClientToDTO(item));
                        }
                    }

                    if (ClientDTOList.Count != 0)
                    {
                        foreach (var item in ClientDTOList)
                        {
                            IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == item.Id).OrderByDescending(e => e.CreatedDate).ToList();
                            if (EcouponCampaign != null)
                            {
                                foreach (var itemEcouponCampaign in EcouponCampaign)
                                {
                                    EcouponCampaignDTOList.Add(Transform.EcouponCampaignToDTO(itemEcouponCampaign));
                                }
                            }
                        }


                        if (search != "" && search != null)
                        {
                            List<EcouponCampaignDTO> EcouponCampaignDTO = new List<EcouponCampaignDTO>();

                            bool IsDate = CommonService.IsDate(search);
                            if (IsDate != true)
                            {
                                // string search
                                var EcouponCampaignsearch = EcouponCampaignDTOList.Where(e => e.Title.ToLower().Contains(search.ToLower()) || e.Message.ToLower().Contains(search.ToLower()) || (e.ReceipentNumber != null ? (e.ReceipentNumber.Contains(search)) : false) || (e.ScheduleTime != null ? (e.ScheduleTime.Contains(search)) : false) || (e.IPAddress != null ? (e.IPAddress.Contains(search)) : false) || (e.SendOn.ToString() != null ? (Convert.ToDateTime(e.SendOn).ToString("dd-MMM-yyyy").ToLower().Contains(search.ToLower())) : false) || (e.ExpiresOn.ToString() != null ? (Convert.ToDateTime(e.ExpiresOn).ToString("dd-MMM-yyyy").ToLower().Contains(search.ToLower())) : false)).OrderByDescending(e => e.CreatedDate);

                                if (EcouponCampaignsearch != null)
                                {
                                    foreach (var itemEcouponCampaignsearch in EcouponCampaignsearch)
                                    {

                                        EcouponCampaignDTO.Add(itemEcouponCampaignsearch);
                                    }
                                }
                                return EcouponCampaignDTO;
                            }
                            else
                            {
                                //date wise search
                                DateTime date = Convert.ToDateTime(search);
                                var EcouponCampaignsearch = EcouponCampaignDTOList.Where(e => e.CreatedDate >= date && e.CreatedDate < date.AddDays(1) || e.ExpiresOn >= date && e.ExpiresOn < date.AddDays(1) || e.SendOn >= date && e.SendOn < date.AddDays(1)).OrderByDescending(e => e.CreatedDate);

                                if (EcouponCampaignsearch != null)
                                {
                                    foreach (var itemEcouponCampaignsearch in EcouponCampaignsearch)
                                    {
                                        EcouponCampaignDTO.Add(itemEcouponCampaignsearch);
                                    }
                                }
                                return EcouponCampaignDTO;

                            }

                        }
                    }

                }
                return EcouponCampaignDTOList;
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

        //Get ecoupon campaign paged list with search criteria and partner id
        public static PageData<EcouponCampaignDTO> GetEcouponCampaignPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        {
            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();
            PageData<EcouponCampaignDTO> pageList = new PageData<EcouponCampaignDTO>();

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

            EcouponCampaignDTOList = GetEcouponCampaignListSearchByPartnerId(PartnerId, pagingInfo.Search);
            IQueryable<EcouponCampaignDTO> EcouponCampaignDTOPagedList = EcouponCampaignDTOList.AsQueryable();

            ////Sorting
            //EcouponCampaignDTOPagedList = PagingService.Sorting<EcouponCampaignDTO>(EcouponCampaignDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (EcouponCampaignDTOPagedList.Count() > 0)
            {
                var ContacDTOPerPage = PagingService.Paging<EcouponCampaignDTO>(EcouponCampaignDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = EcouponCampaignDTOPagedList.Count();

                List<EcouponCampaignDTO> pagedEcouponCampaignDTOList = new List<EcouponCampaignDTO>();
                foreach (var item in ContacDTOPerPage)
                {
                    pagedEcouponCampaignDTOList.Add(item);
                }
                pageList.Data = pagedEcouponCampaignDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Get all ecoupon campaign list with client id
        public static List<EcouponCampaignDTO> GetAllOpenEcouponCampaignsByClientId(int ClientId)
        {
            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();
            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.ExpiresOn > System.DateTime.Now.Date).ToList();
                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                            EcouponCampaignDTO = Transform.EcouponCampaignToDTO(item);

                            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
                            CouponDTOList = CouponService.GetCouponListByEcouponCampaignId(item.Id).Where(e => e.IsRedeem == true).ToList();
                            if (CouponDTOList.Count > 0)
                            {
                                EcouponCampaignDTO.ReedeemedCount = CouponDTOList.Count;
                            }
                            else EcouponCampaignDTO.ReedeemedCount = 0;

                            EcouponCampaignDTOList.Add(EcouponCampaignDTO);// (Transform.EcouponCampaignToDTO(item));
                        }
                    }
                }

                return EcouponCampaignDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get day wise ecoupon campaign message count list
        public static List<MessageSentCountDTO> GetDayWiseEcouponCampaignMsssageSentCount(int ClientId)
        {
            int From = 0;
            int To = 30;
            int MaxTo = 360;
            int SMSCount = 0;

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
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsSent == true && e.SendOn > System.DateTime.Now.Date.AddDays(Todays) && e.SendOn < System.DateTime.Now.Date.AddDays(Fromdays));

                    MessageSentCountDTO MessageSentCountDTO = new MessageSentCountDTO();
                    MessageSentCountDTO.Days = From + " To " + To;
                    foreach (var item in EcouponCampaign)
                    {

                        SMSCount = SMSCount + item.RecipientsCount;
                        MessageSentCountDTO.EcouponCampaignCount = SMSCount;
                    }

                    MessageSentCountDTOList.Add(MessageSentCountDTO);

                    From = From + To + 1;
                }



                return MessageSentCountDTOList;

            }
            catch
            {
                throw;
            }

        }

        //Get contacts paged list by ecoupon campaign id
        public static PageData<ContactDTO> GetContactsByEcouponCampaignId(PagingInfo pagingInfo, int EcouponCampaignId, int ClientId)
        {
            UnitOfWork uow = new UnitOfWork();
            EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
            PageData<ContactDTO> pageList = new PageData<ContactDTO>();

            EcouponCampaignDTO = GetById(EcouponCampaignId);

            if (EcouponCampaignDTO != null)
            {
                if (EcouponCampaignDTO.ReceipentNumber != "")
                {
                    string value = EcouponCampaignDTO.ReceipentNumber;
                    string[] array = value.Replace(" ", "").Split(',');

                    List<ContactDTO> ContactDTOList = new List<ContactDTO>();
                    Contact Contact = new Contact();
                    ContactDTO ContactDTO = new ContactDTO();

                    int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                    int take = pagingInfo.ItemsPerPage;

                    var newlist = array.Skip(skip).Take(take).ToArray();

                    if (pagingInfo.Search != "")
                    {
                        ContactDTOList = SearchContacts(pagingInfo.Search, array, EcouponCampaignId, ClientId);
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
                else if (EcouponCampaignDTO.GroupId != null)
                {
                    // GroupContactDTO GroupContactDTO =  GroupService.GetGroupContactById((int)CampaignDTO.GroupId);
                    pagingInfo.GroupId = (int)EcouponCampaignDTO.GroupId;
                    pageList = ContactService.GetGroupIdWisePresentContactsPagedListByClientId(pagingInfo, ClientId);
                }

            }
            else
            {
                pageList.Data = null;
            }

            return pageList;
        }

        //Get contact list with search criteria
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
                try
                {
                    Contacts = uow.ContactRepo.GetAll().Where(e => e.Name.ToLower().Equals(search.ToLower()) || (e.Email != null ? (e.Email.ToLower().Contains(search.ToLower())) : false) && e.ClientId == ClientId).ToList();
                }
                catch (Exception e)
                {

                }
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

        //Get ecoupon campaign count  by name and client id
        public static int GetEcouponCampaignByName(string Title, int ClientId)
        {

            if (Title == null || Title == "") { return 0; }
            try
            {
                int EcouponCampaignId = 0;
                if (Title != null)
                {
                    UnitOfWork uow = new UnitOfWork();
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.Title.ToLower().Contains(Title.ToLower()) && e.ClientId != ClientId);
                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            EcouponCampaignId = item.Id;
                            return EcouponCampaignId;
                        }
                        return EcouponCampaignId;
                    }
                    else return EcouponCampaignId;
                }
                else return EcouponCampaignId;
            }
            catch
            {
                throw;
            }
        }

        //Extend or expire Ecoupon campaign by ecoupon campaign id, date and expire status
        public static bool ExtendOrExpireEcouponCampaign(int EcouponCampaignId, DateTime ExpiredOn, bool IsExpired)
        {
            try
            {
                if (IsExpired == true)
                {
                    try
                    {
                        bool IsStatusChanged = false;
                        IsStatusChanged = CouponService.ExpireAllCouponsByCampaignId(EcouponCampaignId);
                        EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                        EcouponCampaignDTO = GetById(EcouponCampaignId);
                        EcouponCampaignDTO.ExpiresOn = System.DateTime.Now.Date;
                        if (EcouponCampaignDTO.ForAllContact == true)
                        {
                            EcouponCampaignDTO.GroupId = null;
                        }


                        Edit(EcouponCampaignDTO);
                        return IsStatusChanged;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                        EcouponCampaignDTO = GetById(EcouponCampaignId);
                        EcouponCampaignDTO.ExpiresOn = ExpiredOn;
                        Edit(EcouponCampaignDTO);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

            }
            catch
            {
                throw;
            }

        }

        //Get redeemed coupons count by ecoupon campaign id
        public static int RedeemedCouponsCountByEcouponCampaignId(int EcouponCampaignId)
        {
            int ReedeemedCount = 0;
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            using (var uow = new UnitOfWork())
            {
                IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == EcouponCampaignId && e.IsRedeem == true).ToList();
                if (Coupon != null)
                {
                    ReedeemedCount = Coupon.Count();
                }

            }

            return ReedeemedCount;
        }

        //Get total coupons count by ecoupon campaign id
        public static int TotalCouponsCountByEcouponCampaignId(int EcouponCampaignId)
        {
            int TotalCount = 0;
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            using (var uow = new UnitOfWork())
            {
                IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == EcouponCampaignId).ToList();
                if (Coupon != null)
                {
                    TotalCount = Coupon.Count();
                }

            }

            return TotalCount;
        }

        //Get ecoupon campaign details from mobile and code 
        public static EcouponCampaignDTO GetEcouponCampaignDetailsFromMobileAndCode(string Mobile, string Code)
        {
            try
            {
                CouponDTO CouponDTO = new CouponDTO();
                EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.MobileNumber == Mobile && e.Code == Code && e.IsExpired != true && e.IsRedeem != true);
                    if (Coupon != null)
                    {
                        foreach (var item in Coupon)
                        {
                            CouponDTO.Id = item.Id;
                            CouponDTO.EcouponCampaignId = item.EcouponCampaignId;
                        }

                        EcouponCampaignDTO = EcouponCampaignService.GetById(CouponDTO.EcouponCampaignId);

                    }

                }
                return EcouponCampaignDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get todays coupon message sent count by client id
        public static int GetTodaysCouponMsssageSentCount(int ClientId)
        {
            try
            {
                int TodaysSMSCount = 0;
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsSent == true && e.SendOn >= System.DateTime.Now.Date && e.SendOn < System.DateTime.Now.Date.AddDays(1));
                foreach (var item in EcouponCampaign)
                {
                    TodaysSMSCount = TodaysSMSCount + item.RecipientsCount;
                    //IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == item.Id && e.SentDateTime > System.DateTime.Now.AddDays(-1) && e.SentDateTime < System.DateTime.Now.AddDays(1));

                    //if (Coupon != null)
                    //{
                    //    foreach (var itemCampaignLog in Coupon)
                    //    {
                    //        TodaysSMSCount = TodaysSMSCount + 1;
                    //    }
                    //}
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

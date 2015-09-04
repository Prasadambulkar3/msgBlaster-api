using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;
using System.Configuration;

using System.Security.Cryptography;


namespace MsgBlaster.Service
{
    public class CreditRequestService
    {

        #region "CRUD Functionality"

        //Create credit request
        public static CreditRequestDTO Create(CreditRequestDTO CreditRequestDTO)
        {
            try
            {
                CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();

                PartnerDTO PartnerDTO = new PartnerDTO();
                ClientDTO ClientDTO = new ClientDTO();
                UserDTO UserDTO = new UserDTO();

                PartnerDTO = PartnerService.GetById(CreditRequestDTO.PartnerId);
                ClientDTO = ClientService.GetById(CreditRequestDTO.ClientId);
                UserDTO = UserService.GetById(CreditRequestDTO.RequestedBy);

                GlobalSettings.LoggedInClientId = ClientDTO.Id;
                GlobalSettings.LoggedInUserId = UserDTO.Id;
                int PartnerId = PartnerDTO.Id;
                GlobalSettings.LoggedInPartnerId = PartnerId; 


                if (CreditRequestDTO.RequestedCredit <= 0)
                {
                    return null; //0
                }
                var CreditRequest = new CreditRequest();
                using (var uow = new UnitOfWork())
                {
                    CreditRequest = Transform.CreditRequestToDomain(CreditRequestDTO);
                    CreditRequest.Date = System.DateTime.Now;
                    CreditRequest.OldBalance = ClientDTO.SMSCredit;
                    CreditRequest.IsProvided = false;
                    //CreditRequest.ProvidedCredit = CreditRequest.RequestedCredit;
                    CreditRequest.Amount = CreditRequest.RequestedCredit * CreditRequest.RatePerSMS;
                    double tax = (CreditRequest.Amount * Convert.ToDouble(CreditRequest.Tax)) / 100;
                    CreditRequest.TaxAmount = tax;
                    CreditRequest.GrandTotal = CreditRequest.Amount + tax;


                    bool IsOnlinepaymentSuccess = Onlinepayment(CreditRequest.GrandTotal);

                    if (IsOnlinepaymentSuccess == true)
                    {
                        //CreditRequest.IsPaymentSuccessful = true;
                        //CreditRequest.PaymentDate = System.DateTime.Now;
                        CreditRequest.ProvidedDate = System.DateTime.Now;
                        CreditRequest.IsProvided = true;
                        CreditRequest.IsBillGenerated = true;
                        CreditRequest.PaymentMode = PaymentMode.Card;
                        CreditRequest.PaymentDate = System.DateTime.Now;
                        CreditRequest.IsPaid = true;
                        CreditRequest.ProvidedCredit = CreditRequest.RequestedCredit;

                        uow.CreditRequestRepo.Insert(CreditRequest);
                        uow.SaveChanges();

                        ClientDTO.SMSCredit = ClientDTO.SMSCredit + CreditRequest.RequestedCredit;
                        ClientService.Edit(ClientDTO);

                    }
                    else
                    {
                        //CreditRequest.IsPaymentSuccessful = false;
                        CreditRequest.IsPaid = false;
                        uow.CreditRequestRepo.Insert(CreditRequest);
                        uow.SaveChanges();
                    }


                    //Update TotalAppliedCredit                    
                    ClientDTO.TotalAppliedCredit = ClientDTO.TotalAppliedCredit + CreditRequest.RequestedCredit;
                    ClientService.Edit(ClientDTO);


                    //Generate Link to provide mail trhough mail
                    string APILink = ConfigurationManager.AppSettings["APILink"].ToString() + "api/CreditRequest/ProvideCreditByMailLink?CreditRequestId=" + CreditRequest.Id;

                    // Send Email To Partner                     
                    bool IsMailSent = CommonService.SendEmail("SMS Credit Request", "Hello " + PartnerDTO.Name + ", <br/><br/> The New request of " + CreditRequest.RequestedCredit + " credits applied by " + ClientDTO.Company + "<br/><br/> <a href=" + APILink + ">Approve</a>", PartnerDTO.Email, "", false);

                    CreditRequestDTONew = GetById(CreditRequest.Id);
                    if (CreditRequest.PaymentMode == PaymentMode.Card)
                    {
                        CreditRequestDTONew.OnlinePaymentURL = OnlinePaymentLinkWithTemperproofData(UserDTO.Email, CreditRequest.GrandTotal, UserDTO.Mobile, UserDTO.Name, CreditRequest.Id);
                    }
                    return (CreditRequestDTONew); //.Id);                   

                }

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

        //Edit credit request
        public static void Edit(CreditRequestDTO creditRequestDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = creditRequestDTO.ClientId;
                GlobalSettings.LoggedInUserId = creditRequestDTO.RequestedBy;
                int PartnerId = ClientService.GetById(creditRequestDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId; 

                UnitOfWork uow = new UnitOfWork();
                CreditRequest CreditRequest = Transform.CreditRequestToDomain(creditRequestDTO);
                uow.CreditRequestRepo.Update(CreditRequest);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Edit credit request from online payment status
        public static bool EditCreditRequestForOnlinePayment(CreditRequestDTO CreditRequestDTO, bool IsSuccess)
        {
            try
            {
                GlobalSettings.LoggedInClientId = CreditRequestDTO.ClientId;
                GlobalSettings.LoggedInUserId = CreditRequestDTO.RequestedBy;
                int PartnerId = ClientService.GetById(CreditRequestDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId; 

                UnitOfWork uow = new UnitOfWork();
                if (IsSuccess == true)
                {
                    //CreditRequest.IsPaymentSuccessful = true;
                    //CreditRequest.PaymentDate = System.DateTime.Now;
                    CreditRequestDTO.ProvidedDate = System.DateTime.Now;
                    CreditRequestDTO.IsProvided = true;
                    CreditRequestDTO.IsBillGenerated = true;
                    CreditRequestDTO.PaymentMode = PaymentMode.Card.ToString();
                    CreditRequestDTO.PaymentDate = System.DateTime.Now;
                    CreditRequestDTO.IsPaid = true;

                    CreditRequest CreditRequest = Transform.CreditRequestToDomain(CreditRequestDTO);
                    uow.CreditRequestRepo.Update(CreditRequest);
                    uow.SaveChanges();

                    ClientDTO ClientDTO = new ClientDTO();
                    ClientDTO = ClientService.GetById(CreditRequestDTO.ClientId);                  
                    ClientDTO.TotalProvidedCredit = ClientDTO.TotalProvidedCredit + CreditRequestDTO.RequestedCredit;
                    ClientDTO.SMSCredit = ClientDTO.SMSCredit + CreditRequestDTO.RequestedCredit;
                    ClientService.Edit(ClientDTO);

                    return true;


                }
                else
                {
                    CreditRequestDTO.IsPaid = false;
                    CreditRequest CreditRequest = Transform.CreditRequestToDomain(CreditRequestDTO);
                    uow.CreditRequestRepo.Update(CreditRequest);
                    uow.SaveChanges();
                    return false;
                }


            }
            catch
            {
                throw;
            }
        }

        //Delete credit request
        public static void Delete(int id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                uow.CreditRequestRepo.Delete(id);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get credit request by id
        public static CreditRequestDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                CreditRequest CreditRequest = uow.CreditRequestRepo.GetById(Id);
                CreditRequestDTO CreditRequestDTO = Transform.CreditRequestToDTO(CreditRequest);

                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(CreditRequestDTO.ClientId);
                CreditRequestDTO.ClientName = ClientDTO.Company;

                UserDTO UserDTO = new UserDTO();
                UserDTO = UserService.GetById(CreditRequestDTO.RequestedBy);
                CreditRequestDTO.UserName = UserDTO.Name;


                return CreditRequestDTO;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "List Functionality

        //Get all credit request list
        public static List<CreditRequestDTO> GetCreditRequestList()
        {

            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll();
                    if (CreditRequest != null)
                    {
                        foreach (var item in CreditRequest)
                        {
                            CreditRequestDTOList.Add(Transform.CreditRequestToDTO(item));
                        }
                    }
                }

                return CreditRequestDTOList;
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
        public static List<CreditRequestDTO> GetCreditRequestListByClientId(int ClientId)
        {

            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId).OrderByDescending(e => e.Date).ToList(); ;
                    if (CreditRequest != null)
                    {
                        foreach (var item in CreditRequest)
                        {
                            CreditRequestDTOList.Add(Transform.CreditRequestToDTO(item));
                        }
                    }
                }

                return CreditRequestDTOList;
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

        //Get credit request list by  client id
        public static List<CreditRequestDTO> GetCreditRequestsbyClientId(int ClientId, string search)
        {

            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId).OrderByDescending(e => e.Date).ToList();
                if (CreditRequest != null)
                {
                    if (search != "" & search != null)
                    {

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {
                            // string search
                            // var CreditRequestsearch = CreditRequest.Where(e => e.Amount.ToString().Contains(search.ToLower()) || e.ClientId.ToString().Contains(search) || e.GrandTotal.ToString().Contains(search) || e.IsProvided.ToString().Contains(search) || e.OldBalance.ToString().Contains(search) || e.PartnerId.ToString().Contains(search) || e.RatePerSMS.ToString().Contains(search) || e.RequestedCredit.ToString().Contains(search) || e.Tax.ToString().Contains(search));
                            var CreditRequestsearch = CreditRequest.Where(e => e.OldBalance.ToString() == (search) || e.RatePerSMS.ToString() == (search) || e.RequestedCredit.ToString() == (search)).OrderByDescending(e => e.Date);

                            if (CreditRequestsearch != null)
                            {
                                foreach (var item in CreditRequestsearch)
                                {
                                    CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                                    CreditRequestDTO = Transform.CreditRequestToDTO(item);

                                    ClientDTO ClientDTO = new ClientDTO();
                                    ClientDTO = ClientService.GetById(item.ClientId);
                                    CreditRequestDTO.ClientName = ClientDTO.Company;

                                    UserDTO UserDTO = new UserDTO();
                                    UserDTO = UserService.GetById(item.RequestedBy);
                                    CreditRequestDTO.UserName = UserDTO.Name;

                                    CreditRequestDTOList.Add(CreditRequestDTO);// (Transform.CreditRequestToDTO(item));
                                }
                            }
                            return CreditRequestDTOList;
                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(search);
                            var CreditRequestsearch = CreditRequest.Where(e => e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1)).OrderByDescending(e => e.Date);

                            if (CreditRequestsearch != null)
                            {
                                foreach (var item in CreditRequestsearch)
                                {
                                    CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                                    CreditRequestDTO = Transform.CreditRequestToDTO(item);

                                    ClientDTO ClientDTO = new ClientDTO();
                                    ClientDTO = ClientService.GetById(item.ClientId);
                                    CreditRequestDTO.ClientName = ClientDTO.Company;

                                    UserDTO UserDTO = new UserDTO();
                                    UserDTO = UserService.GetById(item.RequestedBy);
                                    CreditRequestDTO.UserName = UserDTO.Name;

                                    CreditRequestDTOList.Add(CreditRequestDTO);// (Transform.CreditRequestToDTO(item));
                                }
                            }
                            return CreditRequestDTOList;
                        }

                    }
                    else
                    {
                        foreach (var item in CreditRequest)
                        {
                            CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                            CreditRequestDTO = Transform.CreditRequestToDTO(item);

                            ClientDTO ClientDTO = new ClientDTO();
                            ClientDTO = ClientService.GetById(item.ClientId);
                            CreditRequestDTO.ClientName = ClientDTO.Company;

                            UserDTO UserDTO = new UserDTO();
                            UserDTO = UserService.GetById(item.RequestedBy);
                            CreditRequestDTO.UserName = UserDTO.Name;

                            CreditRequestDTOList.Add(CreditRequestDTO);// (Transform.CreditRequestToDTO(item));
                        }
                    }
                }

                return CreditRequestDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get credit request list by  partner id
        public static List<CreditRequestDTO> GetCreditRequestsbyPartnerId(int PartnerId, string search)
        {

            List<CreditRequestDTO> CreditRequestDTO = new List<CreditRequestDTO>();

            //List<PartnerDTO> PartnerDTO = new List<PartnerDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId).ToList();
                List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();

                if (Client != null)
                {
                    foreach (var item in Client)
                    {
                        //var CampaignLog = Campaign.Where(e => e.Id == item.Id).ToList(); // uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == item.Id);
                        IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == item.Id).OrderByDescending(e => e.Date).ToList();
                        if (CreditRequest != null)
                        {
                            foreach (var itemCreditRequest in CreditRequest)
                            {
                                CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
                                CreditRequestDTONew = GetById(itemCreditRequest.Id);
                                CreditRequestDTONew.ClientName = item.Company;
                                UserDTO UserDTO = new UserDTO();
                                UserDTO = UserService.GetById(itemCreditRequest.RequestedBy);
                                CreditRequestDTONew.UserName = UserDTO.Name;

                                CreditRequestDTOList.Add(CreditRequestDTONew);// (Transform.CreditRequestToDTO(CreditRequestDTONew));
                            }
                        }
                    }
                }




                if (CreditRequestDTOList != null)
                {
                    if (search != "" && search != null)
                    {

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {
                            // string search
                            // var CreditRequestsearch = CreditRequest.Where(e => e.Amount.ToString().Contains(search.ToLower()) || e.ClientId.ToString().Contains(search) || e.GrandTotal.ToString().Contains(search) || e.IsProvided.ToString().Contains(search) || e.OldBalance.ToString().Contains(search) || e.PartnerId.ToString().Contains(search) || e.RatePerSMS.ToString().Contains(search) || e.RequestedCredit.ToString().Contains(search) || e.Tax.ToString().Contains(search));
                            var CreditRequestsearch = CreditRequestDTOList.Where(e => e.OldBalance.ToString() == (search) || e.RatePerSMS.ToString() == (search) || e.RequestedCredit.ToString() == (search) || e.ClientName.ToString().ToLower().Contains(search.ToLower()) || e.UserName.ToString().ToLower().Contains(search.ToLower())).OrderByDescending(e => e.Date);

                            if (CreditRequestsearch != null)
                            {
                                foreach (var item in CreditRequestsearch)
                                {

                                    CreditRequestDTO.Add(item);
                                }
                            }
                            return CreditRequestDTO;
                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(search);
                            var CreditRequestsearch = CreditRequestDTOList.Where(e => e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1)).OrderByDescending(e => e.Date);

                            if (CreditRequestsearch != null)
                            {
                                foreach (var item in CreditRequestsearch)
                                {
                                    CreditRequestDTO.Add(item);
                                }
                            }
                            return CreditRequestDTO;

                        }

                    }
                    else
                    {

                        //foreach (var item in CreditRequestDTOList)
                        //{
                        //    CreditRequestDTO.Add(item);
                        //} 
                        return CreditRequestDTOList;

                    }
                }

                return CreditRequestDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get credit request list by  partner id with provided flag
        public static List<CreditRequestDTO> GetRequestedCreditRequestsbyPartnerIdWithProvidedflag(int PartnerId, string search, bool IsProvided, PagingInfo pagingInfo)
        {

            List<CreditRequestDTO> CreditRequestDTO = new List<CreditRequestDTO>();

            //List<PartnerDTO> PartnerDTO = new List<PartnerDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;
                //IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId).ToList();
                List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
                //GetAll(skip, take)
                IQueryable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.IsProvided == IsProvided).AsQueryable();//.OrderByDescending(e => e.Date).ToList();
                CreditRequest = PagingService.Sorting<CreditRequest>(CreditRequest, pagingInfo.SortBy, pagingInfo.Reverse);
                CreditRequest = CreditRequest.Skip(skip).Take(take);


                if (CreditRequest != null)
                {
                    foreach (var item in CreditRequest)
                    {
                        //IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll(skip,take).Where(e => e.ClientId == item.Id && e.IsProvided == IsProvided).OrderByDescending(e => e.Date).ToList();

                        CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
                        CreditRequestDTONew = GetById(item.Id);

                        CreditRequestDTOList.Add(CreditRequestDTONew);// (Transform.CreditRequestToDTO(CreditRequestDTONew));


                    }
                }




                if (CreditRequestDTOList != null)
                {
                    if (search != "" && search != null)
                    {

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {
                            //int ClientId =ClientService.GetByClientName(search, PartnerId);
                            String ClientIdString = ClientService.GetClientIdarrayByCompany(search, PartnerId);
                            // string search
                            // var CreditRequestsearch = CreditRequest.Where(e => e.Amount.ToString().Contains(search.ToLower()) || e.ClientId.ToString().Contains(search) || e.GrandTotal.ToString().Contains(search) || e.IsProvided.ToString().Contains(search) || e.OldBalance.ToString().Contains(search) || e.PartnerId.ToString().Contains(search) || e.RatePerSMS.ToString().Contains(search) || e.RequestedCredit.ToString().Contains(search) || e.Tax.ToString().Contains(search));
                            IQueryable<CreditRequest> CreditRequestsearch = uow.CreditRequestRepo.GetAll().Where(e => (e.OldBalance.ToString() == (search) || e.RatePerSMS.ToString() == (search) || e.RequestedCredit.ToString() == (search) || e.ProvidedCredit.ToString() == (search) || (e.Date.ToString() != null ? (Convert.ToDateTime(e.Date).ToString("dd-MMM-yyyy").ToLower().Contains(search.ToLower())) : false) || (e.ProvidedDate.ToString() != null ? (Convert.ToDateTime(e.ProvidedDate).ToString("dd-MMM-yyyy").ToLower().Contains(search.ToLower())) : false) || (ClientIdString != null ? (e.ClientId.ToString().Split(',').Any(ClientId => ClientIdString.Contains(ClientId))) : false)) && e.PartnerId == PartnerId && e.IsProvided == IsProvided).AsQueryable();//.OrderByDescending(e => e.Date).Skip(skip).Take(take); //|| e.UserName.ToString().ToLower().Contains(search.ToLower()) || e.ClientName.ToString().ToLower().Contains(search.ToLower())
                            CreditRequestsearch = PagingService.Sorting<CreditRequest>(CreditRequestsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            CreditRequestsearch = CreditRequestsearch.Skip(skip).Take(take);

                            if (CreditRequestsearch != null)
                            {
                                foreach (var item in CreditRequestsearch)
                                {
                                    CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
                                    CreditRequestDTONew = GetById(item.Id);
                                    CreditRequestDTO.Add(CreditRequestDTONew);
                                }
                            }
                            return CreditRequestDTO;
                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(search);
                            IQueryable<CreditRequest> CreditRequestsearch = uow.CreditRequestRepo.GetAll().Where(e => (e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1)) && e.IsProvided == IsProvided).AsQueryable();//.OrderByDescending(e => e.Date).Skip(skip).Take(take);
                            CreditRequestsearch = PagingService.Sorting<CreditRequest>(CreditRequestsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            CreditRequestsearch = CreditRequestsearch.Skip(skip).Take(take);

                            if (CreditRequestsearch != null)
                            {
                                foreach (var item in CreditRequestsearch)
                                {
                                    if (item.IsProvided == IsProvided)
                                    {
                                        CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
                                        CreditRequestDTONew = GetById(item.Id);
                                        CreditRequestDTO.Add(CreditRequestDTONew);
                                    }
                                }
                            }
                            return CreditRequestDTO;

                        }

                    }
                    else
                    {
                        return CreditRequestDTOList;
                    }
                }

                return CreditRequestDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get credit requestpaged list by  partner id with provided flag
        public static PageData<CreditRequestDTO> GetCreditRequestPagedListbyPartnerIdWithProvidedflag(PagingInfo pagingInfo, int PartnerId, bool IsProvided)
        {
            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            PageData<CreditRequestDTO> pageList = new PageData<CreditRequestDTO>();

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
                pagingInfo.SortBy = "Date";
            }

            CreditRequestDTOList = GetRequestedCreditRequestsbyPartnerIdWithProvidedflag(PartnerId, pagingInfo.Search, IsProvided, pagingInfo);
            IQueryable<CreditRequestDTO> CreditRequestDTOPagedList = CreditRequestDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;
            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    //int ClientId = ClientService.GetByClientName(pagingInfo.Search, PartnerId);
                    String ClientIdString = ClientService.GetClientIdarrayByCompany(pagingInfo.Search, PartnerId);
                    count = 0;
                    count = uow.CreditRequestRepo.GetAll().Where(e => (e.OldBalance.ToString() == (pagingInfo.Search) || e.RatePerSMS.ToString() == (pagingInfo.Search) || e.RequestedCredit.ToString() == (pagingInfo.Search) || e.ProvidedCredit.ToString() == (pagingInfo.Search) || (e.Date.ToString() != null ? (Convert.ToDateTime(e.Date).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.ProvidedDate.ToString() != null ? (Convert.ToDateTime(e.ProvidedDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (ClientIdString != null ? (e.ClientId.ToString().Split(',').Any(ClientId => ClientIdString.Contains(ClientId))) : false)) && e.PartnerId == PartnerId && e.IsProvided == IsProvided).Count(); //e.UserName.ToLower().Contains(search.ToLower()) || e.ClientName.ToLower().Contains(search.ToLower())
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.CreditRequestRepo.GetAll().Where(e => (e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1)) && e.PartnerId == PartnerId && e.IsProvided == IsProvided).Count();

                }

            }
            else
            {
                count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsProvided == IsProvided).Count();
            }

            ////Sorting
            CreditRequestDTOPagedList = PagingService.Sorting<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CreditRequestDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// CreditRequestDTOPagedList.Count();

                pageList.FailureCount = count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsProvided == false).Count();
                pageList.SuccessCount = count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsProvided == true).Count();


                List<CreditRequestDTO> pagedCreditRequestDTOList = new List<CreditRequestDTO>();
                foreach (var item in CreditRequestDTOPagedList)
                {
                    pagedCreditRequestDTOList.Add(item);
                }
                pageList.Data = pagedCreditRequestDTOList;
            }
            else
            {
                pageList.FailureCount = count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsProvided == false).Count();
                pageList.SuccessCount = count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsProvided == true).Count();
                pageList.Data = null;
            }



            return pageList;
        }

        //Get credit request list by  client id with provided flag
        public static List<CreditRequestDTO> GetCreditRequestsbyClientIdWithProvidedflag(int ClientId, string search, bool IsProvided, PagingInfo pagingInfo)
        {

            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;



                IQueryable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsProvided == IsProvided).AsQueryable();//.OrderByDescending(e => e.Date).ToList();
                CreditRequest = PagingService.Sorting<CreditRequest>(CreditRequest, pagingInfo.SortBy, pagingInfo.Reverse);
                CreditRequest = CreditRequest.Skip(skip).Take(take);



                if (CreditRequest != null)
                {
                    foreach (var item in CreditRequest)
                    {
                        CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                        CreditRequestDTO = GetById(item.Id);

                        //ClientDTO ClientDTO = new ClientDTO();
                        //ClientDTO = ClientService.GetById(item.ClientId);
                        //CreditRequestDTO.ClientName = ClientDTO.Company;

                        //UserDTO UserDTO = new UserDTO();
                        //UserDTO = UserService.GetById(item.RequestedBy);
                        //CreditRequestDTO.UserName = UserDTO.Name;


                        CreditRequestDTOList.Add(CreditRequestDTO);


                    }
                }

                if (CreditRequestDTOList.Count > 0)
                {
                    if (search != "" & search != null)
                    {

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {
                            // string search
                            // var CreditRequestsearch = CreditRequest.Where(e => e.Amount.ToString().Contains(search.ToLower()) || e.ClientId.ToString().Contains(search) || e.GrandTotal.ToString().Contains(search) || e.IsProvided.ToString().Contains(search) || e.OldBalance.ToString().Contains(search) || e.PartnerId.ToString().Contains(search) || e.RatePerSMS.ToString().Contains(search) || e.RequestedCredit.ToString().Contains(search) || e.Tax.ToString().Contains(search));
                            IQueryable<CreditRequest> CreditRequestsearch = uow.CreditRequestRepo.GetAll().Where(e => (e.OldBalance.ToString() == (search) || e.RatePerSMS.ToString() == (search) || e.RequestedCredit.ToString() == (search) || (e.Date.ToString() != null ? (Convert.ToDateTime(e.Date).ToString("dd-MMM-yyyy").ToLower().Contains(search.ToLower())) : false) || (e.ProvidedDate.ToString() != null ? (Convert.ToDateTime(e.ProvidedDate).ToString("dd-MMM-yyyy").ToLower().Contains(search.ToLower())) : false) || e.ProvidedCredit.ToString() == (search)) && e.IsProvided == IsProvided && e.ClientId == ClientId).AsQueryable();//.OrderByDescending(e => e.Date).Skip(skip).Take(take); //|| e.UserName.ToLower().Contains(search.ToLower()) || e.ClientName.ToLower().Contains(search.ToLower())
                            CreditRequestsearch = PagingService.Sorting<CreditRequest>(CreditRequestsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            CreditRequestsearch = CreditRequestsearch.Skip(skip).Take(take);

                            if (CreditRequestsearch != null)
                            {
                                List<CreditRequestDTO> CreditRequestDTOListNew = new List<CreditRequestDTO>();
                                foreach (var item in CreditRequestsearch)
                                {
                                    CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                                    CreditRequestDTO = GetById(item.Id);
                                    CreditRequestDTOListNew.Add(CreditRequestDTO);

                                }

                                return CreditRequestDTOListNew;
                            }

                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(search);
                            IQueryable<CreditRequest> CreditRequestsearch = uow.CreditRequestRepo.GetAll().Where(e => (e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1)) && e.IsProvided == IsProvided && e.ClientId == ClientId).OrderByDescending(e => e.Date).AsQueryable();//.Skip(skip).Take(take);
                            CreditRequestsearch = PagingService.Sorting<CreditRequest>(CreditRequestsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            CreditRequestsearch = CreditRequestsearch.Skip(skip).Take(take);
                            if (CreditRequestsearch != null)
                            {
                                List<CreditRequestDTO> CreditRequestDTOListNew = new List<CreditRequestDTO>();
                                foreach (var item in CreditRequestsearch)
                                {
                                    if (item.IsProvided == IsProvided)
                                    {
                                        CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                                        CreditRequestDTO = GetById(item.Id);
                                        CreditRequestDTOListNew.Add(CreditRequestDTO);
                                    }
                                }

                                return CreditRequestDTOListNew;
                            }

                        }

                    }
                    else
                    {

                        return CreditRequestDTOList;

                    }
                }
                return CreditRequestDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get credit request paged list by  client id with provided flag
        public static PageData<CreditRequestDTO> GetCreditRequestPagedListbyClientIdWithProvidedflag(PagingInfo pagingInfo, int ClientId, bool IsProvided)
        {
            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            PageData<CreditRequestDTO> pageList = new PageData<CreditRequestDTO>();

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
                pagingInfo.SortBy = "Date";
            }

            CreditRequestDTOList = GetCreditRequestsbyClientIdWithProvidedflag(ClientId, pagingInfo.Search, IsProvided, pagingInfo);
            IQueryable<CreditRequestDTO> CreditRequestDTOPagedList = CreditRequestDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;
            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.CreditRequestRepo.GetAll().Where(e => (e.OldBalance.ToString() == (pagingInfo.Search) || e.RatePerSMS.ToString() == (pagingInfo.Search) || e.RequestedCredit.ToString() == (pagingInfo.Search) || (e.Date.ToString() != null ? (Convert.ToDateTime(e.Date).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.ProvidedDate.ToString() != null ? (Convert.ToDateTime(e.ProvidedDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.ProvidedCredit.ToString() == (pagingInfo.Search)) && e.IsProvided == IsProvided && e.ClientId == ClientId).Count(); //e.UserName.ToLower().Contains(search.ToLower()) || e.ClientName.ToLower().Contains(search.ToLower())
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.CreditRequestRepo.GetAll().Where(e => (e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1)) && e.IsProvided == IsProvided && e.ClientId == ClientId).Count();

                }

            }
            else
            {
                count = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsProvided == IsProvided).Count();
            }

            ////Sorting
            CreditRequestDTOPagedList = PagingService.Sorting<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CreditRequestDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// CreditRequestDTOPagedList.Count();

                List<CreditRequestDTO> pagedCreditRequestDTOList = new List<CreditRequestDTO>();
                foreach (var item in CreditRequestDTOPagedList)
                {
                    pagedCreditRequestDTOList.Add(item);
                }
                pageList.Data = pagedCreditRequestDTOList;
            }
            else
            {
                pageList.Data = null;
            }

            pageList.SuccessCount = GetRequestedCreditRequestCount(ClientId);
            pageList.FailureCount = GetProvidedCreditRequestCount(ClientId);


            return pageList;
        }

        //Get credit request refilled list by  client id
        public static List<CreditRequestDTO> GetCreditRequestRefilledListByClientId(int ClientId, int Top)
        {

            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            int count = 0;
            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsProvided == true).OrderByDescending(e => e.Id).ToList(); ;
                    if (CreditRequest != null)
                    {
                        foreach (var item in CreditRequest)
                        {
                            count = count + 1;
                            item.GrandTotal = Convert.ToDouble( String.Format("{0:0.00}", item.GrandTotal));
                            CreditRequestDTOList.Add(Transform.CreditRequestToDTO(item));
                            if (count == Top)
                            {
                                return CreditRequestDTOList;
                            }
                        }
                    }
                }

                return CreditRequestDTOList;
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

        //Get credit request list by  client id
        public static List<CreditRequestDTO> GetCreditRequestsbyClientId(int ClientId, PagingInfo pagingInfo)
        {

            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;



                IQueryable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll(skip, take).Where(e => e.ClientId == ClientId).AsQueryable();//.OrderByDescending(e => e.Date).ToList();
                CreditRequest = PagingService.Sorting<CreditRequest>(CreditRequest, pagingInfo.SortBy, pagingInfo.Reverse);
                CreditRequest = CreditRequest.Skip(skip).Take(take);



                if (CreditRequest != null)
                {
                    foreach (var item in CreditRequest)
                    {
                        CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                        CreditRequestDTO = GetById(item.Id);

                        //ClientDTO ClientDTO = new ClientDTO();
                        //ClientDTO = ClientService.GetById(item.ClientId);
                        //CreditRequestDTO.ClientName = ClientDTO.Company;

                        //UserDTO UserDTO = new UserDTO();
                        //UserDTO = UserService.GetById(item.RequestedBy);
                        //CreditRequestDTO.UserName = UserDTO.Name;


                        CreditRequestDTOList.Add(CreditRequestDTO);


                    }
                }

                if (CreditRequestDTOList.Count > 0)
                {
                    if (pagingInfo.Search != "" & pagingInfo.Search != null)
                    {

                        bool IsDate = CommonService.IsDate(pagingInfo.Search);
                        if (IsDate != true)
                        {
                            // string search
                            // var CreditRequestsearch = CreditRequest.Where(e => e.Amount.ToString().Contains(search.ToLower()) || e.ClientId.ToString().Contains(search) || e.GrandTotal.ToString().Contains(search) || e.IsProvided.ToString().Contains(search) || e.OldBalance.ToString().Contains(search) || e.PartnerId.ToString().Contains(search) || e.RatePerSMS.ToString().Contains(search) || e.RequestedCredit.ToString().Contains(search) || e.Tax.ToString().Contains(search));
                            IQueryable<CreditRequest> CreditRequestsearch = uow.CreditRequestRepo.GetAll().Where(e => (e.OldBalance.ToString() == (pagingInfo.Search) || e.RatePerSMS.ToString() == (pagingInfo.Search) || e.RequestedCredit.ToString() == (pagingInfo.Search) || (e.Date.ToString() != null ? (Convert.ToDateTime(e.Date).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.ProvidedDate.ToString() != null ? (Convert.ToDateTime(e.ProvidedDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.ProvidedCredit.ToString() == (pagingInfo.Search)) && e.ClientId == ClientId).AsQueryable();//.OrderByDescending(e => e.Date).Skip(skip).Take(take); //|| e.UserName.ToLower().Contains(search.ToLower()) || e.ClientName.ToLower().Contains(search.ToLower())
                            CreditRequestsearch = PagingService.Sorting<CreditRequest>(CreditRequestsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            CreditRequestsearch = CreditRequestsearch.Skip(skip).Take(take);

                            if (CreditRequestsearch != null)
                            {
                                List<CreditRequestDTO> CreditRequestDTOListNew = new List<CreditRequestDTO>();
                                foreach (var item in CreditRequestsearch)
                                {
                                    CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                                    CreditRequestDTO = GetById(item.Id);
                                    CreditRequestDTOListNew.Add(CreditRequestDTO);

                                }

                                return CreditRequestDTOListNew;
                            }

                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(pagingInfo.Search);
                            IQueryable<CreditRequest> CreditRequestsearch = uow.CreditRequestRepo.GetAll().Where(e => (e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1)) && e.ClientId == ClientId).OrderByDescending(e => e.Date).AsQueryable();//.Skip(skip).Take(take);
                            CreditRequestsearch = PagingService.Sorting<CreditRequest>(CreditRequestsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            CreditRequestsearch = CreditRequestsearch.Skip(skip).Take(take);
                            if (CreditRequestsearch != null)
                            {
                                List<CreditRequestDTO> CreditRequestDTOListNew = new List<CreditRequestDTO>();
                                foreach (var item in CreditRequestsearch)
                                {
                                    //if (item.IsProvided == IsProvided)
                                    //{
                                    CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                                    CreditRequestDTO = GetById(item.Id);
                                    CreditRequestDTOListNew.Add(CreditRequestDTO);
                                    //}
                                }

                                return CreditRequestDTOListNew;
                            }

                        }

                    }
                    else
                    {

                        return CreditRequestDTOList;

                    }
                }
                return CreditRequestDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get credit request paged list by  client id
        public static PageData<CreditRequestDTO> GetCreditRequestPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            PageData<CreditRequestDTO> pageList = new PageData<CreditRequestDTO>();

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
                pagingInfo.SortBy = "Date";
            }

            CreditRequestDTOList = GetCreditRequestsbyClientId(ClientId, pagingInfo);
            IQueryable<CreditRequestDTO> CreditRequestDTOPagedList = CreditRequestDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;
            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.CreditRequestRepo.GetAll().Where(e => (e.OldBalance.ToString() == (pagingInfo.Search) || e.RatePerSMS.ToString() == (pagingInfo.Search) || e.RequestedCredit.ToString() == (pagingInfo.Search) || (e.Date.ToString() != null ? (Convert.ToDateTime(e.Date).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.ProvidedDate.ToString() != null ? (Convert.ToDateTime(e.ProvidedDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.ProvidedCredit.ToString() == (pagingInfo.Search)) && e.ClientId == ClientId).Count(); //e.UserName.ToLower().Contains(search.ToLower()) || e.ClientName.ToLower().Contains(search.ToLower())
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.CreditRequestRepo.GetAll().Where(e => (e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1)) && e.ClientId == ClientId).Count();

                }

            }
            else
            {
                count = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId).Count();
            }

            ////Sorting
            CreditRequestDTOPagedList = PagingService.Sorting<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CreditRequestDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// CreditRequestDTOPagedList.Count();

                List<CreditRequestDTO> pagedCreditRequestDTOList = new List<CreditRequestDTO>();
                foreach (var item in CreditRequestDTOPagedList)
                {
                    pagedCreditRequestDTOList.Add(item);
                }
                pageList.Data = pagedCreditRequestDTOList;
            }
            else
            {
                pageList.Data = null;
            }

            pageList.SuccessCount = GetRequestedCreditRequestCount(ClientId);
            pageList.FailureCount = GetProvidedCreditRequestCount(ClientId);


            return pageList;
        }

        //Get requested credit request list by  partner id
        public static List<CreditRequestDTO> GetRequestedCreditRequestsbyPartnerId(int PartnerId, PagingInfo pagingInfo)
        {

            List<CreditRequestDTO> CreditRequestDTO = new List<CreditRequestDTO>();

            //List<PartnerDTO> PartnerDTO = new List<PartnerDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;
                //IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId).ToList();
                List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();

                IQueryable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll(skip, take).AsQueryable();//.OrderByDescending(e => e.Date).ToList();
                CreditRequest = PagingService.Sorting<CreditRequest>(CreditRequest, pagingInfo.SortBy, pagingInfo.Reverse);
                CreditRequest = CreditRequest.Skip(skip).Take(take);


                if (CreditRequest != null)
                {
                    foreach (var item in CreditRequest)
                    {
                        //IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll(skip,take).Where(e => e.ClientId == item.Id && e.IsProvided == IsProvided).OrderByDescending(e => e.Date).ToList();

                        CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
                        CreditRequestDTONew = GetById(item.Id);

                        CreditRequestDTOList.Add(CreditRequestDTONew);// (Transform.CreditRequestToDTO(CreditRequestDTONew));


                    }
                }




                if (CreditRequestDTOList != null)
                {
                    if (pagingInfo.Search != "" && pagingInfo.Search != null)
                    {

                        bool IsDate = CommonService.IsDate(pagingInfo.Search);
                        if (IsDate != true)
                        {
                            //int ClientId =ClientService.GetByClientName(search, PartnerId);
                            String ClientIdString = ClientService.GetClientIdarrayByCompany(pagingInfo.Search, PartnerId);
                            // string search
                            // var CreditRequestsearch = CreditRequest.Where(e => e.Amount.ToString().Contains(search.ToLower()) || e.ClientId.ToString().Contains(search) || e.GrandTotal.ToString().Contains(search) || e.IsProvided.ToString().Contains(search) || e.OldBalance.ToString().Contains(search) || e.PartnerId.ToString().Contains(search) || e.RatePerSMS.ToString().Contains(search) || e.RequestedCredit.ToString().Contains(search) || e.Tax.ToString().Contains(search));
                            IQueryable<CreditRequest> CreditRequestsearch = uow.CreditRequestRepo.GetAll().Where(e => (e.OldBalance.ToString() == (pagingInfo.Search) || e.RatePerSMS.ToString() == (pagingInfo.Search) || e.RequestedCredit.ToString() == (pagingInfo.Search) || e.ProvidedCredit.ToString() == (pagingInfo.Search) || (e.Date.ToString() != null ? (Convert.ToDateTime(e.Date).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.ProvidedDate.ToString() != null ? (Convert.ToDateTime(e.ProvidedDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (ClientIdString != null ? (e.ClientId.ToString().Split(',').Any(ClientId => ClientIdString.Contains(ClientId))) : false)) && e.PartnerId == PartnerId).AsQueryable();//.OrderByDescending(e => e.Date).Skip(skip).Take(take); //|| e.UserName.ToString().ToLower().Contains(search.ToLower()) || e.ClientName.ToString().ToLower().Contains(search.ToLower())
                            CreditRequestsearch = PagingService.Sorting<CreditRequest>(CreditRequestsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            CreditRequestsearch = CreditRequestsearch.Skip(skip).Take(take);

                            if (CreditRequestsearch != null)
                            {
                                foreach (var item in CreditRequestsearch)
                                {
                                    CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
                                    CreditRequestDTONew = GetById(item.Id);
                                    CreditRequestDTO.Add(CreditRequestDTONew);
                                }
                            }
                            return CreditRequestDTO;
                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(pagingInfo.Search);
                            IQueryable<CreditRequest> CreditRequestsearch = uow.CreditRequestRepo.GetAll().Where(e => (e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1))).AsQueryable();//.OrderByDescending(e => e.Date).Skip(skip).Take(take);
                            CreditRequestsearch = PagingService.Sorting<CreditRequest>(CreditRequestsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            CreditRequestsearch = CreditRequestsearch.Skip(skip).Take(take);

                            if (CreditRequestsearch != null)
                            {
                                foreach (var item in CreditRequestsearch)
                                {
                                    //if (item.IsProvided == IsProvided)
                                    //{
                                    CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
                                    CreditRequestDTONew = GetById(item.Id);
                                    CreditRequestDTO.Add(CreditRequestDTONew);
                                    //}
                                }
                            }
                            return CreditRequestDTO;

                        }

                    }
                    else
                    {
                        return CreditRequestDTOList;
                    }
                }

                return CreditRequestDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get requested credit request list by  partner id
        public static PageData<CreditRequestDTO> GetCreditRequestPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        {
            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            PageData<CreditRequestDTO> pageList = new PageData<CreditRequestDTO>();

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
                pagingInfo.SortBy = "Date";
            }

            CreditRequestDTOList = GetRequestedCreditRequestsbyPartnerId(PartnerId, pagingInfo);
            IQueryable<CreditRequestDTO> CreditRequestDTOPagedList = CreditRequestDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;
            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    //int ClientId = ClientService.GetByClientName(pagingInfo.Search, PartnerId);
                    String ClientIdString = ClientService.GetClientIdarrayByCompany(pagingInfo.Search, PartnerId);
                    count = 0;
                    count = uow.CreditRequestRepo.GetAll().Where(e => (e.OldBalance.ToString() == (pagingInfo.Search) || e.RatePerSMS.ToString() == (pagingInfo.Search) || e.RequestedCredit.ToString() == (pagingInfo.Search) || e.ProvidedCredit.ToString() == (pagingInfo.Search) || (e.Date.ToString() != null ? (Convert.ToDateTime(e.Date).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.ProvidedDate.ToString() != null ? (Convert.ToDateTime(e.ProvidedDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (ClientIdString != null ? (e.ClientId.ToString().Split(',').Any(ClientId => ClientIdString.Contains(ClientId))) : false)) && e.PartnerId == PartnerId).Count(); //e.UserName.ToLower().Contains(search.ToLower()) || e.ClientName.ToLower().Contains(search.ToLower())
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.CreditRequestRepo.GetAll().Where(e => (e.Date >= date && e.Date < date.AddDays(1) || e.ProvidedDate >= date && e.ProvidedDate < date.AddDays(1)) && e.PartnerId == PartnerId).Count();

                }

            }
            else
            {
                count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId).Count();
            }

            ////Sorting
            CreditRequestDTOPagedList = PagingService.Sorting<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CreditRequestDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// CreditRequestDTOPagedList.Count();

                pageList.FailureCount = count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsProvided == false).Count();
                pageList.SuccessCount = count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsProvided == true).Count();


                List<CreditRequestDTO> pagedCreditRequestDTOList = new List<CreditRequestDTO>();
                foreach (var item in CreditRequestDTOPagedList)
                {
                    pagedCreditRequestDTOList.Add(item);
                }
                pageList.Data = pagedCreditRequestDTOList;
            }
            else
            {
                pageList.FailureCount = count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsProvided == false).Count();
                pageList.SuccessCount = count = uow.CreditRequestRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsProvided == true).Count();
                pageList.Data = null;
            }



            return pageList;
        }

        #endregion

        #region "Create Temperproof Link Functionality"

        //Online payment link with temper proof data
        public static string OnlinePaymentLinkWithTemperproofData(string email, double amount, string phone, string name, int CreditRequestId)
        {

            string salt = ConfigurationManager.AppSettings["InstamojoSaltKey"].ToString();   //"844cf39659b948cf95385947e2523ce7"; // Salt
            //amount =amount.ToString(""
            /*
            The data dictionary should contain all the read-only fields.
            If you want to include custom field as well then add them to
            the dictionary, but don't forget to add "data_" in front of
            the custom field names. 
            */

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"data_amount",String.Format("{0:0.00}", amount) },
                {"data_email", email},
                {"data_phone", phone},
                {"data_name", name},
                //{"data_Field_52722", CreditRequestId.ToString()} 
                {ConfigurationManager.AppSettings["InstamojoCustomField"].ToString() ,CreditRequestId.ToString() }
            };

            string msg = MsgCreator(data);
            string signature = ShaHash(msg, salt);

            string URLOnlinepayment = ConfigurationManager.AppSettings["InstamojoPaymentLink"].ToString();   //   "https://test.instamojo.com/rohitkale/msgblaster-249d6/?data_readonly=data_name&data_readonly=data_email&data_readonly=data_phone&data_readonly=data_amount&data_readonly=data_Field_52722&data_sign=[sign]&data_email=[email]&data_amount=[amount]&data_name=[name]&data_phone=[Phone]&data_Field_52722=[CreditRequestid]";

            URLOnlinepayment = URLOnlinepayment.Replace("%26", "&");
            URLOnlinepayment = URLOnlinepayment.Replace("[sign]", signature);
            URLOnlinepayment = URLOnlinepayment.Replace("[email]", email);
            URLOnlinepayment = URLOnlinepayment.Replace("[amount]", String.Format("{0:0.00}", amount));
            URLOnlinepayment = URLOnlinepayment.Replace("[name]", name);
            URLOnlinepayment = URLOnlinepayment.Replace("[Phone]", phone);
            URLOnlinepayment = URLOnlinepayment.Replace("[CreditRequestid]", CreditRequestId.ToString());



            return URLOnlinepayment;



        }

        public static void PayOnlineEdit(CreditRequestDTO creditRequestDTO)
        {
            try
            {
                //bool IsPaymentSuccessful = false;                
                //IsPaymentSuccessful = GetById(creditRequestDTO.Id).IsPaymentSuccessful;
                //UnitOfWork uow = new UnitOfWork();

                //if (creditRequestDTO.IsPaymentSuccessful != true)
                //{
                //    bool IsOnlinepaymentSuccess = Onlinepayment(creditRequestDTO.PayableAmount);
                //    if (IsOnlinepaymentSuccess == true)
                //    {
                //        creditRequestDTO.IsPaymentSuccessful = true;
                //        creditRequestDTO.PaymentDate = System.DateTime.Now;
                //        creditRequestDTO.IsProvided = true;
                //        creditRequestDTO.IsBillGenerated = true;

                //        ClientDTO ClientDTO = new ClientDTO();
                //        ClientDTO = ClientService.GetById(creditRequestDTO.ClientId);
                //        ClientDTO.SMSCredit = ClientDTO.SMSCredit + creditRequestDTO.RequestedCredit;
                //        ClientService.Edit(ClientDTO);
                //    }
                //    else
                //    {
                //        creditRequestDTO.IsPaymentSuccessful = false;
                //    }
                //}
                //CreditRequest CreditRequest = Transform.CreditRequestToDomain(creditRequestDTO);
                //uow.CreditRequestRepo.Update(CreditRequest);
                //uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public static string MsgCreator(Dictionary<string, string> data)
        {
            var ordered_view = data.OrderBy(key => key.Key.ToLower());
            string message = "";

            foreach (var item in ordered_view)
            {
                message += item.Value + "|";
            }
            return message.Substring(0, message.Length - 1);

        }

        public static string ShaHash(string msg, string salt)
        {
            using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(salt)))
            {
                return ByteToString(hmac.ComputeHash(Encoding.ASCII.GetBytes(msg)));
            }
        }

        public static string ByteToString(IEnumerable<byte> msg)
        {
            return string.Concat(msg.Select(b => b.ToString("x2")));
        }

        #endregion

        #region "Other Functionality"

        //get credit request count by client id
        public static int GetCreditRequestCountByClientId(int ClientId)
        {

            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            try
            {
                int TotalCount = 0;

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId && e.ProvidedDate != null).ToList();
                    // CreditRequest.GroupBy(x => x.RequestedCredit).Select(g => new { Sum = g.Sum(x => x.RequestedCredit) });                    
                    if (CreditRequest != null)
                    {
                        foreach (var item in CreditRequest)
                        {
                            CreditRequestDTOList.Add(Transform.CreditRequestToDTO(item));
                        }
                    }
                    else TotalCount = 0;
                }

                //return CreditRequestDTOList;
                TotalCount = CreditRequestDTOList.Where(p => p.ProvidedCredit > 0).Sum(p => p.ProvidedCredit);

                return TotalCount;

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

        // Provide credits to client
        public static CreditRequestDTO ProvideCredit(CreditRequestDTO CreditRequestDTO)
        {
            CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
            try
            {
                //if (CreditRequestDTO.Tax == null)
                //{
                //    CreditRequestDTO.Tax = 0;
                //}

                CreditRequestDTONew = CreditRequestDTO;
                CreditRequestDTONew.IsProvided = true;
                CreditRequestDTONew.ProvidedDate = System.DateTime.Now;
                CreditRequestDTONew.RatePerSMS = CreditRequestDTO.RatePerSMS;
                CreditRequestDTONew.Tax = CreditRequestDTO.Tax;
                CreditRequestDTONew.ProvidedCredit = CreditRequestDTO.ProvidedCredit;
                CreditRequestDTONew.Amount = CreditRequestDTO.ProvidedCredit * CreditRequestDTO.RatePerSMS;
                double tax = (CreditRequestDTONew.Amount * Convert.ToDouble(CreditRequestDTONew.Tax)) / 100;
                CreditRequestDTONew.TaxAmount = tax;
                CreditRequestDTONew.GrandTotal = CreditRequestDTONew.Amount + tax;

                //if (CreditRequestDTO.GrandTotal != CreditRequestDTONew.GrandTotal )
                //{
                //    CreditRequestDTONew.GrandTotal = CreditRequestDTO.GrandTotal;
                //}

                UnitOfWork uow = new UnitOfWork();
                CreditRequest CreditRequest = Transform.CreditRequestToDomain(CreditRequestDTONew);
                uow.CreditRequestRepo.Update(CreditRequest);
                uow.SaveChanges();


                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(CreditRequest.ClientId);
                ClientDTO.SMSCredit = ClientDTO.SMSCredit + CreditRequest.ProvidedCredit; // CreditRequest.RequestedCredit;
                ClientDTO.TotalProvidedCredit = ClientDTO.TotalProvidedCredit + CreditRequest.ProvidedCredit;
                Client Client = Transform.ClientToDomain(ClientDTO);

                GlobalSettings.LoggedInClientId = CreditRequestDTO.ClientId;
                GlobalSettings.LoggedInUserId = CreditRequestDTO.RequestedBy;
                int PartnerId = ClientService.GetById(CreditRequestDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId; 

                uow.ClientRepo.Update(Client);
                uow.SaveChanges();

                UserDTO UserDTO = new UserDTO();
                UserDTO = UserService.GetById(CreditRequest.RequestedBy);

                DateTime date = (DateTime)CreditRequest.ProvidedDate;
                string DateFormat = date.ToString("dd-MMM-yyyy HH:mm");
                CommonService.SendEmail("msgBlaster SMS credit provided", "<html><body><p>Hello " + Client.Company + ",</p> <br/><p>Your mesgblaster credit details are as follows.<p/> <table><tr><td> Date</td> <td> " + DateFormat + " </td> </tr>  <tr><td> Provided Credits</td> <td> " + CreditRequest.ProvidedCredit + " </td> </tr> <td> Total Balance</td> <td> " + Client.SMSCredit + " </td> </tr>  </table>  </body></html>", UserDTO.Email, "", false);  //<tr><td> Provided Credits</td> <td> " + CreditRequest.ProvidedCredit + " </td> </tr> <tr><td> Rate Per SMS</td> <td> " + CreditRequest.RatePerSMS + " </td> </tr> <tr><td> Amount </td> <td> " + CreditRequest.Amount + " </td> </tr> <tr><td> Tax</td> <td> " + CreditRequest.Tax + " </td> </tr> <tr><td> <strong>Grand Total</strong></td> <td> <strong>" + CreditRequest.GrandTotal + "</strong> </td> </tr>

                CreditRequestDTONew = Transform.CreditRequestToDTO(CreditRequest);
                return CreditRequestDTONew;

            }
            catch (Exception)
            {
                // return CreditRequestDTO;  
                throw;
            }

        }

        //Send bill to client by credit request id
        public static bool SendBillToClient(int CreditRequestId)
        {
            bool IsSend = false;

            CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
            CreditRequestDTO = GetById(CreditRequestId);
            if (CreditRequestDTO.ProvidedDate != null)
            {
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(CreditRequestDTO.ClientId);

                UserDTO UserDTO = new UserDTO();
                UserDTO = UserService.GetById(CreditRequestDTO.RequestedBy);

                DateTime date = (DateTime)CreditRequestDTO.ProvidedDate;
                string DateFormat = date.ToString("dd-MMM-yyyy HH:mm");

                IsSend = CommonService.SendEmail("msgBlaster SMS credit provided bill", "<html><body><p>Hello " + ClientDTO.Company + ",</p> <br/><p>Your mesgblaster Bill details are as follows.<p/> <table><tr><td> Bill Date</td> <td> " + DateFormat + " </td> </tr> <tr><td> Requested Credits</td> <td> " + CreditRequestDTO.RequestedCredit + " </td> </tr> <tr><td> Provided Credits</td> <td> " + CreditRequestDTO.ProvidedCredit + " </td> </tr> <tr><td> Rate Per SMS</td> <td> " + String.Format("{0:0.00}", CreditRequestDTO.RatePerSMS) + " </td> </tr> <tr><td> Amount </td> <td> " + String.Format("{0:0.00}", CreditRequestDTO.Amount) + " </td> </tr><tr><td> Tax</td> <td> " + String.Format("{0:0.00}", CreditRequestDTO.Tax) + "% </td> </tr> <tr><td> <strong>Grand Total</strong></td> <td> <strong>" + String.Format("{0:0.00}", CreditRequestDTO.GrandTotal) + "</strong> </td> </tr>  </table>  </body></html>", UserDTO.Email, "", false);
                IsSend = true;
            }
            else { IsSend = false; }
            return IsSend;

        }

        //Generate bill by credit request details
        public static CreditRequestDTO GenerateBill(CreditRequestDTO CreditRequestDTO)
        {
            CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
            try
            {
                //if (CreditRequestDTO.Tax == null)
                //{
                //    CreditRequestDTO.Tax = 0;
                //}

                if (CreditRequestDTO.IsProvided == true)
                {
                    CreditRequestDTONew = CreditRequestDTO;
                    CreditRequestDTONew.IsBillGenerated = true;
                    CreditRequestDTONew.IsProvided = true;
                    //CreditRequestDTONew.ProvidedDate = System.DateTime.Now;
                    CreditRequestDTONew.RatePerSMS = CreditRequestDTO.RatePerSMS;
                    CreditRequestDTONew.Tax = CreditRequestDTO.Tax;
                    CreditRequestDTONew.ProvidedCredit = CreditRequestDTO.ProvidedCredit;
                    CreditRequestDTONew.Amount = CreditRequestDTO.ProvidedCredit * CreditRequestDTO.RatePerSMS;
                    double tax = (CreditRequestDTONew.Amount * Convert.ToDouble(CreditRequestDTONew.Tax)) / 100;
                    CreditRequestDTONew.TaxAmount = tax;
                    CreditRequestDTONew.GrandTotal = CreditRequestDTONew.Amount + tax;

                    //if (CreditRequestDTO.GrandTotal != CreditRequestDTONew.GrandTotal)
                    //{
                    //    CreditRequestDTONew.GrandTotal = CreditRequestDTO.GrandTotal;
                    //}

                    GlobalSettings.LoggedInClientId = CreditRequestDTO.ClientId;
                    GlobalSettings.LoggedInUserId = CreditRequestDTO.RequestedBy;
                    int PartnerId = ClientService.GetById(CreditRequestDTO.ClientId).PartnerId;
                    GlobalSettings.LoggedInPartnerId = PartnerId; 


                    UnitOfWork uow = new UnitOfWork();
                    CreditRequest CreditRequest = Transform.CreditRequestToDomain(CreditRequestDTONew);
                    uow.CreditRequestRepo.Update(CreditRequest);
                    uow.SaveChanges();

                    ClientDTO ClientDTO = new ClientDTO();
                    ClientDTO = ClientService.GetById(CreditRequest.ClientId);

                    UserDTO UserDTO = new UserDTO();
                    UserDTO = UserService.GetById(CreditRequest.RequestedBy);

                    DateTime date = (DateTime)CreditRequestDTONew.ProvidedDate;
                    string DateFormat = date.ToString("dd-MMM-yyyy HH:mm");

                    CommonService.SendEmail("msgBlaster Bill", "<html><body><p>Hello " + ClientDTO.Company + ",</p> <br/><p>Your mesgblaster bill details are as follows.<p/> <table><tr><td> Date</td> <td> " + DateFormat + " </td> </tr>  <tr><td> Provided Credits</td> <td> " + CreditRequest.ProvidedCredit + " </td> </tr> <td> Total Balance</td> <td> " + ClientDTO.SMSCredit + " </td> </tr> <tr><td> Rate Per SMS</td> <td> " + String.Format("{0:0.00}", CreditRequest.RatePerSMS) + " </td> </tr> <tr><td> Amount </td> <td> " + String.Format("{0:0.00}", CreditRequest.Amount) + " </td> </tr> <tr><td> Tax</td> <td> " + String.Format("{0:0.00}", CreditRequest.Tax) + "% </td> </tr> <tr><td> <strong>Grand Total</strong></td> <td> <strong>" + String.Format("{0:0.00}", CreditRequest.GrandTotal) + "</strong> </td> </tr>  </table>  </body></html>", UserDTO.Email, "", false);  //<tr><td> Provided Credits</td> <td> " + CreditRequest.ProvidedCredit + " </td> </tr> <tr><td> Rate Per SMS</td> <td> " + CreditRequest.RatePerSMS + " </td> </tr> <tr><td> Amount </td> <td> " + CreditRequest.Amount + " </td> </tr> <tr><td> Tax</td> <td> " + CreditRequest.Tax + " </td> </tr> <tr><td> <strong>Grand Total</strong></td> <td> <strong>" + CreditRequest.GrandTotal + "</strong> </td> </tr>

                    CreditRequestDTONew = Transform.CreditRequestToDTO(CreditRequest);
                    return CreditRequestDTONew;
                }
                else
                {
                    return CreditRequestDTO;
                }

            }
            catch (Exception)
            {
                //return CreditRequestDTO;
                throw;
            }

        }

        //Get all credit request count as per client id
        public static int GetAllCreditRequestCountByClientId(int ClientId)
        {

            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            try
            {
                int TotalCount = 0;

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();
                    // CreditRequest.GroupBy(x => x.RequestedCredit).Select(g => new { Sum = g.Sum(x => x.RequestedCredit) });                    
                    if (CreditRequest != null)
                    {
                        foreach (var item in CreditRequest)
                        {
                            CreditRequestDTOList.Add(Transform.CreditRequestToDTO(item));
                        }
                    }
                    else TotalCount = 0;
                }

                //return CreditRequestDTOList;
                TotalCount = CreditRequestDTOList.Where(p => p.ProvidedCredit > 0).Sum(p => p.ProvidedCredit);

                return TotalCount;

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

        //Get approved credit request count as per client id
        public static int GetApprovedCreditRequestCountByClientId(int ClientId)
        {

            List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
            try
            {
                int TotalCount = 0;

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId && e.ProvidedDate != null).ToList();
                    // CreditRequest.GroupBy(x => x.RequestedCredit).Select(g => new { Sum = g.Sum(x => x.RequestedCredit) });                    
                    if (CreditRequest != null)
                    {
                        foreach (var item in CreditRequest)
                        {
                            CreditRequestDTOList.Add(Transform.CreditRequestToDTO(item));
                        }
                    }
                    else TotalCount = 0;
                }

                //return CreditRequestDTOList;
                TotalCount = CreditRequestDTOList.Where(p => p.ProvidedCredit > 0).Sum(p => p.ProvidedCredit);

                return TotalCount;

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

        //Provide credits by using mail link
        public static CreditRequestDTO ProvideCreditByMailLink(int CreditRequestId)
        {
            CreditRequestDTO CreditRequestDTONew = new CreditRequestDTO();
            CreditRequestDTONew = GetById(CreditRequestId);
            if (CreditRequestDTONew.IsProvided == true)
            {
                CreditRequestDTONew = null;
                return CreditRequestDTONew;
            }

            try
            {
                

                //if (CreditRequestDTO.Tax == null)
                //{
                //    CreditRequestDTO.Tax = 0;
                //}


                CreditRequestDTONew.IsProvided = true;
                CreditRequestDTONew.ProvidedDate = System.DateTime.Now;
                CreditRequestDTONew.ProvidedCredit = CreditRequestDTONew.RequestedCredit;
                CreditRequestDTONew.Amount = CreditRequestDTONew.RequestedCredit * CreditRequestDTONew.RatePerSMS;
                double tax = (CreditRequestDTONew.Amount * Convert.ToDouble(CreditRequestDTONew.Tax)) / 100;
                CreditRequestDTONew.TaxAmount = tax;
                CreditRequestDTONew.GrandTotal = CreditRequestDTONew.Amount + tax;

                //if (CreditRequestDTO.GrandTotal != CreditRequestDTONew.GrandTotal )
                //{
                //    CreditRequestDTONew.GrandTotal = CreditRequestDTO.GrandTotal;
                //}

                GlobalSettings.LoggedInClientId = CreditRequestDTONew.ClientId;
                GlobalSettings.LoggedInUserId = CreditRequestDTONew.RequestedBy;
                int PartnerId = ClientService.GetById(CreditRequestDTONew.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId; 


                UnitOfWork uow = new UnitOfWork();
                CreditRequest CreditRequest = Transform.CreditRequestToDomain(CreditRequestDTONew);
                uow.CreditRequestRepo.Update(CreditRequest);
                uow.SaveChanges();


                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(CreditRequest.ClientId);
                ClientDTO.SMSCredit = ClientDTO.SMSCredit + CreditRequest.ProvidedCredit; // CreditRequest.RequestedCredit;
                ClientDTO.TotalProvidedCredit = ClientDTO.TotalProvidedCredit + CreditRequest.ProvidedCredit;
                Client Client = Transform.ClientToDomain(ClientDTO);
                uow.ClientRepo.Update(Client);
                uow.SaveChanges();

                UserDTO UserDTO = new UserDTO();
                UserDTO = UserService.GetById(CreditRequest.RequestedBy);

                DateTime date = (DateTime)CreditRequest.ProvidedDate;
                string DateFormat = date.ToString("dd-MMM-yyyy HH:mm");
                CommonService.SendEmail("msgBlaster SMS credit provided", "<html><body><p>Hello " + Client.Company + ",</p> <br/><p>Your mesgblaster credit details are as follows.<p/> <table><tr><td> Date</td> <td> " + DateFormat + " </td> </tr>  <tr><td> Provided Credits</td> <td> " + CreditRequest.ProvidedCredit + " </td> </tr> <td> Total Balance</td> <td> " + Client.SMSCredit + " </td> </tr>  </table>  </body></html>", UserDTO.Email, "", false);  //<tr><td> Provided Credits</td> <td> " + CreditRequest.ProvidedCredit + " </td> </tr> <tr><td> Rate Per SMS</td> <td> " + CreditRequest.RatePerSMS + " </td> </tr> <tr><td> Amount </td> <td> " + CreditRequest.Amount + " </td> </tr> <tr><td> Tax</td> <td> " + CreditRequest.Tax + " </td> </tr> <tr><td> <strong>Grand Total</strong></td> <td> <strong>" + CreditRequest.GrandTotal + "</strong> </td> </tr>

                CreditRequestDTONew = Transform.CreditRequestToDTO(CreditRequest);
                return CreditRequestDTONew;

            }
            catch (Exception)
            {
                // return CreditRequestDTO;  
                throw;
            }

        }

        //Get requested credit request count as per client id
        public static int GetRequestedCreditRequestCount(int ClientId)
        {
            UnitOfWork uow = new UnitOfWork();
            IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsProvided == false).ToList();
            if (CreditRequest.Count() != 0)
            {
                return CreditRequest.Count();
            }
            return 0;
        }

        //Get provided credit request count as per client id
        public static int GetProvidedCreditRequestCount(int ClientId)
        {
            UnitOfWork uow = new UnitOfWork();
            IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsProvided == true).ToList();
            if (CreditRequest.Count() != 0)
            {
                return CreditRequest.Count();
            }
            return 0;
        }

        //Get pending credit request count as per client id
        public static double GetPendingCreditRequestCount(int ClientId)
        {
            UnitOfWork uow = new UnitOfWork();
            IEnumerable<CreditRequest> CreditRequest = uow.CreditRequestRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsProvided == false && e.ProvidedCredit == 0).ToList();
            return CreditRequest.Sum(e => e.RequestedCredit);
        }

        public static bool Onlinepayment(double PayableAmount)
        {
            bool IsPaymentSucess = false;
            IsPaymentSucess = false;
            return IsPaymentSucess;

        }

        #endregion

        #region "Unwanted code"

        //public static PageData<CreditRequestDTO> GetCreditRequestPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        //{
        //    List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
        //    PageData<CreditRequestDTO> pageList = new PageData<CreditRequestDTO>();

        //    if (pagingInfo == null)
        //    {
        //        PagingInfo PagingInfoCreated = new PagingInfo();
        //        PagingInfoCreated.Page = 1;
        //        PagingInfoCreated.Reverse = false;
        //        PagingInfoCreated.ItemsPerPage = 1;
        //        PagingInfoCreated.Search = "";
        //        PagingInfoCreated.TotalItem = 0;

        //        pagingInfo = PagingInfoCreated;
        //    }
        //    if (pagingInfo.SortBy == "")
        //    {
        //        pagingInfo.SortBy = "Date";
        //    }

        //    CreditRequestDTOList = GetCreditRequestsbyPartnerId(PartnerId, pagingInfo.Search);
        //    IQueryable<CreditRequestDTO> CreditRequestDTOPagedList = CreditRequestDTOList.AsQueryable();

        //    ////Sorting
        //    //CreditRequestDTOPagedList = PagingService.Sorting<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

        //    // paging
        //    if (CreditRequestDTOPagedList.Count() > 0)
        //    {
        //        var ContacDTOPerPage = PagingService.Paging<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
        //        pageList.Count = CreditRequestDTOPagedList.Count();

        //        List<CreditRequestDTO> pagedCreditRequestDTOList = new List<CreditRequestDTO>();
        //        foreach (var item in ContacDTOPerPage)
        //        {
        //            pagedCreditRequestDTOList.Add(item);
        //        }
        //        pageList.Data = pagedCreditRequestDTOList;
        //    }
        //    else
        //    {
        //        pageList.Data = null;
        //    }



        //    return pageList;
        //}

        //public static PageData<CreditRequestDTO> GetCreditRequestPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        //{
        //    List<CreditRequestDTO> CreditRequestDTOList = new List<CreditRequestDTO>();
        //    PageData<CreditRequestDTO> pageList = new PageData<CreditRequestDTO>();

        //    if (pagingInfo == null)
        //    {
        //        PagingInfo PagingInfoCreated = new PagingInfo();
        //        PagingInfoCreated.Page = 1;
        //        PagingInfoCreated.Reverse = false;
        //        PagingInfoCreated.ItemsPerPage = 1;
        //        PagingInfoCreated.Search = "";
        //        PagingInfoCreated.TotalItem = 0;

        //        pagingInfo = PagingInfoCreated;
        //    }

        //    CreditRequestDTOList = GetCreditRequestsbyClientId(ClientId, pagingInfo.Search);
        //    IQueryable<CreditRequestDTO> CreditRequestDTOPagedList = CreditRequestDTOList.AsQueryable();

        //    ////Sorting
        //    //CreditRequestDTOPagedList = PagingService.Sorting<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

        //    // paging
        //    if (CreditRequestDTOPagedList.Count() > 0)
        //    {
        //        var ContacDTOPerPage = PagingService.Paging<CreditRequestDTO>(CreditRequestDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
        //        pageList.Count = CreditRequestDTOPagedList.Count();

        //        List<CreditRequestDTO> pagedCreditRequestDTOList = new List<CreditRequestDTO>();
        //        foreach (var item in ContacDTOPerPage)
        //        {
        //            pagedCreditRequestDTOList.Add(item);
        //        }
        //        pageList.Data = pagedCreditRequestDTOList;
        //    }
        //    else
        //    {
        //        pageList.Data = null;
        //    }



        //    return pageList;
        //}

        #endregion
        
    }
}

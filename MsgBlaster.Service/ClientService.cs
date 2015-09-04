using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;
using System.Web;

using System.IO;

using System.Web.Security;
using System.Net;
//using System.Web.Mvc;

namespace MsgBlaster.Service
{
    public class ClientService
    {

        #region "CRUD Functionality"
       
        //Create Client
        public static ClientDTO Create(ClientDTO ClientDTO)
        {
            try
            {   
                ClientDTO.RegisteredDate = System.DateTime.Now;
                ClientDTO.IsActive = true;
                ClientDTO.AlertOnCredit = false;
                ClientDTO.SMSCredit = 20;
                ClientDTO.SMSGatewayId = 1;
                //ClientDTO.AllowChequePayment = true;
                //ClientDTO.Email = ClientDTO.Email.ToLower();
                ClientDTO.PartnerId = 1;
                ClientDTO.IsDatabaseUploaded = false;
                var Client = new Client();
                using (var uow = new UnitOfWork())
                {
                    Client = Transform.ClientToDomain(ClientDTO);

                    uow.ClientRepo.Insert(Client);
                    uow.SaveChanges();
                    //HttpContext.Current.Session["LoggedClient"] = Client;
                    //HttpContext.Current.Session["LoggedClientId"] = Client.Id;

                    if (Client.Id > 0)
                    {
                        ClientDTO.Id = Client.Id;
                        return ClientDTO;
                    }
                    else throw new OperationCanceledException("Insert operation terminated");
                }

            }
            catch (msgBlasterValidationException)
            {
                throw new System.TimeoutException();
            }
            catch (Exception)
            {
                //HttpContext.Current.Session["LoggedClient"] = null;
                //HttpContext.Current.Session["LoggedClientId"] = "0";
                throw;
            }

        }

        //Edit Client
        public static void Edit(ClientDTO ClientDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = ClientDTO.Id;
                int PartnerId = ClientService.GetById(ClientDTO.Id).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;  

                UnitOfWork uow = new UnitOfWork();
                Client Client = Transform.ClientToDomain(ClientDTO);
                uow.ClientRepo.Update(Client);
                uow.SaveChanges();
                if (ClientDTO.IsMailSend == true)
                {
                    List<UserDTO> UserDTOList = new List<UserDTO>();
                    UserDTOList = UserService.GetUsersbyClientId(ClientDTO.Id, "").Where(e => e.UserType == UserType.Admin.ToString()).OrderBy(e => e.Id).ToList();
                    foreach (var item in UserDTOList)
                    {
                        if (item.IsActive == true && item.UserType == UserType.Admin.ToString())
                        {
                            CommonService.SendEmail("Sender code application approved", "<HTML><BODY><P>Hello " + ClientDTO.Company + "</P><P>Your request for sender code " + ClientDTO.SenderCode + " has approved.</P></BODY></HTML>", item.Email, "", false);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        //Delete Client
        public static void Delete(int id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                uow.ClientRepo.Delete(id);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get Client by id
        public static ClientDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Client Client = uow.ClientRepo.GetById(Id);
                ClientDTO ClientDTO = Transform.ClientToDTO(Client);
                return ClientDTO;
            }
            catch
            {
                throw;
            }
        }

        //Delete sender code document
        public static bool DeleteSenderCodeDocument(string filePath, int ClientId)
        {
            try
            {
                if (File.Exists(filePath))
                {

                    File.Delete(filePath);
                    ClientDTO ClientDTO = new ClientDTO();
                    ClientDTO = GetById(ClientId);
                    ClientDTO.SenderCodeFilePath = null;
                
                    GlobalSettings.LoggedInClientId = ClientDTO.Id;
                    int PartnerId = ClientService.GetById(ClientDTO.Id).PartnerId;
                    GlobalSettings.LoggedInPartnerId = PartnerId;  

                    Edit(ClientDTO);

                    // Send Mail to Admin                   
                    PartnerDTO PartnerDTO = new PartnerDTO();
                    PartnerDTO = PartnerService.GetById(ClientDTO.PartnerId);
                    CommonService.SendEmail("Removed sendercode application request", "<HTML><BODY><P>Hello " + CommonService.GetFirstname(PartnerDTO.Name) + "</P><P>Request for sender code from " + ClientDTO.Company + " has been removed.</P></BODY></HTML>", PartnerDTO.Email, "", false);

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }


        }

        #endregion

        #region "List Functionality"

        //Get List of Client by partner
        public static List<ClientDTO> GetClientsbyPartnerId(int PartnerId, PagingInfo pagingInfo)
        {

            List<ClientDTO> ClientDTO = new List<ClientDTO>();

            //List<PartnerDTO> PartnerDTO = new List<PartnerDTO>();

            try
            {
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                UnitOfWork uow = new UnitOfWork();
                IQueryable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId).AsQueryable();// .ToList().OrderBy(e=>e.Company);
                Client = PagingService.Sorting<Client>(Client, pagingInfo.SortBy, pagingInfo.Reverse);
                Client = Client.Skip(skip).Take(take);

                List<ClientDTO> ClientDTOList = new List<ClientDTO>();

                if (Client != null)
                {
                    foreach (var item in Client)
                    {
                        ClientDTOList.Add(Transform.ClientToDTO(item));

                    }
                }




                if (ClientDTOList != null)
                {
                    if (pagingInfo.Search != "" && pagingInfo.Search != null)
                    {

                        bool IsDate = CommonService.IsDate(pagingInfo.Search);
                        if (IsDate != true)
                        {

                            IQueryable<Client> Clientsearch = uow.ClientRepo.GetAll().Where(e => (e.Address != null ? (e.Address.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.Company.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.AlertCreditBalanceLimit.ToString() != null ? (e.AlertCreditBalanceLimit.ToString().Contains(pagingInfo.Search.ToString())) : false) || e.SMSCredit.ToString().Contains(pagingInfo.Search) || (e.RegisteredDate.ToString() != null ? (Convert.ToDateTime(e.RegisteredDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable();//.OrderBy(e => e.Company);
                            Clientsearch = PagingService.Sorting<Client>(Clientsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Clientsearch = Clientsearch.Skip(skip).Take(take);

                            if (Clientsearch != null)
                            {
                                foreach (var item in Clientsearch)
                                {

                                    ClientDTO.Add(Transform.ClientToDTO(item));
                                }
                            }
                            return ClientDTO;
                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(pagingInfo.Search);
                            IQueryable<Client> Clientsearch = uow.ClientRepo.GetAll().Where(e => e.RegisteredDate >= date && e.RegisteredDate < date.AddDays(1)).AsQueryable();//.OrderBy(e => e.Company);
                            Clientsearch = PagingService.Sorting<Client>(Clientsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Clientsearch = Clientsearch.Skip(skip).Take(take);

                            if (Clientsearch != null)
                            {
                                foreach (var item in Clientsearch)
                                {
                                    ClientDTO.Add(Transform.ClientToDTO(item));
                                }
                            }
                            return ClientDTO;

                        }

                    }
                    else
                    {


                        //foreach (var item in ClientDTOList)
                        //{
                        //    ClientDTO.Add(item);
                        //}
                        //return ClientDTO;
                    }
                }

                return ClientDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Client paged list by partner id
        public static PageData<ClientDTO> GetClientPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        {
            List<ClientDTO> ClientDTOList = new List<ClientDTO>();
            PageData<ClientDTO> pageList = new PageData<ClientDTO>();

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
                pagingInfo.SortBy = "Company";
            }

            ClientDTOList = GetClientsbyPartnerId(PartnerId, pagingInfo);
            IQueryable<ClientDTO> ClientDTOPagedList = ClientDTOList.AsQueryable();

            ////Sorting
            //ClientDTOPagedList = PagingService.Sorting<ClientDTO>(ClientDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.ClientRepo.GetAll().Where(e => (e.Address != null ? (e.Address.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.Company.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.AlertCreditBalanceLimit.ToString() != null ? (e.AlertCreditBalanceLimit.ToString().Contains(pagingInfo.Search.ToString())) : false) || e.SMSCredit.ToString().Contains(pagingInfo.Search) || (e.RegisteredDate.ToString() != null ? (Convert.ToDateTime(e.RegisteredDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)  && e.PartnerId == PartnerId).Count();
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.ClientRepo.GetAll().Where(e => e.RegisteredDate >= date && e.RegisteredDate < date.AddDays(1)  && e.PartnerId == PartnerId).Count();

                }

            }
            else
            {
                count = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId ).Count();
            }



            // paging
            if (ClientDTOPagedList.Count() > 0)
            {
                pageList.Count = count;
                //var ContacDTOPerPage = PagingService.Paging<ClientDTO>(ClientDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                //pageList.Count = ClientDTOPagedList.Count();

                List<ClientDTO> pagedClientDTOList = new List<ClientDTO>();
                foreach (var item in ClientDTOList) //ContacDTOPerPage
                {
                    pagedClientDTOList.Add(item);
                }
                pageList.Data = pagedClientDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Get client list with active or inactive status by partner
        public static List<ClientDTO> GetClientsbyPartnerIdWithIsActive(int PartnerId, string search, bool IsActive, PagingInfo pagingInfo)
        {

            List<ClientDTO> ClientDTO = new List<ClientDTO>();

            //List<PartnerDTO> PartnerDTO = new List<PartnerDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                IQueryable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsActive == IsActive).ToList().AsQueryable();//.OrderBy(e => e.Company);
                Client = PagingService.Sorting<Client>(Client, pagingInfo.SortBy, pagingInfo.Reverse);
                Client = Client.Skip(skip).Take(take);

                List<ClientDTO> ClientDTOList = new List<ClientDTO>();

                if (Client != null)
                {
                    foreach (var item in Client)
                    {
                        ClientDTOList.Add(Transform.ClientToDTO(item));

                    }
                }

                if (ClientDTOList != null)
                {
                    if (search != "" && search != null)
                    {

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {

                            IQueryable<Client> Clientsearch = uow.ClientRepo.GetAll().Where(e => (e.Address != null ? (e.Address.ToLower().Contains(search.ToLower())) : false) || e.Company.ToLower().Contains(search.ToLower()) || (e.AlertCreditBalanceLimit.ToString() != null ? (e.AlertCreditBalanceLimit.ToString().Contains(search.ToString())) : false) || e.SMSCredit.ToString().Contains(search) || (e.RegisteredDate.ToString() != null ? (Convert.ToDateTime(e.RegisteredDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) && e.IsActive == IsActive).AsQueryable();//.OrderBy(e => e.Company);
                            Clientsearch = PagingService.Sorting<Client>(Clientsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Clientsearch = Clientsearch.Skip(skip).Take(take);

                            if (Clientsearch != null)
                            {
                                foreach (var item in Clientsearch)
                                {

                                    ClientDTO.Add(Transform.ClientToDTO(item));
                                }
                            }
                            return ClientDTO.Skip(skip).Take(take).ToList();
                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(search);
                            IQueryable<Client> Clientsearch = uow.ClientRepo.GetAll().Where(e => e.RegisteredDate >= date && e.RegisteredDate < date.AddDays(1) && e.IsActive == IsActive).AsQueryable();//.OrderBy(e => e.Company);
                            Clientsearch = PagingService.Sorting<Client>(Clientsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Clientsearch = Clientsearch.Skip(skip).Take(take);
                            if (Clientsearch != null)
                            {
                                foreach (var item in Clientsearch)
                                {
                                    ClientDTO.Add(Transform.ClientToDTO(item));
                                }
                            }
                            return ClientDTO.Skip(skip).Take(take).ToList();

                        }

                    }
                    else
                    {


                        foreach (var item in ClientDTOList)
                        {
                            ClientDTO.Add(item);
                        }
                        return ClientDTO.Skip(skip).Take(take).ToList();
                    }
                }

                return ClientDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get paged list of client by partner and with active or inactive status
        public static PageData<ClientDTO> GetClientPagedListbyPartnerIdWithIsActive(PagingInfo pagingInfo, int PartnerId, bool IsActive)
        {
            List<ClientDTO> ClientDTOList = new List<ClientDTO>();
            PageData<ClientDTO> pageList = new PageData<ClientDTO>();

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
                pagingInfo.SortBy = "Company";
            }

            ClientDTOList = GetClientsbyPartnerIdWithIsActive(PartnerId, pagingInfo.Search, IsActive, pagingInfo);
            IQueryable<ClientDTO> ClientDTOPagedList = ClientDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.ClientRepo.GetAll().Where(e => (e.Address != null ? (e.Address.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.Company.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.AlertCreditBalanceLimit.ToString() != null ? (e.AlertCreditBalanceLimit.ToString().Contains(pagingInfo.Search.ToString())) : false) || e.SMSCredit.ToString().Contains(pagingInfo.Search) || (e.RegisteredDate.ToString() != null ? (Convert.ToDateTime(e.RegisteredDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) && e.IsActive == IsActive && e.PartnerId == PartnerId).Count();
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.ClientRepo.GetAll().Where(e => e.RegisteredDate >= date && e.RegisteredDate < date.AddDays(1) && e.IsActive == IsActive && e.PartnerId == PartnerId).Count();

                }

            }
            else
            {
                count = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsActive == IsActive).Count();
            }

            ////Sorting
            //ClientDTOPagedList = PagingService.Sorting<ClientDTO>(ClientDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (ClientDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<ClientDTO>(ClientDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// ClientDTOPagedList.Count();
                pageList.SuccessCount = GetActiveClientCount(PartnerId);
                pageList.FailureCount = GetInactiveClientCount(PartnerId);

                List<ClientDTO> pagedClientDTOList = new List<ClientDTO>();
                foreach (var item in ClientDTOPagedList)
                {
                    pagedClientDTOList.Add(item);
                }
                pageList.Data = pagedClientDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Get active clients count
        public static int GetActiveClientCount(int PartnerId)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsActive == true).ToList().OrderBy(e => e.Company);
                if (Client.Count() != 0)
                {
                    return Client.Count();
                }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get inactive clients count
        public static int GetInactiveClientCount(int PartnerId)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId && e.IsActive == false).ToList().OrderBy(e => e.Company);
                if (Client.Count() != 0)
                {
                    return Client.Count();
                }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get all clients
        public static List<ClientDTO> GetAllActiveClients()
        {
            List<ClientDTO> ClientDTOList = new List<ClientDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IList<Client> Clients = uow.ClientRepo.GetAll().Where(e => e.IsActive == true).ToList();

                if (Clients != null)
                {
                    foreach (var item in Clients)
                    {
                        ClientDTOList.Add(Transform.ClientToDTO(item));
                    }
                }

                return ClientDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }
      
        #endregion
        
        #region "Other Functionality"

        //Get client is active or inactive
        public static bool ActiveInactiveClient(int ClientId)
        {

            try
            {
                if (ClientId <= 0) return false;

                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = GetById(ClientId);

                GlobalSettings.LoggedInClientId = ClientDTO.Id;
                int PartnerId = ClientService.GetById(ClientDTO.Id).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;  

                if (ClientDTO == null) return false;

                if (ClientDTO.IsActive == true)
                {
                    ClientDTO.IsActive = false;
                    Edit(ClientDTO);
                    return true;
                }
                else
                {
                    ClientDTO.IsActive = true;
                    Edit(ClientDTO);
                    return true;
                }

            }
            catch
            {
                throw;
            }
        }

        //Send OTP code to provided mobile number
        public static string OTP(string MobileNo)
        {
            string OTPCode = "";
            OTPCode = CommonService.OTPVeificationcode(MobileNo, "MSGBLS");
            return OTPCode;
        }

        //Send OTP by mail to provided email
        public static string OTPByMail(string Email, string Name)
        {
            string OTPCode = "";
            OTPCode = CommonService.EmailVerificationCode();
            bool IsMailSent = CommonService.SendEmail("msgBlaster verification code", "Hello " + Name + ",<br/> Your msgBlaster verification code is - " + OTPCode + "", Email, "", false);
            if (IsMailSent == false)
            {
                return ("Error while mail send");
            }

            return OTPCode;

        }

        //Set minimum balance alert by using client id, balance and alert flag
        public static ClientDTO SetMinimumBalanceAlert(int Id, int Balance, bool ShowAlert)
        {
            try
            {
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(Id);
                ClientDTO.AlertCreditBalanceLimit = Balance;
                ClientDTO.AlertOnCredit = ShowAlert;

                GlobalSettings.LoggedInClientId = ClientDTO.Id;
                int PartnerId = ClientService.GetById(ClientDTO.Id).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;  

                UnitOfWork uow = new UnitOfWork();
                Client Client = Transform.ClientToDomain(ClientDTO);
                uow.ClientRepo.Update(Client);
                uow.SaveChanges();


                Client = uow.ClientRepo.GetById(Id);
                ClientDTO = Transform.ClientToDTO(Client);
                return ClientDTO;
            }
            catch
            {
                throw;
            }
        }
       
        //Get client name count by company name and partner id
        public static int GetByClientName(string Company, int PartnerId)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.Company.ToLower() == Company.ToLower() && e.PartnerId == PartnerId);

                if (Client.ToList().Count > 0)
                {
                    foreach (var item in Client)
                    {
                        ClientDTO ClientDTO = Transform.ClientToDTO(item);
                        return ClientDTO.Id;
                    }
                }

                return 0;
            }
            catch
            {
                throw;
            }
        }

        //Get client Id comma separated string by company name and partner id
        public static string GetClientIdarrayByCompany(string Company, int PartnerId)
        {
            string CompanyId = null;
            if (Company == null || Company == "") { return null; }
            try
            {


                if (Company != null)
                {
                    UnitOfWork uow = new UnitOfWork();
                    IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.Company.ToLower().Contains(Company.ToLower()) && e.PartnerId == PartnerId);

                    if (Client != null)
                    {
                        foreach (var item in Client)
                        {
                            CompanyId = CompanyId + item.Id.ToString() + ",";
                            //return UserId;
                        }
                        if (CompanyId != null)
                        {
                            CompanyId = CompanyId.Remove(CompanyId.LastIndexOf(','));
                        }
                        return CompanyId;
                    }
                    else return CompanyId;
                }
                else return CompanyId;
            }
            catch
            {
                throw;
            }
        }

        //Get TRUE or false for unique client by company name and partner id
        public static bool GetClientIsUniqueByCompanyAndPartnerId(string Company, int PartnerId, int Id)
        {
            try
            {
                if (Company == null || Company == "") { return false; }
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => (e.PartnerId.Equals(PartnerId)) && (e.Id != Id) && (e.Company.ToLower().Equals(Company.ToLower())));// .Where(e => (e.Company.ToLower()==Company.ToLower()) && (e.PartnerId== PartnerId) && (e.Id != Id));
                //ClientDTO ClientDTO = Transform.ClientToDTO(Client);
                if (Client.ToList().Count > 0)
                {
                    return true;
                }
                else return false;
            }
            catch
            {
                throw;
            }
        }

        //Upload Sender code document to the server
        public static string UploadSenderCodeDocument(HttpPostedFile file, string documentPath, int ClientId, string FileName)
        {
            try
            {
                var b = new byte[file.ContentLength];
                string result = "";
                documentPath = documentPath + ClientId; //"\\" + ModuleName
                MemoryStream ms = new MemoryStream(b);
                // MemoryStream ms = new MemoryStream(file.ContentLength);
                bool IsExists = System.IO.Directory.Exists(documentPath);
                if (IsExists == false)
                {
                    System.IO.Directory.CreateDirectory(documentPath);
                }

                var path = System.IO.Path.Combine(documentPath, FileName); //file.FileName

                if (File.Exists(path))
                {
                    result = "File already Exists";
                    return result;
                }
                else
                {
                    file.SaveAs(documentPath + "/" + FileName); //file.FileName
                    ms.Close();

                    ClientDTO ClientDTO = new ClientDTO();
                    ClientDTO = ClientService.GetById(ClientId);
                    ClientDTO.SenderCodeFilePath = ClientId + "/" + FileName;// file.FileName;
                    ClientService.Edit(ClientDTO);
                    result = "File uploaded successfully";

                    // Send Mail to Admin                   
                    PartnerDTO PartnerDTO = new PartnerDTO();
                    PartnerDTO = PartnerService.GetById(ClientDTO.PartnerId);
                    CommonService.SendEmail("New sendercode application request", "<HTML><BODY><P>Hello " + CommonService.GetFirstname(PartnerDTO.Name) + "</P><P>" + ClientDTO.Company + " has requested for new sender code.</P></BODY></HTML>", PartnerDTO.Email, "", false);
                    return result;

                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }


        }
        
        //Upload SQLite database file to the server
        public static string UploadSQLiteDatabase(HttpPostedFile file, string documentPath, int ClientId, string FileName)
        {
            try
            {
                var b = new byte[file.ContentLength];
                string result = "";
                documentPath = documentPath + ClientId; //"\\" + ModuleName
                MemoryStream ms = new MemoryStream(b);
                // MemoryStream ms = new MemoryStream(file.ContentLength);
                bool IsExists = System.IO.Directory.Exists(documentPath);
                if (IsExists == false)
                {
                    System.IO.Directory.CreateDirectory(documentPath);
                }

                var path = System.IO.Path.Combine(documentPath, FileName); //file.FileName

                if (File.Exists(path))
                {
                    result = "File already Exists";
                    return result;
                }
                else
                {
                    file.SaveAs(documentPath + "/" + FileName); //file.FileName
                    ms.Close();

                    //Change Client  IsDatabaseUploaded status
                    ClientDTO ClientDTO = new ClientDTO();
                    ClientDTO = ClientService.GetById(ClientId);
                    ClientDTO.IsDatabaseUploaded = true;

                    GlobalSettings.LoggedInClientId = ClientDTO.Id;
                    int PartnerId = ClientService.GetById(ClientDTO.Id).PartnerId;
                    GlobalSettings.LoggedInPartnerId = PartnerId;  

                    ClientService.Edit(ClientDTO);

                    result = "File uploaded successfully";

                    return result;

                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }


        }

        //Get comma separated ClientId  as per partner
        public static string GetClientIdarrayByPartnerId(int PartnerId)
        {
            string ClientId = null;
            if (PartnerId == 0) { return null; }
            try
            {


                if (PartnerId != 0)
                {
                    UnitOfWork uow = new UnitOfWork();
                    IEnumerable<Client> ClientList = uow.ClientRepo.GetAll().Where(e => e.PartnerId == PartnerId);

                    if (ClientList != null)
                    {
                        foreach (var item in ClientList)
                        {
                            ClientId = ClientId + item.Id.ToString() + ",";
                            //return UserId;
                        }
                        if (ClientId != null)
                        {
                            ClientId = ClientId.Remove(ClientId.LastIndexOf(','));
                        }

                        return ClientId;
                    }
                    else return ClientId;
                }
                else return ClientId;
            }
            catch
            {
                throw;
            }
        }

        //Modify client settings as per client details
        public static ClientDTO ModifyClientSettings(ClientDTO ClientDTOSetting)
        {
            try
            {
                ClientDTO ClientDTO = new ClientDTO();
                if (ClientDTOSetting.Id > 0)
                {
                    ClientDTO = ClientService.GetById(ClientDTOSetting.Id);
                    ClientDTO.AlertCreditBalanceLimit = ClientDTOSetting.AlertCreditBalanceLimit;
                    ClientDTO.AlertOnCredit = ClientDTOSetting.AlertOnCredit;
                    ClientDTO.DefaultCouponMessage = ClientDTOSetting.DefaultCouponMessage;
                    ClientDTO.DefaultCouponExpire = ClientDTOSetting.DefaultCouponExpire;
                    ClientDTO.CouponExpireType = ClientDTOSetting.CouponExpireType;

                    GlobalSettings.LoggedInClientId = ClientDTO.Id;
                    int PartnerId = ClientService.GetById(ClientDTO.Id).PartnerId;
                    GlobalSettings.LoggedInPartnerId = PartnerId;  

                    UnitOfWork uow = new UnitOfWork();
                    Client Client = Transform.ClientToDomain(ClientDTO);
                    uow.ClientRepo.Update(Client);
                    uow.SaveChanges();

                    //Client = uow.ClientRepo.GetById(Client.Id);
                    ClientDTO = Transform.ClientToDTO(Client);
                }
                return ClientDTO;
            }
            catch
            {
                throw;
            }
        }

        public static ClientDTO SendMessagesForTodaysBirthday(ClientDTO ClientDTOSettings)
        {
            try
            {
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(ClientDTOSettings.Id);
                ClientDTO.BirthdayMessage = ClientDTOSettings.BirthdayMessage;
                ClientDTO.IsSendBirthdayMessages = ClientDTOSettings.IsSendBirthdayMessages;
                ClientDTO.IsSendBirthdayCoupons = ClientDTOSettings.IsSendBirthdayCoupons;
                ClientDTO.BirthdayCouponExpire = ClientDTOSettings.BirthdayCouponExpire;
                ClientDTO.BirthdayCouponExpireType = ClientDTOSettings.BirthdayCouponExpireType;
                ClientDTO.MinPurchaseAmountForBirthdayCoupon = ClientDTOSettings.MinPurchaseAmountForBirthdayCoupon;

                GlobalSettings.LoggedInClientId = ClientDTO.Id;
                int PartnerId = ClientService.GetById(ClientDTO.Id).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;  

                UnitOfWork uow = new UnitOfWork();
                Client Client = Transform.ClientToDomain(ClientDTO);
                uow.ClientRepo.Update(Client);
                uow.SaveChanges();


                Client = uow.ClientRepo.GetById(ClientDTOSettings.Id);
                ClientDTO = Transform.ClientToDTO(Client);
                return ClientDTO;
            }
            catch
            {
                throw;
            }
        }

        public static ClientDTO SendMessagesForTodaysAnniversary(ClientDTO ClientDTOSettings)
        {
            try
            {
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(ClientDTOSettings.Id);
                ClientDTO.AnniversaryMessage = ClientDTOSettings.AnniversaryMessage;
                ClientDTO.IsSendAnniversaryMessages = ClientDTOSettings.IsSendAnniversaryMessages;
                ClientDTO.IsSendAnniversaryCoupons = ClientDTOSettings.IsSendAnniversaryCoupons;
                ClientDTO.AnniversaryCouponExpire = ClientDTOSettings.AnniversaryCouponExpire;
                ClientDTO.AnniversaryCouponExpireType = ClientDTOSettings.AnniversaryCouponExpireType;
                ClientDTO.MinPurchaseAmountForAnniversaryCoupon = ClientDTOSettings.MinPurchaseAmountForAnniversaryCoupon;

                GlobalSettings.LoggedInClientId = ClientDTO.Id;
                int PartnerId = ClientService.GetById(ClientDTO.Id).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;  

                UnitOfWork uow = new UnitOfWork();
                Client Client = Transform.ClientToDomain(ClientDTO);
                uow.ClientRepo.Update(Client);
                uow.SaveChanges();


                Client = uow.ClientRepo.GetById(ClientDTOSettings.Id);
                ClientDTO = Transform.ClientToDTO(Client);
                return ClientDTO;
            }
            catch
            {
                throw;
            }
        }


        #endregion
     
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;
using System.Xml.Linq;
using System.Configuration;

namespace MsgBlaster.Service
{
    public class UserService
    {
        #region "CRUD Functionality"

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="UserDTO">UserDTO object</param>
        /// <returns>This will return user details</returns>
        public static  UserDTO Create(UserDTO UserDTO)
        {
            //if (UserDTO.Mobile == null && UserDTO.Mobile == "")
            //{ 
            //    return null; 
            //}


            GlobalSettings.LoggedInClientId = UserDTO.ClientId;            
            int PartnerId = ClientService.GetById(UserDTO.ClientId).PartnerId;
            GlobalSettings.LoggedInPartnerId = PartnerId;

            try
            {
                var User = new User();
                using (var uow = new UnitOfWork())
                {
                    UserDTO.IsActive = true;
                    //UserDTO.UserType = "Admin";
                    User = Transform.UserToDomain(UserDTO);
                    uow.UserRepo.Insert(User);
                    uow.SaveChanges();
                    CommonService.SendEmail("msgBlaster Login details", "Hello " + User.FirstName + ", <br/><br/> <p>Your msgBlaster username and Password is as follows - </p> <br/> <table><tr><td> Username</td><td> = " + User.Email + "</td></tr><tr><td>Password</td><td> = " + User.Password + "</td></tr></table>", User.Email, "", false);
                   
                    if (User.Id > 0)
                    {
                        UserDTO.Id = User.Id;
                        return UserDTO;
                    }
                    else throw new OperationCanceledException("Insert operation terminated");       

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

        /// <summary>
        /// Edit User
        /// </summary>
        /// <param name="UserDTO"> Modify the user as per provided user details </param>
        public static void Edit(UserDTO UserDTO)
        {
            //if (UserDTO.Mobile != null && UserDTO.Mobile != "")
            //{
                try
                {
                    GlobalSettings.LoggedInClientId = UserDTO.ClientId;
                    GlobalSettings.LoggedInUserId = UserDTO.Id;
                    int PartnerId = ClientService.GetById(UserDTO.ClientId).PartnerId;
                    GlobalSettings.LoggedInPartnerId = PartnerId;

                    UserDTO UserDTOOld = new UserDTO();
                    UserDTOOld = GetById(UserDTO.Id);

                    UnitOfWork uow = new UnitOfWork();
                    User User = Transform.UserToDomain(UserDTO);
                    uow.UserRepo.Update(User);
                    uow.SaveChanges();

                    //if (UserDTOOld.Password != UserDTO.Password || UserDTOOld.Email != UserDTO.Email)
                    //{
                    //    CommonService.SendEmail("Your msgBlaster Login details are modified", "Hello " + User.Name + ", <br/><br/> <p>Your latest msgBlaster username and Password is as follows - </p> <br/> <table><tr><td> Username</td><td> = " + User.Email + "</td></tr><tr><td>Password</td><td> = " + User.Password + "</td></tr></table>", User.Email, "", false);
                    //}

                }
                catch
                {
                    throw;
                }
            //}
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="id">Delete user as per Id</param>
        public static void Delete(int id)
        {
            try
            {
                GlobalSettings.LoggedInClientId = GetById(id).ClientId;
                GlobalSettings.LoggedInUserId = id;
                int PartnerId = ClientService.GetById(Convert.ToInt32( GlobalSettings.LoggedInClientId)).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                UnitOfWork uow = new UnitOfWork();
                uow.UserRepo.Delete(id);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get user info by Id
        /// </summary>
        /// <param name="Id">Id of the User</param>
        /// <returns>Returns User Details as per Id</returns>
        public static UserDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                User User = uow.UserRepo.GetById(Id);
                UserDTO UserDTO = Transform.UserToDTO(User);
                return UserDTO;
            }
            catch
            {
                throw;
            }
        }       
        
        #endregion
       
        #region "Login Functionality"
        
        /// <summary>
        /// Sign In user
        /// </summary>
        /// <param name="Email">Email Id of active user</param>
        /// <param name="Password">Password of the active user</param>
        /// <returns></returns>             
        public static UserDTO SignIn(string Email, string Password)//GetClientUserByEmailAndPassword
        {
            try
            {
                UserDTO UserDTO = new UserDTO();
                List<UserDTO> UserDTOList = new List<UserDTO>();

                UnitOfWork uow = new UnitOfWork();
                IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.Email.ToLower() == Email.ToLower() && e.Password == Password && e.IsActive == true);
                if (User != null)
                {
                    foreach (var item in User)
                    {
                        //ClientDTOList.Add(Transform.ClientToDTO(item));
                        UserDTO = Transform.UserToDTO(item);
                        GlobalSettings.LoggedInUserId = UserDTO.Id;
                        UserDTO.UserAccessPrivileges = GetUserAccess(UserDTO.UserType.ToString());
                        // Check Client is Active or not
                        ClientDTO ClientDTO = new ClientDTO();
                        ClientDTO = ClientService.GetById(UserDTO.ClientId);
                        GlobalSettings.LoggedInClientId = ClientDTO.Id;
                        
                    
                        if (ClientDTO.IsActive != true)
                        {
                            UserDTO = null;
                        }
                        //HttpContext.Current.Session["LoggedClient"] = ClientDTO;
                        //HttpContext.Current.Session["LoggedClientId"] = ClientDTO.Id;

                    }
                }
                return UserDTO;
            }
            catch
            {
                throw;
            }
        }
                    
        /// <summary>
        ///  Send mail to the user after he forgotten his password
        /// </summary>
        /// <param name="Email">Email of the user</param>
        /// <returns>If mail present then mail sent to that mail and returns TRUE for success if FALSE then it will be either inactive or not present</returns>
        public static bool ForgotPassword(string Email)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                UserDTO UserDTO = new UserDTO();
                IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.Email.ToLower() == Email.ToLower() && e.IsActive == true);
                if (User.ToList().Count > 0)
                {
                    foreach (var item in User)
                    {
                        UserDTO = Transform.UserToDTO(item);
                    }
                    CommonService.SendEmail("msgBlaster Login details", "Hello " + UserDTO.Name + ", <br/><br/> <p>Your msgBlaster username and password is as follows - </p> <br/> <table><tr><td> Username</td><td> = " + UserDTO.Email + "</td></tr><tr><td>Password</td><td> = " + UserDTO.Password + "</td></tr></table>", UserDTO.Email, "", false);
                    return true;
                }
                else return false;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "List Functionality"

             /// <summary>
        /// Get Users list by Client Id
        /// </summary>
        /// <param name="ClientId">Id of the client</param>
        /// <param name="search">search string</param>
        /// <returns>Returns list of the Client</returns>
             public static List<UserDTO> GetUsersbyClientId(int ClientId, string search)
        {

            List<UserDTO> UserDTO = new List<UserDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.ClientId == ClientId).OrderBy(e => e.Name).ToList();
                if (User != null)
                {
                    if (search != "" && search != null)
                    {

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {
                            // string search
                            var Usersearch = User.Where(e => e.Email.ToLower().Contains(search.ToLower()) || e.Name.ToLower().Contains(search.ToLower()) || e.FirstName.ToLower().Contains(search.ToLower()) || e.LastName.ToLower().Contains(search.ToLower()) || (e.Mobile != null ? (e.Mobile.Contains(search)) : false)).OrderBy(e => e.Name);

                            if (Usersearch != null)
                            {
                                foreach (var item in Usersearch)
                                {

                                    UserDTO.Add(Transform.UserToDTO(item));
                                }
                            }
                            return UserDTO;
                        }
                        else
                        {
                            ////date wise search
                            //DateTime date = Convert.ToDateTime(search);
                            //var ClientUsersearch = ClientUser.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1));

                            //if (ClientUsersearch != null)
                            //{
                            //    foreach (var item in ClientUsersearch)
                            //    {
                            //        ClientUserDTO.Add(Transform.ClientUserToDTO(item));
                            //    }
                            //}
                            //return ClientUserDTO;

                        }

                    }
                    else
                    {


                        foreach (var item in User)
                        {
                            UserDTO.Add(Transform.UserToDTO(item));
                        }
                    }
                }

                return UserDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

             /// <summary>
        /// Get paged list of client as per client id and pageinfo object
        /// </summary>
        /// <param name="pagingInfo">pagingInfo object</param>
        /// <param name="ClientId">Id of the client</param>
        /// <returns>Returns paged list of the user</returns>
             public static PageData<UserDTO> GetUserPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            List<UserDTO> UserDTOList = new List<UserDTO>();
            PageData<UserDTO> pageList = new PageData<UserDTO>();

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

            UserDTOList = GetUsersbyClientId(ClientId, pagingInfo.Search);
            IQueryable<UserDTO> UserDTOPagedList = UserDTOList.AsQueryable();

            ////Sorting
            //ClientUserDTOPagedList = PagingService.Sorting<ClientUserDTO>(ClientUserDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (UserDTOPagedList.Count() > 0)
            {
                var ContacDTOPerPage = PagingService.Paging<UserDTO>(UserDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = UserDTOPagedList.Count();

                pageList.SuccessCount = GetActiveUserCount(ClientId);
                pageList.FailureCount = GetInactiveUserCount(ClientId);

                List<UserDTO> pagedClientUserDTOList = new List<UserDTO>();
                foreach (var item in ContacDTOPerPage)
                {
                    pagedClientUserDTOList.Add(item);
                }
                pageList.Data = pagedClientUserDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

             /// <summary>
        /// Get active or inactive Users by Client id 
        /// </summary>
        /// <param name="ClientId">Id of the Client</param>
        /// <param name="search">search string</param>
        /// <param name="IsActive">TRUE OR FALSE</param>
        /// <param name="pagingInfo">pagingInfo object</param>
        /// <returns> Returns Active or Inactive user list </returns>
             public static List<UserDTO> GetUsersbyClientIdWithIsActive(int ClientId, string search, bool IsActive, PagingInfo pagingInfo)
        {
            List<UserDTO> UserDTOList = new List<UserDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();

                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                IQueryable<User> User = uow.UserRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsActive == IsActive).OrderBy(e => e.Name).AsQueryable();// .ToList().Skip(skip).Take(take);
                User = PagingService.Sorting<User>(User, pagingInfo.SortBy, pagingInfo.Reverse);
                User = User.Skip(skip).Take(take);


                if (User != null)
                {
                    foreach (var user in User)
                    {
                        UserDTO UserDTO = new UserDTO();
                        UserDTO = Transform.UserToDTO(user);
                        LocationDTO LocationDTO = new LocationDTO();
                        UserDTO.Location = LocationService.GetById(UserDTO.LocationId).Name;
                        UserDTOList.Add(UserDTO);
                    }

                    if (search != "" && search != null)
                    {
                        //int LocationId = LocationService.GetByLocationName(search, ClientId);
                        string LocationIdString = LocationService.GetLocationIdarrayByName(search, ClientId);

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {
                            // string search
                            List<UserDTO> UserDTOListSearch = new List<UserDTO>();
                            IQueryable<User> UserSearch = uow.UserRepo.GetAll().Where(e => (e.Email.ToLower().Contains(search.ToLower()) || e.Name.ToLower().Contains(search.ToLower()) || e.FirstName.ToLower().Contains(search.ToLower()) || e.LastName.ToLower().Contains(search.ToLower()) || (e.Mobile != null ? (e.Mobile.Contains(search)) : false) || (LocationIdString != null ? (e.LocationId.ToString().Split(',').Any(LocationId => LocationIdString.Contains(LocationId.ToString()))) : false)) && e.IsActive == IsActive && e.ClientId == ClientId).AsQueryable();//.OrderBy(e => e.Name).ToList().Skip(skip).Take(take); //(e.Location != null ? (e.Location.ToLower().Contains(search.ToLower())) : false) 
                            UserSearch = PagingService.Sorting<User>(UserSearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            UserSearch = UserSearch.Skip(skip).Take(take);

                            foreach (var user in UserSearch)
                            {
                                UserDTO UserDTO = new UserDTO();
                                UserDTO = Transform.UserToDTO(user);
                                LocationDTO LocationDTO = new LocationDTO();
                                UserDTO.Location = LocationService.GetById(UserDTO.LocationId).Name;
                                UserDTOListSearch.Add(UserDTO);
                            }
                            return UserDTOListSearch;

                        }
                        else
                        {

                        }

                    }
                    ////else
                    ////{
                    ////    ////foreach (var item in User)
                    ////    ////{
                    ////    ////    //UserDTO UserDTO = new UserDTO();
                    ////    ////    UserDTOList.Add(Transform.UserToDTO(item));
                    ////    ////}
                    ////}
                }

                return UserDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }


             /// <summary>
        /// Get paged list of active or inactive user by Client id
        /// </summary>
        /// <param name="pagingInfo">pagingInfo object</param>
        /// <param name="ClientId">Id of the client</param>
        /// <param name="IsActive">TRUE OR False</param>
        /// <returns>Returns Active or Inactive user's paged list </returns>
             public static PageData<UserDTO> GetUserPagedListbyClientIdWithIsActive(PagingInfo pagingInfo, int ClientId, bool IsActive)
        {
            List<UserDTO> UserDTOList = new List<UserDTO>();
            PageData<UserDTO> pageList = new PageData<UserDTO>();

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
                pagingInfo.SortBy = "Name";
            }


            UserDTOList = GetUsersbyClientIdWithIsActive(ClientId, pagingInfo.Search, IsActive, pagingInfo);
            IQueryable<UserDTO> UserDTOPagedList = UserDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    //int LocationId = LocationService.GetByLocationName(pagingInfo.Search, ClientId);
                    string LocationIdString = LocationService.GetLocationIdarrayByName(pagingInfo.Search, ClientId);
                    count = 0;
                    count = uow.UserRepo.GetAll().Where(e => (e.Email.ToLower().Contains(pagingInfo.Search.ToLower()) || e.Name.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.Mobile != null ? (e.Mobile.Contains(pagingInfo.Search)) : false) || (LocationIdString != null ? (e.LocationId.ToString().Split(',').Any(LocationId => LocationIdString.Contains(LocationId))) : false)) && e.IsActive == IsActive && e.ClientId == ClientId).OrderBy(e => e.Name).Count();
                }
                else
                {
                    //DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    //count = 0;
                    //count = uow.UserRepo.GetAll().Where(e => e.CreatedDate >= date && e.CreatedDate < date.AddDays(1) || e.ScheduledDate >= date && e.ScheduledDate < date.AddDays(1)).OrderByDescending(e => e.CreatedDate).Count();
                }
            }
            else
            {
                count = uow.UserRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsActive == IsActive).Count();
            }

            ////Sorting
            UserDTOPagedList = PagingService.Sorting<UserDTO>(UserDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (UserDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<UserDTO>(UserDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// UserDTOPagedList.Count();

                pageList.SuccessCount = GetActiveUserCount(ClientId);
                pageList.FailureCount = GetInactiveUserCount(ClientId);


                List<UserDTO> pagedClientUserDTOList = new List<UserDTO>();
                foreach (var item in UserDTOPagedList)
                {
                    pagedClientUserDTOList.Add(item);
                }
                pageList.Data = pagedClientUserDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

             /// <summary>
        /// Get active user count by client id
        /// </summary>
        /// <param name="ClientId">Id of the client</param>
        /// <returns>Returns Count of active users</returns>
             public static int GetActiveUserCount(int ClientId)
        {
            UnitOfWork uow = new UnitOfWork();
            IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsActive == true).OrderBy(e => e.Name).ToList();
            if (User.Count() != 0)
            {
                return User.Count();
            }
            return 0;
        }

             /// <summary>
        /// Get inactive user count by client id
        /// </summary>
        /// <param name="ClientId">Id of the client</param>
        /// <returns>Returns Count of inactive users</returns>
             public static int GetInactiveUserCount(int ClientId)
        {
            UnitOfWork uow = new UnitOfWork();
            IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsActive == false).OrderBy(e => e.Name).ToList();
            if (User.Count() != 0)
            {
                return User.Count();
            }
            return 0;
        }

        #endregion
             
        #region "Other Functions"
            
        /// <summary>
        /// Provide User access as per UserType i.e. Admin, Ecoupon or Normal
        /// </summary>
        /// <param name="UserType">User type of the user</param>
        /// <returns> As per logged in user his access rights also provided </returns>            
        public static UserAccessDTO GetUserAccess(string UserType)
             {
                 UserAccessDTO UserAccessDTO = new UserAccessDTO();

                 string xmlFilePath = ConfigurationManager.AppSettings["xmlFilePath"].ToString(); //AppDomain.CurrentDomain.BaseDirectory + @"Content\UserAccesss.xml";

                 XDocument doc = XDocument.Load(xmlFilePath);

                 IEnumerable<XElement> roleItem = from role in doc.Descendants("Role")
                                                  where role.Attribute("Type").Value.ToLower().Equals(UserType.ToLower())
                                                  select role;

                 var accessList = new List<XElement>();
                 accessList = roleItem.ToList();

                 foreach (XElement list in accessList)
                 {
                     if (list.Element("Groups").Attribute("IsAccess") != null)
                         UserAccessDTO.Groups = Convert.ToBoolean((string)list.Element("Groups").Attribute("IsAccess"));

                     if (list.Element("Contacts").Attribute("IsAccess") != null)
                         UserAccessDTO.Contacts = Convert.ToBoolean((string)list.Element("Contacts").Attribute("IsAccess"));

                     if (list.Element("ImportContacts").Attribute("IsAccess") != null)
                         UserAccessDTO.ImportContacts = Convert.ToBoolean((string)list.Element("ImportContacts").Attribute("IsAccess"));

                     if (list.Element("Templates").Attribute("IsAccess") != null)
                         UserAccessDTO.Templates = Convert.ToBoolean((string)list.Element("Templates").Attribute("IsAccess"));

                     if (list.Element("Users").Attribute("IsAccess") != null)
                         UserAccessDTO.Users = Convert.ToBoolean((string)list.Element("Users").Attribute("IsAccess"));

                     if (list.Element("Locations").Attribute("IsAccess") != null)
                         UserAccessDTO.Locations = Convert.ToBoolean((string)list.Element("Locations").Attribute("IsAccess"));

                     if (list.Element("Campaigns").Attribute("IsAccess") != null)
                         UserAccessDTO.Campaigns = Convert.ToBoolean((string)list.Element("Campaigns").Attribute("IsAccess"));

                     if (list.Element("CreditRequests").Attribute("IsAccess") != null)
                         UserAccessDTO.CreditRequests = Convert.ToBoolean((string)list.Element("CreditRequests").Attribute("IsAccess"));

                     if (list.Element("Coupons").Attribute("IsAccess") != null)
                         UserAccessDTO.Coupons = Convert.ToBoolean((string)list.Element("Coupons").Attribute("IsAccess"));

                     if (list.Element("Settings").Attribute("IsAccess") != null)
                         UserAccessDTO.Settings = Convert.ToBoolean((string)list.Element("Settings").Attribute("IsAccess"));

                     if (list.Element("Redeems").Attribute("IsAccess") != null)
                         UserAccessDTO.Redeems = Convert.ToBoolean((string)list.Element("Redeems").Attribute("IsAccess"));

                     if (list.Element("SenderCode").Attribute("IsAccess") != null)
                         UserAccessDTO.SenderCode = Convert.ToBoolean((string)list.Element("SenderCode").Attribute("IsAccess"));


                 }

                 return UserAccessDTO;
             }

        /// <summary>
             /// Get mail id is unique or not
             /// </summary>
             /// <param name="Email">email of the user</param>
             /// <param name="Id">Id of the user</param>
             /// <returns>returns TRUE or FALSE</returns>
        public static bool IsUniqueEmail(string Email, int Id)
             {
                 if (Email == null || Email == "") { return false; }
                 try
                 {
                     if (Email != null)
                     {
                         UnitOfWork uow = new UnitOfWork();
                         IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.Email.ToLower() == Email.ToLower() && e.Id != Id);
                         if (User.ToList().Count > 0)
                         {
                             return true;
                         }
                         else return false;
                     }
                     else return false;
                 }
                 catch
                 {
                     throw;
                 }
             }

        /// <summary>
             /// Get mobile is unique or not
             /// </summary>
             /// <param name="Mobile">mobile number of the user</param>
             /// <param name="Id">id of the user</param>
             /// <returns>returns TRUE or FALSE</returns>
        public static bool IsUniqueMobile(string Mobile, int Id)
             {
                 if (Mobile == null || Mobile == "") { return false; }
                 try
                 {
                     if (Mobile != null)
                     {
                         UnitOfWork uow = new UnitOfWork();
                         IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.Mobile == Mobile && e.Id != Id);
                         if (User.ToList().Count > 0)
                         {
                             return true;
                         }
                         else return false;
                     }
                     else return false;
                 }
                 catch
                 {
                     throw;
                 }
             }

        /// <summary>
             /// Active or inactive user by UserId
             /// </summary>
             /// <param name="UserId">Id of the User</param>
             /// <returns>Returns TRUE or FALSE</returns>
        public static bool ActiveInactiveUser(int UserId)
             {

                 try
                 {
                     if (UserId <= 0) return false;

                     UserDTO UserDTO = new UserDTO();
                     UserDTO = GetById(UserId);
                     if (UserDTO == null) return false;

                     if (UserDTO.IsActive == true)
                     {
                         UserDTO.IsActive = false;
                         Edit(UserDTO);
                         return true;
                     }
                     else
                     {
                         UserDTO.IsActive = true;
                         Edit(UserDTO);
                         return true;
                     }

                 }
                 catch
                 {
                     throw;
                 }
             }

        /// <summary>
             /// Get count of user by Name and Client Id
             /// </summary>
             /// <param name="Name">Name of the user</param>
             /// <param name="ClientId">Id of the client</param>
             /// <returns>Returns count of the User</returns>
        public static int GetUserByName(string Name, int ClientId)
             {

                 if (Name == null || Name == "") { return 0; }
                 try
                 {
                     int UserId = 0;
                     if (Name != null)
                     {
                         UnitOfWork uow = new UnitOfWork();
                         IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.Name.ToLower().Contains(Name.ToLower()) && e.ClientId == ClientId);
                         if (User != null)
                         {
                             foreach (var item in User)
                             {
                                 UserId = item.Id;
                                 return UserId;
                             }
                             return UserId;
                         }
                         else return UserId;
                     }
                     else return UserId;
                 }
                 catch
                 {
                     throw;
                 }
             }

        /// <summary>
             /// Get UserID comma separated string
             /// </summary>
             /// <param name="Name">Name of the User</param>
             /// <param name="ClientId">Id of the client</param>
             /// <returns>Returns UserId with comma separated string</returns>
        public static string GetUserIdarrayByName(string Name, int ClientId)
             {
                 string UserId = null;
                 if (Name == null || Name == "") { return null; }
                 try
                 {


                     if (Name != null)
                     {
                         UnitOfWork uow = new UnitOfWork();
                         IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.Name.ToLower().Contains(Name.ToLower()) && e.ClientId == ClientId);
                         int i = 0;
                         if (User != null)
                         {
                             foreach (var item in User)
                             {
                                 UserId = UserId + item.Id.ToString() + ",";
                                 //return UserId;
                             }
                             if (UserId != null)
                             {
                                 UserId = UserId.Remove(UserId.LastIndexOf(','));
                             }
                             return UserId;
                         }
                         else return UserId;
                     }
                     else return UserId;
                 }
                 catch
                 {
                     throw;
                 }
             }

         public static int GetAdminUserByClientId(int ClientId)
            {
                try
                {
                    UnitOfWork uow = new UnitOfWork();
                    User User = uow.UserRepo.GetAll().Where(e => e.UserType == 0 && e.ClientId == ClientId).FirstOrDefault();
                    return User.Id;
                }
                catch
                {
                    throw;
                }
            }

        #endregion

    }
}

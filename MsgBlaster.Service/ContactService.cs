using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Repo;
using MsgBlaster.Domain;


namespace MsgBlaster.Service
{
    public class ContactService
    {

        #region "CRUD Functionality"
       
        //Create Contact
        public static int Create(ContactDTO ContactDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = ContactDTO.ClientId;
                int PartnerId = ClientService.GetById(ContactDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;  

                var contact = new Contact();
                using (var uow = new UnitOfWork())
                {
                    contact = Transform.ContactToDomain(ContactDTO);

                    uow.ContactRepo.Insert(contact);
                    uow.SaveChanges();
                    if (ContactDTO.Groups != null)
                    {
                        foreach (var item in ContactDTO.Groups)
                        {
                            AddGroupToContact(contact.Id, item.Id);
                        }
                    }
                    return (contact.Id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Edit Contact
        public static void Edit(ContactDTO contactDTO)
        {
            try
            {

                ContactDTO ContactDTORemoveGroup = new ContactDTO();
                ContactDTORemoveGroup = GetById(contactDTO.Id);

                GlobalSettings.LoggedInClientId = contactDTO.ClientId;
                int PartnerId = ClientService.GetById(contactDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;  

                if (ContactDTORemoveGroup.Groups != null)
                {
                    foreach (var item in ContactDTORemoveGroup.Groups)
                    {
                        RemoveGroupFromContact(ContactDTORemoveGroup.Id, item.Id);
                    }
                }

                UnitOfWork uow = new UnitOfWork();
                Contact contact = Transform.ContactToDomain(contactDTO); ;
                uow.ContactRepo.Update(contact);


                if (contactDTO.Groups.Count != 0)
                {
                    //foreach (var item in contactDTO.Groups)
                    //{
                    //    RemoveGroupFromContact(contact.Id, item.Id);
                    //}

                    foreach (var item in contactDTO.Groups)
                    {
                        AddGroupToContact(contact.Id, item.Id);
                    }
                }

                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get Contact by Id
        public static ContactDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Contact Contact = uow.ContactRepo.GetById(Id, true);
                ContactDTO ContactDTO = Transform.ContactToDTO(Contact);

                List<GroupDTO> GroupDTOList = new List<GroupDTO>();

                GroupDTOList = GroupContactService.GetContactIdWiseGroups(Id);
                ContactDTO.Groups = GroupDTOList;

                //////List<GroupDTO> GroupDTO  = new List<GroupDTO>();              

                //////if (Contact.Groups.Count != 0)
                //////{
                //////    foreach (var item in Contact.Groups)
                //////    {
                //////        GroupDTO.Add(Transform.GroupToDTO(item));
                //////    }

                //////    ContactDTO.GroupList = GroupDTO;
                //////}

                //ContactDTO.FirstName = CommonService.GetFirstname(ContactDTO.Name);
                //string MiddleName = CommonService.GetMiddlename(ContactDTO.Name);
                //if (MiddleName != "")
                //{
                //    ContactDTO.LastName = MiddleName +" "+ CommonService.GetLastname(ContactDTO.Name);
                //}
                //else
                //{
                //    ContactDTO.LastName = CommonService.GetLastname(ContactDTO.Name);
                //}

                return ContactDTO;
            }
            catch
            {
                throw;
            }
        }

        //Delete Contact by Id
        public static void Delete(int Id)
        {
            try
            {
                ContactDTO ContactDTO = new ContactDTO();
                ContactDTO = GetById(Id);

                GlobalSettings.LoggedInClientId = ContactDTO.ClientId;
                int PartnerId = ClientService.GetById(ContactDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;  
                
                if (ContactDTO.Groups.Count > 0)
                {
                    foreach (var item in ContactDTO.Groups)
                    {
                        RemoveGroupFromContact(Id, item.Id);
                    }
                }

                UnitOfWork uow = new UnitOfWork();
                uow.ContactRepo.Delete(Id);
                uow.SaveChanges();

            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "List Functionality"
        
        public static List<ContactDTO> GetListByClientId(int ClientId)
        {

            List<ContactDTO> contactDTO = new List<ContactDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();

                if (Contact != null)
                {
                    foreach (var item in Contact)
                    {
                        contactDTO.Add(Transform.ContactToDTO(item));
                    }
                }

                return contactDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int GetListCountByClientId(int ClientId)
        {

            List<ContactDTO> contactDTO = new List<ContactDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();

                if (Contact != null)
                {
                    return Contact.Count();

                }

                return Contact.Count();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<ContactDTO> GetContactsbyClientId(int ClientId, string search, PagingInfo pagingInfo)
        {

            List<ContactDTO> contactDTO = new List<ContactDTO>();
            GroupDTO GroupDTO = new GroupDTO();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;
                IQueryable<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).AsQueryable();// ToList().Skip(skip).Take(take); //.OrderBy(e => e.Name)

                Contact = PagingService.Sorting<Contact>(Contact, pagingInfo.SortBy, pagingInfo.Reverse);
                Contact = Contact.Skip(skip).Take(take);
                if (Contact != null)
                {
                    if (search != "" && search != null)
                    {

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {
                            // string search

                            IQueryable<Contact> Contactsearch = uow.ContactRepo.GetAll().Where(e => (e.ClientId == ClientId) && ((e.Email != null ? (e.Email.ToLower().Contains(search.ToLower())) : false) || e.MobileNumber.Contains(search) || (e.Name != null ? (e.Name.ToLower().Contains(search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false))).AsQueryable();// .ToList().Skip(skip).Take(take); //.OrderBy(e => e.Name)
                            Contactsearch = PagingService.Sorting<Contact>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Contactsearch = Contactsearch.Skip(skip).Take(take);

                            if (Contactsearch != null)
                            {
                                foreach (var item in Contactsearch)
                                {
                                    ContactDTO ContactDTOObj = new ContactDTO();
                                    ContactDTOObj = GetById(item.Id);
                                    contactDTO.Add(ContactDTOObj);

                                    //ContactDTOObj =Transform.ContactToDTO(item);                                   
                                    // contactDTO.Add(Transform.ContactToDTO(item));

                                }

                            }

                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(search);
                            IQueryable<Contact> Contactsearch = uow.ContactRepo.GetAll().Where(e => (e.ClientId == ClientId) && e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).AsQueryable();//.OrderBy(e => e.Name);

                            Contactsearch = PagingService.Sorting<Contact>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Contactsearch = Contactsearch.Skip(skip).Take(take);

                            if (Contactsearch != null)
                            {
                                foreach (var item in Contactsearch)
                                {
                                    ContactDTO ContactDTOObj = new ContactDTO();
                                    ContactDTOObj = GetById(item.Id);
                                    contactDTO.Add(ContactDTOObj);
                                    // contactDTO.Add(Transform.ContactToDTO(item));

                                }
                            }
                            return contactDTO;

                        }

                    }
                    else
                    {
                        foreach (var item in Contact)
                        {
                            ContactDTO ContactDTOObj = new ContactDTO();
                            ContactDTOObj = GetById(item.Id);
                            contactDTO.Add(ContactDTOObj);
                            //contactDTO.Add(Transform.ContactToDTO(item));



                        }
                    }
                }

                return contactDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<ContactDTO> GetContactsbyGroupId(int ClientId, int GroupId, PagingInfo pagingInfo)
        {
            List<ContactDTO> contactDTO = new List<ContactDTO>();
            List<ContactDTO> contactDTOList = new List<ContactDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;
                contactDTOList = GroupContactService.GetGroupIdWisePresentContacts(pagingInfo);

                //GroupContactDTO GroupContactDTO = GroupService.GetGroupContactById(GroupId); 

                //IQueryable<ContactDTO> ContactDTOList = GroupContactDTO.Contacts.AsQueryable();

                IQueryable<ContactDTO> ContactDTOList = contactDTOList.AsQueryable();
                ContactDTOList = PagingService.Sorting<ContactDTO>(ContactDTOList, pagingInfo.SortBy, pagingInfo.Reverse);
                //ContactDTOList = ContactDTOList.Skip(skip).Take(take);


                //if (pagingInfo.Search != "" && pagingInfo.Search != null)
                //{

                //    bool IsDate = CommonService.IsDate(pagingInfo.Search);
                //    if (IsDate != true)
                //    {

                //        var Contactsearch = contactDTOList.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false)).OrderBy(e => e.Name);

                //        if (Contactsearch != null)
                //        {
                //            foreach (var item in Contactsearch)
                //            {
                //                ContactDTO ContactDTOObj = new ContactDTO();
                //                //ContactDTOObj = GetById(item.Id);
                //                contactDTO.Add(item);                                   

                //            }

                //        } return contactDTO;

                //    }
                //    else
                //    {
                //        //date wise search
                //        DateTime date = Convert.ToDateTime(pagingInfo.Search);
                //        var Contactsearch = contactDTOList.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).OrderBy(e => e.Name);

                //        if (Contactsearch != null)
                //        {
                //            foreach (var item in Contactsearch)
                //            {
                //                ContactDTO ContactDTOObj = new ContactDTO();
                //                //ContactDTOObj = GetById(item.Id);
                //                contactDTO.Add(item);


                //            }
                //        }
                //        return contactDTO;

                //    }

                //}


                //}

                return ContactDTOList.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int GetContactsCountbyGroupId(int ClientId, int GroupId, string Search)
        {
            List<ContactDTO> contactDTO = new List<ContactDTO>();
            List<ContactDTO> contactDTOList = new List<ContactDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();

                IEnumerable<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();

                if (Contact != null)
                {
                    foreach (var item in Contact)
                    {
                        ContactDTO ContactDTO = new ContactDTO();
                        ContactDTO = GetById(item.Id);

                        if (ContactDTO.Groups.Count() > 0)
                        {
                            foreach (var itemContactList in ContactDTO.Groups)
                            {
                                if (itemContactList.Id == GroupId)
                                {
                                    contactDTOList.Add(ContactDTO);
                                }
                            }
                        }

                    }


                    if (Search != "" && Search != null)
                    {

                        bool IsDate = CommonService.IsDate(Search);
                        if (IsDate != true)
                        {
                            // string search

                            var Contactsearch = contactDTOList.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(Search.ToLower())) : false) || e.MobileNumber.Contains(Search) || (e.Name != null ? (e.Name.ToLower().Contains(Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false));//.OrderBy(e => e.Name);

                            if (Contactsearch != null)
                            {
                                foreach (var item in Contactsearch)
                                {
                                    ContactDTO ContactDTOObj = new ContactDTO();
                                    //ContactDTOObj = GetById(item.Id);
                                    contactDTO.Add(item);

                                    //ContactDTOObj =Transform.ContactToDTO(item);                                   
                                    // contactDTO.Add(Transform.ContactToDTO(item));

                                }

                            } return contactDTO.Count();

                        }
                        else
                        {
                            //date wise search
                            DateTime date = Convert.ToDateTime(Search);
                            var Contactsearch = contactDTOList.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1));//.OrderBy(e => e.Name);

                            if (Contactsearch != null)
                            {
                                foreach (var item in Contactsearch)
                                {
                                    ContactDTO ContactDTOObj = new ContactDTO();
                                    //ContactDTOObj = GetById(item.Id);
                                    contactDTO.Add(item);
                                    // contactDTO.Add(Transform.ContactToDTO(item));

                                }
                            }
                            return contactDTO.Count();

                        }

                    }


                }

                return contactDTOList.Count();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static PageData<ContactDTO> GetContactPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            PageData<ContactDTO> pageList = new PageData<ContactDTO>();

            if (pagingInfo == null)
            {
                PagingInfo PagingInfoCreated = new PagingInfo();
                PagingInfoCreated.Page = 1;
                PagingInfoCreated.Reverse = false;
                PagingInfoCreated.ItemsPerPage = 1;
                PagingInfoCreated.Search = "";
                PagingInfoCreated.TotalItem = 0;
                PagingInfoCreated.GroupId = 0;
                pagingInfo = PagingInfoCreated;
            }

            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "Name";
            }



            if (pagingInfo.GroupId == 0)
            {

                ContactDTOList = GetContactsbyClientId(ClientId, pagingInfo.Search, pagingInfo);
            }
            else
            {

                ContactDTOList = GetContactsbyGroupId(ClientId, pagingInfo.GroupId, pagingInfo);

            }

            UnitOfWork uow = new UnitOfWork();
            IQueryable<ContactDTO> ContactDTOPagedList = ContactDTOList.AsQueryable();
            int count = 0;
            if (pagingInfo.GroupId == 0)
            {
                if (pagingInfo.Search != "" && pagingInfo.Search != null)
                {
                    bool IsDate = CommonService.IsDate(pagingInfo.Search);
                    if (IsDate != true)
                    {
                        count = 0;
                        count = uow.ContactRepo.GetAll().Where(e => (e.ClientId == ClientId) && ((e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false))).Count();//.OrderBy(e => e.Name).Count();
                    }
                    else
                    {
                        DateTime date = Convert.ToDateTime(pagingInfo.Search);
                        count = 0;
                        count = uow.ContactRepo.GetAll().Where(e => (e.ClientId == ClientId) && (e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1))).Count();//.OrderBy(e => e.Name);

                    }

                }
                else
                {
                    count = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).Count();
                }
            }
            else
            {

                count = GroupContactService.GetGroupIdWisePresentContactsCount(pagingInfo);// GroupService.GetGroupContactById(pagingInfo.GroupId).Contacts.Count(); // GetContactsCountbyGroupId(ClientId, pagingInfo.GroupId, pagingInfo.Search); // uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).Count(); //GetContactsCountbyGroupId(ClientId, pagingInfo.GroupId, pagingInfo.Search);

            }

            ////Sorting
            //if (pagingInfo.SortBy == "")            
            //{
            //    pagingInfo.SortBy = "Name";
            //}             
            //ContactDTOPagedList = PagingService.Sorting<ContactDTO>(ContactDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (ContactDTOPagedList.Count() > 0)
            {
                // var ContacDTOPerPage = PagingService.Paging<ContactDTO>(ContactDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;

                List<ContactDTO> pagedContactDTOList = new List<ContactDTO>();
                foreach (var item in ContactDTOPagedList)
                {
                    pagedContactDTOList.Add(item);
                }
                pageList.Data = pagedContactDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        public static PageData<ContactDTO> GetExcelContactPagedListbyClientId(PagingInfo pagingInfo, int ClientId, string FilePath, bool IsValid)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            PageData<ContactDTO> pageList = new PageData<ContactDTO>();

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

            ContactDTOList = ReadExcelFile(ClientId, FilePath, pagingInfo.Search, IsValid);
            pageList.FailureCount = GetInValidContactCount(ClientId, FilePath, pagingInfo.Search, false);
            pageList.SuccessCount = GetValidContactCount(ClientId, FilePath, pagingInfo.Search, true);



            IQueryable<ContactDTO> ContactDTOPagedList = ContactDTOList.AsQueryable();

            ////Sorting
            //CampaignDTOPagedList = PagingService.Sorting<CampaignDTO>(CampaignDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "Name";
            }
            ContactDTOPagedList = PagingService.Sorting<ContactDTO>(ContactDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (ContactDTOPagedList.Count() > 0)
            {
                List<ContactDTO> ContacDTOPerPage = new List<ContactDTO>();

                if (pagingInfo.Search == null || pagingInfo.Search == "")
                {
                    ContacDTOPerPage = PagingService.Paging<ContactDTO>(ContactDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page).ToList();

                }
                else
                {
                    ContacDTOPerPage = PagingService.Paging<ContactDTO>(ContactDTOPagedList, pagingInfo.ItemsPerPage, 1).ToList();

                }

                pageList.Count = ContactDTOPagedList.Count();

                List<ContactDTO> pagedContactDTOList = new List<ContactDTO>();

                foreach (var item in ContacDTOPerPage)
                {
                    pagedContactDTOList.Add(item);
                }
                pageList.Data = pagedContactDTOList;
            }
            else
            {
                pageList.Data = null;
            }





            return pageList;
        }

        public static List<ContactDTO> ReadExcelFile(int ClientId, string FilePath, string search, bool IsValid)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();

            ContactDTOList = CommonService.ReadExcelFile(ClientId, FilePath, IsValid).OrderBy(e => e.Name).ToList();
            if (ContactDTOList != null)
            {

                if (search != "" & search != null)
                {
                    bool Isdate = CommonService.IsDate(search);
                    if (Isdate != true)
                    {
                        List<ContactDTO> ContactDTOListNew = new List<ContactDTO>();
                        ContactDTOListNew = ContactDTOList.Where(e => (e.Name != null ? (e.Name.ToLower().Contains(search.ToLower())) : false) || (e.MobileNumber != null ? (e.MobileNumber.Contains(search)) : false)).ToList(); //.OrderBy(e => e.Name)
                        return ContactDTOListNew;
                    }
                    else
                    {
                        List<ContactDTO> ContactDTOListNew = new List<ContactDTO>();
                        DateTime date = Convert.ToDateTime(search);
                        ContactDTOListNew = ContactDTOList.Where(e => e.BirthDate >= date && e.BirthDate < date.AddDays(1) || e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1)).ToList(); //.OrderBy(e => e.Name)
                        return ContactDTOListNew;
                    }

                }


            }

            return ContactDTOList;
        }

        public static int GetValidContactCount(int ClientId, string FilePath, string search, bool IsValid)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            ContactDTOList = CommonService.ReadExcelFile(ClientId, FilePath, IsValid);
            return ContactDTOList.Where(e => e.IsValid == true).Count();
        }

        public static int GetInValidContactCount(int ClientId, string FilePath, string search, bool IsValid)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            ContactDTOList = CommonService.ReadExcelFile(ClientId, FilePath, IsValid);
            return ContactDTOList.Where(e => e.IsValid == false).Count();
        }

        public static List<ContactDTO> GetGroupIdWiseNotPresentContactsByClientId(int ClientId, int GroupId, PagingInfo pagingInfo)
        {
            List<ContactDTO> contactDTO = new List<ContactDTO>();
            List<ContactDTO> contactDTOList = new List<ContactDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                //IEnumerable<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();              
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                List<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();

                List<ContactDTO> contactDTOListNew = new List<ContactDTO>();
                //List<ContactDTO> contactDTOListWithoutGroup = new List<ContactDTO>();

                if (Contact != null)
                {
                    Group Group = uow.GroupRepo.GetById(GroupId); //, true
                    //List<ContactDTO> ContactDTO = new List<ContactDTO>();
                    //ContactDTO = GroupContactService.GetGroupIdWiseContacts(Group.Id);
                    int[] id;
                    id = GroupContactService.GetGroupIdWiseContactIdArray(Group.Id);
                    List<int> ContactIds = new List<int>();
                    ContactIds = id.ToList();
                    Contact.RemoveAll(a => ContactIds.Exists(w => w == a.Id));
                    //foreach (var item in id) //Group.Contacts
                    //{
                    //    Contact Contactn = new Contact();
                    //    Contactn.Id = item;
                    //    Contact.RemoveAll(e => e.Id == Contactn.Id);


                    //}
                    IQueryable<Contact> ContactList = Contact.AsQueryable();
                    ContactList = PagingService.Sorting<Contact>(ContactList, pagingInfo.SortBy, pagingInfo.Reverse);
                    ContactList = ContactList.Skip(skip).Take(take);
                    if (ContactList != null)
                    {
                        foreach (var item in ContactList)
                        {
                            contactDTOListNew.Add(Transform.ContactToDTO(item));
                        }

                        if (pagingInfo.Search != "" && pagingInfo.Search != null)
                        {
                            bool IsDate = CommonService.IsDate(pagingInfo.Search);
                            if (IsDate != true)
                            {
                                IQueryable<Contact> ContactSearch = Contact.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable();
                                ContactSearch = PagingService.Sorting<Contact>(ContactSearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                ContactSearch = ContactSearch.Skip(skip).Take(take);

                                List<ContactDTO> ContactDTOLst = new List<ContactDTO>();

                                foreach (var item in ContactSearch)
                                {
                                    ContactDTOLst.Add(Transform.ContactToDTO(item));
                                }
                                return ContactDTOLst.ToList();
                            }
                            else
                            {
                                DateTime date = Convert.ToDateTime(pagingInfo.Search);
                                IQueryable<Contact> ContactSearch = Contact.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).AsQueryable();//.OrderBy(e => e.Name);

                                ContactSearch = PagingService.Sorting<Contact>(ContactSearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                ContactSearch = ContactSearch.Skip(skip).Take(take);

                                List<ContactDTO> ContactDTOlst = new List<ContactDTO>();

                                foreach (var item in ContactSearch)
                                {
                                    ContactDTOlst.Add(Transform.ContactToDTO(item));
                                }
                                return ContactDTOlst.ToList();
                            }
                        }
                        return contactDTOListNew.ToList();

                    }

                    ////if (Contact.Count() != contactDTOList.Count())
                    ////{
                    ////    foreach (var item in Contact)
                    ////    {                                                        
                    ////        contactDTOListNew.Add(Transform.ContactToDTO(item));
                    ////    }

                    ////   GroupContactDTO GroupDTOtemp = GroupService.GetGroupContactById(GroupId);    
                    ////   //contactDTOListWithoutGroup = contactDTOListNew.Except(GroupDTOtemp.Contacts).ToList();
                    ////    foreach (var item in GroupDTOtemp.Contacts)
                    ////    {
                    ////        contactDTOListNew.RemoveAll(e => e.Id == item.Id);
                    ////    }

                    ////    if (pagingInfo.Search != "" && pagingInfo.Search != null)
                    ////    {

                    ////        bool IsDate = CommonService.IsDate(pagingInfo.Search);
                    ////        if (IsDate != true)
                    ////        {
                    ////            // string search

                    ////            var Contactsearch = contactDTOListNew.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false));//.OrderBy(e => e.Name)

                    ////            if (Contactsearch != null)
                    ////            {
                    ////                foreach (var item in Contactsearch)
                    ////                {
                    ////                    ContactDTO ContactDTOObj = new ContactDTO();                                        
                    ////                    contactDTO.Add(item);
                    ////                }
                    ////               IQueryable<ContactDTO> ContactDTO = contactDTO.AsQueryable();
                    ////               ContactDTO = ContactDTO.Skip(skip).Take(take);
                    ////               return ContactDTO.ToList();
                    ////            } return contactDTO.Skip(skip).Take(take).ToList();

                    ////        }
                    ////        else
                    ////        {
                    ////            //date wise search
                    ////            DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    ////            var Contactsearch = contactDTOListNew.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1));//.OrderBy(e => e.Name);

                    ////            if (Contactsearch != null)
                    ////            {
                    ////                foreach (var item in Contactsearch)
                    ////                {
                    ////                    ContactDTO ContactDTOObj = new ContactDTO();                                      
                    ////                    contactDTO.Add(item);            
                    ////                }

                    ////                IQueryable<ContactDTO> ContactDTO = contactDTO.AsQueryable();
                    ////                ContactDTO = ContactDTO.Skip(skip).Take(take);
                    ////                return ContactDTO.ToList();
                    ////            }
                    ////            return contactDTO.Skip(skip).Take(take).ToList();

                    ////        }

                    ////    }
                    ////    IQueryable<ContactDTO> ContactDTOnew = contactDTOListNew.AsQueryable();
                    ////    ContactDTOnew = ContactDTOnew.Skip(skip).Take(take);
                    ////    return ContactDTOnew.ToList();                        
                    ////}
                }

                return contactDTOList.Skip(skip).Take(take).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int GetGroupIdWiseNotPresentContactsCountByClientId(int ClientId, int GroupId, string Search)
        {
            List<ContactDTO> contactDTO = new List<ContactDTO>();
            List<ContactDTO> contactDTOList = new List<ContactDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();

                //IEnumerable<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();         
                List<GroupDTO> GroupDTO = new List<GroupDTO>();
                List<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();
                List<ContactDTO> contactDTOListNew = new List<ContactDTO>();

                Group Group = uow.GroupRepo.GetById(GroupId); //, true

                //List<ContactDTO> ContactDTO = new List<ContactDTO>();
                //ContactDTO = GroupContactService.GetGroupIdWiseContacts(Group.Id);
                //foreach (var item in ContactDTO) //Group.Contacts
                //{
                //    Contact.RemoveAll(e => e.Id == item.Id);
                //}

                int[] id;
                id = GroupContactService.GetGroupIdWiseContactIdArray(Group.Id);
                List<int> ContactIds = new List<int>();
                ContactIds = id.ToList();
                Contact.RemoveAll(a => ContactIds.Exists(w => w == a.Id));
                //foreach (var item in id) //Group.Contacts
                //{
                //    Contact Contactn = new Contact();
                //    Contactn.Id = item;
                //    Contact.RemoveAll(e => e.Id == Contactn.Id);
                //}



                IQueryable<Contact> ContactList = Contact.AsQueryable();

                if (ContactList != null)
                {

                    if (Search != "" && Search != null)
                    {
                        bool IsDate = CommonService.IsDate(Search);
                        if (IsDate != true)
                        {
                            IQueryable<Contact> ContactSearch = Contact.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(Search.ToLower())) : false) || e.MobileNumber.Contains(Search) || (e.Name != null ? (e.Name.ToLower().Contains(Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false)).AsQueryable();



                            return ContactSearch.Count();
                        }
                        else
                        {
                            DateTime date = Convert.ToDateTime(Search);
                            IQueryable<Contact> ContactSearch = Contact.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).AsQueryable();//.OrderBy(e => e.Name);

                            List<ContactDTO> ContactDTOlst = new List<ContactDTO>();

                            //foreach (var item in ContactList)
                            //{
                            //    ContactDTOlst.Add(Transform.ContactToDTO(item));
                            //}
                            return ContactSearch.Count();
                        }
                    }
                    return ContactList.Count();


                }

                return ContactList.Count();



                ////if (Contact != null)
                ////{
                ////    List<ContactDTO> contactDTOListNew = new List<ContactDTO>();
                ////    if (Contact.Count() != contactDTOList.Count())
                ////    {
                ////        foreach (var item in Contact)
                ////        {
                ////            contactDTOListNew.Add(Transform.ContactToDTO(item));
                ////        }
                ////        GroupContactDTO GroupDTOtemp = GroupService.GetGroupContactById(GroupId);
                ////        foreach (var item in GroupDTOtemp.Contacts)
                ////        {
                ////            contactDTOListNew.RemoveAll(e => e.Id == item.Id);
                ////        }

                ////        if (Search != "" && Search != null)
                ////        {

                ////            bool IsDate = CommonService.IsDate(Search);
                ////            if (IsDate != true)
                ////            {
                ////                // string search

                ////                var Contactsearch = contactDTOListNew.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(Search.ToLower())) : false) || e.MobileNumber.Contains(Search) || (e.Name != null ? (e.Name.ToLower().Contains(Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false));//.OrderBy(e => e.Name);

                ////                if (Contactsearch != null)
                ////                {
                ////                    foreach (var item in Contactsearch)
                ////                    {
                ////                        ContactDTO ContactDTOObj = new ContactDTO();
                ////                        //ContactDTOObj = GetById(item.Id);
                ////                        contactDTO.Add(item);

                ////                        //ContactDTOObj =Transform.ContactToDTO(item);                                   
                ////                        // contactDTO.Add(Transform.ContactToDTO(item));

                ////                    }

                ////                } return contactDTO.Count();

                ////            }
                ////            else
                ////            {
                ////                //date wise search
                ////                DateTime date = Convert.ToDateTime(Search);
                ////                var Contactsearch = contactDTOListNew.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1));//.OrderBy(e => e.Name);

                ////                if (Contactsearch != null)
                ////                {
                ////                    foreach (var item in Contactsearch)
                ////                    {
                ////                        ContactDTO ContactDTOObj = new ContactDTO();
                ////                        //ContactDTOObj = GetById(item.Id);
                ////                        contactDTO.Add(item);
                ////                        // contactDTO.Add(Transform.ContactToDTO(item));

                ////                    }
                ////                }
                ////                return contactDTO.Count();

                ////            }

                ////        }
                ////        return contactDTOListNew.Count();
                ////    }
                ////}               

                //// return contactDTOList.Count();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<ContactDTO> GetGroupIdWisePresentContactsByClientId(int ClientId, int GroupId, PagingInfo pagingInfo)
        {

            List<ContactDTO> contactDTOList = new List<ContactDTO>();
            List<ContactDTO> contactDTO = new List<ContactDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;
                //IEnumerable<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();
                //var contacts = from c in Contact  where ( from g in c.Groups where g.Id ==GroupId select g ).Any() select c;


                Group Group = new Group();
                Group = uow.GroupRepo.GetById(GroupId); //, true
                IQueryable<ContactDTO> Contact = GroupContactService.GetGroupIdWiseContacts(Group.Id).AsQueryable();// Group.Contacts.AsQueryable();
                Contact = PagingService.Sorting<ContactDTO>(Contact, pagingInfo.SortBy, pagingInfo.Reverse);
                //Contact = Contact.Skip(skip).Take(take);
                if (Contact != null)
                {
                    foreach (var item in Contact)
                    {
                        ContactDTO ContactDTOobj = new ContactDTO();
                        ContactDTOobj = item;// Transform.ContactToDTO(item);
                        contactDTOList.Add(ContactDTOobj);
                    }

                    if (pagingInfo.Search != "" && pagingInfo.Search != null)
                    {
                        bool IsDate = CommonService.IsDate(pagingInfo.Search);
                        if (IsDate != true)
                        {
                            IQueryable<ContactDTO> Contactsearch = Contact.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable();
                            //Group.Contacts
                            Contactsearch = PagingService.Sorting<ContactDTO>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Contactsearch = Contactsearch.Skip(skip).Take(take);
                            foreach (var item in Contactsearch)
                            {
                                contactDTO.Add(item); //Transform.ContactToDTO(item)
                            }
                            return contactDTO;
                        }
                        else
                        {
                            DateTime date = Convert.ToDateTime(pagingInfo.Search);
                            IQueryable<ContactDTO> Contactsearch = Contact.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).AsQueryable(); // .Skip(skip).Take(take); //.OrderBy(e => e.Name)
                            //Group.Contacts
                            Contactsearch = PagingService.Sorting<ContactDTO>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Contactsearch = Contact.Skip(skip).Take(take);
                            foreach (var item in Contactsearch)
                            {
                                contactDTO.Add(item); //Transform.ContactToDTO(item)
                            }
                            return contactDTO;
                        }
                    }
                    else

                        return contactDTOList.Skip(skip).Take(take).ToList();
                }
                else
                {
                    //foreach (var item in Contact)
                    //{
                    //    contactDTOList.Add(Transform.ContactToDTO(item));
                    //}
                    return contactDTOList.Skip(skip).Take(take).ToList();
                }





                //GroupContactDTO GroupContactDTO = GroupService.GetGroupContactById(pagingInfo.GroupId);// uow.GroupRepo.GetAll().Where(e => e.ClientID == ClientId).ToList();
                //IQueryable<ContactDTO> ContactDTO = GroupContactDTO.Contacts.AsQueryable();// .Skip(skip).Take(take).ToList();


                //ContactDTO = PagingService.Sorting<ContactDTO>(ContactDTO, pagingInfo.SortBy, pagingInfo.Reverse);
                //ContactDTO = ContactDTO.Skip(skip).Take(take);


                //if (ContactDTO != null)
                //{
                //    contactDTOList = ContactDTO.ToList() ; 
                //    if (pagingInfo.Search != "" && pagingInfo.Search != null)
                //    {
                //        bool IsDate = CommonService.IsDate(pagingInfo.Search);
                //        if (IsDate != true)
                //        {
                //            // string search
                //            IQueryable<ContactDTO> Contactsearch = contactDTOList.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable();// .Skip(skip).Take(take); //.OrderBy(e => e.Name)
                //            Contactsearch = PagingService.Sorting<ContactDTO>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                //            Contactsearch = Contactsearch.Skip(skip).Take(take);

                //            if (Contactsearch != null)
                //            { 
                //                foreach (var item in Contactsearch)
                //                {
                //                    ContactDTO ContactDTOObj = new ContactDTO();                         
                //                    contactDTO.Add(item);
                //                }
                //            } return contactDTO;

                //        }
                //        else
                //        {
                //            //date wise search
                //            DateTime date = Convert.ToDateTime(pagingInfo.Search);
                //            IQueryable<ContactDTO> Contactsearch = contactDTOList.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).AsQueryable(); // .Skip(skip).Take(take); //.OrderBy(e => e.Name)
                //            Contactsearch = PagingService.Sorting<ContactDTO>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                //            Contactsearch = Contactsearch.Skip(skip).Take(take);
                //            if (Contactsearch != null)
                //            {
                //                foreach (var item in Contactsearch)
                //                {
                //                    ContactDTO ContactDTOObj = new ContactDTO();                                   
                //                    contactDTO.Add(item);         
                //                }
                //            }
                //            return contactDTO;
                //        }
                //    }
                //}
                //return contactDTOList ;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int GetGroupIdWisePresentContactsCountByClientId(int ClientId, int GroupId, string Search)
        {

            List<ContactDTO> contactDTOList = new List<ContactDTO>();
            List<ContactDTO> contactDTO = new List<ContactDTO>();
            try
            {

                UnitOfWork uow = new UnitOfWork();
                Group Group = new Group();
                Group = uow.GroupRepo.GetById(GroupId); //, true
                List<ContactDTO> Contact = GroupContactService.GetGroupIdWiseContacts(Group.Id);// Group.Contacts; //Contact

                if (Contact != null)
                {
                    if (Search != "" && Search != null)
                    {
                        bool IsDate = CommonService.IsDate(Search);
                        if (IsDate != true)
                        {
                            List<ContactDTO> Contactsearch = Contact.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(Search.ToLower())) : false) || e.MobileNumber.Contains(Search) || (e.Name != null ? (e.Name.ToLower().Contains(Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false)).ToList(); //.OrderBy(e => e.Name)
                            //Group.Contacts
                            if (Contactsearch != null)
                            {
                                return Contactsearch.Count();
                            }
                        }
                        else
                        {
                            DateTime date = Convert.ToDateTime(Search);
                            List<ContactDTO> Contactsearch = Contact.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).ToList();//.OrderBy(e => e.Name);
                            //Group.Contacts
                            if (Contactsearch != null)
                            {
                                return Contactsearch.Count();
                            }
                        }
                    }
                    return Contact.Count();
                }
                return Contact.Count();

                ////var contacts = from c in Contact  where ( from g in c.Groups where g.Id ==GroupId select g ).Any() select c;
                //GroupContactDTO GroupContactDTO = GroupService.GetGroupContactById(GroupId);// uow.GroupRepo.GetAll().Where(e => e.ClientID == ClientId).ToList();
                //List<ContactDTO> ContactDTO = GroupContactDTO.Contacts;
                //contactDTOList= ContactDTO;

                // if (contactDTOList != null)              
                //  {                   

                //      if (Search != "" && Search != null)
                //      {

                //          bool IsDate = CommonService.IsDate(Search);
                //          if (IsDate != true)
                //          {
                //              // string search

                //              var Contactsearch = contactDTOList.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(Search.ToLower())) : false) || e.MobileNumber.Contains(Search) || (e.Name != null ? (e.Name.ToLower().Contains(Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false)); //.OrderBy(e => e.Name)

                //              if (Contactsearch != null)
                //              {
                //                  foreach (var item in Contactsearch)
                //                  {
                //                      ContactDTO ContactDTOObj = new ContactDTO();                                  
                //                      contactDTO.Add(item);
                //                  }

                //              } return contactDTO.Count();

                //          }
                //          else
                //          {
                //              //date wise search
                //              DateTime date = Convert.ToDateTime(Search);
                //              var Contactsearch = contactDTOList.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1));//.OrderBy(e => e.Name);

                //              if (Contactsearch != null)
                //              {
                //                  foreach (var item in Contactsearch)
                //                  {
                //                      ContactDTO ContactDTOObj = new ContactDTO();                                  
                //                      contactDTO.Add(item);        
                //                  }
                //              }
                //              return contactDTO.Count();
                //          }
                //      }
                //  }
                //  return contactDTOList.Count();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static PageData<ContactDTO> GetGroupIdWiseNotPresentContactsPagedListByClientId(PagingInfo pagingInfo, int ClientId)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            PageData<ContactDTO> pageList = new PageData<ContactDTO>();

            if (pagingInfo == null)
            {
                PagingInfo PagingInfoCreated = new PagingInfo();
                PagingInfoCreated.Page = 1;
                PagingInfoCreated.Reverse = false;
                PagingInfoCreated.ItemsPerPage = 1;
                PagingInfoCreated.Search = "";
                PagingInfoCreated.TotalItem = 0;
                PagingInfoCreated.GroupId = 0;
                pagingInfo = PagingInfoCreated;
            }

            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "Name";
            }

            if (pagingInfo.GroupId == 0)
            {
                ContactDTOList = GetContactsbyClientId(ClientId, pagingInfo.Search, pagingInfo);
            }
            else
            {
                ContactDTOList = GetGroupIdWiseNotPresentContactsByClientId(ClientId, pagingInfo.GroupId, pagingInfo);

            }
            IQueryable<ContactDTO> ContactDTOPagedList = ContactDTOList.AsQueryable();


            UnitOfWork uow = new UnitOfWork();

            int count = 0;
            if (pagingInfo.GroupId == 0)
            {
                if (pagingInfo.Search != "" && pagingInfo.Search != null)
                {
                    bool IsDate = CommonService.IsDate(pagingInfo.Search);
                    if (IsDate != true)
                    {
                        count = 0;
                        count = uow.ContactRepo.GetAll().Where(e => (e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).Count(); //.OrderBy(e => e.Name)
                    }
                    else
                    {
                        DateTime date = Convert.ToDateTime(pagingInfo.Search);
                        count = 0;
                        count = uow.ContactRepo.GetAll().Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).Count();//.OrderBy(e => e.Name)

                    }

                }
                else
                {
                    count = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).Count();
                }
            }
            else
            {

                count = GetGroupIdWiseNotPresentContactsCountByClientId(ClientId, pagingInfo.GroupId, pagingInfo.Search);

            }


            ////Sorting       
            ContactDTOPagedList = PagingService.Sorting<ContactDTO>(ContactDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);


            // paging
            if (ContactDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<ContactDTO>(ContactDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);

                pageList.Count = count;

                int TotalContact = GetListCountByClientId(ClientId);
                pageList.SuccessCount = GroupContactService.GetGroupIdWiseContactsCount(pagingInfo.GroupId, ""); //GroupService.GetGroupContactById(pagingInfo.GroupId).Contacts.Count();
                pageList.FailureCount = TotalContact - pageList.SuccessCount;

                if (pageList.FailureCount < 0)
                {
                    pageList.FailureCount = 0;
                }

                List<ContactDTO> pagedContactDTOList = new List<ContactDTO>();
                foreach (var item in ContactDTOPagedList)
                {
                    pagedContactDTOList.Add(item);
                }
                pageList.Data = pagedContactDTOList;
            }
            else
            {
                int TotalContact = GetListCountByClientId(ClientId);
                pageList.SuccessCount = GroupContactService.GetGroupIdWiseContactsCount(pagingInfo.GroupId, ""); // GroupService.GetGroupContactById(pagingInfo.GroupId).Contacts.Count();

                pageList.FailureCount = TotalContact - pageList.SuccessCount;
                if (pageList.FailureCount < 0)
                {
                    pageList.FailureCount = 0;
                }
                pageList.Data = null;
            }



            return pageList;
        }

        public static PageData<ContactDTO> GetGroupIdWisePresentContactsPagedListByClientId(PagingInfo pagingInfo, int ClientId)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            PageData<ContactDTO> pageList = new PageData<ContactDTO>();

            if (pagingInfo == null)
            {
                PagingInfo PagingInfoCreated = new PagingInfo();
                PagingInfoCreated.Page = 1;
                PagingInfoCreated.Reverse = false;
                PagingInfoCreated.ItemsPerPage = 1;
                PagingInfoCreated.Search = "";
                PagingInfoCreated.TotalItem = 0;
                PagingInfoCreated.GroupId = 0;
                pagingInfo = PagingInfoCreated;
            }


            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "Name";
            }
            if (pagingInfo.GroupId == 0)
            {
                ContactDTOList = GetContactsbyClientId(ClientId, pagingInfo.Search, pagingInfo);
            }
            else
            {
                ContactDTOList = GroupContactService.GetGroupIdWisePresentContacts(pagingInfo);// GetGroupIdWisePresentContactsByClientId(ClientId, pagingInfo.GroupId, pagingInfo);

            }
            IQueryable<ContactDTO> ContactDTOPagedList = ContactDTOList.AsQueryable();


            UnitOfWork uow = new UnitOfWork();

            int count = 0;
            if (pagingInfo.GroupId == 0)
            {
                if (pagingInfo.Search != "" && pagingInfo.Search != null)
                {
                    bool IsDate = CommonService.IsDate(pagingInfo.Search);
                    if (IsDate != true)
                    {
                        count = 0;
                        count = uow.ContactRepo.GetAll().Where(e => (e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).Count(); //.OrderBy(e => e.Name)
                    }
                    else
                    {
                        DateTime date = Convert.ToDateTime(pagingInfo.Search);
                        count = 0;
                        count = uow.ContactRepo.GetAll().Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).Count();//.OrderBy(e => e.Name)

                    }

                }
                else
                {
                    count = 0;
                    count = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId).Count();
                }
            }
            else
            {
                count = 0;
                count = GroupContactService.GetGroupIdWiseContactsCount(pagingInfo.GroupId, pagingInfo.Search);// GetGroupIdWisePresentContactsCountByClientId(ClientId, pagingInfo.GroupId, pagingInfo.Search);

            }


            ////Sorting
            //ContactDTOPagedList = PagingService.Sorting<ContactDTO>(ContactDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);
            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "Name";
            }
            ContactDTOPagedList = PagingService.Sorting<ContactDTO>(ContactDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);


            // paging
            if (ContactDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<ContactDTO>(ContactDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);


                pageList.Count = count;

                int TotalContact = GetListCountByClientId(ClientId);
                pageList.SuccessCount = GroupContactService.GetGroupIdWiseContactsCount(pagingInfo.GroupId, ""); //GroupService.GetGroupContactById(pagingInfo.GroupId).Contacts.Count();
                pageList.FailureCount = TotalContact - pageList.SuccessCount;
                if (pageList.FailureCount < 0)
                {
                    pageList.FailureCount = 0;
                }
                List<ContactDTO> pagedContactDTOList = new List<ContactDTO>();
                foreach (var item in ContactDTOPagedList)
                {
                    pagedContactDTOList.Add(item);
                }
                pageList.Data = pagedContactDTOList;
            }
            else
            {
                int TotalContact = GetListCountByClientId(ClientId);//.Count();
                pageList.SuccessCount = GroupContactService.GetGroupIdWiseContactsCount(pagingInfo.GroupId, ""); // GroupService.GetGroupContactById(pagingInfo.GroupId).Contacts.Count();
                pageList.FailureCount = TotalContact - pageList.SuccessCount;
                if (pageList.FailureCount < 0)
                {
                    pageList.FailureCount = 0;
                }
                pageList.Data = null;
            }



            return pageList;
        }

        public static List<ContactDTO> GetContactsForTodaysBirthday()
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            try
            {
                var today = System.DateTime.Now.Date;
                UnitOfWork uow = new UnitOfWork();
                IList<Contact> Contacts = uow.ContactRepo.GetAll().Where(c => c.BirthDate != null && 
                    (Convert.ToDateTime(c.BirthDate).Month == DateTime.Now.Month) && 
                     Convert.ToDateTime(c.BirthDate).Day == DateTime.Now.Day).ToList();

                if (Contacts != null)
                {
                    foreach (var item in Contacts)
                    {
                        ContactDTOList.Add(Transform.ContactToDTO(item));
                    }
                }

                return ContactDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<ContactDTO> GetContactsForTodaysAnniversary()
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            try
            {
                var today = System.DateTime.Now.Date;
                UnitOfWork uow = new UnitOfWork();
                IList<Contact> Contacts = uow.ContactRepo.GetAll().Where(c => c.AnniversaryDate != null &&
                    (Convert.ToDateTime(c.AnniversaryDate).Month == DateTime.Now.Month) &&
                     Convert.ToDateTime(c.AnniversaryDate).Day == DateTime.Now.Day).ToList();

                if (Contacts != null)
                {
                    foreach (var item in Contacts)
                    {
                        ContactDTOList.Add(Transform.ContactToDTO(item));
                    }
                }

                return ContactDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
        
        #region "Other Functionality"

        public static bool SearchMobileNumber(string MobileNumber, int ClientId, int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.MobileNumber == MobileNumber && e.ClientId == ClientId && e.Id != Id);

                if (Contact.ToList().Count > 0)
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

        public static void AddGroupToContact(int ContactId, int GroupId)
        {
            try
            {
                //UnitOfWork uow = new UnitOfWork();
                //uow.ContactRepo.AddGroup(ContactId, GroupId);               
                //uow.SaveChanges();
                GroupContactDTO GroupContactDTO = new GroupContactDTO();
                GroupContactDTO.ContactId = ContactId;
                GroupContactDTO.GroupId = GroupId;

                GlobalSettings.LoggedInClientId = ContactService.GetById(ContactId).ClientId;// ContactDTO.ClientId;
                int PartnerId = ClientService.GetById(Convert.ToInt32(GlobalSettings.LoggedInClientId)).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId; 

                GroupContactService.Create(GroupContactDTO);
            }
            catch
            {
                throw;
            }
        }

        public static void RemoveGroupFromContact(int ContactId, int GroupId)
        {
            try
            {
                //UnitOfWork uow = new UnitOfWork();
                //uow.ContactRepo.RemoveGroup(GroupId, ContactId);
                //uow.SaveChanges();

                GroupContactDTO GroupContactDTO = new GroupContactDTO();
                GroupContactDTO.ContactId = ContactId;
                GroupContactDTO.GroupId = GroupId;

                GlobalSettings.LoggedInClientId = ContactService.GetById(ContactId).ClientId;// ContactDTO.ClientId;
                int PartnerId = ClientService.GetById(Convert.ToInt32(GlobalSettings.LoggedInClientId)).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId; 

                GroupContactService.DeleteByGroupId(GroupContactDTO.GroupId, GroupContactDTO.ContactId);
            }
            catch
            {
                throw;
            }
        }

        public static ContactDTO GetContactByMobileNumberAndClientId(string MobileNumber, int ClientId)
        {
            try
            {
                ContactDTO ContactDTO = new ContactDTO();
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Contact> Contact = uow.ContactRepo.GetAll().Where(e => e.MobileNumber == MobileNumber && e.ClientId == ClientId);

                if (Contact.ToList().Count > 0)
                {
                    foreach (var item in Contact)
                    {
                        //ContactDTO.Id = item.Id;
                        //ContactDTO.Name = item.Name;
                        ContactDTO = Transform.ContactToDTO(item);
                    }
                    return ContactDTO;

                }
                else return ContactDTO;
            }
            catch
            {
                throw;
            }
        }

        public static bool ImportContactsFromExcelFile(int ClientId, List<ContactDTO> ContactDTOList) //int GroupId,
        {
            //List<ContactDTO> ContactDTOListNew = new List<ContactDTO>();
            //ContactDTOListNew = ContactDTOList;

            bool IsImported = false;
            try
            {
                ContactDTO ContactDTO = new ContactDTO();
                GroupDTO GroupDTO = new GroupDTO();
                // Create Group 
                GroupDTO.Name = "MyGroup " + string.Format("{0:G}", System.DateTime.Now);
                GroupDTO.ClientID = ClientId;
                int GroupId = GroupService.Create(GroupDTO);

                if (GroupId != 0)
                {
                    GroupDTO = GroupService.GetById(GroupId);
                }
                else
                {
                    GroupDTO = null;
                }

                foreach (var item in ContactDTOList)
                {

                    ContactDTO = item;
                    ContactDTO.ClientId = ClientId;
                    if (ContactDTO.Groups == null && GroupDTO != null)
                    {
                        ContactDTO.Groups = new List<GroupDTO>();
                        ContactDTO.Groups.Add(GroupDTO);
                    }

                    bool IsMobileExist = SearchMobileNumber(ContactDTO.MobileNumber, ClientId, ContactDTO.Id);
                    if (IsMobileExist != true)
                    {
                        //Insert Contact information to Contact.
                        try
                        {
                            if (ContactDTO.IsValid == true)
                            {
                                ////Check Contact Present or not
                                //ContactDTO ContactDTOpresent = new ContactDTO();
                                //ContactDTOpresent = ContactService.GetContactByMobileNumberAndClientId(ContactDTO.MobileNumber, ClientId);
                                if (ContactDTO.Id == 0)
                                {
                                    int ContactId = Create(ContactDTO);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    IsImported = true;
                }

                GroupDTO GroupDTOTemp = new GroupDTO();
                GroupDTOTemp.ContactCount = GroupContactService.GetGroupIdWiseContactsCount(GroupId, "");
                if (GroupDTOTemp.ContactCount == 0)
                {
                    GroupService.Delete(GroupId);
                }

                return IsImported;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static string GetContactIdarrayByName(string Contact, int ClientId)
        {
            string ContactId = null;
            if (Contact == null || Contact == "")
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Contact> ContactList = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId);
                if (ContactList != null)
                {
                    foreach (var item in ContactList)
                    {
                        ContactId = ContactId + item.Id.ToString() + ",";
                        //return UserId;
                    }
                    if (ContactId != null)
                    {
                        ContactId = ContactId.Remove(ContactId.LastIndexOf(','));
                    }

                    return ContactId;
                }
                else return ContactId;
            }
            try
            {






                if (Contact != null)
                {
                    UnitOfWork uow = new UnitOfWork();
                    bool IsDate = CommonService.IsDate(Contact);
                    if (IsDate != true)
                    {
                        IEnumerable<Contact> ContactList = uow.ContactRepo.GetAll().Where(e => (e.Email != null ? (e.Email.ToLower().Contains(Contact.ToLower())) : false) || e.MobileNumber.Contains(Contact) || (e.Name != null ? (e.Name.ToLower().Contains(Contact.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(Contact.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(Contact.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(Contact.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(Contact.ToLower())) : false) && e.ClientId == ClientId);
                        if (ContactList != null)
                        {
                            foreach (var item in ContactList)
                            {
                                ContactId = ContactId + item.Id.ToString() + ",";
                                //return UserId;
                            }
                            if (ContactId != null)
                            {
                                ContactId = ContactId.Remove(ContactId.LastIndexOf(','));
                            }

                            return ContactId;
                        }
                        else return ContactId;
                    }
                    else
                    {
                        DateTime date = Convert.ToDateTime(Contact);
                        IEnumerable<Contact> ContactList = uow.ContactRepo.GetAll().Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1));//.OrderBy(e => e.Name);
                        if (ContactList != null)
                        {
                            foreach (var item in ContactList)
                            {
                                ContactId = ContactId + item.Id.ToString() + ",";
                                //return UserId;
                            }
                            if (ContactId != null)
                            {
                                ContactId = ContactId.Remove(ContactId.LastIndexOf(','));
                            }

                            return ContactId;
                        }
                        else return ContactId;
                    }

                }
                else return ContactId;
            }
            catch
            {
                throw;
            }
        }

        public static string GetAllReceipentNumberByClientId(int ClientId)
        {
            string MobileNumber = null;

            UnitOfWork uow = new UnitOfWork();
            IEnumerable<Contact> ContactList = uow.ContactRepo.GetAll().Where(e => e.ClientId == ClientId);
            if (ContactList != null)
            {
                foreach (var item in ContactList)
                {
                    MobileNumber = MobileNumber + item.MobileNumber.ToString() + ",";
                    //return UserId;
                }
                if (MobileNumber != null)
                {
                    MobileNumber = MobileNumber.Remove(MobileNumber.LastIndexOf(','));
                }

                return MobileNumber;
            }
            else return MobileNumber;

        }

        #endregion

    }
}

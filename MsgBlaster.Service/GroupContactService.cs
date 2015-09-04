using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;
using System.Web;


namespace MsgBlaster.Service
{
    public class GroupContactService
    {

        #region "CRUD Functionality"
        
        //Create group contact
        public static GroupContactDTO Create(GroupContactDTO GroupContactDTO)
        {
            try
            {
                var GroupContact = new GroupContact();

                UnitOfWork uow = new UnitOfWork();
                GroupContact = Transform.GroupContactNewToDomain(GroupContactDTO);
                uow.GroupContactRepo.Insert(GroupContact);

                uow.SaveChanges();
                GroupContact.Id = GroupContact.Id;
                return Transform.GroupContactNewToDTO(GroupContact);

            }

            catch (Exception)
            {
                throw;
            }
        }

        //Get groupc ontact details by id
        public static GroupContactDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                GroupContact GroupContact = uow.GroupContactRepo.GetById(Id);
                GroupContactDTO GroupContactDTO = Transform.GroupContactNewToDTO(GroupContact);
                return GroupContactDTO;
            }
            catch
            {
                throw;
            }
        }

        //Delete group contact by group id and contact id 
        public static bool DeleteByGroupId(int GroupId, int ContactId)
        {

            try
            {

                GroupContactDTO GroupContactDTO = new GroupContactDTO();
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<GroupContact> GroupContact = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == GroupId && e.ContactId == ContactId);
                if (GroupContact != null)
                {
                    foreach (var item in GroupContact)
                    {
                        //GroupContactDTO = GetById(item.Id);
                        uow.GroupContactRepo.Delete(item.Id);
                        uow.SaveChanges();
                    }
                }
                return true;
            }
            catch
            {

                throw;
            }
        }

        //Delete group contact by contact id and agroup id
        public static bool DeleteByContactId(int ContactId, int GroupId)
        {

            try
            {

                GroupContactDTO GroupContactDTO = new GroupContactDTO();
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<GroupContact> GroupContact = uow.GroupContactRepo.GetAll().Where(e => e.ContactId == ContactId && e.GroupId == GroupId);
                if (GroupContact != null)
                {
                    foreach (var item in GroupContact)
                    {
                        //GroupContactDTO = GetById(item.Id);
                        uow.GroupContactRepo.Delete(item.Id);
                        uow.SaveChanges();
                    }
                }
                return true;
            }
            catch
            {

                throw;
            }
        }
        
        #endregion

        #region "List Functionality"

        //Get contact present list as per group id
        public static List<ContactDTO> GetGroupIdWisePresentContacts(PagingInfo pagingInfo)
        {
            List<GroupContactDTO> GroupContactDTOList = new List<GroupContactDTO>();
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            List<Contact> ContactList = new List<Contact>();

            int ClientId = 0;
            if (pagingInfo.GroupId != 0)
            {
                ClientId = GroupService.GetById(pagingInfo.GroupId).ClientID;
            }

            try
            {
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                // string search
                //string ContactIdStringall = ContactService.GetContactIdarrayByName(pagingInfo.Search, ClientId);

                UnitOfWork uow = new UnitOfWork();
                IQueryable<GroupContact> GroupContact = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == pagingInfo.GroupId).AsQueryable(); //&& (ContactIdStringall != null ? (e.ContactId.ToString().Split(',').Any(ContactId => ContactIdStringall.Contains(ContactId.ToString()))) : false)
                //GroupContact = PagingService.Sorting<GroupContact>(GroupContact, pagingInfo.SortBy, pagingInfo.Reverse);              
                GroupContact = GroupContact.Skip(skip).Take(take);
                if (GroupContact != null)
                {
                    foreach (var item in GroupContact)
                    {
                        Contact Contact = new Contact();
                        Contact = uow.ContactRepo.GetById(item.ContactId);
                        ContactList.Add(Contact);
                        //ContactDTO ContactDTO = new ContactDTO();
                        //ContactDTO = ContactService.GetById(item.ContactId);
                        //ContactDTOList.Add(ContactDTO);                            
                    }

                    if (ContactList != null)
                    {
                        if (pagingInfo.Search != "" && pagingInfo.Search != null)
                        {
                            List<ContactDTO> ContactDTOSearchList = new List<ContactDTO>();

                            bool IsDate = CommonService.IsDate(pagingInfo.Search);
                            if (IsDate != true)
                            {

                                // string search
                                string ContactIdString = ContactService.GetContactIdarrayByName(pagingInfo.Search, ClientId);
                                IQueryable<GroupContact> GroupContactSearch = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == pagingInfo.GroupId && (ContactIdString != null ? (e.ContactId.ToString().Split(',').Any(ContactId => ContactIdString.Contains(ContactId.ToString()))) : false)).AsQueryable();
                                //GroupContact = PagingService.Sorting<GroupContact>(GroupContact, pagingInfo.SortBy, pagingInfo.Reverse);              
                                GroupContactSearch = GroupContactSearch.Skip(skip).Take(take);
                                if (GroupContactSearch != null)
                                {
                                    List<Contact> ContactListSearch = new List<Contact>();
                                    foreach (var item in GroupContactSearch)
                                    {
                                        Contact Contact = new Contact();
                                        Contact = uow.ContactRepo.GetById(item.ContactId);
                                        ContactListSearch.Add(Contact);
                                    }

                                    IQueryable<Contact> Contactsearch = ContactListSearch.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable();//.OrderBy(e => e.Name);
                                    Contactsearch = PagingService.Sorting<Contact>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                    Contactsearch = Contactsearch.Skip(skip).Take(take);
                                    if (Contactsearch != null)
                                    {
                                        foreach (var item in Contactsearch)
                                        {
                                            ContactDTOSearchList.Add(Transform.ContactToDTO(item));
                                        }

                                    } return ContactDTOSearchList.Skip(skip).Take(take).ToList();

                                }


                            }
                            else
                            {
                                //date wise search
                                string ContactIdString = ContactService.GetContactIdarrayByName(pagingInfo.Search, ClientId);

                                IQueryable<GroupContact> GroupContactSearch = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == pagingInfo.GroupId && (ContactIdString != null ? (e.ContactId.ToString().Split(',').Any(ContactId => ContactIdString.Contains(ContactId.ToString()))) : false)).AsQueryable();
                                //GroupContact = PagingService.Sorting<GroupContact>(GroupContact, pagingInfo.SortBy, pagingInfo.Reverse);              
                                GroupContactSearch = GroupContactSearch.Skip(skip).Take(take);
                                if (GroupContactSearch != null)
                                {
                                    List<Contact> ContactListSearch = new List<Contact>();
                                    foreach (var item in GroupContactSearch)
                                    {
                                        Contact Contact = new Contact();
                                        Contact = uow.ContactRepo.GetById(item.ContactId);
                                        ContactListSearch.Add(Contact);
                                    }


                                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                                    IQueryable<Contact> Contactsearch = ContactListSearch.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).AsQueryable();//.OrderBy(e => e.Name);
                                    Contactsearch = PagingService.Sorting<Contact>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                    Contactsearch = Contactsearch.Skip(skip).Take(take);
                                    if (Contactsearch != null)
                                    {
                                        foreach (var item in Contactsearch)
                                        {
                                            ContactDTOSearchList.Add(Transform.ContactToDTO(item));

                                        }
                                    }
                                    return ContactDTOSearchList.Skip(skip).Take(take).ToList();
                                }
                            }
                        }

                        //ContactList = ContactList.Skip(skip).Take(take).ToList();
                        foreach (var item in ContactList)
                        {
                            ContactDTOList.Add(Transform.ContactToDTO(item));
                        }
                    }


                    return ContactDTOList;// ContactDTOList.Skip(skip).Take(take).ToList();
                }


                return ContactDTOList.Skip(skip).Take(take).ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }

        //Get contact paged list by client id 
        public static PageData<ContactDTO> GetContactPagedListbyClientId(PagingInfo pagingInfo)
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




            ContactDTOList = GetGroupIdWisePresentContacts(pagingInfo);


            UnitOfWork uow = new UnitOfWork();
            IQueryable<ContactDTO> ContactDTOPagedList = ContactDTOList.AsQueryable();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = GetGroupIdWisePresentContactsCount(pagingInfo);
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = GetGroupIdWisePresentContactsCount(pagingInfo);

                }

            }
            else
            {
                count = GetGroupIdWisePresentContactsCount(pagingInfo);
            }


            ////Sorting               
            ContactDTOPagedList = PagingService.Sorting<ContactDTO>(ContactDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

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

        //Get contact list as per group id
        public static List<ContactDTO> GetGroupIdWiseContacts(int GroupId)
        {
            List<GroupContactDTO> GroupContactDTOList = new List<GroupContactDTO>();
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            List<Contact> ContactList = new List<Contact>();

            try
            {

                UnitOfWork uow = new UnitOfWork();
                IEnumerable<GroupContact> GroupContact = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == GroupId).AsQueryable();


                if (GroupContact != null)
                {
                    foreach (var item in GroupContact)
                    {
                        Contact Contact = new Contact();
                        Contact = uow.ContactRepo.GetById(item.ContactId);
                        ContactList.Add(Contact);
                    }

                    if (ContactList != null)
                    {
                        foreach (var item in ContactList)
                        {
                            ContactDTOList.Add(Transform.ContactToDTO(item));
                        }
                    }

                    return ContactDTOList;// ContactDTOList.Skip(skip).Take(take).ToList();
                }
                return ContactDTOList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        //Get group list as per contact id
        public static List<GroupDTO> GetContactIdWiseGroups(int ContactId)
        {
            List<GroupContactDTO> GroupContactDTOList = new List<GroupContactDTO>();
            List<GroupDTO> GroupDTOList = new List<GroupDTO>();
            List<Group> GroupList = new List<Group>();

            try
            {

                UnitOfWork uow = new UnitOfWork();
                IEnumerable<GroupContact> GroupContact = uow.GroupContactRepo.GetAll().Where(e => e.ContactId == ContactId).AsQueryable();


                if (GroupContact != null)
                {
                    foreach (var item in GroupContact)
                    {
                        Group Group = new Group();
                        Group = uow.GroupRepo.GetById(item.GroupId);
                        GroupList.Add(Group);
                    }

                    if (GroupList != null)
                    {
                        foreach (var item in GroupList)
                        {
                            GroupDTOList.Add(Transform.GroupToDTO(item));
                        }
                    }

                    return GroupDTOList;// ContactDTOList.Skip(skip).Take(take).ToList();
                }
                return GroupDTOList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region "Other Functionality"
        //Get present contact count as per group id
        public static int GetGroupIdWisePresentContactsCount(PagingInfo pagingInfo)
        {
            List<GroupContactDTO> GroupContactDTOList = new List<GroupContactDTO>();
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();

            List<Contact> ContactList = new List<Contact>();

            try
            {
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                UnitOfWork uow = new UnitOfWork();
                IQueryable<GroupContact> GroupContact = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == pagingInfo.GroupId).AsQueryable();
                //GroupContact = PagingService.Sorting<GroupContact>(GroupContact, pagingInfo.SortBy, pagingInfo.Reverse);

                if (GroupContact != null)
                {
                    //foreach (var item in GroupContact)
                    //{
                    //    Contact Contact = new Contact();
                    //    Contact = uow.ContactRepo.GetById(item.ContactId);
                    //    ContactList.Add(Contact);
                    //}

                    if (ContactList != null)
                    {
                        if (pagingInfo.Search != "" && pagingInfo.Search != null)
                        {
                            List<ContactDTO> ContactDTOSearchList = new List<ContactDTO>();

                            bool IsDate = CommonService.IsDate(pagingInfo.Search);
                            if (IsDate != true)
                            {
                                // string search

                                IQueryable<Contact> Contactsearch = ContactList.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.MobileNumber.Contains(pagingInfo.Search) || (e.Name != null ? (e.Name.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable();//.OrderBy(e => e.Name);
                                Contactsearch = PagingService.Sorting<Contact>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);

                                if (Contactsearch != null)
                                {
                                    return Contactsearch.Count();

                                }

                            }
                            else
                            {
                                //date wise search
                                DateTime date = Convert.ToDateTime(pagingInfo.Search);
                                IQueryable<Contact> Contactsearch = ContactList.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).AsQueryable();//.OrderBy(e => e.Name);
                                Contactsearch = PagingService.Sorting<Contact>(Contactsearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                if (Contactsearch != null)
                                {
                                    return Contactsearch.Count();
                                }


                            }
                        }
                    }
                    return GroupContact.Count();

                }


                return GroupContact.Count();
            }
            catch (Exception)
            {
                throw;
            }

        }

        //Get group idwise contact count
        public static int GetGroupIdWiseContactsCount(int GroupId, string Search)
        {
            List<GroupContactDTO> GroupContactDTOList = new List<GroupContactDTO>();
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            List<Contact> ContactList = new List<Contact>();

            int ClientId = 0;
            if (GroupId != 0)
            {
                ClientId = GroupService.GetById(GroupId).ClientID;
            }

            try
            {

                UnitOfWork uow = new UnitOfWork();
                IEnumerable<GroupContact> GroupContact = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == GroupId);


                if (GroupContact != null)
                {
                    //foreach (var item in GroupContact)
                    //{
                    //    Contact Contact = new Contact();
                    //    Contact = uow.ContactRepo.GetById(item.ContactId);
                    //    ContactList.Add(Contact);
                    //}

                    if (Search != "" && Search != null)
                    {
                        List<ContactDTO> ContactDTOSearchList = new List<ContactDTO>();

                        bool IsDate = CommonService.IsDate(Search);
                        if (IsDate != true)
                        {

                            // string search
                            string ContactIdString = ContactService.GetContactIdarrayByName(Search, ClientId);
                            IQueryable<GroupContact> GroupContactSearch = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == GroupId && (ContactIdString != null ? (e.ContactId.ToString().Split(',').Any(ContactId => ContactIdString.Contains(ContactId.ToString()))) : false)).AsQueryable();

                            if (GroupContactSearch != null)
                            {
                                //List<Contact> ContactListSearch = new List<Contact>();
                                //foreach (var item in GroupContactSearch)
                                //{
                                //    Contact Contact = new Contact();
                                //    Contact = uow.ContactRepo.GetById(item.ContactId);
                                //    ContactListSearch.Add(Contact);
                                //}

                                //IQueryable<Contact> Contactsearch = ContactListSearch.Where(e => (e.Email != null ? (e.Email.ToLower().Contains(Search.ToLower())) : false) || e.MobileNumber.Contains(Search) || (e.Name != null ? (e.Name.ToLower().Contains(Search.ToLower())) : false) || (e.FirstName != null ? (e.FirstName.ToLower().Contains(Search.ToLower())) : false) || (e.LastName != null ? (e.LastName.ToLower().Contains(Search.ToLower())) : false) || (e.AnniversaryDate.ToString() != null ? (Convert.ToDateTime(e.AnniversaryDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false) || (e.BirthDate.ToString() != null ? (Convert.ToDateTime(e.BirthDate).ToString("dd-MMM-yyyy").ToLower().Contains(Search.ToLower())) : false)).AsQueryable();//.OrderBy(e => e.Name);

                                //if (Contactsearch != null)
                                //{
                                //    foreach (var item in Contactsearch)
                                //    {
                                //        ContactDTOSearchList.Add(Transform.ContactToDTO(item));
                                //    }

                                //} 
                                return GroupContactSearch.Count();// ContactDTOSearchList.Skip(skip).Take(take).ToList();

                            } return GroupContactSearch.Count();


                        }
                        else
                        {
                            //date wise search
                            string ContactIdString = ContactService.GetContactIdarrayByName(Search, ClientId);

                            IQueryable<GroupContact> GroupContactSearch = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == GroupId && (ContactIdString != null ? (e.ContactId.ToString().Split(',').Any(ContactId => ContactIdString.Contains(ContactId.ToString()))) : false)).AsQueryable();

                            if (GroupContactSearch != null)
                            {
                                //List<Contact> ContactListSearch = new List<Contact>();
                                //foreach (var item in GroupContactSearch)
                                //{
                                //    Contact Contact = new Contact();
                                //    Contact = uow.ContactRepo.GetById(item.ContactId);
                                //    ContactListSearch.Add(Contact);
                                //}


                                //DateTime date = Convert.ToDateTime(Search);
                                //IQueryable<Contact> Contactsearch = ContactListSearch.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1)).AsQueryable();//.OrderBy(e => e.Name);

                                //if (Contactsearch != null)
                                //{
                                //    foreach (var item in Contactsearch)
                                //    {
                                //        ContactDTOSearchList.Add(Transform.ContactToDTO(item));

                                //    }
                                //}
                                return GroupContactSearch.Count();// ContactDTOSearchList.Skip(skip).Take(take).ToList();
                            }
                            return GroupContactSearch.Count();
                        }
                    }




                    return GroupContact.Count();// ContactDTOList.Skip(skip).Take(take).ToList();
                }
                return GroupContact.Count();
            }
            catch (Exception)
            {
                throw;
            }

        }

        //Get group id wise Cotact id array
        public static int[] GetGroupIdWiseContactIdArray(int GroupId)
        {
            List<GroupContactDTO> GroupContactDTOList = new List<GroupContactDTO>();
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            List<Contact> ContactList = new List<Contact>();
            int[] id = new int[0];
            try
            {


                UnitOfWork uow = new UnitOfWork();
                IEnumerable<GroupContact> GroupContact = uow.GroupContactRepo.GetAll().Where(e => e.GroupId == GroupId).AsQueryable();


                if (GroupContact != null)
                {
                    int i = 0;
                    string ids = null;
                    id = new int[GroupContact.Count()];
                    foreach (var item in GroupContact)
                    {
                        //Contact Contact = new Contact();
                        //Contact = uow.ContactRepo.GetById(item.ContactId);
                        //ContactList.Add(Contact);
                        id[i] = item.ContactId;


                        // item.Id.ToString();
                        i++;
                    }

                    //if (ContactList != null)
                    //{
                    //    foreach (var item in ContactList)
                    //    {
                    //        ContactDTOList.Add(Transform.ContactToDTO(item));
                    //    }
                    //}

                    // return ContactDTOList;// ContactDTOList.Skip(skip).Take(take).ToList();
                    return id;
                }
                return id; // ContactDTOList;
            }
            catch (Exception)
            {
                throw;
            }

        }
        
        #endregion

        
    }
}

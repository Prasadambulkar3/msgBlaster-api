using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;

namespace MsgBlaster.Service
{
    public class GroupService
    {
        #region "CRUD Functionality"

        //Create group
        public static int Create(GroupDTO GroupDTO)
        {
            

            try
            {

                GlobalSettings.LoggedInClientId = GroupDTO.ClientID;                 
                int PartnerId = ClientService.GetById(GroupDTO.ClientID).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                var group = new Group();

                UnitOfWork uow = new UnitOfWork();
                GroupDTO.ClientID = GroupDTO.ClientID;
                group = Transform.GroupToDomain(GroupDTO);
                uow.GroupRepo.Insert(group);
                
                uow.SaveChanges();


                GroupDTO.Id = group.Id;
                return GroupDTO.Id;

            }

            catch (Exception)
            {
                throw;
            }
        }

        //Edit group
        public static void Edit(GroupDTO GroupDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = GroupDTO.ClientID;
                int PartnerId = ClientService.GetById(GroupDTO.ClientID).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                UnitOfWork uow = new UnitOfWork();
                Group Group = Transform.GroupToDomain(GroupDTO);
                uow.GroupRepo.Update(Group);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Edit Group Contact
        public static void EditGroupContact(GroupContactDTO GroupContactDTO)
        {
            try
            {
                bool IsDeleted;
                GroupContactDTO GroupDTORemoveContact = new GroupContactDTO();
                //GroupDTORemoveContact = GetGroupContactById(GroupContactDTO.Id);
                

                if (GroupContactDTO.UnwantedContacts != null)
                {
                    

                    foreach (var item in GroupContactDTO.UnwantedContacts)
                    {
                        try
                        {
                            IsDeleted = GroupContactService.DeleteByGroupId(GroupContactDTO.Id, item.Id);
                            //RemoveContactFromGroup(item.Id, GroupContactDTO.Id);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                }

                //if (GroupDTORemoveContact.Contacts.Count() > 0)
                //{
                //    foreach (var item in GroupDTORemoveContact.Contacts)
                //    {

                //        RemoveContactFromGroup(item.Id, GroupContactDTO.Id);
                //    }
                //}


                if (GroupContactDTO.Contacts != null)
                {
                    foreach (var item in GroupContactDTO.Contacts)
                    {
                          int contactCount = 0;
                     
                        if (item.Groups.Count != 0)
                        {
                            foreach (var grp in item.Groups)
                            {
                                if(grp.Id == GroupContactDTO.GroupId){

                                    contactCount++;
                                }
                            }
                        
                        }

                        if (contactCount == 0)
                        {
                            GroupContactDTO GroupContactDTOCreate = new GroupContactDTO();
                            GroupContactDTOCreate.GroupId = GroupContactDTO.Id;
                            GroupContactDTOCreate.ContactId = item.Id;
                            GroupContactService.Create(GroupContactDTOCreate);
                           
                        }
                    }

              }


                //uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Delete group by id
        public static bool Delete(int id)
        {
            bool IsCampaignExists = IsChildEntityExistInCampaign(id);
            bool IsCouponCampaignExists = IsChildEntityExistInEcouponCampaign(id);

           
            try
            {
                if (IsCampaignExists != true && IsCouponCampaignExists != true)
                {
                    UnitOfWork uow = new UnitOfWork();
                    uow.GroupRepo.Delete(id);
                    uow.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch
            {
                throw;
            }
        }

        //Get group details by id
        public static GroupDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Group Group = uow.GroupRepo.GetById(Id);
                GroupDTO GroupDTO = Transform.GroupToDTO(Group);
                return GroupDTO;
            }
            catch
            {
                throw;
            }
        }

        //Get group details with contact count
        public static GroupDTO GetByIdWithContactCount(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Group Group = uow.GroupRepo.GetById(Id, true);
                GroupDTO GroupDTO = Transform.GroupToDTO(Group);
                GroupDTO.ContactCount = GroupContactService.GetGroupIdWiseContactsCount(Id, "");// Group.Contacts.Count();
                return GroupDTO;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "List Functionality"

        //Get group list by client id
        public static List<GroupDTO> GetGroupListByClientId(int ClientId)
        {

            List<GroupDTO> GroupDTOList = new List<GroupDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Group> Group = uow.GroupRepo.GetAll().Where(e => e.ClientID == ClientId).OrderBy(e => e.Name).ToList();
                    if (Group != null)
                    {
                        //GroupDTO GroupDTOAllContact = new GroupDTO();
                        //GroupDTOAllContact.Id = 0;
                        //GroupDTOAllContact.Name = "All Contacts";
                        //string ReceipentsNumber = ContactService.GetAllReceipentNumberByClientId(ClientId);
                        //int TotalContactCount = CommonService.GetRecipientsCount(ReceipentsNumber);
                        //GroupDTOAllContact.ContactCount = TotalContactCount;
                        //GroupDTOList.Add(GroupDTOAllContact);

                        foreach (var item in Group)
                        {
                            GroupDTO GroupDTO = new GroupDTO();
                            GroupDTO = Transform.GroupToDTO(item);
                            Group objGroup = uow.GroupRepo.GetById(item.Id);//, true
                            //GroupContactDTO GroupContactDTO = new GroupContactDTO();
                            //GroupContactDTO = GetGroupContactById(GroupDTO.Id);
                            List<ContactDTO> ContactDTO = new List<ContactDTO>();
                            int ContactCount = GroupContactService.GetGroupIdWiseContactsCount(objGroup.Id, "");

                            GroupDTO.ContactCount = ContactCount;// objGroup.Contacts.Count();//GroupContactDTO.Contacts.Count(); //
                            GroupDTOList.Add(GroupDTO);
                        }
                    }
                }
               


                return GroupDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception ex)
            {

                throw;
            }
        }

        //get group list by client id
        public static List<GroupDTO> GetGroupListByClientIdForCampaign(int ClientId)
        {

            List<GroupDTO> GroupDTOList = new List<GroupDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Group> Group = uow.GroupRepo.GetAll().Where(e => e.ClientID == ClientId).OrderBy(e => e.Name).ToList();
                    if (Group != null)
                    {
                        GroupDTO GroupDTOAllContact = new GroupDTO();
                        GroupDTOAllContact.Id = 0;
                        GroupDTOAllContact.Name = "All Contacts";
                        string ReceipentsNumber = ContactService.GetAllReceipentNumberByClientId(ClientId);
                        int TotalContactCount = CommonService.GetRecipientsCount(ReceipentsNumber);
                        GroupDTOAllContact.ContactCount = TotalContactCount;
                        GroupDTOList.Add(GroupDTOAllContact);

                        foreach (var item in Group)
                        {
                            GroupDTO GroupDTO = new GroupDTO();
                            GroupDTO = Transform.GroupToDTO(item);
                            Group objGroup = uow.GroupRepo.GetById(item.Id);//, true
                            //GroupContactDTO GroupContactDTO = new GroupContactDTO();
                            //GroupContactDTO = GetGroupContactById(GroupDTO.Id);
                            List<ContactDTO> ContactDTO = new List<ContactDTO>();
                            int ContactCount = GroupContactService.GetGroupIdWiseContactsCount(objGroup.Id, "");

                            GroupDTO.ContactCount = ContactCount;// objGroup.Contacts.Count();//GroupContactDTO.Contacts.Count(); //
                            GroupDTOList.Add(GroupDTO);
                        }
                    }
                }



                return GroupDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception ex)
            {

                throw;
            }
        }

        // Returns group list which have clients 
        public static List<GroupContactDTO> GetGroupListWithContactPresentByClientId(int ClientId)
        {

            List<GroupContactDTO> GroupDTOList = new List<GroupContactDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Group> Group = uow.GroupRepo.GetAll().Where(e => e.ClientID == ClientId).OrderBy(e => e.Name).ToList();
                    if (Group != null)
                    {
                        foreach (var item in Group)
                        {
                            GroupDTO GroupDTO = new GroupDTO();
                            GroupDTO = GetById(item.Id);
                            GroupContactDTO GroupContactDTO = new GroupContactDTO();
                            //GroupContactDTO = GetGroupContactById(GroupDTO.Id);
                            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
                            ContactDTOList = GroupContactService.GetGroupIdWiseContacts(GroupDTO.Id);// GroupDTOTemp.Id

                            GroupContactDTO.ContactCount = ContactDTOList.Count();// GroupContactDTO.Contacts.Count();
                            if (ContactDTOList.Count() > 0) // GroupContactDTO.Contacts.Count                               
                            {
                                GroupDTOList.Add(GroupContactDTO);// (Transform.GroupContactToDTO(item));                               
                            }
                        }
                    }
                }

                return GroupDTOList;
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

        #endregion

        #region "Other Functionality"

        //Get group by group name , client id and group id
        public static bool GetByNameAndClientId(string Name, int ClientId, int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Group> Group = uow.GroupRepo.GetAll().Where(e => e.Name.ToLower() == Name.ToLower() && e.ClientID == ClientId && e.Id != Id);
                //ClientDTO ClientDTO = Transform.ClientToDTO(Client);
                if (Group.ToList().Count > 0)
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

        //Get group details by group name and client id 
        public static GroupDTO GetGroupByNameAndClientId(string Name, int ClientId)
        {
            try
            {
                GroupDTO GroupDTO = new GroupDTO();
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Group> Group = uow.GroupRepo.GetAll().Where(e => e.Name.ToLower() == Name.ToLower() && e.ClientID == ClientId);
                //ClientDTO ClientDTO = Transform.ClientToDTO(Client);
                if (Group.ToList().Count > 0)
                {
                    foreach (var item in Group)
                    {
                        
                        GroupDTO = Transform.GroupToDTO(item);
                        return GroupDTO;
                    }
                    return GroupDTO;
                }
                else return GroupDTO;
            }
            catch
            {
                throw;
            }
        }

        //Add contact to group contact by contact id and group id
        public static void AddContactToGroup(int ContactId, int GroupId)
        {
            try
            {
                //UnitOfWork uow = new UnitOfWork();
                //uow.GroupRepo.AddContact(GroupId, ContactId );
                //uow.SaveChanges();

                GroupContactDTO GroupContactDTO = new GroupContactDTO();
                GroupContactDTO.ContactId = ContactId;
                GroupContactDTO.GroupId = GroupId;
                GroupContactService.Create(GroupContactDTO);


            }
            catch
            {
                throw;
            }
        }

        //Delete contact from group using contact id and group id
        public static void RemoveContactFromGroup(int ContactId, int GroupId)
        {
            try
            {
                //UnitOfWork uow = new UnitOfWork();
                //uow.GroupRepo.RemoveContact(GroupId, ContactId);
                //uow.SaveChanges();

                GroupContactDTO GroupContactDTO = new GroupContactDTO();
                GroupContactDTO.ContactId = ContactId;
                GroupContactDTO.GroupId = GroupId;
                GroupContactService.DeleteByContactId(GroupContactDTO.ContactId, GroupContactDTO.GroupId);
            }
            catch
            {
                throw;
            }
        }     

        //Check is child entity exist in campaign by using group id
        public static bool IsChildEntityExistInCampaign(int GroupId) //, int ClientId
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.GroupId == GroupId); //&& e.ClientId == ClientId
                if (Campaign.ToList().Count > 0)
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

        //Check is child entity exist in ecoupon campaign
        public static bool IsChildEntityExistInEcouponCampaign(int GroupId) //, int ClientId
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.GroupId == GroupId); //&& e.ClientId == ClientId
                if (EcouponCampaign.ToList().Count > 0)
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

        #endregion

        #region "Unwantaed Code"

        //public static void DeleteGroupContact(int id)
        //{
        //    List<GroupContactDTO> GroupDTOList = new List<GroupContactDTO>();
        //    List<ContactDTO> ContactDTOList = new List<ContactDTO>();

        //    GroupContactDTO GroupDTO = new GroupContactDTO();
        //    GroupDTO = GetGroupContactById(id);




        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        uow.GroupRepo.Delete(id);

        //        if (GroupDTO.Contacts  != null)
        //        {
        //            foreach (var item in GroupDTO.Contacts)
        //            {
        //                RemoveContactFromGroup(item.Id, id);
        //            }
        //        }


        //        uow.SaveChanges();


        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public static GroupContactDTO GetGroupContactById(int Id)
            //{
        //    try
        //    {
        //        //UnitOfWork uow = new UnitOfWork();
        //        //Group Group = uow.GroupRepo.GetById(Id, true);
        //        //GroupContactDTO GroupDTO = Transform.GroupContactToDTO(Group);
        //        //return GroupDTO;
        //        GroupContactDTO GroupContactDTO = new GroupContactDTO();
        //        GroupContactDTO = 


        //        return GroupContactDTO;

        //    }
        //    catch
        //    {
        //        throw;
        //    }
            //}





        // Returns Client wise list
        //public static int CreateGroupContact(GroupContactDTO GroupContactDTO)
        //{
        //    try
        //    {
        //        var group = new Group();

        //        UnitOfWork uow = new UnitOfWork();
        //        GroupContactDTO.ClientID = GroupContactDTO.ClientID;
        //        group = Transform.GroupContactToDomain(GroupContactDTO);
        //        uow.GroupRepo.Insert(group);

        //        uow.SaveChanges();

        //        if (GroupContactDTO.Contacts != null)
        //        {
        //            foreach (var item in GroupContactDTO.Contacts)
        //            {
        //                AddContactToGroup(item.Id, GroupContactDTO.Id);
        //            }
        //        }

        //        GroupContactDTO.Id = group.Id;
        //        return GroupContactDTO.Id;

        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        #endregion

    }
}

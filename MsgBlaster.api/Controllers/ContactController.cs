using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MsgBlaster.DTO;
using MsgBlaster.Service;

namespace MsgBlaster.api.Controllers
{
    public class ContactController : ApiController
    {
        #region "CRUD Functionality"
       
        public int CreateContact(ContactDTO contactDTO)
        {
            try
            {
                return (ContactService.Create(contactDTO));
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public ContactDTO GetById(int Id)
        {
            ContactDTO ContactDTO = new ContactDTO();
            try
            {
                ContactDTO = (ContactService.GetById(Id));
                return ContactDTO;
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public void EditContact(ContactDTO contactDTO)
        {
            try
            {
                ContactService.Edit(contactDTO);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public void RemoveContact(int id)
        {
            try
            {
                ContactService.Delete(id);
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public void AddGroup(int ContactId, int GroupId)
        {
            try
            {
                ContactService.AddGroupToContact(ContactId, GroupId);
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public void RemoveGroup(int ContactId, int GroupId)
        {
            try
            {
                ContactService.RemoveGroupFromContact(ContactId, GroupId);
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        #endregion
        
        #region "List Functionality"

        public List<ContactDTO> GetContactsbyClientId(int clientId)
        {
            try
            {
                return (ContactService.GetListByClientId(clientId));
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        [HttpPost]
        public PageData<ContactDTO> GetContactPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            try
            {
                return ContactService.GetContactPagedListbyClientId(pagingInfo, ClientId);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        [HttpPost]
        public PageData<ContactDTO> GetExcelContactPagedListbyClientId(PagingInfo pagingInfo, int ClientId, string FilePath, bool IsValid)
        {
            try
            {
                return ContactService.GetExcelContactPagedListbyClientId(pagingInfo, ClientId, FilePath, IsValid);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        [HttpPost]
        public PageData<ContactDTO> GetGroupIdWiseNotPresentContactsPagedListByClientId(PagingInfo pagingInfo, int ClientId)
        {
            try
            {
                return ContactService.GetGroupIdWiseNotPresentContactsPagedListByClientId(pagingInfo, ClientId);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        [HttpPost]
        public PageData<ContactDTO> GetGroupIdWisePresentContactsPagedListByClientId(PagingInfo pagingInfo, int ClientId)
        {
            try
            {
                return ContactService.GetGroupIdWisePresentContactsPagedListByClientId(pagingInfo, ClientId);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        #endregion

        #region "Other Functionality"

        public bool GetContact(string mobileNumber, int ClientId, int Id)
        {
            try
            {
                return (ContactService.SearchMobileNumber(mobileNumber, ClientId, Id));
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public ContactDTO GetContactByMobileNumberAndClientId(string MobileNumber, int ClientId)
        {
            try
            {
                return ContactService.GetContactByMobileNumberAndClientId(MobileNumber, ClientId);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        [HttpPost]
        public bool ImprortAllContactsByClientIdAndFilePath(int ClientId, string FilePath)//int DocumentId, int GroupId
        {
            try
            {
                List<ContactDTO> ContactDTOList = new List<ContactDTO>();
                ContactDTOList = CommonService.ReadExcelFile(ClientId, FilePath, true);
                bool result = ContactService.ImportContactsFromExcelFile(ClientId, ContactDTOList); //GroupId,
                //if (result)
                //{
                // DocumentService.Delete(DocumentId, FilePath);                    
                //}

                return result;
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        #endregion

        #region "Unwanted code"
       
        //public List<ContactDTO> GetContactsbyGroupId(int ClientId, int GroupId)
        //{
        //    try
        //    {
        //        return (ContactService.GetContactsbyGroupId(ClientId, GroupId));
        //    }
        //    catch (Exception)
        //    {
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        //        {
        //            Content = new StringContent("An error occurred, please try again or contact the administrator."),
        //            ReasonPhrase = "Critical Exception"
        //        });
        //    }
        //}

        //public List<ContactDTO> GetSearchContactsbyClientId(int clientId, string search)
        //{
        //    try
        //    {
        //        return (ContactService.GetContactsbyClientId(clientId, search));
        //    }
        //    catch (Exception)
        //    {
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        //        {
        //            Content = new StringContent("An error occurred, please try again or contact the administrator."),
        //            ReasonPhrase = "Critical Exception"
        //        });
        //    }
        //}
        
        #endregion
    
    }
}

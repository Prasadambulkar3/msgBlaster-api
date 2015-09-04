using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MsgBlaster.DTO;
using MsgBlaster.Service;
namespace MsgBlaster.api.Controllers
{
    public class GroupController : ApiController
    {
        #region "CRUD Functionality"

        public int CreateGroup(GroupDTO oGroupDTO)
        {

            try
            {
                return (GroupService.Create(oGroupDTO));
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public GroupDTO GetById(int Id)
        {

            try
            {
                return (GroupService.GetById(Id));
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public bool RemoveGroup(int Id)
        {

            try
            {
                return GroupService.Delete(Id);

            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {

                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public void EditGroup(GroupDTO GroupDTO)
        {

            try
            {
                GroupService.Edit(GroupDTO);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public void EditGroupContact(GroupContactDTO GroupContactDTO)
        {
            try
            {
                GroupService.EditGroupContact(GroupContactDTO);
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
        public List<GroupDTO> GetGroupListByClientId(string accessId, int ClientId)
        {
            try
            {
                return GroupService.GetGroupListByClientId(ClientId);
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

        public List<GroupContactDTO> GetGroupListWithContactPresentByClientId(int ClientId)
        {
            try
            {
                return GroupService.GetGroupListWithContactPresentByClientId(ClientId);
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

        public List<GroupDTO> GetGroupListByClientIdForCampaign(string accessId, int ClientId)
        {
            try
            {
                return GroupService.GetGroupListByClientIdForCampaign(ClientId);
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
       
        public bool GetGroupByNameAndClientId(string accessId, string Name, int ClientId, int Id)
        {
            try
            {
                return GroupService.GetByNameAndClientId(Name, ClientId, Id);
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

        //public GroupContactDTO GetGroupContactById(int Id)
        //{
        //    try
        //    {
        //       return GroupService.GetGroupContactById(Id);
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

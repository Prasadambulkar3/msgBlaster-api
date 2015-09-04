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
    public class UserController : ApiController
    {
        #region "CRUD Functionality"

        public UserDTO CreateUser(string accessId, UserDTO UserDTO)
        {

            try
            {
                return UserService.Create(UserDTO);
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

        public void EditUser(string accessId, UserDTO UserDTO)
        {
            try
            {
                UserService.Edit(UserDTO);
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

        public void RemoveUser(string accessId, int ClientUserId)
        {
            try
            {
                UserService.Delete(ClientUserId);
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

        public UserDTO GetUserById(string accessId, int Id)
        {
            try
            {
                return UserService.GetById(Id);
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

        #region "Sign in Functionality"

        [HttpPost]
        public UserDTO UserSignIn(string accessId, string Email, string Password)
        {
            try
            {
                return UserService.SignIn(Email, Password);
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

        [HttpGet]
        public bool ForgotPassword(string Email)
        {
            try
            {
                return (UserService.ForgotPassword(Email));
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

        [HttpPost]
        public PageData<UserDTO> GetUserPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            try
            {
                return UserService.GetUserPagedListbyClientId(pagingInfo, ClientId);
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
        public PageData<UserDTO> GetUserPagedListbyClientIdWithIsActive(string accessId, PagingInfo pagingInfo, int ClientId, bool IsActive)
        {
            try
            {
                return UserService.GetUserPagedListbyClientIdWithIsActive(pagingInfo, ClientId, IsActive);
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
        
        public bool GetIsUniqueEmail(string accessId, string Email, int Id)
        {
            try
            {
                return UserService.IsUniqueEmail(Email, Id);
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

        public bool GetIsUniqueMobile(string accessId, string Mobile, int Id)
        {
            try
            {
                return UserService.IsUniqueMobile(Mobile, Id);
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
        public bool ActiveInactiveUser(int UserId)
        {
            try
            {
                return UserService.ActiveInactiveUser(UserId);
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

        public UserAccessDTO GetUserAccess(string UserType)
        {
            try
            {
                return UserService.GetUserAccess(UserType);
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

    }
}

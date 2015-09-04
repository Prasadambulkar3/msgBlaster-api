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
    public class EcouponCampaignController : ApiController
    {
        #region "CRUD Functionality"

        public int CreateEcouponCampaign(string accessId, EcouponCampaignDTO EcouponCampaignDTO)
        {

            try
            {
                return EcouponCampaignService.Create(EcouponCampaignDTO);
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

        public void EditEcouponCampaign(string accessId, EcouponCampaignDTO EcouponCampaignDTO)
        {
            try
            {
                EcouponCampaignService.Edit(EcouponCampaignDTO);
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

        public void RemoveEcouponCampaign(string accessId, int EcouponCampaignId)
        {
            try
            {
                EcouponCampaignService.Delete(EcouponCampaignId);
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

        public EcouponCampaignDTO GetEcouponCampaignById(string accessId, int Id)
        {
            try
            {
                return EcouponCampaignService.GetById(Id);
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

        #region "List Functionality"

        public List<EcouponCampaignDTO> GetEcouponCampaignListByClientId(string accessId, int ClientId)
        {
            try
            {
                return EcouponCampaignService.GetEcouponCampaignListByClientId(ClientId);
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
        public PageData<EcouponCampaignDTO> GetEcouponCampaignPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            try
            {
                return EcouponCampaignService.GetEcouponCampaignPagedListbyClientId(pagingInfo, ClientId);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or EcouponCampaign the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or EcouponCampaign the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        public List<EcouponCampaignDTO> GetEcouponCampaignCountByClientId(int ClientId)
        {

            {
                try
                {
                    return EcouponCampaignService.GetEcouponCampaignCountByClientId(ClientId);
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
        }

        [HttpPost]
        public PageData<ContactDTO> GetContactsByEcouponCampaignId(PagingInfo pagingInfo, int EcouponCampaignId, int ClientId)
        {
            try
            {
                return EcouponCampaignService.GetContactsByEcouponCampaignId(pagingInfo, EcouponCampaignId, ClientId);
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

        public object GetEcouponCampaignsbyClientId()
        {
            var queryString = HttpContext.Current.Request.QueryString;
            int clientId = Convert.ToInt32(queryString["clientId"]);
            string CampaignName = queryString["CampaignName"];
            string search = queryString["search"];
            DateTime ScheduledDate = Convert.ToDateTime(queryString["ScheduledDate"]);
            DateTime CreatedDate = Convert.ToDateTime(queryString["CreatedDate"]);
            DateTime ExpiryDate = Convert.ToDateTime(queryString["ExpiryDate"]);
            try
            {
                if (search == "default")
                {
                    // return (CampaignService.GetCampaignListByClientId(clientId));
                    return new { Items = EcouponCampaignService.GetEcouponCampaignListByClientId(clientId) };
                }
                else
                {
                    return new { Items = EcouponCampaignService.GetEcouponCampaignListByFilters(clientId, CampaignName, CreatedDate, ScheduledDate, ExpiryDate) };
                }

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

        [HttpGet]
        public bool ExtendOrExpireEcouponCampaign(int EcouponCampaignId, DateTime ExpiredOn, bool IsExpired)
        {
            try
            {
                return EcouponCampaignService.ExtendOrExpireEcouponCampaign(EcouponCampaignId, ExpiredOn, IsExpired);
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
   
    }
}

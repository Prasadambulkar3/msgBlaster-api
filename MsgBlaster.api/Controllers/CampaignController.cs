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
    public class CampaignController : ApiController
    {
        #region "CRUD Functionality"

        public int CreateCampaign(string accessId, CampaignDTO campaignDTO)
        {

            try
            {
                return CampaignService.Create(campaignDTO);
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

        public void EditCampaign(string accessId, CampaignDTO campaignDTO)
        {
            try
            {
                CampaignService.Edit(campaignDTO);
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

        public void RemoveCampaign(int id)
        {
            try
            {
                CampaignService.Delete(id);
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

        public void CancelCampaign(string accessId, CampaignDTO campaignDTO)
        {
            try
            {
                CampaignService.CancelCampaign(campaignDTO);
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

        public CampaignDTO GetCampaignById(string accessId, int Id)
        {
            try
            {
                return CampaignService.GetById(Id);
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


        public int CreateCampaignFromExternalApplication(CampaignDTO campaignDTO)
        {
            try
            {
                return CampaignService.CreateCampaignFromBackend(campaignDTO);
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
       
        [HttpPost]
        public PageData<CampaignDTO> GetCampaignPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            try
            {
                return CampaignService.GetCampaignPagedListbyClientId(pagingInfo, ClientId);
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
        public PageData<CampaignDTO> GetCampaignPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        {
            try
            {
                return CampaignService.GetCampaignPagedListbyPartnerId(pagingInfo, PartnerId);
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
        public PageData<ContactDTO> GetContactsByCampaignId(PagingInfo pagingInfo, int CampaignId, int ClientId)
        {
            try
            {
                return CampaignService.GetContactsByCampaignId(pagingInfo, CampaignId, ClientId);
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

        public object GetCampaignsbyClientId()
        {
            var queryString = HttpContext.Current.Request.QueryString;
            int clientId = Convert.ToInt32(queryString["clientId"]);
            string CampaignName = queryString["CampaignName"];
            string search = queryString["search"];
            DateTime ScheduledDate = Convert.ToDateTime(queryString["ScheduledDate"]);
            DateTime CreatedDate = Convert.ToDateTime(queryString["CreatedDate"]);
            try
            {
                if (search == "default")
                {
                    // return (CampaignService.GetCampaignListByClientId(clientId));
                    return new { Items = CampaignService.GetCampaignListByClientId(clientId) };
                }
                else
                {
                    return new { Items = CampaignService.GetCampaignListByFilters(clientId, CampaignName, CreatedDate, ScheduledDate) };
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
    }
}
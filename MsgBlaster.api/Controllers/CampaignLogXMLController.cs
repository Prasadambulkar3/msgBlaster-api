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
    public class CampaignLogXMLController : ApiController
    {
        #region "List Functionality"
        
        [HttpPost]
        public PageData<CampaignLogDTO> GetCampaignLogPagedListbyCampaignId(int CampaignId, PagingInfo pagingInfo)
        {
            try
            {
                return CampaignLogXMLService.GetCampaignLogPagedListbyCampaignId(CampaignId, pagingInfo);
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
        //[HttpPost]
        //public PageData<CampaignLogDTO> GetCampaignLogPagedListbyCampaignId(int CampaignId, bool IsSuccessful, PagingInfo pagingInfo)
        // {
        //    try
        //    {
        //        return CampaignLogXMLService.GetCampaignLogPagedListbyCampaignId(CampaignId, IsSuccessful, pagingInfo);
        //    }
        //    catch (TimeoutException)
        //    {
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
        //        {
        //            Content = new StringContent("An error occurred, please try again or contact the administrator."),
        //            ReasonPhrase = "Critical Exception"
        //        });
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

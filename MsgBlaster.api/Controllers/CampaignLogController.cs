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
    public class CampaignLogController : ApiController
    {
      
        #region "Unwanted code"

        //public CampaignLogDTO GetCampaignLogById(string accessId, int Id)
        //{
        //    try
        //    {
        //        return CampaignLogService.GetById(Id);
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

        //[HttpPost]
        //public PageData<CampaignLogDTO> GetCampaignLogPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        //{
        //    try
        //    {
        //        return CampaignLogService.GetCampaignLogPagedListbyClientId(pagingInfo, ClientId);
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

        //[HttpPost]
        //public PageData<CampaignLogDTO> GetCampaignLogPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        //{
        //    try
        //    {
        //        return CampaignLogService.GetCampaignLogPagedListbyPartnerId(pagingInfo, PartnerId);
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


        //[HttpPost]
        //public PageData<CampaignLogDTO> GetCampaignLogPagedListbyCampaignIdWithSuccessLog(PagingInfo pagingInfo, int CampaignId, bool IsSuccess)
        //{
        //    try
        //    {
        //        return CampaignLogService.GetCampaignLogPagedListbyCampaignIdWithSuccessLog(pagingInfo, CampaignId, IsSuccess);
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

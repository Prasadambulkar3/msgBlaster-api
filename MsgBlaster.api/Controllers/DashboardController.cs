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
    public class DashboardController : ApiController
    {
        

        /// <summary>
        /// Client Object
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="ClientId"></param>
        /// <returns>Here You will Get Credit Balance, Total Credit Requested, Total Approved credit request</returns>
        public ClientDTO GetClientInfoById(string accessId, int ClientId)
        {
            try
            {
                return ClientService.GetById(ClientId);

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

        //Get Todays Message sent Count by client id
        public TodaysMessageCountDTO GetTodaysMessageSentByClientId(string accessId, int ClientId)
        {
            try
            {
                TodaysMessageCountDTO TodaysMessageCountDTO = new TodaysMessageCountDTO();
                TodaysMessageCountDTO.TodaysTotalCampaignMessageSent = CampaignService.GetTodaysCampaignMsssageSentCount(ClientId);
                TodaysMessageCountDTO.TodaysTotalCouponMessageSent= EcouponCampaignService.GetTodaysCouponMsssageSentCount(ClientId);
                TodaysMessageCountDTO.TodaysTotalMessageSent = TodaysMessageCountDTO.TodaysTotalCampaignMessageSent + TodaysMessageCountDTO.TodaysTotalCouponMessageSent;
                TodaysMessageCountDTO.TotalPendingCreditRequest = CreditRequestService.GetPendingCreditRequestCount(ClientId); 
                return TodaysMessageCountDTO;

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

        //Get location wise reedeemed count by user id
        public List<LocationWiseRedeemedCountDTO> GetLocationGroupWiseCountByUserId(string accessId, int UserId)
        {
            try
            {
                return RedeemedCountService.GetLocationGroupWiseCountByUserId(UserId);

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

        //Get location wise redeemed count by client id
        public List<LocationWiseRedeemedCountDTO> GetLocationGroupWiseCountByClientId(string accessId, int ClientId)
        {
            try
            {
                return RedeemedCountService.GetLocationGroupWiseCountByClientId(ClientId);

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

        //Get all open ecoupon campaign list by client id 
        public List<EcouponCampaignDTO> GetAllOpenEcouponCampaignsByClientId(string accessId, int ClientId)
        {
            try
            {
                return EcouponCampaignService.GetAllOpenEcouponCampaignsByClientId(ClientId);

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

        //get recent blast of the campaign by client id
        public List<CampaignDTO> GetCampaignRecentBlastByClientId(string accessId, int ClientId, int Top)
        {
            try
            {
                return CampaignService.GetCampaignRecentBlastByClientId(ClientId, Top);

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

        //Get refilled credit request list by client id
        public List<CreditRequestDTO> GetCreditRequestRefilledListByClientId(string accessId, int ClientId, int Top)
        {
            try
            {
                return CreditRequestService.GetCreditRequestRefilledListByClientId(ClientId, Top);

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

        //Get recent template list by client id
        public List<TemplateDTO> GetRecentTemplateListByClientId(string accessId, int ClientId, int Top)
        {
            try
            {
                return TemplateService.GetRecentTemplateListByClientId(ClientId, Top);

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

        /// <summary>
        /// Day Wise Campaign And Coupon Msssage Sent Count 
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns>Returns day wise list of message sent</returns>
        public List<MessageSentCountDTO> GetDayWiseCampaignAndCouponMsssageSentCount(string accessId, int ClientId)
        {
            try
            {
                return CampaignService.GetDayWiseCampaignAndCouponMsssageSentCount(ClientId);

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

        /// <summary>
        /// Month Wise Campaign And Coupon Msssage Sent Count 
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns>Returns month wise list of message sent</returns>         
        public List<MessageSentCountDTO> GetMonthWiseCampaignAndCouponMsssageSentCount(string accessId, int ClientId)
        {
            try
            {
                return CampaignService.GetMonthWiseCampaignAndCouponMsssageSentCount(ClientId);

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

        /// <summary>
        /// Get latest Top n Activities of Client
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="ClientId"></param>
        /// <param name="Top"></param>
        /// <returns></returns>
        public List<ActivityLogDTO> GetClientLatestActivities(string accessId, int ClientId, int Top)
        {
            try
            {
                return CommonService.GetClientLatestActivities(ClientId, Top);

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

        /// <summary>
        /// Get latest Top n Activities of partner
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="PartnerId"></param>
        /// <param name="Top"></param>
        /// <returns></returns>
        public List<ActivityLogDTO> GetPartnerLatestActivities(string accessId, int PartnerId, int Top)
        {
            try
            {
                return CommonService.GetPartnerLatestActivities(PartnerId, Top);

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

        #region "Unwanted code"
        //public int GetAllCreditRequestCountByClientId(string accessId, int ClientId)
        //{

        //    try
        //    {
        //        return CreditRequestService.GetAllCreditRequestCountByClientId(ClientId);
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

        //public int GetApprovedCreditRequestCountByClientId(string accessId, int ClientId)
        //{

        //    try
        //    {
        //        return CreditRequestService.GetApprovedCreditRequestCountByClientId(ClientId);
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

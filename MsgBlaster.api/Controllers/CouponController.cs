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
    public class CouponController : ApiController
    {
        #region "CRUD Functionality"
     
        public CouponDTO RedeemCoupon(CouponDTO CouponDTO)
        {
            try
            {
                //return CouponService.RedeemCoupon(Id, Remark, ClientUserId);
                return CouponService.RedeemCoupon(CouponDTO);
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

        public CouponDTO GetCouponById(string accessId, int Id)
        {
            try
            {
                return CouponService.GetById(Id);
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

        public List<CouponDTO> GetCouponListByEcouponCampaignId(string accessId, int EcouponCampaignId)
        {
            try
            {
                return CouponService.GetCouponListByEcouponCampaignId(EcouponCampaignId);
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

        public List<CouponDTO> GetCouponListByEcouponCampaignIdAndMobile(string accessId, int EcouponCampaignId, string Mobile)
        {
            try
            {
                return CouponService.GetCouponListByEcouponCampaignIdAndMobile(EcouponCampaignId, Mobile);
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

        public List<CouponDTO> GetCouponListByClientId(int ClientId)
        {
            try
            {
                return CouponService.GetCouponListByClientId(ClientId);
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
        public PageData<CouponDTO> GetCouponPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            try
            {
                return CouponService.GetCouponPagedListbyClientId(pagingInfo, ClientId);
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
        public PageData<CouponDTO> GetCouponPagedListbyUserId(PagingInfo pagingInfo, int UserId)
        {

            {
                try
                {
                    return CouponService.GetCouponPagedListbyUserId(pagingInfo, UserId);
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
        public PageData<CouponDTO> GetCouponPagedListbyEcouponCampaignId(PagingInfo pagingInfo, int EcouponCampaignId, bool IsSent)
        {
            try
            {
                return CouponService.GetCouponPagedListbyEcouponCampaignId(pagingInfo, EcouponCampaignId, IsSent);
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

        public List<CouponDTO> GetMobileNumberAndClientIdWiseCouponList(String Mobile, int ClientId)
        {
            try
            {
                return CouponService.GetMobileNumberAndClientIdWiseCouponList(Mobile, ClientId);
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

        [HttpGet]      
        public CouponDTO ResendCoupon(int CouponId, int ClientId)
        {
            try
            {
                return CouponService.ResendCoupon(CouponId, ClientId);
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

        public CouponDTO GetCouponDetailsFromMobileAndCode(string Mobile, string Code)
        {

            {
                try
                {
                    return CouponService.GetCouponDetailsFromMobileAndCode(Mobile, Code);
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

        #endregion
        
    }
}

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
    public class CreditRequestController : ApiController
    {
        #region "CRUD Functionality"

        public CreditRequestDTO CreateCreditRequest(string accessId, CreditRequestDTO oCreditRequestDTO)
        {
            try
            {
                return (CreditRequestService.Create(oCreditRequestDTO));
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public CreditRequestDTO GetCreditRequestById(string accessId, int CreditRequestId)
        {

            try
            {
                return CreditRequestService.GetById(CreditRequestId);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        public bool EditCreditRequestForOnlinePayment(CreditRequestDTO creditRequestDTO, bool IsSuccess)
        {

            try
            {
                if (creditRequestDTO.PaymentId == null)
                {
                    return false;
                }
                return CreditRequestService.EditCreditRequestForOnlinePayment(creditRequestDTO, IsSuccess);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }       

        #endregion

        #region "List Functionality"

        [HttpPost]
        public PageData<CreditRequestDTO> GetCreditRequestPagedListbyPartnerIdWithProvidedflag(PagingInfo pagingInfo, int PartnerId, bool IsProvided)
        {
            try
            {
                return CreditRequestService.GetCreditRequestPagedListbyPartnerIdWithProvidedflag(pagingInfo, PartnerId, IsProvided);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        [HttpPost]
        public PageData<CreditRequestDTO> GetCreditRequestPagedListbyClientIdWithProvidedflag(PagingInfo pagingInfo, int ClientId, bool IsProvided)
        {
            try
            {
                return CreditRequestService.GetCreditRequestPagedListbyClientIdWithProvidedflag(pagingInfo, ClientId, IsProvided);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        [HttpPost]
        public PageData<CreditRequestDTO> GetCreditRequestPagedListbyClientId(string accessId, PagingInfo pagingInfo, int ClientId)
        {
            try
            {
                return CreditRequestService.GetCreditRequestPagedListbyClientId(pagingInfo, ClientId);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        [HttpPost]
        public PageData<CreditRequestDTO> GetCreditRequestPagedListbyPartnerId(string accessId, PagingInfo pagingInfo, int PartnerId)
        {
            try
            {
                return CreditRequestService.GetCreditRequestPagedListbyPartnerId(pagingInfo, PartnerId);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        #endregion

        #region "Other Functionality"

        [HttpGet]
        public CreditRequestDTO ProvideCreditByMailLink(int CreditRequestId)
        {
            try
            {
                return CreditRequestService.ProvideCreditByMailLink(CreditRequestId);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        public int GetCreditRequestCount(int accessId, int ClientId)
        {

            try
            {
                return (CreditRequestService.GetCreditRequestCountByClientId(ClientId));
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        [HttpPost]
        public CreditRequestDTO ProvideCredit(CreditRequestDTO CreditRequestDTO)
        {
            try
            {
                return CreditRequestService.ProvideCredit(CreditRequestDTO);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        [HttpPost]
        public CreditRequestDTO GenerateBill(CreditRequestDTO CreditRequestDTO)
        {
            try
            {
                return CreditRequestService.GenerateBill(CreditRequestDTO);
            }
            catch (TimeoutException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }

        #endregion
      
        #region "Unwanted code"

        //[HttpPost]
        //public PageData<CreditRequestDTO> GetCreditRequestPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        //{
        //    try
        //    {
        //        return CreditRequestService.GetCreditRequestPagedListbyClientId(pagingInfo, ClientId);
        //    }
        //    catch (TimeoutException)
        //    {
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
        //        {
        //            Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
        //            ReasonPhrase = "Critical Exception"
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        //        {
        //            Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
        //            ReasonPhrase = "Critical Exception"
        //        });
        //    }

        //}

        //[HttpPost]
        //public PageData<CreditRequestDTO> GetCreditRequestPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        //{
        //    try
        //    {
        //        return CreditRequestService.GetCreditRequestPagedListbyPartnerId(pagingInfo, PartnerId);
        //    }
        //    catch (TimeoutException)
        //    {
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
        //        {
        //            Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
        //            ReasonPhrase = "Critical Exception"
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        //        {
        //            Content = new StringContent("An error occurred, please try again or CreditRequest the administrator."),
        //            ReasonPhrase = "Critical Exception"
        //        });
        //    }

        //}

        #endregion

    }
}

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
using MsgBlaster.DTO.Enums;
using System.Linq;


namespace MsgBlaster.api.Controllers
{
    public class CommonController : ApiController
    {
        #region "Other Functionality"

        public string GetFirstname(string Name)
        {

            try
            {
                return CommonService.GetFirstname(Name);
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

        public string Getlastname(string Name)
        {

            try
            {
                return CommonService.GetLastname(Name);
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
        public string SaveDocumentToFolder(string accessId, string documentPath, int ClientId, int UserId, string FileName)
        {
            try
            {
                HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                return CommonService.SaveDocumentToFolder(file, documentPath, ClientId, UserId, FileName);
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
        public bool Sendmail(string Subject, string body, string To)
        {
            try
            {
                //HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
                return CommonService.SendEmail(Subject, body, To, "", false);
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

        public List<string> GetMacrosList()
        {
            try
            {
                //List<Macros> MacrosList = Enum.GetValues(typeof(Macros)).Cast<Macros>().ToList();
                List<string> MacrosList = Enum.GetNames(typeof(Macros)).ToList(); ;
                return MacrosList;
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

        public List<string> GetUserTypeList()
        {
            try
            {
                //List<Macros> MacrosList = Enum.GetValues(typeof(Macros)).Cast<Macros>().ToList();
                List<string> UserTypeList = Enum.GetNames(typeof(UserType)).ToList(); ;
                return UserTypeList;
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

        public List<string> GetCouponExpireTypeList()
        {
            try
            {
                //List<Macros> MacrosList = Enum.GetValues(typeof(Macros)).Cast<Macros>().ToList();
                List<string> CouponExpireTypeList = Enum.GetNames(typeof(CouponExpireType)).ToList(); ;
                return CouponExpireTypeList;
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
        public List<ContactDTO> ReadExcelFile(int ClientId, string FilePath, bool IsValid)
        {
            try
            {
                //HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
                return CommonService.ReadExcelFile(ClientId, FilePath, IsValid);
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

        public List<ContactColumnDTO> GetContactColumnNames()
        {
            try
            {

                return CommonService.GetContactColumnNames();
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

        public List<string> GetPaymentModeList()
        {
            try
            {
                //List<Macros> MacrosList = Enum.GetValues(typeof(Macros)).Cast<Macros>().ToList();
                List<string> PaymentModeList = Enum.GetNames(typeof(PaymentMode)).ToList(); ;
                return PaymentModeList;
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
      
        ////[HttpGet]
        ////public string CreatePacket(int ClientId, int CampaignId, bool IsCampaign)
        ////{
        ////    try
        ////    {

        ////        return CommonService.CreatePacket(ClientId, CampaignId, IsCampaign);
        ////    }
        ////    catch (TimeoutException)
        ////    {
        ////        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
        ////        {
        ////            Content = new StringContent("An error occurred, please try again or contact the administrator."),
        ////            ReasonPhrase = "Critical Exception"
        ////        });
        ////    }
        ////    catch (Exception)
        ////    {
        ////        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        ////        {
        ////            Content = new StringContent("An error occurred, please try again or contact the administrator."),
        ////            ReasonPhrase = "Critical Exception"
        ////        });
        ////    }
        ////}



        //[HttpGet]
        //public void CreatePacket()
        //{
        //    try
        //    {

        //        CommonService.CreatePacket();
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

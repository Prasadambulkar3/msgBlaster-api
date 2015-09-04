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

using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace MsgBlaster.api
{
    public class PaymentInfoController : ApiController
    {

        [HttpGet]
        public string OnlinePaymentLinkWithTemperproofData(string email, double amount, string phone, string name, int CreditRequestId)
        {

            string salt = ConfigurationManager.AppSettings["InstamojoSaltKey"].ToString();   //"844cf39659b948cf95385947e2523ce7"; // Salt

            /*
            The data dictionary should contain all the read-only fields.
            If you want to include custom field as well then add them to
            the dictionary, but don't forget to add "data_" in front of
            the custom field names. 
            */

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"data_amount", amount.ToString() },
                {"data_email", email},
                {"data_phone", phone},
                {"data_name", name},
                //{"data_Field_52722", CreditRequestId.ToString()} 
                {ConfigurationManager.AppSettings["InstamojoCustomField"].ToString() ,CreditRequestId.ToString() }
            };

            string msg = MsgCreator(data);
            string signature = ShaHash(msg, salt);

            string URLOnlinepayment = ConfigurationManager.AppSettings["InstamojoPaymentLink"].ToString();   //   "https://test.instamojo.com/rohitkale/msgblaster-249d6/?data_readonly=data_name&data_readonly=data_email&data_readonly=data_phone&data_readonly=data_amount&data_readonly=data_Field_52722&data_sign=[sign]&data_email=[email]&data_amount=[amount]&data_name=[name]&data_phone=[Phone]&data_Field_52722=[CreditRequestid]";

            URLOnlinepayment = URLOnlinepayment.Replace("%26", "&");
             URLOnlinepayment = URLOnlinepayment.Replace("[sign]",signature);
             URLOnlinepayment = URLOnlinepayment.Replace("[email]" , email);
             URLOnlinepayment = URLOnlinepayment.Replace("[amount]",amount.ToString());
             URLOnlinepayment = URLOnlinepayment.Replace("[name]",name);
             URLOnlinepayment = URLOnlinepayment.Replace("[Phone]", phone);
             URLOnlinepayment = URLOnlinepayment.Replace("[CreditRequestid]", CreditRequestId.ToString());



             return URLOnlinepayment;



        }

        [HttpGet]
        public string GetPaymentDetailBywebhook()
        {
            return HttpContext.Current.Request.QueryString.ToString(); 
        }

        [HttpGet]
        public string  GetPaymentDetails()
        {
            try
            {
                payment OnlinePaymentDTO = new payment();  //OnlinePaymentDTO            

                var queryString = HttpContext.Current.Request.QueryString;
                string payment_id = queryString["payment_id"];
                string status = queryString["status"];


                string PaymentDetailUrl = ConfigurationManager.AppSettings["PaymentDetailsUrl"].ToString();// "https://test.instamojo.com/api/1.1/payments/[payment_id]?api_key=4709c5655f99d3799ccc22d8da23a137&auth_token=820cb88637df64e703782970c729a5fb";

                PaymentDetailUrl = PaymentDetailUrl.Replace("%26", "&");
                PaymentDetailUrl = PaymentDetailUrl.Replace("[payment_id]", payment_id);

                WebRequest request = HttpWebRequest.Create(PaymentDetailUrl);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseText = reader.ReadToEnd();

               // OnlinePaymentDTO = urlpaymentId;
                return responseText;// OnlinePaymentDTO;

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

        [HttpGet]
        public PaymentDTO GetPaymentDetailsURLByPaimentId(string paymentId)
        {
            try
            {
                payment OnlinePaymentDTO = new payment();  //OnlinePaymentDTO            

                var queryString = HttpContext.Current.Request.QueryString;
                string payment_id = paymentId;
                string PaymentDetailUrl = ConfigurationManager.AppSettings["PaymentDetailsUrl"].ToString();//  "https://test.instamojo.com/api/1.1/payments/[payment_id]?api_key=4709c5655f99d3799ccc22d8da23a137&auth_token=820cb88637df64e703782970c729a5fb";
                PaymentDetailUrl = PaymentDetailUrl.Replace("%26", "&");
                PaymentDetailUrl = PaymentDetailUrl.Replace("[payment_id]", payment_id);

                WebRequest request = HttpWebRequest.Create(PaymentDetailUrl);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseText = reader.ReadToEnd();
                //responseText = responseText.Replace("\n","");


                PaymentDTO PaymentDTO = JsonConvert.DeserializeObject<PaymentDTO>(responseText);
                if (PaymentDTO != null)
                {
                    if (PaymentDTO.payment.custom_fields != null)
                    {
                        int CreditRequestId = Convert.ToInt32( PaymentDTO.payment.custom_fields.Field_52722.value);
                        CreditRequestDTO CreditRequestDTO = new CreditRequestDTO();
                        CreditRequestDTO = CreditRequestService.GetById(CreditRequestId);
                        if (PaymentDTO.success == true)
                        {
                            if (CreditRequestDTO.IsPaid != true)
                            {
                                CreditRequestDTO.PaymentId = PaymentDTO.payment.payment_id;
                                CreditRequestDTO.ProvidedCredit = CreditRequestDTO.RequestedCredit;
                                CreditRequestService.EditCreditRequestForOnlinePayment(CreditRequestDTO, true);
                            }
                        }
                        else 
                        { 
                            CreditRequestDTO.PaymentId = PaymentDTO.payment.payment_id;
                            CreditRequestDTO.IsPaid = false;
                            CreditRequestService.EditCreditRequestForOnlinePayment(CreditRequestDTO, false);
                        }
                         
                    }
                }
               // PaymentInfoDTO PaymentInfoDTO = JsonConvert.DeserializeObject<PaymentInfoDTO>(responseText);

                // OnlinePaymentDTO = urlpaymentId;
                //return PaymentDetailUrl;
                return PaymentDTO;// OnlinePaymentDTO;
               
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

        static string MsgCreator(Dictionary<string, string> data)
        {
            var ordered_view = data.OrderBy(key => key.Key.ToLower());
            string message = "";

            foreach (var item in ordered_view)
            {
                message += item.Value + "|";
            }
            return message.Substring(0, message.Length - 1);

        }

        static string ShaHash(string msg, string salt)
        {
            using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(salt)))
            {
                return ByteToString(hmac.ComputeHash(Encoding.ASCII.GetBytes(msg)));
            }
        }

        static string ByteToString(IEnumerable<byte> msg)
        {
            return string.Concat(msg.Select(b => b.ToString("x2")));
        }

    }
}
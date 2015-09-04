using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using MsgBlaster.DTO;
using MsgBlaster.Repo;
using MsgBlaster.Domain;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Serialization;

using System.Security.Cryptography;
using MsgBlaster.DTO.Enums;
using System.Xml.Schema;
using System.Collections;

namespace MsgBlaster.Service
{
    public class CommonService
    {

        #region "Variable declataion"

        public byte[] key = {
	                16,
	                13,
	                50,
	                98,
	                168,
	                65,
	                59,
	                79,
	                131,
	                45,
	                231,
	                67,
	                91,
	                23,
	                255,
	                143
                };

        public byte[] iv = {
	                255,
	                110,
	                68,
	                26,
	                69,
	                178,
	                200,
	                219
                };

        OleDbConnection Con;
        DataTable NewTable;

        static int MAXSMSLENGTH = 160;
        static int SMSBLOCKLENGTH = 153;

        public static Random _r = new Random();
        public static string veificationcode = "";

        #endregion

        #region "Send mail Functionality"

        public static bool SendEmail(string subject, string messageBody, string toAddress, string ccAddress, bool attachfile) //object sender, EventArgs e
        {
            bool functionReturnValue = false;

            string to = toAddress;// email.Text;
            string from = ConfigurationManager.AppSettings["SMTPEmailID"].ToString();
            string mailsubject = subject;
            string body = messageBody;
            using (MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["SMTPEmailID"].ToString(), toAddress))
            {
                mm.Subject = subject;
                mm.Body = messageBody;

                ////if (attachfile == true)
                ////{
                ////    if (fuAttachment.HasFile)
                ////    {
                ////        string FileName = Path.GetFileName(fuAttachment.PostedFile.FileName);
                ////        mm.Attachments.Add(new Attachment(fuAttachment.PostedFile.InputStream, FileName));
                ////    }
                ////}                    


                //Allow multiple "To" addresses to be separated by a semi-colon
                if ((toAddress.Trim().Length > 0))
                {

                    try
                    {


                        foreach (string addr in toAddress.Split(';'))
                        {
                            System.Net.Mail.MailAddress toadr = new System.Net.Mail.MailAddress(addr);

                            if (mm.To.Contains(toadr))
                            {
                            }
                            else
                            {
                                mm.To.Add(new System.Net.Mail.MailAddress(addr));
                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }


                //Allow multiple "Cc" addresses to be separated by a semi-colon
                if ((ccAddress.Trim().Length > 0))
                {

                    try
                    {

                        foreach (string addr in ccAddress.Split(';'))
                        {
                            System.Net.Mail.MailAddress ccadr = new System.Net.Mail.MailAddress(addr);

                            if (mm.CC.Contains(ccadr))
                            {
                            }
                            else
                            {
                                mm.CC.Add(new System.Net.Mail.MailAddress(addr));
                            }
                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }

                //if ((path.Trim().Length > 0))
                //{
                //    try
                //    {

                //        foreach (string attachment in path.Split(';'))
                //        {
                //            System.Net.Mail.Attachment attch = new System.Net.Mail.Attachment(attachment);

                //            if (mm.Attachments.Contains(attch))
                //            {
                //            }
                //            else
                //            {
                //                mm.Attachments.Add(new System.Net.Mail.Attachment(attachment));
                //            }

                //        }


                //    }
                //    catch (Exception ex)
                //    {
                //    }

                //}



                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["SMTPMailServer"].ToString();
                bool EnableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPEnableSSL"].ToString());
                smtp.EnableSsl = EnableSSL;
                NetworkCredential NetworkCred = new NetworkCredential(ConfigurationManager.AppSettings["SMTPEmailID"].ToString(), ConfigurationManager.AppSettings["SMTPPassword"].ToString());

                bool DefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                smtp.UseDefaultCredentials = DefaultCredentials;
                smtp.Credentials = NetworkCred;
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPortNo"].ToString());
                try
                {
                    smtp.Send(mm);
                    functionReturnValue = true;

                }
                catch (Exception)
                {
                    functionReturnValue = false;

                }
            }

            return functionReturnValue;


        }

        public static string EmailVerificationCode()
        {


            // Use class-level Random so that when this
            // ... method is called many times, it still has
            // ... good Randoms.
            int n = _r.Next();
            // If this declared a local Random, it would
            // ... repeat itself.
            veificationcode = n.ToString();
            veificationcode = veificationcode.Remove(veificationcode.Length - 5, 5);
            return veificationcode;

        }

        #endregion

        #region "OTP Functionality"
        
        public static string OTPVeificationcode(string mobileNo, string Gatewayname)
        {
            // Use class-level Random so that when this
            // ... method is called many times, it still has
            // ... good Randoms.
            int n = _r.Next();
            // If this declared a local Random, it would
            // ... repeat itself.
            veificationcode = n.ToString();
            veificationcode = veificationcode.Remove(veificationcode.Length - 5, 5);


            //Remove below comment to send SMS
            ActualSmsSend(mobileNo, "Your msgBlaster OTP verification Code Is - " + veificationcode, Gatewayname);
            return veificationcode; // +" is your msgBlaster confirmation code.";

        }

        private static String ActualSmsSend(string mobilenumber, string message, string Gateway)
        {
            string result = "";

            if (message != "" && mobilenumber != "")// Check for empty message.
            {

                string Url = ConfigurationManager.AppSettings["TransactionalGateWay"].ToString();
                Url = Url.Replace("%26", "&");
                Url = Url.Replace("[recipient]", mobilenumber);
                Url = Url.Replace("[message]", message);
                Url = Url.Replace("[gateway]", Gateway); //Gateway = "MSGBLS"

                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                result = sr.ReadToEnd();
                sr.Close();

                string statuscode = "";
                if (result.Contains('|'))
                    statuscode = result.Substring(0, result.IndexOf('|'));
                else
                    statuscode = result;
                SMSResult(result);
                myResponse.Close();

                //For RouteSMS CODE = 1025 means insufficient balance so send it from WhizSMS gateway
                if (statuscode == "1025")
                {
                    string resultWhiz = "";
                    string UrlWhiz = ConfigurationManager.AppSettings["TransactionalGateWayWhiz"].ToString();
                    UrlWhiz = UrlWhiz.Replace("%26", "&");
                    UrlWhiz = UrlWhiz.Replace("[recipient]", mobilenumber);
                    UrlWhiz = UrlWhiz.Replace("[message]", message);
                    UrlWhiz = UrlWhiz.Replace("[gateway]", Gateway);

                    HttpWebRequest myRequestWhiz = (HttpWebRequest)WebRequest.Create(UrlWhiz);
                    myRequestWhiz.Method = "GET";
                    WebResponse myResponseWhiz = myRequestWhiz.GetResponse();
                    StreamReader srWhiz = new StreamReader(myResponseWhiz.GetResponseStream(), System.Text.Encoding.UTF8);
                    resultWhiz = srWhiz.ReadToEnd();
                    srWhiz.Close();

                    if (resultWhiz.Contains('|'))
                    {
                        //statuscode = result.Substring(0, result.IndexOf('|'));
                        statuscode = "";
                        string[] words = resultWhiz.Split('|');
                        foreach (string word in words)
                        {
                            Console.WriteLine(word);
                            try
                            {
                                int code = Convert.ToInt32(word);

                                statuscode = code.ToString();
                            }
                            catch (Exception)
                            {
                                string code = word.Replace(" ", "");
                                if (code == "Success")
                                {
                                    code = "0";
                                    statuscode = code.ToString();
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                    }
                    else
                    {
                        statuscode = resultWhiz;
                    }

                    SMSResultWhiz(resultWhiz);
                    myResponseWhiz.Close();
                    return resultWhiz;
                }



            }

            return result;
        }

        #endregion
      
        /// <summary>
        /// Method to get message count.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int GetMessageCount(string message)
        {
            int specialCharCount = 0;

            specialCharCount += message.Count(f => f == '^');
            specialCharCount += message.Count(f => f == '{');
            specialCharCount += message.Count(f => f == '}');
            specialCharCount += message.Count(f => f == '\\');
            specialCharCount += message.Count(f => f == '[');
            specialCharCount += message.Count(f => f == ']');
            specialCharCount += message.Count(f => f == '|');
            specialCharCount += message.Count(f => f == '~');
            specialCharCount += message.Count(f => f == '€');


            int msgLength = message.Length + specialCharCount;


            //// MaxSMSLength, SMSBlockLength these two valus come from database
            //// Calculate the credits required to send this message.
            //string sErrlocation = "Calculating message count-500";

            SettingDTO SettingDTO = new SettingDTO();
            SettingDTO = SettingService.GetById(1);
            MAXSMSLENGTH = SettingDTO.MessageLength;
            SMSBLOCKLENGTH = SettingDTO.SingleMessageLength;


            if (msgLength <= MAXSMSLENGTH)
                return 1;
            else if (msgLength % SMSBLOCKLENGTH != 0)
                return (msgLength / SMSBLOCKLENGTH) + 1;
            else
                return msgLength / SMSBLOCKLENGTH;
        }

        public static string GetIP()
        {
            try
            {
                HttpRequest context = HttpContext.Current.Request;
                // IP Adddress and HostName
                string ipAddress = context.UserHostAddress;
                return ipAddress;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region "Get Random nuber functionality"
 
        private static string _numbers = "0123456789";
        Random random = new Random();
        public int GetRandomNumber()
        {
            StringBuilder builder = new StringBuilder(6);
            string numberAsString = "";
            int numberAsNumber = 0;

            for (var i = 1; i <= 6; i++)
            {
                builder.Append(_numbers[random.Next(0, _numbers.Length)]);
            }

            numberAsString = builder.ToString();
            numberAsNumber = int.Parse(numberAsString);
            return numberAsNumber;
        }

        #endregion

        //Get is provided string is in date format or not
        public static bool IsDate(string datetimestring)
        {
            bool IsDate = false;
            // Alternate choice: If the string has been input by an end user, you might  
            // want to format it according to the current culture:            
            IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {
                DateTime dt2 = DateTime.Parse(datetimestring, culture, System.Globalization.DateTimeStyles.AssumeLocal);
                IsDate = true;
            }
            catch (Exception)
            {
                IsDate = false;
            }
            return IsDate;

        }

        //Get first name by full name
        public static String GetFirstname(string Name)
        {
            string FirstName = "";

            try
            {


                // Input string contain separators.
                string value1 = Name;

                char[] delimiter1 = new char[] { ' ' };   // <-- Split on these
                // ... Use StringSplitOptions.RemoveEmptyEntries.
                string[] array2 = value1.Split(delimiter1,
                    StringSplitOptions.RemoveEmptyEntries);

                //Console.WriteLine();
                foreach (string firstname in array2)
                {
                    Console.WriteLine(firstname);
                    FirstName = firstname;
                    return FirstName;
                }
                return FirstName;

            }
            catch (Exception)
            {
                return FirstName;
            }
        }

        //Get middle name by full name
        public static String GetMiddlename(string Name)
        {
            string MiddletName = "";

            try
            {


                // Input string contain separators.
                string value1 = Name;

                char[] delimiter1 = new char[] { ' ' };   // <-- Split on these
                // ... Use StringSplitOptions.RemoveEmptyEntries.
                string[] array2 = value1.Split(delimiter1,
                    StringSplitOptions.RemoveEmptyEntries);
                if (array2.Count() == 1)
                {
                    return "";
                }
                int TotalCount = array2.Count();
                if (array2.Count() >= 3)
                {
                    for (int i = 0; i <= array2.Count(); i++)
                    {
                        if (i > 0 && i < TotalCount - 1)
                        {
                            MiddletName = MiddletName + " " + array2[i];
                        }

                    }

                }

                return MiddletName;

            }
            catch (Exception)
            {
                return MiddletName;
            }
        }

        //Get last name by full name
        public static String GetLastname(string Name)
        {
            string LasttName = "";

            try
            {


                // Input string contain separators.
                string value1 = Name;

                char[] delimiter1 = new char[] { ' ' };   // <-- Split on these
                // ... Use StringSplitOptions.RemoveEmptyEntries.
                string[] array2 = value1.Split(delimiter1,
                    StringSplitOptions.RemoveEmptyEntries);
                if (array2.Count() == 1)
                {
                    return "";
                }
                //
                Console.WriteLine();
                foreach (string lastname in array2)
                {
                    Console.WriteLine(lastname);
                    LasttName = lastname;

                }
                return LasttName;

            }
            catch (Exception)
            {
                return LasttName;
            }
        }

        //Get RouteSMS Result
        private static string SMSResult(string result)
        {
            string messageID;
            string smsResult;
            bool IsSent = false;
            string statuscode = result;


            switch (statuscode)
            {
                case "11":
                    messageID = statuscode + ": Invalid destination";
                    IsSent = true;
                    break;
                case "1701":
                    messageID = result.Substring(result.IndexOf(':') + 1, result.Length - result.IndexOf(':') - 1);
                    IsSent = true;
                    break;
                case "1702":
                    messageID = "Invalid url error";
                    break;
                case "1703":
                    messageID = "Invalid value in username or password";
                    break;
                case "1704":
                    messageID = "Invalid value in type field";
                    break;
                case "1705":
                    messageID = "Invalid Message";
                    IsSent = true;
                    break;
                case "1706":
                    messageID = "Destination does not exist";
                    IsSent = true;
                    break;
                case "1707":
                    messageID = "Invalid source (Sender)";
                    break;
                case "1708":
                    messageID = "Invalid value for dlr field";
                    break;
                case "1709":
                    messageID = "User validation failed";
                    break;
                case "1710":
                    messageID = "Internal Error";
                    break;
                case "1025":
                    messageID = "Insufficient credits";
                    break;
                case "1032":
                    messageID = "Number is in DND";
                    IsSent = true;
                    break;
                default:
                    //General.WriteInFile("Response :" + result);
                    IsSent = false;
                    break;
            }


            smsResult = statuscode;
            return smsResult;

        }

        //Get WhizSMS result
        private static string SMSResultWhiz(string result)
        {
            string messageID;
            string smsResult;
            bool IsSent = false;

            string statuscode = result;



            switch (statuscode)
            {
                case "0":
                    messageID = "Successfully delivered";
                    IsSent = true;
                    messageID = result.Substring(result.IndexOf(':') + 1, result.Length - result.IndexOf(':') - 1);
                    break;
                case "10001":
                    //messageID = result.Substring(result.IndexOf(':') + 1, result.Length - result.IndexOf(':') - 1);.
                    messageID = "Username / Password incorrect";
                    IsSent = false;
                    break;
                case "10002":
                    messageID = "Contract expired";
                    IsSent = false;
                    break;
                case "10003":
                    messageID = "User Credit expired";
                    IsSent = false;
                    break;
                case "10004":
                    messageID = "User disabled";
                    IsSent = false;
                    break;
                case "10005":
                    messageID = "Service is temporarily unavailable";
                    IsSent = false;
                    break;
                case "10006":
                    messageID = "The specified message does not conform to DTD";
                    IsSent = false;
                    break;
                case "10007":
                    messageID = "unknown request";
                    IsSent = false;
                    break;
                case "10008":
                    messageID = "Invalid URL Error, This means that one of the parameters was not provided or left blank";
                    IsSent = false;
                    break;
                case "20001":
                    messageID = "Unknown number, Number not activated, Invalid destination number length. Destination number not numeric";
                    IsSent = true;
                    break;
                case "20002":
                    messageID = "Switched OFF or Out of Range";
                    IsSent = false;
                    break;
                case "20003":
                    messageID = "Message waiting list full, Handset memory full";
                    IsSent = true;
                    break;
                case "20004":
                    messageID = "Unknown equipment, Illegal equipment, Equipment not Short message equipped";
                    IsSent = true;
                    break;
                case "20005":
                    messageID = "System failure, Far end network error, No Paging response, Temporary network not available";
                    IsSent = false;
                    break;
                case "20006":
                    messageID = "Teleservice not provisioned, Call barred, Operator barring";
                    IsSent = true;
                    break;
                case "20007":
                    messageID = "Sender-ID mismatch";
                    IsSent = false;
                    break;
                case "20008":
                    messageID = "Dropped Messages";
                    IsSent = false;
                    break;
                case "20009":
                    messageID = "Number registered in NDNC";
                    IsSent = true;
                    break;
                case "20010":
                    messageID = "Destination number empty";
                    IsSent = false;
                    break;
                case "20011":
                    messageID = "Sender address empty";
                    IsSent = false;
                    break;
                case "20012":
                    messageID = "SMS over 160 character, Non-compliant message";
                    IsSent = true;
                    break;
                case "20013":
                    messageID = "UDH is invalid";
                    IsSent = false;
                    break;
                case "20014":
                    messageID = "Coding is invalid";
                    IsSent = false;
                    break;
                case "20015":
                    messageID = "SMS text is empty";
                    IsSent = true;
                    break;
                case "20016":
                    messageID = "Invalid Sender Id";
                    IsSent = false;
                    break;
                case "20017":
                    messageID = "Invalid message, Duplicate message ,Submit failed";
                    IsSent = false;
                    break;
                case "20018":
                    messageID = "Invalid Receiver ID (will validate Indian mobile numbers only.)";
                    IsSent = true;
                    break;
                case "20019":
                    messageID = "Invalid Date time for message Schedule (If the date specified in message post for schedule delivery is less than current date or more than expiry date or more than 1 year)";
                    IsSent = false;
                    break;
                case "20020":
                    messageID = "Message text is invalid";
                    IsSent = false;
                    break;
                case "20021":
                    messageID = "Aggregator Id mismatch for template and sender-ID";
                    IsSent = false;
                    break;
                case "20022":
                    messageID = "Noise Word Found in Promotional Message";
                    IsSent = false;
                    break;
                case "20023":
                    messageID = "Invalid Campaign";
                    IsSent = false;
                    break;
                case "40001":
                    messageID = "Message delivered successfully";
                    IsSent = true;
                    break;
                case "40002":
                    messageID = "Message failed";
                    IsSent = false;
                    break;
                case "40003":
                    messageID = "Message ID is invalid";
                    IsSent = false;
                    break;
                case "40004":
                    messageID = "Command Completed Successfully";
                    IsSent = false;
                    break;
                case "40005":
                    messageID = "HTTP disabled";
                    IsSent = false;
                    break;
                case "40006":
                    messageID = "Invalid Port";
                    IsSent = false;
                    break;
                case "40007":
                    messageID = "Invalid Expiry minutes";
                    IsSent = false;
                    break;
                case "40008":
                    messageID = "Invalid Customer Reference Id";
                    IsSent = false;
                    break;
                case "40009":
                    messageID = "Invalid Bill Reference Id";
                    IsSent = false;
                    break;
                case "40010":
                    messageID = "Email Delivery Disabled";
                    IsSent = false;
                    break;
                case "40011":
                    messageID = "HTTPS disabled";
                    IsSent = false;
                    break;
                case "40012":
                    messageID = "Invalid operator id";
                    IsSent = false;
                    break;
                case "50001":
                    messageID = "Cannot update/delete schedule since it has already been processed";
                    IsSent = false;
                    break;
                case "50002":
                    messageID = "Cannot update schedule since the new date-time parameter is incorrect";
                    IsSent = false;
                    break;
                case "50003":
                    messageID = "Invalid SMS ID/GUID";
                    IsSent = false;
                    break;
                case "50004":
                    messageID = "Invalid Status type for schedule search query. The status strings can be PROCESSED, PENDING and ERROR";
                    IsSent = false;
                    break;
                case "50005":
                    messageID = "Invalid date time parameter for schedule search query";
                    IsSent = false;
                    break;
                case "50006":
                    messageID = "Invalid GUID for GUID search query";
                    IsSent = false;
                    break;
                case "50007":
                    messageID = "Invalid command action";
                    IsSent = false;
                    break;
                case "60001":
                    messageID = "The number is in DND list";
                    IsSent = true;
                    break;
                case "60002":
                    messageID = "Insufficient Fund";
                    IsSent = false;
                    break;
                case "60003":
                    messageID = "Validity Expired";
                    IsSent = false;
                    break;
                case "60004":
                    messageID = "Credit back not required";
                    IsSent = false;
                    break;
                case "60005":
                    messageID = "Record is not there in accounts table";
                    IsSent = false;
                    break;
                case "60007":
                    messageID = "Message is accepted";
                    IsSent = false;
                    break;
                case "60008":
                    messageID = "Message validity period has expired";
                    IsSent = false;
                    break;
                case "99999":
                    messageID = "Unknown Error";
                    IsSent = false;
                    break;
                default:
                    messageID = "Unknown Error";
                    IsSent = false;
                    break;

            }


            smsResult = messageID;// statuscode;
            return smsResult;

        }

        #region "Contact Document Functionality"
       
        //Save contact document to the folder
        public static string SaveDocumentToFolder(HttpPostedFile file, string documentPath, int ClientId, int UserId, string FileName)
        {
            try
            {
                var b = new byte[file.ContentLength];
                string result = "";
                documentPath = documentPath + ClientId; //"\\" + ModuleName
                MemoryStream ms = new MemoryStream(b);
                // MemoryStream ms = new MemoryStream(file.ContentLength);
                bool IsExists = System.IO.Directory.Exists(documentPath);
                if (IsExists == false)
                {
                    System.IO.Directory.CreateDirectory(documentPath);
                }

                var path = System.IO.Path.Combine(documentPath, FileName); //file.FileName

                if (File.Exists(path))
                {
                    result = "File already Exists";
                    return result;
                }
                else
                {
                    file.SaveAs(documentPath + "/" + FileName); //file.FileName
                    ms.Close();

                    DocumentDTO DocumentDTO = new DocumentDTO();
                    DocumentDTO.ClientId = ClientId;
                    DocumentDTO.CreatedOn = System.DateTime.Now;
                    DocumentDTO.FileName = FileName;// file.FileName;
                    DocumentDTO.Path = ClientId + "/" + FileName;// file.FileName;
                    DocumentDTO.UserId = UserId;

                    GlobalSettings.LoggedInClientId = ClientId;
                    int PartnerId = ClientService.GetById(ClientId).PartnerId;
                    GlobalSettings.LoggedInPartnerId = PartnerId;
                    GlobalSettings.LoggedInUserId = UserId;

                    DocumentService.Create(DocumentDTO);
                    result = "File uploaded successfully";
                    return result;

                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }


        }

        //Delete contact document
        public static bool RemoveDocument(string documentPath)
        {

            try
            {
                if (File.Exists(documentPath))
                {
                    File.Delete(documentPath);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        //Delete folder
        public static bool DeleteFolder(string Folderpath)
        {
            try
            {
                if (Directory.Exists(Folderpath))
                {
                    string[] filePaths = Directory.GetFiles(Folderpath);
                    foreach (string filePath in filePaths)
                    {
                        File.Delete(filePath);
                    }


                    Directory.Delete(Folderpath);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }


        }

        //Return  list of contacts 
        public static List<ContactDTO> ReadExcelFile(int ClientId, string FilePath, bool IsValid)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            ClientDTO ClientDTO = new ClientDTO();
            ClientDTO = ClientService.GetById(ClientId);
            if (ClientDTO.IsActive != true)
            {
                return ContactDTOList;
            }

            try
            {

                if (File.Exists(FilePath))
                {
                    ContactDTOList = Import(FilePath, IsValid);
                    return ContactDTOList;
                }

                return ContactDTOList;
            }
            catch (Exception)
            {
                throw;
            }
        }
       
        //Import list of contacts
        private static List<ContactDTO> Import(string fname, bool IsValid)
        {
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            bool imported = false;
            //string[] ExcelSheetsCollection;
            try
            {
                string strFileName = fname.Trim();
                if ((strFileName != ""))
                {
                    //bool recordExist = false;
                    string FilePath = strFileName;
                    Connect(strFileName);
                    ContactDTOList = GetExcelSheetNames(strFileName, IsValid);
                }
                imported = true;

                return ContactDTOList;
            }
            catch (Exception)
            {

                imported = false;
                return ContactDTOList;

            }

        }

        /// <summary>         
        /// Connection string for connecting Excel sheet         
        /// </summary>         
        /// <param name="Sheetname"></param>         
        /// <remarks></remarks>         
        private static OleDbConnection Connect(string Sheetname)
        {
            OleDbConnection Con;
            // ' ''strConnection = ConfigurationManager.ConnectionStrings("ConnectionString").ToString ' "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Sheetname + ";Extended Properties=" + Convert.ToChar(34).ToString + "Excel 8.0;HDR=Yes;IMEX=2" + Convert.ToChar(34).ToString
            // strConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Sheetname + ";Extended Properties=" + Convert.ToChar(34).ToString + "Excel 12.0;HDR=Yes" + Convert.ToChar(34).ToString
            try
            {

                string strConnection = ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                            + (Sheetname + (";Extended Properties="
                            + (Convert.ToChar(34).ToString() + ("Excel 12.0;HDR=Yes" + Convert.ToChar(34).ToString())))));
                Con = new OleDbConnection();
                Con.ConnectionString = strConnection;

                return Con;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns the sheet collection
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>                  
        private static List<ContactDTO> GetExcelSheetNames(string strFileName, bool IsValidContact)
        {
            OleDbConnection Con = new OleDbConnection();
            Con = Connect(strFileName);
            DataTable dtTable;
            dtTable = new DataTable();
            DataTable dtTable1 = new DataTable();
            DataTable dtTable2 = new DataTable();
            List<ContactDTO> ContactDTOList = new List<ContactDTO>();
            try
            {
                Con.Open();

                dtTable = Con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                dtTable1 = Con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                dtTable2 = Con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if ((dtTable == null))
                {
                    return null;
                }


                int cnt = dtTable.Rows.Count;
                //string[cnt] ExcelSheets;  
                string[] ExcelSheets = new string[cnt];

                int i = 0, k = 0;
                int totalCount = 0;
                foreach (DataRow Row1 in dtTable1.Rows)
                {
                    string strTableName = Row1["TABLE_NAME"].ToString();
                    ExcelSheets[k] = strTableName.Substring(0, (strTableName.Length - 1)).ToString();
                    if (ExcelSheets[k].Contains("$"))
                    {
                        int count = ExcelSheets[k].IndexOf("$");
                        string s = ExcelSheets[k].Substring(0, count);
                        //s = GetStringValue2(s);
                        ExcelSheets[k] = s;

                    }

                    DataTable DataTable1 = new DataTable();
                    DataTable1 = GetDataTable(ExcelSheets[k], strFileName);
                    totalCount = totalCount + DataTable1.Rows.Count;
                    k++;
                }

                foreach (DataRow Row in dtTable.Rows)
                {
                    string strTableName = Row["TABLE_NAME"].ToString();
                    ExcelSheets[i] = strTableName.Substring(0, (strTableName.Length - 1)).ToString();
                    if (ExcelSheets[i].Contains("$"))
                    {
                        int count = ExcelSheets[i].IndexOf("$");
                        string s = ExcelSheets[i].Substring(0, count);
                        //s = GetStringValue2(s);
                        ExcelSheets[i] = s;

                    }

                    DataTable DataTable = new DataTable();
                    DataTable = GetDataTable(ExcelSheets[i], strFileName);

                    for (int j = 0; (j <= (DataTable.Rows.Count - 1)); j++)
                    {

                        ContactDTO ContactDTO = new ContactDTO();
                        string FirstName = null;
                        string LastName = null;

                        if (!Convert.IsDBNull(DataTable.Rows[j]["FirstName"].ToString()))
                        {
                            FirstName = DataTable.Rows[j]["FirstName"].ToString();
                            if (FirstName == "")
                            {
                                FirstName = null;
                            }
                            ContactDTO.FirstName = FirstName;
                        }
                        else
                        {
                            FirstName = null;
                            ContactDTO.FirstName = FirstName;
                        }

                        if (!Convert.IsDBNull(DataTable.Rows[j]["LastName"].ToString()))
                        {
                            LastName = DataTable.Rows[j]["LastName"].ToString();
                            if (LastName == "")
                            {
                                LastName = null;
                            }
                            ContactDTO.LastName = LastName;
                        }
                        else
                        {
                            LastName = null;
                            ContactDTO.LastName = LastName;
                        }

                        if (FirstName != null || LastName != null)
                        {
                            ContactDTO.Name = FirstName + " " + LastName;
                        }



                        if (FirstName != null && LastName != null)
                        {
                            ContactDTO.Name = FirstName + " " + LastName;
                            ContactDTO.IsValid = true;
                        }
                        else //if (ContactDTO.FirstName == null && ContactDTO.LastName == null)
                        {
                            ContactDTO.Name = FirstName + " " + LastName;
                            ContactDTO.IsValid = false;
                        }




                        //if (!Convert.IsDBNull(DataTable.Rows[j]["Contact Name"].ToString()))
                        //{
                        //    ContactDTO.Name = DataTable.Rows[j]["Contact Name"].ToString();
                        //}
                        //else
                        //{
                        //    ContactDTO.Name = "";
                        //}

                        if (!Convert.IsDBNull(DataTable.Rows[j]["Mobile Number"].ToString()))
                        {
                            ContactDTO.MobileNumber = DataTable.Rows[j]["Mobile Number"].ToString();
                            ContactDTO.IsMobileValid = true;
                            if (ContactDTO.MobileNumber.Length != 10)
                            {
                                ContactDTO.MobileNumber = ContactDTO.MobileNumber;
                                ContactDTO.IsValid = false;
                                ContactDTO.IsMobileValid = false;
                            }

                            bool IsValid = IsValidMobile(ContactDTO.MobileNumber);
                            if (IsValid != true)
                            {
                                ContactDTO.MobileNumber = ContactDTO.MobileNumber;
                                ContactDTO.IsValid = false;
                                ContactDTO.IsMobileValid = false;
                            }
                        }
                        else
                        {
                            ContactDTO.MobileNumber = ContactDTO.MobileNumber;
                            ContactDTO.IsValid = false;
                            ContactDTO.IsMobileValid = false;
                        }

                        if (!Convert.IsDBNull(DataTable.Rows[j]["Gender"].ToString()))
                        {
                            String Gender = DataTable.Rows[j]["Gender"].ToString().ToLower();

                            if (Gender.ToLower() == "male" || Gender.ToLower() == "female")
                            {
                                ContactDTO.Gender = DataTable.Rows[j]["Gender"].ToString();
                            }
                            else
                            {
                                ContactDTO.Gender = "Male";
                            }

                        }
                        else
                        {
                            ContactDTO.Gender = "Male";
                        }


                        if (!Convert.IsDBNull(DataTable.Rows[j]["Email Address"].ToString()))
                        {
                            ContactDTO.Email = DataTable.Rows[j]["Email Address"].ToString();
                            if (ContactDTO.Email == "")
                            {
                                ContactDTO.Email = null;
                            }
                        }
                        else
                        {
                            ContactDTO.Email = null;
                        }

                        if (ContactDTO.FirstName == null && ContactDTO.LastName == null && ContactDTO.MobileNumber == "" && ContactDTO.Email == null)
                        {
                            continue;
                        }

                        if (IsValidContact == true)
                        {

                            if (ContactDTO.IsValid == true)  //if (ContactDTO.Name != "" && ContactDTO.MobileNumber != "")
                            {
                                if (ContactDTO.Name.Length <= 51) //100
                                {
                                    ContactDTO.TotalCount = totalCount;
                                    ContactDTO.IsValid = true;
                                    ContactDTOList.Add(ContactDTO);
                                }
                                else
                                {
                                    ContactDTO.TotalCount = totalCount;
                                    ContactDTO.IsValid = false;
                                    ContactDTOList.Add(ContactDTO);
                                }


                            }
                        }
                        else
                        {
                            if (ContactDTO.IsValid == false)  //if (ContactDTO.Name != "" && ContactDTO.MobileNumber != "")
                            {
                                ContactDTO.TotalCount = totalCount;
                                ContactDTO.IsValid = false;
                                ContactDTOList.Add(ContactDTO);
                            }

                        }



                    }

                    i = (i + 1);
                }
                //return ExcelSheets;
                Con.Close();
                return ContactDTOList;

            }
            catch (Exception)
            {
                Con.Close();
                throw;
            }


        }

        private static bool IsValidMobile(string mobile)
        {
            
            try
            {
                bool IsValid = false;
                int length = mobile.Length;
                if (length == 10)
                {
                    IsValid = true;
                }
                else 
                { 
                    return false;
                }

                long mobileno = Convert.ToInt64(mobile);
                if (mobileno < 0)
                {
                    return false;                    
                }
                else
                {
                    return true;                     
                }

                return IsValid;
            }
            catch (Exception)
            {
                return false;                 
            }
        }

        /// <summary>
        /// Imports the excel sheet data into datatable
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="strFileName"></param>
        private static DataTable GetDataTable(string strTableName, string strFileName)
        {
            OleDbConnection Con = new OleDbConnection();
            Con = Connect(strFileName);
            int nRedCount = 0;
            bool bSucsess = false;
            try
            {

                DataTable dtTable = new DataTable();
                string strCommand;
                strTableName = RemoveDollerSign(strTableName);

                //if (strTableName.Contains("$")) 
                //{
                //    int count = strTableName.IndexOf("$");                
                //    string s = strTableName.Substring(0, count);                
                //    strTableName = s;
                //}


                if (!(strTableName == ""))
                {
                    strCommand = ("Select * from ["
                            + (strTableName + "$]"));
                    Con.Open();
                    OleDbDataAdapter daAdapter = new OleDbDataAdapter(strCommand, Con);
                    dtTable = new DataTable("Original");
                    daAdapter.Fill(dtTable);
                    if (((dtTable.Columns.Count == 3)
                            && (dtTable.Columns[0].ToString() == "Contact Name")))
                    {
                        //copytable(dtTable);  
                        Con.Close();
                        return dtTable;
                    }
                    Con.Close();
                    return dtTable;
                }
                Con.Close();
                return dtTable;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Con.Close();
            }
        }

        private static string RemoveDollerSign(string strTableName)
        {
            if (strTableName.Contains("$"))
            {

                int count = strTableName.IndexOf("$");
                string s = strTableName.Substring(0, count);
                strTableName = s;
            }
            return strTableName;
        }

        #endregion
       
        //Get count from comma separated receipent count
        public static int GetRecipientsCount(string RecipientsNumber)
        {
            int RecipientsCount = 0;

            try
            {


                // Input string contain separators.
                string value1 = RecipientsNumber;

                char[] delimiter1 = new char[] { ',', ';' };   // <-- Split on these
                // ... Use StringSplitOptions.RemoveEmptyEntries.
                string[] array2 = value1.Split(delimiter1,
                    StringSplitOptions.RemoveEmptyEntries);

                RecipientsCount = array2.Count();


                return RecipientsCount;

            }
            catch (Exception)
            {
                return RecipientsCount;
            }
        }

        #region "Activity Functionality"
   
        /// <summary>
        /// Returns the first day of the month for the given date in string format.
        /// </summary>
        /// <param name="self">"this" date</param>
        /// <returns>DateTime in string format representing the first day of the month</returns>
        public static string FirstDayOfMonth(DateTime self)
        {
            DateTime FirstDay = new DateTime(self.Year, self.Month, 1, 0, 0, 0, 0);
            return FirstDay.ToString();
        }

        /// <summary>
        /// Returns the last day of the month for the given date in string format.
        /// </summary>
        /// <param name="self">"this" date</param>
        /// <returns>DateTime in string format representing the last day of the month</returns>
        public static string LastDayOfMonth(DateTime self)
        {
            DateTime date = Convert.ToDateTime(self);
            DateTime LastDay = date.AddMonths(1).AddDays(-1);
            return LastDay.ToString();
        }

        //Get Client's Latest Activities
        public static List<ActivityLogDTO> GetClientLatestActivities(int ClientId, int Top)
        {
            List<ActivityLogDTO> ActivityLogDTOList = new List<ActivityLogDTO>();
            try
            {
                using (var uow = new UnitOfWork())
                {
                    // DateTime yesterdayDate = DateTime.Now.AddDays(-Top).Date;
                    //var LogList = uow.ActivityLogRepo.Get(c => c.Date > yesterdayDate).ToList();
                    var LogList = uow.ActivityLogRepo.GetAll().Where(e => e.ClientId == Convert.ToInt32(ClientId)).OrderByDescending(e => e.Id).ToList();
                    int i = 0;
                    int j = 0;
                    foreach (var log in LogList)
                    {
                        j = 0;
                        if (log.EntityType == "GroupContact" || log.EntityType == "CampaignLogXML" || log.EntityType == "CampaignLog" || log.EntityType == "Coupon")
                        {
                            continue;
                        }

                        if (log.EntityType == "Campaign") //&& log.OperationType == "Added" || log.OperationType == "Modified"
                        {
                            Campaign Campaign = new Campaign();
                            Campaign = uow.CampaignRepo.GetById(log.EntityId);
                            if (Campaign != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }


                                ActivityLogDTO.EntityName = Campaign.Name;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;

                            }
                        }


                        if (log.EntityType == "Client") //&& log.OperationType == "Added" || log.OperationType == "Modified"
                        {
                            Client Client = new Client();
                            Client = uow.ClientRepo.GetById(log.EntityId);
                            if (Client != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = Client.Company;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;

                            }
                        }

                        if (log.EntityType == "Contact") //&& log.OperationType == "Added" || log.OperationType == "Modified"
                        {
                            Contact Contact = new Contact();
                            Contact = uow.ContactRepo.GetById(log.EntityId);
                            if (Contact != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = Contact.Name;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;

                            }
                        }

                        if (log.EntityType == "CreditRequest") //&& log.OperationType == "Added" || log.OperationType == "Modified"
                        {
                            CreditRequest CreditRequest = new CreditRequest();
                            CreditRequest = uow.CreditRequestRepo.GetById(log.EntityId);
                            if (CreditRequest != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = "";// "Credit Request";// uow.CreditRequestRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;

                            }
                        }

                        if (log.EntityType == "Document")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            Document Document = new Document();
                            Document = uow.DocumentRepo.GetById(log.EntityId);
                            if (Document != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = Document.FileName;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;

                            }
                        }

                        if (log.EntityType == "EcouponCampaign")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            EcouponCampaign EcouponCampaign = new EcouponCampaign();
                            EcouponCampaign = uow.EcouponCampaignRepo.GetById(log.EntityId);
                            if (EcouponCampaign != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = EcouponCampaign.Title;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;

                            }
                        }

                        if (log.EntityType == "Group")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            Group Group = new Group();
                            Group = uow.GroupRepo.GetById(log.EntityId);
                            if (Group != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = Group.Name;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;

                            }
                        }

                        //if (log.EntityType == "Partner")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        //{
                        //    Partner Partner = new Partner();
                        //    Partner = uow.PartnerRepo.GetById(log.EntityId);
                        //    if (Partner != null)
                        //    {
                        //        ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                        //        if (log.UserId != null)
                        //        {
                        //            ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                        //        }

                        //        if (log.PartnerId != null)
                        //        {
                        //            ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                        //        }

                        //        if (log.ClientId != null)
                        //        {
                        //            ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                        //        }

                        //        ActivityLogDTO.EntityName = Partner.Name;
                        //        ActivityLogDTO.Date = log.Date;
                        //        ActivityLogDTO.EntityType = log.EntityType;
                        //        ActivityLogDTO.OperationType = log.OperationType;
                        //        ActivityLogDTO.Id = log.Id;
                        //        ActivityLogDTOList.Add(ActivityLogDTO);
                        //        j = 1;
                        //    }
                        //}

                        //if (log.EntityType == "Plan")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        //{
                        //    Plan Plan = new Plan();
                        //    Plan = uow.PlanRepo.GetById(log.EntityId);
                        //    if (Plan != null)
                        //    {
                        //        ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                        //        if (log.UserId != null)
                        //        {
                        //            ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                        //        }

                        //        if (log.PartnerId != null)
                        //        {
                        //            ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                        //        }

                        //        if (log.ClientId != null)
                        //        {
                        //            ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                        //        }

                        //        ActivityLogDTO.EntityName = Plan.Title;
                        //        ActivityLogDTO.Date = log.Date;
                        //        ActivityLogDTO.EntityType = log.EntityType;
                        //        ActivityLogDTO.OperationType = log.OperationType;
                        //        ActivityLogDTO.Id = log.Id;
                        //        ActivityLogDTOList.Add(ActivityLogDTO);
                        //        j = 1;
                        //    }
                        //}


                        if (log.EntityType == "RedeemedCount")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            RedeemedCount RedeemedCount = new RedeemedCount();
                            RedeemedCount = uow.RedeemedCountRepo.GetById(log.EntityId);
                            if (RedeemedCount != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = "";// "Redeemed Count";// uow.RedeemedCountRepo.GetById(Convert.ToInt32(log.UserId)).Title;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }


                        if (log.EntityType == "Setting")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            Setting Setting = new Setting();
                            Setting = uow.SettingRepo.GetById(log.EntityId);
                            if (Setting != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = "";// "Setting";//uow.SettingRepo.GetById(Convert.ToInt32(log.UserId)).Title;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }


                        if (log.EntityType == "Template")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            Template Template = new Template();
                            Template = uow.TemplateRepo.GetById(log.EntityId);
                            if (Template != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = Template.Title;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }

                        if (log.EntityType == "User")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            User User = new User();
                            User = uow.UserRepo.GetById(log.EntityId);
                            if (User != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = User.Name;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }

                        if (log.EntityType == "Location")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            Location Location = new Location();
                            Location = uow.LocationRepo.GetById(log.EntityId);
                            if (Location != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = Location.Name;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }



                        i = i + j;
                        if (i >= Top)
                        {
                            return ActivityLogDTOList;
                        }

                    }

                }
                return ActivityLogDTOList;

            }
            catch (Exception)
            {
                //return ActivityLogDTOList;
                throw;
            }
        }

        //Get Partner's Latest Activities
        public static List<ActivityLogDTO> GetPartnerLatestActivities(int PartnerId, int Top)
        {
            List<ActivityLogDTO> ActivityLogDTOList = new List<ActivityLogDTO>();
            try
            {
                using (var uow = new UnitOfWork())
                {
                    // DateTime yesterdayDate = DateTime.Now.AddDays(-Top).Date;
                    // var LogList = uow.ActivityLogRepo.Get(c => c.Date > yesterdayDate).ToList();
                    var LogList = uow.ActivityLogRepo.GetAll().Where(e => e.PartnerId == Convert.ToInt32(PartnerId)).OrderByDescending(e => e.Id).ToList();
                    int i = 0;
                    int j = 0;
                    foreach (var log in LogList)
                    {
                        j = 0;
                        if (log.EntityType == "GroupContact" || log.EntityType == "CampaignLogXML" || log.EntityType == "CampaignLog" || log.EntityType == "Coupon")
                        {
                            continue;
                        }

                        if (log.EntityType == "Client") //&& log.OperationType == "Added" || log.OperationType == "Modified"
                        {
                            Client Client = new Client();
                            Client = uow.ClientRepo.GetById(log.EntityId);
                            if (Client != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = Client.Company;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }



                        if (log.EntityType == "CreditRequest") //&& log.OperationType == "Added" || log.OperationType == "Modified"
                        {
                            CreditRequest CreditRequest = new CreditRequest();
                            CreditRequest = uow.CreditRequestRepo.GetById(log.EntityId);
                            if (CreditRequest != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = "";// "Credit Request";// uow.CreditRequestRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }



                        if (log.EntityType == "Partner")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            Partner Partner = new Partner();
                            Partner = uow.PartnerRepo.GetById(log.EntityId);
                            if (Partner != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = Partner.Name;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }

                        if (log.EntityType == "Plan")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            Plan Plan = new Plan();
                            Plan = uow.PlanRepo.GetById(log.EntityId);
                            if (Plan != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = Plan.Title;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }


                        if (log.EntityType == "Setting")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        {
                            Setting Setting = new Setting();
                            Setting = uow.SettingRepo.GetById(log.EntityId);
                            if (Setting != null)
                            {
                                ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                                if (log.UserId != null)
                                {
                                    ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                                }

                                if (log.PartnerId != null)
                                {
                                    ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                                }

                                if (log.ClientId != null)
                                {
                                    ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                                }

                                ActivityLogDTO.EntityName = "";// "Setting";// uow.SettingRepo.GetById(Convert.ToInt32(log.UserId)).Title;
                                ActivityLogDTO.Date = log.Date;
                                ActivityLogDTO.EntityType = log.EntityType;
                                ActivityLogDTO.OperationType = log.OperationType;
                                ActivityLogDTO.Id = log.Id;
                                ActivityLogDTOList.Add(ActivityLogDTO);
                                j = 1;
                            }
                        }

                        //if (log.EntityType == "SMSGateway")// && log.OperationType == "Added" || log.OperationType == "Modified")
                        //{
                        //    SMSGateway SMSGateway = new SMSGateway();
                        //    SMSGateway = uow.SMSGatewayRepo.GetById(log.EntityId);
                        //    if (SMSGateway != null)
                        //    {
                        //        ActivityLogDTO ActivityLogDTO = new ActivityLogDTO();
                        //        if (log.UserId != null)
                        //        {
                        //            ActivityLogDTO.User = uow.UserRepo.GetById(Convert.ToInt32(log.UserId)).Name;
                        //        }

                        //        if (log.PartnerId != null)
                        //        {
                        //            ActivityLogDTO.Partner = uow.PartnerRepo.GetById(Convert.ToInt32(log.PartnerId)).Name;
                        //        }

                        //        if (log.ClientId != null)
                        //        {
                        //            ActivityLogDTO.Client = uow.ClientRepo.GetById(Convert.ToInt32(log.ClientId)).Company;
                        //        }

                        //        ActivityLogDTO.EntityName = SMSGateway.Name;
                        //        ActivityLogDTO.Date = log.Date;
                        //        ActivityLogDTO.EntityType = log.EntityType;
                        //        ActivityLogDTO.OperationType = log.OperationType;
                        //        ActivityLogDTO.Id = log.Id;
                        //        ActivityLogDTOList.Add(ActivityLogDTO);
                        //        j = 1;
                        //    }
                        //}





                        i = i + j;
                        if (i >= Top)
                        {
                            return ActivityLogDTOList;
                        }

                    }

                }
                return ActivityLogDTOList;
            }
            catch (Exception)
            {
                //return ActivityLogDTOList;
                throw;
            }
        }
        
        #endregion
        
        public static List<GroupDTO> GetGroupList(string GroupIdList)
        {
            int GroupCount = 0;
            List<GroupDTO> GroupDTOList = new List<GroupDTO>();

            try
            {


                // Input string contain separators.
                string value1 = GroupIdList;

                char[] delimiter1 = new char[] { ',', ';' };   // <-- Split on these
                // ... Use StringSplitOptions.RemoveEmptyEntries.
                string[] array2 = value1.Split(delimiter1,
                    StringSplitOptions.RemoveEmptyEntries);
                GroupCount = array2.Count();
                if (GroupCount > 0)
                {
                    foreach (string GrpId in array2)
                    {
                        int GroupId = Convert.ToInt32(GrpId);
                        GroupDTO GroupDTO = new GroupDTO();
                        GroupDTO = GroupService.GetById(GroupId);
                        GroupDTOList.Add(GroupDTO);

                    }
                }


                return GroupDTOList;

            }
            catch (Exception)
            {
                return GroupDTOList;
            }
        }

        public static List<ContactColumnDTO> GetContactColumnNames()
        {
            List<ContactColumnDTO> ContactColumnDTOList = new List<ContactColumnDTO>();
            string query = "SELECT COLUMN_NAME FROM MsgBlasterTemp4May.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Contacts'";
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["msgBlasterWebContext"].ToString());
            SqlCommand cmd = new SqlCommand(query, Con);
            try
            {
                Con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        ContactColumnDTO ContactColumnDTO = new ContactColumnDTO();
                        ContactColumnDTO.Name = reader.GetValue(i).ToString();
                        if (ContactColumnDTO.Name != "Id" && ContactColumnDTO.Name != "ClientId")
                        {
                            ContactColumnDTOList.Add(ContactColumnDTO);
                        }
                    }
                }
                Con.Close();
                return ContactColumnDTOList;
            }
            catch (Exception)
            {
                Con.Close();
                throw;
            }
            finally
            {
                Con.Close();
            }

        }

        public static string ReadXMLTemplate()
        {
            string ReadXMLTemplate = null;

            string sourceQFile = null;
            try
            {


                sourceQFile = ConfigurationManager.AppSettings["XMLTemplateQDIR"].ToString();

                if (File.Exists(sourceQFile))
                {
                    FileStream fs = new FileStream(sourceQFile, FileMode.Open, FileAccess.Read);
                    StreamReader sread = new System.IO.StreamReader(fs);
                    sread.BaseStream.Seek(0, SeekOrigin.Begin);
                    while (sread.Peek() > -1)
                    {
                        ReadXMLTemplate += sread.ReadLine();
                    }
                    sread.Close();
                }
                else
                {
                    ReadXMLTemplate = "";
                }


                return ReadXMLTemplate.ToString();
            }
            catch (Exception)
            {
                ReadXMLTemplate = "";
            }
            return ReadXMLTemplate;

        }

        //public static string CreatePacket(int ClientId, int CampaignId, bool IsCampaign)
        //{
        //    string TemplatePacket = ReadXMLTemplate();
        //    string NewPacket = null;
        //    if (IsCampaign == true)
        //    {

        //        // Create Campaign Packet
        //        ClientDTO ClientDTO = ClientService.GetById(ClientId);
        //        CampaignDTO CampaignDTO = CampaignService.GetById(CampaignId);
        //        TemplatePacket = TemplatePacket.Replace("[clientId]", CampaignDTO.ClientId.ToString());
        //        NewPacket = TemplatePacket.Replace("[campaignId]", CampaignId.ToString());
        //        //SMSGatewayDTO SMSGatewayDTO = SMSGatewayService.GetById(ClientDTO.SMSGatewayId);
        //        string sender = "";
        //        if (ClientDTO.SenderCode != null && ClientDTO.SenderCode != "")
        //        {
        //            sender = ClientDTO.SenderCode;
        //        }
        //        else
        //        {

        //            sender = "022751"; 
        //        }

        //        NewPacket = NewPacket.Replace("[sender]", sender);
        //        NewPacket = NewPacket.Replace("[campaignname]", CampaignDTO.Name);

        //        string numbers = CampaignDTO.RecipientsNumber;
        //        string value1 = numbers;

        //        char[] delimiter1 = new char[] { ',' };   // <-- Split on these
        //        // ... Use StringSplitOptions.RemoveEmptyEntries.
        //        string[] array2 = value1.Split(delimiter1,
        //            StringSplitOptions.RemoveEmptyEntries);
        //        string MobileNumber = null;
        //        //Console.WriteLine();
        //        foreach (string item in array2)
        //        {

        //            MobileNumber = MobileNumber + "<number>" + item + "</number>";

        //        }



        //        NewPacket = NewPacket.Replace("[number]", MobileNumber);
        //        NewPacket = NewPacket.Replace("[message]", CampaignDTO.Message);

        //        if (CampaignDTO.IsScheduled == true)
        //        {
        //            DateTime ScheduledDate = CampaignDTO.ScheduledDate.Date;
        //            DateTime Time;
        //            if (CampaignDTO.ScheduledTime != "")
        //            {
        //                Time = Convert.ToDateTime(CampaignDTO.ScheduledTime);
        //            }
        //            else Time = Convert.ToDateTime("12:00 AM");

        //            ScheduledDate = Convert.ToDateTime(ScheduledDate.Date.ToString("MM/dd/yyyy") + " " + Time.TimeOfDay);
        //            NewPacket = NewPacket.Replace("[scheduleddate]", ScheduledDate.ToString());
        //        }
        //        else
        //        {
        //            NewPacket = NewPacket.Replace("[scheduleddate]", System.DateTime.Now.ToString());
        //        }



        //    }
        //    else
        //    {
        //        // Create Ecoupon Campaign Packet
        //        ClientDTO ClientDTO = ClientService.GetById(ClientId);
        //        EcouponCampaignDTO EcouponCampaignDTO = EcouponCampaignService.GetById(CampaignId);
        //        TemplatePacket = TemplatePacket.Replace("[clientId]", EcouponCampaignDTO.ClientId.ToString());
        //        NewPacket = TemplatePacket.Replace("[campaignId]", CampaignId.ToString());

        //        string sender = "";
        //        if (ClientDTO.SenderCode != null && ClientDTO.SenderCode != "")
        //        {
        //            sender = ClientDTO.SenderCode;
        //        }
        //        else
        //        {

        //            sender = "022751";
        //        }

        //        //SMSGatewayDTO SMSGatewayDTO = SMSGatewayService.GetById(ClientDTO.SMSGatewayId);
        //        NewPacket = NewPacket.Replace("[sender]", sender);
        //        NewPacket = NewPacket.Replace("[campaignname]", EcouponCampaignDTO.Title);

        //        string numbers = EcouponCampaignDTO.ReceipentNumber;
        //        string value1 = numbers;

        //        char[] delimiter1 = new char[] { ',' };   // <-- Split on these
        //        // ... Use StringSplitOptions.RemoveEmptyEntries.
        //        string[] array2 = value1.Split(delimiter1,
        //            StringSplitOptions.RemoveEmptyEntries);
        //        string MobileNumber = null;
        //        //Console.WriteLine();
        //        foreach (string item in array2)
        //        {

        //            MobileNumber = MobileNumber + "<number>" + item + "</number>";

        //        }



        //        NewPacket = NewPacket.Replace("[number]", MobileNumber);
        //        NewPacket = NewPacket.Replace("[message]", EcouponCampaignDTO.Message);

        //        if (EcouponCampaignDTO.IsScheduled == true)
        //        {
        //            DateTime ScheduledDate = EcouponCampaignDTO.SendOn.Date;
        //            DateTime Time;
        //            if (EcouponCampaignDTO.ScheduleTime != "")
        //            {
        //                Time = Convert.ToDateTime(EcouponCampaignDTO.ScheduleTime);
        //            }
        //            else Time = Convert.ToDateTime("12:00 AM");

        //            ScheduledDate = Convert.ToDateTime(ScheduledDate.Date.ToString("MM/dd/yyyy") + " " + Time.TimeOfDay);
        //            NewPacket = NewPacket.Replace("[scheduleddate]", ScheduledDate.ToString());
        //        }
        //        else
        //        {
        //            NewPacket = NewPacket.Replace("[scheduleddate]", System.DateTime.Now.ToString());
        //        }
        //    }



        //    string xmlFileUploadPath = ConfigurationManager.AppSettings["XMLFileUploadPath"].ToString() + Guid.NewGuid() + "_" + System.DateTime.Now.ToString("MM-dd-yyyy_hh_mm_tt") + "_" + ClientId + ".xml";

        //    if (!File.Exists(xmlFileUploadPath))
        //    {

        //        using (StreamWriter sw = File.CreateText(xmlFileUploadPath))
        //        {
        //            sw.WriteLine(NewPacket);
        //        }
        //    }
        //    return NewPacket;


        //}

        //Resend Coupon
        
        public static bool ResendCoupon(string mobilenumber, string message, int ClientId)
        {
            
           string result = "";
           bool IsSent = false;
           if (message != "" && mobilenumber != "")// Check for empty message.
           {
               ClientDTO ClientDTO = new ClientDTO();
               ClientDTO = ClientService.GetById(ClientId);
               string Url = null;
               //SMSGatewayDTO SMSGatewayDTO = new SMSGatewayDTO();
               //SMSGatewayDTO = SMSGatewayService.GetById(ClientDTO.SMSGatewayId);
               //if (SMSGatewayDTO.Name == "Default")
               //{
               //    Url = ConfigurationManager.AppSettings["PromotionalGateWay"].ToString();
               //}
               //else
               //{
               //    Url = ConfigurationManager.AppSettings["TransactionalGateWay"].ToString();
               //}               
               if (ClientDTO.SenderCode != null && ClientDTO.SenderCode != "")
               {
                   Url = ConfigurationManager.AppSettings["TransactionalGateWay"].ToString();
               }
               else 
               { 
                   
                   Url = ConfigurationManager.AppSettings["PromotionalGateWay"].ToString(); 
               }
               
               Url = Url.Replace("%26", "&");
               Url = Url.Replace("[recipient]", mobilenumber);
               Url = Url.Replace("[message]", message);
               Url = Url.Replace("[gateway]", ClientDTO.SenderCode);   //SMSGatewayDTO.Name

               HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
               myRequest.Method = "GET";
               WebResponse myResponse = myRequest.GetResponse();
               StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
               result = sr.ReadToEnd();
               sr.Close();

               string statuscode = "";
               if (result.Contains('|'))
                   statuscode = result.Substring(0, result.IndexOf('|'));
               else
                   statuscode = result;

               SMSResult(result);
               myResponse.Close();

               if (statuscode == "1701" || statuscode == "1705" || statuscode == "1706" || statuscode == "1032")
               {
                   IsSent = true;
               }
               else
               {
                   IsSent = false;

                   string UrlWhiz = "";
                   string resultWhiz = "";
                   if (ClientDTO.SenderCode != null && ClientDTO.SenderCode != "")
                   {
                       UrlWhiz = ConfigurationManager.AppSettings["TransactionalGateWayWhiz"].ToString();
                   }
                   else
                   {

                       UrlWhiz = ConfigurationManager.AppSettings["PromotionalGateWayWhiz"].ToString();
                   }

                   UrlWhiz = UrlWhiz.Replace("%26", "&");
                   UrlWhiz = UrlWhiz.Replace("[recipient]", mobilenumber);
                   UrlWhiz = UrlWhiz.Replace("[message]", message);
                   UrlWhiz = UrlWhiz.Replace("[gateway]", ClientDTO.SenderCode);   //SMSGatewayDTO.Name

                   HttpWebRequest myRequesWhiz = (HttpWebRequest)WebRequest.Create(UrlWhiz);
                   myRequesWhiz.Method = "GET";
                   WebResponse myResponseWhiz = myRequesWhiz.GetResponse();
                   StreamReader srWhiz = new StreamReader(myResponseWhiz.GetResponseStream(), System.Text.Encoding.UTF8);
                   resultWhiz = srWhiz.ReadToEnd();
                   srWhiz.Close();

                   if (resultWhiz.Contains('|'))
                   {
                       //statuscode = result.Substring(0, result.IndexOf('|'));
                       statuscode = "";
                       string[] words = resultWhiz.Split('|');
                       foreach (string word in words)
                       {
                           Console.WriteLine(word);
                           try
                           {
                               int code = Convert.ToInt32(word);

                               statuscode = code.ToString();
                           }
                           catch (Exception)
                           {
                               string code = word.Replace(" ", "");
                               if (code == "Success")
                               {
                                   code = "0";
                                   statuscode = code.ToString();
                                   IsSent = true;
                               }
                               else
                               {
                                   continue;
                               }
                           }
                       }

                   }
                   else
                   {
                       statuscode = resultWhiz;
                   }


               }
           }
           return IsSent;           

        }


        public static CouponDTO ResendCouponByCouponDTOAndClientId(CouponDTO CouponDTO, int ClientId)
        {

            SettingDTO SettingDTO = new SettingDTO();
            SettingDTO = SettingService.GetById(1);
            double RequiredCredit = CommonService.GetMessageCount(CouponDTO.Message);
            double ActualRequiredCredits = RequiredCredit * SettingDTO.NationalCouponSMSCount;
            int OldId = 0;

            OldId = CouponDTO.Id;
            CouponDTO CouponDTONew = new CouponDTO();
            CouponDTONew = null;

            string result = "";
            bool IsSent = false;
            if (CouponDTO.Message != "" && CouponDTO.MobileNumber != "")// Check for empty message.
            {
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(ClientId);
                string Url = null;
                              
                if (ClientDTO.SenderCode != null && ClientDTO.SenderCode != "")
                {
                    Url = ConfigurationManager.AppSettings["TransactionalGateWay"].ToString();
                }
                else
                {

                    Url = ConfigurationManager.AppSettings["PromotionalGateWay"].ToString();
                }

                Url = Url.Replace("%26", "&");
                Url = Url.Replace("[recipient]", CouponDTO.MobileNumber);
                Url = Url.Replace("[message]", CouponDTO.Message);
                Url = Url.Replace("[gateway]", ClientDTO.SenderCode);   //SMSGatewayDTO.Name

                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                result = sr.ReadToEnd();
                sr.Close();

                string statuscode = "";
                if (result.Contains('|'))
                    statuscode = result.Substring(0, result.IndexOf('|'));
                else
                    statuscode = result;

                SMSResult(result);
                myResponse.Close();

                if (statuscode == "1701" || statuscode == "1705" || statuscode == "1706" || statuscode == "1032")
                {
                    IsSent = true;
                    
                    CouponDTO.SentDateTime = System.DateTime.Now;
                    CouponDTO.Id = 0;
                    CouponDTO.IsExpired = false;
                    int NewCouponId = CouponService.Create(CouponDTO);
                  
                    CouponDTONew = CouponService.GetById(NewCouponId);

                    //Expire previous coupon
                    CouponDTO CouponDTOPrevious = new CouponDTO();
                    CouponDTOPrevious = CouponService.GetById(OldId);
                    CouponDTOPrevious.IsExpired = true;
                    CouponDTOPrevious.MessageId = result;
                    CouponService.Edit(CouponDTOPrevious);


                    // Modify EcouponCampaign message count 
                    EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                    EcouponCampaignDTO = EcouponCampaignService.GetById(CouponDTONew.EcouponCampaignId);
                    EcouponCampaignDTO.ReceipentNumber = EcouponCampaignDTO.ReceipentNumber + "," + CouponDTO.MobileNumber;
                    EcouponCampaignDTO.RecipientsCount = EcouponCampaignDTO.RecipientsCount + 1;
                    if (EcouponCampaignDTO.GroupId == 0)
                    {
                        EcouponCampaignDTO.GroupId = null;
                        EcouponCampaignDTO.Group = null;
                    }

                    EcouponCampaignDTO.RequiredCredits = CouponService.GetECouponCampaignRequiredCreditsByEcouponCampaignId(EcouponCampaignDTO.Id);

                    EcouponCampaignService.EditForEcouponResend(EcouponCampaignDTO);

                    //Modify client SMS credits
                    ClientDTO.SMSCredit = ClientDTO.SMSCredit - ActualRequiredCredits;// RequiredCredit;
                    ClientService.Edit(ClientDTO);


                }
                else
                {
                    IsSent = false;

                    string UrlWhiz = "";
                    string resultWhiz = "";
                    if (ClientDTO.SenderCode != null && ClientDTO.SenderCode != "")
                    {
                        UrlWhiz = ConfigurationManager.AppSettings["TransactionalGateWayWhiz"].ToString();
                    }
                    else
                    {

                        UrlWhiz = ConfigurationManager.AppSettings["PromotionalGateWayWhiz"].ToString();
                    }

                    UrlWhiz = UrlWhiz.Replace("%26", "&");
                    UrlWhiz = UrlWhiz.Replace("[recipient]", CouponDTO.MobileNumber);
                    UrlWhiz = UrlWhiz.Replace("[message]", CouponDTO.Message);
                    UrlWhiz = UrlWhiz.Replace("[gateway]", ClientDTO.SenderCode);   //SMSGatewayDTO.Name

                    HttpWebRequest myRequesWhiz = (HttpWebRequest)WebRequest.Create(UrlWhiz);
                    myRequesWhiz.Method = "GET";
                    WebResponse myResponseWhiz = myRequesWhiz.GetResponse();
                    StreamReader srWhiz = new StreamReader(myResponseWhiz.GetResponseStream(), System.Text.Encoding.UTF8);
                    resultWhiz = srWhiz.ReadToEnd();
                    srWhiz.Close();

                    if (resultWhiz.Contains('|'))
                    {
                        //statuscode = result.Substring(0, result.IndexOf('|'));
                        statuscode = "";
                        string[] words = resultWhiz.Split('|');
                        foreach (string word in words)
                        {
                            Console.WriteLine(word);
                            try
                            {
                                int code = Convert.ToInt32(word);

                                statuscode = code.ToString();
                            }
                            catch (Exception)
                            {
                                string code = word.Replace(" ", "");
                                if (code == "Success")
                                {
                                    code = "0";
                                    statuscode = code.ToString();
                                    IsSent = true;


                                    CouponDTO.SentDateTime = System.DateTime.Now;
                                    CouponDTO.Id = 0;
                                    CouponDTO.IsExpired = false;
                                    int NewCouponId = CouponService.Create(CouponDTO);
                                    
                                    CouponDTONew = CouponService.GetById(NewCouponId);

                                    //Expire previous coupon
                                    CouponDTO CouponDTOPrevious = new CouponDTO();
                                    CouponDTOPrevious = CouponService.GetById(OldId);
                                    CouponDTOPrevious.IsExpired = true;
                                    CouponDTOPrevious.MessageId = resultWhiz;
                                    CouponService.Edit(CouponDTOPrevious);


                                    // Modify EcouponCampaign message count 
                                    EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                                    EcouponCampaignDTO = EcouponCampaignService.GetById(CouponDTONew.EcouponCampaignId);
                                    EcouponCampaignDTO.ReceipentNumber = EcouponCampaignDTO.ReceipentNumber + "," + CouponDTO.MobileNumber;
                                    EcouponCampaignDTO.RecipientsCount = EcouponCampaignDTO.RecipientsCount + 1;
                                    if (EcouponCampaignDTO.GroupId == 0)
                                    {
                                        EcouponCampaignDTO.GroupId = null;
                                        EcouponCampaignDTO.Group = null;
                                    }

                                    EcouponCampaignDTO.RequiredCredits = CouponService.GetECouponCampaignRequiredCreditsByEcouponCampaignId(EcouponCampaignDTO.Id);

                                    EcouponCampaignService.EditForEcouponResend(EcouponCampaignDTO);

                                    //Modify client SMS credits
                                    ClientDTO.SMSCredit = ClientDTO.SMSCredit - ActualRequiredCredits;// RequiredCredit;
                                    ClientService.Edit(ClientDTO);

                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                    }
                    else
                    {
                        statuscode = resultWhiz;
                      
                    }


                }
            }
            return CouponDTONew;

        }


        public static DataTable SelectContatsInNumber(string number, int ClientId)
        {
            string cmdText = "SELECT * FROM Contacts WHERE (MobileNumber IN (" + number + ")) AND ClientId = "+ ClientId;
            return ExecuteNonQueryDt(cmdText);
        }

        public static DataTable ExecuteNonQueryDt(string cmdText)
        {
            DataTable dt = new DataTable("Table");
            SqlConnection con = new SqlConnection( ConfigurationManager.ConnectionStrings["msgBlasterWebContext"].ToString());
            using (con)
            {
                SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
                con.Open();
                da.Fill(dt);
                con.Close();
            }
            return dt;
        }
        
        public static string ReadXMLFile(string xmlFilePath)
        {
            string ReadXMLTemplate = null;

            string sourceQFile = null;
            try
            {


                sourceQFile = xmlFilePath;// ConfigurationManager.AppSettings["XMLTemplateQDIR"].ToString();

                if (File.Exists(sourceQFile))
                {
                    FileStream fs = new FileStream(sourceQFile, FileMode.Open, FileAccess.Read);
                    StreamReader sread = new System.IO.StreamReader(fs);
                    sread.BaseStream.Seek(0, SeekOrigin.Begin);
                    while (sread.Peek() > -1)
                    {
                        ReadXMLTemplate += sread.ReadLine();
                    }
                    sread.Close();
                }
                else
                {
                    ReadXMLTemplate = "";
                }


                return ReadXMLTemplate.ToString();
            }
            catch (Exception)
            {
                ReadXMLTemplate = "";
            }
            return ReadXMLTemplate;

        }

        /// <summary>
        /// Method to get unicode message count.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int GetUnicodeMessageCount(string message)
        {
            int specialCharCount = 0;

            specialCharCount += message.Count(f => f == '^');
            specialCharCount += message.Count(f => f == '{');
            specialCharCount += message.Count(f => f == '}');
            specialCharCount += message.Count(f => f == '\\');
            specialCharCount += message.Count(f => f == '[');
            specialCharCount += message.Count(f => f == ']');
            specialCharCount += message.Count(f => f == '|');
            specialCharCount += message.Count(f => f == '~');
            specialCharCount += message.Count(f => f == '€');


            int msgLength = message.Length + specialCharCount;


            // MaxSMSLength, SMSBlockLength these two valus come from database
            // Calculate the credits required to send this message.
            string sErrlocation = "Calculating message count-500";

            SettingDTO SettingDTO = new SettingDTO();
            SettingDTO = SettingService.GetById(1);
            int MAXSMSLENGTH = SettingDTO.UTFFirstMessageLength;
            int SMSBLOCKLENGTH = SettingDTO.UTFSecondMessageLength;


            if (msgLength <= MAXSMSLENGTH)
            {
                msgLength = 1;
                return 1;
            }
            else
            {
                if (msgLength <= 134)
                {
                    msgLength = 2;
                    return 2;
                }
                else
                {
                    if (msgLength % SMSBLOCKLENGTH != 0)
                        return (msgLength / SMSBLOCKLENGTH) + 1;
                    else
                        return msgLength / SMSBLOCKLENGTH;
                }
            }
        }

        //Remove duplicate Mobile numbers
        public static String RemoveDuplicateMobile(string MobileList)
        {
            string OriginalMobile = MobileList;
            string ModifiedMobileList = null;

            char[] delimiter1 = new char[] { ',', ';' };

            string[] array2 = OriginalMobile.Split(delimiter1, StringSplitOptions.RemoveEmptyEntries);
            ArrayList array1 = new ArrayList();
            int i = 0;
            foreach (string mobile in array2)
            {
                bool IsMobilevalid = IsValidMobile(mobile);
                if (IsMobilevalid == true)
                {
                    string mobilevalue = null;
                    mobilevalue = mobile.Replace(" ", null);
                    if (array1 != null)
                    {
                        if (array1.Contains(mobilevalue) == false)
                        {
                            array1.Add(mobilevalue);
                            if (i == 0)
                            {
                                ModifiedMobileList = mobilevalue;
                            }
                            else
                            {
                                ModifiedMobileList = ModifiedMobileList + "," + mobilevalue;
                            }
                            i = i + 1;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else 
                { 
                    continue; 
                }
            }
            return ModifiedMobileList;
        }

        //Get Consumed Credits for one message
        public static double GetConsumedCreditsForOneMessage(string Message, bool IsCoupon)
        {
            double ConsumedCredits = 0;
            int MessageCount = 0;

            SettingDTO SettingDTO = new SettingDTO();
            SettingDTO = SettingService.GetById(1);

            MessageCount = GetConsumedMessageCount(Message);
            if (IsCoupon == true)
            {
                ConsumedCredits = MessageCount * SettingDTO.NationalCouponSMSCount;
            }
            else
            {
                ConsumedCredits = MessageCount * SettingDTO.NationalCampaignSMSCount;
            }

            return ConsumedCredits;

        }
      
        //Get Consumed message count for one message
        public static int GetConsumedMessageCount(string message)
        {
            int specialCharCount = 0;

            specialCharCount += message.Count(f => f == '^');
            specialCharCount += message.Count(f => f == '{');
            specialCharCount += message.Count(f => f == '}');
            specialCharCount += message.Count(f => f == '\\');
            specialCharCount += message.Count(f => f == '[');
            specialCharCount += message.Count(f => f == ']');
            specialCharCount += message.Count(f => f == '|');
            specialCharCount += message.Count(f => f == '~');
            specialCharCount += message.Count(f => f == '€');


            int oldmsglength = message.Length;
            int messagelength = oldmsglength;
            //Check is message contans [FirstName], [LastName], [Code], [BirthDate], [AnniversaryDate], [Email], [MobileNumber], [Gender], [ExpiresOn]

            if (message.Contains("[FirstName]")) //|| Message.Contains("[LastName]") || Message.Contains("[Code]") || Message.Contains("[BirthDate]") || Message.Contains("[AnniversaryDate]") || Message.Contains("[Email]") || Message.Contains("[MobileNumber]") || Message.Contains("[Gender]") || Message.Contains("[ExpiresOn]") )
            {

                messagelength = messagelength + 14;
            }

            if (message.Contains("[LastName]")) //|| Message.Contains("[Code]") || Message.Contains("[BirthDate]") || Message.Contains("[AnniversaryDate]") || Message.Contains("[Email]") || Message.Contains("[MobileNumber]") || Message.Contains("[Gender]") || Message.Contains("[ExpiresOn]") )
            {

                messagelength = messagelength + 15;
            }

            if (message.Contains("[Code]")) // || Message.Contains("[BirthDate]") || Message.Contains("[AnniversaryDate]") || Message.Contains("[Email]") || Message.Contains("[MobileNumber]") || Message.Contains("[Gender]") || Message.Contains("[ExpiresOn]") )
            {

                //  messagelength = messagelength;                 
            }

            if (message.Contains("[BirthDate]")) //|| Message.Contains("[AnniversaryDate]") || Message.Contains("[Email]") || Message.Contains("[MobileNumber]") || Message.Contains("[Gender]") || Message.Contains("[ExpiresOn]") )
            {
                //  messagelength = messagelength;     
            }

            if (message.Contains("[AnniversaryDate]")) // || Message.Contains("[Email]") || Message.Contains("[MobileNumber]") || Message.Contains("[Gender]") || Message.Contains("[ExpiresOn]") )
            {
                messagelength = messagelength - 6;
            }

            if (message.Contains("[Email]")) // || Message.Contains("[MobileNumber]") || Message.Contains("[Gender]") || Message.Contains("[ExpiresOn]") )
            {
                messagelength = messagelength + 143;
            }

            if (message.Contains("[MobileNumber]")) // || Message.Contains("[Gender]") || Message.Contains("[ExpiresOn]") )
            {
                messagelength = messagelength - 4;
            }

            if (message.Contains("[Gender]")) // || Message.Contains("[ExpiresOn]") )
            {
                messagelength = messagelength - 2;
            }

            if (message.Contains("[ExpiresOn]")) // || Message.Contains("[ExpiresOn]") )
            {
                //messagelength = messagelength  ;
            }


            //int msgLength = message.Length + specialCharCount;

            int msgLength = messagelength + specialCharCount;

            //// MaxSMSLength, SMSBlockLength these two valus come from database
            //// Calculate the credits required to send this message.
            //string sErrlocation = "Calculating message count-500";

            SettingDTO SettingDTO = new SettingDTO();
            SettingDTO = SettingService.GetById(1);
            int MAXSMSLENGTH = SettingDTO.MessageLength;
            int SMSBLOCKLENGTH = SettingDTO.SingleMessageLength;


            if (msgLength <= MAXSMSLENGTH)
                return 1;
            else if (msgLength % SMSBLOCKLENGTH != 0)
                return (msgLength / SMSBLOCKLENGTH) + 1;
            else
                return msgLength / SMSBLOCKLENGTH;
        }

    }         

}

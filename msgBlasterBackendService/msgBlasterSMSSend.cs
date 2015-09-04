using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using MsgBlaster.Repo;
using MsgBlaster.Domain;
using MsgBlaster.DTO;
using MsgBlaster.Service;
using System.Configuration;
using System.Net;


namespace msgBlasterBackendService
{
    class msgBlasterSMSSend
    {
        //static int MAXMOBILELENGTH;
       
        static int MAXSMSLENGTH = 160;
        static int SMSBLOCKLENGTH = 153;
        //static int UPDATEBALAFTER;
        //static int MAXCHECKFORRESPONSE;
        //static int MAXCHECKFORCONNECTION;
        //const int MINMOBILELENGTH = 10;

        static void Main(string[] args)
        {
            
            try
            {
           
                //Read Campaign
                ReadCampaign();

            }
            catch (Exception ex)
            {

                using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\msgBlasterBackendService_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "  Main()" + " - " + ex.Message);
                    streamWriter.Close();
                }
 
            }
        }
        public static void ReadCampaign()
        {
            List<CampaignDTO> CampaignDTOList = new List<CampaignDTO>();
            CampaignDTOList = CampaignService.GetCampaignNotSentList();
            if (CampaignDTOList != null)
            {
                foreach (var item in CampaignDTOList)
                {
                    try
                    {
                        CampaignDTO CampaignDTO = new CampaignDTO();
                        CampaignDTO = item;
                        DateTime ScheduledDate = CampaignDTO.ScheduledDate.Date;
                        DateTime Time;
                        if (CampaignDTO.ScheduledTime != "")
                        {
                            Time = Convert.ToDateTime(CampaignDTO.ScheduledTime);
                        }
                        else Time = Convert.ToDateTime("12:00 AM");

                        ScheduledDate = Convert.ToDateTime(ScheduledDate.Date.ToString("MM/dd/yyyy") + " " + Time.TimeOfDay);

                        Console.WriteLine("Scheduled Time = " + ScheduledDate);

                        if (ScheduledDate <= System.DateTime.Now)
                        {
                            SplitMobile(item.RecipientsNumber, CampaignDTO);
                        }
                        else { }

                    }
                    catch (Exception ex)
                    {
                        using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\msgBlasterBackendService_Log.txt", FileMode.Append, FileAccess.Write))
                        {
                            StreamWriter streamWriter = new StreamWriter(file);
                            streamWriter.WriteLine(System.DateTime.Now + " - " + "  ReadCampaign()" + " - " + ex.Message);
                            streamWriter.Close();
                        }

                        continue;
                    }


                }
            }


            //Check Client Balance

            //Modify Client Balance
        }

        /// <summary>
        /// SplitFunction splits url. Url will be provided comma(,) separated
        /// </summary>
        /// <param name="url">accept url from app config</param>
        /// <returns></returns>
        /// 
        public static String SplitFunction(string url)
        {
            string result = "";

            try
            {
                string finalstring = "";

                // Input string contain separators.
                string value1 = url;

                char[] delimiter1 = new char[] { ',', ';' };   // <-- Split on these
                // ... Use StringSplitOptions.RemoveEmptyEntries.
                string[] array2 = value1.Split(delimiter1,
                    StringSplitOptions.RemoveEmptyEntries);

                //Console.WriteLine();
                foreach (string entry in array2)
                {
                    //Console.WriteLine(entry);
                    result = code(entry);
                    finalstring = finalstring + result;



                }
                result = finalstring;
            }

            catch (Exception ex)
            {

                result = "";

                using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\msgBlasterBackendService_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "  SplitFunction()" + " - " + ex.Message);
                    streamWriter.Close();
                }

            }
            return result;
        }
        /// <summary>
        /// It will return the url data as a string
        /// </summary>
        /// <param name="Url">url </param>
        /// <returns></returns>
        public static String code(string Url)
        {
            string result = "";
            string balance = "";           
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                result = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();

                balance = GetBalance(result);
                Console.WriteLine("Balance = " + balance);

                return balance;

            }
            catch (Exception ex)
            {

                balance = "";

                using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\msgBlasterBackendService_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "  code()" + " - " + ex.Message);
                    streamWriter.Close();
                }


            }

            return balance;
        }                
        /// <summary>
        /// It will return the balance as a string (i.e. BALANCE:14,089.66)
        /// </summary>
        /// <param name="balancestring"></param>
        /// <returns>balance as a string</returns>
        private static String GetBalance(string balancestring)
        {
            //balancestring = "BALANCE:14,089.66";
            string balance = "";
            if (balancestring == "PERMISSION DENIED")
            {
                balance = "PERMISSION DENIED";
            }
            else
            { 

                try 
                {
                    balance = balancestring.Remove(0  , 8);                    
                    balance = balancestring.Replace(",", "");
                    balance = balance.Replace("BALANCE:", "");
                }
                catch (Exception ex) 
                {
                    balance = "";

                    using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\msgBlasterBackendService_Log.txt", FileMode.Append, FileAccess.Write))
                    {
                        StreamWriter streamWriter = new StreamWriter(file);
                        streamWriter.WriteLine(System.DateTime.Now + " - " + "  GetBalance()" + " - " + ex.Message);
                        streamWriter.Close();
                    }

                }

            }
            return balance;
        }        
        public static float TransactionalBalance()
        {
            float TransBalance = 0;
            string Transbalance = code(ConfigurationManager.AppSettings["SMSGATEWAYTRANSBALANCEURL"].ToString());
            return TransBalance;           
        }
        public static float PromotionalBalance()
        {
            float PromoBalance = 0;
            string Promobalance = code(ConfigurationManager.AppSettings["SMSGATEWAYPROMOBALANCEURL"].ToString());
            return PromoBalance;
        }
             
        public static int ModifyClientBalance(int ClientId)
        {
            int SMSCredit = 0;

            return SMSCredit;
        }

        public static bool CheckCampainLogByCampaingIdAndMobile(int CampaignId, string Mobile) 
        {
            bool IsMessageSent = false;
            List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
            CampaignLogDTOList = CampaignLogService.GetCampaignLogListByCampaignIdAndMobile(CampaignId, Mobile);
                if(CampaignLogDTOList.Count() > 0)
                {
                    IsMessageSent= true;
                }else IsMessageSent= false;
            
            //if (CampaignLogDTOList != null)
            //        {
            //            foreach (var item in CampaignLogDTOList)
            //            {
                          
            //            }
            //        }


            return IsMessageSent;
        }

        public static String SplitMobile(string Mobile, CampaignDTO CampaignDTO)
        {
            string result = "";

            try
            {
                string finalstring = "";

                // Input string contain separators.
                string value1 = Mobile;

                char[] delimiter1 = new char[] { ',', ';' };   // <-- Split on these
                // ... Use StringSplitOptions.RemoveEmptyEntries.
                string[] array2 = value1.Split(delimiter1,
                    StringSplitOptions.RemoveEmptyEntries);

                //Console.WriteLine();
                foreach (string mobile in array2)
                {
                    Console.WriteLine(mobile);                    
                    bool isMessageSent = CheckCampainLogByCampaingIdAndMobile(CampaignDTO.Id, Mobile);                  
                   
                    ClientDTO ClientDTO = new ClientDTO();
                    ClientDTO = ClientService.GetById(CampaignDTO.ClientId);
                   
                    //SMSGatewayDTO SMSGatewayDTO = new SMSGatewayDTO();
                    //SMSGatewayDTO = SMSGatewayService.GetById(ClientDTO.SMSGatewayId);
                   
                    if (isMessageSent == false)
                    {
                        Console.Write("Send SMS");
                        // Check the Message required credits and actual client credits
                        string message = "";
                        CampaignDTO.Message = CampaignService.GetById(CampaignDTO.Id).Message;
                        //// macros                            
                               
                               List<Macros> MacrosList = Enum.GetValues(typeof(Macros)).Cast<Macros>().ToList();
                               ContactDTO ContactDTO = new ContactDTO();
                               ContactDTO = ContactService.GetContactByMobileNumberAndClientId(mobile, CampaignDTO.ClientId);
                              
                                 if (MacrosList.Count() > 0) 
                                    {
                                   foreach (var item in MacrosList)
                                   {    
 
                                       if (item.ToString() == "FirstName")
                                       {
                                           string FirstName = "";
                                           FirstName = CommonService.GetFirstname(ContactDTO.Name);
                                           CampaignDTO.Message = CampaignDTO.Message.Replace("[" + item.ToString() + "]", FirstName); 
                                       }

                                       if (item.ToString() == "LastName")
                                       {
                                           string LastName = "";
                                           LastName = CommonService.GetLastname(ContactDTO.Name);

                                           CampaignDTO.Message = CampaignDTO.Message.Replace("[" + item.ToString() + "]", LastName);
                                       }                                    
                                       
                                   }

                                   message = CampaignDTO.Message;

                                   CampaignDTO.Message = ReformatMsg(message);
                                   int SMSMsgCount = GetMessageCount(message);

                                     
                                   if (ClientDTO.SMSCredit >= SMSMsgCount)
                                   {
                                       if (CampaignLogService.GetCampaignLogListByCampaignIdAndMobile(CampaignDTO.Id, mobile).Count != 0)
                                       {
                                           continue;
                                       }

                                       //CampaignDTO.Message
                                       string sender = "";
                                       if (ClientDTO.SenderCode != null && ClientDTO.SenderCode != "")
                                       {
                                           sender = ClientDTO.SenderCode;
                                       }
                                       else
                                       {

                                           sender = "022751";
                                       }

                                       ActualSmsSend(mobile, CampaignDTO.Message, sender, CampaignDTO, ClientDTO);

                                   }
                                   else
                                   {
                                       goto nextprocess;
                                   }

                                  

                               }

                        //CampaignDTO.Message = ReformatMsg(message);
                        //int SMSMsgCount = GetMessageCount(message);
                        
                        //if (ClientDTO.SMSCredit >= SMSMsgCount)
                        //{
                        //     //CampaignDTO.Message
                        //    ActualSmsSend(mobile, CampaignDTO.Message, SMSGatewayDTO.Name, CampaignDTO, ClientDTO);

                        //}
                        //else 
                        //{
                        //    goto nextprocess;
                        //}

                    }
                    else 
                    { 
                                             
                      
                    }
                }

                // Modify Campaign IsSent status
                CampaignDTO.IsSent = true;
                CampaignDTO.Message = CampaignService.GetById(CampaignDTO.Id).Message;
                CampaignService.EditCampaignFromBackend(CampaignDTO);

                nextprocess:
                result = finalstring;
            }

            catch (Exception ex)
            {

                result = "";

                using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\msgBlasterBackendService_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "  SplitMobile()" + " - " + ex.Message);
                    streamWriter.Close();
                }

            }
            return result;
        }

        private static String ActualSmsSend(string mobilenumber, string message, string Gateway, CampaignDTO CampaignDTO, ClientDTO ClientDTO)
        {
            string result = "";

            int SMSMsgCount = GetMessageCount(message);
            message = MsgCorrect(message);

            if (CampaignDTO.MessageCount != SMSMsgCount)
            {
                CampaignDTO.MessageCount = SMSMsgCount;
            }

            if (message != "" && mobilenumber != "")// Check for empty message.
            {

                string Url = ConfigurationManager.AppSettings["TransactionalGateWay"].ToString();  
                Url = Url.Replace("%26", "&");
                Url = Url.Replace("[recipient]", mobilenumber);
                Url = Url.Replace("[message]", message);
                if (Gateway != "022751") //if (Gateway.ToLower() != "default")
                {
                    Url = Url.Replace("[gateway]", Gateway); //Gateway = "MSGBLS"
                }
                else
                {
                    Url="";
                    Url = ConfigurationManager.AppSettings["PromotionalGateWay"].ToString();
                    Url = Url.Replace("%26", "&");
                    Url = Url.Replace("[recipient]", mobilenumber);
                    Url = Url.Replace("[message]", message);
                }

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

                string SMSReplyMessage = SMSResult(statuscode) + "-" + result; //result
                myResponse.Close();


                CampaignLogDTO CampaignLogDTO = new CampaignLogDTO();                
                CampaignLogDTO.CampaignId = CampaignDTO.Id;
                CampaignLogDTO.RecipientsNumber = mobilenumber;
                CampaignLogDTO.MessageStatus = SMSReplyMessage;
                CampaignLogDTO.Message = message;
                CampaignLogDTO.GatewayID = Gateway;
                CampaignLogDTO.SentDateTime = System.DateTime.Now;
                

                CampaignLogDTO.MessageID = statuscode;
                CampaignLogDTO.MessageCount = SMSMsgCount;

                if (statuscode == "1701")
                {
                    CampaignLogDTO.IsSuccess = true;
                }
                else if (statuscode != "1701")
                {
                    CampaignLogDTO.IsSuccess = false;
                }

                CampaignLogService.Create(CampaignLogDTO);

                // Reduce SMS Credits From Clients List
                ClientDTO.SMSCredit = ClientDTO.SMSCredit - SMSMsgCount;// CampaignDTO.MessageCount;
                ClientService.Edit(ClientDTO);


              


            }

            return result;
        }

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
                    messageID = "Success";
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
                    messageID = "Response :" + result;
                    IsSent = false;
                    break;
            }


            smsResult = messageID ;// statuscode;
            return smsResult;

        }
        
        /// <summary>
        /// Method to get message count.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static int GetMessageCount(string message)
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
            MAXSMSLENGTH = SettingDTO.MessageLength;
            SMSBLOCKLENGTH = SettingDTO.SingleMessageLength;


            if (msgLength <= MAXSMSLENGTH)
                return 1;
            else if (msgLength % SMSBLOCKLENGTH != 0)
                return (msgLength / SMSBLOCKLENGTH) + 1;
            else
                return msgLength / SMSBLOCKLENGTH;
        }


        /// <summary>
        /// Method to get unicode message count.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static int GetUnicodeMessageCount(string message)
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
            MAXSMSLENGTH = 70;// SettingDTO.MessageLength;
            SMSBLOCKLENGTH = 67;// SettingDTO.SingleMessageLength;


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

        /// <summary>
        /// Reformate message text
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static string ReformatMsg(string msg)
        {
            if (string.IsNullOrEmpty(msg.Trim()))
            {
                return "";
            }
            msg = msg.Replace("&lt;", "<");
            msg = msg.Replace("&gt;", ">");
            msg = msg.Replace("&amp;", "&");
            msg = msg.Replace("&quot;", "\"");
            msg = msg.Replace("&apos;", "'");
            return msg;
        }

        /// <summary>
        /// Handles conversion of some characters so that they can be represented in url querystring
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static string MsgCorrect(string msg)
        {
            string sErrlocation = "conversion of some characters so that they can be represented in url querystring-Line 535";

            if (string.IsNullOrEmpty(msg.Trim()))
            {
                return "";
            }

            msg = msg.Replace("%", "%25");
            msg = msg.Replace("\"", "%22");// chr(34) = "
            msg = msg.Replace("&lt;", "%3C");
            msg = msg.Replace("&gt;", "%3E");
            msg = msg.Replace("&", "%26");
            msg = msg.Replace("+", "%2B");
            msg = msg.Replace("#", "%23");
            msg = msg.Replace("*", "%2A");
            msg = msg.Replace("!", "%21");
            msg = msg.Replace(",", "%2C");
            msg = msg.Replace("'", "%27");
            msg = msg.Replace("\\", "%5C");
            msg = msg.Replace("=", "%3D");

            return RemoveInvalidChar(msg);
        }

        /// <summary>
        /// Remove invalid charachters from message. i.e remove char with ascii value grater than 125
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static string RemoveInvalidChar(string msg)
        {
            string sErrlocation = "Remove invalid charachters from message. i.e remove char with ascii value grater than 125-Line 836";
            string formatedMsg = null;
            int asc_num = 0;
            if (string.IsNullOrEmpty(msg))
            {
                return formatedMsg;
            }
            string singlechar = null;
            for (int i = 0; i < msg.Length; i++) // Loop through each character of string
            {
                if (i != msg.Length) // check for is it last character of string or not
                    singlechar = msg.Substring(i, 1); // get single char from string
                else
                    singlechar = msg.Substring(i); // get last char

                asc_num = (int)Convert.ToChar(singlechar); // Get the ascci value for this char

                if (asc_num <= 125) // Check for char validation
                    formatedMsg += msg.Substring(i, 1);
            }
            return formatedMsg;
        }







        



    }
}

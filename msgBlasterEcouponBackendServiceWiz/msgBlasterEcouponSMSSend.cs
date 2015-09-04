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

namespace msgBlasterEcouponBackendServiceWiz
{
    class msgBlasterEcouponSMSSend
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
                ExpireCoupon();
                //Read Ecoupon EcouponCampaign
                ReadEcouponEcouponCampaign();

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

        public static void ExpireCoupon()
        {
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            CouponDTOList = CouponService.GetCouponListWhichNotExpired();
            if (CouponDTOList != null)
            {
                foreach (var item in CouponDTOList)
                {
                    EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                    EcouponCampaignDTO = EcouponCampaignService.GetById(item.EcouponCampaignId);
                    if (EcouponCampaignDTO.ExpiresOn != null)
                    {
                        if (EcouponCampaignDTO.ExpiresOn < System.DateTime.Now)
                        {
                            CouponDTO CouponDTO = new CouponDTO();
                            CouponDTO = item;// CouponService.GetById(item.Id);

                            CouponDTO.IsExpired = true;
                            CouponService.Edit(CouponDTO);

                        }
                    }

                }
            }
        }

        public static void ReadEcouponEcouponCampaign()
        {
            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();
            EcouponCampaignDTOList = EcouponCampaignService.GetEcouponCampaignNotSentList();
            if (EcouponCampaignDTOList != null)
            {
                foreach (var item in EcouponCampaignDTOList)
                {
                    try
                    {
                        EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                        EcouponCampaignDTO = item;
                        DateTime ScheduledDate = EcouponCampaignDTO.SendOn.Date;
                        DateTime Time;
                        if (EcouponCampaignDTO.ScheduleTime != "")
                        {
                            Time = Convert.ToDateTime(EcouponCampaignDTO.ScheduleTime);
                        }
                        else Time = Convert.ToDateTime("12:00 AM");

                        ScheduledDate = Convert.ToDateTime(ScheduledDate.Date.ToString("MM/dd/yyyy") + " " + Time.TimeOfDay);

                        Console.WriteLine("Scheduled Time = " + ScheduledDate);

                        if (ScheduledDate <= System.DateTime.Now)
                        {
                            //SplitMobile(item.ReceipentNumber, EcouponCampaignDTO);
                            if (item.GroupId == null && item.ForAllContact == false)
                            {
                                SplitMobile(item.ReceipentNumber, EcouponCampaignDTO);
                            }
                            else
                            {

                                string RecipientsNumberList = null;
                                if (item.GroupId > 0)
                                {
                                    GroupContactDTO GroupContactDTO = new GroupContactDTO();
                                    GroupDTO GroupDTO = new GroupDTO();
                                    GroupDTO = GroupService.GetById(Convert.ToInt32(item.GroupId));
                                    //GroupContactDTO = GroupService.GetGroupContactById(Convert.ToInt32(GroupDTO.Id));

                                    List<ContactDTO> ContactDTO = new List<ContactDTO>();
                                    ContactDTO = GroupContactService.GetGroupIdWiseContacts(GroupDTO.Id);

                                    foreach (var Contactitem in ContactDTO) //GroupContactDTO.Contacts
                                    {
                                        RecipientsNumberList = Contactitem.MobileNumber + "," + RecipientsNumberList;
                                    }
                                }
                                else if(item.ForAllContact == true)
                                {
                                    RecipientsNumberList = null;
                                    RecipientsNumberList = ContactService.GetAllReceipentNumberByClientId(item.ClientId);
                                    RecipientsNumberList = RecipientsNumberList + ",";
                                }

                                item.ReceipentNumber = RecipientsNumberList.Remove(RecipientsNumberList.LastIndexOf(','));
                                item.RecipientsCount = CommonService.GetRecipientsCount(item.ReceipentNumber);
                                EcouponCampaignService.EditEcouponCampaignFromBackend(item);

                                EcouponCampaignDTO EcouponCampaignGrpDTO = new EcouponCampaignDTO();
                                EcouponCampaignGrpDTO = item;
                                SplitMobile(item.ReceipentNumber, EcouponCampaignGrpDTO);
                            } 

                        }
                        else { }

                    }
                    catch (Exception ex)
                    {
                        using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\msgBlasterBackendService_Log.txt", FileMode.Append, FileAccess.Write))
                        {
                            StreamWriter streamWriter = new StreamWriter(file);
                            streamWriter.WriteLine(System.DateTime.Now + " - " + "  ReadEcouponEcouponCampaign()" + " - " + ex.Message);
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
                    balance = balancestring.Remove(0, 8);
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

        public static bool CheckCampainLogByCampaingIdAndMobile(int EcouponCampaignId, string Mobile)
        {
            bool IsMessageSent = false;
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            CouponDTOList = CouponService.GetCouponListByEcouponCampaignIdAndMobile(EcouponCampaignId, Mobile);
            if (CouponDTOList.Count() > 0)
            {
                IsMessageSent = true;
            }
            else IsMessageSent = false;

            //if (CouponDTOList != null)
            //        {
            //            foreach (var item in CouponDTOList)
            //            {

            //            }
            //        }


            return IsMessageSent;
        }

        public static String SplitMobile(string Mobile, EcouponCampaignDTO EcouponCampaignDTO)
        {
            string result = "";
            bool IsSent = false;
            int FalseCount = 0;
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
                    bool isMessageSent = CheckCampainLogByCampaingIdAndMobile(EcouponCampaignDTO.Id, Mobile);

                    ClientDTO ClientDTO = new ClientDTO();
                    ClientDTO = ClientService.GetById(EcouponCampaignDTO.ClientId);

                    //SMSGatewayDTO SMSGatewayDTO = new SMSGatewayDTO();
                    //SMSGatewayDTO = SMSGatewayService.GetById(ClientDTO.SMSGatewayId);

                    if (isMessageSent == false)
                    {
                        Console.Write("Send SMS");
                        //if (ClientDTO.SMSCredit > 0)
                        //{
                            int ecouponcode = 0;
                        createnew:
                            CommonService CommonService = new CommonService();
                            ecouponcode = CommonService.GetRandomNumber();
                            string ecouponcodelength = ecouponcode.ToString();
                            if (ecouponcodelength.Length < 6 || ecouponcodelength.Length > 6)
                            {
                                goto createnew;
                            }
                            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
                            CouponDTOList = CouponService.GetCouponListByCodeAndMobile(ecouponcode.ToString(), mobile);
                            if (CouponDTOList.Count == 0)
                            {
                                string Message = "";
                                EcouponCampaignDTO.Message = EcouponCampaignService.GetById(EcouponCampaignDTO.Id).Message;

                                //macros
                                List<Macros> MacrosList = Enum.GetValues(typeof(Macros)).Cast<Macros>().ToList();

                                ContactDTO ContactDTO = new ContactDTO();
                                ContactDTO = ContactService.GetContactByMobileNumberAndClientId(mobile, EcouponCampaignDTO.ClientId);

                                if (MacrosList.Count() > 0)
                                {
                                    foreach (var item in MacrosList)
                                    {

                                        if (item.ToString() == "FirstName")
                                        {
                                            string FirstName = "";
                                            FirstName = ContactDTO.FirstName;// CommonService.GetFirstname(ContactDTO.Name);
                                            EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", FirstName);
                                        }

                                        if (item.ToString() == "LastName")
                                        {
                                            string LastName = "";
                                            LastName = ContactDTO.LastName;// CommonService.GetLastname(ContactDTO.Name);
                                            EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", LastName);
                                        }

                                        if (item.ToString() == "BirthDate")
                                        {
                                            if (ContactDTO.BirthDate != null)
                                            {
                                                DateTime BirthDate = Convert.ToDateTime(ContactDTO.BirthDate);
                                                EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", BirthDate.ToString("dd-MMM"));
                                            }
                                            else { EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", ""); }
                                        }

                                        if (item.ToString() == "AnniversaryDate")
                                        {
                                            if (ContactDTO.AnniversaryDate != null)
                                            {
                                                DateTime AnniversaryDate = Convert.ToDateTime(ContactDTO.AnniversaryDate);
                                                EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", AnniversaryDate.ToString("dd-MMM"));
                                            }
                                            else { EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", ""); }
                                        }

                                        if (item.ToString() == "Email")
                                        {
                                            if (ContactDTO.Email != null)
                                            {
                                                string Email = ContactDTO.Email;
                                                EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", Email);
                                            }
                                            else { EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", ""); }
                                        }

                                        if (item.ToString() == "MobileNumber")
                                        {
                                            if (ContactDTO.MobileNumber != null)
                                            {
                                                string MobileNumber = ContactDTO.MobileNumber;
                                                EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", MobileNumber);
                                            }
                                            else { EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", ""); }
                                        }

                                        if (item.ToString() == "Gender")
                                        {
                                            if (ContactDTO.Gender != null)
                                            {
                                                string Gender = ContactDTO.Gender;

                                                //if (Gender == "0")
                                                //{
                                                //    Gender = "Male";
                                                //}
                                                //else
                                                //{
                                                //    Gender = "Female";
                                                //}

                                                EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", Gender);
                                            }
                                            else { EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", ""); }
                                        }


                                        if (item.ToString() == "ExpiresOn")
                                        {
                                            if (EcouponCampaignDTO.ExpiresOn != null)
                                            {
                                                DateTime ExpiresOn = Convert.ToDateTime(EcouponCampaignDTO.ExpiresOn);
                                                EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", ExpiresOn.ToString("dd-MMM-yy"));
                                            }
                                            else { EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", ""); }
                                        }


                                        if (item.ToString() == "Code")
                                        {
                                            string Code = ecouponcode.ToString();
                                            EcouponCampaignDTO.Message = EcouponCampaignDTO.Message.Replace("[" + item.ToString() + "]", Code);
                                        }

                                    }

                                    Message = EcouponCampaignDTO.Message;
                                    // Check the Message required credits and actual client credits
                                    double SMSMsgCount = GetMessageCount(Message);

                                    SettingDTO SettingDTO = new SettingDTO();
                                    SettingDTO = SettingService.GetById(1);
                                    SMSMsgCount = SMSMsgCount * SettingDTO.NationalCouponSMSCount;

                                    ////Check Credits
                                    //if (ClientDTO.SMSCredit >= SMSMsgCount)
                                    //{
                                        string sender = "";

                                        List<CouponDTO> CouponDTOListDuplicate = new List<CouponDTO>();
                                        CouponDTOListDuplicate = CouponService.GetCouponListByEcouponCampaignIdAndMobile(EcouponCampaignDTO.Id, mobile);
                                        if (CouponDTOListDuplicate.Count != 0)
                                        {
                                            ////If already sent then skip        
                                            continue;
                                            ////foreach (var item in CouponDTOListDuplicate)
                                            ////{
                                            ////    if (item.IsExpired != true)
                                            ////    {
                                            ////        string MobileDuplicate = null;
                                            ////        CouponDTO CouponDTO = new CouponDTO();
                                            ////        CouponDTO = item;
                                            ////        CouponDTO.IsExpired=true;
                                            ////        CouponService.Edit(CouponDTO);


                                            ////        MobileDuplicate = item.MobileNumber;
                                            ////        Message = item.Message;
                                            ////        ecouponcode = Convert.ToInt32(item.Code);


                                                    
                                            ////        if (ClientDTO.SenderCode != null && ClientDTO.SenderCode != "")
                                            ////        {
                                            ////            sender = ClientDTO.SenderCode;
                                            ////        }
                                            ////        else
                                            ////        {

                                            ////            sender = "022751";
                                            ////        }

                                            ////        IsSent = ActualSmsSend(mobile, Message, sender, EcouponCampaignDTO, ClientDTO, ecouponcode.ToString());
                                            ////        continue;
                                            ////    }

                                            ////}

                                        }
                                       
                                        if (ClientDTO.SenderCode != null && ClientDTO.SenderCode != "")
                                        {
                                            sender = ClientDTO.SenderCode;
                                        }
                                        else
                                        {

                                            sender = "022751";
                                        }

                                        IsSent = ActualSmsSend(mobile, Message, sender, EcouponCampaignDTO, ClientDTO, ecouponcode.ToString());
                                    //}
                                    //else goto nextprocess;

                                }

                                // Message = ReformatMsg(EcouponCampaignDTO.Message + " Your ecoupon code is " + ecouponcode + "");




                            }
                            else if (CouponDTOList.Count >= 1)
                            {
                                goto createnew;
                            }

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
               
                int TotatCouponSent = CouponService.GetCouponCountByEcouponCampaignId(EcouponCampaignDTO.Id);

                //if (EcouponCampaignDTO.RecipientsCount == TotatCouponSent)
                if (EcouponCampaignDTO.RecipientsCount <= TotatCouponSent)
                {                 
                    // Modify EcouponCampaign IsSent status
                    EcouponCampaignDTO.IsSent = true;
                    EcouponCampaignDTO.Message = EcouponCampaignService.GetById(EcouponCampaignDTO.Id).Message;
                    EcouponCampaignDTO.RequiredCredits = CouponService.GetECouponCampaignRequiredCreditsByEcouponCampaignId(EcouponCampaignDTO.Id);
                    EcouponCampaignDTO.CreditsDiffrence = EcouponCampaignDTO.ConsumedCredits - EcouponCampaignDTO.RequiredCredits;

                    if (EcouponCampaignDTO.ConsumedCredits != EcouponCampaignDTO.RequiredCredits)
                    {
                        if (EcouponCampaignDTO.CreditsDiffrence < 0)
                        {
                            //// deduct clients balance
                            ClientDTO ClientDTOUpdate = new ClientDTO();
                            ClientDTOUpdate = ClientService.GetById(EcouponCampaignDTO.ClientId);
                            ClientDTOUpdate.SMSCredit = ClientDTOUpdate.SMSCredit - (-(EcouponCampaignDTO.CreditsDiffrence));
                            ClientService.Edit(ClientDTOUpdate);

                            ////Reconcile Ecoupon Campaign
                            EcouponCampaignDTO.IsReconcile = true;
                            EcouponCampaignDTO.ReconcileDate = System.DateTime.Now;
                            EcouponCampaignService.EditEcouponCampaignFromBackend(EcouponCampaignDTO);

                        }
                        else if (EcouponCampaignDTO.CreditsDiffrence > 0)
                        {
                            ////Add clients balance
                            ClientDTO ClientDTOUpdate = new ClientDTO();
                            ClientDTOUpdate = ClientService.GetById(EcouponCampaignDTO.ClientId);
                            ClientDTOUpdate.SMSCredit = ClientDTOUpdate.SMSCredit + EcouponCampaignDTO.CreditsDiffrence;
                            ClientService.Edit(ClientDTOUpdate);

                            ////Reconcile Ecoupon Campaign
                            EcouponCampaignDTO.IsReconcile = true;
                            EcouponCampaignDTO.ReconcileDate = System.DateTime.Now;
                            EcouponCampaignService.EditEcouponCampaignFromBackend(EcouponCampaignDTO);

                        }
                    }
                    else if (EcouponCampaignDTO.CreditsDiffrence == 0)
                    {
                        EcouponCampaignDTO.IsReconcile = true;
                        EcouponCampaignDTO.ReconcileDate = System.DateTime.Now;
                        EcouponCampaignService.EditEcouponCampaignFromBackend(EcouponCampaignDTO);
                    }

                   // EcouponCampaignService.EditEcouponCampaignFromBackend(EcouponCampaignDTO);
                }

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

        private static bool ActualSmsSend(string mobilenumber, string message, string Gateway, EcouponCampaignDTO EcouponCampaignDTO, ClientDTO ClientDTO, string CouponCode)
        {
            string result = "";
            bool IsSent = false;
            int SMSMsgCount = GetMessageCount(message);
            message = MsgCorrect(message);


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
                    Url = "";
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
                //if (result.Contains('|'))
                //    statuscode = result.Substring(0, result.IndexOf('|'));
                //else
                //    statuscode = result;

                //string SMSReplyMessage = SMSResult(statuscode) + "-" + result; //result

                if (result.Contains('|'))
                {
                    //statuscode = result.Substring(0, result.IndexOf('|'));
                    statuscode = "";
                    string[] words = result.Split('|');
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
                    statuscode = result;
                }

                myResponse.Close();
                if (IsSent == true)
                { 
                CouponDTO CouponDTO = new CouponDTO();
                //CouponDTO.IsSuccess = true;
                CouponDTO.EcouponCampaignId = EcouponCampaignDTO.Id;
                CouponDTO.MobileNumber = mobilenumber;
                CouponDTO.Code = CouponCode;
                CouponDTO.IsRedeem = false;

                CouponDTO.MessageId = result;
                SettingDTO SettingDTO = new SettingDTO();
                SettingDTO = SettingService.GetById(1);
                double ActualSMSMsgCount = SettingDTO.NationalCouponSMSCount * SMSMsgCount;               
                CouponDTO.MessageCount = SMSMsgCount;
                CouponDTO.RequiredCredits = ActualSMSMsgCount;

                CouponDTO.Message = message;
                //CouponDTO.MessageStatus = SMSReplyMessage;
                //CouponDTO.GatewayID = Gateway;
                CouponDTO.SentDateTime = System.DateTime.Now;
                CouponDTO.IsCouponSent = true;
                //CouponDTO.MessageID = statuscode;

                //if (statuscode == "1701")
                //{
                //    CampaignLogDTO.IsSuccess = true;
                //}
                //else if (statuscode != "1701")
                //{
                //    CampaignLogDTO.IsSuccess = false;
                //}
                CouponService.Create(CouponDTO);

                //// Reduce SMS Credits From Client
                //ClientDTO.SMSCredit = ClientDTO.SMSCredit - ActualSMSMsgCount;  //SMSMsgCount;
                //ClientService.Edit(ClientDTO);


                }


            }

            return IsSent; // result;
        }

        private static string SMSResult(string result)
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

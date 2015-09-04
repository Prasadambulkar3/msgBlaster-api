using System.Collections.Generic;
using System.Text;
using System;
using System.Xml;
using System.IO;
using System.Net;
using System.Linq;
using System.Xml.Schema;
using System.Configuration;

using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;
using MsgBlaster.Service;

namespace msgBlasterCampaignSendbywihz
{
    public class QueueProcess
    {
        #region "Variable,Constent Declaration"
        // Store Gateway information
        string GATEWAY_URL = null;
        string GatewayID = null;

        // Get fullpath of folders
        public string SMSQDIR = ConfigurationManager.AppSettings["SMSQDIR_PATH"].ToString();
        public string BADQDIR = ConfigurationManager.AppSettings["INVALIDQDIR_PATH"].ToString();
        public string SENTQDIR = ConfigurationManager.AppSettings["SENTQDIR_PATH"].ToString();
        public static string LOGQDIR = ConfigurationManager.AppSettings["LOGQDIR_PATH"].ToString();
        public string APP_PATH = ConfigurationManager.AppSettings["APP_PATH"].ToString();

        // Constent 
        int MAXMOBILELENGTH;
        int MAXSMSLENGTH;
        int SMSBLOCKLENGTH;
        int UPDATEBALAFTER;
        int MAXCHECKFORRESPONSE;
        int MAXCHECKFORCONNECTION;
        const int MINMOBILELENGTH = 10;

        // Variables to Store campaignId,clientId & Sender
        public string strcampaignId, strclientId, strSender, campaignname, strSource;

        // To move file in specific folder
        public string sourceQFile;
        public string sentQFile;
        public string badQFile;
        public string baseFileName;

        // To read xml files
        public XmlDocument baseFileXML;
        public XmlDocument gatewayXML;
        public XmlDocument queuesettingsXML;

        MsgBlaster.DTO.ClientDTO ClientDTO;
        MsgBlaster.DTO.CampaignDTO CampaignDTO;

        public int creditsRequired;


        // To hold error locations & Error
        public string schemaValidationError;
        public string sErrlocation;
        bool IsValidGateway = false;

        public string xmlfiledata = null;
        #endregion

        #region "Read xml file From SMSQueue Folder and validate (i.e all the required validations are done in this part)"

        public void SendMessages()
        {
            try
            {
                //// Load Queue processor settings 
                //sErrlocation = "Loading QueueProcessor Settings From QueueProcessorSettings.xml file-Line 69";
                //Load settings from QueueProcessorSettings.xml file (File present in basedirectory of application.)
                queuesettingsXML = new XmlDocument();
                queuesettingsXML.Load(ConfigurationManager.AppSettings["SETTIGNDIR_PATH"].ToString() + "QueueProcessorSettings.xml");
                MAXCHECKFORCONNECTION = Convert.ToInt32(queuesettingsXML.DocumentElement["maxcheckforgatewqycon"].InnerText);
                MAXCHECKFORRESPONSE = Convert.ToInt32(queuesettingsXML.DocumentElement["maxcheckforgatewayresponse"].InnerText);
                UPDATEBALAFTER = Convert.ToInt32(queuesettingsXML.DocumentElement["updatebalafterevery"].InnerText);
                MAXSMSLENGTH = Convert.ToInt32(queuesettingsXML.DocumentElement["maxsmslength"].InnerText);
                SMSBLOCKLENGTH = Convert.ToInt32(queuesettingsXML.DocumentElement["smsblocklength"].InnerText);
                MAXMOBILELENGTH = Convert.ToInt32(queuesettingsXML.DocumentElement["maxmobilelength"].InnerText);

                sErrlocation = "Select All files from SMSQueue Folder - Line 112";
                string[] files = System.IO.Directory.GetFiles(SMSQDIR, "*.xml"); // Get all *.xml files from SMSQUEUE folder
                if (files.Length > 0)
                {
                    General.WriteInFile("\r\n" + "Total Number of Files = " + files.Length + "");
                    General.WriteInFile("=========================================================================================================");
                    foreach (string file in files)
                    {
                        baseFileName = System.IO.Path.GetFileName(file);
                        // Prepare file path for copying file to BAD or SENT folder depending on situation.
                        sentQFile = SENTQDIR + baseFileName;
                        sourceQFile = SMSQDIR + baseFileName;
                        badQFile = BADQDIR + baseFileName;

                        // Check for Xml File Validation against the Schema
                        if (!System.IO.File.Exists(ConfigurationManager.AppSettings["SETTIGNDIR_PATH"].ToString() + "XmlSchemaweb.xsd"))
                        {
                            General.WriteInFile("SMSQueue processor was terminated because XmlSchema.xsd file is not available in " + APP_PATH + " directory \r\n");
                            break;
                        }

                        sErrlocation = "File Validation against the Schema - Line 133";
                        if (!IsValidFile(sourceQFile))
                        {
                            MoveFileToBadFolder(schemaValidationError); // Invalid file
                            General.WriteInFile(baseFileName + " file moved to BAD folder because, this file is invalid according to Schema Validation.\r\n");
                            continue;
                        }

                        sErrlocation = "Load basefile for sending - Line 140";
                        baseFileXML = new XmlDocument();
                        baseFileXML.Load(sourceQFile); // load xml file from SMSQUEUE folder to process

                        strcampaignId = baseFileXML.DocumentElement["campaignId"].InnerText;
                        strclientId = baseFileXML.DocumentElement["clientId"].InnerText;
                        strSender = baseFileXML.DocumentElement["sender"].InnerText;
                        double number;
                        bool IsNumeric = double.TryParse(strSender, out number); // Check the number is in Numeric form or not
                        if (IsNumeric && strSender.Length == 10)
                            strSender = "91" + strSender;

                        campaignname = ReformatMsg(baseFileXML.DocumentElement["campaignname"].InnerText);

                        // Check for validation of recipient number count and message count
                        // i.e message count > 1 AND != recipient number count than it is invalid file.  
                        XmlNodeList MessageToSend = baseFileXML.SelectNodes("/packet/numbers/message");
                        XmlNodeList RecipientsToSendMessage = baseFileXML.SelectNodes("/packet/numbers/number");
                        if (MessageToSend.Count > 1 && MessageToSend.Count != RecipientsToSendMessage.Count)
                        {
                            // This indicates that Msg count & Number count doesnot match
                            MoveFileToBadFolder("Msg count & Number count doesnot match.");
                            General.WriteInFile(baseFileName + " file is moved to BAD folder because Msg count & Number count doesnot match.\r\n");
                            continue;
                        }

                        ClientDTO = new MsgBlaster.DTO.ClientDTO();
                        CampaignDTO = new MsgBlaster.DTO.CampaignDTO();

                        ClientDTO.Id = Convert.ToInt32(strclientId);
                        CampaignDTO.Id = Convert.ToInt32(strcampaignId);

                        ClientDTO = MsgBlaster.Service.ClientService.GetById(ClientDTO.Id);
                        CampaignDTO = MsgBlaster.Service.CampaignService.GetById(CampaignDTO.Id);



                        // Befor AddToSMSQueue() validate user
                        sErrlocation = "Check for Valid User - Line 195";
                        int result = ClientDTO.Id;// oClient.IsValidUser(oClient);
                        if (result == 0)// Invalid User
                        {
                            MoveFileToBadFolder("Invalid user.");
                            General.WriteInFile(baseFileName + " file  moved to BAD folder because of invalid campaignId & clientId.\r\n");
                            continue;
                        }

                        //if (result == 2) // Database Error
                        //{
                        //    General.WriteInFile(baseFileName + " file is skiped due to database error during User validation.\r\n");
                        //    continue;//skip this file and continue
                        //}

                        General.WriteInFile("Opening Balance = " + ClientDTO.SMSCredit);// ClientDBOperation.ActualCredits(oClient));
                        // Check for time restriction over message sending for this user
                        sErrlocation = "Check for Send time restriction - Line 214";
                        //double FROMTIME = Convert.ToDouble(oClient.SendFromTime.Replace(':', '.'));
                        //double UPTOTIME = Convert.ToDouble(oClient.SendUptoTime.Replace(':', '.'));
                        double currentTime = Convert.ToDouble(System.DateTime.Now.Hour + "." + System.DateTime.Now.Minute);
                        //if (currentTime < FROMTIME || currentTime > UPTOTIME)
                        //{
                        //    // skip this file due to time restriction.
                        //    General.WriteInFile(baseFileName + " file is skiped due to send time restriction.\r\n");// +
                        //    continue;
                        //}

                        if (baseFileXML.SelectSingleNode("//scheduleddate") != null)
                        {
                            DateTime ScheduledDate;
                            try
                            {
                                ScheduledDate = Convert.ToDateTime(baseFileXML.DocumentElement["scheduleddate"].InnerText);
                            }
                            catch (Exception ex)
                            {
                                MoveFileToBadFolder("Unrecognized scheduled date.");
                                General.WriteInFile(baseFileName + " file moved to BAD folder because of Unrecognized scheduled date.\r\n");
                                continue;
                            }
                            // 1 = Scheduled datetime is > current datetime
                            // 0 = Scheduled datetime = current datetime
                            // -1 = Scheduled datetime < current datetime
                            if (DateTime.Compare(ScheduledDate, System.DateTime.Now) == 1)
                            {
                                // This file will be send in futuer so skip this file
                                General.WriteInFile(baseFileName + " file is skiped due to scheduled datetime restriction.\r\n");
                                continue;
                            }
                        }
                        // Function for preapearing message to broadcast
                        AddToSMSQueue();
                    }
                    General.WriteInFile("========================================================================================================= ");
                }
            }
            catch (Exception ex)
            {
                General.WriteInFile(sErrlocation + "\r\n" + ex.Message);
                General.WriteInFile("========================================================================================================= ");
            }
        }

        #endregion

        #region "Formate And Send message to Gateway"

        /// <summary>
        /// This method get recipient number & corresponding message from campaign file & sent it to smsoutbox.in gateway
        /// </summary>
        public void AddToSMSQueue()
        {
            General.WriteInFile("Sending file : " + baseFileName);
            HttpWebRequest oHttpRequest;
            HttpWebResponse oHttpWebResponse;

            //GatewayInfo oGatewayInfo = new GatewayInfo(oClient.campaignId, oClient.IsInternal);

            if (ClientDTO.SenderCode != "" && ClientDTO.SenderCode != null)
            {
                GATEWAY_URL = ConfigurationManager.AppSettings["TransactionalGateWay"].ToString();
                GatewayID = "WizSMS";// ClientDTO.SenderCode;

                GATEWAY_URL = GATEWAY_URL.Replace("[gateway]", ClientDTO.SenderCode.ToString());
            }
            else
            {
                GATEWAY_URL = ConfigurationManager.AppSettings["PromotionalGateWay"].ToString();
                GatewayID = "WizSMS";// "563485";
            }

            GATEWAY_URL = GATEWAY_URL.Replace("%26", "&");

             
            //GatewayID = oGatewayInfo.GatewayId;                        


            string strURL;
            string sMobileNo = null, mymsg = null, msgtype = null;
            int recipientNumberCount = 0; // count of recipient Mobile number.
            int errorCount = 0; // Error count.
            int creditConsumed = 0; // Credit consumed to sent message.
            int sentMsgCount = 0; // this count indicates how many msg sent successfully
            int creditRequiredForSingleMsg = 0; // Credit required to send message in the case of single message to multiple recipients. 
            creditsRequired = 0;
            bool IsMultipleMsg;

            XmlNodeList MessageToSend = baseFileXML.SelectNodes("/packet/numbers/message");
            XmlNodeList RecipientsToSendMessage = baseFileXML.SelectNodes("/packet/numbers/number");
            recipientNumberCount = RecipientsToSendMessage.Count;

            // Check for is it an MsgBlaster Testing user or not
            ////if (oClient.IsInternal)
            ////{
            ////    General.WriteInFile(oClient.campaignId + " is a Graceworks team member.");
            ////}

            // check for packet contains multiple messages or not.
            if (MessageToSend.Count == RecipientsToSendMessage.Count && MessageToSend.Count != 1)
            {
                IsMultipleMsg = true;
            }
            else
            {
                IsMultipleMsg = false;
                // In the case of single msg to all recipents calculate the credit required to send this message
                if (CampaignDTO.IsUnicode == false)
                {
                    creditRequiredForSingleMsg = GetMessageCount(ReformatMsg(baseFileXML.GetElementsByTagName("message").Item(0).InnerText.TrimEnd()));
                }
                else creditRequiredForSingleMsg = GetUnicodeMessageCount(ReformatMsg(baseFileXML.GetElementsByTagName("message").Item(0).InnerText.TrimEnd()));
                mymsg = MsgCorrect(baseFileXML.GetElementsByTagName("message").Item(0).InnerText.TrimEnd(), CampaignDTO.IsUnicode);
            }

            foreach (XmlNode currentnode in baseFileXML.DocumentElement.GetElementsByTagName("number")) // loop through the each recipient number in this file
            {
                if (currentnode.Attributes.Count == 0) // check for this number message is send or not
                {
                    // Remove unwanted characters from number
                    sMobileNo = currentnode.InnerText.Replace(" ", "");
                    sMobileNo = sMobileNo.Replace("-", "");
                    sMobileNo = sMobileNo.Replace("(", "");
                    sMobileNo = sMobileNo.Replace(")", "");
                    sMobileNo = sMobileNo.Replace(",", "");
                    sMobileNo = sMobileNo.Replace("+", "");
                    double number;
                    bool IsNumeric = double.TryParse(sMobileNo, out number); // Check the number is in Numeric form or not
                    if (sMobileNo.Length < MINMOBILELENGTH || sMobileNo.Length > MAXMOBILELENGTH || !IsNumeric)
                    {
                        // Recipient numbers is invalid so skip this number
                        XmlAttribute errorStamp;
                        errorStamp = baseFileXML.CreateAttribute("notSent");
                        errorStamp.Value = "Invalid recipient number.";
                        currentnode.Attributes.Append(errorStamp);
                        sentMsgCount++;
                        continue;
                    }
                    else
                    {
                        sMobileNo = sMobileNo.Substring(sMobileNo.Length - MINMOBILELENGTH); // Get last 10 digits from number
                        sMobileNo = "91" + sMobileNo; // Add country code to Recipient number
                    }

                    if (IsMultipleMsg) // prepared separate message for this number
                    {
                        mymsg = MsgCorrect(currentnode.NextSibling.InnerText.TrimEnd(), CampaignDTO.IsUnicode);
                    }

                    if (mymsg == "")// Check for empty message.
                    {
                        // If message is empty than dont send this message & add resone why not send to that recipient number.
                        XmlAttribute errorStamp;
                        errorStamp = baseFileXML.CreateAttribute("notSent");
                        errorStamp.Value = "Empty message.";
                        currentnode.Attributes.Append(errorStamp);
                        sentMsgCount++;
                        continue;
                    }

                    int creditRequiredToSendMsg = 0;

                    if (IsMultipleMsg)
                        if (CampaignDTO.IsUnicode == false)
                        {
                            creditRequiredToSendMsg = GetMessageCount(ReformatMsg(currentnode.NextSibling.InnerText.TrimEnd()));
                        }
                        else
                        {
                            creditRequiredToSendMsg = GetUnicodeMessageCount(currentnode.NextSibling.InnerText.TrimEnd());
                        }

                        else
                            creditRequiredToSendMsg = creditRequiredForSingleMsg;

                    ////if (ClientDBOperation.ActualCredits(oClient) - creditConsumed < creditRequiredToSendMsg)//Check for available Credits
                    //if ((ClientDTO.SMSCredit - creditConsumed) < creditRequiredToSendMsg)//Check for available Credits
                    //{
                    //    baseFileXML.Save(sourceQFile);
                    //    if (IsMultipleMsg)
                    //    {
                    //        if (sentMsgCount > 0)
                    //        {
                    //            General.WriteInFile(baseFileName + " is skiped due to unsufficient credits.\r\nMessages sent upto recipient : " + currentnode.PreviousSibling.PreviousSibling.InnerText);
                    //        }
                    //        else
                    //        {
                    //            General.WriteInFile(baseFileName + " is skiped due to unsufficient credits.");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (sentMsgCount > 0)
                    //        {
                    //            General.WriteInFile(baseFileName + " is skiped due to unsufficient credits.\r\nMessages sent upto recipient : " + currentnode.PreviousSibling.InnerText);
                    //        }
                    //        else
                    //        {
                    //            General.WriteInFile(baseFileName + " is skiped due to unsufficient credits.");
                    //        }
                    //    }
                    //    break;
                    //}

                     

                    #region " Submiting to Gateway "

                    strURL = GATEWAY_URL.Replace("[recipient]", sMobileNo).Replace("[message]", mymsg);

                    if (CampaignDTO.IsUnicode == true)
                    {
                        strURL = strURL.Replace("[msgtype]", "1");
                    }
                    else strURL = strURL.Replace("[msgtype]", "0");

                  


                    sErrlocation = "HTTP web request-Line 387";
                    oHttpRequest = (HttpWebRequest)System.Net.WebRequest.Create(strURL);
                    oHttpRequest.Method = "GET";
                TryAgain:
                    try
                    {
                        string messageID = string.Empty;
                        bool IsSent = false;
                        string statuscode = string.Empty;
                        string result = string.Empty;

                        oHttpWebResponse = (HttpWebResponse)oHttpRequest.GetResponse();
                        StreamReader response = new StreamReader(oHttpWebResponse.GetResponseStream());
                        result = response.ReadToEnd();
                        oHttpWebResponse.Close();

                        switch (GatewayID)
                        {
                            case "WizSMS":
                                //if (result.Contains('|'))
                                //    statuscode = result.Substring(0, result.IndexOf('|'));
                                //else
                                //    statuscode = result;

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
                                        General.WriteInFile("Response :" + result);
                                        break;
                                }
                                break;
                            case "SMSPro":
                                //MessageSent GUID="gracew-8be9d47f999e569a" SUBMITDATE="08/26/2013 03:16:12 PM"
                                if (result.Contains("MessageSent"))
                                {
                                    messageID = result.Split('"')[1];
                                    IsSent = true;
                                }
                                else
                                {
                                    messageID = result;
                                    General.WriteInFile(result);
                                }
                                break;
                            default:
                                break;
                        }

                        if (IsSent)
                        {
                            if (IsMultipleMsg)
                                creditConsumed += creditRequiredToSendMsg;
                            else
                                creditConsumed += creditRequiredForSingleMsg;

                            XmlAttribute timespan;
                            timespan = baseFileXML.CreateAttribute("sentTime");
                            timespan.Value = System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                            currentnode.Attributes.Append(timespan);

                            XmlAttribute gatewayID;
                            gatewayID = baseFileXML.CreateAttribute("gatewayID");
                            gatewayID.Value = GatewayID;
                            currentnode.Attributes.Append(gatewayID);

                            XmlAttribute msgID;
                            msgID = baseFileXML.CreateAttribute("msgID");
                            msgID.Value = messageID;
                            currentnode.Attributes.Append(msgID);

                            XmlAttribute msgCode;
                            msgCode = baseFileXML.CreateAttribute("msgCode");
                            msgCode.Value = statuscode;
                            currentnode.Attributes.Append(msgCode);


                            XmlAttribute msgCount;
                            msgCount = baseFileXML.CreateAttribute("msgCount");
                            if (CampaignDTO.IsUnicode == false)
                            {
                                msgCount.Value = GetMessageCount(mymsg).ToString();
                                currentnode.Attributes.Append(msgCount);
                            }
                            else
                            {
                                msgCount.Value = GetUnicodeMessageCount(mymsg).ToString();
                                currentnode.Attributes.Append(msgCount);
                            }

                            XmlAttribute msgCredits;
                            SettingDTO SettingDTO = new SettingDTO();
                            SettingDTO = SettingService.GetById(1);
                            msgCredits = baseFileXML.CreateAttribute("msgCredits");
                            if (IsMultipleMsg)
                            { 
                                msgCredits.Value = creditRequiredToSendMsg.ToString();
                                double credit = Convert.ToDouble(msgCredits.Value);
                                double reqCredits = credit * SettingDTO.NationalCampaignSMSCount;
                                msgCredits.Value = reqCredits.ToString();
                            }
                            else
                            {
                                msgCredits.Value = creditRequiredForSingleMsg.ToString();
                                double credit = Convert.ToDouble(msgCredits.Value);
                                double reqCredits = credit * SettingDTO.NationalCampaignSMSCount;
                                msgCredits.Value = reqCredits.ToString();
                            } // GetMessageCount(mymsg).ToString();
                            currentnode.Attributes.Append(msgCredits);


                            sentMsgCount++;
                            errorCount = 0;

                            //oClient.ReduceCredit = creditConsumed;
                            //if (sentMsgCount % UPDATEBALAFTER == 0)
                            //{
                            //    ClientDBOperation.UpdateCredit(oClient, creditConsumed);
                            //    creditsRequired += creditConsumed;
                            //    baseFileXML.Save(sourceQFile);
                            //    creditConsumed = 0;
                            //}

                            creditConsumed = 0;
                        }
                        else
                        {
                            errorCount += 1;
                            if (errorCount > MAXCHECKFORRESPONSE)
                            {
                                General.WriteInFile("Message sending stoped due to BAD response from Gateway (i.e" + messageID + ")");
                                break;
                            }
                            else
                                goto TryAgain;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount += 1;
                        baseFileXML.Save(sourceQFile);
                        if (errorCount > MAXCHECKFORCONNECTION)
                        {
                            General.WriteInFile(sErrlocation + "\r\nGateway connection unavailable." + "\r\n" + ex.Message);
                            break;
                        }
                        else
                            goto TryAgain;
                    }
                    #endregion

                }
                else
                {
                    sentMsgCount++;
                    if (IsMultipleMsg)
                    {
                        creditsRequired += GetMessageCount(currentnode.NextSibling.InnerText.TrimEnd());
                    }
                    else
                    {
                        creditsRequired += creditRequiredForSingleMsg;
                    }
                }
            }

            baseFileXML.Save(sourceQFile);
            xmlfiledata = CommonService.ReadXMLFile(sourceQFile);

            // xmlfiledata= xmlfiledata.Replace("<?xml version="1.0"?>","");

            creditsRequired += creditConsumed;
            //oClient.TotalCreditsConsumed += creditsRequired;

            if (errorCount == 0 && sentMsgCount == recipientNumberCount) // indicates sending completed successfully
            {
                System.IO.File.Copy(sourceQFile, sentQFile, true);
                if (System.IO.File.Exists(sourceQFile))
                    System.IO.File.Delete(sourceQFile);
                //ClientDBOperation.UpdateCredit(oClient, creditConsumed);

                //oClient.TotalNumMessages += sentMsgCount;
                //ClientDBOperation.UpdateCreditMsgConsumed(oClient); // Update the count of total credits consumed by user till tody & Number of messages send. 
                UpdateMessageLog(CampaignDTO.Id, xmlfiledata);
                General.WriteInFile(baseFileName + " is sent successfully.");
            }
            else
            {
                if (creditConsumed != 0) // creditconsumed is zero means no any message send, hence dont update credit
                {
                    //ClientDBOperation.UpdateCredit(oClient, creditConsumed);
                }
            }

            //#region "Check for Alert message"
            //// Send credit alert and sms alert
            //if (oClient.AlertOnCredit != 0 && oClient.ActualCredits < oClient.AlertOnCredit && oClient.IsAlertOnCredit)
            //{
            //    if (ClientDBOperation.SMSCredits(oClient) >= 1)
            //    {
            //        // get credit alert msg format from QueueProcessorSettings.xml
            //        sErrlocation = "Sending credit goes below minimum limit alert-Line 438";
            //        string message = queuesettingsXML.DocumentElement["lowcreditalertmsg"].InnerText;
            //        message = message.Replace("[CurrentCredits]", oClient.ActualCredits.ToString());
            //        SendAlertMsg(message, "Credit Alert", oClient);
            //        General.WriteInFile("Credit Alert message generated.");
            //        ClientDBOperation.UpdateIsAlertOnCredits(oClient);
            //    }
            //    else
            //    {
            //        sErrlocation = "Due to unsufficient balance Credit Alert message not generated";
            //    }
            //}

            //// send alert when sms count is greater than 
            //if (sentMsgCount > oClient.AlertOnMessage && oClient.AlertOnMessage != 0)
            //{
            //    if (ClientDBOperation.SMSCredits(oClient) >= 1)
            //    {
            //        // get sms alert msg format from QueueProcessorSettings.xml
            //        sErrlocation = "Sending max number of Msg sent alert-Line 448";
            //        string message = queuesettingsXML.DocumentElement["maxnumsmssendalertmsg"].InnerText;
            //        message = message.Replace("[SentMsgCount]", sentMsgCount.ToString()).Replace("[MsgLimit]", oClient.AlertOnMessage.ToString());
            //        SendAlertMsg(message, "SMSCount Alert", oClient);
            //        General.WriteInFile("SMSCount Alert message generated.");
            //    }
            //    else
            //    {
            //        sErrlocation = "Due to unsufficient balance SMSCount Alert message not generated";
            //    }
            //}
            //#endregion

            General.WriteInFile("Closing Balance = " + ClientDTO.SMSCredit + "\r\n"); // ClientDBOperation.ActualCredits(oClient) + "\r\n"
        }

        #endregion


        #region "Private Methods "

        /// <summary>
        /// This method will validate xmlfile agianst the Schecma file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool IsValidFile(string filePath)
        {
            bool isValid = true;
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas.Add(null, ConfigurationManager.AppSettings["SETTIGNDIR_PATH"].ToString() + "XmlSchemaweb.xsd");
                settings.ValidationEventHandler += delegate(object sender, ValidationEventArgs vargs)
                {
                    schemaValidationError = vargs.Message;
                    isValid = false;
                };

                using (XmlReader reader = XmlReader.Create(filePath, settings))
                {
                    while (reader.Read()) { }
                }
                isValid = true;
            }
            catch (Exception XmlExp)
            {
                Console.WriteLine(XmlExp.Message);
                isValid = false;
            }
            return isValid;
        }

        /////// <summary>
        /////// Create xml packet to send Alerts message to recpective people.
        /////// </summary>
        /////// <param name="message"></param>
        /////// <param name="alertType"></param>
        /////// <param name="_oClsClients"></param>
        ////private void SendAlertMsg(string message, string alertType, Client _oClsClients)
        ////{
        ////    try
        ////    {
        ////        sErrlocation = "Creating XmlPackets for sending Alerts -Line 560";
        ////        string messageSource = queuesettingsXML.DocumentElement["messagesource"].InnerText;
        ////        string alertMsgPacket = "<?xml version=" + "\"1.0\"?>" +
        ////                      "<packet>" +
        ////                              "<messagesource>" + messageSource + "</messagesource>" +
        ////                              "<campaignId>" + _oClsClients.campaignId + "</campaignId>" +
        ////                              "<clientId>" + _oClsClients.clientId + "</clientId>" +
        ////                              "<campaignname>" + alertType + "</campaignname>" +
        ////                              "<senddate>" + System.DateTime.Now.ToString("d/MMM/yyyy") + "</senddate>" +
        ////                              "<messagetype>ALERT</messagetype>" +
        ////                              "<messagecount>1</messagecount>" +
        ////                              "<sender>" + queuesettingsXML.DocumentElement["sender"].InnerText + "</sender>" +
        ////                              "<requiredcredits>1</requiredcredits>" +
        ////                              "<numbers>" +
        ////                                  "<number>" + _oClsClients.RegisteredPhone + "</number>" +
        ////                                  "<message>" + message + "</message>" +
        ////                              "</numbers>" +
        ////                      "</packet>";
        ////        // Load this packet into xml
        ////        XmlDocument alertPacket = new XmlDocument();
        ////        alertPacket.LoadXml(alertMsgPacket);
        ////        DateTime DateAndTime = System.DateTime.Now;
        ////        sErrlocation = "Saving XmlPackets -Line 583";
        ////        alertPacket.Save(SMSQDIR + _oClsClients.campaignId + "-" + DateAndTime.Year + DateAndTime.Month + DateAndTime.Day + "-" + DateAndTime.Hour + DateAndTime.Minute + DateAndTime.Second + "1" + "-Q.xml");
        ////        alertPacket = null;
        ////        ClientDBOperation.UpdateSMSCredits(_oClsClients, 1);
        ////    }
        ////    catch (Exception excep)
        ////    {
        ////        General.WriteInFile(sErrlocation + "\r\n" + excep.Message);
        ////    }
        ////}

        /// <summary>
        /// Maintains sent message Log.
        /// </summary>
        /// <param name="msgcount"></param>
        private void UpdateMessageLog(int CampaignId, string XMLdata)
        {
            CampaignLogXMLDTO CampaignLogXMLDTO = new CampaignLogXMLDTO();
            CampaignLogXMLDTO.CampaignId = CampaignId;
            CampaignLogXMLDTO.XMLLog = XMLdata;
            CampaignLogXMLService.Create(CampaignLogXMLDTO);

            //sErrlocation = "Maintain SMSLog-Line 518";
            //SMSLog oSMSLog = new SMSLog();
            //oSMSLog.campaignId = oClient.campaignId;
            //oSMSLog.CampaignName = campaignname.Replace("'", "''");
            //oSMSLog.CreditsConsumed = creditsRequired;
            //oSMSLog.LogDate = System.DateTime.Now;
            //oSMSLog.NumMessages = msgcount;
            //oSMSLog.XMLfile = baseFileName;
            //SMSLogDBOperation.Insert(oSMSLog);

        }

        /// <summary>
        /// This method will move campaign file to Bad folder & adds reason why it is moved to bad folder.
        /// </summary>
        /// <param name="strErrorReason"></param>
        private void MoveFileToBadFolder(string strErrorReason)
        {
            // Append the ErrorReason node to file
            bool AppendErrorNode = false;
            if (baseFileXML == null) // If File not loaded than load into XmlDocument
            {
                try
                {
                    baseFileXML = new XmlDocument();
                    baseFileXML.Load(sourceQFile);
                    AppendErrorNode = true;
                }
                catch (Exception ex) // Unable to load than dont AppendErrorNode
                {
                    AppendErrorNode = false;
                }
            }
            else
            {
                AppendErrorNode = true;
            }
            if (AppendErrorNode)
            {
                XmlElement errorReason = baseFileXML.CreateElement("reason");
                errorReason.InnerText = strErrorReason;
                baseFileXML.ChildNodes.Item(1).AppendChild(errorReason);
                baseFileXML.Save(sourceQFile);
            }

            System.IO.File.Copy(sourceQFile, badQFile, true); // copy File From SMSQUEUE folder to BAD
            if (System.IO.File.Exists(sourceQFile))
                System.IO.File.Delete(sourceQFile); // Delete file from SMSQUEUE folder
        }

        /// <summary>
        /// Method to get message count.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private int GetMessageCount(string message)
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
            sErrlocation = "Calculating message count-Line 575";
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

        /// <summary>
        /// Reformate message text
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private string ReformatMsg(string msg)
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
        private string MsgCorrect(string msg, bool IsUnicode)
        {
            sErrlocation = "conversion of some characters so that they can be represented in url querystring-Line 689";

            if (string.IsNullOrEmpty(msg.Trim()))
            {
                return "";
            }
            if (IsUnicode == false)
            {
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
                msg = msg.Replace("\n", "%0A");
            }

            if (IsUnicode == true) { return ConvertToDecimalNCR(msg); }
            return RemoveInvalidChar(msg);
        }

        /// <summary>
        /// Remove invalid charachters from message. i.e remove char with ascii value grater than 125
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private string RemoveInvalidChar(string msg)
        {
            sErrlocation = "Remove invalid charachters from message. i.e remove char with ascii value grater than 125-Line 836";
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


        private string ConvertToDecimalNCR(string msg)
        {
            string input = msg;// "एकदा संता बंताला स्वत:च्या घरी बोलावतो,जेव्हा संता बंताच्या घरी जातो %";
            char[] values = input.ToCharArray();
            string hexOutput = "";
            string decnrcOutput = "";
            string fulldecnrcOutput = null;
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                hexOutput = hexOutput + String.Format("{0:X4}", value) + " ";


                decnrcOutput = "&#" + value + ";";
                fulldecnrcOutput = fulldecnrcOutput + decnrcOutput;

            }

            fulldecnrcOutput = fulldecnrcOutput.Replace("&", "%26");
            fulldecnrcOutput = fulldecnrcOutput.Replace("#", "%23");
            return fulldecnrcOutput;
        }

        #endregion




    }
}

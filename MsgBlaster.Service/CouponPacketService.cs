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
    public class CouponPacketService
    {
        #region "Method for Message Sending "

        public static string SendMessage(Byte[] array, Byte[] checksum)
        {
            try
            {
                //Checking for Data Accuracy
                Byte[] newchecksum = new MD5CryptoServiceProvider().ComputeHash(array);
                if (checksum.Length == newchecksum.Length)
                {
                    int arraylength = 0;
                    while ((arraylength < checksum.Length) && (newchecksum[arraylength] == checksum[arraylength]))
                    {
                        arraylength++;
                    }
                    if (arraylength != newchecksum.Length)
                    {
                        return ErrorFlag.DataCorrupted.ToString();
                    }
                }


                // Checking User's Validation that is CDKey & MachineID
                XmlSerializer xs = new XmlSerializer(typeof(MsgInformationDTO));
                MemoryStream msgStream = new MemoryStream(array);
                MsgInformationDTO oMsgInformationDTO = (MsgInformationDTO)xs.Deserialize(msgStream);

                CampaignDTO CampaignDTO = new CampaignDTO();
                CampaignDTO = CampaignService.GetById(oMsgInformationDTO.CampaignId);
                //CampaignDTO.ClientId = oMsgInformationDTO.ClientId;
                //CampaignDTO.Id = oMsgInformationDTO.CampaignId;
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(CampaignDTO.ClientId);


                string packet = oMsgInformationDTO.xmlpacket;
                string fname = null;
                if (ValidatePacketAgainstSchema(packet)) // Check xml file validation.
                {
                    if (AllowToProcessClient(CampaignDTO))
                    {
                        DateTime DateAndTime = System.DateTime.Now;
                        SettingDTO SettingDTO = new SettingDTO(); // Get limit on msg size
                        SettingDTO = SettingService.GetById(1);

                        int requiredCredit = 0;
                        XmlDocument x = new XmlDocument();
                        x.LoadXml(packet);
                        XmlNodeList messages = x.SelectNodes("/packet/numbers/message/text()"); // Get all messages from xmlpacket
                        XmlNodeList numbers = x.SelectNodes("/packet/numbers/number");
                        for (int i = 0; i < messages.Count; i++) // Get required credits to send this packet;
                        {
                            requiredCredit += MessageCount(MsgCorrect(messages[i].InnerText.TrimEnd()));
                        }
                        if (messages.Count == 1) // Means one message to all numbers
                        {
                            requiredCredit = requiredCredit * numbers.Count;
                        }

                        if (ClientDTO.SMSCredit >= requiredCredit)
                        {
                            XmlNode root = x.DocumentElement;
                            XmlElement requiredcredits = x.CreateElement("requiredcredits");
                            requiredcredits.InnerText = requiredCredit.ToString();
                            root.InsertBefore(requiredcredits, root.LastChild);
                            //_oClsClients.SMSCredits -= requiredCredit;
                            try
                            {
                                fname = DateAndTime.Year.ToString() + DateAndTime.Month + DateAndTime.Day + "-" + DateAndTime.Hour + DateAndTime.Minute + DateAndTime.Second + "-" + oMsgInformationDTO.CampaignId + "-Q.xml";
                                x.Save(ConfigurationManager.AppSettings["SMSFolderPath"].ToString() + fname);
                                x = null;
                                //dbClients.ReduceSMSCredits(oClient, requiredCredit);


                                CampaignDTO CampaignDTONew = new CampaignDTO();
                                CampaignDTONew = CampaignDTO;
                                //CampaignDTONew.IsSent = true;
                                CampaignDTONew.Status = "Sent";
                                CampaignService.Edit(CampaignDTONew);

                                ClientDTO ClientDTOUpdate = new ClientDTO();
                                ClientDTOUpdate = ClientDTO;
                                ClientDTOUpdate.SMSCredit = ClientDTOUpdate.SMSCredit - requiredCredit;
                                ClientService.Edit(ClientDTOUpdate);




                            }
                            catch (Exception ex)
                            {
                                return ErrorFlag.FailedToWriteData.ToString();      // Returns "FailedToWriteData" enum name if message file not created

                            }
                            //return ErrorFlag.Success.ToString();                // Return "Success" enum name if Message file created in the SMSQueue folder successfully
                            return fname;
                        }
                        else
                            return ErrorFlag.InsufficientCredits.ToString();  // Returns "InsufficientCredits" enum name if SMSCredits are insufficient for sending message
                    }
                    else
                        return ErrorFlag.InvalidUser.ToString();        // Returns "InvalidUser" enum name if the CDKey or MachineID not matching
                }
                else
                    return ErrorFlag.BadXml.ToString(); // Return BAD XmlPacke Error
            }
            catch
            {
                throw;             // Returns error flag name if there are any web exception
            }

        }

        //private static int MessageCount(string message) // calculate credits required to send this message;
        //{
        //    SettingDTO SettingDTO = new SettingDTO(); // Get limit on msg size
        //    SettingDTO = SettingService.GetById(1);
        //    int MAXMSGLENGTH = SettingDTO.MessageLength;
        //    int MSGLENGTH = SettingDTO.SingleMessageLength;

        //    if (message.Length <= MAXMSGLENGTH)
        //        return 1;
        //    else if (message.Length % MSGLENGTH != 0)
        //        return (message.Length / MSGLENGTH) + 1;
        //    else
        //        return message.Length / MSGLENGTH;
        //}

        #endregion

        #region " Xmlpacket validation "

        private static bool ValidatePacketAgainstSchema(string xmlpacket)
        {
            XmlValidatingReader reader = null;
            XmlSchemaCollection myschema = new XmlSchemaCollection();
            try
            {
                //Create the XmlParserContext.
                XmlParserContext context = new XmlParserContext(null, null, "", XmlSpace.None);

                //Implement the reader. 
                reader = new XmlValidatingReader(xmlpacket, XmlNodeType.Element, context);
                ////////Add the schema.
                //// myschema.Add(null, @"C:\inetpub\wwwroot\MsgBlasterWebServiceSetup\XmlSchema.xsd");

                myschema.Add(null, ConfigurationManager.AppSettings["SchemaFile"].ToString()); //AppDomain.CurrentDomain.BaseDirectory + "\\XmlSchema.xsd"
                //myschema.Add(null, ConfigurationManager.AppSettings["SettingFiles"] + "XmlSchema.xsd");


                //Set the schema type and add the schema to the reader.
                reader.ValidationType = ValidationType.Schema;
                reader.Schemas.Add(myschema);

                while (reader.Read())
                {
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion

        public static bool AllowToProcessClient(CampaignDTO CampaignDTO)
        {


            if (CampaignDTO == null)
            {
                return false;
            }
            else
            {
                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(CampaignDTO.ClientId);
                if (ClientDTO.SMSCredit > 0)
                {
                    return true;
                }
                else
                    return false;
            }

        }

        private static string MsgCorrect(string msg)
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

        #region " Message sending........ "
        public static void CreatePacket()
        {
            StringBuilder recipientnumberslist = new StringBuilder();


            try
            {
                bool ismailmarge = false;
                int requiredCreditTosendmsg = 0;
                //DataTable dtContact = new DataTable();
                int MOBILENUMBERLEN = 0;
                string xmlpacket = null;
                List<CampaignDTO> CampaignDTOList = CampaignService.GetCampaignNotSentList();
                if (CampaignDTOList.Count != 0)
                {
                    foreach (var item in CampaignDTOList)
                    {
                        // create xml packet
                        DataTable dtContact = new DataTable();

                        xmlpacket = "<?xml version=" + "\"1.0\"?>";
                        xmlpacket += "<packet>";
                        xmlpacket += "<mbversion>MessageBlaster_Web</mbversion>";
                        xmlpacket += "<messagesource>MSGBLASTER</messagesource>";
                        //DataTable regInfoDT = oCompanyInfo.LoadAll();
                        ClientDTO ClientDTO = new ClientDTO();
                        ClientDTO = ClientService.GetById(item.ClientId);


                        SettingDTO SettingDTO = new SettingDTO();
                        SettingDTO = SettingService.GetById(1);
                        MOBILENUMBERLEN = SettingDTO.MobileNumberLength;

                        ArrayList recipientsnumbers = new ArrayList();
                        MessageLogDTO oMessageLog = new MessageLogDTO();
                        string[] recipients;
                        if (item.GroupId == null) // To check wheather the user sending Group message
                        {
                            recipients = item.RecipientsNumber.ToString().Split(',');
                        }
                        else
                        {
                            recipients = item.RecipientsNumber.ToString().Split(',');
                        }
                        if (recipients.Length == 0)
                        {
                            //oUcButtonControl.showMessage(frmButtonControl.Messageflag.warningMessage, "Select recipients first.");
                            return;
                        }
                        for (int i = 0; i < recipients.Length; i++) // Loop through each recipient number & remove duplicate numbers
                        {
                            if (!string.IsNullOrEmpty(recipients[i].ToString())) // Don`t allow empty number
                            {
                                string mobileNumber = GetValidMobileNumber(recipients[i].ToString().Trim()); // Get only digits from Mobile number
                                if (mobileNumber.Length >= MOBILENUMBERLEN) // Check for valid mobile number
                                {
                                    mobileNumber = mobileNumber.Substring(mobileNumber.Length - MOBILENUMBERLEN);
                                    if (!recipientsnumbers.Contains(mobileNumber)) // Check for number duplication.
                                    {
                                        recipientsnumbers.Add(mobileNumber);
                                        recipientnumberslist.Append(mobileNumber).Append(',');
                                    }
                                }
                            }
                        }
                        if (recipientnumberslist.Length != 0)
                        {
                            oMessageLog.Recipients = recipientnumberslist.ToString().Substring(0, recipientnumberslist.Length - 1);
                        }

                        MsgInformationDTO _oMsginfo = new MsgInformationDTO();

                        _oMsginfo.CampaignId = item.Id;// regInfoDT.Rows[0]["SerialKey"].ToString();
                        //xmlpacket += "<cdkey>" + regInfoDT.Rows[0]["SerialKey"].ToString() + "</cdkey>";
                        xmlpacket += "<campaignId>" + _oMsginfo.CampaignId + "</campaignId>";
                        _oMsginfo.ClientId = item.ClientId;// MachineID.Value();
                        //xmlpacket += "<machineid>" + _oMsginfo.MachineID + "</machineid>";
                        xmlpacket += "<clientId>" + _oMsginfo.ClientId + "</clientId>";

                        if (!string.IsNullOrEmpty(item.Name)) // check for TemplateName
                        {
                            //xmlpacket += "<campaignname>" + MsgCorrect(lkupTemplate.Text) + "</campaignname>";
                            xmlpacket += "<campaignname>" + MsgCorrect(item.Name.ToString()) + "</campaignname>";
                            oMessageLog.MessageTemplateID = _oMsginfo.CampaignId;
                        }
                        else
                        {
                            xmlpacket += "<campaignname>Direct_Message</campaignname>";
                            oMessageLog.MessageTemplateID = _oMsginfo.CampaignId;
                        }

                        if (!string.IsNullOrEmpty(item.GroupId.ToString())) //nameOfGroupForMsgSending
                        {
                            GroupDTO GroupDTO = new GroupDTO();
                            GroupDTO = GroupService.GetById(Convert.ToInt32(item.GroupId));
                            xmlpacket += "<groupname>" + MsgCorrect(GroupDTO.Name) + "</groupname>"; // nameOfGroupForMsgSending
                            oMessageLog.RecipientType = GroupDTO.Name;
                        }
                        else if (!string.IsNullOrEmpty(item.Name))  //nameOfImportedFile // Check for is direct message to imported contact
                        {
                            oMessageLog.RecipientType = item.Name;//  nameOfImportedFile ;
                        }
                        else
                        {
                            oMessageLog.RecipientType = "Direct";
                        }

                        oMessageLog.MessageDateTime = Convert.ToString(System.DateTime.Now);
                        xmlpacket += "<senddate>" + System.DateTime.Now.ToString("d/MMM/yyyy") + "</senddate>";

                        if (!string.IsNullOrEmpty(item.ScheduledDate.ToString())) //scheduledDate.Text // check for sheduled Date
                        {
                            DateTime ScheduledDateTime = DateTime.Parse(item.ScheduledDate.ToString());
                            if (item.ScheduledTime == null || item.ScheduledTime == "")
                            {
                                item.ScheduledTime = "12:00 AM";
                            }
                            DateTime ScheduledTime = Convert.ToDateTime(item.ScheduledTime);
                            ScheduledDateTime = ScheduledDateTime.AddHours(ScheduledTime.TimeOfDay.Hours);
                            ScheduledDateTime = ScheduledDateTime.AddMinutes(ScheduledTime.TimeOfDay.Minutes);
                            DateTime ActualScheduleDatetime = Convert.ToDateTime(item.ScheduledDate.ToString("MM/dd/yyyy") + " " + ScheduledDateTime.TimeOfDay); ;
                            xmlpacket += "<scheduleddate>" + ActualScheduleDatetime.ToString("dd/MMM/yyyy HH:mm tt") + "</scheduleddate>";
                            oMessageLog.MessageScheduledDateTime = Convert.ToString(ScheduledDateTime);
                        }

                        oMessageLog.MessageText = item.Message.ToString().Replace("'", "''"); //memoMessagetxt.Text.Replace("'", "''");

                        if (FormatMessageText(item.Message)) //memoMessagetxt.Text
                        {
                            ismailmarge = true;
                            xmlpacket += "<messagetype>MAILMERGE</messagetype>";
                            oMessageLog.MessageTemplateType = "MAILMERGE";
                            // Get information of numbers which are in Contact list to foramte mail-marge-message
                            string nameOfGroupForMsgSending = null;



                            if (nameOfGroupForMsgSending == null)
                                dtContact = CommonService.SelectContatsInNumber(recipientnumberslist.ToString().Substring(0, recipientnumberslist.Length - 1), item.ClientId);
                            else
                                dtContact = CommonService.SelectContatsInNumber(recipientnumberslist.ToString().Substring(0, recipientnumberslist.Length - 1), item.ClientId);
                        }
                        else
                        {

                            xmlpacket += "<messagetype>NORMAL</messagetype>";
                            oMessageLog.MessageTemplateType = "NORMAL";
                        }

                        oMessageLog.Count = recipientsnumbers.Count;
                        xmlpacket += "<messagecount>" + recipientsnumbers.Count.ToString() + "</messagecount>";

                        //oMessageLog.SenderNumber = lookUpSender.Text;
                        List<UserDTO> UserDTOList = new List<UserDTO>();
                        UserDTOList = UserService.GetUsersbyClientId(ClientDTO.Id, "");
                        if (UserDTOList.Count != 0)
                        {
                            foreach (var itemuser in UserDTOList)
                            {
                                if (itemuser.UserType == "Admin")
                                {
                                    oMessageLog.SenderNumber = itemuser.Mobile;
                                    xmlpacket += "<sender>" + MsgCorrect(oMessageLog.SenderNumber) + "</sender>";
                                }

                            }
                        }



                        xmlpacket += "<numbers>";
                        if (ismailmarge)
                            requiredCreditTosendmsg = AddMsgRecipToXmlpacketMailMerge(item.Message, recipientsnumbers, dtContact.DefaultView, xmlpacket, _oMsginfo, recipientsnumbers.Count);
                        else
                            requiredCreditTosendmsg = AddMsgRecipToXmlpacket(item.Message, recipientsnumbers, xmlpacket, _oMsginfo, recipientsnumbers.Count) * recipientsnumbers.Count;

                        //xmlpacket += "</numbers>";
                        //xmlpacket += "</packet>";
                        //_oMsginfo.xmlpacket = xmlpacket;
                        //_oMsginfo.RequiredCredits = requiredCreditTosendmsg;
                        //Byte[] array = Serializeobject(_oMsginfo);
                        //Byte[] checksum = new MD5CryptoServiceProvider().ComputeHash(array); // calculate checksum for validation

                        //if (requiredCreditTosendmsg > recipientsnumbers.Count)
                        //{
                        //    //DialogResult dlg = XtraMessageBox.Show("You will be charged " + requiredCreditTosendmsg + " credits to send this message." + "\r\n" + "Do you want to send ?", "Conformation", MessageBoxButtons.YesNo);
                        //    //if (dlg == DialogResult.Yes)
                        //    //{

                        //    string responsefromService = SendMessage(array, checksum);
                        //    Response(responsefromService);

                        //    //}
                        //    //else
                        //    //{
                        //    //oUcButtonControl.ShowSend = true;
                        //    //oUcButtonControl.showMessage(frmButtonControl.Messageflag.none, "");
                        //    //oUcButtonControl.ButtonView();
                        //    //this.Update();
                        //    //}
                        //}
                        //else
                        //{
                        //    string responsefromService = SendMessage(array, checksum);
                        //    Response(responsefromService);
                        //}
                    }
                }

            }
            catch (WebException ex)
            {
                //oUcButtonControl.showMessage(frmButtonControl.Messageflag.errorMessage, Global.DisplayConnectionError(ex));
                throw;
            }

        }

        #endregion

        //#region " Fields Mapping "
        //// 
        //// Arraylist of Mail marge macrows 
        ////
        //private static ArrayList getFieldKeyList(ContactDTO ContactDTO)
        //{
        //    ArrayList keyList = new ArrayList();
        //    //keyList.Add("Salutation");
        //    //keyList.Add("Firstname");
        //    //keyList.Add("Lastname");
        //    //keyList.Add("Mobile");
        //    //keyList.Add("EmailID");
        //    //keyList.Add("Company");
        //    //keyList.Add("BirthDate");
        //    //keyList.Add("Anniversary");
        //    //keyList.Add("Jobtitle");
        //    //keyList.Add("Phone(O)");
        //    //keyList.Add("Phone(H)");
        //    //keyList.Add("FAX");
        //    keyList.Add("FirstName");
        //    keyList.Add("LastName");
        //    keyList.Add("Code");
        //    ArrayList list = oCompanyInfo.GetFieldNames();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (!string.IsNullOrEmpty(list[i].ToString()))
        //            keyList.Add(list[i].ToString());
        //    }
        //    return keyList;
        //}
        ////
        //// ArrayList to map macrows with database Fields
        ////
        //private static ArrayList getFieldValueList(ContactDTO ContactDTO)
        //{
        //    ArrayList valueList = new ArrayList();
        //    //valueList.Add("SalutationName");
        //    //valueList.Add("FirstName");
        //    //valueList.Add("LastName");
        //    //valueList.Add("MobileNumber");
        //    //valueList.Add("EmailID");
        //    //valueList.Add("Company");
        //    //valueList.Add("BirthDate");
        //    //valueList.Add("Anniversary");
        //    //valueList.Add("JobTitle");
        //    //valueList.Add("PhoneOffice");
        //    //valueList.Add("PhoneHome");
        //    //valueList.Add("FAX");

        //    valueList.Add("FirstName");
        //    valueList.Add("LastName");
        //    valueList.Add("Code");

        //    ArrayList list = oCompanyInfo.GetFieldNames();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (!string.IsNullOrEmpty(list[i].ToString()))
        //        {
        //            switch (i.ToString())
        //            {
        //                case "0":
        //                    valueList.Add("Label1");
        //                    break;
        //                case "1":
        //                    valueList.Add("Label2");
        //                    break;
        //                case "2":
        //                    valueList.Add("Label3");
        //                    break;
        //                case "3":
        //                    valueList.Add("Label4");
        //                    break;
        //                case "4":
        //                    valueList.Add("Label5");
        //                    break;
        //            }
        //        }
        //    }
        //    return valueList;
        //}

        //#endregion

        #region "Public validation methods"

        public static string GetValidMobileNumber(string mobile)
        {
            StringBuilder recipientnumber = new StringBuilder();
            if (string.IsNullOrEmpty(mobile))
                return string.Empty;
            for (int i = 0; i < mobile.Length; i++)
            {
                try
                {
                    sbyte number = sbyte.Parse(mobile.Substring(i, 1));
                    recipientnumber.Append(number);
                }
                catch (System.Exception ex)
                {
                    Console.Write(ex.Message);
                    continue;
                }
            }
            return recipientnumber.ToString();
        }

        #endregion

        #region " Add recipients & message in xmlpacket "
        //
        // Method for adding recipient number and message if message not contains microws
        //
        public static int AddMsgRecipToXmlpacket(string message, ArrayList recipients, string xmlpacket, MsgInformationDTO _oMsginfo, int recipientsnumberscount)
        {
            //string xmlpacket = null;
            for (int i = 0; i < recipients.Count; i++)
                xmlpacket += "<number>" + recipients[i].ToString().Trim() + "</number>";

            xmlpacket += "<message>" + MsgCorrect(message) + "</message>";

            /////////////////////////////////////////////////////////////////

            xmlpacket += "</numbers>";
            xmlpacket += "</packet>";
            _oMsginfo.xmlpacket = xmlpacket;
            int requiredCreditTosendmsg = MessageCount(message);
            _oMsginfo.RequiredCredits = requiredCreditTosendmsg;
            Byte[] array = Serializeobject(_oMsginfo);
            Byte[] checksum = new MD5CryptoServiceProvider().ComputeHash(array); // calculate checksum for validation

            if (requiredCreditTosendmsg > recipientsnumberscount)
            {
                //DialogResult dlg = XtraMessageBox.Show("You will be charged " + requiredCreditTosendmsg + " credits to send this message." + "\r\n" + "Do you want to send ?", "Conformation", MessageBoxButtons.YesNo);
                //if (dlg == DialogResult.Yes)
                //{

                string responsefromService = SendMessage(array, checksum);
                Response(responsefromService);

                //}
                //else
                //{
                //oUcButtonControl.ShowSend = true;
                //oUcButtonControl.showMessage(frmButtonControl.Messageflag.none, "");
                //oUcButtonControl.ButtonView();
                //this.Update();
                //}
            }
            else
            {
                string responsefromService = SendMessage(array, checksum);
                Response(responsefromService);
            }
            /////////////////////////////////////////////////////////////////
            return MessageCount(message);
        }

        //
        // Method for adding recipient number and mail-marge-message 
        //
        public static int AddMsgRecipToXmlpacketMailMerge(string message, ArrayList recipients, DataView DVrecipientInfo, string xmlpacket, MsgInformationDTO _oMsginfo, int recipientsnumberscount)
        {
            //string xmlpacket = null;
            int messagecount = 0;
            string formatedmessage;
            for (int i = 0; i < recipients.Count; i++)
            {
                DVrecipientInfo.RowFilter = "MobileNumber = '" + recipients[i].ToString().Trim() + "'";
                xmlpacket += "<number>" + recipients[i].ToString().Trim() + "</number>";
                xmlpacket += "<message>";
                formatedmessage = FormatMessageTextMicrows(message, DVrecipientInfo);
                xmlpacket += MsgCorrect(FormatMessageText(formatedmessage, DVrecipientInfo));
                messagecount += MessageCount(formatedmessage);
                xmlpacket += "</message>";
                DVrecipientInfo.RowFilter = "";
            }

            //////////////////////////////////////////////////////////////////////

            xmlpacket += "</numbers>";
            xmlpacket += "</packet>";
            _oMsginfo.xmlpacket = xmlpacket;
            _oMsginfo.RequiredCredits = messagecount;
            Byte[] array = Serializeobject(_oMsginfo);
            Byte[] checksum = new MD5CryptoServiceProvider().ComputeHash(array); // calculate checksum for validation

            if (messagecount > recipientsnumberscount)
            {
                //DialogResult dlg = XtraMessageBox.Show("You will be charged " + requiredCreditTosendmsg + " credits to send this message." + "\r\n" + "Do you want to send ?", "Conformation", MessageBoxButtons.YesNo);
                //if (dlg == DialogResult.Yes)
                //{

                string responsefromService = SendMessage(array, checksum);
                Response(responsefromService);

                //}
                //else
                //{
                //oUcButtonControl.ShowSend = true;
                //oUcButtonControl.showMessage(frmButtonControl.Messageflag.none, "");
                //oUcButtonControl.ButtonView();
                //this.Update();
                //}
            }
            else
            {
                string responsefromService = SendMessage(array, checksum);
                Response(responsefromService);
            }
            //////////////////////////////////////////////////////////////////////
            return messagecount;
        }

        //
        // Removing the mailmarge microws from Message with specific  value 
        //
        private static string FormatMessageText(string mailmargeMessage, DataView recipientInfo)
        {
            string msg = mailmargeMessage;
            DataView dv = recipientInfo;
            string msgtext = msg;
            ArrayList FieldKeyList = new ArrayList();
            ArrayList FieldValueList = new ArrayList();
            FieldKeyList = getFieldKeyList();
            FieldValueList = getFieldValueList();
            while (msg.Contains("[") && msg.Contains("]"))
            {
                string temp = msg.Substring(msg.IndexOf('[') + 1, (msg.IndexOf(']') - 1 - msg.IndexOf('[')));
                if (!temp.Contains(":"))
                {
                    if (FieldKeyList.Contains(temp))
                    {
                        int index = FieldKeyList.IndexOf(temp);
                        string fieldName = FieldValueList[index].ToString();
                        if (dv.Count > 0)
                        {

                            //if (msg.Contains("[FirstName]") == true)
                            //{
                            //    string FirstName = "";
                            //    FirstName = CommonService.GetFirstname(ContactDTO.Name);
                            //    msg = msg.Replace("[FirstName]", FirstName);
                            //}


                            //if (msg.Contains("[LastName]") == true)
                            //{
                            //    string LastName = "";
                            //    LastName = CommonService.GetLastname(ContactDTO.Name);

                            //    msg = msg.Replace("[LastName]", LastName);
                            //}


                            if (fieldName == "BirthDate" || fieldName == "Anniversary")
                                msgtext = msgtext.Replace("[" + temp + "]", Convert.ToDateTime(dv[0][fieldName]).ToString("dd-MMM-yyy").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"));
                            else
                                msgtext = msgtext.Replace("[" + temp + "]", dv[0][fieldName].ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"));
                        }
                        else
                            msgtext = msgtext.Replace("[" + temp + "]", "");
                    }
                }
                else
                {
                    string altText = temp.Substring(temp.IndexOf(':') + 1, temp.Length - 1 - temp.IndexOf(':'));
                    string Fieldkey = temp.Replace(":" + altText, "");
                    if (FieldKeyList.Contains(Fieldkey))
                    {
                        int index = FieldKeyList.IndexOf(Fieldkey);
                        string fieldName = FieldValueList[index].ToString();
                        if (dv.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dv[0][fieldName].ToString()))
                            {
                                if (fieldName == "BirthDate" || fieldName == "Anniversary")
                                    msgtext = msgtext.Replace("[" + temp + "]", Convert.ToDateTime(dv[0][fieldName]).ToString("dd-MMM-yyy").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"));
                                else
                                    msgtext = msgtext.Replace("[" + temp + "]", dv[0][fieldName].ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"));
                            }
                            else
                                msgtext = msgtext.Replace("[" + temp + "]", altText);
                        }
                        else
                            msgtext = msgtext.Replace("[" + temp + "]", altText);
                    }
                }
                msg = msg.Replace(msg.Substring(0, msg.IndexOf(']') + 1), "");

            }
            return msgtext;
        }

        //
        // Calculate the messagecount 
        //
        private static int MessageCount(string message)
        {
            SettingDTO SettingDTO = new SettingDTO();
            SettingDTO = SettingService.GetById(1);


            int MsgLength = SettingDTO.MessageLength;
            int MsgLenPerSingleMessage = SettingDTO.SingleMessageLength;

            if (message.Length <= MsgLength)
                return 1;
            else if (message.Length % MsgLenPerSingleMessage != 0)
                return (message.Length / MsgLenPerSingleMessage) + 1;
            else
                return message.Length / MsgLenPerSingleMessage;
        }
        #endregion

        //
        // Removing the mailmarge microws from Message with specific  value 
        //
        private static string FormatMessageTextMicrows(string mailmargeMessage, DataView recipientInfo)
        {
            string msg = mailmargeMessage;
            DataView dv = recipientInfo;
            string msgtext = msg;
            ArrayList FieldKeyList = new ArrayList();
            ArrayList FieldValueList = new ArrayList();


            if (dv.Count > 0)
            {
                foreach (DataRow drForm in dv.ToTable().Rows)
                {
                    string Name = "";
                    Name = drForm["Name"].ToString();

                    string FirstName = "";
                    FirstName = CommonService.GetFirstname(Name);

                    string LastName = "";
                    LastName = CommonService.GetLastname(Name); ;

                    msgtext = msgtext.Replace("[FirstName]", FirstName);
                    msgtext = msgtext.Replace("[LastName]", LastName);


                }
            }

            
            return msgtext;
        }

        private static ArrayList getFieldValueList(ClientDTO ClientDTO)
        {
            throw new NotImplementedException();
        }

        #region " Cheching For Mail Merge "

        //
        // Method to define message body contains any microw   
        //
        private static bool FormatMessageText(string msg)
        {
            ArrayList list = getFieldKeyList();
            for (int i = 0; i < list.Count; i++)
            {
                if (msg.Contains("[" + list[i].ToString()))
                    return true;
            }
            return false;
        }

        #endregion

        #region " Xml serialization "
        //
        // xml serialization of Object
        //
        public static Byte[] Serializeobject(object obj)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(MsgInformationDTO));
                MemoryStream stream = new MemoryStream();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8);
                xs.Serialize(xmlTextWriter, obj);
                stream = (MemoryStream)xmlTextWriter.BaseStream;
                return stream.ToArray();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return null;
            }
        }
        #endregion

        #region" Response from SendMessage web service"

        private static void Response(string result)
        {
            if (result.Contains("-Q.xml"))
            {
                //oMessageLog.ScheduledMessageCampaign = result;
                //oMessageLog.InsertIntoMessageLog(oMessageLog);
                //oMessageLogList.LoadData();
                //showAvailableCredit();
                //oUcButtonControl.showMessage(frmButtonControl.Messageflag.successMessage, "Sending completed successfully.");
            }
            else
            {
                //oUcButtonControl.showMessage(frmButtonControl.Messageflag.errorMessage, Global.DisplayError(result));
            }
        }
        #endregion

        #region " Fields Mapping "
        // 
        // Arraylist of Mail marge macrows 
        //
        private static ArrayList getFieldKeyList()
        {
            ArrayList keyList = new ArrayList();
            //keyList.Add("Salutation");
            //keyList.Add("Firstname");
            //keyList.Add("Lastname");
            //keyList.Add("Mobile");
            //keyList.Add("EmailID");
            //keyList.Add("Company");
            //keyList.Add("BirthDate");
            //keyList.Add("Anniversary");
            //keyList.Add("Jobtitle");
            //keyList.Add("Phone(O)");
            //keyList.Add("Phone(H)");
            //keyList.Add("FAX");

            //keyList.Add("FirstName");
            //keyList.Add("LastName");
            //keyList.Add("Code");

            //ArrayList list = oCompanyInfo.GetFieldNames();
            List<string> MacrosList = Enum.GetNames(typeof(MsgBlaster.DTO.Enums.Macros)).ToList(); // GetFieldNames();
            for (int i = 0; i < MacrosList.Count; i++)
            {
                if (!string.IsNullOrEmpty(MacrosList[i].ToString()))
                    keyList.Add(MacrosList[i].ToString());
            }
            return keyList;
        }
        //
        // ArrayList to map macrows with database Fields
        //
        private static ArrayList getFieldValueList()
        {
            ArrayList valueList = new ArrayList();
            //valueList.Add("SalutationName");
            //valueList.Add("FirstName");
            //valueList.Add("LastName");
            //valueList.Add("MobileNumber");
            //valueList.Add("EmailID");
            //valueList.Add("Company");
            //valueList.Add("BirthDate");
            //valueList.Add("Anniversary");
            //valueList.Add("JobTitle");
            //valueList.Add("PhoneOffice");
            //valueList.Add("PhoneHome");
            //valueList.Add("FAX");

            //valueList.Add("FirstName");
            //valueList.Add("LastName");
            //valueList.Add("Code");

            List<string> MacrosList = Enum.GetNames(typeof(MsgBlaster.DTO.Enums.Macros)).ToList(); // GetFieldNames();
            for (int i = 0; i < MacrosList.Count; i++)
            {
                if (!string.IsNullOrEmpty(MacrosList[i].ToString()))
                {
                    switch (i.ToString())
                    {
                        case "0":
                            valueList.Add("Label1");
                            break;
                        case "1":
                            valueList.Add("Label2");
                            break;
                        case "2":
                            valueList.Add("Label3");
                            break;
                        case "3":
                            valueList.Add("Label4");
                            break;
                        case "4":
                            valueList.Add("Label5");
                            break;
                    }
                }
            }
            return valueList;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using MsgBlaster.Repo;
using MsgBlaster.Domain;
using MsgBlaster.DTO;
using MsgBlaster.Service;
using System.Configuration;
using System.Net;

namespace msgBlasterBackendServiceForTodaysBirthdayAndAnniversary
{
    public class msgBlasterTodaysBirthdayAndAnniversary
    {
        static void Main(string[] args)
        {
            try
            {
                CreateCampaigns();
            }
            catch (Exception ex)
            {
                using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\msgBlasterTodaysBirthdayAndAnniversaryLog.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "  Main()" + " - " + ex.Message);
                    streamWriter.Close();
                }

            }
        }

        public static void CreateCampaigns()
        {
            List<ClientDTO> ClientDTOList = new List<ClientDTO>();
            ClientDTOList = ClientService.GetAllActiveClients();

            SettingDTO SettingDTO = new SettingDTO();
            SettingDTO = SettingService.GetById(1);

            if (ClientDTOList != null)
            {
                foreach (var item in ClientDTOList)
                {
                    if (item.IsSendBirthdayMessages || item.IsSendBirthdayCoupons)
                    {
                        string RecipientsNumberList = null;
                        int userId = UserService.GetAdminUserByClientId(item.Id);
                        List<ContactDTO> ContactDTOList = new List<ContactDTO>();
                        ContactDTOList = ContactService.GetContactsForTodaysBirthday();
                        if (ContactDTOList.Count > 0)
                        {
                            if (ContactDTOList.Count == 1)
                            {
                                foreach (var Contactitem in ContactDTOList)
                                {
                                    RecipientsNumberList = Contactitem.MobileNumber;
                                }
                            }
                            else {
                                    foreach (var Contactitem in ContactDTOList)
                                    {
                                        RecipientsNumberList = Contactitem.MobileNumber + "," + RecipientsNumberList;
                                    }

                                    RecipientsNumberList = RecipientsNumberList.Remove(RecipientsNumberList.Length - 1);
                            }

                            if (item.IsSendBirthdayMessages)
                            {
                                CampaignDTO CampaignDTO = new CampaignDTO();
                                CampaignDTO.Message = item.BirthdayMessage;
                                CampaignDTO.RecipientsNumber = RecipientsNumberList;
                                CampaignDTO.Name = "Birthday Message_" + string.Format("{0:G}", System.DateTime.Now); ;
                                CampaignDTO.ClientId = item.Id;
                                CampaignDTO.CreatedDate = System.DateTime.Now.Date;
                                CampaignDTO.IsScheduled = false;
                                CampaignDTO.GroupId = null;
                                CampaignDTO.ScheduledDate = System.DateTime.Now.Date;
                                CampaignDTO.CreatedBy = userId;

                                ////Calculate consumed credits 
                                //double ConsumedCreditPerOneMsg = CommonService.GetConsumedCreditsForOneMessage(CampaignDTO.Message, false);
                                //int RecepientsCount = CommonService.GetRecipientsCount(CampaignDTO.RecipientsNumber);
                                //CampaignDTO.ConsumededCredits = RecepientsCount * ConsumedCreditPerOneMsg;


                                CampaignService.CreateCampaignFromBackend(CampaignDTO);
                            }

                            if (item.IsSendBirthdayCoupons)
                            {
                                EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                                EcouponCampaignDTO.Message = item.BirthdayMessage;
                                EcouponCampaignDTO.ReceipentNumber = RecipientsNumberList;
                                EcouponCampaignDTO.Title = "Birthday Coupon_" + string.Format("{0:G}", System.DateTime.Now); ;
                                EcouponCampaignDTO.ClientId = item.Id;
                                EcouponCampaignDTO.CreatedDate = System.DateTime.Now.Date;
                                EcouponCampaignDTO.SendOn = System.DateTime.Now.Date;
                                EcouponCampaignDTO.IsScheduled = false;
                                EcouponCampaignDTO.GroupId = null;
                                EcouponCampaignDTO.MinPurchaseAmount = Convert.ToDouble(item.MinPurchaseAmountForBirthdayCoupon);
                                EcouponCampaignDTO.CreatedBy = userId;

                                int value = Convert.ToInt32(item.BirthdayCouponExpire);

                                string expire_format = item.BirthdayCouponExpireType;
                                if (expire_format == "Day")
                                {
                                    EcouponCampaignDTO.ExpiresOn = System.DateTime.Now.Date.AddDays(value);
                                }
                                else if (expire_format == "Month")
                                {
                                    EcouponCampaignDTO.ExpiresOn = System.DateTime.Now.Date.AddMonths(value);
                                }
                                else if (expire_format == "Week")
                                {
                                    value = value * 7;
                                    EcouponCampaignDTO.ExpiresOn = System.DateTime.Now.Date.AddDays(value);
                                }
                                ////Calculate consumed credits
                                //double ConsumedCreditPerOneMsg = CommonService.GetConsumedCreditsForOneMessage(EcouponCampaignDTO.Message, true);
                                //int RecepientsCount = CommonService.GetRecipientsCount(EcouponCampaignDTO.ReceipentNumber);
                                //EcouponCampaignDTO.ConsumededCredits = RecepientsCount * ConsumedCreditPerOneMsg;
                               
                                EcouponCampaignService.CreateEcouponCampaignBackend(EcouponCampaignDTO);
                            }
                        }
                    }

                    if (item.IsSendAnniversaryMessages || item.IsSendAnniversaryCoupons)
                    {

                        string RecipientsNumberList = null;
                        int userId = UserService.GetAdminUserByClientId(item.Id);
                        List<ContactDTO> ContactDTOList = new List<ContactDTO>();
                        ContactDTOList = ContactService.GetContactsForTodaysAnniversary();
                        if (ContactDTOList.Count > 0)
                        {
                            if (ContactDTOList.Count == 1)
                            {
                                foreach (var Contactitem in ContactDTOList)
                                {
                                    RecipientsNumberList = Contactitem.MobileNumber;
                                }
                            }
                            else
                            {
                                foreach (var Contactitem in ContactDTOList)
                                {
                                    RecipientsNumberList = Contactitem.MobileNumber + "," + RecipientsNumberList;
                                }

                                RecipientsNumberList = RecipientsNumberList.Remove(RecipientsNumberList.Length - 1);
                            }

                            if (item.IsSendAnniversaryMessages)
                            {
                                CampaignDTO CampaignDTO = new CampaignDTO();
                                CampaignDTO.Message = item.AnniversaryMessage;
                                CampaignDTO.RecipientsNumber = RecipientsNumberList;
                                CampaignDTO.Name = "Anniversary Message_" + string.Format("{0:G}", System.DateTime.Now); ;
                                CampaignDTO.ClientId = item.Id;
                                CampaignDTO.CreatedDate = System.DateTime.Now.Date;
                                CampaignDTO.IsScheduled = false;
                                CampaignDTO.GroupId = null;
                                CampaignDTO.ScheduledDate = System.DateTime.Now.Date;
                                CampaignDTO.CreatedBy = userId;

                                ////Calculate consumed credits
                                //double ConsumedCreditPerOneMsg = CommonService.GetConsumedCreditsForOneMessage(CampaignDTO.Message, false);
                                //int RecepientsCount = CommonService.GetRecipientsCount(CampaignDTO.RecipientsNumber);
                                //CampaignDTO.ConsumededCredits = RecepientsCount * ConsumedCreditPerOneMsg;

                                CampaignService.CreateCampaignFromBackend(CampaignDTO);
                            }

                            if (item.IsSendAnniversaryCoupons)
                            {
                                EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                                EcouponCampaignDTO.Message = item.AnniversaryMessage;
                                EcouponCampaignDTO.ReceipentNumber = RecipientsNumberList;
                                EcouponCampaignDTO.Title = "Anniversary Coupon_" + string.Format("{0:G}", System.DateTime.Now); ;
                                EcouponCampaignDTO.ClientId = item.Id;
                                EcouponCampaignDTO.CreatedDate = System.DateTime.Now.Date;
                                EcouponCampaignDTO.SendOn = System.DateTime.Now.Date;
                                EcouponCampaignDTO.IsScheduled = false;
                                EcouponCampaignDTO.GroupId = null;
                                EcouponCampaignDTO.MinPurchaseAmount = Convert.ToDouble(item.MinPurchaseAmountForAnniversaryCoupon);
                                EcouponCampaignDTO.CreatedBy = userId;

                                int value = Convert.ToInt32(item.AnniversaryCouponExpire);

                                string expire_format = item.AnniversaryCouponExpireType;
                                if (expire_format == "Day")
                                {
                                    EcouponCampaignDTO.ExpiresOn = System.DateTime.Now.Date.AddDays(value);
                                }
                                else if (expire_format == "Month")
                                {
                                    EcouponCampaignDTO.ExpiresOn = System.DateTime.Now.Date.AddMonths(value);
                                }
                                else if (expire_format == "Week")
                                {
                                    value = value * 7;
                                    EcouponCampaignDTO.ExpiresOn = System.DateTime.Now.Date.AddDays(value);
                                }

                                ////Calculate consumed credits
                                //double ConsumedCreditPerOneMsg = CommonService.GetConsumedCreditsForOneMessage(EcouponCampaignDTO.Message, true);
                                //int RecepientsCount = CommonService.GetRecipientsCount(EcouponCampaignDTO.ReceipentNumber);
                                //EcouponCampaignDTO.ConsumededCredits = RecepientsCount * ConsumedCreditPerOneMsg;

                                EcouponCampaignService.CreateEcouponCampaignBackend(EcouponCampaignDTO);
                            }
                        }
                    }
                }
            }
        }

       

    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MsgBlaster.Domain;

namespace MsgBlaster.Repo
{
    public class Configurations
    {
        public class CampaignConfigurations : EntityTypeConfiguration<Campaign>
        {
            public CampaignConfigurations()
            {
                //HasMany(a => a.CampaignLogs).WithRequired(i => i.Campaign).WillCascadeOnDelete(true);

                Property(b => b.Name).HasMaxLength(100).IsRequired();
                Property(b => b.RecipientsNumber).IsRequired();
                Property(b => b.CreatedDate).IsRequired();
                Property(b => b.Message).IsRequired();
                Property(b => b.ScheduledTime).HasMaxLength(10);
                Property(b => b.IPAddress).HasMaxLength(20);
                ////  Property(b => b.ScheduledDate).IsRequired();
                //// Property(b => b.ScheduledTime).IsRequired();
                //Property(b => b.Groups).HasMaxLength(100);

                
                
              Property(b => b.CreatedBy).IsRequired();
                //Property(b => b.ClientId).IsRequired(); 
              //HasRequired(b => b.CreatedBy).WithMany(d => d.Users).Map(m => m.MapKey("CreatedBy"));  
              Property(b => b.IsUnicode).IsRequired();
              Property(b => b.LanguageCode).HasMaxLength(50);
            }
        }

        //public class BillConfigurations : EntityTypeConfiguration<Bill>
        //{
        //    public BillConfigurations()
        //    {
        //        HasMany(a => a.BillTaxes).WithRequired(i => i.Bill).WillCascadeOnDelete(true);
        //        HasMany(a => a.Receipts).WithRequired(i => i.Bill).WillCascadeOnDelete(true);
        //    }
        //}

        public class ClientConfigurations : EntityTypeConfiguration<Client>
        {
            public ClientConfigurations()
            {
                
                HasMany(a => a.Templates).WithRequired(i => i.Client).WillCascadeOnDelete(true);
                HasMany(a => a.Contacts).WithRequired(i => i.Client).WillCascadeOnDelete(false);
                HasMany(a => a.Groups).WithRequired(i => i.Client).WillCascadeOnDelete(false);
                HasMany(a => a.Campaigns).WithRequired(i => i.Client).WillCascadeOnDelete(true);
                HasMany(a => a.CreditRequests).WithRequired(i => i.Client).WillCascadeOnDelete(true);
                HasMany(a => a.EcouponCampaigns).WithRequired(i => i.Client).WillCascadeOnDelete(true);
                HasMany(a => a.Users).WithRequired(i => i.Client).WillCascadeOnDelete(false); //true
                HasMany(a => a.Locations).WithRequired(i => i.Client).WillCascadeOnDelete(false);

                //HasMany(a => a.Tickets).WithRequired(i => i.Client).WillCascadeOnDelete(true);


                //Property(b => b.Name).HasMaxLength(100).IsRequired();
                Property(b => b.Company).HasMaxLength(100).IsRequired();
                //Property(b => b.Mobile).HasMaxLength(10).IsRequired();
                //Property(b => b.Email).HasMaxLength(150).IsRequired();
                //Property(b => b.Username).HasMaxLength(15).IsRequired();
                //Property(b => b.Password).HasMaxLength(100).IsRequired();
                Property(b => b.RegisteredDate).IsRequired();
                Property(b => b.Address).HasMaxLength(500);
                Property(b => b.SenderCode).HasMaxLength(6);
                Property(b => b.DefaultCouponMessage).HasMaxLength(30);
            }
        }

        //public class CampaignLogConfigurations : EntityTypeConfiguration<CampaignLog>
        //{
        //    public CampaignLogConfigurations()
        //    {
        //        Property(b => b.RecipientsNumber).IsRequired();
        //        Property(b => b.MessageStatus).HasMaxLength(150).IsRequired();
        //        Property(b => b.Message).IsRequired();
        //        Property(b => b.GatewayID).HasMaxLength(10).IsRequired();
        //        Property(b => b.MessageID).HasMaxLength(10).IsRequired();

        //    }
        //}


        public class EcouponCampaignConfigurations : EntityTypeConfiguration<EcouponCampaign>
        {
            public EcouponCampaignConfigurations()
            {
                HasMany(a => a.Coupons).WithRequired(i => i.EcouponCampaign).WillCascadeOnDelete(false);

                Property(b => b.Title).HasMaxLength(100).IsRequired();
                Property(b => b.CreatedDate).IsRequired();
                Property(b => b.Message).IsRequired();
                //Property(b => b.ExpiresOn).IsRequired();
                Property(b => b.SendOn).IsRequired();

                //Property(b => b.ClientId).IsRequired();
                Property(b => b.CreatedBy).IsRequired();
                Property(b => b.ScheduleTime).HasMaxLength(10);
                Property(b => b.IPAddress).HasMaxLength(20);
                //Property(b => b.Groups).HasMaxLength(100);
                Property(b => b.MinPurchaseAmount).IsRequired();
            }
        }

        public class UserConfigurations : EntityTypeConfiguration<User>
        {
            public UserConfigurations()
            {
                //Property(b => b.Name).HasMaxLength(100).IsRequired();

                Property(b => b.FirstName).HasMaxLength(25).IsRequired();
                Property(b => b.LastName).HasMaxLength(25).IsRequired();

                Property(b => b.Email).HasMaxLength(150).IsRequired();
                ////Property(b => b.Address).HasMaxLength(500).IsRequired();
                //Property(b => b.Location).HasMaxLength(50);
                Property(b => b.LocationId).IsRequired();
                //////Property(b => b.Username).HasMaxLength(15).IsRequired();
                Property(b => b.Password).HasMaxLength(50).IsRequired();
                Property(b => b.ClientId).IsRequired();
                Property(b => b.Mobile).HasMaxLength(10); //.IsRequired();

                ////HasMany(a => a.Campaigns).WithRequired(i => i.User).WillCascadeOnDelete(false);
                ////HasMany(a => a.CreditRequests).WithRequired(i => i.User).WillCascadeOnDelete(false);
                ////HasMany(a => a.EcouponCampaigns).WithRequired(i => i.User).WillCascadeOnDelete(false);

                
               

            }
        }

        public class CouponConfigurations : EntityTypeConfiguration<Coupon>
        {
            public CouponConfigurations()
            {
               // Property(b => b.MobileNumber).HasMaxLength(10).IsRequired();
                Property(b => b.Code).IsRequired();
                Property(b => b.EcouponCampaignId).IsRequired();
                //Property(b => b.RedeemDateTime).IsOptional();
                Property(b => b.Remark).HasMaxLength(500);
                //Property(b => b.ClientUserId).IsOptional();
                //Property(b => b.SentDateTime).IsRequired();
                Property(b => b.Message).IsRequired();
                
                Property(b => b.BillNumber).HasMaxLength(50).IsOptional();
                Property(b => b.BillDate).IsOptional();

            }
        }

        public class PartnerConfigurations : EntityTypeConfiguration<Partner>
        {
            public PartnerConfigurations()
            {
                HasMany(a => a.CreditRequests).WithRequired(i => i.Partner).WillCascadeOnDelete(true);
                HasMany(a => a.Clients).WithRequired(i => i.Partner).WillCascadeOnDelete(false);

                Property(b => b.Name).HasMaxLength(100).IsRequired();
                Property(b => b.Company).HasMaxLength(100).IsRequired();
                Property(b => b.Mobile).HasMaxLength(10).IsRequired();
                Property(b => b.Email).HasMaxLength(150).IsRequired();
                //Property(b => b.Username).HasMaxLength(15).IsRequired();
                Property(b => b.Password).HasMaxLength(50).IsRequired();                

            }
        }

        public class PlanConfigurations : EntityTypeConfiguration<Plan>
        {
            public PlanConfigurations()
            {
                ////HasMany(a => a.CreditRequests).WithRequired(i => i.Plan).WillCascadeOnDelete(true);
                //HasMany(a => a.Clients).WithRequired(i => i.Plan).WillCascadeOnDelete(false);

                Property(b => b.Title).HasMaxLength(20).IsRequired();
                Property(b => b.Price).IsRequired();
                Property(b => b.Min).IsRequired();
                Property(b => b.Max).IsRequired();
            }
        }

        public class ReceiptConfigurations : EntityTypeConfiguration<Receipt>
        {
            public ReceiptConfigurations()
            {

                //Property(b => b.BillId).IsRequired();
                Property(b => b.Date).IsRequired();
                Property(b => b.ReceiptNumber).IsRequired();
                Property(b => b.PaymentMode).IsRequired();

                Property(b => b.PartnerId).IsRequired();
                Property(b => b.ClientId).IsRequired();

            }
        }

        public class GroupConfigurations : EntityTypeConfiguration<Group>
        {
            public GroupConfigurations()
            {
                Property(b => b.Name).HasMaxLength(30).IsRequired();
            }
        }

        public class ContactConfigurations : EntityTypeConfiguration<Contact>
        {
            public ContactConfigurations()
            {
                Property(b => b.MobileNumber).HasMaxLength(10).IsRequired();
                Property(b => b.Email).HasMaxLength(150);
                //Property(b => b.Name).HasMaxLength(100).IsRequired();
                Property(b => b.FirstName).HasMaxLength(25).IsRequired();
                Property(b => b.LastName).HasMaxLength(25).IsRequired();
                Property(b => b.Gender).IsRequired();
            }
        }

        public class CreditRequestConfigurations : EntityTypeConfiguration<CreditRequest>
        {
            public CreditRequestConfigurations()
            {
                Property(b => b.RequestedCredit).IsRequired();
                Property(b => b.Date).IsRequired();
                Property(b => b.RequestedBy).IsRequired();
                //Property(b => b.ClientId).IsRequired();             
                Property(b => b.PaymentId).HasMaxLength(100);               
            }
        }

        public class SettingConfigurations : EntityTypeConfiguration<Setting>
        {
            public SettingConfigurations()
            {
                Property(b => b.MessageLength).IsRequired();
                Property(b => b.SingleMessageLength).IsRequired();
                Property(b => b.MobileNumberLength).IsRequired();
                Property(b => b.NationalCampaignSMSCount).IsRequired();
                Property(b => b.NationalCouponSMSCount).IsRequired();
            }
        }

        //public class SMSGatewayConfigurations : EntityTypeConfiguration<SMSGateway>
        //{
        //    public SMSGatewayConfigurations()
        //    {
        //        HasMany(a => a.Clients).WithRequired(i => i.SMSGateway).WillCascadeOnDelete(true);
        //        Property(b => b.Name).HasMaxLength(10).IsRequired();
        //    }
        //}


        public class TemplateConfigurations : EntityTypeConfiguration<Template>
        {
            public TemplateConfigurations()
            {
                Property(b => b.Title).HasMaxLength(50).IsRequired();
                Property(b => b.Message).IsRequired();
                Property(b => b.ClientId).IsRequired();
            }
        }

        public class DocumentConfigurations : EntityTypeConfiguration<Document>
        {
            public DocumentConfigurations()
            {
                Property(b => b.FileName).HasMaxLength(50).IsRequired();
                Property(b => b.Path).HasMaxLength(100).IsRequired();
                Property(b => b.UserId).IsRequired();
                Property(b => b.ClientId).IsRequired();
                Property(b => b.CreatedOn).IsRequired();

            }
 
        }

        public class ActivityLogConfigurations : EntityTypeConfiguration<ActivityLog>
        {
            public ActivityLogConfigurations()
            {
                Property(b => b.OperationType).HasMaxLength(20).IsRequired();
                Property(b => b.EntityType).HasMaxLength(20).IsRequired();
            }
        }

        public class LocationConfigurations : EntityTypeConfiguration<Location>
        {
            public LocationConfigurations()
            {
                //Property(b => b.ClientId).IsRequired();
                Property(b => b.Name).HasMaxLength(30).IsRequired();
                Property(b => b.ClientId).IsRequired();
              
            }
        }


        public class CampaignLogXMLConfigurations : EntityTypeConfiguration<CampaignLogXML>
        {
            public CampaignLogXMLConfigurations()
            {
                //Property(b => b.ClientId).IsRequired();
                Property(b => b.XMLLog).IsRequired();
                Property(b => b.CampaignId).IsRequired();
            }
        }

        public class GroupContactConfigurations : EntityTypeConfiguration<GroupContact>
        {
            public GroupContactConfigurations()
            {
                //Property(b => b.ClientId).IsRequired();
                Property(b => b.Id).IsRequired();
                Property(b => b.ContactId).IsRequired();
                Property(b => b.GroupId).IsRequired();
            }
        }       


    }
}

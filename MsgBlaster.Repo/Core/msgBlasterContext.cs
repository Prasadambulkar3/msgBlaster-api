using System.Data.Entity;
using System.Reflection;
using MsgBlaster.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsgBlaster.Repo
{
    public class MsgBlasterContext : DbContext 
    {
        public MsgBlasterContext()
            : base("msgBlasterWebContext")
            {
                //this.Database.Connection.ConnectionString = "Data Source=Adserver;Initial Catalog=MsgBlaster5March;User ID=sa;Password=myfair@123";
                //Database.SetInitializer<MsgBlasterContext>(new CreateDatabaseIfNotExists<MsgBlasterContext>());

                Database.SetInitializer(new msgBlasterWebInitializer());

            }

        //public DbSet<Bill> Bills { get; set; }
        //public DbSet<BillTax> BillTaxes { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        //public DbSet<CampaignLog> CampaignLogs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CreditRequest> CreditRequests { get; set; }
        public DbSet<EcouponCampaign> EcouponCampaigns { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Setting> Settings { get; set; }
        //public DbSet<SMSGateway> SMSGateways { get; set; }
        //public DbSet<Tax> Taxes { get; set; }
        public DbSet<Template> Templates { get; set; }
        //public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RedeemedCount> RedeemedCounts { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<CampaignLogXML> CampaignLogXMLs { get; set; }
        public DbSet<GroupContact> GroupContacts { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().Ignore(t => t.Name);
            modelBuilder.Entity<User>().Ignore(t => t.Name);

            modelBuilder.Entity<CreditRequest>().Ignore(t => t._clientName);
            modelBuilder.Entity<CreditRequest>().Ignore(t => t.ClientName);

            modelBuilder.Entity<CreditRequest>().Ignore(t => t._userName);
            modelBuilder.Entity<CreditRequest>().Ignore(t => t.UserName);


            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());           
        }

        public class msgBlasterWebInitializer : CreateDatabaseIfNotExists<MsgBlasterContext>//DropCreateDatabaseAlways //CreateDatabaseIfNotExists //DropCreateDatabaseIfModelChanges
        {
            protected override void Seed(MsgBlasterContext context)
            {
                //context = LoadDummyData(context);

                Setting setting = new Setting()
                {
                    MessageLength = 160,
                    MobileNumberLength = 10,
                    SingleMessageLength = 153,
                    UTFFirstMessageLength =70,
                    UTFSecondMessageLength = 64,
                    UTFNthMessageLength = 67,
                    NationalCampaignSMSCount = 1,
                    NationalCouponSMSCount =1

                };
                context.Settings.Add(setting);

                Plan Plan1 = new Plan()
                {
                    Title = "Upto 1000",
                    Min = 0,
                    Max = 1000,
                    Price = 0.45,
                    Tax = 2


                };
                context.Plans.Add(Plan1);

                Plan Plan2 = new Plan()
                {
                    Title = "Upto 10000",
                    Min = 1000,
                    Max = 10000,
                    Price = 0.35,
                    Tax = 2


                };
                context.Plans.Add(Plan2);

                Plan Plan3 = new Plan()
                {
                    Title = "Upto 30000",
                    Min = 10000,
                    Max = 30000,
                    Price = 0.25,
                    Tax = 2


                };
                context.Plans.Add(Plan3);

                Plan Plan4 = new Plan()
                {
                    Title = "Upto 100000",
                    Min = 30000,
                    Max = 100000,
                    Price = 0.15,
                    Tax = 2


                };
                context.Plans.Add(Plan4);

                Plan Plan5 = new Plan()
                {
                    Title = "Upto 200000",
                    Min = 100000,
                    Max = 200000,
                    Price = 0.10,
                    Tax = 2


                };
                context.Plans.Add(Plan5);

             

              

                //SMSGateway SMSGateway = new SMSGateway()
                //{
                //    Name = "Default",
                //    //Link = "sms6.routesms.com/bulksms/bulksms?username=gracework&password=pra07sha&type=0&dlr=0&destination=[recipient]&source=MSGBLS&message=[message]",
                //    IsActive = true,
                //    IsTransactional = false// GatewayType.Promotional,

                //};
                //context.SMSGateways.Add(SMSGateway);


                //SMSGateway SMSGatewayTrans = new SMSGateway()
                //{
                //    Name = "MSGBLS",
                //    //Link = "sms6.routesms.com/bulksms/bulksms?username=gracework&password=pra07sha&type=0&dlr=0&destination=[recipient]&source=MSGBLS&message=[message]",
                //    IsActive = true,
                //    IsTransactional = true// GatewayType.Promotional,

                //};
                //context.SMSGateways.Add(SMSGatewayTrans);


                Partner adminpartner = new Partner()
                {
                    Company = "Graceworks",
                    Email = "msgblpartner@gmail.com",
                    Mobile = "9860059944",
                    Name = "Admin",
                    Password = "admin123",
                    //Username="admin",
                    //UserType= UserType.Admin,
                    AlertOnCredit = false
                };
                context.Partners.Add(adminpartner);


                Client client = new Client()
                {

                    Address = "Kolhapur",
                    Company = "Graceworks",
                    IsActive = true,
                    //Mobile = "9860059944",
                    //Name = "Test Client",
                    RegisteredDate = System.DateTime.Now,
                    SMSCredit = 20,
                    AlertOnCredit = false,
                    //Email = "Rohit.kale@graceworks.com",
                    Partner = adminpartner,
                    //Username="client",
                    AllowChequePayment = true,
                    //Password = "client",
                    //Plan = Plan1,
                    //SMSGateway = SMSGatewayTrans //SMSGateway   
                };
                context.Clients.Add(client);
                //SMSGateway.Clients.Add(client);

                Group group1 = new Group()
                {
                    Name = "Friends",
                    Client = client
                };
                context.Groups.Add(group1);

                Group group2 = new Group()
                {
                    Name = "Family",
                    Client = client
                };
                context.Groups.Add(group2);

                Location Location1 = new Location()
                {
                    Name = "Mumbai",
                    Client = client
                };
                context.Locations.Add(Location1);

                Location Location2 = new Location()
                {
                    Name = "Pune",
                    Client = client
                };
                context.Locations.Add(Location2);


                User userAdmin = new User()
                {
                   
                    FirstName="Admin",
                    LastName="A",                    
                    Email = "msgblasterclient@gmail.com",
                    //Address = "Kolhapur",
                    Location = Location1,
                    Password = "admin123",
                    Mobile = "9860059944",
                    Client = client,
                    IsActive = true,
                    UserType = UserType.Admin

                };
                context.Users.Add(userAdmin);

                User userEcoupon = new User()
                {                   
                    FirstName = "User",
                    LastName = "U",
                    Email = "msgbluser@gmail.com",
                    //Address = "Kolhapur",
                    Location = Location2,
                    Password = "user123",
                    Mobile = "9960802767",
                    Client = client,
                    IsActive = true,
                    UserType = UserType.Redeem

                };
                context.Users.Add(userEcoupon);





                //CreditRequest creditRequest = new CreditRequest()
                //{
                //    Client = client,
                //    User = userAdmin,
                //    Date = System.DateTime.Now,
                //    RequestedCredit = 10000,
                //    IsProvided = false,
                //    RatePerSMS = 0.25,
                //    Amount = (10000 * 0.25),
                //    GrandTotal = (10000 * 0.25),
                //    Partner = adminpartner,
                //    OldBalance = client.SMSCredit,
                //};
                //context.CreditRequests.Add(creditRequest);

                //Campaign campaignscheduled = new Campaign()
                //{
                //    Client = client,
                //    User = userAdmin,
                //    Name = "Test Campaign",
                //    RecipientsNumber = "9860059944", //98502154, 9960802767, 9860059944, 8390192419
                //    RecipientsCount=1,
                //    Message = "Always applies to the nearest enclosing scope, so you couldn't use it to break out of the outermost loop.",//"+<>&\'%#*&!,'\\=&lt&gt  This is test message",
                //    CreatedDate = System.DateTime.Now,
                //    IsScheduled = false,
                //    ScheduledDate = System.DateTime.Now.Date.AddDays(1),
                //    ScheduledTime = "10:00 AM",
                //    MessageCount = 1,
                //    RequiredCredits = 1,
                //    IsSent = false,

                //};
                //context.Campaigns.Add(campaignscheduled);



                //Campaign campaign = new Campaign()
                //{
                //    Client = client,
                //    User = userAdmin,
                //    Name = "Direct_Message",
                //    RecipientsNumber = "9860059944,9960802767,8390192419, 9850613787", //98502154, 9960802767, 9860059944, 8390192419
                //    RecipientsCount = 4,
                //    Message = "Always applies to the nearest enclosing scope, so you couldn't use it to break out of the outermost loop.",//"+<>&\'%#*&!,'\\=&lt&gt  This is test message",
                //    CreatedDate = System.DateTime.Now,
                //    IsScheduled = false,
                //    ScheduledDate = System.DateTime.Now,
                //    ScheduledTime = "",
                //    MessageCount = 1,
                //    RequiredCredits = 4,
                //    IsSent = true,

                //};
                //context.Campaigns.Add(campaign);

                //CampaignLog campaignLog1 = new CampaignLog()
                //{
                //    Campaign = campaign,
                //    IsSuccess = true,
                //    RecipientsNumber = "9860059944",
                //    MessageStatus = "Succuess-1706",
                //    Message = campaign.Message,
                //    GatewayID = SMSGateway.Name,
                //    SentDateTime = System.DateTime.Now,
                //    MessageID = "1701",
                //    MessageCount = 1
                //};
                //context.CampaignLogs.Add(campaignLog1);

                //CampaignLog campaignLog2 = new CampaignLog()
                //{
                //    Campaign = campaign,
                //    IsSuccess = true,
                //    RecipientsNumber = "9960802767",
                //    MessageStatus = "Succuess-1706",
                //    Message = campaign.Message,
                //    GatewayID = SMSGateway.Name,
                //    SentDateTime = System.DateTime.Now,
                //    MessageID = "1701",
                //    MessageCount = 1
                //};
                //context.CampaignLogs.Add(campaignLog2);

                //CampaignLog campaignLog3 = new CampaignLog()
                //{
                //    Campaign = campaign,
                //    IsSuccess = true,
                //    RecipientsNumber = "8390192419",
                //    MessageStatus = "Succuess-1706",
                //    Message = campaign.Message,
                //    GatewayID = SMSGateway.Name,
                //    SentDateTime = System.DateTime.Now,
                //    MessageID = "1701",
                //    MessageCount = 1
                //};
                //context.CampaignLogs.Add(campaignLog3);


                //CampaignLog campaignLog4 = new CampaignLog()
                //{
                //    Campaign = campaign,
                //    IsSuccess = true,
                //    RecipientsNumber = "9850613787",
                //    MessageStatus = "Succuess-1706",
                //    Message = campaign.Message,
                //    GatewayID = SMSGateway.Name,
                //    SentDateTime = System.DateTime.Now,
                //    MessageID = "1701",
                //    MessageCount = 1
                //};
                //context.CampaignLogs.Add(campaignLog4);


                //EcouponCampaign ecouponCampaign = new EcouponCampaign()
                //{
                //    Client = client,
                //    User = userAdmin,
                //    Title = "Direct_Message",
                //    ReceipentNumber = "9860059944,9960802767,8390192419, 9850613787", // 9960802767, 9860059944, 8390192419
                //    RecipientsCount = 4,
                //    Message = "Always applies to the nearest enclosing scope, If a condition like that arises, you'd need to",//"+<>&\'%#*&!,'\\=&lt&gt  This is test message",
                //    CreatedDate = System.DateTime.Now,
                //    ExpiresOn= System.DateTime.Now.AddDays(25),
                //    IsScheduled = false,
                //    SendOn = System.DateTime.Now,
                //    ScheduleTime = "",
                //    //MessageCount = 1,
                //    //RequiredCredits = 1,
                //    IsSent = true,

                //};
                //context.EcouponCampaigns.Add(ecouponCampaign);

                //EcouponCampaign ecouponCampaign1 = new EcouponCampaign()
                //{
                //    Client = client,
                //    User = userAdmin,
                //    Title = "Test coupon campaign",
                //    ReceipentNumber = "9665781461,9730159369", // 9960802767, 9860059944, 8390192419
                //    RecipientsCount = 2,
                //    Message = "Always applies to the nearest enclosing scope, If a condition like that arises, you'd need to",//"+<>&\'%#*&!,'\\=&lt&gt  This is test message",
                //    CreatedDate = System.DateTime.Now,
                //    ExpiresOn = System.DateTime.Now.AddDays(20),
                //    IsScheduled = false,
                //    SendOn = System.DateTime.Now,
                //    ScheduleTime = "",
                //    //MessageCount = 1,
                //    //RequiredCredits = 1,
                //    IsSent = true,

                //};
                //context.EcouponCampaigns.Add(ecouponCampaign1);

                //Coupon coupon5 = new Coupon()
                //{
                //    MobileNumber = "9665781461",
                //    Code = "123456",
                //    IsRedeem = false,
                //    IsExpired = false,
                //    EcouponCampaign = ecouponCampaign1,
                //    Message = ecouponCampaign.Message,
                //    SentDateTime = System.DateTime.Now.Date,
                //    MessageCount = 1
                //};
                //context.Coupons.Add(coupon5);

                //Coupon coupon6 = new Coupon()
                //{
                //    MobileNumber = "9730159369",
                //    Code = "654321",
                //    IsRedeem = false,
                //    IsExpired = false,
                //    EcouponCampaign = ecouponCampaign1,
                //    Message = ecouponCampaign.Message,
                //    SentDateTime = System.DateTime.Now.Date,
                //    MessageCount = 1

                //};
                //context.Coupons.Add(coupon6);


                //Coupon coupon1 = new Coupon()
                //{
                //    MobileNumber = "9860059944",
                //    Code = "123456",
                //    IsRedeem = false,
                //    IsExpired = false,
                //    EcouponCampaign = ecouponCampaign,
                //    Message = ecouponCampaign.Message,
                //    SentDateTime = System.DateTime.Now.Date,
                //    MessageCount=1
                //};
                //context.Coupons.Add(coupon1);

                //Coupon coupon2 = new Coupon()
                //{
                //    MobileNumber = "9960802767",
                //    Code = "654321",
                //    IsRedeem = false,
                //    IsExpired = false,
                //    EcouponCampaign = ecouponCampaign,
                //    Message = ecouponCampaign.Message,
                //    SentDateTime = System.DateTime.Now.Date,
                //    MessageCount = 1

                //};
                //context.Coupons.Add(coupon2);

                //Coupon coupon3 = new Coupon()
                //{
                //    MobileNumber = "8390192419",
                //    Code = "234567",
                //    IsRedeem = false,
                //    IsExpired = false,
                //    EcouponCampaign = ecouponCampaign,
                //    Message = ecouponCampaign.Message,
                //    SentDateTime = System.DateTime.Now.Date,
                //    MessageCount = 1

                //};
                //context.Coupons.Add(coupon3);

                //Coupon coupon4 = new Coupon()
                //{
                //    MobileNumber = "9850613787",
                //    Code = "345678",
                //    IsRedeem = false,
                //    IsExpired = false,
                //    EcouponCampaign = ecouponCampaign,
                //    Message = ecouponCampaign.Message,
                //    SentDateTime =  System.DateTime.Now.Date,
                //    MessageCount = 1

                //};
                //context.Coupons.Add(coupon4);



                //EcouponCampaign ecouponCampaignScheduled = new EcouponCampaign()
                //{
                //    Client = client,
                //    User = userAdmin,
                //    Title = "Direct_Message",
                //    ReceipentNumber = "9860059944", //98502154, 9960802767, 9860059944, 8390192419
                //    RecipientsCount =1,
                //    Message = "Always applies to the nearest enclosing scope, If a condition like that arises, you'd need to",//"+<>&\'%#*&!,'\\=&lt&gt  This is test message",
                //    CreatedDate = System.DateTime.Now,
                //    IsScheduled = true,
                //    SendOn = System.DateTime.Now.AddDays(2),
                //    ScheduleTime = "10:00 AM",
                //    //MessageCount = 1,
                //    //RequiredCredits = 1,
                //    IsSent = false,

                //};
                //context.EcouponCampaigns.Add(ecouponCampaignScheduled);


                //EcouponCampaign ecouponCampaignExprired = new EcouponCampaign()
                //{
                //    Client = client,
                //    User = userAdmin,
                //    Title = "Direct_Message",
                //    ReceipentNumber = "9860059944", //98502154, 9960802767, 9860059944, 8390192419
                //    RecipientsCount =1,
                //    Message = "Always applies to the nearest enclosing scope, If a condition like that arises, you'd need to",//"+<>&\'%#*&!,'\\=&lt&gt  This is test message",
                //    CreatedDate = System.DateTime.Now,
                //    IsScheduled = false,
                //    SendOn = System.DateTime.Now,
                //    ScheduleTime = "",
                //    ExpiresOn = System.DateTime.Now,
                //    IsSent = true,

                //};
                //context.EcouponCampaigns.Add(ecouponCampaignExprired);


                //Coupon coupon = new Coupon()
                //{
                //    MobileNumber = "9860059944",
                //    Code = "223456",
                //    IsRedeem = false,
                //    IsExpired = true,
                //    EcouponCampaign = ecouponCampaignExprired,
                //    Message = ecouponCampaign.Message,
                //    SentDateTime =  System.DateTime.Now.Date,
                //    MessageCount = 1

                //};
                //context.Coupons.Add(coupon);


                base.Seed(context);
            }
        }
    }
}

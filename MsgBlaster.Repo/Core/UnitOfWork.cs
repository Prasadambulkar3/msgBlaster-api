using System;
using System.Data.Common;
using System.Data.Entity.Validation;
using System.Reflection;
using MsgBlaster.Domain;
using MsgBlaster.Repo.Interfaces;
using MsgBlaster.Domain.Interfaces;
using System.Collections.Generic;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;



namespace MsgBlaster.Repo
{
    public class UnitOfWork : IDisposable
    {
        private readonly MsgBlasterContext _context;
        private bool _disposed;

        public UnitOfWork()
        {
            _disposed = false;
            _context = new MsgBlasterContext();
        }
        internal MsgBlasterContext MsgBlasterContext
        {
            get { return _context; }
        }

        #region Public Repositories

        //private BillRepo _billRepo;
        //public BillRepo BillRepo
        //{
        //    get { return _billRepo ?? (_billRepo = new BillRepo(this)); }
        //    set { _billRepo = value; }
        //}

        //private BillTaxRepo _billTaxRepo;
        //public BillTaxRepo BillTaxRepo
        //{
        //    get { return _billTaxRepo ?? (_billTaxRepo = new BillTaxRepo(this)); }
        //    set { _billTaxRepo = value; }
        //}

        private CampaignRepo _campaignRepo;
        public CampaignRepo CampaignRepo
        {
            get { return _campaignRepo ?? (_campaignRepo = new CampaignRepo(this)); }
            set { _campaignRepo = value; }
        }

        //private CampaignLogRepo _campaignLogRepo;
        //public CampaignLogRepo CampaignLogRepo
        //{
        //    get { return _campaignLogRepo ?? (_campaignLogRepo = new CampaignLogRepo(this)); }
        //    set { _campaignLogRepo = value; }
        //}

        private ClientRepo _clientRepo;
        public ClientRepo ClientRepo
        {
            get { return _clientRepo ?? (_clientRepo = new ClientRepo(this)); }
            set { _clientRepo = value; }
        }

        private ContactRepo _contactRepo;
        public ContactRepo ContactRepo
        {
            get { return _contactRepo ?? (_contactRepo = new ContactRepo(this)); }
            set { _contactRepo = value; }
        }

        private CreditRequestRepo _creditRequestRepo;
        public CreditRequestRepo CreditRequestRepo
        {
            get { return _creditRequestRepo ?? (_creditRequestRepo = new CreditRequestRepo(this)); }
            set { _creditRequestRepo = value; }
        }

        private EcouponCampaignRepo _ecouponCampaignRepo;
        public EcouponCampaignRepo EcouponCampaignRepo
        {
            get { return _ecouponCampaignRepo ?? (_ecouponCampaignRepo = new EcouponCampaignRepo(this)); }
            set { _ecouponCampaignRepo = value; }
        }

        private GroupRepo _groupRepo;
        public GroupRepo GroupRepo
        {
            get { return _groupRepo ?? (_groupRepo = new GroupRepo(this)); }
            set { _groupRepo = value; }
        }

        private PartnerRepo _partnerRepo;
        public PartnerRepo PartnerRepo
        {
            get { return _partnerRepo ?? (_partnerRepo = new PartnerRepo(this)); }
            set { _partnerRepo = value; }
        }

        private PlanRepo _planRepo;
        public PlanRepo PlanRepo
        {
            get { return _planRepo ?? (_planRepo = new PlanRepo(this)); }
            set { _planRepo = value; }
        }

        private ReceiptRepo _receiptRepo;
        public ReceiptRepo ReceiptRepo
        {
            get { return _receiptRepo ?? (_receiptRepo = new ReceiptRepo(this)); }
            set { _receiptRepo = value; }
        }

        private SettingRepo _settingRepo;
        public SettingRepo SettingRepo
        {
            get { return _settingRepo ?? (_settingRepo = new SettingRepo(this)); }
            set { _settingRepo = value; }
        }


        //private SMSGatewayRepo _smsgatewayRepo;
        //public SMSGatewayRepo SMSGatewayRepo
        //{
        //    get { return _smsgatewayRepo ?? (_smsgatewayRepo = new SMSGatewayRepo(this)); }
        //    set { _smsgatewayRepo = value; }
        //}

        //private TaxRepo _taxRepo;
        //public TaxRepo TaxRepo
        //{
        //    get { return _taxRepo ?? (_taxRepo = new TaxRepo(this)); }
        //    set { _taxRepo = value; }
        //}

        private TemplateRepo _templateRepo;
        public TemplateRepo TemplateRepo
        {
            get { return _templateRepo ?? (_templateRepo = new TemplateRepo(this)); }
            set { _templateRepo = value; }
        }


        private CouponRepo _couponRepo;
        public CouponRepo CouponRepo
        {
            get { return _couponRepo ?? (_couponRepo = new CouponRepo(this)); }
            set { _couponRepo = value; }
        }


        private UserRepo _userRepo;
        public UserRepo UserRepo
        {
            get { return _userRepo ?? (_userRepo = new UserRepo(this)); }
            set { _userRepo = value; }
        }


        private RedeemedCountRepo _redeemedCountRepo;
        public RedeemedCountRepo RedeemedCountRepo
        {
            get { return _redeemedCountRepo ?? (_redeemedCountRepo = new RedeemedCountRepo(this)); }
            set { _redeemedCountRepo = value; }
        }

        private ActivityLogRepo _activityLogRepo;
        public ActivityLogRepo ActivityLogRepo
        {
            get { return _activityLogRepo ?? (_activityLogRepo = new ActivityLogRepo(this)); }
            set { _activityLogRepo = value; }
        }


        private DocumentRepo _documentRepo;
        public DocumentRepo DocumentRepo
        {
            get { return _documentRepo ?? (_documentRepo = new DocumentRepo(this)); }
            set { _documentRepo = value; }
        }

        //Location
        private LocationRepo _locationRepo;
        public LocationRepo LocationRepo
        {
            get { return _locationRepo ?? (_locationRepo = new LocationRepo(this)); }
            set { _locationRepo = value; }
        }


        private CampaignLogXMLRepo _campaignLogXMLRepo;
        public CampaignLogXMLRepo CampaignLogXMLRepo
        {
            get { return _campaignLogXMLRepo ?? (_campaignLogXMLRepo = new CampaignLogXMLRepo(this)); }
            set { _campaignLogXMLRepo = value; }
        }



        private GroupContactRepo _groupContactRepo;
        public GroupContactRepo GroupContactRepo
        {
            get { return _groupContactRepo ?? (_groupContactRepo = new GroupContactRepo(this)); }
            set { _groupContactRepo = value; }
        }          

      #endregion

        public void SaveChanges()
        {
            try
            {
                var addedEntities = new List<IEntity>();
                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    var e = entry.Entity;
                    var entityType = e.GetType().Name;
                    if (e is IEntity)
                    {
                        var entity = e as IEntity;
                        if (e is ILoggedEntity)
                        {
                            if (entry.State == EntityState.Added)
                            {
                                addedEntities.Add(entity);
                            }
                            else if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                            {
                                var al = new ActivityLog()
                                {
                                    EntityId = entity.Id,
                                    EntityType = entityType,
                                    Date = DateTime.Now,
                                    PartnerId = GlobalSettings.LoggedInPartnerId,
                                    UserId = GlobalSettings.LoggedInUserId,
                                    OperationType = entry.State.ToString(),
                                    ClientId = GlobalSettings.LoggedInClientId
                                };
                                //if (entry.State == EntityState.Modified)
                                //    cl.ChangeTrackInfo = GetChangedTrackingInfo(entry);
                                _context.ActivityLogs.Add(al);
                            }
                        }
                    }
                }

                _context.SaveChanges();

                String EntityName="";
                if (addedEntities.Any())
                {
                    foreach (var item in addedEntities)
                    {
                        var e = _context.Entry(item).Entity;
                        EntityName = e.GetType().Name;

                        _context.ActivityLogs.Add(new ActivityLog()
                        {
                            EntityId = item.Id,
                            EntityType = e.GetType().Name,
                            Date = DateTime.Now,
                            PartnerId = GlobalSettings.LoggedInPartnerId,
                            UserId = GlobalSettings.LoggedInUserId,
                            OperationType = EntityState.Added.ToString(),
                            ClientId = GlobalSettings.LoggedInClientId
                        });
                    }
                }
                if (EntityName != "CampaignLog" || EntityName != "CampaignLogXML" || EntityName != "Coupon")
                {
                    _context.SaveChanges();
                }

            }
            catch (DbEntityValidationException e)
            {
                throw new Exception("Validation Errors", e);
            }
            catch (DbException e)
            {
                throw new Exception("Validation Errors", e);
            }
            catch (Exception e)
            {
                throw new Exception("Could not save data", e);
            }
        }

        // This is a generic function to get a repo under current uow instance based on the entity type.
        public IGenericRepository<TEntity> GetRepo<TEntity>() where TEntity : class, IEntity
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (typeof(IGenericRepository<TEntity>).IsAssignableFrom(property.PropertyType))
                {
                    //var p = this.GetType().GetProperty(property.Name, BindingFlags.NonPublic);
                    var ret = property.GetValue(this);
                    return ret as IGenericRepository<TEntity>;
                }
            }
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

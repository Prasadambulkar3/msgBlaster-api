using MsgBlaster.Repo;
using MsgBlaster.Domain;
using MsgBlaster.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;


namespace MsgBlaster.Service
{
    public class CouponService
    {
        #region "CRUD Functionality"

        //Create Coupon
        public static int Create(CouponDTO CouponDTO)
        {

            try
            {
                var Coupon = new Coupon();

                UnitOfWork uow = new UnitOfWork();

                Coupon = Transform.CouponToDomain(CouponDTO);
                uow.CouponRepo.Insert(Coupon);
                uow.SaveChanges();
                CouponDTO.Id = Coupon.Id;
                return CouponDTO.Id;

            }

            catch (Exception)
            {
                throw;
            }
        }

        //Edit Coupon
        public static void Edit(CouponDTO CouponDTO)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Coupon Coupon = Transform.CouponToDomain(CouponDTO);
                uow.CouponRepo.Update(Coupon);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Redeem Coupon
        public static CouponDTO RedeemCoupon(CouponDTO CouponDTO)//int Id, 
        {
            try
            {
                CouponDTO CouponDTONew = new CouponDTO();
                List<CouponDTO> CouponDTOList = new List<CouponDTO>();
               
                 using (var uow = new UnitOfWork())
                 {
                    List<Coupon> CouponList = new List<Coupon>();
                    CouponList = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == CouponDTO.EcouponCampaignId && e.MobileNumber == CouponDTO.MobileNumber && e.Code == CouponDTO.Code).ToList();

                    if (CouponList != null)
                    {
                        foreach (var item in CouponList)
                        {
                            CouponDTOList.Add(Transform.CouponToDTO(item));
                        }
                    }
                 }

               // CouponDTOList = GetCouponListByClientId(CouponDTO.ClientId);
                //IEnumerable<CouponDTO> CouponDTOSearch = CouponDTOList.Where(e => e.MobileNumber == CouponDTO.MobileNumber && e.Code == CouponDTO.Code); // && e.IsRedeem == false
                 IEnumerable<CouponDTO> CouponDTOSearch = CouponDTOList;
            
                foreach (var item in CouponDTOSearch)
                {
                    CouponDTONew = item;
                }


                if (CouponDTONew.Id != 0 && CouponDTONew.IsExpired != true && CouponDTONew.IsRedeem != true)
                {

                    CouponDTONew.IsRedeem = true;
                    CouponDTONew.Remark = CouponDTO.Remark;
                    CouponDTONew.UserId = CouponDTO.UserId;
                    CouponDTONew.Amount = CouponDTO.Amount;
                    CouponDTONew.RedeemDateTime = System.DateTime.Now;
                    CouponDTONew.BillDate = CouponDTO.BillDate;
                    CouponDTONew.BillNumber = CouponDTO.BillNumber;

                    UnitOfWork uow = new UnitOfWork();
                    Coupon Coupon = Transform.CouponToDomain(CouponDTONew);
                    uow.CouponRepo.Update(Coupon);
                    uow.SaveChanges();

                    EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                    EcouponCampaignDTO = EcouponCampaignService.GetById(Coupon.EcouponCampaignId);
                    ClientDTO ClientDTO = new ClientDTO();
                    ClientDTO = ClientService.GetById(EcouponCampaignDTO.ClientId);

                    UpdateRedeemCount(Convert.ToInt32(Coupon.UserId), ClientDTO.Id, Coupon.EcouponCampaignId);

                }

                return CouponDTONew;
            }
            catch
            {
                throw;
            }
        }

        //Update Redeem Count
        public static void UpdateRedeemCount(int UserId, int ClientId, int EcouponCampaignId)
        {

            List<RedeemedCountDTO> RedeemedCountDTOList = new List<RedeemedCountDTO>();
            try
            {
                RedeemedCountDTOList = RedeemedCountService.GetByUserId(UserId, EcouponCampaignId);//.Where(e => e.EcouponCampaignId ==EcouponCampaignId);
                if (RedeemedCountDTOList.Count > 0)
                {
                    //Edit RedeemedCount                        
                    foreach (var item in RedeemedCountDTOList)
                    {
                        RedeemedCountDTO RedeemedCountDTO = new RedeemedCountDTO();
                        RedeemedCountDTO = item;
                        RedeemedCountDTO.RedeemCount = RedeemedCountDTO.RedeemCount + 1;
                        RedeemedCountService.Edit(RedeemedCountDTO);
                    }

                }
                else
                {
                    //Create RedeemedCount
                    int RedeemedCount = 0;
                    RedeemedCountDTO RedeemedCountDTO = new RedeemedCountDTO();
                    RedeemedCountDTO.ClientId = ClientId;
                    RedeemedCountDTO.UserId = UserId;
                    RedeemedCountDTO.EcouponCampaignId = EcouponCampaignId;
                    RedeemedCountDTO.RedeemCount = 1;
                    RedeemedCount = RedeemedCountService.Create(RedeemedCountDTO);

                }

            }
            catch
            {
                throw;
            }


        }

        //Get Coupon by Id
        public static CouponDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Coupon Coupon = uow.CouponRepo.GetById(Id);
                CouponDTO CouponDTO = Transform.CouponToDTO(Coupon);
                EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                EcouponCampaignDTO = EcouponCampaignService.GetById(CouponDTO.EcouponCampaignId);

                CouponDTO.CouponCampaignName = EcouponCampaignDTO.Title;
                CouponDTO.CreatedBy = UserService.GetById(EcouponCampaignDTO.CreatedBy).Name;
                CouponDTO.CreatedOn = EcouponCampaignDTO.CreatedDate;
                CouponDTO.ExpiresOn = EcouponCampaignDTO.ExpiresOn;

                return CouponDTO;
            }
            catch
            {
                throw;
            }
        }        

        #endregion



        #region "List Functionality"

        //Return Coupon list as per client 
        public static List<CouponDTO> GetCouponListByClientId(int ClientId)
        {

            List<CouponDTO> CouponDTOList = new List<CouponDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId);
                    //var EcouponCampaigncoupons = from c in EcouponCampaign where (from g in c.Coupons where c.ClientId == ClientId  select g).Any() select c;

                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == item.Id);
                            if (Coupon != null)
                            {
                                foreach (var itemCoupon in Coupon)
                                {
                                    CouponDTO CouponDTO = new CouponDTO();
                                    if (itemCoupon.EcouponCampaignId == item.Id)
                                    {
                                        UserDTO UserDTO = new UserDTO();
                                        CouponDTO = Transform.CouponToDTO(itemCoupon);
                                        if (CouponDTO.UserId != null)
                                        {
                                            int UserId = (int)CouponDTO.UserId;
                                            UserDTO = UserService.GetById(UserId);
                                            CouponDTO.UserName = UserDTO.Name;
                                        }
                                        else { CouponDTO.UserName = ""; }
                                        CouponDTOList.Add(CouponDTO);
                                        // CouponDTOList.Add(Transform.CouponToDTO(itemCoupon));
                                    }
                                }
                            }
                        }


                    }
                }

                return CouponDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Return Coupon redeemed list as per client
        public static List<CouponDTO> GetCouponListByClientIdWithRedeem(int ClientId)
        {

            List<CouponDTO> CouponDTOList = new List<CouponDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId);
                    //var EcouponCampaigncoupons = from c in EcouponCampaign where (from g in c.Coupons where c.ClientId == ClientId  select g).Any() select c;

                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == item.Id && e.IsRedeem == true);
                            if (Coupon != null)
                            {
                                foreach (var itemCoupon in Coupon)
                                {
                                    CouponDTO CouponDTO = new CouponDTO();
                                    if (itemCoupon.EcouponCampaignId == item.Id)
                                    {
                                        UserDTO UserDTO = new UserDTO();
                                        CouponDTO = Transform.CouponToDTO(itemCoupon);
                                        if (CouponDTO.UserId != null)
                                        {
                                            int UserId = (int)CouponDTO.UserId;
                                            UserDTO = UserService.GetById(UserId);
                                            CouponDTO.UserName = UserDTO.Name;
                                        }
                                        else { CouponDTO.UserName = ""; }
                                        CouponDTOList.Add(CouponDTO);
                                        // CouponDTOList.Add(Transform.CouponToDTO(itemCoupon));
                                    }
                                }
                            }
                        }


                    }
                }

                return CouponDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }


        //Return coupon list as per search criteria and client
        public static List<CouponDTO> GetCouponListSearchByClientId(int ClientId, string search, PagingInfo pagingInfo)
        {
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            try
            {
                using (var uow = new UnitOfWork())
                {


                    int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                    int take = pagingInfo.ItemsPerPage;

                    IQueryable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId).AsQueryable();//.OrderByDescending(e=>e.CreatedDate);
                    EcouponCampaign = PagingService.Sorting<EcouponCampaign>(EcouponCampaign, pagingInfo.SortBy, pagingInfo.Reverse);
                    EcouponCampaign = EcouponCampaign.Skip(skip).Take(take);

                    //var EcouponCampaigncoupons = from c in EcouponCampaign where (from g in c.Coupons where c.ClientId == ClientId  select g).Any() select c;

                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            IQueryable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == item.Id).AsQueryable();//.OrderByDescending(e => e.SentDateTime).Skip(skip).Take(take);
                            Coupon = PagingService.Sorting<Coupon>(Coupon, pagingInfo.SortBy, pagingInfo.Reverse);
                            Coupon = Coupon.Skip(skip).Take(take);

                            if (Coupon != null)
                            {
                                foreach (var itemCoupon in Coupon)
                                {
                                    CouponDTO CouponDTO = new CouponDTO();
                                    if (itemCoupon.EcouponCampaignId == item.Id)
                                    {
                                        UserDTO UserDTO = new UserDTO();
                                        CouponDTO = Transform.CouponToDTO(itemCoupon);

                                        EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                                        EcouponCampaignDTO = EcouponCampaignService.GetById(itemCoupon.EcouponCampaignId);
                                        CouponDTO.CouponCampaignName = EcouponCampaignDTO.Title;

                                        if (CouponDTO.UserId != null)
                                        {
                                            int UserId = (int)CouponDTO.UserId;
                                            UserDTO = UserService.GetById(UserId);
                                            CouponDTO.UserName = UserDTO.Name;
                                        }
                                        else
                                        {
                                            CouponDTO.UserName = "";
                                        }

                                        CouponDTOList.Add(CouponDTO);
                                        //CouponDTOList.Add(Transform.CouponToDTO(itemCoupon));
                                    }
                                }
                            }
                        }
                        if (search != "" & search != null)
                        {
                            bool Isdate = CommonService.IsDate(search);
                            if (Isdate != true)
                            {
                                List<CouponDTO> CouponDTOSearchList = new List<CouponDTO>();
                                IQueryable<Coupon> CouponSearch = uow.CouponRepo.GetAll().Where(e => e.Code.Contains(search) || (e.MobileNumber != null ? (e.MobileNumber.Contains(search)) : false) || (e.Remark != null ? (e.Remark.ToLower().Contains(search.ToLower())) : false) || (e.RedeemDateTime.ToString() != null ? (Convert.ToDateTime(e.RedeemDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable();//.OrderByDescending(e => e.SentDateTime); //|| (e.CouponCampaignName != null ? (e.CouponCampaignName.ToLower().Contains(search.ToLower())) : false) || e.UserName.ToLower().Contains(search.ToLower())
                                CouponSearch = PagingService.Sorting<Coupon>(CouponSearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                CouponSearch = CouponSearch.Skip(skip).Take(take);

                                if (CouponSearch != null)
                                {
                                    foreach (var itemsearch in CouponSearch)
                                    {

                                        CouponDTOSearchList.Add(Transform.CouponToDTO(itemsearch));
                                    }
                                }
                                return CouponDTOSearchList;

                            }
                            else
                            {
                                List<CouponDTO> CouponDTOSearchList = new List<CouponDTO>();
                                DateTime date = Convert.ToDateTime(search);
                                IQueryable<Coupon> CouponSearch = uow.CouponRepo.GetAll().Where(e => e.RedeemDateTime >= date && e.RedeemDateTime < date.AddDays(1)).AsQueryable();//.OrderByDescending(e => e.SentDateTime);
                                CouponSearch = PagingService.Sorting<Coupon>(CouponSearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                CouponSearch = CouponSearch.Skip(skip).Take(take);

                                if (CouponSearch != null)
                                {
                                    foreach (var itemsearch in CouponSearch)
                                    {

                                        CouponDTOSearchList.Add(Transform.CouponToDTO(itemsearch));
                                    }
                                }
                                return CouponDTOSearchList;

                            }


                        }

                    }


                }

                return CouponDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Return Coupon paged list as pe client 
        public static PageData<CouponDTO> GetCouponPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            PageData<CouponDTO> pageList = new PageData<CouponDTO>();

            if (pagingInfo == null)
            {
                PagingInfo PagingInfoCreated = new PagingInfo();
                PagingInfoCreated.Page = 1;
                PagingInfoCreated.Reverse = false;
                PagingInfoCreated.ItemsPerPage = 1;
                PagingInfoCreated.Search = "";
                PagingInfoCreated.TotalItem = 0;

                pagingInfo = PagingInfoCreated;
            }

            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "SentDateTime";
            }

            CouponDTOList = GetCouponListSearchByClientId(ClientId, pagingInfo.Search, pagingInfo);
            IQueryable<CouponDTO> CouponDTOPagedList = CouponDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;
            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.CouponRepo.GetAll().Where(e => e.Code.Contains(pagingInfo.Search) || (e.MobileNumber != null ? (e.MobileNumber.Contains(pagingInfo.Search)) : false) || (e.RedeemDateTime.ToString() != null ? (Convert.ToDateTime(e.RedeemDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.Remark != null ? (e.Remark.ToLower().Contains(pagingInfo.Search.ToLower())) : false) && e.EcouponCampaignId == pagingInfo.EcouponCampaignId).Count();
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.CouponRepo.GetAll().Where(e => e.RedeemDateTime >= date && e.RedeemDateTime < date.AddDays(1) && e.EcouponCampaignId == pagingInfo.EcouponCampaignId).Count();

                }

            }
            else
            {
                count = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == pagingInfo.EcouponCampaignId).Count();
            }

            ////Sorting
            //CouponDTOPagedList = PagingService.Sorting<CouponDTO>(CouponDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CouponDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CouponDTO>(CouponDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// CouponDTOPagedList.Count();

                List<CouponDTO> pagedCouponDTOList = new List<CouponDTO>();
                foreach (var item in CouponDTOPagedList)
                {
                    pagedCouponDTOList.Add(item);
                }
                pageList.Data = pagedCouponDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Return Coupon list as per ecoupon campaign id
        public static List<CouponDTO> GetCouponListByEcouponCampaignId(int ECouponCampaignId)
        {

            List<CouponDTO> CouponDTOList = new List<CouponDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == ECouponCampaignId);
                    if (Coupon != null)
                    {
                        foreach (var item in Coupon)
                        {
                            CouponDTOList.Add(Transform.CouponToDTO(item));
                        }
                    }
                }

                return CouponDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Return Coupon list as per ecoupon campaign id and mobile
        public static List<CouponDTO> GetCouponListByEcouponCampaignIdAndMobile(int ECouponCampaignId, string Mobile)
        {

            List<CouponDTO> CouponDTOList = new List<CouponDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == ECouponCampaignId && e.MobileNumber == Mobile);
                    if (Coupon != null)
                    {
                        foreach (var item in Coupon)
                        {
                            CouponDTOList.Add(Transform.CouponToDTO(item));
                        }
                    }
                }

                return CouponDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Return Coupon list as per code and mobile
        public static List<CouponDTO> GetCouponListByCodeAndMobile(string Code, string Mobile)
        {

            List<CouponDTO> CouponDTOList = new List<CouponDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.Code == Code && e.MobileNumber == Mobile && e.IsRedeem == false);
                    if (Coupon != null)
                    {
                        foreach (var item in Coupon)
                        {
                            CouponDTOList.Add(Transform.CouponToDTO(item));
                        }
                    }
                }

                return CouponDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        // Returns Full list with status IsExpired is false
        public static List<CouponDTO> GetCouponListWhichNotExpired()
        {

            List<CouponDTO> CouponDTOList = new List<CouponDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.IsExpired != true);
                    if (Coupon != null)
                    {
                        foreach (var item in Coupon)
                        {
                            CouponDTOList.Add(Transform.CouponToDTO(item));
                        }
                    }
                }

                return CouponDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Return Coupon list as per partner id
        public static List<CouponDTO> GetCouponListSearchByPartnerId(int PartnerId, string search)
        {
            List<EcouponCampaignDTO> EcouponCampaignDTOList = new List<EcouponCampaignDTO>();
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            try
            {
                EcouponCampaignDTOList = EcouponCampaignService.GetEcouponCampaignListSearchByPartnerId(PartnerId, "");

                if (EcouponCampaignDTOList.Count > 0)
                {
                    foreach (var item in EcouponCampaignDTOList)
                    {
                        var EcouponCampaign = CouponDTOList.Where(e => e.ClientId == item.ClientId).OrderByDescending(e => e.SentDateTime);// uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId);
                        if (EcouponCampaign != null)
                        {

                            foreach (var itemEcouponCampaign in EcouponCampaign)
                            {
                                CouponDTO CouponDTONew = new CouponDTO();
                                CouponDTONew = itemEcouponCampaign;
                                UserDTO UserDTO = new UserDTO();

                                EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                                EcouponCampaignDTO = EcouponCampaignService.GetById(itemEcouponCampaign.EcouponCampaignId);
                                CouponDTONew.CouponCampaignName = EcouponCampaignDTO.Title;

                                if (itemEcouponCampaign.UserId != null)
                                {
                                    int UserId = (int)itemEcouponCampaign.UserId;
                                    UserDTO = UserService.GetById(UserId);
                                    CouponDTONew.UserName = UserDTO.Name;
                                }
                                else
                                {
                                    CouponDTONew.UserName = "";
                                }

                                CouponDTOList.Add(CouponDTONew);
                            }
                        }
                    }
                }

                if (search != "" && search != null)
                {
                    bool Isdate = CommonService.IsDate(search);
                    if (Isdate != true)
                    {
                        List<CouponDTO> CouponDTOSearchList = new List<CouponDTO>();
                        var CouponSearch = CouponDTOList.Where(e => e.Code.Contains(search) || (e.MobileNumber != null ? (e.MobileNumber.Contains(search)) : false) || (e.Remark != null ? (e.Remark.ToLower().Contains(search.ToLower())) : false) || e.UserName.ToLower().Contains(search.ToLower()) || (e.RedeemDateTime.ToString() != null ? (Convert.ToDateTime(e.RedeemDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(search.ToLower())) : false) || (e.CouponCampaignName != null ? (e.CouponCampaignName.ToLower().Contains(search.ToLower())) : false)).OrderByDescending(e => e.SentDateTime);
                        if (CouponSearch != null)
                        {
                            foreach (var itemsearch in CouponSearch)
                            {

                                CouponDTOSearchList.Add(itemsearch);
                            }
                        }
                        return CouponDTOSearchList;

                    }
                    else
                    {
                        List<CouponDTO> CouponDTOSearchList = new List<CouponDTO>();
                        DateTime date = Convert.ToDateTime(search);
                        var CouponSearch = CouponDTOList.Where(e => e.RedeemDateTime >= date && e.RedeemDateTime < date.AddDays(1)).OrderByDescending(e => e.SentDateTime);
                        if (CouponSearch != null)
                        {
                            foreach (var itemsearch in CouponSearch)
                            {

                                CouponDTOSearchList.Add(itemsearch);
                            }
                        }
                        return CouponDTOSearchList;
                    }
                }
                return CouponDTOList;
            }

          //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Return Coupon paged list as per partner id
        public static PageData<CouponDTO> GetCouponPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        {
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            PageData<CouponDTO> pageList = new PageData<CouponDTO>();

            if (pagingInfo == null)
            {
                PagingInfo PagingInfoCreated = new PagingInfo();
                PagingInfoCreated.Page = 1;
                PagingInfoCreated.Reverse = false;
                PagingInfoCreated.ItemsPerPage = 1;
                PagingInfoCreated.Search = "";
                PagingInfoCreated.TotalItem = 0;

                pagingInfo = PagingInfoCreated;
            }

            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "SentDateTime";
            }


            CouponDTOList = GetCouponListSearchByPartnerId(PartnerId, pagingInfo.Search);
            IQueryable<CouponDTO> CouponDTOPagedList = CouponDTOList.AsQueryable();

            ////Sorting
            //CouponDTOPagedList = PagingService.Sorting<CouponDTO>(CouponDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CouponDTOPagedList.Count() > 0)
            {
                var ContacDTOPerPage = PagingService.Paging<CouponDTO>(CouponDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = CouponDTOPagedList.Count();

                List<CouponDTO> pagedCouponDTOList = new List<CouponDTO>();
                foreach (var item in ContacDTOPerPage)
                {
                    pagedCouponDTOList.Add(item);
                }
                pageList.Data = pagedCouponDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Return Coupon list as per user id
        public static List<CouponDTO> GetCouponListByUserId(int UserId, PagingInfo pagingInfo)
        {

            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            try
            {
                UserDTO UserDTO = new UserDTO();
                UserDTO = UserService.GetById(UserId);

                using (var uow = new UnitOfWork())
                {
                    int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                    int take = pagingInfo.ItemsPerPage;

                    IQueryable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.UserId == UserId && e.IsExpired != true && e.IsRedeem == true).AsQueryable(); //.OrderByDescending(e => e.SentDateTime);
                    Coupon = PagingService.Sorting<Coupon>(Coupon, pagingInfo.SortBy, pagingInfo.Reverse);
                    Coupon = Coupon.Skip(skip).Take(take);

                    if (Coupon != null)
                    {
                        foreach (var item in Coupon)
                        {

                            CouponDTO CouponDTO = new CouponDTO();
                            CouponDTO = Transform.CouponToDTO(item);
                            CouponDTO.UserName = UserDTO.Name;

                            EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                            EcouponCampaignDTO = EcouponCampaignService.GetById(CouponDTO.EcouponCampaignId);
                            CouponDTO.CouponCampaignName = EcouponCampaignDTO.Title;
                            CouponDTO.ExpiresOn = EcouponCampaignDTO.ExpiresOn;
                            CouponDTO.MinPurchaseAmount = EcouponCampaignDTO.MinPurchaseAmount;

                            CouponDTOList.Add(CouponDTO);// (Transform.CouponToDTO(item));
                        }
                    }

                    if (pagingInfo.Search != "" && pagingInfo.Search != null)
                    {

                        bool Isdate = CommonService.IsDate(pagingInfo.Search);
                        if (Isdate != true)
                        {

                            IQueryable<CouponDTO> CouponList = CouponDTOList.Where(e => e.CouponCampaignName.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.MobileNumber != null ? (e.MobileNumber.Contains(pagingInfo.Search)) : false) || (e.Amount != null ? (e.Amount.ToString() == pagingInfo.Search.ToString()) : false) || (e.Code != null ? (e.Code.Contains(pagingInfo.Search)) : false) || (e.Message != null ? (e.Message.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.RedeemDateTime.ToString() != null ? (Convert.ToDateTime(e.RedeemDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.Remark != null ? (e.Remark.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BillNumber != null ? (e.BillNumber.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BillDate.ToString() != null ? (Convert.ToDateTime(e.BillDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable();//.OrderByDescending(e => e.SentDateTime).ToList();
                            CouponList = PagingService.Sorting<CouponDTO>(CouponList, pagingInfo.SortBy, pagingInfo.Reverse);
                            CouponList = CouponList.Skip(skip).Take(take);

                            List<CouponDTO> CouponDTOListNew = new List<CouponDTO>();
                            if (CouponDTOList.Count > 0)
                            {
                                foreach (var item in CouponList)
                                {
                                    EcouponCampaignDTO EcouponCampaignDTOSearch = new EcouponCampaignDTO();
                                    EcouponCampaignDTOSearch = EcouponCampaignService.GetById(item.EcouponCampaignId);
                                    item.CouponCampaignName = EcouponCampaignDTOSearch.Title;
                                    item.ExpiresOn = EcouponCampaignDTOSearch.ExpiresOn;
                                    item.MinPurchaseAmount = EcouponCampaignDTOSearch.MinPurchaseAmount;

                                    CouponDTOListNew.Add(item);
                                }
                                return CouponDTOListNew;
                            }

                        }
                        else
                        {
                            List<CouponDTO> CouponDTOListNew = new List<CouponDTO>();
                            DateTime date = Convert.ToDateTime(pagingInfo.Search);
                            IQueryable<CouponDTO> CouponList = CouponDTOList.Where(e => e.RedeemDateTime >= date && e.RedeemDateTime < date.AddDays(1) && e.BillDate >= date && e.BillDate < date.AddDays(1)).AsQueryable();//.OrderByDescending(e => e.SentDateTime).ToList();
                            CouponList = PagingService.Sorting<CouponDTO>(CouponList, pagingInfo.SortBy, pagingInfo.Reverse);
                            CouponList = CouponList.Skip(skip).Take(take);
                            if (CouponDTOList.Count > 0)
                            {
                                foreach (var item in CouponList)
                                {
                                    EcouponCampaignDTO EcouponCampaignDTOSearch = new EcouponCampaignDTO();
                                    EcouponCampaignDTOSearch = EcouponCampaignService.GetById(item.EcouponCampaignId);
                                    item.CouponCampaignName = EcouponCampaignDTOSearch.Title;
                                    item.ExpiresOn = EcouponCampaignDTOSearch.ExpiresOn;
                                    item.MinPurchaseAmount = EcouponCampaignDTOSearch.MinPurchaseAmount;

                                    CouponDTOListNew.Add(item);
                                }
                                return CouponDTOListNew;
                            }
                        }

                    }

                }


                return CouponDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Return Coupon paged list as per partner id
        public static PageData<CouponDTO> GetCouponPagedListbyUserId(PagingInfo pagingInfo, int UserId)
        {
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            PageData<CouponDTO> pageList = new PageData<CouponDTO>();

            if (pagingInfo == null)
            {
                PagingInfo PagingInfoCreated = new PagingInfo();
                PagingInfoCreated.Page = 1;
                PagingInfoCreated.Reverse = false;
                PagingInfoCreated.ItemsPerPage = 1;
                PagingInfoCreated.Search = "";
                PagingInfoCreated.TotalItem = 0;

                pagingInfo = PagingInfoCreated;
            }
            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "SentDateTime";
            }

            CouponDTOList = GetCouponListByUserId(UserId, pagingInfo);
            IQueryable<CouponDTO> CouponDTOPagedList = CouponDTOList.AsQueryable();

            ////Sorting
            //CouponDTOPagedList = PagingService.Sorting<CouponDTO>(CouponDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CouponDTOPagedList.Count() > 0)
            {
                var ContacDTOPerPage = PagingService.Paging<CouponDTO>(CouponDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = CouponDTOPagedList.Count();

                List<CouponDTO> pagedCouponDTOList = new List<CouponDTO>();
                foreach (var item in ContacDTOPerPage)
                {
                    pagedCouponDTOList.Add(item);
                }
                pageList.Data = pagedCouponDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Return Coupon list as per Ecoupon campaign id and Is sent status
        public static List<CouponDTO> GetCouponListSearchByEcouponCampaignId(int EcouponCampaignId, string search, bool IsSent, PagingInfo pagingInfo)
        {
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            try
            {
                using (var uow = new UnitOfWork())
                {
                    IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.Id == EcouponCampaignId && e.IsSent == IsSent).OrderByDescending(e => e.CreatedDate);
                    //var EcouponCampaigncoupons = from c in EcouponCampaign where (from g in c.Coupons where c.ClientId == ClientId  select g).Any() select c;
                    int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                    int take = pagingInfo.ItemsPerPage;

                    if (EcouponCampaign != null)
                    {
                        foreach (var item in EcouponCampaign)
                        {
                            IQueryable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == item.Id).AsQueryable();//.OrderByDescending(e => e.SentDateTime).Skip(skip).Take(take);
                            Coupon = PagingService.Sorting<Coupon>(Coupon, pagingInfo.SortBy, pagingInfo.Reverse);
                            Coupon = Coupon.Skip(skip).Take(take);

                            if (Coupon != null)
                            {
                                foreach (var itemCoupon in Coupon)
                                {
                                    CouponDTO CouponDTO = new CouponDTO();
                                    if (itemCoupon.EcouponCampaignId == item.Id)
                                    {
                                        UserDTO UserDTO = new UserDTO();
                                        CouponDTO = Transform.CouponToDTO(itemCoupon);

                                        EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                                        EcouponCampaignDTO = EcouponCampaignService.GetById(itemCoupon.EcouponCampaignId);
                                        CouponDTO.CouponCampaignName = EcouponCampaignDTO.Title;
                                        CouponDTO.MinPurchaseAmount = EcouponCampaignDTO.MinPurchaseAmount;

                                        if (CouponDTO.UserId != null)
                                        {
                                            int UserId = (int)CouponDTO.UserId;
                                            UserDTO = UserService.GetById(UserId);
                                            CouponDTO.UserName = UserDTO.Name;
                                        }
                                        else
                                        {
                                            CouponDTO.UserName = "";
                                        }

                                        CouponDTOList.Add(CouponDTO);
                                        //CouponDTOList.Add(Transform.CouponToDTO(itemCoupon));
                                    }
                                }
                            }
                        }
                        if (search != "" & search != null)
                        {
                            EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                            EcouponCampaignDTO = EcouponCampaignService.GetById(EcouponCampaignId);
                            //int UserId = 0;
                            //UserId = UserService.GetUserByName(search,EcouponCampaignDTO.ClientId);
                            int CampaignId = 0;

                            string UserIdString = UserService.GetUserIdarrayByName(search, EcouponCampaignDTO.ClientId);

                            CampaignId = EcouponCampaignService.GetEcouponCampaignByName(search, EcouponCampaignDTO.ClientId);

                            bool Isdate = CommonService.IsDate(search);
                            if (Isdate != true)
                            {
                                List<CouponDTO> CouponDTOSearchList = new List<CouponDTO>();
                                IQueryable<Coupon> CouponSearch = uow.CouponRepo.GetAll().Where(e => (e.Code.Contains(search) || (e.MobileNumber != null ? (e.MobileNumber.Contains(search)) : false) || (e.Remark != null ? (e.Remark.ToLower().Contains(search.ToLower())) : false) || (e.RedeemDateTime.ToString() != null ? (Convert.ToDateTime(e.RedeemDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (UserIdString != null ? (e.UserId.ToString().Split(',').Any(UserId => UserIdString.Contains(UserId))) : false) || (e.EcouponCampaignId != 0 ? (e.EcouponCampaignId == CampaignId) : false) || (e.BillNumber != null ? (e.BillNumber.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BillDate.ToString() != null ? (Convert.ToDateTime(e.BillDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)) && e.EcouponCampaignId == EcouponCampaignId).AsQueryable();//.OrderByDescending(e => e.SentDateTime).Skip(skip).Take(take); //|| e.UserName.ToLower().Contains(search.ToLower()) || (e.CouponCampaignName != null ? (e.CouponCampaignName.ToLower().Contains(search.ToLower())) : false)

                                CouponSearch = PagingService.Sorting<Coupon>(CouponSearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                CouponSearch = CouponSearch.Skip(skip).Take(take);

                                if (CouponSearch != null)
                                {
                                    foreach (var itemsearch in CouponSearch)
                                    {
                                        if (itemsearch.EcouponCampaignId == EcouponCampaignId)
                                        {
                                            CouponDTO CouponDTO = new CouponDTO();
                                            CouponDTO = Transform.CouponToDTO(itemsearch);

                                            EcouponCampaignDTO EcouponCampaignDTOSearch = new EcouponCampaignDTO();
                                            EcouponCampaignDTOSearch = EcouponCampaignService.GetById(itemsearch.EcouponCampaignId);
                                            CouponDTO.CouponCampaignName = EcouponCampaignDTOSearch.Title;
                                            CouponDTO.MinPurchaseAmount = EcouponCampaignDTOSearch.MinPurchaseAmount;

                                            UserDTO UserDTO = new UserDTO();
                                            if (itemsearch.UserId == null)
                                            { itemsearch.UserId = 0; }
                                            int userId = (int)itemsearch.UserId;
                                            if (userId != 0)
                                            {
                                                UserDTO = UserService.GetById(userId);
                                                CouponDTO.UserName = UserDTO.Name;
                                            }
                                            CouponDTOSearchList.Add(CouponDTO);
                                        }
                                    }
                                }
                                return CouponDTOSearchList;

                            }
                            else
                            {
                                List<CouponDTO> CouponDTOSearchList = new List<CouponDTO>();
                                DateTime date = Convert.ToDateTime(search);
                                IQueryable<Coupon> CouponSearch = uow.CouponRepo.GetAll().Where(e => (e.RedeemDateTime >= date && e.RedeemDateTime < date.AddDays(1) && e.BillDate >= date && e.BillDate < date.AddDays(1)) && e.EcouponCampaignId == EcouponCampaignId).AsQueryable();//.OrderByDescending(e => e.SentDateTime).Skip(skip).Take(take);
                                CouponSearch = PagingService.Sorting<Coupon>(CouponSearch, pagingInfo.SortBy, pagingInfo.Reverse);
                                CouponSearch = CouponSearch.Skip(skip).Take(take);

                                if (CouponSearch != null)
                                {
                                    foreach (var itemsearch in CouponSearch)
                                    {
                                        if (itemsearch.EcouponCampaignId == EcouponCampaignId)
                                        {
                                            CouponDTO CouponDTO = new CouponDTO();
                                            CouponDTO = Transform.CouponToDTO(itemsearch);
                                            UserDTO UserDTO = new UserDTO();

                                            EcouponCampaignDTO EcouponCampaignDTOSearch = new EcouponCampaignDTO();
                                            EcouponCampaignDTOSearch = EcouponCampaignService.GetById(itemsearch.EcouponCampaignId);
                                            CouponDTO.CouponCampaignName = EcouponCampaignDTOSearch.Title;
                                            CouponDTO.MinPurchaseAmount = EcouponCampaignDTOSearch.MinPurchaseAmount;


                                            if (itemsearch.UserId == null)
                                            { itemsearch.UserId = 0; }

                                            int userId = (int)itemsearch.UserId;
                                            if (userId != 0)
                                            {
                                                UserDTO = UserService.GetById(userId);
                                                CouponDTO.UserName = UserDTO.Name;
                                            }
                                            CouponDTOSearchList.Add(CouponDTO);
                                        }

                                        //CouponDTOSearchList.Add(Transform.CouponToDTO(itemsearch));
                                    }
                                }
                                return CouponDTOSearchList;

                            }


                        }

                    }


                }

                return CouponDTOList;
            }
            //catch (LoggedInUserException)
            //{
            //    throw new System.TimeoutException();
            //}
            catch (Exception)
            {

                throw;
            }
        }

        //Return Coupon paged list as per Ecoupon campaign id and Is sent status
        public static PageData<CouponDTO> GetCouponPagedListbyEcouponCampaignId(PagingInfo pagingInfo, int EcouponCampaignId, bool IsSent)
        {
            List<CouponDTO> CouponDTOList = new List<CouponDTO>();
            PageData<CouponDTO> pageList = new PageData<CouponDTO>();

            if (pagingInfo == null)
            {
                PagingInfo PagingInfoCreated = new PagingInfo();
                PagingInfoCreated.Page = 1;
                PagingInfoCreated.Reverse = false;
                PagingInfoCreated.ItemsPerPage = 1;
                PagingInfoCreated.Search = "";
                PagingInfoCreated.TotalItem = 0;

                pagingInfo = PagingInfoCreated;
            }
            if (pagingInfo.SortBy == "")
            {
                pagingInfo.SortBy = "SentDateTime";
            }

            CouponDTOList = GetCouponListSearchByEcouponCampaignId(EcouponCampaignId, pagingInfo.Search, IsSent, pagingInfo);
            IQueryable<CouponDTO> CouponDTOPagedList = CouponDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;
            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                    EcouponCampaignDTO = EcouponCampaignService.GetById(EcouponCampaignId);
                    //int UserId = 0;
                    //UserId = UserService.GetUserByName(pagingInfo.Search, EcouponCampaignDTO.ClientId);
                    string UserIdString = UserService.GetUserIdarrayByName(pagingInfo.Search, EcouponCampaignDTO.ClientId);
                    int CampaignId = 0;
                    CampaignId = EcouponCampaignService.GetEcouponCampaignByName(pagingInfo.Search, EcouponCampaignDTO.ClientId);

                    count = uow.CouponRepo.GetAll().Where(e => (e.Code.Contains(pagingInfo.Search) || (e.MobileNumber != null ? (e.MobileNumber.Contains(pagingInfo.Search)) : false) || (e.Remark != null ? (e.Remark.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.RedeemDateTime.ToString() != null ? (Convert.ToDateTime(e.RedeemDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (UserIdString != null ? (e.UserId.ToString().Split(',').Any(UserId => UserIdString.Contains(UserId))) : false) || (e.EcouponCampaignId != 0 ? (e.EcouponCampaignId == CampaignId) : false) || (e.BillNumber != null ? (e.BillNumber.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || (e.BillDate.ToString() != null ? (Convert.ToDateTime(e.BillDate).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)) && e.EcouponCampaignId == EcouponCampaignId).Count();
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = uow.CouponRepo.GetAll().Where(e => (e.RedeemDateTime >= date && e.RedeemDateTime < date.AddDays(1) && e.BillDate >= date && e.BillDate < date.AddDays(1)) && e.EcouponCampaignId == EcouponCampaignId).Count();

                }

            }
            else
            {
                count = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == EcouponCampaignId).Count();
            }

            ////Sorting
            //CouponDTOPagedList = PagingService.Sorting<CouponDTO>(CouponDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CouponDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CouponDTO>(CouponDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count; // CouponDTOPagedList.Count();

                List<CouponDTO> pagedCouponDTOList = new List<CouponDTO>();
                foreach (var item in CouponDTOPagedList)
                {
                    EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                    EcouponCampaignDTO = EcouponCampaignService.GetById(item.EcouponCampaignId);

                    item.CouponCampaignName = EcouponCampaignDTO.Title;
                    item.CreatedBy = UserService.GetById(EcouponCampaignDTO.CreatedBy).Name;
                    item.CreatedOn = EcouponCampaignDTO.CreatedDate;
                    item.ExpiresOn = EcouponCampaignDTO.ExpiresOn;

                    pagedCouponDTOList.Add(item);
                }
                pageList.Data = pagedCouponDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Return Coupon list as per mobile and Client id
        public static List<CouponDTO> GetMobileNumberAndClientIdWiseCouponList(String Mobile, int ClientId)
        {
            List<CouponDTO> CouponDTODTOList = new List<CouponDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => (e.MobileNumber.Equals(Mobile) && e.IsExpired != true && e.IsRedeem != true));
                if (Coupon != null)
                {
                    foreach (var item in Coupon)
                    {
                        CouponDTO CouponDTO = new CouponDTO();
                        CouponDTO = Transform.CouponToDTO(item);
                        EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                        EcouponCampaignDTO = EcouponCampaignService.GetById(CouponDTO.EcouponCampaignId);
                        CouponDTO.CouponCampaignName = EcouponCampaignDTO.Title;
                        CouponDTO.Message = EcouponCampaignDTO.Message;
                        if (EcouponCampaignDTO.ClientId == ClientId)
                        {
                            CouponDTODTOList.Add(CouponDTO);
                        }
                    }
                }

                return CouponDTODTOList;
            }
            catch
            {
                throw;
            }

        }


        #endregion


        #region "Other Functionality"
        //Get coupon as per Mobile and code 
        public static CouponDTO GetCouponDetailsFromMobileAndCode(string Mobile, string Code)
        {
            try
            {
                CouponDTO CouponDTO = new CouponDTO();
                EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.MobileNumber == Mobile && e.Code == Code);
                    if (Coupon != null)
                    {
                        foreach (var item in Coupon)
                        {
                            CouponDTO = GetById(item.Id);

                        }

                        if (CouponDTO.Id != 0)
                        {
                            EcouponCampaignDTO = EcouponCampaignService.GetById(CouponDTO.EcouponCampaignId);
                            CouponDTO.CouponCampaignName = EcouponCampaignDTO.Title;
                            CouponDTO.ExpiresOn = EcouponCampaignDTO.ExpiresOn;
                            CouponDTO.MinPurchaseAmount = EcouponCampaignDTO.MinPurchaseAmount;
                        }
                    }

                }
                return CouponDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get todays coupon message sent count by client id
        public static int GetTodaysCouponMsssageSentCount(int ClientId)
        {
            try
            {
                int TodaysSMSCount = 0;
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<EcouponCampaign> EcouponCampaign = uow.EcouponCampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsSent == true);
                foreach (var item in EcouponCampaign)
                {
                    IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => e.EcouponCampaignId == item.Id && e.SentDateTime > System.DateTime.Now.Date && e.SentDateTime < System.DateTime.Now.Date.AddDays(1));

                    if (Coupon != null)
                    {
                        foreach (var itemCampaignLog in Coupon)
                        {
                            TodaysSMSCount = TodaysSMSCount + 1;
                        }
                    }
                }

                return TodaysSMSCount;

            }
            catch
            {
                throw;
            }

        }

        //Resend coupon by coupon id and client id
        public static CouponDTO ResendCoupon(int CouponId, int ClientId)
        {
            try
            {
                CouponDTO CouponDTO = new CouponDTO();
                CouponDTO = GetById(CouponId);

                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO = ClientService.GetById(ClientId);

                SettingDTO SettingDTO = new SettingDTO();
                SettingDTO = SettingService.GetById(1);
                double RequiredCredit = CommonService.GetMessageCount(CouponDTO.Message);
                double ActualRequiredCredits = RequiredCredit * SettingDTO.NationalCouponSMSCount;

                if (ClientDTO.SMSCredit >= ActualRequiredCredits) //RequiredCredit
                {

                    // Send Direct SMS 
                    //bool IsSent = CommonService.ResendCoupon(CouponDTO.MobileNumber, CouponDTO.Message, ClientDTO.Id);

                    CouponDTO CouponDTONew = new CouponDTO();
                    CouponDTONew = CommonService.ResendCouponByCouponDTOAndClientId(CouponDTO, ClientDTO.Id);
                    return CouponDTONew;

                    //if (IsSent == true)
                    //{
                        //CouponDTO.SentDateTime = System.DateTime.Now;
                        //CouponDTO.Id = 0;
                        //int NewCouponId = Create(CouponDTO);
                        //CouponDTO CouponDTONew = new CouponDTO();
                        //CouponDTONew = GetById(NewCouponId);

                        //////Expire previous coupon
                        //CouponDTO CouponDTOPrevious = new CouponDTO();
                        //CouponDTOPrevious = GetById(CouponId);
                        //CouponDTOPrevious.IsExpired = true;
                        //Edit(CouponDTOPrevious);


                        ////// Modify EcouponCampaign message count 
                        //EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                        //EcouponCampaignDTO = EcouponCampaignService.GetById(CouponDTONew.EcouponCampaignId);
                        //EcouponCampaignDTO.ReceipentNumber = EcouponCampaignDTO.ReceipentNumber + "," + CouponDTO.MobileNumber;
                        //EcouponCampaignDTO.RecipientsCount = EcouponCampaignDTO.RecipientsCount + 1;
                        //if (EcouponCampaignDTO.GroupId == 0)
                        //{ 
                        //    EcouponCampaignDTO.GroupId = null;
                        //    EcouponCampaignDTO.Group = null;
                        //}

                        //EcouponCampaignDTO.RequiredCredits = GetECouponCampaignRequiredCreditsByEcouponCampaignId(EcouponCampaignDTO.Id);

                        //EcouponCampaignService.EditForEcouponResend(EcouponCampaignDTO);

                        //////Modify client SMS credits
                        //ClientDTO.SMSCredit = ClientDTO.SMSCredit - ActualRequiredCredits;// RequiredCredit;
                        //ClientService.Edit(ClientDTO);

                        //return CouponDTONew;
                    //}
                    //else
                    //{
                    //    CouponDTO = null;
                    //    return CouponDTO;
                    //}
                }
                CouponDTO = null;
                return CouponDTO;
            }
            catch
            {
                throw;
            }

        }

        //Expire all coupons by campaign id
        public static bool ExpireAllCouponsByCampaignId(int EcouponCampaignId)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => (e.EcouponCampaignId == EcouponCampaignId && e.IsRedeem != true));
                if (Coupon != null)
                {
                    foreach (var item in Coupon)
                    {
                        CouponDTO CouponDTO = new CouponDTO();
                        CouponDTO = GetById(item.Id);
                        CouponDTO.IsExpired = true;
                        Edit(CouponDTO);
                    }
                    return true;
                }
                else return false;

            }
            catch
            {
                throw;
            }
        }

        //Get coupon count by ecoupon campaign id
        public static int GetCouponCountByEcouponCampaignId(int EcouponCampaignId)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => (e.EcouponCampaignId == EcouponCampaignId && e.IsCouponSent == true));
                return Coupon.Count();
            }
            catch
            {
                throw;
            }
        }

        //Get Sum of ECoupon campaign required credits from Coupon by using EcouponCampaignId
        public static double GetECouponCampaignRequiredCreditsByEcouponCampaignId(int EcouponCampaignId)
        {
            try
            {
                double RequiredCredits = 0;
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Coupon> Coupon = uow.CouponRepo.GetAll().Where(e => (e.EcouponCampaignId == EcouponCampaignId && e.IsCouponSent == true));

                if (Coupon != null)
                {
                    foreach (var item in Coupon)
                    {
                        RequiredCredits = RequiredCredits + item.RequiredCredits;
                    }
                    return RequiredCredits;
                }

                return RequiredCredits;
            }
            catch
            {
                throw;
            }
        }

        #endregion
     



    }
}

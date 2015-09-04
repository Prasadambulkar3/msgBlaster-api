using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;

namespace MsgBlaster.Service
{
    public class PlanService
    {
        #region "CRUD Functionality"

        //Create plan
        public static int Create(PlanDTO PlanDTO)
        {
            try
            {
                var Plan = new Plan();
                using (var uow = new UnitOfWork())
                {
                    Plan = Transform.PlanToDomain(PlanDTO);
                    uow.PlanRepo.Insert(Plan);
                    uow.SaveChanges();
                    return (Plan.Id);

                }

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

        //Edit plan
        public static void Edit(PlanDTO planDTO)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Plan Plan = Transform.PlanToDomain(planDTO);
                uow.PlanRepo.Update(Plan);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Delete plan by id
        public static void Delete(int id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                uow.PlanRepo.Delete(id);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get plan details by id
        public static PlanDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Plan Plan = uow.PlanRepo.GetById(Id);
                PlanDTO PlanDTO = Transform.PlanToDTO(Plan);
                return PlanDTO;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "List Functionality"

        //get all plan list
        public static List<PlanDTO> GetPlanList()
        {

            List<PlanDTO> PlanDTOList = new List<PlanDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Plan> Plan = uow.PlanRepo.GetAll().OrderBy(e => e.Max);
                    if (Plan != null)
                    {
                        foreach (var item in Plan)
                        {
                            PlanDTOList.Add(Transform.PlanToDTO(item));
                        }
                    }
                }

                return PlanDTOList;
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

        //get plan list with search criteria
        public static List<PlanDTO> GetPlanList(PagingInfo pagingInfo)
        {

            List<PlanDTO> PlanDTO = new List<PlanDTO>();

            //List<PartnerDTO> PartnerDTO = new List<PartnerDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                IQueryable<Plan> Plan = uow.PlanRepo.GetAll().AsQueryable();// .Where(e => e.PartnerId == PartnerId && e.IsActive == IsActive).ToList().AsQueryable();//.OrderBy(e => e.Company);
                Plan = PagingService.Sorting<Plan>(Plan, pagingInfo.SortBy, pagingInfo.Reverse);
                Plan = Plan.Skip(skip).Take(take);

                List<PlanDTO> PlanDTOList = new List<PlanDTO>();

                if (Plan != null)
                {
                    foreach (var item in Plan)
                    {
                        PlanDTOList.Add(Transform.PlanToDTO(item));

                    }
                }

                if (PlanDTOList != null)
                {
                    if (pagingInfo.Search != "" && pagingInfo.Search != null)
                    {

                        bool IsDate = CommonService.IsDate(pagingInfo.Search);
                        if (IsDate != true)
                        {

                            IQueryable<Plan> Plansearch = uow.PlanRepo.GetAll().Where(e => (e.Title != null ? (e.Title.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.Price.ToString().ToLower().Contains(pagingInfo.Search.ToLower()) || (e.Min.ToString() != null ? (e.Min.ToString().Contains(pagingInfo.Search.ToString())) : false) || e.Max.ToString().Contains(pagingInfo.Search) || (e.Tax.ToString() != null ? (e.Tax.ToString().ToLower().Contains(pagingInfo.Search.ToLower())) : false)).AsQueryable(); //|| e.IsEcoupon.ToString().Contains(pagingInfo.Search) //.OrderBy(e => e.Company); 
                            Plansearch = PagingService.Sorting<Plan>(Plansearch, pagingInfo.SortBy, pagingInfo.Reverse);
                            Plansearch = Plansearch.Skip(skip).Take(take);

                            if (Plansearch != null)
                            {
                                foreach (var item in Plansearch)
                                {

                                    PlanDTO.Add(Transform.PlanToDTO(item));
                                }
                            }
                            return PlanDTO.Skip(skip).Take(take).ToList();
                        }
                        else
                        {
                            ////date wise search
                            //DateTime date = Convert.ToDateTime(pagingInfo.Search);                         

                        }

                    }
                    else
                    {


                        foreach (var item in PlanDTOList)
                        {
                            PlanDTO.Add(item);
                        }
                        return PlanDTO.Skip(skip).Take(take).ToList();
                    }
                }

                return PlanDTOList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get plan paged list
        public static PageData<PlanDTO> GetPlanPagedList(PagingInfo pagingInfo)
        {
            List<PlanDTO> PlanDTOList = new List<PlanDTO>();
            PageData<PlanDTO> pageList = new PageData<PlanDTO>();

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
                pagingInfo.SortBy = "Title";
            }

            PlanDTOList = GetPlanList(pagingInfo);

            IQueryable<PlanDTO> PlanDTOPagedList = PlanDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.PlanRepo.GetAll().Where(e => (e.Title != null ? (e.Title.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.Price.ToString().ToLower().Contains(pagingInfo.Search.ToLower()) || (e.Min.ToString() != null ? (e.Min.ToString().Contains(pagingInfo.Search.ToString())) : false) || e.Max.ToString().Contains(pagingInfo.Search) || (e.Tax.ToString() != null ? (e.Tax.ToString().ToLower().Contains(pagingInfo.Search.ToLower())) : false)).Count();//|| e.IsEcoupon.ToString().Contains(pagingInfo.Search)  //.OrderBy(e => e.Company);
                }
                else
                {
                    //DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    //count = 0;
                    //count = uow.ClientRepo.GetAll().Where(e => e.RegisteredDate >= date && e.RegisteredDate < date.AddDays(1) && e.IsActive == IsActive && e.PartnerId == PartnerId).Count();

                }

            }
            else
            {
                count = uow.PlanRepo.GetAll().Count();
            }

            ////Sorting
            PlanDTOPagedList = PagingService.Sorting<PlanDTO>(PlanDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (PlanDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<ClientDTO>(ClientDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// ClientDTOPagedList.Count();


                List<PlanDTO> pagedPlanDTOList = new List<PlanDTO>();
                foreach (var item in PlanDTOPagedList)
                {
                    pagedPlanDTOList.Add(item);
                }
                pageList.Data = pagedPlanDTOList;
            }
            else
            {
                pageList.Data = null;
            }


            return pageList;

        }
 
        #endregion

        #region "Other Functionality"
       
        //Check plan name is present by title and id
        public static bool GetPlanByTitle(string Title, int Id)
        {
            try
            {
                if (Title == null || Title == "") { return false; }
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Plan> Plan = uow.PlanRepo.GetAll().Where(e => e.Title.ToLower() == Title.ToLower() && e.Id != Id);
                if (Plan.ToList().Count > 0)
                {
                    return true;
                }
                else return false;
            }
            catch
            {
                throw;
            }
        }

        //Check child entities present by id
        public static bool HasChildEntities(int Id)
        {
            try
            {
                using (var uow = new UnitOfWork())
                {

                    var count = uow.PlanRepo.Count(a => a.Id == Id);
                    if (count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (msgBlasterValidationException)
            {
                throw new System.TimeoutException();
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

    }
}

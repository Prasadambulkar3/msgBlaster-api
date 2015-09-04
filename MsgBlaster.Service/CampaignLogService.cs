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
    public class CampaignLogService
    {
        //public static int Create(CampaignLogDTO CampaignLogDTO)
        //{
        //    try
        //    {
        //        var CampaignLog = new CampaignLog();
        //        using (var uow = new UnitOfWork())
        //        {
        //            CampaignLog = Transform.CampaignLogToDomain(CampaignLogDTO);
        //            uow.CampaignLogRepo.Insert(CampaignLog);
        //            uow.SaveChanges();
        //            return (CampaignLog.Id);

        //        }

        //    }
        //    //catch (LoggedInUserException)
        //    //{
        //    //    throw new System.TimeoutException();
        //    //}
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}

        //public static void Edit(CampaignLogDTO campaignLogDTO)
        //{
        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        CampaignLog CampaignLog = Transform.CampaignLogToDomain(campaignLogDTO);
        //        uow.CampaignLogRepo.Update(CampaignLog);
        //        uow.SaveChanges();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public static void Delete(int id)
        //{
        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        uow.CampaignLogRepo.Delete(id);
        //        uow.SaveChanges();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public static CampaignLogDTO GetById(int Id)
        //{
        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        CampaignLog CampaignLog = uow.CampaignLogRepo.GetById(Id);
        //        CampaignLogDTO CampaignLogDTO = Transform.CampaignLogToDTO(CampaignLog);
        //        CampaignLogDTO.MessageStatus = CampaignLogDTO.MessageStatus.Substring(0, CampaignLogDTO.MessageStatus.IndexOf('-'));
        //        return CampaignLogDTO;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public static List<CampaignLogDTO> GetCampaignLogList()
        //{

        //    List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();

        //    try
        //    {

        //        using (var uow = new UnitOfWork())
        //        {
        //            IEnumerable<CampaignLog> CampaignLog = uow.CampaignLogRepo.GetAll();
        //            if (CampaignLog != null)
        //            {
        //                foreach (var item in CampaignLog)
        //                {
        //                    CampaignLogDTOList.Add(Transform.CampaignLogToDTO(item));
        //                }
        //            }
        //        }

        //        return CampaignLogDTOList;
        //    }
        //    //catch (LoggedInUserException)
        //    //{
        //    //    throw new System.TimeoutException();
        //    //}
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public static List<CampaignLogDTO> GetCampaignLogListByCampaignId(int CampaignId)
        //{

        //    List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();

        //    try
        //    {

        //        using (var uow = new UnitOfWork())
        //        {
        //            IEnumerable<CampaignLog> CampaignLog = uow.CampaignLogRepo.GetAll().Where(e=>e.CampaignId == CampaignId);
        //            if (CampaignLog != null)
        //            {
        //                foreach (var item in CampaignLog)
        //                {
        //                    CampaignLogDTOList.Add(Transform.CampaignLogToDTO(item));
        //                }
        //            }
        //        }

        //        return CampaignLogDTOList;
        //    }
        //    //catch (LoggedInUserException)
        //    //{
        //    //    throw new System.TimeoutException();
        //    //}
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public static List<CampaignLogDTO> GetCampaignLogListByCampaignIdAndMobile(int CampaignId, string Mobile)
        //{

        //    List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();

        //    try
        //    {

        //        using (var uow = new UnitOfWork())
        //        {
        //            IEnumerable<CampaignLog> CampaignLog = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == CampaignId && e.RecipientsNumber == Mobile);
        //            if (CampaignLog != null)
        //            {
        //                foreach (var item in CampaignLog)
        //                {
        //                    CampaignLogDTOList.Add(Transform.CampaignLogToDTO(item));
        //                }
        //            }
        //        }

        //        return CampaignLogDTOList;
        //    }
        //    //catch (LoggedInUserException)
        //    //{
        //    //    throw new System.TimeoutException();
        //    //}
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public static List<CampaignLogDTO> GetCampaignLogListSearchByClientId(int ClientId, string search, PagingInfo pagingInfo)
        //{
        //    List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
        //    int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
        //    int take = pagingInfo.ItemsPerPage;

        //    try
        //    {

        //        using (var uow = new UnitOfWork())
        //        {
                   

        //            IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId);
        //            //var campaigns = from c in Campaign where (from g in c.CampaignLogs where c.ClientId == ClientId select g).Any() select c;
                 
        //            if (Campaign != null)
        //            {
                        
        //                foreach (var item in Campaign)
        //                {
        //                    IEnumerable<CampaignLog> CampaignLog = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == item.Id).OrderByDescending(e => e.SentDateTime);

        //                    if (CampaignLog != null)
        //                    {
        //                        foreach (var itemCampaignLog in CampaignLog)
        //                        {
        //                            CampaignLogDTO CampaignLogDTO = new CampaignLogDTO();
        //                            CampaignLogDTO = Transform.CampaignLogToDTO( itemCampaignLog);
        //                            CampaignLogDTO.MessageStatus = CampaignLogDTO.MessageStatus.Substring(0, CampaignLogDTO.MessageStatus.IndexOf('-'));
        //                            CampaignLogDTO.CampaignName = item.Name;
        //                            CampaignLogDTOList.Add(CampaignLogDTO);// (Transform.CampaignLogToDTO(itemCampaignLog));
        //                        }
        //                    }
        //                }


        //                        if (search != "" && search != null)
        //                        {
        //                            bool Isdate = CommonService.IsDate(search);
        //                            if (Isdate != true)
        //                            {
        //                                List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
        //                                var CampaignLogSearch = CampaignLogDTOList.Where(e => e.MessageStatus.ToLower().Contains(search.ToLower()) || e.Message.ToLower().Contains(search.ToLower()) || (e.MessageID != null ? (e.MessageID.ToLower().Contains(search.ToLower())) : false) || e.RecipientsNumber.Contains(search) || (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(search.ToLower())) : false)).OrderByDescending(e => e.SentDateTime).Skip(skip).Take(take) ; //|| (e.CampaignName != null ? (e.CampaignName.ToLower().Contains(search.ToLower())) : false)
        //                                if (CampaignLogSearch != null)
        //                                {
        //                                    foreach (var itemsearch in CampaignLogSearch)
        //                                    {

        //                                        CampaignLogDTOSearchList.Add(itemsearch);
        //                                    }
        //                                }
        //                                return CampaignLogDTOSearchList;

        //                            }
        //                            else
        //                            {
        //                                List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
        //                                DateTime date = Convert.ToDateTime(search);
        //                                var CampaignLogSearch = CampaignLogDTOList.Where(e => e.SentDateTime >= date && e.SentDateTime < date.AddDays(1)).OrderByDescending(e => e.SentDateTime).Skip(skip).Take(take);
        //                                if (CampaignLogSearch != null)
        //                                {
        //                                    foreach (var itemsearch in CampaignLogSearch)
        //                                    {

        //                                        CampaignLogDTOSearchList.Add(itemsearch);
        //                                    }
        //                                }
        //                                return CampaignLogDTOSearchList;

        //                            }
        //                        }
        //                    }
                        
        //        }

        //        return CampaignLogDTOList.Skip(skip).Take(take).ToList();
        //    }
        //    //catch (LoggedInUserException)
        //    //{
        //    //    throw new System.TimeoutException();
        //    //}
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public static PageData<CampaignLogDTO> GetCampaignLogPagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        //{
        //    List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
        //    PageData<CampaignLogDTO> pageList = new PageData<CampaignLogDTO>();

        //    if (pagingInfo == null)
        //    {
        //        PagingInfo PagingInfoCreated = new PagingInfo();
        //        PagingInfoCreated.Page = 1;
        //        PagingInfoCreated.Reverse = false;
        //        PagingInfoCreated.ItemsPerPage = 1;
        //        PagingInfoCreated.Search = "";
        //        PagingInfoCreated.TotalItem = 0;

        //        pagingInfo = PagingInfoCreated;
        //    }

        //    CampaignLogDTOList = GetCampaignLogListSearchByClientId(ClientId, pagingInfo.Search, pagingInfo);
        //    IQueryable<CampaignLogDTO> CampaignLogDTOPagedList = CampaignLogDTOList.AsQueryable();

        //    UnitOfWork uow = new UnitOfWork();
        //    int count = 0;

        //    if (pagingInfo.Search != "" && pagingInfo.Search != null)
        //    {
        //        bool IsDate = CommonService.IsDate(pagingInfo.Search);
        //        if (IsDate != true)
        //        {
        //            count = 0;
        //            count = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == pagingInfo.CampaignId && e.MessageStatus.ToLower().Contains(pagingInfo.Search.ToLower()) || e.Message.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.MessageID != null ? (e.MessageID.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.RecipientsNumber.Contains(pagingInfo.Search) || (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)).Count();
        //        }
        //        else
        //        {
        //            DateTime date = Convert.ToDateTime(pagingInfo.Search);
        //            count = 0;
        //            count = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == pagingInfo.CampaignId && e.SentDateTime >= date && e.SentDateTime < date.AddDays(1)).Count();

        //        }

        //    }
        //    else
        //    {
        //        count = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == pagingInfo.CampaignId).Count();
        //    }


        //    ////Sorting
        //    //CampaignLogDTOPagedList = PagingService.Sorting<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

        //    // paging
        //    if (CampaignLogDTOPagedList.Count() > 0)
        //    {
        //        //var ContacDTOPerPage = PagingService.Paging<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
        //        pageList.Count = count;// CampaignLogDTOPagedList.Count();

        //        List<CampaignLogDTO> pagedCampaignLogDTOList = new List<CampaignLogDTO>();
        //        foreach (var item in CampaignLogDTOPagedList)
        //        {
        //            pagedCampaignLogDTOList.Add(item);
        //        }
        //        pageList.Data = pagedCampaignLogDTOList;
        //    }
        //    else
        //    {
        //        pageList.Data = null;
        //    }



        //    return pageList;
        //}

        
        //public static List<CampaignLogDTO> GetCampaignLogListSearchByPartnerId(int PartnerId, string search ,PagingInfo pageinfo)
        //{
        //    List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
            
        //    try
        //    {

        //        using (var uow = new UnitOfWork())
        //        {
        //            var Campaign = CampaignService.GetCampaignsbyPartnerId(PartnerId, "", pageinfo);// uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId);
        //            //var campaigns = from c in Campaign where (from g in c.CampaignLogs where c.ClientId == ClientId select g).Any() select c;

        //            if (Campaign != null)
        //            {
        //                foreach (var item in Campaign)
        //                {
        //                    //var CampaignLog = Campaign.Where(e => e.Id == item.Id).ToList(); // uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == item.Id);
        //                    IEnumerable<CampaignLog> CampaignLog = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == item.Id).OrderByDescending(e => e.SentDateTime);
        //                    if (CampaignLog != null)
        //                    { 
        //                        foreach (var itemCampaignLog in CampaignLog)
        //                        {
                                   
        //                            CampaignLogDTO CampaignLogDTO = new CampaignLogDTO();
        //                            CampaignLogDTO = Transform.CampaignLogToDTO(itemCampaignLog);
        //                            CampaignLogDTO.MessageStatus = CampaignLogDTO.MessageStatus.Substring(0, CampaignLogDTO.MessageStatus.IndexOf('-'));
        //                            CampaignLogDTO.CampaignName = item.Name;
        //                            CampaignLogDTOList.Add(CampaignLogDTO);// (Transform.CampaignLogToDTO(itemCampaignLog));
        //                        }
        //                    }
        //                }
                   

        //                        if (search != "" && search != null)
        //                        {
        //                            bool Isdate = CommonService.IsDate(search);
        //                            if (Isdate != true)
        //                            {
        //                                List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
        //                                var CampaignLogSearch = CampaignLogDTOList.Where(e => e.MessageStatus.ToLower().Contains(search.ToLower()) || e.Message.ToLower().Contains(search.ToLower()) || (e.MessageID != null ? (e.MessageID.ToLower().Contains(search.ToLower())) : false) || e.RecipientsNumber.Contains(search) || (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(search.ToLower())) : false) || (e.CampaignName != null ? (e.CampaignName.ToLower().Contains(search.ToLower())) : false)).OrderByDescending(e =>e.SentDateTime);
        //                                if (CampaignLogSearch != null)
        //                                {
        //                                    foreach (var itemsearch in CampaignLogSearch)
        //                                    {

        //                                        CampaignLogDTOSearchList.Add(itemsearch);
        //                                    }
        //                                }
        //                                return CampaignLogDTOSearchList;

        //                            }
        //                            else
        //                            {
        //                                List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
        //                                DateTime date = Convert.ToDateTime(search);
        //                                var CampaignLogSearch = CampaignLogDTOList.Where(e => e.SentDateTime >= date && e.SentDateTime < date.AddDays(1)).OrderByDescending(e => e.SentDateTime);
        //                                if (CampaignLogSearch != null)
        //                                {
        //                                    foreach (var itemsearch in CampaignLogSearch)
        //                                    {

        //                                        CampaignLogDTOSearchList.Add(itemsearch);
        //                                    }
        //                                }
        //                                return CampaignLogDTOSearchList;

        //                            }
        //                        }
        //            }

                    
        //        }

        //        return CampaignLogDTOList;
        //    }
        //    //catch (LoggedInUserException)
        //    //{
        //    //    throw new System.TimeoutException();
        //    //}
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public static PageData<CampaignLogDTO> GetCampaignLogPagedListbyPartnerId(PagingInfo pagingInfo, int PartnerId)
        //{
        //    List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
        //    PageData<CampaignLogDTO> pageList = new PageData<CampaignLogDTO>();

        //    if (pagingInfo == null)
        //    {
        //        PagingInfo PagingInfoCreated = new PagingInfo();
        //        PagingInfoCreated.Page = 1;
        //        PagingInfoCreated.Reverse = false;
        //        PagingInfoCreated.ItemsPerPage = 1;
        //        PagingInfoCreated.Search = "";
        //        PagingInfoCreated.TotalItem = 0;

        //        pagingInfo = PagingInfoCreated;
        //    }

        //    CampaignLogDTOList = GetCampaignLogListSearchByPartnerId(PartnerId, pagingInfo.Search, pagingInfo);
        //    IQueryable<CampaignLogDTO> CampaignLogDTOPagedList = CampaignLogDTOList.AsQueryable();

        //    ////Sorting
        //    //CampaignLogDTOPagedList = PagingService.Sorting<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

        //    // paging
        //    if (CampaignLogDTOPagedList.Count() > 0)
        //    {
        //        var ContacDTOPerPage = PagingService.Paging<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
        //        pageList.Count = CampaignLogDTOPagedList.Count();

        //        List<CampaignLogDTO> pagedCampaignLogDTOList = new List<CampaignLogDTO>();
        //        foreach (var item in ContacDTOPerPage)
        //        {
        //            pagedCampaignLogDTOList.Add(item);
        //        }
        //        pageList.Data = pagedCampaignLogDTOList;
        //    }
        //    else
        //    {
        //        pageList.Data = null;
        //    }



        //    return pageList;
        //}

        //public static List<CampaignLogDTO> GetCampaignLogListSearchByCampaignIdWithSuccessLog(int CampaignId, string search, bool IsSuccess, PagingInfo pagingInfo)
        //{
        //    List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
        //    int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
        //    int take = pagingInfo.ItemsPerPage;


        //    if (CampaignId <= 0)
        //    {
        //        return CampaignLogDTOList;
        //    }
        //    int ClientId = 0;
        //    try
        //    {
        //        CampaignDTO CampaignDTO = new CampaignDTO();
        //        CampaignDTO = CampaignService.GetById(CampaignId);                
        //        ClientId = CampaignDTO.ClientId;
        //        if (ClientId <= 0) 
        //        {
        //            return CampaignLogDTOList;
        //        }

        //        using (var uow = new UnitOfWork())
        //        {
        //            IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.Id == CampaignId );
        //            //var campaigns = from c in Campaign where (from g in c.CampaignLogs where c.ClientId == ClientId select g).Any() select c;

        //            if (Campaign != null)
        //            {

        //                foreach (var item in Campaign)
        //                {                          

        //                    IEnumerable<CampaignLog> CampaignLog = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == item.Id && e.IsSuccess == IsSuccess).OrderByDescending(e => e.SentDateTime);
 
        //                    if (CampaignLog != null)
        //                    {
        //                        foreach (var itemCampaignLog in CampaignLog)
        //                        {
        //                            CampaignLogDTO CampaignLogDTO = new CampaignLogDTO();
        //                            CampaignLogDTO = Transform.CampaignLogToDTO(itemCampaignLog);
        //                            CampaignLogDTO.MessageStatus = CampaignLogDTO.MessageStatus.Substring(0, CampaignLogDTO.MessageStatus.IndexOf('-'));
        //                            CampaignLogDTO.CampaignName = item.Name;
        //                            CampaignLogDTOList.Add(CampaignLogDTO);// (Transform.CampaignLogToDTO(itemCampaignLog));
        //                        }
        //                    }
        //                }


        //                if (search != "" && search != null)
        //                {
        //                    bool Isdate = CommonService.IsDate(search);
        //                    if (Isdate != true)
        //                    {
        //                        List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
        //                        var CampaignLogSearch = CampaignLogDTOList.Where(e => e.MessageStatus.ToLower().Contains(search.ToLower()) || e.Message.ToLower().Contains(search.ToLower()) || (e.MessageID != null ? (e.MessageID.ToLower().Contains(search.ToLower())) : false) || e.RecipientsNumber.Contains(search) || (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(search.ToLower())) : false) && e.IsSuccess == IsSuccess && e.CampaignId == CampaignId).OrderByDescending(e => e.SentDateTime).Skip(skip).Take(take);//|| (e.CampaignName != null ? (e.CampaignName.ToLower().Contains(search.ToLower())) : false) 
        //                        if (CampaignLogSearch != null)
        //                        {
        //                            foreach (var itemsearch in CampaignLogSearch)
        //                            {

        //                                CampaignLogDTOSearchList.Add(itemsearch);
        //                            }
        //                        }
        //                        return CampaignLogDTOSearchList;

        //                    }
        //                    else
        //                    {
        //                        List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
        //                        DateTime date = Convert.ToDateTime(search);
        //                        var CampaignLogSearch = CampaignLogDTOList.Where(e => e.SentDateTime >= date && e.SentDateTime < date.AddDays(1) && e.IsSuccess == IsSuccess && e.CampaignId == CampaignId).OrderByDescending(e => e.SentDateTime).Skip(skip).Take(take);
        //                        if (CampaignLogSearch != null)
        //                        {
        //                            foreach (var itemsearch in CampaignLogSearch)
        //                            {

        //                                CampaignLogDTOSearchList.Add(itemsearch);
        //                            }
        //                        }
        //                        return CampaignLogDTOSearchList;

        //                    }
        //                }
        //            }

        //        }

        //        return CampaignLogDTOList.Skip(skip).Take(take).ToList();
        //    }
        //    //catch (LoggedInUserException)
        //    //{
        //    //    throw new System.TimeoutException();
        //    //}
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


        //public static PageData<CampaignLogDTO> GetCampaignLogPagedListbyCampaignIdWithSuccessLog(PagingInfo pagingInfo, int CampaignId, bool IsSuccess)
        //{
        //    List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
        //    PageData<CampaignLogDTO> pageList = new PageData<CampaignLogDTO>();

        //    if (pagingInfo == null)
        //    {
        //        PagingInfo PagingInfoCreated = new PagingInfo();
        //        PagingInfoCreated.Page = 1;
        //        PagingInfoCreated.Reverse = false;
        //        PagingInfoCreated.ItemsPerPage = 1;
        //        PagingInfoCreated.Search = "";
        //        PagingInfoCreated.TotalItem = 0;

        //        pagingInfo = PagingInfoCreated;
        //    }

        //    CampaignLogDTOList = GetCampaignLogListSearchByCampaignIdWithSuccessLog(CampaignId, pagingInfo.Search, IsSuccess, pagingInfo);
        //    IQueryable<CampaignLogDTO> CampaignLogDTOPagedList = CampaignLogDTOList.AsQueryable();
        //    UnitOfWork uow = new UnitOfWork();
        //    int count = 0;
        //    if (pagingInfo.Search != "" && pagingInfo.Search != null)
        //    {
        //        bool IsDate = CommonService.IsDate(pagingInfo.Search);
        //        if (IsDate != true)
        //        {
        //            count = 0;
        //            count = uow.CampaignLogRepo.GetAll().Where(e => e.MessageStatus.ToLower().Contains(pagingInfo.Search.ToLower()) || e.Message.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.MessageID != null ? (e.MessageID.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.RecipientsNumber.Contains(pagingInfo.Search) || (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(pagingInfo.Search.ToLower())) : false) && e.IsSuccess == IsSuccess && e.CampaignId == CampaignId).Count();
        //        }
        //        else
        //        {
        //            DateTime date = Convert.ToDateTime(pagingInfo.Search);
        //            count = 0;
        //            count = uow.CampaignLogRepo.GetAll().Where(e => e.SentDateTime >= date && e.SentDateTime < date.AddDays(1) && e.IsSuccess == IsSuccess && e.CampaignId == CampaignId).Count();

        //        }

        //    }
        //    else
        //    {
        //        count = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == CampaignId && e.IsSuccess == IsSuccess).Count();
        //    }

        //    ////Sorting
        //    //CampaignLogDTOPagedList = PagingService.Sorting<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

        //    // paging
        //    if (CampaignLogDTOPagedList.Count() > 0)
        //    {
        //        //var ContacDTOPerPage = PagingService.Paging<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
        //        pageList.Count = count;//  CampaignLogDTOPagedList.Count();



        //        List<CampaignLogDTO> pagedCampaignLogDTOList = new List<CampaignLogDTO>();
        //        foreach (var item in CampaignLogDTOPagedList)
        //        {
        //            pagedCampaignLogDTOList.Add(item);
        //        }
        //        pageList.Data = pagedCampaignLogDTOList;
        //        pageList.SuccessCount = GetSuccessCount(CampaignId);
        //        pageList.FailureCount = GetFailureCount(CampaignId);
        //    }
        //    else
        //    {
        //        pageList.Data = null;
        //        pageList.SuccessCount = GetSuccessCount(CampaignId);
        //        pageList.FailureCount = GetFailureCount(CampaignId);
        //    }



        //    return pageList;
        //}

        //public static int GetSuccessCount(int CampaignId)
        //{
        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        IEnumerable<CampaignLog> CampaignLog = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == CampaignId && e.IsSuccess==true);
        //        if (CampaignLog != null) 
        //        {
        //            return CampaignLog.Count();
        //        }

        //        return 0;

        //    }
        //    catch 
        //    {
        //        throw;
        //    }
 
        //}
        
        //public static int GetFailureCount(int CampaignId)
        //{
        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        IEnumerable<CampaignLog> CampaignLog = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == CampaignId && e.IsSuccess == false );
        //        if (CampaignLog != null)
        //        {
        //            return CampaignLog.Count();
        //        }

        //        return 0;

        //    }
        //    catch
        //    {
        //        throw;
        //    }

        //}

        //public static int GetTodaysCampaignMsssageSentCount(int ClientId)
        //{
        //    try
        //    {
        //        int TodaysSMSCount = 0;
        //        UnitOfWork uow = new UnitOfWork();
        //        IEnumerable<Campaign> Campaign = uow.CampaignRepo.GetAll().Where(e => e.ClientId == ClientId && e.IsSent==true);
        //        foreach (var item in Campaign)
        //        {
        //            IEnumerable<CampaignLog> CampaignLog = uow.CampaignLogRepo.GetAll().Where(e => e.CampaignId == item.Id && e.SentDateTime > System.DateTime.Now.AddDays(-1) && e.SentDateTime < System.DateTime.Now.AddDays(1));
                   
        //            if (CampaignLog != null)
        //            {
        //                foreach (var itemCampaignLog in CampaignLog)
        //                {
        //                    TodaysSMSCount = TodaysSMSCount + 1;
        //                }
        //            } 
        //        }

        //        return TodaysSMSCount;

        //    }
        //    catch
        //    {
        //        throw;
        //    }

        //}


    }
}

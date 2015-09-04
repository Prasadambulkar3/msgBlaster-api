using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Repo;
using MsgBlaster.Domain;

namespace MsgBlaster.Service
{
    public class TemplateService
    {
        #region "CRUD Functionality"

        //Create template
        public static int Create(TemplateDTO TemplateDTO)
        {
            if (TemplateDTO.Title == null || TemplateDTO.Title == "") { return 0; }

            try
            {
                var Template = new Template();

                GlobalSettings.LoggedInClientId = TemplateDTO.ClientId;                
                int PartnerId = ClientService.GetById(TemplateDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                UnitOfWork uow = new UnitOfWork();
                Template = Transform.TemplateToDomain(TemplateDTO);
                uow.TemplateRepo.Insert(Template);

                uow.SaveChanges();
                TemplateDTO.Id = Template.Id;
                return TemplateDTO.Id;

            }

            catch (Exception)
            {
                throw;
            }
        }

        //Edit template
        public static void Edit(TemplateDTO TemplateDTO)
        {
            if (TemplateDTO.Title != null || TemplateDTO.Title != "")
            {
                try
                {
                    GlobalSettings.LoggedInClientId = TemplateDTO.ClientId;
                    int PartnerId = ClientService.GetById(TemplateDTO.ClientId).PartnerId;
                    GlobalSettings.LoggedInPartnerId = PartnerId;

                    UnitOfWork uow = new UnitOfWork();
                    Template Template = Transform.TemplateToDomain(TemplateDTO);
                    uow.TemplateRepo.Update(Template);
                    uow.SaveChanges();
                }
                catch
                {
                    throw;
                }
            }
        }

        //Delete template
        public static void Delete(int id)
        {
            try
            {
                GlobalSettings.LoggedInClientId = TemplateService.GetById(id).ClientId;
                int PartnerId = ClientService.GetById(Convert.ToInt32(GlobalSettings.LoggedInClientId)).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                UnitOfWork uow = new UnitOfWork();
                uow.TemplateRepo.Delete(id);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get template details by id
        public static TemplateDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Template Template = uow.TemplateRepo.GetById(Id);
                TemplateDTO TemplateDTO = Transform.TemplateToDTO(Template);
                return TemplateDTO;
            }
            catch
            {
                throw;
            }
        }       

        #endregion

        #region "List Functionality"

        //Get template client wise list
        public static List<TemplateDTO> GetTemplateListByClientId(int ClientId)
        {

            List<TemplateDTO> TemplateDTOList = new List<TemplateDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Template> Template = uow.TemplateRepo.GetAll().Where(e => e.ClientId == ClientId).OrderBy(e => e.Title).ToList();
                    if (Template != null)
                    {
                        foreach (var item in Template)
                        {
                            TemplateDTOList.Add(Transform.TemplateToDTO(item));
                        }
                    }
                }

                return TemplateDTOList;
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

        //Get template client wise list by client id and search criteria
        public static List<TemplateDTO> GetTemplatesbyClientId(int ClientId, string search, PagingInfo pagingInfo)
        {

            List<TemplateDTO> templateDTO = new List<TemplateDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;
                IQueryable<Template> Templates = uow.TemplateRepo.GetAll().Where(e => e.ClientId == ClientId).AsQueryable();//.OrderBy(e => e.Title).Skip(skip).Take(take).ToList();
                Templates = PagingService.Sorting<Template>(Templates, pagingInfo.SortBy, pagingInfo.Reverse);
                var Template = Templates.Skip(skip).Take(take).ToList();

                if (Template != null)
                {
                    if (search != "" && search != null)
                    {

                        bool IsDate = CommonService.IsDate(search);
                        if (IsDate != true)
                        {
                            // string search
                            IQueryable<Template> Templatesearchs = uow.TemplateRepo.GetAll().Where(e => (e.Title.ToLower().Contains(search.ToLower()) || e.Message.ToLower().Contains(search.ToLower())) && e.ClientId == ClientId).AsQueryable();//.OrderBy(e => e.Title).Skip(skip).Take(take);
                            Templatesearchs = PagingService.Sorting<Template>(Templatesearchs, pagingInfo.SortBy, pagingInfo.Reverse);
                            var Templatesearch = Templatesearchs.Skip(skip).Take(take).ToList();

                            if (Templatesearch != null)
                            {
                                foreach (var item in Templatesearch)
                                {

                                    templateDTO.Add(Transform.TemplateToDTO(item));
                                }
                            }
                            return templateDTO;
                        }
                        else
                        {
                            ////date wise search
                            //DateTime date = Convert.ToDateTime(search);
                            //var Templatesearch = Template.Where(e => e.AnniversaryDate >= date && e.AnniversaryDate < date.AddDays(1) || e.BirthDate >= date && e.BirthDate < date.AddDays(1));

                            //if (Templatesearch != null)
                            //{
                            //    foreach (var item in Templatesearch)
                            //    {
                            //        templateDTO.Add(Transform.TemplateToDTO(item));
                            //    }
                            //}
                            //return templateDTO;

                        }

                    }
                    else
                    {


                        foreach (var item in Template)
                        {
                            templateDTO.Add(Transform.TemplateToDTO(item));
                        }
                    }
                }

                return templateDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get template paged list by client id
        public static PageData<TemplateDTO> GetTemplatePagedListbyClientId(PagingInfo pagingInfo, int ClientId)
        {
            List<TemplateDTO> TemplateDTOList = new List<TemplateDTO>();
            PageData<TemplateDTO> pageList = new PageData<TemplateDTO>();

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


            TemplateDTOList = GetTemplatesbyClientId(ClientId, pagingInfo.Search, pagingInfo);
            IQueryable<TemplateDTO> TemplateDTOPagedList = TemplateDTOList.AsQueryable();


            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = uow.TemplateRepo.GetAll().Where(e => e.Title.ToLower().Contains(pagingInfo.Search.ToLower()) || e.Message.ToLower().Contains(pagingInfo.Search.ToLower()) && e.ClientId == ClientId).Count();
                }
                else
                {


                }

            }
            else
            {
                count = uow.TemplateRepo.GetAll().Where(e => e.ClientId == ClientId).Count();
            }
            ////Sorting
            //TemplateDTOPagedList = PagingService.Sorting<TemplateDTO>(TemplateDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (TemplateDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<TemplateDTO>(TemplateDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// TemplateDTOPagedList.Count();

                List<TemplateDTO> pagedTemplateDTOList = new List<TemplateDTO>();
                foreach (var item in TemplateDTOPagedList)
                {
                    pagedTemplateDTOList.Add(item);
                }
                pageList.Data = pagedTemplateDTOList;
            }
            else
            {
                pageList.Data = null;
            }



            return pageList;
        }

        //Get recent template list by client id
        public static List<TemplateDTO> GetRecentTemplateListByClientId(int ClientId, int Top)
        {

            List<TemplateDTO> TemplateDTOList = new List<TemplateDTO>();
            int count = 0;
            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Template> Template = uow.TemplateRepo.GetAll().Where(e => e.ClientId == ClientId).OrderByDescending(e => e.Id).ToList();
                    if (Template != null)
                    {
                        count = 1;
                        foreach (var item in Template)
                        {
                            TemplateDTOList.Add(Transform.TemplateToDTO(item));
                            if (count == Top)
                            {
                                return TemplateDTOList;
                            }
                            count = count + 1;
                        }

                    }
                }

                return TemplateDTOList;
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

        #endregion

        #region "Other Functionality"
        
        //Get template details by message 
        public static TemplateDTO GetTemplateByMessage(string Message)
        {
            try
            {
                TemplateDTO TemplateDTO = new TemplateDTO();
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Template> Template = uow.TemplateRepo.GetAll().Where(e => e.Message.ToLower() == Message.ToLower() && e.ClientId == GlobalSettings.LoggedInClientId);
                if (Template != null)
                {
                    foreach (var item in Template)
                    {
                        TemplateDTO = Transform.TemplateToDTO(item);
                    }
                }

                return TemplateDTO;
            }
            catch
            {
                throw;
            }
        }

        //Check template present flag by using title, client id and id
        public static bool GetByTitleAndClientId(string Title, int ClientId, int Id)
        {
            if (Title == null || Title == "") { return false; }
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Template> Template = uow.TemplateRepo.GetAll().Where(e => e.Title.ToLower() == Title.ToLower() && e.ClientId == ClientId && e.Id != Id);
                //ClientDTO ClientDTO = Transform.ClientToDTO(Client);
                if (Template.ToList().Count > 0)
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

        #endregion
       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;


namespace MsgBlaster.Service
{
    public class CampaignLogXMLService
    {
        #region "CRUD Functionality"

        /// <summary>
        /// Create CampaignLogXML
        /// </summary>
        /// <param name="CampaignLogXMLDTO">CampaignLogXML Object</param>
        /// <returns></returns>
        public static int Create(CampaignLogXMLDTO CampaignLogXMLDTO)
        {
            try
            {
                var CampaignLogXML = new CampaignLogXML();
                using (var uow = new UnitOfWork())
                {
                    CampaignLogXML = Transform.CampaignLogXMLToDomain(CampaignLogXMLDTO);
                    uow.CampaignLogXMLRepo.Insert(CampaignLogXML);
                    uow.SaveChanges();
                    return (CampaignLogXML.Id);

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

        #endregion

        #region "List Functionality"

        /// <summary>
        /// Campaign log by Campaign id
        /// </summary>
        /// <param name="CampaignId">campaign Id</param>
        /// <param name="IsSuccessful">TRUE OR FALSE</param>
        /// <param name="pagingInfo">pagingInfo object</param>
        /// <returns>Returns list of CampaignLogDTO</returns>
        public static List<CampaignLogDTO> GetCampaignLogXMLByCampaignId(int CampaignId, bool IsSuccessful, PagingInfo pagingInfo)
        {
            List<CampaignLogXMLDTO> CampaignLogXMLDTOList = new List<CampaignLogXMLDTO>();

            List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
            CampaignDTO CampaignDTO = new CampaignDTO();
            CampaignDTO = CampaignService.GetById(CampaignId);
            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                IEnumerable<CampaignLogXML> CampaignLogXML = uow.CampaignLogXMLRepo.GetAll().Where(e => e.CampaignId == CampaignId).ToList();

                if (CampaignLogXML != null)
                {
                    foreach (var item in CampaignLogXML)
                    {
                        CampaignLogXMLDTOList.Add(Transform.CampaignLogXMLToDTO(item));
                    }

                    if (CampaignLogXMLDTOList != null)
                    {
                        foreach (var item in CampaignLogXMLDTOList)
                        {
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["msgBlasterWebContext"].ToString());
                            con.Open();
                            SqlCommand command = new SqlCommand("Select XMLLog From CampaignLogXMLs WHERE Id=" + item.Id, con);

                            XmlDocument xdoc = new XmlDocument();
                            XmlReader reader = command.ExecuteXmlReader();

                            if (reader.Read())
                            {
                                xdoc.Load(reader);

                                //XmlNodeList msgList = xdoc.SelectNodes("/packet/numbers/message");
                                //XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");


                                foreach (XmlNode currentnode in xdoc.DocumentElement.GetElementsByTagName("numbers"))
                                {

                                    //


                                    XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");
                                    foreach (XmlNode node in numList)
                                    {
                                        CampaignLogDTO CampaignLogDTO = new CampaignLogDTO();

                                        CampaignLogDTO.RecipientsNumber = node.InnerText;// node.SelectNodes("number").Item(0).InnerText;// node.SelectSingleNode("number").InnerText;
                                        string MsgStatus = null;
                                        string code = null;

                                        if (node.Attributes != null)
                                        {
                                            code = node.Attributes["msgCode"].Value;
                                            CampaignLogDTO.SentDateTime = Convert.ToDateTime(node.Attributes["sentTime"].Value);
                                            CampaignLogDTO.MessageCount = Convert.ToInt32(node.Attributes["msgCount"].Value);
                                            CampaignLogDTO.RequiredCredits = Convert.ToDouble(node.Attributes["msgCredits"].Value);
                                            CampaignLogDTO.CampaignName = CampaignDTO.Name;
                                            CampaignLogDTO.CreatedOn = CampaignDTO.CreatedDate;
                                            CampaignLogDTO.CreatedBy = UserService.GetById(CampaignDTO.CreatedBy).Name;

                                        }

                                        MsgStatus = CodeDescription(code);
                                        CampaignLogDTO.MessageStatus = MsgStatus;
                                        if (IsSuccessful == true)
                                        {
                                            if (code == "1701" || code == "0")
                                            {
                                                CampaignLogDTOList.Add(CampaignLogDTO);
                                            }
                                        }
                                        else if (code != "1701" && code != "0")
                                        {
                                            CampaignLogDTOList.Add(CampaignLogDTO);
                                        }

                                    }
                                }
                            }
                            con.Close();


                            if (pagingInfo.Search != "" && pagingInfo.Search != null)
                            {
                                bool Isdate = CommonService.IsDate(pagingInfo.Search);
                                if (Isdate != true)
                                {
                                    List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
                                    var CampaignLogSearch = CampaignLogDTOList.Where(e => e.MessageStatus.ToLower().Contains(pagingInfo.Search.ToLower()) || e.RecipientsNumber.Contains(pagingInfo.Search) || (e.SentDateTime.ToString() != null ? (Convert.ToDateTime(e.SentDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).OrderByDescending(e => e.SentDateTime);//.Skip(skip).Take(take); //|| (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)|| e.Message.ToLower().Contains(pagingInfo.Search.ToLower())|| (e.MessageID != null ? (e.MessageID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)|| (e.CampaignName != null ? (e.CampaignName.ToLower().Contains(search.ToLower())) : false)
                                    if (CampaignLogSearch != null)
                                    {
                                        foreach (var itemsearch in CampaignLogSearch)
                                        {
                                            itemsearch.CampaignName = CampaignDTO.Name;
                                            itemsearch.CreatedOn = CampaignDTO.CreatedDate;
                                            itemsearch.CreatedBy = UserService.GetById(CampaignDTO.CreatedBy).Name;

                                            CampaignLogDTOSearchList.Add(itemsearch);
                                        }
                                    }

                                    IQueryable<CampaignLogDTO> CampaignLogDTOQuarableSearch = CampaignLogDTOSearchList.AsQueryable();
                                    return CampaignLogDTOQuarableSearch.Skip(skip).Take(take).ToList();

                                }
                                else
                                {
                                    List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
                                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                                    var CampaignLogSearch = CampaignLogDTOList.Where(e => e.SentDateTime >= date && e.SentDateTime < date.AddDays(1)).OrderByDescending(e => e.SentDateTime);//.Skip(skip).Take(take);
                                    if (CampaignLogSearch != null)
                                    {
                                        foreach (var itemsearch in CampaignLogSearch)
                                        {
                                            itemsearch.CampaignName = CampaignDTO.Name;
                                            itemsearch.CreatedOn = CampaignDTO.CreatedDate;
                                            itemsearch.CreatedBy = UserService.GetById(CampaignDTO.CreatedBy).Name;
                                            CampaignLogDTOSearchList.Add(itemsearch);
                                        }
                                    }
                                    IQueryable<CampaignLogDTO> CampaignLogDTOQuarableDate = CampaignLogDTOSearchList.AsQueryable();
                                    return CampaignLogDTOQuarableDate.Skip(skip).Take(take).ToList();

                                }
                            }

                        }

                    }


                }
                //List<CampaignLogXMLDTO> CampaignLogXMLDTOSearchList = new List<CampaignLogXMLDTO>();                

                IQueryable<CampaignLogDTO> CampaignLogDTOQuarableall = CampaignLogDTOList.AsQueryable();
                CampaignLogDTOQuarableall = PagingService.Sorting<CampaignLogDTO>(CampaignLogDTOQuarableall, pagingInfo.SortBy, pagingInfo.Reverse);
                return CampaignLogDTOQuarableall.Skip(skip).Take(take).ToList();

            }

            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Campaign LogPaged List by Campaign Id
        /// </summary>
        /// <param name="CampaignId">Campaign Id</param>
        /// <param name="IsSuccessful">TRUE OR FALSE</param>
        /// <param name="pagingInfo">pagingInfo</param>
        /// <returns>Returns paged list of the campaign log</returns>
        public static PageData<CampaignLogDTO> GetCampaignLogPagedListbyCampaignId(int CampaignId, bool IsSuccessful, PagingInfo pagingInfo)
        {
            List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
            PageData<CampaignLogDTO> pageList = new PageData<CampaignLogDTO>();

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
            CampaignLogDTOList = GetCampaignLogXMLByCampaignId(CampaignId, IsSuccessful, pagingInfo);
            IQueryable<CampaignLogDTO> CampaignLogDTOPagedList = CampaignLogDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = GetCampaignLogXMLPagedCountByCampaignId(CampaignId, IsSuccessful, pagingInfo);
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = GetCampaignLogXMLPagedCountByCampaignId(CampaignId, IsSuccessful, pagingInfo);

                }

            }
            else
            {
                count = GetCampaignLogXMLPagedCountByCampaignId(CampaignId, IsSuccessful, pagingInfo);
            }


            ////Sorting
            // CampaignLogDTOPagedList = PagingService.Sorting<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CampaignLogDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// CampaignLogDTOPagedList.Count();

                List<CampaignLogDTO> pagedCampaignLogDTOList = new List<CampaignLogDTO>();
                foreach (var item in CampaignLogDTOPagedList)
                {
                    pagedCampaignLogDTOList.Add(item);
                }
                pageList.Data = pagedCampaignLogDTOList;
                pageList.SuccessCount = GetCampaignLogXMLCountByCampaignId(CampaignId, true);
                pageList.FailureCount = GetCampaignLogXMLCountByCampaignId(CampaignId, false);
            }
            else
            {
                pageList.Data = null;
                pageList.SuccessCount = GetCampaignLogXMLCountByCampaignId(CampaignId, true);
                pageList.FailureCount = GetCampaignLogXMLCountByCampaignId(CampaignId, false);
            }



            return pageList;
        }

        /// <summary>
        ///  Campaign Log Success or Failed count
        /// </summary>
        /// <param name="CampaignId">Campaign Id</param>
        /// <param name="IsSuccessful">TRUE OR False</param>
        /// <param name="pagingInfo">pagingInfo object</param>
        /// <returns>Count of Campaign Log</returns>
        public static int GetCampaignLogXMLPagedCountByCampaignId(int CampaignId, bool IsSuccessful, PagingInfo pagingInfo)
        {
            List<CampaignLogXMLDTO> CampaignLogXMLDTOList = new List<CampaignLogXMLDTO>();

            List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<CampaignLogXML> CampaignLogXML = uow.CampaignLogXMLRepo.GetAll().Where(e => e.CampaignId == CampaignId).ToList();
                if (CampaignLogXML != null)
                {
                    foreach (var item in CampaignLogXML)
                    {
                        CampaignLogXMLDTOList.Add(Transform.CampaignLogXMLToDTO(item));
                    }

                    if (CampaignLogXMLDTOList != null)
                    {
                        foreach (var item in CampaignLogXMLDTOList)
                        {
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["msgBlasterWebContext"].ToString());
                            con.Open();
                            SqlCommand command = new SqlCommand("Select XMLLog From CampaignLogXMLs WHERE Id=" + item.Id, con);

                            XmlDocument xdoc = new XmlDocument();
                            XmlReader reader = command.ExecuteXmlReader();

                            if (reader.Read())
                            {
                                xdoc.Load(reader);

                                //XmlNodeList msgList = xdoc.SelectNodes("/packet/numbers/message");
                                //XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");


                                foreach (XmlNode currentnode in xdoc.DocumentElement.GetElementsByTagName("numbers"))
                                {

                                    //


                                    XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");
                                    foreach (XmlNode node in numList)
                                    {
                                        CampaignLogDTO CampaignLogDTO = new CampaignLogDTO();
                                        CampaignLogDTO.RecipientsNumber = node.InnerText;// node.SelectNodes("number").Item(0).InnerText;// node.SelectSingleNode("number").InnerText;
                                        string MsgStatus = null;
                                        string code = null;

                                        if (node.Attributes != null)
                                        {
                                            code = node.Attributes["msgCode"].Value;
                                            CampaignLogDTO.SentDateTime = Convert.ToDateTime(node.Attributes["sentTime"].Value);
                                            CampaignLogDTO.MessageCount = Convert.ToInt32(node.Attributes["msgCount"].Value);
                                            CampaignLogDTO.RequiredCredits = Convert.ToDouble(node.Attributes["msgCredits"].Value);

                                        }

                                        MsgStatus = CodeDescription(code);
                                        CampaignLogDTO.MessageStatus = MsgStatus;
                                        if (IsSuccessful == true)
                                        {
                                            if (code == "1701" || code == "0")
                                            {
                                                CampaignLogDTOList.Add(CampaignLogDTO);
                                            }
                                        }
                                        else if (code != "1701" && code != "0")
                                        {
                                            CampaignLogDTOList.Add(CampaignLogDTO);
                                        }

                                    }
                                }

                            }
                            con.Close();


                            if (pagingInfo.Search != "" && pagingInfo.Search != null)
                            {
                                bool Isdate = CommonService.IsDate(pagingInfo.Search);
                                if (Isdate != true)
                                {
                                    List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
                                    //var CampaignLogSearch = CampaignLogDTOList.Where(e => e.MessageStatus.ToLower().Contains(pagingInfo.Search.ToLower()) || e.Message.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.MessageID != null ? (e.MessageID.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.RecipientsNumber.Contains(pagingInfo.Search) || (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)).OrderByDescending(e => e.SentDateTime);
                                    var CampaignLogSearch = CampaignLogDTOList.Where(e => e.MessageStatus.ToLower().Contains(pagingInfo.Search.ToLower()) || e.RecipientsNumber.Contains(pagingInfo.Search) || (e.SentDateTime.ToString() != null ? (Convert.ToDateTime(e.SentDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).OrderByDescending(e => e.SentDateTime); //|| (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)|| e.Message.ToLower().Contains(pagingInfo.Search.ToLower())|| (e.MessageID != null ? (e.MessageID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)|| (e.CampaignName != null ? (e.CampaignName.ToLower().Contains(search.ToLower())) : false)

                                    if (CampaignLogSearch != null)
                                    {
                                        foreach (var itemsearch in CampaignLogSearch)
                                        {

                                            CampaignLogDTOSearchList.Add(itemsearch);
                                        }
                                    }
                                    return CampaignLogDTOSearchList.Count();

                                }
                                else
                                {
                                    List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
                                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                                    var CampaignLogSearch = CampaignLogDTOList.Where(e => e.SentDateTime >= date && e.SentDateTime < date.AddDays(1)).OrderByDescending(e => e.SentDateTime);
                                    if (CampaignLogSearch != null)
                                    {
                                        foreach (var itemsearch in CampaignLogSearch)
                                        {

                                            CampaignLogDTOSearchList.Add(itemsearch);
                                        }
                                    }
                                    return CampaignLogDTOSearchList.Count();

                                }
                            }

                        }


                    }
                    //List<CampaignLogXMLDTO> CampaignLogXMLDTOSearchList = new List<CampaignLogXMLDTO>();
                }
                return CampaignLogDTOList.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  Campaign Log Paged Count By Campaign Id
        /// </summary>
        /// <param name="CampaignId">campaign Id</param>
        /// <param name="pagingInfo">pagingInfo object</param>
        /// <returns>Paged count of Campaign log</returns>
        public static int GetCampaignLogXMLPagedCountByCampaignId(int CampaignId, PagingInfo pagingInfo)
        {
            List<CampaignLogXMLDTO> CampaignLogXMLDTOList = new List<CampaignLogXMLDTO>();

            List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<CampaignLogXML> CampaignLogXML = uow.CampaignLogXMLRepo.GetAll().Where(e => e.CampaignId == CampaignId).ToList();
                if (CampaignLogXML != null)
                {
                    foreach (var item in CampaignLogXML)
                    {
                        CampaignLogXMLDTOList.Add(Transform.CampaignLogXMLToDTO(item));
                    }

                    if (CampaignLogXMLDTOList != null)
                    {
                        foreach (var item in CampaignLogXMLDTOList)
                        {
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["msgBlasterWebContext"].ToString());
                            con.Open();
                            SqlCommand command = new SqlCommand("Select XMLLog From CampaignLogXMLs WHERE Id=" + item.Id, con);

                            XmlDocument xdoc = new XmlDocument();
                            XmlReader reader = command.ExecuteXmlReader();

                            if (reader.Read())
                            {
                                xdoc.Load(reader);

                                //XmlNodeList msgList = xdoc.SelectNodes("/packet/numbers/message");
                                //XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");


                                foreach (XmlNode currentnode in xdoc.DocumentElement.GetElementsByTagName("numbers"))
                                {

                                    //


                                    XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");
                                    foreach (XmlNode node in numList)
                                    {
                                        CampaignLogDTO CampaignLogDTO = new CampaignLogDTO();
                                        CampaignLogDTO.RecipientsNumber = node.InnerText;// node.SelectNodes("number").Item(0).InnerText;// node.SelectSingleNode("number").InnerText;
                                        string MsgStatus = null;
                                        string code = null;

                                        if (node.Attributes != null)
                                        {
                                            code = node.Attributes["msgCode"].Value;
                                            CampaignLogDTO.SentDateTime = Convert.ToDateTime(node.Attributes["sentTime"].Value);
                                            CampaignLogDTO.MessageCount = Convert.ToInt32(node.Attributes["msgCount"].Value);
                                            CampaignLogDTO.RequiredCredits = Convert.ToDouble(node.Attributes["msgCredits"].Value);

                                        }

                                        MsgStatus = CodeDescription(code);
                                        CampaignLogDTO.MessageStatus = MsgStatus;
                                        //if (IsSuccessful == true)
                                        //{
                                        if (code == "1701" || code == "0")
                                        {
                                            CampaignLogDTOList.Add(CampaignLogDTO);
                                        }
                                        else
                                            //}
                                            //else if (code != "1701" && code != "0")
                                            //{
                                            CampaignLogDTOList.Add(CampaignLogDTO);
                                        //}

                                    }
                                }

                            }
                            con.Close();


                            if (pagingInfo.Search != "" && pagingInfo.Search != null)
                            {
                                bool Isdate = CommonService.IsDate(pagingInfo.Search);
                                if (Isdate != true)
                                {
                                    List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
                                    //var CampaignLogSearch = CampaignLogDTOList.Where(e => e.MessageStatus.ToLower().Contains(pagingInfo.Search.ToLower()) || e.Message.ToLower().Contains(pagingInfo.Search.ToLower()) || (e.MessageID != null ? (e.MessageID.ToLower().Contains(pagingInfo.Search.ToLower())) : false) || e.RecipientsNumber.Contains(pagingInfo.Search) || (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)).OrderByDescending(e => e.SentDateTime);
                                    var CampaignLogSearch = CampaignLogDTOList.Where(e => e.MessageStatus.ToLower().Contains(pagingInfo.Search.ToLower()) || e.RecipientsNumber.Contains(pagingInfo.Search) || (e.SentDateTime.ToString() != null ? (Convert.ToDateTime(e.SentDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).OrderByDescending(e => e.SentDateTime); //|| (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)|| e.Message.ToLower().Contains(pagingInfo.Search.ToLower())|| (e.MessageID != null ? (e.MessageID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)|| (e.CampaignName != null ? (e.CampaignName.ToLower().Contains(search.ToLower())) : false)

                                    if (CampaignLogSearch != null)
                                    {
                                        foreach (var itemsearch in CampaignLogSearch)
                                        {

                                            CampaignLogDTOSearchList.Add(itemsearch);
                                        }
                                    }
                                    return CampaignLogDTOSearchList.Count();

                                }
                                else
                                {
                                    List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
                                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                                    var CampaignLogSearch = CampaignLogDTOList.Where(e => e.SentDateTime >= date && e.SentDateTime < date.AddDays(1)).OrderByDescending(e => e.SentDateTime);
                                    if (CampaignLogSearch != null)
                                    {
                                        foreach (var itemsearch in CampaignLogSearch)
                                        {

                                            CampaignLogDTOSearchList.Add(itemsearch);
                                        }
                                    }
                                    return CampaignLogDTOSearchList.Count();

                                }
                            }

                        }


                    }
                    //List<CampaignLogXMLDTO> CampaignLogXMLDTOSearchList = new List<CampaignLogXMLDTO>();
                }
                return CampaignLogDTOList.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  Campaign Log By CampaignId
        /// </summary>
        /// <param name="CampaignId">Campaign Id</param>
        /// <param name="pagingInfo">pagingInfo object</param>
        /// <returns>Returns list of campaign Log </returns>
        public static List<CampaignLogDTO> GetCampaignLogXMLByCampaignId(int CampaignId, PagingInfo pagingInfo)
        {
            List<CampaignLogXMLDTO> CampaignLogXMLDTOList = new List<CampaignLogXMLDTO>();

            List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
            CampaignDTO CampaignDTO = new CampaignDTO();
            CampaignDTO = CampaignService.GetById(CampaignId);
            try
            {
                UnitOfWork uow = new UnitOfWork();
                int skip = (pagingInfo.Page - 1) * pagingInfo.ItemsPerPage;
                int take = pagingInfo.ItemsPerPage;

                IEnumerable<CampaignLogXML> CampaignLogXML = uow.CampaignLogXMLRepo.GetAll().Where(e => e.CampaignId == CampaignId).ToList();

                if (CampaignLogXML != null)
                {
                    foreach (var item in CampaignLogXML)
                    {
                        CampaignLogXMLDTOList.Add(Transform.CampaignLogXMLToDTO(item));
                    }

                    if (CampaignLogXMLDTOList != null)
                    {
                        foreach (var item in CampaignLogXMLDTOList)
                        {
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["msgBlasterWebContext"].ToString());
                            con.Open();
                            SqlCommand command = new SqlCommand("Select XMLLog From CampaignLogXMLs WHERE Id=" + item.Id, con);

                            XmlDocument xdoc = new XmlDocument();
                            XmlReader reader = command.ExecuteXmlReader();

                            if (reader.Read())
                            {
                                xdoc.Load(reader);

                                //XmlNodeList msgList = xdoc.SelectNodes("/packet/numbers/message");
                                //XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");


                                foreach (XmlNode currentnode in xdoc.DocumentElement.GetElementsByTagName("numbers"))
                                {

                                    //


                                    XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");
                                    foreach (XmlNode node in numList)
                                    {
                                        CampaignLogDTO CampaignLogDTO = new CampaignLogDTO();

                                        CampaignLogDTO.RecipientsNumber = node.InnerText;// node.SelectNodes("number").Item(0).InnerText;// node.SelectSingleNode("number").InnerText;
                                        string MsgStatus = null;
                                        string code = null;

                                        if (node.Attributes != null)
                                        {
                                            code = node.Attributes["msgCode"].Value;
                                            CampaignLogDTO.SentDateTime = Convert.ToDateTime(node.Attributes["sentTime"].Value);
                                            CampaignLogDTO.MessageCount = Convert.ToInt32(node.Attributes["msgCount"].Value);
                                            CampaignLogDTO.RequiredCredits = Convert.ToDouble(node.Attributes["msgCredits"].Value);
                                            CampaignLogDTO.CampaignId = CampaignId;
                                            CampaignLogDTO.CampaignName = CampaignDTO.Name;
                                            CampaignLogDTO.CreatedOn = CampaignDTO.CreatedDate;
                                            CampaignLogDTO.CreatedBy = UserService.GetById(CampaignDTO.CreatedBy).Name;

                                        }

                                        MsgStatus = CodeDescription(code);
                                        CampaignLogDTO.MessageStatus = MsgStatus;
                                        //if (IsSuccessful == true)
                                        //{
                                        if (code == "1701" || code == "0")
                                        {
                                            CampaignLogDTOList.Add(CampaignLogDTO);
                                        }
                                        else
                                            //}
                                            //else if (code != "1701" && code != "0")
                                            //{
                                            CampaignLogDTOList.Add(CampaignLogDTO);
                                        // }

                                    }
                                }
                            }
                            con.Close();


                            if (pagingInfo.Search != "" && pagingInfo.Search != null)
                            {
                                bool Isdate = CommonService.IsDate(pagingInfo.Search);
                                if (Isdate != true)
                                {
                                    List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
                                    var CampaignLogSearch = CampaignLogDTOList.Where(e => e.MessageStatus.ToLower().Contains(pagingInfo.Search.ToLower()) || e.RecipientsNumber.Contains(pagingInfo.Search) || (e.SentDateTime.ToString() != null ? (Convert.ToDateTime(e.SentDateTime).ToString("dd-MMM-yyyy").ToLower().Contains(pagingInfo.Search.ToLower())) : false)).OrderByDescending(e => e.SentDateTime);//.Skip(skip).Take(take); //|| (e.GatewayID != null ? (e.GatewayID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)|| e.Message.ToLower().Contains(pagingInfo.Search.ToLower())|| (e.MessageID != null ? (e.MessageID.ToLower().Contains(pagingInfo.Search.ToLower())) : false)|| (e.CampaignName != null ? (e.CampaignName.ToLower().Contains(search.ToLower())) : false)
                                    if (CampaignLogSearch != null)
                                    {
                                        foreach (var itemsearch in CampaignLogSearch)
                                        {
                                            itemsearch.CampaignName = CampaignDTO.Name;
                                            itemsearch.CreatedOn = CampaignDTO.CreatedDate;
                                            itemsearch.CreatedBy = UserService.GetById(CampaignDTO.CreatedBy).Name;

                                            CampaignLogDTOSearchList.Add(itemsearch);
                                        }
                                    }

                                    IQueryable<CampaignLogDTO> CampaignLogDTOQuarableSearch = CampaignLogDTOSearchList.AsQueryable();
                                    return CampaignLogDTOQuarableSearch.Skip(skip).Take(take).ToList();

                                }
                                else
                                {
                                    List<CampaignLogDTO> CampaignLogDTOSearchList = new List<CampaignLogDTO>();
                                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                                    var CampaignLogSearch = CampaignLogDTOList.Where(e => e.SentDateTime >= date && e.SentDateTime < date.AddDays(1)).OrderByDescending(e => e.SentDateTime);//.Skip(skip).Take(take);
                                    if (CampaignLogSearch != null)
                                    {
                                        foreach (var itemsearch in CampaignLogSearch)
                                        {
                                            itemsearch.CampaignName = CampaignDTO.Name;
                                            itemsearch.CreatedOn = CampaignDTO.CreatedDate;
                                            itemsearch.CreatedBy = UserService.GetById(CampaignDTO.CreatedBy).Name;
                                            CampaignLogDTOSearchList.Add(itemsearch);
                                        }
                                    }
                                    IQueryable<CampaignLogDTO> CampaignLogDTOQuarableDate = CampaignLogDTOSearchList.AsQueryable();
                                    return CampaignLogDTOQuarableDate.Skip(skip).Take(take).ToList();

                                }
                            }

                        }

                    }


                }
                //List<CampaignLogXMLDTO> CampaignLogXMLDTOSearchList = new List<CampaignLogXMLDTO>();                

                IQueryable<CampaignLogDTO> CampaignLogDTOQuarableall = CampaignLogDTOList.AsQueryable();
                CampaignLogDTOQuarableall = PagingService.Sorting<CampaignLogDTO>(CampaignLogDTOQuarableall, pagingInfo.SortBy, pagingInfo.Reverse);
                return CampaignLogDTOQuarableall.Skip(skip).Take(take).ToList();

            }

            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Campaign Log Paged List by CampaignId
        /// </summary>
        /// <param name="CampaignId">Campaign Id</param>
        /// <param name="pagingInfo">pagingInfo object</param>
        /// <returns>Returns paged data list of Campaign log</returns>
        public static PageData<CampaignLogDTO> GetCampaignLogPagedListbyCampaignId(int CampaignId, PagingInfo pagingInfo)
        {
            List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();
            PageData<CampaignLogDTO> pageList = new PageData<CampaignLogDTO>();

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
            CampaignLogDTOList = GetCampaignLogXMLByCampaignId(CampaignId, pagingInfo);
            IQueryable<CampaignLogDTO> CampaignLogDTOPagedList = CampaignLogDTOList.AsQueryable();

            UnitOfWork uow = new UnitOfWork();
            int count = 0;

            if (pagingInfo.Search != "" && pagingInfo.Search != null)
            {
                bool IsDate = CommonService.IsDate(pagingInfo.Search);
                if (IsDate != true)
                {
                    count = 0;
                    count = GetCampaignLogXMLPagedCountByCampaignId(CampaignId, pagingInfo);
                }
                else
                {
                    DateTime date = Convert.ToDateTime(pagingInfo.Search);
                    count = 0;
                    count = GetCampaignLogXMLPagedCountByCampaignId(CampaignId, pagingInfo);

                }

            }
            else
            {
                count = GetCampaignLogXMLPagedCountByCampaignId(CampaignId, pagingInfo);
            }


            ////Sorting
            // CampaignLogDTOPagedList = PagingService.Sorting<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.SortBy, pagingInfo.Reverse);

            // paging
            if (CampaignLogDTOPagedList.Count() > 0)
            {
                //var ContacDTOPerPage = PagingService.Paging<CampaignLogDTO>(CampaignLogDTOPagedList, pagingInfo.ItemsPerPage, pagingInfo.Page);
                pageList.Count = count;// CampaignLogDTOPagedList.Count();

                List<CampaignLogDTO> pagedCampaignLogDTOList = new List<CampaignLogDTO>();
                foreach (var item in CampaignLogDTOPagedList)
                {
                    CampaignDTO CampaignDTO = new CampaignDTO();
                    CampaignDTO = CampaignService.GetById(item.CampaignId);
                    item.CampaignName = CampaignDTO.Name;
                    item.CreatedBy = UserService.GetById(CampaignDTO.CreatedBy).Name;
                    pagedCampaignLogDTOList.Add(item);
                }
                pageList.Data = pagedCampaignLogDTOList;
                pageList.SuccessCount = GetCampaignLogXMLCountByCampaignId(CampaignId, true);
                pageList.FailureCount = GetCampaignLogXMLCountByCampaignId(CampaignId, false);
            }
            else
            {
                pageList.Data = null;
                pageList.SuccessCount = GetCampaignLogXMLCountByCampaignId(CampaignId, true);
                pageList.FailureCount = GetCampaignLogXMLCountByCampaignId(CampaignId, false);
            }



            return pageList;
        }

        #endregion        

        #region "Other Functionality"

        /// <summary>
        /// SMS code description
        /// </summary>
        /// <param name="code">Code</param>
        /// <returns>Returns description as per code in string </returns>
        public static string CodeDescription(string code)
        {
            string messageID;
            switch (code)
            {
                case "11":
                    messageID = "Invalid destination";
                    break;
                case "1701":
                    messageID = "Success";

                    break;
                case "1702":
                    messageID = "Invalid url error";
                    break;
                case "1703":
                    messageID = "Invalid value in username or password";
                    break;
                case "1704":
                    messageID = "Invalid value in type field";
                    break;
                case "1705":
                    messageID = "Invalid Message";

                    break;
                case "1706":
                    messageID = "Destination does not exist";

                    break;
                case "1707":
                    messageID = "Invalid source (Sender)";
                    break;
                case "1708":
                    messageID = "Invalid value for dlr field";
                    break;
                case "1709":
                    messageID = "User validation failed";
                    break;
                case "1710":
                    messageID = "Internal Error";
                    break;
                case "1025":
                    messageID = "Insufficient credits";
                    break;
                case "1032":
                    messageID = "Number is in DND";
                    break;
                default:
                    messageID = "Unknown Error";

                    break;
            }




            switch (code)
            {
                case "0":
                    messageID = "Successfully delivered";
                    break;
                case "10001":
                    messageID = "Username / Password incorrect";

                    break;
                case "10002":
                    messageID = "Contract expired";

                    break;
                case "10003":
                    messageID = "User Credit expired";

                    break;
                case "10004":
                    messageID = "User disabled";

                    break;
                case "10005":
                    messageID = "Service is temporarily unavailable";

                    break;
                case "10006":
                    messageID = "The specified message does not conform to DTD";

                    break;
                case "10007":
                    messageID = "unknown request";

                    break;
                case "10008":
                    messageID = "Invalid URL Error, This means that one of the parameters was not provided or left blank";

                    break;
                case "20001":
                    messageID = "Unknown number, Number not activated, Invalid destination number length. Destination number not numeric";

                    break;
                case "20002":
                    messageID = "Switched OFF or Out of Range";

                    break;
                case "20003":
                    messageID = "Message waiting list full, Handset memory full";

                    break;
                case "20004":
                    messageID = "Unknown equipment, Illegal equipment, Equipment not Short message equipped";

                    break;
                case "20005":
                    messageID = "System failure, Far end network error, No Paging response, Temporary network not available";

                    break;
                case "20006":
                    messageID = "Teleservice not provisioned, Call barred, Operator barring";

                    break;
                case "20007":
                    messageID = "Sender-ID mismatch";

                    break;
                case "20008":
                    messageID = "Dropped Messages";

                    break;
                case "20009":
                    messageID = "Number registered in NDNC";

                    break;
                case "20010":
                    messageID = "Destination number empty";

                    break;
                case "20011":
                    messageID = "Sender address empty";

                    break;
                case "20012":
                    messageID = "SMS over 160 character, Non-compliant message";

                    break;
                case "20013":
                    messageID = "UDH is invalid";

                    break;
                case "20014":
                    messageID = "Coding is invalid";

                    break;
                case "20015":
                    messageID = "SMS text is empty";

                    break;
                case "20016":
                    messageID = "Invalid Sender Id";

                    break;
                case "20017":
                    messageID = "Invalid message, Duplicate message ,Submit failed";

                    break;
                case "20018":
                    messageID = "Invalid Receiver ID (will validate Indian mobile numbers only.)";

                    break;
                case "20019":
                    messageID = "Invalid Date time for message Schedule (If the date specified in message post for schedule delivery is less than current date or more than expiry date or more than 1 year)";

                    break;
                case "20020":
                    messageID = "Message text is invalid";

                    break;
                case "20021":
                    messageID = "Aggregator Id mismatch for template and sender-ID";

                    break;
                case "20022":
                    messageID = "Noise Word Found in Promotional Message";

                    break;
                case "20023":
                    messageID = "Invalid Campaign";

                    break;
                case "40001":
                    messageID = "Message delivered successfully";

                    break;
                case "40002":
                    messageID = "Message failed";

                    break;
                case "40003":
                    messageID = "Message ID is invalid";

                    break;
                case "40004":
                    messageID = "Command Completed Successfully";

                    break;
                case "40005":
                    messageID = "HTTP disabled";

                    break;
                case "40006":
                    messageID = "Invalid Port";

                    break;
                case "40007":
                    messageID = "Invalid Expiry minutes";

                    break;
                case "40008":
                    messageID = "Invalid Customer Reference Id";

                    break;
                case "40009":
                    messageID = "Invalid Bill Reference Id";

                    break;
                case "40010":
                    messageID = "Email Delivery Disabled";

                    break;
                case "40011":
                    messageID = "HTTPS disabled";

                    break;
                case "40012":
                    messageID = "Invalid operator id";

                    break;
                case "50001":
                    messageID = "Cannot update/delete schedule since it has already been processed";

                    break;
                case "50002":
                    messageID = "Cannot update schedule since the new date-time parameter is incorrect";

                    break;
                case "50003":
                    messageID = "Invalid SMS ID/GUID";

                    break;
                case "50004":
                    messageID = "Invalid Status type for schedule search query. The status strings can be PROCESSED, PENDING and ERROR";

                    break;
                case "50005":
                    messageID = "Invalid date time parameter for schedule search query";

                    break;
                case "50006":
                    messageID = "Invalid GUID for GUID search query";

                    break;
                case "50007":
                    messageID = "Invalid command action";

                    break;
                case "60001":
                    messageID = "The number is in DND list";

                    break;
                case "60002":
                    messageID = "Insufficient Fund";

                    break;
                case "60003":
                    messageID = "Validity Expired";

                    break;
                case "60004":
                    messageID = "Credit back not required";

                    break;
                case "60005":
                    messageID = "Record is not there in accounts table";

                    break;
                case "60007":
                    messageID = "Message is accepted";

                    break;
                case "60008":
                    messageID = "Message validity period has expired";

                    break;
                case "99999":
                    messageID = "Unknown Error";

                    break;
                default:
                    messageID = "Error";
                    break;
            }

            return messageID;
        }

        /// <summary>
        /// Campaign log count by campaign Id
        /// </summary>
        /// <param name="CampaignId">Campaign Id</param>
        /// <param name="IsSuccessful">TRUE OR FALSE</param>
        /// <returns> Returns count of Campaign LOG</returns>
        public static int GetCampaignLogXMLCountByCampaignId(int CampaignId, bool IsSuccessful)
        {
            List<CampaignLogXMLDTO> CampaignLogXMLDTOList = new List<CampaignLogXMLDTO>();

            List<CampaignLogDTO> CampaignLogDTOList = new List<CampaignLogDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<CampaignLogXML> CampaignLogXML = uow.CampaignLogXMLRepo.GetAll().Where(e => e.CampaignId == CampaignId).ToList();
                if (CampaignLogXML != null)
                {
                    foreach (var item in CampaignLogXML)
                    {
                        CampaignLogXMLDTOList.Add(Transform.CampaignLogXMLToDTO(item));
                    }

                    if (CampaignLogXMLDTOList != null)
                    {
                        foreach (var item in CampaignLogXMLDTOList)
                        {
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["msgBlasterWebContext"].ToString());
                            con.Open();
                            SqlCommand command = new SqlCommand("Select XMLLog From CampaignLogXMLs WHERE Id=" + item.Id, con);

                            XmlDocument xdoc = new XmlDocument();
                            XmlReader reader = command.ExecuteXmlReader();

                            if (reader.Read())
                            {
                                xdoc.Load(reader);

                                //XmlNodeList msgList = xdoc.SelectNodes("/packet/numbers/message");
                                //XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");


                                foreach (XmlNode currentnode in xdoc.DocumentElement.GetElementsByTagName("numbers"))
                                {

                                    //


                                    XmlNodeList numList = xdoc.SelectNodes("/packet/numbers/number");
                                    foreach (XmlNode node in numList)
                                    {
                                        CampaignLogDTO CampaignLogDTO = new CampaignLogDTO();
                                        CampaignLogDTO.RecipientsNumber = node.InnerText;// node.SelectNodes("number").Item(0).InnerText;// node.SelectSingleNode("number").InnerText;
                                        string MsgStatus = null;
                                        string code = null;

                                        if (node.Attributes != null)
                                        {
                                            code = node.Attributes["msgCode"].Value;
                                            CampaignLogDTO.SentDateTime = Convert.ToDateTime(node.Attributes["sentTime"].Value);
                                            CampaignLogDTO.MessageCount = Convert.ToInt32(node.Attributes["msgCount"].Value);
                                            CampaignLogDTO.RequiredCredits = Convert.ToDouble(node.Attributes["msgCredits"].Value); 

                                        }

                                        MsgStatus = CodeDescription(code);
                                        CampaignLogDTO.MessageStatus = MsgStatus;
                                        if (IsSuccessful == true)
                                        {
                                            if (code == "1701" || code == "0")
                                            {
                                                CampaignLogDTOList.Add(CampaignLogDTO);
                                            }
                                        }
                                        else if (code != "1701" && code != "0")
                                        {
                                            CampaignLogDTOList.Add(CampaignLogDTO);
                                        }

                                    }
                                }

                            }
                            con.Close();                             

                        }


                    }
                    //List<CampaignLogXMLDTO> CampaignLogXMLDTOSearchList = new List<CampaignLogXMLDTO>();
                }
                return CampaignLogDTOList.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}

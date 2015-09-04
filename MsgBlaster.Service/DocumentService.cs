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
    public class DocumentService
    {
        #region "CRUD Functionality"

        //Create document
        public static int Create(DocumentDTO DocumentDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = DocumentDTO.ClientId;
                GlobalSettings.LoggedInUserId = DocumentDTO.UserId;
                int PartnerId = ClientService.GetById(DocumentDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId; 

                var Document = new Document();
                using (var uow = new UnitOfWork())
                {
                    Document = Transform.DocumentToDomain(DocumentDTO);
                    uow.DocumentRepo.Insert(Document);
                    uow.SaveChanges();
                    return (Document.Id);

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

        //Delete document by id and file path
        public static bool Delete(int Id, string FilePath)
        {
            bool IsDeleted = false;
            try
            {

                DocumentDTO DocumentDTO = new DocumentDTO();
                DocumentDTO = GetById(Id);
                GlobalSettings.LoggedInClientId = DocumentDTO.ClientId;
                GlobalSettings.LoggedInUserId = DocumentDTO.UserId;
                int PartnerId = ClientService.GetById(DocumentDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId; 

                IsDeleted = CommonService.RemoveDocument(FilePath); //+ DocumentDTO.Path
                if (IsDeleted != false)
                {
                    UnitOfWork uow = new UnitOfWork();
                    uow.DocumentRepo.Delete(Id);
                    uow.SaveChanges();
                }
                return IsDeleted;
            }
            catch
            {
                throw;
            }
        }

        //get document details by id
        public static DocumentDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Document Document = uow.DocumentRepo.GetById(Id);
                DocumentDTO DocumentDTO = Transform.DocumentToDTO(Document);
                return DocumentDTO;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "List Functionality"
        //Get document list by client id
        public static List<DocumentDTO> GetDocumentListByClientId(int ClientId)
        {

            List<DocumentDTO> DocumentDTOList = new List<DocumentDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Document> Document = uow.DocumentRepo.GetAll().Where(e => e.ClientId == ClientId);
                    if (Document != null)
                    {
                        foreach (var item in Document)
                        {
                            DocumentDTO DocumentDTO = new DocumentDTO();
                            DocumentDTO = Transform.DocumentToDTO(item);
                            UserDTO UserDTO = new UserDTO();
                            UserDTO = UserService.GetById(DocumentDTO.UserId);
                            DocumentDTO.User = UserDTO.Name;

                            ClientDTO ClientDTO = new ClientDTO();
                            ClientDTO = ClientService.GetById(DocumentDTO.ClientId);
                            DocumentDTO.Client = ClientDTO.Company;

                            DocumentDTOList.Add(DocumentDTO);
                        }
                    }
                }

                return DocumentDTOList;
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
        
        //Delete all documents by client id ad document folder path
        public static bool DeleteAllDocument(int ClientId, string DocumentFolderPath)
        {
            bool IsDeleted = false;
            try
            {
                List<DocumentDTO> DocumentDTOList = new List<DocumentDTO>();

                using (var uow = new UnitOfWork())
                {


                    IEnumerable<Document> Document = uow.DocumentRepo.GetAll().Where(e => e.ClientId == ClientId);
                    if (Document != null)
                    {
                        foreach (var item in Document)
                        {

                            GlobalSettings.LoggedInClientId = item.ClientId;
                            GlobalSettings.LoggedInUserId = item.UserId;
                            int PartnerId = ClientService.GetById(item.ClientId).PartnerId;
                            GlobalSettings.LoggedInPartnerId = PartnerId; 


                            IsDeleted = CommonService.RemoveDocument(DocumentFolderPath + item.Path);
                            if (IsDeleted == true)
                            {
                                uow.DocumentRepo.Delete(item.Id);
                                uow.SaveChanges();
                                IsDeleted = true;
                            }
                            else IsDeleted = false;
                        }

                        IsDeleted = CommonService.DeleteFolder(DocumentFolderPath + ClientId);
                    }

                    return IsDeleted;
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion
        
    }
}

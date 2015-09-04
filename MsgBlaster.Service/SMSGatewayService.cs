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
    public class SMSGatewayService
    {

        #region "Unwanted Code"
      
        //public static int Create(SMSGatewayDTO SMSGatewayDTO)
        //{
        //    try
        //    {
        //        var SMSGateway = new SMSGateway();
        //        using (var uow = new UnitOfWork())
        //        {
        //            SMSGateway = Transform.SMSGatewayToDomain(SMSGatewayDTO);
        //            uow.SMSGatewayRepo.Insert(SMSGateway);
        //            uow.SaveChanges();
        //            return (SMSGateway.Id);

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

        //public static void Edit(SMSGatewayDTO SMSGatewayDTO)
        //{
        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        SMSGateway SMSGateway = Transform.SMSGatewayToDomain(SMSGatewayDTO);
        //        uow.SMSGatewayRepo.Update(SMSGateway);
        //        uow.SaveChanges();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public static bool Delete(int Id)
        //{
        //    bool IsExists = IsChildEntityExist(Id);

        //    try
        //    {
        //        if (IsExists != true)
        //        {
        //            UnitOfWork uow = new UnitOfWork();
        //            uow.SMSGatewayRepo.Delete(Id);
        //            uow.SaveChanges();
        //            return true;
        //        }
        //        else return false;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public static SMSGatewayDTO GetById(int Id)
        //{
        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        SMSGateway SMSGateway = uow.SMSGatewayRepo.GetById(Id);
        //        SMSGatewayDTO SMSGatewayDTO = Transform.SMSGatewayToDTO(SMSGateway);
        //        return SMSGatewayDTO;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public static List<SMSGatewayDTO> GetSMSGatewayList()
        //{

        //    List<SMSGatewayDTO> SMSGatewayDTOList = new List<SMSGatewayDTO>();

        //    try
        //    {

        //        using (var uow = new UnitOfWork())
        //        {
        //            IEnumerable<SMSGateway> SMSGateway = uow.SMSGatewayRepo.GetAll().OrderBy(e => e.Name);
        //            if (SMSGateway != null)
        //            {
        //                foreach (var item in SMSGateway)
        //                {
        //                    SMSGatewayDTOList.Add(Transform.SMSGatewayToDTO(item));
        //                }
        //            }
        //        }

        //        return SMSGatewayDTOList;
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

        //public static bool IsChildEntityExist(int SMSGatewayId) // 
        //{
        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        IEnumerable<Client> Client = uow.ClientRepo.GetAll().Where(e => e.SMSGatewayId == SMSGatewayId); //&& e.ClientId == ClientId
        //        if (Client.ToList().Count > 0)
        //        {
        //            return true;
        //        }
        //        else return false;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public static bool GetSMSGatewayIsUniqueByName(string Name,  int Id)
        //{
        //    try
        //    {
        //        UnitOfWork uow = new UnitOfWork();
        //        IEnumerable<SMSGateway> SMSGateway = uow.SMSGatewayRepo.GetAll().Where(e => e.Name.ToLower() == Name.ToLower() && e.Id != Id);
        //        //ClientDTO ClientDTO = Transform.ClientToDTO(Client);
        //        if (SMSGateway.ToList().Count > 0)
        //        {
        //            return true;
        //        }
        //        else return false;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
       
        #endregion
        
    }
}

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
    public class SettingService
    {
        #region "CRUD Functionality"

        //Edit settings
        public static void Edit(SettingDTO SettingDTO)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Setting Setting = Transform.SettingToDomain(SettingDTO);
                uow.SettingRepo.Update(Setting);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get settings details by id
        public static SettingDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Setting Setting = uow.SettingRepo.GetById(Id);
                SettingDTO SettingDTO = Transform.SettingToDTO(Setting);
                return SettingDTO;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "List Functionality"

        public static List<SettingDTO> GetSettingList()
        {

            List<SettingDTO> SettingDTOList = new List<SettingDTO>();

            try
            {

                using (var uow = new UnitOfWork())
                {
                    IEnumerable<Setting> Setting = uow.SettingRepo.GetAll();
                    if (Setting != null)
                    {
                        foreach (var item in Setting)
                        {
                            SettingDTOList.Add(Transform.SettingToDTO(item));
                        }
                    }
                }

                return SettingDTOList;
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
     
    }
}

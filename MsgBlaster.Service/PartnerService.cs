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
    public class PartnerService
    {

        #region "CRUD Functionality"
        
        //Edit partner
        public static void Edit(PartnerDTO PartnerDTO)
        {
            try
            {
                GlobalSettings.LoggedInPartnerId = PartnerDTO.Id;  

                UnitOfWork uow = new UnitOfWork();
                Partner Partner = Transform.PartnerToDomain(PartnerDTO);
                uow.PartnerRepo.Update(Partner);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get partner information by id
        public static PartnerDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Partner Partner = uow.PartnerRepo.GetById(Id);
                PartnerDTO PartnerDTO = Transform.PartnerToDTO(Partner);
                return PartnerDTO;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "Sign in Functionality"

        //Partner sign in by using email and password
        public static PartnerDTO SignIn(string Email, string Password)
        {
            try
            {
                PartnerDTO PartnerDTO = new PartnerDTO();
                List<PartnerDTO> PartnerDTOList = new List<PartnerDTO>();

                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Partner> Partner = uow.PartnerRepo.GetAll().Where(e => e.Email.ToLower() == Email.ToLower() && e.Password == Password);
                if (Partner != null)
                {
                    foreach (var item in Partner)
                    {
                        PartnerDTO = Transform.PartnerToDTO(item);
                        GlobalSettings.LoggedInPartnerId = PartnerDTO.Id;
                    }
                }
                return PartnerDTO;
            }
            catch
            {
                throw;
            }
        }

        //Get login details by email
        public static bool ForgotPassword(string Email)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                PartnerDTO PartnerDTO = new PartnerDTO();
                IEnumerable<Partner> Partner = uow.PartnerRepo.GetAll().Where(e => e.Email.ToLower() == Email.ToLower());
                if (Partner.ToList().Count > 0)
                {
                    foreach (var item in Partner)
                    {
                        PartnerDTO = Transform.PartnerToDTO(item);
                    }
                    CommonService.SendEmail("msgBlaster Credentials", "Hello " + PartnerDTO.Name + ", <br/><br/> <p>Your msgBlaster username and password is as follows - </p> <br/> <table><tr><td> Username</td><td> = " + PartnerDTO.Email + "</td></tr><tr><td>Password</td><td> = " + PartnerDTO.Password + "</td></tr></table>", PartnerDTO.Email, "", false);
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
        
        #region "Other Functionality"

        //Check is mail is unique by email and partner id
        public static bool IsUniqueEmail(string Email, int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Partner> Partner = uow.PartnerRepo.GetAll().Where(e => e.Email.ToLower() == Email.ToLower() && e.Id != Id);
                if (Partner.ToList().Count > 0)
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

        //Check is mobile is unique by email and partner id
        public static bool IsUniqueMobile(string Mobile, int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Partner> Partner = uow.PartnerRepo.GetAll().Where(e => e.Mobile == Mobile && e.Id != Id);
                if (Partner.ToList().Count > 0)
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

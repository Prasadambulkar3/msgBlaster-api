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
    public class RegisterClientService
    {
        #region "Register Client"

        /// <summary>
        /// Register Client
        /// </summary>
        /// <param name="RegisterClientDTO">RegisterClientDTO object</param>
        /// <returns>Register Client details </returns>
        public static RegisterClientDTO RegisterClient(RegisterClientDTO RegisterClientDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = null;
                GlobalSettings.LoggedInUserId = null;
                GlobalSettings.LoggedInPartnerId = null;

                RegisterClientDTO.UserType = "Admin";
                RegisterClientDTO RegisterClientDTONew = new RegisterClientDTO();

                ClientDTO ClientDTO = new ClientDTO();
                ClientDTO.Company = RegisterClientDTO.Company;
                ClientDTO.Address = RegisterClientDTO.Address;
                ClientDTO.IsDatabaseUploaded = false;

                ClientDTO ClientDTONew = new ClientDTO();
                ClientDTONew = ClientService.Create(ClientDTO);

                GlobalSettings.LoggedInClientId = ClientDTONew.Id;

                LocationDTO LocationDTO = new LocationDTO();
                LocationDTO.Name = RegisterClientDTO.Location;
                LocationDTO.ClientId = ClientDTONew.Id;
                int LocationId = 0;
                LocationId = LocationService.Create(LocationDTO);




                UserDTO UserDTO = new UserDTO();
                //UserDTO.Name = RegisterClientDTO.Name;
                UserDTO.FirstName = RegisterClientDTO.FirstName;
                UserDTO.LastName = RegisterClientDTO.LastName;
                UserDTO.Email = RegisterClientDTO.Email;
                UserDTO.Password = RegisterClientDTO.Password;
                UserDTO.Mobile = RegisterClientDTO.Mobile;

                UserDTO.LocationId = LocationId;
                UserDTO.ClientId = ClientDTONew.Id;


                UserDTO UserDTONew = new UserDTO();
                UserDTONew = UserService.Create(UserDTO);
                UserDTONew.UserType = "Admin";
                UserDTONew.UserAccessPrivileges = UserService.GetUserAccess(UserDTONew.UserType.ToString());
                GlobalSettings.LoggedInUserId = UserDTONew.Id;




                //Assign client values to Registerclient 
                RegisterClientDTONew.Address = ClientDTONew.Address;
                RegisterClientDTONew.ClientId = ClientDTONew.Id;
                RegisterClientDTONew.Company = ClientDTONew.Company;

                //Assign user values to Registerclient 
                RegisterClientDTONew.Email = UserDTONew.Email;
                RegisterClientDTONew.Mobile = UserDTONew.Mobile;
                //RegisterClientDTONew.Name = UserDTONew.Name;
                RegisterClientDTONew.FirstName = UserDTONew.FirstName;
                RegisterClientDTONew.LastName = UserDTONew.LastName;
                RegisterClientDTONew.Password = UserDTONew.Password;
                RegisterClientDTONew.Id = UserDTONew.Id;
                RegisterClientDTONew.UserAccessPrivileges = UserDTONew.UserAccessPrivileges;

                return RegisterClientDTONew;

            }
            catch (msgBlasterValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                //HttpContext.Current.Session["LoggedClient"] = null;
                //HttpContext.Current.Session["LoggedClientId"] = "0";
                throw;
            }

        }

        #endregion
       
    }
}

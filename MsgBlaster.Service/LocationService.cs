using MsgBlaster.Repo;
using MsgBlaster.Domain;
using MsgBlaster.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;


namespace MsgBlaster.Service
{
    public class LocationService
    {
        #region "CRUD Functionality"

        //Create location
        public static int Create(LocationDTO LocationDTO)
        {

            try
            {
                GlobalSettings.LoggedInClientId = LocationDTO.ClientId;                 
                int PartnerId = ClientService.GetById(LocationDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                var Location = new Location();

                UnitOfWork uow = new UnitOfWork();
                Location = Transform.LocationToDomain(LocationDTO);
                uow.LocationRepo.Insert(Location);

                uow.SaveChanges();
                Location.Id = Location.Id;
                return Location.Id;

            }

            catch (Exception)
            {
                throw;
            }
        }

        //Edit location
        public static void Edit(LocationDTO LocationDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = LocationDTO.ClientId;
                int PartnerId = ClientService.GetById(LocationDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                UnitOfWork uow = new UnitOfWork();
                Location Location = Transform.LocationToDomain(LocationDTO);
                uow.LocationRepo.Update(Location);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get location details by id
        public static LocationDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                Location Location = uow.LocationRepo.GetById(Id);
                LocationDTO LocationDTO = Transform.LocationToDTO(Location);
                return LocationDTO;
            }
            catch
            {
                throw;
            }
        }

        //Delete location by id
        public static bool Delete(int Id)
        {

            GlobalSettings.LoggedInClientId = LocationService.GetById(Id).ClientId;
            int PartnerId = ClientService.GetById(Convert.ToInt32( GlobalSettings.LoggedInClientId)).PartnerId;
            GlobalSettings.LoggedInPartnerId = PartnerId;

            bool IsExists = IsChildEntityExist(Id);
            try
            {
                if (IsExists != true)
                {
                    LocationDTO LocationDTO = new LocationDTO();
                    LocationDTO = GetById(Id);
                    UnitOfWork uow = new UnitOfWork();
                    uow.LocationRepo.Delete(Id);
                    uow.SaveChanges();
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

        #region "List Functionality"

        //Get location list by client id
        public static List<LocationDTO> GetListByClientId(int ClientId)
        {

            List<LocationDTO> LocationDTO = new List<LocationDTO>();

            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Location> Location = uow.LocationRepo.GetAll().Where(e => e.ClientId == ClientId).ToList();

                if (Location != null)
                {
                    foreach (var item in Location)
                    {
                        LocationDTO.Add(Transform.LocationToDTO(item));
                    }
                }

                return LocationDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region "Other Functionality"

        //Get flag for location is uniqure by location name, client id and location id
        public static bool GetLocationIsUniqueByNameAndClientId(string Name, int ClientId, int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Location> Location = uow.LocationRepo.GetAll().Where(e => e.Name.ToLower() == Name.ToLower() && e.ClientId == ClientId && e.Id != Id);
                //ClientDTO ClientDTO = Transform.ClientToDTO(Client);
                if (Location.ToList().Count > 0)
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

        //Check is child entity exist in user 
        public static bool IsChildEntityExist(int LocationId) //, int ClientId 
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<User> User = uow.UserRepo.GetAll().Where(e => e.LocationId == LocationId); //&& e.ClientId == ClientId
                if (User.ToList().Count > 0)
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

        //Get location count by location name and client id
        public static int GetByLocationName(string Location, int ClientId)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<Location> LocationList = uow.LocationRepo.GetAll().Where(e => e.Name.ToLower() == Location.ToLower() && e.ClientId == ClientId);

                if (Location.ToList().Count > 0)
                {
                    foreach (var item in LocationList)
                    {
                        LocationDTO LocationDTO = Transform.LocationToDTO(item);
                        return LocationDTO.Id;
                    }
                }

                return 0;
            }
            catch
            {
                throw;
            }
        }

        //Get location id in comma separated string by location name and client id
        public static string GetLocationIdarrayByName(string Location, int ClientId)
        {
            string LocationId = null;
            if (Location == null || Location == "") { return null; }
            try
            {


                if (Location != null)
                {
                    UnitOfWork uow = new UnitOfWork();
                    IEnumerable<Location> LocationList = uow.LocationRepo.GetAll().Where(e => e.Name.ToLower().Contains(Location.ToLower()) && e.ClientId == ClientId);

                    if (LocationList != null)
                    {
                        foreach (var item in LocationList)
                        {
                            LocationId = LocationId + item.Id.ToString() + ",";
                            //return UserId;
                        }
                        if (LocationId != null)
                        {
                            LocationId = LocationId.Remove(LocationId.LastIndexOf(','));
                        }

                        return LocationId;
                    }
                    else return LocationId;
                }
                else return LocationId;
            }
            catch
            {
                throw;
            }
        }

        #endregion


        

    }
}

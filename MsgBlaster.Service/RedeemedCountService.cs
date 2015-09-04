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
    public class RedeemedCountService
    {
       
        #region "CRUD Functionality"
        
        //Create redeemed Count
        public static int Create(RedeemedCountDTO RedeemedCountDTO)
        {

            try
            {
                var RedeemedCount = new RedeemedCount();

                GlobalSettings.LoggedInClientId = RedeemedCountDTO.ClientId;
                GlobalSettings.LoggedInUserId = RedeemedCountDTO.UserId;
                int PartnerId = ClientService.GetById(RedeemedCountDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                UnitOfWork uow = new UnitOfWork();
                RedeemedCount = Transform.RedeemedCountToDomain(RedeemedCountDTO);
                uow.RedeemedCountRepo.Insert(RedeemedCount);

                uow.SaveChanges();
                RedeemedCount.Id = RedeemedCount.Id;
                return RedeemedCount.Id;

            }

            catch (Exception)
            {
                throw;
            }
        }

        //Edit redeemed count
        public static void Edit(RedeemedCountDTO RedeemedCountDTO)
        {
            try
            {
                GlobalSettings.LoggedInClientId = RedeemedCountDTO.ClientId;
                GlobalSettings.LoggedInUserId = RedeemedCountDTO.UserId;
                int PartnerId = ClientService.GetById(RedeemedCountDTO.ClientId).PartnerId;
                GlobalSettings.LoggedInPartnerId = PartnerId;

                UnitOfWork uow = new UnitOfWork();
                RedeemedCount RedeemedCount = Transform.RedeemedCountToDomain(RedeemedCountDTO);
                uow.RedeemedCountRepo.Update(RedeemedCount);
                uow.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get redeemed count by id
        public static RedeemedCountDTO GetById(int Id)
        {
            try
            {
                UnitOfWork uow = new UnitOfWork();
                RedeemedCount RedeemedCount = uow.RedeemedCountRepo.GetById(Id);
                RedeemedCountDTO RedeemedCountDTO = Transform.RedeemedCountToDTO(RedeemedCount);
                return RedeemedCountDTO;
            }
            catch
            {
                throw;
            }
        }
        
        #endregion

        #region "List Functionality"

        //Get redeemed count list by user id and campaign id
        public static List<RedeemedCountDTO> GetByUserId(int UserId, int CampaignId)
        {
            List<RedeemedCountDTO> RedeemedCountDTOList = new List<RedeemedCountDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<RedeemedCount> RedeemedCount = uow.RedeemedCountRepo.GetAll().Where(e => e.UserId == UserId && e.EcouponCampaignId == CampaignId);
                if (RedeemedCount != null)
                {
                    foreach (var item in RedeemedCount)
                    {
                        EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                        EcouponCampaignDTO = EcouponCampaignService.GetById(item.EcouponCampaignId);
                        if (EcouponCampaignDTO.ExpiresOn > System.DateTime.Now)
                        {

                            RedeemedCountDTO RedeemedCountDTO = new RedeemedCountDTO();
                            RedeemedCountDTO = Transform.RedeemedCountToDTO(item);

                            UserDTO UserDTO = new UserDTO();
                            UserDTO = UserService.GetById(UserId);
                            RedeemedCountDTO.UserName = UserDTO.Name;
                            RedeemedCountDTO.Location = LocationService.GetById(UserDTO.LocationId).Name;
                            ClientDTO ClientDTO = new ClientDTO();
                            ClientDTO = ClientService.GetById(UserDTO.ClientId);
                            RedeemedCountDTO.ClientName = ClientDTO.Company;

                            RedeemedCountDTOList.Add(RedeemedCountDTO);
                        }
                    }
                }

                return RedeemedCountDTOList;
            }
            catch
            {
                //  throw;
                return RedeemedCountDTOList;
            }
        }

        //Get reedeemed count by clinet id and campaign id
        public static List<RedeemedCountDTO> GetByClientId(int ClientId, int CampaignId)
        {
            List<RedeemedCountDTO> RedeemedCountDTOList = new List<RedeemedCountDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<RedeemedCount> RedeemedCount = uow.RedeemedCountRepo.GetAll().Where(e => e.ClientId == ClientId && e.EcouponCampaignId == CampaignId);
                if (RedeemedCount != null)
                {
                    foreach (var item in RedeemedCount)
                    {
                        EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                        EcouponCampaignDTO = EcouponCampaignService.GetById(item.EcouponCampaignId);
                        if (EcouponCampaignDTO.ExpiresOn > System.DateTime.Now)
                        {

                            RedeemedCountDTO RedeemedCountDTO = new RedeemedCountDTO();
                            RedeemedCountDTO = Transform.RedeemedCountToDTO(item);

                            UserDTO UserDTO = new UserDTO();
                            UserDTO = UserService.GetById(item.UserId);
                            RedeemedCountDTO.UserName = UserDTO.Name;
                            RedeemedCountDTO.Location = LocationService.GetById(UserDTO.LocationId).Name;
                            ClientDTO ClientDTO = new ClientDTO();
                            ClientDTO = ClientService.GetById(ClientId);
                            RedeemedCountDTO.ClientName = ClientDTO.Company;
                            RedeemedCountDTO.CampaignName = EcouponCampaignDTO.Title;
                            RedeemedCountDTOList.Add(RedeemedCountDTO);
                        }
                    }
                }

                return RedeemedCountDTOList;
            }
            catch
            {
                //  throw;
                return RedeemedCountDTOList;
            }
        }

        //Get redeemed count list by client id and user id
        public static List<RedeemedCountDTO> GetByClientIdandUserId(int ClientId, int CampaignId, int UserId)
        {
            List<RedeemedCountDTO> RedeemedCountDTOList = new List<RedeemedCountDTO>();
            try
            {
                UnitOfWork uow = new UnitOfWork();
                IEnumerable<RedeemedCount> RedeemedCount = uow.RedeemedCountRepo.GetAll().Where(e => e.ClientId == ClientId && e.EcouponCampaignId == CampaignId && e.UserId == UserId);
                if (RedeemedCount != null)
                {
                    foreach (var item in RedeemedCount)
                    {
                        EcouponCampaignDTO EcouponCampaignDTO = new EcouponCampaignDTO();
                        EcouponCampaignDTO = EcouponCampaignService.GetById(item.EcouponCampaignId);
                        if (EcouponCampaignDTO.ExpiresOn > System.DateTime.Now)
                        {

                            RedeemedCountDTO RedeemedCountDTO = new RedeemedCountDTO();
                            RedeemedCountDTO = Transform.RedeemedCountToDTO(item);

                            UserDTO UserDTO = new UserDTO();
                            UserDTO = UserService.GetById(item.UserId);
                            RedeemedCountDTO.UserName = UserDTO.Name;
                            RedeemedCountDTO.Location = LocationService.GetById(UserDTO.LocationId).Name;
                            ClientDTO ClientDTO = new ClientDTO();
                            ClientDTO = ClientService.GetById(ClientId);
                            RedeemedCountDTO.ClientName = ClientDTO.Company;
                            RedeemedCountDTO.CampaignName = EcouponCampaignDTO.Title;
                            RedeemedCountDTOList.Add(RedeemedCountDTO);
                        }
                    }
                }

                return RedeemedCountDTOList;
            }
            catch
            {
                //  throw;
                return RedeemedCountDTOList;
            }
        }

        //Get location wise redeemed count by user id
        public static List<LocationWiseRedeemedCountDTO> GetLocationGroupWiseCountByUserId(int UserId)
        {
            List<RedeemedCountDTO> RedeemedCountDTOList = new List<RedeemedCountDTO>();
            List<LocationWiseRedeemedCountDTO> LocationWiseRedeemedCountDTOList = new List<LocationWiseRedeemedCountDTO>();
            using (var uow = new UnitOfWork())
            {
                IEnumerable<RedeemedCount> RedeemedCount = uow.RedeemedCountRepo.GetAll().ToList();
                foreach (var item in RedeemedCount)
                {

                    RedeemedCountDTOList = GetByUserId(UserId, item.EcouponCampaignId).OrderBy(e => e.Location).ToList();
                }
            }
            ////IEnumerable<RedeemedCount> RedeemedCount = GetByUserId(UserId).GroupBy(e => e.Location)
            ////    .Select(e => new { Location = e.Key, RedeemCount = e.Count() });




            if (RedeemedCountDTOList.Count != 0)
            {

                string PreviousLocation = "";
                foreach (var item in RedeemedCountDTOList)
                {


                    LocationWiseRedeemedCountDTO LocationWiseRedeemedCountDTO = new LocationWiseRedeemedCountDTO();


                    if (PreviousLocation != item.Location)
                    {
                        LocationWiseRedeemedCountDTO.RedeemCount = 0;
                        LocationWiseRedeemedCountDTO.Location = item.Location;
                        LocationWiseRedeemedCountDTO.RedeemCount = RedeemedCountDTOList.Where(e => e.Location == item.Location).Sum(e => e.RedeemCount); // item.RedeemCount + LocationWiseRedeemedCountDTO.RedeemCount;
                        LocationWiseRedeemedCountDTOList.Add(LocationWiseRedeemedCountDTO);
                    }


                    PreviousLocation = item.Location;



                }



            }
            return LocationWiseRedeemedCountDTOList;


        }

        //get location group wise redeemed count by client id
        public static List<LocationWiseRedeemedCountDTO> GetLocationGroupWiseCountByClientId(int ClientId)
        {
            List<RedeemedCountDTO> RedeemedCountDTOList = new List<RedeemedCountDTO>();
            List<LocationWiseRedeemedCountDTO> LocationWiseRedeemedCountDTOList = new List<LocationWiseRedeemedCountDTO>();

            //RedeemedCountDTOList = GetByClientId(ClientId).OrderBy(e => e.Location).ToList();

            ////IEnumerable<RedeemedCount> RedeemedCount = GetByUserId(UserId).GroupBy(e => e.Location)
            ////    .Select(e => new { Location = e.Key, RedeemCount = e.Count() });

            using (var uow = new UnitOfWork())
            {
                IEnumerable<RedeemedCount> RedeemedCount = uow.RedeemedCountRepo.GetAll().ToList();
                foreach (var item in RedeemedCount)
                {

                    RedeemedCountDTOList = GetByClientIdandUserId(ClientId, item.EcouponCampaignId, item.UserId).OrderBy(e => e.Location).ToList();

                    if (RedeemedCountDTOList.Count != 0)
                    {

                        string PreviousLocation = "";
                        foreach (var itemRedeemedCount in RedeemedCountDTOList)
                        {


                            LocationWiseRedeemedCountDTO LocationWiseRedeemedCountDTO = new LocationWiseRedeemedCountDTO();


                            if (PreviousLocation != itemRedeemedCount.Location)
                            {
                                LocationWiseRedeemedCountDTO.RedeemCount = 0;
                                LocationWiseRedeemedCountDTO.UserName = itemRedeemedCount.UserName;
                                LocationWiseRedeemedCountDTO.CampaignName = itemRedeemedCount.CampaignName;// EcouponCampaignService.GetById(item.EcouponCampaignId).Title;
                                LocationWiseRedeemedCountDTO.Location = itemRedeemedCount.Location;
                                LocationWiseRedeemedCountDTO.RedeemCount = RedeemedCountDTOList.Where(e => e.Location == itemRedeemedCount.Location).Sum(e => e.RedeemCount); // item.RedeemCount + LocationWiseRedeemedCountDTO.RedeemCount;
                                LocationWiseRedeemedCountDTOList.Add(LocationWiseRedeemedCountDTO);
                            }


                            PreviousLocation = itemRedeemedCount.Location;



                        }



                    }

                }
            }


            return LocationWiseRedeemedCountDTOList;


        }

        #endregion
       
    }
}

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Collections;
using System.Data;

namespace MsgBlaster.DTO
{
    public class MsgInformationDTO
    {
        #region "Declaration"

        private int _clientId;
        private int _campaignId;
        private string _xmlpacket;
        private int _mRequiredCredits;
        #endregion

        #region "Properties"
        public int RequiredCredits
        {
            get { return _mRequiredCredits; }
            set { _mRequiredCredits = value; }
        }
        public string xmlpacket
        {
            get { return _xmlpacket; }
            set { _xmlpacket = value; }
        }

        public int ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }
        public int CampaignId
        {
            get { return _campaignId; }
            set { _campaignId = value; }
        }

        #endregion
    }
}

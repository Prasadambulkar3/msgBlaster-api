using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;
using MsgBlaster.Service;


namespace msgBlasterCampaignSendbywihz
{
    class Program
    {
        static void Main(string[] args)
        {
            CampaignPacketService.CreatePacket();

            QueueProcess _oQueueProcess = new QueueProcess();
            _oQueueProcess.SendMessages();
        }
    }
}

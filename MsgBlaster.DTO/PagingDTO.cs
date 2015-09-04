using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.Service
{
    public class PageData<T>
    {
        public int Count { get; set; }
        public List<T> Data { get; set; }

        public int? SuccessCount { get; set; }
        public int? FailureCount { get; set; }
    }

    public class PagingInfo
    {
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
        public string SortBy { get; set; }
        public bool Reverse { get; set; }
        public string Search { get; set; }
        public SearchCriteria[] AdvSearch { get; set; }
        public int TotalItem { get; set; }
        public int GroupId { get; set; }
        public int CampaignId { get; set; }
        public int EcouponCampaignId { get; set; }
        ////For Searching Purpose
        //public PatientSearch PatientSearch { get; set; }

       
    }

    public class SearchCriteria
    {

        public string criteria { get; set; }
        public string value { get; set; }
        public string columnName { get; set; }
        public string logic { get; set; }
        public string columnType { get; set; }
    }




}

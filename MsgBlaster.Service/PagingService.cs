using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO;
using MsgBlaster.Domain;
using MsgBlaster.Repo;
using System.Linq.Dynamic;
 
namespace MsgBlaster.Service
{
     public static class PagingService
    {
        ////Required System.Linq.Dynamic.dll
         public static IQueryable<T> Sorting<T>(IQueryable<T> sortObject, string columnName, bool isReverse)
         {
             return sortObject.OrderBy(columnName + (isReverse ? " descending" : ""));
         }

         public static IQueryable<T> Paging<T>(IQueryable<T> pagingObject, int itemPerPage, int pageNumber)
         {
             return pagingObject = pagingObject.Skip((pageNumber - 1) * itemPerPage).Take(itemPerPage);
         }

        
    }
}

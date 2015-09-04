using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class PlanDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public bool IsEcoupon { get; set; }
        public double? Tax { get; set; }
    }
}

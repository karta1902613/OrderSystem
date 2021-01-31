using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Model
{
    public class QueryShop
    {        
        public string shopName { get; set; }
    }
    public class UpdateShop
    {
        public string shopId { get; set; }
        public string shopName { get; set; }
        public string shopTel { get; set; }
        public string shopAddr { get; set; }
        public string shopStatus { get; set; }
        public string isDeliver { get; set; }
        public string limitTime { get; set; }
        public string minCost { get; set; }
    }

}

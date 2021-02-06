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
    public class ShopData
    {
        public string shopId { get; set; }
        public string shopName { get; set; }
        public string shopTel { get; set; }
        public string shopAddr { get; set; }
        public string shopStatus { get; set; }
        public bool isDeliver { get; set; }
        public string limitTime { get; set; }
        public string minCost { get; set; }
        public string shopType { get; set; }
        public string statusId { get; set; }
        public string statusType { get; set; }        
        public string actUser { get; set; }
        public DateTime actTime { get; set; }
        public string creatUser { get; set; }
        public DateTime creatTime { get; set; }
    }    

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Store.Contract.Model
{
    public class AdjustmentModel
    {
    }

    public class StDetailsGoods
    {     
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string lot_no { get; set; }
        public decimal qty { get; set; }
        public decimal sec_qty { get; set; }
        public string mo_id { get; set; }
        public string vendor_name { get; set; }
        
    }

    
    public class st_adjustment_mostly
    {
        public string id { get; set; }
        public string department_id { get; set; }
        public string date { get; set; }
        public string mode { get; set; }
        public string handler { get; set; }
        public string remark { get; set; }
        public string state { get; set; }
        public string transfers_state { get; set; }
        public string update_count { get; set; }
        public string create_date { get; set; }
        public string create_by { get; set; }
        public string update_date { get; set; }
        public string update_by { get; set; }
        public string adjust_reason { get; set; }
        public string servername { get; set; }
        public string check_by { get; set; }
        public string check_date { get; set; }
        public string head_status { get; set; }

    }
   
    public class st_a_subordination
    {
        public string id { get; set; }
        public string sequence_id { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string color { get; set; }
        public string location { get; set; }
        public string carton_code { get; set; }
        public string unit { get; set; }
        public decimal qty { get; set; }
        public decimal ib_amount { get; set; }
        public decimal price { get; set; }
        public string transfers_state { get; set; }
        public string sec_unit { get; set; }
        public decimal sec_qty { get; set; }
        public decimal ib_weight { get; set; }
        public string lot_no { get; set; }
        public string remark { get; set; }
        public string row_status { get; set; }

       
    }
}

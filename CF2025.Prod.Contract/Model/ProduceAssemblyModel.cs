using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Prod.Contract
{
    public class jo_assembly_mostly
    {
        public string id { get; set; }
        public string con_date { get; set; }
        public string bill_origin { get; set; }
        public string bill_type_no { get; set; }
        public string handover_id { get; set; }
        public string out_dept { get; set; }
        public string in_dept { get; set; }
        public string handler { get; set; }
        public string remark { get; set; }
        public string create_by { get; set; }
        public string update_by { get; set; }
        public string check_by { get; set; }
        public string update_count { get; set; }
        public string create_date { get; set; }
        public string update_date { get; set; }
        public string check_date { get; set;}
        public string state { get; set; }
        public string stock_type { get; set; }        
        public string head_status { get; set; }
    }

    public class jo_assembly_details
    {
        public string id { get; set; }
        public string sequence_id { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public decimal con_qty { get; set; }
        public string unit_code { get; set; }
        public decimal sec_qty { get; set; }
        public string sec_unit { get; set; }
        public string lot_no { get; set; }
        public decimal package_num { get; set; }
        public string color_name { get; set; }
        public string four_color { get; set; }
        public string app_supply_side { get; set; }
        public string remark { get; set; }
        public string return_qty_nonce { get; set; }
        public string sign_by { get; set; }
        public string sign_date { get; set; }
        public string location { get; set; }
        public string carton_code { get; set; }
        public int prd_id { get; set; }
        public string jo_id { get; set; }
        public string jo_sequence_id { get; set; }
        public string aim_jo_id { get; set; }
        public string aim_jo_sequence { get; set; }        
        public decimal qc_qty { get; set; }
        public string qc_result { get; set; }
        public string return_reason { get; set; }
        public string row_status { get; set; }
    }

    public class jo_assembly_details_part
    {

        public string id { get; set; }
        public string upper_sequence { get; set; }
        public string sequence_id { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public decimal con_qty { get; set; }
        public string unit_code { get; set; }
        public decimal sec_qty { get; set; }
        public string sec_unit { get; set; }       
        public decimal package_num { get; set; }
        public string remark { get; set; }
        public decimal bom_qty { get; set; }
        public decimal base_qty { get; set; }
        public string lot_no { get; set; }
        public string row_status { get; set; }
    }
    public class goods_id_for_plan
    {
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string location { get; set; }
        public decimal prod_qty { get; set; }
        public decimal qty { get; set; }
        public decimal sec_qty { get; set; }
        public string jo_id { get; set; }
        public string jo_sequence_id { get; set; }
    }
   
    public class details_mo_lot
    {
        public string lot_no { get; set; }
        public decimal qty { get; set; }
        public decimal sec_qty { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string vendor_name { get; set; }
        public string is_qc { get; set; }
    }

    public class check_part_stock
    {
        public string within_code { get; set; }
        public string out_dept { get; set; }
        public string upper_sequence { get; set; }
        public string sequence_id { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string lot_no { get; set; }
        public decimal con_qty { get; set; }
        public decimal sec_qty { get; set; }
        
    }

    public class assembly_qty
    {
        public decimal con_qty { get; set; }
        public decimal sec_qty { get; set; }
    }

    public class shadding_color_info
    {
        public decimal prod_qty { get; set; }
        public decimal sec_qty { get; set; }
        public decimal color_qty { get; set; }
        public string shading_color { get; set; }
        public string shading_color_state { get; set; }
        public string next_wp_id { get; set; }
        public decimal obligate_qty { get; set; }
       
    }


}

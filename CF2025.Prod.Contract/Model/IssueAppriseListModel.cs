using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Prod.Contract.Model
{
    public class query_condition
    {
        //倉庫發料查詢條件
        public string jo_id_s { get; set; }
        public string jo_id_e { get; set; }
        public string goods_id_s { get; set; }
        public string goods_id_e { get; set; }
        public string mo_id_s { get; set; }
        public string mo_id_e { get; set; }
        public string charge_dept_s { get; set; }
        public string charge_dept_e { get; set; }
        public string location_id { get; set; }
        public string four_color { get; set; }
        public string production_date_s { get; set; }
        public string production_date_e { get; set; }
        public string check_date_s { get; set; }
        public string check_date_e { get; set; }
    }

    public class query_data
    {
        //領料申請明細查詢
        public string id { get; set; }
        public string sequence_id { get; set; }
        public string ver { get; set; }
        public string materiel_id { get; set; }
        public string goods_id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string carton_code { get; set; }
        public string unit { get; set; }
        public string sec_unit { get; set; }
        public decimal fl_qty { get; set; }
        public decimal issues_qty { get; set; }
        public decimal sec_qty { get; set; }
        public decimal already_sec_qty { get; set; }
        public string color { get; set; }
        public string mo_id { get; set; }
        public string dept_id { get; set; }
        public string upper_sequence { get; set; }
        public string plate_effect { get; set; }
        public string outer_layer { get; set; }
        public string color_effect { get; set; }
        public string mrp_id { get; set; }
        public string location_name { get; set; }
        public string carton_name { get; set; }
        public string dept_name { get; set; }
        public string dept_name_en { get; set; }
        public string color_name { get; set; }
        public string unit_name { get; set; }
        public string plate_name { get; set; }
        public string outer_name { get; set; }
        public string color_effect_name { get; set; }
        public string SPID { get; set; }
        public string key_id { get; set; }//領料申請明細添加到倉庫發料單時,為方便與主表對應而增加此對應值
       // public string id1 { get; set; }

    }
    public class query_data_sub
    {
        //倉庫發料明細
        public string production_date { get; set; }
        public decimal fl_qty { get; set; }
        public decimal issues_qty { get; set; }
        public string unit { get; set; }
        public decimal sec_qty { get; set; }
        public decimal already_sec_qty { get; set; }
        public string sec_unit { get; set; }
        public string remark { get; set; }
        public string color { get; set; }
        public string mrp_id { get; set; }
        public string contract_cid { get; set; }
        public string so_order_id { get; set; }
        public string obligate_mo_id { get; set; }
        public string lot_no { get; set; }


        public string id { get; set; }
        public string ver { get; set; }
        public string upper_sequence { get; set; }
        public string sequence_id { get; set; }        
        public string materiel_id { get; set; }
        public string name { get; set; }
        public string basic_unit { get; set; }
        public decimal r_qty { get; set; }
        public decimal ir_qty { get; set; }
        public decimal base_qty { get; set; }
        public decimal wastage_percent { get; set; }
        public string so_sequence_id { get; set; }
        public string location { get; set; }
        public string carton_code { get; set; }
        public string mo_id { get; set; }
        public string dept_id { get; set; }
        public string key_id { get; set; }//領料申請明細添加到倉庫發料單時,為方便與主表對應而增加此對應值
        //public string id1 { get; set; }
        //public string sequence_id1 { get; set; }
    }

    public class st_inventory_mostly
    {
        //倉庫發料主檔
        public string id { get; set; }
        public string inventory_date { get; set; }
        public string origin { get; set; }
        public string state { get; set; }
        public string bill_type_no { get; set; }
        public string department_id { get; set; }
        public string linkman { get; set; }
        public string handler { get; set; }
        public string remark { get; set; }
        public string create_by { get; set; }
        public string create_date { get; set; }
        public string update_by { get; set; }
        public string update_date { get; set; }
        public string check_by { get; set; }
        public string check_date { get; set; }
        public string ii_location { get; set; }
        public string tum_type { get; set; }
        public string update_count { get; set; }
        public string transfers_state { get; set; }
        public string servername { get; set; }
        public string head_status { get; set; }

    }

    public class st_i_subordination
    {
        //倉庫發料明細一
        public string id { get; set; }
        public string sequence_id { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string goods_id_new { get; set; }
        public string goods_name { get; set; }
        public string goods_name_new { get; set; }
        public string color { get; set; }
        public string inventory_issuance { get; set; }
        public string ii_code { get; set; }
        public string ir_lot_no { get; set; }
        public string obligate_mo_id { get; set; }
        public decimal i_amount { get; set; }
        public decimal i_weight { get; set; }
        public string inventory_receipt { get; set; }
        public string ir_code { get; set; }
        public string ii_lot_no { get; set; }
        public string ref_lot_no { get; set; }
        public decimal ib_qty { get; set; }
        public decimal ib_weight { get; set; }
        public string unit { get; set; }
        public string sec_unit { get; set; }
        public string remark { get; set; }
        public string ref_id { get; set; }
        public string jo_sequence_id { get; set; }
        public string so_no { get; set; }
        public string contract_cid { get; set; }
        public string mrp_id { get; set; }
        public string sign_by { get; set; }
        public string sign_date { get; set; }
        public string vendor_id { get; set; }
        public string vendor_name { get; set; }

        public string base_unit { get; set; }
        public decimal rate { get; set; }
        public string state { get; set; }
        public string transfers_state { get; set; }
        public string ref_sequence_id { get; set; }
        public string only_detail { get; set; }

        public string row_status { get; set; }
    }


    public class st_cc_details_schedule
    {
        //倉庫發料明細表二
        public string id { get; set; }
        public string upper_sequence { get; set; }
        public string sequence_id { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; } //null
        public string color { get; set; }
        public string unit { get; set; }
        public string inventory_issuance { get; set; }
        public string ii_code { get; set; }
        public decimal i_amount { get; set; }
        public decimal i_weight { get; set; }
        public string ir_lot_no { get; set; }
        public string obligate_mo_id { get; set; }
        public string inventory_receipt { get; set; }
        public string ir_code { get; set; }
        public string remark { get; set; }
        public string ref_id { get; set; }
        public string ref_lot_no { get; set; }
        public string so_no { get; set; }
        public string contract_cid { get; set; }
        public string mrp_id { get; set; }

        public string base_unit { get; set; } //null
        public decimal rate { get; set; } //null
        public decimal ib_qty { get; set; } //null
        public decimal order_qty { get; set; }
        public string ii_lot_no { get; set; } //null
        public decimal average_cost { get; set; } //null
        public string state { get; set; }
        public string transfers_state { get; set; }
        public decimal ib_weight { get; set; } //null
        public decimal order_sec_qty { get; set; } //null
        public string so_sequence_id { get; set; }
        public string ref_sequence_id { get; set; }
        public string inventory_date { get; set; } //null
        public string department_id { get; set; } //null
        public string only_detail { get; set; }
        public string vendor_id { get; set; } //null
        public string servername { get; set; } //null

        public string row_status { get; set; }
    }
    //public class st_lot_no
    //{
    //    public string lot_no { get; set; }
    //    public decimal qty { get; set; }
    //    public decimal sec_qty { get; set; }
    //    public string mo_id { get; set; }
    //}
}

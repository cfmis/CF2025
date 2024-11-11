using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Store.Contract
{
    public class TransferOutUnconfirmModel
    {
    }
    //轉出未確認列表查詢
    public class TransferOutFind
    {
        public string id { get; set; }
        public string id_end { get; set; }
        public string mo_id { get; set; }
        public string mo_id_end { get; set; }
        public string goods_id { get; set; }
        public string goods_id_end { get; set; }
        public string transfer_date { get; set; }
        public string transfer_date_end { get; set; }
        public string goods_name { get; set; }
        public string location_id { get; set; }
        public string shelf { get; set; }
        public string customer_id { get; set; }
        public string customer_id_end { get; set; }
    }
    //轉出未確認列表
    public class TransferOutDetails
    {
        public string id { get; set; }
        public string transfer_date { get; set; }
        public string mo_id { get; set; }
        public string sequence_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string color { get; set; }
        public string unit { get; set; }
        public decimal transfer_amount { get; set; }
        public decimal al_transfer_amount { get; set; }
        public string sec_unit { get; set; }
        public decimal sec_qty { get; set; }
        public decimal package_num { get; set; }
        public decimal nwt { get; set; }
        public decimal gross_wt { get; set; }
        public string location_id { get; set; }        
        public string move_location_id { get; set; }
        public decimal inventory_qty { get; set; }
        public string lot_no { get; set; }
        public string remark { get; set; }
        
    }
    //轉入單主檔
    public class TransferInHead
    {
        public string id { get; set; }
        public string transfer_date { get; set; }
        public string department_id { get; set; }
        public string handler { get; set; }
        public string remark { get; set; }
        public string create_by { get; set; }
        public string create_date { get; set; }        
        public string update_by { get; set; }
        public string update_date { get; set; }        
        public string check_by { get; set; }
        public string check_date { get; set; }
        public string update_count { get; set; }        
        public string state { get; set; }        
        public string bill_type_no { get; set; }
        public string group_no { get; set; }
        public string head_status { get; set; }
    }
    //轉入單明細
    public class TransferInDetails
    {
        public string id { get; set; }      
        public string mo_id { get; set; }
        public string sequence_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string do_color { get; set; }
        public string unit { get; set; }
        public decimal transfer_amount { get; set; }
        public string sec_unit { get; set; }
        public decimal sec_qty { get; set; }
        public decimal package_num { get; set; }
        public decimal nwt { get; set; }
        public decimal gross_wt { get; set; }
        public string location_id { get; set; }
        public string carton_code { get; set; }
        public string shelf { get; set; }
        public string position_first { get; set; }
        public string move_location_id { get; set; }
        public string move_carton_code { get; set; }        
        public string lot_no { get; set; }
        public string remark { get; set; }
        public string transfer_out_id { get; set; }
        public string transfer_out_sequence_id { get; set; }
        public bool shipment_suit { get; set; }
        public string row_status { get; set; }
    }

    public class TransferDetailPart
    {
        public string id { get; set; }
        public string upper_sequence { get; set; }
        public string sequence_id { get; set; }
        public string goods_id { get; set; }
        public decimal jo_qty { get; set; }
        public decimal c_qty { get; set; }
        public decimal con_qty { get; set; }
        public string unit_code { get; set; }
        public string remark { get; set; }
        public string sec_unit { get; set; }
        public decimal sec_qty { get; set; }
        public string mo_id { get; set; }
        public string location { get; set; }
        public string carton_code { get; set; }
        public decimal bom_qty { get; set; }
        public decimal inventory_qty { get; set; }
        public decimal inventory_sec_qty { get; set; }
        public string lot_no { get; set; }
        public string row_status { get; set; }

        public string goods_name { get; set; }
        public string goods_unit { get; set; }
        public string mrp_id { get; set; }
        public decimal order_qty { get; set; }
        public decimal transfer_amount { get; set; }
        public decimal nostorage_qty { get; set; }
        public decimal obligate_qty { get; set; }
    }

    public class ApproveReturnData
    {
        public string id { get; set; }
        public string sequence_id { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public decimal qty { get; set; }
        public decimal sec_qty { get; set; }
        public decimal qty_out { get; set; }
        public decimal sec_qty_out { get; set; }
        public string approve_status { get; set; }
        public string error_info { get; set; }
        public string action_type { get; set; } 
        public string move_location_id { get; set; }
    }

    public class ItemLotNo
    {
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string lot_no { get; set; }
        public decimal qty { get; set; }
        public decimal sec_qty { get; set; }
        public string location_id { get; set; }
        public string do_color { get; set; }
        public decimal order_qty { get; set; }
        public decimal qty_rate { get; set; }
        public decimal dosage { get; set; }

    }

}

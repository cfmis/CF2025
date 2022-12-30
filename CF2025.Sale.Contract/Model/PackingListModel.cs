using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Sale.Contract
{
    public class PackingListModel
    {
        public string ID { get; set; }
        public int EditFlag { get; set; }
    }
    public class xl_packing_list_model : PackingListModel
    {
        public string packing_date { get; set; }
        public string invoice_id { get; set; }
        public string matter { get; set; }
        public string state { get; set; }
        public string packing_type { get; set; }
        public string customer_id { get; set; }
        public string customer { get; set; }
        public string linkman { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string per { get; set; }
        public string remark { get; set; }
        public string packing_list { get; set; }
        public string packing_list2 { get; set; }
        public string packing_list3 { get; set; }
        public string port_id { get; set; }
        public string ap_id { get; set; }
        public string destination { get; set; }
        public string create_by { get; set; }
        public string create_date { get; set; }
        public string update_by { get; set; }
        public string update_date { get; set; }
        public string messrs { get; set; }
        public string shippedby { get; set; }
        public string perss { get; set; }
        public string sailing_date { get; set; }
        public string contrainer_no { get; set; }
        public string order_id { get; set; }
        public string proceduce_area { get; set; }
        public string shipping_tool { get; set; }
        public string origin_id { get; set; }
        public string seal_no { get; set; }
        public string registration_mark { get; set; }
        public string type { get; set; }
        public decimal total_box_qty { get; set; }
        public string fake_name { get; set; }

    }
    public class xl_packing_list_details_model : PackingListModel
    {
        public string sequence_id { get; set; }
        public string po_no { get; set; }
        public string order_id { get; set; }
        public string mo_id { get; set; }
        public string item_no { get; set; }
        public string descript { get; set; }
        public string english_goods_name { get; set; }
        public string color { get; set; }
        public decimal pcs_qty { get; set; }
        public string unit_code { get; set; }
        public string box_no { get; set; }
        public string symbol { get; set; }
        public decimal pbox_qty { get; set; }
        public int box_qty { get; set; }
        public string carton_size { get; set; }
        public decimal cube_ctn { get; set; }
        public decimal total_cube { get; set; }
        public decimal nw_each { get; set; }
        public decimal gw_each { get; set; }
        public decimal tal_nw { get; set; }
        public decimal tal_gw { get; set; }
        public string ref_id { get; set; }
        public string remark { get; set; }
        public string tr_id { get; set; }
        public string mark_no { get; set; }
        public string so_sequence_id { get; set; }
        public string ref_sequence_id { get; set; }
        public decimal length { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public string spec { get; set; }
        public string sec_unit { get; set; }
        public decimal sec_qty { get; set; }
        public string Pl_id { get; set; }
        public string Pl_sequence_id { get; set; }
        public decimal pl_qty { get; set; }
        public string state { get; set; }
        public string tr_sequence_id { get; set; }
        public string shipment_suit { get; set; }

    }
    public class PackingListViewModel
    {
        public xl_packing_list_model xl_packing_list_mostly = new xl_packing_list_model();
        public List<xl_packing_list_details_model> xl_packing_list_details = new List<xl_packing_list_details_model>();
    }
}

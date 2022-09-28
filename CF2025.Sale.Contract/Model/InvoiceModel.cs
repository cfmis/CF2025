using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Sale.Contract
{
    public class InvoiceModel
    {
        public int EditFlag { get; set; }
        public string ID { get; set; }
        public int Ver { get; set; }
        public string state { get; set; }
        public string matter { get; set; }
        public decimal m_rate { get; set; }
        public decimal exchange_rate { get; set; }
    }
    public class so_invoice_mostly:InvoiceModel
    {
        public string oi_date { get; set; }
        public string separate { get; set; }
        public string Shop_no { get; set; }
        public string it_customer { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string payment_date { get; set; }
        public string linkman { get; set; }
        public string l_phone { get; set; }
        public string department_id { get; set; }
        public string email { get; set; }
        public string issues_wh { get; set; }
        public string bill_type_no { get; set; }
        public string merchandiser { get; set; }
        public string merchandiser_phone { get; set; }
        public string po_no { get; set; }
        public string shipping_methods { get; set; }
        public string seller_id { get; set; }
        public string m_id { get; set; }
        public decimal goods_sum { get; set; }
        public decimal other_fare { get; set; }
        public decimal disc_rate { get; set; }
        public decimal disc_amt { get; set; }
        public decimal disc_spare { get; set; }
        public decimal total_sum { get; set; }
        public string tax_ticket { get; set; }
        public decimal tax_sum { get; set; }
        public string amount { get; set; }
        public decimal other_fee { get; set; }
        public decimal total_package_num { get; set; }
        public string total_weight { get; set; }
        public string remark2 { get; set; }
        public string ship_remark { get; set; }
        public string ship_remark2 { get; set; }
        public string ship_remark3 { get; set; }
        public string remark { get; set; }
        public string p_id { get; set; }
        public string pc_id { get; set; }
        public string sm_id { get; set; }
        public string accounts { get; set; }
        public string per { get; set; }
        public string final_destination { get; set; }
        public string issues_state { get; set; }
        public string transport_style { get; set; }
        public string ship_date { get; set; }
        public string loading_port { get; set; }
        public string ap_id { get; set; }
        public string tranship_port { get; set; }
        public string finally_buyer { get; set; }
        public string mo_group { get; set; }
        public string packinglistno { get; set; }
        public int box_no { get; set; }
        public string create_by { get; set; }
        public string create_date { get; set; }
        public string update_by { get; set; }
        public string update_date { get; set; }
        public string check_date { get; set; }
        public string cd_disc { get; set; }
        public string area { get; set; }
        public string as_id { get; set; }
        public string bill_address { get; set; }
        public string flag { get; set; }
        public string confirm_status { get; set; }
        public string servername { get; set; }
        public string fake_name { get; set; }
        public string fake_bill_address { get; set; }
        public string fake_address { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        
    }
 
    public class so_invoice_details : InvoiceModel
    {
        public string sequence_id { get; set; }
        public string mo_id { get; set; }
        public string shipment_suit { get; set; }
        public string goods_id { get; set; }
        public string table_head { get; set; }
        public string goods_name { get; set; }
        public decimal u_invoice_qty { get; set; }
        public decimal ui_qty { get; set; }
        public string goods_unit { get; set; }
        public decimal sec_qty { get; set; }
        public string sec_unit { get; set; }
        public decimal invoice_price { get; set; }
        public string p_unit { get; set; }
        public decimal disc_rate { get; set; }
        public decimal disc_price { get; set; }
        public decimal total_sum { get; set; }
        public decimal disc_amt { get; set; }
        public string buy_id { get; set; }
        public string order_id { get; set; }
        public string issues_id { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string ncv { get; set; }
        public string is_print { get; set; }
        public string apprise_id { get; set; }
        public string is_free { get; set; }
        public string corresponding_code { get; set; }
        public decimal nwt { get; set; }
        public decimal gross_wt { get; set; }
        public decimal package_num { get; set; }
        public int box_no { get; set; }
        public string color { get; set; }
        public string spec { get; set; }
        public string subject_id { get; set; }
        public string contract_cid { get; set; }
        public string Delivery_Require { get; set; }
        public string location_id { get; set; }
        public string brand_category { get; set; }
        public string customer_test_id { get; set; }
        public string customer_goods { get; set; }
        public string customer_color_id { get; set; }
        public string big_class { get; set; }
        public string remark { get; set; }
        public int so_ver { get; set; }
        public string so_sequence_id { get; set; }
        public string so_bom_sequence { get; set; }
        public string carton_code { get; set; }
    }

    public class viewOc
    {
        public so_invoice_mostly ocMostly = new so_invoice_mostly();
        public so_invoice_details ocDetails = new so_invoice_details();
        public so_other_fare ocOtherFare = new so_other_fare();
    }
    public class viewInv
    {
        public so_invoice_mostly ocMostly = new so_invoice_mostly();
        public List<so_invoice_details> ocDetails = new List<so_invoice_details>();
        public List<so_other_fare> ocOtherFare = new List<so_other_fare>();
    }
    public class so_order_details: so_invoice_details
    {
        public string it_customer { get; set; }
        public string goods_ename { get; set; }
        public string add_remark { get; set; }
        public List<so_other_fare> ocOtherFare = new List<so_other_fare>();
    }
    public class so_other_fare
    {
        public int EditFlag { get; set; }
        public string sequence_id { get; set; }
        public string fare_id { get; set; }
        public string name { get; set; }
        public decimal tf_percent { get; set; }
        public string sum_kind { get; set; }
        public decimal qty { get; set; }
        public decimal price { get; set; }
        public decimal fare_sum { get; set; }
        public string mo_id { get; set; }
        public string mould_no { get; set; }
        public string remark { get; set; }
        public string is_free { get; set; }
    }

    public class so_invoice_report
    {
        public string within_code { get; set; }
        public string id { get; set; }
        public int ver { get; set; }
        public string sequence { get; set; }
        public string oi_date { get; set; }
        public string it_customer { get; set; }
        public string customer_name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string sellerman { get; set; }
        public string payment { get; set; }
        public string shop_no { get; set; }
        public string ship_date { get; set; }
        public string currency { get; set; }
        public string currency_sign { get; set; }
        public string customer_bill_no { get; set; }
        public string contract_cid { get; set; }
        public string mo_id { get; set; }
        public string goods_id { get; set; }
        public string goods_id_a { get; set; }
        public string goods_name { get; set; }
        public string goods_name_a { get; set; }
        public string order_id { get; set; }
        public decimal u_invoice_qty { get; set; }
        public string goods_unit { get; set; }
        public decimal invoice_price { get; set; }
        public string p_unit { get; set; }
        public decimal total_sum { get; set; }
        public string sort { get; set; }
        public string types { get; set; }
        public string types_desc { get; set; }
        public decimal total_amount { get; set; }
        public string table_head { get; set; }
        public string customer_goods { get; set; }
        public string customer_color_id { get; set; }
        public string linkman { get; set; }
        public decimal total_package_num { get; set; }
        public string is_free { get; set; }
        public string state { get; set; }
        public string package_unit { get; set; }
        public string total_weight { get; set; }
        public string transport_style { get; set; }       
        public string bill_address { get; set; }
        public string remark { get; set; }
        public string remark2 { get; set; }
        public string invoice_remark { get; set; }

        //出口發票用到的字段
        public string cust_info { get; set; }
        public string ship_remark { get; set; }
        public string ship_remark2 { get; set; }
        public string ship_remark3 { get; set; }
        public string goods { get; set; }
        public string size { get; set; }
        public string unit_code { get; set; }
        public string m_id { get; set; }
        public string amount_english { get; set; }
        public string po_no { get; set; }
        public string loading_name { get; set; }
        public string ap_name { get; set; }
        public string email { get; set; }
        public string l_phone { get; set; }
        public string shipping_methods { get; set; }
        public string per { get; set; }
        public string finally_buyer { get; set; }
        public string finally_buyer_name { get; set; }
        public string pc_id { get; set; }
        public string is_print { get; set; }
        public string invoice_date { get; set; }
        public string flag { get; set; }
        public string order_id_desc { get; set; }
        public string contract_cid_desc { get; set; }

    }

    public class so_invoice_ship_remark
    {
        public string within_code { get; set; }
        public string id { get; set; }
        public int ver { get; set; }
        public string ship_remark { get; set; }
        public string ship_remark2 { get; set; }
        public string ship_remark3 { get; set; }
    }

    public class view_so_invoice_mostly : InvoiceModel
    {
        public string oi_date { get; set; }
        public string sd_date { get; set; }
        public string return_state { get; set; }
        public string it_customer { get; set; }
        public string cust_cname { get; set; }
        public string separate { get; set; }
        public string sent_goods_state { get; set; }
        public string transport_style { get; set; }
        public string seller_id { get; set; }
        public string consignment_date { get; set; }
        public string create_by { get; set; }
        public string create_time { get; set; }
        public string check_by { get; set; }
        public string receipt_person { get; set; }
        public string receipted_date { get; set; }
    }

}

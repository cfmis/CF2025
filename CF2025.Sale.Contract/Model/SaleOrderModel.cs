using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Sale.Contract
{
    public class SaleOrderModel
    {
    }

    public class so_order_manage
    {
        public string id { get; set; } //編號
        public string order_date { get; set; } //訂單日期
        public string origin_id { get; set; }//訂單業源
        public string state { get; set; } //狀態
        public string type { get; set; } //類型
        public string incept_order_date { get; set; } //東莞接單日期
        public string agent { get; set; } //洋行編號
        public string area { get; set; }   //區域
        public string it_customer { get; set; }  //客戶編號
        public string seller_id { get; set; } //銷售員編號
        public string jiedan_person { get; set; }//接單員
        public string season { get; set; }	//季度
        public string linkman { get; set; } //聯繫人
        public string l_phone { get; set; } //聯繫人電話
        public string fax { get; set; } //傳真
        public string email { get; set; } //電郵地址
        public string merchandiser { get; set; } //跟單員
        public string merchandiser_phone { get; set; }//跟單員電話
        public string merchandiser_email { get; set; }  //跟單員電郵
        public string m_id { get; set; } //貨幣編號
        public string exchange_rate { get; set; } //比率
        public string port_id { get; set; } //發貨港口
        public string ap_id { get; set; } //目的港口
        public string contract_id { get; set; } //PO/NO
        public string p_id { get; set; } //付款方式
        public string pc_id { get; set; } //價格條件
        public string sm_id { get; set; } //裝運方式

        public decimal transport_rate { get; set; } //裝運費比率(%)
        public decimal disc_rate { get; set; } //折扣(%)
        public decimal disc_amt { get; set; } //折扣額
        public decimal disc_spare { get; set; } //折扣後金額
        public decimal insurance_rate { get; set; } //保險費比率(%)
        public decimal other_fare { get; set; } //附加費用合計
        public decimal tax_ticket { get; set; }//稅種編號
        public decimal tax_sum { get; set; } //稅款總額
        public decimal accounts { get; set; } //銀行賬號
        public decimal goods_sum { get; set; } //貨品金額
        public decimal total_sum { get; set; } //總金額

        public string ship_mark { get; set; }   //船嘜
        public string remark { get; set; }  //附帶條款
        public string create_by { get; set; }
        public string create_date { get; set; }
        public string update_by { get; set; }
        public string update_date { get; set; }

        public string name { get; set; }//客戶描述
        public string e_name { get; set; }  //客戶英文描述
        public string s_address { get; set; } //送貨地址
        public string customer_address { get; set; } //客戶地址
        public string country_id { get; set; } //國別
    }

    public class so_order_detail
    {
        public string within_code { get; set; }
        public string id { get; set; }
        public int ver { get; set; }
        public string sequence_id { get; set; }  //序號
        public string mo_type { get; set; }  //制單種類
        public string mo_dept { get; set; }  //mo部門
        public string mo_group { get; set; }//組別
        public string mo_id { get; set; }   //頁數
        public string mo_ver { get; set; }    //頁數版本
        public string goods_id { get; set; }    //貨品編碼
        public string table_head { get; set; }  //客戶款號
        public string cust_approve_state { get; set; }  //客戶批單狀態
        public string cust_approve_remark { get; set; } //客戶批單備註
        public string goods_name { get; set; }  //貨品名稱
        public string notices_id { get; set; }  //制單更改單號
        public string brand_id { get; set; }    //牌子編碼
        public string division { get; set; }    //Division
        public string office { get; set; }      //寫字樓
        public decimal order_qty { get; set; }   //訂單數量
        public string goods_unit { get; set; }  //單位
        public string location_id { get; set; } //倉庫
        public decimal disc_rate { get; set; }   //折扣(%)
        public decimal unit_price { get; set; }  //單價
        public string p_unit { get; set; }      //單價單位
        public string n_mo_id { get; set; }     //N單單號
        public decimal disc_amt { get; set; }    //折扣額
        public decimal total_sum { get; set; }   //總金額
        public string packing_method { get; set; }  //包裝方法
        public string shipment_flag { get; set; }   //出貨標識
        public string is_free { get; set; }   //是否免費
        public string isprint { get; set; }   //是否列印
        public string abnormal { get; set; }  //異常制單
        public string draw_id { get; set; } //畫稿編號
        public string is_provide_spm { get; set; }//是否提供SPM
        public string oro_test { get; set; }  //ORO測試
        public string customer_goods { get; set; }  //客戶產品編號
        public string customer_goods_name { get; set; } //客戶產品名稱
        public string country { get; set; } //國家
        public string spm_no { get; set; }  //SPM编号	
        public string state { get; set; }   //狀態
        public string customer_color_id { get; set; } //客戶顏色編號
        public string customer_color_name { get; set; } //客戶顏色名稱
        public string quotation_id { get; set; } //報價單編號
        public string contract_cid { get; set; } //PO/NO
        public string customer_size { get; set; }   //客戶尺寸
        public string brand_category { get; set; }  //牌子類別
        public string customer_test_id { get; set; }  //檢驗分類編號
        public string plan_complete { get; set; }   //計劃回港日期
        public string arrive_date { get; set; } //交貨日期
        public string factory_ship_out_date { get; set; }//交客日期
        public string actual_bto_hk_date { get; set; }   //實際回港日期
        public string actual_bto_hk_qty { get; set; }   //實際回港數量
        public string add_remark { get; set; }  //OC備注
        public string remark { get; set; }      //備注
        public string plate_remark { get; set; }  //電鍍/噴油備註
        public string get_color_sample { get; set; } //取色辦
    }
    public class so_order_special_info
    {
        public string within_code { get; set; }
        public string id { get; set; }
        public int ver { get; set; }
        public string upper_sequence { get; set; }
        public string sequence_id { get; set; }  //序號

        public string nickle_free { get; set; }  //無叻
        public string plumbum_free { get; set; }  //無鉛
        public string full_inspection_free { get; set; }  //全檢
        public string keep_ban_free { get; set; }  //跟辨




    }

    public class so_other_fares
    {

    }
}

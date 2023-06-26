using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CF.Core.Config;


namespace CF.SQLServer.DAL
{
    public class PubFunDAL
    {
        SQLHelper sh = new SQLHelper(CachedConfigContext.Current.DaoConfig.OA);//DaoConfig.OA:DGERP4;DaoConfig.CRM:CF2025
        DataTable tblStatus = new DataTable();
        string strStatus = "";
        string within_code = "0000";
                 
        /// <summary>
        /// 更新庫存
        /// </summary>
        /// <param name="as_type">触发器类型:I(Insert)/D(Delete)</param>
        /// <param name="as_action_id">交易类型</param>
        /// <param name="as_id">交易单号</param>
        /// <param name="as_sequence_id">(交易单序号,*表示全部的交易单明细)</param>
        /// <param name="as_check_date">单据交易日期</param>
        /// <param name="as_errtext">更新库存错误信息 </param>
        /// <returns>成功返回'00',失敗返回'-1'加出錯信息</returns>
        public string of_update_st_details(string as_type, string as_action_id, string as_id, string as_sequence_id, string as_check_date, string as_errtext)
        {           
            strStatus = "00";//返回的字串strStatus:-1失敗;00:成功
            string ls_action_id, ls_id, ls_sequence_id, ls_goods_id = "", ls_action_content, ls_unit_kind, ls_main_unit_kind;
            string ls_unit, ls_customer_id, ls_vendor_id, ls_sec_unit_kind, ls_sec_unit, ls_lot_no, ls_lot_remark, strSql = "";
            string ls_ii_location_id, ls_ii_code, ls_mo_id, ls_goods_name, ls_shelf, ls_lot_mo, ls_carton_code;
            decimal ldc_ib_qty, ldc_ii_qty = 0, ldc_sec_qty = 0, ldc_lot_qty = 0, ldc_lot_sec_qty = 0;
            int ll_rc = 0, ll_count = 0, ll_business = 0;            
            string strSqlTrans = " SET XACT_ABORT ON "+ " BEGIN TRANSACTION ";
            string strSqlCommit = " COMMIT TRANSACTION ";
            DataTable lds_details = new DataTable();
            //是否存在交易
            strSql = string.Format(
            @"Select A.action_id,A.id,A.sequence_id,A.goods_id,B.name As goods_name,A.ii_location_id,A.ii_code,
			Isnull(A.ii_qty,0) As ii_qty,Isnull(A.sec_qty,0) As sec_qty,A.sec_unit,A.ib_qty,A.unit,A.customer_id,A.vendor_id,A.lot_no,
			Isnull(A.mo_id,'') As mo_id,C.kind As unit_kind,D.kind As sec_unit_kind,E.kind As main_unit_kind,A.shelf,A.lot_remark,
            H.action_content	
            From st_business_record A WITH (NOLOCK) 
                Left Join it_goods B WITH (NOLOCK) On A.within_code = B.within_code And A.goods_id = B.id
				Left Join cd_units C WITH (NOLOCK) On A.within_code = C.within_code And A.unit = C.id
				Left Join cd_units D WITH (NOLOCK) On A.within_code = D.within_code And A.sec_unit = D.id
				Left Join cd_units E WITH (NOLOCK) On B.within_code = E.within_code And B.unit_code = E.id
				Left Join cd_productline F WITH (NOLOCK) On A.within_code = F.within_code And A.ii_location_id = F.id
				Left Join (Select T.id,Ltrim(Rtrim(T.content)) As action_content From sys_business_action T WITH (NOLOCK) Where T.language='1') H On A.action_id=H.id
            Where A.within_code ='{0}' And A.id ='{1}' And (A.sequence_id ='{2}' Or '{2}'='*')
		          And A.action_id ='{3}' And Convert(Char(19),A.check_date,121) = Convert(Char(19),'{4}',121)
            Order By A.id,A.sequence_id", within_code, as_id, as_sequence_id, as_action_id, as_check_date);            
            DataTable lds_st_business = new DataTable();
            lds_st_business = sh.ExecuteSqlReturnDataTable(strSql);
                 
            //--仓库仓位改成一样，即效果就是不考虑舱位，不管单据上的舱位2012-05-20
            //lds_st_business.Retrieve(as_within_code, as_id, as_sequence_id, as_action_id, adt_check_date)
                        
            if (lds_st_business.Rows.Count == 0)
            {                  
                strStatus = "-1" + "NO st_business_record details"; ;
                return strStatus;//沒有交易數據則直接退出                
            }
            //--
            for (ll_business = 0; ll_business < lds_st_business.Rows.Count; ll_business++)
            {
                ls_action_id = lds_st_business.Rows[ll_business]["action_id"].ToString();
                ls_id = lds_st_business.Rows[ll_business]["id"].ToString();
                ls_sequence_id = lds_st_business.Rows[ll_business]["sequence_id"].ToString();
                ls_goods_id = lds_st_business.Rows[ll_business]["goods_id"].ToString();
                ls_goods_name = lds_st_business.Rows[ll_business]["goods_name"].ToString();
                ls_ii_location_id = lds_st_business.Rows[ll_business]["ii_location_id"].ToString();
                ls_ii_code = lds_st_business.Rows[ll_business]["ii_code"].ToString();
                ldc_ii_qty = checkDecimal(lds_st_business.Rows[ll_business]["ii_qty"].ToString());// decimal.Parse(lds_st_business.Rows[ll_business]["ii_qty"].ToString());
                ldc_sec_qty = checkDecimal(lds_st_business.Rows[ll_business]["sec_qty"].ToString());
                ls_sec_unit = lds_st_business.Rows[ll_business]["sec_unit"].ToString();
                ldc_ib_qty = checkDecimal(lds_st_business.Rows[ll_business]["ib_qty"].ToString());
                ls_unit = lds_st_business.Rows[ll_business]["unit"].ToString();
                ls_customer_id = lds_st_business.Rows[ll_business]["customer_id"].ToString();
                ls_vendor_id = lds_st_business.Rows[ll_business]["vendor_id"].ToString();
                ls_lot_no = lds_st_business.Rows[ll_business]["lot_no"].ToString();
                ls_mo_id = lds_st_business.Rows[ll_business]["mo_id"].ToString();
                ls_unit_kind = lds_st_business.Rows[ll_business]["unit_kind"].ToString().Trim();
                ls_sec_unit_kind = lds_st_business.Rows[ll_business]["sec_unit_kind"].ToString().Trim();
                ls_main_unit_kind = lds_st_business.Rows[ll_business]["main_unit_kind"].ToString().Trim();
                ls_shelf = lds_st_business.Rows[ll_business]["shelf"].ToString().Trim();
                ls_lot_remark = lds_st_business.Rows[ll_business]["lot_remark"].ToString();
                ls_action_content = lds_st_business.Rows[ll_business]["action_content"].ToString();
                //--
                ls_goods_id = string.IsNullOrEmpty(ls_goods_id) ? "" : ls_goods_id;
                ls_goods_name = string.IsNullOrEmpty(ls_goods_name) ? "" : ls_goods_name;
                ls_customer_id = string.IsNullOrEmpty(ls_customer_id) ? "" : ls_customer_id;
                ls_vendor_id = string.IsNullOrEmpty(ls_vendor_id) ? "" : ls_vendor_id;
                ls_lot_no = string.IsNullOrEmpty(ls_lot_no) ? "" : ls_lot_no;
                ls_mo_id = string.IsNullOrEmpty(ls_mo_id) ? "" : ls_mo_id;
                ls_shelf = string.IsNullOrEmpty(ls_shelf) ? "" : ls_shelf;
                ls_lot_remark = string.IsNullOrEmpty(ls_lot_remark) ? "" : ls_lot_remark;
                //--
                //if(Isnull(ldc_ib_qty) Or ldc_ib_qty = 0 Then
                //    as_errtext = '*****' + ls_goods_id + ' HAVE NOT UNIT CONVERSION(需要添加单位转换) ***' + '******'
                //    Return - 1
                //End If
                //--当单位的性质和货品资料设定的单位性质一样时，数量需要转换2010-09-02 Jeff
                DataTable lds_change_unit = new DataTable();
                if (ls_unit_kind == ls_main_unit_kind)
                {
                    strSql = string.Format(@"select DBO.FN_CHANGE_UNITBYCV('{0}','{1}','{2}',{3},'{4}','{5}','{6}') as 'qty'",
                        within_code, ls_goods_id, ls_unit, ldc_ii_qty, ls_customer_id, ls_vendor_id, "/");
                    lds_change_unit = sh.ExecuteSqlReturnDataTable(strSql);
                    if (lds_change_unit.Rows.Count > 0)
                    {
                        ldc_ii_qty = checkDecimal(lds_change_unit.Rows[0]["qty"].ToString());
                    }
                }
                if (ls_sec_unit_kind == ls_main_unit_kind)
                {
                    strSql = string.Format(@"select DBO.FN_CHANGE_UNITBYCV('{0}','{1}','{2}',{3},'{4}','{5}','{6}') as 'qty'",
                        within_code, ls_goods_id, ls_sec_unit, ldc_sec_qty, ls_customer_id, ls_vendor_id, "/");
                    lds_change_unit = sh.ExecuteSqlReturnDataTable(strSql);
                    if (lds_change_unit.Rows.Count > 0)
                    {
                        ldc_sec_qty = checkDecimal(lds_change_unit.Rows[0]["qty"].ToString());
                    }
                }
                //--调整单调整负数的时候改变@action_content
                if (ls_action_id == "03" && (ldc_ii_qty < 0 || ldc_sec_qty < 0))
                {
                    ls_action_content = "-";
                }
                //--01:采购入仓；12：销售退货；15：生产回仓；07：生产退料
                switch (as_type)
                {
                    case "I":
                        ls_lot_remark = "";//清空变量
                        if (ls_action_content.Trim() == "+")
                        {
                            //--start 20081226 huangyun 当为入库时,系统自动分配批号
                            if (string.IsNullOrEmpty(ls_lot_no) || ls_lot_no.Length == 0) //--如果批号为空时,自动产生批号
                            {
                                ls_lot_no = ls_id + ls_sequence_id;
                                //--当然最好的话,这里还要回写到单据表中批号字段
                            }
                            ls_lot_remark = ls_lot_remark + ls_lot_no.Trim() + "/@/" + string.Format("{0:F2}", ldc_ii_qty) + "/@/" + string.Format("{0:F2}", ldc_sec_qty) + ";";
                            //--start 20121103 huangyun
                            //ll_count = lds_details.Retrieve(gs_company, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_mo_id)
                            strSql = get_string_of_lds_details(within_code, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_mo_id);
                            lds_details = sh.ExecuteSqlReturnDataTable(strSql);
                            ll_count = lds_details.Rows.Count;
                            if (ll_count == 0)
                            {
                                strSql = string.Format(
                                @" Insert Into st_details_lot(within_code,goods_id,goods_name,qty,sec_qty,location_id,carton_code,lot_no,mo_id,vendor_id,update_date,in_date,remark)
                                Values('{0}','{1}','{2}',{3},{4},'{5}','{6}','{7}','{8}','{9}', GETDATE(), GETDATE(),'{10}')",
                                within_code, ls_goods_id, ls_goods_name, ldc_ii_qty, ldc_sec_qty, ls_ii_location_id, ls_ii_code, ls_lot_no, ls_mo_id, ls_vendor_id,
                                string.IsNullOrEmpty(ls_shelf) ? ls_ii_code : ls_shelf);
                            }
                            else
                            {
                                strSql = string.Format(
                                @" Update st_details_lot WITH(ROWLOCK)
                                Set qty = IsNull(qty,0) + IsNull({0},0),sec_qty = Isnull(sec_qty,0) + Isnull({1},0),
							        update_date=GETDATE(),in_date=GETDATE(),remark =(Case '{2}' When '' Then remark Else '{2}' End)
					            Where within_code='{3}' And location_id ='{4}' And carton_code='{5}' And goods_id='{6}' And Isnull(lot_no,'')='{7}' And IsNull(mo_id,'')='{8}' ",
                                ldc_ii_qty, ldc_sec_qty, ls_shelf, within_code, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_mo_id);
                            }
                            //--
                            string result = sh.ExecuteSqlUpdate(strSqlTrans + strSql + strSqlCommit);
                            if (result != "")
                            {                                
                                strStatus ="-1"+ result + "\r\n" + "<st_details_lot>" + "\r\n";
                                return strStatus;
                            }
                            //--end 20121103
                        }// --end if ls_action_content ='+' Then

                        if (ls_action_content.Trim() == "-")
                        {
                            /*修改成如下代码jeff 2011-03-18*/
                            ldc_ii_qty = System.Math.Abs(ldc_ii_qty);//ABS(ldc_ii_qty)
                            ldc_sec_qty = System.Math.Abs(ldc_sec_qty);
                            ls_carton_code = ls_ii_code;//--考虑到发票（action_id =25）不会选择仓位,这里要自动选择仓位和批号
                            //--start 20091103 huangyun 有批号时，先按指定的批号出库.一般情况下指定批号时,都会在程序中判断是否有批号库存.所以这里直接出库,不用检查库存了
                            //--Jeff2012-07-28增加排序方法，批号和页数相同时优先
                            //--20131126 huangyun因为有可能仓库盘点只调整重量,所以需要包含数量为0的情况
                            //--重量这里可能会出现负数，要当做是0来考虑2012-05-28 
                            strSql = string.Format(
                            @"Select A.carton_code,A.lot_no,A.mo_id,Isnull(A.qty,0) As 'qty',(Case When Isnull(A.sec_qty,0) <0 Then 0 Else Isnull(A.sec_qty,0) End) As 'sec_qty'
                            From st_details_lot A WITH (NOLOCK)
                            Where A.within_code ='{0}' And A.location_id ='{1}' And (A.carton_code='{2}' Or '{3}' IN('11','25') Or Left(a.goods_id,3)='F0-')  
                                  And A.goods_id ='{4}' And (Isnull(A.qty,0)>0 Or Isnull(A.sec_qty,0) >0)
                            Order By (Case When (Isnull(A.lot_no,'') = '{5}' And Isnull(A.mo_id,'')='{6}') Then '0'
						                   When (Isnull(A.mo_id,'') = '{6}') Then '1'
						                   When (Isnull(A.lot_no,'') = '{5}') Then '2'
						                   When ((Isnull(A.mo_id,'') <>'' And Isnull(A.mo_id,'') <> '{6}')) Then '4'
				                      Else '3' End) Asc,A.update_date Asc,A.carton_code,A.lot_no Asc",
                            within_code, ls_ii_location_id, ls_ii_code, ls_action_id, ls_goods_id, ls_lot_no, ls_mo_id);
                            DataTable lds_details_lot1 = new DataTable();
                            lds_details_lot1 = sh.ExecuteSqlReturnDataTable(strSql);
                            for (ll_rc = 0; ll_rc < lds_details_lot1.Rows.Count; ll_rc++)
                            {
                                if (ldc_ii_qty == 0 && ldc_sec_qty == 0)
                                {
                                    continue;
                                }
                                //--
                                ls_ii_code = lds_details_lot1.Rows[ll_rc]["carton_code"].ToString();
                                ls_lot_no = lds_details_lot1.Rows[ll_rc]["lot_no"].ToString();
                                ls_lot_mo = lds_details_lot1.Rows[ll_rc]["mo_id"].ToString();
                                ldc_lot_qty = checkDecimal(lds_details_lot1.Rows[ll_rc]["qty"].ToString());
                                ldc_lot_sec_qty = checkDecimal(lds_details_lot1.Rows[ll_rc]["sec_qty"].ToString());
                                //--
                                if (ldc_ii_qty == 0) //不需要扣减数量,只需要扣减重量
                                {
                                    ldc_lot_qty = 0;
                                }
                                if (ldc_sec_qty == 0)//不需要扣减重量
                                {
                                    ldc_lot_sec_qty = 0;
                                }
                                //--
                                if (ldc_lot_qty > ldc_ii_qty)
                                {
                                    ldc_lot_qty = ldc_ii_qty;
                                    ldc_ii_qty = 0;
                                }
                                else
                                {
                                    ldc_ii_qty = ldc_ii_qty - ldc_lot_qty;
                                }
                                //---
                                if (ldc_lot_sec_qty > ldc_sec_qty)
                                {
                                    ldc_lot_sec_qty = ldc_sec_qty;
                                    ldc_sec_qty = 0;
                                }
                                else
                                {
                                    ldc_sec_qty = ldc_sec_qty - ldc_lot_sec_qty;
                                }
                                //--因为数量与重量并没有对应的比例关系，在批次中，我们是以数量为主要条件来判断的。
                                //--所以有可能会出现总批次汇总的重量不等于交易的总重量
                                //--20130128 huangyun Add 多个不同的批号的情况
                                ls_lot_remark = ls_lot_remark + ls_lot_no.Trim() + "/@/" + string.Format("{0:F2}", ldc_lot_qty) + "/@/" + string.Format("{0:F2}", ldc_lot_sec_qty) + ";";
                                //--start 20121103 huangyun
                                //ll_count = lds_details.Retrieve(gs_company, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_lot_mo)
                                strSql = string.Format(
                                @"Select A.carton_code,A.lot_no,A.mo_id,Isnull(A.qty, 0) As qty,(Case When Isnull(A.sec_qty, 0) < 0 Then 0 Else Isnull(A.sec_qty, 0) End) As sec_qty
                                From st_details_lot A WITH (NOLOCK)
                                Where within_code ='{0}' And location_id ='{1}' And carton_code ='{2}' And goods_id='{3}' And Isnull(lot_no,'')='{4}' And IsNull(mo_id,'') ='{5}'",
                                within_code, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_lot_mo);
                                lds_details = sh.ExecuteSqlReturnDataTable(strSql);
                                ll_count = lds_details.Rows.Count;                                
                                if (ll_count == 0)
                                {
                                    strSql = string.Format(
                                    @" Insert Into st_details_lot(within_code, goods_id,goods_name,qty,sec_qty,location_id,carton_code,lot_no, mo_id,vendor_id,out_date,remark)
                                    Values('{0}','{1}','{2}', -{3}, -{4},'{5}','{6}','{7}','{8}','{9}', GETDATE(),'{10}')",
                                           within_code, ls_goods_id, ls_goods_name, ldc_lot_qty, ldc_lot_sec_qty, ls_ii_location_id, ls_ii_code, ls_lot_no, ls_lot_mo,
                                           ls_vendor_id, string.IsNullOrEmpty(ls_shelf) ? ls_ii_code : ls_shelf);
                                }
                                else
                                {
                                    strSql = string.Format(
                                    @" Update st_details_lot WITH(ROWLOCK)
                                    Set qty = Case When (IsNull(qty,0)-IsNull( {0} ,0)) >0 Then (IsNull(qty,0)-IsNull( {0} ,0)) Else 0 End,
                                        sec_qty = Case When (Isnull(sec_qty,0)-Isnull( {1} ,0)) >0 Then (Isnull(sec_qty,0)-Isnull( {1} ,0)) Else 0 End,
                                        out_date= GETDATE(),
                                        remark = (Case '{2}' When '' Then remark Else '{2}' End)
                                    Where within_code='{3}' And location_id='{4}' And carton_code='{5}' And goods_id='{6}' And Isnull(lot_no,'')='{7}' And IsNull(mo_id,'')='{8}'",
                                    ldc_lot_qty, ldc_lot_sec_qty, ls_shelf, within_code, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_lot_mo);
                                }
                                string result = sh.ExecuteSqlUpdate(strSqlTrans + strSql + strSqlCommit);
                                //--
                                if (result != "")
                                {
                                    strStatus = "-1" + result + "\r\n" + "<st_details_lot>" + "\r\n";
                                    return strStatus;                                    
                                }
                                //--end 20121103
                            }// --end for() lds_details_lot1
                        }// --end if ls_action_content ='-' Then

                        //--start 20130128 huangyun将批号字符的信息回写到交易表中
                        //--如果交易表批号信息中只有一个批号,并且批号为空时,将批号回写到交易表中去
                        if (ls_lot_remark != "")
                        {
                            strSql = string.Format(
                            @" Update A WITH(ROWLOCK)
                            Set A.lot_remark ='{0}',A.lot_no=(Case When(CHARINDEX(';','{0}')=LEN('{0}')) Then Left('{0}',CHARINDEX('/@/','{0}')-1) Else A.lot_no End)
				            From st_business_record A
                            Where A.within_code ='{1}' And A.action_id ='{2}' And A.id = '{3}' And A.sequence_id = '{4}' And
                                  A.goods_id ='{5}' And Convert(Char(19), A.check_date,120) = Convert(Char(19),'{6}',120)",
                            ls_lot_remark, within_code, ls_action_id, ls_id, ls_sequence_id, ls_goods_id, as_check_date);
                            string result = sh.ExecuteSqlUpdate(strSqlTrans + strSql + strSqlCommit);
                            if (result != "")
                            {
                                strStatus = "-1" + result + "\r\n"+"<st_business_record>"+ "\r\n";
                                return strStatus;
                            }
                        }
                        //--end 20130128
                        break;//增加交易表时处理
                    case "D":
                        if (ls_action_content.Trim() == "+")
                        {
                            ls_carton_code = ls_ii_code;
                            ldc_ii_qty = System.Math.Abs(ldc_ii_qty);
                            ldc_sec_qty = System.Math.Abs(ldc_sec_qty);
                            //--Jeff2012-07-28增加排序方法，批号和页数相同时优先
                            //--发票、转出、转入单要加上过滤页数的条件 2012-09-10
                            //lds_details_lot2.Retrieve(gs_company, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_mo_id, ls_action_id, ls_id)                           
                            strSql = string.Format(
                            @"Select a.carton_code,A.lot_no,IsNull(A.mo_id,'') As 'mo_id',Isnull(A.qty,0) As 'qty',
                                (Case When Isnull(A.sec_qty,0) <0 Then 0 Else Isnull(A.sec_qty,0) End) As 'sec_qty'
                            From st_details_lot A WITH (NOLOCK)
                            Where A.within_code ='{0}' And A.location_id='{1}'
	                            And (Case When '{2}' In('25','45','46','47','48') Or ('{2}' = '11' And Left('{3}',3)<>'ADI') Then Isnull(A.mo_id,'') Else '{4}' End) = '{4}'
	                            And (A.carton_code ='{5}' Or '{2}' IN('11','25')) And A.goods_id ='{6}' And (Isnull(A.qty,0)>0 Or Isnull(A.sec_qty,0) >0)
                             Order By (Case When (Isnull(A.lot_no,'') ='{7}' And Isnull(A.mo_id,'') = '{4}') Then '0'
					                        When (Isnull(A.mo_id,'') = '{4}') Then '1'
					                        When (Isnull(A.lot_no,'') = '{7}') Then '2'
					                        When (Isnull(A.mo_id,'') <>'' And Isnull(A.mo_id,'')<>'{4}') Then '4' Else '3' 
                                       End ) Asc,A.update_date Asc,A.carton_code,A.lot_no Asc",
                            within_code, ls_ii_location_id, ls_action_id, ls_id, ls_mo_id, ls_ii_code, ls_goods_id, ls_lot_no);
                            DataTable lds_details_lot2 = new DataTable();
                            lds_details_lot2 = sh.ExecuteSqlReturnDataTable(strSql);
                            for (ll_rc = 0; ll_rc < lds_details_lot2.Rows.Count; ll_rc++)
                            {
                                if (ldc_ii_qty == 0 && ldc_sec_qty == 0)
                                {
                                    continue;
                                }
                                //--
                                ls_ii_code = lds_details_lot2.Rows[ll_rc]["carton_code"].ToString();
                                ls_lot_no = lds_details_lot2.Rows[ll_rc]["lot_no"].ToString();
                                ls_lot_mo = lds_details_lot2.Rows[ll_rc]["mo_id"].ToString();
                                ldc_lot_qty = checkDecimal(lds_details_lot2.Rows[ll_rc]["qty"].ToString());
                                ldc_lot_sec_qty = checkDecimal(lds_details_lot2.Rows[ll_rc]["sec_qty"].ToString());
                                //--
                                if (ldc_ii_qty == 0)//不需要扣减数量,只需要扣减重量
                                {
                                    ldc_lot_qty = 0;
                                }
                                if (ldc_sec_qty == 0)//不需要扣减重量
                                {
                                    ldc_lot_sec_qty = 0;
                                }
                                //--
                                if (ldc_lot_qty > ldc_ii_qty)
                                {
                                    ldc_lot_qty = ldc_ii_qty;
                                    ldc_ii_qty = 0;
                                }
                                else
                                {
                                    ldc_ii_qty = ldc_ii_qty - ldc_lot_qty;
                                }
                                //---
                                if (ldc_lot_sec_qty > ldc_sec_qty)
                                {
                                    ldc_lot_sec_qty = ldc_sec_qty;
                                    ldc_sec_qty = 0;
                                }
                                else
                                {
                                    ldc_sec_qty = ldc_sec_qty - ldc_lot_sec_qty;
                                }
                                //--反批准时一定会存在库存	
                                strSql = string.Format(
                                @" Update st_details_lot WITH(ROWLOCK)
                                Set qty = Case When (IsNull(qty,0)-IsNull( {0} ,0)) >0 Then (IsNull(qty,0)-IsNull( {0} ,0)) Else 0 End, 
                                    sec_qty = Case When (Isnull(sec_qty,0)-Isnull( {1} ,0)) >0 Then (Isnull(sec_qty,0)-Isnull( {1} ,0)) Else 0 End, 
                                    out_date = GETDATE(),
                                    remark = (Case '{2}' When '' Then remark Else '{2}' End)
                                Where within_code='{3}' And location_id='{4}' And carton_code='{5}' And goods_id='{6}' And Isnull(lot_no,'')='{7}' And IsNull(mo_id,'')='{8}'",
                                ldc_lot_qty, ldc_lot_sec_qty, ls_shelf, within_code, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_lot_mo);
                                //--
                                string result = sh.ExecuteSqlUpdate(strSqlTrans + strSql + strSqlCommit);
                                if (result != "")
                                {
                                    strStatus= "-1"+ result + "\r\n"+"<st_details_lot>"+ "\r\n";
                                    return strStatus;                                 
                                }
                            } //-- end for(...) of lds_details_lot2
                        } // --end if of ls_action_content =='+'
                        if (ls_action_content.Trim() == "-")
                        {
                            //--如果有多个批号时,需要使用fn_st_business_record_lot来进行拆分
                            //if(ls_lot_remark != "") && Pos(ls_lot_remark,";") != ls_lot_remark.Length)
                            if (ls_lot_remark != "" && (ls_lot_remark.IndexOf(";") + 1) != ls_lot_remark.Length)
                            {
                                ls_lot_no = "";
                            }
                            //--start 20130128 huangyun按不同的批号,反批准时仍然回到原来的批号中去的
                            //lds_details_business_lot.Retrieve(ls_lot_remark);                                                      
                            strSql = string.Format(@"Select A.lot_no,A.ii_qty,A.sec_qty From Dbo.fn_st_business_record_lot('',0,0,'{0}') A", ls_lot_remark);
                            DataTable lds_details_business_lot = new DataTable();
                            lds_details_business_lot = sh.ExecuteSqlReturnDataTable(strSql);
                            //--
                            for (ll_rc = 0; ll_rc < lds_details_business_lot.Rows.Count; ll_rc++)
                            {
                                ls_lot_no = lds_details_business_lot.Rows[ll_rc]["lot_no"].ToString();
                                ldc_ii_qty = checkDecimal(lds_details_business_lot.Rows[ll_rc]["ii_qty"].ToString());
                                ldc_sec_qty = checkDecimal(lds_details_business_lot.Rows[ll_rc]["sec_qty"].ToString());
                                //--
                                ldc_ii_qty = System.Math.Abs(ldc_ii_qty);
                                ldc_sec_qty = System.Math.Abs(ldc_sec_qty);
                                //--start 20121129 huangyun 往库存表中添加数据时,如果批号为空就自动生成一个批号
                                if (string.IsNullOrEmpty(ls_lot_no) || ls_lot_no.Length == 0)
                                {
                                    ls_lot_no = ls_id + ls_sequence_id;
                                }
                                //--end 20121129
                                //ll_count = lds_details.Retrieve(gs_company, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_mo_id)
                                strSql = get_string_of_lds_details(within_code, ls_ii_location_id, ls_ii_code, ls_goods_id, ls_lot_no, ls_mo_id);
                                lds_details = sh.ExecuteSqlReturnDataTable(strSql);
                                ll_count = lds_details.Rows.Count;                                
                                if (ll_count == 0)
                                {
                                    strSql = string.Format(
                                    @" Insert Into st_details_lot
                                    (within_code, goods_id, goods_name, qty, sec_qty, location_id, carton_code, lot_no, mo_id,vendor_id, update_date, in_date,remark)
                                    Values('{0}','{1}','{2}',{3},{4},'{5}','{6}','{7}','{8}','{9}',GETDATE(),GETDATE(),(Case '{10}' When '' Then '{11}' Else '{10}' End))",
                                    within_code,ls_goods_id, ls_goods_name,ldc_ii_qty,ldc_sec_qty,ls_ii_location_id,ls_ii_code,ls_lot_no,ls_mo_id, ls_vendor_id, ls_shelf,ls_ii_code);
                                }
                                else
                                {
                                    strSql = string.Format(
                                    @" Update st_details_lot WITH(ROWLOCK)
                                      Set qty = IsNull(qty, 0) + IsNull({0},0), sec_qty=Isnull(sec_qty, 0) + Isnull({1}, 0),update_date = GETDATE(),in_date = GETDATE(),
								          remark = (Case '{2}' When '' Then remark Else '{2}' End)
						              Where within_code='{3}' And location_id='{4}' And carton_code='{5}' And goods_id='{6}' And Isnull(lot_no,'')='{7}' And IsNull(mo_id,'')='{8}'",
                                    ldc_ii_qty,ldc_sec_qty,ls_shelf,within_code,ls_ii_location_id,ls_ii_code,ls_goods_id,ls_lot_no,ls_mo_id);
                                }
                                //--
                                string result = sh.ExecuteSqlUpdate(strSqlTrans + strSql + strSqlCommit);
                                if (result != "")
                                {                                    
                                    strStatus = "-1"+ result + "\r\n" + "<st_details_lot>"+ "\r\n";
                                    return strStatus;
                                }
                            } //--end of for(...lds_details_business_lot...)
                        } //--end if ls_action_content =="-"
                        break;
                } //--end swith 多條件分支選擇
            } //--end 最外層For循環
                                  
            return strStatus;
        } //--end function of_update_st_details


        /// <summary>
        /// 返回公共的SQL查詢語句
        /// </summary>
        /// <param name="as_within_code"></param>
        /// <param name="as_ii_location_id"></param>
        /// <param name="as_ii_code"></param>
        /// <param name="as_goods_id"></param>
        /// <param name="as_lot_no"></param>
        /// <param name="as_mo_id"></param>
        /// <returns></returns>
        private string get_string_of_lds_details(string as_within_code, string as_ii_location_id, string as_ii_code, string as_goods_id, string as_lot_no, string as_mo_id)
        {
            string result = string.Format(
                @"Select A.carton_code,A.lot_no,A.mo_id,Isnull(A.qty, 0) As qty,(Case When Isnull(A.sec_qty, 0) < 0 Then 0 Else Isnull(A.sec_qty, 0) End) As sec_qty
                From st_details_lot A WITH (NOLOCK)
                Where within_code ='{0}' And location_id ='{1}' And carton_code ='{2}' And goods_id='{3}' And Isnull(lot_no,'')='{4}' And IsNull(mo_id,'') ='{5}'",
                as_within_code, as_ii_location_id, as_ii_code, as_goods_id, as_lot_no, as_mo_id);
            return result;
        }

        /// <summary>
        /// 檢查當前日期是否可以反批準
        /// </summary>
        /// <param name="as_id">單據編號</param>
        /// <param name="as_window_id">GEO窗口代碼</param>
        /// <returns>返回整數值:0不可以反批準;1可以反批準</returns>
        public int wf_check_inventory_date(string as_id,string as_window_id)
        {
            int rtn = 0;
            int li_cnt = 0;
            string strSql = string.Format(
            @"Select Count(1) as counts 
		    From st_business_record a WITH (NOLOCK)
		    Where a.within_code ='{0}' And id ='{1}' And Convert(nvarchar(10),check_date,121)=Convert(nvarchar(10),getdate(),121)",
            within_code,as_id);
            DataTable dt = sh.ExecuteSqlReturnDataTable(strSql);
            li_cnt = int.Parse(dt.Rows[0]["counts"].ToString());
            if(li_cnt > 0)
            {
                rtn = 1;
            }
            switch (as_window_id)
            {
                case "w_storage_count":  //盘点
                    if (li_cnt > 0)
                    {
                        rtn = 1;
                        break;
                    }
                    //--start 20130110 huangyun 假如盘点没有差异时,是没有交易数据的,这个时候应该是当作正常的数据,允许反批准
                    strSql = string.Format(@"Select Count(1) From st_business_record a WITH(NOLOCK) Where a.within_code='{0}' and id='{1}'", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["counts"].ToString());
                    if (li_cnt == 0)
                    {
                        //如果没有当前交易数据,也允许反批准
                        rtn = 1;
                    }
                    //--end 20130110
                    break;
                case "w_st_adjustment": //调整                    
                    break;
                case "w_changestore_cc":
                case "w_changestore_cc_lot":
                case "w_changestore_cj":
                case "w_changestore_cd":
                case "w_changestore_ce": //仓库转仓                   
                    break;
                case "w_transfer_out":  //转出单                    
                    break;
                case "w_transfer_in":  //转入单                    
                    break;
                case "w_jo_bill_despose":  //倉庫報廢                    
                    break;
                case "w_despose_material_sales":  //廢料銷售                  
                    break;
                case "w_directly_despose":  //直接報廢入庫               
                    break;
                case "w_dept_scrap":  //車間報廢,開料單
                case "w_jo_goods_split":                   
                    break;
                case "w_po_receipt_goods": //倉庫收貨                    
                    break;
                case "w_sale_return": //銷售退貨                    
                    break;
                case "w_sale_goods": //銷售出貨                   
                    break;
                case "w_finished_split": //成品拆分,成品組裝 
                case "w_product_assembly":                                  
                    break;
                case "w_out_process_out":  //外發加工單                   
                    break;
                case "w_out_return":  //外發返電處理                    
                    break;
                case "w_out_ir_mostly":  //外發退料                    
                    break;
                case "w_produce_assembly":  //组装转换單                    
                    break;
                case "w_produce_rechange": //移交單(無生產計畫流程) 
                case "w_produce_rechange_stock":                   
                    break;
                case "w_return_rechange": //移交退回單//移交退回單(無生產計畫流程)
                case "w_return_rechange_stock":                    
                    break;
                //case "w_sales_invoice":case "w_dg_delivery":  //銷售發票,東莞送貨單2012-06-07暂不控制发票
                case "w_purchase_return":  //採購退貨                    
                    break;
                default:
                    rtn = 1;
                    break;
            } //--end swith
            return rtn;
        }

        /// <summary>
        /// 检查库存数量是否大于0
        /// </summary>
        /// <param name="as_id"></param>
        /// <param name="as_window_id"></param>
        /// <returns></returns>
        public string wf_check_inventory_qty(string as_id, string as_window_id)
        {
            //在更新库存表之后检查库存数量是否大于0，这里的控制保存之前判断库存的意义不一样
            //保存之前的库存判断不能控制并发的发生           
            string strSatus="00";//strSatus字串前兩位:-1代表失敗;00:成功
            string ls_goods_id = "", strSql = "", ls_errtext = "", rtn = "00";           
            int li_cnt = 0;
            DataTable dt = new DataTable();


            switch (as_window_id)
            {
                case "w_st_adjustment":  //调整
                    strSql = string.Format(
                        @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from st_a_subordination b where b.within_code='{0}' and b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code)", within_code, as_id);
                    dt=sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_changestore_cc":  //仓库转仓
                    strSql = string.Format(
                        @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and exists(select * from st_i_subordination b where b.id ='{0}' and a.goods_id =b.goods_id
		                and a.location_id =b.inventory_issuance and a.carton_code=b.ii_code)", as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    if (li_cnt > 0)
                    {
                        rtn = "-1";
                        ls_errtext = "不允許出現負庫存" + "[" + ls_goods_id + "]";
                        break;
                    }
                    strSql = string.Format(
                        @"Select count(*) as li_cnt, max(a.goods_id) as goods_id From st_details_lot a Where(a.sec_qty < 0 oR A.qty < 0)
                        and Exists(select * from st_cc_details_schedule b where b.within_code='{0}' And b.id ='{1}' and a.goods_id = b.goods_id
                        and a.location_id = b.inventory_issuance and a.carton_code = b.ii_code) ", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_transfer_out":  //转出单
                    strSql = string.Format(
                        @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from st_transfer_detail b where b.id ='{0}' and a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)",as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    if (li_cnt > 0)
                    {
                        rtn = "-1";
                        ls_errtext = "不允許出現負庫存" + "[" + ls_goods_id + "]";
                        break;
                    }
                    strSql = string.Format(
                        @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from st_transfer_detail_part b where b.within_code='{0}' And b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code)",within_code,as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_transfer_in":  //转入单
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from st_transfer_detail b where b.id ='{0}' and a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)", as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    if(li_cnt > 0)
                    {
                        rtn = "-1";
                        ls_errtext = "不允許出現負庫存" + "[" + ls_goods_id + "]";
                        break;
                    }
                    break;
                case "w_jo_bill_despose":  //倉庫報廢
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from jo_bill_despose_materiel_detail b where b.within_code='{0}' And b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)",within_code,as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    if (li_cnt > 0)
                    {
                        rtn = "-1";
                        ls_errtext = "不允許出現負庫存" + "[" + ls_goods_id + "]";
                        break;
                    }
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from jo_bill_despose_detail b where b.within_code='{0}' and b.id='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_despose_material_sales":  //廢料銷售
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from jo_bill_despose_detail b where b.within_code='{0}' and b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_directly_despose":  //直接報廢入庫
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from jo_bill_despose_detail b where b.within_code='{0}' and b.id='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_dept_scrap":  //車間報廢,開料單
                case "w_jo_goods_split":
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from jo_bill_despose_materiel_detail b where b.within_code='{0}' and b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    if (li_cnt > 0)
                    {
                        rtn = "-1";
                        ls_errtext = "不允許出現負庫存" + "[" + ls_goods_id + "]";
                        break;
                    }                    
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from jo_bill_despose_detail b where b.within_code='{0}' and b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_po_receipt_goods"://倉庫收貨
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from po_receipt_details b where b.within_code='{0}' and b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_sale_return":  //銷售退貨
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from so_return_details b where b.id ='{0}' and a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)", as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    if (li_cnt > 0)
                    {
                        rtn = "-1";
                        ls_errtext = "不允許出現負庫存" + "[" + ls_goods_id + "]";
                        break;
                    }
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id from st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from so_return_details_part b where b.within_code='{0}' and b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_sale_goods":  //銷售出貨貨
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from so_issues_details b where b.id ='{0}' and a.goods_id =b.goods_id
		                and a.location_id =b.ii_location and a.carton_code =b.carton_code)", as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_finished_split":  //成品拆分,成品組裝
                case "w_product_assembly":
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from st_finished_split_details b where b.id ='{0}' And a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)", as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    if (li_cnt > 0)
                    {
                        rtn = "-1";
                        ls_errtext = "不允許出現負庫存" + "[" + ls_goods_id + "]";
                        break;
                    }
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from st_finished_split_details_part b where b.within_code='{0}' And b.id ='{1}' And a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_out_process_out"://外發加工單
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from op_outpro_out_detail b where b.within_code='{0}' And b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code) ", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_out_return"://外發返電處理
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from op_return_details b where b.within_code='{0}' And b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_out_ir_mostly"://外發退料
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from op_ir_details b where b.within_code='{0}' And b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_produce_assembly"://组装转换單
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from jo_materiel_con_details_part b where b.within_code='{0}' And b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code and a.mo_id =b.mo_id)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_return_rechange"://移交退回單
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from jo_materiel_con_details b where b.within_code='{0}' And b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location and a.carton_code =b.carton_code and a.mo_id =b.mo_id)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                case "w_purchase_return"://採購退貨
                    strSql = string.Format(
                       @"Select count(*) as li_cnt,max(a.goods_id) as goods_id From st_details_lot a Where (a.sec_qty <0 oR A.qty <0)
		                and Exists(select * from po_return_details b where b.within_code='{0}' And b.id ='{1}' and a.goods_id =b.goods_id
		                and a.location_id =b.location_id and a.carton_code =b.carton_code)", within_code, as_id);
                    dt = sh.ExecuteSqlReturnDataTable(strSql);
                    li_cnt = int.Parse(dt.Rows[0]["li_cnt"].ToString());
                    ls_goods_id = dt.Rows[0]["goods_id"].ToString();
                    break;
                default:
                    break;
            }  //--end swith
            
            if(li_cnt > 0)
            {
                rtn = "-1";
                ls_errtext = "不允許出現負庫存" + "[" + ls_goods_id + "]";
            }
            strSatus = rtn + ls_errtext;

            return strSatus;
        }

        public decimal checkDecimal(string strVal)
        {
            decimal result = 0;
            if (!string.IsNullOrEmpty(strVal))
            {
                if (strVal == "0")
                {
                    strVal = "0.00";
                }
                result = Math.Round(decimal.Parse(strVal),2); //string.Format("{0:F2}", ldc_ii_qty)
            }
            else
            {
                result = 0;
            }
            return result;
        }

        public int checkInt(string strVal)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(strVal))
            {
                decimal dec_value = decimal.Parse(strVal);
                result = (int)dec_value;
            }
            else
            {
                result = 0;
            }

            return result;

            //方法二
            //int li_index = strVal.IndexOf(".");
            //int li_dec= strVal.Length - li_index;
            //if (li_index > 0)
            //{
            //    if (li_index == 1 && strVal == "0.0")
            //    {
            //        strVal = "0";
            //    }
            //    else
            //    {
            //        string str_dec = strVal.Substring(li_index, li_dec);  //取小數點開始后面的部分,例如:0.00
            //        strVal = strVal.Replace(str_dec, "");
            //    }
            //}
            //result = int.Parse(strVal);
        }

    }
}

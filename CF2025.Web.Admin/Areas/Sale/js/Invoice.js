var Main = {
    data() {
    return {
			selectTab: 'tab1',
            edit_mode: 0,
            showEdit: false,
            selectRow: null,
            tableDetails: [],
            editDetails: {},
            preveditDetails:{},
            searchProductMo: '',
            loading3: false,
            formData:{
				linkman:'',
			},
            prevForm: {},
            editFormChanged: false, // 是否修改标识
            sexList: [
            { label: '', value: '' },
            { label: '女', value: '0' },
            { label: '男', value: '1' }
            ],
			DocSourceTypeList:[],
            SalesmanList:[],
			CurrList:[],
			OutStoreList:[],
			InvSourceTypeList:[],
			PaymentTypeList:[],
			PriceCondList:[],
			ShipModeList:[],
			AccountList:[],
			ShipWayList:[],
			ShipPortList:[],
			MoGroupList:[],
			QtyUnitList:[],
            tableData: [
            { id: 10001, name: 'Test1', role: 'Develop', sex: 'Man', age: 28, address: 'test abc',sex:'1' },
            { id: 10002, name: 'Test2', role: 'Test', sex: 'Women', age: 22, address: 'Guangzhou',sex:'0' },
            { id: 10003, name: 'Test3', role: 'PM', sex: 'Man', age: 32, address: 'Shanghai',sex:'1' },
            { id: 10004, name: 'Test4', role: 'Designer', sex: 'Women', age: 23, address: 'test abc',sex:'0' },
            { id: 10005, name: 'Test5', role: 'Develop', sex: 'Women', age: 30, address: 'Shanghai',sex:'0' },
            { id: 10006, name: 'Test6', role: 'Designer', sex: 'Women', age: 21, address: 'test abc',sex:'1' },
            { id: 10007, name: 'Test7', role: 'Test', sex: 'Man', age: 29, address: 'test abc',sex:'1' },
            { id: 10008, name: 'Test8', role: 'Develop', sex: 'Man', age: 35, address: 'test abc',sex:'1' }
          
			],
            name: '',
            sex: 1,
            date:'',
    }
    },
    created() {
		// this.getSalesman();
        // this.getCurr();
		this.getComboxList("DocSourceTypeList");
		this.getComboxList("SalesmanList");
		this.getComboxList("CurrList");
		this.getComboxList("OutStoreList");
		this.getComboxList("InvSourceTypeList");
		this.getComboxList("PaymentTypeList");
		this.getComboxList("PriceCondList");
		this.getComboxList("ShipModeList");
		this.getComboxList("AccountList");
		this.getComboxList("ShipWayList");
		this.getComboxList("ShipPortList");
		this.getComboxList("MoGroupList");
		
		this.getComboxList("QtyUnitList");
    },
    methods: {
		showMsg() {
                    //if (this.name.length >= 9)
                    alert("ok");
                    //var vl = value;
                },

		formatSex (value) {
          if (value === '1') {
            return '男'
          }
          if (value === '0') {
            return '女'
          }
          return ''
        },
        addNew() {
            this.edit_mode = 1;
            nowDateTime = comm.getCurrentTime();
            this.formData = {
                EditFlag:1,
				ID:'',
                mo_id: '',
                oi_date: comm.getCurrentDate(),
                Ver: 0,
				separate:'',
				Shop_no:'',
                it_customer: '',
                phone: '',
                fax: '',
                payment_date: '',
                linkman: '',
                l_phone: '',
                department_id: '',
                email: '',
                issues_wh: '',
                bill_type_no: '',
                merchandiser: '',
                merchandiser_phone: '',
                po_no:'',
                shipping_methods:'',
				seller_id:'',
                m_id: '',
                exchange_rate: '',
                goods_sum: 0.00,
                other_fare: 0.00,
                disc_rate: 0.00,
                disc_amt: 0.00,
                disc_spare: 0.00,
                total_sum: 0.00,
                tax_ticket: '',
                tax_sum: 0.00,
                amount: 0.00,
                other_fee: 0.00,
                total_package_num:0,
                total_weight: '',
                remark2: '',
                ship_remark: '',
                ship_remark2: '',
                ship_remark3:'',
                remark:'',
				p_id:'',
                pc_id: '',
                sm_id: '',
                accounts: '',
                per: '',
                final_destination: '',
                issues_state: '',
                transport_style: '',
                ship_date: '',
                loading_port: '',
                ap_id: '',
                tranship_port: '',
                finally_buyer: '',
                mo_group:'',
                packinglistno: '',
                box_no: '',
                create_by: '',
                create_date: nowDateTime,
                update_by:'',
                update_date:nowDateTime,
				state:'',
                check_date: '',

            };
            //深度複製一個對象，用來判斷數據是否有修改
            this.prevForm = JSON.parse(JSON.stringify(this.formData));
			this.showInsertEvent ();
            },
            productMoBlurEvent() {
                if (this.formData.ProductMo == "")
                    return;
                if (this.edit_mode == 1)
                    this.getPlanFromOrder();
            },
            searchByProductMo(_ProductMo) {
                this.edit_mode = 0,
                this.loading3 = true;
                setTimeout(() => {
                    this.getPlanHead(_ProductMo);
                    this.getPlanDetails(_ProductMo);
                    this.loading3 = false;
                }, 500);
                
            },
		getDataDetails(value){
			alert(value);
		},
            getDataMostly(value) {
                var _self = this;
                //axios.get("GetGoodsDetails", { params: { goods_id: _id, } })//也可以將參數寫在這裡
                axios.get("GetDataMostly", { params: { mo_id: value } }).then(
                (response) => {
                    this.formData.it_customer = response.data.it_customer;
                    this.formData.m_id = response.data.m_id;
                    this.formData.merchandiser = response.data.merchandiser;

                    //深度複製一個對象，用來判斷數據是否有修改
                    this.prevForm = JSON.parse(JSON.stringify(this.formData));
                    //var ImagePath = "/art/artwork/" + "AAAA/A888020.bmp";
                    //this.ArtImageUrl = ImagePath;
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
            },
            getPlanDetails(_ProductMo) {
                var _self = this;
                axios.get("GetPlanDetailsByMo", { params: { ProductMo: _ProductMo } }).then(
                (response) => {
                    this.tableDetails = response.data;
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
            },
            getPlanFromOrder() {
                var _self = this;
                //axios.get("GetGoodsDetails", { params: { goods_id: _id, } })//也可以將參數寫在這裡
                axios.get("GetOrderByMo", { params: { ProductMo: _self.formData.ProductMo } }).then(
                (response) => {
                    this.formData.ProductMo = response.data[0].ProductMo,
                    this.formData.CustomerID = response.data[0].CustomerID,
                    this.formData.GoodsID = response.data[0].GoodsID,
                    this.formData.OrderQty = response.data[0].OrderQty,
                    this.formData.OrderUnit = response.data[0].OrderUnit,
                    this.formData.ProductRemark = response.data[0].ProductRemark,
                    this.formData.ArtImageUrl = response.data[0].ArtImageUrl,
                    this.tableDetails = response.data
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
            },
            submitSearch() {

            },
            showInsertEvent () {
               this.cleanDetailsTextBox();
               this.selectRow = null;
               this.showEdit = true;

            },
			cleanDetailsTextBox(){
				this.editDetails = {
                    EditFlag: 0,
					ID:this.formData.ID,
					sequence_id:'',
                    mo_id: '',
					shipment_suit:'',
                    goods_id: '',
                    table_head: '',
                    goods_name: '',
                    u_invoice_qty: 0,
                    goods_unit: '',
                    sec_qty: 0.00,
                    sec_unit: 'KG',
					invoice_price: 0.000,
                    disc_rate: 0.00,
                    disc_price: 0.000,
                    total_sum: 0.00,
                    disc_amt: 0.00,
                    buy_id: '',
                    order_id: '',
                    issues_id: '',
					ref1: '',
                    ref2: '',
                    ncv: '',
                    apprise_id: '',
                    is_free: '',
                    corresponding_code: '',
                    nwt: 0.00,
                    gross_wt: 0.00,
					package_num: '',
                    box_no: '',
                    color: '',
                    spec: '',
                    subject_id: '',
                    contract_cid: '',
                    Delivery_Require: '',
                    location_id: 'Y10',
					brand_category: '',
                    customer_test_id: '',
                    customer_goods: '',
                    customer_color_id: '',
                    remark: '',
                };
			},
			getComboxList(SourceType) {
                var _self = this;///Base/BaseData///, { params: { ProductMo: this.editDetails.GoodsID } }
				var url="GetComboxList";
				if(SourceType=="QtyUnitList" || SourceType=="MoGroupList" || SourceType=="SalesmanList" || SourceType=="CurrList")
					url="/Base/DataComboxList/GetComboxList";
				url+="?SourceType="+SourceType;
                axios.get(url).then(
                    (response) => {
						if(SourceType=="DocSourceTypeList")
							this.DocSourceTypeList = response.data;
						else if(SourceType=="SalesmanList")
							this.SalesmanList = response.data;
						else if(SourceType=="CurrList")
							this.CurrList = response.data;
						else if(SourceType=="OutStoreList")
							this.OutStoreList = response.data;
						else if(SourceType=="InvSourceTypeList")
							this.InvSourceTypeList = response.data;
						else if(SourceType=="PaymentTypeList")
							this.PaymentTypeList = response.data;
						else if(SourceType=="PriceCondList")
							this.PriceCondList = response.data;
						else if(SourceType=="ShipModeList")
							this.ShipModeList = response.data;
						else if(SourceType=="AccountList")
							this.AccountList = response.data;
						else if(SourceType=="ShipWayList")
							this.ShipWayList = response.data;
						else if(SourceType=="ShipPortList")
							this.ShipPortList = response.data;
						else if(SourceType=="MoGroupList")
							this.MoGroupList = response.data;
						else if(SourceType=="QtyUnitList")
							this.QtyUnitList = response.data;
                    },
                    (response) => {
                        alert(response.status);
                    }
                ).catch(function (response) {
                    alert(response);
                });
            },
            updateEvent() {
                //this.showEdit = false
                //this.$XModal.message({ content: '保存成功', status: 'success' })
                //Object.assign(this.selectRow, this.tableDetails)
                //return;
                if (this.selectRow) {
                    this.showEdit = false
                    //this.$XModal.message({ content: '保存成功', status: 'success' })
                    for (let i in this.editDetails) {
                        if (this.editDetails[i] != this.preveditDetails[i]) {
                            this.editDetails.EditFlag = 1;
                            break;
                        }
                    }
                    Object.assign(this.selectRow, this.editDetails)
                } else {
                    this.editDetails.EditFlag = 1;
                    this.tableDetails.push(this.editDetails);
					this.cleanDetailsTextBox();
                }
            },
            getGoodsByID(){
                if (this.editDetails.GoodsID != "") {
                    var _self = this;
                    axios.get("GetGoodsByID", { params: { GoodsID: this.editDetails.GoodsID } }).then(
                    (response) => {
                        this.editDetails.GoodsCname = response.data.goods_cname
                    },
                    (response) => {
                        alert(response.status);
                    }
                ).catch(function (response) {
                    alert(response);
                });
                }
            },
            editRowEvent (row) {
                this.editDetails = {
                    EditFlag: 0,
                    GoodsID: row.GoodsID,
                    GoodsCname: row.GoodsCname,
                    RequestQty: row.RequestQty,
                    RequestDate:row.RequestDate,
                    WipID: row.WipID,
                    NextWipID: row.NextWipID
                }
                //深度複製一個對象，用來判斷數據是否有修改
                this.preveditDetails = JSON.parse(JSON.stringify(this.editDetails));
                this.selectRow = row
                this.showEdit = true
            },
            saveEvent() {
                this.validData();
                var _self = this;
                var PlanHead = _self.formData;
                var PlanDetails = _self.tableDetails;
                axios.post("SavePlan", { PlanHead, PlanDetails }).then(
                (response) => {
                    this.SavePlan = {
                        ProductMo: "",
                    };
                    alert("更新成功!");
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
            },
            validData() {
                for (let i in this.formData) {
                    if (this.formData[i] != this.prevForm[i]) {
                        this.formData.EditFlag = 1;
                        break;
                    }
                }
            },
            deleteEvent() {
                let selectRecords = this.$refs.xTable.getCheckboxRecords()
                if (selectRecords.length) {
                    this.$refs.xTable.removeCheckboxRow()
                } else {
                    alert('请至少选择一条数据！')
                    //this.$xDetails.message({ content: 'warning 提示框', status: 'warning' })
                }
            }, 
    },
	watch: {
                //// watch监听 判断是否修改  
                //formData: {
                //    handler (val, oldVal) {
                //        for (let i in this.formData) {
                //            if (val[i] != this.prevForm[i]) {
                //                this.editFormChanged = true;
                //                break;
                //            } else {
                //                this.editFormChanged = false;
                //            }
                //        }
                //        console.log(this.editFormChanged);
                //    },
                //    deep: true
                //}

            }
};
 
var app = new Vue(Main).$mount('#app');


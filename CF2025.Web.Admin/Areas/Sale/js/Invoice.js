var Main = {
    data() {
    return {
			selectTab: 'tab1',
            edit_mode: 0,
            showEdit: false,
            selectRow: null,
            tableDetails: [],
			tableFareDetails:[],
            editDetails: {},
            preveditDetails:{},
            searchProductMo: '',
            loading3: false,
			selectedRowIndex:-1,
			selectedFareRowIndex:-1,
			newDocFlag:0,
            // formData:{
				// it_customer:'',
				// merchandiser: '',
				// oi_date: comm.getCurrentDate(),
				// Ver: 0,
				// separate:'01',
				// create_date:comm.getCurrentTime(),
				// create_by: 'admin',
				// update_date:comm.getCurrentTime(),
				// update_by: 'admin',
				// state:'0',
				// state_name:'未批準',
			// },
			formData:{},
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
			GetQtyUnitRateList:[],
            tableData: [
            // { id: 10001, name: 'Test1', role: 'Develop', sex: 'Man', age: 28, address: 'test abc',sex:'1' },
            // { id: 10002, name: 'Test2', role: 'Test', sex: 'Women', age: 22, address: 'Guangzhou',sex:'0' },
            // { id: 10003, name: 'Test3', role: 'PM', sex: 'Man', age: 32, address: 'Shanghai',sex:'1' },
            // { id: 10004, name: 'Test4', role: 'Designer', sex: 'Women', age: 23, address: 'test abc',sex:'0' },
            // { id: 10005, name: 'Test5', role: 'Develop', sex: 'Women', age: 30, address: 'Shanghai',sex:'0' },
            // { id: 10006, name: 'Test6', role: 'Designer', sex: 'Women', age: 21, address: 'test abc',sex:'1' },
            // { id: 10007, name: 'Test7', role: 'Test', sex: 'Man', age: 29, address: 'test abc',sex:'1' },
            // { id: 10008, name: 'Test8', role: 'Develop', sex: 'Man', age: 35, address: 'test abc',sex:'1' },
          
			],
            name: '',
            sex: 1,
            date:'',
    }
    },
    created() {
		// this.getSalesman();
        // this.getCurr();
		this.initMostlyValue();
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
		this.getComboxList("QtyUnitRateList");
		
		this.newDoc();
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
        newDoc() {
            this.edit_mode = 1;
			this.newDocFlag=1;
			this.initMostlyValue();
            //深度複製一個對象，用來判斷數據是否有修改
            this.prevForm = JSON.parse(JSON.stringify(this.formData));
			this.tableDetails = undefined;
			this.tableDetails = new Array();
			this.addNewItem();
			this.tableFareDetails=undefined;
			this.tableFareDetails = new Array();
            },
		tableDetailsCellClickEvent({rowIndex}){
			this.selectedRowIndex=rowIndex;
		},
		tableFareDetailsCellClickEvent({rowIndex}){
			this.selectedFareRowIndex=rowIndex;
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
		initMostlyValue(){
			nowDateTime = comm.getCurrentTime();
			this.formData = {
            EditFlag:1,
			ID:'',
            mo_id: '',
            oi_date: comm.getCurrentDate(),
            Ver: 0,
			separate:'1',
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
            exchange_rate: 0.000,
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
            create_by: 'admin',
            create_date: nowDateTime,
            update_by:'admin',
            update_date:nowDateTime,
			state:'0',
			state_name:'未批準',
            check_date: '',
			};
		},
		addNewItem(){
			this.initDetailsValue();
            this.selectRow = null;
			this.tableDetails.push(this.editDetails);
		},
		getDataFromOC(row){
			if(row.mo_id=='')
				return;
			if(this.newDocFlag==1 && this.tableDetails.length==1)
			{
				this.getMostlyFromOc(row.mo_id);
			}
			
			this.getDetailsFromOc(row);

			// debugger;
		},
        getMostlyFromOc(mo_id) {
            //axios.get("GetGoodsDetails", { params: { goods_id: _id, } })//也可以將參數寫在這裡
            axios.post("GetMostlyFromOc", { mo_id }).then(
            (response) => {
				if(response.data.it_customer!=null)
				{
				var retData=response.data;
                this.formData.it_customer = retData.it_customer;
				this.formData.finally_buyer = retData.finally_buyer;
				this.formData.seller_id = retData.seller_id;
				this.formData.po_no = retData.po_no;
				this.formData.phone = retData.phone;
				this.formData.linkman = retData.linkman;
				this.formData.l_phone = retData.l_phone;
				this.formData.fax = retData.fax;
				this.formData.email = retData.email;
				this.formData.merchandiser = retData.merchandiser;
				this.formData.merchandiser_phone = retData.merchandiser_phone;
				this.formData.m_id = retData.m_id;
				this.formData.exchange_rate = retData.exchange_rate;
				this.formData.p_id = retData.p_id;
				this.formData.pc_id = retData.pc_id;
				this.formData.sm_id = retData.sm_id;
				this.formData.tax_ticket = retData.tax_ticket;
				this.formData.ship_remark = retData.ship_remark;
				this.formData.remark = retData.remark;
				// this.formData=response.data;

                //深度複製一個對象，用來判斷數據是否有修改
                this.prevForm = JSON.parse(JSON.stringify(this.formData));
				this.formData.bill_type_no=this.getBillTypeNo(this.formData.it_customer,this.formData.m_id);
				}
            },
            (response) => {
                alert(response.status);
            }
        ).catch(function (response) {
            alert(response);
        });
        },
		getBillTypeNo(it_customer,m_id)
		{
			let cust_type=it_customer.substr(0,2);
			let bill_type_no="";
			if(cust_type=='DL' && m_id=='HKD')
				bill_type_no='L';
			else if(cust_type=='DC' && m_id=='HKD')
				bill_type_no='C';
			else if(cust_type=='DO' || it_customer.substr(1,2)=='DU')
				bill_type_no='U';
			else if(cust_type=='DC' && m_id=='USD')
				bill_type_no='A';
			else if(cust_type=='DL' && m_id=='USD')
				bill_type_no='E';
			else if(cust_type=='DR')
				bill_type_no='R';
			return bill_type_no;
		},
        getDetailsFromOc(row) {
            //axios.get("GetGoodsDetails", { params: { goods_id: _id, } })//也可以將參數寫在這裡
			var mo_id=row.mo_id;
            axios.post("GetDetailsFromOc", { mo_id }).then(
            (response) => {
				var ocData=response.data[0];
				if(ocData.ocDetails.goods_id==null)
					return;
				if(ocData.ocMostly.it_customer!=this.formData.it_customer)
				{
					alert("該頁數的客戶編號為："+ocData.ocMostly.it_customer+",不能在同一張發票中加入不同的客戶資料!")
					return;
				}
				else
				{
					//////在表格中插入行的方法有兩種：
					//////第一種方法，替換當前行的值
					row.goods_id = ocData.ocDetails.goods_id;
					row.goods_name = ocData.ocDetails.goods_name;
					row.customer_goods = ocData.ocDetails.customer_goods;
					row.customer_color_id = ocData.ocDetails.customer_color_id;
					row.u_invoice_qty = ocData.ocDetails.u_invoice_qty;
					row.goods_unit = ocData.ocDetails.goods_unit;
					row.invoice_price = ocData.ocDetails.invoice_price;
					row.p_unit = ocData.ocDetails.p_unit;
					row.color = ocData.ocDetails.color;
					row.contract_cid = ocData.ocDetails.contract_cid;
					row.order_id = ocData.ocDetails.order_id;
					row.disc_rate = ocData.ocDetails.disc_rate;
					row.location_id = ocData.ocDetails.location_id;
					row.big_class = ocData.ocDetails.big_class;
					row.is_free = ocData.ocDetails.is_free;
					row.is_print = ocData.ocDetails.is_print;
					row.table_head = ocData.ocDetails.table_head;
					row.remark = ocData.ocDetails.remark;
					row.customer_test_id = ocData.ocDetails.customer_test_id;
					//////第二種方法，刪除當前行，再插入新的行，但這種方法插入後，單元格會失去焦點，要重新點擊以獲取焦點
					// this.tableDetails.splice(this.selectedRowIndex,1);
					// this.tableDetails.push(ocData.ocDetails);
					if(row.location_id.substr(0,1)=='Y')
						this.formData.issues_wh='H';
					else
						this.formData.issues_wh='D';
					const find_mo_id=this.tableFareDetails.find(fare_mo_id=>fare_mo_id.mo_id===row.mo_id);
					if(find_mo_id==undefined)
					{
					if(ocData.ocOtherFare.mo_id!='')
					{
					for (var i = 0; i < response.data.length; i++) {
						var ocOtherFare=response.data[i].ocOtherFare;
                        // this.tableFareDetails.push({'fare_id':ocOtherFare.fare_id,'name':ocOtherFare.name
							// ,'qty':ocOtherFare.qty
							// ,'price':ocOtherFare.price,'fare_sum':ocOtherFare.fare_sum
							// ,'mo_id':ocOtherFare.mo_id,'is_free':ocOtherFare.is_free
							// ,'remark':ocOtherFare.remark});
						this.tableFareDetails.push(ocOtherFare);
                    }
					}
					}
					this.sumGoodsAmt(row);
					
				}
            },
            (response) => {
                alert(response.status);
            }
        ).catch(function (response) {
            alert(response);
        });
        },
		
		sumGoodsAmt(row)
		{
			let total_sum=0;
			let u_invoice_qty=row.u_invoice_qty;
			let invoice_price=row.invoice_price;
			let disc_rate=row.disc_rate;
			let disc_amt=0;
			row.disc_price = Number((invoice_price - (invoice_price * (disc_rate/100))).toFixed(2));//折扣後單價
			if(row.goods_unit==row.p_unit)
				total_sum=u_invoice_qty * invoice_price;
			else
			{
				let goods_unit_rate=0;
				let p_unit_rate=1;
				const Unit=this.QtyUnitRateList.find(Unit=>Unit.value===row.goods_unit);
				goods_unit_rate=Unit.rate;
				p_unit_rate=this.QtyUnitRateList.find(Unit=>Unit.value===row.p_unit).rate;
				total_sum=((u_invoice_qty * goods_unit_rate ) / p_unit_rate) * invoice_price
			}
			disc_amt=Number((total_sum * (disc_rate/100)).toFixed(2));//折扣額
			row.disc_amt=disc_amt;
			row.total_sum=Number(total_sum - disc_amt).toFixed(2);
			this.sumInvAmt();
		},
		sumInvAmt()
		{
			let inv_total_sum=0;
			for(var i=0;i<this.tableDetails.length;i++){
				inv_total_sum += Number(this.tableDetails[i].total_sum);
			}
			this.formData.goods_sum=Number(inv_total_sum.toFixed(2));//發票貨品總金額
			this.formData.disc_amt = Number((inv_total_sum * (this.formData.disc_rate/100)).toFixed(2));//折扣額
			this.formData.disc_spare=Number((inv_total_sum - this.formData.disc_amt).toFixed(2));//折扣後金額
			this.formData.total_sum=this.formData.disc_spare;//總金額
		},
		tablemo_idInputEvent(row) {
			row.mo_id=row.mo_id.toUpperCase();
			},
        submitSearch(row) {

        },
        showInsertEvent() {
           this.initDetailsValue();
           this.selectRow = null;
           this.showEdit = true;

        },
		initDetailsValue(){
			this.selectedRowIndex=-1;
			this.editDetails = {
                EditFlag: 1,
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
				p_unit: '',
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
				is_print:'',
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
                location_id: '',
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
			else if(SourceType=="QtyUnitRateList")
				url="/Base/DataComboxList/GetQtyUnitRateList";
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
					else if(SourceType=="QtyUnitRateList")
						this.QtyUnitRateList = response.data;
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
		getDataByID(ID){
			// debugger;
			// let ID1=ID;
            axios.post("GetDataByID", {ID }).then(
            (response) => {
				// this.initMostlyValue();
				// let ocMostly=response.data[0].ocMostly;
				// this.formData.ID=ocMostly.ID;
                // this.formData.it_customer=ocMostly.it_customer;
				
				this.formData=response.data[0].ocMostly;
				this.tableDetails = undefined;
				this.tableDetails = new Array();
				for (var i = 0; i < response.data.length; i++) {
					var ocDetails=response.data[i].ocDetails;
					this.editDetails=ocDetails;
                    this.tableDetails.push(this.editDetails);
                }
            },
            (response) => {
                alert(response.status);
            }
            ).catch(function (response) {
                alert(response);
            });
			axios.post("GetFareDataByID", {ID }).then(
            (response) => {
				this.tableFareDetails=response.data;
				// this.tableFareDetails = undefined;
				// this.tableFareDetails = new Array();
				// for (var i = 0; i < response.data.length; i++) {
					// var ocDetails=response.data[i].ocDetails;
					// this.editDetails=ocDetails;
                    // this.tableDetails.push(this.editDetails);
                // }

            },
            (response) => {
                alert(response.status);
            }
            ).catch(function (response) {
                alert(response);
            });
		},
		setCurrentRowUpdateState(row){
			row.EditFlag=1;
		},
		setFareCurrentRowUpdateState(row){
			row.EditFlag=1;
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
				this.initDetailsValue();
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
		currentChangeEvent({ row }){
			// alert("1");
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
            let InvMostly = this.formData;
            let InvDetails = this.tableDetails;
			let InvOtherFare=this.tableFareDetails;
            axios.post("SaveInvoice", { InvMostly, InvDetails,InvOtherFare }).then(
            (response) => {
                this.formData.ID=response.data.ReturnValue;
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
		deleteTableItemEvent() {
			var table1=this.$refs.xTable;
			// this.$refs.xTable.setCurrentRow(tableDetails[1])
			// this.$XModal.alert("fsda");
			// var rowIndex=this.$refs.xTable.getCurrentRecord();
			if(this.selectedRowIndex>=0)
				this.tableDetails.splice(this.selectedRowIndex,1);
			else
				this.$XModal.alert("沒有選中記錄!");
			this.selectedRowIndex=-1;
			return;
            //this.$refs.xTable.moveCurrentRow();
			//this.$refs.xTable.moveSelected();
			let selectRecords=this.$refs.xTable.getActiveRecord();
            if (selectRecords.length) {
                this.$refs.xTable.removeCheckboxRow();
            } else {
                alert('请至少选择一条数据！')
                //this.$xDetails.message({ content: 'warning 提示框', status: 'warning' })
            }
        }, 
    },
	watch: {
            //// watch监听 判断是否修改  
            formData: {
               handler (val, oldVal) {
                   // for (let i in this.formData) {
                       // if (val[i] != this.prevForm[i]) {
                           // this.editFormChanged = true;
                           // break;
                       // } else {
                           // this.editFormChanged = false;
                       // }
                   // }
                   // console.log(this.editFormChanged);
				   this.formData.it_customer=this.formData.it_customer.toUpperCase();
				   this.formData.phone=this.formData.phone.toUpperCase();
				   this.formData.fax=this.formData.fax.toUpperCase();
               },
               deep: true
            },
			// editDetails: {
               // handler (val, oldVal) {
                   // for (let i in this.formData) {
                       // if (val[i] != this.prevForm[i]) {
                           // this.editFormChanged = true;
                           // break;
                       // } else {
                           // this.editFormChanged = false;
                       // }
                   // }
                   // console.log(this.editFormChanged);
				   // this.editDetails.mo_id=this.editDetails.mo_id.toUpperCase();
				   // this.editDetails.goods_id=this.editDetails.goods_id.toUpperCase();
               // },
               // deep: true
            //}

        },
	
};
 
var app = new Vue(Main).$mount('#app');
// 给 vue 实例挂载内部对象，例如
Vue.prototype.$XModal = VXETable.modal;


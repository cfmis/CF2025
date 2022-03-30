
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
			editFareDetails:{},
            preveditDetails:{},
            searchProductMo: '',
            loading3: false,
			selectedRowIndex:-1,
			selectedFareRowIndex:-1,
			showSent:false,
			issues_state:'',
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
			TackFareList:[],
			issues_state_list:[],
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
		this.initDetailsValue();
		this.initFareDetailsValue();
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
		this.getComboxList("TackFareList");
		// this.getComboxList("issues_state_list");
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
		//初始化發票表頭資料
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
			m_rate:0.000,
            exchange_rate: 0.000,
            goods_sum: 0.00,
            other_fare: 0.00,
            disc_rate: 0.00,
            disc_amt: 0.00,
            disc_spare: 0.00,
            total_sum: 0.00,
            tax_ticket: '',
            tax_sum: 0.00,
            amount: '',
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
			matter:'未批準',
			flag:'',
			area:'',
			state_name:'未批準',
            check_date: '',
			};
		},
		addNewItem(){
			this.initDetailsValue();
            this.selectRow = null;
			this.tableDetails.push(this.editDetails);
		},
		addNewFareItem(){
			if(this.selectedRowIndex==-1)
			{
				this.$XModal.alert("請選擇對應的記錄!");
				return;
			}
			this.initFareDetailsValue();
			this.editFareDetails.mo_id=this.tableDetails[this.selectedRowIndex].mo_id;
            // this.selectRow = null;
			this.tableFareDetails.push(this.editFareDetails);
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
				this.formData.area = retData.area;
				this.formData.m_rate = retData.m_rate;
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
				var ocData=response.data;
				if(ocData.goods_id==null)
					return;
				if(ocData.it_customer!=this.formData.it_customer)
				{
					alert("該頁數的客戶編號為："+ocData.it_customer+",不能在同一張發票中加入不同的客戶資料!")
					return;
				}
				else
				{
					//////在表格中插入行的方法有兩種：
					//////第一種方法，替換當前行的值
					row.goods_id = ocData.goods_id;
					row.goods_name = ocData.goods_name;
					row.customer_goods = ocData.customer_goods;
					row.customer_color_id = ocData.customer_color_id;
					row.u_invoice_qty = ocData.u_invoice_qty;
					row.goods_unit = ocData.goods_unit;
					row.invoice_price = ocData.invoice_price;
					row.p_unit = ocData.p_unit;
					row.color = ocData.color;
					row.contract_cid = ocData.contract_cid;
					row.order_id = ocData.order_id;
					row.disc_rate = ocData.disc_rate;
					row.location_id = ocData.location_id;
					row.big_class = ocData.big_class;
					row.is_free = ocData.is_free;
					row.is_print = ocData.is_print;
					row.table_head = ocData.table_head;
					row.remark = ocData.remark;
					row.customer_test_id = ocData.customer_test_id;
					row.so_ver = ocData.so_ver;
					row.so_sequence_id = ocData.so_sequence_id;
					row.ncv = '0';
					row.shipment_suit='1';
					row.state='0';
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
					if(ocData.ocOtherFare[0].mo_id!='')
					{
					for (var i = 0; i < ocData.ocOtherFare.length; i++) {
						var ocOtherFare=ocData.ocOtherFare[i];
                        // this.tableFareDetails.push({'fare_id':ocOtherFare.fare_id,'name':ocOtherFare.name
							// ,'qty':ocOtherFare.qty
							// ,'price':ocOtherFare.price,'fare_sum':ocOtherFare.fare_sum
							// ,'mo_id':ocOtherFare.mo_id,'is_free':ocOtherFare.is_free
							// ,'remark':ocOtherFare.remark});
						this.tableFareDetails.push(ocOtherFare);
                    }
					}
					}
					this.getGoodsWegFromStore(row);
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
		setUnitUpdateState(row){
			this.getGoodsWegFromStore(row);
			this.sumGoodsAmt(row);
			row.EditFlag=1;
		},
		u_invoice_qtyBlurEvent(row){
			this.getGoodsWegFromStore(row);
			this.sumGoodsAmt(row);
		},
		sumGoodsAmt(row)
		{
			// this.checkGoodsQty(row);
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
		sumOtherFareAmt(row)
		{
			row.fare_sum = Number((row.qty * row.price).toFixed(2));//折扣額
			this.sumInvAmt();
		},
		sumInvAmt()
		{
			let inv_total_sum=0;
			for(var i=0;i<this.tableDetails.length;i++){
				inv_total_sum += Number(this.tableDetails[i].total_sum);
			}
			for(var i=0;i<this.tableFareDetails.length;i++){
				inv_total_sum += Number(this.tableFareDetails[i].fare_sum);
			}
			this.formData.goods_sum=Number(inv_total_sum.toFixed(2));//發票貨品總金額
			this.formData.disc_amt = Number((inv_total_sum * (this.formData.disc_rate/100)).toFixed(2));//折扣額
			this.formData.disc_spare=Number((inv_total_sum - this.formData.disc_amt).toFixed(2));//折扣後金額
			this.formData.total_sum=this.formData.disc_spare;//總金額
			
			// 格式化金額
			// this.formData.goods_sum=this.formatterAmount(this.formData.goods_sum);
			// this.formData.disc_amt =this.formatterAmount(this.formData.disc_amt);
			// this.formData.disc_spare =this.formatterAmount(this.formData.disc_spare);
			// this.formData.total_sum=this.formatterAmount(this.formData.total_sum);
		},
		getGoodsWegFromStore(row){
			let mo_id=row.mo_id;
			let goods_id=row.goods_id;
			let location_id=row.location_id;
			let u_invoice_qty=row.u_invoice_qty;
			let goods_unit=row.goods_unit;
			axios.post("GetGoodsWegFromStore", {mo_id,goods_id,location_id,u_invoice_qty,goods_unit }).then(
            (response) => {
				row.sec_qty=response.data;
            },
            (response) => {
                alert(response.status);
            }
            ).catch(function (response) {
                alert(response);
            });
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
		//初始化發票明細表記錄
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
                ncv: '0',
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
				state:'0',
            };
		},
		//初始化發票附加費記錄
		initFareDetailsValue(){
			
			this.editFareDetails = {
                EditFlag: 1,
				sequence_id:'',
				fare_id:'',
				name:'',
				tf_percent:0.00,
				sum_kind:0.00,
				qty:0,
				price:0.00,
				fare_sum:0.00,
				mould_no:'',
				remark:'',
				is_free:'',
                mo_id: '',
			}
		},
		async getComboxList(SourceType) {
            var _self = this;///Base/BaseData///, { params: { ProductMo: this.editDetails.GoodsID } }
			var url="GetComboxList";
			if(SourceType=="QtyUnitList" || SourceType=="MoGroupList" || SourceType=="SalesmanList" || SourceType=="CurrList" || SourceType=="TackFareList" || SourceType=="issues_state_list")
				url="/Base/DataComboxList/GetComboxList";
			else if(SourceType=="QtyUnitRateList")
				url="/Base/DataComboxList/GetQtyUnitRateList";
			url+="?SourceType="+SourceType;
            await axios.get(url).then(
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
					else if(SourceType=="TackFareList")
						this.TackFareList = response.data;
					else if(SourceType=="issues_state_list")
						this.issues_state_list = response.data;
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
		async getInvoiceByID(ID){
			this.initMostlyValue();
			this.initDetailsValue();
			this.initFareDetailsValue();
            await axios.post("GetInvoiceByID", {ID }).then(
            (response) => {
				// this.initMostlyValue();
				// let ocMostly=response.data[0].ocMostly;
				// this.formData.ID=ocMostly.ID;
                // this.formData.it_customer=ocMostly.it_customer;
				// if(response.data.length>0)
				// {
				this.formData=response.data.ocMostly;
				this.prevForm = JSON.parse(JSON.stringify(this.formData));
				this.tableDetails = undefined;
				this.tableDetails = new Array();
				////數組可以逐行插入，也可以直接賦值
				////1--循環逐行插入
				// for (var i = 0; i < response.data.ocDetails.length; i++) {
                    // this.tableDetails.push(response.data.ocDetails[i]);
                // }
				////2--直接賦值
				this.tableDetails=response.data.ocDetails;
				this.tableFareDetails = undefined;
				this.tableFareDetails = new Array();
				this.tableFareDetails=response.data.ocOtherFare;
				// }
            },
            (response) => {
                alert(response.status);
            }
            ).catch(function (response) {
                alert(response);
            });

			/* await axios.post("GetFareDataByID", {ID }).then(
            (response) => {
				this.tableFareDetails = undefined;
				this.tableFareDetails = new Array();
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
            }); */
		},
		setCurrentRowUpdateState(row){
			row.EditFlag=1;
		},
		setFareUpdate(row){
			const FareList=this.TackFareList.find(FareList=>FareList.value===row.fare_id);
			row.name=FareList.label;
			this.setFareCurrentRowUpdateState(row);
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
            if (!this.validData())
			{
				return false;
			}
            let InvMostly = this.formData;
            let InvDetails = this.tableDetails;
			let InvOtherFare=this.tableFareDetails;
            axios.post("SaveInvoice", { InvMostly, InvDetails,InvOtherFare }).then(
            (response) => {
				if(response.data.Status=="0")
				{
					this.formData.ID=response.data.ReturnValue;
					this.$XModal.alert(response.data.Msg);
					this.getInvoiceByID(this.formData.ID);
				}
				else
					this.$XModal.alert(response.data.Msg);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
        },
       validData() {
			let valid_flag=true;
			let edit_flag=false;
            for (let i in this.formData) {
                if (this.formData[i] != this.prevForm[i]) {
                    this.formData.EditFlag = 1;
					edit_flag=true;
                    break;
                }
            }
			if(this.formData.EditFlag = 1)
			{
				if(this.formData.it_customer=='')
				{
					this.$XModal.alert("客戶編號不能為空!");
					return false;
				}
			}
			if(!valid_flag)
				return valid_flag;
			this.tableDetails.some((item, i)=>{
　　　　　　　　if(item.EditFlag==1){
　　　　　　　　　　edit_flag=true;
　　　　　　　　　　if(item.mo_id=='')
					{
						valid_flag=false;
						this.$XModal.alert("第 "+(i+1).toString()+" 行的制單編號不能為空!");
　　　　　　　　　　	return true;//在数组的some方法中，如果return true，就会立即终止这个数组的后续循环
					}else if(item.goods_id=='')
					{
						valid_flag=false;
						this.$XModal.alert("第 "+(i+1).toString()+" 行的產品編號不能為空!");
　　　　　　　　　　	return true;//在数组的some方法中，如果return true，就会立即终止这个数组的后续循环
					}else if(Number(item.u_invoice_qty)==0)
					{
						valid_flag=false;
						this.$XModal.alert("第 "+(i+1).toString()+" 行的發票數量不能為零!");
　　　　　　　　　　	return true;//在数组的some方法中，如果return true，就会立即终止这个数组的后续循环
					}
　　　　　　　　}
　　　　　　})
			if(!valid_flag)
				return valid_flag;
			this.tableFareDetails.some((item, i)=>{
　　　　　　　　if(item.EditFlag==1){
　　　　　　　　　　edit_flag=true;
　　　　　　　　　　//在数组的some方法中，如果return true，就会立即终止这个数组的后续循环
　　　　　　　　　　return true;
　　　　　　　　}
　　　　　　})
			if(!edit_flag)
			{
				this.$XModal.alert("沒有要儲存的記錄!");
				return false;
			}
			return true;
        },
		async checkGoodsQty (row) {
			try {
				let res = await axios.get('CheckGoodsQty', {
				params: {ID:this.formData.ID,mo_id:row.mo_id,goods_id:row.goods_id,u_invoice_qty:row.u_invoice_qty,goods_unit:row.goods_unit,sec_qty:row.sec_qty,location_id:row.location_id}
				})
				if(res.data.Status=="1")
				{
					//// debugger;
					this.$XModal.alert(res.data.Msg);
					      //// return Promise.reject(res)
					//// valid_flag=false;
				}
			} catch (err) {
				console.log(err)
				alert('请求出错！')
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
			if(this.selectedRowIndex<0)
			{
				this.$XModal.alert("沒有選中記錄!");
				return;
			}
            let mo_id = this.tableDetails[this.selectedRowIndex].mo_id;
			let sequence_id = this.tableDetails[this.selectedRowIndex].sequence_id;
			this.tableDetails.splice(this.selectedRowIndex,1);
			const InvOtherFare = this.tableFareDetails.filter(InvOtherFare=>InvOtherFare.mo_id===mo_id);
			let rowCount=this.tableFareDetails.length;
			for(let j=0;j<rowCount;j++)
			{
			this.tableFareDetails.forEach((item,i) => {
				if(item.mo_id==mo_id)
				{
					this.tableFareDetails.splice(i,1);
					return;
				}
			})
			}
			this.sumInvAmt();
			if(sequence_id==null)
				return;
			let InvMostly = this.formData;
			// let InvOtherFare=this.tableFareDetails;
			// const InvOtherFare=this.tableFareDetails.find(InvOtherFare=>InvOtherFare.mo_id===InvDetails.mo_id);
			this.updateDelRecords(InvMostly, sequence_id,InvOtherFare);
			this.selectedRowIndex=-1;
        }, 
		delFareRowEvent(row){
			let rIndex=row.rowIndex;
			let sequence_id=this.tableFareDetails[rIndex].sequence_id
			let InvOtherFare = new Array();
			InvOtherFare.push({"sequence_id":this.tableFareDetails[rIndex].sequence_id});
			this.tableFareDetails.splice(rIndex,1);
			this.sumInvAmt();
			if(sequence_id=="")
				return;
			let InvMostly = this.formData;
			let DetailsSequence_id="";
			this.updateDelRecords(InvMostly, DetailsSequence_id,InvOtherFare);
		},
		updateDelRecords(InvMostly, sequence_id,InvOtherFare){
            axios.post("DelInvoice", { InvMostly, sequence_id,InvOtherFare }).then(
            (response) => {
				
                this.formData.ID=response.data.ReturnValue;
                alert("刪除成功!");
				this.getInvoiceByID(this.formData.ID);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
			
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
		approveEvent() {
            /* if (!this.validData())
			{
				return false;
			} */
            let InvMostly = this.formData;
            let InvDetails = this.tableDetails;
			let InvOtherFare=this.tableFareDetails;
            axios.post("ApproveInvoice", { InvMostly, InvDetails,InvOtherFare }).then(
            (response) => {
				if(response.data.Status=="0")
				{
					this.formData.ID=response.data.ReturnValue;
					this.$XModal.alert(response.data.Msg);
					this.getInvoiceByID(this.formData.ID);
				}
				else
					this.$XModal.alert(response.data.Msg);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
        },
		//新版本
		newVersion() {
            /* if (!this.validData())
			{
				return false;
			} */
            let InvMostly = this.formData;
            let InvDetails = this.tableDetails;
			let InvOtherFare=this.tableFareDetails;
            axios.post("NewVersion", { InvMostly, InvDetails,InvOtherFare }).then(
            (response) => {
				if(response.data.Status=="0")
				{
					this.formData.ID=response.data.ReturnValue;
					this.$XModal.alert(response.data.Msg);
					this.getInvoiceByID(this.formData.ID);
				}
				else
					this.$XModal.alert(response.data.Msg);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
        },
		showModalSent(){
			if(this.formData.state!="1")
			{
				this.$XModal.alert("只有已批準的發票才可以進行發貨確認!");
				return;
			}
			if(this.issues_state_list.length===0)
			{
				this.getComboxList("issues_state_list");
			}
			this.showSent=true;
		},
		//發貨確認
		confirmSent() {
            /* if (!this.validData())
			{
				return false;
			} */
			if(this.issues_state==="")
			{
				this.$XModal.alert("發貨狀態不能為空!");
				return;
			}
			let issues_state=this.issues_state;
            let InvDetails = this.tableDetails;
            axios.post("ConfirmSent", { InvDetails,issues_state }).then(
            (response) => {
				if(response.data.Status=="0")
				{
					this.formData.ID=response.data.ReturnValue;
					// this.$XModal.alert(response.data.Msg);
					this.getInvoiceByID(this.formData.ID);
					this.$refs.xModalSent.close();
				}
				else
					this.$XModal.alert(response.data.Msg);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
        },
		//取消發貨
		cancelSent() {
            /* if (!this.validData())
			{
				return false;
			} */
			if(this.formData.state!="7")
			{
				this.$XModal.alert("只有已做發貨確認的發票才可進行取消發貨操作!");
				return;
			}
            let InvDetails = this.tableDetails;
            axios.post("CancelSent", { InvDetails }).then(
            (response) => {
				if(response.data.Status=="0")
				{
					this.formData.ID=response.data.ReturnValue;
					// this.$XModal.alert(response.data.Msg);
					this.getInvoiceByID(this.formData.ID);
				}
				else
					this.$XModal.alert(response.data.Msg);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
        },
		//注銷
		async cancelDoc() {

            let ID = this.formData.ID;
            await axios.post("CancelDoc", { ID }).then(
            (response) => {
				if(response.data.Status=="0")
				{
					this.formData.ID=response.data.ReturnValue;
					// this.$XModal.alert(response.data.Msg);
					
				}
				else
					this.$XModal.alert(response.data.Msg);
				this.getInvoiceByID(ID);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
        },
		formatterRowAmount ({ cellValue }) {
              return XEUtils.commafy(XEUtils.toNumber(cellValue), { digits: 2 })
            },
		formatterQty ({ cellValue }) {
              return XEUtils.commafy(XEUtils.toNumber(cellValue), { digits: 0 })
            },
		formatterAmount (amountValue) {
			// alert("ok");
			// debugger;
              return XEUtils.commafy(XEUtils.toNumber(amountValue), { digits: 2 })
            },
		showWindow(){
			// location.href="Create?TB_iframe=true&height=600&width=600";
			tb_show('',"Create?TB_iframe=true&height=600&width=800",'');
			// let ID='';
			// axios.post("CreateService").then(
            // (response) => {
				// debugger;
				// if(response.data.Status=="0")
				// {
					
				// }
				// else
					// this.$XModal.alert(response.data.Msg);
            // },
            // (response) => {
                // alert(response.status);
            // }
			// ).catch(function (response) {
				// alert(response);
			// });
			
			
		},
		
		dataTableReturnList(){
			// location.href="Create?TB_iframe=true&height=600&width=600";
			let ID='';
			axios.post("DataTableReturnList").then(
            (response) => {
				debugger;
				if(response.data.Status=="0")
				{
					
				}
				else
					this.$XModal.alert(response.data.Msg);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
			
			
		}
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
	computed: {
	goods_sum: {
      get () {
        return this.formatterAmount(this.formData.goods_sum);
      },
      set (val) {
        this.formData.goods_sum = this.formatterAmount(val);
      }
    },
	disc_amt: {
      get () {
        return this.formatterAmount(this.formData.disc_amt);
      },
      set (val) {
        this.formData.disc_amt = this.formatterAmount(val);
      }
    },
	disc_spare: {
      get () {
        return this.formatterAmount(this.formData.disc_spare);
      },
      set (val) {
        this.formData.disc_spare = this.formatterAmount(val);
      }
    },
	total_sum: {
      get () {
        return this.formatterAmount(this.formData.total_sum);
      },
      set (val) {
        this.formData.total_sum = this.formatterAmount(val);
      }
    }
  }

};
 
var app = new Vue(Main).$mount('#app');
// 给 vue 实例挂载内部对象，例如
Vue.prototype.$XModal = VXETable.modal;

// 给 vue 实例挂载内部对象，例如
// Vue.prototype.$XModal = VXETable.modal;
// Vue.prototype.$XPrint = VXETable.print;
// Vue.prototype.$XSaveFile = VXETable.saveFile;
// Vue.prototype.$XReadFile = VXETable.readFile;


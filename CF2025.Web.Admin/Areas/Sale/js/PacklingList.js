
var Main = {

data() {
    return {
        searchID:"",
		selectTab: 'tab1',
		headerCellStyle: { background: '#F5F7FA', color: '#606266', height: '25px', padding: '2px' },
        edit_mode: 0,
        showEdit: false,
        selectRow: null,
        curReportRow:null,
        tableDetails: [],
        tableReport:[],
		editDetails: {},
        preveditDetails:{},
        searchProductMo: '',
        loading3: false,
		selectedRowIndex:-1,
		selectedFareRowIndex:-1,
		showSent:false,
        showPrint:false,
		showEditItem:false,
		newDocFlag:0,

		formData:{},
        prevForm: {},
        editFormChanged: false, // 是否修改标识

		DocSourceTypeList:[],
		ShipPortList:[],
		QtyUnitList:[],
		
		cd_carton_size:[],
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
	}
},
created() {

	this.initMostlyValue();
	this.initDetailsValue();
	this.getComboxList("PL01");//單據來源
	this.getComboxList("cd_carton_size");
	this.getComboxList("ShipPortList");
	
	this.getComboxList("QtyUnitList");

	},
mounted(){
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
    },
	addNewItem(){
		// // let row=this.$refs.xTable.currentRow;
		//  this.initDetailsValue();
		// //  this.tableDetails.push(this.editDetails);
		//   this.$refs.xTable.insertAt(this.editDetails, -1);
		//   let index=this.$refs.xTable.tableData.length;
		// let row=this.$refs.xTable.tableData[index - 1];//this.$refs.xTable.currentRow;
		//   this.$refs.xTable.setActiveCell(row, "mo_id");
		//  return;
		// // let $table=this.$refs.xTable;
		// const $table = this.$refs.xTable.value
		// $table.insertAt(-1);
		// // this.$refs.xTable.insertAt(-1);
		// let aa=1;
		// aa=aa+1;
		// return;
		debugger;
		this.initDetailsValue();
        this.selectRow = null;
		this.tableDetails.push(this.editDetails);
		// this.$refs.xTable.insertAt(this.editDetails, -1);
		let index=this.tableDetails.length;
		let row=this.$refs.xTable.data[index - 1];//this.$refs.xTable.currentRow;
		this.selectRow = row;
		this.$refs.xTable.setActiveCell(row, "mo_id");//設置進入焦點，row也可以用：this.editDetails
	},
	async addNewItem1(){
		let $table=this.$refs.xTable;
		let row=-1;
		const { row: newRow } = await $table.insertAt(row)
		await $table.setEditCell(newRow, 'name')
	  },
	tableDetailsCellClickEvent({rowIndex}){
		this.selectedRowIndex=rowIndex;
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
	//初始化裝箱單表頭資料
	initMostlyValue(){
		nowDateTime = comm.getCurrentTime();
		this.formData = {
        EditFlag:1,
		ID:'',
        mo_id: '',
        packing_date: comm.getCurrentDate(),
		origin_id:'',
		invoice_id:'',
		state:'0',
        customer_id: '',
		customer:'',
		sailing_date:comm.getCurrentDate(),
		shipping_tool:'',
		linkman: '',
		phone: '',
		fax:'',
		registration_mark:'',
		packing_list:'',
		packing_list2:'',
		packing_list3:'',
		destination:'',
		port_id:'',
		ap_id:'',
        contrainer_no:'',
		seal_no:'',
		messrs:'',
		proceduce_area:'',
		shippedby:'',
		perss:'',
		total_box_qty:0,
		per:'',
		remark:'',
		create_by: 'admin',
        create_date: nowDateTime,
        update_by:'admin',
        update_date:nowDateTime,
		
		matter:'未批準',
		flag:'',
		};
		
	},
	//初始化裝箱單明細表記錄
	initDetailsValue(){
		this.selectedRowIndex=-1;
		this.editDetails = {
            EditFlag: 1,
			ID:this.formData.ID,
			sequence_id:'',
			po_no:'',
			order_id:'',
            mo_id: '',
			shipment_suit:'',
            item_no: '',
			descript:'',
			english_goods_name:'',
			color:'',
			box_no: '',
			pcs_qty:0,
			unit_code:'',
			sec_unit:'',
			symbol:'',
			cube_ctn:0,
			box_qty:0,
			carton_size:'',
			cube_ctn:0.00,
			total_cube:0.00,
			nw_each:0.00,
			gw_each:0.00,
			tal_nw:0.00,
			tal_gw:0.00,
			ref_id:'',
			remark:'',
			tr_id:'',
        };
	},

	invoice_idEvent(ID){
		if(this.formData.origin_id!='4')
			return;
		if(ID=='')
			return;
		// this.getDataFromInvoice(ID,'');
		this.getDataFromOcInvPk('1','INV');
	},
	mo_idRowEvent(row)
	{
		// if(row.mo_id=='')
		// 	return;
		// if(row.item_no!='')
		// 	return;
		if(row.EditFlag===0 || row.mo_id==="")
			return;
		if(this.formData.origin_id=='4')//從銷售發票匯入
			this.getDataFromOcInvPk('2','INV');
		else if(this.formData.origin_id=='6')//從內部裝箱單匯入
			this.getDataFromOcInvPk('2','PK');
		else//從銷售訂單匯入
			this.getDataFromOcInvPk('2','OC1');
	},
	mo_idModalEvent(){
		if(this.editDetails.mo_id=='')
			return;
		if(this.editDetails.item_no!='')
			return;
		if(this.formData.origin_id=='4')//從銷售發票匯入
			this.getDataFromOcInvPk('3','INV');
		else if(this.formData.origin_id=='6')//從內部裝箱單匯入
			this.getDataFromOcInvPk('3','PK');
		else//從銷售訂單匯入
			this.getDataFromOcInvPk('3','OC1');
	},
	async getDataFromOcInvPk(group_type,source_type){
		
		// if(row.EditFlag==0)
			// return;
		// let mo_id='';
		// if(row==null)
			// mo_id=this.editDetails.mo_id;
		// else
			// mo_id=row.mo_id;
		// if(mo_id=='')
			// return;
		debugger;
		let row;
		if(group_type=='2')
			row=this.$refs.xTable.currentRow;
		if(row==null || group_type=='3')
		{
		let index=this.$refs.xTable.tableData.length;
		row=this.$refs.xTable.tableData[index - 1];//this.$refs.xTable.currentRow;
		}
        // this.preveditDetails = JSON.parse(JSON.stringify(this.editDetails));
        // this.selectRow = row;
		// Object.assign(this.editDetails,this.selectRow );
		/* if(source_type=='OC')
			this.getDataFromOC(set_flag,row);
		else if(source_type=='INV')
			this.getDataFromInvoice('',row.mo_id);
		else
			this.getDataFromPackingList('1',row); */
		let set_flag=1;
		let postStr='';
		let mo_id=row.mo_id;
		if(source_type=='OC0' || source_type=='OC1')
		{
			if(source_type=='OC0')
				set_flag=0;
			postStr="GetDataFromOc?set_flag="+set_flag+"&mo_id="+mo_id;
		}
		else if(source_type=='PK')
			postStr="GetDataFromPackingList?packing_type=1"+"&mo_id="+mo_id;
		else
		{
			let ID='';
			if(group_type=='1')
			{
				ID=this.formData.invoice_id;
				mo_id='';
			}
			postStr="GetDataFromInvoice?ID="+ID+"&mo_id="+mo_id;
		}
		
    	await axios.post(postStr).then(
    	(response) => {
			if(this.newDocFlag==1 && this.tableDetails.length==1)
			{
				this.fillMostlyData(response.data.xl_packing_list_mostly);
			}
			const customer_id=response.data.xl_packing_list_mostly.customer_id
			if(customer_id!==this.formData.customer_id)
			{
				this.cleanRow(row);
				alert("該頁數的客戶編號為："+customer_id+",不能在同一張裝箱單中加入不同的客戶資料!");
			}else
			this.fillDetailsData(set_flag,response.data.xl_packing_list_details,row);
    	},
    	(response) => {
    	    alert(response.status);
    	}
    	).catch(function (response) {
    	    alert(response);
    	});
    	
			// this.addNewItem();
		},
		cleanRow(row){
			row.po_no='';
			row.order_id='';
			row.ID='';
			row.item_no = '';
			row.descript = '';
			row.english_goods_name='';
			row.color = '';
			row.pcs_qty = '';
			row.unit_code = '';
			row.sec_unit='';
			row.shipment_suit='0';
			row.box_no = '';
			row.symbol = '';
			row.pbox_qty='';
			row.box_qty = '';
			row.carton_size = '';
			row.cube_ctn = '';
			row.total_cube='';
			
			row.nw_each = '';
			row.gw_each = '';
			row.tal_nw='';
			row.tal_gw = '';
			row.ref_id = '';
			row.remark = '';
			row.tr_id='';
		},

	fillMostlyData(data){
		if(data.customer_id!=null)
		{
			// this.formData.ID='';
			this.formData.invoice_id=data.invoice_id;
            this.formData.customer_id = data.customer_id;
			this.formData.customer = data.customer;
			this.formData.phone = data.phone;
			this.formData.linkman = data.linkman;
			this.formData.fax = data.fax;
			this.formData.remark = data.remark;
			this.formData.packing_list=data.packing_list;
			this.formData.packing_list2=data.packing_list2;
			this.formData.packing_list3=data.packing_list3;
			this.formData.port_id=data.port_id;
			this.formData.ap_id=data.ap_id;
			this.formData.per=data.per;
			this.formData.matter='未批準';
			this.formData.state='0';

        //深度複製一個對象，用來判斷數據是否有修改
        this.prevForm = JSON.parse(JSON.stringify(this.formData));
		}
	},
	fillDetailsData(set_flag,data,row){

		/* else
		{
		// 第二種方法，刪除當前行，再插入新的行，但這種方法插入後，單元格會失去焦點，要重新點擊以獲取焦點
		// this.tableDetails.splice(this.selectedRowIndex,1);
		this.tableDetails.push(ocData);
		} */


		/* let row=row;
		let index=0;
		if(row==null)
		{
			// this.$refs.xTable.setCurrentRow(0);
			//用這個：　　this.$refs.xTable.tableData[0]　　　獲取表格中的任一行數據
			index=this.$refs.xTable.tableData.length;
			row=this.$refs.xTable.tableData[index - 1];//this.$refs.xTable.currentRow;
		} */


		//這個是獲取表格的所有屬性
		// let tb=this.$refs.xTable;
		//用這個來判斷當前行是否為空行
		//如果為空行，就在這行開始填充記錄
		//如果不為空行，就從最後面插入記錄
		var start_row=1;
		// if(row.item_no=='')
		// {
			start_row=1;
			var ocData=data[0];
			row.EditFlag= 1,
			row.mo_id=ocData.mo_id,
			row.po_no=ocData.po_no;
			row.order_id=ocData.order_id;
			row.ID=this.formData.ID;
			row.item_no = ocData.item_no;
			row.descript = ocData.descript;
			row.english_goods_name=ocData.english_goods_name;
			row.color = ocData.color;
			row.pcs_qty = ocData.pcs_qty;
			row.sec_unit='KG';
			row.unit_code = ocData.unit_code;
			row.shipment_suit=set_flag;
			
			row.box_no = ocData.box_no;
			row.symbol = '@';
			row.pbox_qty=ocData.pbox_qty;
			row.box_qty = ocData.box_qty;
			row.carton_size = ocData.carton_size;
			row.cube_ctn = ocData.cube_ctn;
			row.total_cube=ocData.total_cube;
			
			row.nw_each = ocData.nw_each;
			row.gw_each = ocData.gw_each;
			row.tal_nw=ocData.tal_nw;
			row.tal_gw = ocData.tal_gw;
			row.ref_id = ocData.ref_id;
			row.remark = ocData.remark;
			row.tr_id=ocData.tr_id;
		// }
		// else
		// 	start_row=0;
		for (var i = start_row; i < data.length; i++){
			var ocData=data[i];
			var ocDetails={
				EditFlag: 1,
				mo_id:ocData.mo_id,
				po_no:ocData.po_no,
				order_id:ocData.order_id,
				ID:this.formData.ID,
				item_no : ocData.item_no,
				descript : ocData.descript,
				english_goods_name:ocData.english_goods_name,
				color : ocData.color,
				pcs_qty : ocData.pcs_qty,
				unit_code : ocData.unit_code,
				sec_unit:'KG',
				shipment_suit:set_flag,
				
				box_no : ocData.box_no,
				symbol : '@',
				pbox_qty:ocData.pbox_qty,
				box_qty : ocData.box_qty,
				carton_size : ocData.carton_size,
				cube_ctn : ocData.cube_ctn,
				total_cube:ocData.total_cube,
				
				nw_each : ocData.nw_each,
				gw_each : ocData.gw_each,
				tal_nw:ocData.tal_nw,
				tal_gw : ocData.tal_gw,
				ref_id : ocData.ref_id,
				remark : ocData.remark,
				tr_id:ocData.tr_id,
			};
			this.tableDetails.push(ocDetails);
		}
		// 插入記錄後，定位到表格的最後一筆記錄
		// index=this.tableDetails.length;
        // this.selectRow = this.tableDetails[index - 1];
		// Object.assign(this.editDetails,this.selectRow );

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

	async getComboxList(SourceType) {
        var _self = this;///Base/BaseData///, { params: { ProductMo: this.editDetails.GoodsID } }
		var url="GetComboxList";
		if(SourceType=="QtyUnitList" || SourceType=="cd_carton_size"
			|| SourceType=="PL01")
			url="/Base/DataComboxList/GetComboxList";
		else if(SourceType=="ShipPortList")
			url="/Sale/Invoice/GetComboxList";
		url+="?SourceType="+SourceType;
        await axios.get(url).then(
            (response) => {
				if(SourceType=="PL01")
					this.DocSourceTypeList = response.data;
				else if(SourceType=="ShipPortList")
					this.ShipPortList = response.data;
				else if(SourceType=="cd_carton_size")
					this.cd_carton_size = response.data;
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

	setCurrentRowUpdateState(row){
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
        // this.editDetails = {
        //     EditFlag: 0,
		// 	mo_id:row.mo_id,
		// 	shipment_suit:row.shipment_suit,
        //     item_no: row.item_no,
        //     table_head: row.table_head,
        //     goods_name: row.goods_name,
        //     u_invoice_qty:row.u_invoice_qty,
        //     goods_unit: row.goods_unit,
        //     sec_qty: row.sec_qty,
		// 	sec_unit:row.sec_unit,
		// 	invoice_price:row.invoice_price,
        //     disc_rate: row.disc_rate,
        //     disc_price: row.disc_price,
        //     total_sum: row.total_sum,
        //     disc_amt:row.disc_amt,
        //     buy_id: row.buy_id,
        //     order_id: row.order_id,
		// 	issues_id:row.issues_id,
		// 	ref1:row.ref1,
        //     ref2: row.ref2,
        //     ncv: row.ncv,
        //     apprise_id: row.apprise_id,
        //     is_free:row.is_free,
        //     corresponding_code: row.corresponding_code,
        //     nwt: row.nwt,
		// 	gross_wt:row.gross_wt,
		// 	package_num:row.package_num,
        //     box_no: row.box_no,
        //     color: row.color,
        //     spec: row.spec,
        //     subject_id:row.subject_id,
        //     contract_cid: row.contract_cid,
        //     Delivery_Require: row.Delivery_Require,
		// 	location_id:row.location_id,
		// 	brand_category:row.brand_category,
        //     customer_test_id: row.customer_test_id,
        //     customer_goods: row.customer_goods,
        //     customer_color_id: row.customer_color_id,
        //     remark:row.remark
        // }
		
		//深度複製一個對象，用來判斷數據是否有修改
		debugger;
		// this.preveditDetails = JSON.parse(JSON.stringify(this.editDetails));//this.editDetails
		// this.initDetailsValue();

		this.selectRow = row;
		let index=this.$refs.xTable.tableData.length;
		let curRow=this.$refs.xTable.currentRow;
		let row1=this.$refs.xTable.tableData[index - 1];//this.$refs.xTable.currentRow;
		Object.assign(this.editDetails,this.selectRow );
        this.showEdit = true;
    },
    saveEvent() {
        if (!this.validData())
		{
			return false;
		}
        let PkMostly = this.formData;
        let PkDetails = this.tableDetails;
        axios.post("Save", { PkMostly, PkDetails }).then(
        (response) => {
			if(response.data.Status=="0")
			{
				this.formData.ID=response.data.ReturnValue;
				this.$XModal.alert(response.data.Msg);
				this.GetPackingListByID(this.formData.ID);
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
		// if(this.formData.ID==='')
		// 	return false;
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
			if(this.formData.customer_id=='')
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
				}else if(item.item_no=='')
				{
					valid_flag=false;
					this.$XModal.alert("第 "+(i+1).toString()+" 行的產品編號不能為空!");
　　　　　　　　	return true;//在数组的some方法中，如果return true，就会立即终止这个数组的后续循环
				}else if(Number(item.pcs_qty)==0)
				{
					valid_flag=false;
					this.$XModal.alert("第 "+(i+1).toString()+" 行的數量不能為零!");
　　　　　　　　	return true;//在数组的some方法中，如果return true，就会立即终止这个数组的后续循环
				}
　　　　　　}
　　　　})
		if(!valid_flag)
			return valid_flag;

		if(!edit_flag)
		{
			this.$XModal.alert("沒有要儲存的記錄!");
			return false;
		}
		return true;
    },

	async GetPackingListByID(ID){
		if(this.formData.ID=='')
			return;
		this.initMostlyValue();
		this.initDetailsValue();
		let packing_type=this.$refs.packing_type.value;
		await axios.post("GetPackingListByID", {packing_type,ID }).then(
		(response) => {
			this.newDocFlag=0;
			this.formData=response.data.xl_packing_list_mostly;
			this.prevForm = JSON.parse(JSON.stringify(this.formData));
			this.tableDetails = undefined;
			this.tableDetails = new Array();
			////數組可以逐行插入，也可以直接賦值
			////1--循環逐行插入
			// for (var i = 0; i < response.data.ocDetails.length; i++) {
				// this.tableDetails.push(response.data.ocDetails[i]);
			// }
			////2--直接賦值
			this.tableDetails=response.data.xl_packing_list_details;
			// }
		},
		(response) => {
			alert(response.status);
		}
		).catch(function (response) {
			alert(response);
		});

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
		var row=this.$refs.xTable.currentRow;
		if(row===null)
		{
			this.$XModal.alert("沒有選中記錄!");
			return;
		}
		let ID=row.ID;
		let sequence_id=row.sequence_id;
		axios.post("Delete", { ID,sequence_id }).then(
			(response) => {
				
				this.formData.ID=response.data.ReturnValue;
				alert(response.data.Msg);
				this.GetPackingListByID(this.formData.ID);
			},
			(response) => {
				alert(response.status);
			}
			).catch(function (response) {
				alert(response);
			});
		this.selectedRowIndex=-1;
    }, 


	approveEvent() {
		if(this.formData.ID=='')
			return;
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
				this.GetPackingListByID(this.formData.ID);
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
		if(this.formData.ID=='')
			return;
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
				this.GetPackingListByID(this.formData.ID);
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
		if(this.formData.ID=='')
			return;
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
			this.GetPackingListByID(ID);
        },
        (response) => {
            alert(response.status);
        }
		).catch(function (response) {
			alert(response);
		});
    },
	//項目修改
	showModalEditItem(){
		// if(this.formData.state!="0")
		// {
			// this.$XModal.alert("非編輯狀態，不能再進行修改!");
			// return;
		// }
		if(this.AreaList.length===0)
		{
			this.getComboxList("AreaList");
		}
		this.showEditItem=true;
	},
    additionSaveEvent() {
		if (!this.validData())
		{
			return false;
		}
		this.formData.flag=this.$refs.input1.value;
        let InvMostly = this.formData;
        axios.post("AdditionSaveInvoice", { InvMostly }).then(
        (response) => {
			if(response.data.Status=="0")
			{
				this.formData.ID=response.data.ReturnValue;
				this.$XModal.alert(response.data.Msg);
				this.GetPackingListByID(this.formData.ID);
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

	formatterQty ({ cellValue }) {
          return XEUtils.commafy(XEUtils.toNumber(cellValue), { digits: 0 })
        },

	showWindow(){
		// location.href="Create?TB_iframe=true&height=600&width=600";
		tb_show('',"Create?TB_iframe=true&height=600&width=800",'');
		// let ID='';
		// axios.post("CreateService").then(
        // (response) => {
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
    //顯示通用查詢頁面
    showFindWindos(){
	    comm.openWindos('Invoice');
	},
    findByID() {
        if(this.searchID ===""){
            return;
        }
        this.formData.ID = this.searchID;
        setTimeout(() => {
            this.GetPackingListByID(this.formData.ID);
        }, 500);
    },
    //列印
    printEvent(){
        if(this.formData.ID){
            var id="w_sales_invoice";
            axios.post("GetSelectReport?ID="+ id +"&customer_id=" + this.formData.customer_id + "&m_id=" + this.formData.m_id).then(
                (response) => {
                    this.showPrint = true;
                    this.curReportRow = null;
                    if(response.data){
                        this.tableReport = response.data;
                    }else{
                        this.tableReport=[];				
                    }
                }
		    ).catch(function (response) {
		        alert(response);			       
		    });               
            
        }else{
            this.$XModal.alert("請首先查詢出發票數據!");
            return;
        }
    },       
    tablePrintCellClickEvent(row){
        this.curReportRow = row.data[row.$rowIndex];
    },
    printReport(){
        if(this.curReportRow){
            console.log(this.curReportRow.reportid);
            var url= "Print?ID=" + this.formData.ID + "&Ver=" + this.formData.Ver+"&report_id=" + this.curReportRow.reportid;
            comm.showMessageDialog(url,'列印',1024,768,true);
        }else{
            this.$XModal.alert({ content: '請首先指定列印的報表類型!',mask: false });
        }
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
			    if(this.formData.customer_id!=null)
					this.formData.customer_id=this.formData.customer_id.toUpperCase();
			    if(this.formData.phone!=null)
					this.formData.phone=this.formData.phone.toUpperCase();
				if(this.formData.fax!=null)
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


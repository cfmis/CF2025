/**
 * 倉庫發料
 */
var MainIn = {
    data() {
        return {            
            searchID:"", 
            headStatus:"",//主檔為新增或編輯
            tableData:[],           
            curRowIndex:null,
            selectRow: null,
            selectRow2:null,
            curDelRow:null,            
            //headerCellStyle:{background:'#F5F7FA',color:'#606266',height:'25px',padding:'2px'},
            loading: false,
            loading2:false,
            showEdit: false,
            showEdit2:false,
            submitLoading: false,
            submitLoading2: false,
            showEditStore:false,
            showFind:false,
            isEditCell:false,
            server_date:"", 
            stateFontColor:"color:black",            
            tableHeight1 :400,
            tableHeight2 :350,
            btnItemTitle:"劉覽",
            btnItemTitle2:"劉覽",
            headData: { id: '', inventory_date: '', origin: '4', state: '0',bill_type_no:'01',department_id:'',linkman:'',handler:'',remark: '',create_by: '', 
                create_date: '', update_by: '',update_date: '',check_by: '', check_date: '', tum_type:'A',update_count:'1',transfers_state:'0',
                servername:'hkserver.cferp.dbo',head_status:'' },
            rowDataEdit1: { id:'',sequence_id:'',mo_id:'',goods_id:'',goods_name:'',inventory_issuance:'',ii_code:'',ir_lot_no:null,obligate_mo_id:'',
                i_amount:0,i_weight:0,inventory_receipt:'',ir_code:'',ii_lot_no:'',ref_lot_no:'',ib_qty:0,ib_weight:0,unit:'PCS',remark:null,
                ref_id:'', jo_sequence_id:'', so_no:null,contract_cid:null,mrp_id:null, sign_by:null,sign_date:null, vendor_id:'',base_unit:'PCS',rate:1,state:'0',
                transfers_state:'0',ref_sequence_id:'',only_detail:'0',row_status:'' },
            rowDataEdit2: { id:'',upper_sequence:'',sequence_id:'',mo_id:'',goods_id:'',goods_name:null,unit:'PCS',inventory_issuance:'',ii_code:'',i_amount:0,i_weight:0,
                ir_lot_no:'',obligate_mo_id:'',inventory_receipt:'',ir_code:'',remark:null,ref_id:'',ref_lot_no:null,so_no:'',contract_cid:null,mrp_id:null,
                base_unit:null,rate:null,ib_qty:null,order_qty:0,ii_lot_no:null,average_cost:null,state:'0',transfers_state:'0',ib_weight:null,order_sec_qty:null,
                so_sequence_id:'',ref_sequence_id:'',inventory_date:null,department_id:null,only_detail:'0',vendor_id:null,servername:null,row_status:''},
            formRules: {
                id: [{ required: true},{ min: 13, message: '請度13个字符' }],
                inventory_date:[{ required: true, message: '請輸入轉倉日期' }],
                department_id:[{ required: true, message: '請輸入部門編號' }]
            },
            rulesRowEdit1: {
                mo_id:[{ required: true, message: '請輸入頁數'},{min:9,message: '頁數長度至少是9个英文字符'}],
                goods_id: [{ required: true, message: '請輸入貨品編號' }],
                inventory_issuance: [{ required: true, message: '請選擇轉出倉' }],
                ir_lot_no: [{ required: true, message: '請選擇批號' }],
                obligate_mo_id:[{ required: true, message: '請輸入庫存頁數'},{min:9,message: '頁數長度至少是9个英文字符'}],                
                i_amount: [{required:true, type: 'number', min: 1, max: 10000000, message: '轉倉數量不可為空(輸入範圍1~10000000)'}],
                i_weight: [{required:true, type: 'number', min:0.01,max:99999,message:'轉倉重量不可為空(輸入範圍0.01~99999)'}],
                inventory_receipt: [{ required: true, message: '請選擇轉入倉' }],
                ii_lot_no: [{ required: true, message: '轉出批號不可為空' }],                
                unit: [{ required: true, message: '請輸入單位' }]
            },
            rulesRowEdit2: {
                i_amount: [{ type: 'number', min: 1, max: 10000000, message: '轉倉數量不可為空(輸入範圍1~10000000)'}],
                i_weight: [{ type: 'number', min:0.01,max:99999,message:'轉倉重量不可為空(輸入範圍0.01~99999)'}],
                ir_lot_no: [{ required: true, message: '批號不可為空' }]
            },
            originList:[],
            billTypeNoList:[],
            deptList: [],
            stateList: [],
            lotNoList:[],
            qtyUnitList: [],            
            locationList: [],
            confirmData1: [],
            confirmData2: [],
            originData2:[],
            gridData1: [],
            gridData2: [],
            delData1:[],
            delData2:[],
            curRowSeqId:"",
            Rows:[],
            tempSelectRow1:{},
            tempSelectRow2:{},
            seledtLotNoRow: null,
            showLot:false,
            validStockFlag:true,
            showGoodsX:0,
            showGoodsY:0,
            
            formDataFind:{},
            isEditHead: false,//主檔編輯狀態
            isEditDetail: false,//明細編輯狀態
            isReadOnlyHead: true,//主檔對象只讀狀態
            isReadOnlyDetail: true,//明細對象只讀狀態
            isDisableDropBoxHead: true,//主檔下拉列表框失效狀態
            isDisableDropBoxDetail: true,//明細下拉列表框失效狀態       
            flagChild:"",//是否是從轉出待確認列表中帶出轉入單頁面的標志
            userId:"",            
            locationId:'',
           
            tempHeadData: {},
            tempGridData1: {},
            tempGridData2: {}, 
            tableHeight :350,
            tempSaveData:[],
            checkStockData:[],
            findData:[],
            insertRecords:{}, 
            removeRecords:{}, 
            updateRecords:{},            
            validRules: {
                mo_id: [
                  { required: true, message: '頁數必须填写' }
                ],
                goods_id: [
                  { required: true, message: '貨品必须填写' }
                ],
                transfer_amount: [
                  { type: 'number', min: 1, max: 10000000, message: '输入 1 ~ 10000000 范围' }
                ],
                location_id: [
                  { required: true, message: '倉庫必须填写' }
                ]
            },
            
            loading : false,
            tableColumn: [
                    { field: 'lot_no', title: '批號' }, 
                    { field: 'qty', title: '數量' },
                    { field: 'sec_qty', title: '重量' },
                    { field: 'mo_id', title: '頁數' },
                    { field: 'vendor_name', title: '供應商名稱' },
            ],
            tableLotNoList: [{lot_no:'',qty:0,sec_qty:0,mo_id:0,vendor_name:''}],//2024/01/22 orign code: tableLotNoList: []
            pagerConfig: {
                total: 0,
                currentPage: 1,
                pageSize: 10
            },

        }
    },
    created() {
        this.getComboxList("WH01");//開單來源
        this.getComboxList("H");//單據類型
        this.getComboxList("DeptList");//部門編碼   
        this.getComboxList("StateList");//狀態 
        this.getComboxList("LocationList");//倉庫,貨架,由選擇的倉庫帶出       
        this.getComboxList("QtyUnitList");//數量單位
        //this.getComboxList("WegUnitList");//重量單位
        this.flagChild = $("#isChild").val();
        this.userId = $("#user_id").val();
        this.tempHeadData = this.headData;
        this.tempGridData1 = this.gridData1;
        this.tempGridData2 = this.gridData2;
        this.initData();        
    },
    methods: { 
        //初始化數據
        initData(){            
            //判斷是否是從轉出待確認列表中帶出轉入單頁面
            if(this.flagChild ==="1"){               
                 this.confirmData1 = parent.app.tempAddData1; //confirmData1 此時為對象  
                 this.confirmData2 = parent.app.tempAddData2;
                 //this.locationId = parent.app.formData.location_id;                 
                 if(this.confirmData1.length>0){
                     //生成表頭
                     this.insertEvent();
                     //this.$refs.xTable1;此代碼在頁面創建階段created()內引用會引起錯誤,因表格元素尚未渲染完成,插入明細的操作須放到mounded()方法內實現
                     //this.gridData = JSON.parse(JSON.stringify(confirmData));//必須按此方法賦值,否則出現兩邊數據同時更改
                 }
            }
        },        
        findByID() {
            if(this.headStatus != "" || this.searchID ===""){
                return;
            }                      
            this.curDelRow = null;
            this.loading = true;
            setTimeout(() => {
                this.getHead(this.searchID);
                this.getDetails(this.searchID);
                this.getDetailsPartByID(this.searchID);
                this.loading = false;
            }, 500);            
        },
        showFindWindos(){
            comm.openWindos('ChangeStore');
        },
        printEvent(){
           //
        },  
        //动态改变样式,更改第一頁DIV邊框高度
        //getStyle(){
        //    return {
        //        height:this.div_tab1_height,
        //        border: '1px solid #dcdfe6'
        //    }
        //},
        //点击编辑前的逻辑判断        
        activeMethod(){
            this.isEditCell = (this.headData.state ==="0")?true:false;
            return this.isEditCell;
        },
        //初始化下拉列表框數據
        getComboxList(SourceType) {
            axios.get("/Base/DataComboxList/GetComboxList?SourceType=" + SourceType).then(
                (response) => {
                    switch (SourceType) {
                        case "WH01":
                            this.originList = response.data;
                            break;
                        case "H":
                            this.billTypeNoList = response.data;
                            break;
                        case "DeptList":
                            this.deptList = response.data;
                            break;
                        case "StateList":
                            this.stateList = response.data;
                            break;
                        case "LocationList":
                            this.locationList = response.data;
                            break;
                        case "QtyUnitList":
                            this.qtyUnitList = response.data;
                            break;                                           
                        default:
                            break
                    }
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        //主檔新增
        insertEvent () {
            this.setStatusHead(true);
            this.backupData();//編輯前首先暫存主檔/明細臨時數據
            this.headStatus = "NEW";
            //清空數據
            this.clearHeadData();           
            this.gridData1 = [];//清空明細表格1數據
            this.gridData2 = [];//清空明細表格2數據 
            //新增后初始化相關對象的初始值            
            let d = new Date();//生成日期對象:Fri Oct 15 2021 17:51:20 GMT+0800
            let date = comm.getDate(d, 0);//轉成年月日字符串格式            
            let dateTime = comm.datetimeFormat(d, "yyyy-MM-dd hh:mm:ss");
            this.$set(this.headData, "inventory_date", date);
            this.$set(this.headData, "create_by", this.userId);
            this.$set(this.headData, "create_date", dateTime);
            this.getMaxID();           
        },
        
        getSequenceId(i){
            return comm.pad(i + 1,4) +"h";
        },
        editHeadEvent() {
            if (this.headData.id == "") {
                this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!', status: 'warning' });                
                return;
            };
            if(this.headData.state == "2"){
                this.$XModal.alert({ content: '注銷狀態,當前操作無效!', mask: false });
                return;
            }
            if(this.headData.state == "1"){
                this.$XModal.alert({ content: '批準狀態,當前操作無效!', mask: false });
                return;
            }
            this.headStatus = "EDIT";
            this.setStatusHead(true);
            this.isDisableDropBoxHead = true;//禁用
            this.backupData();//編輯前首先暫存主檔/明細臨時數據
            this.$set(this.headData, "update_by", this.userId);
        },
        delHeadEvent() {            
            if ((this.headData.id != "") && (this.gridData1.length > 0)) {
                if(this.headData.state == "2"){
                    this.$XModal.alert({ content: '已是注銷狀態,當前操作無效!', mask: false });
                    return;
                }
                if(this.headData.state == "1"){
                    this.$XModal.alert({ content: '批準狀態,當前操作無效!', mask: false });
                    return;
                }
                this.$XModal.confirm('請確定是否要注銷此轉入單？').then(type => {
                    if (type == 'confirm') {                        
                        let head = JSON.parse(JSON.stringify(this.headData));                        
                        axios.post("/ChangeStore/DeleteHead",{head}).then(
                            (response) => {
                                if(response.data=="OK"){                                  
                                    this.setStatusHead(false);
                                    //重查刷新數據
                                    this.getHead(this.headData.id);
                                    this.$XModal.alert({ content: '當前操作成功!', mask: false });
                                }else{
                                    this.$XModal.alert({ content: '當前操作失敗!',status: 'error' , mask: false });
                                }                              
                            }
                        ).catch(function (response) {    
                            alert(response);
                        });                        
                    }                    
                })
            } else {
                this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!', status: 'warning' });
                return;
            };
        },        
        //還原
        revertEvent() {
            this.setStatusHead(false);
            this.headData = JSON.parse(JSON.stringify(this.tempHeadData));//還原主表
            this.gridData1 = JSON.parse(JSON.stringify(this.tempGridData1));//還原明細 
            this.gridData2 = JSON.parse(JSON.stringify(this.tempGridData2));
            this.headStatus = "";            
        },
        setStatusHead(blValue) {            
            this.isEditHead = blValue; //設置編輯狀態標識            
            this.isReadOnlyHead = !(blValue); //設置對象可編輯狀態
            this.isDisableDropBoxHead = !(blValue); //設置下拉列表框可編輯狀態           
            if(blValue){
                this.btnItemTitle="修改";
            }else{
                this.btnItemTitle="劉覽";
            }
        },
        clearHeadData(){
            this.headData.id="",
            this.headData.inventory_date = "", 
            this.headData.origin = "4",
            this.headData.bill_type_no="01",
            this.headData.department_id="",
            this.headData.handler = "",
            this.headData.linkman="",
            this.headData.remark = "", 
            this.headData.create_by = "", 
            this.headData.update_by = "", 
            this.headData.check_by = "", 
            this.headData.update_count = "1", 
            this.headData.create_date = "", 
            this.headData.update_date = "",
            this.headData.check_date = "",
            this.headData.state ="0",
            this.headData.tum_type ="A",
            this.headData.transfers_state ="0",
            this.headData.servername ="hkserver.cferp.dbo",
            this.headData.head_status="NEW"           
        },        
        clearRowDataEdit1(){
            this.rowDataEdit1= {
                id:'',sequence_id:'', mo_id: '', goods_id: '', goods_name: '',inventory_issuance:'',ii_code:'',ir_lot_no:null,obligate_mo_id:'',
                i_amount:0,i_weight:0,inventory_receipt:'',ir_code:'',ii_lot_no:'',ref_lot_no:'',ib_qty:0,ib_weight:0,unit:'PCS',remark:null,
                ref_id:'', jo_sequence_id:'', so_no:null,contract_cid:null,mrp_id:null, sign_by:null,sign_date:null, vendor_id:'',base_unit:'PCS',
                rate:1,state:'0',transfers_state:'0',ref_sequence_id:'',only_detail:'0',row_status:''
            }
        },
        clearRowDataEdit2(){
            this.rowDataEdit2= {
                id:'',upper_sequence:'',sequence_id:'',mo_id:'',goods_id:'',goods_name:null,unit:'PCS',inventory_issuance:'',ii_code:'',i_amount:0,i_weight:0,
                ir_lot_no:'',obligate_mo_id:'',inventory_receipt:'',ir_code:'',remark:null,ref_id:'',ref_lot_no:null,so_no:'',contract_cid:null,mrp_id:null,
                base_unit:null,rate:null,ib_qty:null,order_qty:0,ii_lot_no:null,average_cost:null,state:'0',transfers_state:'0',ib_weight:null,order_sec_qty:null,
                so_sequence_id:'',ref_sequence_id:'',inventory_date:null,department_id:null,only_detail:'0',vendor_id:null,servername:null,row_status:''
            }
        },
        setRowDataEdit1(curRow){//從領料申請明細中選中的記錄添加到發料單
            this.rowDataEdit1= {
                id:curRow.id,sequence_id:curRow.sequence_id, mo_id: curRow.mo_id, goods_id: curRow.goods_id, goods_name: curRow.goods_name,
                inventory_issuance:curRow.inventory_issuance,ii_code:curRow.ii_code,ir_lot_no:curRow.ir_lot_no,obligate_mo_id:curRow.obligate_mo_id,
                i_amount:curRow.i_amount,i_weight:curRow.i_weight,inventory_receipt:curRow.inventory_receipt,ir_code:curRow.ir_code,
                ii_lot_no:curRow.ii_lot_no,ref_lot_no:null,ib_qty:0,ib_weight:0, unit:curRow.unit,remark:curRow.remark, 
                ref_id:curRow.ref_id,jo_sequence_id:curRow.jo_sequence_id,so_no:null,contract_cid:null,mrp_id:curRow.mrp_id,sign_by:curRow.sign_by,
                sign_date:curRow.sign_date,vendor_id:curRow.vendor_id,base_unit:curRow.base_unit,rate:1,state:'0',
                transfers_state:'0',ref_sequence_id:curRow.ref_sequence_id,only_detail:0,row_status:curRow.row_status
            }
        },
        setRowDataEdit2(curRow){
            this.rowDataEdit2={
                id:curRow.id,upper_sequence:curRow.upper_sequence,sequence_id:curRow.sequence_id,mo_id:curRow.mo_id,goods_id:curRow.goods_id,goods_name:null,unit:'PCS',
                inventory_issuance:curRow.inventory_issuance,ii_code:curRow.ii_code,i_amount:curRow.i_amount,i_weight:curRow.i_weight,ir_lot_no:curRow.ir_lot_no,
                obligate_mo_id:curRow.obligate_mo_id,inventory_receipt:curRow.inventory_receipt,ir_code:curRow.ir_code,remark:null,ref_id:curRow.ref_id,ref_lot_no:null,
                so_no:curRow.so_no,contract_cid:null,mrp_id:null,base_unit:null,rate:null,ib_qty:null,order_qty:curRow.order_qty,ii_lot_no:null,average_cost:null,state:'0',
                transfers_state:'0',ib_weight:null,order_sec_qty:null,so_sequence_id:curRow.so_sequence_id,ref_sequence_id:curRow.ref_sequence_id,inventory_date:null,
                department_id:null,only_detail:'0',vendor_id:null,servername:null,row_status:'NEW'
            }
        },
        setAddRowDataEdit1(curRow){//從領料申請明細中添加過來的
            let irLotNo="",refSequenceId="",obligateMoId="";
            let arr = this.confirmData2;
            for(let i = 0; i < arr.length; i++) {
                if(curRow.key_id = arr[i].key_id){
                    irLotNo = arr[i].lot_no;
                    refSequenceId = arr[i].sequence_id;
                    obligateMoId = arr[i].obligate_mo_id;
                    break;
                }
            }
            this.rowDataEdit1 = {
                sequence_id:curRow.sequence_id, mo_id: curRow.mo_id, goods_id: curRow.materiel_id, goods_name: curRow.name,
                inventory_issuance:curRow.location,ii_code:curRow.location,ir_lot_no: irLotNo, obligate_mo_id: obligateMoId,
                i_amount:curRow.fl_qty,i_weight:curRow.sec_qty,inventory_receipt:curRow.dept_id,ir_code:curRow.dept_id,ii_lot_no: irLotNo,
                ref_lot_no:null,ib_qty:0,ib_weight:0, unit:curRow.unit,remark:curRow.remark, 
                ref_id:curRow.id,jo_sequence_id:curRow.upper_sequence,
                so_no:null,contract_cid:null,mrp_id:curRow.mrp_id,sign_by:curRow.sign_by,
                sign_date:curRow.sign_date,vendor_id:curRow.vendor_id,base_unit:curRow.unit,rate:1,state:'0',
                transfers_state:'0',ref_sequence_id: refSequenceId,only_detail:0,row_status:"NEW"
            }
        },
        //要與領料申請中的字段對應
        setAddRowDataEdit2(keyId,upperSequence) {           
            let tmp_arr = [];
            tmp_arr = this.tmpfilterOriginData2(keyId); //篩選出key_id對應的記錄
            for(let i = 0; i < tmp_arr.length; i++){
                this.rowDataEdit2 = {
                    id:'',upper_sequence:upperSequence,sequence_id:'',mo_id:tmp_arr[i].mo_id,goods_id:tmp_arr[i].materiel_id,goods_name:tmp_arr[i].name,
                    unit:tmp_arr[i].unit,inventory_issuance:tmp_arr[i].location,ii_code:tmp_arr[i].carton_code,i_amount:tmp_arr[i].fl_qty,i_weight:tmp_arr[i].sec_qty,
                    ir_lot_no:tmp_arr[i].lot_no,obligate_mo_id:tmp_arr[i].obligate_mo_id,inventory_receipt:tmp_arr[i].dept_id,ir_code:tmp_arr[i].dept_id,remark:null,
                    ref_id:tmp_arr[i].id,ref_lot_no:null,so_no:tmp_arr[i].so_order_id, contract_cid:null,mrp_id:null,base_unit:null,rate:null,ib_qty:null,
                    order_qty:tmp_arr[i].fl_qty,ii_lot_no:null,average_cost:null,state:'0',transfers_state:'0',ib_weight:null,order_sec_qty:null,
                    so_sequence_id:tmp_arr[i].so_sequence_id,ref_sequence_id:tmp_arr[i].sequence_id,inventory_date:null,department_id:null,only_detail:'0',
                    vendor_id:null,servername:null,row_status:'NEW'
                };
                this.originData2.push(this.rowDataEdit2);
            }
        },
        tmpfilterOriginData2(keyId){
            return this.confirmData2.filter(item => {                
                let str = item.key_id;
                return str.indexOf(keyId) !== -1
            })
        },
        //點擊表格一時帶出表格二,通過upper_sequence過慮
        filterOriginData2(upperSequence){            
            if (upperSequence !== '') {
                let keyword = upperSequence;
                return this.originData2.filter(item => {
                    // 以upper_sequence来搜索
                    let str = item.upper_sequence;
                    return str.indexOf(keyword) !== -1
                })
            } else {
                return this.originData2 || [];
            }
        },
        backupData () {
            this.tempHeadData = JSON.parse(JSON.stringify(this.headData));//暫存主檔臨時數據
            this.tempGridData1 = JSON.parse(JSON.stringify(this.gridData1));//暫存明細臨時數據
            this.tempGridData2 = JSON.parse(JSON.stringify(this.gridData2));//暫存明細臨時數據
        },
        //新增明細
        insertRowEvent() {
            if(this.headStatus ==""){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            if (this.headData.id == "") {
                this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!', mask: false });
                return;
            };
            let rowEndIndex = -1;//添加至行尾
            let $table = this.$refs.xTable1;
            this.clearRowDataEdit1();
            this.selectRow = null;
            this.$set(this.rowDataEdit1, "id", this.headData.id);  
            this.$set(this.rowDataEdit1, "sequence_id", this.getSequenceId($table.tableData.length));
            this.$set(this.rowDataEdit1, "row_status", 'NEW');

            $table.insertAt(this.rowDataEdit1,rowEndIndex).then(({ row }) => {
                $table.setActiveCell(rowEndIndex, 'mo_id');
            });

            //顯示彈窗
            this.cartonCodeList=null;
            let index = $table.tableData.length-1;//新增的行號索引
            this.curRowIndex = index; //記錄新增的行號索引
            let rw = $table.tableData[index];  //取新增行對象值
            this.setRowDataEdit1(rw);
            this.selectRow = rw; //將當前行賦值給彈窗
            this.showEdit = true;
            
        },
        saveEvent(){           
            if(this.headData.id==="" && this.headData.department_id===""){
                //mask: false 彈窗時父窗口不需庶置
                this.$XModal.alert({ content: '編號不可以為空!', mask: false });
                return;
            }
            if(this.headData.transfer_date ===""){
                this.$XModal.alert({ content: '轉入日期不可以為空!', mask: false });
                return;
            }            
            const $table = this.$refs.xTable1;
            if($table.tableData.length == 0){
                this.$XModal.alert({ content: '明細資料一不可以為空!', mask: false });
                return;
            }
            const $table2 = this.$refs.xTable2;
            if($table2.tableData.length == 0){
                this.$XModal.alert({ content: '明細資料二不可以為空!', mask: false });
                return;
            }
            this.saveAll();
        }, 
        editRowEvent1(row){
            //this.rowDataEdit = row; //不可以直接賦row給this.rowDataEdit
            this.setRowDataEdit1(row);
            this.selectRow = row;
            this.showEdit = true;
        },
        //暫時將當前行的修改更新至表格
        tempUpdateRowEvent(){           
            if(this.headSatus ===""){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            Object.assign(this.selectRow, this.rowDataEdit1);
            //this.$set($table.tableData[1], this.rowDataEdit);
            this.showEdit = false;
        },
        //撤消對當前行的修改,不影響到表格
        tempUndoRowEvent(){
            this.clearRowDataEdit1();
            this.showEdit = false;
        },       
               
        //項目刪除
        tempDelRowEvent:async function(){
            if(this.headData.state !='0'){
                this.$XModal.alert({ content: "此單據已批準,當前操作無效!", mask: false });
                return;
            }
            let status = await comm.checkApproveState('st_inventory_mostly',this.headData.id);
            if(status ==='1'||status ==='2'){
                let str = (status ==='1')?"批準":"注銷";
                this.$XModal.alert({ content: `后端數據已是${str}狀態,當前操作無效!`, mask: false });
                return;
            }
            if(this.curDelRow){
                let $table = this.$refs.xTable1;
                if($table.tableData.length===1){
                    //if(this.gridData1.length===1){
                    this.$XModal.alert({ content: "已是最后一行,不可以繼續刪除!", mask: false });
                    return;
                }
                this.$XModal.confirm("是否確定要刪除當前行？").then(type => {                    
                    if(type ==="confirm"){
                        //刪除表格2原始表中對應的數據                        
                        let lenths = this.originData2.length - 1;                        
                        for( let i=lenths;i>=0;i--){
                            if(this.originData2[i].upper_sequence === this.selectRow.sequence_id){                                    
                                this.originData2.splice(i,1);//移除
                            }
                        }
                        if(this.selectRow.row_status ==='NEW'){
                            //移除表格二數組中的元素
                            this.gridData2=[];
                        }else{
                            //記錄需刪除表格2后臺已有的對應數據                            
                            for(let index in this.gridData2){
                                if(this.gridData2[index].row_status != "NEW"){
                                    this.delData2.push(this.gridData2[index]);
                                }
                            }
                            this.tableData2 = [];
                            //記錄需刪除表格1的后臺數據
                            this.delData1.push(this.selectRow);         
                        }
                        //this.gridData1.splice(this.curRowIndex,1);//移除表格1刪除的當前行
                        $table.tableData.splice(this.curRowIndex,1);//移除表格1刪除的當前行
                        this.isEditHead = true;
                        this.curDelRow = null;
                    }
                })
            }else{
                this.$XModal.alert({ content: '請首先指定需要刪除的項目!',mask: false });
            }
        },
        
        ////轉出倉貨品對應的批號        
        //getLotNoList(location_id,mo_id,goods_id) {
        //    axios.get("/Stor/ChangeStore/GetLotNoList?location_id=" + location_id+"&mo_id="+mo_id+"&goods_id="+goods_id).then(
        //        (response) => {
        //            this.lotNoList = response.data;
        //        },
        //        (response) => {
        //            alert(response.status);
        //        }
        //    ).catch(function (response) {
        //        alert(response);
        //    });
        //},
        
        //批準&反批準 val: 1--批準,0--反批準
        approveEvent:async function (val) {
            if ((this.headData.id != "") && (this.gridData1.length > 0)) {
                //批準,反批準都要檢查是否是註銷狀態
                if(this.headData.state === "2"){
                    this.$XModal.alert({ content: '注銷狀態,當前操作無效!', mask: false });
                    return;
                }
                let ls_success= (val==='1')?"批準成功!":"反批準成功!";
                let ls_error= (val==='1')?"批準失敗!":"反準失敗!";
                let ls_type = (val==='1')?"批準":"反批準";
                let ls_is_approve = `確定是否要進行【${ls_type}】操作？`;
                //獲取后端單據狀態
                let status = comm.checkApproveState('st_inventory_mostly',this.headData.id);
                if(status==='2'){
                    this.$XModal.alert({ content: "后端數據已是注銷狀態,當前操作無效!", mask: false });
                    return;
                }
                if(val==='1') {//批準時                   
                    //檢查是否已批準
                    if(this.headData.state === "1"){
                        this.$XModal.alert({ content: '已是批准狀態,當前操作無效!', mask: false });
                        return;
                    }
                    if(status==="1"){
                        //后臺數據已為批準狀態,已被別的用戶批準
                        this.$XModal.alert({ content: "后端數據已是批準狀態,當前操作無效!", mask: false });
                        return;
                    }
                    //批準前檢查庫存                    
                    let strError = await this.checkStock(this.headData.id); //此處必須加await,且checkStock函數也要設置成同步執行                    
                    if(this.validStockFlag===false){
                        //檢查庫存不足,返回并放棄當前保存
                        this.$XModal.alert({ content: strError, mask: false });
                        return;
                    }
                }
                if(val==='0'){ 
                    //進行反批准
                    //進行當前反批準操作前再次檢查后端是否已被別的用戶反批準.
                    if(status==="0"){
                        //后臺數據已是未批準狀態,已被別的用戶反批準
                        this.$XModal.alert({ content: "后端數據已是未批準狀態,當前操作無效!", mask: false });
                        return;
                    }
                    ////設置后端服務器日期存儲在this.server_date
                    //this.getDateServer();
                    ////檢查已批準日期是否為當日,超過當日則不可反批準                  
                    ////將批準日期(字符串)轉換為對象
                    //let objCheckDate = new Date(this.headData.check_date);
                    ////再格式化為統一的日期字符串格式(yyyy-MM-dd)
                    //let check_date = comm.getDate(objCheckDate, 0);
                    //if(check_date != this.server_date){
                    //    this.$XModal.alert({ content: '注意:【批準日期】必須為當前日期,方可進行此操作!', mask: false });
                    //    return;
                    //}
                    //檢查已批準日期是否為當日,超過當日則不可反批準
                    let isApprove = await comm.canApprove(this.headData.id,"w_changestore_cc");//倉庫發料
                    if(isApprove ==="0"){
                        this.$XModal.alert({ content: "注意:【批準日期】必須為當前日期,方可進行此操作!", mask: false });
                        return;
                    }
                    //對方已经签收了的单据不能再反批准
                    let signFor = await comm.checkSignfor(this.headData.id,"ChangeStore");
                    if(signFor != "0"){
                        this.$XModal.alert({ content: "接收貨部門已簽收,不可以再進行反批準操作!", mask: false });
                        return;
                    }
                }
                this.$XModal.confirm(ls_is_approve).then(type => {
                    if (type == "confirm") {
                        this.headData.check_by = this.userId;
                        let head = JSON.parse(JSON.stringify(this.headData));
                        let approve_type = val;
                        let user_id = this.userId;
                        axios.post("/ChangeStore/Approve",{head,user_id,approve_type}).then(
                            (res) => {
                                if(res.data ==="OK"){
                                    this.setStatusHead(false);
                                    //重查刷新數據
                                    this.getHead(this.headData.id);  
                                    this.$XModal.message({ content: ls_success, status: 'success' });
                                }else{                                    
                                    this.$XModal.message({ content: ls_error,status: 'warning' , mask: false });
                                }                              
                            }
                        ).catch(function (response) {
                            this.$XModal.alert({ content: "系統錯誤:" + response,status: 'error' , mask: false });                            
                        });
                   }
               })
         } else {
               this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!', status: 'warning' });
               return;
          };
        },
        //彈窗中確認修改
        setModify() {           
            if(this.headStatus===""){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            if(this.rowDataEdit1.row_status != 'NEW'){
                this.rowDataEdit1.row_status='EDIT';//編輯標記
            }
            let strItem = this.rowDataEdit1.goods_id;           
            if(strItem.substring(0, 2)==='ML'){
                this.$set(this.rowDataEdit1,"unit","KG");
                this.$set(this.rowDataEdit1,"base_unit","KG");
            }            
            Object.assign(this.selectRow, this.rowDataEdit1);
            this.showEdit = false;
        },
        dbclickEvent(row){
            //console.log(row.data[row.$rowIndex]);//取得行對象
            //this.rowDataEdit = row.data[row.$rowIndex]; //此方式是對像,彈窗更改,表格也跟著改
            //this.rowDataEdit = JSON.parse(JSON.stringify(row.data[row.$rowIndex]));
            let rw = row.data[row.$rowIndex];
            this.setRowDataEdit(rw);            
            this.selectRow = rw;
            this.showEdit = true;
        },
        cellClickEvent(row){
            this.Rows = row;//表格的數據對象
            this.curRowIndex = row.$rowIndex;//當前行的索引
            this.selectRow = row.data[row.$rowIndex];//表格一中的當前行對象           
            this.curDelRow = row.data[row.$rowIndex];//存放可能冊除的當前行對象
            if(this.headData.state==='0'){
                this.tempSelectRow1 = JSON.parse(JSON.stringify(this.selectRow));//暫存當前行,以便檢查是否有更改
            }           
            this.setRowDataEdit1(this.selectRow); //將當前行與編輯窗口數據對象關聯
            //this.showGoodsY = row.$event.clientY;            
            this.curRowSeqId = this.selectRow.sequence_id;
            this.getFilteredTableData2(this.curRowSeqId);
        },
        cellClickEvent2(row){                   
            this.selectRow2 = row.data[row.$rowIndex];//表格二中的當前行對象 
            if(this.headData.state==='0'){
                this.tempSelectRow2 = JSON.parse(JSON.stringify(this.selectRow2));//暫存當前行,以便檢查是否有更改
            }
            //this.showGoodsY = row.$event.clientY;
        },
        getHead(id) {
            //此處的URL需指定到祥細控制器及之下的動Action,否則在轉出待確認彈窗中的查詢將數出錯,請求相對路徑的問題
            axios.get("/ChangeStore/GetHeadByID", { params: { id: id }}).then(
                (response) => {                
                    this.headData = {
                        id: response.data.id,
                        inventory_date: response.data.inventory_date,
                        department_id: response.data.department_id,
                        linkman: response.data.linkman,
                        handler: response.data.handler,
                        origin: response.data.origin,
                        bill_type_no: response.data.bill_type_no,
                        remark: response.data.remark,
                        create_by: response.data.create_by,
                        update_by: response.data.update_by,
                        check_by: response.data.check_by,
                        update_count: response.data.update_count,
                        create_date: response.data.create_date,
                        update_date: response.data.update_date,
                        check_date: response.data.check_date,
                        state: response.data.state
                    }
                    if(response.data.state==='2'){
                        this.stateFontColor = "color:red";
                    }else{
                        this.stateFontColor = "color:black";
                    }
                    //深度複製一個對象，用來判斷數據是否有修改
                    //this.prevForm = JSON.parse(JSON.stringify(this.headData));                    
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        getDetails: async function (id) {
            var _self = this;
            await axios.get("/ChangeStore/GetDetailsByID", { params: { id: id }  }).then(
                (response) => {
                    this.gridData1 = response.data;
                    if(response.data.length>0){
                        //默認根據表格一的第一行,過慮出表格二對應序號的數據    
                        this.curRowSeqId = response.data[0].sequence_id;
                        axios.get("/ChangeStore/GetDetailsPartByID", { params: { id: id }  }).then(
                            (response) => {
                                this.originData2 = response.data;//全部數據暫存到臨時變量中                            
                                this.getFilteredTableData2(this.curRowSeqId);                            
                            }
                        ).catch(function (response) {
                            alert(response);
                        });
                    }                    
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        getDetailsPartByID (id) {
            var _self = this;            
            axios.get("/ChangeStore/GetDetailsPartByID", { params: { id: id }  }).then(
                (response) => {
                    this.originData2 = response.data;//全部數據暫存到臨時變量中                    
                    this.getFilteredTableData2(this.curRowSeqId);                    
                }
            ).catch(function (response) {
                alert(response);
            });
        },

        //開啟公共查詢頁面
        showQueryEvent(id) {            
            var url = "/Base/PublicQuery?window_id=" + id;
            comm.showMessageDialog(url, "查詢", 1024, 768, true);
        },        
        saveAll: async function() {
              const $table = this.$refs.xTable1;
              const errMap = $table.fullValidate().catch(errMap => errMap);
              let check_tableData1_flag = true;
              await errMap;//同步操作,等待校驗結果
              errMap.then(res=>{
                  if(res){ //res为promise對象中的值,如果值是對象,說明數據校驗通不過
                      check_tableData1_flag = false;
                      this.$XModal.alert({ content: '注意:明細數據不完整,請返回檢查!', mask: false });
                  }
              });
              await check_tableData1_flag;
              if(!check_tableData1_flag){
                  return;
              }
              //檢查表格二數據完整性
              let upperSequence="";
              for(let i=0;i<this.originData2.length;i++){
                  if(this.originData2[i].ir_lot_no==""){
                      upperSequence=this.originData2[i].upper_sequence;
                      check_tableData1_flag = false;
                      break;
                  }
                  if(this.originData2[i].i_amount==0){
                      upperSequence=this.originData2[i].upper_sequence;
                      check_tableData1_flag = false;
                      break;
                  }
                  //
                  if(this.originData2[i].i_weight==0){
                      upperSequence=this.originData2[i].upper_sequence;
                      check_tableData1_flag = false;
                      break;
                  }
              }
              if(!check_tableData1_flag){                  
                  this.$XModal.alert({ content: `注意:表格一第${upperSequence}行對應明細表二的數據不完整,請返回檢查!`, mask: false });
                  return;
              }
              
              //start 檢查當前用戶是否有負責部門的操作權限
              let depArr = [];
              let obj = {};
              let items = $table.tableData;
              items.forEach(p => {
                  if(!obj[p.inventory_issuance]){
                      depArr.push(p.inventory_issuance);
                      obj[p.inventory_issuance] = true;
                  }
              })           
              let user = this.userId;
              let deptId = "";
              let rights = "";            
              for (let i = 0; i < depArr.length; i++) {
                  deptId = depArr[i] ;
                  rights = await comm.checkUserDeptRights(user,deptId);
                  if(rights != "1")
                  {
                      break;//终止循环
                  }
              }           
              if(rights != "1"){
                  this.$XModal.alert({ content: `當前用戶: 【${user} 】,無負責部門【${deptId}】操作權限!`,status: 'info' , mask: false });
                  return;
              }
              //end

              /*
              //檢查成份庫存是否夠扣減
              //傳給后端存儲過程的表數據類型參數結構
              this.partData = [];
              for(let i in this.originData2){
                  this.partData.push(
                      {within_code:'0000',out_dept:this.originData2[i].inventory_issuance,upper_sequence:this.originData2[i].upper_sequence,sequence_id:this.originData2[i].sequence_id,
                      mo_id:this.originData2[i].mo_id,goods_id:this.originData2[i].goods_id,lot_no:this.originData2[i].ir_lot_no,con_qty:this.originData2[i].i_amount,
                      sec_qty:this.originData2[i].i_weight});
              }
              let partData = this.partData;
              let strError = await this.checkStock(partData); //此處必須加await,且checkStock函數也要設置成同步執行
              if(this.validStockFlag===false){
                  //檢查成份庫存不足,返回放棄當前保存
                  this.$XModal.alert({ content: strError, mask: false });
                  return;
              }*/

              //START 2024/03/01              
              this.tempSaveData = [];
              let items_update = null;
              for(let i=0;i<$table.tableData.length;i++){
                  items_update = $table.tableData[i];
                  if(items_update.row_status==='NEW' || items_update.row_status==='EDIT'){
                      this.tempSaveData.push(items_update);
                  }
              }             
              //END 2024/03/01
              this.headData.head_status = this.headStatus;//表頭新增或修改的標識
              let headData = JSON.parse(JSON.stringify(this.headData));             
              //let lstDetailData1 = this.gridData1;//舊的方法取不到值
              let lstDetailData1 = this.tempSaveData;//新的方法
              let lstDetailData2 = this.originData2;
              let lstDelData1 = this.delData1;
              let lstDelData2 = this.delData2; 

              //const items= { insertRecords, removeRecords} = $table.getRecordset();
              //if(items.insertRecords.length>0){
              //  for(let i=0;i<items.insertRecords.length;i++){
              //      this.$set(items.insertRecords[i], "row_status", "NEW" );
              //      this.tempSaveData.push(items.insertRecords[i]);
              //  }
              //}
              //if(items.removeRecords.length>0){
              //  for(let i=0;i<items.removeRecords.length;i++){
              //      this.$set(items.removeRecords[i], "row_status", "DEL" );
              //      this.tempSaveData.push(items.removeRecords[i]);
              //  }
              //}

              //this.headData.head_status = this.headStatus;//表頭是新增或者修改
              //let head = JSON.parse(JSON.stringify(this.headData));
              //let detailData = this.tempSaveData;    lstDetailData1
              let user_id = this.userId;
              axios.post("/ChangeStore/Save",{headData,lstDetailData1,lstDetailData2,lstDelData1,lstDelData2,user_id}).then(
                (response) => {
                    if(response.data=="OK"){
                        this.setStatusHead(false);
                        //重查刷新數據
                        this.getHead(this.headData.id);
                        this.getDetails(this.headData.id);  
                        this.getDetailsPartByID(this.headData.id);  
                        this.$XModal.message({ content: '數據保存成功!', status: 'success' });
                    }else{
                        this.$XModal.alert({ content: '數據保存失敗!',status: 'error' , mask: false });                                 
                    }
                }
              ).catch(function (response) {
                 alert(response);
              });
              this.headStatus = "";              
        },        
        //批準前檢查庫存是否夠扣除
        checkStock:async function(id){
            let rtn="";
            await axios.post("/ChangeStore/CheckStock",{id}).then((response) => {
                    if(response.data.length>0){
                        //庫存檢查不通過
                        let seqNo = response.data[0].sequence_id;
                        let con_qty = response.data[0].con_qty;
                        let sec_qty = response.data[0].sec_qty;
                        let mo_id = response.data[0].mo_id;
                        let goods_id = response.data[0].goods_id;
                        let row_no=0;
                        this.validStockFlag = false;                       
                        this.tableData = JSON.parse(JSON.stringify(this.gridData1));
                        for(let i=0;i<this.tableData.length;i++){
                            if(this.tableData[i].sequence_id === seqNo){
                                row_no = i+1;
                                let rw = this.tableData[i];//如果按this.rows.data[i]取值有可能出錯,因些時有可能還未點擊表格而未能觸發cellClick事件.this.rows是空的對象從而引起錯誤
                                this.tempSelectRow = JSON.parse(JSON.stringify(rw));//暫存當前行,以便監控器監測以后該行是否有改動
                                this.$refs.xTable1.setCurrentRow(rw);//定位至當前索引所在的行
                                this.rowDataEdit = rw;
                                this.selectRow = rw;                               
                                break;
                            }
                        } 
                        rtn =`第 ${row_no} 行【${mo_id}】【${response.data[0].goods_id}】庫存不足!\n 数量差異: ${con_qty}\n 重量差異: ${sec_qty}`;
                        //this.$XModal.alert({ content: `第 ${row_no} 行【${this.selectRow.mo_id} -- ${this.selectRow.goods_id}】\n成份:【${response.data[0].goods_id}】庫存不足!\n 数量差異: ${con_qty}\n 重量差異: ${sec_qty}`,status: 'warning' , mask: false });
                    }else{
                        this.validStockFlag = true;//庫存檢查通過
                    }
            }) 
            return rtn;
        },

             
        //服務器端日期
        getDateServer: async function() {
            let res = await axios.get("/Base/DataComboxList/GetDateServer");
            this.server_date = res.data;
        },
        //點擊表格一時表格二跟著變化
        getFilteredTableData2 (sequenceId) {
            this.gridData2 = [];
            if (sequenceId !='') {
                this.gridData2 = this.filterOriginData2(sequenceId); //cancel 2024/01/22                
                //console.log(this.gridData2);
            }
        },
        filterOriginData2(sequenceId){
            if (sequenceId !== '') {
                let keyword = sequenceId;// this.selectRow.sequence_id;               
                return this.originData2.filter(item => {
                    //搜索
                    let str = item.upper_sequence;
                    return str.indexOf(keyword) !== -1;
                })
            } else {
                return this.originData2 || [];
            }
        },
        showLotNo:async function(event){           
            if(this.headData.state==='0') {//單元格為可編輯狀態時
                let location_id = this.selectRow2.inventory_issuance;
                let goods_id = this.selectRow2.goods_id;
                let mo_id = this.selectRow2.obligate_mo_id;                
                if(this.$utils.isEmpty(goods_id)){
                    this.$refs.xTable2.setActiveCell(this.selectRow2, "goods_id");
                    return;
                }
                
                this.seledtLotNoRow = null;
                //this.showGoodsX = event.clientX;
                //提取批号数据 //批號下拉列表框
                await axios.get("/Base/Common/GetStDetailsLotNo", { params: { location_id:location_id,goods_id:goods_id,mo_id:mo_id } }).then(
                    (response) => {
                        this.lotNoList = response.data;
                        //this.showLot = true;//顯示彈窗
                    }
                ).catch(function (response) {
                    this.lotNoList=[];
                    alert(response);
                });

                this.tableLotNoList = this.lotNoList;
                let $pulldown = this.$refs.pulldownRef;
                if ($pulldown) {
                    $pulldown.togglePanel();
                    //$pulldown.showPanel();
                }
            }
        },
        //cellClickGetLotNo(row){
        //    this.seledtLotNoRow = row.data[row.$rowIndex];
        //},
        //changeLotNo() {
        //    if(this.seledtLotNoRow){
        //        if(this.selectRow2.ir_lot_no != this.seledtLotNoRow.lot_no){
        //            this.$set(this.selectRow2, "ir_lot_no", this.seledtLotNoRow.lot_no);
        //            this.$set(this.selectRow, "ir_lot_no", this.seledtLotNoRow.lot_no);
        //            this.$set(this.selectRow, "ii_lot_no", this.seledtLotNoRow.lot_no);
        //        }
        //        this.showLot = false;
        //    }else{
        //        this.$XModal.alert({ content: '請指定批號!', mask: false });
        //    }
        //},
        //取最大單據號
        getMaxID:async function() {
            //var $table = this.$refs.xTable1;
            await axios.get("/Base/Common/GetMaxIDStock?bill_id=" + 'ST03' + "&serial_len=4" ).then(
                (response) => {
                    this.headData.id = response.data;
                }
            );
        }, 
        closePage(){
            if(this.headStatus !=""){
                this.$XModal.confirm('當前頁面是編輯狀態,尚未保存,是否確定要關閉當前頁面?').then(type => {
                    if (type == 'confirm') {
                        if(this.flagChild ==="1"){
                            parent.comm.closeWindow();//关闭窗口
                        }
                    }
                })
            }else{
                if(this.flagChild ==="1"){
                    parent.comm.closeWindow();
                }
            }
        },
        cellLotNoClick(row){//批號
            let $pulldown = this.$refs.pulldownRef;
            if ($pulldown) {
                let rowIndex = row.$rowIndex;
                let mo_id = row.data[rowIndex].mo_id;  
                let ir_lot_no = row.data[rowIndex].lot_no;                              
                this.$set(this.selectRow2,"ir_lot_no",ir_lot_no );                
                if(this.selectRow2.row_status===""){
                    this.$set(this.selectRow2,"row_status", "EDIT");
                }
                this.$set(this.selectRow,"ir_lot_no",ir_lot_no );
                this.$set(this.selectRow,"ii_lot_no",ir_lot_no );
                this.$set(this.selectRow,"obligate_mo_id",mo_id);
                if(this.selectRow.row_status===""){
                    this.$set(this.selectRow,"row_status", "EDIT");
                }
                $pulldown.hidePanel();
            }
        },
        pageChangeEvent({ currentPage, pageSize }) {
            this.pagerConfig.currentPage = currentPage;
            this.pagerConfig.pageSize = pageSize;
        },      
        headerCellStyle(){
            return `background:'#F5F7FA',color:'#606266',height:'25px',padding:'2px'`;
        },


    }, //END OF METHOND

   
    watch: {
        searchID : function (val) {
            this.searchID = val.toUpperCase();
        },
        rowDataEdit1: {
            handler (val, oldVal) {
                //小寫轉大寫
                if(this.rowDataEdit1.mo_id){
                    this.rowDataEdit1.mo_id = this.rowDataEdit1.mo_id.toUpperCase();
                }
                if(this.rowDataEdit1.goods_id){
                    this.rowDataEdit1.goods_id = this.rowDataEdit1.goods_id.toUpperCase();
                }               
            },
            deep: true
        }, //end of rowDataEdit1
        selectRow2: {            
            handler (val, oldVal) {
                if(this.headData.state ==='0'){
                    let $table2 = this.$refs.xTable2;
                    let rs = $table2.tableData.length;
                    //更改了表格二中的數量或重量                
                    //更改數量                               
                    if(this.tempSelectRow2.mo_id===undefined){
                        return;
                    }
                    
                    if(this.tempSelectRow2.i_amount != this.selectRow2.i_amount ){
                        if(rs===1){//表格二只有一條記錄                       
                            this.$set(this.rowDataEdit1,"i_amount",this.selectRow2.i_amount);                                    
                        }else{
                            //加總表格二中的數量
                            let i_amount=0;
                            for (let i = 0; i < rs; i++) {                               
                                i_amount = i_amount + parseFloat($table2.tableData[i].i_amount);//parseFloat(response.data[0].sec_qty);
                            }
                            this.$set(this.rowDataEdit1,"i_amount",i_amount );
                        }                                
                        if(this.rowDataEdit1.row_status===""){  
                            this.$set(this.rowDataEdit1,"row_status", "EDIT");
                        }
                        Object.assign(this.selectRow, this.rowDataEdit1);//將值寫入對象,這樣才會刷新表格的值   
                        if(this.selectRow2.row_status===""){
                            this.$set(this.selectRow2,"row_status","EDIT");
                        }
                    }
                    //更改重量
                    if(this.tempSelectRow2.i_weight != this.selectRow2.i_weight ){
                        if(rs===1){//表格二只有一條記錄
                            this.$set(this.rowDataEdit1,"i_weight",this.selectRow2.i_weight );
                        }else{
                            let i_weight=0;
                            for (let i = 0; i < rs; i++) {
                                i_weight = i_weight + parseFloat($table2.tableData[i].i_weight);
                            }
                            this.$set(this.rowDataEdit1,"i_weight",i_weight );
                        }
                        if(this.rowDataEdit1.row_status===""){  
                            this.$set(this.rowDataEdit1,"row_status", "EDIT");
                        }
                        Object.assign(this.selectRow, this.rowDataEdit1);//將值寫入對象,這樣才會刷新表格的值
                        if(this.selectRow2.row_status===""){
                            this.$set(this.selectRow2,"row_status","EDIT");
                        }
                    }
                }
            },
            deep: true
        }// end of rowDataEdit2   
    },// end of watch
    computed: {
        //
    },
    mounted() {
        if(this.flagChild ==="1"){
            this.tableHeight1=300; //add 2024/03/01
            this.tableHeight2 =200; //add 2024/03/01
            //this.tableHeight2=$(parent.window).height()-(172+300+35); //默認表格二高度500 2024/03/01 cancel            
            //this.div_tab1_height = $(parent.window).height()-155 +'px';//第一页Div的高度
            let that = this; 
            window.onresize = () => {
                return (() => {
                    //that.tableHeight2=$(parent.window).height()-(172+300+35);//500 2024/03/01 cancel
                    //that.div_tab1_height = $(parent.window).height()-155 +'px';
                    that.tableHeight1=300;
                    that.tableHeight2=200                    
                })()
            }; 
           
            let $table1 = this.$refs.xTable1;
            let str_key_id = "",str_upper_sequence = "";
            //-1 為添加至行尾 
            this.originData2 = [];
            for (let i = 0; i < this.confirmData1.length; i++) {
                this.clearRowDataEdit1();
                str_key_id = this.confirmData1[i].key_id;
                this.setAddRowDataEdit1(this.confirmData1[i]);
                this.rowDataEdit1.sequence_id = this.getSequenceId(i); //生成序號作為表格二的upper_sequence
                $table1.insertAt(this.rowDataEdit1,-1).then(({ row }) => {
                    $table1.setActiveCell(-1,'mo_id');
                });
                this.clearRowDataEdit2();
                //主表的sequence_id就是從表格的upper_sequence,再根據主表key_id,找出從表的key_id對應的數據
                this.setAddRowDataEdit2(str_key_id,this.rowDataEdit1.sequence_id);
            }
            //更新單據最大編號
            axios.get("/Base/Common/GetMaxIDStock?bill_id=" + 'ST03' + "&serial_len=4" ).then(
                (response) => {
                    this.headData.id = response.data;
                    for (let i = 0; i < $table1.tableData.length; i++) {
                        this.$set($table1.tableData[i], "id", this.headData.id );                        
                    }
                    let arr = this.originData2;
                    for (let i = 0; i < arr.length; i++) {
                        arr[i].id = this.headData.id ;
                        arr[i].sequence_id = this.getSequenceId(i);                                                
                    }                    
                }
            );
            //某單元格獲取焦點            
            if( $table1.tableData.length>0){
                $table1.setCurrentRow($table1.tableData[0]);  // :row-config="{height: 25,  isCurrent:true}"                
                this.getFilteredTableData2($table1.tableData[0].sequence_id);               
                //$table1.getActiveRecord($table1.tableData[0]);//此方法無效果,新版本解決此問題可能
                
                //設置默認選中的第一行的值
                this.selectRow = $table1.tableData[0];
                this.curRowIndex = 0;//當前行的索引               
                this.curDelRow = $table1.tableData[0];//存放可能冊除的當前行對象
                this.curRowSeqId = this.selectRow.sequence_id;
                if(this.headData.state==='0'){
                    this.tempSelectRow1 = JSON.parse(JSON.stringify(this.selectRow));//暫存當前行,以便檢查是否有更改
                }
            }
        }else{
            this.tableHeight1=400;
            this.tableHeight2=$(parent.window).height()-(172+400+35);//205;
        }
        
        let that = this;       
        window.onresize = () => {
            return (() => {
                that.tableHeight2=$(parent.window).height()-(172+400+35);//205;                 
            })()
        };
    }
}

var app = new Vue(MainIn).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;
Vue.prototype.$utils = XEUtils;

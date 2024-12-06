var MainIn = {
    data() {
        return {            
            searchID:"", 
            headStatus:"",//主檔為新增或編輯
            tableData:[],
            curRowIndex:null,
            selectRow: null,
            curDelRow:null,
            loading: false,           
            showEdit: false,           
            submitLoading: false,            
            showEditStore:false,
            showFind:false,
            isEditCell:false,
            server_date:"", 
            stateFontColor:"color:black",            
            tableHeight1 :400,           
            btnItemTitle:"劉覽",           
            headData: { id: '', inventory_date: '', origin: '1', state: '0',bill_type_no:'01',department_id:'',linkman:'',handler:'',remark: '',create_by: '', 
                create_date: '', update_by: '',update_date: '',check_by: '', check_date: '', tum_type:'B',update_count:'1',transfers_state:'0',
                servername:'hkserver.cferp.dbo',head_status:'' },
            rowDataEdit1: { id:'',sequence_id:'',mo_id:'',goods_id:'',goods_name:'',inventory_issuance:'',ii_code:'',ir_lot_no:null,obligate_mo_id:'',
                i_amount:0,i_weight:0,inventory_receipt:'',ir_code:'',ii_lot_no:'',ref_lot_no:'',ib_qty:0,ib_weight:0,unit:'PCS',remark:null,
                ref_id:'', jo_sequence_id:'', so_no:null,contract_cid:null,mrp_id:null, sign_by:null,sign_date:null, vendor_id:'',base_unit:'PCS',rate:1,state:'0',
                transfers_state:'0',ref_sequence_id:'',only_detail:'0',row_status:'' },           
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
            originList:[],
            billTypeNoList:[],
            deptList: [],
            stateList: [],           
            qtyUnitList: [],            
            locationList: [],
            changeStoreGoodsList:[],
            gridData1: [],           
            delData1:[],
            Rows:[],
            tempSelectRow1:{},
            showLot:false,
            validStockFlag:true,           
            formDataFind:{},
            isEditHead: false,//主檔編輯狀態
            isEditDetail: false,//明細編輯狀態
            isReadOnlyHead: true,//主檔對象只讀狀態
            isReadOnlyDetail: true,//明細對象只讀狀態
            isDisableDropBoxHead: true,//主檔下拉列表框失效狀態
            isDisableDropBoxDetail: true,//明細下拉列表框失效狀態
            userId:"",            
            locationId:'',           
            tempHeadData: {},
            tempGridData1: {},
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
                { field: 'goods_id', title: '貨品編號',width:200 },
                { field: 'goods_name', title: '貨品名稱' }
            ],
            tableGoodsList: [{goods_id:'',goods_name:''}],//2024/01/22 orign code: tableLotNoList: []
            tableColumnLotNo: [
                { field: 'lot_no', title: '批號' },
                { field: 'qty', title: '數量' },
                { field: 'sec_qty', title: '重量' },
                { field: 'mo_id', title: '頁數' },
                { field: 'vendor_name', title: '供應商名稱' },
            ],
            tableLotNoList: [{lot_no:'',qty:0,sec_qty:0,mo_id:0,vendor_name:''}],
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
        this.userId = $("#user_id").val();
        this.tempHeadData = this.headData;
        this.tempGridData1 = this.gridData1;       
    },
    methods: {
        findByID() {
            if(this.headStatus != "" || this.searchID ===""){
                return;
            }                      
            this.curDelRow = null;
            this.loading = true;
            setTimeout(() => {
                this.getHead(this.searchID);
                this.getDetails(this.searchID);
                this.loading = false;
            }, 500);            
        },
        showFindWindos(){
            comm.openWindos('ChangeStoreCe');
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
            //新增后初始化相關對象的初始值            
            var d = new Date();//生成日期對象:Fri Oct 15 2021 17:51:20 GMT+0800
            var date = comm.getDate(d, 0);//轉成年月日字符串格式            
            var dateTime = comm.datetimeFormat(d, "yyyy-MM-dd hh:mm:ss");
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
                        var head = JSON.parse(JSON.stringify(this.headData));                        
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
            this.headStatus = "";            
        },
        setStatusHead(blValue) {            
            this.isEditHead = blValue; //設置編輯狀態標識            
            this.isReadOnlyHead = !(blValue); //設置對象可編輯狀態
            this.isDisableDropBoxHead = !(blValue); //設置下拉列表框可編輯狀態
            this.btnItemTitle = (blValue)?"修改":"劉覽";
        },
        clearHeadData(){
            this.headData.id="",
            this.headData.inventory_date = "", 
            this.headData.origin = "1",
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
            this.headData.tum_type ="B",
            this.headData.transfers_state ="0",
            this.headData.servername ="hkserver.cferp.dbo",
            this.headData.head_status="NEW"
        },        
        clearRowDataEdit1(){
            this.rowDataEdit1= {
                id:'',sequence_id:'', mo_id: '', goods_id: '', goods_name: '',inventory_issuance:'',ii_code:'',ir_lot_no:null,obligate_mo_id:'',
                i_amount:0,i_weight:0,inventory_receipt:'',ir_code:'',ii_lot_no:'',ref_lot_no:'',ib_qty:0,ib_weight:0,unit:'PCS',remark:null,
                ref_id:'', jo_sequence_id:'', so_no:null,contract_cid:null,mrp_id:null, sign_by:null,sign_date:null, vendor_id:'',base_unit:'PCS',
                rate:1,state:'0',transfers_state:'0',ref_sequence_id:'',only_detail:'0',row_status:'' }
        },
        setRowDataEdit1(curRow) {
            this.rowDataEdit1={
                id:curRow.id,sequence_id:curRow.sequence_id, mo_id: curRow.mo_id, goods_id: curRow.goods_id, goods_name: curRow.goods_name,
                inventory_issuance:curRow.inventory_issuance,ii_code:curRow.ii_code,ir_lot_no:curRow.ir_lot_no,obligate_mo_id:curRow.obligate_mo_id,
                i_amount:curRow.i_amount,i_weight:curRow.i_weight,inventory_receipt:curRow.inventory_receipt,ir_code:curRow.ir_code,
                ii_lot_no:curRow.ii_lot_no,ref_lot_no:null,ib_qty:0,ib_weight:0, unit:curRow.unit,remark:curRow.remark, 
                ref_id:curRow.ref_id,jo_sequence_id:curRow.jo_sequence_id,so_no:null,contract_cid:null,mrp_id:curRow.mrp_id,sign_by:curRow.sign_by,
                sign_date:curRow.sign_date,vendor_id:curRow.vendor_id,base_unit:curRow.base_unit,rate:1,state:'0',
                transfers_state:'0',ref_sequence_id:curRow.ref_sequence_id,only_detail:0,row_status:curRow.row_status }
        },
        backupData () {
            this.tempHeadData = JSON.parse(JSON.stringify(this.headData));//暫存主檔臨時數據
            this.tempGridData1 = JSON.parse(JSON.stringify(this.gridData1));//暫存明細臨時數據
        },
        //**新增明細
        insertRowEvent() {
            if(this.headStatus ==""){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            if (this.headData.id == "") {
                this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!', mask: false });
                return;
            };
            var rowEndIndex = -1;//添加至行尾
            var $table = this.$refs.xTable1;
            this.clearRowDataEdit1();
            this.selectRow = null;
            this.$set(this.rowDataEdit1, "id", this.headData.id);  
            this.$set(this.rowDataEdit1, "sequence_id", this.getSequenceId($table.tableData.length));
            this.$set(this.rowDataEdit1, "row_status", "NEW");

            $table.insertAt(this.rowDataEdit1, rowEndIndex).then(({ row }) => {
                $table.setActiveCell(rowEndIndex, "mo_id");
            });
            //顯示彈窗
            this.cartonCodeList = null;
            var index = $table.tableData.length-1;//新增的行號索引
            this.curRowIndex = index; //記錄新增的行號索引
            var rw = $table.tableData[index];  //取新增行對象值
            this.setRowDataEdit1(rw);
            this.selectRow = rw; //將當前行賦值給彈窗
            this.showEdit = true;
        },
        //**保存事件
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
            this.saveAll();
        },
        //**點擊表格行末的編號按鈕，顯示修改窗口
        editRowEvent1(row){
            //this.rowDataEdit = row; //不可以直接賦row給this.rowDataEdit
            this.setRowDataEdit1(row);
            this.selectRow = row;
            this.showEdit = true;
        },
        //**暫時將當前行的修改更新至表格
        tempUpdateRowEvent(){           
            if(this.headSatus ===""){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            Object.assign(this.selectRow, this.rowDataEdit1);
            //this.$set($table.tableData[1], this.rowDataEdit);
            this.showEdit = false;
        },
        //**撤消對當前行的修改,不影響到表格
        tempUndoRowEvent(){
            this.clearRowDataEdit1();
            this.showEdit = false;
        },    
        //**取頁數對應計劃中的貨品編號資料
        getPlanItemList:async function(mo_id) {            
            //檢查頁數是否合法
            if(mo_id===""){
                return;
            }
            if(mo_id.length !=9)
            {
                return;
            }
            let returnMoId ="";
            await axios.get("/ChangeStoreZc/CheckMo?mo_id=" + mo_id).then(
                (response) => { 
                    returnMoId = response.data;
                }
            ).catch(function (response) {
                returnMoId = "";
                //alert(response);
            });             
            if(returnMoId ===""){
                this.$XModal.alert({ content: `當前頁數:【${mo_id}】不存在，請返回檢查！`, mask: false });
                this.$set(this.rowDataEdit1,"mo_id","");
                this.$set(this.rowDataEdit1, "obligate_mo_id", ""); 
                return;
            }
            this.$set(this.rowDataEdit1, "obligate_mo_id", mo_id);//頁數與庫存頁數保持一致   
            //獲得頁數對應的流程
            axios.get("/ChangeStoreZc/GetPlanItemList?mo_id=" + mo_id).then(
                (response) => {
                    this.changeStoreGoodsList = response.data;
                }                
            ).catch(function (response) {
                alert(response);
            });
        },
        //**檢查貨品編號是否存在
        checkGoodsId:async function(goods_id) {            
            if(goods_id ===""){
                return;
            }
            if(goods_id.length <14)
            {
                return;
            }
            await axios.get("/ChangeStoreZc/CheckGoodsID?goods_id=" + goods_id).then(
                (response) => { 
                    if(response.data.goods_id != ""){
                        this.$set(this.rowDataEdit1,"goods_id",response.data.goods_id);
                        this.$set(this.rowDataEdit1, "goods_name", response.data.goods_name);
                        //this.$set(this.rowDataEdit1, "color", response.data.vendor_name);//暫時用vendor_name代替
                    }else{
                        this.$set(this.rowDataEdit1,"goods_id","");
                        this.$set(this.rowDataEdit1, "goods_name","");
                        //this.$set(this.rowDataEdit1, "color", "");
                    }
                }
            ).catch(function (response) {
                this.$set(this.rowDataEdit1,"goods_id","");
                //alert(response);
            });             
            if(this.rowDataEdit1.goods_id ===""){
                this.$XModal.alert({ content: `當前貨品編碼:【${goods_id}】不存在，請返回檢查！`, mask: false });                
                this.$set(this.rowDataEdit1, "goods_name", "");
                //this.$set(this.rowDataEdit1, "color", "");
            } 
        },
        //**項目刪除
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
                var $table = this.$refs.xTable1;
                if($table.tableData.length===1){
                    //if(this.gridData1.length===1){
                    this.$XModal.alert({ content: "已是最后一行,不可以繼續刪除!", mask: false });
                    return;
                }
                this.$XModal.confirm("是否確定要刪除當前行？").then(type => {                    
                    if(type ==="confirm"){                        
                        if(this.selectRow.row_status != 'NEW'){
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
        //**批準&反批準 val: 1--批準,0--反批準
        approveEvent:async function (val) {
            if ((this.headData.id != "") && (this.gridData1.length > 0)) {
                //批準,反批準都要檢查是否是註銷狀態
                if(this.headData.state === "2"){
                    this.$XModal.alert({ content: '注銷狀態,當前操作無效!', mask: false });
                    return;
                }
                var ls_success= (val==='1')?"批準成功!":"反批準成功!";
                var ls_error= (val==='1')?"批準失敗!":"反準失敗!";
                var ls_type = (val==='1')?"批準":"反批準";
                var ls_is_approve = `確定是否要進行【${ls_type}】操作？`;
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
                    var strError = await this.checkStock(this.headData.id); //此處必須加await,且checkStock函數也要設置成同步執行                    
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
                    //var objCheckDate = new Date(this.headData.check_date);
                    ////再格式化為統一的日期字符串格式(yyyy-MM-dd)
                    //var check_date = comm.getDate(objCheckDate, 0);
                    //if(check_date != this.server_date){
                    //    this.$XModal.alert({ content: '注意:【批準日期】必須為當前日期,方可進行此操作!', mask: false });
                    //    return;
                    //}
                    //檢查已批準日期是否為當日,超過當日則不可反批準
                    var isApprove = await comm.canApprove(this.headData.id,"w_changestore_ce");//倉庫轉倉
                    if(isApprove ==="0"){
                        this.$XModal.alert({ content: "注意:【批準日期】必須為當前日期,方可進行此操作!", mask: false });
                        return;
                    }                    
                    //對方已经签收了的单据不能再反批准
                    var signFor = await comm.checkSignfor(this.headData.id,"ChangeStore");
                    if(signFor != "0"){
                        this.$XModal.alert({ content: "接收貨部門已簽收,不可以再進行反批準操作!", mask: false });
                        return;
                    }
                }
                this.$XModal.confirm(ls_is_approve).then(type => {
                    if (type == "confirm") {
                        this.headData.check_by = this.userId;
                        var head = JSON.parse(JSON.stringify(this.headData));
                        var approve_type = val;
                        var user_id = this.userId;
                        axios.post("/ChangeStoreZc/Approve",{head,user_id,approve_type}).then(
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
        //**彈窗中確認修改
        setModify() {           
            if(this.headStatus ===""){
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
        //**雙擊表格單元格時觸發的事件，暫未用到
        dbclickEvent(row){
            //console.log(row.data[row.$rowIndex]);//取得行對象
            //this.rowDataEdit = row.data[row.$rowIndex]; //此方式是對像,彈窗更改,表格也跟著改
            //this.rowDataEdit = JSON.parse(JSON.stringify(row.data[row.$rowIndex]));
            var rw = row.data[row.$rowIndex];
            this.setRowDataEdit(rw);            
            this.selectRow = rw;
            this.showEdit = true;
        },
        //**點擊表格單元格時觸發的事件
        cellClickEvent(row){
            this.Rows = row;//表格的數據對象
            this.curRowIndex = row.$rowIndex;//當前行的索引
            this.selectRow = row.data[row.$rowIndex];//表格一中的當前行對象           
            this.curDelRow = row.data[row.$rowIndex];//存放可能冊除的當前行對象
            if(this.headData.state ==='0'){
                this.tempSelectRow1 = JSON.parse(JSON.stringify(this.selectRow));//暫存當前行,以便檢查是否有更改
            }           
            this.setRowDataEdit1(this.selectRow); //將當前行與編輯窗口數據對象關聯
        }, 
        //**取主檔資料
        getHead(id) {
            //此處的URL需指定到祥細控制器及之下的動Action,否則在轉出待確認彈窗中的查詢將數出錯,請求相對路徑的問題
            axios.get("/ChangeStoreZc/GetHeadByID", { params: { id: id }}).then(
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
        //**取明細資料
        getDetails: async function (id) {
            var _self = this;
            await axios.get("/ChangeStoreZc/GetDetailsByID", { params: { id: id }  }).then(
                (response) => {
                    this.gridData1 = response.data;                                     
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        //**開啟公共查詢頁面
        showQueryEvent(id) {            
            var url = "/Base/PublicQuery?window_id=" + id;
            comm.showMessageDialog(url, "查詢", 1024, 768, true);
        },
        //**保存
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

            //START 2024/03/01              
            this.tempSaveData=[];
            var items_update=null;
            for(var i=0;i<$table.tableData.length;i++){
                items_update = $table.tableData[i];
                if(items_update.row_status==='NEW' || items_update.row_status==='EDIT'){
                    this.tempSaveData.push(items_update);
                }
            }
            //END 2024/03/01
            this.headData.head_status = this.headStatus;//表頭新增或修改的標識
            var headData = JSON.parse(JSON.stringify(this.headData));
            var lstDetailData1 = this.tempSaveData;//新的方法            
            var lstDelData1 = this.delData1;                  
            var user_id = this.userId;
            axios.post("/ChangeStoreZc/Save",{headData,lstDetailData1,lstDelData1,user_id}).then(
              (response) => {
                  if(response.data ==="OK"){
                      this.setStatusHead(false);
                      //重查刷新數據
                      this.getHead(this.headData.id);
                      this.getDetails(this.headData.id);
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
        //**批準前檢查庫存是否夠扣除
        checkStock:async function(id){
            let rtn="";
            await axios.post("/ChangeStoreZc/CheckStock",{id}).then((response) => {
                    if(response.data.length>0){
                        //庫存檢查不通過
                        let seqNo = response.data[0].sequence_id;
                        let con_qty = response.data[0].con_qty;
                        let sec_qty = response.data[0].sec_qty;
                        let mo_id = response.data[0].mo_id;
                        let goods_id = response.data[0].goods_id;
                        let row_no=0;
                        this.validStockFlag = false;    
                        this.tableData = JSON.parse(JSON.stringify(this.gridData1));//??待測試此代碼                       
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
                    }else{
                        this.validStockFlag = true;//庫存檢查通過
                    }
            }) 
            return rtn;
        },
        //**服務器端日期
        getDateServer: async function() {
          var res = await axios.get("/Base/DataComboxList/GetDateServer");
          this.server_date = res.data;
        },
        showGoods:async function(event){            
            if(this.headData.state==='0') {//可編輯狀態時
                let mo_id = this.rowDataEdit1.mo_id;
                await axios.get("/ChangeStoreZc/GetPlanItemList", { params: { mo_id:mo_id } }).then(
                    (response) => {
                        this.tableGoodsList = response.data;
                        //this.showLot = true;//顯示彈窗
                    }
                ).catch(function (response) {
                    this.tableGoodsList = [];
                    alert(response);
                });
                let $pulldown = this.$refs.pulldownRef;
                if ($pulldown) {
                    $pulldown.togglePanel();
                    //$pulldown.showPanel();
                }
            }
        },
        showLotNo:async function(event){
            if(this.headData.state==='0') {//單元格為可編輯狀態時                
                let location_id = this.rowDataEdit1.inventory_issuance;
                let goods_id = this.rowDataEdit1.goods_id;
                let mo_id = this.rowDataEdit1.mo_id;
                if(this.$utils.isEmpty(goods_id)){
                    //this.$refs.xTable2.setActiveCell(this.selectRow2, "goods_id");
                    return;
                }
                //**提取批号数据 //批號下拉列表框
                await axios.get("/Base/Common/GetStDetailsLotNo", { params: { location_id:location_id,goods_id:goods_id,mo_id:mo_id } }).then(
                    (response) => {
                        this.tableLotNoList = response.data;                        
                    }
                ).catch(function (response) {
                    this.tableLotNoList=[];
                    alert(response);
                });
                let $pulldown = this.$refs.pulldownLotNoRef;
                if ($pulldown) {
                    $pulldown.togglePanel();                   
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
        //**取最大單據號
        getMaxID:async function() {
            var $table = this.$refs.xTable1;
            await axios.get("/Base/Common/GetMaxIDStock?bill_id=" + 'ST10' + "&serial_len=4" ).then(
                (response) => {
                    this.headData.id = response.data;
                }
            );
        },
        //**顯示生產計劃流程中的貨品編碼列表
        cellGoodsClick(row){
            let $pulldown = this.$refs.pulldownRef;
            if ($pulldown) {
                let rowIndex = row.$rowIndex;
                let goods_id = row.data[rowIndex].goods_id;
                let goods_name = row.data[rowIndex].goods_name;
                this.$set(this.rowDataEdit1,"goods_id",goods_id );
                this.$set(this.rowDataEdit1,"goods_name",goods_name );
                if(this.rowDataEdit1.row_status ===""){
                    this.$set(this.rowDataEdit1,"row_status", "EDIT");
                }
                $pulldown.hidePanel();
            }
        },
        //**顯示是庫存中的批號列表
        cellLotNoClick(row){
            let $pulldown = this.$refs.pulldownLotNoRef;
            if ($pulldown) {
                let rowIndex = row.$rowIndex;
                let ir_lot_no = row.data[rowIndex].lot_no;
                this.$set(this.rowDataEdit1,"ir_lot_no",ir_lot_no );
                this.$set(this.rowDataEdit1,"ii_lot_no",ir_lot_no );
                if(this.rowDataEdit1.row_status ===""){
                    this.$set(this.rowDataEdit1,"row_status", "EDIT");
                }
                $pulldown.hidePanel();
            }
        },
        pageChangeEvent({ currentPage, pageSize }) {
            this.pagerConfig.currentPage = currentPage;
            this.pagerConfig.pageSize = pageSize;
        }, 
        //**表格一中表頭的樣式
        headerCellStyle(){
            return `background:'#F5F7FA',color:'#606266',height:'25px',padding:'2px'`;
        },
        cellClassName({ row, column }) {
            // 为所有行添加一个自定义类名
            return 'custom-row-class';
        }


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
    },// end of watch
    computed: {
        //
    },
    mounted() {
        this.tableHeight1=$(parent.window).height()-(172+35);       
        let that = this;       
        window.onresize = () => {
            return (() => {
                that.tableHeight1=$(parent.window).height()-(172+35);            
            })()
        };
    }
}

var app = new Vue(MainIn).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;
Vue.prototype.$utils = XEUtils;

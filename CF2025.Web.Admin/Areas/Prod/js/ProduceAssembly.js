var main = {
    data() {
        return {  
            selectTab: "tab1",
            searchID: "", 
            headEditFlag: "",//主檔為新增或編輯
            curRowIndex: null,
            curRowIndex2: null,
            selectRow: null,
            selectRow2:null,
            curDelRow2: null,
            seledtLotNoRow:null,
            curChangeItem:null,
            curLotNo:null,
            curChangeItemName:'',
            curJoId:"",
            curSequenceId:"",
            headerCellStyle:{background:'#F5F7FA',color:'#606266',height:'25px',padding:'2px'},
            loading: false,
            showEdit: false,
            submitLoading: false,
            showEditStore:false,
            showFind:false,
            showGoods:false,
            showLot:false,
            showGoodsX:0,
            showGoodsY:0,
            server_date:"",
            stateFontColor:"color:black",
            btnItemTitle:"劉覽",
            headData: { id: "", con_date: "", out_dept: "", in_dept: "",bill_origin:"2",handover_id:"",handler:"", remark: "", create_by: "", update_by: "", check_by: "", 
                update_count: "", create_date: "", update_date: "", check_date: "", state: "0",head_status:"" },
            rowDataEdit: {
                id: '', mo_id: '', goods_id: '', goods_name: '',con_qty: 0,  unit_code: 'PCS', sec_qty: 0.00,sec_unit: 'KG',lot_no:'', package_num: '0',color_name: '', 
                four_color: '', app_supply_side: '', remark: '',
                return_qty_nonce: '',sequence_id: '', location: '',carton_code:'', prd_id: 0,  jo_id:'',jo_sequence_id:'',row_status:''
            },
            rowAssembly:{mo_id:'',goods_id:'',goods_name:'',lot_no:'',con_qty:0,unit_code:'PCS',sec_qty:0,sec_unit:'KG',package_num:0,remark:'',bom_qty:0,base_qty:0,
                row_status:'',upper_sequence:'',sequence_id:''},
            BillOrigin:[],
            deptList: [],
            stateList: [],
            qtyUnitList: [],
            wtUnitList: [],
            locationList: [],
            cartonCodeList: [{ label: '', value: '' }],
            goodsPlanList:[],
            lotNoList:[],
            tableData1: [],
            tableData2: [],
            originData2:[],
            delData1:[],
            delData2:[],
            curRowSeqId:"",
            formDataFind:{},
            isEditHead: false,//主檔編輯狀態
            isEditDetail: false,//明細編輯狀態
            isReadOnly: true,//主檔明細共用,控制是否可以更改資料
            isDisableGenNo:true,//是否可以更改負責部門與接收部門并生成單據編號   
           
            cancelButton:false,//是否显示表格中的取消按钮
            rowEditStatus2:false,//表格行對象數據是否有被修改標識
            isEditCell:false,
            userId:"",
            
            tempHeadData: {},
            tempTableData1: {},
            tempTableData2: {},
            tempSelectRow:{},
            tempSelectRow2:{},
            tableHeight :450,
            tempSaveData:[],
            checkStockData:[],
            findData:[],
            insertRecords:{}, 
            removeRecords:{}, 
            updateRecords:{},
            //itemFlag:false,//輸入貨品編碼的長度標識,18位長度true,小于18位長度false
            //moFlag:false,//輸入頁數的長度標識,9位長度true,小于9位長度false
            formRules: {
                id: [{ required: true},{ min: 13, message: '請輸入13个字符長度' }],
                con_date:[{ required: true, message: '請輸入日期' }],
                out_dept:[{ required: true, message: '請選擇組裝部門' }],
                in_dept:[{ required: true, message: '請選擇收貨部門' }]
            },            
            validRules: {
                mo_id: [{ required: true, message: '頁數不可為空' }],
                goods_id: [{ required: true, message: '貨品編碼不可為空'}],
                con_qty: [{ type: 'number', min: 1, max: 10000000, message: '數量不可為空(輸入範圍1~10000000)'}],
                unit_code: [{ required: true, message: '單位不可為空' }], 
                sec_qty: [{ type: 'number', min:0.01,max:99999,message:'重量不可為空(輸入範圍0.01~99999)' }],
                sec_unit:[{ required: true, message: '重量單位不可為空' }],
                //lot_no:[{required:true,message: '批號不可為空'}]
            },
            validRules2: {
                mo_id: [{ required: true, message: '頁數不可為空' }],
                goods_id: [{ required: true, message: '貨品編碼不可為空'}],
                con_qty: [{ type: 'number', min: 0.01, max: 10000000, message: '數量不可為空(輸入範圍0.01~10000000)'}],
                unit_code: [{ required: true, message: '單位不可為空' }], 
                sec_qty: [{ type: 'number', min:0.01,max:99999,message:'重量不可為空(輸入範圍0.01~99999)' }],
                sec_unit:[{ required: true, message: '重量單位不可為空' }],
                lot_no:[{required:true,message: '批號不可為空'}]
            },
            div_tab1_height:'300px',
            Rows:[],
            partData:[],
            validStockFlag:false,
            preUpdateFlag:false,
            tmp_turn_over:[],//移交單臨時數據
            tmp_turn_over_qc:[],//QC移交單臨時數據
            valid_user_id:false,

        }
    },    
    created() {
        this.getComboxList("BillOrigin");//開單來源
        this.getComboxList("DeptList");//部門編碼   
        this.getComboxList("StateList");//狀態 
        this.getComboxList("LocationList");//倉庫,貨架,由選擇的倉庫帶出 
        this.getComboxList("QtyUnitList");//數量單位
        this.getComboxList("WegUnitList");//重量單位
        this.userId = $("#user_id").val();
        this.tempHeadData = this.headData;
        this.tempTableData1 = this.tableData1;
        this.tempTableData2 = this.originData2;    
    },
    methods: {
        //setRefFocus(el)  {
        //    if (el.ref !== undefined) {
        //        el.ref.focus()
        //}},
        findByID() {
            if(this.isEditHead===true){
                this.$XModal.message({ content: "編輯狀態,當前操作無效!", status: "info" });
                return;
            }
            if(this.headEditFlag != "" || this.searchID ===""){
                return;
            }
            //this.selectRow = null;
            //selectTab: "tab2",
            this.loading = true;
            setTimeout(() => {
                this.getHead(this.searchID);
                this.getDetails(this.searchID);
                this.getDetailsPart(this.searchID);
                this.loading = false;
            }, 500);
            //this.findAllData();
            //this.backupData();//備份原始數據
        },
        setItem(goods_id,goods_name){
            this.$set(this.selectRow2,"goods_id",goods_id);
            this.$set(this.selectRow2,"goods_name",goods_name);
            if(this.selectRow2.row_status ==""){
                this.$set(this.selectRow2,"row_status","EDIT");//設置修改狀態
            }
        },
        showFindWindos(){            
            comm.openWindos('ProduceAssembly');
        },
        showFindItem(){
            comm.openFindItem();
        },
        showPassword:async function(){
            await comm.openPassword(this.userId);
        },
        printEvent(){
            //this.showPassword();
            //test
            //var parentWindowHeight=$(parent.window).height();
            //this.tableHeight=$(parent.window).height()-205;
            //var ss=this.$refs.xTable1.getRecordset();           
            //某單元格獲取焦點
            //this.$refs.xTable1.setActiveCell(this.selectRow, "con_qty");
            //var result = this.filterOriginData2();
        },         
        filterOriginData2(){
            if (this.curRowSeqId !== '') {
                let keyword = this.curRowSeqId.toLowerCase();
                return this.originData2.filter(item => {
                    // 以upper_sequence来搜索
                    let str = item.upper_sequence.toLowerCase();
                    return str.indexOf(keyword) !== -1
                })
            } else {
                return this.originData2 || [];
            }
        },
        setUpperCase(strField,value,type){
            value = comm.stringToUppercase(value);
            if(type==='1'){
                this.$set(this.selectRow, strField, value ); //如字母相同只是大小寫不同,修改狀態合仍然為FALSE
                this.setCancelStatus();
            }else{
                this.$set(this.selectRow2, strField, value );
            }           
            //Object.assign(this.selectRow, this.rowDataEdit);
        },
        
        checkNumber(obj,strField,value) {
            comm.allNumber(obj,strField,value);
            this.setCancelStatus();
        },
        checkNumberDec2(obj,strField,value){    
            comm.allNumberDec2(obj,strField,value);           
            this.setCancelStatus();
        },
        checkNumberPart(obj,strField,value) {
            comm.allNumber(obj,strField,value);
            this.setCancelStatus2();
        },
        checkNumberDec2Part(obj,strField,value){    
            comm.allNumberDec2(obj,strField,value);
            this.setCancelStatus2();
        },
        ////點擊表格當前行的還原按鈕
        //revertData(row) {
        //    const $table = this.$refs.xTable1;
        //    if(row.row_status === "NEW"){
        //        //移除所有新增的行
        //        $table.remove(row);
        //    }else{
        //        $table.revertData(row);
        //    }
        //    this.setCancelStatusAll();//檢查是否顯示工具欄中的Cancel按鈕
        //},
        //當前表格行有更改時設置行還原按鈕顯示狀態
        setCancelStatus(){
            //項目新增時cancelButton值為true,非新增狀態的項目修改也要確保cancelButton值為true
            //cancelButton為true 是[行取消按鈕是否顯示的關鍵]
            if(this.selectRow.row_status != "NEW"){
                this.cancelButton = this.$refs.xTable1.isUpdateByRow(this.selectRow);
                if(this.cancelButton === true){
                    this.$set(this.selectRow, "row_status", "EDIT" );
                }else{
                    this.$set(this.selectRow, "row_status", "" );
                }
            }
        },
        setCancelStatus2(){            
            if(this.selectRow2.row_status != "NEW"){                
                this.rowEditStatus2 = this.$refs.xTable2.isUpdateByRow(this.selectRow2);
                if(this.rowEditStatus2 === true){
                    this.$set(this.selectRow2, "row_status", "EDIT" );
                }else{
                    this.$set(this.selectRow2, "row_status", "" );
                }
            }
        },
        /*控制工具欄中Cancel按鈕的顯示
        *當表格中沒有數據修改時設置按鈕不顯示
        */
        setCancelStatusAll(){
            var $table = this.$refs.xTable1; 
            var isCancelAll = false; 
            for (var i = 0; i< $table.tableData.length; i++) {
                if($table.tableData[i].row_status === 'NEW'){
                    isCancelAll=true;
                    break;
                }
                isCancel = $table.isUpdateByRow($table.tableData[i]);//是否有更改
                if(isCancel===true){
                    isCancelAll = true;
                    break;
                }
            }
            this.cancelButton = isCancelAll;
        },
        //初始化下拉列表框
        getComboxList(SourceType) {
            axios.get("/Base/DataComboxList/GetComboxList?SourceType=" + SourceType).then(
                (response) => {
                    switch (SourceType) {
                        case "BillOrigin":
                            this.BillOrigin = response.data;
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
                        case "WegUnitList":
                            this.wtUnitList = response.data;
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
        insertEvent() {
            this.setStatusHead(true);
            this.backupData();//編輯前首先暫存主檔/明細臨時數據           
            this.headEditFlag = "NEW";
            this.isReadOnly = false; //可編輯
            this.isDisableGenNo = false; //可用成單據編號
            //清空數據
            this.clearHeadData();
            this.tableData1 = [];//清空明細表格數據   
            this.tableData2 = [];
            //新增后初始化相關對象的初始值            
            var d = new Date();//生成日期對象:Fri Oct 15 2021 17:51:20 GMT+0800
            var date = comm.getDate(d, 0);//轉成年月日字符串格式            
            var dateTime = comm.datetimeFormat(d, "yyyy-MM-dd hh:mm:ss");
            this.$set(this.headData, "con_date", date);
            this.$set(this.headData, "create_by", this.userId);
            this.$set(this.headData, "create_date", dateTime);
            this.$set(this.headData, "bill_origin", '2');  
            this.$set(this.headData, "head_status", this.headEditFlag);  
            
        },
        delHeadEvent() {          
            if ((this.headData.id != "") && (this.tableData1.length > 0)) {
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
                        axios.post("/TransferIn/DeleteHead",{head}).then(
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
        //還原表頭及全部的明細
        revertEvent() {
            this.setStatusHead(false);
            this.headData = JSON.parse(JSON.stringify(this.tempHeadData));//還原主表
            this.tableData1 = JSON.parse(JSON.stringify(this.tempTableData1));//還原明細 
            this.originData2 = JSON.parse(JSON.stringify(this.tempTableData2));//還原明細
            this.delData1=[];
            this.delData2=[];
            this.Rows = JSON.parse(JSON.stringify(this.tableData1));

            this.isDisableGenNo = this.tempDisableGenNo;
            this.headEditFlag = "";
            this.isReadOnly = (this.headData.state==='0')?false:true;
            this.cancelButton = false;

            if(this.tableData1.length>0){                
                this.tempSelectRow = JSON.parse(JSON.stringify(this.Rows[0]));
                this.selectRow = this.Rows[0];//JSON.parse(JSON.stringify(this.Rows[0]));/;////this.Rows.data[0];//當前行
                //this.selectRow = this.tableData1[0];
                var $table = this.$refs.xTable1;
                $table.setCurrentRow(this.selectRow);//定位至當前索引所在的行
                $table.setActiveCell(this.selectRow, "mo_id");//設置單元格焦點     
                this.curRowSeqId = this.selectRow.sequence_id;
                this.setTableData2ByCurrentMo();
            }      
            
            //this.selectRow = this.tableData1[0];
            ////this.selectRow = JSON.parse(JSON.stringify(this.tableData1[0]));//this.tableData1[0];     
            //var $table = this.$refs.xTable1;
            //$table.setCurrentRow(this.selectRow);//定位至當前索引所在的行
            //$table.setActiveCell(this.selectRow, "mo_id");//設置單元格焦點   


            //this.canAddItem = true;
            //const $table = xTable.value
            //const type = await VXETable.modal.confirm('您确定要还原数据吗?')
            //if (type === 'confirm') {
            //    $table.revertData()
            //}
        },
        test(){
            console.log(this.tableData1);
            console.log(this.originData2);
            console.log(this.tempSelectRow2);
            
        },
        setStatusHead(blValue) {            
            this.isEditHead = blValue; //設置編輯狀態標識
            this.btnItemTitle = (this.headData.state==='0')?'修改':'劉覽';
        },
        clearHeadData(){
            this.headData.id="",
            this.headData.con_date = "",
            this.headData.bill_origin = "2",
            this.headData.out_dept = "",
            this.headData.in_dept = "",
            this.headData.handover_id ="",
            this.headData.handler = "",
            this.headData.remark = "",
            this.headData.create_by = "",
            this.headData.update_by = "",
            this.headData.check_by = "",
            this.headData.update_count = "",
            this.headData.create_date = "",
            this.headData.update_date = "",
            this.headData.check_date = "",
            this.headData.state ="0" 
            this.headData.head_status ="";
        },
        clearRowDataEdit(){
            this.rowDataEdit = {id: '', mo_id: '', goods_id: '', goods_name: '',con_qty: 0,  unit_code: 'PCS', sec_qty: 0.00,sec_unit: 'KG',
                lot_no:'', package_num: '0',color_name: '', four_color: '', app_supply_side: '', remark: '', return_qty_nonce: '',sequence_id: '',
                location: '',carton_code:'', prd_id: 0,  jo_id:'',jo_sequence_id:'',row_status:''
            }
        },
        clearAssembly(){
            this.rowAssembly = {mo_id:'',goods_id:'',goods_name:'',lot_no:'',con_qty:0,unit_code:'PCS',sec_qty:0,sec_unit:'KG',package_num:0,remark:'',
                bom_qty:0,base_qty:0,row_status:'',upper_sequence:'',sequence_id:''};
        },       
        backupData () {
            this.tempHeadData = JSON.parse(JSON.stringify(this.headData));//暫存主檔臨時數據
            this.tempTableData1 = JSON.parse(JSON.stringify(this.tableData1));//暫存明細臨時數據
            this.tempTableData2 = JSON.parse(JSON.stringify(this.originData2));
        },
        //項目新增1
        insertRowEvent() {
            if(this.headData.state !="0"){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            if (this.headData.id == "") {              
                this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!', mask: false });
                return;
            }; 
            var rowEndIndex = -1;//添加至行尾
            var $table = this.$refs.xTable1;
            this.clearRowDataEdit();
            this.selectRow = null;
            this.tableData2 = [];
            this.$set(this.rowDataEdit, "id", this.headData.id); 
            this.$set(this.rowDataEdit, "sequence_id", this.getSequenceId($table.tableData.length));
            this.$set(this.rowDataEdit, "row_status", 'NEW');
            this.cancelButton = true;

            this.tableData1.push(this.rowDataEdit);
            this.selectRow = this.rowDataEdit;
            $table.setCurrentRow(this.rowDataEdit);//定位至當前索引所在的行
            $table.setActiveCell(this.rowDataEdit, "mo_id");//設置單元格焦點
            
            //顯示彈窗
            //this.cartonCodeList=null;
            //var index = $table.tableData.length-1;//新增的行號索引
            //this.curRowIndex = index; //記錄新增的行號索引
            //var rw = $table.tableData[index];  //取新增行對象值
            ////this.setRowDataEdit(rw);//2022/05/24 Cancel
            //this.rowDataEdit = rw;//2022/05/24 Add  修改表格與彈窗同步
            //this.selectRow = rw; //將當前行賦值給彈窗
            //this.showEdit = true;            
        },
        insertRowEvent2(){
            if(this.headData.state !="0"){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            if (this.headData.id == "") {              
                this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!', mask: false });
                return;
            };           
            var rowEndIndex = -1;//添加至行尾
            var $table = this.$refs.xTable2;
            this.clearAssembly();            
            this.$set(this.rowAssembly, "id", this.headData.id);
            this.$set(this.rowAssembly, "mo_id", this.selectRow.mo_id);
            this.$set(this.rowAssembly, "upper_sequence", this.selectRow.sequence_id);
            this.$set(this.rowAssembly, "sequence_id", this.getSequenceId($table.tableData.length));
            this.$set(this.rowAssembly, "row_status", 'NEW');
            this.rowEditStatus2 = true;
            this.tableData2.push(this.rowAssembly);
            this.selectRow2 = this.rowAssembly;
            $table.setCurrentRow(this.rowAssembly);//定位至當前索引所在的行
            $table.setActiveCell(this.rowAssembly, "goods_id");//設置單元格焦點
            
            //$table.insertAt(this.rowAssembly,rowEndIndex).then(({ row }) => {
            //    this.selectRow2 = row;
            //    $table.setCurrentRow(row);//定位至當前索引所在的行
            //    $table.setActiveCell(row, "mo_id");//設置單元格焦點                
            //});
        },        
        editRowEvent(row){
            this.tempSelectRow = JSON.parse(JSON.stringify(row));//暫存當前行,以便檢查是否有更改            
            this.selectRow = row;
            this.rowDataEdit = row;
            this.isReadOnly = (this.headData.state==='0')?false:true;//2022/05/30 add            
            this.showEdit = true;
        },
        ////暫時將當前行的修改更新至表格
        //tempUpdateRowEvent(){
        //    if(this.headEditFlag ===""){
        //        this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
        //        return;
        //    }
        //    Object.assign(this.selectRow, this.rowDataEdit);
        //    //this.$set($table.tableData[1], this.rowDataEdit);
        //    this.showEdit = false;
        //},

        ////撤消對當前行的修改,不影響到表格
        //tempUndoRowEvent(){
        //    this.clearRowDataEdit();
        //    this.showEdit = false;
        //},

        //暫時將當前行從表格中移除
        //tempDelRowEvent(row){
        //    this.$XModal.confirm('您确定要删除吗？').then(type => {
        //        if(type ==='confirm'){
        //            //this.$XModal.message({ content: `点击了 ${type}` });
        //            const $table = this.$refs.xTableIn;
        //            $table.remove(row); 
        //        }
        //    })
        //},
        //項目刪除
        tempDelRowEvent:async function(){
            if(this.headData.state !='0'){
                this.$XModal.alert({ content: "此單據已批準,當前操作無效!", mask: false });
                return;
            }
            let status = await comm.checkApproveStatus('jo_assembly_mostly',this.headData.id);
            if(status ==='1'||status ==='2'){
                let str = (status ==='1')?"批準":"注銷";
                this.$XModal.alert({ content: `后端數據已是${str}狀態,當前操作無效!`, mask: false });
                return;
            }
            if(this.curDelRow){
                if(this.tableData1.length===1){
                    this.$XModal.alert({ content: "已是最后一行,不可以繼續刪除!", mask: false });
                    return;
                }
                this.$XModal.confirm("是否確定要刪除當前行？").then(type => {                    
                    if(type ==="confirm"){
                        //刪除表格2原始表中對應的數據                        
                        var lenths = this.originData2.length - 1;                        
                        for( var i=lenths;i>=0;i--){
                            if(this.originData2[i].upper_sequence === this.selectRow.sequence_id){                                    
                                 this.originData2.splice(i,1);//移除
                             }
                        }
                        if(this.selectRow.row_status ==='NEW'){
                            //移除表格二數組中的元素
                            this.tableData2=[];
                        }else{
                            //記錄需刪除表格2后臺已有的對應數據                            
                            for(let index in this.tableData2){
                                if(this.tableData2[index].row_status != "NEW"){
                                   this.delData2.push(this.tableData2[index]);
                                }
                            }
                            this.tableData2 = [];                            
                            //記錄需刪除表格1的后臺數據
                            this.delData1.push(this.selectRow);         
                        }
                        this.tableData1.splice(this.curRowIndex,1);//移除表格1刪除的當前行
                        this.isEditHead = true;
                        this.curDelRow = null;
                    }
                })
            }else{
                this.$XModal.alert({ content: '請首先指定需要刪除的項目!',mask: false });
            }
        },
        //項目刪除2
        tempDelRowEvent2:async function(){
            if(this.headData.state !='0'){
                this.$XModal.alert({ content: "此單據已批準,當前操作無效!", mask: false });
                return;
            }            
            if(this.curDelRow2){
                if(this.tableData2.length===1){
                    this.$XModal.alert({ content: "已是最后一行,不可以繼續刪除!", mask: false });
                    return;
                }
                let status = await comm.checkApproveStatus('jo_assembly_mostly',this.headData.id);
                if(status ==='1'|| status ==='2'){
                    let str = (status ==='1')?"批準":"注銷";
                    this.$XModal.alert({ content: `后端數據已是${str}狀態,當前操作無效!`, mask: false });
                    return;
                }
                this.$XModal.confirm("是否確定要刪除當前行？").then(type => {
                    if(type ==="confirm"){
                        //刪除表格2原始表中對應的行對象
                        var lenths = this.originData2.length - 1;  
                        for( var i=lenths;i>=0;i--){
                            if(this.originData2[i].upper_sequence === this.selectRow2.upper_sequence && this.originData2[i].sequence_id===this.selectRow2.sequence_id){                                    
                                this.originData2.splice(i,1);//移除
                            }
                        }
                        if(this.selectRow2.row_status != 'NEW'){
                            this.delData2.push(this.selectRow2);
                        }
                        this.tableData2.splice(this.curRowIndex2,1); //刪除表格2當前行數據(后面新增加的行直接移除)
                        this.isEditHead = true;
                        this.curDelRow2 = null;
                    }
                })
            }else{
                this.$XModal.alert({ content: '請首先指定需要刪除的項目!',mask: false });
            }
        },        
        getMaxID(bill_id, dept_id) {
            //請求后臺Action并传多個參數,多個參數傳值方法:第一參數用url..."?id="+value,后面的參數用+"&Ver="+value
            var $table = this.$refs.xTable1;
            axios.get("/Base/Common/GetMaxID?bill_id=" + bill_id + "&dept_id=" + dept_id + "&serial_len=5").then(
                (response) => {
                    this.headData.id = response.data;
                    //直接更改表格中某單元格的值,不可以直接更改綁定數組的方式,否則將取不到數據的更改狀態
                    for (var i = 0; i < $table.tableData.length; i++) {
                        this.$set($table.tableData[i], "id", this.headData.id );
                        this.$set($table.tableData[i], "sequence_id", this.getSequenceId(i));
                    }
                }
            );
        }, 
        getSequenceId(i){          
            return comm.pad(i + 1,4) +"h";
        },
        //取貨架資料
        getCartonCodeList(location_id) {
            axios.get("/Base/DataComboxList/GetCartonCodeList?LocationId=" + location_id).then(
                (response) => {
                    //this.$set(this.rowDataEdit, "shelf", "");//貨架 賦值給下拉列表框前將值清空
                    this.cartonCodeList = response.data;
                    this.$set(this.rowDataEdit, "carton_code", location_id);//倉位默認與倉庫保持一致    
                    this.$set(this.rowDataEdit, "shelf", location_id)
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        //批準&反批準 val: 1--批準,0--反批準
        approveEvent:async function(val) {
            if(this.isEditHead===true){
                this.$XModal.alert({ content: "編輯狀態,當前操作無效!", mask: false });
                return;
            };
            if ((this.headData.id != "") && (this.tableData1.length > 0)) {
                //批準,反批準都要檢查是否是註銷狀態
                if(this.headData.state === "2"){
                    this.$XModal.alert({ content: "注銷狀態,當前操作無效!", mask: false });
                    return;
                }
                var msg_success="";
                var msg_error=""; 
                var mst_is_approve="確定是否要進行當操作？";
                //取后端單據狀態
                let status = comm.checkApproveStatus('jo_assembly_mostly',this.headData.id);               
                if(status==='2'){
                    this.$XModal.alert({ content: "后端數據已是注銷狀態,當前操作無效!", mask: false });
                    return;
                }
                if(val==='1') {//準備批準時
                    //進行當前批準操作前再次檢查后端是否已被別的用戶批準.
                    if(this.headData.state === "1"){
                        this.$XModal.alert({ content: "后端數據已是批準狀態,當前操作無效!", mask: false });
                        return;
                    }
                    if(status==="1"){
                        //后臺數據已為批準狀態,已被別的用戶批準
                        this.$XModal.alert({ content: "已是批准狀態,當前操作無效!", mask: false });
                        return;
                    }
                    msg_success = "批準成功!";
                    msg_error = "批準失敗!";
                    mst_is_approve +="【批準】";
                }
                if(val==='0'){                    
                    //反批准
                    //進行當前反批準操作前再次檢查后端是否已被別的用戶反批準.
                    if(status==="0"){
                        //后臺數據已為批準狀態,已被別的用戶批準
                        this.$XModal.alert({ content: "后端數據已是未批準狀態,當前操作無效!", mask: false });
                        return;
                    }
                    //設置后端服務器日期存儲在this.server_date
                    this.getDateServer();
                    //檢查已批準日期是否為當日,超過當日則不可反批準                  
                    //將批準日期(字符串)轉換為對象
                    var objCheckDate = new Date(this.headData.check_date);
                    //再格式化為統一的日期字符串格式(yyyy-MM-dd)
                    var check_date = comm.getDate(objCheckDate, 0);
                    if(check_date != this.server_date){
                        this.$XModal.alert({ content: "注意:【批準日期】必須為當前日期,方可進行此操作!", mask: false });
                        return;
                    }                    
                    msg_success = "反批準成功!";
                    msg_error = "反批準失敗!";
                    mst_is_approve +="【反批準】";
                }                
                this.$XModal.confirm(mst_is_approve).then(type => {
                    if (type == "confirm") {
                        this.headData.check_by = this.userId;
                        var head = JSON.parse(JSON.stringify(this.headData));//深拷貝轉成JSON數據格式
                        var approve_type = val;
                        axios.post("/ProduceAssembly/Approve",{head,approve_type}).then(
                            (response) => {
                                if(response.data[0].approve_status==="OK"){
                                    response.data[0].ProductMo
                                    this.setStatusHead(false);
                                    //重查刷新數據
                                    this.getHead(this.headData.id);
                                    //this.$XModal.alert({ content: msg_success, mask: false });
                                    this.$XModal.message({ content: msg_success, status: "success" });
                                }else{
                                        if(response.data[0].action_type==="STOCK"){
                                            //庫存不足
                                            this.checkStockData=response.data;
                                            msg_error += "【"+response.data[0].error_info+"】";
                                            this.showEditStore = true;
                                        }
                                        this.$XModal.message({ content: msg_error,status: "warning" , mask: false });
                                }
                            }
                        ).catch(function (response) {
                            this.$XModal.alert({ content: "系統錯誤:"+response,status: "error" , mask: false });                            
                        });
                        }
                })
            } else {
                    this.$XModal.alert({ content: "主檔編號不可為空,當前操作無效!", status: "warning" });
                    return;
            };
        },        
        //雙擊顯示彈窗
        dbclickEvent(row){
            //this.rowDataEdit = row.data[row.$rowIndex]; //此方式是對像,彈窗更改,表格也跟著改
            //this.rowDataEdit = JSON.parse(JSON.stringify(row.data[row.$rowIndex]));
            var rw = row.data[row.$rowIndex];
            this.rowDataEdit = rw;//2022.05.24 add
            this.selectRow = rw;
            if(this.headData.state==='0'){
                this.tempSelectRow = JSON.parse(JSON.stringify(this.selectRow));//暫存當前行,以便檢查是否有更改                
            }
            this.isReadOnly = (this.headData.state ==='0')?false:true;//2022/05/30 add
            this.showEdit = true;
        },
        //點擊表格一單元格時
        cellClickEvent(row){
            this.Rows = row;//表格的數據對象
            this.curRowIndex = row.$rowIndex;//當前行的索引
            this.selectRow = row.data[row.$rowIndex];//當前行
            this.curDelRow = row.data[row.$rowIndex];//存放可能冊除的當前行
            if(this.headData.state==='0'){
                this.tempSelectRow = JSON.parse(JSON.stringify(this.selectRow));//暫存當前行,以便檢查是否有更改
            }
            //if(row.column.property==='goods_id'){
            //    //設置貨品編號彈窗起止位置                
            //    //this.setFindItemPosition(row.$event.clientX,row.$event.clientY);//2022-06-20                
            //} 
            this.showGoodsY = row.$event.clientY;
            this.curRowSeqId = this.selectRow.sequence_id;
            this.setTableData2ByCurrentMo();
        },
        cellClickEvent2(row){
            this.curRowIndex2 = row.$rowIndex;//當前行的索引
            this.selectRow2 = row.data[row.$rowIndex];//當前行
            this.curDelRow2 = row.data[row.$rowIndex];//存放可能冊除的當前行
            this.showGoodsY = row.$event.clientY;
            if(this.headData.state==='0'){
                this.tempSelectRow2 = JSON.parse(JSON.stringify(this.selectRow2));//暫存當前行,以便檢查是否有更改
            }
        },
        //查找流程中的货品编码
        showItem(event,type){
            var x=0,y=0;
            if(type ==='1'){
                //type=1为表格1调用
                //event.clientY有些不准确,改为取cellClickEvent的Y座标(this.showGoodsY).
                x = event.clientX;
                y = this.showGoodsY;
            }else{
                //type=2编辑弹窗表单中调表
                x = event.clientX-250;
                y = event.clientY;
            }
            this.setFindItemPosition(x,y);
        },
        showLotNo(event){
            if(this.headData.state==='0'){//單元格為可編輯狀態時
                this.seledtLotNoRow = null;
                this.showGoodsX = event.clientX;                 
                //提取批号数据
                this.getLotNoData(this.selectRow2.goods_id,this.headData.out_dept,this.selectRow.mo_id);
                this.showLot = true;
            }
        },
        //弹窗中的选中确认
        cellClickGetItems(row){
            this.curChangeItem = row.data[row.$rowIndex].goods_id;
            this.curChangeItemName = row.data[row.$rowIndex].goods_name;
            this.curJoId = row.data[row.$rowIndex].jo_id;
            this.curJoSequenceId = row.data[row.$rowIndex].jo_sequence_id;
        },
        cellClickGetLotNo(row){
            this.seledtLotNoRow = row.data[row.$rowIndex];
        },
        //在指定的屏幕位置中顯示貨品編碼列表
        setFindItemPosition(x,y){
            if(this.isEditCell){
                //單元格為可編輯狀態時
                this.curChangeItem = null;
                //x,y點擊點鼠標的座標
                this.showGoodsX = x;
                this.showGoodsY = y;
                //提取生产流程数据
                this.getItemData(this.selectRow.mo_id,this.headData.out_dept,this.headData.in_dept);
                this.showGoods = true;
            }
        },
        //主檔查詢
        getHead(id) {
            //此處的URL需指定到祥細控制器及之下的動Action,否則在轉出待確認彈窗中的查詢將數出錯,請求相對路徑的問題
            axios.get("/ProduceAssembly/GetHeadByID", { params: { id: id }}).then(
                (response) => {                
                    this.headData = {
                        id: response.data.id,
                        con_date: response.data.con_date,
                        bill_origin: response.data.bill_origin,
                        out_dept: response.data.out_dept,
                        in_dept: response.data.in_dept,
                        handover_id: response.data.handover_id,
                        handler: response.data.handler,
                        remark: response.data.remark,
                        create_by: response.data.create_by,
                        update_by: response.data.update_by,
                        check_by: response.data.check_by,
                        update_count: response.data.update_count,
                        create_date: response.data.create_date,
                        update_date: response.data.update_date,
                        check_date: response.data.check_date,
                        state: response.data.state,
                        row_status:""
                    }
                    if(response.data.state==='2'){
                        this.stateFontColor = "color:red";
                    }else{
                        this.stateFontColor = "color:black";
                    }
                    this.headEditFlag = "";//新單或舊數據的修改標識
                    this.isReadOnly = (response.data.state==='0')?false:true;
                    this.btnItemTitle = (this.headData.state==='0')?'修改':'劉覽';
                    if(this.isReadOnly === false){
                        //可以更改資料,還要多判斷未準的單據,此類型即狀態是未批準的,不允許再更改組裝部門與收貨部門                        
                        this.isDisableGenNo = (this.headEditFlag==="")?true:false; //true:禁用,false:可用
                    }else{
                        this.isDisableGenNo = true;
                    }
                    this.tempDisableGenNo = this.isDisableGenNo;
                    //日期統一轉成yyyy-MM-dd格式,否則判斷是否有修改時引起不正確的判斷
                    this.$set(this.headData,'con_date',this.$utils.toDateString(this.headData.con_date, 'yyyy-MM-dd'));
                    //深度複製一個對象，用來判斷數據是否有修改
                    this.tempHeadData = JSON.parse(JSON.stringify(this.headData));
                    //const ymd = this.$utils.toDateString(this.tempHeadData.con_date, 'yyyy-MM-dd');
                    //console.log(ymd);
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        //明細查詢,必須同步提取數據
        //否則表格二會出現選執行代碼,取不到數據的情況.
        getDetails:async function(id) {
            await axios.get("/ProduceAssembly/GetDetailsByID", { params: { id: id }  }).then(
                (response) => {
                    this.tableData1 = response.data;
                    this.curRowSeqId = "";
                    if(this.tableData1.length>0){
                        this.curRowSeqId = this.tableData1[0].sequence_id;
                        console.log(this.curRowSeqId);
                        if(this.headData.state==='0'){
                            this.tempSelectRow = JSON.parse(JSON.stringify(this.tableData1[0]));//暫存當前行,以便檢查是否有更改                           
                        }
                        this.selectRow = this.tableData1[0];
                        //this.selectRow = JSON.parse(JSON.stringify(this.tableData1[0]));//this.tableData1[0];     
                        var $table = this.$refs.xTable1;
                        $table.setCurrentRow(this.selectRow);//定位至當前索引所在的行
                        $table.setActiveCell(this.selectRow, "mo_id");//設置單元格焦點
                    }
                    this.tempTableData1 = JSON.parse(JSON.stringify(this.tableData1));//暫存明細臨時數據
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        //成分查詢,須同步提取數據
        getDetailsPart:async function (id) {                       
           await axios.get("/ProduceAssembly/GetDetailsPartByID", { params: { id: id}  }).then(
                (response) => {
                    this.originData2 = response.data;//全部數據暫存到臨時變量中
                    this.tempTableData2 =  JSON.parse(JSON.stringify(response.data));
                    this.setTableData2ByCurrentMo(); //從臨時變量中找出對應的數據出來填充表格2,默認表格1中的第一行                    
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
        //动态改变样式,更改第一頁DIV邊框高度
        getStyle(){
            return {
                height:this.div_tab1_height,
                border: '1px solid #dcdfe6'
            }
        },
        //點擊表格一時表格二跟著變化
        setTableData2ByCurrentMo () {
            this.tableData2 =[];            
            if (this.curRowSeqId !='') {
                this.tableData2 = this.filterOriginData2();
            }
        },
        //記錄前后定位
        handlerNextOrPrev(type){
            var curIndex=0;
            if(type==="Next"){
                if(this.curRowIndex < this.Rows.data.length-1){
                    curIndex = this.curRowIndex + 1;//定位至下一條記錄
                    this.curRowIndex = curIndex;
                }else{
                    //已是最后一條記錄
                    curIndex = this.curRowIndex;
                }
            }else{//Prev前一條記錄
                if(this.curRowIndex > 0){
                    curIndex = this.curRowIndex - 1;
                    this.curRowIndex = curIndex;
                }else{
                    curIndex = 0;
                }
            }
            var rw = this.Rows.data[curIndex];//取當前索引對應的行對象            
            this.tempSelectRow = JSON.parse(JSON.stringify(rw));//暫存當前行,以便檢查是否有更改            
            this.$refs.xTable1.setCurrentRow(rw);//定位至當前索引所在的行
            this.rowDataEdit = rw;
            this.selectRow = rw;
            this.curRowSeqId = this.selectRow.sequence_id;
            this.setTableData2ByCurrentMo();
        },
        checkMo: async function(mo_id,type){
            if(mo_id !="" && this.headData.state === '0' ){
                if(this.$utils.getSize(mo_id)<9){
                    //檢查頁數,長度不足9位
                    await this.$XModal.alert({ content: `頁數長度【${mo_id}】有誤,請返回檢查!`,status: 'warning',mask: true });
                    this.$set(this.selectRow, "goods_id","");
                    this.$set(this.selectRow, "goods_name","");
                    if(type==='1'){
                        this.$refs.xTable1.setActiveCell(this.selectRow, "mo_id");
                    }else{//編輯彈窗頁數輸入框獲取焦點
                        this.$refs.editMoId.focus();
                    }
                }else{
                    //檢查頁數,長度等于9位
                    let res = await axios.post("/Base/PublicItemQuery/CheckByMo", { mo_id: mo_id  })
                    if(this.$utils.isEmpty(res.data)){
                        await this.$XModal.alert({ content: `頁數【${mo_id}】不存在,請返回檢查!`,status: 'warning',mask: true });
                        this.$set(this.selectRow, "goods_id","");
                        this.$set(this.selectRow, "goods_name","");
                        if(type==='1'){
                            this.$refs.xTable1.setActiveCell(this.selectRow, "mo_id");
                        }else{//編輯彈窗頁數輸入框獲取焦點
                            this.$refs.editMoId.focus();
                        }
                    }
                }
            }
        },         
        checkItem:async function(goods_id,type){
            if(this.$utils.getSize(goods_id)<18){
                //檢查貨品編號,長度小于18位 
                if(goods_id !="" && this.headData.state === '0' ){
                    await this.$XModal.alert({ content: `貨品編碼【${goods_id}】長度有誤,請返回檢查!`,status: 'warning',mask: true });                    
                    if(type==='1'){
                        this.$refs.xTable1.setActiveCell(this.selectRow, "goods_id");
                    }else{
                        this.$refs.xTable2.setActiveCell(this.selectRow2, "goods_id");
                    }
                }
            }else{
                //檢查貨品編號,長度等于18位
                let goods_name = "";
                let res = await axios.post("/Base/PublicItemQuery/CheckItemById", { goods_id: goods_id } );
                goods_name = res.data;
                if(goods_name===""){
                    await this.$XModal.alert({ content: `貨品編碼(18位長度)【${goods_id}】不存在,請返回檢查!`,status: 'warning',mask: true });
                    if(type==='1'){                    
                        this.$refs.xTable1.setActiveCell(this.selectRow, "goods_id");
                    }else{
                        this.$refs.xTable2.setActiveCell(this.selectRow2, "goods_id");
                    }
                }else{
                    (type==='1')?this.$set(this.selectRow,"goods_name",goods_name):this.$set(this.selectRow2,"goods_name",goods_name);
                }
            }
        },       
        getItemData(mo_id,out_dept,in_dept){
            if(this.$utils.isEmpty(mo_id)){
                this.$refs.xTable1.setActiveCell(this.selectRow, "mo_id");
                //this.$XModal.alert({ content: '頁數不可以為空!', mask: false });
                return;
            }
            axios.get("/ProduceAssembly/GetGoodsId", { params: { mo_id: mo_id,out_dept:out_dept,in_dept:in_dept } }).then(
                (response) => {
                    this.goodsPlanList = response.data;
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        getLotNoData(goods_id,out_dept,mo_id){
            if(this.$utils.isEmpty(goods_id)){
                this.$refs.xTable2.setActiveCell(this.selectRow2, "goods_id");                
                return;
            }
            axios.get("/ProduceAssembly/GetLotNoList", { params: { goods_id: goods_id,location_id:out_dept,mo_id:mo_id } }).then(
                (response) => {
                    this.lotNoList = response.data;
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        //確認更改了貨品編碼
        changeItem() {
            if(this.curChangeItem){                
                if(this.selectRow.goods_id != this.curChangeItem){                  
                    this.$set(this.selectRow, "goods_id", this.curChangeItem);
                    this.$set(this.selectRow, "goods_name", this.curChangeItemName);
                    this.$set(this.selectRow,"jo_id",this.curJoId);
                    this.$set(this.selectRow,"jo_sequence_id",this.curJoSequenceId);
                    this.setCancelStatus();
                    //更改成新的貨品編號,再改回原來的貨品編號,這種情況要強制設置狀態為Edit
                    if(this.selectRow.row_status ===''){
                        this.$set(this.selectRow, "row_status", 'EDIT');
                        this.cancelButton = true;
                    }
                    var status="";
                    if(this.selectRow.row_status ==='NEW'){
                        status = 'NEW';
                    }                    
                    if(this.selectRow.row_status ==='EDIT'){
                        status = 'EDIT';
                        var lenths = this.originData2.length -1;
                        for(var i=lenths;i>=0;i--){
                            if(this.originData2[i].upper_sequence === this.selectRow.sequence_id){
                                this.delData2.push(this.originData2[i]); //更改表格一中的貨品編號,保存時先刪除表格二中舊記錄,再重新插入新的記錄
                                //先移除,后再添加
                                this.originData2.splice(i,1);
                            }
                        }
                        console.log( this.delData2);
                    }
                    //重新設置表格二中的
                    //成份組成
                    axios.get("/ProduceAssembly/GetAssembly", { params: { mo_id: this.selectRow.mo_id,goods_id:this.selectRow.goods_id,out_dept:this.headData.out_dept,in_dept:this.headData.in_dept } }).then(
                        (response) => {
                            for(var i=0;i<response.data.length;i++){
                                this.$set(response.data[i],'unit_code','PCS');
                                this.$set(response.data[i],'sec_unit','KG');
                                this.$set(response.data[i],'id',this.headData.id);
                                this.$set(response.data[i],'upper_sequence',this.selectRow.sequence_id);
                                this.$set(response.data[i],'sequence_id',comm.pad(i + 1,4) +"h");
                                this.$set(response.data[i],'row_status','NEW');// this.$set(response.data[i],'row_status',status);
                                //原料編號數量默認為0.01
                                var strGoodsId=response.data[i].goods_id;
                                if(strGoodsId.substring(0,2)==="ML"){
                                    this.$set(response.data[i],'con_qty',0.01);
                                }else{
                                    this.$set(response.data[i],'con_qty',this.selectRow.con_qty);
                                    this.$set(response.data[i],'sec_qty',this.selectRow.sec_qty);
                                }
                                this.originData2.push(response.data[i]);
                            }                            
                            this.setTableData2ByCurrentMo();
                        }
                    ).catch(function (response) {
                        alert(response);
                        return;
                    });
                }                
                this.showGoods = false;
            }else{
                this.$XModal.alert({ content: '請指定貨品編碼!', mask: false });
            }
        },
        //確認选中的批号
        changeLotNo() {
            if(this.seledtLotNoRow){
                if(this.selectRow2.lot_no != this.seledtLotNoRow.lot_no){
                    this.$set(this.selectRow2, "mo_id", this.seledtLotNoRow.mo_id);
                    this.$set(this.selectRow2, "lot_no", this.seledtLotNoRow.lot_no);
                    var strGoodsId=this.seledtLotNoRow.goods_id;
                    if(strGoodsId.substring(0,2)==='ML'){
                        this.$set(this.selectRow2, "con_qty", 0.01);//原料數量默認為0.01
                    }
                }
                this.showLot = false;
            }else{
                this.$XModal.alert({ content: '請指定批號!', mask: false });
            }
        },
        ////******************************************************
        //change(value) {
        //    console.log(value)
        //},
        //filterSelectMethod(key) {
        //    console.log(key);
        //    if (key) {
        //        // 如果这样写就是不对的，在过滤之后，oitions的值就变为了只有过滤后的值了
        //        this.options = this.options.filter(
        //            item =>
        //                item.food.includes(key) ||
        //                item.select.includes(key) ||
        //                item.cookMethod.includes(key)
        //        )
        //    } else {               
        //        this.options = this.options
        //    }
        //},
        ////******************************************************        
        
        //点击编辑前的逻辑判断
        //activeMethod({ row, rowIndex, column, columnIndex }){ 
        activeMethod(){
            this.isEditCell = (this.headData.state ==="0")?true:false;
            return this.isEditCell;
        },
        //服務器端日期
        getDateServer: async function() {
            var res= await axios.get("/Base/DataComboxList/GetDateServer");
            this.server_date = res.data;
        },
        saveAllEvent: async function() {            
            const $table = this.$refs.xTable1;
            if(this.headData.id==="" && this.headData.out_dept===""){                
                this.$XModal.alert({ content: '組裝部門不可為空!', mask: false });//mask: false 彈窗時父窗口不需庶置
                return;
            }
            if(this.headData.transfer_date ===""){
                this.$XModal.alert({ content: '組裝日期不可為空!', mask: false });
                return;
            }
            if($table.tableData.length == 0){
                this.$XModal.alert({ content: '明細資料不可為空!', mask: false });
                return;
            }
            if(this.$refs.xTable2.tableData.length == 0){
                this.$XModal.alert({ content: '成份資料不可為空!', mask: false });
                return;
            }
            // 保存前對成份明細資料1進行有效性檢查
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
            //保存前對成份明細資料2進行有效性檢查
            let items_update = null,row_no=0,sequence_id="",check_tableData2_flag = true;
            let strDept="102,202,302,122,222,322,104,124";
            for(let i=0;i<$table.tableData.length;i++){
                row_no = i+1;
                items_update = $table.tableData[i];
                sequence_id = items_update.sequence_id;
                for(let j=0;j < this.originData2.length;j++){
                    if(this.originData2[j].upper_sequence === sequence_id){
                        //開料部門,批號不可為空
                        if(strDept.indexOf(this.headData.out_dept) != -1){
                            if(this.originData2[j].lot_no === "") {
                                this.$refs.xTable1.setCurrentRow(items_update);//定位至某一行
                                this.curRowSeqId = sequence_id;
                                this.setTableData2ByCurrentMo();
                                check_tableData2_flag = false;                               
                                break;
                            }
                        }
                        if(this.originData2[j].goods_id === "" || parseFloat(this.originData2[j].con_qty)<=0 || parseFloat(this.originData2[j].sec_qty)<=0) {
                            this.$refs.xTable1.setCurrentRow(items_update);//定位至某一行
                            this.curRowSeqId = sequence_id;
                            this.setTableData2ByCurrentMo();
                            check_tableData2_flag = false;
                            break;
                        }
                    }
                }
                if(!check_tableData2_flag){
                    break;
                }
            }   
            if(!check_tableData2_flag){
                //成份資料數據有效性通不過
                this.$XModal.alert({ content: `第${row_no}行:【${items_update.mo_id}】【${items_update.goods_id}】\n對應成份資料數據不完整,請返回檢查!`, mask: false });
                return;
            }            
            //保存前各项檢查,是否交QC,是否大于生产数等,返回放棄當前保存
            await this.checkPreUpdate();
            if(!this.preUpdateFlag){
                console.log("no pass");
                //console.log(this.tmp_turn_over);
                //console.log(this.tmp_turn_over_qc);
                return;
            }            
            if(this.preUpdateFlag){
                console.log("pass");
                return;
            }
           
            return;

            //檢查成份庫存是否夠扣減
            //傳給后端的存儲過程的表數據類型參數結構
            this.partData = [];
            for(let i in this.originData2){
                this.partData.push(
                    {within_code:'0000',out_dept:this.headData.out_dept,upper_sequence:this.originData2[i].upper_sequence,sequence_id:this.originData2[i].sequence_id,
                    mo_id:this.originData2[i].mo_id,goods_id:this.originData2[i].goods_id,lot_no:this.originData2[i].lot_no,con_qty:this.originData2[i].con_qty,
                    sec_qty:this.originData2[i].sec_qty});                
            }
            var partData = this.partData;
            await this.checkStock(partData); //此處必須加await,且checkStock函數也要設置成同步執行
            if(!this.validStockFlag){
                //檢查成份庫存不足,返回放棄當前保存
                return;
            }            
            //******以上是數據校驗部分

            this.headData.head_status = this.headEditFlag;//表頭新增或修改的標識
            var headData = JSON.parse(JSON.stringify(this.headData));
            var lstDetailData1 = this.tableData1;
            var lstDetailData2 = this.originData2;
            var lstDelData1 = this.delData1;
            var lstDelData2 = this.delData2;           
            axios.post("/ProduceAssembly/Save",{headData,lstDetailData1,lstDetailData2,lstDelData1,lstDelData2 }).then(
                (response) => {
                    if(response.data=="OK"){                                  
                        this.setStatusHead(false);
                        //重查刷新數據
                        this.getHead(this.headData.id);
                        this.getDetails(this.headData.id);
                        this.getDetailsPart(this.headData.id);
                        this.cancelButton = false;
                        this.$XModal.message({ content: '數據保存成功!', status: 'success' });
                    }else{
                        this.$XModal.alert({ content: '數據保存失敗!',status: 'error' , mask: false });                                 
                    }
                }
            ).catch(function (response) {    
                alert(response);
            });
            this.headEditFlag = "";
            this.delData1=[];
            this.delData2=[];
           
        },

        //保存前檢查庫存是否夠扣除
        checkStock:async function(partData){           
            await axios.post("/ProduceAssembly/CheckStock",{partData}).then(
                (response) => {                    
                    if(response.data.length>0){
                        //庫存檢查不通過
                        let seqNo=response.data[0].upper_sequence;
                        let con_qty=response.data[0].con_qty;
                        let sec_qty=response.data[0].sec_qty;
                        let row_no=0;
                        this.validStockFlag = false;
                        for(let i=0;i<this.tableData1.length;i++){
                            if(this.tableData1[i].sequence_id === seqNo){
                                row_no = i+1;
                                let rw = this.Rows.data[i];
                                this.tempSelectRow = JSON.parse(JSON.stringify(rw));//暫存當前行,以便檢查是否有更改
                                this.$refs.xTable1.setCurrentRow(rw);//定位至當前索引所在的行
                                this.rowDataEdit = rw;
                                this.selectRow = rw;
                                this.curRowSeqId = this.selectRow.sequence_id;
                                this.setTableData2ByCurrentMo();
                                break;
                            }
                        }                       
                        this.$XModal.alert({ content: `第 ${row_no} 行【${this.selectRow.mo_id} -- ${this.selectRow.goods_id}】\n成份:【${response.data[0].goods_id}】庫存不足!\n 数量差異: ${con_qty}\n 重量差異: ${sec_qty}`,status: 'warning' , mask: false });
                    }else{
                        this.validStockFlag = true;//庫存檢查通過
           }}) 
        },
        //--start保存前檢查
        checkPreUpdate:async function(){
            let li_rtn,li_rc,li_row,li_prd_id,li_count,li_row_no;
            let	ls_op_dept='',ls_assembly_dept='',ls_sequence_id='',ls_lot_no='',ls_materiel_id='',ls_message='',ls_materiel_mo_id='',ls_parameter='';          
            let	ls_mo_id='',ls_goods_id='',ls_id='',ls_in_dept='',ls_id_tmp='';
            let	lb_pope = false,lb_prompt = false;           
            let	ldc_assembly_qty=0,ldc_qty_other=0,ldc_sec_qty_other=0,ldc_prod_qty=0,ldc_prod_sec_qty=0,ldc_qc_qty=0,ldc_qc_sec_qty=0,ldc_sec_qty=0;
            this.preUpdateFlag = true;
            //清空移交單臨時數據
            this.tmp_turn_over = [];//移交單臨時數組
            this.tmp_turn_over_qc = [];//QC移交單臨時數組
            ls_id = this.headData.id;
            ls_assembly_dept = this.headData.out_dept;
            ls_in_dept = this.headData.in_dept; //dw_master.GetItemString(dw_master.GetRow(),'in_dept')
            if(this.$utils.getSize(ls_in_dept) === 0){
                ls_in_dept = '';
            }    
            if(ls_in_dept===""){
                //如不輸入接收部門,則不需生成移交單及交QC移交單數據
                return;
            }
            //--是否可超生產數開移交單
            await axios.post("/ProduceAssembly/CheckParameters",{id:'JO0501'}).then(
                 (response) => {
                     ls_parameter=response.data;
            })               
            if(ls_parameter === null || ls_parameter.length===0){
                ls_parameter='1';
            }  
            //檢查組裝部門是否為外發加工部門
            //await axios.post("/ProduceAssembly/CheckPlateDept",{id:ls_assembly_dept}).then(
            //      (response) => {
            //          ls_op_dept = response.data;
            //})
            //if(ls_op_dept===null || ls_op_dept.length===0){
            //    ls_op_dept='0';
            //} 
            for(let li_row=0;li_row<this.tableData1.length;li_row++){
                li_row_no = this.$utils.toInteger(li_row)+1;
                ls_mo_id = this.tableData1[li_row].mo_id;
                ls_goods_id = this.tableData1[li_row].goods_id;
                ldc_assembly_qty = parseFloat(this.tableData1[li_row].con_qty);
                ldc_sec_qty = parseFloat(this.tableData1[li_row].sec_qty);
                ls_sequence_id = this.tableData1[li_row].sequence_id;                
                li_prd_id = parseInt(this.tableData1[li_row].prd_id);
                if(li_prd_id===null){
                    li_prd_id=0;
                }
                //--start 檢查使用批量組裝功能時避免重復做組裝轉換
                ls_id_tmp = '';
                if(li_prd_id>0){
                    await axios.post("/ProduceAssembly/CheckIsExistsPrdId",{prd_id:li_prd_id,id:ls_id}).then(
                    (response) => {
                        ls_id_tmp = response.data;
                    })
                    if(this.$utils.getSize(ls_id_tmp)>0){
                        await this.$XModal.alert({ content: `第 ${li_row_no} 行【${ls_mo_id}--${ls_goods_id}】\n已做過組裝轉換!\n\n【${ls_id_tmp}】`,status: 'warning' , mask: false });
                        this.preUpdateFlag = false;
                        break;
                    }
                }                
                //--end
                //--start从生产数据库中批量增加时,有可能生产计划中的流程已经改变
                if(ls_in_dept !=''){
                    await axios.post("/ProduceAssembly/CheckIsExistsPlan",{mo_id:ls_mo_id, goods_id:ls_goods_id, wp_id:ls_assembly_dept, next_wp_id:ls_in_dept}).then(
                        (response) => {
                            li_count = parseInt(response.data);
                        })
                    if(li_count=== 0){
                        await this.$XModal.alert({ content: `第 ${li_row_no} 行,生產計劃中查找不到對應的流程!\n【${ls_mo_id}】\n【${ls_goods_id}】\n [${ls_assembly_dept}-->${ls_in_dept}]`,status: 'warning' , mask: false });
                        this.preUpdateFlag = false;
                        break;
                    }
                }
                //--end
                //将当前数据Copy到移交单数据中
                //dw_detail.RowsCopy(ll_row,ll_row,Primary!,ids_turn_over,ids_turn_over.RowCount() + 1,Primary!)
                this.tmp_turn_over.push(JSON.parse(JSON.stringify(this.tableData1[li_row])));               
                //--判断当前流程是否有未完成的对应的QC流程,如果有提示当前组装数量是否包含QC数量
                //檢查是否有交QC的流程  ldc_qc_qty = wf_get_qc_qty(ll_row)
                await axios.post("/ProduceAssembly/GetQcQty",{mo_id:ls_mo_id, goods_id:ls_goods_id, wp_id:ls_assembly_dept}).then(
                    (response) => {
                        ldc_qc_qty = parseFloat(response.data);
                })
                //如果没有输入收货部门就不需要自动生成移交单,所以不需要进行QC流程的判断,2022/04/26 增加是否先交P10生產部的檢查  
                if(ldc_qc_qty > 0 && ls_in_dept != '') {                  
                    await this.$XModal.confirm(`第${li_row_no}行【頁數:${ls_mo_id}--${ls_goods_id}】有未完成移交QC的流程,請確認當前組裝數量中已經包含有交QC的數量!`).then(type => {
                        if (type === 'cancel') {
                            this.preUpdateFlag = false;
                        }
                    })
                    if(!this.preUpdateFlag){
                        break;
                    }
                    //--先计算正常移交单的数量
                    var li_row_qc =0;
                    if(ldc_assembly_qty > ldc_qc_qty){ //即有正常移交单数量的情况
                        ldc_qc_sec_qty = 0.01;  //默认QC重量
                        ldc_assembly_qty = ldc_assembly_qty - ldc_qc_qty;
                        ldc_sec_qty = ldc_sec_qty - ldc_qc_sec_qty;
                        if(ldc_sec_qty <0.01){
                            ldc_sec_qty = 0.01;  //默认最小重量
                        }
                        Object.assign(this.tmp_turn_over[li_row].con_qty, ldc_assembly_qty); //如果要生成交QC移交單,需修改原來非交QC移交單的數量
                        Object.assign(this.tmp_turn_over[li_row].sec_qty, ldc_sec_qty); //如果要生成交QC移交單,需修改原來非交QC移交單的重量
                        //插入一行交QC的移交單數據
                        this.tmp_turn_over_qc.push(JSON.parse(JSON.stringify(this.tableData1[li_row]))); //將當前數據Copy到移交單數據中
                        li_row_qc = this.tmp_turn_over_qc.length;
                        Object.assign(this.tmp_turn_over_qc[li_row_qc].con_qty, ldc_qc_qty); //当作QC数据
                        Object.assign(this.tmp_turn_over_qc[li_row_qc].sec_qty, ldc_qc_sec_qty); //当作QC数据
                    }else{
                        this.tmp_turn_over_qc.push(JSON.parse(JSON.stringify(this.tableData1[li_row]))); //仅QC移交單數據
                        li_row_qc = this.tmp_turn_over_qc.length;
                    }
                    Object.assign(this.tmp_turn_over_qc[li_row_qc].location, '702');
                    Object.assign(this.tmp_turn_over_qc[li_row_qc].carton_code, '702');
                }
                //--判断組裝数量是否大于生產計劃單中的數量
                ldc_qty_other = 0;
                ldc_sec_qty_other = 0;
                //--已组装过的数量
                await axios.post("/ProduceAssembly/GetAssemblyQty", { id:ls_id,mo_id: ls_mo_id,goods_id:ls_goods_id,out_dept:ls_assembly_dept } ).then(
                    (response) => {                            
                        ldc_qty_other = parseFloat(response.data[0].con_qty);
                        ldc_sec_qty_other = response.data[0].sec_qty;
                    }
                );
                //检查流程中的生产数量
                await axios.post("/ProduceAssembly/GetPlanProdQty", { out_dept:ls_assembly_dept,mo_id: ls_mo_id,goods_id:ls_goods_id }).then(
                    (response) => {                            
                        ldc_prod_qty = parseFloat(response.data);
                    }
                );                
                if(ldc_prod_qty === 0){                    
                    await this.$XModal.alert({ content: `第 ${li_row_no} 行【${ls_mo_id} -- ${ls_goods_id}】\n\n對應生計劃流程中的生產數量為 0，不允許保存！`,status: 'warning' , mask: false });
                    this.preUpdateFlag = false;
                    break;
                }                
                //--ls_parameter:检查是否允許超过生产数量移交,0為不允許,但可通過密碼確認放行
                if(ls_parameter === '0' && lb_prompt === false){
                    if(ldc_assembly_qty+ldc_qty_other>ldc_prod_qty){
                      await this.$XModal.confirm(`第${li_row_no}行:【${ls_mo_id}--${ls_goods_id}】\n移交数已超過生產數，如要繼續保存，請選擇【确定】\n并在彈出窗口中輸入有權限的用户和密碼!`).then(type => {
                           if (type === 'cancel') {
                               this.preUpdateFlag = false;
                           }
                      })
                      if(!this.preUpdateFlag){
                          break;
                      }
                      //*****************************
                      //打開密碼確認窗口,要等此窗口做完所有的事情才執后面的語句,否則就處于等待狀態   
                      await this.$prompt(`當前用戶: ${this.userId}`,"請輸入密碼",{
                          confirmButtonText: '确定',
                          cancelButtonText: '取消',
                          closeOnClickModal: false,//禁需點擊彈窗之外空白處或選中是自動關閉的問題
                          type:"warning", // 图标样式 "warning"|"error"...
                          inputType:"Password",
                          inputValue: '',                          
                          //inputErrorMessage: '输入不能为空',
                          //inputValidator: (value) => {// 点击按钮时，对文本框里面的值进行验证
                          //    if(!value) {
                          //        return '输入不能为空';
                          //    }
                          //},        
                          //阻止关闭（beforeClose中如果不调用done()弹框就无法关闭）
                          beforeClose: (action, instance, done) => {
                               if(action === 'confirm' ){//&& !ipReg.test(instance.inputValue)                                 
                                   let strPassword = instance.inputValue;                                  
                                    axios.get("/Base/Common/GetUserInfo?user_id=" + this.userId + "&password=" + strPassword).then(
                                       (response) => {
                                           if (response.data === "USER_ID_ERROR") {
                                               this.valid_user_id = false;
                                               this.$message({type:'warning',message: '輸入用戶有誤!'});                                              
                                               return;
                                           }                                           
                                           if (response.data === "PASSWORD_ERROR") {
                                               this.valid_user_id = false;
                                               this.$message({type:'warning',message: '輸入的密碼有誤!!'});
                                               return;
                                           }
                                           this.valid_user_id = true;//用戶密碼檢查通過                                          
                                           done();
                                       } 
                                   ).catch(function (response) {
                                       this.valid_user_id = false;
                                       this.$message({type:'warning',message: '程序錯誤!'});
                                       alert(response);
                                   }); 
                               }else{
                                   this.valid_user_id = false;
                                   done();
                               } 
                          }
                      }).then(({value}) => {
                          if(value==='cancel'){
                              this.valid_user_id = false;
                          }
                          //console.log(value); // TO DO DO ...                         
                      }).catch((err) => {
                          console.log(err);
                      }); 
                      ////************
                      
                      if(this.valid_user_id ===false){ //this.valid_user_id為密碼確認窗口檢查通過時/this.valid_user_id=true,不通過this.valid_user_id=false                         
                          this.preUpdateFlag = false; //保存前檢查通不過,退出循環
                          break;//退出循環
                      }else{
                          this.preUpdateFlag = true; //保存前檢查通過,繼續循環
                      }                                           
                      lb_prompt = true;//当有多个货品时,也只提示一次 只需彈出密碼檢查一次   
                      await axios.get("/ProduceAssembly/GetUserPope", { params: { user_id:this.userId} }).then(
                          (response) => {
                              lb_pope = response.data;
                          }
                      );
                      if(!lb_pope){
                          await this.$XModal.alert({ content: '當前用戶沒有移交特批的權限!',status: 'info' , mask: false });
                          this.preUpdateFlag = false;
                          break;
                      }
                   }
                }                
            } //--end for
        },//--end 保存前檢查
        
        //恢復所有行的更改
        revertDataAll: async function() {
            const type = await this.$XModal.confirm('確定要還原當前表格所做的更改(恢復到修改之前)?');
            if (type === 'confirm') {
                const xTable = this.$refs.xTable1;
                const xTable2 = this.$refs.xTable2;
                xTable.revertData();
                xTable2.revertData();
                this.originData2 = JSON.parse(JSON.stringify(this.tempTableData2));
                this.cancelButton = false;
                //this.tempTableData2 =  JSON.parse(JSON.stringify(response.data));
            }
        },        
        cellStyle({ row, rowIndex, column }){            
            if(row.row_status !=""){
                return {
                    backgroundColor: '#FFF8D7',
                    color: '#000000'
                }
            }
        },
        
       //beforeHideMethod: async () => {
       //    const type = await VXETable.modal.confirm('您确定要关闭吗？')
       //    if (type === 'confirm') {
       //        VXETable.modal.message({ content: `允许关闭 ${type}`, status: 'success' })
       //    } else {
       //        VXETable.modal.message({ content: `禁止关闭 ${type}`, status: 'error' })
       //        return new Error()
       //    }
       //}

    }, //methods end
    watch: {
        headData:{
            handler (newValue, oldValue){
                if(this.headEditFlag !="NEW"){
                    let edit_flag = false;
                    for (let i in this.headData) {
                        if (this.headData[i] != this.tempHeadData[i]) {
                            edit_flag = true;
                            break;
                        }
                    }
                    if(edit_flag){                        
                        this.setStatusHead(true);//設置工具欄狀態,切換按鈕顯示                        
                        if(this.headEditFlag ===""){
                            //已保存的數據,如有修改
                            this.headEditFlag = "Edit";
                        }
                    }
                }
            },
            deep: true
        },
        //監控表格1當前數據行的變化
        selectRow:{
            handler (val, oldVal) {
                if(this.headData.state ==='0'){                    
                    if(this.selectRow===null){
                        //2022/08/08
                        return;
                    }                    
                    let edit_flag = false;
                    let upper_sequence = "";
                    let sequence_id = "";
                    let item="";
                    for (let i in this.selectRow) {
                        if (this.selectRow[i] != this.tempSelectRow[i]) {                            
                            edit_flag = true;                            
                            if(this.selectRow.con_qty !=this.tempSelectRow.con_qty){
                                //數量有更改,成份數量跟著改
                                for(var j=0;j < this.tableData2.length;j++){
                                    if(this.tableData2[j].row_status === ""){
                                        this.tableData2[j].row_status = "EDIT";
                                    }                                    
                                    item = this.tableData2[j].goods_id;
                                    if( item.substr(0,2) === "ML"){
                                        this.tableData2[j].con_qty = 0.01;
                                    }else{
                                        this.tableData2[j].con_qty = this.selectRow.con_qty;
                                    }
                                    upper_sequence = this.tableData2[j].upper_sequence;
                                    sequence_id = this.tableData2[j].sequence_id;
                                    for(var k=0;k < this.originData2.length;k++){
                                        if(this.originData2[k].upper_sequence === upper_sequence && this.originData2[k].sequence_id ===sequence_id){
                                            Object.assign(this.originData2[k], this.tableData2[j]);//當前行寫入數組
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    if(edit_flag){
                        if(this.headEditFlag==="" || this.headEditFlag===undefined){
                            this.headEditFlag="EDIT"                            
                        }
                        this.setStatusHead(true);//設置工具欄狀態,切換按鈕顯示
                    }
                }
            },
            deep: true
        },
        //監控表格2當前數據行的變化
        selectRow2:{
            handler (val, oldVal) {               
                if(this.headData.state ==='0'){
                    let editFlag = false;
                    let upper_sequence = this.selectRow2.upper_sequence;
                    let sequence_id = this.selectRow2.sequence_id; 
                    for (let i in this.selectRow2) {
                        if (this.selectRow2[i] != this.tempSelectRow2[i]) {
                            editFlag = true;
                            if(this.headEditFlag ==="" || this.headEditFlag===undefined){
                                this.headEditFlag="EDIT"
                            }
                            break;
                        }
                    }
                    if(editFlag){
                        this.setStatusHead(true);//設置工具欄狀態,切換按鈕顯示
                        if(this.selectRow2.row_status ==='EDIT'){
                            for(var i=0;i < this.originData2.length;i++){
                                if(this.originData2[i].upper_sequence === upper_sequence && this.originData2[i].sequence_id ===sequence_id){
                                    Object.assign(this.originData2[i], this.selectRow2);//當前行寫入數組
                                    break;
                                }
                            }
                        }
                        if(this.selectRow2.row_status ==='NEW'){
                            let existsFlag = false;
                            for(var i=0;i < this.originData2.length;i++){
                                if(this.originData2[i].upper_sequence === upper_sequence && this.originData2[i].sequence_id ===sequence_id){
                                    existsFlag = true;//已存在
                                    Object.assign(this.originData2[i], this.selectRow2);
                                    break;
                                }
                            }
                            if(!existsFlag){
                                this.originData2.push(this.selectRow2);
                            }
                        }                        
                    }
                }
            },
            deep: true
        },
        searchID : function (val) {
            this.searchID = val.toUpperCase();
        }
    },
    computed: {
        //
    },
    mounted() {
        this.tableHeight=$(parent.window).height()-(172+400+35);
        this.div_tab1_height = $(parent.window).height()-155 +'px';//第一页Div的高度
        let that = this; 
        window.onresize = () => {
            return (() => {
                that.tableHeight=$(parent.window).height()-(172+400+35); 
                that.div_tab1_height = $(parent.window).height()-155 +'px';
            })()
        };  
        this.selectTab= "tab2";
    }

}

var app = new Vue(main).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;
Vue.prototype.$utils = XEUtils;

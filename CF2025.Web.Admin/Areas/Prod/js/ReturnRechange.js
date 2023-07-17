var main ={
    data()  {
        return {
            searchID: "", 
            headEditFlag: "",//主檔為新增或編輯
            curRowIndex: null,
            selectRow: null,
            selectFindItemRow:null, //選擇貨品編號
            seledtLotNoRow:null,//選擇批號號           
            headerCellStyle:{background:'#F5F7FA',color:'#606266',height:'25px',padding:'2px'},
            loading: false,
            showEdit: false,
            submitLoading: false,          
            showFind:false,
            showGoods:false,
            showLot:false,
            showGoodsX:0,
            showGoodsY:0,          
            stateFontColor:"color:black",
            btnItemTitle:"劉覽",
            headData: { id: "", con_date: "", out_dept: "", in_dept: "",bill_origin:"2",bill_type_no:"R",stock_type:"0",handler:"", remark: "", create_by: "", update_by: "", check_by: "", 
                update_count: "", create_date: "", update_date: "", check_date: "", state: "0",head_status:"" },
            rowDataEdit: {
                id: '', mo_id: '', goods_id: '', goods_name: '',con_qty: 0,  unit_code: 'PCS', sec_qty: 0.00,sec_unit: 'KG',lot_no:'', package_num: '0',color_name: '', 
                four_color: '', app_supply_side: '', remark: '', return_qty_nonce: '',sequence_id: '', location: '',carton_code:'', prd_id: 0,  jo_id:'',jo_sequence_id:'',
                qc_qty:0,return_reason:'',row_status:'',aim_jo:'',aim_jo_sequence:'',qc_result:''
            },
            billOriginList:[],
            deptList:[],
            stateList:[],
            qtyUnitList:[],
            wtUnitList:[],
            locationList:[],
            cartonCodeList: [{ label: '', value: '' }],
            goodsPlanList:[],
            lotNoList:[],
            tableData1:[],
            delData1:[],
            formDataFind:{},
            isEditHead: false,//主檔編輯狀態
            isEditDetail: false,//明細編輯狀態
            isReadOnly: true,//主檔明細共用,控制是否可以更改資料
            isDisableGenNo:true,//是否可以更改負責部門與接收部門并生成單據編號
            isEditCell:false,
            userId:"",            
            tempHeadData: {},
            tempTableData1: {},
            tempSelectRow:{},       
            tableHeight :450,
            tempSaveData:[],
            checkStockData:[],
            findData:[],
            insertRecords:{}, 
            removeRecords:{}, 
            updateRecords:{},            
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
            div_tab1_height:'300px',
            rows:[],            
            validStockFlag:false,
            preUpdateFlag:false,
            tmp_turn_over:[],//移交單臨時數據
            tmp_turn_over_qc:[],//QC移交單臨時數據
            valid_user_id:false,
            unok_status:"1",//檢查移交單是否已批準,如果已批準則返回"1",反批準按鈕不顯示, 當前默認1是不顯示
            ok_status:"0",//控制批準按鈕的顯示, 當前默認0是顯示            
            temp_goods_id:"",
          
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
    },

    methods: {
        findByID:async function() {
            if(this.isEditHead===true){
                this.$XModal.message({ content: "編輯狀態,當前操作無效!", status: "info" });
                return;
            }
            if(this.headEditFlag != "" || this.searchID ===""){
                return;
            }
            this.loading = true;
            //同步優先執行,因setTimeout()中的異步執行要依賴同步執行的結果
            await this.getHead(this.searchID); 
            await this.getDetails(this.searchID);
            //異步執行 
            setTimeout(() => {
                //this.checkRechangeApproveStatus();//檢查控制反批準按鈕的顯示/隱藏.   *********
                this.loading = false;
            }, 500);
            //this.ok_status = this.headData.state;//控制批準按鈕的顯示(0:顯示)       ************     
        },

        //打開查詢窗體
        showFindWindos(){            
            comm.openWindos('ReturnRechange');
        },

        //打開物品查詢窗體
        showFindItem(){
            comm.openFindItem();
        },
        //showPassword:async function(){
        //    await comm.openPassword(this.userId);
        //},
        printEvent(){
            //
        },    

        focusInDept(){
            this.$refs['inDept'].focus(); //this.$refs包含自定義的引用對象
        },
        setUpperCase(strField,value){
            value = comm.stringToUppercase(value);
            this.$set(this.selectRow, strField, value ); //如字母相同只是大小寫不同,修改狀態合仍然為FALSE
            this.setCancelStatus();            
        },        
        checkNumber(obj,strField,value) {
            comm.allNumber(obj,strField,value);
            this.setCancelStatus();
        },
        checkNumberDec2(obj,strField,value){    
            comm.allNumberDec2(obj,strField,value);           
            this.setCancelStatus();
        },
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
        
        //初始化下拉列表框
        getComboxList(SourceType) {
            axios.get("/Base/DataComboxList/GetComboxList?SourceType=" + SourceType).then(
                (response) => {
                    switch (SourceType) {
                        case "BillOrigin":
                            this.billOriginList = response.data;
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
            this.isDisableGenNo = false; //可用生成單據編號
            //清空數據
            this.clearHeadData();
            this.tableData1 = [];//清空明細表格數據   
           
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
                if(this.headData.state == "2") {
                    this.$XModal.alert({ content: '已是注銷狀態,當前操作無效!',status: 'info', mask: false });
                    return;
                }
                if(this.headData.state == "1"){
                    this.$XModal.alert({ content: '批準狀態,當前操作無效!',status: 'info', mask: false });
                    return;
                }
                this.$XModal.confirm('請確定是否要注銷此移交退回單？').then(type => {
                    if (type == 'confirm') {
                        var head = JSON.parse(JSON.stringify(this.headData));
                        var user_id = this.userId;
                        axios.post("/ReturnRechange/DeleteHead",{head,user_id}).then(
                            (response) => {
                                if(response.data=="OK"){                
                                    this.setStatusHead(false);
                                    //重查刷新數據
                                    this.getHead(this.headData.id);                                    
                                    this.$XModal.message({ content: '注銷成功!', status: 'success' });
                                }else{
                                    this.$XModal.alert({ content: '注銷失敗!',status: 'error', mask: false });
                                }
                        }).catch(function (response) { 
                               alert(response);
                        });                        
                    }                    
                })
            } else {
                this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!', status: 'warning', mask: false });
                return;
            };
        },
        
        //還原表頭及全部的明細
        revertEvent() {
            this.setStatusHead(false);
            this.headData = JSON.parse(JSON.stringify(this.tempHeadData));//還原主表
            this.tableData1 = JSON.parse(JSON.stringify(this.tempTableData1));//還原明細            
            this.delData1 = [];          
            this.rows = JSON.parse(JSON.stringify(this.tableData1));
            this.isDisableGenNo = this.tempDisableGenNo;
            this.headEditFlag = "";
            this.isReadOnly = (this.headData.state==='0')?false:true;
            //this.cancelButton = false;

            if(this.tableData1.length>0){                
                this.tempSelectRow = JSON.parse(JSON.stringify(this.rows[0]));
                this.selectRow = this.rows[0];//JSON.parse(JSON.stringify(this.rows[0]));/;////this.rows.data[0];//當前行
                //this.selectRow = this.tableData1[0];
                var $table = this.$refs.xTable1;
                $table.setCurrentRow(this.selectRow);//定位至當前索引所在的行
                $table.setActiveCell(this.selectRow, "mo_id");//設置單元格焦點
            }
        },        
        setStatusHead(blValue) {            
            this.isEditHead = blValue; //設置編輯狀態標識
            this.btnItemTitle = (this.headData.state==='0')?'修改':'劉覽';
        },
        clearHeadData(){
            this.headData.id="",
            this.headData.con_date = "",
            this.headData.bill_origin = "2",
            this.headData.bill_type_no = "R",
            this.headData.out_dept = "",
            this.headData.in_dept = "",
            this.headData.stock_type ="0",
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
            this.rowDataEdit = {id: '', mo_id: '', goods_id: '', goods_name: '',con_qty: 0,  unit_code: 'PCS', sec_qty: 0.00,sec_unit: 'KG',lot_no:'',
                 package_num: '0',color_name: '', four_color: '', app_supply_side: '', remark: '', return_qty_nonce: '',sequence_id: '',
                location: '', carton_code:'', prd_id:0, jo_id:'', jo_sequence_id:'', qc_qty:0, return_reason:'',row_status:'', aim_jo:'', aim_jo_sequence:'', qc_result:''
            }
        },
        backupData () {
            this.tempHeadData = JSON.parse(JSON.stringify(this.headData));//暫存主檔臨時數據
            this.tempTableData1 = JSON.parse(JSON.stringify(this.tableData1));//暫存明細臨時數據
        },
        //項目新增1
        insertRowEvent() {
            if(this.headData.state !="0"){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!',status: 'info', mask: false });
                return;
            }
            if (this.headData.id == "") {              
                this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!',status: 'info', mask: false });
                return;
            }; 
            var rowEndIndex = -1;//添加至行尾
            var $table = this.$refs.xTable1;
            this.clearRowDataEdit();
            this.selectRow = null;           
            this.$set(this.rowDataEdit, "id", this.headData.id); 
            this.$set(this.rowDataEdit, "sequence_id", this.getSequenceId($table.tableData.length));
            this.$set(this.rowDataEdit, "row_status", 'NEW');
            //this.cancelButton = true;

            this.tableData1.push(this.rowDataEdit);
            this.selectRow = this.rowDataEdit;
            $table.setCurrentRow(this.rowDataEdit);//定位至當前索引所在的行
            $table.setActiveCell(this.rowDataEdit, "mo_id");//設置單元格焦點                               
        },              
        editRowEvent(row){
            this.tempSelectRow = JSON.parse(JSON.stringify(row));//暫存當前行,以便檢查是否有更改            
            this.selectRow = row;
            this.rowDataEdit = row;
            this.isReadOnly = (this.headData.state==='0')?false:true; 
            this.btnItemTitle = (this.isReadOnly)?'劉覽':'修改';
            this.showEdit = true;
        },

        //項目刪除
        tempDelRowEvent:async function(){
            if(this.headData.state !='0'){
                this.$XModal.alert({ content: "此單據已批準,當前操作無效!",status: 'info', mask: false });
                return;
            }
            let status = await comm.checkApproveState('jo_materiel_con_mostly',this.headData.id);
            if(status ==='1'||status ==='2'){
                let str = (status ==='1')?"批準":"注銷";
                this.$XModal.alert({ content: `后端數據已是${str}狀態,當前操作無效!`,status: 'info', mask: false });
                return;
            }
            if(this.curDelRow){
                if(this.tableData1.length===1){
                    this.$XModal.alert({ content: "已是最后一行,不可以繼續刪除!",status: 'info', mask: false });
                    return;
                }
                this.$XModal.confirm("是否確定要刪除當前行？").then(type => {
                    if(type ==="confirm"){
                        this.delData1.push(this.selectRow); 
                        this.tableData1.splice(this.curRowIndex,1);//移除表格1刪除的當前行
                        this.isEditHead = true;
                        this.curDelRow = null;
                    }
                })
            }else{
                this.$XModal.alert({ content: '請首先指定需要刪除的項目!',status: 'info',mask: false });
            }
        },
        //取移交單最大單號
        getMaxID(outDept,inDept,docType) {
            if(outDept==='' || inDept==='' ){
                return;
            }
            var $table = this.$refs.xTable1;
            axios.get("/Base/Common/GetMaxIDJo07?out_dept=" + outDept + "&in_dept=" + inDept + "&doc_type=" + docType).then(
                (response) => {
                    this.headData.id = response.data;
                    //直接更改表格中某單元格的值,不可以直接更改綁定數組的方式,否則將取不到數據的更改狀態
                    for (var i = 0; i < $table.tableData.length; i++) {
                        this.$set($table.tableData[i], "id", this.headData.id );
                        this.$set($table.tableData[i], "sequence_id", this.getSequenceId(i));
                        this.$set($table.tableData[i], "location", outDept);
                        this.$set($table.tableData[i],"carton_code",outDept);
                    }
                }
            );
        }, 

        //生成明細序號
        getSequenceId(i){          
            return comm.pad(i + 1,4) +"h";
        },        

        //批準&反批準 val: 1--批準,0--反批準
        approveEvent:async function(val) {
            if(this.isEditHead===true){
                this.$XModal.alert({ content: "編輯狀態,當前操作無效!", mask: false });
                return;
            } 
            //批準,反批準都要檢查是否是註銷狀態
            if(this.headData.state === "2"){
                this.$XModal.alert({ content: "注銷狀態,當前操作無效!", mask: false });
                return;
            }
            
            if ((this.headData.id != "") && (this.tableData1.length > 0)) {
                let	ls_id,ls_bill_type_no,is_active_name,ls_success,ls_error,ls_type,ls_approve,status;
                    
                ls_id = this.headData.id;
                ls_bill_type_no = this.headData.bill_type_no;
                is_active_name = (val==='1')?"pfc_ok":"pfc_unok";
                ls_success = (val==='1')?"批準成功!":"反批準成功!";
                ls_error = (val==='1')?"批準失敗!":"反準失敗!";
                ls_type = (val==='1')?"批準":"反批準";
                ls_approve = `確定是否要進行【${ls_type}】操作？`;
                //取后端單據狀態
                status = await comm.checkApproveState('jo_materiel_con_mostly',this.headData.id);
                if(status==='2'){
                    this.$XModal.alert({ content: "后端數據已是注銷狀態,當前操作無效!", mask: false });
                    return;
                }
                //點擊批準按鈕
                if(val==='1') {
                    //進行當前批準操作前再次檢查后端是否已被別的用戶批準.
                    if(this.headData.state === "1"){
                        this.$XModal.alert({ content: "已是批准狀態,當前操作無效!", mask: false });
                        return;
                    }
                    if(status === "1"){
                        //后臺數據已為批準狀態,已被別的用戶批準
                        this.$XModal.alert({ content: "后端數據已是批準狀態,當前操作無效!", mask: false });
                        return;
                    }
                    //批準前檢查庫存                    
                    var strError = await this.checkStock(this.headData.id); //此處必須加await,且checkStock函數也要設置成同步執行                    
                    if(this.validStockFlag === false){
                        //檢查庫存不足,返回并放棄當前批準操作
                        this.$XModal.alert({ content: strError, mask: false });
                        return;
                    }
                }
                //點擊反批準按鈕
                if(val === "0") {
                    //進行反批准
                    //進行當前反批準操作前再次檢查后端是否已被別的用戶反批準.
                    if(status === "0"){
                        //后臺數據已為批準狀態,已被別的用戶批準
                        this.$XModal.alert({ content: "后端數據已是未批準狀態,當前操作無效!", mask: false });
                        return;
                    }
                    //檢查已批準日期是否為當日?,超過當日則不可反批準
                    var isApprove = await comm.canApprove(this.headData.id,"w_return_rechange");//移交單
                    if(isApprove === "0"){
                        this.$XModal.alert({ content: "注意:【批準日期】必須為當前日期,方可進行此操作!", mask: false });
                        return;
                    }
                    //對方已经签收了的单据不能再反批准
                    var signFor = await comm.checkSignfor(this.headData.id);
                    if(signFor != "0"){
                        this.$XModal.alert({ content: "接收貨部門已簽收,不可以再進行反批準操作!", mask: false });
                        return;
                    }
                }                  
                
                this.$XModal.confirm(ls_approve).then(type => {
                     if (type === "confirm") {
                            this.headData.check_by = this.userId;
                            var head = JSON.parse(JSON.stringify(this.headData));//深拷貝轉成JSON數據格式
                            var approve_type = val;
                            var user_id = this.userId;
                            this.loading = true;
                       
                            //當前是批準,相反就是反批準,當前是反批準相反就是批準,第二個參數'1'--不顯示,'0'--顯示
                            this.showButtonApprove(approve_type,"1"); //暫不顯示批準或反批準按鈕                            
                            axios.post("/ReturnRechange/Approve",{head,user_id,approve_type}).then(
                                (res) => {
                                    if(res.data ==="OK"){
                                        this.setStatusHead(false);
                                        //重查刷新數據
                                        this.getHead(this.headData.id);//刷新表頭即可
                                        this.$XModal.message({ content: ls_success, status: "success" });  
                                        this.showButtonApprove(approve_type,"0");//成功則顯示相反操作的按鈕
                            } else{
                                        this.$XModal.message({ content: ls_error + res.data,status: "warning" , mask: false });  
                                        this.showButtonApprove(approve_type,"1");//失敗則禁止顯示相反操作的按鈕
                            }
                                    this.loading = false;
                            }).catch(function (res) {
                                this.loading = false;
                                this.$XModal.alert({ content: "系統錯誤:" + res,status: "error" , mask: false }); 
                                this.showButtonApprove(approve_type,"1");//失敗則禁止顯示批準或反批準按鈕
                            });
                     }
                })
            } else {
                 this.$XModal.alert({ content: "主檔編號不可為空,當前操作無效!", status: "warning" });
                 return;
            };
        },//end of approveEvent
       
        //**showButtonApprove(arg1,arg2)控制與當前動作按鈕的相反按鈕的顯示
        //當按下反批準/批準按鈕立即禁止顯示當前按鈕,當操作失敗時再恢復顯示
        //動作類型:approve_type:1--批準;0--反批準. 控制按鈕顯示:value: 1--隱藏,0--顯示
        //當前是批準,相反就是反批準,當前是反批準相反就是批準,第二個參數'1'--不顯示,'0'--顯示
        showButtonApprove(approve_type,value){
            if(approve_type ==="1"){
                this.unok_status = value; //反批準按鈕
            }else{
                this.ok_status = value; //批準按鈕
            }
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
            this.rows = row;//表格的數據對象
            this.curRowIndex = row.$rowIndex;//當前行的索引
            this.selectRow = row.data[row.$rowIndex];//當前行
            this.curDelRow = row.data[row.$rowIndex];//存放可能冊除的當前行
            if(this.headData.state==='0'){
                this.tempSelectRow = JSON.parse(JSON.stringify(this.selectRow));//暫存當前行,以便檢查是否有更改
            }           
            this.showGoodsY = row.$event.clientY;           
        },
        
        //查找流程中的货品编码
        showItem(event){
            this.selectFindItemRow = null;
            var x=0,y=0;
            //event.clientY有些不准确,改为取cellClickEvent的Y座标(this.showGoodsY).
            x = event.clientX;
            y = this.showGoodsY;
            this.setFindItemPosition(x,y);
        },
        showLotNo(event){
            if(this.headData.state==='0') {//單元格為可編輯狀態時
                this.seledtLotNoRow = null;
                this.showGoodsX = event.clientX;
                //提取批号数据
                this.getLotNoData(this.selectRow.goods_id,this.headData.out_dept,this.selectRow.mo_id);
                this.showLot = true;
            }
        },
        //弹窗中确认选中的貨品
        cellClickGetItems(row){
            this.selectFindItemRow = row.data[row.$rowIndex];
        },
        cellClickGetLotNo(row){
            this.seledtLotNoRow = row.data[row.$rowIndex];
        },
        //在指定的屏幕位置中顯示貨品編碼列表
        setFindItemPosition(x,y){
            if(this.isEditCell){//單元格為可編輯狀態時
                //this.curChangeItem = null;
                //x,y點擊點鼠標的座標
                this.showGoodsX = x;
                this.showGoodsY = y;
                //提取生产流程数据
                this.getItemData(this.selectRow.mo_id,this.headData.out_dept,this.headData.in_dept);
                this.showGoods = true;
            }
        },

        //主檔查詢
        getHead:async function(id) {           
            //此處的URL需指定到祥細控制器及之下的動Action,否則在轉出待確認彈窗中的查詢將數出錯,請求相對路徑的問題
            await axios.get("/ReturnRechange/GetHeadByID", { params: { id: id }}).then(
            //await axios.post("/ProduceAssembly/GetHeadByID", { id: id }).then(
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
                        stock_type:response.data.stock_type,
                        row_status:""
                    }
                    this.$set(this.headData,'state',response.data.state);
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
                    rtn = this.headData.handover_id;
                    if(this.headData.state ==='1'){
                        // this.showButtonApprove("1",'0');
                        this.unok_status = "0";//顯示反批準按鈕
                    }else{
                        if(this.headData.state ==='0'){
                            // this.showButtonApprove("0",'0') ;
                            this.ok_status = "0";//顯示批準按鈕
                        }else{
                            this.ok_status = "1";//隱藏批準按鈕
                            this.unok_status = "1";//隱藏反批準按鈕
                        }
                    }
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },//--end getHead

        //明細查詢,必須同步提取數據
        //否則表格二會出現選執行代碼,取不到數據的情況.
        getDetails:async function(id) {
            await axios.get("/ReturnRechange/GetDetailsByID", { params: { id: id }  }).then(
                (response) => {
                    this.tableData1 = response.data;                    
                    if(this.tableData1.length>0){                                           
                        if(this.headData.state ==='0'){
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

        //記錄前后定位
        handlerNextOrPrev(type){
            var curIndex=0;
            if(type==="Next"){
                if(this.curRowIndex < this.rows.data.length-1){
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
            var rw = this.rows.data[curIndex];//取當前索引對應的行對象            
            this.tempSelectRow = JSON.parse(JSON.stringify(rw));//暫存當前行,以便檢查是否有更改            
            this.$refs.xTable1.setCurrentRow(rw);//定位至當前索引所在的行
            this.rowDataEdit = rw;
            this.selectRow = rw;         
        },

        convertSecQtyByQty: async function(row){
            if(row.row_status ==='EDIT' || row.row_status ==='NEW'){
                let res = await axios.post("/Base/Common/QtyToSecQty", { within_code: "0000",location_id:this.headData.out_dept,goods_id:row.goods_id,qty:row.con_qty } );
                this.$set(row, "sec_qty",res.data);
            }
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

        checkItem:async function(goods_id){
            //行處于編輯狀態才進行檢查
            let status = this.selectRow.row_status;
            if(status===""){
                return;
            }
            if(this.$utils.getSize(goods_id)<18){
                //檢查貨品編號,長度小于18位 
                if(goods_id !="" && this.headData.state === '0' ){
                    await this.$XModal.alert({ content: `貨品編碼【${goods_id}】長度有誤,請返回檢查!`,status: 'warning',mask: true });                    
                    this.$refs.xTable1.setActiveCell(this.selectRow, "goods_id");                    
                }
            }else{
                //檢查貨品編號,長度等于18位
                let goods_name = "";
                let res = await axios.post("/Base/PublicItemQuery/CheckItemById", { goods_id: goods_id } );
                goods_name = res.data;

                if(goods_name===""){
                    await this.$XModal.alert({ content: `貨品編碼(18位長度)【${goods_id}】不存在,請返回檢查!`,status: 'warning',mask: true });             
                    this.$refs.xTable1.setActiveCell(this.selectRow, "goods_id");                    
                }else{                  
                    //更改了貨品編碼才更新貨品名稱
                    if(this.selectRow.goods_id === this.temp_goods_id){
                        this.$set(this.selectRow,"goods_name",goods_name)
                    }                    
                }
            }
        }, 
        //負責部門,接收部門互調換
        getItemData(mo_id,out_dept,in_dept){
            if(this.$utils.isEmpty(mo_id)){
                this.$refs.xTable1.setActiveCell(this.selectRow, "mo_id");
                //this.$XModal.alert({ content: '頁數不可以為空!', mask: false });
                return;
            }
            axios.get("/ProduceAssembly/GetGoodsId", { params: { mo_id: mo_id,out_dept:in_dept,in_dept:out_dept } }).then(
                (response) => {
                    this.goodsPlanList = response.data;
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        
        //批號下拉列表框
        getLotNoData(goods_id,out_dept,mo_id){
            if(this.$utils.isEmpty(goods_id)){
                this.$refs.xTable1.setActiveCell(this.selectRow, "goods_id");                
                return;
            }
            axios.get("/ProduceAssembly/GetLotNoList", { params: { goods_id: goods_id,location_id:out_dept,mo_id:mo_id } }).then(
                (response) => {
                    this.lotNoList = response.data;
                    //要對checkbox綁定的對應字段值轉換成布尔型值,否则不会出现打勾的效果
                    for (var i = 0; i < this.lotNoList.length; i++) {
                        if(this.lotNoList[i].is_qc==='1'){
                            this.$set(this.lotNoList[i], "is_qc", true );
                        }else{
                            this.$set(this.lotNoList[i], "is_qc", true );
                        }
                    }
                }
            ).catch(function (response) {
                alert(response);
            });
        },

        //確認选中的貨品編碼(更改)
        changeItem() {
            //if(this.curChangeItem){  
            if(this.selectFindItemRow) {
                if(this.selectRow.goods_id != this.selectFindItemRow.goods_id){
                    //貨品編碼被修改時                   
                    this.$set(this.selectRow, "goods_id", this.selectFindItemRow.goods_id);
                    this.$set(this.selectRow, "goods_name", this.selectFindItemRow.goods_name);
                    this.$set(this.selectRow,"jo_id",this.selectFindItemRow.jo_id);
                    this.$set(this.selectRow,"jo_sequence_id",this.selectFindItemRow.jo_sequence_id);
                    this.$set(this.selectRow, "con_qty", this.selectFindItemRow.qty);
                    this.$set(this.selectRow, "sec_qty", this.selectFindItemRow.sec_qty);
                    this.setCancelStatus();
                    //更改成新的貨品編號,再改回原來的貨品編號,這種情況要強制設置狀態為Edit
                    if(this.selectRow.row_status ===''){
                        this.$set(this.selectRow, "row_status", 'EDIT');                    
                    }
                }                
                this.showGoods = false;
            }else{
                this.$XModal.alert({ content: '請指定貨品編碼!', mask: false });
            }
        },

        //確認选中的批号
        changeLotNo() {
            if(this.seledtLotNoRow){
                if(this.selectRow.lot_no != this.seledtLotNoRow.lot_no){
                    this.$set(this.selectRow, "lot_no", this.seledtLotNoRow.lot_no);
                    let isQc = (this.seledtLotNoRow.is_qc)?'1':'0';
                    this.$set(this.selectRow, "qc_result", isQc);
                    //console.log(this.selectRow);
                }
                this.showLot = false;
            }else{
                this.$XModal.alert({ content: '請指定批號!', mask: false });
            }
        },     
        
        //点击编辑前的逻辑判断
        //activeMethod({ row, rowIndex, column, columnIndex }){ 
        activeMethod(){
            this.isEditCell = (this.headData.state ==="0")?true:false;
            return this.isEditCell;
        },

        //保存
        saveAllEvent: async function() {
            if(this.headData.id==="" && this.headData.out_dept===""){                
                this.$XModal.alert({ content: '移交部門不可為空!', mask: false });//mask: false 彈窗時父窗口不需庶罩
                return;
            }
            if(this.headData.transfer_date ===""){
                this.$XModal.alert({ content: '移交日期不可為空!', mask: false });
                return;
            }
            const $table = this.$refs.xTable1;
            if($table.tableData.length == 0){
                this.$XModal.alert({ content: '明細資料不可為空!', mask: false });
                return;
            }
            
            // 保存前對細資料1進行有效性檢查
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
                 
            //檢查當前用戶是否有負責部門的操作權限
            var userid = this.userId;
            var deptid = this.headData.out_dept;
            var rights = await comm.checkUserDeptRights(userid,deptid);
            if(rights !="1")
            {                
                this.$XModal.alert({ content: `當前用戶: 【${userid} 】,無負責部門【${deptid}】操作權限!`,status: 'info' , mask: false });
                return;
            }
           
            //保存前各项檢查,是否交QC,
            await this.checkPreUpdate();
            if(!this.preUpdateFlag){
                //各项檢查通不過
                return;
            }           

            this.headData.head_status = this.headEditFlag;//表頭新增或修改的標識
            var headData = JSON.parse(JSON.stringify(this.headData));
            var lstDetailData1 = this.tableData1;
            var lstDelData1 = this.delData1;            
            var user_id = this.userId;
            //console.log(lstDetailData1);
            axios.post("/ReturnRechange/Save",{headData,lstDetailData1,lstDelData1,user_id}).then(
                (response) => {
                    if(response.data=="OK"){
                        this.setStatusHead(false);
                        //重查刷新數據
                        this.searchID = this.headData.id;
                        this.findByID();
                        //this.unok_status = "1";//禁止顯示反批準按鈕  ************
                        this.$XModal.message({ content: '數據保存成功!', status: 'success' });
                    }else{
                        this.$XModal.alert({ content: '數據保存失敗!',status: 'error' , mask: false });
                    }
            }).catch(function (response) {
                alert(response);
            });
            this.headEditFlag = "";
            this.delData1 = [];
    },

    //檢查控制反批準按鈕的顯示/隱藏.
    checkRechangeApproveStatus:async function(){           
        await axios.post("/ProduceAssembly/CheckRechangeStatus",
        {within_code:'0000',handover_id:this.headData.handover_id, con_date:this.headData.con_date,id:this.headData.id,state:'1'}
              ).then((response) => {
                this.unok_status = response.data;                    
        })
    },

    //保存前檢查庫存是否夠扣除
    checkStock:async function(id){
        let rtn="";
        await axios.post("/ReturnRechange/CheckStock",{id}).then((response) => {                
                if(response.data.length>0){
                    //庫存檢查不通過
                    let seqNo = response.data[0].sequence_id;
                    let con_qty = response.data[0].con_qty;
                    let sec_qty = response.data[0].sec_qty;
                    let mo_id = response.data[0].mo_id;
                    let goods_id = response.data[0].goods_id;
                    let row_no=0;
                    this.validStockFlag = false;                       
                    for(let i=0;i<this.tableData1.length;i++){
                        if(this.tableData1[i].sequence_id === seqNo){
                            row_no = i+1;
                            let rw = this.tableData1[i];//如果按this.rows.data[i]取值有可能出錯,因些時有可能還未點擊表格而未能觸發cellClick事件.this.rows是空的對象從而引起錯誤
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
    
    //--start保存前檢查
    checkPreUpdate:async function() {
            let	ls_id='',ls_goods_id='',ls_aim_jo_id,ls_dept_id,ls_bill_type_no,ls_error;
            let	ls_sequence_id='',ls_out_dept,ls_in_dept='',ls_mo_id='',ls_servername='hkserver.cferp.dbo';
            let	ldt_con_date,ldt_check_date;
            let	ldec_con_qty=0,ldec_sec_qty=0;
            let	ll_cnt,ll_cnt_state1,ll_cnt_state3,ll_cnt_state4,li_row_no;
                    
            ls_out_dept = this.headData.out_dept;
            ls_in_dept = this.headData.in_dept;
            ls_id = this.headData.id;
            this.preUpdateFlag = true;                  
            if(this.$utils.getSize(ls_in_dept) === 0){
                ls_in_dept = '';
            }    
            if(ls_in_dept===""){
                await this.$XModal.alert({ content: `接收部門不可為空!`,status: 'warning' , mask: false });
                this.preUpdateFlag = false;
                return;
            }
            if(ls_id===""){                
                await this.$XModal.alert({ content: `編號不可為空!`,status: 'warning' , mask: false });
                this.preUpdateFlag = false;
                return;
            }            
            ls_bill_type_no = 'R';
            ldt_check_date = this.headData.check_date;
            
            //檢查退貨原因不可為空/退貨流程是否存在
            let ls_cnt='0'
            for(let li_row=0;li_row<this.tableData1.length;li_row++) {
                li_row_no = this.$utils.toInteger(li_row)+1;
                ls_return_reason = this.tableData1[li_row].return_reason;
                if(this.$utils.getSize(ls_return_reason)<5){
                    await this.$XModal.alert({ content: `第 ${li_row_no} 行,请填写退货原因,并且不能小于5个字符`,status: 'warning' , mask: false });
                    this.preUpdateFlag = false;
                    break;
                }
                ls_mo_id = this.tableData1[li_row].mo_id;
                ls_goods_id = this.tableData1[li_row].goods_id;
                //注意調換部門
                await axios.post("/ReturnRechange/CheckPlanFlow",{mo_id:ls_mo_id,goods_id:ls_goods_id,out_dept:ls_in_dept,in_dept:ls_out_dept}).then(
                    (response) => {
                       ls_cnt = response.data;
                });
                if(ls_cnt ==='0'){
                    await this.$XModal.alert({ content: `第 ${li_row_no} 行,頁數【${ls_mo_id}--${ls_goods_id}】\n對應流程不存在!`,status: 'warning' , mask: false });
                    this.preUpdateFlag = false;
                    break;
                }
            }
        },//--end 保存前檢查

        //行的狀態為編輯狀態時黃色背景顯示
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
        selectRow:{//--start selectRow:
            handler (val, oldVal) {
                if(this.headData.state ==='0') {
                    if(this.selectRow===null){
                        //2022/08/08
                        return;
                    }                    
                    let edit_flag = false;
                    for (let i in this.selectRow) {
                        if (this.selectRow[i] != this.tempSelectRow[i]) {                            
                            edit_flag = true; 
                            //貨品編號有更改 //2022/11/14 避免貨品名稱與貨品編號不對應的問題
                            if(this.selectRow.goods_id != this.tempSelectRow.goods_id) {
                                //記錄更改的貨品編號
                                this.temp_goods_id = this.selectRow.goods_id;
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
                } //-- end if(this.headData.state ==='0')
            }, //-- end handler
            deep: true
        }, // --end selectRow:        
        
        searchID : function (val) {
            this.searchID = val.toUpperCase();
        },        
    },//-- end of watch
    
    computed: {
        //
    },
    mounted() {
        this.tableHeight = $(parent.window).height()-(172+55);        
        let that = this; 
        window.onresize = () => {
            return (() => {
                that.tableHeight = $(parent.window).height()-(172+55);               
            })()
        };
    }

} //-- end fo main

var app = new Vue(main).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;
Vue.prototype.$utils = XEUtils;

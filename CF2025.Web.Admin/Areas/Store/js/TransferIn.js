var MainIn = {
    data() {
        return {            
            searchID:"", 
            headStatus:"",//主檔為新增或編輯
            curRowIndex:null,
            selectRow: null,
            curDelRow:null,
            headerCellStyle:{background:'#F5F7FA',color:'#606266',height:'25px',padding:'2px'},
            loading: false,
            showEdit: false,
            submitLoading: false,
            showEditStore:false,
            showFind:false,            
            server_date:"", 
            stateFontColor:"color:black",
            btnItemTitle:"劉覽",
            headData: { id: "", transfer_date: "", department_id: "", handler: "", remark: "", create_by: "", update_by: "", check_by: "", update_count: "", create_date: "", update_date: "", check_date: "", state: "0",head_status:"" },
            rowDataEdit: {
                id: '', mo_id: '', goods_id: '', goods_name: '', unit: '', location_id: '', shelf: '', transfer_amount: 0, sec_unit: '', sec_qty: 0.00, nwt: 0.00, gross_wt: 0.00, package_num: 0,
                position_first: '', transfer_out_id: '', lot_no: '', remark: '', move_location_id: '', do_color: '', sequence_id: '', transfer_out_sequence_id:'',row_status:''
            },
            formRules: {
                id: [{ required: true},{ min: 13, message: '請度13个字符' }],
                transfer_date:[{ required: true, message: '請輸入日期' }],
                department_id:[{ required: true, message: '請選擇部門' }]           
            },
            rulesRowEdit: {
                mo_id:[
                    { required: true, message: '請輸入頁數'},
                    {min:9,message: '頁數長度至少是9个英文字符'}
                ],
                goods_id: [{ required: true, message: '請輸入貨品編號' }],
                unit: [{ required: true, message: '請輸入單位' }],
                transfer_amount: [{ required: true, message: '請輸入轉倉數量'}],
                sec_unit:[{ required: true, message: '請輸入重量單位' }],
                sec_qty: [{ required: true, message: '請輸入重量'}],
                location_id: [{ required: true, message: '請選擇轉入的倉庫' }]
            },
            deptList: [],
            stateList: [],
            qtyUnitList: [],
            wtUnitList: [],
            locationList: [],
            cartonCodeList: [{ label: '', value: '' }],
            gridData: [],
            formDataFind:{},
            isEditHead: false,//主檔編輯狀態
            isEditDetail: false,//明細編輯狀態
            isReadOnlyHead: true,//主檔對象只讀狀態
            isReadOnlyDetail: true,//明細對象只讀狀態
            isDisableDropBoxHead: true,//主檔下拉列表框失效狀態
            isDisableDropBoxDetail: true,//明細下拉列表框失效狀態       
            flagChild:"",//是否是從轉出待確認列表中帶出轉入單頁面的標志
            userId:"",
            confirmData:[],
            locationId:'',
            shelf:'',
            tempHeadData: {},
            tempGridData: {},                 
            tableHeight :450,
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
            //windowWidth: document.documentElement.clientWidth, //实时屏幕宽度
            //windowHeight: document.documentElement.clientHeight, //实时屏幕高度
        }
    },
    created() {       
        this.getComboxList("DeptList");//部門編碼   
        this.getComboxList("StateList");//狀態 
        this.getComboxList("LocationList");//倉庫,貨架,由選擇的倉庫帶出       
        this.getComboxList("QtyUnitList");//數量單位
        this.getComboxList("WegUnitList");//重量單位
        this.flagChild = $("#isChild").val();
        this.userId = $("#user_id").val();
        this.tempHeadData = this.headData;
        this.tempGridData = this.gridData;
        this.initData();
        //this.tableHeight=($(window).height()-200);
        //this.getDateServer();//服器端日期("yyyy-MM-dd"),批準,反批準日期批較使用    
        //$(window).resize(function() {
        //    alert('height:'+$(window).innerHeight() );
        //    alert($(window).width());
        //})
    },
    methods: { 
        //初始化數據
        initData(){
            //判斷是否是從轉出待確認列表中帶出轉入單頁面
            if(this.flagChild ==="1"){
                 this.confirmData = parent.app.selectData; //confirmData 此時為對象            
                 this.locationId = parent.app.formData.location_id;
                 this.shelf = parent.app.formData.shelf;
                 if(this.confirmData.length>0){
                     //生成表頭
                     this.insertEvent();
                     //this.$refs.xTableIn;此代碼在頁面創建階段created()內引用會引起錯誤,因表格元素尚未渲染完成,插入明細的操作須放到mounded()方法內實現
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
                this.loading = false;
            }, 500);            
        },
        showFindWindos(){
            comm.openWindos('TransferIn');
        },
        printEvent(){
            //test
            //var parentWindowHeight=$(parent.window).height();
            //this.tableHeight=$(parent.window).height()-205;

        },       
        //初始化下拉列表框
        getComboxList(SourceType) {
            axios.get("/Base/DataComboxList/GetComboxList?SourceType=" + SourceType).then(
                (response) => {
                    switch (SourceType) {
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
            this.headStatus = "NEW";
            //清空數據
            this.clearHeadData();
            //this.$refs["formHead"].resetFields();//清空所有對象值
            this.gridData = [];//清空明細表格數據            
            //新增后初始化相關對象的初始值            
            var d = new Date();//生成日期對象:Fri Oct 15 2021 17:51:20 GMT+0800
            var date = comm.getDate(d, 0);//轉成年月日字符串格式            
            var dateTime = comm.datetimeFormat(d, "yyyy-MM-dd hh:mm:ss");
            this.$set(this.headData, "transfer_date", date);
            this.$set(this.headData, "create_by", this.userId);
            this.$set(this.headData, "create_date", dateTime);           
            //this.setToolBarStatus(true);
            //this.isDisableArea = false;
            //this.canAddItem = false;
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
            if ((this.headData.id != "") && (this.gridData.length > 0)) {
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
        //還原
        revertEvent() {
            this.setStatusHead(false);
            this.headData = JSON.parse(JSON.stringify(this.tempHeadData));//還原主表
            this.gridData = JSON.parse(JSON.stringify(this.tempGridData));//還原明細 
            this.headStatus = "";
            //this.canAddItem = true;
            //const $table = xTable.value
            //const type = await VXETable.modal.confirm('您确定要还原数据吗?')
            //if (type === 'confirm') {
            //    $table.revertData()
            //}
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
            this.headData.transfer_date = "", 
            this.headData.department_id = "",
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
        },
        clearRowDataEdit(){
            this.rowDataEdit= {
                id: '', mo_id: '', goods_id: '', goods_name: '', unit: 'PCS', location_id: '',carton_code:'', shelf: '', transfer_amount: 0, 
                sec_unit: 'KG', sec_qty: 0.00, nwt: 0.00, gross_wt: 0.00, package_num: '0', position_first: '', transfer_out_id: '', lot_no: '',
                remark: '', move_location_id: '',move_carton_code:'', do_color: '', sequence_id: '', transfer_out_sequence_id:'',row_status:''
            }
        },
        setRowDataEdit(curRow){
            if(curRow.location_id){
                this.getCartonCodeList(curRow.location_id);
            }
            this.rowDataEdit= {
                id: curRow.id, mo_id: curRow.mo_id, goods_id: curRow.goods_id, goods_name: curRow.goods_name, unit: curRow.unit, 
                location_id: curRow.location_id,carton_code:curRow.location_id, shelf: curRow.shelf, transfer_amount: curRow.transfer_amount, 
                sec_unit: curRow.sec_unit, sec_qty: curRow.sec_qty, nwt: curRow.nwt, gross_wt: curRow.gross_wt, package_num: curRow.package_num,
                position_first: curRow.position_first, transfer_out_id: curRow.transfer_out_id, lot_no: curRow.lot_no, remark: curRow.remark,
                move_location_id: curRow.move_location_id,move_carton_code: curRow.move_location_id,do_color: curRow.do_color, sequence_id: curRow.sequence_id,
                transfer_out_sequence_id:curRow.transfer_out_sequence_id,row_status:curRow.row_status
            }
            
        },
        backupData () {
            this.tempHeadData = JSON.parse(JSON.stringify(this.headData));//暫存主檔臨時數據
            this.tempGridData = JSON.parse(JSON.stringify(this.gridData));//暫存明細臨時數據
        },
        //新增明細
        insertRowEvent() {
            if(this.headStatus ==""){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            if (this.headData.id == "") {
                //this.$XModal.message({ content: '主檔編號不可為空,當前操作無效!', status: 'warning' });
                this.$XModal.alert({ content: '主檔編號不可為空,當前操作無效!', mask: false });
                return;
            };
            //if (this.isEditHead) {
            //    this.$XModal.alert({ content: '請選首先保存主檔資料,當前操作無效!', mask: false });
            //    return;
            //}
            //this.clearRowData();
            //this.$set(this.rowDataEdit, "id", this.headData.id);
            //this.selectRow = null;
            //this.showEdit = true;
            //-------------
            var rowEndIndex = -1;//添加至行尾
            var $table = this.$refs.xTableIn;
            this.clearRowDataEdit();
            this.selectRow = null;
            this.$set(this.rowDataEdit, "id", this.headData.id);  
            this.$set(this.rowDataEdit, "sequence_id", this.getSequenceId($table.tableData.length));
            this.$set(this.rowDataEdit, "row_status", 'NEW');

            $table.insertAt(this.rowDataEdit,rowEndIndex).then(({ row }) => {
                $table.setActiveCell(rowEndIndex, 'mo_id');
            });

            //顯示彈窗
            this.cartonCodeList=null;
            var index = $table.tableData.length-1;//新增的行號索引
            this.curRowIndex = index; //記錄新增的行號索引
            var rw = $table.tableData[index];  //取新增行對象值
            this.setRowDataEdit(rw);            
            this.selectRow = rw; //將當前行賦值給彈窗
            this.showEdit = true;
            
        },
        saveEvent(){           
            if(this.headData.id==="" && this.headData.department_id===""){
                //mask: false 彈窗時父窗口不需庶置
                this.$XModal.alert({ content: '部門不可以為空!', mask: false });
                return;
            }
            if(this.headData.transfer_date ===""){
                this.$XModal.alert({ content: '轉入日期不可以為空!', mask: false });
                return;
            }            
            const $table = this.$refs.xTableIn;
            if($table.tableData.length == 0){
                this.$XModal.alert({ content: '明細資料不可以為空!', mask: false });
                return;
            }
            this.saveAll();
        }, 
        editRowEvent(row){
            //this.rowDataEdit = row; //不可以直接賦row給this.rowDataEdit

            this.setRowDataEdit(row);
            this.selectRow = row;
            this.showEdit = true;
        },
        //暫時將當前行的修改更新至表格
        tempUpdateRowEvent(){           
            if(this.headSatus ===""){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            Object.assign(this.selectRow, this.rowDataEdit);
            //this.$set($table.tableData[1], this.rowDataEdit);
            this.showEdit = false;
        },
        //撤消對當前行的修改,不影響到表格
        tempUndoRowEvent(){
            this.clearRowDataEdit();
            this.showEdit = false;
        },
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
        tempDelRowEvent(){
            if(this.headStatus===""){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            if(this.curDelRow){           
                this.$XModal.confirm('是否確定要刪除當前行？').then(type => {
                    if(type ==='confirm'){                        
                        this.$refs.xTableIn.remove(this.curDelRow);                        
                        this.curDelRow = null;
                    }
                })
            }else{
                this.$XModal.alert({ content: '請首先指定需要刪除的項目!',mask: false });
            }
        },
        getMaxID(bill_id, dept_id) {           
            //請求后臺Action并传多個參數,多個參數傳值方法:第一參數用url..."?id="++value,后面的參數用+"&Ver="+value
            var $table = this.$refs.xTableIn;            
            axios.get("/Base/Common/GetMaxID?bill_id=" + bill_id + "&dept_id=" + dept_id + "&serial_len=3").then(
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
        approveEvent:async function (val) {
            if ((this.headData.id != "") && (this.gridData.length > 0)) {
                //批準,反批準都要檢查是否是註銷狀態
                if(this.headData.state === "2"){
                    this.$XModal.alert({ content: '注銷狀態,當前操作無效!', mask: false });
                    return;
                }
                var ls_success= (val==='1')?"批準成功!":"反批準成功!";
                var ls_error= (val==='1')?"批準失敗!":"反準失敗!";
                var ls_type = (val==='1')?"批準":"反批準";
                var ls_is_approve = `確定是否要進行【${ls_type}】操作？`;
                //取后端單據狀態
                let status = comm.checkApproveState('st_transfer_mostly',this.headData.id);
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
                }
                if(val==='0'){ 
                    //進行反批准
                    //進行當前反批準操作前再次檢查后端是否已被別的用戶反批準.
                    if(status==="0"){
                        //后臺數據已為批準狀態,已被別的用戶批準
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
                    var isApprove = await comm.canApprove(this.headData.id,"w_transfer_in");//转入單
                    if(isApprove ==="0"){
                        this.$XModal.alert({ content: "注意:【批準日期】必須為當前日期,方可進行此操作!", mask: false });
                        return;
                    }
                }
                this.$XModal.confirm(ls_is_approve).then(type => {
                    if (type == "confirm") {
                        this.headData.check_by = this.userId;
                        var head = JSON.parse(JSON.stringify(this.headData));
                        var approve_type = val;
                        axios.post("/TransferIn/Approve",{head,approve_type}).then(
                            (response) => {
                                if(response.data[0].approve_status ==="OK"){
                                    this.setStatusHead(false);
                                    //重查刷新數據
                                    this.getHead(this.headData.id);                                   
                                    this.$XModal.message({ content: ls_success, status: 'success' });
                                }else{
                                    if(response.data[0].action_type ==="STOCK"){
                                        //庫存不足
                                        this.checkStockData = response.data;
                                        ls_error += "【" + response.data[0].error_info + "】";
                                        this.showEditStore = true;
                                    }
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
        //彈窗中確認
        submitEvent() {
            if(this.headStatus===""){
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            if(this.rowDataEdit.row_status != 'NEW'){
                this.rowDataEdit.row_status='EDIT';//編輯標記
            }
            Object.assign(this.selectRow, this.rowDataEdit);
            this.showEdit = false;
        },
        dbclickEvent(row){
            //console.log(row.data[row.$rowIndex]);//取得行對象
            //this.rowDataEdit = row.data[row.$rowIndex]; //此方式是對像,彈窗更改,表格也跟著改
            //this.rowDataEdit = JSON.parse(JSON.stringify(row.data[row.$rowIndex]));
            var rw = row.data[row.$rowIndex];
            this.setRowDataEdit(rw);            
            this.selectRow = rw;
            this.showEdit = true;
        },
        cellClickEvent(row){
            //row.$rowIndex;//行索引
            this.curDelRow = row.data[row.$rowIndex];            
        },
        getHead(id) {
            //此處的URL需指定到祥細控制器及之下的動Action,否則在轉出待確認彈窗中的查詢將數出錯,請求相對路徑的問題
            axios.get("/TransferIn/GetHeadByID", { params: { id: id }}).then(
                (response) => {                
                    this.headData = {
                        id: response.data.id,
                        transfer_date: response.data.transfer_date,
                        department_id: response.data.department_id,
                        handler: response.data.handler,
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
        getDetails(id) {
            var _self = this;            
            axios.get("/TransferIn/GetDetailsByID", { params: { id: id }  }).then(
                (response) => {
                    this.gridData = response.data;
                },
                (response) => {
                    alert(response.status);
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
              const $table = this.$refs.xTableIn;
              const errMap = $table.fullValidate().catch(errMap => errMap);
              await errMap;//同步操作,等待校驗結果
              errMap.then(res=>{
                  this.tempSaveData=[]; 
                  if(res) // res为promise對象中的值,如果值是對象,說明數據校驗通不過
                  {
                      this.$XModal.alert({ content: '注意:明細數據不完整,請返回檢查!', mask: false });
                      return;
                  }else{                      
                      var items_update=null;
                      for(var i=0;i<$table.tableData.length;i++){
                          items_update = $table.tableData[i];                
                          if(items_update.row_status==='EDIT'){
                              this.tempSaveData.push(items_update);
                          }
                      }            
                      const items= { insertRecords, removeRecords} = $table.getRecordset();
                      if(items.insertRecords.length>0){
                          for(var i=0;i<items.insertRecords.length;i++){
                              this.$set(items.insertRecords[i], "row_status", "NEW" );
                              this.tempSaveData.push(items.insertRecords[i]);
                          }
                      }
                      if(items.removeRecords.length>0){
                          for(var i=0;i<items.removeRecords.length;i++){
                              this.$set(items.removeRecords[i], "row_status", "DEL" );
                              this.tempSaveData.push(items.removeRecords[i]);
                          }
                      }
                      this.headData.head_status = this.headStatus;//表頭是新增或者修改
                      var head = JSON.parse(JSON.stringify(this.headData));
                      var detailData = this.tempSaveData; 
                      axios.post("/TransferIn/Save",{head,detailData }).then(
                          (response) => {
                              if(response.data=="OK"){                                  
                                  this.setStatusHead(false);
                                  //重查刷新數據
                                  this.getHead(this.headData.id);
                                  this.getDetails(this.headData.id);
                                  //this.$XModal.alert({ content: '數據保存成功!',status: 'success',mask: false });                                  
                                  this.$XModal.message({ content: '數據保存成功!', status: 'success' });
                              }else{
                                  this.$XModal.alert({ content: '數據保存失敗!',status: 'error' , mask: false });                                 
                              }                              
                          }
                      ).catch(function (response) {    
                          alert(response);
                      });
                      this.headStatus = "";
                  }
              })
        },
        //服務器端日期
        getDateServer: async function() {
            var res= await axios.get("/Base/DataComboxList/GetDateServer");
            this.server_date = res.data;
        }        
    },

    watch: {
        rowDataEdit: {
            handler (val, oldVal) {
                if(this.rowDataEdit.mo_id){
                    this.rowDataEdit.mo_id = this.rowDataEdit.mo_id.toUpperCase();
                }
                if(this.rowDataEdit.goods_id){
                    this.rowDataEdit.goods_id = this.rowDataEdit.goods_id.toUpperCase();
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
        if(this.flagChild ==="1"){  
            this.tableHeight=$(window).height()-150;
            var $table = this.$refs.xTableIn;
            //-1 為添加至行尾            
            for (var i = 0; i < this.confirmData.length; i++) {
                this.clearRowDataEdit();      
                this.setRowDataEdit(this.confirmData[i]);
                this.rowDataEdit.location_id = this.locationId;
                this.rowDataEdit.carton_code = this.locationId;
                this.rowDataEdit.shelf = this.shelf;
                this.rowDataEdit.do_color = this.confirmData[i].color;
                this.rowDataEdit.transfer_out_id = this.confirmData[i].id;
                this.rowDataEdit.transfer_out_sequence_id = this.confirmData[i].sequence_id;
                this.rowDataEdit.id = '';
                this.rowDataEdit.sequence_id = '';               
                $table.insertAt(this.rowDataEdit,-1).then(({ row }) => {
                    $table.setActiveCell(-1, 'mo_id');
                })
            }
        }else{
            this.tableHeight=$(parent.window).height()-205;
        }
        
        let that = this;       
        window.onresize = () => {
            return (() => {
                that.tableHeight=$(parent.window).height()-205;                 
            })()
        };
    }
}

var app = new Vue(MainIn).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;

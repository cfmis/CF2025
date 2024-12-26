/**
 * 轉出單
 */
var main = {
    data() {
        return {
            searchID:"", 
            headStatus:"",//主檔為新增或編輯
            curRowIndex:null,
            curRowIndex2:null,
            selectRow: null,
            selectRow2: null,
            tempSelectRow:{},
            tempSelectRow2:{},
            curDelRow: null,
            componentKey: 0,
            tableHeight1: 500,
            tableHeight2: 100,
            headerCellStyle:{background:'#F5F7FA',color:'#606266',height:'25px',padding:'2px'},
            loading: false,
            showEdit: false,
            submitLoading: false,
            showEditStore: false,
            validStockFlag: false,//檢查庫存是否夠扣標識
            isEditHead: false,//主檔編輯狀態             
            btnItemTitle:"劉覽",
            isReadOnlyHead: true,//主檔對象只讀狀態
            isReadOnlyDetail: true,//明細對象只讀狀態
            isDisableDropBoxHead: true,//主檔下拉列表框失效狀態
            isDisableDropBoxDetail: true,//明細下拉列表框失效狀態              
            tempHeadData: {},
            tempTableData1: {},
            tempTableData2: {},
            tableData1: [],
            tableData2: [],
            originData2:[],
            delData1:[],
            delData2:[],
            curRowSeqId:"",
            stateFontColor:"color:black",
            unok_status:"1",//檢查移交單是否已批準,如果已批準則返回"1",反批準按鈕不顯示, 當前默認1是不顯示
            ok_status:"0",//控制批準按鈕的顯示, 當前默認0是顯示  
            headData: {
                id: "", transfer_date: "", bill_type_no: "", group_no: "", department_id: "", handler: "", remark: "",
                update_by: "", state: "0", create_by: "", create_date: "", update_date: "", update_count: "1",remark:"",
                check_by: "", check_date: "", head_status: ""
            },           
            rowDataEdit: {
                mo_id: '', shipment_suit: true, goods_id: '', goods_name: '', unit: '', transfer_amount: 0, sec_unit: '', sec_qty: 0, package_num: 0,
                position_first: '', nwt: 0, gross_wt: 0, location_id: '', carton_code: '', inventory_qty: 0, inventory_sec_qty: 0, lot_no: '', remark: '',
                do_color:'', move_location_id:'', move_carton_code:'',tran_id:'', id:'', sequence_id:'', row_status: ''
            }, 
            rowDataEdit2:{
                mo_id:'',goods_id:'',goods_name:'',inventory_qty:0,inventory_sec_qty:0.00,con_qty:0,sec_qty:0.00,sec_unit:'KG',
                location:'',carton_code:'',order_qty:0,transfer_amount:0,nostorage_qty:0,goods_unit:'SET',remark:'',mrp_id:'',unit_code:'PCS',
                lot_no:'',obligate_qty:0, id:'',upper_sequence:'',sequence_id:'',row_status:'',qty_rate:0.0000000,bom_qty:0
            },           
            Rows:[],
            deptList: [],
            stateList: [],
            billTypeList: [],
            groupNoList: [],
            qtyUnitList: [],
            locationList: [],
            wtUnitList: [],
            goodsLotNoList:[],           
            //server_date:"", 
            stateFontColor: "color:black",
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
            rulesRowEdit: {
                mo_id: [{ required: true, message: '請輸入頁數'},{ min:9,message: '頁數長度至少是9个英文字符'}],
                goods_id: [{ required: true, message: '請輸入貨品編號' }],
                unit: [{ required: true, message: '請輸入單位' }],
                transfer_amount: [{ required: true, message: '請輸入轉倉數量'}],
                sec_unit:[{ required: true, message: '請輸入重量單位' }],
                sec_qty: [{ required: true, message: '請輸入重量'}],
                location_id: [{ required: true, message: '請選擇轉入的倉庫' }]
            },
            //查找貨品編號
            tableColumn: [
                   { field: 'mo_id', title: '頁數', width: '100' },
                   { field: 'goods_id', title: '貨品編號',width: '175' ,showOverflow: true},
                   { field: 'goods_name', title: '貨品名稱',width: '180',showOverflow: true},
                   { field: 'lot_no', title: '批號',width: '80',showOverflow: true }, 
                   { field: 'qty', title: '數量',width: '70' },
                   { field: 'sec_qty', title: '重量',width: '60' },                  
                   { field: 'location_id', title: '倉庫',width: '60' },
                   { field: 'do_color', title: '顏色',width: '20',showOverflow: true}
            ],           
            tableLotNoList: [{mo_id:'',goods_id:'',goods_name:'',lot_no:'',qty:0,sec_qty:0,location_id:''}],//2024/01/22 orign code: tableLotNoList: []
            //pagerConfig: {total: 0,currentPage: 1,pageSize: 10},
            //頁面第一個表格上部分的空間(172)+表格二尾部剩余空間(35),例如35,設置越小,表示表格二尾部距離瀏覽器底部空間的距離越小
            constSpace: 172+35,
            constP1: 0.0001,
            constP2: 0.0001,  
           
        } //--end of return
    },//--end of data
    created() {
        this.getComboxList("DeptList");//部門編碼   
        this.getComboxList("MoGroupList");//营业员组别  
        this.getComboxList("StateList");//狀態
        this.getComboxList("BillTypeOut");//單據類型        
        this.getComboxList("LocationList");//倉庫,貨架,由選擇的倉庫帶出       
        this.getComboxList("QtyUnitList");//數量單位
        this.getComboxList("WegUnitList");//重量單位
        this.userId = $("#user_id").val();
        this.tempHeadData = this.headData;
        this.tempTableData1 = this.tableData1;
        this.tempTableData2 = this.originData2;

        
        //記錄表格一，表格二的高度比
        let tbH1 = this.tableHeight1;
        this.tableHeight2 = $(parent.window).height()-(this.constSpace+tbH1);
        let tbH2 = this.tableHeight2;
        let windowH2 = $(parent.window).height()-this.constSpace;
        this.constP1 = tbH1/windowH2;
        this.constP2 = tbH2/windowH2;       
    },
    methods: {
        //服務器端日期
        //getDateServer: async function() {
        //    var res= await axios.get("/Base/DataComboxList/GetDateServer");
        //    this.server_date = res.data;
        //}
        findByID:async function(){
            if (this.headStatus != "" || this.searchID === "") {
                return;
            }
            this.loading = true;
            //同步優先執行,因setTimeout()中的異步執行要依賴同步執行的結果
            await this.getHead(this.searchID);
            await this.getDetails(this.searchID);
            this.curDelRow = null;            
            setTimeout(() => {               
                this.getDetailsPart(this.searchID);
                this.loading = false;
            }, 500);
        },
        showFindWindos() {
            comm.openWindos('TransferOut');
        },
        printEvent() {
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
                        case "MoGroupList":
                            this.groupNoList = response.data;
                            break;
                        case "BillTypeOut":
                            this.billTypeList = response.data;
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
        insertEvent:async function() {
            //--權限檢查,20067為模塊對應菜單的代碼,例子為轉出單            
            let power = await comm.checkPermission(this.userId,"20067","pubAdd");
            if (power === "0") {
                return;
            }  
            //--
            this.setStatusHead(true);
            this.backupData();//編輯前首先暫存主檔/明細臨時數據
            this.headStatus ="NEW";
            //清空數據
            this.clearHeadData();
            //this.$refs["formHead"].resetFields();//清空所有對象值
            this.tableData1 = [];//清空明細表格數據   
            this.tableData2 = [];
            this.originData2 = [];
            //新增后初始化相關對象的初始值            
            let d = new Date();//生成日期對象:Fri Oct 15 2021 17:51:20 GMT+0800
            let date = comm.getDate(d, 0);//轉成年月日字符串格式            
            let dateTime = comm.datetimeFormat(d, "yyyy-MM-dd hh:mm:ss");
            this.$set(this.headData, "transfer_date", date);
            this.$set(this.headData, "create_by", this.userId);
            this.$set(this.headData, "create_date", dateTime);   
            this.$set(this.headData,"department_id",'601')
            //this.setToolBarStatus(true);
            //this.isDisableArea = false;
            //this.canAddItem = false;
        },
        dbclickEvent(row) {            
            //var rw = row.data[row.$rowIndex];
            //this.setRowDataEdit(rw);
            //this.selectRow = rw;
            //this.showEdit = true;
        },
        cellClickEvent(row) {
            this.Rows = row;//表格的數據對象
            this.curRowIndex = row.$rowIndex;//當前行的索引
            this.selectRow = row.data[row.$rowIndex];//當前行
            this.curDelRow = row.data[row.$rowIndex];//存放可能冊除的當前行
            if(this.headData.state==='0'){
                this.tempSelectRow = JSON.parse(JSON.stringify(this.selectRow));//暫存當前行,以便檢查是否有更改
            }           
            this.curRowSeqId = this.selectRow.sequence_id;
            this.setTableData2ByCurrentMo();
        },
        cellClickEvent2(row){
            this.curRowIndex2 = row.$rowIndex;//當前行的索引
            this.selectRow2 = row.data[row.$rowIndex];//當前行                
            if(this.headData.state==='0'){
                this.tempSelectRow2 = JSON.parse(JSON.stringify(this.selectRow2));//暫存當前行,以便檢查是否有更改
            }
        },
        checkStockData() {

        },
        backupData () {
            this.tempHeadData = JSON.parse(JSON.stringify(this.headData));//暫存主檔臨時數據
            this.tempTableData1 = JSON.parse(JSON.stringify(this.tableData1));//暫存明細臨時數據
            this.tempTableData2 = JSON.parse(JSON.stringify(this.originData2));
        },
        setStatusHead(blValue) {
            this.isEditHead = blValue; //設置編輯狀態標識            
            this.isReadOnlyHead = !(blValue); //設置對象可編輯狀態
            this.isDisableDropBoxHead = !(blValue); //設置下拉列表框可編輯狀態           
            if (blValue) {
                this.btnItemTitle = "修改";
            } else {
                this.btnItemTitle = "劉覽";
            }
        },
        clearHeadData() {
            this.headData.id = "",
            this.headData.transfer_date = "",
            this.headData.bill_type_no = "",
            this.headData.group_no = "",
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
            this.headData.state = "0"
        },
        getMaxID(billId, billTypeNo,groupNo) {            
            if(billTypeNo==="" || groupNo===""){
                return;
            }            
            //請求后臺Action并传多個參數,多個參數傳值方法:第一參數用url..."?id="++value,后面的參數用+"&Ver="+value
            let $table = this.$refs.xTable1;
            axios.get("/TransferOut/GetMaxID?billId=" + billId + "&billType=" + billTypeNo + "&groupNo=" + groupNo + "&serialLen=2").then(
                (response) => {
                    this.headData.id = response.data;
                    //直接更改表格中某單元格的值,不可以直接更改綁定數組的方式,否則將取不到數據的更改狀態
                    for (let i = 0; i < $table.tableData.length; i++) {
                        this.$set($table.tableData[i], "id", this.headData.id);
                        this.$set($table.tableData[i], "sequence_id", this.getSequenceId(i));
                    }
                }
            );
        },
        getSequenceId(i) {
            return comm.pad(i + 1, 4) + "h";
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
            //if (this.isEditHead) {
            //    this.$XModal.alert({ content: '請選首先保存主檔資料,當前操作無效!', mask: false });
            //    return;
            //}
            //this.clearRowData();
            //this.$set(this.rowDataEdit, "id", this.headData.id);
            //this.selectRow = null;
            //this.showEdit = true;
            //-------------
           
            let locationId = "801";
            let rowEndIndex = -1;//添加至行尾
            let $table = this.$refs.xTable1;
            let seqId = this.getSequenceId($table.tableData.length);
            this.clearRowDataEdit();
            this.selectRow = null;
            this.$set(this.rowDataEdit, "id", this.headData.id);  
            this.$set(this.rowDataEdit, "sequence_id", seqId); 
            this.$set(this.rowDataEdit, "shipment_suit", 1);//是否套件
            this.$set(this.rowDataEdit, "package_num", 0);
            this.$set(this.rowDataEdit, "transfer_amount", 0);
            this.$set(this.rowDataEdit, "sec_qty", 0);
            this.$set(this.rowDataEdit, "nwt", 0);
            this.$set(this.rowDataEdit, "gross_wt", 0);
            this.$set(this.rowDataEdit, "unit", "PCS");
            this.$set(this.rowDataEdit, "sec_unit", "KG");
            this.$set(this.rowDataEdit, "row_status", 'NEW');            
            let billTypeNo = this.headData.bill_type_no;
            let shipmentSuit = this.rowDataEdit.shipment_suit;
            this.setLocation(billTypeNo,shipmentSuit);//設置臨時流動倉位
            //2024-10-22 cancel
            $table.insertAt(this.rowDataEdit,rowEndIndex).then(({ row }) => {
                $table.setActiveCell(rowEndIndex, 'mo_id');
            });  
            //this.tableData1.push(this.rowDataEdit);// 2024-10-22 add
            //debugger;
            //顯示彈窗
            this.cartonCodeList = null;
            let index = $table.tableData.length-1;//新增的行號索引
            //let index = $table.$data.length-1;//新增的行號索引
            this.curRowIndex = index; //記錄新增的行號索引
            let rw = $table.tableData[index];  //取新增行對象值
            this.setRowDataEdit(rw);
            this.selectRow = rw; //將當前行賦值給彈窗
            this.curRowSeqId = seqId;
            this.showEdit = true; 
        },
        setLocation(billTypeNo,shipmentSuit) {           
            //VAT:JN--JX,VN--T單,DN:回港,SN:辦單,LN:東莞D
            if(shipmentSuit===1){
                //套件
                switch(billTypeNo) {
                    case 'DN':
                    case 'SN':
                        this.$set(this.rowDataEdit, "location_id", '801');
                        this.$set(this.rowDataEdit, "carton_code", '801');
                        this.$set(this.rowDataEdit, "move_location_id", '801');
                        this.$set(this.rowDataEdit, "move_carton_code", '801');
                        break;
                    case 'JN':
                        this.$set(this.rowDataEdit, "location_id", 'JX0');
                        this.$set(this.rowDataEdit, "carton_code", 'JX0');
                        this.$set(this.rowDataEdit, "move_location_id", 'JX0');
                        this.$set(this.rowDataEdit, "move_carton_code", 'JX0');
                        break;
                    case 'LN':
                        this.$set(this.rowDataEdit, "location_id", 'D00');
                        this.$set(this.rowDataEdit, "carton_code", 'D00');
                        this.$set(this.rowDataEdit, "move_location_id", 'D00');
                        this.$set(this.rowDataEdit, "move_carton_code", 'D00');
                        break;
                    case 'VN':
                        this.$set(this.rowDataEdit, "location_id", 'T00');
                        this.$set(this.rowDataEdit, "carton_code", 'T00');
                        this.$set(this.rowDataEdit, "move_location_id", 'T00');
                        this.$set(this.rowDataEdit, "move_carton_code", 'T00');
                        break;
                    default:
                        this.$set(this.rowDataEdit, "location_id", '');
                        this.$set(this.rowDataEdit, "carton_code", '');
                        this.$set(this.rowDataEdit, "move_location_id", '');
                        this.$set(this.rowDataEdit, "move_carton_code", '');
                }
            }else{
                //非套件                
                switch(billTypeNo) {
                    case 'DN':
                    case 'SN':
                        this.$set(this.rowDataEdit, "location_id", '601');
                        this.$set(this.rowDataEdit, "carton_code", '601');
                        this.$set(this.rowDataEdit, "move_location_id", '801');
                        this.$set(this.rowDataEdit, "move_carton_code", '801');
                        break;
                    case 'JN':
                        this.$set(this.rowDataEdit, "location_id", '601');
                        this.$set(this.rowDataEdit, "carton_code", '601');
                        this.$set(this.rowDataEdit, "move_location_id", 'JX0');
                        this.$set(this.rowDataEdit, "move_carton_code", 'JX0');
                        break;
                    case 'LN':
                        this.$set(this.rowDataEdit, "location_id", '601');
                        this.$set(this.rowDataEdit, "carton_code", '601');
                        this.$set(this.rowDataEdit, "move_location_id", 'D00');
                        this.$set(this.rowDataEdit, "move_carton_code", 'D00');
                        break;
                    case 'VN':
                        this.$set(this.rowDataEdit, "location_id", '601');
                        this.$set(this.rowDataEdit, "carton_code", '601');
                        this.$set(this.rowDataEdit, "move_location_id", 'T00');
                        this.$set(this.rowDataEdit, "move_carton_code", 'T00');
                        break;
                    default:
                        this.$set(this.rowDataEdit, "location_id", '');
                        this.$set(this.rowDataEdit, "carton_code", '');
                        this.$set(this.rowDataEdit, "move_location_id", '');
                        this.$set(this.rowDataEdit, "move_carton_code", '');
                }
            }
        },
        editRowEvent(row){
            //this.rowDataEdit = row; //不可以直接賦row給this.rowDataEdit
            this.setRowDataEdit(row);
            this.selectRow = row;
            this.showEdit = true;
        },
        setRowDataEdit(curRow){
            this.rowDataEdit= {
                id: curRow.id, mo_id:curRow.mo_id,shipment_suit:curRow.shipment_suit, goods_id: curRow.goods_id, goods_name: curRow.goods_name, unit: curRow.unit, 
                location_id: curRow.location_id,carton_code:curRow.location_id, transfer_amount: curRow.transfer_amount,sec_unit: curRow.sec_unit,sec_qty: curRow.sec_qty, 
                nwt: curRow.nwt, gross_wt: curRow.gross_wt, package_num: curRow.package_num, position_first: curRow.position_first,lot_no:curRow.lot_no, remark: curRow.remark,
                move_location_id: curRow.move_location_id,move_carton_code: curRow.move_location_id,do_color:curRow.do_color,sequence_id:curRow.sequence_id,
                row_status:curRow.row_status
            } 
        },
        clearRowDataEdit(){
            this.rowDataEdit= {
                id: '', mo_id: '',shipment_suit:0, goods_id: '', goods_name: '', unit: 'PCS', location_id: '',carton_code:'', transfer_amount: 0, 
                sec_unit: 'KG', sec_qty: 0.00, nwt: 0.00, gross_wt: 0.00, package_num: '0', position_first: '', lot_no: '', remark: '',
                move_location_id: '',move_carton_code:'', do_color: '',tran_id:'', sequence_id: '', row_status:''
            }
        },
        clearRowDataEdit2(){
            this.rowDataEdit2= {
                mo_id:'',goods_id:'',goods_name:'',inventory_qty:0,inventory_sec_qty:0.00,con_qty:0,sec_qty:0.00,sec_unit:'KG',
                location:'',carton_code:'',order_qty:0,transfer_amount:0,nostorage_qty:0,goods_unit:'SET',remark:'',mrp_id:'',
                unit_code:'PCS',lot_no:'',obligate_qty:0, id:'',upper_sequence:'',sequence_id:'',row_status:'',qty_rate:0.0000000,bom_qty:0
            }
        },        
        //彈窗中確認
        submitEvent() {
            if(!this.isEditHead){//或者if(this.headStatus==="")
                this.$XModal.alert({ content: '非編輯狀態,不可以進行此操作!', mask: false });
                return;
            }
            if(this.rowDataEdit.row_status != 'NEW'){
                this.rowDataEdit.row_status='EDIT';//編輯標記
            }
            Object.assign(this.selectRow, this.rowDataEdit);
            this.forceRerender();//強制渲染表格中的checkbox
            this.showEdit = false;

            //重新設置表格二數組originData2值
            if(this.rowDataEdit.row_status==='NEW'){
                axios.get("/TransferOut/GetPackingInfoByMo", { params: { moId: this.selectRow.mo_id } }).then(
                    (response) => {  
                        //先刪除表格2原始表中對應的數據，然后再添加，避免重復添加 
                        let lenths = this.originData2.length;
                        if(lenths>0){ 
                            lenths =lenths -1;                            
                            for( let i=lenths;i>=0;i--){
                                if(this.originData2[i].upper_sequence === this.selectRow.sequence_id){                                    
                                    this.originData2.splice(i,1);//移除
                                }
                            }
                        }
                        if(this.selectRow.shipment_suit){
                            let conQty=0,conSecQty=0.00,totalSecQty=0.00;                        
                            let transferAmount = this.selectRow.transfer_amount;//表一中的轉倉數量
                            for(let i=0;i<response.data.length;i++){
                                if(response.data[i].qty === transferAmount){
                                    conQty = response.data[i].qty * response.data[i].dosage;
                                    conSecQty = (conQty * response.data[i].qty_rate).toFixed(2); // response.data[i].sec_qty;
                                }else{
                                    conQty = transferAmount * response.data[i].dosage;
                                    conSecQty = (conQty * response.data[i].dosage * response.data[i].qty_rate).toFixed(2);
                                }  
                                totalSecQty = parseFloat(totalSecQty) + parseFloat(conSecQty);
                                this.originData2.push({
                                    mo_id:this.selectRow.mo_id,goods_id:response.data[i].goods_id,goods_name:response.data[i].goods_name,
                                    inventory_qty:response.data[i].qty,inventory_sec_qty:response.data[i].sec_qty,con_qty:conQty,sec_qty:conSecQty,sec_unit:'KG',
                                    location:'601',carton_code:'601',order_qty:response.data[i].order_qty,transfer_amount:0,nostorage_qty:0,goods_unit:'SET',remark:'',mrp_id:'',
                                    unit_code:'PCS',lot_no:response.data[i].lot_no,obligate_qty:0,id:this.headData.id,upper_sequence:this.selectRow.sequence_id,
                                    sequence_id:comm.pad(i + 1,4) +"h",row_status:'NEW',qty_rate:response.data[i].qty_rate,bom_qty:response.data[i].dosage
                                });
                            }       
                            totalSecQty = totalSecQty.toFixed(2);
                            this.selectRow.sec_qty = totalSecQty;
                            this.rowDataEdit.sec_qty = totalSecQty;
                        }
                        Object.assign(this.selectRow, this.rowDataEdit);
                        //過慮出表格二的值
                        this.setTableData2ByCurrentMo();
                    }
                ).catch(function (response) {
                    alert(response);
                    return;
                });                
          }
        },// --end of submitEvent
        //強制渲染CheckBox列.--保存行明細時執行this.forceRerender();
        forceRerender() {
            this.componentKey += 1;
        },
        moChangeEvent(moId,suit){            
            if(this.isMoEmpty(moId)){
                return;
            }
            axios.get("/TransferOut/GetProductIdByMo", { params: { moId:moId,suit:suit } }).then(
                (response) => {                   
                    this.rowDataEdit.goods_id = response.data[0].goods_id;
                    this.rowDataEdit.goods_name = response.data[0].goods_name;
                    this.rowDataEdit.do_color = response.data[0].do_color;
                    this.rowDataEdit.lot_no = response.data[0].lot_no;
                    this.rowDataEdit.transfer_amount = response.data[0].qty;
                    this.rowDataEdit.sec_qty = response.data[0].sec_qty;
                }
            ).catch(function (response) {
                this.rowDataEdit.goods_id="";
                this.rowDataEdit.goods_name ="";
                this.rowDataEdit.do_color = "";
                this.rowDataEdit.lot_no = "";
                this.rowDataEdit.transfer_amount = 0;
                this.rowDataEdit.sec_qty = 0;
                alert(response);
            });
        },
        shipmentSuitChangeEvent(moId,suit){  
            if(!this.isEditHead){//是否編輯狀態                
                return;
            }
            if(this.isMoEmpty(moId)){
                return;
            }
            this.moChangeEvent(moId,suit);            
        },
        //顯示庫存批號 
        getItemData(location_id,mo_id){
            if(this.$utils.isEmpty(mo_id)){
                this.$refs.xTable1.setActiveCell(this.selectRow, "mo_id");                
                return;
            }
            axios.get("/Adjustment/GetStGoodsLotNo", { params: { within_code: '0000',location_id:location_id,mo_id:mo_id } }).then(
                (response) => {
                    this.goodsLotNoList = response.data;
                }
            ).catch(function (response) {
                alert(response);
            });
        },   
        isMoEmpty(moId){
            let result=false;
            if(this.$utils.isEmpty(moId)){
                this.rowDataEdit.goods_id="";
                this.rowDataEdit.goods_name="";
                this.rowDataEdit.do_color="";
                result=true;
            }
            return result;
        },
        //--start*多欄列表,下拉容器,查找庫存表中的貨品編號與批號
        cellLotNoClick(row){
            let $pulldown = this.$refs.pulldownRef;
            if ($pulldown) {
                let rowIndex = row.$rowIndex;
                let goodsId = row.data[rowIndex].goods_id;  
                let goodsName = row.data[rowIndex].goods_name;  
                let lotNo = row.data[rowIndex].lot_no;
                let doColor = row.data[rowIndex].do_color;
                if(this.$utils.getSize (goodsId)===18){
                    this.$set(this.rowDataEdit,"shipment_suit",0);
                }                
                this.$set(this.rowDataEdit,"goods_id",goodsId);
                this.$set(this.rowDataEdit,"goods_name",goodsName);
                this.$set(this.rowDataEdit,"lot_no",lotNo);
                this.$set(this.rowDataEdit,"do_color",doColor);
                if(this.rowDataEdit.row_status===""){
                    this.$set(this.rowDataEdit,"row_status", "EDIT");
                }
                $pulldown.hidePanel();
            }
        },
        //pageChangeEvent({ currentPage, pageSize }) {
        //    this.pagerConfig.currentPage = currentPage;
        //    this.pagerConfig.pageSize = pageSize;
        //},
        //--end
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
        //點擊表格一時表格二跟著變化
        setTableData2ByCurrentMo () {
            this.tableData2 = [];
            if (this.curRowSeqId !='') {
                this.tableData2 = this.filterOriginData2();
            }
        },
        //項目刪除
        tempDelRowEvent:async function(){
            if(this.headData.state !='0'){
                this.$XModal.alert({ content: "此單據已批準,當前操作無效!", mask: false });
                return;
            }
            let status = await comm.checkApproveState('st_transfer_mostly',this.headData.id);
            if(status ==='1'||status ==='2'){
                let str = (status ==='1')?"批準":"注銷";
                this.$XModal.alert({ content: `后端數據已是${str}狀態,當前操作無效!`, mask: false });
                return;
            }
            if(this.curDelRow){
                let $table = this.$refs.xTable1;
                let len = $table.$data.tableData.length;//原來的代碼$table.tableData.length取值有問題
                if(len ===1){
                    this.$XModal.alert({ content: "已是最后一行,不可以繼續刪除!", mask: false });
                    return;
                }
                this.$XModal.confirm("是否確定要刪除當前行？").then(type => {
                    if(type ==="confirm"){
                        //刪除表格2原始表中對應的數據
                        let lenths = this.originData2.length - 1;
                        for( let i=lenths;i>=0;i--){
                            if(this.originData2[i].upper_sequence === this.selectRow.sequence_id){                                    
                                this.originData2.splice(i,1);//移除原始數組中對應的值
                            }
                        }
                        if(this.selectRow.row_status ==='NEW'){
                            //移除表格二數組中的元素
                            this.tableData2=[];
                        }else{
                            //記錄需刪除表格2后臺中已存在的對應數據                            
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
        //保存
        saveAllEvent: async function() {
            if(this.headData.id ==="" ){                
                this.$XModal.alert({ content: '編號不可為空!', mask: false });
                return;
            }
            if(this.headData.transfer_date ===""){
                this.$XModal.alert({ content: '轉出日期不可為空!', mask: false });
                return;
            }
            if(this.headData.department_id ===""){
                this.$XModal.alert({ content: '部門不可為空!', mask: false });
                return;
            }
            const $table = this.$refs.xTable1;
            if($table.tableData.length == 0){
                this.$XModal.alert({ content: '明細資料不可為空!', mask: false });
                return;
            }           
            //保存前對成份明細資料1進行有效性檢查
            const errMap = $table.fullValidate().catch(errMap => errMap);
            let checkTableData1Flag = true;
            await errMap;//同步操作,等待校驗結果
            errMap.then(res=>{
                if(res){ //res为promise對象中的值,如果值是對象,說明數據校驗通不過
                    checkTableData1Flag = false;
                    this.$XModal.alert({ content: '注意:明細數據不完整,請返回檢查!', mask: false });
                }
            });
            await checkTableData1Flag;
            if(!checkTableData1Flag){
                return;
            }
            
            //--start保存前對成份明細資料2進行有效性檢查
            let itemsUpdate1 = null,row_no=0, sequence_id="",checkTableData2Flag = true;            
            for(let i=0;i<$table.tableData.length;i++){
                row_no = i+1;
                itemsUpdate1 = $table.tableData[i];
                sequence_id = itemsUpdate1.sequence_id;
                for(let j=0;j < this.originData2.length;j++){
                    if(this.originData2[j].upper_sequence === sequence_id){
                        //數量,重量不可為0
                        if(this.originData2[j].goods_id === "" || parseFloat(this.originData2[j].con_qty)<=0 || parseFloat(this.originData2[j].sec_qty)<=0) {
                            this.$refs.xTable1.setCurrentRow(itemsUpdate1);//定位至某一行
                            this.curRowSeqId = sequence_id;
                            this.setTableData2ByCurrentMo();
                            checkTableData2Flag = false;
                            break;
                        }
                    }
                }
                if(!checkTableData2Flag){
                    break;
                }
            }   
            if(!checkTableData2Flag){
                //成份資料數據有效性通不過
                this.$XModal.alert({ content: `第${row_no}行:【${itemsUpdate1.mo_id}】【${itemsUpdate1.goods_id}】\n對應成份資料數據不完整,請返回檢查!`, mask: false });
                return;
            }
            //--end 保存前對成份明細資料2進行有效性檢查

            //檢查當前用戶是否有負責部門的操作權限
            let userid = this.userId;
            let deptid = this.headData.department_id;
            let rights = await comm.checkUserDeptRights(userid,deptid);
            if(rights !="1")
            {                
                this.$XModal.alert({ content: `當前用戶: 【${userid} 】,無負責部門【${deptid}】操作權限!`,status: 'info' , mask: false });
                return;
            }
           
            //檢查成份庫存是否夠扣減
            //傳給后端存儲過程的表數據類型參數結構
            this.partData = [];
            for(let i in this.originData2){
                this.partData.push(
                    {within_code:'0000', out_dept:this.headData.department_id, upper_sequence:this.originData2[i].upper_sequence,
                    sequence_id:this.originData2[i].sequence_id, mo_id:this.originData2[i].mo_id, goods_id:this.originData2[i].goods_id,
                    lot_no:this.originData2[i].lot_no, con_qty:this.originData2[i].con_qty, sec_qty:this.originData2[i].sec_qty}
                );
            }
            let partData = this.partData;
            //此處必須加await,且checkStock函數也要設置成同步執行
            let strError = await this.checkStock(partData);
            if(this.validStockFlag===false){
                //檢查成份庫存不足,返回放棄當前保存
                this.$XModal.alert({ content: strError, mask: false });
                return;
            }            
            //******以上代碼是數據校驗部分

            this.headData.head_status = this.headStatus;//表頭新增或修改的標識           
            let headData = JSON.parse(JSON.stringify(this.headData)); //{key1:value1,key2:value2}            
            this.tableData1 = $table.tableData;
            let lstDetailData1 = this.tableData1; //數組格式[{key1:value1},{key2:value2}]           
            let lstDetailData2 = this.originData2; //數組格式[{key1:value1},{key2:value2}]
            let lstDelData1 = this.delData1; 
            let lstDelData2 = this.delData2;
            let user_id = this.userId;
            axios.post("/TransferOut/Save",{headData,lstDetailData1,lstDetailData2,lstDelData1,lstDelData2,user_id}).then(
                (response) => {
                    if(response.data=="OK"){                                  
                        this.setStatusHead(false);
                        //重查刷新數據                       
                        this.searchID = this.headData.id;
                        this.findByID();
                        this.$XModal.message({ content: '數據保存成功!', status: 'success' });
                    }else{
                        this.$XModal.alert({ content: '數據保存失敗!',status: 'error' , mask: false });                                 
                    }
                }
            ).catch(function (response) {    
                alert(response);
            });
            this.headStatus = "";
            this.delData1 = [];
            this.delData2 = [];
            
        },//--end of saveAllEvents

        //保存前檢查庫存是否夠扣除
        checkStock:async function(partData){
            let rtn="";
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
                                let rw = this.tableData1[i] ;//this.Rows.data[i];
                                this.tempSelectRow = JSON.parse(JSON.stringify(rw));//暫存當前行,以便檢查是否有更改
                                this.$refs.xTable1.setCurrentRow(rw);//定位至當前索引所在的行
                                this.rowDataEdit = rw;
                                this.selectRow = rw;
                                this.curRowSeqId = this.selectRow.sequence_id;
                                this.setTableData2ByCurrentMo();
                                break;
                            }
                        }    
                        rtn =`第 ${row_no} 行【${this.selectRow.mo_id} -- ${this.selectRow.goods_id}】\n成份:【${response.data[0].goods_id}】庫存不足!\n 数量差異: ${con_qty}\n 重量差異: ${sec_qty}`;
                        //this.$XModal.alert({ content: `第 ${row_no} 行【${this.selectRow.mo_id} -- ${this.selectRow.goods_id}】\n成份:【${response.data[0].goods_id}】庫存不足!\n 数量差異: ${con_qty}\n 重量差異: ${sec_qty}`,status: 'warning' , mask: false });
                    }else{
                        this.validStockFlag = true;//庫存檢查通過
                    }
                }
            )
            return rtn;
        },        
        showLotNo:async function(data){
            if(!this.isEditHead){//是否編輯狀態                
                return;
            }
            if(this.headStatus !='NEW'){//是否編輯狀態                
                return;
            }
            if(this.headData.state==='0') {//單元格為可編輯狀態時
                let moId = data.mo_id;
                let locationId = this.headData.department_id;
                this.seledtLotNoRow = null;                
                //提取批号数据 //批號下拉列表框
                await axios.get("/TransferOut/GetStDetailsLotNo", { params: { moId:moId,locationId:locationId } }).then(
                    (response) => {
                        //this.lotNoList = response.data;//cancel 2024/10/12     
                        this.tableLotNoList = response.data;
                    }
                ).catch(function (response) {
                    //this.lotNoList=[];//cancel 2024/10/12
                    this.tableLotNoList = [{mo_id:'',goods_id:'',goods_name:'',lot_no:'',qty:0,sec_qty:0,location_id:''}];
                    alert(response);
                });

                //this.tableLotNoList = this.lotNoList; //cancel 2024/10/12
                let $pulldown = this.$refs.pulldownRef;
                if ($pulldown) {
                    $pulldown.togglePanel();  //$pulldown.showPanel();
                }
            }
        },
        //主檔查詢
        getHead:async function(id) {           
            //此處的URL需指定到祥細控制器及之下的動Action,否則在轉出待確認彈窗中的查詢將數出錯,請求相對路徑的問題
            await axios.get("/TransferOut/GetHeadByID", { params: { id: id }}).then(
                (response) => {                
                    this.headData = {
                        id: response.data.id,
                        transfer_date: response.data.transfer_date,
                        bill_type_no: response.data.bill_type_no,
                        group_no: response.data.group_no,
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
                        state: response.data.state,
                        row_status:""
                    }                   
                    this.$set(this.headData,'state',response.data.state);
                    this.stateFontColor = (response.data.state==='2')?"color:red":"color:black";                   
                    this.headStatus = "";//新單或舊數據的修改標識                   
                    this.btnItemTitle = (this.headData.state==='0')?'修改':'劉覽';
                    //日期統一轉成yyyy-MM-dd格式,否則判斷是否有修改時引起不正確的判斷
                    this.$set(this.headData,'transfers_state',this.$utils.toDateString(this.headData.transfers_state, 'yyyy-MM-dd'));
                    //深度複製一個對象，用來判斷數據是否有修改
                    this.tempHeadData = JSON.parse(JSON.stringify(this.headData));
                    //const ymd = this.$utils.toDateString(this.tempHeadData.transfers_state, 'yyyy-MM-dd');                  
                    if(this.headData.state ==='1'){                        
                        this.unok_status = "0";//顯示反批準按鈕
                    }else{
                        if(this.headData.state ==='0'){                           
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
            await axios.get("/TransferOut/GetDetailsByID", { params: { id: id }  }).then(
                (response) => {
                    this.tableData1 = response.data;
                    this.curRowSeqId = "";
                    if(this.tableData1.length>0){
                        this.curRowSeqId = this.tableData1[0].sequence_id;                       
                        if(this.headData.state==='0'){
                            this.tempSelectRow = JSON.parse(JSON.stringify(this.tableData1[0]));//暫存當前行,以便檢查是否有更改                           
                        }
                        this.selectRow = this.tableData1[0];
                        //this.selectRow = JSON.parse(JSON.stringify(this.tableData1[0]));//this.tableData1[0];     
                        let $table = this.$refs.xTable1;
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
            await axios.get("/TransferOut/GetDetailsPartByID", { params: { id: id}  }).then(
                 (response) => {
                     this.originData2 = response.data;//全部數據暫存到臨時變量中
                     this.tempTableData2 =  JSON.parse(JSON.stringify(response.data));
                     this.setTableData2ByCurrentMo(); //從臨時變量中找出對應的數據出來填充表格2,默認表格1中的第一行                    
                 }
             ).catch(function (response) {
                 alert(response);
             });
        },
        //批準&反批準 approveType: 1--批準,0--反批準
        approveEvent:async function(approveType) {
            if(this.isEditHead===true){
                this.$XModal.alert({ content: "當前為編輯狀態,當前操作無效!", mask: false });
                return;
            };
            if ((this.headData.id != "") && (this.tableData1.length > 0)) {
                //批準,反批準都要檢查是否是註銷狀態
                if(this.headData.state === "2"){
                    this.$XModal.alert({ content: "注銷狀態,當前操作無效!", mask: false });
                    return;
                }
                let ls_success= (approveType==='1')?"批準成功!":"反批準成功!";
                let ls_error= (approveType==='1')?"批準失敗!":"反準失敗!";
                let ls_type = (approveType==='1')?"批準":"反批準";
                let ls_is_approve = `確定是否要進行【${ls_type}】操作？`;
                //取后端單據狀態
                let status = comm.checkApproveState('st_transfer_mostly',this.headData.id);               
                if(status==='2'){
                    this.$XModal.alert({ content: "后端數據已是注銷狀態,當前操作無效!", mask: false });
                    return;
                }
                //--批準/反批準權限檢查,20067為模塊對應菜單的代碼,例子為轉出單            
                let power = await comm.checkPermission(this.userId,"20067","pubApprove");
                if (power === "0") {
                    return;
                }
                if(approveType==='1') {//準備批準時
                    //進行當前批準操作前再次檢查后端是否已被別的用戶批準.
                    if(this.headData.state === "1"){
                        this.$XModal.alert({ content: "已是批准狀態,當前操作無效!", mask: false });
                        return;
                    }
                    if(status==="1"){
                        //后臺數據已為批準狀態,已被別的用戶批準
                        this.$XModal.alert({ content: "后端數據已是批準狀態,當前操作無效!", mask: false });
                        return;
                    }
                }
                if(approveType==='0'){                    
                    //進行反批准
                    //進行當前反批準操作前再次檢查后端是否已被別的用戶反批準.
                    if(status==="0"){
                        //后臺數據已為批準狀態,已被別的用戶批準
                        this.$XModal.alert({ content: "后端數據已是未批準狀態,當前操作無效!", mask: false });
                        return;
                    }                   
                    //檢查已批準日期是否為當日?,超過當日則不可反批準
                    let isApprove = await comm.canApprove(this.headData.id,"w_transfer_out");//轉出單
                    if(isApprove ==="0"){
                        this.$XModal.alert({ content: "注意:【批準日期】必須為當前日期(只能對當日期的單據進行反批準)", mask: false });
                        return;
                    }
                }
                this.$XModal.confirm(ls_is_approve).then(type => {
                    if (type == "confirm") {
                        this.headData.check_by = this.userId;
                        let head = JSON.parse(JSON.stringify(this.headData));//深拷貝轉成JSON數據格式
                        let approve_type = approveType;
                        let user_id = this.userId;
                        this.loading = true;
                        //當按下反批準/批準按鈕立即禁止顯示當前按鈕,當操作失敗時再恢復顯示
                        this.showButtonApprove(approve_type,"1");//隱藏反批準/批準按鈕
                        axios.post("/TransferOut/Approve",{head,user_id,approve_type}).then(
                            (res) => {
                                if(res.data ==="OK"){
                                    this.setStatusHead(false);                                    
                                    //重查刷新數據
                                    this.getHead(this.headData.id);//刷新表頭即可
                                    this.$XModal.message({ content: ls_success, status: "success" });                                    
                                }else{
                                    this.$XModal.alert({ content: ls_error + res.data,status: "warning" , mask: false });  
                                    this.showButtonApprove(approve_type,"0");//顯示反批準/批準按鈕
                                }
                                this.loading = false;
                            }
                        ).catch(function (res) {
                            this.loading = false;
                            this.$XModal.alert({ content: "系統錯誤:" + res,status: "error" , mask: false }); 
                            this.showButtonApprove(approve_type,"0");//顯示反批準/批準按鈕                            
                        });
                    }
                })
            } else {
                this.$XModal.alert({ content: "主檔編號不可為空,當前操作無效!", status: "warning" });
                return;
            };
        },
        //*禁止/顯示 反批準/批準 按鈕,        
        showButtonApprove(approve_type,value){
            //approve_type:1--批準;0--反批準. value:1--隱藏,0--顯示
            if(approve_type ==="1"){                
                this.unok_status = value;//禁止顯示反批準按鈕
            }else{
                this.ok_status = value;//禁止顯示批準按鈕
            }
        },
        
        ////記錄前后定位
        //handlerNextOrPrev(type){
        //    let curIndex=0;
        //    if(type==="Next"){
        //        if(this.curRowIndex < this.Rows.data.length-1){
        //            curIndex = this.curRowIndex + 1;//定位至下一條記錄
        //            this.curRowIndex = curIndex;
        //        }else{
        //            //已是最后一條記錄
        //            curIndex = this.curRowIndex;
        //        }
        //    }else{//Prev前一條記錄
        //        if(this.curRowIndex > 0){
        //            curIndex = this.curRowIndex - 1;
        //            this.curRowIndex = curIndex;
        //        }else{
        //            curIndex = 0;
        //        }
        //    }
        //    var rw = this.Rows.data[curIndex];//取當前索引對應的行對象            
        //    this.tempSelectRow = JSON.parse(JSON.stringify(rw));//暫存當前行,以便檢查是否有更改            
        //    this.$refs.xTable1.setCurrentRow(rw);//定位至當前索引所在的行
        //    this.rowDataEdit = rw;
        //    this.selectRow = rw;
        //    this.curRowSeqId = this.selectRow.sequence_id;
        //    //this.setTableData2ByCurrentMo();
        //},



    },// end of methond 
    watch: {
            rowDataEdit: {
                handler (val, oldVal) {
                    if(this.rowDataEdit.mo_id){
                        this.rowDataEdit.mo_id = this.rowDataEdit.mo_id.toUpperCase();
                    }
                    //if(this.rowDataEdit.goods_id){
                    //    this.rowDataEdit.goods_id = this.rowDataEdit.goods_id.toUpperCase();
                    //}                               
                },
                deep: true
            },
            searchID : function (val) {
                this.searchID = val.toUpperCase(); 
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
                            if(this.selectRow2.row_status ===""){
                                this.selectRow2.row_status ="EDIT";
                            }
                            break;
                        }
                    }
                    if(editFlag){
                        if(this.selectRow2.row_status ==='EDIT'){
                            for(let i=0;i < this.originData2.length;i++){
                                if(this.originData2[i].upper_sequence === upper_sequence && this.originData2[i].sequence_id ===sequence_id){
                                    Object.assign(this.originData2[i], this.selectRow2);//當前行寫入數組
                                    break;
                                }
                            }
                        }
                        if(this.selectRow2.row_status ==='NEW'){
                            let existsFlag = false;
                            for(let i=0;i < this.originData2.length;i++){
                                if(this.originData2[i].upper_sequence === upper_sequence && this.originData2[i].sequence_id ===sequence_id){
                                    existsFlag = true;//已存在
                                    Object.assign(this.originData2[i], this.selectRow2);
                                    break;
                                }
                            }
                        }                        
                    }
                }
            },
            deep: true
        }//-end of selectRow2
    },
    mounted() { 
        let that = this; 
        window.onresize = () => {
            return (() => {                
                let windowH2 = $(parent.window).height()-that.constSpace;
                that.tableHeight1 = windowH2*that.constP1;
                that.tableHeight2 = windowH2*that.constP2;
                            
            })()
        };
    } //--end of mounted()

}

var app = new Vue(main).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;
Vue.prototype.$utils = XEUtils;

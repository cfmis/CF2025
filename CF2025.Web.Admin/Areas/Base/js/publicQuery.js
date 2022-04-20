var publicQuery={
    data() {
        return {
            window_id: "",
            userId: "",
            headerCellStyle:{background:'#F5F7FA',color:'#606266',height:'25px',padding:'2px'},
            curDelRow:null,
            curResultRow:null,
            tableCondition: [],
            tempSaveData:[],
            rowCondition:{window_id:'', field_name: '',operators: '',field_value: '',logic: '' ,table_name:'',field_type:'',id:0,sequence_id:'001',row_status:''},
            tableResult: [],
            operatorList: [
                { label: '', value: '' },
                { label: '等于', value: '=' },
                { label: '開頭', value: 'LIKE' },
                { label: '包含', value: 'IN' },
                { label: '不包含', value: 'NOT IN' },
                { label: '大于', value: '>' },
                { label: '大于', value: '<' },
                { label: '大于等于', value: '>=' },
                { label: '小于等于', value: '<=' },
                { label: '不等于', value: '<>' },
            ],
            logicList: [ { label: '且', value: 'AND' }, { label: '或者', value: 'OR' },{ label: '', value: '' } ],
            fieldNameList: [],
            validRules: {
                field_name: [{ required: true, message: '請選擇欄位名稱' }],
                operators: [{ required: true, message: '請選擇操作符' }]                       
            }            
        }
    },
    created() {
        this.userId = document.getElementById("user_id").value;
        this.window_id = document.getElementById("window_id").value;        
        this.initData();
    },
    methods: {        
        //格式化欄位標簽翻譯
        formatFieldName(val) {
            var result = "";
            var objJson = null;
            for (var i = 0; i < this.fieldNameList.length; i++) {
                 objJson = this.fieldNameList[i];               
                 if (objJson['value'] == val) {
                     result = objJson["label"];
                     break;
                 }                 
            }           
            return result;
        },
        //初始化操作符
        formatOperator(val) {
            switch (val) {
                case '=':
                    return '等于';
                    break;
                case 'LIKE':
                    return '開頭';
                    break;
                case 'IN':
                    return '包含';
                    break;
                case 'NOT IN':
                    return '不包含';
                    break;
                case '>':
                    return '大于';
                    break;
                case '<':
                    return '小于';
                    break;
                case '>=':
                    return '大于等于';
                    break;
                case '<=':
                    return '小于等于';
                    break;
                case '<>':
                    return '不等于';
                    break;
                case '':
                    return '';
                    break;
                default:
                    return '';
                    break;
            }
        },
        //初始化邏輯符
        formatLogic(val) {
            switch (val) {
                case 'AND':
                    return '且';
                    break;
                case 'OR':
                    return '或者';
                    break;
                case '':
                    return '';
                    break;
                default:
                    return '';
                    break;
            }
        },
        fieldNameChangeEvent(row){            
            //更改當前欄位名稱,聯動更改其它欄位的值
            var table_name="";
            var field_type="";
            for (var i = 0; i < this.fieldNameList.length; i++) {
                objJson = this.fieldNameList[i];               
                if (objJson['value'] == row.row.field_name) {
                    table_name = objJson["table_name"];
                    field_type = objJson["field_type"];
                    break;
                }
            } 
            row.row.table_name = table_name;
            row.row.field_type = field_type;
        },        
        initData() {           
            this.getfieldNameList(this.window_id); //初始化欄位名稱下拉列表框        
            this.getSavedList();//初始化當前用戶對應頁面已保存的查詢條件
        },
        //提取欄位名稱下拉列表
        getfieldNameList(id) {          
            axios.get("/Base/PublicQuery/GetFieldName?window_id=" + id).then(
                (response) => {                    
                    this.fieldNameList = response.data;
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        //初始化對應用戶查詢條件數據
        getSavedList(){
            axios.get("/Base/PublicQuery/GetSaved?user_id=" + this.userId + "&window_id=" + this.window_id).then(
                (response) => {
                    this.tableCondition = response.data;                    
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },     
        cellClickEvent(row){            
            this.curDelRow = row.data[row.$rowIndex];//row.$rowIndex是行索引
        },
        cellClickResultEvent(row){
            this.curResultRow = row.data[row.$rowIndex];
        },
        cellDBLClickEvent(row){
            var rowValues = row.data[row.$rowIndex];//row.$rowIndex是行索引
            this.setParentFind(rowValues.id);
        },
        insertRowEvent() {
            var rowEndIndex = -1;//添加至行尾$table.tableData.length           
            var $table = this.$refs.tableConditionRef;            
            var seq_id= comm.pad($table.tableData.length + 1,3) ;
            this.$set(this.rowCondition, "sequence_id", seq_id);
            this.$set(this.rowCondition, "row_status", 'NEW');
            this.$set(this.rowCondition,"window_id",this.window_id);
            $table.insertAt(this.rowCondition,rowEndIndex).then(({ row }) => {
                $table.setActiveCell(rowEndIndex, 'field_name');
            });
        },
        delRowEvent() {
            if(this.curDelRow){   
                this.$refs.tableConditionRef.remove(this.curDelRow);                        
                this.curDelRow = null; 
            }else{
                this.$XModal.alert({ content: '請首先指定需要刪除的項目!',mask: false });
            }
        },
        saveEvent() {
            this.saveAll();//saveAll()處理同步
        },
        okEvent(){
            if(this.curResultRow){
                this.setParentFind(this.curResultRow.id);
            }else{
                this.$XModal.alert({ content: '請首先在查詢結果中指定當前行!',mask: false });
            }
        },
        setParentFind(id){
            parent.app.searchID = id; //值賦給父窗口中的變量
            parent.app.findByID();//調用父窗口的函數
            parent.comm.closeWindow();//关闭窗口
        },
        //查詢
        searchEvent(window_id) {
            var $table = this.$refs.tableConditionRef;
            if($table.tableData.length===0){
                this.$XModal.alert({ content: '查詢條件不可為空!', mask: false });
                return;
            }
            var tableArr = []; 
            var strValue="",strFieldValue="";  
            //組合字段值關系運算符表達式
            for(var i=0;i<$table.tableData.length;i++){
                if(i < $table.tableData.length-1){
                    if($table.tableData[i].field_name==="" || $table.tableData[i].operators==="" || $table.tableData[i].logic===""){
                        this.$XModal.alert({ content: '第【'+(i+1)+'】行的欄位名稱、操作符、邏輯符不可同時為空!', mask: false });
                        return;
                    }
                }else{
                    //最后一行不用檢查邏輯符
                    if($table.tableData[i].field_name==="" || $table.tableData[i].operators===""){
                        this.$XModal.alert({ content: '第【'+(i+1)+'】行的欄位名稱、操作符!', mask: false });
                        return;
                    }
                    this.$set($table.tableData[i],"logic","");//最后一行的邏輯符設置為空
                }
                if($table.tableData[i].field_value===""){
                    this.$XModal.alert({ content: '第【'+(i+1)+'】行欄位值不可以為空!', mask: false });
                    return;
                }
                var strOperatior = $table.tableData[i].operators; //操作符
                //構建WHERE之后查詢值的語句
                //類型為D要檢查日期的有效性 
                if($table.tableData[i].field_type==="D")
                {
                    var strDate = $table.tableData[i].field_value;
                    var d =comm.checkDate(strDate);
                    if(d===false){
                        return;
                    }                    
                }                
                //字段值類型
                if($table.tableData[i].field_type==="C" || $table.tableData[i].field_type==="D")
                {
                    strFieldValue = "'" + $table.tableData[i].field_value + "'"; 
                }else{
                    if($table.tableData[i].field_type==="N"){
                        //數值型的操作符只可以是 =,>,>=,<,<=,<>
                        if($table.tableData[i].operators==="IN" || $table.tableData[i].operators==="NOT IN" || $table.tableData[i].operators==="LIKE"){
                            this.$XModal.alert({ content: '注意: 第【'+(i+1)+'】行,數值型字段操作符只允許是: \n\r【等于,大于,大于或等于,小于,小于或等于,不等于】', mask: false });
                            return;
                        }
                        //數值型字段檢查輸入的合法性
                        if(!isNaN($table.tableData[i].field_value ))
                        {
                            strFieldValue = $table.tableData[i].field_value ;
                        }
                        else{
                            this.$XModal.alert({ content: '第【'+(i+1)+'】行欄位值輸入的不是數字!', mask: false });
                            return;  
                        }                        
                    }else{
                        strFieldValue = $table.tableData[i].field_value ;
                    }                    
                }               
                //開頭
                if($table.tableData[i].operators==="LIKE"){
                    strFieldValue ="'"+ $table.tableData[i].field_value+"%'";
                }
                //包含
                if($table.tableData[i].operators==="IN"){
                    strOperatior = "LIKE"
                    strFieldValue = "'%"+$table.tableData[i].field_value+"%'";
                }
                //不包含
                if($table.tableData[i].operators==="NOT IN"){
                    strOperatior = "NOT LIKE"
                    strFieldValue = "'%"+$table.tableData[i].field_value+"%'";
                }
                strValue += $table.tableData[i].table_name +"." + $table.tableData[i].field_name + " "+ strOperatior + " " + strFieldValue + " "+ $table.tableData[i].logic +" ";
            }
            
            //構建SELECT 后面的查詢字段
            var strSelect=""
            var joinArr=[];
            var fromArr=[];
            var orderbyArr=[];
            for(var i=1;i<this.fieldNameList.length;i++){
                //i=0是空格,所以循環從1開始.
                if(i===1){
                    strSelect += this.fieldNameList[i].table_name +"." + this.fieldNameList[i].value;
                }else{
                    strSelect += "," + this.fieldNameList[i].table_name +"." + this.fieldNameList[i].value;
                }
                //構建FROM之后表名
                if(this.fieldNameList[i].from_table !==""){
                    if(fromArr.indexOf(this.fieldNameList[i].from_table) === -1) {
                        fromArr.push(this.fieldNameList[i].from_table)
                    }
                }
                //構建WHERE之后表間的關聯語句,存儲至臨時數組
                if(this.fieldNameList[i].table_relation !==""){
                    if(joinArr.indexOf(this.fieldNameList[i].table_relation) === -1) {
                        joinArr.push(this.fieldNameList[i].table_relation)
                    }
                }
                //構建Order by數組
                if(this.fieldNameList[i].table_relation !==""){
                    if(orderbyArr.indexOf(this.fieldNameList[i].order_by) === -1) {
                        orderbyArr.push(this.fieldNameList[i].order_by)
                    }
                }
            }
            var strJoin ="";
            if(joinArr[0].length>0){
                strJoin = joinArr[0];
            }else{
                strJoin = " 1=1 ";
            }
            //構建From表來源
            var strTable ="";
            if(fromArr[0].length>0){
                strTable = fromArr[0];
            }else{
                this.$XModal.alert({ content: '注意:后臺查詢構造語字段【from_table】內容不正確!', mask: false });
                return;
            }
            //構建排序
            var strOrderby="";
            if(orderbyArr[0].length>0){
                strOrderby = orderbyArr[0];            
            }
            //構建完整的sql語句
            //在strTable(table_from)中寫有inner join left join,strJoin(table_relation)就不用寫           
            var strSql="SELECT " + strSelect +" FROM "+ strTable +" WHERE "+ strJoin +" AND "+ strValue 
            if(strOrderby !==""){
                strSql +=" Order by "+ strOrderby;
            }
            axios.get("/Base/PublicQuery/Query?sqlText=" + strSql).then(
                (response) => {
                    if(response.data){
                        var jsonObj = JSON.parse(response.data); //jason字符串转换为json对象
                        this.tableResult = jsonObj;
                    }else{
                        this.tableResult=[];
                        this.$XModal.message({ content: '沒有滿足查找條件的數據!', status: 'info' });
                    }
                }
           ).catch(function (response) {
               alert(response);
           }); 
        },

        saveAll: async function() {
            const $table = this.$refs.tableConditionRef;
            const errMap = $table.fullValidate().catch(errMap => errMap);
            await errMap;//同步操作,等待校驗結果
            errMap.then(res=>{
                this.tempSaveData=[]; 
                if(res) // res为promise對象中的值,如果值是對象,說明數據校驗通不過
                {
                    this.$XModal.alert({ content: '注意:查詢條件數據不完整,請返回檢查!', mask: false });
                }else{
                    //新增或者是否有更改的數據
                    this.tempSaveData=[];
                    for(var i=0;i<$table.tableData.length;i++){
                        this.tempSaveData.push($table.tableData[i]);
                    }
                    //添加從后臺取出數據后補刪除的記錄
                    var items= { removeRecords} = $table.getRecordset();
                    if(items.removeRecords.length>0){
                        for(var i=0;i<items.removeRecords.length;i++){
                            this.$set(items.removeRecords[i], "row_status", "DEL" );
                            this.tempSaveData.push(items.removeRecords[i]);
                        }
                    }
                    if(this.tempSaveData.length===0){
                        this.$XModal.alert({ content: '沒有需要保存的數據!',mask: false });
                        return;
                    }
                    var detailData = this.tempSaveData;
                    var user_id = this.userId;                   
                    axios.post("/Base/PublicQuery/SaveQuery",{user_id:user_id,detailData:detailData}).then(
                        (response) => {
                            if(response.data=="OK"){
                                this.getSavedList();
                                this.$XModal.message({ content: '數據保存成功!', status: 'success' });
                            }else{
                                this.$XModal.alert({ content: '數據保存失敗!',status: 'error' , mask: false });                                 
                            }
                        }
                    ).catch(function (response) {
                        alert(response);
                    });                   
                }
            })
        },
        formatDate({ cellValue }){
           return XEUtils.toDateString(cellValue, 'yyyy-MM-dd');
        },
        formatDateTime({ cellValue }){
           return XEUtils.toDateString(cellValue, 'yyyy-MM-dd HH:mm:ss');
        },

    } //end methods

    
}
var app = new Vue(publicQuery).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;
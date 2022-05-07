var main = {
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
            showEditStore: false,
            isEditHead: false,//主檔編輯狀態
            headData: {},
            
            server_date:"", 
            stateFontColor:"color:black",
        }
    },
    created() {
        //this.getComboxList("DeptList");//部門編碼   
        //this.getComboxList("StateList");//狀態 
        //this.getComboxList("LocationList");//倉庫,貨架,由選擇的倉庫帶出       
        //this.getComboxList("QtyUnitList");//數量單位
        //this.getComboxList("WegUnitList");//重量單位
        //this.flagChild = $("#isChild").val();
        //this.userId = $("#user_id").val();       
    },
    methods: {
        //服務器端日期
        //getDateServer: async function() {
        //    var res= await axios.get("/Base/DataComboxList/GetDateServer");
        //    this.server_date = res.data;
        //}
        findByID() {
            if (this.headStatus != "" || this.searchID === "") {
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
        showFindWindos() {
            comm.openWindos('TransferIn');
        },
        printEvent() {
            //test
            //var parentWindowHeight=$(parent.window).height();
            //this.tableHeight=$(parent.window).height()-205;

        },
    },
    watch: {
            //rowDataEdit: {
            //    handler (val, oldVal) {
            //        if(this.rowDataEdit.mo_id){
            //            this.rowDataEdit.mo_id = this.rowDataEdit.mo_id.toUpperCase();
            //        }
            //        if(this.rowDataEdit.goods_id){
            //            this.rowDataEdit.goods_id = this.rowDataEdit.goods_id.toUpperCase();
            //        }                               
            //    },
            //    deep: true
            //},
            //searchID : function (val) {
            //    this.searchID = val.toUpperCase();            
            //}
    },
    mounted() {
        //this.tableHeight=$(parent.window).height()-205;        
        //let that = this;       
        //window.onresize = () => {
        //    return (() => {
        //        that.tableHeight=$(parent.window).height()-205;                 
        //    })()
        //};
    }

}

var app = new Vue(main).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;

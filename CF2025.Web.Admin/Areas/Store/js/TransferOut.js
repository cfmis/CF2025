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
            headData: {
                id: "", transfer_date: "", bill_type_no: "", group_no: "", department_id: "", handler: "", remark: "",
                update_by: "", state: "0", create_by: "", create_date: "", update_date: "", update_count: "1",remark:"",
                check_by: "", check_date: "", head_status: ""
            },
            gridData: [],
            gridDataPart:[],
            rowDataEdit: {
                mo_id: '', shipment_suit: '0', goods_id: '', goods_name: '', unit: '', transfer_amount: 0, sec_unit: '', sec_qty: 0, package_num: 0,
                position_first: '', nwt: 0, gross_wt: 0, location_id: '', carton_code: '', inventory_qty: 0, inventory_sec_qty: 0, lot_no: '', remark: '',
                do_color:'', move_location_id:'', move_carton_code:'', id:'', sequence_id:'', row_status: ''
            },
            deptList: [],
            billTypeList:[],
            groupNoList: [],


            //server_date:"", 
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

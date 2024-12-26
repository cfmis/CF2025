/**
 * 轉出待確認列表
 */
var Main = {
    data() {
        return {            
            selectRow: null,
            loading: false,
            isDisable: false,
            tableHeight: 450,
            headerCellStyle: { background: '#F5F7FA', color: '#606266', height: '25px', padding: '2px' },
            formData: { mo_id: '', mo_id_end: '', goods_id: '', goods_id_end: '', transfer_date: '', transfer_date_end: '', id: '', id_end: '', customer_id: '', customer_id_end: '', location_id:'',shelf:'' },
            tableData: [] ,
            CustomerList: [],
            LocationList: [],            
            CartonCodeList: [{ label: '', value: '' }],
            selectData: [],
            flagCallChild:false,           
        }
    },
    created() {        
        this.getComboxList("CustomerList");//客戶編號
        this.getComboxList("LocationList");
    },   
    methods: {
        resetEvent() {
            this.formData = { mo_id: '', mo_id_end: '', goods_id: '', goods_id_end: '', transfer_date: '', transfer_date_end: '', id: '', id_end: '', customer_id: '', customer_id_end: '' };
        },
        //查找未確認入倉資料
        searchData() {
            this.loading = true;            
            setTimeout(() => {
                this.getDetails();
                //this.loading = false;
            }, 3000);
        },        
        getDetails() {//查詢
            var searchParams = {
                mo_id: this.formData.mo_id,
                mo_id_end: this.formData.mo_id_end,
                goods_id: this.formData.goods_id,
                goods_id_end: this.formData.goods_id_end,
                transfer_date: this.formData.transfer_date,
                transfer_date_end: this.formData.transfer_date_end,
                location_id: this.formData.location_id,
                shelf: this.formData.shelf,
                id: this.formData.id,
                id_end: this.formData.id_end
            }
            axios.get("GetDataList", { params: searchParams }).then(
                (response) => {
                    this.tableData = response.data;
                    this.loading = false;
                },
                (response) => {
                    alert(response.status);
                }                
            ).catch(function (response) {
                alert(response);
            });
        },

        //初始化下拉列表框
        getComboxList(SourceType) {           
            axios.get("/Base/DataComboxList/GetComboxList?SourceType=" + SourceType).then(
                (response) => {
                    if (SourceType == "CustomerList")
                        this.CustomerList = response.data;
                    else if (SourceType == "LocationList")
                        this.LocationList = response.data;                    
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        //取貨架資料
        getCartonCodeList(location_id) {            
            axios.get("/Base/DataComboxList/GetCartonCodeList?LocationId=" + location_id).then(
                (response) => {                   
                    this.$set(this.formData, "shelf", "");
                    this.CartonCodeList = response.data;
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        //確認
        getSelectionEvent() {           
            //var rowIndex =-1;
            //const $table = this.$refs.xTable
            //const record = {sex: '1',date12: '2021-01-01'}
            //$table.insertAt(record, rowIndex) 插入至表格尾部
            //$table.setActiveCell(newRow, 'name')            
            var $table = this.$refs.xTable;
            var selectRecords = $table.getCheckboxRecords();
            if (selectRecords.length > 0) {
                if (this.formData.location_id == "")
                {
                    this.$XModal.alert({ content: '請指定待轉入的倉庫 !', status: 'warning' });
                    return;
                }
                this.selectData = selectRecords;
                $table.removeCheckboxRow();//移除父表格中選中的記錄
                var url = "/Store/TransferIn?flagChild=" + "1";
                comm.showMessageDialog(url, "轉入單", 1024, 768, true);
            } else {
                this.$XModal.alert({ content: '請首先查詢出數據,并選擇需轉入的頁數資料 !', status: 'warning' });
            }                        
            
        },        
        //validData() {
        //檢查數據是否有過更改
        //    for (let i in this.formData) {
        //        if (this.formData[i] != this.prevForm[i]) {
        //            this.formData.EditFlag = 1;
        //            break;
        //        }
        //    }
        //},
        //轉大寫
        upperCase(val, fieldName) {           
            if (val) {
                var strUpper = val.toUpperCase();
                this.$set(this.formData, fieldName, strUpper);
            }
        },        
        setToEndValue(val, fieldName) {            
            if (val) {
                this.$set(this.formData, fieldName, val);
            }
        },
        handleCreate (val) {//val是新增加的数据
            this.LocationList.push({
                value: val,
                label: val
            });
        },
        
    },
    watch: {
        formData: {           
            handler (val, oldVal) {                
                this.formData.mo_id = this.formData.mo_id.toUpperCase();
                this.formData.mo_id_end = this.formData.mo_id_end.toUpperCase();
                this.formData.goods_id = this.formData.goods_id.toUpperCase();
                this.formData.goods_id_end = this.formData.goods_id_end.toUpperCase();
                this.formData.id = this.formData.id.toUpperCase();
                this.formData.id_end = this.formData.id_end.toUpperCase();
                this.formData.customer_id = this.formData.customer_id.toUpperCase();
                this.formData.customer_id_end = this.formData.customer_id_end.toUpperCase();
            },
            deep: true
        },
        
    },
    computed: {
        //
    },
    mounted() {
        this.tableHeight=$(parent.window).height()-241;
        let that = this;       
        window.onresize = () => {
            return (() => {
                that.tableHeight = $(parent.window).height() - 241;               
            })()
        };
    }

};

var app = new Vue(Main).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;

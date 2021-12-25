var Main = {
    data() {
        return {            
            selectRow: null,      
            tableDetails:[],
            loading: false,
            formData: {}, 
            //sexList: [
            //    { label: '', value: '' },
            //    { label: '女', value: '0' },
            //    { label: '男', value: '1' }
            //],
            CustomerList: [],
            LocationList: [],
            CartonCodeList: [{ label: '', value: ''}],
            tableData: [],           
            
            //tableData: [
            //{ id: 10001, name: 'Test1', role: 'Develop', sex: 'Man', age: 28, address: 'test abc', sex: '1' },
            //{ id: 10002, name: 'Test2', role: 'Test', sex: 'Women', age: 22, address: 'Guangzhou', sex: '0' },
            //{ id: 10003, name: 'Test3', role: 'PM', sex: 'Man', age: 32, address: 'Shanghai', sex: '1' },
            //{ id: 10004, name: 'Test4', role: 'Designer', sex: 'Women', age: 23, address: 'test abc', sex: '0' },
            //{ id: 10005, name: 'Test5', role: 'Develop', sex: 'Women', age: 30, address: 'Shanghai', sex: '0' },
            //{ id: 10006, name: 'Test6', role: 'Designer', sex: 'Women', age: 21, address: 'test abc', sex: '1' },
            //{ id: 10007, name: 'Test7', role: 'Test', sex: 'Man', age: 29, address: 'test abc', sex: '1' },
            //{ id: 10008, name: 'Test8', role: 'Develop', sex: 'Man', age: 35, address: 'test abc', sex: '1' }

            //],
            name: '',
            sex: 1,
            date: '',

            //resetEvent: VxeFormEvents.Reset = () => {
            //  VXETable.modal.message({ content: '重置事件', status: 'info' })
            //}

           

        }
    },
    created() {        
        this.getComboxList("CustomerList");//客戶編號
        this.getComboxList("LocationList");
    },
    methods: {
        resetEvent() {
            alert("test!");
            //VXETable.modal.message({ content: '重置事件', status: 'info' })
        },
        showMsg() {
            //if (this.name.length >= 9)
            alert("ok");
            //var vl = value;
        },
        showRowValue(value) {
            alert(value);
        },
        //formatSex (value) {
        //    if (value === '1') {
        //        return '男'
        //    }
        //    if (value === '0') {
        //        return '女'
        //    }
        //    return ''
        //},        
        //productMoBlurEvent() {
        //    if (this.formData.ProductMo == "")
        //        return;
        //    if (this.edit_mode == 1)
        //        this.getPlanFromOrder();
        //},
        //查找未確認入倉資料
        searchData() {
            this.loading = true;
            setTimeout(() => {
                this.getDetails();
                this.loading = false;
            }, 500);
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
                    this.tableDetails = response.data;
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
            var _self = this;///Base/BaseData///, { params: { ProductMo: this.editPlanDetails.GoodsID } }
            axios.get("GetComboxList?SourceType=" + SourceType).then(
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
            axios.get("GetCartonCodeList?LocationId=" + location_id).then(
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
       
        getGoodsByID() {
            if (this.editPlanDetails.GoodsID != "") {
                var _self = this;
                axios.get("GetGoodsByID", { params: { GoodsID: this.editPlanDetails.GoodsID } }).then(
                (response) => {
                    this.editPlanDetails.GoodsCname = response.data.goods_cname
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
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
        }        
        
    },
    watch:{
        // watch监听 判断是否修改  
        formData() {           
            this.formData.goods_id = this.formData.goods_id.toUpperCase()
        },
        //formData: {           
        //    handler (val, oldVal) {
        //        //for (let i in this.formData) {
        //        //    if (val[i] != this.prevForm[i]) {
        //        //        this.editFormChanged = true;
        //        //        break;
        //        //    } else {
        //        //        this.editFormChanged = false;
        //        //    }
        //        //}
        //        //console.log(this.editFormChanged);
        //        this.formData.goods_id = this.formData.goods_id.toUpperCase()
        //    },
        //    deep: true
        //}
    },
    //computed: {
    //    cpGoods_id: {
    //        get: function () {
    //            return this.formData.goods_id;
    //        },
    //        set: function (val) {
    //            this.formData.goods_id = val.toUpperCase();//转大小写的方法
    //        }
    //    }
    //}

};

var app = new Vue(Main).$mount('#app');


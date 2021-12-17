var Main = {
    data() {
        return {            
            selectRow: null,            
            tableDetails:[],
            loading3: false,
            formData: {}, 
            sexList: [
                { label: '', value: '' },
                { label: '女', value: '0' },
                { label: '男', value: '1' }
            ],
            DocSourceTypeList: [],
            SalesmanList: [],
            CurrList: [],
            OutStoreList: [],
            InvSourceTypeList: [],
            PaymentTypeList: [],
            PriceCondList: [],
            ShipModeList: [],
            AccountList: [],
            ShipWayList: [],
            ShipPortList: [],
            MoGroupList: [],
            tableData:[],
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
        }
    },
    created() {        
        //this.getComboxList("DocSourceTypeList");
        //this.getComboxList("SalesmanList");
        //this.getComboxList("CurrList");
        //this.getComboxList("OutStoreList");
        //this.getComboxList("InvSourceTypeList");
        //this.getComboxList("PaymentTypeList");
        //this.getComboxList("PriceCondList");
        //this.getComboxList("ShipModeList");
        //this.getComboxList("AccountList");
        //this.getComboxList("ShipWayList");
        //this.getComboxList("ShipPortList");
        //this.getComboxList("MoGroupList");
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
        formatSex (value) {
            if (value === '1') {
                return '男'
            }
            if (value === '0') {
                return '女'
            }
            return ''
        },        
        productMoBlurEvent() {
            if (this.formData.ProductMo == "")
                return;
            if (this.edit_mode == 1)
                this.getPlanFromOrder();
        },
        searchData() {
            this.loading3 = true;
            setTimeout(() => {                
                this.getDetails();
                this.loading3 = false;
            }, 500);

        },        
        getDetails() {//查詢
            var _self = this;
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

        //searchData() {//查詢
        //    this.isLoading = true;
        //    //this.tableTempList = [];
        //    var searchParams = {
        //        OcID: this.searchFormData.OcID,
        //        CustomerID: this.searchFormData.CustomerID,
        //        OrderDate: this.searchFormData.OrderDate,
        //        ReceivedDate: this.searchFormData.ReceivedDate,
        //        ForeignFirm: this.searchFormData.ForeignFirm,
        //        Area: this.searchFormData.Area,
        //        SallerID: this.searchFormData.SallerID,
        //        Season: this.searchFormData.Season,
        //        ContractID: this.searchFormData.ContractID,
        //        BrandID: this.searchFormData.BrandID,
        //        ProductMo: this.searchFormData.ProductMo,
        //        ProductID: this.searchFormData.ProductID
        //    }
        //    axios.get("GetOcHeadReturnList", { params: searchParams }).then(
        //        (response) => {
        //            this.tableTempList = [];
        //            for (var i = 0; i < response.data.length; i++) {
        //                this.tableTempList.push(response.data[i]);
        //            }
        //            //_self.goodsList.push({ goods_id:"",goods_cname:"" });
        //            //this.fillData();
        //            this.tableDataSearch = this.tableTempList;
        //            this.isLoading = false;
        //        },
        //        (response) => {
        //            alert(response.status);
        //        }
        //    ).catch(function (response) {
        //        this.isLoading = false;
        //        alert(response);
        //    });
        //},

        getComboxList(SourceType) {
            var _self = this;///Base/BaseData///, { params: { ProductMo: this.editPlanDetails.GoodsID } }
            axios.get("GetComboxList?SourceType=" + SourceType).then(
                (response) => {
                    if (SourceType == "DocSourceTypeList")
                        this.DocSourceTypeList = response.data;
                    else if (SourceType == "SalesmanList")
                        this.SalesmanList = response.data;
                    else if (SourceType == "CurrList")
                        this.CurrList = response.data;
                    else if (SourceType == "OutStoreList")
                        this.OutStoreList = response.data;
                    else if (SourceType == "InvSourceTypeList")
                        this.InvSourceTypeList = response.data;
                    else if (SourceType == "PaymentTypeList")
                        this.PaymentTypeList = response.data;
                    else if (SourceType == "PriceCondList")
                        this.PriceCondList = response.data;
                    else if (SourceType == "ShipModeList")
                        this.ShipModeList = response.data;
                    else if (SourceType == "AccountList")
                        this.AccountList = response.data;
                    else if (SourceType == "ShipWayList")
                        this.ShipWayList = response.data;
                    else if (SourceType == "ShipPortList")
                        this.ShipPortList = response.data;
                    else if (SourceType == "MoGroupList")
                        this.MoGroupList = response.data;
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
        updateEvent() {
            //this.showEdit = false
            //this.$XModal.message({ content: '保存成功', status: 'success' })
            //Object.assign(this.selectRow, this.tablePlanDetails)
            //return;
            if (this.selectRow) {
                this.showEdit = false
                //this.$XModal.message({ content: '保存成功', status: 'success' })
                for (let i in this.editPlanDetails) {
                    if (this.editPlanDetails[i] != this.prevEditPlanDetails[i]) {
                        this.editPlanDetails.EditFlag = 1;
                        break;
                    }
                }
                Object.assign(this.selectRow, this.editPlanDetails)
            } else {
                this.editPlanDetails.EditFlag = 1;
                this.tablePlanDetails.push(this.editPlanDetails);
                //this.editPlanDetails = {};
                this.editPlanDetails = {
                    EditFlag: 0,
                    ProductMo: this.formData.ProductMo,
                    Ver: this.formData.Ver,
                    GoodsID: '',
                    GoodsCname: '',
                    RequestQty: '',
                    RequestDate: '',
                    WipID: '',
                    NextWipID: ''
                };
            }
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
        editRowEvent (row) {
            this.editPlanDetails = {
                EditFlag: 0,
                GoodsID: row.GoodsID,
                GoodsCname: row.GoodsCname,
                RequestQty: row.RequestQty,
                RequestDate: row.RequestDate,
                WipID: row.WipID,
                NextWipID: row.NextWipID
            }
            //深度複製一個對象，用來判斷數據是否有修改
            this.prevEditPlanDetails = JSON.parse(JSON.stringify(this.editPlanDetails));
            this.selectRow = row
            this.showEdit = true
        },
        saveEvent() {
            this.validData();
            var _self = this;
            var PlanHead = _self.formData;
            var PlanDetails = _self.tablePlanDetails;
            axios.post("SavePlan", { PlanHead, PlanDetails }).then(
            (response) => {
                this.SavePlan = {
                    ProductMo: "",
            };
                alert("更新成功!");
            },
            (response) => {
                alert(response.status);
            }
        ).catch(function (response) {
            alert(response);
        });
        },

        validData() {
            for (let i in this.formData) {
                if (this.formData[i] != this.prevForm[i]) {
                    this.formData.EditFlag = 1;
                    break;
                }
            }
        },
        deleteEvent() {
            let selectRecords = this.$refs.xTable.getCheckboxRecords()
            if (selectRecords.length) {
                this.$refs.xTable.removeCheckboxRow()
            } else {
                alert('请至少选择一条数据！')
                //this.$xDetails.message({ content: 'warning 提示框', status: 'warning' })
            }
        },
    },
    watch: {
        //// watch监听 判断是否修改  
        //formData: {
        //    handler (val, oldVal) {
        //        for (let i in this.formData) {
        //            if (val[i] != this.prevForm[i]) {
        //                this.editFormChanged = true;
        //                break;
        //            } else {
        //                this.editFormChanged = false;
        //            }
        //        }
        //        console.log(this.editFormChanged);
        //    },
        //    deep: true
        //}

    }
};

var app = new Vue(Main).$mount('#app');


var Main = {
    data() {
        return {
				selectTab: 'tab1',
                edit_mode: 0,
                showEdit: false,
                selectRow: null,
                tablePlanDetails: [],
                editPlanDetails: {},
                prevEditPlanDetails:{},
                searchProductMo: '',
                loading3: false,
                //ArtImageUrl: '/Images/photo.png',
                //formData: {
                //    ProductMo: '',
                //    PlanDate: '',
                //    Ver: 0,
                //    OrderQty: 0,
                //    OrderUnit: '',
                //    CustomerID: '',
                //    RequestDate: '',
                //    DeliveryDate: '',
                //    GoodsID: '',
                //    ProductRemark: '',
                //    MoRemark: '',
                //    PlanRemark: '',
                //    ApprovedTime: '',
                //    ApprovedUser: '',
                //    CreateUser: '',
                //    CreateTime: '',
                //    AmendUser: '',
                //    AmendTime:'',
                //},
                formData:{
					linkman:'',
				},
                prevForm: {},
                editFormChanged: false, // 是否修改标识
                sexList: [
                { label: '', value: '' },
                { label: '女', value: '0' },
                { label: '男', value: '1' }
                ],
				DocSourceTypeList:[],
                SalesmanList:[],
				CurrList:[],
				OutStoreList:[],
				InvSourceTypeList:[],
				PaymentTypeList:[],
				PriceCondList:[],
				ShipModeList:[],
				AccountList:[],
				ShipWayList:[],
				ShipPortList:[],
				MoGroupList:[],
                tableData: [
                { id: 10001, name: 'Test1', role: 'Develop', sex: 'Man', age: 28, address: 'test abc',sex:'1' },
                { id: 10002, name: 'Test2', role: 'Test', sex: 'Women', age: 22, address: 'Guangzhou',sex:'0' },
                { id: 10003, name: 'Test3', role: 'PM', sex: 'Man', age: 32, address: 'Shanghai',sex:'1' },
                { id: 10004, name: 'Test4', role: 'Designer', sex: 'Women', age: 23, address: 'test abc',sex:'0' },
                { id: 10005, name: 'Test5', role: 'Develop', sex: 'Women', age: 30, address: 'Shanghai',sex:'0' },
                { id: 10006, name: 'Test6', role: 'Designer', sex: 'Women', age: 21, address: 'test abc',sex:'1' },
                { id: 10007, name: 'Test7', role: 'Test', sex: 'Man', age: 29, address: 'test abc',sex:'1' },
                { id: 10008, name: 'Test8', role: 'Develop', sex: 'Man', age: 35, address: 'test abc',sex:'1' }
              
				],
                name: '',
                sex: 1,
                date:'',
        }
    },
    created() {
		// this.getSalesman();
        // this.getCurr();
		this.getComboxList("DocSourceTypeList");
		this.getComboxList("SalesmanList");
		this.getComboxList("CurrList");
		this.getComboxList("OutStoreList");
		this.getComboxList("InvSourceTypeList");
		this.getComboxList("PaymentTypeList");
		this.getComboxList("PriceCondList");
		this.getComboxList("ShipModeList");
		this.getComboxList("AccountList");
		this.getComboxList("ShipWayList");
		this.getComboxList("ShipPortList");
		this.getComboxList("MoGroupList");
    },
    methods: {
		showMsg() {
                    //if (this.name.length >= 9)
                    alert("ok");
                    //var vl = value;
                },
			showRowValue(value){
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
                addNew() {
                    this.edit_mode = 1;
                    nowDateTime = comm.getCurrentTime();
                    this.formData = {
                        EditFlag:1,
						ID:'',
                        mo_id: '',
                        oi_date: comm.getCurrentDate(),
                        Ver: 0,
						separate:'',
						Shop_no:'',
                        it_customer: '',
                        phone: '',
                        fax: '',
                        payment_date: '',
                        linkman: '',
                        l_phone: '',
                        department_id: '',
                        email: '',
                        PlanRemark: '',
                        ApprovedTime: nowDateTime,
                        ApprovedUser: '',
                        CreateUser: '',
                        CreateTime: nowDateTime,
                        AmendUser: '',
                        AmendTime: nowDateTime,
                        ArtImageUrl: '/Images/photo.png',
                    };
                    //深度複製一個對象，用來判斷數據是否有修改
                    this.prevForm = JSON.parse(JSON.stringify(this.formData));
                },
                productMoBlurEvent() {
                    if (this.formData.ProductMo == "")
                        return;
                    if (this.edit_mode == 1)
                        this.getPlanFromOrder();
                },
                searchByProductMo(_ProductMo) {
                    this.edit_mode = 0,
                    this.loading3 = true;
                    setTimeout(() => {
                        this.getPlanHead(_ProductMo);
                        this.getPlanDetails(_ProductMo);
                        this.loading3 = false;
                    }, 500);
                    
                },
                getPlanHead(_ProductMo) {
                    var _self = this;
                    //axios.get("GetGoodsDetails", { params: { goods_id: _id, } })//也可以將參數寫在這裡
                    axios.get("GetPlanHeadByMo", { params: { ProductMo: _ProductMo } }).then(
                    (response) => {
                        //this.formData.ProductMo = response.data.ProductMo,
                        //    this.formData.Ver = response.data.Ver,
                        //    this.formData.PlanDate = response.data.PlanDate,
                        //    this.formData.CustomerID = response.data.CustomerID,
                        //    this.formData.GoodsID = response.data.GoodsID,
                        //    this.formData.OrderQty = response.data.OrderQty,
                        //    this.formData.OrderUnit = response.data.OrderUnit,
                        //    this.formData.MoRemark = response.data.MoRemark,
                        //    this.formData.PlanRemark = response.data.PlanRemark,
                        //    this.formData.ProductRemark = response.data.ProductRemark
                        this.formData = {
                            ProductMo: response.data.ProductMo,
                            Ver: response.data.Ver,
                            PlanDate: response.data.PlanDate,
                            CustomerID: response.data.CustomerID,
                            GoodsID: response.data.GoodsID,
                            OrderQty: response.data.OrderQty,
                            OrderUnit: response.data.OrderUnit,
                            MoRemark: response.data.MoRemark,
                            PlanRemark: response.data.PlanRemark,
                            ProductRemark: response.data.ProductRemark,
                            ArtImageUrl: response.data.ArtImageUrl,
                            CreateUser: response.data.CreateUser,
                            CreateTime: response.data.CreateTime,
                            AmendUser: response.data.AmendUser,
                            AmendTime: response.data.AmendTime,
                        }
                        //深度複製一個對象，用來判斷數據是否有修改
                        this.prevForm = JSON.parse(JSON.stringify(this.formData));
                        //var ImagePath = "/art/artwork/" + "AAAA/A888020.bmp";
                        //this.ArtImageUrl = ImagePath;
                    },
                    (response) => {
                        alert(response.status);
                    }
                ).catch(function (response) {
                    alert(response);
                });
                },
                getPlanDetails(_ProductMo) {
                    var _self = this;
                    axios.get("GetPlanDetailsByMo", { params: { ProductMo: _ProductMo } }).then(
                    (response) => {
                        this.tablePlanDetails = response.data;
                    },
                    (response) => {
                        alert(response.status);
                    }
                ).catch(function (response) {
                    alert(response);
                });
                },
                getPlanFromOrder() {
                    var _self = this;
                    //axios.get("GetGoodsDetails", { params: { goods_id: _id, } })//也可以將參數寫在這裡
                    axios.get("GetOrderByMo", { params: { ProductMo: _self.formData.ProductMo } }).then(
                    (response) => {
                        this.formData.ProductMo = response.data[0].ProductMo,
                        this.formData.CustomerID = response.data[0].CustomerID,
                        this.formData.GoodsID = response.data[0].GoodsID,
                        this.formData.OrderQty = response.data[0].OrderQty,
                        this.formData.OrderUnit = response.data[0].OrderUnit,
                        this.formData.ProductRemark = response.data[0].ProductRemark,
                        this.formData.ArtImageUrl = response.data[0].ArtImageUrl,
                        this.tablePlanDetails = response.data
                    },
                    (response) => {
                        alert(response.status);
                    }
                ).catch(function (response) {
                    alert(response);
                });
                },
                submitSearch() {

                },
                showInsertEvent () {
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
                    },
                   this.selectRow = null
                   this.showEdit = true

                },
				getComboxList(SourceType) {
                    var _self = this;///Base/BaseData///, { params: { ProductMo: this.editPlanDetails.GoodsID } }
                    axios.get("GetComboxList?SourceType="+SourceType).then(
                        (response) => {
							if(SourceType=="DocSourceTypeList")
								this.DocSourceTypeList = response.data;
							else if(SourceType=="SalesmanList")
								this.SalesmanList = response.data;
							else if(SourceType=="CurrList")
								this.CurrList = response.data;
							else if(SourceType=="OutStoreList")
								this.OutStoreList = response.data;
							else if(SourceType=="InvSourceTypeList")
								this.InvSourceTypeList = response.data;
							else if(SourceType=="PaymentTypeList")
								this.PaymentTypeList = response.data;
							else if(SourceType=="PriceCondList")
								this.PriceCondList = response.data;
							else if(SourceType=="ShipModeList")
								this.ShipModeList = response.data;
							else if(SourceType=="AccountList")
								this.AccountList = response.data;
							else if(SourceType=="ShipWayList")
								this.ShipWayList = response.data;
							else if(SourceType=="ShipPortList")
								this.ShipPortList = response.data;
							else if(SourceType=="MoGroupList")
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
                            Ver:this.formData.Ver,
                            GoodsID: '',
                            GoodsCname: '',
                            RequestQty: '',
                            RequestDate: '',
                            WipID: '',
                            NextWipID: ''
                        };
                    }
                },
                getGoodsByID(){
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
                        RequestDate:row.RequestDate,
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


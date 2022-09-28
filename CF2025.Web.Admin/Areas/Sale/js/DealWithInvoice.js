
var Main = {
	
    data() {
        return {
            tableDetails: [],
			headerCellStyle: { background: '#F5F7FA', color: '#606266', height: '25px', padding: '2px' },
            loading3: false,
			showSent:false,
			showReceipt:false,
            formData:{
				it_customer_from:'',
				it_customer_to:'',
				oi_date_from: comm.getCurrentDate(),
				oi_date_to: comm.getCurrentDate(),
				sd_date_from:'',
				sd_date_to:'',
				ID: '',
				mo_id: '',
				state:'',
			},
			issues_state:'',
			return_state:'('+comm.getCurrentDate()+')',
			modelData:{
				
			},
			issues_state_list:[],
			set_state_list:[],
			select_id_list:[],
    }
    },
    created() {

		// this.getComboxList("issues_state_list");
		this.getComboxList("set_state_list");
    },
    methods: {

		showMsg() {
                    //if (this.name.length >= 9)
                    alert("ok");
                    //var vl = value;
                },

		async getComboxList(SourceType) {
            var _self = this;///Base/BaseData///, { params: { ProductMo: this.editDetails.GoodsID } }
			var url="/Base/DataComboxList/GetComboxList";

			url+="?SourceType="+SourceType;
            await axios.get(url).then(
                (response) => {
					if(SourceType=="issues_state_list")
						this.issues_state_list = response.data;
					else if(SourceType=="set_state_list")
						this.set_state_list = response.data;
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },
		async findInvoice (search_type) {
			try {
				this.loading3 = true;
				let res = await axios.get('FindInvoice', {
				params: {search_type:search_type
					,it_customer_from:this.formData.it_customer_from,it_customer_to:this.formData.it_customer_to
					,oi_date_from:this.formData.oi_date_from,oi_date_to:this.formData.oi_date_to
					,sd_date_from:this.formData.sd_date_from,sd_date_to:this.formData.sd_date_to
					,ID:this.formData.ID,mo_id:this.formData.mo_id,state:this.formData.state}
				})
				this.loading3 = false;
				this.tableDetails=res.data;
			} catch (err) {
				this.loading3 = false;
				console.log(err)
				alert('请求出错！')
			}
		},
		showModalSent(){
			const $table = this.$refs.xTable;
            const selectRecords = $table.getCheckboxRecords();
			if(selectRecords.length==0)
			{
				VXETable.modal.alert(`沒有選擇数据!`);
				return;
			}
			
			if(this.issues_state_list.length===0)
			{
				this.getComboxList("issues_state_list");
			}
			this.showSent=true;
		},
		showModalReceipt(){
			const $table = this.$refs.xTable;
            const selectRecords = $table.getCheckboxRecords();
			if(selectRecords.length==0)
			{
				VXETable.modal.alert(`沒有選擇数据!`);
				return;
			}
			
			if(this.issues_state_list.length===0)
			{
				this.getComboxList("issues_state_list");
			}
			this.showReceipt=true;
		},
		//發貨確認
		confirmSent() {
			if(this.issues_state==="")
			{
				this.$XModal.alert("發貨狀態不能為空!");
				return;
			}
			const selectRecords = this.$refs.xTable.getCheckboxRecords();
			for (var i = 0; i < selectRecords.length; i++) {
				this.select_id_list.push(selectRecords[i]);
			}
			let issues_state=this.issues_state;
            let InvoiceModel = this.select_id_list;
			// this.$XModal.alert(issues_state);
            axios.post("ConfirmSent", { InvoiceModel,issues_state }).then(
            (response) => {
				if(response.data.Status=="0")
				{
					this.findInvoice('Sent');
					this.$refs.xModalSent.close();
				}
				else
					this.$XModal.alert(response.data.Msg +"\r\n"+"發票編號："+ response.data.ReturnValue);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
        },
		//簽收設定
		confirmReceipt(conf_flag) {
			if(this.return_state==="")
			{
				this.$XModal.alert("回單狀態不能為空!");
				return;
			}
			const selectRecords = this.$refs.xTable.getCheckboxRecords();
			for (var i = 0; i < selectRecords.length; i++) {
				this.select_id_list.push(selectRecords[i]);
			}
			let return_state=this.return_state;
            let InvoiceModel = this.select_id_list;
            axios.post("ConfirmReceipt", { InvoiceModel,conf_flag,return_state }).then(
            (response) => {
				if(response.data.Status=="0")
				{
					this.findInvoice('Receipt');
					this.$refs.xModalSent.close();
				}
				else
					this.$XModal.alert(response.data.Msg +"\r\n"+"發票編號："+ response.data.ReturnValue);
            },
            (response) => {
                alert(response.status);
            }
			).catch(function (response) {
				alert(response);
			});
        }
    }


};
 
var app = new Vue(Main).$mount('#app');
// 给 vue 实例挂载内部对象，例如
Vue.prototype.$XModal = VXETable.modal;

// 给 vue 实例挂载内部对象，例如
// Vue.prototype.$XModal = VXETable.modal;
// Vue.prototype.$XPrint = VXETable.print;
// Vue.prototype.$XSaveFile = VXETable.saveFile;
// Vue.prototype.$XReadFile = VXETable.readFile;


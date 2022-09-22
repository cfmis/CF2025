
var Main = {
	
    data() {
        return {
            formData:{
				UserName:'',
				Pwd:'',
				ConfPwd: '',
			},
    }
    },
    created() {

		// this.getComboxList("issues_state_list");
		this.getComboxList("set_state_list");
    },
    methods: {

		//確認
		ModifyPwd() {
			if(this.formData.UserName==="")
			{
				this.$XModal.alert("帳號不能為空!");
				return;
			}
			if(this.formData.Pwd==="")
			{
				this.$XModal.alert("密碼不能為空!");
				return;
			}
			if(this.formData.ConfPwd==="")
			{
				this.$XModal.alert("確認密碼不能為空!");
				return;
			}
			if(this.formData.Pwd !=this.formData.ConfPwd)
			{
				this.$XModal.alert("兩次密碼必須要相同!");
				return;
			}
			// this.$XModal.alert(issues_state);
			var UserName=this.formData.UserName;
			var Pwd=this.formData.Pwd;
            axios.post("ModifyPwd", { UserName,Pwd }).then(
            (response) => {
				if(response.data.Status=="0")
				{
					this.$XModal.alert(response.data.Msg);
				}
				else
					this.$XModal.alert(response.data.Msg);
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


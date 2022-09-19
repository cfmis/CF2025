var pwdConfirm = {
    data() {
        return {
            isPass: true,           
            loginData:{ user_id:'',user_name:'',password:'' }
        }
    },
    created() {        
        this.loginData.user_id = document.getElementById("user_id").value;
    },
    methods: {
        getUserName: async function(user_id) {
            if (user_id === "") {
                this.loginData.user_name = "";
                return;
            }
            await axios.get("/Base/Common/GetUserName?user_id=" + user_id).then(
                (response) => {
                    if(response.data===""){                        
                        this.$XModal.message({ content: '此用戶不存在!', status: 'warning', mask: false });                        
                    }
                    this.$set(this.loginData,'user_name',response.data);
                }                
            ).catch(function (response) {
                alert(response);
            });
           
        },   
        //檢查用戶與密碼是否正確
        confirmEvent:async function() {
            if (this.loginData.user_id === "") {
                this.$XModal.alert({ content: '用戶不可以為空!',status: 'info', mask: false });
                return;
            }
            await axios.get("/Base/Common/GetUserInfo?user_id=" + this.loginData.user_id + "&password=" + this.loginData.password).then(
                (response) => {
                    if (response.data === "USER_ID_ERROR") {
                        this.isPass = false;
                        this.$XModal.alert({ content: '輸入用戶有誤!', status: 'warning', mask: false });
                        return;
                    }
                    if (response.data === "PASSWORD_ERROR") {
                        this.isPass = false;
                        this.$XModal.alert({ content: '輸入的密碼有誤!', status: 'warning', mask: false });
                        return;
                    }
                    this.isPass = true;
                }               
            ).catch(function (response) {
                this.isPass = false;
                alert(response);
            });
            this.setParentFind();
        },     
       
        cancelEvent() {
            this.isPass = false;
            this.setParentFind();
        },        
       
        setParentFind(){
            parent.app.valid_user_id = this.isPass; //值賦給父窗口中的變量
            parent.comm.closeWindow();//关闭窗口
        },
  } //end methods
    
}
var app = new Vue(pwdConfirm).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;
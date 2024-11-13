var comm= {
    openWindos:function(id){
        var url = "/Base/PublicQuery?window_id=" + id;//對應Controller
        this.showMessageDialog(url, "查詢", 800, 600, true); //1024 600
    }, 
    /*查找貨品編號窗口*
    * Allen
    */
    openFindItem:function(id){
        var url = "/Base/PublicItemQuery" ;
        this.showMessageDialog(url, "查詢", 800, 600, true); //1024 600
    }, 
    /*密碼確認窗口*
    * Allen
    */
    openPassword: function(user_id){
        var url = "/Base/Common/PasswordConfirm?user_id=" + user_id;
        this.showMessageDialog(url, "密碼確認", 500, 300, true);
    }, 
    showMessageDialog: function(url, title, width, height, shadow){ 
        var content = '<iframe src="' + url + '" width="100%" height="99%" frameborder="0" scrolling="no"></iframe>';       
        //content += '<a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-cancel" style="width:80px" onclick="javascript:$(' + '\'#msgwindow\'' + ').dialog(' + '\'close\'' + ')">' + '关闭' + '</a>';
        var boarddiv = '<div id="msgwindow" title="' + title + '"></div>'//style="overflow:hidden;"可以去掉滚动条       
        $(document.body).append(boarddiv);
        var win = $('#msgwindow').dialog({
            content: content,
            width: width,
            height: height,
            modal: shadow,
            title: title,
            onClose: function () {
                //$(this).dialog('destroy');//后面可以关闭后的事件
                //document.getElementById('btnSerach').click();
            }
        });
        win.dialog('open');
    },
    closeWindow:function(){
        $('#msgwindow').dialog('close');
    },

    /*檢查輸入日期是否正確*
    * Allen
    */
    checkDate:function(strDate){
        var r =/^(?:(?!0000)[0-9]{4}-(?:(?:0[1-9]|1[0-2])-(?:0[1-9]|1[0-9]|2[0-8])|(?:0[13-9]|1[0-2])-(?:29|30)|(?:0[13578]|1[02])-31)|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)-02-29)$/;
        if(!r.exec(strDate)){
            alert("日期格式不正確或者輸入有誤!\n\r【日期格式：YYYY-MM-DD 例如：2010-08-08,注意閏年】\n\r");
            return false;
        }
        else {
            return true;
        }
    },
  
    // 日期格式化 fmt 格式化字符如 'yyyy-MM-dd hh:mm:ss' v 可以是日期或字符串
    datetimeFormat: function(v, fmt) {
        if ((typeof v).toLocaleLowerCase() == 'string') { v = new Date(v.replace(/-/g, '/')) }
        var o = {
            'M+': v.getMonth() + 1, // 月份
            'd+': v.getDate(), // 日
            'h+': v.getHours(), // 小时
            'm+': v.getMinutes(), // 分
            's+': v.getSeconds(), // 秒
            'q+': Math.floor((v.getMonth() + 3) / 3), // 季度
            'S': v.getMilliseconds() // 毫秒
        }
        if (/(y+)/.test(fmt)) { fmt = fmt.replace(RegExp.$1, (v.getFullYear() + '').substr(4 - RegExp.$1.length)) }
        for (var k in o) {
            if (new RegExp('(' + k + ')').test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (('00' + o[k]).substr(('' + o[k]).length)))
            }
        }
        return fmt
    },
    // 获取日期的星期 v=日期字符串或日期对象 返回值 0-6对应 周日-周六
    getWeek: function(v) {
        if ((typeof v).toLocaleLowerCase() == 'string') { v = new Date(v.replace(/-/g, '/')) }
        var end = v.getDay()
        return end
    },
    // 字符串转日期 v必须为 -格式 如 2020-05
    toDateTime: function(v) {
        if ((typeof v).toLocaleLowerCase() == 'string') { v = new Date(v.replace(/-/g, '/')) }
        return v
    },
    // 用于将数组进行name匹配 这里用作export2excel.js插件的智能字段识别导出
    formatJson: function(filterVal, jsonData) {
        return jsonData.map(v => filterVal.map(j => v[j]))
    },

    /**获取当前时间**
    * Allen 
    */
    getCurrentDate:function() {
        const date = new Date();
        let year = date.getFullYear();
        let month = date.getMonth() + 1;
        let day = date.getDate();
        month = month < 10 ? "0" + month : month; //月小于10，加0
        day = day < 10 ? "0" + day : day; //day小于10，加0
        return `${year}-${month}-${day}`;
    },

    /**獲取當前日期時間**
    * Allen
    */
    getCurrentTime: function() {
        //获取当前时间并打印
        var _this = this;
        var dateTime="";
        let yy = new Date().getFullYear();
        let mm = new Date().getMonth()+1;
        let dd = new Date().getDate();
        let hh = new Date().getHours();
        let mf = new Date().getMinutes()<10 ? '0'+new Date().getMinutes() : new Date().getMinutes();
        let ss = new Date().getSeconds()<10 ? '0'+new Date().getSeconds() : new Date().getSeconds();
        dateTime = yy+'-'+mm+'-'+dd+' '+hh+':'+mf+':'+ss;
        return (dateTime)
    },

    /**返回字符格式日期(yyyy-MM-dd)**
    *date:須是日期格式對象
    *addDays:天數增加幾天,例如:0則返當前日期,2,返回前日期再加上兩天
    *參數例子:addDays=0則返當前日期; addDays=2返回前日期再加上兩天
    * Allen
    */
    getDate: function(date,addDays){
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = (addDays==0)?date.getDate():date.getDate() + addDays;
        month = month<10?'0'+ month:month;
        day = day<10?'0'+ day:day;
        return (year +'-'+ month +'-'+ day).toString();
    },
    
    /**數字轉固定長度字串,前面補0 **
    * num 傳入數值(整數),n為字串長度
    * Allen
    */
    pad: function(num, n) {
        //return Array(n>num?(n-(''+num).length+1):0).join(0)+num;
        var len = num.toString().length;
        while(len < n) {
            num = "0" + num;
            len++;
        }
        return num;
    },

    /**字串轉大寫**
    * Allen 2022/05/20
    */
    stringToUppercase: function(str) {       
        var val="";
        if(str){
            val = str.toUpperCase();
        }else{
            val="";
        }
        return val;
    },
    /**输入为全数字(整数)** 
    * Allen 2022/05/21
    */
    allNumber(obj,strField,value) {
        //obj对像为数组,例如表格当前行对象
        value = value.replace(/[^\d]/g,""); //只能输入数字
        if(value === ""){
            value = 0;
        }
        //this.amount = value  //注意这里是string，你要数字类型记得自己转一下
        //this.$set(this.selectRow, strField, value ); vue中的赋值方法
        obj[strField] = parseInt(value);
    },    
    /**输入为全数字(两位小数)** 
    * Allen 2022/05/21
    */
    allNumberDec2(obj,strField,value){
        //obj对像为数组,例如表格当前行对象
        value = '' + (value ===''?0.00:value); //保证value是字符串
        value = value 
            .replace(/[^\d.]/g, '') // 清除“数字”和“.”以外的字符 
            .replace(/\.{2,}/g, '.') // 只保留第一个. 清除多余的 
            .replace(/^\./g, '') //保证第一个为数字而不是. 
            .replace('.', '$#$') 
            .replace(/\./g, '') 
            .replace('$#$', '.') 
            .replace(/^(\-)*(\d+)\.(\d\d).*$/, '$1$2.$3'); //可输入两位小数 
            //.replace(/^(\-)*(\d+)\.(\d\d\d).*$/, '$1$2.$3'); //可输入三位小数          
        if(value === ""){
            value = '0';
        }   
        if (value.indexOf('.') < 0 && value != '') { 
            //以上已经过滤,此处控制的是如果没有小数点,首位不能为类似于 01、02的金额 
            value = parseFloat(value); 
        }
        obj[strField] = parseFloat(value);//避免有返回00.00的现象
    }, 
    /**獲取單據狀態**
    * Allen 2022/11/22
    */
    checkApproveState:async function (tableName,id){
        let result = "0";
        await axios.get("/Base/Common/CheckApproveState",{params:{table_name:tableName,id:id}}).then(
            (res)=>{
                result = res.data;
            });
        return result;
    },  
    /**檢查用戶部門權限**
    *返回值:0--無權限;1--有權限
    * Allen 2022/11/22
    */
    checkUserDeptRights:async function(user_id,dept_id){
        let result = "0";
        await axios.get("/Base/Common/CheckUserDeptRights",{params:{user_id:user_id,dept_id:dept_id}}).then(
            (res)=>{
                result = res.data;
            });
        return result;
    },
    /**檢查當前單據是否可以反批準(檢查日期)**
    *返回值: "1"--可以反批準;"0"--不可以反批準
    * Allen 2022/11/23
    */
    canApprove:async function(id,window_id){
        let result = "0";
        await axios.get("/Base/Common/CheckCanApprove?id=" + id + "&window_id=" + window_id).then(            
              (res) => {
                  result = res.data;
              })
        return result;
    },
    /**檢查移交單是否已簽收**
    *返回值: "1"--已有簽收;"0"--未簽收
    *  Allen 2022/12/07
    */
    checkSignfor:async function(id){
        let result = "1";
        await axios.get("/Base/Common/CheckSignfor", { params: { id: id }}).then(
             (res) => {
                 result = res.data ;
             })
        return result;
    },
    /**生產計劃是否已批準
    * 返回值: "0"--未批準 ;"1"--已批準;
    * Allen_Leung 2022/12/09
    */
    getPlanApproveState:async function(mo_id){
        let result = 0;
        await axios.get("/Base/Common/GetPlanApproveState", { params: { mo_id: mo_id }}).then(
             (res) => {
                 result = res.data ;
             })
        return result;
    },
    /**生產計劃Hold狀態**
    * 返回值: "0"--未Hold;"1"--已Hold;
    * Allen_Leung 2022/12/09
    */
    getPlanHoldState:async function(mo_id,goods_id,out_dept,in_dept){
        let result = 0;        
        await axios.get("/Base/Common/GetPlanHoldState", { params: { mo_id:mo_id,goods_id:goods_id,out_dept:out_dept,in_dept:in_dept} }).then(
             (res) => {
                 result = res.data ;
             })
        return result;
    },
    /**OC Hold狀態**
    * 返回值: "0"--未Hold;"1"--已Hold;
    * Allen_Leung 2022/12/09
    */
    getOcHoldState:async function(mo_id){
        let result = 0;
        await axios.get("/Base/Common/GetOcHoldState", { params: { mo_id:mo_id}}).then(
             (res) => {
                 result = res.data ;
             })
        return result;
    },
    /**
    *權限檢查
    */
    checkPermission:async function (user_id,menu_id,func_name){    
        //var user_id = $("#user_id").val();
        //user_id = document.getElementById("user_id").value;
        var isCan = await comm.checkAuthority(user_id, menu_id, func_name);
        if (isCan === "0") {
            $.messager.alert('提示信息', "注意:當前操作權限不足!", 'warning');          
            return '0';
        }
        return "1";
    },
    /**
    *檢查用戶是否有某按鈕的操作權限.
    */
    checkAuthority:async function(user_id,menu_id,func_name){    
       let result = "0";
       await axios.get("/Base/RoleAuthorityPowers/CheckAuthority?user_id=" + user_id + "&menu_id=" + menu_id+ "&func_name=" + func_name).then(            
         (res) => {
             result = res.data;
         })    
       return result;
    }, 

    //獲取當前日期時間
    getWipID: async  function() {
	var result='aaa';
        await axios.get("GetWipID").then(
            (response) => {
				// return 'abc';
                // return(response.data);				
				result=response.data;
				var dd='111';
				return(result);
            },
            (response) => {
                alert(response.status);
            }
        ).catch(function (response) {
            alert(response);
        });
	    return result;
    }


}




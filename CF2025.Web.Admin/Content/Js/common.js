var comm= {
    openWindos(id){
        var url = "/Base/PublicQuery?window_id=" + id;
        this.showMessageDialog(url, "查詢", 1024, 600, true); 
    }, 
    showMessageDialog:function(url, title, width, height, shadow){        
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
   //檢查輸入日期是否正確
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
  // 获取当前时间
  getCurrentDate:function() {
      const date = new Date();
      let year = date.getFullYear();
      let month = date.getMonth() + 1;
      let day = date.getDate();
      month = month < 10 ? "0" + month : month; //月小于10，加0
      day = day < 10 ? "0" + day : day; //day小于10，加0
      return `${year}-${month}-${day}`;
  },
  //獲取當前日期時間
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

  //返回字符格式日期(yyyy-MM-dd)
  //date:須是日期格式對象
  //addDays:天數增加幾天,例如:0則返當前日期,2,返回前日期再加上兩天
  //參數例子:addDays=0則返當前日期; addDays=2返回前日期再加上兩天
  getDate: function(date,addDays){
      var year = date.getFullYear();
      var month = date.getMonth() + 1;
      var day = (addDays==0)?date.getDate():date.getDate() + addDays;
      month = month<10?'0'+ month:month;
      day = day<10?'0'+ day:day;
      return (year +'-'+ month +'-'+ day).toString();
  },
    
  //數字轉固定長度字串,前面補0
  //num 傳入數值(整數),n為字串長度
  pad: function(num, n) {
       //return Array(n>num?(n-(''+num).length+1):0).join(0)+num;
       var len = num.toString().length;
       while(len < n) {
           num = "0" + num;
           len++;
       }
       return num;
  },
  
  //獲取當前日期時間
  getWipID: async  function() {
	var result='aaa';
        await axios.get("GetWipID").then(
            (response) => {
				// return 'abc';
                // return(response.data);
				debugger;
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




var comm= {
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




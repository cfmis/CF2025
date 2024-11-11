$(function () {
    //插入拓展按钮
    var buttons = $.extend([], $.fn.datebox.defaults.buttons);
    buttons.splice(1, 0, {
        text: '清空',   //按钮名称
        handler: function (target) {
            //alert('确定');
            //$('#txtArrangeDate_from').datebox('setValue', "");
            //target.datebox('setValue', "");
            $(target).datebox('setValue', '').datebox('hidePanel');
        }
    });
    $('.easyui-datebox-expand').datebox({             //将两个输入框，执行日期输入框方法
        panelWidth: 200,
        panelHeight: 253,
        //sharedCalendar: '#easyui-sc',        //将日历控件指向id为sc的元素
        //formatter: changeDateFormatter,//這個也可以,調用自定義函數,函數在publicfunc.js中
        formatter: function (date) {
            var y = date.getFullYear();
            var m = date.getMonth() + 1;
            var d = date.getDate();
            return y + '/' + (m < 10 ? ('0' + m) : m) + '/' + (d < 10 ? ('0' + d) : d);
        },
        //parser: changeDateParser,//這個也可以,調用自定義函數,函數在publicfunc.js中
        parser: function (s) {
            if (!s) return new Date();
            var ss = (s.split('/'));
            var y = parseInt(ss[0], 10);
            var m = parseInt(ss[1], 10);
            var d = parseInt(ss[2], 10);
            if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
                return new Date(y, m - 1, d);
            } else {
                return new Date();
            }
        },
        editable: false,
        buttons: buttons
    });
    $('#easyui-sc-expand').calendar({             //将id为sc元素执行日历方法
        firstDay: 1
    })
})
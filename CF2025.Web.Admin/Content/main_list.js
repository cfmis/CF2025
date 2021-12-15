var Main = {
    data() {
            return {
              collapseStatus3: true,
              formData3: {
                name: '',
                nickname: '',
                sex: '',
                age: 30,
                status: '1',
                weight: null,
                date: null,
                active: false,
                single: '1',
                flagList: []
              },
              formData4: {
                name: '',
                nickname: '',
                sex: '0',
                role: '',
                age: 22,
                val1: [],
                val2: false,
                val3: '',
                flag: false
              }
            }
          },
          methods: {
            searchEvent () {
              this.$XModal.message({ message: '查询事件', status: 'info' })
            },
            resetEvent () {
              this.$XModal.message({ message: '重置事件', status: 'info' })
            }
		  }
};
 
var app = new Vue(Main).$mount('#app');
// 给 vue 实例挂载内部对象，例如
Vue.prototype.$XModal = VXETable.modal;
Vue.prototype.$XPrint = VXETable.print;
Vue.prototype.$XSaveFile = VXETable.saveFile;
Vue.prototype.$XReadFile = VXETable.readFile;
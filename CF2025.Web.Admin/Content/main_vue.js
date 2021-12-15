var Main = {
    data() {
        return {
            tableData: [],
            loading2: false,
            formData2: {
                name: '',
                nickname: 'Testing',
                sex: '',
                age: 26,
                date: null,
                address: null
            },
            formRules2: {
                name: [
                    {required: true, message: '请輸入名稱'},
                    {min: 3, max: 5, message: '长度在 3 到 5 个字符'}
                ],
                nickname: [
                    {required: true, message: '请输入昵称'}
                ],
                sex: [
                    {required: true, message: '请选择性别1'}
                ],
				address: [
                    {required: true, message: '请选择地址'},
					{min: 3, max: 5, message: '长度在 3 到 5 个字符'}
                ]
            },
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
        }
    },
    created() {
 
    },
    methods: {
 
    }
};
 
var app = new Vue(Main).$mount('#app');

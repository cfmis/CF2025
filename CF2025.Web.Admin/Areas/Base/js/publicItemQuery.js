/**
 * 貨品編碼基本資料查詢
 */
var ItemQuery = {
    data() {
        return {           
            headerCellStyle:{background:'#F5F7FA',color:'#606266',height:'25px',padding:'2px'},
            curRow:null,
            curResultRow:null,
            formData: { type: '0001', blueprint_id: '', goods_id: '', goods_name: '', modality: '0', big_class: '', base_class: '', small_class: '', datum: '', size_id: '', results:1000 },
            tableResult: [],
            typeList: [{ label: '產品編碼規則 (0001)', value: '0001' }, { label: 'F0成品編碼 (0002)', value: '0002' }, { label: '採購雜項分類 (0003)', value: '0003' }],
            bigClassList: [{ label: '', value: '' }],
            baseClassList: [{ label: '', value: '' }],
            smallClassList: [{ label: '', value: '' }],
            datumList:[{ label: '', value: '' }]
            
        }
    },
    created() {        
        this.getComboxList("BigClass");//大類
        this.getComboxList("BaseClass");//中類
        this.getComboxList("SmallClass");//小類
        this.getComboxList("Datum");//材質
    },
    
    methods: { 
        //初始化下拉列表框
        getComboxList(SourceType) {
            axios.get("/Base/DataComboxList/GetComboxList?SourceType=" + SourceType).then(
                (response) => {
                    switch (SourceType) {
                        case "BigClass":
                            this.bigClassList = response.data;
                            break;
                        case "BaseClass":
                            this.baseClassList = response.data;
                            break;
                        case "SmallClass":
                            this.smallClassList = response.data;
                            break;
                        case "Datum":
                            this.datumList = response.data;
                            break;                        
                        default:
                            break
                    }
                },
                (response) => {
                    alert(response.status);
                }
            ).catch(function (response) {
                alert(response);
            });
        },       
        cellClickEvent(row){
            this.curRow = row.data[row.$rowIndex];//row.$rowIndex是行索引
        },
        cellDBLClickEvent(row){
            this.setParentFind(this.curRow.goods_id, this.curRow.goods_name);
        },       
        okEvent(){
            if(this.curRow){
                this.setParentFind(this.curRow.goods_id, this.curRow.goods_name);
            }else{
                this.$XModal.alert({ content: '請首先在查詢結果中指定當前行!',mask: false });
            }
        },
        setParentFind(goods_id,goods_name){
            parent.app.setItem(goods_id, goods_name);//調用父窗口的函數            
            parent.comm.closeWindow();//关闭窗口
        },
        exitEvent() {
            parent.comm.closeWindow();//关闭窗口
        },
        //查詢
        searchEvent() {
            var searchParams = {
                results: this.formData.results,
                type: this.formData.type,
                blueprint_id: this.formData.blueprint_id,
                goods_id: this.formData.goods_id,
                goods_name: this.formData.goods_name,
                modality: this.formData.modality,
                datum: this.formData.datum,
                size_id: this.formData.size_id,
                big_class: this.formData.big_class,
                base_class: this.formData.base_class,
                small_class: this.formData.small_class                
            }
            axios.get("/Base/PublicItemQuery/Query", { params: searchParams }).then(
                (response) => {
                    if(response.data){
                        this.tableResult = response.data;
                    }else{
                        this.tableResult=[];
                        this.$XModal.message({ content: '沒有滿足查找條件的數據!', status: 'info' });
                    }
                }
           ).catch(function (response) {
               alert(response);
           }); 
        },           

     } //end methods
}
var app = new Vue(ItemQuery).$mount('#app');
Vue.prototype.$XModal = VXETable.modal;
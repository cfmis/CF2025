export default defineComponent({
          setup () {

            const demo4 = reactive({
              value4: '',
              tableColumn4: [
                { field: 'name', title: 'Name' },
                { field: 'role', title: 'Role' },
                { field: 'sex', title: 'Sex' }
              ],
              loading4: false,
              tableData4: [] as any[],
              tableList4: [
                { name: 'Test1', role: '前端', sex: '男' },
                { name: 'Test2', role: '后端', sex: '男' },
                { name: 'Test3', role: '测试', sex: '男' },
                { name: 'Test4', role: '设计师', sex: '女' },
                { name: 'Test5', role: '前端', sex: '男' },
                { name: 'Test6', role: '前端', sex: '男' },
                { name: 'Test7', role: '前端', sex: '男' }
              ],
              tablePage4: {
                total: 0,
                currentPage: 1,
                pageSize: 10
              }
            })

            const xDown4 = ref({} as VxePulldownInstance)

            const focusEvent4 = () => {
              const $pulldown4 = xDown4.value
              $pulldown4.showPanel()
            }

            const keyupEvent4 = () => {
              demo4.loading4 = true
              setTimeout(() => {
                demo4.loading4 = false
                if (demo4.value4) {
                  demo4.tableData4 = demo4.tableList4.filter((row) => row.name.indexOf(demo4.value4) > -1)
                } else {
                  demo4.tableData4 = demo4.tableList4.slice(0)
                }
              }, 100)
            }

            const suffixClick4 = () => {
              const $pulldown4 = xDown4.value
              $pulldown4.togglePanel()
            }

            const cellClickEvent4 = ({ row }: ItemVO) => {
              const $pulldown4 = xDown4.value
              demo4.value4 = row.name
              $pulldown4.hidePanel()
            }

            const pageChangeEvent4: VxeGridEvents.PageChange = ({ currentPage, pageSize }) => {
              demo4.tablePage4.currentPage = currentPage
              demo4.tablePage4.pageSize = pageSize
            }

            onMounted(() => {
              keyupEvent4()
            })

            return {
              debugger;
              demo4,
              xDown4,
              focusEvent4,
              keyupEvent4,
              suffixClick4,
              cellClickEvent4,
              pageChangeEvent4
              }
            };

});
		
// var app = new Vue(Main).$mount('#app');



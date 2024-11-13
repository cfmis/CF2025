namespace CF2025.Base.Contract
{
    public class BaseModel
    {
    }
    
    public class ModelQueryList
    {
        public string value { get; set; }
        public string label { get; set; }
        public string table_name { get; set; }
        public string field_type { get; set; }
        public string from_table { get; set; }
        public string table_relation { get; set; }
        public string order_by { get; set; }
        public int col_width { get; set; }
    }

    public class ModelQuerySavedList
    {
        public int id { get; set; }
        public string window_id { get; set; }
        public string field_name { get; set; }
        public string operators { get; set; }
        public string field_value { get; set; }
        public string logic { get; set; }
        public string table_name { get; set; }
        public string field_type { get; set; }
        public string sequence_id { get; set; }
        public string row_status { get; set; }
    }

    public class ModelItemQuery
    {
        public int results { get; set; }
        public string type { get; set; }
        public string blueprint_id { get; set; }
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string modality { get; set; }
        public string datum { get; set; }
        public string size_id { get; set; }
        public string big_class { get; set; }
        public string base_class { get; set; }
        public string small_class { get; set; }
    }

    public class ModelStLotNo
    {
        public string lot_no { get; set; }
        public decimal qty { get; set; }
        public decimal sec_qty { get; set; }
        public string mo_id { get; set; }
        public string vendor_name { get; set; }
        
    }

    public class PermissonModels
    {
        public string PermissionID { get; set; }
        public bool isPermission { get; set; }
    }

    public class RoleAuthorityPowersModels
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int AuthorityID { get; set; }
        public int PowersID { get; set; }
        public string Remark { get; set; }
        public string CreateBy { get; set; }
        public string CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateAt { get; set; }

        public string RoleName { get; set; }
        public string AuthorityName { get; set; }
        public string Powers { get; set; }
        public string PowersDesc { get; set; }
    }

    public class ListDataModels
    {
        public string value { get; set; }
        public string label { get; set; }

    }

}

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

}

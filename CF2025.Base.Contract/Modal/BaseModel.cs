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
}

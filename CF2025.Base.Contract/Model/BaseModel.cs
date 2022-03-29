using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Base.Contract.Model
{
    public class BaseModel
    {
    }

    //public class ModelBaseList1
    //{
    //    public string value { get; set; }
    //    public string label { get; set; }
    //}

    public class ModelQueryList
    {
        public string value { get; set; }
        public string label { get; set; }
        public string table_name { get; set; }
        public string field_type { get; set; }
        public string from_table { get; set; }
        public string table_relation { get; set; }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CF.Core.Config;

namespace CF2025.Sys.Contract
{
    public class AdminMenuConfig : ConfigFileBase
    {
        public AdminMenuConfig()
        {
        }

        public AdminMenuGroup[] AdminMenuGroups { get; set; }
    }
    public class AdminMenuGroupAll
    {
        public List<AdminMenuGroup> AdminMenuGroup { get; set; }
        public string DivId { get; set; }
    }
    public class AdminMenuGroup
    {

        public List<AdminMenu> AdminMenuArray { get; set; }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Permission { get; set; }

        public string Info { get; set; }
        public string DivId { get; set; }
        public string WebClass { get; set; }
        public string UlId { get; set; }
        public string Class { get; set; }
        public string MenuLayer { get; set; }
        public string WebId { get; set; }
    }

    public class AdminMenu
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Permission { get; set; }

        public string Info { get; set; }
        public string DivId { get; set; }
        public string WebClass { get; set; }
        public string UlId { get; set; }
        public string Class { get; set; }
        public string MenuLayer { get; set; }
        public string WebId { get; set; }
    }
    public class AdminMenuModels
    {

        public string AuthorityName { get; set; }


        public string weburl { get; set; }

    }
}

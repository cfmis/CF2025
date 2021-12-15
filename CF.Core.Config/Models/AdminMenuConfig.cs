using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CF.Core.Config
{
    public class AdminMenuConfig : ConfigFileBase
    {
        public AdminMenuConfig()
        {
        }

        public AdminMenuGroup[] AdminMenuGroups { get; set; }
    }

    public class AdminMenuGroup
    {

        public List<AdminMenu> AdminMenuArray { get; set; }
        public string Id { get; set; }

         public string Name { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Permission { get; set; }

        public string Info { get; set; }
    }

    public class AdminMenu
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Permission { get; set; }

        public string Info { get; set; }
    }
    public class AdminMenuModels
    {

        public string AuthorityName { get; set; }


        public string weburl { get; set; }

    }
}

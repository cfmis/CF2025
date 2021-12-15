using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CF.Core.Cache;
using CF.Core.Service;
using CF.Account.Contract;
//using CF.Crm.Contract;

namespace CF.Web
{
    public class ServiceContext
    {
        public static ServiceContext Current
        {
            get
            {
                //var gt = CacheHelper.GetItem<ServiceContext>("ServiceContext", () => new ServiceContext());
                return CacheHelper.GetItem<ServiceContext>("ServiceContext", () => new ServiceContext());
            }
        }

        public IAccountService AccountService
        {
            get
            {
                return ServiceHelper.CreateService<IAccountService>();
            }
        }

        //public ICmsService CmsService
        //{
        //    get
        //    {
        //        return ServiceHelper.CreateService<ICmsService>();
        //    }
        //}

        //public ICrmService CrmService
        //{
        //    get
        //    {
        //        return ServiceHelper.CreateService<ICrmService>();
        //    }
        //}

        //public IOAService OAService
        //{
        //    get
        //    {
        //        return ServiceHelper.CreateService<IOAService>();
        //    }
        //}
    }
}

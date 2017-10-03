using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUBEAccounts.Common;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        private BLL.DataKeyValue DataKeyValueDAL_BLL(DAL.DataKeyValue DataKeyValueFrom)
        {
            BLL.DataKeyValue DataKeyValueTo = DataKeyValueFrom.toCopy<BLL.DataKeyValue>(new BLL.DataKeyValue());
            return DataKeyValueTo;
        }

        public List<BLL.DataKeyValue> DataKeyValue_List()
        {

            var l1 = DB.DataKeyValues.Where(x => x.FundMasterId == Caller.FundMasterId).ToList()
                             .Select(x => DataKeyValueDAL_BLL(x)).ToList();
            return l1;
        }
    }
}
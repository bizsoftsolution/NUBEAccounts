using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        public List<BLL.PayeeList> PayeeList()
        {
            List<BLL.PayeeList> pl = new List<BLL.PayeeList>();
            BLL.PayeeList pl1 = new BLL.PayeeList();
             foreach(var p in DB.Payments.Select(x => x.PayTo).Distinct().ToList())
            {
                pl1 = new BLL.PayeeList();
                pl1.PayName = p;
                pl.Add(pl1);
            }
            foreach (var p in DB.Receipts.Select(x => x.ReceivedFrom).Distinct().ToList())
            {
                pl1 = new BLL.PayeeList();
                pl1.PayName = p;
                pl.Add(pl1);
            }
            return pl;
        }
    }
}
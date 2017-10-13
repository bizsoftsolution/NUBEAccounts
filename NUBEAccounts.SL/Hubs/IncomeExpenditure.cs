using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        #region IncomeExpenditure

        public List<BLL.IncomeExpenditure> IncomeExpenditure_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.IncomeExpenditure> lstIncomeExpenditure = new List<BLL.IncomeExpenditure>();
            var l1 = DB.AccountGroups.Where(x => x.FundMasterId == Caller.FundMasterId && (x.GroupName == "Income" || x.GroupName == "Expenses")).ToList();
            decimal GTotalDr = 0, GTotalCr = 0, GTotalDrOP = 0, GTotalCrOP = 0;
            foreach (var ag in l1)
            {
                decimal TotalDr = 0, TotalCr = 0, TotalDrOP = 0, TotalCrOP = 0;
                lstIncomeExpenditure.AddRange(IncomeExpenditureByGroupName(ag, dtFrom, dtTo, "", ref TotalDr, ref TotalCr, ref TotalDrOP, ref TotalCrOP));
                GTotalDr += TotalDr;
                GTotalCr += TotalCr;
                GTotalDrOP += TotalDrOP;
                GTotalCrOP += TotalCrOP;

            }

            BLL.IncomeExpenditure tb = new BLL.IncomeExpenditure();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "Total ";
            tb.CrAmt = GTotalCr;
            tb.DrAmt = GTotalDr;
            tb.CrAmtOP = GTotalCrOP;
            tb.DrAmtOP = GTotalDrOP;
            lstIncomeExpenditure.Add(tb);



            tb = new BLL.IncomeExpenditure();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = "Surplus/Deficit ";
            tb.CrAmt = GTotalCr > GTotalDr ? Math.Abs(GTotalDr - GTotalCr) : 0;
            tb.DrAmt = GTotalCr < GTotalDr ? Math.Abs(GTotalDr - GTotalCr) : 0; ;
            tb.CrAmtOP = GTotalCrOP > GTotalDrOP ? Math.Abs(GTotalDrOP - GTotalCrOP) : 0;
            tb.DrAmtOP = GTotalCrOP < GTotalDrOP ? Math.Abs(GTotalDrOP - GTotalCrOP) : 0;
            lstIncomeExpenditure.Add(tb);


            return lstIncomeExpenditure;
        }

        List<BLL.IncomeExpenditure> IncomeExpenditureByGroupName(DAL.AccountGroup ag, DateTime dtFrom, DateTime dtTo, string Prefix, ref decimal TotalDr, ref decimal TotalCr, ref decimal TotalDrOP, ref decimal TotalCrOP)
        {
            decimal GTotalDr = 0, GTotalCr = 0, GTotalDrOP = 0, GTotalCrOP = 0;

            List<BLL.IncomeExpenditure> lstIncomeExpenditure = new List<BLL.IncomeExpenditure>();
            decimal total = 0;
            GetLedgerTotal(ag, ref total);
            if (total == 0) return lstIncomeExpenditure;

            BLL.IncomeExpenditure tb = new BLL.IncomeExpenditure();

            tb = new BLL.IncomeExpenditure();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = Prefix + ag.GroupName;
            tb.CrAmt = null;
            tb.DrAmt = null;
            tb.CrAmtOP = null;
            tb.DrAmtOP = null;

            lstIncomeExpenditure.Add(tb);


            foreach (var uag in ag.AccountGroup1)
            {
                lstIncomeExpenditure.AddRange(IncomeExpenditureByGroupName(uag, dtFrom, dtTo, Prefix + "     ", ref GTotalDr, ref GTotalCr, ref GTotalDrOP, ref GTotalCrOP));
            }

            decimal OPDr = 0, OPCr = 0, Dr = 0, Cr = 0;

            foreach (var l in ag.Ledgers)
            {
                tb = new BLL.IncomeExpenditure();
                tb.Ledger = LedgerDAL_BLL(l);

                LedgerBalance(l, dtFrom, dtTo, ref OPDr, ref OPCr, ref Dr, ref Cr);

                tb.DrAmt = Dr-OPDr;
                tb.CrAmt = Cr-OPCr;
                tb.DrAmtOP = OPDr;
                tb.CrAmtOP = OPCr;

                if (tb.DrAmtOP!=0||tb.CrAmtOP!=0)
                {
                    tb.Ledger.AccountGroup.GroupCode = Prefix + "     " + tb.Ledger.AccountGroup.GroupCode;
                    lstIncomeExpenditure.Add(tb);
                    GTotalDr += tb.DrAmt ?? 0;
                    GTotalCr += tb.CrAmt ?? 0;

                }

                GTotalDrOP += tb.DrAmtOP ?? 0;
                GTotalCrOP += tb.CrAmtOP ?? 0;
            }

            if (GTotalDr > GTotalCr)
            {
                GTotalDr = Math.Abs(GTotalDr - GTotalCr);
                GTotalCr = 0;
            }
            else
            {
                GTotalCr = Math.Abs(GTotalDr - GTotalCr);
                GTotalDr = 0;
            }

            if (GTotalDrOP > GTotalCrOP)
            {
                GTotalDrOP = Math.Abs(GTotalDrOP - GTotalCrOP);
                GTotalCrOP = 0;
            }
            else
            {
                GTotalCrOP = Math.Abs(GTotalDrOP - GTotalCrOP);
                GTotalDrOP = 0;
            }
            tb = new BLL.IncomeExpenditure();
            tb.Ledger = new BLL.Ledger();
            tb.Ledger.AccountName = Prefix + "Total " + ag.GroupName;
            tb.CrAmt = GTotalCr;
            tb.DrAmt = GTotalDr;
            tb.CrAmtOP = GTotalCrOP;
            tb.DrAmtOP = GTotalDrOP;


            lstIncomeExpenditure.Add(tb);

            TotalDr += GTotalDr;
            TotalCr += GTotalCr;
            TotalDrOP += GTotalDrOP;
            TotalCrOP += GTotalCrOP;

            return lstIncomeExpenditure;
        }
     
        #endregion
    }
}
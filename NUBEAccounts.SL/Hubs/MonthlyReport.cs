using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        public List<BLL.MonthlyReport> MonthlyReport_List(DateTime dtFrom, DateTime dtTo, bool isMonthly)
        {
            BLL.MonthlyReport sr = new BLL.MonthlyReport();

            List<BLL.MonthlyReport> rv = new List<BLL.MonthlyReport>();
            var l1 = DB.AccountGroups.Where(x => x.FundMasterId == Caller.FundMasterId && (x.GroupName == "Income" || x.GroupName == "Expenses")).ToList();
            foreach (var ag in l1)
            {
                sr = new BLL.MonthlyReport();
                sr.Description = ag.GroupName;
                rv.Add(sr);

                rv.AddRange(IncomeExpenditureByGroupName(ag, dtFrom, dtTo, "", isMonthly));
                foreach(var uag in ag.AccountGroup1)
                {
                    rv.AddRange(IncomeExpenditureByGroupName(uag, dtFrom, dtTo, "", isMonthly));
                }
            }
           
           
            return rv;

        }
        List<BLL.MonthlyReport> IncomeExpenditureByGroupName(DAL.AccountGroup ag, DateTime dtFrom, DateTime dtTo, string Prefix, bool isMonthly)
        {
            int n = Math.Abs((dtTo.Year * 12 + (dtTo.Month - 1)) - (dtFrom.Year * 12 + (dtFrom.Month - 1)));
            if (isMonthly == false) n = n / 2;
            if (n > 12) n = 12;


            List<BLL.MonthlyReport> rv = new List<BLL.MonthlyReport>();
            BLL.MonthlyReport sr = new BLL.MonthlyReport();
            decimal[] tamt = new decimal[12];


            #region CustomeWise

            decimal[] gtamt = new decimal[12];

            foreach (var l in ag.Ledgers)
            {
                decimal[] amt = new decimal[12];

                for (int i = 0; i <= n; i++)
                {
                    decimal dr = 0, cr = 0;
                   
                    amt[i] = isMonthly == true ? 0 : 1;
                    if (isMonthly == true)
                    {
                        DateTime dt = dtFrom.AddMonths(i);

                        dr = l.PaymentDetails.Where(x => x.Payment.PaymentDate.Year == dt.Year && x.Payment.PaymentDate.Month == dt.Month).Sum(x => x.Amount);
                        cr= l.Payments.Where(x => x.PaymentDate.Year == dt.Year && x.PaymentDate.Month == dt.Month).Sum(x => x.Amount);

                        dr += l.Receipts.Where(x => x.ReceiptDate.Year == dt.Year && x.ReceiptDate.Month == dt.Month).Sum(x => x.Amount);
                        cr += l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate.Year == dt.Year && x.Receipt.ReceiptDate.Month == dt.Month).Sum(x => x.Amount);

                        cr += l.JournalDetails.Where(x => x.Journal.JournalDate.Year == dt.Year && x.Journal.JournalDate.Month == dt.Month).Sum(x => x.CrAmt);
                        dr += l.JournalDetails.Where(x => x.Journal.JournalDate.Year == dt.Year && x.Journal.JournalDate.Month == dt.Month).Sum(x => x.DrAmt);
                        amt[i] = Math.Abs(dr - cr);
                    }
                    else
                    {
                        DateTime dt = dtFrom.AddYears(i);
                        dr = l.PaymentDetails.Where(x => x.Payment.PaymentDate.Year == dt.Year).Sum(x => x.Amount);
                        cr = l.Payments.Where(x => x.PaymentDate.Year == dt.Year).Sum(x => x.Amount);

                        dr += l.Receipts.Where(x => x.ReceiptDate.Year == dt.Year).Sum(x => x.Amount);
                        cr += l.ReceiptDetails.Where(x => x.Receipt.ReceiptDate.Year == dt.Year).Sum(x => x.Amount);

                        dr += l.JournalDetails.Where(x => x.Journal.JournalDate.Year == dt.Year).Sum(x => x.DrAmt);
                        cr += l.JournalDetails.Where(x => x.Journal.JournalDate.Year == dt.Year).Sum(x => x.CrAmt);
                        amt[i] = Math.Abs(dr - cr);
                    }
                }
                sr = new BLL.MonthlyReport();
                sr.Amount = amt.Sum();

                if (sr.Amount > 0)
                {
                    sr.Description = string.Format("   {0}-{1}", l.LedgerCode, l.LedgerName);
                    sr.M1 = amt[0];
                    sr.M2 = amt[1];
                    sr.M3 = amt[2];
                    sr.M4 = amt[3];
                    sr.M5 = amt[4];
                    sr.M6 = amt[5];
                    sr.M7 = amt[6];
                    sr.M8 = amt[7];
                    sr.M9 = amt[8];
                    sr.M10 = amt[9];
                    sr.M11 = amt[10];
                    sr.M12 = amt[11];
                    rv.Add(sr);
                }

                for (int i = 0; i < 12; i++)
                {
                    tamt[i] += amt[i];
                }

            }
            sr = new BLL.MonthlyReport();
            sr.Amount = tamt.Sum();



            for (int i = 0; i < 12; i++)
            {
                gtamt[i] += tamt[i];
            }

            sr = new BLL.MonthlyReport();
            sr.Amount = gtamt.Sum();

            if (sr.Amount != 0)
            {
                sr.Description = string.Format("Grand Total");
                sr.M1 = gtamt[0];
                sr.M2 = gtamt[1];
                sr.M3 = gtamt[2];
                sr.M4 = gtamt[3];
                sr.M5 = gtamt[4];
                sr.M6 = gtamt[5];
                sr.M7 = gtamt[6];
                sr.M8 = gtamt[7];
                sr.M9 = gtamt[8];
                sr.M10 = gtamt[9];
                sr.M11 = gtamt[10];
                sr.M12 = gtamt[11];
                rv.Add(sr);

               // rv.Add(new BLL.MonthlyReport());
            }
            #endregion

            return rv;
        }

    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        #region Subsidary Accounts

        public List<BLL.SubsidaryAccounts> SubsidaryAccounts_List(DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.SubsidaryAccounts> lstSubsidaryAccounts = new List<BLL.SubsidaryAccounts>();
            var l1 = DB.AccountGroups.Where(x => x.CompanyId == Caller.CompanyId && (x.GroupName == "Other Current Asset" || x.GroupName == "Other Current Liability" )).ToList();
            decimal GTotalDr = 0, GTotalCr = 0, GTotalDrOP = 0, GTotalCrOP = 0;
            foreach (var ag in l1)
            {
                decimal TotalDr = 0, TotalCr = 0, TotalDrOP = 0, TotalCrOP = 0;
                lstSubsidaryAccounts.AddRange(SubsidaryAccountsByGroupName(ag, dtFrom, dtTo, "", ref TotalDr, ref TotalCr, ref TotalDrOP, ref TotalCrOP));

                GTotalDr += TotalDr;
                GTotalCr += TotalCr;
                GTotalDrOP += TotalDrOP;
                GTotalCrOP += TotalCrOP;

            }


            BLL.SubsidaryAccounts tb = new BLL.SubsidaryAccounts();
            tb.LedgerList = new BLL.Ledger();
            tb.LedgerList.AccountName = "Total ";
            tb.CrAmt = GTotalCr;
            tb.DrAmt = GTotalDr;
            tb.CrAmtOP = GTotalCrOP;
            tb.DrAmtOP = GTotalDrOP;
            lstSubsidaryAccounts.Add(tb);


            return lstSubsidaryAccounts;
        }

        List<BLL.SubsidaryAccounts> SubsidaryAccountsByGroupName(DAL.AccountGroup ag, DateTime dtFrom, DateTime dtTo, string Prefix, ref decimal TotalDr, ref decimal TotalCr, ref decimal TotalDrOP, ref decimal TotalCrOP)
        {
            decimal GTotalDr = 0, GTotalCr = 0, GTotalDrOP = 0, GTotalCrOP = 0;
            List<BLL.SubsidaryAccounts> lstSubsidaryAccounts = new List<BLL.SubsidaryAccounts>();
            decimal total = 0;
            GetLedgerTotal(ag, ref total);
            if (total == 0) return lstSubsidaryAccounts;

            BLL.SubsidaryAccounts tb = new BLL.SubsidaryAccounts();

            //tb = new BLL.SubsidaryAccounts();
            //tb.LedgerList = new BLL.Ledger();
            //tb.LedgerList.AccountName = Prefix + ag.GroupName;
            //tb.CrAmt = null;
            //tb.DrAmt = null;
            //tb.CrAmtOP = null;
            //tb.DrAmtOP = null;

           // lstSubsidaryAccounts.Add(tb);


            //foreach (var uag in ag.AccountGroup1)
            //{
            //    lstSubsidaryAccounts.AddRange(SubsidaryAccountsByGroupName(uag, dtFrom, dtTo, Prefix + "", ref GTotalDr, ref GTotalCr, ref GTotalDrOP, ref GTotalCrOP));
            //}

            decimal OPDr = 0, OPCr = 0, Dr = 0, Cr = 0;

            foreach (var l in ag.Ledgers)
            {
                tb = new BLL.SubsidaryAccounts();
                tb.LedgerList = LedgerDAL_BLL(l);

                LedgerBalance(l, dtFrom, dtTo, ref OPDr, ref OPCr, ref Dr, ref Cr);

                tb.DrAmt = Dr;
                tb.CrAmt = Cr;
                tb.DrAmtOP = OPDr;
                tb.CrAmtOP = OPCr;
                lstSubsidaryAccounts.Add(tb);
                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    tb.LedgerList.AccountGroup.GroupCode = Prefix + "" + tb.LedgerList.AccountGroup.GroupCode;
                    //lstSubsidaryAccounts.Add(tb);
                    GTotalDr += tb.DrAmt ?? 0;
                    GTotalCr += tb.CrAmt ?? 0;

                    GTotalDrOP += tb.DrAmtOP ?? 0;
                    GTotalCrOP += tb.CrAmtOP ?? 0;
                }


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



            //tb = new BLL.SubsidaryAccounts();
            //tb.LedgerList = new BLL.Ledger();
            //// tb.LedgerList.AccountName = Prefix + "Total " + ag.GroupName;
            //tb.LedgerList.AccountName = "Total ";
            //tb.CrAmt = GTotalCr;
            //tb.DrAmt = GTotalDr;
            //tb.CrAmtOP = GTotalCrOP;
            //tb.DrAmtOP = GTotalDrOP;


            //lstSubsidaryAccounts.Add(tb);

            TotalDr += GTotalDr;
            TotalCr += GTotalCr;
            TotalDrOP += GTotalDrOP;
            TotalCrOP += GTotalCrOP;

            return lstSubsidaryAccounts;
        }

        #endregion
    }
}
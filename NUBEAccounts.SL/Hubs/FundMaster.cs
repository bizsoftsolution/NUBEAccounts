using System;
using System.Collections.Generic;
using System.Linq;
using NUBEAccounts.Common;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub
    {
        
        #region FundMaster

        BLL.FundMaster FundMasterDAL_BLL(DAL.FundMaster d)
        {
            return d.toCopy<BLL.FundMaster>(new BLL.FundMaster());
        }

        public List<string> FundMaster_AcYearList(int FundMasterId)
        {
            return DB.ACYearMasters.Where(x=> x.FundMasterId==FundMasterId).Select(x => x.ACYear).ToList();
        }

        public List<BLL.FundMaster> FundMaster_List()
        {
            List<BLL.FundMaster> rv = new List<BLL.FundMaster>();
            try
            {
                rv= DB.FundMasters.ToList().Select(x => FundMasterDAL_BLL(x)).ToList();
            }
            catch(Exception ex)
            {

            }
            return rv;
        }

        

        public int FundMaster_Save(BLL.FundMaster fm)
        {
            try
            {
                
                DAL.FundMaster d = DB.FundMasters.Where(x => x.Id == fm.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.FundMaster();
                    DB.FundMasters.Add(d);

                    fm.IsActive = true;
                    fm.toCopy<DAL.FundMaster>(d);

                    DateTime dt = DateTime.Now;
                    DAL.ACYearMaster acym = new DAL.ACYearMaster()
                                            {
                                                ACYear = dt.Month > 3 ? string.Format("{0} - {1}", dt.Year, dt.Year + 1) : string.Format("{0} - {1}", dt.Year - 1, dt.Year),
                                                ACYearStatusId =(int)AppLib.ACYearStatus.Open
                                            };
                    d.ACYearMasters.Add(acym);

                    DB.SaveChanges();
                    fm.Id = d.Id;
                    if (d.Id != 0)
                    {
                        FundSetup(fm);
                        CurrencySetup(fm);                                                                       
                    }
                }
                else
                {
                    fm.toCopy<DAL.FundMaster>(d);
                    DB.SaveChanges();                 
                }
                Clients.Clients(OtherLoginClientsOnGroup).FundMaster_Save(fm); 

                return fm.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        private void CurrencySetup(BLL.FundMaster cm)
        {
                DAL.CustomFormat cf = new DAL.CustomFormat();
                cf.FundMasterId = cm.Id;
                cf.CurrencyPositiveSymbolPrefix = "RM";
                cf.CurrencyPositiveSymbolSuffix = "RM";
                cf.CurrencyNegativeSymbolPrefix = "RM";
                cf.CurrencyNegativeSymbolSuffix = "RM";
                cf.CurrencyToWordPrefix = "Ringgit";
                cf.CurrencyToWordSuffix= "Ringgit";
                cf.DecimalToWordPrefix = "Cent";
                cf.DecimalToWordSuffix= "Cent";
                cf.DigitGroupingBy = 2;
                cf.CurrencyCaseSensitive = 2;
                cf.DecimalSymbol = ".";
                cf.DigitGroupingSymbol = ",";
                cf.IsDisplayWithOnlyOnSuffix = true;
                cf.NoOfDigitAfterDecimal = 2;

                DB.CustomFormats.Add(cf);
                DB.SaveChanges();
            
        }

        public void FundMaster_Delete(int pk)
        {
            try
            {
                var d = DB.FundMasters.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null)
                {
                    d.IsActive = false;

                    DB.SaveChanges();
                    LogDetailStore(d.toCopy<BLL.FundMaster>(new BLL.FundMaster()), LogDetailType.DELETE);
                }
                           
                Clients.Clients(OtherLoginClientsOnGroup).FundMaster_Delete(pk);                
            }
            catch (Exception ex) { }
        }

        private void FundSetup(BLL.FundMaster fm)
        {
            UserSetup(fm);
            AccountSetup(fm);
        }

        void UserSetup(BLL.FundMaster fm)
        {
            DAL.UserAccount ua = new DAL.UserAccount();
            ua.LoginId = fm.UserId;
            ua.UserName = fm.UserId;
            ua.Password = fm.Password;

            DAL.UserType ut = new DAL.UserType();
            ut.TypeOfUser = BLL.DataKeyValue.Administrator_Key;
            ut.FundMasterId = fm.Id;
            ut.UserAccounts.Add(ua);

            foreach (var utfd in DB.UserTypeFormDetails)
            {
                DAL.UserTypeDetail utd = new DAL.UserTypeDetail();
                utd.UserTypeFormDetailId = utfd.Id;
                utd.IsViewForm = true;
                utd.AllowInsert = true;
                utd.AllowUpdate = true;
                utd.AllowDelete = true;
                ut.UserTypeDetails.Add(utd);
            }

            DB.UserTypes.Add(ut);
            DB.SaveChanges();

            SetDataKeyValue(fm.Id, ut.TypeOfUser, ut.Id);



        }

        void SetDataKeyValue(int FundMasterId, string DataKey, int DataValue)
        {
            DAL.DataKeyValue dk = new DAL.DataKeyValue();
            dk.FundMasterId = FundMasterId;
            dk.DataKey = DataKey.Trim(' ');
            dk.DataValue = DataValue;
            DB.DataKeyValues.Add(dk);
            DB.SaveChanges();
        }
        int GetDataKeyValue(int FundMasterId, string key)
        {
            return DB.DataKeyValues.Where(x => x.FundMasterId == FundMasterId && x.DataKey == key).FirstOrDefault().DataValue;
        }
        void AccountSetup(BLL.FundMaster cmp)
        {
            DAL.AccountGroup pr = new DAL.AccountGroup();
            pr.GroupName = BLL.DataKeyValue.Primary_Key;
            pr.GroupCode = string.Empty;
            pr.FundMasterId = cmp.Id;
            DB.AccountGroups.Add(pr);
            DB.SaveChanges();
            SetDataKeyValue(cmp.Id, pr.GroupName, pr.Id);


            AccountSetup_Asset(pr);
            AccountSetup_Liabilities(pr);
            AccountSetup_Income(pr);
            AccountSetup_Expense(pr);

            DAL.Ledger PL = new DAL.Ledger();
            PL.LedgerName = BLL.DataKeyValue.Profit_Loss_Ledger_Key;
            PL.AccountGroupId = pr.Id;
            DB.Ledgers.Add(PL);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, PL.LedgerName, PL.Id);



        }

        void AccountSetup_Asset(DAL.AccountGroup pr)
        {
            DAL.AccountGroup ast = new DAL.AccountGroup();
            ast.GroupName = BLL.DataKeyValue.Assets_Key;
            ast.GroupCode = "100";
            ast.FundMasterId = pr.FundMasterId;
            ast.UnderGroupId = pr.Id;
            DB.AccountGroups.Add(ast);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, ast.GroupName, ast.Id);


            #region Current Assets
            DAL.AccountGroup ca = new DAL.AccountGroup();
            ca.GroupName = BLL.DataKeyValue.CurrentAssets_Key;
            ca.GroupCode = "110";
            ca.UnderGroupId = ast.Id;
            ca.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(ca);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, ca.GroupName, ca.Id);


            DAL.AccountGroup ch = new DAL.AccountGroup();
            ch.GroupName = BLL.DataKeyValue.CashInHand_Key;
            ch.GroupCode = "111";
            ch.UnderGroupId = ca.Id;
            ch.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(ch);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, ch.GroupName, ch.Id);

            DAL.Ledger cL = new DAL.Ledger();
            cL.LedgerName = BLL.DataKeyValue.CashLedger_Key;
            cL.AccountGroupId = ch.Id;
            DB.Ledgers.Add(cL);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, cL.LedgerName, cL.Id);

            DAL.AccountGroup dp = new DAL.AccountGroup();
            dp.GroupName = BLL.DataKeyValue.Deposits_Key;
            dp.GroupCode = "112";
            dp.UnderGroupId = ca.Id;
            dp.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(dp);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, dp.GroupName, dp.Id);


            DAL.AccountGroup la = new DAL.AccountGroup();
            la.GroupName = BLL.DataKeyValue.LoansandAdvances_Key;
            la.GroupCode = "113";
            la.UnderGroupId = ca.Id;
            la.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(la);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, la.GroupName, la.Id);


            DAL.AccountGroup ba = new DAL.AccountGroup();
            ba.GroupName = BLL.DataKeyValue.BankAccounts_Key;
            ba.GroupCode = "114";
            ba.UnderGroupId = ca.Id;
            ba.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(ba);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, ba.GroupName, ba.Id);


            DAL.AccountGroup SIH = new DAL.AccountGroup();
            SIH.GroupName = BLL.DataKeyValue.StockInHand_Key;
            SIH.GroupCode = "115";
            SIH.UnderGroupId = ca.Id;
            SIH.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(SIH);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, SIH.GroupName, SIH.Id);

            DAL.Ledger st = new DAL.Ledger();
            st.LedgerName = BLL.DataKeyValue.Stock_In_Hand_Ledger_Key;
            st.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(st);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, st.LedgerName, st.Id);

            DAL.Ledger sti = new DAL.Ledger();
            sti.LedgerName = BLL.DataKeyValue.Stock_Inward_Ledger_Key;
            sti.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(sti);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, sti.LedgerName, sti.Id);

            DAL.Ledger sto = new DAL.Ledger();
            sto.LedgerName = BLL.DataKeyValue.Stock_Outward_Ledger_Key;
            sto.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(sto);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, sto.LedgerName, sto.Id);

            DAL.AccountGroup sd = new DAL.AccountGroup();
            sd.GroupName = BLL.DataKeyValue.SundryDebtors_Key;
            sd.GroupCode = "116";
            sd.UnderGroupId = ca.Id;
            sd.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(sd);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, sd.GroupName, sd.Id);


            DAL.Ledger SP = new DAL.Ledger();
            SP.LedgerName = BLL.DataKeyValue.StockInProcess_Ledger_Key;
            SP.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(SP);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, SP.LedgerName, SP.Id);

            DAL.Ledger SS = new DAL.Ledger();
            SS.LedgerName = BLL.DataKeyValue.StockSeperated_Ledger_Key;
            SS.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(SS);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, SS.LedgerName, SS.Id);



            #endregion

            #region Fixed Assets

            DAL.AccountGroup fa = new DAL.AccountGroup();
            fa.GroupName = BLL.DataKeyValue.FixedAssets_Key;
            fa.GroupCode = "120";
            fa.UnderGroupId = ast.Id;
            fa.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(fa);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, fa.GroupName, fa.Id);

            #endregion


            #region Misc. Expenses

            DAL.AccountGroup me = new DAL.AccountGroup();
            me.GroupName = BLL.DataKeyValue.MiscExpenses_Key;
            me.GroupCode = "130";
            me.UnderGroupId = ast.Id;
            me.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(me);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, me.GroupName, me.Id);

            #endregion

            DAL.AccountGroup Inv = new DAL.AccountGroup();
            Inv.GroupName = BLL.DataKeyValue.Investments_Key;
            Inv.GroupCode = "140";
            Inv.UnderGroupId = ast.Id;
            Inv.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(Inv);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, Inv.GroupName, Inv.Id);

        }

        void AccountSetup_Liabilities(DAL.AccountGroup pr)
        {
            DAL.AccountGroup liab = new DAL.AccountGroup();
            liab.GroupName = BLL.DataKeyValue.Liabilities_Key;
            liab.GroupCode = "200";
            liab.FundMasterId = pr.FundMasterId;
            liab.UnderGroupId = pr.Id;
            DB.AccountGroups.Add(liab);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, liab.GroupName, liab.Id);

            #region Current Liabilities
            DAL.AccountGroup cl = new DAL.AccountGroup();
            cl.GroupName = BLL.DataKeyValue.CurrentLiabilities_Key;
            cl.GroupCode = "210";
            cl.UnderGroupId = liab.Id;
            cl.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(cl);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, cl.GroupName, cl.Id);

            DAL.AccountGroup DT = new DAL.AccountGroup();
            DT.GroupName = BLL.DataKeyValue.DutiesTaxes_Key;
            DT.GroupCode = "211";
            DT.UnderGroupId = cl.Id;
            DT.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(DT);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, DT.GroupName, DT.Id);

            DAL.Ledger IT = new DAL.Ledger();
            IT.LedgerName = BLL.DataKeyValue.Input_Tax_Ledger_Key;
            IT.AccountGroupId = DT.Id;
            DB.Ledgers.Add(IT);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, IT.LedgerName, IT.Id);


            DAL.Ledger OT = new DAL.Ledger();
            OT.LedgerName = BLL.DataKeyValue.Output_Tax_Ledger_Key;
            OT.AccountGroupId = DT.Id;
            DB.Ledgers.Add(OT);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, OT.LedgerName, OT.Id);

            DAL.AccountGroup prov = new DAL.AccountGroup();
            prov.GroupName = BLL.DataKeyValue.Provisions_Key;
            prov.GroupCode = "212";
            prov.UnderGroupId = cl.Id;
            prov.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(prov);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, prov.GroupName, prov.Id);

            DAL.AccountGroup sc = new DAL.AccountGroup();
            sc.GroupName = BLL.DataKeyValue.SundryCreditors_Key;
            sc.GroupCode = "212";
            sc.UnderGroupId = cl.Id;
            sc.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(sc);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, sc.GroupName, sc.Id);


            #region Loans
            DAL.AccountGroup l = new DAL.AccountGroup();
            l.GroupName = BLL.DataKeyValue.Loans_Key;
            l.GroupCode = "220";
            l.UnderGroupId = liab.Id;
            l.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(l);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, l.GroupName, l.Id);


            DAL.AccountGroup BOAc = new DAL.AccountGroup();
            BOAc.GroupName = BLL.DataKeyValue.BankODAc_Key;
            BOAc.GroupCode = "221";
            BOAc.UnderGroupId = l.Id;
            BOAc.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(BOAc);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, BOAc.GroupName, BOAc.Id);

            DAL.AccountGroup SL = new DAL.AccountGroup();
            SL.GroupName = BLL.DataKeyValue.SecuredLoans_Key;
            SL.GroupCode = "221";
            SL.UnderGroupId = l.Id;
            SL.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(SL);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, SL.GroupName, SL.Id);

            DAL.AccountGroup USL = new DAL.AccountGroup();
            USL.GroupName = BLL.DataKeyValue.UnSecuredLoans_Key;
            USL.GroupCode = "222";
            USL.UnderGroupId = l.Id;
            USL.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(USL);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, USL.GroupName, USL.Id);

            #endregion


            DAL.AccountGroup BD = new DAL.AccountGroup();
            BD.GroupName = BLL.DataKeyValue.BranchDivisions_Key;
            BD.GroupCode = "230";
            BD.UnderGroupId = liab.Id;
            BD.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(BD);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, BD.GroupName, BD.Id);



            DAL.AccountGroup Cap = new DAL.AccountGroup();
            Cap.GroupName = BLL.DataKeyValue.CapitalAccount_Key;
            Cap.GroupCode = "240";
            Cap.UnderGroupId = liab.Id;
            Cap.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(Cap);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, Cap.GroupName, Cap.Id);

            DAL.AccountGroup RS = new DAL.AccountGroup();
            RS.GroupName = BLL.DataKeyValue.ReservesSurplus_Key;
            RS.GroupCode = "250";
            RS.UnderGroupId = liab.Id;
            RS.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(RS);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, RS.GroupName, RS.Id);

            DAL.AccountGroup SAC = new DAL.AccountGroup();
            SAC.GroupName = BLL.DataKeyValue.SuspenseAc_Key;
            SAC.GroupCode = "260";
            SAC.UnderGroupId = liab.Id;
            SAC.FundMasterId = pr.FundMasterId;
            DB.AccountGroups.Add(SAC);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, SAC.GroupName, SAC.Id);

            #endregion
        }

        void AccountSetup_Income(DAL.AccountGroup pr)
        {
            DAL.AccountGroup Inc = new DAL.AccountGroup();
            Inc.GroupName = BLL.DataKeyValue.Income_Key;
            Inc.GroupCode = "300";
            Inc.FundMasterId = pr.FundMasterId;
            Inc.UnderGroupId = pr.Id;
            DB.AccountGroups.Add(Inc);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, Inc.GroupName, Inc.Id);



            #region Direct Income

            DAL.AccountGroup DInc = new DAL.AccountGroup();
            DInc.GroupName = BLL.DataKeyValue.DirectIncome_Key;
            DInc.GroupCode = "310";
            DInc.FundMasterId = pr.FundMasterId;
            DInc.UnderGroupId = Inc.Id;
            DB.AccountGroups.Add(DInc);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, DInc.GroupName, DInc.Id);

            #endregion

            #region Indirect Income

            DAL.AccountGroup IndInc = new DAL.AccountGroup();
            IndInc.GroupName = BLL.DataKeyValue.IndirectIncome_Key;
            IndInc.GroupCode = "320";
            IndInc.FundMasterId = pr.FundMasterId;
            IndInc.UnderGroupId = Inc.Id;
            DB.AccountGroups.Add(IndInc);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, IndInc.GroupName, IndInc.Id);

            #endregion

            DAL.AccountGroup Sa = new DAL.AccountGroup();
            Sa.GroupName = BLL.DataKeyValue.SalesAccount_Key;
            Sa.GroupCode = "330";
            Sa.FundMasterId = pr.FundMasterId;
            Sa.UnderGroupId = Inc.Id;
            DB.AccountGroups.Add(Sa);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, Sa.GroupName, Sa.Id);

            DAL.Ledger salL = new DAL.Ledger();
            salL.LedgerName = BLL.DataKeyValue.SalesAccount_Ledger_Key;
            salL.AccountGroupId = Sa.Id;
            DB.Ledgers.Add(salL);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, salL.LedgerName, salL.Id);

            DAL.Ledger SRL = new DAL.Ledger();
            SRL.LedgerName = BLL.DataKeyValue.Sales_Return_Ledger_Key;
            SRL.AccountGroupId = Sa.Id;
            DB.Ledgers.Add(SRL);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, SRL.LedgerName, SRL.Id);

            DAL.Ledger JR = new DAL.Ledger();
            JR.LedgerName = BLL.DataKeyValue.JobOrderReceived_Ledger_Key;
            JR.AccountGroupId = Inc.Id;
            DB.Ledgers.Add(JR);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, JR.LedgerName, JR.Id);


        }

        void AccountSetup_Expense(DAL.AccountGroup pr)
        {
            DAL.AccountGroup Exp = new DAL.AccountGroup();
            Exp.GroupName = BLL.DataKeyValue.Expenses_Key;
            Exp.GroupCode = "400";
            Exp.FundMasterId = pr.FundMasterId;
            Exp.UnderGroupId = pr.Id;
            DB.AccountGroups.Add(Exp);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, Exp.GroupName, Exp.Id);

            #region Direct Expense

            DAL.AccountGroup DExp = new DAL.AccountGroup();
            DExp.GroupName = BLL.DataKeyValue.DirectExpenses_Key;
            DExp.GroupCode = "410";
            DExp.FundMasterId = pr.FundMasterId;
            DExp.UnderGroupId = Exp.Id;
            DB.AccountGroups.Add(DExp);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, DExp.GroupName, DExp.Id);

            #endregion

            #region Indirect Expense

            DAL.AccountGroup IndExp = new DAL.AccountGroup();
            IndExp.GroupName = BLL.DataKeyValue.IndirectExpense_Key;
            IndExp.GroupCode = "320";
            IndExp.FundMasterId = pr.FundMasterId;
            IndExp.UnderGroupId = Exp.Id;
            DB.AccountGroups.Add(IndExp);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, IndExp.GroupName, IndExp.Id);
            #endregion

            DAL.AccountGroup Pur = new DAL.AccountGroup();
            Pur.GroupName = BLL.DataKeyValue.PurchaseAccount_Key;
            Pur.GroupCode = "330";
            Pur.FundMasterId = pr.FundMasterId;
            Pur.UnderGroupId = Exp.Id;
            DB.AccountGroups.Add(Pur);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, Pur.GroupName, Pur.Id);

         

            DAL.Ledger PurL = new DAL.Ledger();
            PurL.LedgerName = BLL.DataKeyValue.PurchaseAccount_Ledger_Key;
            PurL.AccountGroupId = Pur.Id;
            DB.Ledgers.Add(PurL);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, PurL.LedgerName, PurL.Id);

            DAL.Ledger PRL = new DAL.Ledger();
            PRL.LedgerName = BLL.DataKeyValue.Purchase_Return_Ledger_Key;
            PRL.AccountGroupId = Pur.Id;
            DB.Ledgers.Add(PRL);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, PRL.LedgerName, PRL.Id);

            DAL.Ledger JO = new DAL.Ledger();
            JO.LedgerName = BLL.DataKeyValue.JobOrderIssued_Ledger_Key;
            JO.AccountGroupId = Exp.Id;
            DB.Ledgers.Add(JO);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, JO.LedgerName, JO.Id);

            DAL.AccountGroup salary = new DAL.AccountGroup();
            salary.GroupName = BLL.DataKeyValue.Salary_Key;
            salary.GroupCode = "340";
            salary.FundMasterId = pr.FundMasterId;
            salary.UnderGroupId = IndExp.Id;
            DB.AccountGroups.Add(salary);
            DB.SaveChanges();
            SetDataKeyValue(pr.FundMasterId, salary.GroupName, salary.Id);



        }

        #endregion
    }
}
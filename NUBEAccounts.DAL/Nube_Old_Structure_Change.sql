
ALTER TABLE AccountGroups ALTER COLUMN Under numeric(18,0) 

ALTER TABLE AccountGroups ADD FOREIGN KEY(Under) references AccountGroups(AccountGroupId)
                               
ALTER TABLE Ledger ADD FOREIGN KEY (AccountGroupId) REFERENCES AccountGroups(AccountGroupId)

ALTER TABLE Ledger ADD primary key(LedgerId)
-----------------------------------------------------------------------------
ALTER TABLE LedgerOP alter column LedgerOPID numeric(18,0) not null

ALTER TABLE LedgerOP ADD primary key(LedgerOPID)

ALTER TABLE LedgerOP alter column LedgerID numeric(18,0) not null

alter table LedgerOP add foreign key(LedgerID) references Ledger(LedgerId)

------------------------------------------------------------------------------

ALTER TABLE JournalMaster ADD primary key(JournalId)

alter table JournalDetails add primary key(JDId)

alter table journalDetails add foreign key(JournalID) references JournalMaster(JournalID)

alter table journalDetails add foreign key(LedgerID) references Ledger(LedgerId)

-----------------------------------------------------------------------------

ALTER TABLE PaymentMaster alter column PaymentId numeric(18,0) not null

ALTER TABLE PaymentMaster ADD primary key(PaymentId)

alter table PaymentMaster add foreign key(LedgerId) references Ledger(LedgerId)

ALTER TABLE PaymentDetails alter column PDID numeric(18,0) not null

alter table PaymentDetails add primary key(PDID)

alter table PaymentDetails add foreign key(PaymentId) references PaymentMaster(PaymentId)

alter table PaymentDetails add foreign key(LedgerID) references Ledger(LedgerId)


-----------------------------------------------------------------------------------------

ALTER TABLE ReceiptMaster alter column ReceiptId numeric(18,0) not null

ALTER TABLE ReceiptMaster ADD primary key(ReceiptId)

alter table PaymentMaster add foreign key(LedgerId) references Ledger(LedgerId)

ALTER TABLE ReceiptDetails alter column RDID numeric(18,0) not null

alter table ReceiptDetails add primary key(RDID)

alter table ReceiptDetails add foreign key(ReceiptId) references ReceiptMaster(ReceiptId)

alter table ReceiptDetails add foreign key(LedgerID) references Ledger(LedgerId)


---add identity manually
---add ledgerId foreign key manually


 


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NUBEAccounts.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ACYearMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACYearMaster()
        {
            this.ACYearLedgerBalances = new HashSet<ACYearLedgerBalance>();
        }
    
        public short Id { get; set; }
        public string ACYear { get; set; }
        public Nullable<short> ACYearStatusId { get; set; }
        public Nullable<int> FundMasterId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACYearLedgerBalance> ACYearLedgerBalances { get; set; }
        public virtual ACYearStatu ACYearStatu { get; set; }
        public virtual FundMaster FundMaster { get; set; }
    }
}

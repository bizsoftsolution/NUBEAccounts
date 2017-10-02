using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NUBEAccounts.SL.Hubs
{
    public class SLUser
    {
        public string ConnectionId { get; set; }
        public int UserId { get; set; }
        public int FundMasterId { get; set; }
        public string AccYear { get; set; }
    }
}
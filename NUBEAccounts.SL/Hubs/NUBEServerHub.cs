using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.AspNet.SignalR;
using NUBEAccounts.DAL;

namespace NUBEAccounts.SL.Hubs
{
    public partial class NubeServerHub : Hub
    {
        #region Constant

        enum LogDetailType
        {
            INSERT,
            UPDATE,
            DELETE
        }

        #endregion

        #region Field

        private static DAL.DBNUBEAccountsEntities DB = new DBNUBEAccountsEntities();

        private static List<SLUser> UserList = new List<SLUser>();
        private static List<DAL.EntityType> _entityTypeList;
        private static List<DAL.LogDetailType> _logDetailTypeList;

        #endregion

        #region Property

        private static List<DAL.EntityType> EntityTypeList
        {
            get
            {
                if (_entityTypeList == null)
                {
                    _entityTypeList = DB.EntityTypes.ToList();
                }
                return _entityTypeList;
            }
            set
            {
                _entityTypeList = value;
            }
        }
        private static List<DAL.LogDetailType> LogDetailTypeList
        {
            get
            {
                if (_logDetailTypeList == null) _logDetailTypeList = DB.LogDetailTypes.ToList();
                return _logDetailTypeList;
            }
            set
            {
                _logDetailTypeList = value;
            }
        }
        public bool isLoginUser
        {
            get
            {
                SLUser u = UserList.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
                return u.UserId != 0 ? true : false;
            }
        }

        #region ClientSelection

        private SLUser Caller
        {
            get
            {
                SLUser u = UserList.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
                if (u == null)
                {
                    u = new SLUser() { ConnectionId = Context.ConnectionId, UserId = 0, FundMasterId = 0 };
                    UserList.Add(u);
                }
                return u;
            }
        }

        private List<string> AllClients
        {
            get
            {
                return UserList.Select(x => x.ConnectionId.ToString()).ToList();
            }
        }

        private List<string> AllLoginClients
        {
            get
            {
                return UserList.Where(x => x.UserId != 0)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> OtherClients
        {
            get
            {
                return UserList.Where(x => x.ConnectionId != Context.ConnectionId)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> OtherLoginClients
        {
            get
            {
                return UserList.Where(x => x.ConnectionId != Context.ConnectionId && x.UserId != 0)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> AllClientsOnGroup
        {
            get
            {
                return UserList.Where(x => x.FundMasterId == Caller.FundMasterId)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> AllLoginClientsOnGroup
        {
            get
            {
                return UserList.Where(x => x.FundMasterId == Caller.FundMasterId && x.UserId != 0)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> OtherClientsOnGroup
        {
            get
            {
                return UserList.Where(x => x.FundMasterId == Caller.FundMasterId && x.UserId != Caller.UserId)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> OtherLoginClientsOnGroup
        {
            get
            {
                return UserList.Where(x => x.FundMasterId == Caller.FundMasterId && x.UserId != 0 && x.UserId != Caller.UserId)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        #endregion

        #endregion

        #region Method

        public override Task OnConnected()
        {
            SLUser u = UserList.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (u == null)
            {
                u = new SLUser() { ConnectionId = Context.ConnectionId, UserId = 0, FundMasterId = 0 };
                UserList.Add(u);
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            SLUser u = UserList.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (u != null)
            {
                UserList.Remove(u);
            }

            return base.OnDisconnected(stopCalled);
        }

        private int EntityTypeId(string Type)
        {
            DAL.EntityType et = EntityTypeList.Where(x => x.Entity == Type).FirstOrDefault();
            if (et == null)
            {
                et = new DAL.EntityType();
                DB.EntityTypes.Add(et);
                EntityTypeList.Add(et);
                et.Entity = Type;
                DB.SaveChanges();
            }
            return et.Id;
        }

        private int LogDetailTypeId(LogDetailType Type)
        {
            DAL.LogDetailType ldt = LogDetailTypeList.Where(x => x.Type == Type.ToString()).FirstOrDefault();
            return ldt.Id;
        }

        private void LogDetailStore(object Data, LogDetailType Type)
        {
            try
            {
                Type t = Data.GetType();
                long EntityId = Convert.ToInt64(t.GetProperty("Id").GetValue(Data));
                int ETypeId = EntityTypeId(t.Name);

                DAL.LogMaster l = DB.LogMasters.Where(x => x.EntityId == EntityId && x.EntityTypeId == ETypeId).FirstOrDefault();
                DAL.LogDetail ld = new DAL.LogDetail();
                DateTime dt = DateTime.Now;


                if (l == null)
                {
                    l = new DAL.LogMaster();
                    DB.LogMasters.Add(l);
                    l.EntityId = EntityId;
                    l.EntityTypeId = ETypeId;
                    l.CreatedAt = dt;
                    l.CreatedBy = Caller.UserId;
                }

                if (Type == LogDetailType.UPDATE)
                {
                    l.UpdatedAt = dt;
                    l.UpdatedBy = Caller.UserId;
                }
                else if (Type == LogDetailType.DELETE)
                {
                    l.DeletedAt = dt;
                    l.DeletedBy = Caller.UserId;
                }

                DB.SaveChanges();

                DB.LogDetails.Add(ld);
                ld.LogMasterId = l.Id;
                ld.RecordDetail = new JavaScriptSerializer().Serialize(Data);
                ld.EntryBy = Caller.UserId;
                ld.EntryAt = dt;
                ld.LogDetailTypeId = LogDetailTypeId(Type);
                DB.SaveChanges();
            }
            catch (Exception ex) { }

        }


        #endregion

    }
}
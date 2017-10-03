using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace NUBEAccounts.BLL
{
    public static class NubeAccountClient
    {
        #region Field
        private static HubConnection _hubCon;
        private static IHubProxy _NubeAccountHub;
        public static string URLPath = "";
        #endregion

        #region Property
        public static HubConnection Hubcon
        {
            get
            {
                if (_hubCon == null) HubConnect();
                return _hubCon;
            }
            set
            {
                _hubCon = value;
            }
        }

        public static IHubProxy NubeAccountHub
        {
            get
            {
                if (_NubeAccountHub == null) HubConnect();
                if (Hubcon.State != ConnectionState.Connected) HubConnect();
                return _NubeAccountHub;
            }
            set
            {
                _NubeAccountHub = value;
            }
        }
        #endregion

        #region Method
        public static void HubConnect()
        {            
            try
            {
                Common.AppLib.WriteLog(URLPath);
                Common.AppLib.WriteLog("Service Starting...");
                _hubCon = new HubConnection(URLPath);
                Common.AppLib.WriteLog("Service Started");
                _NubeAccountHub = _hubCon.CreateHubProxy("NubeServerHub");
                Common.AppLib.WriteLog("Hub Created");
                _hubCon.Start(new LongPollingTransport()).Wait();
                Common.AppLib.WriteLog("Hub Started");

            }
            catch (Exception ex)
            {
               // AccountBuddy.Common.AppLib.WriteLog("Could Not Start Service");
            }           
        }

        public static void HubDisconnect()
        {
            _hubCon.Stop();
        }
        #endregion
    }
}

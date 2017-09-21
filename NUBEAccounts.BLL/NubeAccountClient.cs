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
            //string URLPath = "http://ubs3/fmcg/SignalR";
            string URLPath = "http://localhost:51707";
            //string URLPath = "http://localhost/NUBEAccountSL/";
            // string URLPath = "http://192.168.1.170/fmcg/SignalR"; 
            try
            {
                NUBEAccounts.Common.AppLib.WriteLog(URLPath);
                NUBEAccounts.Common.AppLib.WriteLog("Service Starting...");
                _hubCon = new HubConnection(URLPath);
                NUBEAccounts.Common.AppLib.WriteLog("Service Started");
                _NubeAccountHub = _hubCon.CreateHubProxy("NubeServerHub");
                NUBEAccounts.Common.AppLib.WriteLog("Hub Created");
                _hubCon.Start(new LongPollingTransport()).Wait();
                NUBEAccounts.Common.AppLib.WriteLog("Hub Started");

            }
            catch (Exception ex)
            {
               // AccountBuddy.Common.AppLib.WriteLog("Could Not Start Service");

            }
            // _hubCon = new HubConnection("http://110.4.40.46/fmcgsl/SignalR");
            // _hubCon = new HubConnection("http://ubs3/fmcg/SignalR");

        }

        public static void HubDisconnect()
        {
            _hubCon.Stop();
        }
        #endregion
    }
}

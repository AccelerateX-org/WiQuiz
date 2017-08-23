using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WIQuest.Web.Hubs
{
    public class StatisticsHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}
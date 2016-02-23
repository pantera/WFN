﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Wokhan.WindowsFirewallNotifier.Common.Helpers;
using Wokhan.WindowsFirewallNotifier.Common.Helpers.IPHelpers;

namespace Wokhan.WindowsFirewallNotifier.Console.Helpers.ViewModels
{
    public class MonitoredConnection : Connection
    {
        private IPHelper.I_OWNER_MODULE rawConnection;

        public string Name { get; set; }

        //private TCPHelper.TCP_ESTATS_BANDWIDTH_RW_v0 prevState;
        private object rawrow;

        public MonitoredConnection(IPHelper.I_OWNER_MODULE row) : base(row)
        {
            this.rawConnection = row;
            EnableStats();
        }

        private void EnableStats()
        {
            try
            {
                if (this.rawConnection is TCPHelper.MIB_TCPROW_OWNER_MODULE)
                {
                    rawrow = ((TCPHelper.MIB_TCPROW_OWNER_MODULE)this.rawConnection).ToTCPRow();
                    TCPHelper.EnsureStatsAreEnabled((TCPHelper.MIB_TCPROW)rawrow);
                }
                else
                {
                    rawrow = ((TCP6Helper.MIB_TCP6ROW_OWNER_MODULE)this.rawConnection).ToTCPRow();
                    TCP6Helper.EnsureStatsAreEnabled((TCP6Helper.MIB_TCP6ROW)rawrow);
                }
            }
            catch
            {
                IsAccessDenied = true;
            }
        }

        internal TCPHelper.TCP_ESTATS_BANDWIDTH_ROD_v0 EstimateBandwidth()
        {
            if (rawrow != null && !IsAccessDenied)
            {
                //var x = DateTime.Now.Ticks;

                return (rawrow is TCPHelper.MIB_TCPROW ? TCPHelper.GetTCPBandwidth((TCPHelper.MIB_TCPROW)rawrow) : TCP6Helper.GetTCPBandwidth((TCP6Helper.MIB_TCP6ROW)rawrow));
                //this.PointsOut.Add(new Point(x, ret.OutboundBandwidth));
                //this.PointsIn.Add(new Point(x, ret.InboundBandwidth));

                //return ret;
            }
            else
            {
                return new TCPHelper.TCP_ESTATS_BANDWIDTH_ROD_v0 { InboundBandwidth = 0, OutboundBandwidth = 0 };
            }
        }
    }
}

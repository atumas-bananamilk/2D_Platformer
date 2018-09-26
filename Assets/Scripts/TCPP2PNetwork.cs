using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TCPP2PNetwork {
    //public void P2PConnect(int port, string Host)
    //{
    //    if (!Connected)
    //    {
    //        Connected = true;
    //        SetIP(port, Host);
    //        t1 = new Thread(new ThreadStart(CreateListener));
    //        t1.Name = "Listener Thread";
    //        t1.Priority = ThreadPriority.AboveNormal;
    //        t1.Start();
    //    }
    //}

    //public void P2PDisconnect()
    //{
    //    if (Connected)
    //    {
    //        t1.Abort();
    //        peerListener.Stop();
    //        Connected = false;
    //    }

    //}

    //private void P2PCreateListener()
    //{
    //    try
    //    {
    //        int iLength = 0;
    //        Socket tc = null;
    //        peerListener = new TcpListener(IPAddress.Any, port);
    //        peerListener.Start();
    //        while (true)
    //        {
    //            if (!peerListener.Pending())
    //            {
    //                //Read data
    //            }
    //        }
    //    }
    //    catch { }
    //}
}
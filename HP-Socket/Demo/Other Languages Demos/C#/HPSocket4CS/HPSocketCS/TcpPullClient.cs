﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPSocketCS.SDK;

namespace HPSocketCS
{
    public class TcpPullClient : TcpClient
    {
        public delegate HandleResult OnPullReceive(TcpPullClient sender, int iLength);

        protected OnPullReceive OnPullReceiveCallback;
        protected HPSocketSdk.OnPullReceive SDK_OnPullReceiveCallback;
       
        HandleResult SDK_OnPullReceive(IntPtr pClient, int iLength)
        {
            return OnPullReceiveCallback(this, iLength);
        }

        public TcpPullClient()
        {
            CreateListener();
       }

        ~TcpPullClient()
        {
            Destroy();
        }

        /// <summary>
        /// 创建socket监听&服务组件
        /// </summary>
        /// <param name="isUseDefaultCallback">是否使用tcppullclient类默认回调函数</param>
        /// <returns></returns>
        protected override bool CreateListener()
        {
            if (IsCreate == true || pListener != IntPtr.Zero || pClient != IntPtr.Zero)
            {
                return false;
            }

            pListener = HPSocketSdk.Create_HP_TcpPullClientListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }

            pClient = HPSocketSdk.Create_HP_TcpPullClient(pListener);
            if (pClient == IntPtr.Zero)
            {
                return false;
            }

            IsCreate = true;

            return true;
        }

        /// <summary>
        /// 抓取数据
        /// 用户通过该方法从 Socket 组件中抓取数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="pBuffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public FetchResult Fetch(IntPtr pBuffer, int size)
        {
            return HPSocketSdk.HP_TcpPullClient_Fetch(pClient, pBuffer, size);
        }

        /// <summary>
        /// 抓取数据
        /// 用户通过该方法从 Socket 组件中抓取数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="pBuffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public FetchResult Peek(IntPtr pBuffer, int size)
        {
            return HPSocketSdk.HP_TcpPullClient_Peek(pClient, pBuffer, size);
        }

        /// <summary>
        /// 设置回调函数
        /// </summary>
        /// <param name="prepareConnect"></param>
        /// <param name="connect"></param>
        /// <param name="send"></param>
        /// <param name="recv"></param>
        /// <param name="close"></param>
        /// <param name="error"></param>
        public virtual void SetCallback(OnPrepareConnect prepareConnect, OnConnect connect,
            OnSend send, OnPullReceive recv, OnClose close,
            OnError error)
        {

            // 设置 Socket 监听器回调函数
            SetOnPullReceiveCallback(recv);
            base.SetCallback(prepareConnect, connect, send, null, close, error);
        }

        public virtual void SetOnPullReceiveCallback(OnPullReceive recv)
        {
            if (recv != null)
            {
                OnPullReceiveCallback = new OnPullReceive(recv);
                SDK_OnPullReceiveCallback = new HPSocketSdk.OnPullReceive(SDK_OnPullReceive);
            }
            else
            {
                OnPullReceiveCallback = null;
                SDK_OnPullReceiveCallback = null;
            }

            HPSocketSdk.HP_Set_FN_Client_OnPullReceive(pListener, SDK_OnPullReceiveCallback);
        }

        /// <summary>
        /// 释放TcpPullClient和TcpPullClientListener
        /// </summary>
        public override void Destroy()
        {
            Stop();

            if (pClient != IntPtr.Zero)
            {
                HPSocketSdk.Destroy_HP_TcpPullClient(pClient);
                pClient = IntPtr.Zero;
            }
            if (pListener != IntPtr.Zero)
            {
                HPSocketSdk.Destroy_HP_TcpPullClientListener(pListener);
                pListener = IntPtr.Zero;
            }

            IsCreate = false;
        }
    }
}

using Guna.UI2.WinForms;
using MessageHelper;
using NewSocketUI.Attributes;
using NewSocketUI.Helper;
using NewSocketUI.Model;
using NLog;
using NLog.Config;
using NLog.Targets;
using SocketLib.SocketCommand;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static MessageHelper.Model.MessageModel;


namespace NewSocketUI
{
    public partial class SocketMain : Form
    {
       private StocketListen StocketCommectListen = new StocketListen();



        private delegate void delegateShowMessage(string message, MessageLevel logType, SocketMessageType stocket);

        #region 共用變數
        private System.Threading.Timer threadTimer1;//顯示當下日期時間
        public FromSet FS = new FromSet();
        public LeftButtonSet LBS = new LeftButtonSet();
        public Configuration ConfigFile;
        public string FromState = "1";
        Logger logger = LogManager.GetCurrentClassLogger();
        public SAASocketClient SAASC = new SAASocketClient();
        public object SAACommect { get; private set; }
        #endregion 共用變數


        public SocketMain()
        {
            InitializeComponent();
            this.Text = string.Format("Socker上報系統 版本{0} 更新日期:{1}", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString(), new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
            CreateLogger();
            ShowMessage.OnMessage += LogMessage_OnMessage;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region 判斷程式是否有重覆開啟
            bool isRun = false;
            System.Threading.Mutex m = new System.Threading.Mutex(false, "NewSocketUI", out isRun);
            if (!isRun)
            {
                MessageBox.Show("程式己運行！");
                this.Close();
            }
            #endregion 判斷程式是否有重覆開啟
            this.WindowState = FormWindowState.Maximized;
            FS = GetFromConfig();
            LBS = GetLeftButtonConfig();
            this.InitialSet();
            logger.Trace("程式啟動");
            SAASC = GetSocketConfig();
            //logger.Trace("Trace");
            //logger.Debug("Debug");
            //logger.Info("Info");
            //logger.Warn("Warn");
            //logger.Error("Error");
            //logger.Fatal("Fatal");
            SocketStart(SAASC);
        }

        #region Socket [是否已否連線]

        private void SocketStart(SAASocketClient Sa)
        {
            if (ScoketState_uiSignal.Level == 0)
            {
                ScoketState_uiSignal.Level = 5;
                StocketCommectListen.SocketStart(Sa);
                this.StockState_toolStripStatusLabel.Text = string.Format("客戶端連接:{0}-服務端埠:{1}-Subject:{2}，己連線並開始監聽", Sa.SocketIP, Sa.SocketPort, Sa.SocketSubject);

            }
            else
            {
                ScoketState_uiSignal.Level = 0;
                this.StockState_toolStripStatusLabel.Text = string.Format("客戶端連接:{0}-服務端埠:{1}-Subject:{2}，己斷線並結束監聽", Sa.SocketIP, Sa.SocketPort, Sa.SocketSubject);
            }
        }

        #endregion Socket [是否已否連線]


        #region 顯示訊息至各元件上

        private void LogMessage_OnMessage(string message, MessageLevel logType, SocketMessageType stocket)
        {
            if (this.InvokeRequired) // 若非同執行緒
            {
                delegateShowMessage InvokeMessage = new delegateShowMessage(LogMessage_OnMessage); //利用委派執行
                this.Invoke(InvokeMessage, message, logType, stocket);
            }
            else
            {
                if (stocket == SocketMessageType.Server)
                {
                    if (logType == MessageLevel.Normal)
                    {
                        MessageHelperSTB.SendrichTextBox(message, Color.Black, Server_richTextBox);
                    }
                    else if (logType == MessageLevel.Error)
                    {
                        MessageHelperSTB.SendrichTextBox(message, Color.Red, Server_richTextBox);
                    }
                    else if (logType == MessageLevel.Warning)
                    {
                        MessageHelperSTB.SendrichTextBox(message, Color.Purple, Server_richTextBox);
                    }
                }

                else if (stocket == SocketMessageType.Client)
                {
                    try
                    {
                        if (logType == MessageLevel.Normal)
                        {
                            MessageHelperSTB.SendrichTextBox(message, Color.Black, Client_richTextBox);
                        }
                        else if (logType == MessageLevel.Error)
                        {
                            MessageHelperSTB.SendrichTextBox(message, Color.Red, Client_richTextBox);
                        }
                        else if (logType == MessageLevel.Warning)
                        {
                            MessageHelperSTB.SendrichTextBox(message, Color.Purple, Client_richTextBox);
                        }

                    }
                    catch (Exception e)
                    {
                        ShowMessage.LogMessage(e.StackTrace);
                    }
                }
                else if (stocket == SocketMessageType.From)
                {
                    ScoketState_uiSignal.Level = 0;

                    if (message.IndexOf("己連線並開始監聽", StringComparison.Ordinal) >=0)
                    {
                        ScoketState_uiSignal.Level = 5;
                    }
                    else if (message.IndexOf("連線中", StringComparison.Ordinal) >= 0)
                    {
                        ScoketState_uiSignal.Level = 3;
                    }


                    this.StockState_toolStripStatusLabel.Text = message;
                }
                else if (stocket == SocketMessageType.Systems)
                {
                    if (message.Contains("完成與Server端的連線建立"))
                    {
                        //SocketStart(Sa);
                    }

                    if (logType == MessageLevel.Normal)
                    {
                        MessageHelperSTB.SendrichTextBox(message, Color.Black, System_richTextBox);
                    }
                    else if (logType == MessageLevel.Error)
                    {
                        MessageHelperSTB.SendrichTextBox(message, Color.Red, System_richTextBox);
                    }
                    else if (logType == MessageLevel.Warning)
                    {
                        MessageHelperSTB.SendrichTextBox(message, Color.Purple, System_richTextBox);
                    }
                }

            }

        }

        #endregion 顯示訊息至各元件上


        #region [狀態列設定]
        private void ThreadTimerInitial()
        {
            threadTimer1 = new System.Threading.Timer(threadTimer_Tick, null, 100, 1000);
        }

        private void threadTimer_Tick(object states)
        {
            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.toolLabelNowTime.Text = $"現在時間： {DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}";

                }));
            }
            catch
            {
                ;
            }
        }

        #endregion [狀態列設定]

        #region [頁面初始設定]
        /// <summary>
        /// 取得From Seting Value
        /// </summary>
        private FromSet GetFromConfig()
        {

            string ConfigFileRoute = Path.Combine(Directory.GetCurrentDirectory(), "", "App.config");
            FromSet GetFS = new FromSet();
            if (File.Exists(ConfigFileRoute))
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = ConfigFileRoute;
                ConfigFile = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                GetFS.FromIllustrate = ConfigFile.AppSettings.Settings["FromIllustrate"].Value.ToString();
                GetFS.ServerLog = ConfigFile.AppSettings.Settings["ServerLog"].Value.ToString();
                GetFS.ClientLog = ConfigFile.AppSettings.Settings["ClientLog"].Value.ToString();
                GetFS.SystemLog = ConfigFile.AppSettings.Settings["SystemLog"].Value.ToString();
                GetFS.SimulationCommandArea = ConfigFile.AppSettings.Settings["SimulationCommandArea"].Value.ToString();
                GetFS.SocketSetArea = ConfigFile.AppSettings.Settings["SocketSetArea"].Value.ToString();
            }
            return GetFS;
        }

        /// <summary>
        /// 取得左側按鈕中文說明
        /// </summary>
        /// <returns></returns>
        private LeftButtonSet GetLeftButtonConfig()
        {
            string ConfigFileRoute = Path.Combine(Directory.GetCurrentDirectory(), "", "App.config");
            LeftButtonSet GetLBS = new LeftButtonSet();
            if (File.Exists(ConfigFileRoute))
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = ConfigFileRoute;
                ConfigFile = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                GetLBS.AllLogButton = ConfigFile.AppSettings.Settings["AllLogButton"].Value.ToString();
                GetLBS.SocketAllLogButton = ConfigFile.AppSettings.Settings["SocketAllLogButton"].Value.ToString();
                GetLBS.ServerLogButton = ConfigFile.AppSettings.Settings["ServerLogButton"].Value.ToString();
                GetLBS.ClientLogButton = ConfigFile.AppSettings.Settings["ClientLogButton"].Value.ToString();
                GetLBS.SystemLogButton = ConfigFile.AppSettings.Settings["SystemLogButton"].Value.ToString();
                GetLBS.SimulationCommand = ConfigFile.AppSettings.Settings["SimulationCommand"].Value.ToString();
                GetLBS.SocketSet = ConfigFile.AppSettings.Settings["SocketSet"].Value.ToString();
            }
            return GetLBS;
        }


        /// <summary>
        /// 取得Socket Seting Value
        /// </summary>
        private SAASocketClient GetSocketConfig()
        {

            string ConfigFileRoute = Path.Combine(Directory.GetCurrentDirectory(), "", "App.config");
            SAASocketClient SetSAASC = new SAASocketClient();
            if (File.Exists(ConfigFileRoute))
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = ConfigFileRoute;
                ConfigFile = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                SetSAASC.SocketIP = ConfigFile.AppSettings.Settings["SocketIP"].Value.ToString();
                SetSAASC.SocketPort = ConfigFile.AppSettings.Settings["SocketPort"].Value.ToString();
                SetSAASC.SocketSubject = ConfigFile.AppSettings.Settings["SocketSubject"].Value.ToString();
                SetSAASC.SAA_DatabaseName = ConfigFile.AppSettings.Settings["SAA_Database"].Value.ToString();
                SetSAASC.SAA_DatabaseIP = ConfigFile.AppSettings.Settings["SAA_DatabaseIP"].Value.ToString();
                SetSAASC.Plasma_DatabaseName = ConfigFile.AppSettings.Settings["Plasma_Database"].Value.ToString();
                SetSAASC.Plasma_DatabaseIP = ConfigFile.AppSettings.Settings["Plasma_DatabaseIP"].Value.ToString();

            }
            return SetSAASC;
        }


        #region [畫面初始設定]

        private void InitialSet()
        {

            AllLog_Guna2Button.Text = LBS.AllLogButton.ToString();
            SocketAllLog_Guna2Button.Text = LBS.SocketAllLogButton.ToString();
            ServerLog_Guna2Button.Text = LBS.ServerLogButton.ToString();
            ClientLog_Guna2Button.Text = LBS.ClientLogButton.ToString();
            SystemLog_Guna2Button.Text = LBS.SystemLogButton.ToString();
            SimulationCommand_Guna2Button.Text = LBS.SimulationCommand.ToString();
            SocketSet_Guna2Button.Text = LBS.SocketSet.ToString();

            ClientLog_uiTitlePanel.Text = FS.ServerLog.ToString();
            ServerLog_uiTitlePanel.Text = FS.ClientLog.ToString();
            SystemLog_uiTitlePanel.Text = FS.SystemLog.ToString();
            SimulationCommand_uiTitlePanel.Text = FS.SimulationCommandArea.ToString();
            SocketSetArea_uiTitlePanel.Text = FS.SocketSetArea.ToString();

        }

        private void MainInitial(string sType)
        {
            ClientLog_uiTitlePanel.Visible = false;
            ServerLog_uiTitlePanel.Visible = false;
            SystemLog_uiTitlePanel.Visible = false;
            SimulationCommand_uiTitlePanel.Visible = false;
            SocketSetArea_uiTitlePanel.Visible = false;

            double dHeight = RightOperate_uiPanel.Height;


            switch (sType)
            {
                case "1":

                    ClientLog_uiTitlePanel.Visible = true;
                    ClientLog_uiTitlePanel.Height = Convert.ToInt16(Math.Ceiling(dHeight / 3));
                    ServerLog_uiTitlePanel.Visible = true;
                    ServerLog_uiTitlePanel.Height = Convert.ToInt16(Math.Ceiling(dHeight / 3));
                    SystemLog_uiTitlePanel.Visible = true;
                    SystemLog_uiTitlePanel.Height = Convert.ToUInt16(dHeight) - (Convert.ToInt16(Math.Ceiling(dHeight / 3)) * 2);
                    break;

                case "2":

                    ClientLog_uiTitlePanel.Visible = true;
                    ClientLog_uiTitlePanel.Height = Convert.ToInt16(Math.Ceiling(dHeight / 2));
                    ServerLog_uiTitlePanel.Visible = true;
                    ServerLog_uiTitlePanel.Height = Convert.ToUInt16(dHeight) - Convert.ToInt16(Math.Ceiling(dHeight / 2));
                    break;

                case "3":

                    ClientLog_uiTitlePanel.Visible = true;
                    ClientLog_uiTitlePanel.Height = Convert.ToInt16(dHeight);
                    break;

                case "4":

                    ServerLog_uiTitlePanel.Visible = true;
                    ServerLog_uiTitlePanel.Height = Convert.ToInt16(dHeight);
                    break;

                case "5":

                    SystemLog_uiTitlePanel.Visible = true;
                    SystemLog_uiTitlePanel.Height = Convert.ToInt16(dHeight);
                    break;

                case "6":

                    SimulationCommand_uiTitlePanel.Visible = true;
                    SimulationCommand_uiTitlePanel.Height = Convert.ToInt16(dHeight);
                    break;

                case "7":

                    SocketSetArea_uiTitlePanel.Visible = true;
                    SocketSetArea_uiTitlePanel.Height = Convert.ToInt16(dHeight);
                    break;

            }
        }
        #endregion [畫面初始設定]

        #endregion [頁面初始設定]

        #region  [按鈕指令集]

        #region [按鈕處理作業]

        private void ZoomInOut_Guna2Button_Click(object sender, EventArgs e)
        {
            if (ZoomInOut_Guna2Button.Tag.ToString() == "zoomout")
            {
                ZoomInOut_Guna2Button.Tag = "zoomin";
                ZoomInOut_Guna2Button.Image = Properties.Resources.zoomin_32;
                LeftOperate_uiPanel.Width = 50;

                AllLog_Guna2Button.Text = "";
                SocketAllLog_Guna2Button.Text = "";
                ServerLog_Guna2Button.Text = "";
                ClientLog_Guna2Button.Text = "";
                SystemLog_Guna2Button.Text = "";
                SimulationCommand_Guna2Button.Text = "";
                SocketSet_Guna2Button.Text = "";


                AllLog_Guna2Button.ImageAlign = HorizontalAlignment.Center;
                SocketAllLog_Guna2Button.ImageAlign = HorizontalAlignment.Center;
                ServerLog_Guna2Button.ImageAlign = HorizontalAlignment.Center;
                ClientLog_Guna2Button.ImageAlign = HorizontalAlignment.Center;
                SystemLog_Guna2Button.ImageAlign = HorizontalAlignment.Center;
                SimulationCommand_Guna2Button.ImageAlign = HorizontalAlignment.Center;
                SocketSet_Guna2Button.ImageAlign = HorizontalAlignment.Center;

                AllLog_Guna2Button.Width = 40;
                SocketAllLog_Guna2Button.Width = 40;
                ServerLog_Guna2Button.Width = 40;
                ClientLog_Guna2Button.Width = 40;
                SystemLog_Guna2Button.Width = 40;
                SimulationCommand_Guna2Button.Width = 40;
                SocketSet_Guna2Button.Width = 40;
            }
            else
            {
                ZoomInOut_Guna2Button.Tag = "zoomout";
                ZoomInOut_Guna2Button.Image = Properties.Resources.zoomout_32;
                LeftOperate_uiPanel.Width = 210;

                AllLog_Guna2Button.Text = LBS.AllLogButton.ToString();
                SocketAllLog_Guna2Button.Text = LBS.SocketAllLogButton.ToString();
                ServerLog_Guna2Button.Text = LBS.ServerLogButton.ToString();
                ClientLog_Guna2Button.Text = LBS.ClientLogButton.ToString();
                SystemLog_Guna2Button.Text = LBS.SystemLogButton.ToString();
                SimulationCommand_Guna2Button.Text = LBS.SimulationCommand .ToString();
                SocketSet_Guna2Button.Text = LBS.SocketSet.ToString();

                AllLog_Guna2Button.ImageAlign = HorizontalAlignment.Left;
                SocketAllLog_Guna2Button.ImageAlign = HorizontalAlignment.Left;
                ServerLog_Guna2Button.ImageAlign = HorizontalAlignment.Left;
                ClientLog_Guna2Button.ImageAlign = HorizontalAlignment.Left;
                SystemLog_Guna2Button.ImageAlign = HorizontalAlignment.Left;
                SimulationCommand_Guna2Button.ImageAlign = HorizontalAlignment.Left;
                SocketSet_Guna2Button.ImageAlign = HorizontalAlignment.Left;

                AllLog_Guna2Button.Width = 200;
                SocketAllLog_Guna2Button.Width = 200;
                ServerLog_Guna2Button.Width = 200;
                ClientLog_Guna2Button.Width = 200;
                SystemLog_Guna2Button.Width = 200;
                SimulationCommand_Guna2Button.Width = 200;
                SocketSet_Guna2Button.Width = 200;

            }
        }

        private void AllLog_Guna2Button_Click(object sender, EventArgs e)
        {
            FromState = "1";
            this.MainInitial(FromState);
        }

        private void SocketAllLog_Guna2Button_Click(object sender, EventArgs e)
        {
            FromState = "2";
            this.MainInitial(FromState);
        }

        private void ServerLog_Guna2Button_Click(object sender, EventArgs e)
        {
            FromState = "3";
            this.MainInitial(FromState);
        }

        private void ClientLog_Guna2Button_Click(object sender, EventArgs e)
        {
            FromState = "4";
            this.MainInitial(FromState);
        }

        private void SystemLog_Guna2Button_Click(object sender, EventArgs e)
        {
            FromState = "5";
            this.MainInitial(FromState);
        }

        private void SimulationCommand_Guna2Button_Click(object sender, EventArgs e)
        {
            FromState = "6";
            this.MainInitial(FromState);
        }

        private void SocketSet_Guna2Button_Click(object sender, EventArgs e)
        {
            FromState = "7";
            this.MainInitial(FromState);
        }
        #endregion [按鈕處理作業]

        #region [按鈕說明顯示]
        private void ZoomInOut_Guna2Button_MouseEnter(object sender, EventArgs e)
        {
            if (ZoomInOut_Guna2Button.Tag.ToString() == "zoomout")
            {
                uiToolTip.Show("隱藏操作按鈕", (Guna2Button)sender);

            }
            else
            {
                uiToolTip.Show("顯示操作按鈕", (Guna2Button)sender);
            }
        }

        private void AllLog_Guna2Button_MouseEnter(object sender, EventArgs e)
        {
            if (ZoomInOut_Guna2Button.Tag.ToString() == "zoomin")
            {
                uiToolTip.Show(LBS.AllLogButton.ToString(), (Guna2Button)sender);
            }
        }

        private void SocketAllLog_Guna2Button_MouseEnter(object sender, EventArgs e)
        {
            if (ZoomInOut_Guna2Button.Tag.ToString() == "zoomin")
            {
                uiToolTip.Show(LBS.SocketAllLogButton.ToString(), (Guna2Button)sender);
            }
        }

        private void ServerLog_Guna2Button_MouseEnter(object sender, EventArgs e)
        {
            if (ZoomInOut_Guna2Button.Tag.ToString() == "zoomin")
            {
                uiToolTip.Show(LBS.ServerLogButton.ToString(), (Guna2Button)sender);
            }
        }

        private void ClientLog_Guna2Button_MouseEnter(object sender, EventArgs e)
        {
            if (ZoomInOut_Guna2Button.Tag.ToString() == "zoomin")
            {
                uiToolTip.Show(LBS.ClientLogButton.ToString(), (Guna2Button)sender);
            }
        }

        private void SystemLog_Guna2Button_MouseEnter(object sender, EventArgs e)
        {
            if (ZoomInOut_Guna2Button.Tag.ToString() == "zoomin")
            {
                uiToolTip.Show(LBS.SystemLogButton.ToString(), (Guna2Button)sender);
            }

        }

        private void SimulationCommand_Guna2Button_MouseEnter(object sender, EventArgs e)
        {
            if (ZoomInOut_Guna2Button.Tag.ToString() == "zoomin")
            {
                uiToolTip.Show(LBS.SimulationCommand.ToString(), (Guna2Button)sender);
            }
        }

        private void SocketSet_Guna2Button_MouseEnter(object sender, EventArgs e)
        {
            if (ZoomInOut_Guna2Button.Tag.ToString() == "zoomin")
            {
                uiToolTip.Show(LBS.SocketSet.ToString(), (Guna2Button)sender);
            }
        }


        #endregion [按鈕說明顯示]


        #endregion  [按鈕指令集]

        #region [畫面操作]
        private void SocketMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ICON_NotifyIcon.Visible = true;
                this.Hide();
            }
            else
            {
                this.ICON_NotifyIcon.Visible = false;
                this.MainInitial(FromState);
            }

        }

        private void SocketMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否要關閉程式 ?", "警告訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                logger.Trace("程式關閉");
                System.Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        #endregion [畫面操作]

        #region [長駐時事件處理]

        private void 開啟ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Maximized;
        }

        private void 關閉ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void contextMenuStrip_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void ICON_NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Maximized;
        }



        #endregion [長駐時事件處理]

        #region [Log記錄]
        private static void CreateLogger()
        {
            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget
            {
                FileName = "${basedir}/logs/${shortdate}.log",
                Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} [${uppercase:${level}}] ${message}",
            };
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
            LogManager.Configuration = config;

        }
        #endregion [Log記錄]

        private void System_richTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            ;
        }
    }
}

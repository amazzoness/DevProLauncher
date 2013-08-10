﻿using System;
using System.Windows.Forms;
using System.Threading;
using DevProLauncher.Network.Enums;
using DevProLauncher.Helpers;
using System.Diagnostics;
using DevProLauncher.Windows.MessageBoxs;

namespace DevProLauncher.Windows
{
    public partial class MainFrm : Form
    {
        public readonly HubGameList_frm GameWindow;
        readonly LoginFrm m_loginWindow;
        readonly ChatFrm m_chatWindow;
        readonly SupportFrm m_devpointWindow;
        readonly FileManagerFrm m_filemanagerWindow;
        readonly CustomizeFrm m_customizerWindow;

        public MainFrm()
        {
            InitializeComponent();

            var version = Program.Version.ToCharArray();
            Text = "DevPro" + " v" + version[0] + "." + version[1] + "." + version[2] + " r" + Program.Version[3];

            LauncherHelper.LoadBanlist();

            var loginTab = new TabPage("Login");
            m_loginWindow = new LoginFrm();
            loginTab.Controls.Add(m_loginWindow);
            mainTabs.TabPages.Add(loginTab);

            m_chatWindow = new ChatFrm();
            GameWindow = new HubGameList_frm();
            m_devpointWindow = new SupportFrm();
            m_filemanagerWindow = new FileManagerFrm();
            m_customizerWindow = new CustomizeFrm();
            LauncherHelper.CardManager.Init();

            var connectThread = new Thread(Loaded) { IsBackground = true};
            connectThread.Start();
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void Loaded()
        {
            if (!Program.ChatServer.Connect(Program.Config.ServerAddress, Program.Config.ChatPort))
                MessageBox.Show(Program.LanguageManager.Translation.pMsbErrorToServer);
            else
                m_loginWindow.Connected();
        }
        public void Login()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(Login));
                return;
            }

            mainTabs.TabPages.Remove(mainTabs.SelectedTab);

            var gamelistTab = new TabPage("GameList");
            gamelistTab.Controls.Add(GameWindow);
            mainTabs.TabPages.Add(gamelistTab);

            var chatTab = new TabPage("Chat (Beta v4)");
            chatTab.Controls.Add(m_chatWindow);
            mainTabs.TabPages.Add(chatTab);

            var filemanagerTab = new TabPage("File Manager");
            filemanagerTab.Controls.Add(m_filemanagerWindow);
            mainTabs.TabPages.Add(filemanagerTab);

            var cuztomizerTab = new TabPage("Customizer");
            cuztomizerTab.Controls.Add(m_customizerWindow);
            mainTabs.TabPages.Add(cuztomizerTab);

            var devpointTab = new TabPage("Support DevPro");
            devpointTab.Controls.Add(m_devpointWindow);
            mainTabs.TabPages.Add(devpointTab);
                
            ConnectionCheck.Enabled = true;
            ConnectionCheck.Tick += CheckConnection;
            
            UpdateUsername();

            ProfileBtn.Enabled = true;

            Program.ChatServer.SendPacket(DevServerPackets.UserList);
            Program.ChatServer.SendPacket(DevServerPackets.FriendList);
            Program.ChatServer.SendPacket(DevServerPackets.DevPoints);

        }

        public void UpdateUsername()
        {
            Text = "DevPro" + " v" + Program.Version[0] + "." + Program.Version[1] + "." + Program.Version[2] + " r" + Program.Version[3] + " - " + Program.UserInfo.username;
        }

        public void ReLoadLanguage()
        {
            //m_gameWindow.ApplyTranslation();
            m_filemanagerWindow.ApplyTranslations();
            m_customizerWindow.ApplyTranslation();
            m_chatWindow.ApplyTranslations();
        }

        private void CheckConnection(object sender, EventArgs e)
        {
            if (!Program.ChatServer.Connected())
            {
                var connectionCheck = (System.Windows.Forms.Timer)sender;
                Hide();
                connectionCheck.Enabled = false;
                if (MessageBox.Show("Disconnected from server.", "Server", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    var process = new Process();
                    var startInfos = new ProcessStartInfo(Application.ExecutablePath, "-r");
                    process.StartInfo = startInfos;
                    process.Start();
                    Application.Exit();
                }
                else
                {
                    Application.Exit();
                }

            }
        }

        private void OfflineBtn_Click(object sender, EventArgs e)
        {
            LauncherHelper.RunGame("");
        }

        private void siteBtn_Click(object sender, EventArgs e)
        {
            Process.Start("http://devpro.org");
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            Process.Start("http://devpro.org/staff/");
        }

        private void DeckBtn_Click(object sender, EventArgs e)
        {
            LauncherHelper.GenerateConfig();
            LauncherHelper.RunGame("-d");
        }
        private void ReplaysBtn_Click(object sender, EventArgs e)
        {
            LauncherHelper.GenerateConfig();
            LauncherHelper.RunGame("-r");
        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            var profile = new ProfileFrm();
            profile.ShowDialog();
        }

        private void OptionsBtn_Click(object sender, EventArgs e)
        {
            var settings = new Settings();
            settings.ShowDialog();

        }
    }
}

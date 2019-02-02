using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateRegions;

namespace StateMigration.UI
{
    public partial class mainFrom : MetroFramework.Forms.MetroForm
    {
        #region GlobalVariables        

        CancellationTokenSource cts = new CancellationTokenSource();
        
        Regions regions = new Regions();        

        SystemDetails sysdetails = new SystemDetails();

        SettingsModel settings = new SettingsModel();     

        List<string> currentStats = new List<string>();

        List<string> custompaths = new List<string>();

        public OperationModel CurrentOperationModel { get; set; }
        
        private static string user = string.Empty;
        
        private static string TargetPath = string.Empty;        

        System.Timers.Timer btnInitializetimer = new System.Timers.Timer();        

        #endregion

        #region Main

        public mainFrom()
        {            
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeUIDefaults();
            
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        

        private void MainFrom_Load(object sender, EventArgs e)
        {                  
            btnInitializetimer.Interval = 200;
            btnInitializetimer.Elapsed += btnInitializetimer_Elapsed;
            btnInitializetimer.Start();

        }

        #endregion
        
        #region ButtonEvents             

        private void mtbtnBackup_Click(object sender, EventArgs e)
        {
            InitializeBackupUI();

            IDestination destination = new Destination();        

            cts = new CancellationTokenSource();
            
            user = ((mttxtboxUserID.Text).Equals(Environment.UserName) ? Environment.UserName : mttxtboxUserID.Text).ToLower();

            TargetPath = string.IsNullOrWhiteSpace(destination.Resolve(objPerson)) ? Path.Combine(@"C:\Temp", user) : Path.Combine(destination.Resolve(objPerson), user);
            
            StartBackup(regions, user,objPerson, TargetPath, cts.Token);

        }

        private void mtbtnRestore_Click(object sender, EventArgs e)
        {
            InitializeRestoreUI();

            IDestination destination = new Destination();

            cts = new CancellationTokenSource();

            user = ((mttxtboxUserID.Text).Equals(Environment.UserName) ? Environment.UserName : mttxtboxUserID.Text).ToLower();
            TargetPath = string.IsNullOrWhiteSpace(destination.Resolve(objPerson)) ? Path.Combine(@"C:\Temp", user) : Path.Combine(destination.Resolve(objPerson), user);
            
            StartRestore(user, TargetPath, cts.Token);

        }

        private void mtbtnCancel_Click(object sender, EventArgs e)
        {
            cts.Cancel();           

        }

        private void mtbtnExit_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate {
                cts.Dispose();                
            });            
            Application.Exit();
        }

        #endregion

        #region MainfromClose

        private void mainFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnInitializetimer != null)
            {
                btnInitializetimer.Dispose();
            }

            if (t1 != null)
            {
                t1.Stop();
                t1.Dispose();
            }

        }

        #endregion


    }
}

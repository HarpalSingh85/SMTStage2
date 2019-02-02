using System;
using System.Drawing;
using StateMigrationTool;
using System.Windows.Forms;
using StateMigrationBackend.Models;
using System.Linq;
using StateMigrationBackend.StateRegions;

namespace StateMigration.UI
{
    public partial class mainFrom
    {
        #region UI Helpers

        private async void InitializeUIDefaults()
        {
            IAutoDetection detect = new AutoDetection();            
            sysdetails = await detect.DetectAsync();           

            //About
            InitializeAboutListView();

            //ProgressBars            
            HideProgressbar();

            //Setting Tab
            CBO cbo = new CBO();
            mtcboBRtype.DataSource = new BindingSource(cbo.GetBRTypes(), null);
            mtcboBRtype.DisplayMember = "Key";
            mtcboBRtype.ValueMember = "Value";
            mtcboBRtype.SelectedIndex = 0;

            mtcboLogtype.DataSource = new BindingSource(cbo.GetLogTypes(), null);
            mtcboLogtype.DisplayMember = "Key";
            mtcboLogtype.ValueMember = "Value";
            mtcboLogtype.SelectedIndex = 0;

            mtTbResUtilization.Value = 2;
            SetResoureUtlizationLevel(mtTbResUtilization.Value);

            mtAutoProfileDetect.Checked = true;
            mtbtnSettingUserProfile.Enabled = false;

            mtAutoProfilePathDetect.Checked = true;
            mtbtnSettingUserProfilePath.Enabled = false;

            mtAutoSaveSharedDevices.Checked = true;

            mtTbLogOption.Checked = true;

            mtTbAIOverride.Checked = true;

            //Backup Tab
            UIMessages msg = new UIMessages();
            BackupResponseLabelWarning(msg.Show(5));
            InitializeBackupListView();

            //Restore Tab
            RestoreResponseLabelWarning(msg.Show(5));
            InitializeRestoreListView();


            //User ID Textbox
            mttxtboxUserID.UseCustomBackColor = true;
            mttxtboxUserID.UseCustomForeColor = true;

            mttxtboxUserID.BackColor = Color.FromArgb(252, 252, 252);
            mttxtboxUserID.ForeColor = Color.FromArgb(69, 69, 69);
            mttxtboxUserID.Text = Environment.UserName.ToUpper();

            //Main UI
            mtbtnBackup.Enabled = false;
            mtbtnRestore.Visible = false;
            mtbtnRestore.Enabled = false;
            mtbtnCancel.Enabled = false;
            mtTabctrl.SelectedTab = mtPagebackup;
            
            EnableTabPage(mtPagerestore, true);
            EnableTabPage(mtPagesettings, true);
            EnableTabPage(mtPagebackup, true);

            lstViewDefaultPaths.ForeColor = Color.LimeGreen;           
            lstViewDefaultPaths.BackColor = Color.FromArgb(48, 48, 48);
            lbldefaultPaths.ForeColor = Color.White;
            lbldefaultPaths.BackColor = Color.FromArgb(109, 84, 249);
            mtdefaultReslbl.Text = "";            


            lbldefaultPaths.Text = "Default Assigned Paths (These will be used as target paths for backup / restore)";
            mtdefaultReslbl.UseCustomBackColor = true;
            mtdefaultReslbl.UseCustomForeColor = true;
            mtdefaultReslbl.ForeColor = Color.White;
            mtdefaultReslbl.BackColor = Color.FromArgb(211, 159, 23);
            mtdefaultReslbl.Text = "Pre-Defined paths for Backup && Restore operations.";

        }

        private void InitializeBackupUI()
        {
            UIMessages msg = new UIMessages();
            BackupResponseLabelWarning(msg.Show(5));

            mtlstbxBackup.Items.Clear();
            mttxtboxUserID.Enabled = false;
            mttxtboxUserID.ForeColor = Color.DarkGray; 
                       
            mtbtnBackup.Enabled = false;
            mtbtnCancel.Enabled = true;
                      
            HideTabPage(mtTabctrl, mtPagerestore);
            EnableTabPage(mtPagerestore, false);
            EnableTabPage(mtPagesettings, false);
            EnableTabPage(mtPagebackup, true);

        }

        private void InitializeRestoreUI()
        {
            UIMessages msg = new UIMessages();
            RestoreResponseLabelWarning(msg.Show(5));

            mtlstbxRestore.Items.Clear();
            mttxtboxUserID.Enabled = false;
            mttxtboxUserID.ForeColor = Color.DarkGray;  
                      
            mtbtnRestore.Enabled = false;
            mtbtnCancel.Enabled = true;      
                       
            HideTabPage(mtTabctrl, mtPagebackup);
            EnableTabPage(mtPagebackup, false);
            EnableTabPage(mtPagesettings, false);
            EnableTabPage(mtPagerestore, true);

        }

        private void InitializeUIException(string exception)
        {
            UIMessages msg = new UIMessages();
            HideProgressbar();
            if(t1 != null)
            {
                t1.Stop();
                t1.Dispose();
            }            
            ReDrawUI(msg.Show(6), exception, Color.White, Color.FromArgb(255, 98, 0), ContentAlignment.MiddleCenter);            
        }

        private void InitializeUISuccess(string message, string ListBoxMessage)
        {
            HideProgressbar();
            if (t1 != null)
            {
                t1.Stop();
                t1.Dispose();
            }
            ReDrawUI(message, ListBoxMessage, Color.White, Color.DarkGreen, ContentAlignment.MiddleCenter, true, Color.LimeGreen);            
        }

        private void InitiatlizeUICancelled(string message)
        {
            HideProgressbar();
            if (t1 != null)
            {
                t1.Stop();
                t1.Dispose();
            }
            ReDrawUI(message, message, Color.White, Color.DarkOrange, ContentAlignment.MiddleCenter);
        }                

        private void ReDrawUI(string message, string ListboxMessage, Color ForeColor, Color BackColor, ContentAlignment Alignment)
        {
            if(mtTabctrl.IsHandleCreated)
            {
                mtTabctrl?.Invoke((MethodInvoker)(() => {
                    if (mtTabctrl.SelectedTab == mtPagebackup)
                    {
                        AddBackupListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));
                        AddBackupListView(ListboxMessage, BackColor);
                        AddBackupListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                        if(BckReslbl.IsHandleCreated)
                        {
                            BckReslbl?.Invoke((MethodInvoker)(() =>
                            {
                                BckReslbl.UseCustomForeColor = true;
                                BckReslbl.UseCustomBackColor = true;
                                BckReslbl.TextAlign = Alignment;
                                BckReslbl.BackColor = BackColor;
                                BckReslbl.ForeColor = ForeColor;
                                BckReslbl.FontSize = MetroFramework.MetroLabelSize.Medium;
                                BckReslbl.Text = message;
                            }));
                        }
                        

                        mtbtnRestore.Invoke((MethodInvoker)(() => {
                            mtbtnRestore.Visible = false;
                        }));


                        mtbtnBackup.Invoke((MethodInvoker)(() => {
                            mtbtnBackup.Enabled = true;
                            mtbtnBackup.Visible = true;
                        }));

                    }
                    else if (mtTabctrl.SelectedTab == mtPagerestore)
                    {

                        AddRestoreListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));
                        AddRestoreListView(ListboxMessage, BackColor);
                        AddRestoreListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                        if(RtReslbl.IsHandleCreated)
                        {
                            RtReslbl?.Invoke((MethodInvoker)(() =>
                            {
                                RtReslbl.UseCustomForeColor = true;
                                RtReslbl.UseCustomBackColor = true;
                                RtReslbl.TextAlign = Alignment;
                                RtReslbl.BackColor = BackColor;
                                RtReslbl.ForeColor = ForeColor;
                                RtReslbl.FontSize = MetroFramework.MetroLabelSize.Medium;
                                RtReslbl.Text = message;
                            }));
                        }
                        

                        mtbtnBackup.Invoke((MethodInvoker)(() => {
                            mtbtnBackup.Visible = false;
                        }));

                        mtbtnRestore.Invoke((MethodInvoker)(() => {
                            mtbtnRestore.Enabled = true;
                            mtbtnRestore.Visible = true;
                        }));
                    }
                    else
                    {
                        if (mtbtnBackup.Visible)
                        {

                            AddBackupListView(message);
                            AddBackupListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));
                            AddBackupListView(ListboxMessage, BackColor);
                            AddBackupListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                            if(BckReslbl.IsHandleCreated)
                            {
                                BckReslbl.Invoke((MethodInvoker)(() =>
                                {
                                    BckReslbl.UseCustomForeColor = true;
                                    BckReslbl.UseCustomBackColor = true;
                                    BckReslbl.TextAlign = Alignment;
                                    BckReslbl.BackColor = BackColor;
                                    BckReslbl.ForeColor = ForeColor;
                                    BckReslbl.FontSize = MetroFramework.MetroLabelSize.Medium;
                                    BckReslbl.Text = message;
                                }));
                            }
                            
                            if(mtbtnBackup.IsHandleCreated)
                            {
                                mtbtnBackup.Invoke((MethodInvoker)(() => mtbtnBackup.Enabled = true));
                            }

                        }
                        else
                        {
                            AddRestoreListView(message);
                            AddRestoreListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));
                            AddRestoreListView(ListboxMessage, BackColor);
                            AddRestoreListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                            if(RtReslbl.IsHandleCreated)
                            {
                                RtReslbl.Invoke((MethodInvoker)(() =>
                                {
                                    RtReslbl.UseCustomForeColor = true;
                                    RtReslbl.UseCustomBackColor = true;
                                    RtReslbl.TextAlign = Alignment;
                                    RtReslbl.BackColor = BackColor;
                                    RtReslbl.ForeColor = ForeColor;
                                    RtReslbl.FontSize = MetroFramework.MetroLabelSize.Medium;
                                    RtReslbl.Text = message;
                                }));
                            }

                            if (mtbtnRestore.IsHandleCreated)
                            {
                                mtbtnRestore.Invoke((MethodInvoker)(() => mtbtnRestore.Enabled = true));
                            }

                        }
                    }


                }));
            }
            

            mttxtboxUserID.Invoke((MethodInvoker)(() => mttxtboxUserID.Enabled = true));

            mtbtnCancel.Invoke((MethodInvoker)(() => mtbtnCancel.Enabled = false));

            mtTabctrl.Invoke((MethodInvoker)(() => {

                EnableTabPage(mtPagebackup, true);
                ShowTabPage(mtTabctrl, mtPagebackup);
                EnableTabPage(mtPagerestore, true);
                ShowTabPage(mtTabctrl, mtPagerestore);
                EnableTabPage(mtPagesettings, true);

            }));


        }

        private void ReDrawUI(string message, string ListboxMessage, Color ForeColor, Color BackColor, ContentAlignment Alignment, bool OverrideListViewForeColor, Color ListViewForeColor)
        {
            if(mtTabctrl.IsHandleCreated)
            {
                mtTabctrl.Invoke((MethodInvoker)(() => {
                    if (mtTabctrl.SelectedTab == mtPagebackup)
                    {
                        AddBackupListView(message);
                        AddBackupListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                        if (OverrideListViewForeColor)
                        { AddBackupListView(ListboxMessage, ListViewForeColor); }
                        else
                            AddBackupListView(ListboxMessage, BackColor);

                        AddBackupListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                        if(BckReslbl.IsHandleCreated)
                        {
                            BckReslbl.Invoke((MethodInvoker)(() =>
                            {
                                BckReslbl.UseCustomForeColor = true;
                                BckReslbl.UseCustomBackColor = true;
                                BckReslbl.TextAlign = Alignment;
                                BckReslbl.BackColor = BackColor;
                                BckReslbl.ForeColor = ForeColor;
                                BckReslbl.FontSize = MetroFramework.MetroLabelSize.Medium;
                                BckReslbl.Text = message;
                            }));
                        }
                        

                        mtbtnRestore.Invoke((MethodInvoker)(() => {
                            mtbtnRestore.Visible = false;
                        }));

                        mtbtnBackup.Invoke((MethodInvoker)(() => {
                            mtbtnBackup.Enabled = true;
                            mtbtnBackup.Visible = true;
                        }));

                    }
                    else if (mtTabctrl.SelectedTab == mtPagerestore)
                    {
                        AddRestoreListView(message);
                        AddRestoreListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));


                        if (OverrideListViewForeColor)
                        { AddRestoreListView(ListboxMessage, ListViewForeColor); }
                        else
                            AddRestoreListView(ListboxMessage, BackColor);

                        AddRestoreListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                        if(RtReslbl.IsHandleCreated)
                        {
                            RtReslbl.Invoke((MethodInvoker)(() =>
                            {
                                RtReslbl.UseCustomForeColor = true;
                                RtReslbl.UseCustomBackColor = true;
                                RtReslbl.TextAlign = Alignment;
                                RtReslbl.BackColor = BackColor;
                                RtReslbl.ForeColor = ForeColor;
                                RtReslbl.FontSize = MetroFramework.MetroLabelSize.Medium;
                                RtReslbl.Text = message;
                            }));
                        }
                        
                        mtbtnBackup.Invoke((MethodInvoker)(() => {
                            mtbtnBackup.Visible = false;
                        }));

                        mtbtnRestore.Invoke((MethodInvoker)(() => {
                            mtbtnRestore.Enabled = true;
                            mtbtnRestore.Visible = true;
                        }));
                    }
                    else
                    {
                        if (mtbtnBackup.Visible)
                        {

                            AddBackupListView(message);
                            AddBackupListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                            if (OverrideListViewForeColor)
                            { AddBackupListView(ListboxMessage, ListViewForeColor); }
                            else
                                AddBackupListView(ListboxMessage, BackColor);

                            AddBackupListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                            if(BckReslbl.IsHandleCreated)
                            {
                                BckReslbl.Invoke((MethodInvoker)(() =>
                                {
                                    BckReslbl.UseCustomForeColor = true;
                                    BckReslbl.UseCustomBackColor = true;
                                    BckReslbl.TextAlign = Alignment;
                                    BckReslbl.BackColor = BackColor;
                                    BckReslbl.ForeColor = ForeColor;
                                    BckReslbl.FontSize = MetroFramework.MetroLabelSize.Medium;
                                    BckReslbl.Text = message;
                                }));
                            }

                            if (mtbtnBackup.IsHandleCreated)
                            {
                                mtbtnBackup.Invoke((MethodInvoker)(() => mtbtnBackup.Enabled = true));
                            }

                        }
                        else
                        {
                            AddRestoreListView(message);
                            AddRestoreListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));


                            if (OverrideListViewForeColor)
                            { AddRestoreListView(ListboxMessage, ListViewForeColor); }
                            else
                                AddRestoreListView(ListboxMessage, BackColor);

                            AddRestoreListView(string.Concat(Enumerable.Repeat("=", ListboxMessage.Length)));

                            if(RtReslbl.IsHandleCreated)
                            {
                                RtReslbl.Invoke((MethodInvoker)(() =>
                                {
                                    RtReslbl.UseCustomForeColor = true;
                                    RtReslbl.UseCustomBackColor = true;
                                    RtReslbl.TextAlign = Alignment;
                                    RtReslbl.BackColor = BackColor;
                                    RtReslbl.ForeColor = ForeColor;
                                    RtReslbl.FontSize = MetroFramework.MetroLabelSize.Medium;
                                    RtReslbl.Text = message;
                                }));
                            }

                            if (mtbtnRestore.IsHandleCreated)
                            {
                                mtbtnRestore.Invoke((MethodInvoker)(() => mtbtnRestore.Enabled = true));
                            }

                        }
                    }

                }));

                mtTabctrl.Invoke((MethodInvoker)(() => {

                    EnableTabPage(mtPagebackup, true);
                    ShowTabPage(mtTabctrl, mtPagebackup);
                    EnableTabPage(mtPagerestore, true);
                    ShowTabPage(mtTabctrl, mtPagerestore);
                    EnableTabPage(mtPagesettings, true);

                }));
            }
                       
            if(mttxtboxUserID.IsHandleCreated)
            {
                mttxtboxUserID.Invoke((MethodInvoker)(() => mttxtboxUserID.Enabled = true));
            }
            
            if(mtbtnCancel.IsHandleCreated)
            {
                mtbtnCancel.Invoke((MethodInvoker)(() => mtbtnCancel.Enabled = false));
            }
        }

        private void ProgressbarMarquee()
        {
            altProgressbar.Invoke((MethodInvoker)(() => {

                altProgressbar.Enabled = true;
                altProgressbar.Visible = true;
                altProgressbar.HideProgressText = true;
                altProgressbar.MarqueeAnimationSpeed = 30;
                altProgressbar.ProgressBarStyle = ProgressBarStyle.Marquee;
                altProgressbar.Update();
            }));
            
        }

        private void StopProgressbarMarquee()
        {
            altProgressbar.Invoke((MethodInvoker)(() => {
                                
                altProgressbar.MarqueeAnimationSpeed = 0;
                altProgressbar.ProgressBarStyle = ProgressBarStyle.Continuous;
                altProgressbar.Update();
            }));

        }

        private void SetProgressBarContinuous(int min, int max, int value)
        {
            altProgressbar.Invoke((MethodInvoker)(() => {

                altProgressbar.ResetText();
                altProgressbar.Minimum = min;
                altProgressbar.Maximum = max;
                altProgressbar.Value = value;
                altProgressbar.Enabled = true;
                altProgressbar.Visible = true;
                altProgressbar.HideProgressText = false;
                altProgressbar.Update();
                altProgressbar.ProgressBarStyle = ProgressBarStyle.Continuous;

            }));
        }

        private void UpdateProgressBarContinuous()
        {
            if(altProgressbar.IsHandleCreated)
            {
                altProgressbar.Invoke((MethodInvoker)(() => {

                    altProgressbar.Increment(1);
                }));
            }
            
        }

        private void HideProgressbar()
        {
            altProgressbar.Invoke((MethodInvoker)(() => {

                altProgressbar.Enabled = false;
                altProgressbar.Visible = false;
                
            }));
        }

        private void InitializeBackupListView()
        {            
            mtlstbxBackup.Invoke((MethodInvoker)(() => {
                mtlstbxBackup.View = View.Details;
                mtlstbxBackup.HeaderStyle = ColumnHeaderStyle.None;
                ColumnHeader h = new ColumnHeader
                {
                    Width = mtlstbxBackup.ClientSize.Width - SystemInformation.VerticalScrollBarWidth
                };
                mtlstbxBackup.Columns.Add(h);

                mtlstbxBackup.Items.Clear();
                mtlstbxBackup.ForeColor = Color.LimeGreen;
                mtlstbxBackup.BackColor = Color.FromArgb(48, 48, 48);
            }));                   
            
        }

        private void AddBackupListView(string Message)
        {
            if(mtlstbxBackup.IsHandleCreated)
            {
                mtlstbxBackup.Invoke((MethodInvoker)(() => {
                    mtlstbxBackup.Items.Add(Message);
                    //mtlstbxBackup.Update();
                }));
            }           

        }

        private void AddBackupListView(string Message, Color ForeColor)
        {
            if(mtlstbxBackup.IsHandleCreated)
            {
                mtlstbxBackup.Invoke((MethodInvoker)(() => {
                    mtlstbxBackup.Items.Add(Message).ForeColor = ForeColor;
                    //mtlstbxBackup.Update();
                }));
            }                      

        }


        private void BackupResponseLabelInitialise()
        {
            if (BckReslbl.IsHandleCreated)
            {
                BckReslbl?.Invoke((MethodInvoker)(() =>
                {
                    BckReslbl.UseCustomBackColor = true;
                    BckReslbl.UseCustomForeColor = true;
                    BckReslbl.ForeColor = Color.FromArgb(203, 252, 142);
                    BckReslbl.BackColor = Color.FromArgb(31, 72, 20);                    
                    BckReslbl.TextAlign = ContentAlignment.MiddleLeft;
                    BckReslbl.FontSize = MetroFramework.MetroLabelSize.Small;                    
                    BckReslbl.Update();
                }));
            }
        }

        private void BackupResponseLabelUpdate(string Message)
        {
            if (BckReslbl.IsHandleCreated)
            {
                BckReslbl?.Invoke((MethodInvoker)(() =>
                {
                    BckReslbl.Text = Message;
                    //BckReslbl.Update();
                }));
            }
        }

        private void RestoreResponseLabelInitialise()
        {
            if (RtReslbl.IsHandleCreated)
            {
                RtReslbl?.Invoke((MethodInvoker)(() =>
                {
                    RtReslbl.UseCustomBackColor = true;
                    RtReslbl.UseCustomForeColor = true;
                    RtReslbl.ForeColor = Color.FromArgb(203, 252, 142);
                    RtReslbl.BackColor = Color.FromArgb(31, 72, 20);                    
                    RtReslbl.TextAlign = ContentAlignment.MiddleLeft;
                    RtReslbl.FontSize = MetroFramework.MetroLabelSize.Small;
                    //RtReslbl.Update();
                }));
            }
        }

        private void RestoreResponseLabelUpdate(string Message)
        {
            if (RtReslbl.IsHandleCreated)
            {
                RtReslbl?.Invoke((MethodInvoker)(() =>
                {
                    RtReslbl.Text = Message;
                    //RtReslbl.Update();
                }));
            }
        }


        private void InitializeRestoreListView()
        {            
            mtlstbxRestore.Invoke((MethodInvoker)(() => {
                mtlstbxRestore.View = View.Details;
                mtlstbxRestore.HeaderStyle = ColumnHeaderStyle.None;
                ColumnHeader h = new ColumnHeader
                {
                    Width = mtlstbxRestore.ClientSize.Width - SystemInformation.VerticalScrollBarWidth
                };
                mtlstbxRestore.Columns.Add(h);

                mtlstbxRestore.Items.Clear();
                mtlstbxRestore.ForeColor = Color.LimeGreen;
                mtlstbxRestore.BackColor = Color.FromArgb(48, 48, 48);
            }));                            

        }

        private void AddRestoreListView(string Message)
        {
            if(mtlstbxRestore.IsHandleCreated)
            {
                mtlstbxRestore.Invoke((MethodInvoker)(() => {
                    mtlstbxRestore.Items.Add(Message);
                   // mtlstbxRestore.Update();

                }));
            }

        }

        private void AddRestoreListView(string Message, Color ForeColor)
        {
            if(mtlstbxRestore.IsHandleCreated)
            {
                mtlstbxRestore.Invoke((MethodInvoker)(() => {
                    mtlstbxRestore.Items.Add(Message).ForeColor = ForeColor;
                    //mtlstbxRestore.Update();
                }));
            }            

        }


        private void InitializeAboutListView()
        {           
            listBoxMain.Invoke((MethodInvoker)(() => {
                listBoxMain.View = View.Details;
                listBoxMain.HeaderStyle = ColumnHeaderStyle.None;
                ColumnHeader h = new ColumnHeader
                {
                    Width = listBoxMain.ClientSize.Width - SystemInformation.VerticalScrollBarWidth
                };
                listBoxMain.Columns.Add(h);

                listBoxMain.Items.Clear();
                //listBoxMain.ForeColor = Color.LimeGreen;
                //listBoxMain.BackColor = Color.FromArgb(48, 48, 48);
                listBoxMain.ForeColor = Color.FromArgb(62, 62, 66);
                listBoxMain.BackColor = Color.White;
            }));
            
        }

        

        private void EnableTabPage(TabPage page, bool enable)
        {
            foreach (Control ctl in page.Controls) ctl.Enabled = enable;
        }

        private void HideTabPage(MetroFramework.Controls.MetroTabControl ctl, MetroFramework.Controls.MetroTabPage page)
        {            
            ctl.HideTab(page);           
        }

        private bool IsTabPageHidden(MetroFramework.Controls.MetroTabControl ctl, MetroFramework.Controls.MetroTabPage page)
        {
            return ctl.IsTabHidden(page) ? true : false;
        }


        private void ShowTabPage(MetroFramework.Controls.MetroTabControl ctl, MetroFramework.Controls.MetroTabPage page)
        {            
            ctl.ShowTab(page);
        }


        #endregion

    }
}

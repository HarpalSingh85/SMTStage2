using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using StateMigrationBackend.LogEngine;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateMigrationEngine;
using StateMigrationBackend.StateMigrationEngine.Interfaces;
using StateMigrationTool;
using StateMigrationBackend.StateRegions;
using System.Timers;

namespace StateMigration.UI
{
    public partial class mainFrom
    {

        #region Backup Helpers                    

        StringBuilder stringBuilder = new StringBuilder();
        
        System.Timers.Timer t1 = null;

        private async void StartBackup(Regions regions, string _user , Person oPerson, string _TargetPath, CancellationToken token)
        {           

            try
            {
                string source = string.Empty;

                if(!string.IsNullOrWhiteSpace(oPerson.CustomHomeDirectory))
                {
                    source = oPerson.CustomHomeDirectory;
                }

                mtlstbxBackup.Items.Clear();
                if (settings.SharedDevices)
                {
                    
                    ISharedResourceHelper regVal = new SharedResourceHelper();
                    regVal.OnNetworkSharesBackupStart += RegVal_OnNetworkSharesBackupStart;
                    regVal.OnNetworkSharesBackupCompleted += RegVal_OnNetworkSharesBackupCompleted;
                    regVal.OnNetworkPrintersBackupStart += RegVal_OnNetworkPrintersBackupStart;
                    regVal.OnNetworkPrintersBackupCompleted += RegVal_OnNetworkPrintersBackupCompleted;
                    regVal.OnNetworkAtomicChange += RegVal_OnNetworkAtomicChange;
                    regVal.OnPrinterAtomicChange += RegVal_OnPrinterAtomicChange;
                    CurrentOperationModel = await regVal.GetResourcesBackupAsync(_user, custompaths, _TargetPath, token);

                    if (custompaths.Count > 0)
                    {
                        CurrentOperationModel.CustomPaths = new System.Collections.Generic.List<string>();
                        CurrentOperationModel.CustomPaths.AddRange(custompaths.ToList());
                    }

                }


                
                IDataBackup dbval = new DataBackup();
                dbval.OnBackupStart += Dbval_OnBackupStart;
                dbval.OnCalculationStart += Dbval_OnCalculationStart;
                dbval.OnBackupComplete += Dbval_OnBackupComplete;
                dbval.OnAtomicCurrentCopyStatus += Dbval_OnAtomicCurrentCopyStatus;
                dbval.OnAtomicTotalCounts += Dbval_OnAtomicTotalCounts;

                //ASYNC METHOD

                switch(settings.BROperationType)
                {
                    case "Auto":
                        settings.ResourceUtilization = settings.ResourceUtilization;
                        break;

                    case "Full":
                        switch(settings.ResourceUtilization)
                        {
                            case 4:
                                settings.ResourceUtilization = 1;
                                break;

                            case 5:
                                settings.ResourceUtilization = 2;
                                break;

                            case 6:
                                settings.ResourceUtilization = 3;
                                break;
                            default:
                                settings.ResourceUtilization = settings.ResourceUtilization;
                                break;
                        }
                        break;

                    case "Difference":
                        switch (settings.ResourceUtilization)
                        {
                            case 1:
                                settings.ResourceUtilization = 4;
                                break;

                            case 2:
                                settings.ResourceUtilization = 5;
                                break;

                            case 3:
                                settings.ResourceUtilization = 6;
                                break;
                            default:
                                settings.ResourceUtilization = settings.ResourceUtilization;
                                break;
                        }
                        break;
                }
                

                switch(settings.ResourceUtilization)
                {
                    case 1:
                        await dbval.BackupAsync(regions, _TargetPath, source, _user, settings.LogOptions, settings.LogType, LogOperation.BackupOperation, token);
                        break;

                    case 2:
                        await dbval.BackupPartialParallelAsync(regions, _TargetPath, source, _user, settings.LogOptions, settings.LogType, LogOperation.BackupOperation, token);
                        break;

                    case 3:
                        await dbval.BackupParallelAsync(regions, _TargetPath, source, _user, settings.LogOptions, settings.LogType, LogOperation.BackupOperation, token);
                        break;

                    case 4:
                        await dbval.DifferenceBackupAsync(regions, _TargetPath, source, _user, settings.LogOptions, settings.LogType, LogOperation.BackupOperation, token);
                        break;

                    case 5:
                        await dbval.DifferenceBackupPartialParallelAsync(regions, _TargetPath, source, _user, settings.LogOptions, settings.LogType, LogOperation.BackupOperation, token);
                        break;

                    case 6:
                        await dbval.DifferenceBackupParallelAsync(regions, _TargetPath, source, _user, settings.LogOptions, settings.LogType, LogOperation.BackupOperation, token);
                        break;

                    default:
                        await dbval.BackupAsync(regions, _TargetPath, source, _user, settings.LogOptions, settings.LogType, LogOperation.BackupOperation, token);
                        break;

                }
                              
                

            }
            catch (OperationCanceledException OEx)
            {
                InitiatlizeUICancelled(OEx.Message);
            }
            catch (Exception Ex)
            {
                InitializeUIException(Ex.Message);
            }

        }

        private void Dbval_OnCalculationStart(object sender, bool e)
        {
            t1 = new System.Timers.Timer();
            stringBuilder.Clear();
            stringBuilder.Append("Calculating");
            t1.Interval = 100;
            t1.Elapsed += T1_Elapsed;
            t1.Start();           

        }

        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (mtlstbxBackup.IsHandleCreated)
            {
                if (stringBuilder.Length > 30)
                {
                    stringBuilder.Replace(".", "");
                }
                stringBuilder.Append(".");
                mtlstbxBackup?.Invoke((MethodInvoker)(() =>
                {                    
                    ListViewItem item = mtlstbxBackup.Items
                                    .Cast<ListViewItem>()
                                    .FirstOrDefault(x => x.Text.Contains("Calculating"));
                    if(item != null)
                    {
                        mtlstbxBackup.Items.Remove(item);
                    }
                    
                    mtlstbxBackup.Items.Add(stringBuilder.ToString());
                    mtlstbxBackup.Items[mtlstbxBackup.Items.Count - 1].EnsureVisible();
                }));
            }

        }




        private void RegVal_OnNetworkSharesBackupStart(object sender, StringEventArgs e)
        {
            t1 = new System.Timers.Timer();
            stringBuilder.Clear();
            stringBuilder.Append("Getting Shares");
            t1.Interval = 100;
            t1.Elapsed += T1_SharesElapsed;
            t1.Start();
            AddBackupListView(stringBuilder.ToString());
            ProgressbarMarquee();
        }

        private void T1_SharesElapsed(object sender, ElapsedEventArgs e)
        {           

            if (mtlstbxBackup.IsHandleCreated)
            {
                if (stringBuilder.Length > 30)
                {
                    stringBuilder.Replace(".", "");
                }                
                stringBuilder.Append(".");

                mtlstbxBackup?.Invoke((MethodInvoker)(() => {
                    
                    ListViewItem item = mtlstbxBackup.Items
                                    .Cast<ListViewItem>()
                                    .FirstOrDefault(x => x.Text.Contains("Getting Shares"));
                    if(item != null)
                    {
                        mtlstbxBackup.Items.Remove(item);
                    }
                    
                    mtlstbxBackup.Items.Add(stringBuilder.ToString());
                    mtlstbxBackup.Items[mtlstbxBackup.Items.Count - 1].EnsureVisible();
                }));
                
            }
           
        }

        private void RegVal_OnNetworkAtomicChange(object sender, StringEventArgs e)
        {
            AddBackupListView(e.Message);
        }

        private void RegVal_OnAtomicNetworkStateChange(object sender, string e)
        {
            ProgressbarMarquee();
        }

        private void RegVal_OnNetworkSharesBackupCompleted(object sender, StringEventArgs e)
        {
            t1.Stop();
            
            if(mtlstbxBackup.IsHandleCreated)
            {
                stringBuilder.Insert(stringBuilder.Length, "  ->Done");
                mtlstbxBackup?.Invoke((MethodInvoker)(() => {
                    
                    ListViewItem item = mtlstbxBackup.Items
                                    .Cast<ListViewItem>()
                                    .FirstOrDefault(x => x.Text.Contains("Getting Shares"));
                    if(item != null)
                    {
                        mtlstbxBackup.Items.Remove(item);
                    }
                    
                    mtlstbxBackup.Items.Insert(0, stringBuilder.ToString());
                    mtlstbxBackup.Items[mtlstbxBackup.Items.Count - 1].EnsureVisible();
                    stringBuilder.Clear();
                }));                

            }
            t1.Dispose();
            StopProgressbarMarquee();
            HideProgressbar();
        }




        private void RegVal_OnNetworkPrintersBackupStart(object sender, StringEventArgs e)
        {
            t1 = new System.Timers.Timer();
            stringBuilder.Clear();
            stringBuilder.Append("Getting Printers");
            t1.Interval = 100;
            t1.Elapsed += T1_PrintersElapsed;
            t1.Start();            
            ProgressbarMarquee();
        }

        private void T1_PrintersElapsed(object sender, ElapsedEventArgs e)
        {
            if (mtlstbxBackup.IsHandleCreated)
            {               
                if (stringBuilder.Length > 30)
                {
                    stringBuilder.Replace(".", "");
                }
                stringBuilder.Append(".");
                mtlstbxBackup?.Invoke((MethodInvoker)(() =>
                {                    
                    ListViewItem item = mtlstbxBackup.Items
                                    .Cast<ListViewItem>()
                                    .FirstOrDefault(x => x.Text.Contains("Getting Printers"));
                    if(item != null)
                    {
                        mtlstbxBackup.Items.Remove(item);
                    }
                    
                    mtlstbxBackup.Items.Add(stringBuilder.ToString());
                    mtlstbxBackup.Items[mtlstbxBackup.Items.Count - 1].EnsureVisible();
                }));
            }            

        }

        private void RegVal_OnPrinterAtomicChange(object sender, StringEventArgs e)
        {
            AddBackupListView(e.Message);
        }

        private void RegVal_OnAtomicPrinterStateChange(object sender, string e)
        {
            ProgressbarMarquee();
        }

        private void RegVal_OnNetworkPrintersBackupCompleted(object sender, StringEventArgs e)
        {
            t1.Stop();
            t1.Dispose();
            if (mtlstbxBackup.IsHandleCreated)
            {
                stringBuilder.Append("  ->Done");
                mtlstbxBackup?.Invoke((MethodInvoker)(() =>
                {
                    mtlstbxBackup.Items.Remove(mtlstbxBackup.Items
                                    .Cast<ListViewItem>()
                                    .LastOrDefault(x => x.Text.Contains("Getting Printers")));
                    
                    ListViewItem item = mtlstbxBackup.Items
                                    .Cast<ListViewItem>()
                                    .LastOrDefault(x => x.Text.Contains("Capturing"));

                    if(item != null)
                    {
                        mtlstbxBackup.Items.Insert(item.Index,stringBuilder.ToString());
                    }
                    else
                        mtlstbxBackup.Items.Add(stringBuilder.ToString());
                    mtlstbxBackup.Items[mtlstbxBackup.Items.Count - 1].EnsureVisible();
                    stringBuilder.Clear();
                }));

            }
            StopProgressbarMarquee();
            HideProgressbar();

        }
        



        private void Dbval_OnBackupStart(object sender, StringEventArgs e)
        {            
            AddBackupListView(e.Message);
        }

        private void Dbval_OnAtomicTotalCounts(object sender, int e)
        {
            t1.Stop();
            t1.Dispose();            

            if (mtlstbxBackup.IsHandleCreated)
            {
                stringBuilder.Append("  ->Done");
                mtlstbxBackup?.Invoke((MethodInvoker)(() =>
                {
                    mtlstbxBackup.Items.Remove(mtlstbxBackup.Items
                                    .Cast<ListViewItem>()
                                    .LastOrDefault(x => x.Text.Contains("Calculating")));
                    
                    mtlstbxBackup.Items.Add(stringBuilder.ToString());
                    mtlstbxBackup.Items[mtlstbxBackup.Items.Count - 1].EnsureVisible();
                    mtlstbxBackup.Update();
                    stringBuilder.Clear();
                }));

            }

            BackupResponseLabelInitialise();
            SetProgressBarContinuous(0,e,0);
        }

        private void Dbval_OnAtomicCurrentCopyStatus(object sender, StringEventArgs e)
        {          
            BackupResponseLabelUpdate($"Copying : {e.Message}");
            UpdateProgressBarContinuous();              

        }

        private void Dbval_OnBackupComplete(object sender, StringEventArgs e)
        {
            UIMessages msg = new UIMessages();        
            InitializeUISuccess(e.Message , msg.Show(7));
            HideProgressbar();
        }
        

        #endregion

        #region BackupTab

        private void BackupResponseLabelSuccess(string _message)
        {
            BckReslbl.UseCustomBackColor = true;
            BckReslbl.UseCustomForeColor = true;
            BckReslbl.BackColor = Color.FromArgb(41, 106, 13);
            BckReslbl.ForeColor = Color.White;
            BckReslbl.Text = _message;
        }

        private void BackupResponseLabelWarning(object _message)
        {
            BckReslbl.UseCustomBackColor = true;
            BckReslbl.UseCustomForeColor = true;
            BckReslbl.BackColor = Color.FromArgb(255, 90, 0);
            BckReslbl.ForeColor = Color.White;
            BckReslbl.Text = _message.ToString();
        }

        private void BackupResponseLabelReset()
        {
            BckReslbl.UseCustomBackColor = false;
            BckReslbl.UseCustomForeColor = false;
            BckReslbl.Text = "";

        }

        #endregion

    }
}

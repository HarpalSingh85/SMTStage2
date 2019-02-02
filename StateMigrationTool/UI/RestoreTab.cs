using System;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using StateMigrationBackend.LogEngine;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateMigrationEngine;
using StateMigrationBackend.StateMigrationEngine.Interfaces;
using StateMigrationTool;
using System.Linq;

namespace StateMigration.UI
{
    public partial class mainFrom
    {
        #region Restore Helpers     
                

        private async void StartRestore(string _user, string _TargetPath, CancellationToken token)
        {
            mtlstbxRestore.Items.Clear();

            try
            {               

                if (settings.SharedDevices)
                {
                    ISharedResourceHelper regVal = new SharedResourceHelper();
                    regVal.OnNetworkPrintersRestoreStart += RegVal_OnNetworkPrintersRestoreStart;
                    regVal.OnNetworkPrintersRestoreCompleted += RegVal_OnNetworkPrintersRestoreCompleted;
                    regVal.OnNetworkSharesRestoreStart += RegVal_OnNetworkSharesRestoreStart;
                    regVal.OnNetworkSharesRestoreCompleted += RegVal_OnNetworkSharesRestoreCompleted;
                    regVal.OnRestoreStateChange += RegVal_OnRestoreStateChange;
                    regVal.OnRestoreStateError += RegVal_OnRestoreStateError;

                    await regVal.SetResourcesAsync(_user, _TargetPath, token);
                }
                                

                IDataRestore dRval = new DataRestore();
                dRval.OnDataRestoreStart += DRval_OnDataRestoreStart;
                dRval.OnDataRestoreComplete += DRval_OnDataRestoreComplete;
                dRval.OnCalculationStart += DRval_OnCalculationStart;
                dRval.OnAtomicCurrentCopyStatus += DRval_OnAtomicCurrentCopyStatus;
                dRval.OnAtomicTotalCounts += DRval_OnAtomicTotalCounts;

                switch (settings.BROperationType)
                {
                    case "Auto":
                        settings.ResourceUtilization = settings.ResourceUtilization;
                        break;

                    case "Full":
                        switch (settings.ResourceUtilization)
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

                switch (settings.ResourceUtilization)
                {
                    case 1:
                        await dRval.RestoreAsync(regions, _TargetPath, _user, settings.LogOptions, settings.LogType, LogOperation.RestoreOperation, token);
                        break;

                    case 2:
                        await dRval.RestorePartialParallelAsync(regions,_TargetPath, _user, settings.LogOptions, settings.LogType, LogOperation.RestoreOperation, token);
                        break;

                    case 3:
                        await dRval.RestoreParallelAsync(regions, _TargetPath, _user, settings.LogOptions, settings.LogType, LogOperation.RestoreOperation, token);
                        break;

                    case 4:
                        await dRval.DifferenceRestoreAsync(regions, _TargetPath, _user, settings.LogOptions, settings.LogType, LogOperation.RestoreOperation, token);
                        break;

                    case 5:
                        await dRval.DifferenceRestorePartialParallelAsync(regions, _TargetPath, _user, settings.LogOptions, settings.LogType, LogOperation.RestoreOperation, token);
                        break;

                    case 6:
                        await dRval.DifferenceRestoreParallelAsync(regions, _TargetPath, _user, settings.LogOptions, settings.LogType, LogOperation.RestoreOperation, token);
                        break;

                    default:
                        await dRval.DifferenceRestoreAsync(regions, _TargetPath, _user, settings.LogOptions, settings.LogType, LogOperation.RestoreOperation, token);
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
        

        private void DRval_OnCalculationStart(object sender, bool e)
        {           
            t1 = new System.Timers.Timer();
            stringBuilder.Clear();
            stringBuilder.Append("Calculating");
            t1.Interval = 100;
            t1.Elapsed += T1_ElapsedRestore;
            t1.Start();
        }

        private void T1_ElapsedRestore(object sender, ElapsedEventArgs e)
        {
            if (mtlstbxRestore.IsHandleCreated)
            {
                if (stringBuilder.Length > 30)
                {
                    stringBuilder.Replace(".", "");
                }
                stringBuilder.Append(".");
                mtlstbxRestore?.Invoke((MethodInvoker)(() =>
                {
                    ListViewItem item = mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .FirstOrDefault(x => x.Text.Contains("Calculating"));
                    if(item != null)
                    {
                        mtlstbxRestore.Items.Remove(item);
                    }
                    
                    mtlstbxRestore.Items.Add(stringBuilder.ToString());
                    mtlstbxRestore.Items[mtlstbxRestore.Items.Count - 1].EnsureVisible();
                    mtlstbxRestore.Update();
                }));
            }
        }



        private void RegVal_OnNetworkPrintersRestoreStart(object sender, StringEventArgs e)
        {
            AddRestoreListView(e.Message);
            t1 = new System.Timers.Timer();
            stringBuilder.Clear();
            stringBuilder.Append("Restoring Printers");
            t1.Interval = 100;
            t1.Elapsed += T1_PrintersRestoreElapsed;
            t1.Start();
            ProgressbarMarquee();
            
        }

        private void T1_PrintersRestoreElapsed(object sender, ElapsedEventArgs e)
        {
            if (mtlstbxRestore.IsHandleCreated)
            {
                if (stringBuilder.Length > 30)
                {
                    stringBuilder.Replace(".", "");
                }
                stringBuilder.Append(".");
                mtlstbxRestore?.Invoke((MethodInvoker)(() =>
                {                    
                    ListViewItem item = mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .FirstOrDefault(x => x.Text.Contains("Restoring Printers"));
                    if(item != null)
                    {
                        mtlstbxRestore.Items.Remove(item);
                    }
                    
                    ListViewItem itm = mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .FirstOrDefault(x => x.Text.Contains("Printers restore initiated"));
                    
                    if(itm != null)
                    {
                        mtlstbxRestore.Items.Insert(itm.Index + 1, stringBuilder.ToString());
                    }
                    
                    mtlstbxRestore.Items[mtlstbxRestore.Items.Count - 1].EnsureVisible();
                    mtlstbxRestore.Update();
                }));
            }
        }

        private void RegVal_OnNetworkPrintersRestoreCompleted(object sender, StringEventArgs e)
        {
            
            t1.Stop();
            t1.Dispose();
            if (mtlstbxRestore.IsHandleCreated)
            {
                stringBuilder.Append("  ->Done");
                mtlstbxRestore?.Invoke((MethodInvoker)(() =>
                {
                    mtlstbxRestore.Items.Remove(mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .LastOrDefault(x => x.Text.Contains("Restoring Printers")));
                    int item = mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .LastOrDefault(x => x.Text.Contains("Printers restore initiated")).Index;

                    mtlstbxRestore.Items.Insert(item + 1, stringBuilder.ToString());
                    mtlstbxRestore.Items[mtlstbxRestore.Items.Count - 1].EnsureVisible();
                    mtlstbxRestore.Update();
                    stringBuilder.Clear();
                }));

            }
            AddRestoreListView(e.Message);
            StopProgressbarMarquee();
            HideProgressbar();
        }

        

        private void RegVal_OnNetworkSharesRestoreStart(object sender, StringEventArgs e)
        {            
            AddRestoreListView(e.Message); t1 = new System.Timers.Timer();
            stringBuilder.Clear();
            stringBuilder.Append("Restoring Shared Drives");
            t1.Interval = 100;
            t1.Elapsed += T1_SharesRestoreElapsed;
            t1.Start();
            ProgressbarMarquee();
        }

        private void T1_SharesRestoreElapsed(object sender, ElapsedEventArgs e)
        {
            if (mtlstbxRestore.IsHandleCreated)
            {
                if (stringBuilder.Length > 30)
                {
                    stringBuilder.Replace(".", "");
                }
                stringBuilder.Append(".");
                mtlstbxRestore?.Invoke((MethodInvoker)(() =>
                {
                    
                    ListViewItem item = mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .FirstOrDefault(x => x.Text.Contains("Restoring Shared Drives"));
                    if(item != null)
                    {
                        mtlstbxRestore.Items.Remove(item);
                    }
                    
                                        
                    ListViewItem itm = mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .FirstOrDefault(x => x.Text.Contains("Network shares restore initiated"));

                    if(itm != null)
                    {
                        mtlstbxRestore.Items.Insert(itm.Index + 1, stringBuilder.ToString());
                    }
                    
                    mtlstbxRestore.Items[mtlstbxRestore.Items.Count - 1].EnsureVisible();
                    mtlstbxRestore.Update();
                }));
            }
        }

        private void RegVal_OnNetworkSharesRestoreCompleted(object sender, StringEventArgs e)
        {
            t1.Stop();
            t1.Dispose();
            if (mtlstbxRestore.IsHandleCreated)
            {
                stringBuilder.Append("  ->Done");
                mtlstbxRestore?.Invoke((MethodInvoker)(() =>
                {
                    mtlstbxRestore.Items.Remove(mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .LastOrDefault(x => x.Text.Contains("Restoring Shared Drives")));
                    int item = mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .LastOrDefault(x => x.Text.Contains("Network shares restore initiated")).Index;

                    mtlstbxRestore.Items.Insert(item + 1, stringBuilder.ToString());
                    mtlstbxRestore.Items[mtlstbxRestore.Items.Count - 1].EnsureVisible();
                    stringBuilder.Clear();
                    mtlstbxRestore.Update();
                }));

            }
            AddRestoreListView(e.Message);
            StopProgressbarMarquee();
            HideProgressbar();
        }



        private void DRval_OnDataRestoreStart(object sender, StringEventArgs e)
        {            
            AddRestoreListView(e.Message);
            //SetProgressBarContinuous();
        }

        private void DRval_OnAtomicTotalCounts(object sender, int e)
        {

            t1.Stop();
            t1.Dispose();

            if (mtlstbxRestore.IsHandleCreated)
            {
                stringBuilder.Append("  ->Done");
                mtlstbxRestore?.Invoke((MethodInvoker)(() =>
                {
                    mtlstbxRestore.Items.Remove(mtlstbxRestore.Items
                                    .Cast<ListViewItem>()
                                    .LastOrDefault(x => x.Text.Contains("Calculating")));

                    mtlstbxRestore.Items.Add(stringBuilder.ToString());
                    mtlstbxRestore.Items[mtlstbxRestore.Items.Count - 1].EnsureVisible();
                    mtlstbxRestore.Update();
                    stringBuilder.Clear();
                }));

            }

            RestoreResponseLabelInitialise();
            SetProgressBarContinuous(0,e,0);
        }

        private void DRval_OnAtomicCurrentCopyStatus(object sender, StringEventArgs e)
        {
            
            RestoreResponseLabelUpdate($"Copying : {e.Message}");
            UpdateProgressBarContinuous();
        }

        private void DRval_OnDataRestoreComplete(object sender, StringEventArgs e)
        {
            UIMessages msg = new UIMessages();
            InitializeUISuccess(e.Message, msg.Show(7));
            HideProgressbar();                       
        }



        private void RegVal_OnRestoreStateError(object sender, StringEventArgs e)
        {            
            InitializeUIException(e.Message);
        }

        private void RegVal_OnRestoreStateChange(object sender, StringEventArgs e)
        {
            ProgressbarMarquee();            
            AddRestoreListView(e.Message);
        }
        
        #endregion

        #region RestoreTab

        private void RestoreResponseLabelSuccess(string _message)
        {
            RtReslbl.UseCustomBackColor = true;
            RtReslbl.UseCustomForeColor = true;
            RtReslbl.BackColor = Color.FromArgb(41, 106, 13);
            RtReslbl.ForeColor = Color.White;
            RtReslbl.Text = _message;
        }

        private void RestoreResponseLabelWarning(object _message)
        {
            RtReslbl.UseCustomBackColor = true;
            RtReslbl.UseCustomForeColor = true;
            RtReslbl.BackColor = Color.FromArgb(255, 90, 0);
            RtReslbl.ForeColor = Color.White;
            RtReslbl.Text = _message.ToString();
        }

        private void RestoreResponseLabelReset()
        {
            RtReslbl.UseCustomBackColor = false;
            RtReslbl.UseCustomForeColor = false;
            RtReslbl.Text = "";
        }

        #endregion
    }
}

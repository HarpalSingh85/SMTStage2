using StateMigrationBackend.Models;
using StateMigrationBackend.StateRegions;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StateMigration.UI
{
    public partial class mainFrom
    {
        #region Global EventHandlers    

        Person objPerson = new Person();

        private async void mttxtboxUserID_TextChanged(object sender, EventArgs e)
        {
            Regions regions = new Regions();
            IUserDetails userdetails = new UserDetails();           
            

            if(!string.IsNullOrWhiteSpace(mttxtboxUserID.Text) && Regex.Match(mttxtboxUserID.Text, "[A-Za-z0-9]+", RegexOptions.CultureInvariant).Success)
            {
                                
                    lstViewDefaultPaths.Items.Clear();

                    try
                    {
                        objPerson = await userdetails.GetUserDetails(Regex.Match(mttxtboxUserID.Text, "[A-Za-z0-9]+", RegexOptions.CultureInvariant).Value);                        
                        DisplayView(objPerson);


                        if (Directory.Exists($@"C:\Users\{mttxtboxUserID.Text}"))
                        {
                            var dirItems = Directory.EnumerateDirectories($@"C:\users\{mttxtboxUserID.Text}", "*.*", SearchOption.TopDirectoryOnly);

                            if (dirItems.Count() > 0)
                            {
                                foreach (var item in dirItems)
                                {
                                    lstViewDefaultPaths.Items.Add(item);
                                }
                            }
                            else
                            {
                                lstViewDefaultPaths.Items.Clear();
                                lstViewDefaultPaths.Items.Add("Nothing found / user profile is empty");

                            }



                        }
                        else
                            lstViewDefaultPaths.Items.Add("Nothing found / user profile is empty");


                    }
                    catch (Exception Ex)
                    {
                        InitializeUIException(Ex.Message);
                    }
                
               
            }
            else
            {
                try
                {
                    
                    objPerson = await userdetails.GetUserDetails(Environment.UserName);
                    if(objPerson !=null)
                    {
                        DisplayView(objPerson);
                    }                   
                                            
                    lstViewDefaultPaths.Items.Clear();
                }
                catch (Exception Ex)
                {
                    InitializeUIException(Ex.Message);
                }

            }            

        }

        private void mtTabctrl_SelectedIndexChanged(object sender, EventArgs e)
        {           

            if(mtTabctrl.SelectedTab == mtPagebackup)
            {
                if(mtlstbxBackup.IsHandleCreated)
                {
                    mtlstbxBackup.Invoke((MethodInvoker)(() =>
                    {

                        if(mtlstbxRestore.Items.Count > 0 && !IsTabPageHidden(mtTabctrl,mtPagerestore))
                        {
                            currentStats = mtlstbxRestore.Items.Cast<ListViewItem>()
                                     .Select(item => item.Text)
                                     .ToList();
                        }
                        

                        mtbtnBackup.Visible = true;
                        mtbtnRestore.Visible = false;

                        if (currentStats.Count != 0  && !currentStats.Contains("Not defined"))
                        {
                            mtlstbxBackup.Items.Clear();

                            foreach (var item in currentStats)
                            {
                                mtlstbxBackup.Items.Add(item);
                            }
                            mtlstbxBackup.Update();

                        }
                        else
                             DisplayView(objPerson);
                        


                    }));
                }
                if (mtPagebackup.IsHandleCreated)
                {
                    mtPagebackup.Invoke((MethodInvoker)(() => mtPagebackup.Update()));
                }


            }
            else if(mtTabctrl.SelectedTab == mtPagerestore)
            {
                if(mtlstbxRestore.IsHandleCreated)
                {
                    mtlstbxRestore.Invoke((MethodInvoker)(() =>
                    {

                        if (mtlstbxBackup.Items.Count > 0 && !IsTabPageHidden(mtTabctrl,mtPagerestore))
                        {
                            currentStats = mtlstbxBackup.Items.Cast<ListViewItem>()
                                     .Select(item => item.Text)
                                     .ToList();
                        }
                        
                        mtbtnBackup.Visible = false;
                        mtbtnRestore.Visible = true;


                        if (currentStats.Count != 0 && !currentStats.Contains("Not defined"))
                        {
                            mtlstbxRestore.Items.Clear();

                            foreach (var item in currentStats)
                            {
                                mtlstbxRestore.Items.Add(item);
                            }

                        }
                        else
                            DisplayView(objPerson);                       


                    }));

                }

                if (mtPagerestore.IsHandleCreated)
                {
                    mtPagerestore.Invoke((MethodInvoker)(() => mtPagerestore.Update()));
                }


            }
            else if(mtTabctrl.SelectedTab == mtPagedefaultpaths)
            {
                if(mtbtnBackup.IsHandleCreated && !IsTabPageHidden(mtTabctrl,mtPagebackup))
                {
                    if (mtlstbxBackup.Items.Count > 0)
                    {
                        currentStats = mtlstbxBackup.Items.Cast<ListViewItem>()
                                 .Select(item => item.Text)
                                 .ToList();
                    }

                    mtbtnBackup.Invoke((MethodInvoker)(() =>
                    {
                        if (mtbtnBackup.Visible) { mtbtnRestore.Visible = false; }
                    }));
                }

                if (mtbtnRestore.IsHandleCreated && !IsTabPageHidden(mtTabctrl, mtPagerestore))
                {

                    if (mtlstbxRestore.Items.Count > 0)
                    {
                        currentStats = mtlstbxRestore.Items.Cast<ListViewItem>()
                                 .Select(item => item.Text)
                                 .ToList();
                    }

                    mtbtnRestore.Invoke((MethodInvoker)(() =>
                    {
                        if (mtbtnRestore.Visible) { mtbtnBackup.Visible = false; }
                    }));
                }

                if (mtPagedefaultpaths.IsHandleCreated)
                {
                    mtPagedefaultpaths.Invoke((MethodInvoker)(() => mtPagedefaultpaths.Update()));

                }

            }
            else if (mtTabctrl.SelectedTab == mtPagesettings)
            {                

                if (mtbtnBackup.IsHandleCreated && !IsTabPageHidden(mtTabctrl, mtPagebackup))
                {
                    if (mtlstbxBackup.Items.Count > 0)
                    {
                        currentStats = mtlstbxBackup.Items.Cast<ListViewItem>()
                                 .Select(item => item.Text)
                                 .ToList();
                    }

                    mtbtnBackup.Invoke((MethodInvoker)(() =>
                    {
                        if (mtbtnBackup.Visible) { mtbtnRestore.Visible = false; }
                    }));
                }

                if (mtbtnRestore.IsHandleCreated && !IsTabPageHidden(mtTabctrl, mtPagerestore))
                {

                    if (mtlstbxRestore.Items.Count > 0)
                    {
                        currentStats = mtlstbxRestore.Items.Cast<ListViewItem>()
                                 .Select(item => item.Text)
                                 .ToList();
                    }

                    mtbtnRestore.Invoke((MethodInvoker)(() =>
                    {
                        if (mtbtnRestore.Visible) { mtbtnBackup.Visible = false; }
                    }));
                }

                if (mtPagesettings.IsHandleCreated)
                {
                    mtPagesettings.Invoke((MethodInvoker)(() => mtPagesettings.Update()));

                }

            }
            else 
            {                
                if(listBoxMain.IsHandleCreated)
                {
                    listBoxMain.Invoke((MethodInvoker)(() =>
                    {
                        listBoxMain.Items.Clear();
                        listBoxMain.Items.Add($"Machine Name     :   {sysdetails.MachineName}");
                        listBoxMain.Items.Add($"Operating System :   {sysdetails.OperatingSystem}");
                        listBoxMain.Items.Add($"Logical CPU :    {sysdetails.LogicalCPUCount} Available");
                        listBoxMain.Items.Add($"Ram Size    :    {sysdetails.RAMSize} Gb");
                    }));
                }
                               

                if (mtbtnBackup.IsHandleCreated && !IsTabPageHidden(mtTabctrl, mtPagebackup))
                {
                    if (mtlstbxBackup.Items.Count > 0)
                    {
                        currentStats = mtlstbxBackup.Items.Cast<ListViewItem>()
                                 .Select(item => item.Text)
                                 .ToList();
                    }

                    mtbtnBackup.Invoke((MethodInvoker)(() =>
                    {
                        if (mtbtnBackup.Visible) { mtbtnRestore.Visible = false; }
                    }));
                }

                if (mtbtnRestore.IsHandleCreated && !IsTabPageHidden(mtTabctrl, mtPagerestore))
                {

                    if (mtlstbxRestore.Items.Count > 0)
                    {
                        currentStats = mtlstbxRestore.Items.Cast<ListViewItem>()
                                 .Select(item => item.Text)
                                 .ToList();
                    }

                    mtbtnRestore.Invoke((MethodInvoker)(() =>
                    {
                        if (mtbtnRestore.Visible) { mtbtnBackup.Visible = false; }
                    }));
                }

                if (mtPageAbout.IsHandleCreated)
                {
                    mtPageAbout.Invoke((MethodInvoker)(() => mtPageAbout.Update()));
                }

            }

        }

        #endregion

    }
}

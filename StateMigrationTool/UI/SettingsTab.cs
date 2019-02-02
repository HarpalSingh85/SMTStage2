using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using StateMigrationBackend.StateRegions;
using StateMigrationBackend.LogEngine;

namespace StateMigration.UI
{
    public partial class mainFrom
    {        

        #region SettingsTab        

        private void SettingEnable(bool _enabled)
        {
            mtTbResUtilization.Enabled = _enabled;
            mtTbLogOption.Enabled = _enabled;
            mtcboLogtype.Enabled = _enabled;
            mtAutoProfileDetect.Enabled = _enabled;
            mtAutoProfilePathDetect.Enabled = _enabled;
            mtAutoSaveSharedDevices.Enabled = _enabled;
            mtcboBRtype.Enabled = _enabled;
        }

        private async void OptimizeSettings(bool optimize)
        {
            IAutoDetection detect = new AutoDetection();
            if (optimize)
            {                
                mtTbResUtilization.Value = await detect.GetOptimizedValue(); 
                SetResoureUtlizationLevel(mtTbResUtilization.Value);
                mtTbLogOption.Checked = optimize;
                mtcboLogtype.SelectedIndex = 0;
                mtAutoProfileDetect.Checked = optimize;
                mtAutoProfilePathDetect.Checked = optimize;
                mtAutoSaveSharedDevices.Checked = optimize;
                mtcboBRtype.SelectedIndex = 0;
            }
        }

        private void SetResoureUtlizationLevel(int value)
        {

            switch (value)
            {
                case 1:
                    
                        mtReslbl?.Invoke((MethodInvoker)(() =>
                        {
                            mtReslbl.BackColor = Color.FromArgb(82, 177, 40);
                            mtReslbl.ForeColor = Color.White;
                            mtReslbl.Text = "Very low";
                            mtReslbl.Update();

                        }));                    
                    
                    break;

                case 2:                    
                        mtReslbl?.Invoke((MethodInvoker)(() =>
                        {
                            mtReslbl.BackColor = Color.FromArgb(77, 164, 29);
                            mtReslbl.ForeColor = Color.White;
                            mtReslbl.Text = "Low";
                            mtReslbl.Update();
                        }));
                    
                    
                    break;

                case 3:
                    
                        mtReslbl?.Invoke((MethodInvoker)(() =>
                        {
                            mtReslbl.BackColor = Color.FromArgb(60, 142, 23);
                            mtReslbl.ForeColor = Color.White;
                            mtReslbl.Text = "Low-Medium";
                            mtReslbl.Update();
                        }));
                    
                    break;

                case 4:
                    
                        mtReslbl?.Invoke((MethodInvoker)(() =>
                        {
                            mtReslbl.BackColor = Color.FromArgb(48, 123, 28);
                            mtReslbl.ForeColor = Color.White;
                            mtReslbl.Text = "Medium";                            
                            mtReslbl.Update();
                        }));
                    
                    
                    break;

                case 5:
                    
                        mtReslbl?.Invoke((MethodInvoker)(() =>
                        {
                            mtReslbl.BackColor = Color.FromArgb(41, 106, 13);
                            mtReslbl.ForeColor = Color.White;
                            mtReslbl.Text = "High";
                            mtReslbl.Update();
                        }));
                    
                   
                    break;

                case 6:
                    
                        mtReslbl?.Invoke((MethodInvoker)(() =>
                        {
                            mtReslbl.BackColor = Color.FromArgb(41, 106, 13);
                            mtReslbl.ForeColor = Color.White;
                            mtReslbl.Text = "Very High";
                            mtReslbl.Update();

                        }));
                    
                    
                    break;
            }
        }

        private void mtTbResUtilization_Scroll(object sender, ScrollEventArgs e)
        {
            SetResoureUtlizationLevel(mtTbResUtilization.Value);
            settings.ResourceUtilization = mtTbResUtilization.Value;
        }

        private void mtcboLogtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((KeyValuePair<string, string>)mtcboLogtype.SelectedItem).Value.Equals("Auto"))
            {
                SettingResponseLabelInfo($"Logging is now system managed.");
                settings.LogType = LogState.Overwrite;
            }
            else if (((KeyValuePair<string, string>)mtcboLogtype.SelectedItem).Value.Equals("Append"))
            {
                SettingResponseLabelInfo($"Logs will be appended.");
                settings.LogType = LogState.Append;
            }
            else
            {
                SettingResponseLabelInfo($"Logs will be overwritten.");
                settings.LogType = LogState.Overwrite;
            }
            var _logtype = ((KeyValuePair<string, string>)mtcboLogtype.SelectedItem).Value;

        }

        private void mtcboBRtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((KeyValuePair<string, string>)mtcboBRtype.SelectedItem).Value.Equals("Auto"))
            {
                SettingResponseLabelInfo($"Backup / Restore operations are now system managed.");
                settings.BROperationType = "Auto";
            }
            else if (((KeyValuePair<string, string>)mtcboBRtype.SelectedItem).Value.Equals("Full"))
            {
                SettingResponseLabelInfo($"Full Data Backup / Restore Operations will be performed.");
                settings.BROperationType = "Full";
            }
            else
            {
                SettingResponseLabelInfo($"Only files && directory changes will be Backed up / Restored during respective operation.");
                settings.BROperationType = "Difference";
            }
            var _brtype = ((KeyValuePair<string, string>)mtcboBRtype.SelectedItem).Value;
        }

        private void mtAutoProfileDetect_CheckedChanged(object sender, EventArgs e)
        {
            mtbtnSettingUserProfile.Enabled = (!mtAutoProfileDetect.Checked) ? true : false;

            if (!mtAutoProfileDetect.Checked)
            {
                SettingResponseLabelInfo($"User Profile Detection Overridden, Select user profile.");
            }
            else
                SettingResponseLabelInfo($"Auto User Profile Detection enabled.");

        }

        private void mtAutoProfilePathDetect_CheckedChanged(object sender, EventArgs e)
        {
            mtbtnSettingUserProfilePath.Enabled = (!mtAutoProfilePathDetect.Checked) ? true : false;

            if (!mtAutoProfilePathDetect.Checked)
            {
                SettingResponseLabelInfo($"User Profile Path detection overridden, Select User Profile Path.");
            }
            else
                SettingResponseLabelInfo($"Auto User Profile Path detection enabled.");
        }
                
        private void mtTbAIOverride_CheckedChanged(object sender, EventArgs e)
        {
            if (mtTbAIOverride.Checked)
            {
                SettingResponseLabelSuccess($"Auto Detection enabled, all app settings are optimzed.");
                OptimizeSettings(mtTbAIOverride.Checked);
            }
            else
            {
                SettingResponseLabelDanger($"Auto Detection disabled, irregular changes may cause system to slow down during operation.");
                settings.ResourceUtilization = mtTbResUtilization.Value;                
            }


            var _res = (mtTbAIOverride.Checked) ? false : true;
            SettingEnable(_res);
        }        

        private void mtTbLogOption_CheckedChanged(object sender, EventArgs e)
        {
            if (mtTbLogOption.Checked)
            {
                settings.LogOptions = LogOptions.Enabled;
                SettingResponseLabelInfo("Error logging Enabled.");
            }
            else
            {
                settings.LogOptions = LogOptions.Disabled;
                SettingResponseLabelInfo("Error logging Disabled.");

            }
                
              
                
        }

        private void mtAutoSaveSharedDevices_CheckedChanged(object sender, EventArgs e)
        {
            if (!mtAutoSaveSharedDevices.Checked)
            {
                settings.SharedDevices = false;
                SettingResponseLabelInfo($"App will not save / restore Networks and Printers Shares.");
            }
            else
            {
                settings.SharedDevices = true;
                SettingResponseLabelInfo($"App will automatically save / restore Networks and Printers Shares.");
            }
                
        }

        private void mtbtnSettingUserProfile_Click(object sender, EventArgs e)
        {
            fbdUserProfile.Description = "Select User Profile";            
            fbdUserProfile.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fbdUserProfile.ShowDialog() == DialogResult.OK)
            {                
                objPerson.CustomHomeDirectory = fbdUserProfile.SelectedPath;
                SettingResponseLabelInfo($"Selected Source : {objPerson.CustomHomeDirectory}");
                DisplayView(objPerson);
            }
        }

        private void mtbtnSettingUserProfilePath_Click(object sender, EventArgs e)
        {            
            fbdUserProfilePath.Description = "Select User Profile Path";
            fbdUserProfilePath.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fbdUserProfilePath.ShowDialog() == DialogResult.OK)
            {
                Regions regions = new Regions();
                objPerson.CustomProfilePathDirectory = fbdUserProfilePath.SelectedPath;
                regions.SetPath(objPerson.CustomProfilePathDirectory);
                SettingResponseLabelInfo($"Selected Destination : {objPerson.CustomProfilePathDirectory}");
                DisplayView(objPerson);
            }
        }



        #endregion

        #region Settings Response Label

        private void SettingResponseLabelReset()
        {
            blSettingResponse.UseCustomBackColor = false;
            blSettingResponse.UseCustomForeColor = false;
            blSettingResponse.Text = "";
            if (mtTbAIOverride.Checked)
            {
                OptimizeSettings(mtTbAIOverride.Checked);
                SettingResponseLabelSuccess($"Auto Detection enabled, all app settings are optimzed.");
            }
            else
            {
                SettingResponseLabelDanger($"Auto Detection disabled, irregular changes may cause system to slow down during operation.");
            }

        }

        private void SettingResponseLabelSuccess(string _message)
        {
            blSettingResponse.UseCustomBackColor = true;
            blSettingResponse.UseCustomForeColor = true;
            blSettingResponse.BackColor = Color.FromArgb(72, 134, 21);
            blSettingResponse.ForeColor = Color.White;
            blSettingResponse.Text = _message;
        }

        private void SettingResponseLabelWarning(string _message)
        {
            blSettingResponse.UseCustomBackColor = true;
            blSettingResponse.UseCustomForeColor = true;
            blSettingResponse.BackColor = Color.FromArgb(255, 90, 0);
            blSettingResponse.ForeColor = Color.White;
            blSettingResponse.Text = _message;
        }

        private void SettingResponseLabelDanger(string _message)
        {
            blSettingResponse.UseCustomBackColor = true;
            blSettingResponse.UseCustomForeColor = true;
            blSettingResponse.BackColor = Color.FromArgb(255, 46, 0);
            blSettingResponse.ForeColor = Color.White;
            blSettingResponse.Text = _message;
        }

        private void SettingResponseLabelInfo(string _message)
        {
            blSettingResponse.UseCustomBackColor = true;
            blSettingResponse.UseCustomForeColor = true;
            blSettingResponse.BackColor = Color.FromArgb(25, 137, 8);
            blSettingResponse.ForeColor = Color.White;
            blSettingResponse.Text = _message;
        }

        #endregion

        #region MouseGestures

        private void lstViewDefaultPaths_MouseEnter(object sender, EventArgs e)
        {
            mtdefaultReslbl.UseCustomBackColor = true;
            mtdefaultReslbl.UseCustomForeColor = true;
            mtdefaultReslbl.ForeColor = Color.White;
            mtdefaultReslbl.BackColor = Color.FromArgb(72, 134, 21);
            mtdefaultReslbl.Text = "Right click anywhere to add or remove items.";
        }

        private void lstViewDefaultPaths_MouseLeave(object sender, EventArgs e)
        {

            mtdefaultReslbl.UseCustomBackColor = true;
            mtdefaultReslbl.UseCustomForeColor = true;
            mtdefaultReslbl.ForeColor = Color.White;
            mtdefaultReslbl.BackColor = Color.FromArgb(211, 159, 23);
            mtdefaultReslbl.Text = "Pre-Defined paths for Backup && Restore operations.";
        }


        private void mtsettinglblAI_MouseEnter(object sender, EventArgs e)
        {
            SettingResponseLabelSuccess($"Automatically optimizes app settings to optimal level.");
        }

        private void mtsettinglblAI_MouseLeave(object sender, EventArgs e)
        {
            SettingResponseLabelReset();
        }


        private void mtSettinglblLO_MouseEnter(object sender, EventArgs e)
        {
            SettingResponseLabelInfo($"Enables / Disables error logging during operation.");

        }

        private void mtSettinglblLO_MouseLeave(object sender, EventArgs e)
        {
            SettingResponseLabelReset();
        }

        private void mtSettinglblLT_MouseEnter(object sender, EventArgs e)
        {
            SettingResponseLabelInfo($"Choose whether to append or overwrite error logs.");
        }

        private void mtSettinglblLT_MouseLeave(object sender, EventArgs e)
        {
            SettingResponseLabelReset();
        }

        private void mtSettinglblRU_MouseEnter(object sender, EventArgs e)
        {
            SettingResponseLabelInfo($"Sets Multithreading Level for optimal system resource utilization.");
        }

        private void mtSettinglblRU_MouseLeave(object sender, EventArgs e)
        {
            SettingResponseLabelReset();
        }

        private void mtlblUserprof_MouseEnter(object sender, EventArgs e)
        {
            SettingResponseLabelInfo($"Enable / Disable automatic user profile detection.");
        }

        private void mtlblUserprof_MouseLeave(object sender, EventArgs e)
        {
            SettingResponseLabelReset();
        }

        private void mtlblProfpath_MouseEnter(object sender, EventArgs e)
        {
            SettingResponseLabelInfo($"Enable / Disable automatic user profile path (NAS share) detection.");
        }

        private void mtlblProfpath_MouseLeave(object sender, EventArgs e)
        {
            SettingResponseLabelReset();
        }

        private void mtlblSharesave_MouseEnter(object sender, EventArgs e)
        {
            SettingResponseLabelInfo($"Choose whether to save / not save Network Shares && Printers.");
        }

        private void mtlblSharesave_MouseLeave(object sender, EventArgs e)
        {
            SettingResponseLabelReset();
        }

        private void mtlblbr_MouseEnter(object sender, EventArgs e)
        {
            SettingResponseLabelInfo($"Choose between Auto, Full && Difference copy algorithms.");
        }

        private void mtlblbr_MouseLeave(object sender, EventArgs e)
        {
            SettingResponseLabelReset();
        }
        #endregion
    }
}

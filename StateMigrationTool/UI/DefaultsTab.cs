using System.Linq;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateRegions;
using System.Windows.Forms;
using System;

namespace StateMigration.UI
{
    public partial class mainFrom
    {
        #region ContextMenu

        private void addItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fbdDefaults.Description = "Select Item";
            fbdDefaults.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fbdDefaults.ShowDialog() == DialogResult.OK)
            {
                lstViewDefaultPaths.Items.Add(fbdDefaults.SelectedPath);
                lstViewDefaultPaths.Update();
                regions.SetPath(fbdDefaults.SelectedPath);
                custompaths.Add(fbdDefaults.SelectedPath);
                //SettingResponseLabelInfo($"Selected Source : {objPerson.CustomHomeDirectory}");

            }
        }

        private void removeItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (lstViewDefaultPaths.SelectedIndex != -1)
            {
                for (int i = lstViewDefaultPaths.SelectedItems.Count - 1; i >= 0; i--)
                    lstViewDefaultPaths.Items.Remove(lstViewDefaultPaths.SelectedItems[i]);
            }

        }

        #endregion            

    }
}


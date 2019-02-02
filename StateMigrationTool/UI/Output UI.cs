using System.Linq;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateRegions;
using System.Windows.Forms;

namespace StateMigration.UI
{
    public partial class mainFrom
    {
        #region Output UI

        private void DisplayView(Person _customLoad)
        {
            if (mtTabctrl.SelectedTab == mtPagebackup)
            {
                DisplayBackupListView(_customLoad);
            }
            else if (mtTabctrl.SelectedTab == mtPagerestore)
            {
                DisplayRestoreListView(_customLoad);
            }
            else
            {
                DisplayBackupListView(_customLoad);
                DisplayRestoreListView(_customLoad);
            }
            
        }

        private void DisplayBackupListView(Person _customLoad)
        {

            mtlstbxBackup.Items.Clear();

            AddBackupListView((_customLoad.Name) ?? "Username : Not Identified");
            AddBackupListView((_customLoad.Company) ?? "Company : Not Identified");

            if (!string.IsNullOrWhiteSpace(_customLoad.HomeDirectory))
            {
                AddBackupListView($"Home Directory : {_customLoad.HomeDirectory}");
            }
            else
                AddBackupListView("Home Directory : Not Identified");

            if (!string.IsNullOrWhiteSpace(_customLoad.ProfilePathDirectory))
            {
                AddBackupListView($"Profile Directory : {_customLoad.ProfilePathDirectory}");
            }
            else
                AddBackupListView("Profile Directory : Not Identified");
                        
            if (!string.IsNullOrWhiteSpace(_customLoad.CustomHomeDirectory))
            {
                _customLoad.CustomHomeDirectory = _customLoad.CustomHomeDirectory;

                AddBackupListView(string.Concat(Enumerable.Repeat("=", ($"Selected Home Directory : {_customLoad.CustomHomeDirectory}").Length)));
                AddBackupListView($"Selected Home Directory : {_customLoad.CustomHomeDirectory}");
                AddBackupListView(string.Concat(Enumerable.Repeat("=", ($"Selected Home Directory : {_customLoad.CustomHomeDirectory}").Length)));
            }

            if (!string.IsNullOrWhiteSpace(_customLoad.CustomProfilePathDirectory))
            {
                _customLoad.CustomProfilePathDirectory = _customLoad.CustomProfilePathDirectory;

                AddBackupListView(string.Concat(Enumerable.Repeat("=", ($"Selected Profile Directory : {_customLoad.CustomProfilePathDirectory}").Length)));
                AddBackupListView($"Selected Profile Directory : {_customLoad.CustomProfilePathDirectory}");
                AddBackupListView(string.Concat(Enumerable.Repeat("=", ($"Selected Profile Directory : {_customLoad.CustomProfilePathDirectory}").Length)));
            }

            //currentStats.Clear();
            //currentStats.AddRange(mtlstbxBackup.Items.Cast<ListViewItem>().Select(x => x.Text));
            
        }

        private void DisplayRestoreListView(Person _customLoad)
        {
            mtlstbxRestore.Items.Clear();

            AddRestoreListView((_customLoad.Name) ?? "Username : Not Identified");
            AddRestoreListView((_customLoad.Company) ?? "Company : Not Identified");

            if (!string.IsNullOrWhiteSpace(_customLoad.HomeDirectory))
            {
                AddRestoreListView($"Home Directory : {_customLoad.HomeDirectory}");
            }
            else
                AddRestoreListView("Home Directory : Not Identified");


            if (!string.IsNullOrWhiteSpace(_customLoad.ProfilePathDirectory))
            {
                AddRestoreListView($"Profile Directory : {_customLoad.ProfilePathDirectory}");
            }
            else
                AddRestoreListView("Profile Directory : Not Identified");

            if (!string.IsNullOrWhiteSpace(_customLoad.CustomHomeDirectory))
            {
                _customLoad.CustomHomeDirectory = _customLoad.CustomHomeDirectory;
                AddRestoreListView(string.Concat(Enumerable.Repeat("=", ($"Selected Home Directory : {_customLoad.CustomHomeDirectory}").Length)));
                AddRestoreListView($"Selected Home Directory : {_customLoad.CustomHomeDirectory}");
                AddRestoreListView(string.Concat(Enumerable.Repeat("=", ($"Selected Home Directory : {_customLoad.CustomHomeDirectory}").Length)));
            }

            if (!string.IsNullOrWhiteSpace(_customLoad.CustomProfilePathDirectory))
            {
                _customLoad.CustomProfilePathDirectory = _customLoad.CustomProfilePathDirectory;

                AddRestoreListView(string.Concat(Enumerable.Repeat("=", ($"Selected Profile Directory : {_customLoad.CustomProfilePathDirectory}").Length)));
                AddRestoreListView($"Selected Profile Directory : {_customLoad.CustomProfilePathDirectory}");
                AddRestoreListView(string.Concat(Enumerable.Repeat("=", ($"Selected Profile Directory : {_customLoad.CustomProfilePathDirectory}").Length)));
            }

            //currentStats.Clear();
            //currentStats.AddRange(mtlstbxRestore.Items.Cast<ListViewItem>().Select(x => x.Text));

        }

       #endregion

    }
}


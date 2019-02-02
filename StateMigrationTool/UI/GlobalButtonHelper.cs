
using StateMigrationBackend.Models;
using System.Windows.Forms;

namespace StateMigration.UI
{
    public partial class mainFrom
    {
        #region GlobalButtonHelpers

        private void btnInitializetimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsPersonvalid(objPerson))
            {
                if (mtbtnBackup.IsHandleCreated)
                    mtbtnBackup.Invoke((MethodInvoker)(() => mtbtnBackup.Enabled = true));
                if (mtbtnRestore.IsHandleCreated)
                    mtbtnRestore.Invoke((MethodInvoker)(() => mtbtnRestore.Enabled = true));
                btnInitializetimer.Stop();

            }

        }

        private bool IsPersonvalid(Person _perobj)
        {
            if (_perobj != null)
            {
                return (string.IsNullOrWhiteSpace(_perobj.Name) && string.IsNullOrWhiteSpace(_perobj.Company)) ? false : true;
            }
            else
                return false;

        }

        #endregion

    }
}


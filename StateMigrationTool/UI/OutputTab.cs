using System.Linq;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateRegions;
using System.Windows.Forms;

namespace StateMigration.UI
{
    public partial class mainFrom
    {
        private void mtabtlinklblEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:UK.IT.WintelServerSupport@fly.virgin.com");
        }

        private void mtabtlinklblSkype_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("sip:<Harpal.Singh@fly.virgin.com>");
        }

    }
}


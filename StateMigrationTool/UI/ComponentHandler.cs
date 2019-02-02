using MetroFramework.Controls;
using System.Windows.Forms;

namespace StateMigration.UI
{
    
    public class CustomPictureBox : PictureBox
    {
        public CustomPictureBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var parms = base.CreateParams;
        //        parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN                
        //        return parms;
        //    }
        //}

    }

    public class CustomListView : ListView
    {
        public CustomListView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var parms = base.CreateParams;
        //        parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN                
        //        return parms;
        //    }
        //}

    }

    public class CustomGroupBox : GroupBox
    {
        public CustomGroupBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var parms = base.CreateParams;
        //        parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
        //        return parms;
        //    }
        //}

    }

    public class CustomTextbox : MetroTextBox
    {
        public CustomTextbox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
                return parms;
            }
        }
    }


    public class CustomListBox : ListBox
    {
        public CustomListBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
    }


}

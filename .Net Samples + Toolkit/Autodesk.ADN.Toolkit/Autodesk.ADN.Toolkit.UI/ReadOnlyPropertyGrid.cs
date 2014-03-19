using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Autodesk.ADN.Toolkit.UI
{
    public partial class ReadOnlyPropertyGrid :
       PropertyGrid
    {
        private bool isReadOnly = true;

        public bool ReadOnly
        {
            get
            {
                return isReadOnly;
            }
            set
            {
                isReadOnly = value;
                SetObjectAsReadOnly();
            }
        }

        protected override void OnSelectedObjectsChanged(EventArgs e)
        {
            SetObjectAsReadOnly();
            base.OnSelectedObjectsChanged(e);
        }

        private void SetObjectAsReadOnly()
        {
            if (SelectedObject != null)
            {
                TypeDescriptor.AddAttributes(
                    SelectedObject,
                    new Attribute[] 
                    { 
                        new ReadOnlyAttribute(isReadOnly) 
                    });

                Refresh();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ReadOnlyPropertyGrid
            // 
            this.HelpVisible = false;
            this.ResumeLayout(false);

        }
    }   
}

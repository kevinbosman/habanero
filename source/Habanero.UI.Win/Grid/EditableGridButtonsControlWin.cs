using System;
using System.Collections.Generic;
using System.Text;
using Habanero.UI.Base;
using Habanero.UI.Base;
using Habanero.UI.Win;

namespace Habanero.UI.Win
{
    /// <summary>
    /// Provides a set of buttons for use on an <see cref="IEditableGridControl"/>.
    /// By default, Save and Cancel buttons are available.
    /// </summary>
    public class EditableGridButtonsControlWin : ButtonGroupControlWin, IEditableGridButtonsControl
    {
        /// <summary>
        /// Fires when the Save button is clicked
        /// </summary>
        public event EventHandler SaveClicked;

        /// <summary>
        /// Fires when the Cancel button is clicked
        /// </summary>
        public event EventHandler CancelClicked;


        public EditableGridButtonsControlWin(IControlFactory controlFactory)
            : base(controlFactory)
        {
            IButton cancelButton = AddButton("Cancel", FireCancelButtonClicked);
            cancelButton.Visible = true;
            IButton saveButton = AddButton("Save", FireSaveButtonClicked);
            saveButton.Visible = true;
        }

        void FireCancelButtonClicked(object sender, EventArgs e)
        {
            if (CancelClicked != null)
            {
                this.CancelClicked(this, new EventArgs());
            }
        }
        void FireSaveButtonClicked(object sender, EventArgs e)
        {
            if (SaveClicked != null)
            {
                this.SaveClicked(this, new EventArgs());
            }
        }
    }
}

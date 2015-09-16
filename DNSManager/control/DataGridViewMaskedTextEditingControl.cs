using System;
using System.Windows.Forms;

namespace DNSManager.control
{
    /// <summary>
    ///     DataGridViewMaskedTextEditingControl is the MaskedTextBox that is hosted
    ///     in a DataGridViewMaskedTextColumn.
    /// </summary>
    public class DataGridViewMaskedTextEditingControl : MaskedTextBox, IDataGridViewEditingControl
    {
        #region Constructor

        public DataGridViewMaskedTextEditingControl()
        {
            Mask = string.Empty;
        }

        #endregion

        #region MaskedTextBox event

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            EditingControlValueChanged = true;
            if (EditingControlDataGridView != null)
            {
                EditingControlDataGridView.CurrentCell.Value = Text;
            }
        }

        #endregion

        #region Fields

        #endregion

        #region Interface's properties

        public DataGridView EditingControlDataGridView { get; set; }

        public object EditingControlFormattedValue
        {
            get { return Text; }
            set
            {
                if (value is string)
                    Text = (string) value;
            }
        }

        public int EditingControlRowIndex { get; set; }

        public bool EditingControlValueChanged { get; set; }

        public Cursor EditingPanelCursor => Cursor;

        public bool RepositionEditingControlOnValueChange => false;

        #endregion

        #region Interface's methods

        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            //	get the current cell to use the specific mask string
            var cell = EditingControlDataGridView.CurrentCell as DataGridViewMaskedTextCell;
            if (cell != null)
            {
                Mask = cell.Mask;
            }
        }

        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            //  Note: In a DataGridView, one could prefer to change the row using
            //	the up/down keys.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                    return true;
                default:
                    return false;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
                SelectAll();
            else
            {
                SelectionStart = 0;
                SelectionLength = 0;
            }
        }

        #endregion
    }
}
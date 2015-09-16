using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DNSManager.control
{
    /// <summary>
    ///     DataGridViewMaskedTextColumn hosts a DGV-MaskedTextCell collection
    ///     containing a Mask property.
    /// </summary>
    [ToolboxBitmap(typeof (MaskedTextBox))]
    public class DataGridViewMaskedTextColumn : DataGridViewColumn
    {
        #region Fields

        private static Type columnType = typeof (DataGridViewMaskedTextColumn);

        #endregion

        #region Mask property

        /// <summary>
        ///     Input String that rules the possible input values in each cell of the column.
        /// </summary>
        [Category("Masking")]
        public string Mask
        {
            get
            {
                if (MaskedTextCellTemplate == null)
                {
                    throw new InvalidOperationException("DataGridViewColumn: CellTemplate required");
                }
                return MaskedTextCellTemplate.Mask;
            }
            set
            {
                if (Mask == value) return;
                // If the mask is changed, the cell template has to be changed,
                // and each cell of the column has to be adapted.
                MaskedTextCellTemplate.Mask = value;
                if (DataGridView == null) return;
                var rows = DataGridView.Rows;
                var count = rows.Count;
                for (var i = 0; i < count; i++)
                {
                    var cell = rows.SharedRow(i).Cells[Index] as DataGridViewMaskedTextCell;
                    if (cell != null)
                    {
                        cell.Mask = value;
                    }
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Standard constructor without arguments
        /// </summary>
        public DataGridViewMaskedTextColumn()
            : this(string.Empty)
        {
        }

        /// <summary>
        ///     Constructor using a Mask string
        /// </summary>
        /// <param name="maskString">Mask string used in the EditingControl</param>
        public DataGridViewMaskedTextColumn(string maskString)
            : base(new DataGridViewMaskedTextCell())
        {
            SortMode = DataGridViewColumnSortMode.Automatic;
            Mask = maskString;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Converting the current DGV-MaskedTextColumn instance to a string value.
        /// </summary>
        /// <returns>
        ///     String value of the instance containing the name
        ///     and column index.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder(0x40);
            builder.Append("DataGridViewMaskedTextColumn { Name=");
            builder.Append(Name);
            builder.Append(", Index=");
            builder.Append(Index.ToString());
            builder.Append(" }");
            return builder.ToString();
        }

        /// <summary>
        ///     Creates a copy of a DGV-MaskedTextColumn containing the DGV-Column properties.
        /// </summary>
        /// <returns>Instance of a DGV-MaskedTextColumn using the Mask string.</returns>
        public override object Clone()
        {
            var col = (DataGridViewMaskedTextColumn) base.Clone();
            col.Mask = Mask;
            col.CellTemplate = (DataGridViewMaskedTextCell) CellTemplate.Clone();
            return col;
        }

        #endregion

        #region Derived properties

        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if ((value != null) && !(value is DataGridViewMaskedTextCell))
                {
                    throw new InvalidCastException(
                        "DataGridView: WrongCellTemplateType, must be DataGridViewMaskedTextCell");
                }
                base.CellTemplate = value;
            }
        }

        [DefaultValue(1)]
        public new DataGridViewColumnSortMode SortMode
        {
            get { return base.SortMode; }
            set { base.SortMode = value; }
        }

        private DataGridViewMaskedTextCell MaskedTextCellTemplate => (DataGridViewMaskedTextCell) CellTemplate;

        #endregion
    }
}
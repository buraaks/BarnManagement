using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BarnManagement.UI.CustomControls
{
    public class DataGridViewProgressBarColumn : DataGridViewColumn
    {
        public DataGridViewProgressBarColumn()
        {
            this.CellTemplate = new DataGridViewProgressBarCell();
        }
    }

    public class DataGridViewProgressBarCell : DataGridViewTextBoxCell
    {
        public DataGridViewProgressBarCell()
        {
            this.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object? value, object? formattedValue, string? errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts & ~DataGridViewPaintParts.ContentForeground);

            if (value is int || value is double || value is float)
            {
                double progressVal = System.Convert.ToDouble(value);
                if (progressVal < 0) progressVal = 0;
                if (progressVal > 100) progressVal = 100;

                float percentage = (float)(progressVal / 100.0);
                
                // İlerleme çubuğunu çiz
                graphics.FillRectangle(Brushes.LightGreen, cellBounds.X + 2, cellBounds.Y + 2, (int)(percentage * (cellBounds.Width - 4)), cellBounds.Height - 4);
                
                // Üstüne metni çiz
                string text = $"{progressVal:F0} %";
                SizeF textSize = graphics.MeasureString(text, cellStyle.Font);
                float textX = cellBounds.X + (cellBounds.Width - textSize.Width) / 2;
                float textY = cellBounds.Y + (cellBounds.Height - textSize.Height) / 2;
                
                graphics.DrawString(text, cellStyle.Font, Brushes.Black, textX, textY);
            }
            else
            {
                // Sayı değilse metin çiz
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }
    }
}

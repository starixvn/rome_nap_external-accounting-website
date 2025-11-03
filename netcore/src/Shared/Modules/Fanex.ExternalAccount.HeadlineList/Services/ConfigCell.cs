namespace Fanex.ExternalAccount.HeadlineList.Services
{
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    public class ConfigCell
    {
        public string DefaultFontName { get; }
        public byte DefaultFontHeight { get; }
        public short SuspendedColor { get; }
        public short ClosedByAdminColor { get; }
        public short ClosedByUplineColor { get; }
        public short ClosedByArchiveColor { get; }
        public short DisableColor { get; }

        public ICellStyle DisableCellStyle { get; }
        public ICellStyle SuspendedCellStyle { get; }
        public ICellStyle ClosedByAdminCellStyle { get; }
        public ICellStyle ClosedByUplineCellStyle { get; }
        public ICellStyle ClosedByArchiveCellStyle { get; }
        public ICellStyle DefaultCellStyle { get; }

        //left
        public ICellStyle DisableCellStyle_Center { get; }
        public ICellStyle SuspendedCellStyle_Center { get; }
        public ICellStyle ClosedByAdminCellStyle_Center { get; }
        public ICellStyle ClosedByUplineCellStyle_Center { get; }
        public ICellStyle ClosedByArchiveCellStyle_Center { get; }
        public ICellStyle DefaultCellStyle_Center { get; }
        
        public ICellStyle HeadlineCellStyle { get; }
        public ICellStyle NumberCellStyle { get; }

        public ConfigCell(HSSFWorkbook workbook)
        {
            DefaultFontName = "Tahoma";
            DefaultFontHeight = 9;
            var palette = workbook.GetCustomPalette();
            SuspendedColor = palette.FindSimilarColor(255, 204, 153).Indexed;
            ClosedByAdminColor = palette.FindSimilarColor(153, 153, 255).Indexed;
            ClosedByUplineColor = palette.FindSimilarColor(255, 102, 0).Indexed;
            ClosedByArchiveColor = palette.FindSimilarColor(150, 150, 150).Indexed;
            DisableColor = palette.FindSimilarColor(148, 189, 94).Indexed;

            SuspendedCellStyle = FillColor(workbook, SuspendedColor, HorizontalAlignment.Left);
            ClosedByAdminCellStyle = FillColor(workbook, ClosedByAdminColor, HorizontalAlignment.Left);
            ClosedByUplineCellStyle = FillColor(workbook, ClosedByUplineColor, HorizontalAlignment.Left);
            ClosedByArchiveCellStyle = FillColor(workbook, ClosedByArchiveColor, HorizontalAlignment.Left);
            DisableCellStyle = FillColor(workbook, DisableColor, HorizontalAlignment.Left);
            DefaultCellStyle = BaseStyle(workbook,HorizontalAlignment.Left, VerticalAlignment.Center);
            HeadlineCellStyle = BaseStyle(workbook,HorizontalAlignment.Left, VerticalAlignment.Top);

            SuspendedCellStyle_Center = FillColor(workbook, SuspendedColor, HorizontalAlignment.Center);
            ClosedByAdminCellStyle_Center = FillColor(workbook, ClosedByAdminColor, HorizontalAlignment.Center);
            ClosedByUplineCellStyle_Center = FillColor(workbook, ClosedByUplineColor, HorizontalAlignment.Center);
            ClosedByArchiveCellStyle_Center = FillColor(workbook, ClosedByArchiveColor, HorizontalAlignment.Center);
            DisableCellStyle_Center = FillColor(workbook, DisableColor, HorizontalAlignment.Center);
            DefaultCellStyle_Center = BaseStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center);
            
            NumberCellStyle = BaseStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Top);
        }

        private ICellStyle BaseStyle(HSSFWorkbook workbook, HorizontalAlignment align, VerticalAlignment vertical)
        {
            var style = workbook.CreateCellStyle();

            style.SetFont(DefaultFontStyle(workbook));

            style.VerticalAlignment = vertical;
            style.Alignment = align;
            DefaultBorderStyle(ref style);

            style.WrapText = true;

            return style;
        }

        private ICellStyle FillColor(HSSFWorkbook workbook, short color, HorizontalAlignment align)
        {
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.CloneStyleFrom(BaseStyle(workbook, align, VerticalAlignment.Center));
            cellStyle.FillForegroundColor = color;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.Alignment = align;
            return cellStyle;
        }

        private IFont DefaultFontStyle(HSSFWorkbook workbook)
        {
            var font = workbook.CreateFont();
            font.FontName = DefaultFontName;
            font.FontHeightInPoints = DefaultFontHeight;
            font.Color = HSSFColor.Black.Index;
            return font;
        }

        private void DefaultBorderStyle(ref ICellStyle style)
        {
            var borderColorIndex = HSSFColor.Black.Index;
            style.BottomBorderColor = borderColorIndex;
            style.BorderBottom = BorderStyle.Thin;
            style.TopBorderColor = borderColorIndex;
            style.BorderTop = BorderStyle.Thin;
            style.LeftBorderColor = borderColorIndex;
            style.BorderLeft = BorderStyle.Thin;
            style.RightBorderColor = borderColorIndex;
            style.BorderRight = BorderStyle.Thin;
        }

        public ICellStyle HeaderStyle(HSSFWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();
            var headerFont = DefaultFontStyle(workbook);
            headerFont.IsBold = true;
            style.SetFont(headerFont);


            var palette = workbook.GetCustomPalette();
            var colorIndex = palette.FindSimilarColor(131, 202, 255).Indexed;

            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            DefaultBorderStyle(ref style);

            style.FillForegroundColor = colorIndex;
            style.FillPattern = FillPattern.SolidForeground;

            return style;
        }
    }
}
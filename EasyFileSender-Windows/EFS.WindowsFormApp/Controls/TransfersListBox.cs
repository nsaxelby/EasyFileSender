using EFS.Shared.EventModels;
using EFS.Utilities;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EFS.WindowsFormApp.Controls
{
    public partial class TransfersListBox : ListBox
    {
        public TransfersListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = 75;
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                IFileTransferStatus ftsObj;
                if (DesignMode)
                {
                    // Depends on what type you want to see in the designer

                    //ftsObj = new SendFileTransferStatus()
                    //{
                    //    Complete = false,
                    //    DestinationIP = "192.168.0.1",
                    //    FileSizeBytes = 951619276,
                    //    FileName = "FileExample.mov",
                    //    Successful = false,
                    //    TransferID = Guid.NewGuid(),
                    //    DateTimeStarted = DateTime.UtcNow.AddSeconds(-10),
                    //    TransferredSizeBytes = 271809638
                    //};

                    ftsObj = new IncomingFileTransferStatus()
                    {
                        Complete = false,
                        SourceIP = "123.123.123.123",
                        FileSizeBytes = 951619276,
                        FileName = "FileExample.mov",
                        Successful = false,
                        TransferID = Guid.NewGuid(),
                        DateTimeStarted = DateTime.UtcNow.AddSeconds(-10),
                        TransferredSizeBytes = 271809638
                    };
                }
                else
                {
                    if (Items[e.Index].GetType() == typeof(SendFileTransferStatus))
                    {
                        ftsObj = (SendFileTransferStatus)Items[e.Index];
                    }
                    else if(Items[e.Index].GetType() == typeof(IncomingFileTransferStatus))
                    {
                        ftsObj = (IncomingFileTransferStatus)Items[e.Index];
                    }
                    else
                    {
                        ftsObj = null;
                    }
                }

                if (ftsObj != null)
                {
                    Color backgroundColorObj;
                    if (ftsObj.GetType() == typeof(IncomingFileTransferStatus))
                    {
                        backgroundColorObj = StaticColors.lighterBlueColor;
                    }
                    else
                    {
                        backgroundColorObj = StaticColors.lightGreyColor;
                    }

                    DrawItemState drawItemState = e.State;
                    // This prevents selections
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        // BackColor is not a settable property, so we need to new a DrawItemEventArgs
                        drawItemState = drawItemState ^ DrawItemState.Selected;
                    }

                    e = new DrawItemEventArgs(e.Graphics,
                          e.Font,
                          e.Bounds,
                          e.Index,
                          drawItemState,
                          Color.Black,
                          backgroundColorObj);

                    e.DrawBackground();

                    // Draw A Border
                    PointF[] borderLines = new PointF[5];
                    // Bottom Left
                    borderLines[0] = new PointF(e.Bounds.Left, e.Bounds.Bottom);
                    // Bottom Right
                    borderLines[1] = new PointF(e.Bounds.Right - 1, e.Bounds.Bottom);
                    // TopRight
                    borderLines[2] = new PointF(e.Bounds.Right - 1, e.Bounds.Top);
                    // TopLeft
                    borderLines[3] = new PointF(e.Bounds.Left, e.Bounds.Top);
                    // BottomLeft
                    borderLines[4] = new PointF(e.Bounds.Left, e.Bounds.Bottom);
                    e.Graphics.DrawLines(new Pen(Color.Black), borderLines);

                    const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;

                    // File Name
                    // Label
                    Rectangle fileNameLabelRect = e.Bounds;
                    fileNameLabelRect.X += 3;
                    fileNameLabelRect.Y += 5;
                    string textToDisplay = "File:";
                    fileNameLabelRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                    fileNameLabelRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                    TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, fileNameLabelRect, e.ForeColor, flags);
                    // Value
                    Rectangle fileNameValueRect = e.Bounds;
                    fileNameValueRect.X += 60;
                    fileNameValueRect.Y = fileNameLabelRect.Y;
                    textToDisplay = Path.GetFileName(ftsObj.FileName);
                    fileNameValueRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                    fileNameValueRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                    TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, fileNameValueRect, e.ForeColor, flags);

                    // Speed
                    // Label
                    Rectangle speedLabelRect = e.Bounds;
                    speedLabelRect.X = fileNameLabelRect.X;
                    speedLabelRect.Y = fileNameLabelRect.Y + fileNameLabelRect.Height + 1;
                    textToDisplay = "Speed:";
                    speedLabelRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                    speedLabelRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                    TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, speedLabelRect, e.ForeColor, flags);
                    // Value
                    Rectangle speedValueRect = e.Bounds;
                    speedValueRect.X = fileNameValueRect.X;
                    speedValueRect.Y = fileNameValueRect.Y + fileNameValueRect.Height + 1;
                    textToDisplay = Path.GetFileName(DrawingUIUtilities.ConvertBytesPerSecondToMbpsString(ftsObj.SpeedBytesPerSecond));
                    speedValueRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                    speedValueRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                    TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, speedValueRect, e.ForeColor, flags);

                    // Source IP ( If recieving panel )
                    if (ftsObj.GetType() == typeof(IncomingFileTransferStatus))
                    {
                        // Label
                        Rectangle sourceLabelRect = e.Bounds;
                        sourceLabelRect.X = fileNameLabelRect.X + speedLabelRect.Width + speedValueRect.Width + 80;
                        sourceLabelRect.Y = speedValueRect.Y;
                        textToDisplay = "Source IP:";
                        sourceLabelRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                        sourceLabelRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                        TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, sourceLabelRect, e.ForeColor, flags);
                        // Value
                        Rectangle sourceValueRect = e.Bounds;
                        sourceValueRect.X = sourceLabelRect.X + sourceLabelRect.Width + 5;
                        sourceValueRect.Y = speedValueRect.Y;
                        textToDisplay = ((IncomingFileTransferStatus)ftsObj).SourceIP;
                        sourceValueRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                        sourceValueRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                        TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, sourceValueRect, e.ForeColor, flags);
                    }

                    // Progress Text
                    // Label
                    Rectangle progressLabelRect = e.Bounds;
                    progressLabelRect.X = fileNameLabelRect.X;
                    progressLabelRect.Y = speedLabelRect.Y + speedLabelRect.Height + 1;
                    textToDisplay = "Progress:";
                    progressLabelRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                    progressLabelRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                    TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, progressLabelRect, e.ForeColor, flags);
                    // Value
                    Rectangle progressValueRect = e.Bounds;
                    progressValueRect.X = fileNameValueRect.X;
                    progressValueRect.Y = speedValueRect.Y + speedValueRect.Height + 1;
                    textToDisplay = DrawingUIUtilities.ConvertProgressIntoProgressString(ftsObj.TransferredSizeBytes, ftsObj.FileSizeBytes, ftsObj.Progress);
                    progressValueRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                    progressValueRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                    TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, progressValueRect, e.ForeColor, flags);

                    // Failed message
                    if (ftsObj.Exception != null)
                    {
                        // Label
                        Rectangle errorLabelRect = e.Bounds;
                        errorLabelRect.X = fileNameLabelRect.X + speedLabelRect.Width + speedValueRect.Width + 80;
                        errorLabelRect.Y = speedValueRect.Y + speedValueRect.Height + 1;
                        textToDisplay = "Failed:";
                        errorLabelRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                        errorLabelRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                        TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, errorLabelRect, Color.DarkRed, flags);
                        // Value
                        Rectangle errorValueRect = e.Bounds;
                        errorValueRect.X = errorLabelRect.X + errorLabelRect.Width + 1;
                        errorValueRect.Y = errorLabelRect.Y;
                        textToDisplay = ftsObj.Exception.Message;
                        errorValueRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                        errorValueRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                        TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, errorValueRect, e.ForeColor, flags);
                    }
                    else if(ftsObj.Complete && ftsObj.Successful)
                    {
                        // Label
                        Rectangle errorLabelRect = e.Bounds;
                        errorLabelRect.X = fileNameLabelRect.X + speedLabelRect.Width + speedValueRect.Width + 80;
                        errorLabelRect.Y = speedValueRect.Y + speedValueRect.Height + 1;
                        textToDisplay = "Complete";
                        errorLabelRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                        errorLabelRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                        TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, errorLabelRect, Color.Green, flags);
                    }
                    else
                    {
                        // Label
                        Rectangle errorLabelRect = e.Bounds;
                        errorLabelRect.X = fileNameLabelRect.X + speedLabelRect.Width + speedValueRect.Width + 80;
                        errorLabelRect.Y = speedValueRect.Y + speedValueRect.Height + 1;
                        textToDisplay = "Processing";
                        errorLabelRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                        errorLabelRect.Height = DrawingUIUtilities.GetHeightFromFontText(e.Font, textToDisplay, e);
                        TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, errorLabelRect, Color.Orange, flags);
                    }

                    // Progress Bar
                    // Background
                    Rectangle progressBarFullRect = e.Bounds;
                    progressBarFullRect.X += 5;
                    progressBarFullRect.Y = progressLabelRect.Y + progressLabelRect.Height + 2;
                    progressBarFullRect.Width -= 10;
                    progressBarFullRect.Height = 15;
                    SolidBrush darkGreyBrush = new SolidBrush(StaticColors.darkGreyColor);                    
                    e.Graphics.FillRectangle(darkGreyBrush, progressBarFullRect);
                    // Completed
                    if (progressBarFullRect.Width >= 5)
                    {
                        int widthInProgressBar = DrawingUIUtilities.GetPixelsWidthToDraw(progressBarFullRect.Width, ftsObj.Progress);

                        Rectangle progressBarCurrentProgressRect = progressBarFullRect;
                        progressBarCurrentProgressRect.Width = widthInProgressBar;
                        progressBarCurrentProgressRect.Height = 15;

                        SolidBrush darkBlueBrush = new SolidBrush(StaticColors.darkBlueColor);
                        e.Graphics.FillRectangle(darkBlueBrush, progressBarCurrentProgressRect);
                    }
                }
            }
        }

        // Credit to prevent flickering
        // http://yacsharpblog.blogspot.com/2008/07/listbox-flicker.html
        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);
            if (this.Items.Count > 0)
            {
                for (int i = 0; i < this.Items.Count; ++i)
                {
                    System.Drawing.Rectangle irect = this.GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                            irect, i,
                            DrawItemState.Default, this.ForeColor,
                            this.BackColor));
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }
    }
}

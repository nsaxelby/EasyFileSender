using EFS.Global.Models;
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
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                FileTransferStatus ftsObj = new FileTransferStatus();
                if (DesignMode)
                {
                    ftsObj = new FileTransferStatus()
                    {
                        Complete = false,
                        DestinationIP = "192.168.0.1",
                        FileSizeBytes = 951619276,
                        SourceFile = "FileExample.mov",
                        Successful = false,
                        TransferID = Guid.NewGuid(),
                        DateTimeStarted = DateTime.UtcNow.AddSeconds(-10),
                        TransferredSizeBytes = 271809638
                    };
                }
                else
                {
                    if (Items[e.Index].GetType() == typeof(FileTransferStatus))
                    {
                        ftsObj = (FileTransferStatus)Items[e.Index];
                    }
                    else
                    {
                        ftsObj = null;
                    }
                }

                if (ftsObj != null)
                {
                    Color backgroundColorObj = StaticColors.lightGreyColor;
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
                    textToDisplay = Path.GetFileName(ftsObj.SourceFile);
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
    }
}

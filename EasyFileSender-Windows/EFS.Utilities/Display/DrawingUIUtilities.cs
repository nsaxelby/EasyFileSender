using System;
using System.Drawing;
using System.Windows.Forms;

namespace EFS.Utilities
{
    public static class DrawingUIUtilities
    {
        public static int GetWidthFromFontText(Font stringFont, string stringToMeasure, DrawItemEventArgs e)
        {
            SizeF stringSize = e.Graphics.MeasureString(stringToMeasure, stringFont);
            return (int)Math.Round(stringSize.Width);
        }

        public static int GetHeightFromFontText(Font stringFont, string stringToMeasure, DrawItemEventArgs e)
        {
            SizeF stringSize = e.Graphics.MeasureString(stringToMeasure, stringFont);
            return (int)Math.Round(stringSize.Height);
        }

        public static string ConvertBytesPerSecondToMbpsString(double bytesPerSeconds)
        {
            // Div 8, to turn it into bits ( small b ) rather than Bytes ( big B )
            double mbpsValue = (bytesPerSeconds * 8) / 1000 / 1000;
            return mbpsValue.ToString("00.00") + " Mbps";
        }

        public static string ConvertProgressIntoProgressString(long bytesDoneSoFar, long totalFileSizeBytes, double progressPercentage)
        {
            // If it's under 2 MB, show as KBs
            if (totalFileSizeBytes < 1024 * 1024 * 2)
            {
                double doneSoFarKB = bytesDoneSoFar / 1024;
                double totalFileSizeKB = totalFileSizeBytes  / 1024;
                return doneSoFarKB.ToString("0.0") + "/" + totalFileSizeKB.ToString("0.0") + " KB (" + progressPercentage.ToString("0") + "%)";
            }
            else
            {
                // MB, file size, so we div by 1024 rather than 1k ( as with network speeds )
                double doneSoFarMB = bytesDoneSoFar / 1024 / 1024;
                double totalFileSizeMB = totalFileSizeBytes / 1024 / 1024;
                return doneSoFarMB.ToString("0.0") + "/" + totalFileSizeMB.ToString("0.0") + " MB (" + progressPercentage.ToString("0") + "%)";
            }
        }

        public static int GetPixelsWidthToDraw(int pixelsParentBar, double percentage)
        {
            double onePercentPixels = pixelsParentBar / (double)100;
            double pixelsWidth = onePercentPixels * percentage;
            int pixelsRounded = (int)(Math.Round(pixelsWidth));
            return pixelsRounded;
        }
    }
}

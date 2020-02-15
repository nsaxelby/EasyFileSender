using EFS.Global.Enums;
using EFS.Utilities;
using EFS.WindowsFormApp.Controls;
using EFS.WindowsFormApp.ViewModels;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace EFS.WindowsFormApp
{
    public partial class ClientListBox : ListBox
    {
        public ClientListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = 18;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;

            if (e.Index >= 0)
            {
                ClientInfoViewModelListItem ciObj = new ClientInfoViewModelListItem();
                if (DesignMode)
                {
                    if(e.Index % 2 == 0)
                    {
                        ciObj.ClientType = ClientTypeEnum.windows.ToString();
                    }
                    else
                    {
                        ciObj.ClientType = ClientTypeEnum.mac.ToString();
                    }
                    
                    ciObj.IpAddress = "192.168.0.10" + e.Index.ToString();
                    ciObj.Version = VersionNumberEnum.v1.ToString();
                }
                else
                {
                    if (Items[e.Index].GetType() == typeof(ClientInfoViewModelListItem))
                    {
                        ciObj = (ClientInfoViewModelListItem)Items[e.Index];
                    }
                    else
                    {
                        ciObj = null;
                    }
                }

                if (ciObj != null)
                {
                    //if the item state is selected them change the back color 
                    Color backgroundColorObj;
                    DrawItemState drawItemState = e.State;
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        if (ciObj.IsSelfClient)
                        {
                            backgroundColorObj = StaticColors.lighterBlueColor;
                        }
                        else
                        {
                            // BackColor is not a settable property, so we need to new a DrawItemEventArgs
                            backgroundColorObj = StaticColors.yellowColor;
                        }
                        drawItemState = drawItemState ^ DrawItemState.Selected;
                    }
                    else if (ciObj.IsSelfClient)
                    {
                        backgroundColorObj = StaticColors.darkGreyColor;
                    }
                    else
                    {
                        backgroundColorObj = StaticColors.lightGreyColor;
                    }

                    e = new DrawItemEventArgs(e.Graphics,
                          e.Font,
                          e.Bounds,
                          e.Index,
                          drawItemState,
                          Color.Black,
                          backgroundColorObj);

                    e.DrawBackground();

                    // IP Text
                    var textRect = e.Bounds;
                    textRect.X += 1;
                    string textToDisplay = ciObj.IsSelfClient ? "You: " + ciObj.IpAddress : ciObj.IpAddress;
                    textRect.Width = DrawingUIUtilities.GetWidthFromFontText(e.Font, textToDisplay, e);
                    TextRenderer.DrawText(e.Graphics, textToDisplay, e.Font, textRect, e.ForeColor, flags);

                    // Icon
                    string iconString = "windows-32-ico.png";
                    if(ciObj.ClientType == ClientTypeEnum.mac.ToString())
                    {
                        iconString = "apple-32.png";
                    }
                    Assembly asm = Assembly.GetExecutingAssembly();
                    int pixelsIconWHP = textRect.Height - 2;
                    Point pnt = new Point(e.Bounds.Width - 12 - pixelsIconWHP, e.Bounds.Y+1);
                    Image img = new Bitmap(Image.FromStream(ExtractResourceFile(asm, iconString)), new Size(pixelsIconWHP, pixelsIconWHP));
                    e.Graphics.DrawImage(img, pnt);                    
                }
            }
        }



        // Source: https://stackoverflow.com/questions/23010910/how-to-retrieve-a-jpg-image-using-getmanifestresourcestream-method
        public static Stream ExtractResourceFile(Assembly assembly, String fileName)
        {
            // get all embedded resource file names including namespace
            string[] fileNames = assembly.GetManifestResourceNames();

            string resourceName = null;
            string temp = "." + fileName.ToUpper();
            foreach (var item in fileNames)
            {
                if (item.ToUpper().EndsWith(temp))
                {
                    resourceName = item;
                }
            }
            if (resourceName == null)
            {
                throw new Exception("Embedded resource [" + fileName + "] not found");
            }

            // get stream
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception("Embedded resource [" + resourceName + "] could not be opened.");
            }
            return stream;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewSocketUI.Helper
{
    internal class MessageHelperSTB
    {
        /// <summary>
        /// 在RichTextBox元件上顯示訊息
        /// </summary>
        /// <param name="sMessage">要顯示的訊息</param>
        /// <param name="sColor">要顯示的顏色 Red異常 Purple警告 其它使用者自訂  </param>
        /// <param name="ShowrichTextBox">要顯示的位置元件</param>
        public static void SendrichTextBox(string sMessage, Color sColor, RichTextBox ShowRichTextBox)
        {

            ShowRichTextBox.ReadOnly = false;


            ShowRichTextBox.Focus();
            ShowRichTextBox.Select(ShowRichTextBox.TextLength, 0);
            ShowRichTextBox.ScrollToCaret();

            Clipboard.Clear();
            Bitmap bmp = new Bitmap(Properties.Resources.Info);
            if (sColor == Color.Red)
            {
                bmp = new Bitmap(Properties.Resources.Error);
            }
            else if (sColor == Color.Purple)
            {
                bmp = new Bitmap(Properties.Resources.Warn);
            }
            Clipboard.SetImage(bmp);
            ShowRichTextBox.Paste();

            ShowRichTextBox.SelectionColor = sColor;
            //Font font = new Font("新細明體", 14);

            //ShowRichTextBox.SelectionFont = font;




            ShowRichTextBox.AppendText("   " + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "   " + sMessage + Environment.NewLine);

            ShowRichTextBox.ReadOnly = true;

        }

    }
}

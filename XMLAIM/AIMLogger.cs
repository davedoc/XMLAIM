using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XMLAIM
{
    class AIMLogger
    {
        TextBox tb;

        public AIMLogger(TextBox textbox)
        {
            tb = textbox;
            tb.Clear();
            tb.Visible = false;
        }

        public void log(string message)
        {
            if (tb.Visible == true)
            {
                tb.Text = tb.Text + "\r\n" + message;
            }
            else
            {
                tb.Visible = true;
                tb.Text = message;
            }
        }
    }
}

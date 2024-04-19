using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenStem
{
    public class globalKeyHandler : IMessageFilter
    {
        public event Action<Keys> KeyPressed;

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_KEYDOWN = 0x0100;
            if (m.Msg == WM_KEYDOWN)
            {
                Keys keyData = (Keys)(int)m.WParam;
                KeyPressed?.Invoke(keyData);
            }
            return false; // Allow normal processing of the message
        }
    }
}

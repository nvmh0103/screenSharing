using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace screenSharing
{
    class Click
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int flags, int dx, int dy, int data, int extraInfo);
        [Flags]
        public enum mouseEventFlags
        {
            LEFTDOWN=0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00000002,
            RIGHTDOWN = 0x00000008,
            RIGHTUP =  0x00000010,
            MOUSEWHEEL=0x00000800
            
        }
        public void leftClick(Point p)
        {
            Cursor.Position = p;
            mouse_event((int)(mouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            mouse_event((int)(mouseEventFlags.LEFTUP), 0, 0, 0, 0);
        }
        public void rightClick(Point p)
        {
            Cursor.Position = p;
            mouse_event((int)(mouseEventFlags.RIGHTDOWN), 0, 0, 0, 0);
            mouse_event((int)(mouseEventFlags.RIGHTUP), 0, 0, 0, 0);
        }
        public void dobuleClick(Point p)
        {
            Cursor.Position = p;
            mouse_event((int)(mouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            mouse_event((int)(mouseEventFlags.LEFTUP), 0, 0, 0, 0);
            mouse_event((int)(mouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            mouse_event((int)(mouseEventFlags.LEFTUP), 0, 0, 0, 0);
        }
        public void holdClick(Point p)
        {
            Cursor.Position = p;
            mouse_event((int)(mouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
        }
        public void wheelUp()
        {
            mouse_event((int)(mouseEventFlags.MOUSEWHEEL),0,0,120,0);
        }
        public void wheelDown()
        {
            mouse_event((int)(mouseEventFlags.MOUSEWHEEL), 0, 0, -120, 0);
        }
    }
}

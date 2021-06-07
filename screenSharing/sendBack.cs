using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace screenSharing
{   
    [Serializable]
    class sendBack
    {
        private Point mouse;
        private bool isLeftClicked;
        private bool isDoubleClicked;
        private bool isRightClicked;
        sendBack()
        {

        }
        public sendBack(Point mouse, bool isClicked,bool isDoubleClicked,bool isRightClicked)
        {
            this.mouse = mouse;
            this.isLeftClicked = isClicked;
            this.isDoubleClicked = isDoubleClicked;
            this.isRightClicked = isRightClicked;
        }
        public Point getMouse()
        {
            return this.mouse;
        }
        public bool getLeftClick()
        {
            return this.isLeftClicked;
        }
        public bool getDoubleClick()
        {
            return this.isDoubleClicked;
        }
        public bool getRightClick()
        {
            return this.isRightClicked;
        }
    }
}

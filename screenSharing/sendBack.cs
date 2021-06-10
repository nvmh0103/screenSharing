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
        private bool isHoldClicked;
        sendBack()
        {

        }
        public sendBack(Point mouse, bool isClicked,bool isDoubleClicked,bool isRightClicked,bool isHoldClicked)
        {
            this.mouse = mouse;
            this.isLeftClicked = isClicked;
            this.isDoubleClicked = isDoubleClicked;
            this.isRightClicked = isRightClicked;
            this.isHoldClicked = isHoldClicked;
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
        public bool getHoldClick()
        {
            return this.isHoldClicked;
        }
    }
}

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
        private bool isWheelUp;
        private bool isWheelDown;
        sendBack()
        {

        }
        public sendBack(Point mouse, bool isClicked, bool isDoubleClicked, bool isRightClicked, bool isHoldClicked, bool isWheelUp, bool isWheelDown)
        {
            this.mouse = mouse;
            this.isLeftClicked = isClicked;
            this.isDoubleClicked = isDoubleClicked;
            this.isRightClicked = isRightClicked;
            this.isHoldClicked = isHoldClicked;
            this.isWheelUp = isWheelUp;
            this.isWheelDown = isWheelDown;
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
        public bool getWheelUp()
        {
            return this.isWheelUp;
        }
        public bool getWheelDown()
        {
            return this.isWheelDown;
        }
    }
}

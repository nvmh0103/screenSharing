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
        private string res;
        private bool isLeftClicked;
        private bool isDoubleClicked;
        private bool isRightClicked;
        private bool isHoldClicked;
        private bool isWheelUp;
        private bool isWheelDown;
        private string keyboard;
        sendBack()
        {

        }
        public sendBack(Point mouse, string res,bool isClicked, bool isDoubleClicked, bool isRightClicked, bool isHoldClicked, bool isWheelUp, bool isWheelDown,string keyboard)
        {
            this.mouse = mouse;
            this.res = res;
            this.isLeftClicked = isClicked;
            this.isDoubleClicked = isDoubleClicked;
            this.isRightClicked = isRightClicked;
            this.isHoldClicked = isHoldClicked;
            this.isWheelUp = isWheelUp;
            this.isWheelDown = isWheelDown;
            this.keyboard = keyboard;
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
        public string getKeyBoard()
        {
            return this.keyboard;
        }
        
        public string getRes()
        {
            return this.res;
        }
    }
}

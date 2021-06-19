using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace screenSharing
{
    [Serializable()]
    class Data
    {
        int DataType;
        object ObjectData;


        public Data(int type, string text)
        {
            DataType = type;
            ObjectData = text;
        }

        public Data(int type, Bitmap bitmap)
        {
            DataType = type;
            ObjectData = bitmap;
        }
        
        public int GetDataType()
        {
            return DataType;
        }

        public object GetData()
        {
            return ObjectData;
        }
    }
}

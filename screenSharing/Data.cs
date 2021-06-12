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
        string TextData;
        Bitmap BitmapData;
        string Filename;
        byte[] FileData;

        public Data(int type, string text)
        {
            DataType = type;
            TextData = text;
        }

        public Data(int type, Bitmap bitmap)
        {
            DataType = type;
            BitmapData = bitmap;
        }

        public Data(int type, byte[] data, string name)
        {
            DataType = type;
            FileData = data;
            Filename = name;
        }

        public int GetDataType()
        {
            return DataType;
        }

        public string GetTextData()
        {
            return TextData;
        }

        public Bitmap GetBitmapData()
        {
            return BitmapData;
        }

        public byte[] GetFileData()
        {
            return FileData;
        }

        public string GetFilename()
        {
            return Filename;
        }
    }
}

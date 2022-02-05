using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class ImageData
    {
        public string imageName;
        public string fileLocation;
        public int[] colorIntensity;
        public int[] colorCode;
        public int[] features;

        public ImageData()
        {
            fileLocation = "";
            imageName = "";
            colorIntensity = new int[25];
            colorCode = new int[64];
            features = new int[colorCode.Length + colorIntensity.Length];
        }

        public ImageData (String location)
        {
            fileLocation=location;
            imageName = Path.GetFileName(location);
            colorIntensity = getColorIntensity(location);
            colorCode = getColorCode(location);
            List<int> fList = new List<int>(colorIntensity);
            fList.AddRange(colorCode);
            features = fList.ToArray();
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is ImageData))
            {
                return false;
            } else
            {
                return this.fileLocation.Equals(((ImageData)obj).fileLocation);
            }
        }

        //returning histogram bins of each image which contains colour intensity using formula
        //I = 0.299R + 0.587G + 0.114B
        private int[] getColorIntensity(string imageName)
        {
            int[] histogramBins = new int[25];
            Bitmap bmp = new Bitmap(imageName);
            for (int i = 1; i < bmp.Width; i++)
                for (int j = 1; j < bmp.Height; j++)
                {
                    Color pixel = bmp.GetPixel(i, j);
                    double intensity = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                    int bin = (int)intensity % 10;
                    bin = (bin == 25) ? 24 : bin;
                    histogramBins[bin]++;
                }
            return histogramBins;
        }

        //returning histogram bins of each image which contains colour code
        //by transforming each colour pixel into a 6-bit code
        private int[] getColorCode(string imageName)
        {
            int[] colorCode = new int[64];
            Bitmap bmp = new Bitmap(imageName);
            for (int i = 1; i < bmp.Width; i++)
                for (int j = 1; j < bmp.Height; j++)
                {
                    Color pixel = bmp.GetPixel(i, j);
                    string red = convertto8bit(Convert.ToString(pixel.R, 2));
                    string green = convertto8bit(Convert.ToString(pixel.G, 2));
                    string blue = convertto8bit(Convert.ToString(pixel.B, 2));
                    string _6bcode = red.Substring(0, 2) + green.Substring(0, 2) + blue.Substring(0, 2);
                    int colcode = Convert.ToInt32(_6bcode, 2);
                    colorCode[colcode]++;
                }
            return colorCode;
        }

        private string convertto8bit(string binaryString)
        {
            int length = binaryString.Length;
            if (length < 8)
            {
                for (int i = 1; i <= 8 - length; i++)
                {
                    binaryString = "0" + binaryString;
                }
            }
            return binaryString;
        }
    }
}

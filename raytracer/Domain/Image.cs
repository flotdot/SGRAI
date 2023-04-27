using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace raytracer.Domain
{
    class Image
    {
        public Size size;
        public Color3 color3;
        public double[,] pixelRed;
        public double[,] pixelGreen;
        public double[,] pixelBlue;



        public Image(Size size, Color3 color3)
        {
            this.size = size;
            this.color3 = color3;
            this.pixelRed = null;
            this.pixelGreen = null;
            this.pixelBlue = null;
        }

        public void createPixelMatrix (int x, int y)
        {
            pixelRed = new double[x, y];
            pixelGreen = new double[x, y];
            pixelBlue = new double[x, y];
        }
        public void setValueRed (int x, int y, double value)
        {
            pixelRed[x, y] = value;

        }
        public double getValueRed (int x, int y)
        {
            return pixelRed[x, y];
        }
        public void setValueGreen(int x, int y, double value)
        {
            pixelGreen[x, y] = value;

        }
        public double getValueGreen(int x, int y)
        {
            return pixelGreen[x, y];
        }
        public void setValueBlue(int x, int y, double value)
        {
            pixelBlue[x, y] = value;

        }
        public double getValueBlue(int x, int y)
        {
            return pixelBlue[x, y];
        }

        public Bitmap convertMatrixToBitmap()
        {
            if (pixelRed == null || pixelGreen == null || pixelBlue == null)
            {
                return null;
            }

            Bitmap bitmap = new Bitmap(Convert.ToInt32(size.sizeX), Convert.ToInt32(size.sizeY));
            double red = 0;
            double blue = 0;
            double green = 0;

            for (int i = 0; i < size.sizeX; i += 1)
            {
                for (int j = 0; j < size.sizeY; j += 1)
                {                                      
                    red = pixelRed[i, j];
                    blue = pixelBlue[i, j];
                    green = pixelGreen[i, j];
                   

                    if (red > 1)
                    {
                        red = 1;
                    }
                    if (green > 1)
                    {
                        green = 1;
                    }
                    if (blue > 1)
                    {
                        blue = 1;
                    }
                    
                    Color newColor = Color.FromArgb(Convert.ToInt32(255*red), Convert.ToInt32(255*green), Convert.ToInt32(255*blue));
                    bitmap.SetPixel(i, j, newColor);
                }
            }

            return bitmap;
        }       

    }
}

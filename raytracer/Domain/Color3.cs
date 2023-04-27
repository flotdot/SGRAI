using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer.Domain
{
    class Color3
    {
        public double red;
        public double green;
        public double blue;

        public Color3(double red, double green, double blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
        public void checkRange()
        {
            red = checkColor(red);
            green = checkColor(green);
            blue = checkColor(blue);
        }
        private static double checkColor(double c)
        {
            if (c < 0)
            {
                return 0;
            }
            else if (c > 1)
            {
                return 1;
            }
            else
            {
                return c;
            }
        }
    }
}

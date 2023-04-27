using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer.Domain
{
    class Angle
    {
        public char plane; 
        public double degrees;


        public Angle(char plane, double degrees)
        {

            switch (plane)
            {
                case 'X':
                    this.plane = 'x';
                    break;
                case 'x':
                    this.plane = 'x';
                    break;
                case 'Y':
                    this.plane = 'y';
                    break;
                case 'y':
                    this.plane = 'y';
                    break;
                case 'Z':
                    this.plane = 'z';
                    break;
                case 'z':
                    this.plane = 'z';
                    break;
                default:
                    throw new InvalidOperationException("Couldn't read plane");

            }
            this.plane = plane;
            this.degrees = degrees;
        }
    }
}

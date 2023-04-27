using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer.Domain
{
    class Vector4
    {
        public double x;
        public double y;
        public double z;
        public double w;
        
        //w=0 vector; 
        //w=1 point;
        public Vector4(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}

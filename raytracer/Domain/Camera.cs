using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer.Domain
{
    class Camera
    {
        public Transformation transformation;
        public double distance;
        public double FOV;

        public Camera(Transformation transformation, double distance, double fOV)
        {
            this.transformation = transformation;
            this.distance = distance;
            this.FOV = fOV;
        }
    }
}

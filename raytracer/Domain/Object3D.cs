using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer.Domain
{
    abstract class Object3D
    {

        
        public abstract bool intersect(Ray ray);

        public abstract void obtainRealPoints(Transformation transformationCam);
    }
}

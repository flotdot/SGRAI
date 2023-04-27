using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer.Domain
{
    class Hit
    {
        public bool found;

        public Object3D obj3d;

        public Material material;
        //w==1
        public Vector4 pointOfIntersection;
        //w==0
        public Vector4 normal;

        public Vector4 pointOfOBJ;
        //Distance from point of origin to point of Intersection
        public double distanceOToP;
        //minimal Distance found to the moment;
        public double minDist;


        public Hit(bool found, Material material, Vector4 pointOfIntersection, Vector4 normal, double distanceOToP, double minDist,Object3D obj3d)
        {

            if (pointOfIntersection.w != 1)
            {
                throw new InvalidOperationException("pointOfIntersection isn't a point");
            }
            if (normal.w != 0)
            {
                throw new InvalidOperationException("normal isn't a vector");
            }
            this.found = found;
            this.material = material;
            this.pointOfIntersection = pointOfIntersection;
            this.normal = normal;
            this.distanceOToP = distanceOToP;
            this.minDist = minDist;
            this.obj3d = obj3d;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using raytracer.Utils;

namespace raytracer.Domain
{
    class Triangle
    {
        public Triangles t;

        public Transformation transformation;

        double[,] minverse;

        public Material material;
        //point 1
        public Vector4 p1;
        //point 2
        public Vector4 p2;
        //point 3
        public Vector4 p3;

        public Triangle(Triangles t, Transformation transformation, Material material, Vector4 p1, Vector4 p2, Vector4 p3)
        {

            if (p1.w == 1 && p2.w == 1 && p3.w == 1)
            {
                this.material = material;

                this.p1 = p1;
                this.p2 = p2;
                this.p3 = p3;
                this.transformation = transformation;
                this.t = t;
            }
            else
            {
                throw new InvalidOperationException("p1 or p2 or p3 isnt a point");
            }
        }

        public bool intersect(Ray ray, double kEpsilon)
        {

            double[] rayDir = { ray.direction.x, ray.direction.y, ray.direction.z, ray.direction.w };
            double[] dir = RaytracerMath.multiply1(rayDir, minverse);
            double[] rayOr = { ray.origin.x, ray.origin.y, ray.origin.z, ray.origin.w };
            double[] or = RaytracerMath.multiply1(rayOr, minverse);


            Vector4 direction = new Vector4(dir[0], dir[1], dir[2], dir[3]);
            Vector4 origin = new Vector4(or[0],or[1],or[2],or[3]);

            Vector4 edge1 = new Vector4(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z, 0);
            Vector4 edge2 = new Vector4(p3.x - p1.x, p3.y - p1.y, p3.z - p1.z, 0);

            Vector4 h = RaytracerMath.crossVectors(direction, edge2);
            double a = RaytracerMath.dotProduct(edge1, h);

            if(a>-kEpsilon && a < kEpsilon) return false;

            double f = 1 / a;
            Vector4 s = new Vector4(origin.x - p1.x, origin.y - p1.y, origin.z - p1.z, 0);
            double u = f * RaytracerMath.dotProduct(s, h);


            if (u < 0.0 || u > 1.0) return false;


            Vector4 q = RaytracerMath.crossVectors(s, edge1);
            double v = f * RaytracerMath.dotProduct(direction, q);

            if (v < 0.0 || u + v > 1.0) return false;

            double t = f * RaytracerMath.dotProduct(edge2, q);
            if (t > kEpsilon)
            {

                double[] poi = { origin.x + direction.x * t, origin.y + direction.y * t, origin.z + direction.z * t, 1 };
                double[] poiAux = RaytracerMath.multiply1(poi, transformation.matrix);
                Vector4 pointOfIntersection = new Vector4(poiAux[0], poiAux[1], poiAux[2], poiAux[3]);

                Vector4 normalAux = RaytracerMath.crossVectors(edge1, edge2);

                double[] normAux = { normalAux.x, normalAux.y, normalAux.z, normalAux.w };
                double[] norm = RaytracerMath.multiply1(normAux, transformation.matrix);

                Vector4 lenghtVector = new Vector4(pointOfIntersection.x - ray.origin.x, pointOfIntersection.y - ray.origin.y, pointOfIntersection.z - ray.origin.z, 0);
                double lenght = RaytracerMath.lenghtOfVector(lenghtVector);

                ray.addHit(new Hit(true, material, pointOfIntersection, RaytracerMath.Normalize(new Vector4(norm[0],norm[1],norm[2],norm[3])), lenght, lenght,this.t));

                return true;
            }
            else
            {
                return false;
            }
        }

        public void obtainRealPoints(Transformation transformationCam)
        {
            transformation = new Transformation(transformation.getOperationsList(), RaytracerMath.multiply3(transformationCam.matrix, this.transformation.matrix));
            minverse = RaytracerMath.invertMatrix(transformation.matrix);
        }
    }
}

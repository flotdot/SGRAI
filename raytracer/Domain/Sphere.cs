using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using raytracer.Utils;


namespace raytracer.Domain
{
    class Sphere:Object3D
    {
        public Transformation transformation;
        public Material material;
        public double radius;
        public double sphereX;
        public double sphereY;
        public double sphereZ;
        double[,] minverse;

        public Sphere(Transformation transformation, Material material)
        {
            this.transformation = transformation;
            this.material = material;

            this.radius = 1;
            //use Transformation to find center
            this.sphereX = 0;
            this.sphereY = 0;
            this.sphereZ = 0;

        }

        public override bool intersect(Ray ray)
        {
            
            if (minverse == null)
            {
                return false;
            }


            double[] rayDir = { ray.direction.x, ray.direction.y, ray.direction.z, ray.direction.w };
            double[] dir = RaytracerMath.multiply1(rayDir, minverse);
            double[] rayOr = { ray.origin.x, ray.origin.y, ray.origin.z, ray.origin.w};
            double[] or = RaytracerMath.multiply1(rayOr, minverse);


            Vector4 dirVec = new Vector4(dir[0], dir[1], dir[2], dir[3]);
            dirVec = RaytracerMath.Normalize(dirVec);

            Vector4 orVec = new Vector4(or[0], or[1], or[2], or[3]);


            Vector4 oc = new Vector4(orVec.x - sphereX, orVec.y - sphereY, orVec.z - sphereZ, 0);

            double a = RaytracerMath.dotProduct(dirVec, dirVec);

            double b = 2 * RaytracerMath.dotProduct(oc, dirVec);

            double c = RaytracerMath.dotProduct(oc, oc) - radius*radius;

            double discriminant = b * b - 4 * a * c;

            if (discriminant < 0) return false;

            discriminant = Math.Sqrt(discriminant);
            double tPositive = (-b + discriminant) / (2 * a);
            double tNegative = (-b - discriminant) / (2 * a);

            //point of intersections
            Vector4 pPositive = new Vector4((orVec.x + dirVec.x * tPositive), (orVec.y + dirVec.y * tPositive), (orVec.z + dirVec.z * tPositive), 1);

            Vector4 pNegative = new Vector4((orVec.x + dirVec.x * tNegative), (orVec.y + dirVec.y * tNegative), (orVec.z + dirVec.z * tNegative), 1);

            Vector4 normalP = new Vector4(pPositive.x - sphereX, pPositive.y - sphereY, pPositive.z - sphereZ, 0);

            Vector4 normalN = new Vector4(pNegative.x - sphereX, pNegative.y - sphereY, pNegative.z - sphereZ, 0);

            double[] pPos = { pPositive.x, pPositive.y, pPositive.z, pPositive.w };
            double[] nPos = { pNegative.x, pNegative.y, pNegative.z, pNegative.w };

            pPos = RaytracerMath.multiply1(pPos, transformation.matrix);
            nPos = RaytracerMath.multiply1(nPos, transformation.matrix);

            double[] norP = { normalP.x, normalP.y, normalP.z, normalP.w };
            double[] norN = { normalN.x, normalN.y, normalN.z, normalN.w };

            norP = RaytracerMath.multiply1(norP, transformation.matrix);
            norN = RaytracerMath.multiply1(norN, transformation.matrix);


            //vector to interssection
            Vector4 vPositive = new Vector4(pPos[0] - ray.origin.x, pPos[1] - ray.origin.y, pPos[2] - ray.origin.z, 0);

            Vector4 vNegative = new Vector4(nPos[0] - ray.origin.x, nPos[1] - ray.origin.y, nPos[2] - ray.origin.z, 0);


            double lengthPositive = RaytracerMath.lenghtOfVector(vPositive);
            double lengthNegative = RaytracerMath.lenghtOfVector(vNegative);


            if (tPositive < Math.Pow(10,-6) || tNegative < Math.Pow(10, -6))
            {
                return false;
            }

            if (lengthNegative > lengthPositive)
            {
                
                ray.addHit(new Hit(true, material, new Vector4(pPos[0], pPos[1], pPos[2], 1),RaytracerMath.Normalize( new Vector4(norP[0], norP[1], norP[2],0)), lengthPositive, lengthPositive,this));
            }
            else
            {
                
                ray.addHit(new Hit(true, material, new Vector4(nPos[0], nPos[1], nPos[2], 1), RaytracerMath.Normalize(new Vector4(norN[0], norN[1], norN[2], 0)), lengthNegative, lengthNegative,this));
            }

            return true;
        }


        public override void obtainRealPoints(Transformation transformationCam)
        {
            transformation= new Transformation(transformation.getOperationsList(), RaytracerMath.multiply3(transformationCam.matrix, this.transformation.matrix));
            minverse = RaytracerMath.invertMatrix(transformation.matrix);
        }
    }
}

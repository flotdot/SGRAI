using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using raytracer.Utils;

namespace raytracer.Domain
{
    class Box :Object3D
    {
        public Transformation transformation;
        public Material material;
        double[,] minverse;
        public double size;
        public double xpmin, xpmax, ypmin, ypmax, zpmax, zpmin;

        public Box(Transformation transformation, Material material)
        {
            this.transformation = transformation;
            this.material = material;
            this.size = 1;
        }

        public override bool intersect(Ray ray)
        {
            
            if (minverse == null)
            {
                return false;
            }


            double[] rayDir = {ray.direction.x, ray.direction.y, ray.direction.z, ray.direction.w };
            double[] dir = RaytracerMath.multiply1(rayDir, minverse);
            double[] rayOr = { ray.origin.x, ray.origin.y, ray.origin.z, ray.origin.w };
            double[] or = RaytracerMath.multiply1(rayOr, minverse);

            if (dir[0] == 0)
            {
                if (dir[0] < xpmin || dir[0] > xpmax)
                {
                    return false;
                }
            }
            if (dir[1] == 0)
            {
                if (dir[1] < ypmin || dir[1] > ypmax)
                {
                    return false;
                }
            }

            if (dir[2] == 0)
            {
                if (or[2] < zpmin || or[2] > zpmax)
                {
                    return false;
                }
            }

            
            double tnear = (xpmin - or[0]) / dir[0];
            double tfar = (xpmax - or[0]) / dir[0];

            
            if(tnear > tfar )
            {
                double t = tnear;
                tnear = tfar;
                tfar = t;
            }
            if (0 > tfar)
            {
                return false;
            }
       


            double t1 = (ypmin - or[1]) / dir[1];
            double t2 = (ypmax - or[1]) / dir[1];

            if (t1 > t2)
            {
                double t = t1;
                t1 = t2;
                t2 = t;
            }
            if (t1 > tnear)
            {
                tnear = t1;
            }
            if (t2 < tfar)
            {
                tfar = t2;
            }
            if (tnear > tfar)
            {
                return false;
            }
        

            t1 = (zpmin - or[2]) / dir[2];
            t2 = (zpmax - or[2]) / dir[2];

            if (t1 > t2)
            {
                double t = t1;
                t1 = t2;
                t2 = t;
            }
            if (t1 > tnear)
            {
                tnear = t1;
            }
            if (t2 < tfar)
            {
                tfar = t2;
            }
            if (tnear > tfar)
            {
                return false;
            }



            if (tnear < 0)
            {
                if (tfar < 0)
                {
                    return false;
                }
                else
                {
                    tnear = tfar;
                }
            }

            Vector4 Poi = new Vector4(or[0] + dir[0] * tnear, or[1] + dir[1] * tnear, or[2] + dir[2] * tnear, 1);

            double[] PoiRay = { Poi.x, Poi.y, Poi.z, Poi.w };

            
            //todo normal

            Vector4 normalAux = new Vector4(Poi.x, Poi.y, Poi.z,0);
                
            
            if (Math.Abs( normalAux.x) > Math.Abs(normalAux.y))
            {
                normalAux.y = 0;
                if (Math.Abs(normalAux.x) > Math.Abs(normalAux.z))
                {
                    normalAux.z=0;
                }
                else
                {
                    normalAux.x = 0;
                }
            }
            else
            {
                normalAux.x = 0;
                if (Math.Abs(normalAux.y) > Math.Abs(normalAux.z))
                {
                    normalAux.z = 0;
                }
                else
                {
                    normalAux.y = 0;
                }
            }
            
            double[] auxNormal ={normalAux.x,normalAux.y,normalAux.z,0 }; 
            double[] normalRet= RaytracerMath.multiply1(auxNormal, transformation.matrix);




            double[] poi = RaytracerMath.multiply1(PoiRay, transformation.matrix);
            Poi = new Vector4(poi[0], poi[1], poi[2], poi[3]);

            double lenght = RaytracerMath.lenghtOfVector( new Vector4(Poi.x-ray.origin.x, Poi.y - ray.origin.y, Poi.z - ray.origin.z,0));

            ray.addHit(new Hit(true,material,Poi,RaytracerMath.Normalize(new Vector4(normalRet[0], normalRet[1], normalRet[2], 0)), lenght, lenght,this));
            return true;

        }


        public override void obtainRealPoints(Transformation transformationCam)
        {
            double aux;
            transformation = new Transformation(transformation.getOperationsList(),RaytracerMath.multiply3(transformationCam.matrix, this.transformation.matrix));

            minverse = RaytracerMath.invertMatrix(transformation.matrix);         

            xpmin = -size/2;
            xpmax = size / 2;
            ypmin = -size / 2;
            ypmax = size / 2;
            zpmin = -size / 2;
            zpmax = size / 2;


        }
    }
}

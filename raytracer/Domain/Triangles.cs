using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using raytracer.Utils;

namespace raytracer.Domain
{
    class Triangles:Object3D
    {
        public Transformation transformation;
        private ArrayList triangles;

        double[,] minverse;
        public double xpmin, xpmax, ypmin, ypmax, zpmax, zpmin;


        public Triangles(Transformation transformation)
        {
            this.transformation = transformation;
            this.triangles = new ArrayList();
            xpmin = double.MaxValue;
            xpmax = double.MinValue;
            ypmin = double.MaxValue;
            ypmax = double.MinValue;
            zpmin = double.MaxValue;
            zpmax = double.MinValue;
        }


        public void addTriangle(Triangle t)
        {
            checkLimits(t.p1);
            checkLimits(t.p2);
            checkLimits(t.p3);


            this.triangles.Add(t);
        }

        private void checkLimits(Vector4 p)
        {
            if(p.x<xpmin)
            {
                xpmin = p.x;
            }
            if (p.x > xpmax)
            {
                xpmax = p.x;
            }
            if (p.y < ypmin )
            {
                ypmin = p.y;
            }
            if (p.y > ypmax )
            {
                ypmax = p.y;
            }
            if (p.z < zpmin )
            {
                zpmin = p.z;
            }
            if (p.z > zpmax )
            {
                zpmax = p.z;
            }
        }

        public ArrayList getTriangles()
        {
            return (ArrayList) triangles.Clone();
        }


        public override bool intersect(Ray ray)
        {
            foreach(Object obj in triangles)
            {
                Triangle t = (Triangle)obj;

                t.intersect(ray, 1 * Math.Pow(10, -6));
            }
            if (ray.getHits().Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void obtainRealPoints(Transformation transformationCam)
        {
            transformation = new Transformation(transformation.getOperationsList(), RaytracerMath.multiply3(transformationCam.matrix, this.transformation.matrix));

            minverse = RaytracerMath.invertMatrix(transformation.matrix);

            foreach (Object obj in triangles)
            {
                Triangle t = (Triangle)obj;
                t.obtainRealPoints(transformationCam);
            }
        }

        public bool boundingBox(Ray ray)
        {


            if (minverse == null)
            {
                return false;
            }


            double[] rayDir = { ray.direction.x, ray.direction.y, ray.direction.z, ray.direction.w };
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

            if (tnear > tfar)
            {
                double t = tnear;
                tnear = tfar;
                tfar = t;
            }
            if (tfar < 0)
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

            if (tfar < 0)
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

            if (tfar < 0)
            {
                return false;
            }
            return true;
        }
    }
}

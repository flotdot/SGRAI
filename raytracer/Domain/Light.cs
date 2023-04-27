using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using raytracer.Utils;

namespace raytracer.Domain
{
    class Light
    {
        public Transformation transformation;
        public Color3 color;

        public double x, y, z;

        public Light(Transformation transformation, Color3 color)
        {
            this.transformation = transformation;
            this.color = color;
            x = 0;
            y = 0;
            z = 0;
        }

        public void obtainRealPoints(Transformation transformationCam)
        {
            Transformation trans = new Transformation(transformation.getOperationsList(), RaytracerMath.multiply3(transformationCam.matrix, this.transformation.matrix));
            double[] p1 = { 0, 0, 0, 1 };

            double[] p1Transformed = RaytracerMath.multiply1(p1, trans.matrix);

            this.x = p1Transformed[0]; this.y = p1Transformed[1]; this.z = p1Transformed[2];


        }
    }
}

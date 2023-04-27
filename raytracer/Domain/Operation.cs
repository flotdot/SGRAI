using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using raytracer.Utils;

namespace raytracer.Domain
{
    class Operation
    {
        public char typeOfOperation;

        public Vector4 vec;

        public Angle angle;


        public Operation(char typeOfOperation, Vector4 vec, Angle angle)
        {
            switch (typeOfOperation)
            {
                case 'T':
                    this.typeOfOperation = 't';
                    this.vec = vec;
                    this.angle = null;
                    break;
                case 't':
                    this.typeOfOperation = 't';
                    this.vec = vec;
                    this.angle = null;
                    break;
                case 'R':
                    this.typeOfOperation = 'r';
                    this.vec = null;
                    this.angle = angle;
                    break;
                case 'r':
                    this.typeOfOperation = 'r';
                    this.vec = null;
                    this.angle = angle;
                    break;
                case 'S':
                    this.typeOfOperation = 's';
                    this.vec = vec;
                    this.angle = null;
                    break;
                case 's':
                    this.typeOfOperation = 's';
                    this.vec = vec;
                    this.angle = null;
                    break;
                default:
                    throw new InvalidOperationException("Couldn't read typeOfOperation");

            }
            
        }
        public double[,] applyOperationToMatrix(double[,] matrix)
        {
            switch (this.typeOfOperation)
            {
                case 't':

                    return RaytracerMath.translate(vec.x, vec.y, vec.z, matrix);

                case 'r':
                    switch (this.angle.plane)
                    {
                        case 'x':
                            return RaytracerMath.rotateXMatrix(this.angle.degrees, matrix);
                        case 'y':
                            return RaytracerMath.rotateYMatrix(this.angle.degrees, matrix);
                        case 'z':
                            return RaytracerMath.rotateZMatrix(this.angle.degrees, matrix);
                    }
                    break;
                case 's':
                    return RaytracerMath.scale(vec.x, vec.y, vec.z, matrix);
            }

            return null;
        }

    }
}

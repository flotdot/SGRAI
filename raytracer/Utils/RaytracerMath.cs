using System;
using System.Numerics;
using raytracer.Domain;


namespace raytracer.Utils
{
    class RaytracerMath
    {
       
        /**
        double p1; //vertice
        double p2;
        double p3;
        double norm; // normal = (a-b) * (c-a)

        //Vetor1= p1-p2
        //Vetor2= p3-p1
        */
        public static Domain.Vector4 Normalize(Domain.Vector4 crossVectors) {
            if (crossVectors.w == 0)
            {
                double magnitude = Math.Sqrt((crossVectors.x * crossVectors.x) +
                                    (crossVectors.y * crossVectors.y) +
                                    (crossVectors.z * crossVectors.z));
                // |a| = sqrt((ax * ax) + (ay * ay) + (az * az))
                return new Domain.Vector4(crossVectors.x / magnitude, crossVectors.y / magnitude, crossVectors.z / magnitude, 0);
            }
            else
            {
                throw new InvalidOperationException("crossVectors isnt a vector");
            }        
        }

        public static double dotProduct(Domain.Vector4 vec1, Domain.Vector4 vec2)
        {

            return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
            
        }

        public static double lenghtOfVector(Domain.Vector4 vec)
        {
            if (vec.w == 0)
            {
                return Math.Sqrt((vec.x * vec.x) +
                                    (vec.y * vec.y) +
                                    (vec.z * vec.z));               
            }
            else
            {
                throw new InvalidOperationException("vec isnt a vector");
            }
        }
        public static Domain.Vector4 createVectorA(Domain.Vector4 p1, Domain.Vector4 p2) //from p1 to p2
        {
            if (p1.w == 1 && p2.w == 1)
            {
                return new Domain.Vector4(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z, 0);
            } else
            {
                throw new InvalidOperationException("p1 or p2 isnt a point");
            }

        }
        //get normal
        public static Domain.Vector4 crossVectors(Domain.Vector4 createVectorA, Domain.Vector4 createVectorB) //from p1 to p2
        {
            if (createVectorA.w == 0 && createVectorB.w == 0)
            {
                return new Domain.Vector4(createVectorA.y*createVectorB.z-createVectorB.y*createVectorA.z,
                    createVectorA.z*createVectorB.x-createVectorB.z*createVectorA.x,
                    createVectorA.x*createVectorB.y-createVectorB.x*createVectorA.y, 0);
            }
            else
            {
                throw new InvalidOperationException("createVectorA or createVectorB isnt a vector");
            }
        }
        public static double degreeToRad(double degrees)
        {
            return degrees*Math.PI/180;
        }
        public static double height(double distance, double fov)
        {
            return 2.0*distance*Math.Tan(degreeToRad(fov) / 2.0); 
        }
        public static double width(double height, double Hres, double Vres)
        {
            return height*Hres/Vres;
        }
        public static double pixelDimension(double height, double Vres)
        {
            return height/Vres;
        }



        public static double[,] getIdentityMatrix()
        {
            double[,] transformMatrix = new double[4, 4];
            transformMatrix[0,0] = 1.0;
            transformMatrix[0,1] = 0.0;
            transformMatrix[0,2] = 0.0;
            transformMatrix[0,3] = 0.0;
            transformMatrix[1,0] = 0.0;
            transformMatrix[1,1] = 1.0;
            transformMatrix[1,2] = 0.0;
            transformMatrix[1,3] = 0.0;
            transformMatrix[2,0] = 0.0;
            transformMatrix[2,1] = 0.0;
            transformMatrix[2,2] = 1.0;
            transformMatrix[2,3] = 0.0;
            transformMatrix[3,0] = 0.0;
            transformMatrix[3,1] = 0.0;
            transformMatrix[3,2] = 0.0;
            transformMatrix[3,3] = 1.0;

            return transformMatrix;
        }

        public static double[] multiply1(double[] pointA,double[,] transformMatrix)// multiplica uma matriz 4 x 4 por uma matriz-coluna representativa de um ponto ou vector expresso em coordenadas homogéneas
        {
            double[] pointB = new double[4];
            for(int i = 0; i < 4; i+=1)
            {
                for(int j = 0; j < 4; j += 1)
                {
                    pointB[i] += transformMatrix[i,j] * pointA[j];
                }
            }
            return pointB;
        }


        public static double[,] multiply3(double[,] matrixA,double[,] transformMatrix)// multiplica duas matrizes 4 x 4
        {
            double[,] matrixB = new double[4, 4];


            for (int i = 0; i < 4; i += 1)
            {
                for (int j = 0; j < 4; j += 1)
                {
                    for(int k = 0; k < 4; k += 1)
                    {
                        matrixB[i,j]+= matrixA[i,k] * transformMatrix[k,j];
                    }

                }
            }
            return matrixB;
        }

        public static double[,] translate(double x,double y,double z, double[,] transformMatrix)
        {
            double[,] translateMatrix = new double[4, 4];

            translateMatrix[0,0] = 1.0;
            translateMatrix[0,1] = 0.0;
            translateMatrix[0,2] = 0.0;
            translateMatrix[0,3] = x;
            translateMatrix[1,0] = 0.0;
            translateMatrix[1,1] = 1.0;
            translateMatrix[1,2] = 0.0;
            translateMatrix[1,3] = y;
            translateMatrix[2,0] = 0.0;
            translateMatrix[2,1] = 0.0;
            translateMatrix[2,2] = 1.0;
            translateMatrix[2,3] = z;
            translateMatrix[3,0] = 0.0;
            translateMatrix[3,1] = 0.0;
            translateMatrix[3,2] = 0.0;
            translateMatrix[3,3] = 1.0;

            return multiply3(transformMatrix, translateMatrix);
        }



        public static double[,] rotateXMatrix(double a, double[,] transformMatrix)
        {
            double[,] rotateXMatrix = new double[4, 4];
            double angle = a*Math.PI / 180.0;
            rotateXMatrix[0,0] = 1.0;
            rotateXMatrix[0,1] = 0.0;
            rotateXMatrix[0,2] = 0.0;
            rotateXMatrix[0,3] = 0.0;
            rotateXMatrix[1,0] = 0.0;
            rotateXMatrix[1,1] = Math.Cos(angle);
            rotateXMatrix[1,2] = -Math.Sin(angle);
            rotateXMatrix[1,3] = 0.0;
            rotateXMatrix[2,0] = 0.0;
            rotateXMatrix[2,1] = Math.Sin(angle);
            rotateXMatrix[2,2] = Math.Cos(angle);
            rotateXMatrix[2,3] = 0.0;
            rotateXMatrix[3,0] = 0.0;
            rotateXMatrix[3,1] = 0.0;
            rotateXMatrix[3,2] = 0.0;
            rotateXMatrix[3,3] = 1.0;
            return multiply3(transformMatrix, rotateXMatrix);
        }

        public static double[,] rotateYMatrix(double a, double[,] transformMatrix)
        {
            double[,] rotateYMatrix = new double[4, 4];
            double angle = a * Math.PI / 180.0;
            rotateYMatrix[0,0] = Math.Cos(angle);
            rotateYMatrix[0,1] = 0.0;
            rotateYMatrix[0,2] = Math.Sin(angle);
            rotateYMatrix[0,3] = 0.0;
            rotateYMatrix[1,0] = 0.0;
            rotateYMatrix[1,1] = 1.0;
            rotateYMatrix[1,2] = 0.0;
            rotateYMatrix[1,3] = 0.0;
            rotateYMatrix[2,0] = -Math.Sin(angle);
            rotateYMatrix[2,1] = 0.0;
            rotateYMatrix[2,2] = Math.Cos(angle);
            rotateYMatrix[2,3] = 0.0;
            rotateYMatrix[3,0] = 0.0;
            rotateYMatrix[3,1] = 0.0;
            rotateYMatrix[3,2] = 0.0;
            rotateYMatrix[3,3] = 1.0;
            return multiply3(transformMatrix, rotateYMatrix);
        }
        public static double[,] rotateZMatrix(double a, double[,] transformMatrix)
        {
            double[,] rotateZMatrix = new double[4, 4];
            double angle = a * Math.PI / 180.0;
            rotateZMatrix[0,0] = Math.Cos(angle);
            rotateZMatrix[0,1] = -Math.Sin(angle);
            rotateZMatrix[0,2] = 0.0;
            rotateZMatrix[0,3] = 0.0;
            rotateZMatrix[1,0] = Math.Sin(angle);
            rotateZMatrix[1,1] = Math.Cos(angle);
            rotateZMatrix[1,2] = 0.0;
            rotateZMatrix[1,3] = 0.0;
            rotateZMatrix[2,0] = 0.0;
            rotateZMatrix[2,1] = 0.0;
            rotateZMatrix[2,2] = 1.0;
            rotateZMatrix[2,3] = 0.0;
            rotateZMatrix[3,0] = 0.0;
            rotateZMatrix[3,1] = 0.0;
            rotateZMatrix[3,2] = 0.0;
            rotateZMatrix[3,3] = 1.0;
            return multiply3(transformMatrix, rotateZMatrix);
        }

        public static double[,] scale(double x,double y,double z, double[,] transformMatrix)
        {
            double[,] scaleMatrix=new double[4, 4];

            scaleMatrix[0,0] = x;
            scaleMatrix[0,1] = 0.0;
            scaleMatrix[0,2] = 0.0;
            scaleMatrix[0,3] = 0.0;
            scaleMatrix[1,0] = 0.0;
            scaleMatrix[1,1] = y;
            scaleMatrix[1,2] = 0.0;
            scaleMatrix[1,3] = 0.0;
            scaleMatrix[2,0] = 0.0;
            scaleMatrix[2,1] = 0.0;
            scaleMatrix[2,2] = z;
            scaleMatrix[2,3] = 0.0;
            scaleMatrix[3,0] = 0.0;
            scaleMatrix[3,1] = 0.0;
            scaleMatrix[3,2] = 0.0;
            scaleMatrix[3,3] = 1.0;
            return multiply3(transformMatrix, scaleMatrix);
        }

        public static double[,] neg(double[,] matrix)
        {
            double[,] retM = new double[4, 4];

            retM[0, 0] = matrix[0, 0];
            retM[0, 1] = matrix[0, 1];
            retM[0, 2] = matrix[0, 2];
            retM[0, 3] = matrix[0, 3];
            retM[1, 0] = matrix[1, 0];
            retM[1, 1] = matrix[1, 1];
            retM[1, 2] = matrix[1, 2];
            retM[1, 3] = matrix[1, 3];
            retM[2, 0] = matrix[2, 0];
            retM[2, 1] = matrix[2, 1];
            retM[2, 2] = matrix[2, 2];
            retM[2, 3] = matrix[2, 3];
            retM[3, 0] = matrix[3, 0];
            retM[3, 1] = matrix[3, 1];
            retM[3, 2] = matrix[3, 2];
            retM[3, 3] = matrix[3, 3];

            return retM;
        }

        public static double[,] invertMatrix(double[,] mat)
        {
            double[,] ret = new double[4, 4];
            Matrix4x4 m = new Matrix4x4((float)mat[0, 0], (float)mat[0, 1], (float)mat[0, 2], (float)mat[0, 3],
                                        (float)mat[1, 0], (float)mat[1, 1], (float)mat[1, 2], (float)mat[1, 3],
                                        (float)mat[2, 0], (float)mat[2, 1], (float)mat[2, 2], (float)mat[2, 3],
                                        (float)mat[3, 0], (float)mat[3, 1], (float)mat[3, 2], (float)mat[3, 3]);

            Matrix4x4 r;

            if (Matrix4x4.Invert(m, out r))
            {
                ret[0, 0] = r.M11;
                ret[0, 1] = r.M12;
                ret[0, 2] = r.M13;
                ret[0, 3] = r.M14;
                ret[1, 0] = r.M21;
                ret[1, 1] = r.M22;
                ret[1, 2] = r.M23;
                ret[1, 3] = r.M24;
                ret[2, 0] = r.M31;
                ret[2, 1] = r.M32;
                ret[2, 2] = r.M33;
                ret[2, 3] = r.M34;
                ret[3, 0] = r.M41;
                ret[3, 1] = r.M42;
                ret[3, 2] = r.M43;
                ret[3, 3] = r.M44;

                return ret;
            }
            return null;
        }

    }
}

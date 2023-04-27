using System;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using raytracer.Utils;

namespace raytracer.Domain
{
    class Transformation
    {
        private ArrayList listOfOperations;

        public double[,] matrix;


        public Transformation()
        {
            this.listOfOperations = new ArrayList();
            this.matrix = RaytracerMath.getIdentityMatrix();
        }

        public Transformation(ArrayList listOfOperations, double[,] matrix)
        {
            this.listOfOperations = listOfOperations;
            this.matrix = matrix;
        }

        public void addOperation(Operation op)
        {
            this.listOfOperations.Add(op);
        }

        public void applyOperations()
        {
            foreach (Object obj in listOfOperations)
            {
                Operation op = (Operation)obj;
                matrix = op.applyOperationToMatrix(matrix);
            }
        }

        public ArrayList getOperationsList()
        {
            return (ArrayList) listOfOperations.Clone();
        }
    }
}

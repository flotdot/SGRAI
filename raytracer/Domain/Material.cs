using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer.Domain
{
    class Material
    {
        public Color3 color3;
        public double ambiente;
        public double difusa;
        public double especular;
        public double refracao;
        public double indice_de_refracao;

        public Material(Color3 color3, double ambiente, double difusa, double especular, double refracao, double indice_de_refracao)
        {
            this.color3 = color3;
            this.ambiente = ambiente;
            this.difusa = difusa;
            this.especular = especular;
            this.refracao = refracao;
            this.indice_de_refracao = indice_de_refracao;
        }

    }
}

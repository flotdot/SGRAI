using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer.Domain
{
    class Scene
    {
        public Image image;
        public Camera camera;

        private ArrayList allTransformations;
        private ArrayList allMaterial;
        private ArrayList allLights;
        private ArrayList allObjects3D;

        public Scene()
        {
            this.image = null;
            this.camera = null;
            this.allTransformations = new ArrayList();
            this.allMaterial = new ArrayList();
            this.allLights = new ArrayList();
            this.allObjects3D = new ArrayList();
        }


        public void addTransformation(Transformation t)
        {
            this.allTransformations.Add(t);
        }

        public void addMaterial(Material m)
        {
            this.allMaterial.Add(m);
        }

        public void addLights(Light l)
        {
            this.allLights.Add(l);
        }

        public void addObject3D(Object3D obj3D)
        {
            this.allObjects3D.Add(obj3D);
        }


        public ArrayList getAllTransformations()
        {
            return (ArrayList)this.allTransformations;
        }

        public ArrayList getAllMaterial()
        {
            return (ArrayList)this.allMaterial;
        }

        public ArrayList getAllLights()
        {
            return (ArrayList)this.allLights;
        }

        public ArrayList getAllObjects3D()
        {
            return (ArrayList)this.allObjects3D;
        }


        public Transformation getTransformationByIndex(int i)
        {
            return (Transformation)allTransformations[i];
        }
        public Material getMaterialByIndex(int i)
        {
            return (Material)allMaterial[i];
        }
    }
}

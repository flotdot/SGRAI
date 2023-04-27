using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using raytracer.Domain;
using System.Text.RegularExpressions;

namespace raytracer.Utils
{
    class RaytracerFileReader
    {
        public static String openFile(String path)
        {
            return System.IO.File.ReadAllText(path);

        }

        public static Scene loadSceneIntoMemory(String path)
        {
            Scene ret = new Scene();
            String file = openFile(path);

            String[] configs = Regex.Split(file, "\r\n\r\n");

            for(int i=0; i < configs.Length; i += 1)
            {
                String config = configs[i];

                if (config.Contains("Image"))
                {
                    ret.image=parseStrToImg(config);

                }
                if (config.Contains("Camera"))
                {
                    ret.camera = parseStrToCamera(config,ret);
                }
                if (config.Contains("Transformation"))
                {
                    ret.addTransformation(parseStrToTransfomation(config));
                }
                if (config.Contains("Material"))
                {
                    ret.addMaterial(parseStrToMaterial(config));

                }
                if (config.Contains("Light"))
                {
                    ret.addLights(parseStrToLight(config, ret));
                }
                if (config.Contains("Triangles"))
                {
                    ret.addObject3D(parseStrToTriangles(config, ret));
                }
                if (config.Contains("Sphere"))
                {
                    ret.addObject3D(parseStrToSphere(config, ret));
                }
                if (config.Contains("Box"))
                {
                    ret.addObject3D(parseStrToBox(config, ret));
                }
            }

            return ret;
        } 



        private static Triangles parseStrToTriangles(String config,Scene sc)
        {
            String conf1 = Regex.Split(config, "\r\n}")[0];
            String conf2 = Regex.Split(conf1, "\r\n{\r\n")[1];
            String[] confs = Regex.Split(conf2, "\r\n");

            Transformation t = sc.getTransformationByIndex(Convert.ToInt32(confs[0]));
            
            Triangles triangles = new Triangles(t);

            for(int i =1; i <confs.Length; i += 4)
            {
                Triangle  triangle= parseStrToTriangle(confs[i]+"\r\n"+ confs[i+1] + "\r\n"+ confs[i+2] + "\r\n"+ confs[i+3],sc,t, triangles);
                triangles.addTriangle(triangle);
            }
            return triangles;
        }

        private static Triangle parseStrToTriangle(String confs,Scene sc, Transformation t,Triangles triangles)
        {
            String[] confs1 = Regex.Split(confs, "\r\n");
            Material m = sc.getMaterialByIndex(Convert.ToInt32(confs1[0]));

            Vector4 p1 = parseStrToVector4(confs1[1], 1);
            Vector4 p2 = parseStrToVector4(confs1[2], 1);
            Vector4 p3 = parseStrToVector4(confs1[3], 1);

            Triangle ret = new Triangle(triangles,t, m, p1, p2, p3);

            return ret;

        }

        private static Sphere parseStrToSphere(String config, Scene sc)
        {
            String conf1 = Regex.Split(config, "\r\n}")[0];
            String conf2 = Regex.Split(conf1, "\r\n{\r\n")[1];
            String[] confs = Regex.Split(conf2, "\r\n");

            Transformation t = sc.getTransformationByIndex(Convert.ToInt32(confs[0]));

            Material m = sc.getMaterialByIndex(Convert.ToInt32(confs[1]));

            Sphere ret = new Sphere(t, m);

            return ret;
        }
        private static Box parseStrToBox(String config, Scene sc)
        {
            String conf1 = Regex.Split(config, "\r\n}")[0];
            String conf2 = Regex.Split(conf1, "\r\n{\r\n")[1];
            String[] confs = Regex.Split(conf2, "\r\n");

            Transformation t = sc.getTransformationByIndex(Convert.ToInt32(confs[0]));

            Material m = sc.getMaterialByIndex(Convert.ToInt32(confs[1]));

            Box ret = new Box(t, m);

            return ret;
        }

        private static Image parseStrToImg(String config)
        {   
            String conf1 = Regex.Split(config, "\r\n}")[0];
            String conf2 = Regex.Split(conf1, "\r\n{\r\n")[1];
            String [] confs = Regex.Split(conf2, "\r\n");

            Size size = parseStrToSize(confs[0]);
            Color3 color = parseStrToColor3(confs[1]);

            Image image = new Image(size, color);

            return image;
        }

        private static Transformation parseStrToTransfomation(String config)
        {
            if (config.Contains("\r\n{\r\n}"))
            {
                Transformation ret = new Transformation();
                return ret;
            }
            else 
            {
                String conf1 = Regex.Split(config, "\r\n}")[0];
                String conf2 = Regex.Split(conf1, "\r\n{\r\n")[1];
                String[] confs = Regex.Split(conf2, "\r\n");

                Transformation ret = new Transformation();
                for (int i = 0; i < confs.Length; i += 1) {
                    ret.addOperation(parseStrToOperation(confs[i]));
                }
                ret.applyOperations();
                return ret;
            }
        }

        private static Material parseStrToMaterial(String config)
        {
            String conf1 = Regex.Split(config, "\r\n}")[0];
            String conf2 = Regex.Split(conf1, "\r\n{\r\n")[1];
            String[] confs = Regex.Split(conf2, "\r\n");
            String[] materialValues = Regex.Split(confs[1], " ");

            Color3 color = parseStrToColor3(confs[0]);
            Material ret = new Material(color, Convert.ToDouble(materialValues[0]), Convert.ToDouble(materialValues[1]), Convert.ToDouble(materialValues[2]), Convert.ToDouble(materialValues[3]), Convert.ToDouble(materialValues[4]));

            return ret;
        }

        private static Camera parseStrToCamera(String config,Scene sc)
        {
            String conf1 = Regex.Split(config, "\r\n}")[0];
            String conf2 = Regex.Split(conf1, "\r\n{\r\n")[1];
            String[] confs = Regex.Split(conf2, "\r\n");

            Transformation t = sc.getTransformationByIndex(Convert.ToInt32(confs[0]));
            Camera ret = new Camera(t, Convert.ToDouble(confs[1]), Convert.ToDouble(confs[2]));

            return ret;
        }

        private static Light parseStrToLight(String config, Scene sc)
        {
            String conf1 = Regex.Split(config, "\r\n}")[0];
            String conf2 = Regex.Split(conf1, "\r\n{\r\n")[1];
            String[] confs = Regex.Split(conf2, "\r\n");

            Transformation t = sc.getTransformationByIndex(Convert.ToInt32(confs[0]));
            Color3 c = parseStrToColor3(confs[1]);

            Light ret = new Light(t, c);

            return ret;
        }






        private static Operation parseStrToOperation(String conf)
        {
            String[] confs = Regex.Split(conf, " ");
            Operation ret = null;


            if (confs[0].Contains("t") || confs[0].Contains("T"))
            {
                ret = new Operation('t', parseStrToVector4(confs[1] + " " + confs[2] + " " + confs[3], 0),null);

            }
            if (confs[0].Contains("r") || confs[0].Contains("R"))
            {
                ret = new Operation('r', null, parseStrToAngle(conf));

            }
            if (confs[0].Contains("s") || confs[0].Contains("S"))
            {
                ret = new Operation('s', parseStrToVector4(confs[1] + " " + confs[2] + " " + confs[3], 0), null);
            }

            return ret;

        }


        private static Vector4 parseStrToVector4(String conf,Double w)
        {
            String[] confs =Regex.Split(conf, " ");
            Vector4 ret = new Vector4(Convert.ToDouble(confs[0]), Convert.ToDouble(confs[1]), Convert.ToDouble(confs[2]), w);
            return ret;
        }

        private static Angle parseStrToAngle(String conf)
        {
            String[] confs = Regex.Split(conf, " ");
            Angle ret = null;
            if(confs[0].Contains("x") || confs[0].Contains("X"))
            {
                ret= new Angle('x',Convert.ToDouble(confs[1]));
            }
            if (confs[0].Contains("y") || confs[0].Contains("Y"))
            {
                ret= new Angle('y', Convert.ToDouble(confs[1]));
            }
            if (confs[0].Contains("z") || confs[0].Contains("Z"))
            {
                ret= new Angle('z', Convert.ToDouble(confs[1]));
            }

            return ret;

        }

        private static Size parseStrToSize(String conf)
        {
            String[] confs = Regex.Split(conf," ");
            Size ret = new Size(Convert.ToDouble(confs[0]), Convert.ToDouble(confs[1]));
            return ret;
        }

        private static Color3 parseStrToColor3(String conf)
        {
            String[] confs = Regex.Split(conf, " ");
            Color3 ret = new Color3(Convert.ToDouble(confs[0]), Convert.ToDouble(confs[1]), Convert.ToDouble(confs[2]));
            return ret;
        }


 
    }
}

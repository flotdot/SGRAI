using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using raytracer.Domain;
using raytracer.Utils;

namespace raytracer.Utils
{
    class RayTracerGenerator
    {
        public static void generatePrimaryRays (Scene sc,Camera cam, Image img, int rec)
        {
            Vector4 origin = new Vector4(0, 0, cam.distance, 0);
            double Vres = img.size.sizeY;
            double Hres = img.size.sizeX;
            double height = RaytracerMath.height(cam.distance, cam.FOV);
            double width = RaytracerMath.width(height, Hres, Vres);
            double s = RaytracerMath.pixelDimension(height, Vres);

            img.createPixelMatrix(Convert.ToInt32(Hres), Convert.ToInt32(Vres));

            for (int j=0; j<Vres; j+=1)
            {
                for (int i=0; i<Hres; i+=1)
                {
                    double Px = (i + 0.5)*s-width/2.0;
                    double Py = -(j + 0.5)*s+height/2.0;
                    double Pz = 0.0;

                    Vector4 directon = new Vector4(Px - 0.0, Py - 0.0, Pz - cam.distance, 0);
                    Vector4 directionNormalized = RaytracerMath.Normalize(directon);

                    Ray ray = new Ray(origin,directon);
                    Color3 color = null;// ray.traceray(rec,sc.getAllObjects3D(),img);
                    color.checkRange();
                    img.setValueRed(i, j,Convert.ToInt32(255.0*color.red));
                    img.setValueGreen(i, j, Convert.ToInt32(255.0 * color.green));
                    img.setValueBlue(i, j, Convert.ToInt32(255.0 * color.blue));

                }
            }

        }


        public static void generatePrimaryRaysMultiThreaded(Scene sc,Camera cam ,Image img ,int rec,int nTotalThread,int nSamples, bool shadows, bool ambient, bool difuse,bool specular,bool refraction,bool antiAliased,bool softShadow)
        {


            Vector4 origin = new Vector4(0, 0,cam.distance, 1);

            double Vres = img.size.sizeY;
            double Hres = img.size.sizeX;
            double height = RaytracerMath.height(cam.distance, cam.FOV);
            double width = RaytracerMath.width(height, Hres, Vres);
            double s = RaytracerMath.pixelDimension(height, Vres);

            img.createPixelMatrix(Convert.ToInt32(Hres), Convert.ToInt32(Vres));

            threadAux[] threadAuxes = new threadAux[nTotalThread];

            for(int i=0; i < nTotalThread; i += 1)
            {
                threadAuxes[i] = new threadAux(Vres, Hres, height, width, s,sc, img, cam, rec, nTotalThread, i,nSamples, origin, shadows, ambient, difuse, specular, refraction, antiAliased, softShadow);

            }
            for (int i = 0; i < nTotalThread; i += 1)
            {
                threadAuxes[i].generatePrimaryRaysMultiThread();
            }
            for (int i = 0; i < nTotalThread; i += 1)
            {
                threadAuxes[i].Join();
            }
        }



        class threadAux
        {
            private double VresStart;
  
            private double VresFinal;
            private double HresFinal;
            private int nThread;
            private int nTotalThread;
            private double height;
            private double width;

            private double s;

            private Scene sc;
            private Image img;
            private Camera cam;

            private int rec;
            private int nSamples;

            private Vector4 origin;

            private ThreadStart threadStart;
            private Thread thread;

            bool shadows;
            bool ambient;
            bool difuse;
            bool specular;
            bool refraction;
            bool antiAliased;
            bool softShadow;

            public threadAux(double vres, double hres, double height, double width, double s, Scene sc, Image img, Camera cam, int rec, int nTotalThread, int nThread ,int nSamples, Vector4 origin, bool shadows, bool ambient, bool difuse,bool specular,bool refraction,bool antiAliased,bool softShadow)
            {
                this.origin = origin;
                this.height = height;
                this.width = width;
                this.s = s;
                this.sc = sc;
                this.img = img;
                this.cam = cam;
                this.rec = rec;
                this.nSamples = Convert.ToInt32(Math.Sqrt( nSamples));
                this.antiAliased = antiAliased;
                this.softShadow = softShadow;


                this.shadows = shadows;
                this.ambient = ambient;
                this.difuse = difuse;
                this.specular = specular;
                this.refraction = refraction;

                this.nThread = nThread;
                this.nTotalThread = nTotalThread;

                VresFinal = vres;

                HresFinal = hres;

                threadStart = new ThreadStart(generatePrimaryRaysMultiThreadAux);
                thread = new Thread(threadStart);
            }
            public void generatePrimaryRaysMultiThread()
            {
                this.thread.Start();

            }
            public bool Join()
            {
                this.thread.Join();

                return true;
            }


            private void generatePrimaryRaysMultiThreadAux()
            {
                Color3 color;
                Ray ray;

                for (int i = nThread; i < Convert.ToInt32(this.HresFinal); i += nTotalThread)
                {
                    if (i == 260) {
                        Console.WriteLine();
                    }
                    
                    for (int j =0; j < Convert.ToInt32(this.VresFinal); j += 1)
                    {
                        if (j == 165)
                        {
                            Console.WriteLine();
                        }
                        double Px = (i + 0.5) * this.s - this.width / 2.0;
                        double Py = -(j + 0.5) * this.s + this.height / 2.0;
                        double Pz = 0.0;

                        Vector4 directon = new Vector4(Px - 0.0, Py - 0.0, Pz - cam.distance, 0);
                        directon = RaytracerMath.Normalize(directon);

                        ray = new Ray(this.origin, directon);
                        color = ray.traceray(this.rec,this.sc.getAllLights(),this.sc.getAllObjects3D(),this.img, shadows, ambient, difuse,specular,refraction,softShadow);
                        color.checkRange();



                        this.img.setValueRed(i, j, color.red);
                        this.img.setValueGreen( i, j,  color.green);
                        this.img.setValueBlue(i,  j,  color.blue);
                    }
                }

                int searchArea =2;
                int iMinus = 0;
                int jMinus = 0;
                int iPlus = 0;
                int jPlus = 0;
                Color3 sampledColors=new Color3(0,0,0);

                if (antiAliased)
                {
                    for (int i = nThread; i < Convert.ToInt32(this.HresFinal); i += nTotalThread)
                    {

                        iMinus = i - searchArea;
                        iPlus = i + searchArea;

                        if(iMinus < 0)
                        {
                            iMinus = 0;
                        }
                        if(iPlus > Convert.ToInt32(this.HresFinal) - 1)
                        {
                            iPlus = Convert.ToInt32(this.HresFinal) - 1;
                        }

                        for (int j = 0; j < Convert.ToInt32(this.VresFinal); j += 1)
                        {
                            jMinus = j - searchArea;
                            jPlus = j + searchArea;

                            if (jMinus < Convert.ToInt32(this.VresStart))
                            {
                                jMinus = Convert.ToInt32(this.VresStart);
                            }
                            if (jPlus > Convert.ToInt32(this.VresFinal) - 1)
                            {
                                jPlus = Convert.ToInt32(this.VresFinal) - 1;
                            }
                            if (img.pixelRed[i, jMinus] != img.pixelRed[i, j] || img.pixelRed[i, j] != img.pixelRed[i,jPlus] || img.pixelRed[iMinus, j ] != img.pixelRed[i, j ] || img.pixelRed[i, j ] != img.pixelRed[iPlus, j] ||
                                img.pixelBlue[i, jMinus] != img.pixelBlue[i, j ] || img.pixelBlue[i, j] != img.pixelBlue[i, jPlus] || img.pixelBlue[iMinus, j ] != img.pixelBlue[i, j ] || img.pixelBlue[i, j] != img.pixelBlue[iPlus, j] ||
                                img.pixelGreen[i, jMinus] != img.pixelGreen[i, j] || img.pixelGreen[i, j] != img.pixelGreen[i, jPlus] || img.pixelGreen[iMinus, j ] != img.pixelGreen[i, j] || img.pixelGreen[i, j] != img.pixelGreen[iPlus, j]
                                )
                            {
                                sampledColors = getImdiatepoints(i, j, nSamples);

                                img.pixelRed[i, j] = (img.pixelRed[i, j] + sampledColors.red) / (nSamples * nSamples + 1);
                                img.pixelBlue[i, j] = (img.pixelBlue[i, j] + sampledColors.blue) / (nSamples * nSamples + 1);
                                img.pixelGreen[i, j] = (img.pixelGreen[i, j] + sampledColors.green) / (nSamples * nSamples + 1);
                            }
                        }
                    }
                }
            }



            //antialiasing uniform jitter pattern
            public Color3 getImdiatepoints(int i,int j,int nSamples)
            {
                Ray ray;
                Color3 ret=new Color3(0,0,0);
                Random rnd = new Random();
                Vector4 directon;
                double randomY;
                double randomX;
                double Px ;
                double Py;
                double Pz;

                for (int y=0; y < nSamples; y += 1)
                {
                    randomY = rnd.NextDouble() / nSamples;
                    for (int x = 0; x < nSamples; x += 1)
                    {
                        randomX = rnd.NextDouble() / nSamples;

                        Px = (i + x/nSamples+randomX) * this.s - this.width / 2.0;
                        Py = -(j +y/nSamples +randomY) * this.s + this.height / 2.0;
                        Pz = 0.0;


                        directon = new Vector4(Px - 0.0, Py - 0.0, Pz - cam.distance, 0);
                        directon = RaytracerMath.Normalize(directon);


                        ray = new Ray(this.origin, directon);
                        Color3 color = ray.traceray(this.rec, this.sc.getAllLights(), this.sc.getAllObjects3D(), this.img, shadows, ambient, difuse, specular, refraction,softShadow);
                        color.checkRange();

                        ret.red += color.red;
                        ret.blue += color.blue;
                        ret.green += color.green;
                    }
                }                

                return ret;
            }

        }
    }
}

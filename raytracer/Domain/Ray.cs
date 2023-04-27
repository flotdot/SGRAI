using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using raytracer.Utils;

namespace raytracer.Domain
{
    class Ray
    {
        public Vector4 origin;
        public Vector4 direction;

        private ArrayList hits;

        // origin.w ==1 - must be a point
        public Ray(Vector4 origin, Vector4 direction)
        {
            if (origin.w!=1) {
                throw new InvalidOperationException("Origin isnt a point");
            }
            this.origin = origin;
            this.direction = direction;
            this.hits = new ArrayList();
        }

        public void addHit(Hit hit)
        {
            hits.Add(hit);
        }

        public ArrayList getHits()
        {
            return (ArrayList)hits.Clone();
        }

        public Color3 traceray(int rec,ArrayList lights,ArrayList objects3D,Image img,bool shadows,bool ambient,bool difuse,bool specular,bool refraction,bool softShadow)
        {
            Hit hit=null;
            Color3 ret = new Color3(0,0,0);

            bool currentShadowFlag = difuse;
            bool currentSoftShadowFlag = difuse;

            foreach (Object obj in objects3D)
            {
                if (obj.GetType().Name.Equals("Triangles"))
                {
                    Triangles obj3D = (Triangles)obj;
                    if (obj3D.boundingBox(this))
                    {
                        obj3D.intersect(this);
                    }

                }
                else
                {
                    Object3D obj3D = (Object3D)obj;
                    obj3D.intersect(this);
                }
            }
            
            foreach(Object obj in hits)
            {
                Hit aux = (Hit)obj;
                
                if(hit!=null && aux != null && aux.distanceOToP>Math.Pow(10,-5))
                {
                    if (aux.minDist < hit.minDist)
                    {
                        hit = aux;
                    }
                }else if(hit== null && aux.distanceOToP > Math.Pow(10, -5))
                {
                    hit=aux;
                } 
            }
            
            if (hit != null)
            {
                foreach(Object obj in lights)
                {
                    Light l = (Light)obj;

                    if (ambient)
                    {
                       /*
                        img.pixelAmbientRed[iMap,jMap] = img.pixelAmbientRed[iMap, jMap] + l.color.red * hit.material.color3.red * hit.material.ambiente;
                        img.pixelAmbientBlue[iMap, jMap] = img.pixelAmbientBlue[iMap, jMap] + l.color.blue * hit.material.color3.blue * hit.material.ambiente;
                        img.pixelAmbientGreen[iMap, jMap] = img.pixelAmbientGreen[iMap, jMap] + l.color.green * hit.material.color3.green * hit.material.ambiente;
                        */
 
                        ret.red = ret.red + l.color.red * hit.material.color3.red * hit.material.ambiente;
                        ret.blue = ret.blue + l.color.blue * hit.material.color3.blue * hit.material.ambiente;
                        ret.green = ret.green + l.color.green * hit.material.color3.green * hit.material.ambiente;

                    }

                    Vector4 i = new Vector4(l.x - hit.pointOfIntersection.x, l.y - hit.pointOfIntersection.y, l.z - hit.pointOfIntersection.z,0);

                    double tLight = RaytracerMath.lenghtOfVector(i);

                    i=RaytracerMath.Normalize(i);

                    double costheta = RaytracerMath.dotProduct(hit.normal, i);

                    if (costheta > 0)
                    {
                        Vector4 p = new Vector4(hit.pointOfIntersection.x + Math.Pow(10, -6) * hit.normal.x, hit.pointOfIntersection.y + Math.Pow(10, -6) * hit.normal.y, hit.pointOfIntersection.z + Math.Pow(10, -6) * hit.normal.z, 1);

                        Ray shadowRay = new Ray(p, i);

                        if (shadows)
                        {
                            if (!shadowIntersect(shadowRay,objects3D,hit,tLight))
                            {
                                currentShadowFlag = false;
                            }
                            if (softShadow)
                            {

                                currentSoftShadowFlag = checkSoftShadows(l, hit, p, objects3D, difuse);

                            }

                        }
                                          

                        if (difuse && currentShadowFlag && currentSoftShadowFlag)
                        {
                            /*
                           img.pixelNotDiffuseRed[iMap,jMap] = ret.red;
                           img.pixelNotDiffuseBlue[iMap, jMap] = ret.blue;
                           img.pixelNotDiffuseGreen[iMap, jMap] = ret.green;*/
                            

                            ret.red = ret.red + l.color.red * hit.material.color3.red * hit.material.difusa * costheta;
                            ret.blue = ret.blue + l.color.blue * hit.material.color3.blue * hit.material.difusa * costheta;
                            ret.green = ret.green + l.color.green * hit.material.color3.green * hit.material.difusa * costheta;                           

                        }else if(difuse && currentShadowFlag && !currentSoftShadowFlag)
                        {

                            ret.red = ret.red + l.color.red * hit.material.color3.red * hit.material.difusa * costheta * 0.4;
                            ret.blue = ret.blue + l.color.blue * hit.material.color3.blue * hit.material.difusa * costheta * 0.4;
                            ret.green = ret.green + l.color.green * hit.material.color3.green * hit.material.difusa * costheta * 0.4;
                        }

                    }

                    if (rec > 0)
                    {

                        double cosThetaV = -RaytracerMath.dotProduct(direction, hit.normal);
                        if (hit.material.especular > 0 && specular)
                        {

                            
                            Vector4 p=new Vector4(hit.pointOfIntersection.x, hit.pointOfIntersection.y , hit.pointOfIntersection.z, 1);
                           
                            Vector4 r = new Vector4(direction.x+2*cosThetaV*hit.normal.x, direction.y + 2 * cosThetaV * hit.normal.y, direction.z + 2 * cosThetaV * hit.normal.z,0);
                            r = RaytracerMath.Normalize(r);

                            Ray reflectedRay = new Ray(p, r);

                            Color3 colorReflected = reflectedRay.traceray(rec-1, lights, objects3D, img, shadows, ambient, difuse,specular,refraction,softShadow);

                            /*
                            img.pixelSpecularRed[iMap,jMap] = img.pixelSpecularRed[iMap, jMap] + hit.material.color3.red * (hit.material.especular + (1 - hit.material.especular) * Math.Pow(1 - cosThetaV, 5)) * colorReflected.red;
                            img.pixelSpecularBlue[iMap, jMap] = img.pixelSpecularBlue[iMap, jMap] + hit.material.color3.blue * (hit.material.especular + (1 - hit.material.especular) * Math.Pow(1 - cosThetaV, 5)) * colorReflected.blue;
                            img.pixelSpecularGreen[iMap, jMap] = img.pixelSpecularGreen[iMap, jMap]+ hit.material.color3.green * (hit.material.especular + (1 - hit.material.especular) * Math.Pow(1 - cosThetaV, 5)) * colorReflected.green;
*/
                            
                            ret.red= ret.red+ hit.material.color3.red * (hit.material.especular + (1 - hit.material.especular) * Math.Pow(1 - cosThetaV, 5)) * colorReflected.red;
                            ret.blue =ret.blue + hit.material.color3.blue * (hit.material.especular + (1 - hit.material.especular) * Math.Pow(1 - cosThetaV, 5)) * colorReflected.blue;
                            ret.green = ret.green + hit.material.color3.green * (hit.material.especular + (1 - hit.material.especular) * Math.Pow(1 - cosThetaV, 5)) * colorReflected.green;
                            

                        }

                        if (hit.material.refracao > 0 && refraction)
                        {
                            double eta = 1 / hit.material.indice_de_refracao;
                            double cosThetaR = Math.Sqrt(1 - eta * eta * (1 - cosThetaV * cosThetaV));

                            if (cosThetaV < 0)
                            {
                                eta = hit.material.indice_de_refracao;
                                cosThetaR = -cosThetaR;
                            }

                            Vector4 r = new Vector4(eta * direction.x + (eta * cosThetaV - cosThetaR) * hit.normal.x, eta * direction.y + (eta * cosThetaV - cosThetaR) * hit.normal.y, eta * direction.z + (eta * cosThetaV - cosThetaR) * hit.normal.z,0);
                            r = RaytracerMath.Normalize(r);

                            Ray refractedRay = new Ray(hit.pointOfIntersection, r);

                            Color3 c=refractedRay.traceray(rec - 1, lights, objects3D, img, shadows, ambient, difuse, specular, refraction,softShadow);

                           /*
                            img.pixelRefractionRed[iMap, jMap] = img.pixelRefractionRed[iMap, jMap] + hit.material.color3.red * hit.material.refracao * c.red;
                            img.pixelRefractionBlue[iMap, jMap] = img.pixelRefractionBlue[iMap, jMap] + hit.material.color3.blue * hit.material.refracao * c.blue;
                            img.pixelRefractionGreen[iMap, jMap] = img.pixelRefractionGreen[iMap, jMap] + hit.material.color3.green * hit.material.refracao * c.green;
                             */
                            ret.red = ret.red+ hit.material.color3.red * hit.material.refracao * c.red;
                            ret.blue = ret.blue + hit.material.color3.blue * hit.material.refracao * c.blue;
                            ret.green = ret.green + hit.material.color3.green * hit.material.refracao * c.green;
                             


                        }

                    }
                }

                /**/
                ret.red = ret.red / lights.Count;

                ret.blue = ret.blue / lights.Count;

                ret.green = ret.green / lights.Count;

                return ret;
            }
            else
            {
                /*
                img.pixelNotDiffuseRed[iMap, jMap] = img.color3.red;
                img.pixelNotDiffuseBlue[iMap, jMap] = img.color3.blue;
                img.pixelNotDiffuseGreen[iMap, jMap] = img.color3.green;*/
                return img.color3;
            }
        }




        public bool shadowIntersect(Ray shadowRay, ArrayList objects3D,Hit hit,double tLight) {

            Hit hitShadow = null;

            foreach (Object obj3d in objects3D)
            {
                if (obj3d.GetType().Name.Equals("Triangles"))
                {
                    Triangles obj3ds = (Triangles)obj3d;
                    if (hit.obj3d != obj3ds)
                    {
                        obj3ds.intersect(shadowRay);
                    }
                }
                else
                {
                    Object3D obj3ds = (Object3D)obj3d;
                    if (hit.obj3d != obj3ds)
                    {
                        obj3ds.intersect(shadowRay);
                    }
                }
            }
            foreach (Object obj3ds in shadowRay.getHits())
            {
                Hit auxS = (Hit)obj3ds;

                if (hitShadow != null && auxS != null)
                {
                    if (auxS.minDist < hitShadow.minDist && auxS.distanceOToP > Math.Pow(10, -5))
                    {
                        hitShadow = auxS;
                    }
                }
                else if (hitShadow == null)
                {
                    hitShadow = auxS;
                }
            }

            if (hitShadow != null)
            {
                if (hitShadow.minDist > Math.Pow(10, -6) && hitShadow.minDist < tLight)
                {
                    return false;
                }

            }
            return true;
        }


        //uniform Jitter
        public bool checkSoftShadows(Light l, Hit hit,Vector4 p,ArrayList objects3D,bool difuse)
        {
            int nSamples = 12;

            //do randoms here
            double ix ;
            double iy ;
            double iz = (l.z - hit.pointOfIntersection.z);
            double iw = (1 - hit.pointOfIntersection.w);
            Random rnd = new Random();

            for(int x=0; x < nSamples;x+=4)
            {
                ix = ((l.x - hit.pointOfIntersection.x) - nSamples / 2)+x+rnd.NextDouble();
                for (int y = 0; y < nSamples; y += 4)
                {
                    iy = ((l.y - hit.pointOfIntersection.y) - nSamples / 2)+y+rnd.NextDouble();

                    Vector4 i1 = new Vector4(ix,iy,iz,iw);
                    double tLight = RaytracerMath.lenghtOfVector(i1);
                    i1 = RaytracerMath.Normalize(i1);
                    double costheta1 = RaytracerMath.dotProduct(hit.normal, i1);

                    if (costheta1 > 0)  
                    {
                        Ray shadowRay = new Ray(p, i1);


                        if (!shadowIntersect(shadowRay, objects3D, hit, tLight))
                        {                            
                            return false;
                        }

                    }
                }
            }
            return difuse;
        }


    }
}

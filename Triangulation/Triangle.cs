using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliX.Triangulation
{
    public class Triangle
    {
        public Vector2[] points = new Vector2[3];
        public Triangle[] triangles = new Triangle[3];

        public Vector2 centroid;

        public System.Drawing.Color color;

        //Организовать автоматический перерасчет центроида при изменении точек, использовать свойства
        public Triangle(Vector2 _a, Vector2 _b, Vector2 _c)
        {
            points[0] = _a;
            points[1] = _b;
            points[2] = _c;

            centroid = points[2] - ((points[2] - (points[0] + ((points[1] - points[0]) * 0.5))) * 0.6666666);
        }

        //Возвращает точку из T2 невходящую в T1
        public static Vector2 Get4Point(Triangle T1, Triangle T2)
        {
            for (int i = 0; i < 3; i++)
                if (T2.points[i] != T1.points[0] && T2.points[i] != T1.points[1] && T2.points[i] != T1.points[2])
                    return T2.points[i];

            return null;
        }
    }
}
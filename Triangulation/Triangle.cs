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

       
        public static Vector2 Get4Point(Triangle T1, Triangle T2)
        {
            for (int i = 0; i < 3; i++)
                if (T2.points[i] != T1.points[0] && T2.points[i] != T1.points[1] && T2.points[i] != T1.points[2])
                    return T2.points[i];

            return null;
        }

        //Возвращает 4 точки, где: [0] принадлежит T1, [1] и [2] общие, [3] принадлежит T2
        public static Vector2[] Get4Point2(Triangle T1, Triangle T2)
        {
            Vector2[] Points = new Vector2[4];

            for (int i = 0; i < 3; i++)
            {
                if (T2.points[i] != T1.points[0] && T2.points[i] != T1.points[1] && T2.points[i] != T1.points[2])
                    Points[3] = T2.points[i];

                if (T1.points[i] != T2.points[0] && T1.points[i] != T2.points[1] && T1.points[i] != T2.points[2])
                    Points[0] = T1.points[i];

                if (T2.points[i] == T1.points[1])
                    Points[1] = T2.points[i];
                else if (T2.points[i] == T1.points[2])
                    Points[2] = T2.points[i];

            }

            return Points;
        }
    }
}
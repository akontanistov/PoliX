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
        public Arc[] arcs = new Arc[3];
        public Triangle[] triangles = new Triangle[3];

        public Vector2 centroid;

        public System.Drawing.Color color;

        //Организовать автоматический перерасчет центроида при изменении точек, использовать свойства
        public Triangle(Vector2 _a, Vector2 _b, Vector2 _c)
        {
            points[0] = _a;
            points[1] = _b;
            points[2] = _c;

            arcs[0] = new Arc(_a,_b);
            arcs[1] = new Arc(_b, _c);
            arcs[2] = new Arc(_c, _a);

            centroid = points[2] - ((points[2] - (points[0] + ((points[1] - points[0]) * 0.5))) * 0.6666666);
        }

        public Triangle(Arc _arc, Vector2 _a)
        {
            points[0] = _arc.A;
            points[1] = _arc.B;
            points[2] = _a;

            arcs[0] = _arc;
            arcs[1] = new Arc(points[1], points[2]);
            arcs[2] = new Arc(points[2], points[0]);

            centroid = points[2] - ((points[2] - (points[0] + ((points[1] - points[0]) * 0.5))) * 0.6666666);
        }

        public Triangle(Arc _arc0, Arc _arc1, Arc _arc2)
        {
            arcs[0] = _arc0;
            arcs[1] = _arc1;
            arcs[2] = _arc2;

            points[0] = _arc0.A;
            points[1] = _arc0.B;

            if (_arc1.A == _arc0.A || _arc1.A == _arc0.B)
                points[2] = _arc1.B;
            else if (_arc1.B == _arc0.A || _arc1.B == _arc0.B)
                points[2] = _arc1.A;
            else if (points[2] != _arc2.A && points[2] != _arc2.B)
            { 
                Console.WriteLine("ARC0.A: " + _arc0.A.x + " " + _arc0.A.y);
                Console.WriteLine("ARC0.B: " + _arc0.B.x + " " + _arc0.B.y);
                Console.WriteLine("ARC1.A: " + _arc1.A.x + " " + _arc1.A.y);
                Console.WriteLine("ARC1.B: " + _arc1.B.x + " " + _arc1.B.y);
                Console.WriteLine("ARC2.A: " + _arc2.A.x + " " + _arc2.A.y);
                Console.WriteLine("ARC2.B: " + _arc2.B.x + " " + _arc2.B.y);

                throw new Exception("Попытка создать треугольник из трех непересекающихся ребер");

            }

            centroid = points[2] - ((points[2] - (points[0] + ((points[1] - points[0]) * 0.5))) * 0.6666666);
        }

        public Arc GetArcBeatwen2Points(Vector2 _a, Vector2 _b)
        {
            for (int i = 0; i < 3; i++)
                if (arcs[i].A == _a && arcs[i].B == _b || arcs[i].A == _b && arcs[i].B == _a)
                    return arcs[i];
             
            return null;
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
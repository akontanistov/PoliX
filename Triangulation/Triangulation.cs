using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliX.Triangulation
{
    public class Triangulation
    {
        public List<Vector2> points = new List<Vector2>();
        public List<Triangle> triangles = new List<Triangle>();

        public Triangulation(List<Vector2> _points)
        {
            points = _points;
            //Добавление суперструктуры
            triangles.Add(new Triangle(points[0], points[1], points[2]));
            triangles.Add(new Triangle(points[0], points[2], points[3]));
            //Добавление ссылок в ребра на смежные треугольники суперструктуры

            Triangle CurentTriangle = null;
            Triangle NewTriangle1 = null;
            Triangle NewTriangle2 = null;

            Arc NewArc0 = null;
            Arc NewArc1 = null;
            Arc NewArc2 = null;

            Arc OldArc1 = null;
            Arc OldArc2 = null;

            for (int i = 4; i < _points.Count; i++)
            {
                CurentTriangle = GetTriangleForPoint(_points[i]);

                if (CurentTriangle != null)
                {
                    //Для ускорения алгоритма, найденный треугольник не удаляется, а становится одним из трех новых
                    //Создание новых ребер, которые совместно с ребрами преобразуемого треугольника образуют новые три треугольника 
                    NewArc0 = new Arc(CurentTriangle.points[0], _points[i]);
                    NewArc1 = new Arc(CurentTriangle.points[1], _points[i]);
                    NewArc2 = new Arc(CurentTriangle.points[2], _points[i]);

                    //Сохранение ребер преобразуемого треугольника
                    //OldArc0 = CurentTriangle.GetArcBeatwen2Points(CurentTriangle.points[0], CurentTriangle.points[1]);
                    OldArc1 = CurentTriangle.GetArcBeatwen2Points(CurentTriangle.points[1], CurentTriangle.points[2]);
                    OldArc2 = CurentTriangle.GetArcBeatwen2Points(CurentTriangle.points[2], CurentTriangle.points[0]);

                    //Изменение текущего треугольника: третья точка треугольника заменяется на новую
                    CurentTriangle.points[2] = _points[i];
                    CurentTriangle.arcs[1] = NewArc1;
                    CurentTriangle.arcs[2] = NewArc0;

                    //Дополнительно создаются два треугольника
                    NewTriangle1 = new Triangle(OldArc1, NewArc2, NewArc1);
                    NewTriangle2 = new Triangle(OldArc2, NewArc0, NewArc2);

                    //Новым ребрам передаются ссылки на образующие их треугольники
                    NewArc0.trAB = CurentTriangle;
                    NewArc0.trBA = NewTriangle2;
                    NewArc1.trAB = NewTriangle1;
                    NewArc1.trBA = CurentTriangle;
                    NewArc2.trAB = NewTriangle2;
                    NewArc2.trBA = NewTriangle1;

                    triangles.Add(NewTriangle1);
                    triangles.Add(NewTriangle2);

                    //CheckDelaunayAndRebuild(CurentTriangle);
                    //CheckDelaunayAndRebuild(NewTriangle1);
                    //CheckDelaunayAndRebuild(NewTriangle2);

                }

            }
        }

        //Возвращает триугольник в котором находится данная точка
        private Triangle GetTriangleForPoint(Vector2 _point)
        {
            for(int i = 0; i < triangles.Count; i++)
            {
                if (IsPointInTriangle(triangles[i], _point))
                    return triangles[i];
            }
            return null;
        }

        private bool IsPointInTriangle(Triangle _triangle, Vector2 _point)
        {
            Vector2 P1 = _triangle.points[0];
            Vector2 P2 = _triangle.points[1];
            Vector2 P3 = _triangle.points[2];
            Vector2 P4 = _point;

            double a = (P1.x - P4.x) * (P2.y - P1.y) - (P2.x - P1.x) * (P1.y - P4.y);
            double b = (P2.x - P4.x) * (P3.y - P2.y) - (P3.x - P2.x) * (P2.y - P4.y);
            double c = (P3.x - P4.x) * (P1.y - P3.y) - (P1.x - P3.x) * (P3.y - P4.y);

            if ((a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0))
                return true;
            else
                return false;
        }

        static bool IsDelaunay(Vector2 A, Vector2 B, Vector2 C, Vector2 _CheckNode)
        {
            double x0 = _CheckNode.x;
            double y0 = _CheckNode.y;
            double x1 = A.x;
            double y1 = A.y;
            double x2 = B.x;
            double y2 = B.y;
            double x3 = C.x;
            double y3 = C.y;

            double[] matrix  = { (x1 - x0)*(x1 - x0) + (y1 - y0)*(y1 - y0), x1 - x0, y1 - y0,
                                 (x2 - x0)*(x2 - x0) + (y2 - y0)*(y2 - y0), x2 - x0, y2 - y0,
                                 (x3 - x0)*(x3 - x0) + (y3 - y0)*(y3 - y0), x3 - x0, y3 - y0};

            double matrixDeterminant = matrix[0] * matrix[4] * matrix[8] + matrix[1] * matrix[5] * matrix[6] + matrix[2] * matrix[3] * matrix[7] -
                                        matrix[2] * matrix[4] * matrix[6] - matrix[0] * matrix[5] * matrix[7] - matrix[1] * matrix[3] * matrix[8];

            double a = x1 * y2 * 1 + y1 * 1 * x3 + 1 * x2 * y3
                     - 1 * y2 * x3 - y1 * x2 * 1 - 1 * y3 * x1;

            //Sgn(a)
            if (a < 0)
                matrixDeterminant *= -1d;

            if (matrixDeterminant < 0d)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static void CheckDelaunayAndRebuild(Triangle T1)
        {
            Triangle T2 = null;
            Vector2[] CurentPoints = new Vector2[4];

            for (int i = 0; i < 3; i++)
            {
                T2 = T1.triangles[i];

                if (T2 != null)
                {
                    CurentPoints = Triangle.Get4Point2(T1, T2);
                    if (CurentPoints[0] != null && CurentPoints[1] != null && CurentPoints[2] != null && CurentPoints[3] != null)
                        if (!IsDelaunay(CurentPoints[0], CurentPoints[0], CurentPoints[0], CurentPoints[0]))
                        {
                            //Перестроение
                            T1.points[0] = CurentPoints[0];
                            T1.points[1] = CurentPoints[1];
                            T1.points[2] = CurentPoints[3];

                            T2.points[0] = CurentPoints[3];
                            T2.points[1] = CurentPoints[2];
                            T2.points[2] = CurentPoints[0];
                        }

                }
            }

        }
    }
}

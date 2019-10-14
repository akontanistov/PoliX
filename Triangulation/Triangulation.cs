using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliX.Triangulation
{
    public class Triangulation
    {
        public List<Node> nodes = new List<Node>();
        public List<Triangle> triangles = new List<Triangle>();

        public Triangulation()
        { }

        public Triangulation(Node _node0, Node _node1, Node _node2, Node _node3)
        {
            if(IsDelaunay(_node0, _node1, _node2, _node3))
                triangles.Add(new Triangle(_node0, _node1, _node2));
            if (IsDelaunay(_node0, _node2, _node3, _node1))
                triangles.Add(new Triangle(_node0, _node2, _node3));
            if (IsDelaunay(_node1, _node2, _node3, _node0))
                triangles.Add(new Triangle(_node1, _node2, _node3));
            if (IsDelaunay(_node0, _node1, _node3, _node2))
                triangles.Add(new Triangle(_node0, _node1, _node3));

            nodes.Add(_node0);
            nodes.Add(_node1);
            nodes.Add(_node2);
            nodes.Add(_node3);

        }

        private Triangulation(Triangle _triangle)
        {
            triangles.Add(_triangle);
            nodes.Add(_triangle.nodes[0]);
            nodes.Add(_triangle.nodes[1]);
            nodes.Add(_triangle.nodes[2]);

            ////Удалить
            ////Добавление ссылок на все ноды входящие в триангуляцию
            //for (int i = 0; i < triangles.Count; i++)
            //{
            //    for (int j = 0; j < triangles[i].nodes.Length; j++)
            //    {
            //        if (!nodes.Contains(triangles[i].nodes[j]))
            //        {
            //            nodes.Add(triangles[i].nodes[j]);
            //        }
            //    }
            //}

        }

        public static Triangulation Triangulate(List<Node> _nodes, bool _isHorizontalDivide)
        {
            //Обеспечивает чередование направления деления пространства при рекурсивном выполнении алгоритма.
            bool isHorizontalDivide = _isHorizontalDivide;

            if (_nodes.Count == 3)
            {
                return new Triangulation(new Triangle(_nodes[0], _nodes[1], _nodes[2]));
            }
            else if (_nodes.Count == 4)
            {
                return new Triangulation(_nodes[0], _nodes[1], _nodes[2], _nodes[3]);
            }
            else if (_nodes.Count == 8)
            {
                double[] key = new double[8];
                Node[] _nodesSorted = new Node[8];

                for (int i = 0; i < 8; i++)
                {
                    _nodesSorted[i] = _nodes[i];
                    if(isHorizontalDivide)
                        key[i] = _nodes[i].point.x;
                    else
                        key[i] = _nodes[i].point.y;
                }

                Array.Sort(key, _nodesSorted);

                List<Node> nodes1 = new List<Node>();
                List<Node> nodes2 = new List<Node>();

                for (int i = 0; i < 8; i++)
                {
                    if (i < 4)
                        nodes1.Add(_nodesSorted[i]);
                    else
                        nodes2.Add(_nodesSorted[i]);
                }

                return TriangulationMerge(Triangulate(nodes1, !isHorizontalDivide), Triangulate(nodes2, !isHorizontalDivide), isHorizontalDivide);
            }
            else if (_nodes.Count < 12)
            {
                double[] key = new double[_nodes.Count];
                Node[] _nodesSorted = new Node[_nodes.Count];

                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodesSorted[i] = _nodes[i];
                    if (isHorizontalDivide)
                        key[i] = _nodes[i].point.x;
                    else
                        key[i] = _nodes[i].point.y;
                }

                Array.Sort(key, _nodesSorted);

                List<Node> node1 = new List<Node>();
                List<Node> node2 = new List<Node>();

                for (int i = 0; i < _nodes.Count; i++)
                {
                    if (i < 3)
                        node1.Add(_nodesSorted[i]);
                    else
                        node2.Add(_nodesSorted[i]);
                }

                return TriangulationMerge(Triangulate(node1, !isHorizontalDivide), Triangulate(node2, !isHorizontalDivide), isHorizontalDivide);
            }
            else if (_nodes.Count >= 12)
            {
                double[] key = new double[_nodes.Count];
                Node[] _pointsSorted = new Node[_nodes.Count];

                for (int i = 0; i < _nodes.Count; i++)
                {
                    _pointsSorted[i] = _nodes[i];
                    if (isHorizontalDivide)
                        key[i] = _nodes[i].point.x;
                    else
                        key[i] = _nodes[i].point.y;
                }

                Array.Sort(key, _pointsSorted);

                List<Node> node1 = new List<Node>();
                List<Node> node2 = new List<Node>();

                int firstListLength = _nodes.Count / 2;


                for (int i = 0; i < _nodes.Count; i++)
                {
                    if (i < firstListLength)
                        node1.Add(_pointsSorted[i]);
                    else
                        node2.Add(_pointsSorted[i]);
                }

                return TriangulationMerge(Triangulate(node1, !isHorizontalDivide), Triangulate(node2, !isHorizontalDivide), isHorizontalDivide);
            }
            else
            {
                Console.WriteLine("Ошибка работы алгоритма триангуляции");
                return new Triangulation();
            }
        }

        /**public static Triangulation TriangulateGreedy(List<Point> _points)
        {
            List<Arc> arcs = new List<Arc>();

            //Создание списка всевозможных ребер
            for(int i = 0; i < _points.Count; i++)
            {
                for (int j = i+1; j < _points.Count; j++)
                {
                    arcs.Add(new Arc(_points[i], _points[j]));
                }
            }

            //Сравнить быстродействие
            //Сортировка по убыванию
            //var sortedArcs = from u in arcs
            //                 orderby u.sqrMagnitude
            //                select u;

            double[] sqrDistSort = new double[arcs.Count];
            Arc[] arcsSort = new Arc[arcs.Count];

            for (int i = 0; i < arcs.Count; i++)
            {
                sqrDistSort[i] = arcs[i].sqrMagnitude;
                arcsSort[i] = arcs[i];
            }

            Array.Sort(sqrDistSort, arcsSort);


            //Поиск не пересекающихся ребер
            List<Arc> arcsFinal = new List<Arc>();
            arcsFinal.Add(arcsSort[0]);


            for (int i = 0; i < arcsSort.Length; i++)
            {
                for (int j = 0; j < arcsFinal.Count; j++)
                {
                    if (!Arc.ArcIntersect(arcsSort[i], arcsSort[j]))
                    {
                        arcsFinal.Add(arcsSort[i]);
                    }
                }
            }

            //Преобразование ребер в триангуляцию
            for (int i = 0; i < arcsFinal.Count; i++)
            {
                if (arcsFinal[i].trAB != null && arcsFinal[i].trBA != null)
                    continue;



            }


        }
        **/
        
        //возвращает список 
        public static List<Arc> GetLinkedArcs(Arc targetArc, List<Arc> _arcs)
        {
            return new List<Arc>();
        }

        public static Triangulation TriangulationMerge(Triangulation _T1, Triangulation _T2, bool _isHorisontal)
        {
            Triangulation newTriang = Triangulation.TriangulationMergeSimple(_T1,_T2);

            Node P1 = _T1.nodes[0];
            Node P2 = _T2.nodes[0];
            Node P3 = _T1.nodes[0];
            Node P4 = _T2.nodes[0];

            //Если пространство делится по ширине осуществляется поиск самых верхних и нижних точек, иначе левых и правых
            if (_isHorisontal)
                for(int i = 0; i < _T1.nodes.Count; i++)
                {
                    if(P1.point.y < _T1.nodes[i].point.y)
                        P1 = _T1.nodes[i];
                    if (P3.point.y > _T1.nodes[i].point.y)
                        P3 = _T1.nodes[i];
                }
            else
                for (int i = 0; i < _T1.nodes.Count; i++)
                {
                    if (P1.point.x < _T1.nodes[i].point.x)
                        P1 = _T1.nodes[i];
                    if (P3.point.x > _T1.nodes[i].point.x)
                        P3 = _T1.nodes[i];
                }

            if (_isHorisontal)
                for (int i = 0; i < _T2.nodes.Count; i++)
                {
                    if (P2.point.y < _T2.nodes[i].point.y)
                        P2 = _T2.nodes[i];
                    if (P4.point.y > _T2.nodes[i].point.y)
                        P4 = _T2.nodes[i];
                }
            else
                for (int i = 0; i < _T2.nodes.Count; i++)
                {
                    if (P2.point.x < _T2.nodes[i].point.x)
                        P2 = _T2.nodes[i];
                    if (P4.point.x > _T2.nodes[i].point.x)
                        P4 = _T2.nodes[i];
                }

            P1.TestID = 1;
            P2.TestID = 1;
            P3.TestID = 2;
            P4.TestID = 2;

            //Определение первого ребра в цепочке внешних ребер в Т1 для сращивания 
            //Для этого определяется ребро которое образовывает меньший угол с ребром P1P2
            Arc courceArcT1 = null;
            Vector2 P1P2 = P2.point - P1.point;

            foreach (Arc arc in P1.arcs)
            {
                if (arc.IsBorder)
                {
                    if (courceArcT1 == null)
                    {
                        courceArcT1 = arc;
                    }
                    else if (Vector2.AngleBetweenVectors(courceArcT1.GetSecondNode(P1).point - P1.point, P1P2) > Vector2.AngleBetweenVectors(arc.GetSecondNode(P1).point - P1.point, P1P2))
                    {
                        courceArcT1 = arc;
                    }

                }
            }

            //Определение первого ребра в цепочке внешних ребер в Т2 для сращивания 
            //Для этого определяется ребро которое образовывает меньший угол с ребром P2P1
            Arc courceArcT2 = null;
            Vector2 P2P1 = P1.point - P2.point;

            foreach (Arc arc in P2.arcs)
            {
                if (arc.IsBorder)
                {
                    if (courceArcT2 == null)
                    {
                        courceArcT2 = arc;
                    }
                    else if (Vector2.AngleBetweenVectors(courceArcT2.GetSecondNode(P2).point - P2.point, P2P1) > Vector2.AngleBetweenVectors(arc.GetSecondNode(P2).point - P2.point, P2P1))
                    {
                        courceArcT2 = arc;
                    }
                }
            }

            //Определение последовательности крайних ребер которые будут сращиваться со смежной последовательностью противоположной триангуляции
            List<Arc> borderArcsT1 = new List<Arc>();
            List<Arc> borderArcsT2 = new List<Arc>();

            borderArcsT1.Add(courceArcT1);

            Node curentNode = courceArcT1.GetSecondNode(P1);

            for (int i = 0; i < borderArcsT1.Count; i++)
            {
                if (curentNode == P3)
                {
                    break;
                }
                else
                {
                    borderArcsT1.Add(curentNode.GetSecondBorderArc(borderArcsT1[i]));
                    curentNode = borderArcsT1[i + 1].GetSecondNode(curentNode);
                }
            }

            borderArcsT2.Add(courceArcT2);
            curentNode = courceArcT2.GetSecondNode(P2);

            for (int i = 0; i < borderArcsT2.Count; i++)
            {
                if (curentNode == P4)
                {
                    break;
                }
                else
                {
                    borderArcsT2.Add(curentNode.GetSecondBorderArc(borderArcsT2[i]));
                    curentNode = borderArcsT2[i + 1].GetSecondNode(curentNode);
                }
            }

            //Определение большего массива
            List<Arc> borderArcsMax = null;
            List<Arc> borderArcsMin = null;
            Arc curentArc = new Arc(P1, P2);
            

            if (borderArcsT1.Count > borderArcsT2.Count)
            { borderArcsMax = borderArcsT1; borderArcsMin = borderArcsT2; }
            else
            { borderArcsMax = borderArcsT2; borderArcsMin = borderArcsT1; }

            for (int i = 0; i < borderArcsMax.Count; i++)
            {
                if (i < borderArcsMin.Count)
                {
                    newTriang.triangles.Add(new Triangle(curentArc, borderArcsMin[i], ref curentArc));

                    newTriang.triangles.Add(new Triangle(curentArc, borderArcsMax[i], ref curentArc));
                }
                else
                {
                    //Создание из текущего ребра и оставшихся ребер большего массива
                    newTriang.triangles.Add(new Triangle(curentArc, borderArcsMax[i], ref curentArc));
                }
            }

            return newTriang;
        }


        //1) Найти минимальный отрезок между триангуляциями
        //2) Создать массив всех граничных точек расположенный последовательно от соответствующих точек минимального отрезка для каждой триангуляции.
        //   Для Т1 массив против часовой стрелки, для Т2 по часовой
        //3) Начать строительство новых треугольников от минимального отрезка по массивам граничных точек, до тех пор пока не будет создан отрезок непересекающий ни с одним граничным отрезком обоих триангуляций
        public static Triangulation TriangulationMerge2(Triangulation _T1, Triangulation _T2, bool _isHorisontal)
        {
            Triangulation newTriang = Triangulation.TriangulationMergeSimple(_T1, _T2);

            //Ближайшие точки
            Node P1T1 = null;
            Node P1T2 = null;

            //Граничные ребра последовательно от точек P1T1 и P1T2 соответственно 
            List<Arc> BorderNodesT1;
            List<Arc> BorderNodesT2;

            //Граничные ребра последовательно от точек P1T1 и P1T2 соответственно 
            List<Arc> arcsT1;
            List<Arc> arcsT2;

            //Получение любой крайней точки
            for (int i = 0; i <  _T1.triangles.Count && P1T1 == null; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (_T1.triangles[i].arcs[j].IsBorder)
                    {
                        P1T1 = _T1.triangles[i].arcs[j].A;
                        break;
                    }
                }
            }
            for (int i = 0; i < _T2.triangles.Count && P1T2 == null; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (_T2.triangles[i].arcs[j].IsBorder)
                    {
                        P1T2 = _T2.triangles[i].arcs[j].A;
                        break;
                    }
                }
            }
            //Предварительное заполнение массивов граничных ребер


                return newTriang;
        }


        //Объединяет две триангуляции без перестроения связей
        public static Triangulation TriangulationMergeSimple(Triangulation _T1, Triangulation _T2)
        {
            Triangulation newTriang = new Triangulation();

            foreach (Triangle triang in _T1.triangles)
            {
                newTriang.triangles.Add(triang);
            }

            foreach (Triangle triang in _T2.triangles)
            {
                newTriang.triangles.Add(triang);
            }

            foreach (Node node in _T1.nodes)
            {
                newTriang.nodes.Add(node);
            }

            foreach (Node node in _T2.nodes)
            {
                newTriang.nodes.Add(node);
            }

            return newTriang;
        }

        //Проверка условия делоне через уравнение описанной окружности
        public static bool IsDelaunay(Node A, Node B, Node C, Node _CheckNode)
        {
            double x0 = _CheckNode.point.x;
            double y0 = _CheckNode.point.y;
            double x1 = A.point.x;
            double y1 = A.point.y;
            double x2 = B.point.x;
            double y2 = B.point.y;
            double x3 = C.point.x;
            double y3 = C.point.y;

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


    }
}

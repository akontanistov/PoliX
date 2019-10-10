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
        public List<Arc> arcs = new List<Arc>();
        public List<Arc> arcsBorder = new List<Arc>();
        public List<Triangle> triangles = new List<Triangle>();

        public Triangulation()
        { }

        public Triangulation(Node _node0, Node _node1, Node _node2, Node _node3)
        {
            if(IsDeloney(_node0, _node1, _node2, _node3))
                triangles.Add(new Triangle(_node0, _node1, _node2));
            if (IsDeloney(_node0, _node2, _node3, _node1))
                triangles.Add(new Triangle(_node0, _node2, _node3));
            if (IsDeloney(_node1, _node2, _node3, _node0))
                triangles.Add(new Triangle(_node1, _node2, _node3));
            if (IsDeloney(_node0, _node1, _node3, _node2))
                triangles.Add(new Triangle(_node0, _node1, _node3));

            //Добавление ссылок на все ноды входящие в триангуляцию
            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = 0; j < triangles[i].nodes.Length; j++)
                {
                    if (!nodes.Contains(triangles[i].nodes[j]))
                    {
                        nodes.Add(triangles[i].nodes[j]);
                    }
                }
            }

            //Добавление ссылок на все ребра входящие в триангуляцию
            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = 0; j < triangles[i].arcs.Length; j++)
                {
                    if (!arcs.Contains(triangles[i].arcs[j]))
                    {
                        arcs.Add(triangles[i].arcs[j]);
                    }
                }
            }

            //Поиск границ триангулции в виде массива ребер
            for (int i = 0; i < arcs.Count; i++)
            {
                if (arcs[i].isBorder)
                {
                    arcsBorder.Add(arcs[i]);
                }
            }

        }

        private Triangulation(Triangle _triangle)
        {
            triangles.Add(_triangle);
            nodes.Add(_triangle.nodes[0]);
            nodes.Add(_triangle.nodes[1]);
            nodes.Add(_triangle.nodes[2]);

            //Добавление ссылок на все ноды входящие в триангуляцию
            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = 0; j < triangles[i].nodes.Length; j++)
                {
                    if (!nodes.Contains(triangles[i].nodes[j]))
                    {
                        nodes.Add(triangles[i].nodes[j]);
                    }
                }
            }

            //Добавление ссылок на все ребра входящие в триангуляцию
            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = 0; j < triangles[i].arcs.Length; j++)
                {
                    if (!arcs.Contains(triangles[i].arcs[j]))
                    {
                        arcs.Add(triangles[i].arcs[j]);
                    }
                }
            }
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

            //Поиск общих касательных для триангуляций. Вертикальных либо горизонтальных, в зависимости от расположения триагуляций
            int maxIndexT1 = 0;
            int maxIndexT2 = 0;
            int minIndexT1 = 0;
            int minIndexT2 = 0;

            double maxValue = 0;
            double minValue = 0;

            for (int i = 0; i < _T1.nodes.Count; i++)
            {
                //Поиск самой верхней ноды каждой триангуляции
                if (_isHorisontal)
                {
                    if (maxValue < _T1.nodes[i].point.y)
                    {
                        maxValue = _T1.nodes[i].point.y;
                        maxIndexT1 = i;
                    }
                    if (minValue > _T1.nodes[i].point.y)
                    {
                        minValue = _T1.nodes[i].point.y;
                        minIndexT1 = i;
                    }
                }
                //Поиск самой левой ноды каждой триангуляции
                else
                {
                    if (maxValue < _T1.nodes[i].point.x)
                    {
                        maxValue = _T1.nodes[i].point.x;
                        maxIndexT1 = i;
                    }
                    if (minValue > _T1.nodes[i].point.x)
                    {
                        minValue = _T1.nodes[i].point.x;
                        minIndexT1 = i;
                    }
                }
            }

            maxValue = 0;
            minValue = 0;

            for (int i = 0; i < _T2.nodes.Count; i++)
            {
                //Поиск самой верхней ноды каждой триангуляции
                if (_isHorisontal)
                {
                    if (maxValue < _T2.nodes[i].point.y)
                    {
                        maxValue = _T2.nodes[i].point.y;
                        maxIndexT2 = i;
                    }
                    if (minValue > _T2.nodes[i].point.y)
                    {
                        minValue = _T2.nodes[i].point.y;
                        minIndexT2 = i;
                    }
                }
                //Поиск самой левой ноды каждой триангуляции
                else
                {
                    if (maxValue < _T2.nodes[i].point.x)
                    {
                        maxValue = _T2.nodes[i].point.x;
                        maxIndexT2 = i;
                    }
                    if (minValue > _T2.nodes[i].point.x)
                    {
                        minValue = _T2.nodes[i].point.x;
                        minIndexT2 = i;
                    }
                }
            }

            Node P1 = _T1.nodes[maxIndexT1];
            Node P2 = _T2.nodes[maxIndexT2];
            Node P3 = _T1.nodes[minIndexT1];
            Node P4 = _T2.nodes[minIndexT2];

            //направляющее ребро, по нему определяется по какой части оболочки будут сращиваться триангуляции
            Arc courceArcT1 = null;
            Arc courceArcT2 = null;
            foreach (Arc arc in P1.arcs)
            {
                if (arc.isBorder)
                {
                    if (courceArcT1 == null)
                    {
                        courceArcT1 = arc;
                    }
                    else
                        courceArcT2 = arc;
                }
            }



            //Начальные ребра объединяющие триангуляции
            //Arc topArc = Node.GetCommonArc(P1, P2);
            //Arc bottomArc = Node.GetCommonArc(P3, P4);




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

            foreach (Arc arc in _T1.arcs)
            {
                newTriang.arcs.Add(arc);
            }

            foreach (Arc arc in _T2.arcs)
            {
                newTriang.arcs.Add(arc);
            }

            return newTriang;
        }

        //Проверка условия делоне через уравнение описанной окружности
        public static bool IsDeloney(Node A, Node B, Node C, Node _CheckNode)
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

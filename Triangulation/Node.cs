using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliX.Triangulation
{
    public class Node //Узел
    {
        public Vector2 point;

        //Ссылки на ребра в которые входит данный узел
        public List<Arc> arcs = new List<Arc>();

        //Ссылки на треугольники в которые входит данный узел
        //public List<Triangle> triangles = new List<Triangle>();

        public Node(Vector2 _point)
        {
            point = _point;
        }

        //Возвращает ребро принадлежащее двум нодам
        public static Arc GetCommonArc(Node node1, Node node2)
        {
            for (int i = 0; i < node1.arcs.Count; i++)
            {
                if (node2.arcs.Contains(node1.arcs[i]))
                {
                    return node1.arcs[i];
                }
            }
            return null;
        }

    }
}

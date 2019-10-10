using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliX.Triangulation
{
    public class Triangle
    {
        // [0] - A
        // [1] - B
        // [2] - C
        public Node[] nodes = new Node[3];

        // [0] - AB
        // [1] - AC
        // [2] - BC
        public Arc[] arcs = new Arc[3];

        public Vector2 centroid;

        public System.Drawing.Color color;

        //Организовать автоматический перерасчет центроида при изменении точек, использовать свойства
        public Triangle(Node _a, Node _b, Node _c)
        {
            nodes[0] = _a;
            nodes[1] = _b;
            nodes[2] = _c;

            //Создание ребер и передача ссылок на них в ноды
            arcs[0] = new Arc(_a, _b);
            nodes[0].arcs.Add(arcs[0]);
            nodes[1].arcs.Add(arcs[0]);
            arcs[1] = new Arc(_a, _c);
            nodes[0].arcs.Add(arcs[1]);
            nodes[2].arcs.Add(arcs[1]);
            arcs[2] = new Arc(_b, _c);
            nodes[1].arcs.Add(arcs[2]);
            nodes[2].arcs.Add(arcs[2]);


            //Добавление ссылок на данный треугольник во все ребра
            for (int i = 0; i < 3; i++)
            {
                if (arcs[i].trAB == null)
                {
                    arcs[i].trAB = this;
                }
                else if (arcs[i].trBA == null)
                {
                    arcs[i].isBorder = false;
                    arcs[i].trBA = this;
                }
            }

            centroid = nodes[2].point - ((nodes[2].point - (nodes[0].point + ((nodes[1].point - nodes[0].point) * 0.5f))) * 0.6666666f);
        }

        public Triangle(Arc _a, Arc _b, Arc _c)
        {
            arcs[0] = _a;
            arcs[1] = _b;
            arcs[2] = _c;

            //поиск ссылок на ноды треугольника
            nodes[0] = arcs[0].A;
            nodes[1] = arcs[0].B;

            if (arcs[1].A != nodes[0] && arcs[1].A != nodes[1])
                nodes[2] = arcs[1].A;
            else
                nodes[2] = arcs[1].B;

            //Запись ссылок на данный треугольник в ребра
            for(int i = 0; i < 2; i++)
            {
                if (arcs[i].trAB == null)
                    arcs[i].trAB = this;
                else if (arcs[i].trBA == null)
                    arcs[i].trBA = this;
                else
                    Console.WriteLine("Проблемы с Triangle(Arc _a, Arc _b, Arc _c)");
            }

            centroid = nodes[2].point - ((nodes[2].point - (nodes[0].point + ((nodes[1].point - nodes[0].point) * 0.5f))) * 0.6666666f);
        }
    }
}
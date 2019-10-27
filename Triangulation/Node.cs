using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliX.Triangulation
{
    public class Node //Узел
    {
        public int TestID = 0;


        readonly public Vector2 point;

        public Node(Vector2 _point)
        {
            point = _point;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliX
{
    public class Vector2
    {
        public double x;
        public double y;

        public Vector2()
        {
            x = 0f;
            y = 0f;
        }

        public Vector2(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public double sqrMagnitude()
        {
            return (x * x + y * y);
        }

        public static Vector2 operator-(Vector2 _a, Vector2 _b)
        {
            return new Vector2(_a.x - _b.x, _a.y - _b.y);
        }

        public static Vector2 operator +(Vector2 _a, Vector2 _b)
        {
            return new Vector2(_a.x + _b.x, _a.y + _b.y);
        }

        public static Vector2 operator *(Vector2 _a, double s)
        {
            return new Vector2(_a.x * s, _a.y * s);
        }

        public static double crossProduct(Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v2.x * v1.y;
        }

        public static Vector2 Vector2Rnd(Double RangeXmin, Double RangeXmax, Double RangeYmin, Double RangeYmax)
        {
            return new Vector2(Helper.RndRange(RangeXmin, RangeXmax), Helper.RndRange(RangeYmin, RangeYmax));
        }

    }
}

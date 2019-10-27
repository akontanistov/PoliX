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

        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y);
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

        //Ошибка?
        public static double CrossProduct(Vector2 v1, Vector2 v2) //Векторное произведение
        {
            return v1.x * v2.y - v2.x * v1.y;
        }

        public static double DotProduct(Vector2 v1, Vector2 v2) //Скалярное произведение
        {
            return v1.x * v2.x + v2.y * v1.y;
        }

        public static double AngleBetweenVectors(Vector2 v1, Vector2 v2)
        {
            return Math.Acos(Vector2.DotProduct(v1, v2) / (v1.Magnitude() * v2.Magnitude()));
        }

        public static Vector2 Vector2Rnd(Double RangeXmin, Double RangeXmax, Double RangeYmin, Double RangeYmax)
        {
            return new Vector2(Helper.RndRange(RangeXmin, RangeXmax), Helper.RndRange(RangeYmin, RangeYmax));
        }

        //Если Точка P2 находится слева от вектора (P1-P0) возвращает true
        public static int isLeft(Vector2 P0, Vector2 P1, Vector2 P2)
        {
            double result = ((P1.x - P0.x) * (P2.y - P0.y) - (P2.x - P0.x) * (P1.y - P0.y));

            if (result > 0d)
                return 1;
            else if (result < 0d)
                return -1;
            else
                return 0; //Лежит на линии
        }

    }
}

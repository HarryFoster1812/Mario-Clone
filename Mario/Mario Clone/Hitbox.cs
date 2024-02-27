using System;
using System.Windows;
using static System.Windows.Rect;

namespace Mario_Clone
{
    public class Hitbox
    {
        public bool Active = true;
        public double x;
        public double y;
        public double sizex;
        public double sizey;
        public Hitbox(double X,double Y, double sizeX,double sizeY)
        {
            x = X;
            y = Y;
            sizex = sizeX;
            sizey = sizeY;
        }

        public bool Collides(Hitbox other)
        {
            Rect rect1 = new Rect(x, y, sizex, sizey);
            Rect rect2 = new Rect(other.x, other.y, other.sizex, other.sizey);
            return rect1.IntersectsWith(rect2);
        }

    }

}

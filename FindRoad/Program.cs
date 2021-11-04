using System;

namespace FindRoad
{
    class Program
    {
        static void Main(string[] args)
        {
            Navigation Nav;
            while (true)
            {
                Nav = new Navigation();
                Nav.setMaze(25);
                Nav.setMountain();
                Nav.A_Star_4way_0();

                Console.SetCursorPosition(0, 0);
                Nav.OnDraw();

                Console.ReadLine();
            }
        }
    }
}

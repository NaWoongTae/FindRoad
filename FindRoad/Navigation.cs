using System;
using System.Collections.Generic;
using System.Text;

namespace FindRoad
{
    class Tile
    {
        public Tile prev;
        public int x;
        public int y;

        public int g;
        public int h;

        public int f { get => g + h; }

        public int type; // 출력용
        public Tile(int x, int y, Tile prev = null)
        {
            this.x = x;
            this.y = y;
            this.prev = prev;
        }
    }

    class Navigation
    {
        static int Len = 25;
        Tile[][] grid;
        int[][] mtHeight;
        Random rand = new Random();

        public void setMaze(int size = 0)
        {
            if (size > 0)
                Len = size;

            grid = new Tile[Len][];
            for (int i = 0; i < Len; i++)
            {
                grid[i] = new Tile[Len];
                for (int j = 0; j < Len; j++)
                {
                    grid[i][j] = new Tile(j, i);
                    if (rand.Next(0, 4) == 0)
                    {
                        grid[i][j].type = 1;
                    }
                }
            }

            grid[0][0].type = 2;
            grid[Len - 1][Len - 1].type = 0;
        }

        public void setMaze2(int size = 0)
        {
            if (size > 0)
                Len = size;

            grid = new Tile[Len][];
            for (int i = 0; i < Len; i++)
            {
                grid[i] = new Tile[Len];
                for (int j = 0; j < Len; j++)
                {
                    grid[i][j] = new Tile(j, i);
                }
            }

            grid[1][2].type = 1;
            grid[1][3].type = 1;
            grid[2][0].type = 1;
            grid[2][1].type = 1;

            grid[0][0].type = 2;
            grid[Len - 1][Len - 1].type = 0;
        }

        // 산 만들기
        public void setMountain()
        {
            mtHeight = new int[Len][];
            for (int i = 0; i < Len; i++)
            {
                mtHeight[i] = new int[Len];

                for (int j = 0; j < Len; j++)
                {
                    // mtHeight[i][j] = j * (Len - 1 - i); // 탑 : 우상
                    mtHeight[i][j] = i * (Len - 1 - j); // 탑 : 좌하
                    //Console.Write($"{mtHeight[i][j]} ");
                }
                //Console.WriteLine();
            }
        }

        // ========================================================================

        public void findRoot_0()
        {
            Queue<Tile> open = new Queue<Tile>();
            int[] xpos = new int[4] { 1, 0, -1, 0 };
            int[] ypos = new int[4] { 0, 1, 0, -1 };

            open.Enqueue(grid[0][0]);

            while (open.Count > 0)
            {
                Tile bx = open.Dequeue();
                if (bx.x == Len - 1 && bx.y == Len - 1)
                    break;

                for (int i = 0; i < 4; i++)
                {
                    int x = bx.x + xpos[i];
                    int y = bx.y + ypos[i];

                    if (x < 0 || y < 0 || x >= Len || y >= Len)
                        continue;

                    Tile nxt = grid[y][x];
                    if (nxt.type == 0)
                    {
                        open.Enqueue(nxt);
                        nxt.prev = bx;
                        nxt.type = 2;
                    }
                }
            }

            List<Tile> close = new List<Tile>();
            Tile bx2 = grid[Len - 1][Len - 1];

            while (bx2 != null)
            {
                bx2.type = 3;
                close.Add(bx2);
                bx2 = bx2.prev;
            }

            close.Reverse();
            //var arr = close.ToArray();
        }

        public void findRoot_1()
        {
            Queue<Tile> qTile = new Queue<Tile>();

            int[] dirX = new int[4] { 1, 0, -1, 0 };
            int[] dirY = new int[4] { 0, 1, 0, -1 };

            qTile.Enqueue(grid[0][0]);

            while (qTile.Count > 0)
            {
                Tile tile = qTile.Dequeue();
                if (tile == grid[Len - 1][Len - 1])
                    break;

                for (int i = 0; i < 4; i++)
                {
                    int x = tile.x + dirX[i];
                    int y = tile.y + dirY[i];

                    if (x < 0 || y < 0 || x == Len || y == Len)
                        continue;

                    Tile nxt = grid[y][x];
                    if (nxt.type == 0)
                    {
                        nxt.type = 2;
                        nxt.prev = tile;
                        qTile.Enqueue(nxt);
                    }
                }
            }

            List<Tile> close = new List<Tile>();
            Tile t = grid[Len - 1][Len - 1];
            while (t != null)
            {
                close.Add(t);
                t.type = 3;
                t = t.prev;
            }

            close.Reverse();
        }

        public void findRoot_2()
        {
            Queue<Tile> qBox = new Queue<Tile>();

            int[] dirX = new int[8] { 1, 1, 1, 0, -1, -1, -1, 0 };
            int[] dirY = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };

            qBox.Enqueue(grid[0][0]);

            while (qBox.Count > 0)
            {
                Tile now = qBox.Dequeue();

                if (now == grid[Len - 1][Len - 1])
                    break;

                for (int i = 0; i < 8; i++)
                {
                    int x = now.x + dirX[i];
                    int y = now.y + dirY[i];

                    if (x < 0 || y < 0 || x == Len || y == Len)
                        continue;

                    if (dirX[i] != 0 && dirY[i] != 0)
                    {
                        if (grid[y - dirY[i]][x].type == 1 && grid[y][x - dirX[i]].type == 1)
                            continue;
                    }

                    Tile nxt = grid[y][x];
                    int h = now.h + ((i % 2 == 0) ? 14 : 10);
                    if (nxt.type != 1 && nxt.h > h)
                    {
                        qBox.Enqueue(nxt);
                        nxt.type = 2;
                        nxt.prev = now;
                        //nxt.h = h;
                    }
                }
            }

            List<Tile> tList = new List<Tile>();
            Tile t = grid[Len - 1][Len - 1];
            while (t != null)
            {
                tList.Add(t);
                t.type = 3;
                t = t.prev;
            }

            tList.Reverse();
        }

        public void findRoot_3()
        {
            Queue<Tile> qTile = new Queue<Tile>();

            int[] dirX = new int[8] { 1, 0, -1, 0, 1, 1, -1, -1 };
            int[] dirY = new int[8] { 0, 1, 0, -1, -1, 1, 1, -1 };

            qTile.Enqueue(grid[0][0]);

            while (qTile.Count > 0)
            {
                Tile now = qTile.Dequeue();

                for (int i = 0; i < 8; i++)
                {
                    int x = now.x + dirX[i];
                    int y = now.y + dirY[i];

                    if (x < 0 || y < 0 || x == Len || y == Len)
                        continue;

                    if (3 < i)
                    {
                        if (grid[y][x - dirX[i]].type == 1 && grid[y - dirY[i]][x].type == 1)
                        {
                            continue;
                        }
                    }

                    Tile nxt = grid[y][x];
                    int h = now.h + ((i < 4) ? 10 : 14);
                    if (nxt.type != 1 && nxt.h > h)
                    {
                        nxt.type = 2;
                        //nxt.h = h;
                        nxt.prev = now;
                        qTile.Enqueue(nxt);
                    }
                }
            }

            Tile last = grid[Len - 1][Len - 1];
            List<Tile> t = new List<Tile>();
            while (last != null)
            {
                last.type = 3;
                t.Add(last);
                last = last.prev;
            }
        }

        public int solution()
        {
            int[] dirx = new int[4] { 1, 0, -1, 0 };
            int[] diry = new int[4] { 0, 1, 0, -1 };

            int yLen = grid.Length;// maps.GetLength(0);
            int xLen = grid[0].Length;// maps.GetLength(1);
            //Tile[,] grid = new Tile[yLen, xLen];

            //for (int y = 0; y < yLen; y++)
            //{
            //    for (int x = 0; x < xLen; x++)
            //    {
            //        grid[y, x] = new Tile(x, y, maps[y, x]);
            //    }
            //}

            Queue<Tile> qTile = new Queue<Tile>();
            qTile.Enqueue(grid[0][0]);

            while (qTile.Count > 0)
            {
                Tile now = qTile.Dequeue();
                if (now.x == xLen - 1 && now.y == yLen - 1)
                    break;

                for (int i = 0; i < 4; i++)
                {
                    int x = now.x + dirx[i];
                    int y = now.y + diry[i];

                    if (x < 0 || y < 0 || x == xLen || y == yLen)
                        continue;

                    Tile nxt = grid[y][x];
                    if (nxt.type == 0)
                    {
                        nxt.type = 2;
                        nxt.prev = now;
                        qTile.Enqueue(nxt);
                    }
                }
            }

            Tile last = grid[yLen - 1][xLen - 1];
            Stack<Tile> t = new Stack<Tile>();
            while (last != null)
            {
                last.type = 3;
                t.Push(last);
                last = last.prev;
            }

            // Console.Write($"({t.Count})");

            if (t.Peek() == grid[0][0])
                return t.Count;
            else
                return -1;
        }

        public void findRoot_4()
        {
            int[] dirX = new int[] { 0, 1, 0, -1 };
            int[] dirY = new int[] { 1, 0, -1, 0 };
            Queue<Tile> open = new Queue<Tile>();
            open.Enqueue(grid[0][0]);

            while (open.Count > 0)
            {
                Tile now = open.Dequeue();

                if (now == grid[Len - 1][Len - 1])
                    break;

                for (int i = 0; i < 4; i++)
                {
                    int x = now.x + dirX[i];
                    int y = now.y + dirY[i];

                    if (x < 0 || x >= Len || y < 0 || y >= Len)
                        continue;

                    Tile nxt = grid[y][x];
                    if (nxt.type == 0)
                    {
                        nxt.type = 2;
                        nxt.prev = now;
                        open.Enqueue(nxt);
                    }
                }
            }

            Tile rot = grid[Len - 1][Len - 1];
            while (rot != null)
            {
                rot.type = 3;
                rot = rot.prev;
            }
        }

        public void findRoot_5(int s, int e)
        {
            grid[0][0].type = 0;

            Tile start = grid[s][s];
            Tile end = grid[e][e];
            start.type = 0;
            end.type = 0;

            // =======================================================================

            int[] dirX = new int[8] { 1, 0, -1, 0, 1, 1, -1, -1 };
            int[] dirY = new int[8] { 0, 1, 0, -1, -1, 1, 1, -1 };

            // =======================================================================

            List<Tile> open = new List<Tile>();
            open.Add(start);
            List<Tile> close = new List<Tile>();

            while (open.Count > 0)
            {
                Tile now = open[0];
                open.RemoveAt(0);

                if (now == end)
                    break;

                close.Add(now);

                for (int i = 0; i < 8; i++)
                {
                    int x = now.x + dirX[i];
                    int y = now.y + dirY[i];

                    if (x < 0 || y < 0 || x >= Len || y >= Len) // 그리드 안의 위치
                        continue;

                    Tile nxt = grid[y][x];
                    if (nxt.type == 1 || close.Contains(nxt)) // 벽이거나 닫힌계
                        continue;

                    if (i >= 4) // 막힌 대각선 체크
                    {
                        if (grid[y][x - dirX[i]].type == 1 && grid[y - dirY[i]][x].type == 1)
                            continue;
                    }

                    int h = now.h + ((i < 4) ? 10 : 14);
                    int sub = (end.x - x) - (end.y - y);
                    int g = (int)MathF.Abs(sub) * 10 + (int)MathF.Min(end.x - x, end.y - y) * 14;

                    if (nxt.f == 0) // 첫방문이거나
                    {
                        nxt.h = h;
                        nxt.g = g;
                        nxt.prev = now;
                    }

                    if (open.Contains(nxt)) // 오픈에 있고
                    {
                        if (nxt.f > g + h) // 기존의 비용보다 싸다면
                        {
                            nxt.h = h;
                            nxt.g = g;
                            nxt.prev = now;
                        }
                    }
                    else if (!close.Contains(nxt)) // 클로즈에도 없으면
                    {
                        nxt.type = 2;

                        int m = 0;
                        for (; m < open.Count; m++)
                        {
                            if (open[m].f > nxt.f)
                            {
                                open.Insert(m, nxt);
                                break;
                            }
                        }
                        if (m == open.Count)
                            open.Add(nxt);
                    }

                }
            }

            Tile t = end;
            while (t != null)
            {
                t.type = 3;
                t = t.prev;
            }
        }

        public void Dijkstra()
        {
            int[] dirx = new int[8] { 1, 0, -1, 0, 1, 1, -1, -1 };
            int[] diry = new int[8] { 0, 1, 0, -1, 1, -1, -1, 1 };

            Queue<Tile> open = new Queue<Tile>();
            open.Enqueue(grid[0][0]);

            while (open.Count > 0)
            {
                Tile now = open.Dequeue();

                if (now == grid[Len - 1][Len - 1])
                    break;

                for (int i = 0; i < 8; i++)
                {
                    int x = now.x + dirx[i];
                    int y = now.y + diry[i];

                    if (x < 0 || y < 0 || x >= Len || y >= Len)
                        continue;

                    if (i > 3 && grid[y - diry[i]][x].type == 1 && grid[y][x - dirx[i]].type == 1)
                        continue;

                    Tile nxt = grid[y][x];
                    if (nxt.type == 0)
                    {
                        nxt.type = 2;
                        nxt.prev = now;
                        open.Enqueue(nxt);
                    }
                }
            }

            Tile t = grid[Len - 1][Len - 1];
            while (t != null)
            {
                t.type = 3;
                t = t.prev;
            }
        }

        public void A_Star()
        {
            Tile goal = grid[Len - 1][Len - 1];

            int[] dirx = new int[8] { 1, 0, -1, 0, 1, 1, -1, -1 };
            int[] diry = new int[8] { 0, -1, 0, 1, 1, -1, -1, 1 };

            List<Tile> open = new List<Tile>();
            open.Add(grid[0][0]);

            while (open.Count > 0)
            {
                Tile now = open[0];
                open.RemoveAt(0);

                if (now == goal)
                    break;

                for (int i = 0; i < 8; i++)
                {
                    int x = now.x + dirx[i];
                    int y = now.y + diry[i];

                    if (x < 0 || y < 0 || x >= Len || y >= Len)
                        continue;

                    Tile nxt = grid[y][x];
                    int h = now.h + ((i < 4) ? 10 : 14);
                    int sub = (int)MathF.Abs((goal.x - x) - (goal.y - y));
                    int g = (int)MathF.Min(goal.x - x, goal.y - y) * 14 + sub * 10;

                    if (nxt.type == 0 || (nxt.type == 2 && nxt.f > h + g))
                    {
                        nxt.h = h;
                        nxt.g = g;

                        if (nxt.type == 2)
                            open.Remove(nxt);
                        else if (i > 3 && grid[y - diry[i]][x].type == 1 && grid[y][x - dirx[i]].type == 1)
                            continue;

                        bool chk = false;
                        for (int p = 0; p < open.Count; p++)
                        {
                            if (open[p].f > nxt.f)
                            {
                                open.Insert(p, nxt);
                                chk = true;
                                break;
                            }
                        }
                        if (chk == false)
                            open.Add(nxt);

                        nxt.type = 2;
                        nxt.prev = now;
                    }
                }
            }

            Tile nt = goal;
            while (nt != null)
            {
                nt.type = 3;
                nt = nt.prev;
            }
        }

        public void A_Star_Mountain()
        {
            Tile end = grid[Len - 1][Len - 1];

            int[] dirx = new int[8] { 1, 0, -1, 0, 1, 1, -1, -1 };
            int[] diry = new int[8] { 0, -1, 0, 1, 1, -1, -1, 1 };

            List<Tile> open = new List<Tile>();
            open.Add(grid[0][0]);

            while (open.Count > 0)
            {
                Tile now = open[0];
                open.RemoveAt(0);

                if (now == end)
                    break;

                for (int i = 0; i < 8; i++)
                {
                    int x = now.x + dirx[i];
                    int y = now.y + diry[i];

                    if (x < 0 || y < 0 || x >= Len || y >= Len)
                        continue;

                    Tile nxt = grid[y][x];
                    int h = now.h + ((i > 3) ? 14 : 10);
                    int g = mtHeight[y][x] + 14 * (int)MathF.Min(end.x - x, end.y - y) + 10 * (int)MathF.Abs((end.x - x) - (end.y - y));

                    if (nxt.type == 0 || (nxt.type == 2 && nxt.f > h + g))
                    {
                        if (nxt.type == 2)
                            open.Remove(nxt);
                        else if (i > 3 && grid[y - diry[i]][x].type == 1 && grid[y][x - dirx[i]].type == 1)
                            continue;

                        nxt.h = h;
                        nxt.g = g;

                        bool chk = false;
                        for (int j = 0; j < open.Count; j++)
                        {
                            if (chk = (open[j].f > nxt.f))
                            {
                                open.Insert(j, nxt);
                                break;
                            }
                        }
                        if (chk == false)
                            open.Add(nxt);

                        nxt.type = 2;
                        nxt.prev = now;
                    }
                }
            }

            while (end != null)
            {
                end.type = 3;
                end = end.prev;
            }
        }

        public void Dijkstra_4way_0()
        {
            Tile end = grid[Len - 1][Len - 1];

            int[] dirx = new int[4] { 1, 0, -1, 0 };
            int[] diry = new int[4] { 0, -1, 0, 1 };

            Queue<Tile> open = new Queue<Tile>();
            open.Enqueue(grid[0][0]);

            while(open.Count > 0)
            {
                Tile now = open.Dequeue();

                if (now == end)
                    break;

                for (int i = 0; i < 4; i++)
                {
                    int x = now.x + dirx[i];
                    int y = now.y + diry[i];

                    if (x < 0 || y < 0 || x >= Len || y >= Len)
                        continue;

                    Tile nxt = grid[y][x];
                    if (nxt.type == 0)
                    {
                        nxt.type = 2;
                        nxt.prev = now;
                        open.Enqueue(nxt);
                    }
                }
            }

            while (end != null)
            {
                end.type = 3;
                end = end.prev;
            }
        }

        public void A_Star_4way_0()
        {
            Tile end = grid[Len-1][Len-1];

            int[] dirx = new int[4] { 0, 1, 0, -1 };
            int[] diry = new int[4] { 1, 0, -1, 0 };

            List<Tile> open = new List<Tile>();
            open.Add(grid[0][0]);

            while (open.Count > 0)
            {
                Tile now = open[0];
                open.RemoveAt(0);

                if (now == end)
                    break;

                for (int i = 0; i < 4; i++)
                {
                    int x = now.x + dirx[i];
                    int y = now.y + diry[i];

                    if (x < 0 || y < 0 || x >= Len || y >= Len)
                        continue;

                    int h = now.h + 10;
                    int g = (int)(MathF.Abs(end.x - x) + MathF.Abs(end.y - y)) * 10;

                    Tile nxt = grid[y][x];
                    if (nxt.type == 0 || (nxt.type == 2 && nxt.f > h + g))
                    {
                        nxt.h = h;
                        nxt.g = g;

                        if (nxt.type == 0)
                        {
                            bool chk = false;
                            for (int j = 0; j < open.Count; j++)
                            {
                                if (chk = (open[j].f > nxt.f))
                                {
                                    open.Insert(j, nxt);
                                    break;
                                }
                            }
                            if (!chk)
                                open.Add(nxt);
                        }

                        nxt.type = 2;
                        nxt.prev = now;
                    }
                }
            }

            while (end != null)
            {
                end.type = 3;
                end = end.prev;
            }
        }

        // 다익스트라 + 대각 가능 + 8방향
        public void Dijkstra_0()
        {
            int[] dirX = new int[8] { 1, 1, 1, 0, -1, -1, -1, 0 };
            int[] dirY = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };

            Queue<Tile> open = new Queue<Tile>();
            open.Enqueue(grid[0][0]);
            Tile end = grid[Len - 1][Len - 1];

            while (open.Count > 0)
            {
                Tile now = open.Dequeue();

                for (int i = 0; i < 8; i++)
                {
                    int x = now.x + dirX[i];
                    int y = now.y + dirY[i];

                    if (x < 0 || y < 0 || x >= Len || y >= Len)
                        continue;

                    Tile nxt = grid[y][x];
                    if (nxt.type == 0)
                    {
                        open.Enqueue(nxt);
                        nxt.type = 2;
                        nxt.prev = now;
                    }
                }                
            }

            while (end != null)
            {
                end.type = 3;
                end = end.prev;                
            }
        }

        // A* + 8방향
        public void A_Star_0()
        {
            int[] dirX = new int[8] { 1, 1, 1, 0, -1, -1, -1, 0 };
            int[] dirY = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };

            List<Tile> open = new List<Tile>();
            open.Add(grid[0][0]);
            Tile end = grid[Len - 1][Len - 1];

            while (open.Count > 0)
            {
                Tile now = open[0];
                open.RemoveAt(0);

                if (now == end)
                    break;

                for (int i = 0; i < 8; i++)
                {
                    int x = now.x + dirX[i];
                    int y = now.y + dirY[i];

                    if (x < 0 || y < 0 || x >= Len || y >= Len)
                        continue;

                    if (grid[y][x - dirX[i]].type == 1 && grid[y - dirY[i]][x].type == 1)
                        continue;

                    int h = now.h + ((i % 2 == 1) ? 10 : 14);
                    int g = (int)MathF.Min(end.x - x, end.y - y) * 14 + (int)MathF.Abs((end.x - x) - (end.y - y)) * 10;

                    Tile nxt = grid[y][x];

                    if (nxt.type == 0 || (nxt.type == 2 && nxt.f > h + g))
                    {
                        nxt.h = h;
                        nxt.g = g;

                        if (open.Contains(nxt))
                            open.Remove(nxt);

                        bool isAdd = false;
                        for (int j = 0; j < open.Count; j++)
                        {
                            if (isAdd = (open[j].f > nxt.f))
                            {
                                open.Insert(j, nxt);
                                break;
                            }
                        }
                        if (!isAdd)
                            open.Add(nxt);

                        nxt.type = 2;
                        nxt.prev = now;
                    }
                }
            }

            while(end != null)
            {
                end.type = 3;
                end = end.prev;
            }
        }

        /// <summary> 출력용 메서드 </summary>
        public void OnDraw()
        {
            for (int i = 0; i < Len; i++)
            {
                for (int j = 0; j < Len; j++)
                {
                    switch (grid[i][j].type)
                    {
                        case 0:
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write('□');
                            break;
                        case 1:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write('■');
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write('＠');
                            break;
                        case 3:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write('☆');
                            break;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}

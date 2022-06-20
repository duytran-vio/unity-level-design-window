using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAI
{
    static int m, n;
    static int [,] cost;
    public struct Point{
        public int x;
        public int y;

        public Point(int _x, int _y){
            x = _x;
            y = _y;
        }
    };

    static Point start, end;

    static int [] h = new int[5]{1, 0, -1, 0, 1};

    static public Point [,] trace;
    static int [,] d;
    static bool [,] inPath;

    static public int minPathValue = -1;

    static public int attemptStepsValue = 0;
    static public Point [] attemptSteps;

    static public Point [] minPath;

    static public int nMinPath;

    static public float diffRating;

    static public void addData(int _n, int _m, int [,] _cost, int _startX, int _startY, int _endX, int _endY){
        n = _n;
        m = _m;
        cost = new int [n, m];
        for(int i = 0; i < n; i++){
            for(int j = 0; j < m; j++){
                cost[i, j] = _cost[i, j];
                // Debug.Log("cost[" + i + "," + j + "]= " + cost[i,j]);
            }
        }

        start.x = _startX;
        start.y = _startY;
        end.x = _endX;
        end.y = _endY;

        Init();
    }
    
    static void Init(){
        minPath = new Point[n * m];
        nMinPath = 0;
        attemptStepsValue = 0;
        attemptSteps = new Point[n * m];
        d = new int [n, m];
        trace = new Point[n, m];
        inPath = new bool[n, m];
        // Queue<Point> Q = new Queue<Point>();
        for(int i = 0; i < n; i++){
            for(int j = 0; j < m; j++){
                d[i,j] = (int)1e9;
                inPath[i,j] = false;
                trace[i,j].x = -1;
                trace[i,j].y = -1;
            }
        }
    }

    static int disToEnd(int x, int y){
        return Mathf.Abs(end.x - x)  +  Mathf.Abs(end.y - y) ;
    }

    static bool valid(int x, int y){
        return x >=0 && x < n && y >= 0 && y < m && cost[x, y] != -1 ;
    }

    static Point FindLowest(){
        int c = (int)1e9;
        Point res = new Point(-1, -1);
        for(int i = 0; i < n; i++){
            for(int j = 0; j < m; j++){
                if (inPath[i,j]) continue;
                if (d[i,j] + disToEnd(i, j) < c){
                    c = d[i,j] + disToEnd(i, j);
                    res.x = i;
                    res.y = j;
                }
            }
        }
        return res;
    }

    static public void FindMiniumPath(){
        
        
        d[start.x, start.y] = 0;
        
        // Q.Enqueue(start);
        while(true){
            Point u = FindLowest();
            // Debug.Log("u: " + u.x + " " + u.y);
            if (u.x == -1 && u.y == -1){
                minPathValue = -1;
                break;
            }
            attemptStepsValue++;
            attemptSteps[attemptStepsValue - 1] = new Point(u.x, u.y);
            inPath[u.x, u.y] = true;
            if (inPath[end.x, end.y]){
                minPathValue = d[end.x, end.y];
                break;
            }
            // u.x = Q.Peek().x;
            // u.y = Q.Peek().y;
            // Q.Dequeue();
            for (int i = 0; i < 4; i++){
                Point v;
                v.x = u.x + h[i];
                v.y = u.y + h[i + 1];
                // Debug.Log("v: " + v.x + " " + v.y + " cost: " + cost[v.x, v.y]);

                if (valid(v.x, v.y) && !inPath[v.x, v.y] && d[u.x,u.y] + cost[v.x,v.y] < d[v.x,v.y]){
                    d[v.x, v.y] = d[u.x, u.y] + cost[v.x, v.y];
                    trace[v.x, v.y] = new Point(u.x, u.y);
                }
            }
        }
        GetMinPath();
        diffRating = (float)1 - ((float)nMinPath / (nMinPath + (attemptStepsValue - 2 - nMinPath) ));
    }

    static public void GetMinPath(){
        if (minPathValue == -1) return;
        Point u = end;
        minPath[nMinPath] = new Point(u.x, u.y);
        nMinPath++;
        while(u.x != start.x || u.y != start.y){
            // Debug.Log("d["+u.x+","+u.y+"] = "+d[u.x,u.y] + ", " + cost[u.x,u.y]);
            var x = trace[u.x, u.y].x;
            var y = trace[u.x, u.y].y;
            minPath[nMinPath] = new Point(x, y);
            nMinPath++;
            u = trace[u.x, u.y];
        }
    }
}

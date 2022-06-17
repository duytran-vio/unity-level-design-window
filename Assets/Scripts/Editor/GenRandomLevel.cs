using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenRandomLevel
{
    static public int n, m;
    static float obs_per;

    static string [] movable_cell;
    static string start_sym;
    static string end_sym;
    static string [] obs_cells;

    public struct Point{
        public int x;
        public int y;

        public Point(int _x, int _y){
            x = _x;
            y = _y;
        }
    };

    public static Point start, end;

    public static string[,] level;

    public static void AddLevelData(int _n, int _m, float _obs_per, string[] _movable_cell, string _start_sym, string _end_sym, string [] _obs_cells){
        n = _n;
        m = _m;
        obs_per = _obs_per;
        // movable_cell = _movable_cell;
        movable_cell = new string[_movable_cell.Length];
        for(int i = 0; i < _movable_cell.Length; i++){
            movable_cell[i] = _movable_cell[i];
        }
        start_sym = _start_sym;
        end_sym = _end_sym;
        obs_cells = new string[_obs_cells.Length];
        for(int i = 0; i < _obs_cells.Length; i++){
            obs_cells[i] = _obs_cells[i];
        }
    }

    public static Point create_random_coordinate(int minX, int maxX, int minY, int maxY){
        int x = (int)Random.Range(minX, maxX);
        int y = (int)Random.Range(minY, maxY);
        // Debug.Log("random point: " + minX + " "+ maxX + " "+ minY + " " + maxY + ", Point: "+ x + " " + y);
        return new Point(x, y);
    }

    public static void GenNewRandomLevel(){
        level = new string[n, m];
        for(int i = 0; i < n; i++){
            for(int j = 0; j < m; j++){
                int index = (int)Random.Range(0, movable_cell.Length);
                // Debug.Log(index + movable_cell[index]);
                level[i,j] = movable_cell[index];
            }
        }
        start = create_random_coordinate(0, n/2, 0, m/2);
        end = create_random_coordinate(n/2 + 1, n, m/2 + 1, m);
        int n_obs = (int)Mathf.Round(n * m * obs_per);
        // Debug.Log(n_obs);
        for(int i = 0; i < n_obs; i++){
            Point cell;
            do{
                cell = create_random_coordinate(0, n, 0, m);
            }
            while ((cell.x == start.x && cell.y == start.y) || (cell.x == end.x && cell.y == end.y) || !isMovableCell(level[cell.x,cell.y]));
            int index = (int)Random.Range(0, obs_cells.Length);
            // Debug.Log(""+ index + " " + obs_cells[index]);
            level[cell.x,cell.y] = obs_cells[index];
        }
        level[start.x, start.y] = start_sym;
        level[end.x, end.y] = end_sym;
    }

    static bool isMovableCell(string s){
        for(int i = 0; i < movable_cell.Length; i++){
            if (movable_cell[i] == s){
                return true;
            }
        }
        return false;
    }
}

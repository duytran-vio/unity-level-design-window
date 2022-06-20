using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelToScene: EditorWindow
{
    Rect viewSection;

    static GameObject map;

    static string [,] a;
    
    public struct Point{
        public int x;
        public int y;

        public Point(int _x, int _y){
            x = _x;
            y = _y;
        }
    };

    static Point start, end;

    static int n, m ;

    static Dictionary<string, GameObject> moveable_prefab;
    static Dictionary<string, GameObject> obs_prefab;

    static float offsetX, offsetY;

    static GameObject player;

    static string wallAssetPath = "Assets/Resources/wall.prefab";

    [MenuItem("Extend/Spawn object to Scene")]
    static void OpenWindow(){
        var window = GetWindow<LevelToScene>("Level Design");
        window.position = new Rect(0, 0, 600, 400);
        window.Show();
        // InitColor();
    }

    void OnGUI(){
        testing();
    }

    static void testing(){
        // string path = "Assets/Resources/object.prefab";
        // GameObject a = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (GUILayout.Button("Spawn", GUILayout.Width(90))){
            GameObject aOnScene = Instantiate(obs_prefab["X"], new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
            aOnScene.name = "ground";
            aOnScene.transform.parent = map.transform;
            
        }
        if (GUILayout.Button("Spawn map", GUILayout.Width(90))){
            // GameObject aOnScene = Instantiate(a, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
            // aOnScene.name = "ground";
            spawnMap();
        }
    }

    
    static public void addData(int _n, int _m, string [,] _a, Dictionary<string, string> _moveable_cell, Dictionary<string, string> _obs_cell){

        offsetX = 0;
        offsetY = 0;
        n = _m;
        m = _n;
        a = new string [n,m];
        for(int i = 0; i < _n; i++){
            for(int j = 0; j < _m; j++){
                a[j,_n - i - 1] = _a[i, j];
                if (a[j,_n - i - 1] == "S"){
                    start.x = j;
                    start.y = _n - i - 1;
                }
                if (a[j,_n - i - 1] == "T"){
                    end.x = j;
                    end.y = _n - i - 1;
                }
            }
        }
        LoadPrefab(_moveable_cell, _obs_cell);
    }

    static void LoadPrefab(Dictionary<string, string> _moveable_cell, Dictionary<string, string> _obs_cell){
        moveable_prefab = new Dictionary<string, GameObject>();
        obs_prefab = new Dictionary<string, GameObject>();
        foreach(KeyValuePair<string, string> e in _moveable_cell){
            moveable_prefab.Add(e.Key, AssetDatabase.LoadAssetAtPath<GameObject>(e.Value));
        }
        foreach(KeyValuePair<string, string> e in _obs_cell){
            obs_prefab.Add(e.Key, AssetDatabase.LoadAssetAtPath<GameObject>(e.Value));
        }
        obs_prefab.Add("W", AssetDatabase.LoadAssetAtPath<GameObject>(wallAssetPath));
    }

    static public void SpawnLevelToScene(){
        spawnMap();
        spawnWall();
        for(int i = 0; i < n; i++){
            for(int j = 0; j < m; j++){
                spawnCube(a[i,j], i, j);
            }
        }
        spawnPlayer(start.x, start.y);
    }

    static void spawnPlayer(int x, int y){
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null){
            DestroyImmediate(player);
        }
        string player_path = "Assets/Resources/Player.prefab";
        GameObject player_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(player_path);
        player = Instantiate(player_prefab, new Vector3(x + offsetX, 1.5f, y + offsetY), Quaternion.identity);
        player.tag = "Player";
        player.name = "Player";
        LevelSolution levelSolution = player.GetComponent<PlayerController>().levelSolution;
        levelSolution.nMinPath = PathfindingAI.nMinPath;
        levelSolution.x = new int[levelSolution.nMinPath];
        levelSolution.y = new int[levelSolution.nMinPath];
        for(int i = 0; i < levelSolution.nMinPath; i++){
            levelSolution.x[i] = PathfindingAI.minPath[i].y;
            levelSolution.y[i] = m - PathfindingAI.minPath[i].x - 1;
        }

        // levelSolution.minPath = new LevelSolution.Point[levelSolution.nMinPath];
        // for(int i = 0; i < levelSolution.nMinPath; i++){
        //     levelSolution.minPath[i].x = PathfindingAI.minPath[i].x;
        //     levelSolution.minPath[i].y = PathfindingAI.minPath[i].y;
        // }
        // levelSolution.test = new LevelSolution.Point();
        // levelSolution.test.x = 2;
        // levelSolution.test.y = 2;
    }

    static void spawnMap(){
        map = GameObject.FindGameObjectWithTag("Map");
        if (map != null){
            DestroyImmediate(map);
        }
        map = new GameObject("Map");
        map.tag = "Map";
    }

    static void spawnWall(){
        for(int i = 0; i < n; i++){
            spawnObstacleCube("W", i, -1);
            spawnObstacleCube("W", i, m);
        }

        for(int i = 0; i < m; i++){
            spawnObstacleCube("W", -1, i);
            spawnObstacleCube("W", n, i);
        }
    }
    
    static void spawnCube(string cell_value, int x, int y){
        if (moveable_prefab.ContainsKey(cell_value)){
            spawnMoveableCube(cell_value, x, y);
        }
        else{
            spawnObstacleCube(cell_value, x, y);
        }
    }

    static void spawnMoveableCube(string cell_value, int x, int y){
        GameObject cube = Instantiate(moveable_prefab[cell_value], new Vector3(x + offsetX, 0, y + offsetY), Quaternion.identity);
        cube.transform.parent = map.transform;
    }

    static void spawnObstacleCube(string cell_value, int x, int y){
        GameObject cube = Instantiate(obs_prefab[cell_value], new Vector3(x + offsetX,  0, y + offsetY), Quaternion.identity);
        GameObject cube2 = Instantiate(obs_prefab[cell_value], new Vector3(x + offsetX, 1, y + offsetY), Quaternion.identity);
        cube.transform.parent = map.transform;
        cube2.transform.parent = map.transform;
    }

    void Update(){
        
    }
    
}

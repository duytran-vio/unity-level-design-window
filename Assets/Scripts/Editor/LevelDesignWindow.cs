using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelEvalData{
    public string levelDataPath;
    public int minPathValue;
    public int attemptedSteps;
    public float levelDifficulty;
}

public class LevelDesignWindow : EditorWindow
{
    struct Point{
        public int X;
        public int Y;
    }
    Rect levelListSection;
    Rect levelCustomSection;

    Rect levelPreviewSection;

    private int maxSizeX;
    private int maxSizeY;

    private int sizeX;
    private int sizeY;
    private Point start;
    private Point end;

    private float cellWidth;
    private string cellOption;

    public string [,] a;

    public string [,] originMap;
    public bool isOriginMapPreview;

    private string selectLevelFile;

    public Dictionary<string, GUIStyle> cellStyle;
    private string save_file_name;
    private string ROOT_SAVE;

    public Vector2 scrollPos = Vector2.zero;
    public Vector2 scrollLevelListPos = Vector2.zero;

    static float windowWidth = 1200f;
    static float windowHeight = 710f;

    private float previewLevelScrollWidth;
    private float previewLevelScrollHeight;
    private bool isSolveLevel;

    private string defaultCell;

    public LevelEvalData levelEvalData;

    public float obs_per;

    public int nEasyLevel;
    public int nHardLevel;

    public bool viewGenMultiLevel;

    private int tabMode;

    private float diffDecisionRate;

    Dictionary<string, string> moveable_asset = new Dictionary<string, string>(){
            {"O", "Assets/Resources/moveable.prefab"},
            {"R", "Assets/Resources/river.prefab"},
            {"A", "Assets/Resources/sand.prefab"},
            {"S", "Assets/Resources/player.prefab"},
            {"T", "Assets/Resources/player.prefab"},
            {"o", "Assets/Resources/attempt.prefab"},
            {"*", "Assets/Resources/minpath.prefab"}
        };

    Dictionary<string, string> obs_asset = new Dictionary<string, string>(){
            {"X", "Assets/Resources/obstacle.prefab"},
        };

    [MenuItem("Extend/LevelDesignWindow")]
    static void OpenWindow(){
        var window = GetWindow<LevelDesignWindow>("Level Design");
        window.position = new Rect(0, 0, windowWidth, windowHeight);
        window.Show();
        // InitColor();
    }

    void Awake(){
        maxSizeX = 60;
        maxSizeY = 60;
        cellWidth = 20f;
        ROOT_SAVE = "Assets/Resources/LevelDesign/";
        selectLevelFile = "";
        isOriginMapPreview = true;
        defaultCell = "X";
        diffDecisionRate = 0.7f;
        
        InitParams();
    }

    void InitParams(){
        cellOption = "X";
        sizeX = 0;
        sizeY = 0;
        start.X = 0;
        start.Y = 0;
        end.X = 0;
        end.Y = 0;
        save_file_name = "";
        isSolveLevel = false;
        obs_per = 0;
        nEasyLevel = 0;
        nHardLevel = 0;
        viewGenMultiLevel = false;
        tabMode = 0;
    }

    void InitColor(){
        var red = new GUIStyle(GUI.skin.button);
        red.normal.textColor = Color.red;
        var green = new GUIStyle(GUI.skin.button);
        green.normal.textColor = Color.green;
        var blue = new GUIStyle(GUI.skin.button);
        blue.normal.textColor = Color.blue;
        var yellow = new GUIStyle(GUI.skin.button);
        yellow.normal.textColor = Color.yellow;

        var white = new GUIStyle(GUI.skin.button);
        white.normal.textColor = Color.white;

        var pink = new GUIStyle(GUI.skin.button);
        pink.normal.textColor = new Color(236f/255f, 72f/255f, 174f/255f); 

        var lightblue = new GUIStyle(GUI.skin.button);
        lightblue.normal.textColor = new Color(79f/255f, 230f/255f, 230f/255f); 

        cellStyle = new Dictionary<string, GUIStyle>(){
            {"X", red},
            {"O", green},
            {"R", blue},
            {"A", yellow},
            {"S", white},
            {"T", white},
            {"o", pink},
            {"*", lightblue}
        };
    }

    void OnGUI(){
        InitColor();
        DrawLayout();    
        DrawLevelListSection();
        DrawLevelCustomSection();
        DrawLevelPreviewSection();
    }

    void DrawLayout(){
        levelListSection.x = 0;
        levelListSection.y = 0;
        levelListSection.width = 150;
        levelListSection.height = Screen.height;

        
        levelCustomSection.x = levelListSection.width;
        levelCustomSection.y = 0;
        levelCustomSection.width = 300f;
        levelCustomSection.height = Screen.height;

        levelPreviewSection.x = levelListSection.width + levelCustomSection.width;
        levelPreviewSection.y = 0;
        levelPreviewSection.width = Screen.width - levelPreviewSection.x;
        levelPreviewSection.height = Screen.height;

        previewLevelScrollWidth = Screen.width - levelPreviewSection.x;
        previewLevelScrollHeight = Screen.height - 40f;
    }

    void DrawLevelListSection(){
        GUILayout.BeginArea(levelListSection);
        EditorGUILayout.LabelField("Level List");
        EditorGUILayout.Space();

        var info = new DirectoryInfo(ROOT_SAVE);
        var file_lst = info.GetFiles();

        if (GUILayout.Button("New level...")){
            selectLevelFile = "";
            save_file_name = "";
            isSolveLevel = false;
            isOriginMapPreview = true;
            InitParams();
        }

        EditorGUILayout.Space();
        scrollLevelListPos = EditorGUILayout.BeginScrollView(scrollLevelListPos, GUILayout.Width(levelListSection.width), GUILayout.Height(levelListSection.height - 85f));
        for(int i = 0 ; i < file_lst.Length; i++){
            string[] name_split = file_lst[i].Name.Split('\\');
            string file_name = name_split[name_split.Length - 1];
            string [] file_name_ele = file_name.Split('.');
            if (file_name_ele[file_name_ele.Length - 1] == "meta") continue;
            if (GUILayout.Button(file_name)){
                selectLevelFile = file_name;
                save_file_name = file_name_ele[0];
                isSolveLevel = false;
                isOriginMapPreview = true;
                ReadFromLevelFile();
            }
        }

        
        EditorGUILayout.EndScrollView();
        
        GUILayout.EndArea();
    }

    void ReadFromLevelFile(){
        if (selectLevelFile == ""){
            return;
        }   

        string file_path = ROOT_SAVE + selectLevelFile;
        StreamReader reader = new StreamReader(file_path);

        string temp = reader.ReadLine();
        string[] s = temp.Split(' ');

        sizeX = (int)System.Convert.ToInt64(s[0]);
        sizeY = (int)System.Convert.ToInt64(s[1]);
        start.X = (int)System.Convert.ToInt64(s[2]);
        start.Y = (int)System.Convert.ToInt64(s[3]);
        end.X = (int)System.Convert.ToInt64(s[4]);
        end.Y = (int)System.Convert.ToInt64(s[5]);

        a = new string[sizeX, sizeY];
        
        for (int i = 0; i < sizeX; i++){
            for(int j = 0; j < sizeY; j++){
                temp = reader.ReadLine();
                s = temp.Split(' ');
                int x = (int)System.Convert.ToInt64(s[0]);
                int y = (int)System.Convert.ToInt64(s[1]);
                a[x,y] = s[2];
            }
        }

        // save_file_name = selectLevelFile;
    }

    void DrawLevelCustomSection(){
        GUILayout.BeginArea(levelCustomSection);
        // scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(1000 - levelListSection.width), GUILayout.Height(710));
        
        tabMode = GUILayout.Toolbar((int)tabMode, new string[]{"Level Customize", "Generate multi level"});

        switch (tabMode){
            case 0: DrawLevelCustomize(); break;
            case 1: DrawGenMultiLevel(); break;
        }      
        // DrawLevelArea();
        // EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    void DrawLevelCustomize(){
        EditorGUILayout.LabelField("Level Customize");
                
        DrawSize();
        DrawPosition();
        DrawCellOption();
        DrawSaveResetLevel();
        DrawRunPathFindingAI();
        DrawLevelToScene();
    }

    void DrawSize(){
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Size");
        {
            int oldSizeX = sizeX;
            int oldSizeY = sizeY;
            sizeX = (int)EditorGUILayout.Slider("X", sizeX, 0, maxSizeX);
            sizeY = (int)EditorGUILayout.Slider("Y", sizeY, 0, maxSizeY);
            if (GUI.changed){
                ReSizeMatrix(oldSizeX, oldSizeY);
            }

            obs_per = EditorGUILayout.FloatField("Obstacle Percent: ", obs_per);
            // GUILayout.BeginHorizontal();

            if (GUILayout.Button("Random Level", GUILayout.Width(90))){
                GenUntilExistPath();
            }

            // if (GUILayout.Button("Generate multi levels", GUILayout.Width(150))){
            //     viewGenMultiLevel = !viewGenMultiLevel;
            // }

            // GUILayout.EndHorizontal();

            // if(viewGenMultiLevel){
            //     DrawGenMultiLevel();
            // }
        }
    }

    void DrawGenMultiLevel(){
        EditorGUILayout.Space();
        nEasyLevel = EditorGUILayout.IntField("Number of easy level:", nEasyLevel);
        nHardLevel = EditorGUILayout.IntField("Number of hard level:", nHardLevel);
        if (GUILayout.Button("Generate!", GUILayout.Width(90))){
            RunGenMultiLevel();
        }
    }

    void RunGenMultiLevel(){
        int cntEasy = 0;
        int cntHard = 0;
        while(cntEasy < nEasyLevel){
            sizeX = (int)Random.Range(5, 40);
            sizeY = (int)Random.Range(5, 40);
            obs_per = Random.Range(0.1f, 0.4f);
            GenUntilExistPath();
            if (PathfindingAI.diffRating < diffDecisionRate){
                cntEasy++;
                save_file_name = "easy_" + cntEasy;
                WriteLevelToFile();
            }
        }

        while(cntHard < nHardLevel){
            sizeX = (int)Random.Range(20, 60);
            sizeY = (int)Random.Range(20, 60);
            obs_per = Random.Range(0.3f, 0.5f);
            GenUntilExistPath();
            if (PathfindingAI.diffRating >= diffDecisionRate){
                cntHard++;
                save_file_name = "hard_" + cntHard;
                WriteLevelToFile();
            }
        }
    }

    void GenUntilExistPath(){
        do{
            GenerateRandomLevel();
            isSolveLevel = true;
            AddDataPathFinding();
            PathfindingAI.FindMiniumPath();
        }while (PathfindingAI.minPathValue == -1 && obs_per <= 0.5);
    }

    void GenerateRandomLevel(){
        GenRandomLevel.AddLevelData(sizeX, sizeY, obs_per, new string[]{"O","O","O","O","O","O", "R", "A"} , "S", "T", new string[]{"X"});
        GenRandomLevel.GenNewRandomLevel();
        start.X = GenRandomLevel.start.x;
        start.Y = GenRandomLevel.start.y;
        end.X = GenRandomLevel.end.x;
        end.Y = GenRandomLevel.end.y;

        a = new string[sizeX, sizeY];
        for(int i = 0; i < sizeX; i++){
            for(int j = 0; j < sizeY; j++){
                a[i,j] = GenRandomLevel.level[i, j];
            }
        }

    }

    void ReSizeMatrix(int oldSizeX, int oldSizeY){
        string[,] b = new string[oldSizeX, oldSizeY];
        for(int i = 0; i < oldSizeX; i++){
            for(int j = 0; j < oldSizeY; j++){
                b[i,j] = a[i,j];
            }
        }

        a = new string [sizeX,sizeY];
        for(int i = 0; i < sizeX; i++){
            for(int j = 0; j < sizeY; j++){
                a[i,j] = defaultCell;
            }
        }
        
        for(int i = 0; i < Mathf.Min(oldSizeX, sizeX); i++){
            for(int j = 0; j < Mathf.Min(oldSizeY, sizeY); j++){
                a[i,j] = b[i,j];
            }
        }

        start.X = Mathf.Max(Mathf.Min(start.X, sizeX - 1), 0);
        start.Y = Mathf.Max(Mathf.Min(start.Y, sizeY - 1), 0);
        end.X = Mathf.Max(Mathf.Min(end.X, sizeX - 1), 0);
        end.Y = Mathf.Max(Mathf.Min(end.Y, sizeY - 1), 0);

        b = null;
    }

    void DrawPosition(){
        EditorGUILayout.Space();
        Point oldStart = start;
        Point oldEnd = end;
        {
            // GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Start position:");
            start.X = (int)EditorGUILayout.Slider("X", start.X, 0, sizeX - 1);
            start.Y = (int)EditorGUILayout.Slider("Y", start.Y, 0, sizeY - 1);
            // GUILayout.EndHorizontal();
            if (GUILayout.Button("Random Start", GUILayout.Width(130)))
            {
                start.X = Random.Range(0, sizeX);
                start.Y = Random.Range(0, sizeY);
                
            }
        }

        {
            // GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("End position:");
            end.X = (int)EditorGUILayout.Slider("X", end.X, 0, sizeX - 1);
            end.Y = (int)EditorGUILayout.Slider("Y", end.Y, 0, sizeY - 1);
            // GUILayout.EndHorizontal();
            if (GUILayout.Button("Random End", GUILayout.Width(130)))
            {
                end.X = Random.Range(0, sizeX);
                end.Y = Random.Range(0, sizeY);
            }
        }
        if (sizeX == 0 || sizeY == 0) return;
        a[oldStart.X, oldStart.Y] = defaultCell;
        a[oldEnd.X, oldEnd.Y] = defaultCell;

        a[start.X, start.Y] = "S";
        a[end.X, end.Y] = "T";

    }

    void DrawCellOption(){
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Change default cell:");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("X", cellStyle["X"], GUILayout.Width(cellWidth))){
            defaultCell = "X";
        }
        if (GUILayout.Button("O", cellStyle["O"],GUILayout.Width(cellWidth))){
            defaultCell = "O";
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Cell Options:");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("X", cellStyle["X"], GUILayout.Width(cellWidth))){
            cellOption = "X";
        }
        if (GUILayout.Button("O", cellStyle["O"],GUILayout.Width(cellWidth))){
            cellOption = "O";
        }
        if (GUILayout.Button("R", cellStyle["R"],GUILayout.Width(cellWidth))){
            cellOption = "R";
        }
        if (GUILayout.Button("A", cellStyle["A"],GUILayout.Width(cellWidth))){
            cellOption = "A";
        }
        if (GUILayout.Button("S", cellStyle["S"],GUILayout.Width(cellWidth))){
            cellOption = "S";
        }
        if (GUILayout.Button("T", cellStyle["T"],GUILayout.Width(cellWidth))){
            cellOption = "T";
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.TextField("Current cell option: ", cellOption);
    }

    void DrawSaveResetLevel(){
        EditorGUILayout.Space();
        save_file_name = EditorGUILayout.TextField("Save file name:", save_file_name);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Level", GUILayout.Width(90)) && save_file_name != ""){
            
            WriteLevelToFile();
            // AssetDatabase.ImportAsset(save_path);

            // TextAsset asset = (TextAsset)Resources.Load(save_file_name);
            // Debug.Log(asset.text);
        }

        if (GUILayout.Button("Reset", GUILayout.Width(90))){
            InitParams();
        }

        GUILayout.EndHorizontal();
    }

    void WriteLevelToFile(){
        LoadMap();
        string save_path = ROOT_SAVE + save_file_name + ".txt";
        Debug.Log(save_path);
        StreamWriter writer = new StreamWriter(save_path, false);
        writer.WriteLine("" + sizeX + " " + sizeY + " " + start.X + " " + start.Y + " " + end.X + " " + end.Y);
        for(int i = 0; i < sizeX; i++){
            for(int j = 0; j < sizeY; j++){
                writer.WriteLine("" + i + " " + j + " " + a[i, j]);
            }
        }
        writer.Close();
    }

    void DrawLevelPreviewSection(){
        GUILayout.BeginArea(levelPreviewSection);
        EditorGUILayout.LabelField("Preview Level");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(previewLevelScrollWidth), GUILayout.Height(previewLevelScrollHeight));
        DrawLevelArea();
        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();
    }
    void DrawLevelArea(){
        EditorGUILayout.Space();
        
        for(int i = 0; i < sizeX; i++){
            GUILayout.BeginHorizontal();
            for(int j = 0; j < sizeY; j++){
                if (GUILayout.Button(a[i,j], cellStyle[a[i,j]], GUILayout.Width(cellWidth))){
                    if (cellOption == "S"){
                        a[start.X, start.Y] = "X";
                        start.X = i;
                        start.Y = j;
                        a[i, j] = "S";
                    }
                    else if (cellOption == "T"){
                        a[end.X, end.Y] = "X";
                        end.X = i;
                        end.Y = j;
                        a[i, j] = "T";
                    }
                    else
                        a[i,j] = cellOption;
                }
               
            }
            GUILayout.EndHorizontal();
        }
    }

    void DebugTable(int [,] cost){
        for(int i = 0; i < sizeX; i++){
            GUILayout.BeginHorizontal();
            for(int j = 0; j < sizeY; j++){
                EditorGUILayout.LabelField(""+cost[i,j]);
            }
            GUILayout.EndHorizontal();
        }
    }

    void DrawRunPathFindingAI(){
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Solving Level:");
        if (GUILayout.Button("Solve level", GUILayout.Width(90)) && isOriginMapPreview){
            isSolveLevel = true;
            AddDataPathFinding();
            PathfindingAI.FindMiniumPath();
        }

        if (isSolveLevel){
            EditorGUILayout.IntField("Min path cost:", PathfindingAI.minPathValue);
            EditorGUILayout.IntField("Min path steps:", PathfindingAI.nMinPath - 1);
            EditorGUILayout.IntField("Attemtped Steps:", PathfindingAI.attemptStepsValue - 2);
            EditorGUILayout.FloatField("Level Difficult:", PathfindingAI.diffRating);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show attempted Steps", GUILayout.Width(150))){
                SaveMap();
                GetAttemptSteps();
            }
            if (GUILayout.Button("Show min Path", GUILayout.Width(130))){
                SaveMap();
                GetMinPath();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Origin map", GUILayout.Width(150))){
                LoadMap();
            }
            if (GUILayout.Button("Export Json", GUILayout.Width(130))){
                ExportToJson();
            }
            EditorGUILayout.EndHorizontal();

        }
    }

    void GetMinPath(){
        isOriginMapPreview = false;
        for(int i = 0; i < PathfindingAI.nMinPath; i++){
            var x = PathfindingAI.minPath[i].x;
            var y = PathfindingAI.minPath[i].y;
            a[x, y] = "*";
        }
    }

    void GetAttemptSteps(){
        isOriginMapPreview = false;
        for(int i = 1; i < PathfindingAI.attemptStepsValue - 1; i++){
            var x = PathfindingAI.attemptSteps[i].x;
            var y = PathfindingAI.attemptSteps[i].y;
            a[x, y] = "o";
        }
    }

    void SaveMap(){
        if (!isOriginMapPreview) return;
        originMap = new string[sizeX, sizeY];
        for(int i = 0; i < sizeX; i++){
            for(int j = 0; j < sizeY; j++){
                originMap[i,j] = a[i,j];
            }
        }
    }

    void LoadMap(){
        if (isOriginMapPreview) return;
        isOriginMapPreview = true;
        for(int i = 0; i < sizeX; i++){
            for(int j = 0; j < sizeY; j++){
                a[i,j] = originMap[i,j];
            }
        }
    }
    
    void AddDataPathFinding(){
        Dictionary<string, int>c = new Dictionary<string, int>(){
            {"X", -1},
            {"O", 1},
            {"R", 5},
            {"A", 3},
            {"S", 0},
            {"T", 0},
        };
        int [,] cost = new int [sizeX, sizeY];
        for(int i = 0; i < sizeX; i++){
            for(int j = 0; j < sizeY; j++){
                cost[i,j] = c[a[i,j]];
            }
        }
        Debug.Log("Path Finding: " + "n = " + sizeX + ", m = " + sizeY + ", start = (" + start.X + "," + start.Y + ") = " + cost[start.X, start.Y] +", end = ("  + end.X + "," + end.Y + ") = " + cost[end.X, end.Y]);
        PathfindingAI.addData(sizeX, sizeY, cost, start.X, start.Y, end.X, end.Y);
    }

    void ExportToJson(){
        levelEvalData = new LevelEvalData();
        levelEvalData.levelDataPath = ROOT_SAVE + selectLevelFile;
        levelEvalData.minPathValue = PathfindingAI.minPathValue;
        levelEvalData.attemptedSteps = PathfindingAI.attemptStepsValue;
        levelEvalData.levelDifficulty = PathfindingAI.diffRating;
        string json_data = JsonUtility.ToJson(levelEvalData);
        string levelEval_save_path = "Assets/Resources/LevelEval/";
        File.WriteAllText(levelEval_save_path + "eval_" + selectLevelFile, json_data);
    }

    void DrawLevelToScene(){
        EditorGUILayout.Space();
        if (GUILayout.Button("View on scene", GUILayout.Width(100))){
            LevelToScene.addData(sizeX, sizeY, a, moveable_asset, obs_asset);
            LevelToScene.SpawnLevelToScene();
        }
    }
}

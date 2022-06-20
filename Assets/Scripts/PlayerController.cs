using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public string [,] a;

    public LevelSolution levelSolution;

    public float speed;

    public int minPathIndex;
    
    // public LevelSolution levelSolution;
    void Start()
    {
        minPathIndex = levelSolution.nMinPath - 2;
    }

    // Update is called once per frame
    void Update()
    {
        // for(int i = levelSolution.nMinPath - 1; i >= 0; i--){
        if (minPathIndex >= 0){
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(levelSolution.x[minPathIndex], 1.5f, levelSolution.y[minPathIndex]), Time.deltaTime * speed);
            if (transform.position.x == levelSolution.x[minPathIndex] && transform.position.z == levelSolution.y[minPathIndex]){
                minPathIndex--;
            // Debug.Log(new Vector3(levelSolution.x[minPathIndex], 1.5f, levelSolution.y[minPathIndex]));
        }

        }
        // }
    }
}

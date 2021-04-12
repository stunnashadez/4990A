using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


/*
    This is the gui class that handles controls
    It allows the user to choose between algorithms to execute,
    change levels, visualize pathfinding process and see results
    from execution.

    It also allows user to randomize seeker and target position
*/
public class gui : MonoBehaviour
{
    public Rect windowRect = new Rect(20, 20, 430, 400);
    public static float visualdelay = 0.0F;                 //This controls the loop delay for visual pathfinding (i.e. it slows down the search)
    public static bool isVisualEnabled = false;             //This controls if visual pathfinding is enabled (i.e. whether you want to see the process or not)
    public static string results = "Result: ";              //This string displays the algorithm results on the gui
    public Transform seeker, target;                        //Reference to seeker GameObject and target GameObject
    public Transform Camera;                                //Reference to camera (this is needed to change camera perspective)
    public static bool isRunning = false;                   //Checks if algorithm is running, this is to avoid launching 2 pathfinding algo's at the same time
    public static int cameraPOS = 0;                        //Saves the camera's current position (preset, 1 = some coordinates, preset 2 = some other coords)
    public static int level = 0;                            //Saves the current level loaded

    AStar aStar;                                            //Object declaration for pathfinding algorithms
    public static Gridd grid;
    DFS dfs;
    BFS bfs;

    void Awake()
    {
        aStar = GetComponent<AStar>();
        dfs = GetComponent<DFS>();
        bfs = GetComponent<BFS>();
        grid = GetComponent<Gridd>();
    }

    void OnGUI()
    {
        windowRect = GUI.Window(0, windowRect, drawWindow, "");

    }

    void drawWindow(int windowID)
    {

        GUI.Label(new Rect(10, 10, 200, 35), "Run: ");

        if (GUI.Button(new Rect(10, 30, 100, 50), "A* Algorithm") && isRunning == false)
        {
            grid.CreateGrid();   //Refreshes grid in case user moved objects
            aStar.Run();
        }

        if (GUI.Button(new Rect(10, 90, 100, 50), "DFS Algorithm") && isRunning == false)
        {
            grid.CreateGrid();  //Refreshes grid in case user moved objects
            dfs.Run();
        }

        if (GUI.Button(new Rect(10, 150, 100, 50), "BFS Algorithm") && isRunning == false)
        {
            grid.CreateGrid();  //Refreshes grid in case user moved objects
            bfs.Run();
        }
        if (GUI.Button(new Rect(260, 345, 100, 50), "Change Level") && isRunning == false)
        {
            level++;
            switch (level)
            {
                case 1:
                    SceneManager.LoadScene(level);
                    break;
                default:
                    SceneManager.LoadScene(0);
                    level = 0;
                    break;

            }
        }
        GUI.Label(new Rect(200, 10, 200, 35), "Options: ");

        if (GUI.Button(new Rect(200, 30, 150, 50), "Change Camera"))
        {
            cameraPOS++;

            switch (cameraPOS)
            {
                case 1:
                    Camera.position = new Vector3(-34F, 144.18F, -27.8F);
                    Camera.rotation = Quaternion.Euler(79.30F, 5.3F, 5F);
                    break;
                case 2:
                    Camera.position = new Vector3(-54.51F, 34.2F, -74.38F);
                    Camera.rotation = Quaternion.Euler(16.801F, 39.013F, 0.17F);
                    break;
                default:
                    Camera.position = new Vector3(-19.6F, 99.1F, -84.4F);
                    Camera.rotation = Quaternion.Euler(51.73F, 1.58F, 1.01F);
                    cameraPOS = 0;
                    break;

            }
        }

        if (GUI.Button(new Rect(200, 90, 150, 50), "Randomize Seeker") && isRunning == false)
        {
            grid.CreateGrid();  //Refreshes grid in case user moved objects
            grid.path = new List<Node>(); //Reset displayed grid
            while (true)
            {
                Vector3 randomPos = new Vector3(Random.Range(-50.0f, 50.0f), 0, Random.Range(-50.0f, 50.0f));
                Node seekerNode = grid.NodeFromWorldPoint(randomPos);
                if (seekerNode.walkable)
                {
                    seeker.position = randomPos;
                    break;
                }
            }
        }

        if (GUI.Button(new Rect(200, 150, 150, 50), "Randomize Target") && isRunning == false)
        {
            grid.CreateGrid();  //Refreshes grid in case user moved objects
            grid.path = new List<Node>(); //Reset displayed grid
            while (true)
            {
                Vector3 randomPos = new Vector3(Random.Range(-50.0f, 50.0f), 0, Random.Range(-50.0f, 50.0f));
                Node targetNode = grid.NodeFromWorldPoint(randomPos);
                if (targetNode.walkable)
                {
                    target.position = randomPos;
                    break;
                }
            }
        }

        GUI.Label(new Rect(10, 245, 200, 50), "Visualization Speed: ");
        visualdelay = GUI.HorizontalSlider(new Rect(10, 270, 200, 50), visualdelay, 0.05F, 0.00F);
        isVisualEnabled = GUI.Toggle(new Rect(10, 220, 200, 50), isVisualEnabled, "View Pathfinding Process");


        GUI.Label(new Rect(10, 345, 300, 50), results);
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SubsystemsImplementation;


public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject clearText;
    public GameObject ParticlePrefab;
    public GameObject RengaPrefab;

    public GameObject title;
    public GameObject PressSpace;

    public GameObject StageSelect;

    public GameObject ResetText;

    public GameObject howtoplay;

    public GameObject Easy;
    public GameObject Normal;
    public GameObject Hard;

    public GameObject tab1;
    public GameObject tab2;
    public GameObject tab3;

    int goalNum = 0;
    GameObject[] goal;

    int moveTimer = 0;

    int particleMax = 4;
    GameObject[] particle;

    enum Scene
    {
        Title,
        Stage,
        Game,
    }

    Scene currentScene;

    int currentStage;

    int[,] map;             //レベルデザイン用の配列
    GameObject[,] field;    //ゲーム管理用の配列
    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == null) { continue; }
                if (field[y, x].tag == "Player")
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
    bool MovePlayer(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        /*パーティクルの生成*/
        for (int i = 0; i < particleMax; ++i)
        {
            particle[i] = Instantiate(
            ParticlePrefab,
            field[GetPlayerIndex().y, GetPlayerIndex().x].transform.position,
            Quaternion.identity
            );
        }

        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }

        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Renga")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MovePlayer(tag, moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MovePlayer(tag, moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        Vector3 moveToPosition = new Vector3(
            moveTo.x, field.GetLength(0) - moveTo.y, 0
            );
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);

        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;
        return true;
    }
    bool isCleard()
    {
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                return false;
            }
        }
        return true;
    }

    void Initialize(Scene scene, int currentStage)
    {
        switch (scene)
        {
            case Scene.Title:
                title.SetActive(true);
                PressSpace.SetActive(true);

                break;

            case Scene.Stage:
                StageSelect.SetActive(true);
                PressSpace.SetActive(true);
                Normal.SetActive(true);
                Easy.SetActive(true);
                Hard.SetActive(true);

                break;

            case Scene.Game:
                playerPrefab.SetActive(true);
                /*ステージごとにマップデータ取得*/
                switch (currentStage)
                {
                    case 0:
                        map = new int[,] {
                            {4, 4, 4, 4, 4, 4, 4},
                            {4, 0, 2, 0, 3, 0, 4},
                            {4, 3, 0, 0, 2, 0, 4},
                            {4, 4, 4, 0, 0, 0, 4},
                            {4, 0, 0, 0, 2, 0, 4},
                            {4, 0, 3, 0, 0, 1, 4},
                            {4, 4, 4, 4, 4, 4, 4}
                    };
                        break;

                    case 1:
                        map = new int[,] {
                            { 4, 4, 4, 4, 4, 4, 4},
                            { 4, 3, 4, 0, 4, 0, 4},
                            { 4, 0, 4, 0, 4, 3, 4},
                            { 4, 0, 4, 0, 2, 0, 4},
                            { 4, 0, 2, 2, 0, 0, 4},
                            { 4, 3, 0, 0, 0, 1, 4},
                            { 4, 4, 4, 4, 4, 4, 4}
                    };
                        break;

                    case 2:
                        map = new int[,] {
                            {0, 4, 4, 4, 4, 4, 4},
                            {4, 4, 3, 4, 0, 0, 4},
                            {4, 3, 0, 0, 2, 3, 4},
                            {4, 4, 2, 2, 2, 0, 4},
                            {4, 0, 0, 0, 2, 0, 4},
                            {4, 4, 3, 4, 3, 1, 4},
                            {0, 4, 4, 4, 4, 4, 4}
                      };
                        break;

                }// Stage

                field = new GameObject[map.GetLength(0), map.GetLength(1)];

                // ゴールの数を取得
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    for (int x = 0; x < map.GetLength(1); x++)
                    {
                        if (map[y, x] == 1)
                        {
                            field[y, x] = Instantiate(
                                playerPrefab,
                                new Vector3(x, map.GetLength(0) - y, 0),
                                Quaternion.identity
                                );
                        }
                        if (map[y, x] == 2)
                        {
                            field[y, x] = Instantiate(
                                boxPrefab,
                                new Vector3(x, map.GetLength(0) - y, 0),
                                Quaternion.identity
                          );

                        }
                        if ((map[y, x] == 3))
                        {
                            goalNum++;
                        }
                        if (map[y, x] == 4)
                        {
                            field[y, x] = Instantiate(
                               RengaPrefab,
                                new Vector3(x, map.GetLength(0) - y, 0),
                                Quaternion.identity
                                );
                        }

                    }
                }

                //　ゴールの生成
                goal = new GameObject[goalNum];
                int goalIndex = goalNum;
                //　ゴール座標の決定
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    for (int x = 0; x < map.GetLength(1); x++)
                    {
                        if (map[y, x] == 3)
                        {
                            goalIndex--;
                            if (goalIndex < 0)
                            {
                                break;
                            }
                            goal[goalIndex] = Instantiate(
                               goalPrefab,
                                new Vector3(x, map.GetLength(0) - y, 0),
                                Quaternion.identity
                                );
                        }

                    }
                }

                break; // Game
        }   // Scene
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        particle = new GameObject[particleMax];
        currentScene = Scene.Title;
        Initialize(currentScene, currentStage);
    }


    // Update is called once per frame
    void Update()
    {
        switch (currentScene)
        {
            case Scene.Title:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentScene = Scene.Stage;
                    currentStage = 0;
                    title.SetActive(false);
                    Initialize(currentScene, currentStage);
                }
                break;

            case Scene.Stage:
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    currentStage++;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentStage--;
                }
                if (currentStage < 0)
                {
                    currentStage = 0;
                }
                if (currentStage > 2)
                {
                    currentStage = 2;
                }
                switch (currentStage)
                {
                    case 0:

                        tab1.SetActive(true);
                        tab2.SetActive(false);
                        tab3.SetActive(false);
                        break;

                    case 1:
                        tab1.SetActive(false);
                        tab2.SetActive(true);
                        tab3.SetActive(false);

                        break;

                    case 2:
                        tab1.SetActive(false);
                        tab2.SetActive(false);
                        tab3.SetActive(true);
                        break;
                }


                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentScene = Scene.Game;
                    StageSelect.SetActive(false);
                    PressSpace.SetActive(false);
                    howtoplay.SetActive(true);
                    ResetText.SetActive(true);
                    Normal.SetActive(false);
                    Easy.SetActive(false);
                    Hard.SetActive(false);
                    tab1.SetActive(false);
                    tab2.SetActive(false);
                    tab3.SetActive(false);

                    Initialize(currentScene, currentStage);
                }
                break;

            case Scene.Game:
                if (!isCleard())
                {
                    if (moveTimer > 0)
                    {
                        moveTimer--;
                    }
                    if (moveTimer == 0)
                    {
                        if (Input.GetKeyDown(KeyCode.R))
                        {
                            for (int y = 0; y < map.GetLength(0); y++)
                            {
                                for (int x = 0; x < map.GetLength(1); x++)
                                {
                                    Destroy(field[y, x]);
                                }
                            }
                            for (int i = 0; i < goalNum; i++)
                            {
                                Destroy(goal[i]);
                            }
                            goalNum = 0;
                            Initialize(currentScene, currentStage);
                        }
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            //移動処理
                            Vector2Int playerIndex = GetPlayerIndex();
                            Vector2Int velosity = new Vector2Int(1, 0);

                            MovePlayer(tag, playerIndex, playerIndex + velosity);
                            moveTimer = 180;
                            if (isCleard())
                            {
                                //ゲームオブジェクトのSetActiveメソッドを使い有効化
                                clearText.SetActive(true);
                                PressSpace.SetActive(true);
                                howtoplay.SetActive(false);
                                ResetText.SetActive(false);

                            }
                        }
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            //移動処理
                            Vector2Int playerIndex = GetPlayerIndex();
                            Vector2Int velosity = new Vector2Int(-1, 0);

                            MovePlayer(tag, playerIndex, playerIndex + velosity);
                            moveTimer = 180;
                            if (isCleard())
                            {
                                //ゲームオブジェクトのSetActiveメソッドを使い有効化
                                clearText.SetActive(true);
                                PressSpace.SetActive(true);
                                howtoplay.SetActive(false);
                                ResetText.SetActive(false);

                            }
                        }
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            //移動処理
                            Vector2Int playerIndex = GetPlayerIndex();
                            Vector2Int velosity = new Vector2Int(0, -1);

                            MovePlayer(tag, playerIndex, playerIndex + velosity);
                            moveTimer = 180;
                            if (isCleard())
                            {
                                //ゲームオブジェクトのSetActiveメソッドを使い有効化
                                clearText.SetActive(true);
                                PressSpace.SetActive(true);
                                howtoplay.SetActive(false);
                                ResetText.SetActive(false);

                            }
                        }
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            //移動処理
                            Vector2Int playerIndex = GetPlayerIndex();
                            Vector2Int velosity = new Vector2Int(0, 1);

                            MovePlayer(tag, playerIndex, playerIndex + velosity);
                            moveTimer = 180;
                            if (isCleard())
                            {
                                //ゲームオブジェクトのSetActiveメソッドを使い有効化
                                clearText.SetActive(true);
                                PressSpace.SetActive(true);
                                howtoplay.SetActive(false);
                                ResetText.SetActive(false);

                            }
                        }

                    }

                }
                else /*クリア後の処理*/
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        for (int y = 0; y < map.GetLength(0); y++)
                        {
                            for (int x = 0; x < map.GetLength(1); x++)
                            {
                                Destroy(field[y, x]);
                            }
                        }
                        for (int i = 0; i < goalNum; i++)
                        {
                            Destroy(goal[i]);
                        }
                        goalNum = 0;
                        currentScene = Scene.Title;
                        clearText.SetActive(false);
                        PressSpace.SetActive(false);
                        Initialize(currentScene, currentStage);
                    }
                }
                break;// Game



        }//Switch

    }
}
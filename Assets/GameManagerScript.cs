using System.Collections.Generic;

using UnityEngine;


public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject clearText;
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
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }

        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MovePlayer(tag, moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        //field[moveFrom.y, moveFrom.x].transform.position =
        //    new Vector3(moveTo.x, field.GetLength(0) - moveTo.y, 0);
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
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);

        map = new int[,] {
            { 0, 0, 3, 0, 0 },
            { 0, 0, 2, 0, 0 },
            { 3, 2, 1, 2, 3 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };
        field = new GameObject
            [
            map.GetLength(0),
            map.GetLength(1)
            ];
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
                if (map[y, x] == 3)
                {
                    field[y, x] = Instantiate(
                        goalPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0),
                        Quaternion.identity
                  );

                }

            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!isCleard())
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //移動処理
                Vector2Int playerIndex = GetPlayerIndex();
                Vector2Int velosity = new Vector2Int(1, 0);
                MovePlayer(tag, playerIndex, playerIndex + velosity);
                if (isCleard())
                {
                    //ゲームオブジェクトのSetActiveメソッドを使い有効化
                    clearText.SetActive(true);
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //移動処理
                Vector2Int playerIndex = GetPlayerIndex();
                Vector2Int velosity = new Vector2Int(-1, 0);
                MovePlayer(tag, playerIndex, playerIndex + velosity);
                if (isCleard())
                {
                    //ゲームオブジェクトのSetActiveメソッドを使い有効化
                    clearText.SetActive(true);
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //移動処理
                Vector2Int playerIndex = GetPlayerIndex();
                Vector2Int velosity = new Vector2Int(0, -1);
                MovePlayer(tag, playerIndex, playerIndex + velosity);
                if (isCleard())
                {
                    //ゲームオブジェクトのSetActiveメソッドを使い有効化
                    clearText.SetActive(true);
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //移動処理
                Vector2Int playerIndex = GetPlayerIndex();
                Vector2Int velosity = new Vector2Int(0, 1);
                MovePlayer(tag, playerIndex, playerIndex + velosity);
                if (isCleard())
                {
                    //ゲームオブジェクトのSetActiveメソッドを使い有効化
                    clearText.SetActive(true);
                }
            }

        }

    }
}
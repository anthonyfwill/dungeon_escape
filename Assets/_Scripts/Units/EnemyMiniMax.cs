using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyMiniMax : MonoBehaviour
{
    private static List<int> hero_Num;
    //function creates a jagged array based on the _tiles variable that holds the tile type and location
    private static int[][] VectorMap(Dictionary<Vector2, Tile> map)
    {
        var mapWidth = 16;
        var mapHeight = 9;
        if (map == null) { return null; }
        int[][] state = new int[16][];
        for (int i = 0; i < mapWidth; i++)
        {
            state[i] = new int[mapHeight];
        }
        foreach (var kvp in map)
        {
            Vector2 position = kvp.Key;
            int value = -1;
            if (kvp.Value.TileName == "Grasslands")
                value = 0;
            else
            {
                value = -1;
            }
            int x = (int)position.x;
            int y = (int)position.y;

            // Check if the position is within the bounds of the jagged array
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            {
                state[x][y] = value;
            }
        }


        return state;


    }
    //for testing purposes, prints out the jagged array
    public static void PrintArr(int[][] arr)
    {
        for (int i = 0; i < 16; i++)
        {
            string row = "";
            for (int j = 0; j < 9; j++)
            {
                row += arr[i][j] + "\t";
            }
            print(row);
        }
    }
    //heuristic funtion that uses manhatten distance to find the best move
    private static float heuristic(int[][] arr)
    {
        int closesthero = -1;
        int man = 1000;
        var delimiter = 0;
        for (int i = 0; i < hero_Num.Count; i++)
        {
            
            var heroLoc1 = findPlayer(hero_Num[i], arr);
            var enemyLoc1 = findPlayer(4, arr);
           
            if (enemyLoc1 == null && heroLoc1 == null)
            {
                return 10;
            }
            else if (heroLoc1 == null)
            {
                heroLoc1 = enemyLoc1;
                delimiter = 20;
            }
            else if (enemyLoc1 == null)
            {
                enemyLoc1 = heroLoc1;
                delimiter = -5;
            }
            
            var manhatten1 = Math.Abs(heroLoc1[0] - enemyLoc1[0]) + Math.Abs(heroLoc1[1] - enemyLoc1[1]);
            if(manhatten1 < man)
            {
                closesthero = hero_Num[i];
                man = manhatten1;
            }
        }
       
        return 50 - man +delimiter;


    }
    //clones the jagged array
    private static int[][] Cloner(int[][] arr)
    {
        int[][] clonedArr = new int[arr.Length][];
        for (int i = 0; i < arr.Length; i++)
        {
            clonedArr[i] = new int[arr[i].Length];
            for (int j = 0; j < arr[i].Length; j++)
            {
                clonedArr[i][j] = arr[i][j];
            }
        }
        return clonedArr;
    }
    //makes the move in the jagged array, returns the new jagged array with the implemented move
    private static int[][] setMove(int[][] state, int hero, int direction, int x, int y)
    {

        var clonedArray = Cloner(state);
        if (direction == 1)
        {
            clonedArray[x][y] = 0;
            clonedArray[x + 1][y] = hero;

        }
        else if (direction == 2)
        {
            clonedArray[x][y] = 0;
            clonedArray[x - 1][y] = hero;

        }
        else if (direction == 3)
        {
            clonedArray[x][y] = 0;
            clonedArray[x][y + 1] = hero;

        }
        else if (direction == 4)
        {
            clonedArray[x][y] = 0;
            clonedArray[x][y - 1] = hero;

        }
        return clonedArray;
    }
    //find all possible moves and returns a list of directions and states
    public static List<Tuple<int, int[][]>> Moves(int[][] state, int hero, int x, int y)
    {
        int[] directions = { 1, 2, 3, 4 };
        List<int> dir = new List<int>();
        List<Tuple<int, int[][]>> allowedeDir = new List<Tuple<int, int[][]>>();
        if (hero == 1 || hero == 2 || hero == 3 )
        {
            foreach (int direction in directions)
            {

                if (direction == 1)
                {
                    if (x + 1 >= 0 && x + 1 < 16 && y >= 0 && y < 9)
                    {
                        if (state[x + 1][y] == 0 || state[x + 1][y] == 4)
                        {
                            var temparry = setMove(state, hero, direction, x, y);
                            allowedeDir.Add(Tuple.Create(direction, temparry));
                            dir.Add(direction);
                        }
                    }

                }
                else if (direction == 2)
                {
                    if (x - 1 >= 0 && x - 1 < 16 && y >= 0 && y < 9)
                    {
                        if (state[x - 1][y] == 0 || state[x - 1][y] == 4)
                        {
                            var temparry = setMove(state, hero, direction, x, y);
                            allowedeDir.Add(Tuple.Create(direction, temparry));
                            dir.Add(direction);
                        }
                    }

                }
                else if (direction == 3)
                {
                    if (y + 1 >= 0 && y + 1 < 9 && x >= 0 && x < 16)
                    {
                        if (state[x][y + 1] == 0 || state[x][y + 1] == 4)
                        {
                            var temparry = setMove(state, hero, direction, x, y);
                            allowedeDir.Add(Tuple.Create(direction, temparry));
                            dir.Add(direction);
                        }
                    }

                }
                else if (direction == 4)
                {
                    if (y - 1 >= 0 && y - 1 < 9 && x >= 0 && x < 16)
                    {
                        if (state[x][y - 1] == 0 || state[x][y - 1] == 4)
                        {
                            var temparry = setMove(state, hero, direction, x, y);
                            allowedeDir.Add(Tuple.Create(direction, temparry));
                            dir.Add(direction);
                        }
                    }

                }

            }
        }
        else
        {
            foreach (int direction in directions)
            {

                if (direction == 1)
                {
                    if (x + 1 >= 0 && x + 1 < 16 && y >= 0 && y < 9)
                    {
                        if (state[x + 1][y] == 0 || state[x + 1][y] == 1 || state[x + 1][y] == 2 || state[x + 1][y] == 3)
                        {
                            var temparry = setMove(state, hero, direction, x, y);
                            allowedeDir.Add(Tuple.Create(direction, temparry));
                            dir.Add(direction);
                        }
                    }

                }
                else if (direction == 2)
                {
                    if (x - 1 >= 0 && x - 1 < 16 && y >= 0 && y < 9)
                    {
                        if (state[x - 1][y] == 0 || state[x - 1][y] == 1 || state[x - 1][y] == 2 || state[x - 1][y] == 3)
                        {
                            var temparry = setMove(state, hero, direction, x, y);
                            allowedeDir.Add(Tuple.Create(direction, temparry));
                            dir.Add(direction);
                        }
                    }

                }
                else if (direction == 3)
                {
                    if (y + 1 >= 0 && y + 1 < 9 && x >= 0 && x < 16)
                    {
                        if (state[x][y + 1] == 0 || state[x][y + 1] == 1 || state[x][y + 1] == 2 || state[x][y + 1] == 3)
                        {
                            var temparry = setMove(state, hero, direction, x, y);
                            allowedeDir.Add(Tuple.Create(direction, temparry));
                            dir.Add(direction);
                        }
                    }

                }
                else if (direction == 4)
                {
                    if (y - 1 >= 0 && y - 1 < 9 && x >= 0 && x < 16)
                    {
                        if (state[x][y - 1] == 0 || state[x][y - 1] == 1 || state[x][y - 1] == 2 || state[x][y - 1] == 3)
                        {
                            var temparry = setMove(state, hero, direction, x, y);
                            allowedeDir.Add(Tuple.Create(direction, temparry));
                            dir.Add(direction);
                        }
                    }
                }

            }
        }
        string temp = "";
        foreach (var d in allowedeDir)
        {
            temp = temp + " " + d;
            //PrintArr(state);
        }
        //print($"Allowed Directions: {temp}");
        return allowedeDir;
    }
    //the minimizer for the minimax function
    private static float minVal(int[][] state, int depth, float alpha, float beta, bool agent)
    {
        var hero = findPlayer(hero_Num[0], state);
        var v = float.PositiveInfinity;
        foreach (var move in Moves(state, hero_Num[0], hero[0], hero[1]))
        {
            v = Math.Min(v, minimax(move.Item2, depth - 1, alpha, beta, agent));
            if (v <= alpha)
            {
                return v;
            }
            beta = Math.Min(beta, v);
        }

        return v;
    }
    //the maximizer for the minimax function
    private static float maxVal(int[][] state, int depth, float alpha, float beta, bool agent)
    {
        var enemy = findPlayer(4, state);
        var v = float.NegativeInfinity;
        foreach (var move in Moves(state, 4, enemy[0], enemy[1]))
        {
            v = Math.Max(v, minimax(move.Item2, depth - 1, alpha, beta, agent));
            if (v >= beta)
            {
                //print($"negative infi {v}");
                return v;
            }
            alpha = Math.Max(alpha, v);
        }

        return v;
    }
    //minimax funtion that has the heuristics
    private static float minimax(int[][] state, int depth, float alpha, float beta, bool agent)
    {
        //PrintArr(state);
        //print("----------------------------------------------");

        if (depth == 0 || findPlayer(4, state) == null || findPlayer(hero_Num[0], state) == null)
            return heuristic(state);
        if (agent == true)
        {
            return maxVal(state, depth, alpha, beta, false);
        }
        else
        {
            return minVal(state, depth, alpha, beta, true);
        }
    }
    //finds the location of the player in the jagged array and returns location
    private static int[] findPlayer(int target, int[][] array)
    {

        int[] temp = new int[2];
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array[i].Length; j++)
            {
                if (array[i][j] == target)
                {
                    temp[0] = i;
                    temp[1] = j;
                    return temp;// Value found
                }
            }
        }
        return null; // Value not found
    }

    //main player move function
    public static void PlayerMove()
    {
        //if hero is dead or escaped game is won
        if (UnitManager.Instance.EscapeCount == 3 || UnitManager.Instance.DeadHeroes == 3)
        {
            
            GameManager.Instance.ChangeState(GameState.WonGame);
            return;
        }//if its not the enemy turn, leave
        if (GameManager.Instance.GameState != GameState.EnemiesTurn) return;
        Dictionary<Vector2, Tile> map = GridManager.Instance.GetGrid();
        int[][] arr = VectorMap(map);
        //create a new jagged array
        //find the locations of the heroes
        BaseUnit currentenemy = UnitManager.Instance.SelectedEnemy;
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero_Location = (UnitManager.Instance.SelectedHeroes[0] != null) ? (UnitManager.Instance.SelectedHeroes[0].transform.position) : (dummy_location);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);
        Vector3 enemy_Location = (UnitManager.Instance.SelectedEnemy != null) ? (UnitManager.Instance.SelectedEnemy.transform.position) : (dummy_location);
        BaseEnemy currentE = (currentenemy != null) ? ((BaseEnemy)currentenemy) : (null);
        //if the enemy is dead go to hero turn
        if (currentenemy == null)
        {
            
            GameManager.Instance.ChangeState(GameState.HeroesTurn);
            return;

        }
        //as long as the heroes exist then mark thier locations in the jagged array
        List<int> heroes = new List<int>();
        if (hero_Location != dummy_location)
        {
            arr[(int)hero_Location.x][(int)hero_Location.y] = 1;
            heroes.Add(1);
        }
        if (hero2_Location != dummy_location)
        {
            arr[(int)hero2_Location.x][(int)hero2_Location.y] = 2;
            heroes.Add(2);  
        }
        if (hero3_Location != dummy_location)
        {
            arr[(int)hero3_Location.x][(int)hero3_Location.y] = 3;
            heroes.Add(3);
        }
        if (enemy_Location != dummy_location)
        {
            arr[(int)enemy_Location.x][(int)enemy_Location.y] = 4;
        }
        var testxy = currentenemy.transform.position;
        var enemynumber = arr[(int)testxy.x][(int)testxy.y];
        
        hero_Num = heroes;
            //get all possible moves of hero and run minimax
            List<Tuple<int, int[][]>> list = Moves(arr, enemynumber, (int)testxy.x, (int)testxy.y);
            if (list.Count == 0) {  GameManager.Instance.ChangeState(GameState.HeroesTurn); return; }
            var bestMove = list[0].Item1;
            var depth = 5;
            var alpha = float.NegativeInfinity;
            var beta = float.PositiveInfinity;
            bool agent = false;
            
            foreach (var move in list)
            {
                //take the best move
                var rs = minimax(move.Item2, depth, alpha, beta, agent);
                
                if (rs > alpha)
                {

                    alpha = rs;
                    bestMove = move.Item1;
                    
                }

            }

        //if all three heroes are alive then run nash
            if (bestMove == 1) 
            {
                currentE.transform.position += new Vector3(1f, 0f, 0f);
            }
            else if (bestMove == 2)
            {
                currentE.transform.position += new Vector3(-1f, 0f, 0f);
            }
            else if (bestMove == 3)
            {
                currentE.transform.position += new Vector3(0f, 1f, 0f);
            }
            else if (bestMove == 4)
            {
                currentE.transform.position += new Vector3(0f, -1f, 0f);
            }
            




        //if the enemy kills incease the dead heroes counter and print ti
        if (UnitManager.Instance.SelectedEnemy != null)
        {
            if (UnitManager.Instance.SelectedEnemy.transform.position == hero_Location)
            {
                print("Enemy kills");
                Destroy(UnitManager.Instance.SelectedHeroes[0].gameObject);
                UnitManager.Instance.DeadHeroes += 1;
                print(UnitManager.Instance.SelectedHeroes[0]);
            }
            else if (UnitManager.Instance.SelectedEnemy.transform.position == hero2_Location)
            {
                print("Enemy kills");
                Destroy(UnitManager.Instance.SelectedHeroes[1].gameObject);
                UnitManager.Instance.DeadHeroes += 1;
                print(UnitManager.Instance.SelectedHeroes[1]);
            }
            else if (UnitManager.Instance.SelectedEnemy.transform.position == hero3_Location)
            {
                print("Enemy kills");
                Destroy(UnitManager.Instance.SelectedHeroes[2].gameObject);
                UnitManager.Instance.DeadHeroes += 1;
                print(UnitManager.Instance.SelectedHeroes[2]);
            }
        }

        
        //swtich to heroes turn

        GameManager.Instance.ChangeState(GameState.HeroesTurn);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Nash : MonoBehaviour
{
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
        string row = "";
        for (int i = 0; i < 16; i++)
        {
            
            for (int j = 0; j < 9; j++)
            {
                row += arr[i][j] + "\t";
            }
            row += "\n";
        }
        print(row);
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
    private static int hero_Num;
    //heuristic funtion that uses manhatten distance to find the best move
    private static float heuristic(int hero, int[][] arr)
    {
        var heroLoc = findPlayer(hero, arr);
        var enemyLoc = findPlayer(4, arr);
        var delimiter = 0;
        if (heroLoc == null)
        {
            heroLoc = enemyLoc;
            delimiter = -5;
        }
        else if (enemyLoc == null)
        {
            enemyLoc = heroLoc;
            delimiter = 20;
        }

        var manhatten = Math.Abs(heroLoc[0] - enemyLoc[0]) + Math.Abs(heroLoc[1] - enemyLoc[1]);
        return 50 - manhatten + delimiter;


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
    //find all possible moves and returns a list of directions and states
    public static List<Tuple<int, int[][]>> Moves(int[][] state, int hero, int x, int y)
    {
        int[] directions = { 1, 2, 3, 4 };
        List<int> dir = new List<int>();
        List<Tuple<int, int[][]>> allowedeDir = new List<Tuple<int, int[][]>>();
        if (hero == 1 || hero == 2 || hero == 3)
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
            temp = temp + " " + d.Item1;

           // PrintArr(d.Item2);
        }
       // print($"Allowed Directions{hero}: {temp}");

        return allowedeDir;
    }
    //the maximizer for the minimax function
    private static float maxVal(int[][] state, int depth, float alpha, float beta, bool agent, int hero)
    {
        var hero_pos = findPlayer(hero, state);
        var v = float.NegativeInfinity;
        foreach (var move in Moves(state, hero, hero_pos[0], hero_pos[1]))
        {
            v = Math.Max(v, minimax(move.Item2, depth - 1, alpha, beta, agent, hero));

            if (v >= beta)
            {
                //print($"negative infi {v}");
                return v;
            }
            alpha = Math.Max(alpha, v);
        }
        
        return v;
    }
    //the minimizer for the minimax function
    private static float minVal(int[][] state, int depth, float alpha, float beta, bool agent, int hero)
    {
        var enemy = findPlayer(4, state);
        var v = float.PositiveInfinity;
        
        foreach (var move in Moves(state, 4, enemy[0], enemy[1]))
        {
            v = Math.Min(v, minimax(move.Item2, depth - 1, alpha, beta, agent, hero));
            
            if (v <= alpha)
            {
                
                return v;
            }
            beta = Math.Min(beta, v);
            
        }

        return v;
    }
    //minimax funtion that has the heuristics
    private static float minimax(int[][] state, int depth, float alpha, float beta, bool agent, int hero)
    {
        //PrintArr(state);
        //print("----------------------------------------------");
        if (depth == 0 || findPlayer(4, state) == null || findPlayer(hero, state) == null)
            return heuristic(hero,state);
        
        if (agent)
        {
           
            return maxVal(state, depth, alpha, beta, false, hero);
        }
        else
        {
            
            return minVal(state, depth, alpha, beta, true, hero);
        }
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
    private static Tuple<List<Tuple<int, Tuple<int, int>[][]>>,List<Tuple<float, float, float>[][]>> NashTable3Player(Vector3 hero1, Vector3 hero2, Vector3 hero3, int[][] arr)
    {
        //gets the locations of the heroes
        var h1Name = arr[(int)hero1.x][(int)hero1.y];
        var h2Name = arr[(int)hero2.x][(int)hero2.y];
        var h3Name = arr[(int)hero3.x][(int)hero3.y];
        int[] names = { h1Name, h2Name, h3Name };
        //gets all possible moves of h1
        var h1Moves = Moves(arr, h1Name, (int)hero1.x, (int)hero1.y);
        //creates objects to store the player directiosn and player moves
        List<Tuple<int,Tuple<int,int>[][]>> playerdirections = new List<Tuple<int, Tuple<int, int>[][]>>();
        List<Tuple<float, float, float>[][]> playermoves = new List<Tuple<float, float, float>[][]>();
        //for each possible move for player 1
        for (int i = 0; i < h1Moves.Count; i++)
        {
            //find the best move for player 2
            var h2Moves = Moves(h1Moves[i].Item2, h2Name, (int)hero2.x, (int)hero2.y);
           
            Tuple<float, float, float>[][] temp1 = new Tuple<float, float, float>[h2Moves.Count][];
            Tuple<int, int>[][] temp2 = new Tuple<int, int>[h2Moves.Count][];
            
            for (int j = 0; j < h2Moves.Count; j++)
            {
                
                //find the best possible moves for  player 3
                var h3Moves = Moves(h2Moves[j].Item2, h3Name, (int)hero3.x, (int)hero3.y);
                
                temp1[j] = new Tuple<float, float, float>[h3Moves.Count];
                temp2[j] = new Tuple<int, int>[h3Moves.Count];
                for (int k = 0; k < h3Moves.Count; k++)
                {
                    var hList = new List<float>();
                    //use minimax to find the best move to make based on the moves of the previous heroes.
                    for(int counter = 0; counter < 3; counter++)
                    {
                        var depth = 3;
                        var alpha = float.NegativeInfinity;
                        var beta = float.PositiveInfinity;
                        bool agent = false;
                        var res = minimax(h3Moves[k].Item2, depth, alpha, beta, agent, names[counter]);
                        
                        hList.Add(res);

                    }
                    temp1[j][k] = Tuple.Create(hList[0], hList[1], hList[2]);
                    temp2[j][k] = Tuple.Create(h2Moves[j].Item1, h3Moves[k].Item1);
                }
            }
            playermoves.Add(temp1);
            playerdirections.Add(Tuple.Create(h1Moves[i].Item1, temp2));
        }
        return Tuple.Create(playerdirections,playermoves);
    }

    public static int NashEQ()
    {
        //find the old locations of the heroes
        Vector3 hero1 = UnitManager.Instance.hero1Pos;
        Vector3 hero2 = UnitManager.Instance.hero2Pos;
        Vector3 hero3 = UnitManager.Instance.hero3Pos;
        Vector3 enemy = UnitManager.Instance.enemyPos;
        Dictionary<Vector2, Tile> map = GridManager.Instance.GetGrid();
        int[][] arr = VectorMap(map);
        //create the jagged arrau and find the current locations of the agents
        BaseUnit currentHero = UnitManager.Instance.SelectedHeroes[GameManager.heroTurn - 1];
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero_Location = (UnitManager.Instance.SelectedHeroes[0] != null) ? (UnitManager.Instance.SelectedHeroes[0].transform.position) : (dummy_location);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);
        Vector3 enemy_Location = (UnitManager.Instance.SelectedEnemy != null) ? (UnitManager.Instance.SelectedEnemy.transform.position) : (dummy_location);
        //add them to the jegged array
        if (hero1!= dummy_location)
        {
            arr[(int)hero1.x][(int)hero1.y] = 1;
        }
        if (hero2 != dummy_location)
        {
            arr[(int)hero2.x][(int)hero2.y] = 2;
        }
        if (hero3 != dummy_location)
        {
            arr[(int)hero3.x][(int)hero3.y] = 3;
        }
        if (enemy != dummy_location)
        {
            arr[(int)enemy.x][(int)enemy.y] = 4;
        }
        var testxy = currentHero.transform.position;
        var heronumber = arr[(int)testxy.x][(int)testxy.y];
        //run nash to find the table of possible moves and nash values
        var nashTable = NashTable3Player(hero1, hero2, hero3, arr);
        Vector2Int h1 = new Vector2Int();
        Vector2Int h2 = new Vector2Int();
        Vector2Int h3 = new Vector2Int();
        Vector2Int e1 = new Vector2Int();
        //find the change is directions
        if (hero_Location != dummy_location)
        {
            var newpos = hero_Location - hero1;
            h1 = new Vector2Int((int)newpos.x,(int)newpos.y);
            UnitManager.Instance.hero1Pos = hero_Location;
        }
        if (hero2_Location != dummy_location)
        {
            var newpos = hero2_Location - hero2;
            h2 = new Vector2Int((int)newpos.x, (int)newpos.y);
            UnitManager.Instance.hero2Pos = hero2_Location;
        }
        if (hero3_Location != dummy_location)
        {
            var newpos = hero3_Location - hero3;
            h3 = new Vector2Int((int)newpos.x, (int)newpos.y);
            
        }
        if (enemy_Location != dummy_location)
        {
            var newpos = enemy_Location - enemy;
            e1 = new Vector2Int((int)newpos.x, (int)newpos.y);
            UnitManager.Instance.enemyPos = enemy_Location;
        }
        Vector2Int[] hlist = { h1, h2};
        int[] hd = new int[2];
        for(int i = 0; i< 2; i++)
        {
            if (hlist[i].x == 0 && hlist[i].y == 1)
            {
                hd[i] = 3;
            }
            if (hlist[i].x == 0 && hlist[i].y == -1)
            {
                hd[i] = 4;
            }
            if (hlist[i].x == 1 && hlist[i].y == 0)
            {
                hd[i] = 1;
            }
            if (hlist[i].x == -1 && hlist[i].y == 0)
            {
                hd[i] = 2;
            }
            if (hlist[i].x == 0 && hlist[i].y == 0)
            {
                return 5;
            }
        }
        //use the change in direction to search through the index table
        var indexNashTable = nashTable.Item1;
        var valueNashTable = nashTable.Item2;
        //print($"{hd[0]}, {hd[1]}");
        List<int> index = new List<int>();
        List<Tuple<int, int>[]> index2 = new List<Tuple<int, int>[]>();
        for (int i = 0;i< indexNashTable.Count;i++)
        {
            if (hd[0] == indexNashTable[i].Item1)
            {
                index.Add(i);
                var test = indexNashTable[i].Item2;
                for(int j = 0; j < test.Length; j++)
                {
                    
                    if (test[j][0].Item1 == hd[1])
                    {
                        index.Add(j);
                        index2.Add(test[j]);
                    }

                }
            }
        }
        //use information from the index to find the correct best move hero 3 can take
        var player2options = valueNashTable[index[0]][index[1]];
        float maxval = -1f;
        int tempIndex = -1;
        for(int i = 0; i < player2options.Length; i++)
        {
           
            if (player2options[i].Item3 > maxval)
            {
                maxval = player2options[i].Item3;
                tempIndex = i;
            }
        }
        index.Add(tempIndex);
        var bestMove = index2[0][index[2]].Item2;

        //return the best move
        return bestMove;
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;

public class MiniMax : MonoBehaviour
{
    private static int hero_Num;
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

    private static float heuristic(int[][] arr)
    {
        var heroLoc = findPlayer(hero_Num, arr);
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
            delimiter = 50;
        }

            var manhatten = Math.Abs(heroLoc[0] - enemyLoc[0]) + Math.Abs(heroLoc[1] - enemyLoc[1]);

        return 50 - manhatten + delimiter ; // remove delimiter
        
       
    }

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
    private static int[][] setMove(int[][] state,int hero, int direction, int x, int y)
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
            clonedArray[x ][y -1] = hero;
            
        }
        return clonedArray;
    }

    public static List<Tuple<int, int[][]>> Moves(int[][] state,int hero, int x, int y)
    {
        int[] directions = { 1, 2, 3, 4 };
        List<int> dir = new List<int>();
        List<Tuple<int, int[][]>> allowedeDir = new List<Tuple<int, int[][]>>();
        if(hero == hero_Num)
        {
            foreach (int direction in directions)
            {

                if (direction == 1)
                {
                    if(x + 1 >= 0 && x + 1 < 16 && y >= 0 && y < 9)
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
                    if(x - 1 >= 0 && x - 1 < 16 && y >= 0 && y < 9)
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
                    if (y + 1 >= 0 && y + 1 <9 && x >= 0 && x < 16)
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
        foreach(var d in allowedeDir)
        {
            temp = temp+" "+ d;
            //PrintArr(state);
        }
        //print($"Allowed Directions: {temp}");
        return allowedeDir;
    }

    private static float minVal(int[][] state, int depth, float alpha, float beta, bool agent)
    {
        var enemy = findPlayer(4, state);
        var v = float.PositiveInfinity;
        foreach ( var move in Moves(state, 4, enemy[0], enemy[1]))
        {
            v = Math.Min(v, minimax(move.Item2, depth - 1, alpha, beta, agent));
            if(v <= alpha)
            {
                return v;
            }
            beta = Math.Min(beta, v);
        }
          
        return v;
    }

    private static float maxVal(int[][] state, int depth, float alpha, float beta, bool agent)
    {
        var hero = findPlayer(hero_Num, state);
        var v = float.NegativeInfinity;
        foreach (var move in Moves(state, hero_Num, hero[0], hero[1]))
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

    private static float minimax(int[][] state, int depth, float alpha, float beta, bool agent)
    {
        //PrintArr(state);
        //print("----------------------------------------------");
        if(depth == 0 || findPlayer(4,state) == null || findPlayer(hero_Num,state) == null)
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


    public static void PlayerMove()
    {
        if (UnitManager.Instance.EscapeCount == 3 || UnitManager.Instance.DeadHeroes == 3)
        {
            
            GameManager.Instance.ChangeState(GameState.WonGame);
            return;
        }
        if (GameManager.Instance.GameState != GameState.Heroes3Turn)
        {
            
            //GameManager.Instance.ChangeState(GameState.EnemiesTurn);
            return;
        }
        Dictionary<Vector2, Tile> map = GridManager.Instance.GetGrid();
        int[][] arr = VectorMap(map);
        
        BaseUnit currentHero = UnitManager.Instance.SelectedHeroes[2];
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero_Location = (UnitManager.Instance.SelectedHeroes[0] != null) ? (UnitManager.Instance.SelectedHeroes[0].transform.position) : (dummy_location);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);
        Vector3 enemy_Location = (UnitManager.Instance.SelectedEnemy != null) ? (UnitManager.Instance.SelectedEnemy.transform.position) : (dummy_location);
        BaseHero currentHeroBH = (currentHero != null) ? ((BaseHero)currentHero) : (null);
        if (currentHero == null) 
        {
            
            GameManager.Instance.ChangeState(GameState.EnemiesTurn);
            return;

        }
        if(hero_Location != dummy_location)
        {
            arr[(int)hero_Location.x][(int)hero_Location.y] = 1;
        }
        if (hero2_Location != dummy_location)
        {
            arr[(int)hero2_Location.x][(int)hero2_Location.y] = 2;
        }
        if (hero3_Location != dummy_location)
        {
            arr[(int)hero3_Location.x][(int)hero3_Location.y] = 3;
        }
        if (enemy_Location != dummy_location)
        {
            arr[(int)enemy_Location.x][(int)enemy_Location.y] = 4;
        }
        var testxy = currentHero.transform.position;
        var heronumber = arr[(int)testxy.x][(int)testxy.y];

        hero_Num = heronumber;
        
        if(UnitManager.Instance.CanEscape == false)
        {
            List<Tuple<int, int[][]>> list = Moves(arr, heronumber, (int)testxy.x, (int)testxy.y);
            if(list.Count == 0) { GameManager.Instance.ChangeState(GameState.EnemiesTurn); return; }
            var bestMove = list[0].Item1;
            var depth = 5;
            var alpha = float.NegativeInfinity;
            var beta = float.PositiveInfinity;
            bool agent= false;
            //print($"Hero Number: {hero_Num}, Hero Location: {testxy}, beforebestMove: {bestMove}");
            foreach (var move in list)
            {

                var rs = minimax(move.Item2, depth, alpha, beta, agent);
                //print($"rs: {rs}, {alpha}");
                if (rs > alpha)
                {
                    
                    alpha = rs;
                    bestMove = move.Item1;
                    //print($"In loop: {bestMove} , {alpha}");
                }

            }
            if(UnitManager.Instance.DeadHeroes == 0)
            {

                
                bestMove = Nash.NashEQ();
            }
            //($"Hero Number: {hero_Num}, Hero Location: {testxy}, afterbestMove: {bestMove}");
            //the best move will be implemented here
            if (bestMove == 1) // x if left and right, y is up and down
            {
                currentHeroBH.transform.position += new Vector3(1f, 0f, 0f);
            }
            else if (bestMove == 2)
            {
                currentHeroBH.transform.position += new Vector3(-1f, 0f, 0f);
            }
            else if (bestMove == 3)
            {
                currentHeroBH.transform.position += new Vector3(0f, 1f, 0f);
            }
            else if (bestMove == 4)
            {
                currentHeroBH.transform.position += new Vector3(0f, -1f, 0f);
            }
            else if (bestMove == 5)
            {
                currentHeroBH.transform.position += new Vector3(0f, 0f, 0f);
            }

            Vector3 hero3_LocationUpdate = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);
            UnitManager.Instance.hero3Pos = hero3_LocationUpdate;

        }
        else if(UnitManager.Instance.CanEscape == true)
        {
            
            Vector2 pos = new Vector2((int)hero3_Location.x, (int)hero3_Location.y);
            var HeroTile = GridManager.Instance.GetTileAtPosition(pos);
            //if (HeroTile != null) { print($"HeroTile: {HeroTile.gridLocation.x}, {HeroTile.gridLocation.y}"); }
            Vector2 pos2 = new Vector2((int)UnitManager.Instance.EscapeExit.x, (int)UnitManager.Instance.EscapeExit.y);
            var PortalTile = GridManager.Instance.GetTileAtPosition(pos2);
            //if (PortalTile != null) { print($"PortalTile: {PortalTile.gridLocation.x}, {PortalTile.gridLocation.y}"); }
            var path = AstarMIniMax.Astar(HeroTile, PortalTile);
            

            if (path.Count == 0) 
            {  
                GameManager.Instance.ChangeState(GameState.HeroesTurn);
                return;
            }
            var nextTile = path[0].gridLocation;
            var newPos = nextTile - HeroTile.gridLocation;
            
            if(newPos.x == 0 && newPos.y == 1)
            {
                currentHeroBH.transform.position += new Vector3(0f, 1f, 0f);
            }
            if (newPos.x == 0 && newPos.y == -1)
            {
                currentHeroBH.transform.position += new Vector3(0f, -1f, 0f);
            }
            if (newPos.x == 1 && newPos.y == 0)
            {
                currentHeroBH.transform.position += new Vector3(1f, 0f, 0f);
            }
            if (newPos.x == -1 && newPos.y == 0)
            {
                currentHeroBH.transform.position += new Vector3(-1f, 0f, 0f);
            }
        }

        int currentTurn = GameManager.heroTurn;
        //print(currentTurn);
        GameManager.heroTurn = (GameManager.heroTurn < 3) ? (GameManager.heroTurn + 1) : (1);
        //GameManager.Instance.ChangeState(GameState.EnemiesTurn);

        if (UnitManager.Instance.SelectedEnemy != null && currentHeroBH != null && UnitManager.Instance.SelectedEnemy.transform.position == currentHeroBH.transform.position)
        {
            print("Player kills");
            Destroy(UnitManager.Instance.SelectedEnemy.gameObject);
            GameManager.Instance.Nash_total_kills++;
            UnitManager.Instance.CanEscape = true;
            UnitManager.Instance.SpawnExit();
        }

        if (currentHeroBH != null && UnitManager.Instance.CanEscape && currentHeroBH.transform.position == UnitManager.Instance.EscapeExit)
        {
            print(currentHeroBH.transform.position);
            Destroy(currentHeroBH.gameObject);
            print("Escaped");
            UnitManager.Instance.EscapeCount += 1;
            if (UnitManager.Instance.EscapeCount + UnitManager.Instance.DeadHeroes == 3)
            {
                
                GameManager.Instance.ChangeState(GameState.WonGame);
            }
        }

     
        
            
            GameManager.Instance.ChangeState(GameState.EnemiesTurn);
        

    }


    
    

}

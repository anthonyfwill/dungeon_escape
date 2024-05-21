using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AstarMIniMax : MonoBehaviour 
{



    public static List<Tile> Astar(Tile start, Tile end)
    {   //if game is done return empty list
        if(UnitManager.Instance.EscapeCount == 3 || UnitManager.Instance.DeadHeroes == 3)
        {
            return new List<Tile>();
        }
        //create open and closed list, fi nd the heroes locations
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero_Location = (UnitManager.Instance.SelectedHeroes[0] != null) ? (UnitManager.Instance.SelectedHeroes[0].transform.position) : (dummy_location);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);
        
        Vector2Int hero1 = new Vector2Int((int)hero_Location.x,(int)hero_Location.y);
        Vector2Int hero2 = new Vector2Int((int)hero2_Location.x, (int)hero2_Location.y);
        Vector2Int hero3 = new Vector2Int((int)hero3_Location.x, (int)hero3_Location.y);
        //add the start to the open list
        openList.Add(start);
        while (openList.Count > 0)
        {   //while open list is not empty
            //find the best tile based on the F value
            Tile currentTile = openList.OrderBy(x => x.F).First();
            openList.Remove(currentTile);
            closedList.Add(currentTile);
            //remove it from the open list and add it to the closed list and check if the current tile is the goal
            if(currentTile == end)
            {
                //return the best path
                return GetFinishedLast(start, end);
            }
            //find the neighbhoring tiles
            var neighborTiles = getNTiles(currentTile);
            
            var neighborDirections = getNTilesDirections(currentTile);
            //for each possible neighboring tile
            foreach ( var tile in neighborTiles)
            {
                //if its traverable
                bool isOccupied = false;
                if(tile.gridLocation == hero1 || tile.gridLocation == hero2 || tile.gridLocation == hero3)
                {
                    isOccupied = true;
                }
                if (!tile.Walkable || isOccupied || closedList.Contains(tile))
                {
                    continue;
                }
                //get the manhatten distance to get G and H
                tile.G = GetManhattenDistance(start, tile);
                tile.H = GetManhattenDistance(end, tile);
                tile.previousTile = currentTile;
                //store the current tile and the previous tile for backtracking and add the tile to the open list
                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                    
                }
            }

        }
        //print("cant find end");
        return new List<Tile>();
    }
    //used to find the path to the start and goal
    private static List<Tile> GetFinishedLast(Tile start, Tile end)
    {
        var list = new List<Tile>();
        var dirList = new List<int>();
        Tile currentTile = end;
        while(currentTile != start)
        {
            list.Add(currentTile);
            
            currentTile = currentTile.previousTile;
        }
        //print($"is list {list.Count}");
        list.Reverse();
        return list;
    }

    //manhatten distance funtion
    private static int GetManhattenDistance(Tile start, Tile tile)
    {
        return Mathf.Abs(start.gridLocation.x - tile.gridLocation.x) + Mathf.Abs(start.gridLocation.y - tile.gridLocation.y);
    }
    //find the neighboring tiles and adds them to list
    private static List<Tile> getNTiles(Tile currentTile)
    {
        var map = GridManager.Instance.GetGrid();
        List<Tile> neighbhors = new List<Tile>();
        List<Tuple<int, Tile>> neighbourDirections = new List<Tuple<int, Tile>>();
        //up
        Vector2Int LocationCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y +1);
        if (map.ContainsKey(LocationCheck))
        {
            neighbhors.Add(map[LocationCheck]);
            neighbourDirections.Add(new Tuple<int, Tile>(3, map[LocationCheck]));
        }
        //down
        LocationCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);
        if (map.ContainsKey(LocationCheck))
        {
            neighbhors.Add(map[LocationCheck]);
            neighbourDirections.Add(new Tuple<int, Tile>(4, map[LocationCheck]));
        }
        //left
        LocationCheck = new Vector2Int(currentTile.gridLocation.x-1, currentTile.gridLocation.y);
        if (map.ContainsKey(LocationCheck))
        {
            neighbhors.Add(map[LocationCheck]);
            neighbourDirections.Add(new Tuple<int, Tile>(2, map[LocationCheck]));
        }
        //right
        LocationCheck = new Vector2Int(currentTile.gridLocation.x+1, currentTile.gridLocation.y);
        if (map.ContainsKey(LocationCheck))
        {
            neighbhors.Add(map[LocationCheck]);
            neighbourDirections.Add(new Tuple<int, Tile>(1, map[LocationCheck]));
        }

        return neighbhors;
    }
    //tester funtion not in use.
    private static List<Tuple<int, Tile>> getNTilesDirections(Tile currentTile)
    {
        var map = GridManager.Instance.GetGrid();
        
        List<Tuple<int, Tile>> neighbourDirections = new List<Tuple<int, Tile>>();
        //up
        Vector2Int LocationCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y + 1);
        if (map.ContainsKey(LocationCheck))
        {
            
            neighbourDirections.Add(new Tuple<int, Tile>(3, map[LocationCheck]));
        }
        //down
        LocationCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);
        if (map.ContainsKey(LocationCheck))
        {
            
            neighbourDirections.Add(new Tuple<int, Tile>(4, map[LocationCheck]));
        }
        //left
        LocationCheck = new Vector2Int(currentTile.gridLocation.x - 1, currentTile.gridLocation.y);
        if (map.ContainsKey(LocationCheck))
        {
            
            neighbourDirections.Add(new Tuple<int, Tile>(2, map[LocationCheck]));
        }
        //right
        LocationCheck = new Vector2Int(currentTile.gridLocation.x + 1, currentTile.gridLocation.y);
        if (map.ContainsKey(LocationCheck))
        {
            
            neighbourDirections.Add(new Tuple<int, Tile>(1, map[LocationCheck]));
        }

        return neighbourDirections;
    }
}

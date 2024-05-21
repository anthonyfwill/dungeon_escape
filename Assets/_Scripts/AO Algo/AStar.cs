using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using C5; 


public class AStar : MonoBehaviour {
    public static void HeroMove() {
        if (UnitManager.Instance.SelectedEnemy != null) {
            //HeroMove1();
            MCTSMove.PlayMCTS();
        } else {
            HeroMove2();
        }
    }

    public static void HeroMove1() { 
        BaseUnit currentHero = UnitManager.Instance.SelectedHeroes[0];
        if (currentHero != null && UnitManager.Instance.heroPath.Count > 0) {
            BaseHero currentHeroBH = (BaseHero)currentHero;
            currentHeroBH.transform.position = UnitManager.Instance.heroPath.Pop();
            if (currentHeroBH.transform.position == UnitManager.Instance.SelectedEnemy.transform.position) {
                Destroy(UnitManager.Instance.SelectedEnemy.gameObject);
                UndoPath();
                UnitManager.Instance.heroPathSet.Clear();
                print("Hero1 killed enemy");
                UnitManager.Instance.CanEscape = true;
                UnitManager.Instance.SpawnExit();
            }
        }
        GameManager.Instance.ChangeState(GameState.Heroes2Turn);
    }

    public static void HeroMove2() { 
        BaseUnit currentHero = UnitManager.Instance.SelectedHeroes[0];
        if (currentHero != null && UnitManager.Instance.heroPath.Count > 0) {
            BaseHero currentHeroBH = (BaseHero)currentHero;
            currentHeroBH.transform.position = UnitManager.Instance.heroPath.Pop();
            if (UnitManager.Instance.CanEscape && currentHeroBH.transform.position == UnitManager.Instance.EscapeExit) {
                UndoPath();
                Destroy(currentHeroBH.gameObject);
                print("Escaped");
                UnitManager.Instance.EscapeCount += 1;
                if (UnitManager.Instance.EscapeCount + UnitManager.Instance.DeadHeroes == 3) {
                    GameManager.Instance.ChangeState(GameState.WonGame);
                }
            } else {
                GameManager.Instance.ChangeState(GameState.Heroes2Turn);
            }
        }
        GameManager.Instance.ChangeState(GameState.Heroes2Turn);
    }

    public static void KillEnemy() {
        if (UnitManager.Instance.SelectedEnemy != null) {
            print("Player kills");
            Destroy(UnitManager.Instance.SelectedEnemy.gameObject);
            UnitManager.Instance.heroPathSet.Clear();
            UnitManager.Instance.CanEscape = true;
            UnitManager.Instance.SpawnExit();
        }
    }

    public static bool ValidPath() {
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);

        if (UnitManager.Instance.heroPathSet.Count == 0 || UnitManager.Instance.heroPathSet.Contains(hero2_Location) || UnitManager.Instance.heroPathSet.Contains(hero3_Location) ) {
            return false;
        }

        return true;
    }

    public static void AStarSearch1() {
        UnitManager.Instance.heroPath.Clear();

        BaseUnit currentHero = UnitManager.Instance.SelectedHeroes[0];

        if((GameManager.Instance.GameState != GameState.HeroesTurn) || (currentHero == null)) {
            GameManager.Instance.ChangeState(GameState.Heroes2Turn);
            //return new Dictionary<int, Vector2>();
            return;
        }

        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);

        BaseHero currentHeroBH = (BaseHero)currentHero;

        // Creating a dictionary where keys are integers and values are arrays of integers
        //Dictionary<int, Vector2> possibleMove = new Dictionary<int, Vector2>();
        for (int row = 0; row < 16; ++row) {
            for (int col = 0; col < 9; ++col) {
                var location = new Vector3(row, col, 0f);
                // Node tmpNode = Node.SetValues(int.MaxValue, (tmpNode.GCost + (int)ManhattanDistance(location, EscapeExit)), row, col, null);

                Node tmpNode = new Node();
                tmpNode.GCost = int.MaxValue;
                tmpNode.FCost = tmpNode.GCost + (int)ManhattanDistance(location, UnitManager.Instance.SelectedEnemy.transform.position);
                tmpNode.ParentNode = null;
                tmpNode.x = row;
                tmpNode.y = col;

                UnitManager.Instance.tileCosts[location] = tmpNode;
            }
        }

        UnitManager.Instance.tileCosts[currentHeroBH.transform.position].GCost = 0;

        // Create an IntervalHeap of integers
        var possibleMove = new IntervalHeap<(int, int, (float, float))>();
        System.Collections.Generic.HashSet<Vector3> closedMoves = new System.Collections.Generic.HashSet<Vector3>();

        int manhattanDistance = (int)ManhattanDistance(currentHeroBH.transform.position, UnitManager.Instance.SelectedEnemy.transform.position);
        Node startNode = new Node();
        startNode.HCost = manhattanDistance;
        startNode.GCost = 0;
        startNode.FCost = CalculateFCost(startNode.GCost, startNode.HCost);
        startNode.ParentNode = null;
        startNode.x = (int)currentHeroBH.transform.position.x;
        startNode.y = (int)currentHeroBH.transform.position.y;


        possibleMove.Add((startNode.FCost, startNode.HCost, (currentHeroBH.transform.position.x, currentHeroBH.transform.position.y)));

        while (!possibleMove.IsEmpty) {
            var currentNode = possibleMove.DeleteMin();
            var currentHCost = currentNode.Item2;
            Vector3 tmpLocation = new Vector3(currentNode.Item3.Item1, currentNode.Item3.Item2, 0f);
            int currentGCost = UnitManager.Instance.tileCosts[tmpLocation].GCost;

            if (tmpLocation == UnitManager.Instance.SelectedEnemy.transform.position) {
                UndoPath();
                FindPath(UnitManager.Instance.tileCosts[tmpLocation]);
                print(currentNode);
                print("Found Path");
                UnitManager.Instance.heroPath.Pop();
                return;
            }

            closedMoves.Add(tmpLocation);
            float currentY = tmpLocation.y;
            float currentX = tmpLocation.x;

            foreach(var neighbor in AllValidNeighbors(hero2_Location, hero3_Location, currentX, currentY)) {
                if (!closedMoves.Contains(neighbor)){
                    float tentativeGCost = currentGCost +  ManhattanDistance(tmpLocation, neighbor);

                    if (tentativeGCost < UnitManager.Instance.tileCosts[neighbor].GCost) {
                        // Set parent of this node\
                        Node parentNode = UnitManager.Instance.tileCosts[tmpLocation];
                        UnitManager.Instance.tileCosts[neighbor].ParentNode = parentNode;

                        // Create node for neighbor
                        Node neighborNode = new Node();
                        neighborNode.HCost = (int)ManhattanDistance(neighbor, UnitManager.Instance.SelectedEnemy.transform.position);
                        neighborNode.GCost = (int)tentativeGCost;
                        neighborNode.FCost = CalculateFCost(neighborNode.GCost, neighborNode.HCost);
                        neighborNode.ParentNode = null;

                        UnitManager.Instance.tileCosts[neighbor].GCost = neighborNode.GCost;
                        UnitManager.Instance.tileCosts[neighbor].HCost = neighborNode.HCost;
                        UnitManager.Instance.tileCosts[neighbor].FCost = neighborNode.FCost;

                        var newMove = (UnitManager.Instance.tileCosts[neighbor].FCost, UnitManager.Instance.tileCosts[neighbor].HCost, (neighbor.x, neighbor.y));

                        if (!possibleMove.Contains(newMove)) {
                            possibleMove.Add(newMove);
                        }
                    }
                }
            }
        }

    }

    public static void AStarSearch2() {
        UnitManager.Instance.heroPath.Clear();

        BaseUnit currentHero = UnitManager.Instance.SelectedHeroes[0];

        if((GameManager.Instance.GameState != GameState.HeroesTurn) || (currentHero == null)) {
            GameManager.Instance.ChangeState(GameState.Heroes2Turn);
            //return new Dictionary<int, Vector2>();
            return;
        }

        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);

        BaseHero currentHeroBH = (BaseHero)currentHero;

        // Creating a dictionary where keys are integers and values are arrays of integers
        //Dictionary<int, Vector2> possibleMove = new Dictionary<int, Vector2>();
        for (int row = 0; row < 16; ++row) {
            for (int col = 0; col < 9; ++col) {
                var location = new Vector3(row, col, 0f);
                // Node tmpNode = Node.SetValues(int.MaxValue, (tmpNode.GCost + (int)ManhattanDistance(location, EscapeExit)), row, col, null);

                Node tmpNode = new Node();
                tmpNode.GCost = int.MaxValue;
                tmpNode.FCost = tmpNode.GCost + (int)ManhattanDistance(location, UnitManager.Instance.EscapeExit);
                tmpNode.ParentNode = null;
                tmpNode.x = row;
                tmpNode.y = col;

                UnitManager.Instance.tileCosts[location] = tmpNode;
            }
        }

        UnitManager.Instance.tileCosts[currentHeroBH.transform.position].GCost = 0;

        // Create an IntervalHeap of integers
        var possibleMove = new IntervalHeap<(int, int, (float, float))>();
        System.Collections.Generic.HashSet<Vector3> closedMoves = new System.Collections.Generic.HashSet<Vector3>();

        int manhattanDistance = (int)ManhattanDistance(currentHeroBH.transform.position, UnitManager.Instance.EscapeExit);
        Node startNode = new Node();
        startNode.HCost = manhattanDistance;
        startNode.GCost = 0;
        startNode.FCost = CalculateFCost(startNode.GCost, startNode.HCost);
        startNode.ParentNode = null;
        startNode.x = (int)currentHeroBH.transform.position.x;
        startNode.y = (int)currentHeroBH.transform.position.y;


        possibleMove.Add((startNode.FCost, startNode.HCost, (currentHeroBH.transform.position.x, currentHeroBH.transform.position.y)));

        while (!possibleMove.IsEmpty) {
            var currentNode = possibleMove.DeleteMin();
            var currentHCost = currentNode.Item2;
            Vector3 tmpLocation = new Vector3(currentNode.Item3.Item1, currentNode.Item3.Item2, 0f);
            int currentGCost = UnitManager.Instance.tileCosts[tmpLocation].GCost;

            if (tmpLocation == UnitManager.Instance.EscapeExit) {
                UndoPath();
                FindPath(UnitManager.Instance.tileCosts[tmpLocation]);
                print(currentNode);
                print("Found Path");
                UnitManager.Instance.heroPath.Pop();
                return;
            }

            closedMoves.Add(tmpLocation);
            float currentY = tmpLocation.y;
            float currentX = tmpLocation.x;

            foreach(var neighbor in AllValidNeighbors(hero2_Location, hero3_Location, currentX, currentY)) {
                if (!closedMoves.Contains(neighbor)){
                    float tentativeGCost = currentGCost +  ManhattanDistance(tmpLocation, neighbor);

                    if (tentativeGCost < UnitManager.Instance.tileCosts[neighbor].GCost) {
                        // Set parent of this node\
                        Node parentNode = UnitManager.Instance.tileCosts[tmpLocation];
                        UnitManager.Instance.tileCosts[neighbor].ParentNode = parentNode;

                        // Create node for neighbor
                        Node neighborNode = new Node();
                        neighborNode.HCost = (int)ManhattanDistance(neighbor, UnitManager.Instance.EscapeExit);
                        neighborNode.GCost = (int)tentativeGCost;
                        neighborNode.FCost = CalculateFCost(neighborNode.GCost, neighborNode.HCost);
                        neighborNode.ParentNode = null;

                        UnitManager.Instance.tileCosts[neighbor].GCost = neighborNode.GCost;
                        UnitManager.Instance.tileCosts[neighbor].HCost = neighborNode.HCost;
                        UnitManager.Instance.tileCosts[neighbor].FCost = neighborNode.FCost;

                        var newMove = (UnitManager.Instance.tileCosts[neighbor].FCost, UnitManager.Instance.tileCosts[neighbor].HCost, (neighbor.x, neighbor.y));

                        if (!possibleMove.Contains(newMove)) {
                            possibleMove.Add(newMove);
                        }
                    }
                }
            }
        }

    } 

    public static void AStarSearch() {
        if (UnitManager.Instance.SelectedEnemy != null) {
            //AStarSearch1();
            //MCTSMove.PlayMCTS();
            return;
        } else {
            if (!ValidPath()) {
                AStarSearch2();
            }
        }
    }

    public static Stack<Vector3> FindPath(Node endNode) {
        while (endNode != null) {
            UnitManager.Instance.heroPath.Push(new Vector3(endNode.x, endNode.y, 0f));
            Tile tilePath = GridManager.Instance.GetTileAtPosition(new Vector2(endNode.x, endNode.y));
            //tilePath.LightPath();

            UnitManager.Instance.heroPathSet.Add(new Vector3(endNode.x, endNode.y, 0f));
            endNode = endNode.ParentNode;
        }
        
        return UnitManager.Instance.heroPath;
    }

    public static void UndoPath() {
        float currentX = UnitManager.Instance.SelectedHeroes[0].transform.position.x;
        float currentY = UnitManager.Instance.SelectedHeroes[0].transform.position.y;
        print(UnitManager.Instance.SelectedHeroes[0].transform.position);

        Tile currentTile = GridManager.Instance.GetTileAtPosition(new Vector2(currentX, currentY));
        currentTile.Init((int)currentX, (int)currentY);

        foreach(var box in UnitManager.Instance.heroPathSet) {
            Vector3 current = new Vector3(box.x, box.y, 0f);
            if (current == UnitManager.Instance.SelectedHeroes[0].transform.position) continue;
            
            Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(box.x, box.y));
            tile1.Init((int)box.x, (int)box.y);
        }
    }

    public static List<Vector3> AllValidNeighbors(Vector3 hero2_Location, Vector3 hero3_Location, float currentX, float currentY) {
        // Creating a dictionary where keys are integers and values are arrays of integers
        Dictionary<int, Vector2> directions = new Dictionary<int, Vector2>();

        // List of all Valid neighbors
        List<Vector3> possibleNeighbors = new List<Vector3>();

        // Adding elements to the dictionary
        directions.Add(0, new Vector2(-1f,0f));
        directions.Add(1, new Vector2(1f,0f));
        directions.Add(2, new Vector2(0f,-1f));
        directions.Add(3, new Vector2(0f,1f));

        foreach(var keyValue in directions) {
            var direction = keyValue.Value;
            float tmpX = currentX + direction.x;
            float tmpY = currentY + direction.y;
            Vector3 tmpLocation = new Vector3(tmpX, tmpY, 0f);
            Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(tmpX, tmpY));

            if ((0 <= tmpY && tmpY <= 8) && 
                (0 <= tmpX && tmpX <= 15) &&
                (tmpLocation != hero2_Location) &&
                (tmpLocation != hero3_Location) &&
                (tile1.TileName != "Mountain")) {
                possibleNeighbors.Add(tmpLocation);
            }
        }

        return possibleNeighbors;
    }

    public static float ManhattanDistance(Vector3 currentNode, Vector3 targetNode) {
        // Manhattans Distance
        //Eucliadian distance
        float difX = System.Math.Abs(targetNode.x - currentNode.x);
        float difY = System.Math.Abs(targetNode.y - currentNode.y);
        float mDistance = difX + difY;
        float diagnol = difX * difX + difY * difY;
        float eDistance = (float)System.Math.Sqrt(diagnol);
        return 40 * mDistance + 20 * eDistance + 30 * diagnol;
    }    

    public static int CalculateFCost(int gCost, int hCost) {
        return gCost + hCost;
    }

    public static void SpawnExit() {
        var random = new System.Random();
        bool goodExit = false;
        

        while (!goodExit) {
            // Generate a random integer: 0 (left/right) or 1 (up/bottom)
            int side = random.Next(0, 2);
            // Generate a random integer: 0 (left or down) or 1 (right or up)
            int select = random.Next(0, 2);

            if (side == 1) {
                float randomNum = (float)Random.Range(0, 16);
                if (select == 1) {
                    UnitManager.Instance.EscapeExit = new Vector3(randomNum, 8f, 0f);
                } else {
                    UnitManager.Instance.EscapeExit = new Vector3(randomNum, 0f, 0f);
                }
            } else {
                float randomNum = (float)Random.Range(0, 9);
                if (select == 1) {
                    UnitManager.Instance.EscapeExit = new Vector3(15f, randomNum, 0f);
                } else {
                    UnitManager.Instance.EscapeExit = new Vector3(0f, randomNum, 0f);
                }
            }
            float newX = UnitManager.Instance.EscapeExit.x;
            float newY = UnitManager.Instance.EscapeExit.y;

            Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(newX, newY));
            if (tile1.TileName != "Mountain") {
                tile1._isPortalSpawned = true;
                tile1.ColorPortal();
                goodExit = true;
                if (UnitManager.Instance.SelectedHeroes[0].transform.position == UnitManager.Instance.EscapeExit) {
                    Destroy(UnitManager.Instance.SelectedHeroes[0].gameObject);
                }
                if (UnitManager.Instance.SelectedHeroes[1].transform.position == UnitManager.Instance.EscapeExit) {
                    Destroy(UnitManager.Instance.SelectedHeroes[1].gameObject);
                }
                if (UnitManager.Instance.SelectedHeroes[2].transform.position == UnitManager.Instance.EscapeExit) {
                    Destroy(UnitManager.Instance.SelectedHeroes[2].gameObject);
                }
            }

        }

        print(UnitManager.Instance.EscapeExit);
    }
}

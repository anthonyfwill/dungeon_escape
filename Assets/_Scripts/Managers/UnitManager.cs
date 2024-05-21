using C5;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : MonoBehaviour {
    public static UnitManager Instance;
    public Vector3 hero1Pos;
    public Vector3 hero2Pos;
    public Vector3 hero3Pos;
    public Vector3 enemyPos;
    private List<ScriptableUnit> _units;
    public BaseHero SelectedHero;
    public BaseUnit[] SelectedHeroes = new BaseHero[3];
    public Vector3 EscapeExit;
    public bool CanEscape = false;           /// <summary>
    
    public int EscapeCount = 0;
    public int DeadHeroes = 0;
    public Dictionary<Vector3, Node> tileCosts = new Dictionary<Vector3, Node>();
    public System.Collections.Generic.HashSet<Vector3> heroPathSet = new System.Collections.Generic.HashSet<Vector3>();
    public Stack<Vector3> heroPath = new Stack<Vector3>();
    public BaseEnemy SelectedEnemy;
    public bool valid_game;

    [SerializeField] public Portal portal; //Prefab for Portal
    private bool portal_spawned = false;

    void Update()
    {
        if((CanEscape) && (!portal_spawned))
        {
            float Exit_X = EscapeExit.x;
            float Exit_Y = EscapeExit.y;
            Vector3 exit_vec = new Vector3(Exit_X, Exit_Y, 0.0f); 
            Instantiate(portal, exit_vec, Quaternion.identity);
            portal_spawned = true;
            portal.transform.position = new Vector3(Exit_X, Exit_Y, 0.0f);
        }
    }

    void Awake() {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();

    }

    public void SpawnHeroes() {
        var heroes = _units.Where(u => u.Faction == Faction.Hero);
        int trackHero = 0;

        foreach(var hero in heroes) {
            var heroPrefab = (BaseUnit)hero.UnitPrefab;
            var spawnedHero = Instantiate(heroPrefab);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            SelectedHeroes[trackHero] = spawnedHero;
            randomSpawnTile.SetUnit(spawnedHero);
            trackHero += 1;
        }
        hero1Pos = SelectedHeroes[0].transform.position;
        hero2Pos = SelectedHeroes[1].transform.position;
        hero3Pos = SelectedHeroes[2].transform.position;
        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies()
    {
        var enemyCount = 1;
        valid_game = true;

        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            SelectedEnemy = spawnedEnemy;
            randomSpawnTile.SetUnit(spawnedEnemy);
            print(SelectedEnemy.transform.position);
        }
        enemyPos = SelectedEnemy.transform.position;
        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }

    public void SetSelectedHero(BaseHero hero) {
        SelectedHero = hero;
        MenuManager.Instance.ShowSelectedHero(hero);
    }

    public void PlayerMove() {
        print(GameManager.Instance.GameState);
        if ((GameManager.Instance.GameState != GameState.Heroes2Turn))
        {
            
            return;
        }
        print("passed");

        BaseUnit currentHero = SelectedHeroes[1];
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero_Location = (SelectedHeroes[0] != null) ? (SelectedHeroes[0].transform.position) : (dummy_location);
        Vector3 hero2_Location = (SelectedHeroes[1] != null) ? (SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (SelectedHeroes[2] != null) ? (SelectedHeroes[2].transform.position) : (dummy_location);
        BaseHero currentHeroBH = (currentHero != null) ? ((BaseHero)currentHero) : (null);

        var random = new System.Random();
        bool inBounds = false;

        while (!inBounds && currentHero != null) {
            // Generate a random integer: 0 (move in x) or 1 (move in y)
            int direction = random.Next(0, 2);

            // Generate a random integer: 0 (move in down or right) or 1 (move up or left)
            int randomNum = random.Next(0, 2);
            float progress = (randomNum == 1) ? 1f : -1f;

            float currentY = currentHeroBH.transform.position.y;
            float currentX = currentHeroBH.transform.position.x;

            float tmp_y = currentHeroBH.transform.position.y + progress;
            float tmp_x = currentHeroBH.transform.position.x + progress;

            Vector3 tmp_final1 = new Vector3(currentX, tmp_y, 0f);
            Vector3 tmp_final2 = new Vector3(tmp_x, currentY, 0f);

            Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(currentX, tmp_y));
            Tile tile2 = GridManager.Instance.GetTileAtPosition(new Vector2(tmp_x, currentY));

            if (direction == 1) {
                if ((0 <= tmp_y && tmp_y <= 8) && 
                    (tmp_final1 != hero_Location) &&
                    (tmp_final1 != hero2_Location) &&
                    (tmp_final1 != hero3_Location) &&
                    (tile1.TileName != "Mountain")) {
                    currentHeroBH.transform.position += new Vector3(0f, progress, 0f);
                    inBounds = true;
                }
            } 
            else {
                if (0 <= tmp_x && tmp_x <= 15 && 
                    (tmp_final2 != hero_Location) &&
                    (tmp_final2 != hero2_Location) &&
                    (tmp_final2 != hero3_Location) &&
                    (tile2.TileName != "Mountain")) {
                    currentHeroBH.transform.position += new Vector3(progress, 0f, 0f);
                    inBounds = true;
                }
            }    
        }

        int currentTurn = GameManager.heroTurn;
        //print(currentTurn);
        GameManager.heroTurn = (GameManager.heroTurn < 3) ? (GameManager.heroTurn + 1) : (1);
        //GameManager.Instance.ChangeState(GameState.EnemiesTurn);

        if (SelectedEnemy != null && currentHeroBH != null && SelectedEnemy.transform.position == currentHeroBH.transform.position) {
            print("Player kills");
            Destroy(SelectedEnemy.gameObject);
            CanEscape = true;
            SpawnExit();
        }

        if (currentHeroBH != null && CanEscape && currentHeroBH.transform.position == EscapeExit) {
            print(currentHeroBH.transform.position);
            Destroy(currentHeroBH.gameObject);
            print("Escaped");
            EscapeCount += 1;
            if (EscapeCount + DeadHeroes == 3) {
                
                GameManager.Instance.ChangeState(GameState.WonGame);
            }
        }

        
            GameManager.Instance.ChangeState(GameState.Heroes3Turn);
        

        
    }

    public void HeroMove()
    {
        if (SelectedEnemy != null)
        {
            Invoke("HeroMove1", 1);
        }
        else
        {
            Invoke("HeroMove2", 1);
        }
    }


    public void HeroMove1()
    {
        BaseUnit currentHero = SelectedHeroes[0];
        if (currentHero != null && heroPath.Count > 0)
        {
            BaseHero currentHeroBH = (BaseHero)currentHero;
            currentHeroBH.transform.position = heroPath.Pop();
            if (currentHeroBH.transform.position == SelectedEnemy.transform.position)
            {
                Destroy(SelectedEnemy.gameObject);
                UndoPath();
                heroPathSet.Clear();
                print("Hero1 killed enemy");
                CanEscape = true;
                SpawnExit();
            }
        }
        GameManager.Instance.ChangeState(GameState.Heroes2Turn);
    }

    public void HeroMove2()
    {
        BaseUnit currentHero = SelectedHeroes[0];
        if (currentHero != null && heroPath.Count > 0)
        {
            BaseHero currentHeroBH = (BaseHero)currentHero;
            currentHeroBH.transform.position = heroPath.Pop();
            if (CanEscape && currentHeroBH.transform.position == EscapeExit)
            {
                UndoPath();
                Destroy(currentHeroBH.gameObject);
                print("Escaped");
                EscapeCount += 1;
                if (EscapeCount + DeadHeroes == 3)
                {
                    GameManager.Instance.ChangeState(GameState.WonGame);
                }
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.Heroes2Turn);
            }
        }
        GameManager.Instance.ChangeState(GameState.Heroes2Turn);
    }


    public void KillEnemy()
    {
        if (SelectedEnemy != null)
        {
            print("Player kills");
            Destroy(SelectedEnemy.gameObject);
            heroPathSet.Clear();
            CanEscape = true;
            SpawnExit();
        }
    }


    public bool ValidPath()
    {
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (SelectedHeroes[1] != null) ? (SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (SelectedHeroes[2] != null) ? (SelectedHeroes[2].transform.position) : (dummy_location);

        if (heroPathSet.Count == 0 || heroPathSet.Contains(hero2_Location) || heroPathSet.Contains(hero3_Location))
        {
            return false;
        }

        return true;
    }

    public void AStarSearch1()
    {
        heroPath.Clear();

        BaseUnit currentHero = SelectedHeroes[0];

        if ((GameManager.Instance.GameState != GameState.HeroesTurn) || (currentHero == null))
        {
            GameManager.Instance.ChangeState(GameState.Heroes2Turn);
            //return new Dictionary<int, Vector2>();
            return;
        }

        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (SelectedHeroes[1] != null) ? (SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (SelectedHeroes[2] != null) ? (SelectedHeroes[2].transform.position) : (dummy_location);

        BaseHero currentHeroBH = (BaseHero)currentHero;

        // Creating a dictionary where keys are integers and values are arrays of integers
        //Dictionary<int, Vector2> possibleMove = new Dictionary<int, Vector2>();
        for (int row = 0; row < 16; ++row)
        {
            for (int col = 0; col < 9; ++col)
            {
                var location = new Vector3(row, col, 0f);
                // Node tmpNode = Node.SetValues(int.MaxValue, (tmpNode.GCost + (int)ManhattanDistance(location, EscapeExit)), row, col, null);

                Node tmpNode = new Node();
                tmpNode.GCost = int.MaxValue;
                tmpNode.FCost = tmpNode.GCost + (int)ManhattanDistance(location, SelectedEnemy.transform.position);
                tmpNode.ParentNode = null;
                tmpNode.x = row;
                tmpNode.y = col;

                tileCosts[location] = tmpNode;
            }
        }

        tileCosts[currentHeroBH.transform.position].GCost = 0;

        // Create an IntervalHeap of integers
        var possibleMove = new IntervalHeap<(int, int, (float, float))>();
        System.Collections.Generic.HashSet<Vector3> closedMoves = new System.Collections.Generic.HashSet<Vector3>();

        int manhattanDistance = (int)ManhattanDistance(currentHeroBH.transform.position, SelectedEnemy.transform.position);
        Node startNode = new Node();
        startNode.HCost = manhattanDistance;
        startNode.GCost = 0;
        startNode.FCost = CalculateFCost(startNode.GCost, startNode.HCost);
        startNode.ParentNode = null;
        startNode.x = (int)currentHeroBH.transform.position.x;
        startNode.y = (int)currentHeroBH.transform.position.y;


        possibleMove.Add((startNode.FCost, startNode.HCost, (currentHeroBH.transform.position.x, currentHeroBH.transform.position.y)));

        while (!possibleMove.IsEmpty)
        {
            var currentNode = possibleMove.DeleteMin();
            var currentHCost = currentNode.Item2;
            Vector3 tmpLocation = new Vector3(currentNode.Item3.Item1, currentNode.Item3.Item2, 0f);
            int currentGCost = tileCosts[tmpLocation].GCost;

            if (tmpLocation == SelectedEnemy.transform.position)
            {
                UndoPath();
                FindPath(tileCosts[tmpLocation]);
                print(currentNode);
                print("Found Path");
                heroPath.Pop();
                return;
            }

            closedMoves.Add(tmpLocation);
            float currentY = tmpLocation.y;
            float currentX = tmpLocation.x;

            foreach (var neighbor in AllValidNeighbors(hero2_Location, hero3_Location, currentX, currentY))
            {
                if (!closedMoves.Contains(neighbor))
                {
                    float tentativeGCost = currentGCost + ManhattanDistance(tmpLocation, neighbor);

                    if (tentativeGCost < tileCosts[neighbor].GCost)
                    {
                        // Set parent of this node\
                        Node parentNode = tileCosts[tmpLocation];
                        tileCosts[neighbor].ParentNode = parentNode;

                        // Create node for neighbor
                        Node neighborNode = new Node();
                        neighborNode.HCost = (int)ManhattanDistance(neighbor, SelectedEnemy.transform.position);
                        neighborNode.GCost = (int)tentativeGCost;
                        neighborNode.FCost = CalculateFCost(neighborNode.GCost, neighborNode.HCost);
                        neighborNode.ParentNode = null;

                        tileCosts[neighbor].GCost = neighborNode.GCost;
                        tileCosts[neighbor].HCost = neighborNode.HCost;
                        tileCosts[neighbor].FCost = neighborNode.FCost;

                        var newMove = (tileCosts[neighbor].FCost, tileCosts[neighbor].HCost, (neighbor.x, neighbor.y));

                        if (!possibleMove.Contains(newMove))
                        {
                            possibleMove.Add(newMove);
                        }
                    }
                }
            }
        }

    }


    public void AStarSearch2()
    {
        heroPath.Clear();

        BaseUnit currentHero = SelectedHeroes[0];

        if ((GameManager.Instance.GameState != GameState.HeroesTurn) || (currentHero == null))
        {
            GameManager.Instance.ChangeState(GameState.Heroes2Turn);
            //return new Dictionary<int, Vector2>();
            return;
        }

        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (SelectedHeroes[1] != null) ? (SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (SelectedHeroes[2] != null) ? (SelectedHeroes[2].transform.position) : (dummy_location);

        BaseHero currentHeroBH = (BaseHero)currentHero;

        // Creating a dictionary where keys are integers and values are arrays of integers
        //Dictionary<int, Vector2> possibleMove = new Dictionary<int, Vector2>();
        for (int row = 0; row < 16; ++row)
        {
            for (int col = 0; col < 9; ++col)
            {
                var location = new Vector3(row, col, 0f);
                // Node tmpNode = Node.SetValues(int.MaxValue, (tmpNode.GCost + (int)ManhattanDistance(location, EscapeExit)), row, col, null);

                Node tmpNode = new Node();
                tmpNode.GCost = int.MaxValue;
                tmpNode.FCost = tmpNode.GCost + (int)ManhattanDistance(location, EscapeExit);
                tmpNode.ParentNode = null;
                tmpNode.x = row;
                tmpNode.y = col;

                tileCosts[location] = tmpNode;
            }
        }

        tileCosts[currentHeroBH.transform.position].GCost = 0;

        // Create an IntervalHeap of integers
        var possibleMove = new IntervalHeap<(int, int, (float, float))>();
        System.Collections.Generic.HashSet<Vector3> closedMoves = new System.Collections.Generic.HashSet<Vector3>();

        int manhattanDistance = (int)ManhattanDistance(currentHeroBH.transform.position, EscapeExit);
        Node startNode = new Node();
        startNode.HCost = manhattanDistance;
        startNode.GCost = 0;
        startNode.FCost = CalculateFCost(startNode.GCost, startNode.HCost);
        startNode.ParentNode = null;
        startNode.x = (int)currentHeroBH.transform.position.x;
        startNode.y = (int)currentHeroBH.transform.position.y;


        possibleMove.Add((startNode.FCost, startNode.HCost, (currentHeroBH.transform.position.x, currentHeroBH.transform.position.y)));

        while (!possibleMove.IsEmpty)
        {
            var currentNode = possibleMove.DeleteMin();
            var currentHCost = currentNode.Item2;
            Vector3 tmpLocation = new Vector3(currentNode.Item3.Item1, currentNode.Item3.Item2, 0f);
            int currentGCost = tileCosts[tmpLocation].GCost;

            if (tmpLocation == EscapeExit)
            {
                UndoPath();
                FindPath(tileCosts[tmpLocation]);
                print(currentNode);
                print("Found Path");
                heroPath.Pop();
                return;
            }

            closedMoves.Add(tmpLocation);
            float currentY = tmpLocation.y;
            float currentX = tmpLocation.x;

            foreach (var neighbor in AllValidNeighbors(hero2_Location, hero3_Location, currentX, currentY))
            {
                if (!closedMoves.Contains(neighbor))
                {
                    float tentativeGCost = currentGCost + ManhattanDistance(tmpLocation, neighbor);

                    if (tentativeGCost < tileCosts[neighbor].GCost)
                    {
                        // Set parent of this node\
                        Node parentNode = tileCosts[tmpLocation];
                        tileCosts[neighbor].ParentNode = parentNode;

                        // Create node for neighbor
                        Node neighborNode = new Node();
                        neighborNode.HCost = (int)ManhattanDistance(neighbor, EscapeExit);
                        neighborNode.GCost = (int)tentativeGCost;
                        neighborNode.FCost = CalculateFCost(neighborNode.GCost, neighborNode.HCost);
                        neighborNode.ParentNode = null;

                        tileCosts[neighbor].GCost = neighborNode.GCost;
                        tileCosts[neighbor].HCost = neighborNode.HCost;
                        tileCosts[neighbor].FCost = neighborNode.FCost;

                        var newMove = (tileCosts[neighbor].FCost, tileCosts[neighbor].HCost, (neighbor.x, neighbor.y));

                        if (!possibleMove.Contains(newMove))
                        {
                            possibleMove.Add(newMove);
                        }
                    }
                }
            }
        }

    }


    public void AStarSearch()
    {
        if (SelectedEnemy != null)
        {
            AStarSearch1();
        }
        else
        {
            if (!ValidPath())
            {
                AStarSearch2();
            }
        }
    }

    public Stack<Vector3> FindPath(Node endNode)
    {
        while (endNode != null)
        {
            heroPath.Push(new Vector3(endNode.x, endNode.y, 0f));
            Tile tilePath = GridManager.Instance.GetTileAtPosition(new Vector2(endNode.x, endNode.y));
            tilePath.LightPath();

            heroPathSet.Add(new Vector3(endNode.x, endNode.y, 0f));
            endNode = endNode.ParentNode;
        }

        return heroPath;
    }

    public void UndoPath()
    {
        float currentX = SelectedHeroes[0].transform.position.x;
        float currentY = SelectedHeroes[0].transform.position.y;
        print(SelectedHeroes[0].transform.position);

        Tile currentTile = GridManager.Instance.GetTileAtPosition(new Vector2(currentX, currentY));
        currentTile.Init((int)currentX, (int)currentY);

        foreach (var box in heroPathSet)
        {
            Vector3 current = new Vector3(box.x, box.y, 0f);
            if (current == SelectedHeroes[0].transform.position) continue;

            Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(box.x, box.y));
            tile1.Init((int)box.x, (int)box.y);
        }
    }

    public List<Vector3> AllValidNeighbors(Vector3 hero2_Location, Vector3 hero3_Location, float currentX, float currentY)
    {
        // Creating a dictionary where keys are integers and values are arrays of integers
        Dictionary<int, Vector2> directions = new Dictionary<int, Vector2>();

        // List of all Valid neighbors
        List<Vector3> possibleNeighbors = new List<Vector3>();

        // Adding elements to the dictionary
        directions.Add(0, new Vector2(-1f, 0f));
        directions.Add(1, new Vector2(1f, 0f));
        directions.Add(2, new Vector2(0f, -1f));
        directions.Add(3, new Vector2(0f, 1f));

        foreach (var keyValue in directions)
        {
            var direction = keyValue.Value;
            float tmpX = currentX + direction.x;
            float tmpY = currentY + direction.y;
            Vector3 tmpLocation = new Vector3(tmpX, tmpY, 0f);
            Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(tmpX, tmpY));

            if ((0 <= tmpY && tmpY <= 8) &&
                (0 <= tmpX && tmpX <= 15) &&
                (tmpLocation != hero2_Location) &&
                (tmpLocation != hero3_Location) &&
                (tile1.TileName != "Mountain"))
            {
                possibleNeighbors.Add(tmpLocation);
            }
        }

        return possibleNeighbors;
    }

    public float ManhattanDistance(Vector3 currentNode, Vector3 targetNode)
    {
        // Manhattans Distance
        //Eucliadian distance
        float difX = System.Math.Abs(targetNode.x - currentNode.x);
        float difY = System.Math.Abs(targetNode.y - currentNode.y);
        float mDistance = difX + difY;
        float diagnol = difX * difX + difY * difY;
        float eDistance = (float)System.Math.Sqrt(diagnol);
        return 40 * mDistance + 20 * eDistance + 30 * diagnol;
    }

    public int CalculateFCost(int gCost, int hCost)
    {
        return gCost + hCost;
    }

    public void SpawnExit() {
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
                    EscapeExit = new Vector3(randomNum, 8f, 0f);
                } else {
                    EscapeExit = new Vector3(randomNum, 0f, 0f);
                }
            } else {
                float randomNum = (float)Random.Range(0, 9);
                if (select == 1) {
                    EscapeExit = new Vector3(15f, randomNum, 0f);
                } else {
                    EscapeExit = new Vector3(0f, randomNum, 0f);
                }
            }
            float newX = EscapeExit.x;
            float newY = EscapeExit.y;

            Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(newX, newY));
            if (tile1.TileName != "Mountain") {
                goodExit = true;
                tile1._isPortalSpawned = true;
                tile1.ColorPortal();
                goodExit = true;
                if (SelectedHeroes[0] != null && SelectedHeroes[0].transform.position == EscapeExit)
                {
                    Destroy(SelectedHeroes[0].gameObject);
                    EscapeCount += 1;
                }
                if (SelectedHeroes[1] != null && SelectedHeroes[1].transform.position == EscapeExit)
                {
                    Destroy(SelectedHeroes[1].gameObject);
                    EscapeCount += 1;
                }
                if (SelectedHeroes[2] != null && SelectedHeroes[2].transform.position == EscapeExit)
                {
                    Destroy(SelectedHeroes[2].gameObject);
                    EscapeCount += 1;
                }
            }

        }
        
        print(EscapeExit);
    }

    public void EnemyMove() {  
        //print(GameManager.Instance.GameState);
        var random = new System.Random();
        bool inBounds = false;

        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero_Location = (SelectedHeroes[0] != null) ? (SelectedHeroes[0].transform.position) : (dummy_location);
        Vector3 hero2_Location = (SelectedHeroes[1] != null) ? (SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (SelectedHeroes[2] != null) ? (SelectedHeroes[2].transform.position) : (dummy_location);

        if (hero_Location == dummy_location && hero2_Location == dummy_location && hero3_Location == dummy_location) {
            
            GameManager.Instance.ChangeState(GameState.LostGame);
        }

        while (!inBounds && SelectedEnemy != null) {
            // Generate a random integer: 0 (move in x) or 1 (move in y)
            int direction = random.Next(0, 2);

            // Generate a random integer: 0 (move in down or right) or 1 (move up or left)
            int randomNum = random.Next(0, 2);
            float progress = (randomNum == 1) ? 1f : -1f;
            float tmp_y = SelectedEnemy.transform.position.y + progress;
            float tmp_x = SelectedEnemy.transform.position.x + progress;

            if (direction == 1) {
                if (0 <= tmp_y && tmp_y <= 8) {
                    float currentX = SelectedEnemy.transform.position.x;
                    float currentY = SelectedEnemy.transform.position.y + progress;
                    Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(currentX, currentY));

                    if (tile1.TileName != "Mountain") {
                        SelectedEnemy.transform.position += new Vector3(0f, progress, 0f);
                        inBounds = true;
                    }
                }
            } 
            else {
                if (0 <= tmp_x && tmp_x <= 15) {
                    float currentX = SelectedEnemy.transform.position.x + progress;
                    float currentY = SelectedEnemy.transform.position.y;
                    Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(currentX, currentY));

                    if (tile1.TileName != "Mountain") {
                        SelectedEnemy.transform.position += new Vector3(progress, 0f, 0f);
                        inBounds = true;
                    }
                }
            }    
        }

        if (SelectedEnemy != null) {
            if (SelectedEnemy.transform.position == hero_Location) {
                print("Enemy kills");
                UndoPath();
                Destroy(SelectedHeroes[0].gameObject);
                DeadHeroes += 1;
                print(SelectedHeroes[0]);
            } 
            else if (SelectedEnemy.transform.position == hero2_Location) {
                print("Enemy kills");
                Destroy(SelectedHeroes[1].gameObject);
                DeadHeroes += 1;
                print(SelectedHeroes[1]);
            } 
            else if (SelectedEnemy.transform.position == hero3_Location) {
                print("Enemy kills");
                Destroy(SelectedHeroes[2].gameObject);
                DeadHeroes += 1;
                print(SelectedHeroes[2]);
            } 
        }

        //SelectedEnemy.OccupiedTile = this;
        
        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    public void ResetGame() {

        if(EscapeCount == 3)
        {
            GameManager.Instance.win++;
        }
        else
        {
            GameManager.Instance.loss++;
        }

        GameManager.Instance.WriteCSV();

        if (SelectedHeroes[0] != null) {
            Destroy(SelectedHeroes[0].gameObject);
        } 
        else if (SelectedHeroes[1] != null) {
            Destroy(SelectedHeroes[1].gameObject);
        } 
        else if (SelectedHeroes[2] != null) {
            Destroy(SelectedHeroes[2].gameObject);
        } 

        if (SelectedEnemy != null) {
            Destroy(SelectedEnemy.gameObject);        
        } 
        EscapeExit = new Vector3(-1f, -1f, -1f);
        EscapeCount = 0;
        DeadHeroes = 0;
        CanEscape = false;
        SceneManager.LoadScene("Scenes/SampleScene");
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MCTS : MonoBehaviour 
{
    
    public static void Selection(PracticeNode currentNode) {
        if (currentNode.children.Count == 0) {
            Expansion(currentNode);

            var random = new System.Random();
            int randomMove = random.Next(0, currentNode.children.Count);

            var outcome = Simulation(currentNode.children[randomMove]);
            Backpropogation(currentNode.children[randomMove], outcome);
        } else {

            int bestChildIdx = BestChild(currentNode);

            Selection(currentNode.children[bestChildIdx]);
        }
    }
    
    public static int BestChild(PracticeNode currentNode) {
         // Find Best Child
        float greatestUCB = currentNode.children[0].ucbScore;
        int bestChildIdx = 0;

        for (int i = 1; i < currentNode.children.Count; ++i) {
            if (greatestUCB < currentNode.children[i].ucbScore) {
                greatestUCB = currentNode.children[i].ucbScore;
                bestChildIdx = i;
            }
        }
        return bestChildIdx;
    }

    public static int BestChild2(PracticeNode currentNode) {
         // Find Best Child
        float greatestUCB = currentNode.children[0].ucbScore;
        int bestChildIdx = 0;
        print(currentNode.children[0].ucbScore);

        for (int i = 1; i < currentNode.children.Count; ++i) {
            print(currentNode.children[i].ucbScore);
            if (greatestUCB < currentNode.children[i].ucbScore) {
                greatestUCB = currentNode.children[i].ucbScore;
                
                bestChildIdx = i;
            }
        }
        print("level");
        return bestChildIdx;
    }

    public static void Expansion(PracticeNode currentNode) {
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);

        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);
        
        State currentHeroState = currentNode.state;
        float currentX = currentHeroState.heroLocation.x;
        float currentY = currentHeroState.heroLocation.y;

        AllValidNeighbors(currentNode, hero2_Location, hero3_Location, currentX, currentY);
    }

    public static string Simulation(PracticeNode childNode) {
        Vector3 tmpHero = new Vector3(childNode.state.heroLocation.x, childNode.state.heroLocation.y, 0f);
        System.Collections.Generic.HashSet<Vector3> heroPast = new System.Collections.Generic.HashSet<Vector3>();
        heroPast.Add(tmpHero);


        Vector3 tmpEnemy = new Vector3(childNode.state.enemyLocation.x, childNode.state.enemyLocation.y, 0f);
        System.Collections.Generic.HashSet<Vector3> enemyPast = new System.Collections.Generic.HashSet<Vector3>();
        enemyPast.Add(tmpEnemy);

        bool startTurn = (childNode.turn == "hero") ? (true) : (false);
        bool tmpTurn = startTurn;
        int count = 0;

        /*while (tmpHero != tmpEnemy) {
            if (tmpTurn) {
                PlayerMove(tmpHero, heroPast);
            } else {
                //EnemyMove(tmpEnemy, enemyPast);
            }
            //tmpTurn = !tmpTurn;
            count += 1;

            if (count == 5) {
                break;
            }
        }*/

        while (count != 100) {
            if (tmpTurn) {
                PlayerMove(tmpHero, tmpEnemy);
            } else {
                EnemyMove(tmpEnemy);
            }

            if (tmpHero == tmpEnemy) {
                return (tmpTurn) ? ("hero") : ("dragon");
            }

            tmpTurn = !tmpTurn;
            count += 1;
        }

        //print(tmpTurn);
        float before = Heuristic(childNode.state.heroLocation, childNode.state.enemyLocation);
                
        float after = Heuristic(tmpHero, tmpEnemy);

        string winner;

        if (before > after) {
            winner = "hero";
        } else if (before < after) {
            winner = "dragon";
        } else {
            winner = "draw";
        }

        return winner;
    }

    public static void Backpropogation(PracticeNode childNode, string winner){
        PracticeNode parent = new PracticeNode();
        parent = childNode.parent;

        childNode.visitCount += 1;

        UCB(childNode.turn, winner, childNode, parent);
        //print(childNode.ucbScore);

        childNode = parent;

        while(childNode.parent != null) {
            childNode.visitCount += 1;

            // Update UCB Score
            if (childNode.turn == "hero") {
                UCB(childNode.turn, winner, childNode, childNode.parent);
            } else {
                UCB(childNode.turn, winner, childNode, childNode.parent);
            }
            //print(childNode.ucbScore);

            childNode = childNode.parent;
        }

        childNode.visitCount += 1;
        //print("done");
    }

    public static void UCB(string turn, string winner, PracticeNode currentNode, PracticeNode parent) {
        // If it is the hero turn, the lower the heuristic the better (they're closer to the dragon)
        // If it is the dragons turn, the higher the heuristic the better (they're closer to the dragon)
        int wi;
        if (winner == "draw") {
            wi = 0;
        } else if(turn == winner) {
            wi = 1;
        } else {
            wi = -1;
        }

        currentNode.value += wi;

        float exploitation = ((float)currentNode.value/currentNode.visitCount);
        //print(exploitation);
        float exploration = (float)(System.Math.Sqrt(2 * System.Math.Log((parent.visitCount+1)/currentNode.visitCount)));
        //print(System.Math.Log((parent.visitCount+1)/currentNode.visitCount));
        currentNode.ucbScore += (exploitation + exploration);
    }

    public static float Heuristic(Vector3 currentNode, Vector3 targetNode) {
        // Manhattans Distance
        //Eucliadian distance
        float difX = System.Math.Abs(targetNode.x - currentNode.x);
        float difY = System.Math.Abs(targetNode.y - currentNode.y);
        float mDistance = difX + difY;
        float diagnol = difX * difX + difY * difY;
        float eDistance = (float)System.Math.Sqrt(diagnol);
        return (float)(mDistance/23);
    }  

    public static void PlayerMove(Vector3 currentLocation, Vector3 targetLocation) {
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);
        
        float currentX = currentLocation.x;
        float currentY = currentLocation.y;

        var random = new System.Random();

        // Creating a dictionary where keys are integers and values are arrays of integers
        Dictionary<int, Vector2> directions = new Dictionary<int, Vector2>();

        // List of all Valid neighbors
        List<(Vector3, float)> possibleNeighbors = new List<(Vector3, float)>();

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
                float before = Heuristic(currentLocation, targetLocation);
                float after = Heuristic(tmpLocation, targetLocation);

                float tmpScore;
                
                if (before > after) {
                    tmpScore = .9f;
                } else if (before < after) {
                    tmpScore = .1f;
                } else {
                    tmpScore = .2f;
                }
                possibleNeighbors.Add((tmpLocation, tmpScore));
            }
        }

        //int move = random.Next(0, possibleNeighbors.Count);
        //currentLocation = possibleNeighbors[move].Item1;
        currentLocation = GetRandomValue(possibleNeighbors);
    }

    public static void EnemyMove(Vector3 currentLocation) {
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);
        
        float currentX = currentLocation.x;
        float currentY = currentLocation.y;

        var random = new System.Random();
        bool inBounds = false;

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
                (tile1.TileName != "Mountain")) {

                possibleNeighbors.Add(tmpLocation);
            }
        }

        int move = random.Next(0, possibleNeighbors.Count);
        currentLocation = possibleNeighbors[move];
    }

    public static Vector3 GetRandomValue(List<(Vector3, float)> weightedValueList)
    {
        Vector3 output = new Vector3(-1f, -1f, -1f);
 
        //Getting a random weight value
        float totalWeight = 0f;

        foreach (var entry in weightedValueList)
        {
            totalWeight += entry.Item2;
        }

        float rndWeightValue = Random.Range(1f, totalWeight + 1f);
 
        //Checking where random weight value falls
        float processedWeight = 0f;
        foreach (var entry in weightedValueList)
        {
            processedWeight += entry.Item2;
            if(rndWeightValue <= processedWeight)
            {
                output = entry.Item1;
                break;
            }
        }
 
        return output;
    }


    public static void AllValidNeighbors(PracticeNode parent, Vector3 hero2_Location, Vector3 hero3_Location, float currentX, float currentY) {
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

                PracticeNode aChild = new PracticeNode();

                // Set parent for this child
                aChild.parent = parent;
                //print(tmpLocation);

                // Set state for this child
                if (parent.turn == "hero") {
                    aChild.state.heroLocation = tmpLocation;
                    aChild.state.enemyLocation = parent.state.enemyLocation;
                    aChild.turn = "enemy";
                } else {
                    aChild.state.heroLocation = parent.state.heroLocation;
                    aChild.state.enemyLocation = tmpLocation;
                    aChild.turn = "hero";
                }

                parent.children.Add(aChild);
            }
        }
    }

    /*
    public static void PlayerMove(Vector3 currentLocation, System.Collections.Generic.HashSet<Vector3> pastMoves) {
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);
        
        var random = new System.Random();
        bool inBounds = false;

        while (!inBounds) {
            // Generate a random integer: 0 (move in x) or 1 (move in y)
            int direction = random.Next(0, 2);

            // Generate a random integer: 0 (move in down or right) or 1 (move up or left)
            int randomNum = random.Next(0, 2);
            float progress = (randomNum == 1) ? 1f : -1f;

            float currentY = currentLocation.y;
            float currentX = currentLocation.x;

            float tmp_y = currentLocation.y + progress;
            float tmp_x = currentLocation.x + progress;

            Vector3 tmp_final1 = new Vector3(currentX, tmp_y, 0f);
            Vector3 tmp_final2 = new Vector3(tmp_x, currentY, 0f);

            Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(currentX, tmp_y));
            Tile tile2 = GridManager.Instance.GetTileAtPosition(new Vector2(tmp_x, currentY));

            if (direction == 1) {
                if ((0 <= tmp_y && tmp_y <= 8) && 
                    (tmp_final1 != hero2_Location) &&
                    (tmp_final1 != hero3_Location) &&
                    (tile1.TileName != "Mountain")) {
                    currentLocation += new Vector3(0f, progress, 0f);
                    inBounds = true;
                    print("yes a");
                }
            } 
            else {
                if (0 <= tmp_x && tmp_x <= 15 && 
                    (tmp_final2 != hero2_Location) &&
                    (tmp_final2 != hero3_Location) &&
                    (tile2.TileName != "Mountain")) {
                    currentLocation += new Vector3(progress, 0f, 0f);
                    inBounds = true;
                    print("yes b");
                }
            }    
        }
        pastMoves.Add(currentLocation);
    }
    public Dictionary<string, float> args;
    
    public MCTS(Dictionary<string, float> arg) {
        args = arg;
    }

    public search() {
        MCTSNode root = MCTSNode(heroLocation, enemyLocation, args)
        for(int i = 0; i < args['num_searches']) {
            MCTSNode node = root;

            while (node.is_fully_expanded()) {
                node = node.select();
            }

            
        }
    }*/
    /*public Vector3 heroLocation = UnitManager.Instance.SelectedHeroes[0];
    public Vector3 enemyLocation = UnitManager.Instance.SelectedEnemy;
    public MCTS parentMCTS = null; 
    public Vector3 parentAction = null;
    public List<MCTS> children;
    public int number_Of_visits = 0;
    public Dictionary<int, int> results;
    public Dictionary<Vector3, Node> state = new Dictionary<Vector3, Node>(); 
 
    List<Vector3> untriedActions = untried_actions();
    public bool heroTurn;


    public static List<Vector3> untried_actions() {
        if((GameManager.Instance.GameState != GameState.HeroesTurn) || (currentHero == null)) {
            GameManager.Instance.ChangeState(GameState.Heroes2Turn);
            //return new Dictionary<int, Vector2>();
            return new List<Vector3>();
        }

        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);


        float currentY = heroLocation.y;
        float currentX = heroLocation.x;
        
        return AStar.AllValidNeighbors(hero2_Location, hero3_Location, currentX, currentY);
    }

    public List<Vector3> AllValidNeighbors(Vector3 hero2_Location, Vector3 hero3_Location, float currentX, float currentY) {
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

    public int GetQ() {
        int wins = results[1];
        int loses = results[-1];
        
        return win - loses;
    }

    public int GetN() {
        return number_Of_visits;
    }

    public MCTS expand(MCTS parent) {
        if (untriedActions.Count > 0) {
            Vector3 action = untriedActions[0];
            untriedActions.RemoveAt(0);
            
            Vector3 nextHeroLocation = heroLocation;
            Vector3 nextEnemyLocation = enemyLocation;

            if (!heroTurn) {
                nextHeroLocation = action;
            } else {
                nextEnemyLocation = action;
            }

            childMCST = new MCTS();
            childMCST.heroLocation = nextHeroLocation;
            childMCST.enemyLocation = nextEnemyLocation;
            childMCST.parentMCTS = parent;
            parentAction = action;
            children.Add(childMCST);
        }
        return null;
    }

    public is_terminal_node() {
        return is_game_over();
    }

    public rollout() {
        bool tmpHeroTurn = heroTurn;
        Vector3 tmpHeroLocation = heroLocation;
        Vector3 tmpEnemyLocation = enemyLocation;

        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);

        while (tmpHeroLocation != tmpEnemyLocation) {
            float currentY = tmpHeroLocation.y;
            float currentX = tmpHeroLocation.x;
            List<Vector3> possibleMoves = AllValidNeighbors(hero2_Location, hero3_Location, currentX, currentY);

            Vector3 action = rollout_policy(possibleMoves);

            if (!heroTurn) {
                nextHeroLocation = action;
            } else {
                nextEnemyLocation = action;
            }
        }
    }*/


    /*public static void LostGame(bool enemyTurn) {
        return;
    }

    public static void Move(Vector3 unit, Vector3 newMove) {
        unit = newMove;
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTSNode : MonoBehaviour
{
    //Game

    //args
    /*public Dictionary<string, float> args;

    //state
    public Vector3 heroLocation;
    public Vector3 enemyLocation;

    //Parent
    public MCTSNode parent;

    //Action
    public Vector3 action_taken;

    //Children
    public Dictionary<Vector3, MCTSNode> children;

    public Dictionary<Vector3, int> expandable_moves;

    public int visit_count = 0;
    public int value_sum = 0;

    public MCTSNode(Vector3 hero, Vector3 enemy, Dictionary<string, float> arg, MCTSNode parentNode = null, Vector3 action = null) {
        heroLocation = hero;
        enemyLocation = enemy;
        parent = parentNode;
        action_taken = action;
        args = arg;
        expandable_moves = get_valid_moves();
    }

    public bool is_fully_expanded() {
        return (expandable_moves.Count == 0 && children.Count == 0);
    }

    public MCTSNode select() {
        MCTSNode best_child = null;
        float best_ucb = float.NegativeInfinity;

        foreach(var child in children.Values) {
            //float ucb = get_ucb(child);
            float ucb = 0;

            if (ucb > best_ucb) {
                best_child = child;
                best_ucb = ucb;
            }
        }
        //return (best_child, best_ucb);
        return best_child;
    }

    public float get_ucb(MCTSNode child) {
        float q_value = 1 - ((child.value_sum / child.visit_count) + 1) / 2;

        return args["C"] * (float)System.Math.Sqrt(System.Math.Log(visit_count) / visit_count);
    }

    public Vector3 Selection() {
        List<int> visit_counts = new List<int>();
        List<Vector3> actions = new List<Vector3>();

        foreach(var child in childStates.Values) {
            visit_counts.Add(child.visitCount);
        }

        foreach(var move in childStates.Keys) {
            actions.Add(move);
        }

        Vector3 action;

        if (temperature == 0) {
            // Find the maximum value in the list
            int max = visit_counts.Max();

            // Find the index of the maximum value
            int maxIndex = visit_counts.IndexOf(max);
            action = actions[maxIndex];
        } else if (temperature == float.PositiveInfinity){
           var random = new System.Random();
           int random_idx = random.Next(0, actions.Count);

           action = actions[random_idx];

        }else {
            List<float> distribution = new List<float>();
            float sum = 0;
            var random = new System.Random();

            for(int i = 0; i < visit_counts.Count; ++i) {
                float num = (float)System.Math.Pow(visit_counts[i], (1/temperature)); 
                distribution.Add(num);
                sum += num;
            }

            for(int i = 0; i < visit_counts.Count; ++i) {
                float num = (float)(distribution[i]/sum); 
                visit_counts[i] = num;
                sum += num;
            }

            int random_idx = random.Next(0, distribution.Count);

            action = actions[random_idx];

        }

        return action;
    }

    public List<Vector3> get_valid_moves() {
        BaseUnit currentHero = UnitManager.Instance.SelectedHeroes[0];

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
    }*/

}

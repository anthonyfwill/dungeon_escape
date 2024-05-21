using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State1
{
    public int visitCount = 0;
    //toplay
    public int turn = 1;
    public float prior;
    public int valueSum = 0;
    //public State parentState;
    public float ucbScore = 0;
    //public Dictionary<Vector3, State> childStates;
    //state
    public Vector3 heroLocation;
    public Vector3 enemyLocation;

    /*public float StateValue() {
        if (visitCount == 0) {
            return 0;
        }
        return valueSum / visitCount;
    }

    public float CalculateUCB(State child) {
        float prior_score = child.prior * (float)System.Math.Sqrt(parentState.visitCount) / (child.visitCount + 1);
        float value_score = 0;

        if (child.visitCount > 0) {
            value_score = -child.StateValue();
        } else {
            value_score = 0;
        }

        return value_score + prior_score;
    }

    public bool IsExpanded() {
        return childStates.Count > 0;
    }

    public List<Vector3> ExpandStates() {
        UnitManager.Instance.heroPath.Clear();

        BaseUnit currentHero = UnitManager.Instance.SelectedHeroes[0];

        if((GameManager.Instance.GameState != GameState.HeroesTurn) || (currentHero == null)) {
            GameManager.Instance.ChangeState(GameState.Heroes2Turn);
            return new List<Vector3>();
        }

        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero2_Location = (UnitManager.Instance.SelectedHeroes[1] != null) ? (UnitManager.Instance.SelectedHeroes[1].transform.position) : (dummy_location);
        Vector3 hero3_Location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);

        BaseHero currentHeroBH = (BaseHero)currentHero;

        Vector3 tmpLocation = currentHeroBH.transform.position;
        float currentY = tmpLocation.y;
        float currentX = tmpLocation.x;
        
        List<Vector3> possibleMoves = AStar.AllValidNeighbors(hero2_Location, hero3_Location, currentX, currentY);

        foreach(Vector3 move in possibleMoves) {
            children
        }
    }

    public List<Vector3> AllValidNeighbors(Vector3 hero2_Location, Vector3 hero3_Location, float currentX, float currentY) {
        // Creating a dictionary where keys are integers and values are arrays of integers
        Dictionary<int, Vector3> directions = new Dictionary<int, Vector3>();

        // Adding elements to the dictionary
        directions.Add(0, new Vector3(-1f,0f,0f));
        directions.Add(1, new Vector3(1f,0f,0f));
        directions.Add(2, new Vector3(0f,-1f,0f));
        directions.Add(3, new Vector3(0f,1f,0f));

        foreach(var keyValue in directions) {
            Vector3 direction = keyValue.Value;
            float tmpX = currentX + direction.x;
            float tmpY = currentY + direction.y;
            Vector3 tmpLocation = new Vector3(tmpX, tmpY, 0f);
            Tile tile1 = GridManager.Instance.GetTileAtPosition(new Vector2(tmpX, tmpY));

            if ((0 <= tmpY && tmpY <= 8) && 
                (0 <= tmpX && tmpX <= 15) &&
                (tmpLocation != hero2_Location) &&
                (tmpLocation != hero3_Location) &&
                (tile1.TileName != "Mountain")) {
                State child = new State();
                child.turn *= -1;
                child.prior = 0;
                childStates[direction] = child;
            }
        }

        return possibleNeighbors;
    }

    public Vector3 Selection(float temperature) {
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

            /*for(int i = 0; i < visit_counts.Count; ++i) {
                float num = (float)(distribution[i]/sum); 
                visit_counts[i] = num;
                sum += num;
            }

            int random_idx = random.Next(0, distribution.Count);

            action = actions[random_idx];

        }

        return action;
    }

    public (Vector3, State) SelectChild() {
        float best_score = float.NegativeInfinity;
        Vector3 best_action = new Vector3(-1f, -1f,-1f);
        State best_child = null;

        foreach(Vector3 action in childStates.Keys) {
            float score = CalculateUCB(childStates[action]);

            if (score > best_score) {
                best_score = score;
                best_action = action;
                best_child = childStates[action];
            }
        }

        return (best_action, best_child);
    }*/


}

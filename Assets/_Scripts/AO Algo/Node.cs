using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public int GCost;
    public int HCost;
    public int FCost;
    public int x;
    public int y;

    public Node ParentNode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public bool Expanded() {
        return children.Count > 0;
    }

    public double Value() {
        if (visit_count == 0) {
            return 0;
        }
        return (value_sum / visit_count);
    }

    public void SelectAction(float temperature) {
        List<int> visitCounts = new List<int>();

        foreach(var child in children.Values) {
            visitCounts.Add(child.visit_count);
        }

        List<Vector3> actions = new List<Vector3>();

        foreach(var move in children.Keys) {
            actions.Add(move);
        }

        int action; 

        if (temperature == 0) {
            action = visitCounts.Max();
        } else if (temperature == float.PositiveInfinity){
           var random = new System.Random();
           int random_idx = random.Next(0, actions.Count);

           //action = actions[random_idx];

        }/* else {
            float distribution = System.Math.Pow(visit_count, (1/temperature)); 
            distribution = distribution / 
        }'''

        
    }

    public float UCBScore() {
        pastScore = prior * System.Math.Sqrt(ParentNode.visit_count) / (visit_count + 1);
        float valueScore;

        if (visit_count > 0) {
            //valueScore = 
            return 0;
        }
    }*/
    
}

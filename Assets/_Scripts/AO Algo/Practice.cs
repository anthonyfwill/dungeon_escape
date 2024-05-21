using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PracticeNode 
{
    public State state = new State();
    public PracticeNode parent;
    public List<PracticeNode> children = new List<PracticeNode>();
    public Vector3 previousAction = new Vector3(0f, 0f, 0f);
    public int visitCount = 0;
    public int value = 0;
    public float ucbScore = 0f;
    public string turn;

    public PracticeNode(PracticeNode parentNode = null) {
        parent = parentNode;
        //action = newAction;
    }

    /*public int BestChild() {
         // Find Best Child
        float greatestUCB = children[0].ucbScore;
        int bestChildIdx = 0;

        for (int i = 1; i < children.Count; ++i) {
            if (greatestUCB < children[i].ucbScore) {
                greatestUCB = children[i].ucbScore;
                bestChildIdx = i;
            }
        }
        return bestChildIdx;
    }*/

    /*public float StateValue() {
        if (visitCount == 0) {
            return 0;
        }
        return valueSum / visitCount;
    }*/

    /*public float CalculateUCB(PracticeNode child) {
        float prior_score = child.prior * (float)System.Math.Sqrt(parent.visitCount) / (child.visitCount + 1);
        float value_score = 0;

        return prior_score;
    }*/

}

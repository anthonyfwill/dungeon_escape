using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using C5; 

public class MCTSMove : MonoBehaviour
{

    public static void PlayMCTS() 
    {
        if(UnitManager.Instance.SelectedHeroes[0] != null)
        {
            PracticeNode current = new PracticeNode();

            // Set parent for this child
            current.parent = null;

        
            current.state.heroLocation = UnitManager.Instance.SelectedHeroes[0].transform.position;
            current.state.enemyLocation = UnitManager.Instance.SelectedEnemy.transform.position;
            current.turn = "hero";

            for (int iterations = 0; iterations < 1000; ++iterations) {
                MCTS.Selection(current);
            }

            PracticeNode bestNode = new PracticeNode();
            bestNode = current.children[MCTS.BestChild2(current)];

            BaseUnit currentHero = UnitManager.Instance.SelectedHeroes[0];
            BaseHero currentHeroBH = (BaseHero)currentHero;

            currentHeroBH.transform.position = bestNode.state.heroLocation;

            if (currentHeroBH.transform.position == UnitManager.Instance.SelectedEnemy.transform.position) {
                Destroy(UnitManager.Instance.SelectedEnemy.gameObject);
                print("Hero1 killed enemy");
                GameManager.Instance.Monte_total_kills++;
                UnitManager.Instance.CanEscape = true;
                UnitManager.Instance.SpawnExit();
            }

            
        }
        GameManager.Instance.ChangeState(GameState.Heroes2Turn);
    }
}


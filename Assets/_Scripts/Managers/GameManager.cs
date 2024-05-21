using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public static int heroTurn;

    private int Monte_total_moves;
    private int Nash_total_moves;
    public int RL_total_moves;

    public int Monte_total_deaths;
    public int Nash_total_deaths;
    public int RL_total_deaths;

    public int Monte_total_kills;
    public int Nash_total_kills;
    public int RL_total_kills;

    public int win;
    public int loss;

    void Awake()
    {
        Instance = this;
        heroTurn = 1;
    }

    void Start()
    {
        ChangeState(GameState.GenerateGrid);
        Monte_total_moves = 0;
        Nash_total_moves = 0;
        RL_total_moves = 0;
        Monte_total_deaths = 0;
        Nash_total_deaths = 0;
        RL_total_deaths = 0;
        Monte_total_kills = 0;
        Nash_total_kills = 0;
        RL_total_kills = 0;
        win = 0;
        loss = 0;
    }
    public void HeroMoves()
    {
        //print("Hero 1 Turn");
        AStar.AStarSearch();
        Invoke("HeroMove", .3f);
    }
    public void HeroMove()
    {
        Monte_total_moves++;
        AStar.HeroMove();
    }
    public void Hero2Moves()
    {
        
    }
    public void Hero3Moves()
    {
        Nash_total_moves++;
        print("Hero 3 Turn");
        //UnitManager.Instance.PlayerMove();
        MiniMax.PlayerMove();
    }
    public void enemyMoves()
    {
        print("Enemy Turn");
        //UnitManager.Instance.EnemyMove();
        if (UnitManager.Instance.SelectedEnemy.miniMax)
        {
            
            EnemyMiniMax.PlayerMove();
        }
        else
        {
            UnitManager.Instance.EnemyMove();
        }
    }
    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.GenerateGrid:
               // print("Generate Grid");
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnHeroes:
                //print("Spawn hero");
                UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                //print("Generate enemy");
                UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.HeroesTurn:
                
                //UnitManager.Instance.PlayerMove();
                Invoke("HeroMoves", 0);
                break;
            case GameState.Heroes2Turn:
                if(UnitManager.Instance.SelectedHeroes[1] == null)
                {
                    ChangeState(GameState.Heroes3Turn);
                }
                //Invoke("Hero2Moves", .1f);
                print("HERO 2 TURN-----------------------");
                break;
            case GameState.Heroes3Turn:
                print("HERO 3 TURN-----------------------");
                Invoke("Hero3Moves", .2f);
                MiniMax.PlayerMove();
                break;
            case GameState.EnemiesTurn:
                
                Invoke("enemyMoves", 0);
                break;
            case GameState.LostGame:
                UnitManager.Instance.ResetGame();
                GameManager.Instance.ChangeState(GameState.GenerateGrid);
                break;    
            case GameState.WonGame:
                UnitManager.Instance.ResetGame();
                GameManager.Instance.ChangeState(GameState.GenerateGrid);
                break; 
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void WriteCSV() 
    {
        /*TextWriter tw = new StreamWriter("/Volumes/Main/Unity/1s DungeonEscape/newD/dungeonEscapeMonteNashRL.csv", true);
        tw.WriteLine(Monte_total_moves + "," + RL_total_moves + "," + Nash_total_moves + "," 
        + Monte_total_deaths + "," + RL_total_deaths + "," + Nash_total_deaths + "," + win + "," + loss + "," 
        + Monte_total_kills + "," + RL_total_kills + "," + Nash_total_kills);
        tw.Close();*/

    }
}

public enum GameState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    HeroesTurn = 3,
    Heroes2Turn = 4,
    Heroes3Turn = 5,
    EnemiesTurn = 6,
    LostGame = 7,
    WonGame = 8
}
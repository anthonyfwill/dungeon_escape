using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using UnityEngine.Events;


public class Hero3Agent : Agent
{

    [SerializeField] private Transform targetEnemy1;

    [SerializeField] private Transform targetEnemy2;

    private const int width = 16;
    private const int height = 9;

    private int[,] EnemyGrid;
    private int[,] MountainGrid;

    //public VoidEventChannelSO RequestMoveChannel;

    private bool is_UP_valid = false;
    private bool is_DOWN_valid = false;
    private bool is_RIGHT_valid = false;
    private bool is_LEFT_valid = false;

    private Vector3 previous_move1;
    private Vector3 previous_move2;
    private Vector3 previous_move3;
    private Vector3 previous_move4;
    private Vector3 previous_move5;

    //VectorSensorComponent m_GoalSensor;
    Vector3 dummy_location = new Vector3(-1f, -1f, -1f);

    private bool won_game;
    private int number_of_moves;

    //BaseUnit Current_Hero = UnitManager.Instance.SelectedHeroes[1];
    

    // Start is called before the first frame update #f3007f


    public override void OnEpisodeBegin()
    {
        Create_One_Hot_Grid();
        //Create_One_Hot_Grid2();
        previous_move1 = new Vector3(-1f, -1f, -1f);
        previous_move2 = new Vector3(-1f, -1f, -1f);
        previous_move3 = new Vector3(-1f, -1f, -1f);
        previous_move4 = new Vector3(-1f, -1f, -1f);
        number_of_moves = 0;
        won_game = false;

    }


    public override void Initialize()
    {
        
    }

    private void Start()
    {
        //print("Start");
        Academy.Instance.AutomaticSteppingEnabled = false;
    }

    private void Update()
    {
        if(number_of_moves > 1000)
        {
            //this.EndEpisode();
            UnitManager.Instance.ResetGame();
        }
    }

    private void FixedUpdate()
    {
        if(!UnitManager.Instance.valid_game)
        {
            //this.EndEpisode();
            //UnitManager.Instance.ResetGame();
        }
        Academy.Instance.EnvironmentStep();
        if(GameManager.Instance.GameState == GameState.Heroes2Turn)
        {
            MakeMove();
        }
    }

    void OnDestroy()
    {
        if(!won_game)
        {
            AddReward(-5f);
        }
        EndEpisode();
        print("Destroyed");
        GameManager.Instance.ChangeState(GameState.Heroes3Turn);
        //UnitManager.Instance.ResetGame();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //print("CollectObservations");

        BaseUnit Current_Hero = UnitManager.Instance.SelectedHeroes[1];

        sensor.AddObservation(Current_Hero.transform.position);
        //sensor.AddOneHotObservation(EnemyGrid);

        if(UnitManager.Instance.SelectedEnemy != null)
        {
            sensor.AddObservation(UnitManager.Instance.SelectedEnemy.transform.position);
        }
        else if(UnitManager.Instance.CanEscape)
        {
            //sensor.AddObservation(UnitManager.Instance.EscapeExit);
            sensor.AddObservation(UnitManager.Instance.EscapeExit);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                sensor.AddOneHotObservation(EnemyGrid[x, y], 1);
            }
        }

/*         for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                sensor.AddOneHotObservation(GridManager.Instance.MountainGrid[x, y], 1);
            }
        } */
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        //actionMask.SetActionEnabled(branch, actionIndex, isEnabled);
        
        //BaseUnit Current_Hero = UnitManager.Instance.SelectedHeroes[1];

        //print("WriteDiscreteActionMask");
        Vector3 dummy_location = new Vector3(-1f, -1f, -1f);
        Vector3 hero1_Location = (UnitManager.Instance.SelectedHeroes[0] != null) ? (UnitManager.Instance.SelectedHeroes[0].transform.position) : (dummy_location);
        Vector3 hero3_location = (UnitManager.Instance.SelectedHeroes[2] != null) ? (UnitManager.Instance.SelectedHeroes[2].transform.position) : (dummy_location);

        BaseUnit Current_Hero = UnitManager.Instance.SelectedHeroes[1];
        float currentY = Current_Hero.transform.position.y;
        float currentX = Current_Hero.transform.position.x;
        //print($"{currentX}, {currentY}");
        //print("MASK CALLED");

        is_UP_valid = false;
        is_DOWN_valid = false;
        is_RIGHT_valid = false;
        is_LEFT_valid = false;

        float Y_up = currentY + 1f;
        float Y_down = currentY - 1f;
        float X_right = currentX + 1f;
        float X_left = currentX - 1f;

        Vector3 UP_pos = new Vector3(currentX, Y_up, 0f);
        Vector3 DOWN_pos = new Vector3(currentX, Y_down, 0f);
        Vector3 RIGHT_pos = new Vector3(X_right, currentY, 0f);
        Vector3 LEFT_pos = new Vector3(X_left, currentY, 0f);

        //Tile tile_UP = GridManager.Instance.GetTileAtPosition(new Vector2((int)currentX, (int)Y_up));
        //Tile tile_DOWN = GridManager.Instance.GetTileAtPosition(new Vector2((int)currentX, (int)Y_down));
        //Tile tile_RIGHT = GridManager.Instance.GetTileAtPosition(new Vector2((int)X_right, (int)currentY));
        //Tile tile_LEFT = GridManager.Instance.GetTileAtPosition(new Vector2((int)X_left, (int)currentY));

        //bool tile_UP_valid = GridManager.Instance.GetTileAtPosition(new Vector2((int)currentX, (int)Y_up))._isWalkable;
        //bool tile_DOWN_valid = GridManager.Instance.GetTileAtPosition(new Vector2((int)currentX, (int)Y_down))._isWalkable;
        //bool tile_RIGHT_valid = GridManager.Instance.GetTileAtPosition(new Vector2((int)X_right, (int)currentY))._isWalkable;
        //bool tile_LEFT_valid = GridManager.Instance.GetTileAtPosition(new Vector2((int)X_left, (int)currentY))._isWalkable;

        
        //(tile_UP.TileName != "Mountain") &&
        //(tile_DOWN.TileName != "Mountain") &&
        //(tile_RIGHT.TileName != "Mountain") &&
        //(tile_LEFT.TileName != "Mountain") &&
        

        if((Y_up <= 8) && (UP_pos != hero1_Location) && (UP_pos != hero3_location) && (Y_up >= 0))
        {
            if(GridManager.Instance.GetTileAtPosition(new Vector2((int)currentX, (int)Y_up))._isWalkable)
            {
                is_UP_valid = true;
            }
        }
        if((Y_down <= 8) && (DOWN_pos != hero1_Location) && (DOWN_pos != hero3_location) &&  (Y_down >= 0))
        {
            if(GridManager.Instance.GetTileAtPosition(new Vector2((int)currentX, (int)Y_down))._isWalkable)
            {
                is_DOWN_valid = true;
            }
        }
        if((X_right <= 15) && (RIGHT_pos != hero1_Location) && (RIGHT_pos != hero3_location) &&  (X_right >= 0))
        {
            if(GridManager.Instance.GetTileAtPosition(new Vector2((int)X_right, (int)currentY))._isWalkable)
            {
                is_RIGHT_valid = true;
            }
        }
        if((X_left <= 15) && (LEFT_pos != hero1_Location) && (LEFT_pos != hero3_location) &&  (X_left >= 0))
        {
            if(GridManager.Instance.GetTileAtPosition(new Vector2((int)X_left, (int)currentY))._isWalkable)
            {
                is_LEFT_valid = true;
            }
        }

        if(!is_UP_valid)
        {
            //print("up false");
            actionMask.SetActionEnabled(0, 0, false);
        }
        if(!is_DOWN_valid)
        {
            //print("down false");
            actionMask.SetActionEnabled(0, 1, false);
        }
        if(!is_RIGHT_valid)
        {
            //print("right false");
            actionMask.SetActionEnabled(0, 2, false);
        }
        if(!is_LEFT_valid)
        {
            //print("left false");
            actionMask.SetActionEnabled(0, 3, false);
        }

        if((!is_UP_valid) && (!is_DOWN_valid) && (!is_RIGHT_valid) && (!is_LEFT_valid))
        {
            //this.EndEpisode();
            //UnitManager.Instance.ResetGame();
        }

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //print("OnActionReceived");
        int direction = actions.DiscreteActions[0];
        
        BaseUnit Current_Hero = UnitManager.Instance.SelectedHeroes[1];
        previous_move5 = previous_move4;       
        previous_move4 = previous_move3;
        previous_move3 = previous_move2;
        previous_move2 = previous_move1;
        previous_move1 = Current_Hero.transform.position;


        if((direction == 0))
        {
            Current_Hero.transform.position += new Vector3(0f, 1f, 0f);
        }
        if((direction == 1))
        {
            Current_Hero.transform.position += new Vector3(0f, -1f, 0f);
        }
        if((direction == 2))
        {
            Current_Hero.transform.position += new Vector3(1f, 0f, 0f);
        }
        if((direction == 3))
        {
            Current_Hero.transform.position += new Vector3(-1f, 0f, 0f);
        }
        if((direction == 4))
        {
            this.transform.position = this.transform.position;
            AddReward(-3f);
        }

        float currentX = Current_Hero.transform.position.x;
        float currentY = Current_Hero.transform.position.y;

        //print($"unt: {UnitManager.Instance.SelectedEnemy.transform.position} --- Enemy1: {targetEnemy1.transform.position}");
        
        if(UnitManager.Instance.SelectedEnemy != null)
        {
            if(UnitManager.Instance.SelectedEnemy.transform.position == Current_Hero.transform.position)
            //if(Current_Hero.transform.position == targetEnemy1.transform.position)
            {
                AddReward(25f);
                print("Player kills");
                Destroy(UnitManager.Instance.SelectedEnemy.gameObject);
                GameManager.Instance.RL_total_kills++;
                UnitManager.Instance.CanEscape = true;
                UnitManager.Instance.SpawnExit();
            }
            else if(Current_Hero.transform.position == previous_move1)
            {
                AddReward(-9f);
                //print("-1f Penalty");
            }
            else if(Current_Hero.transform.position == previous_move2)
            {
                AddReward(-8f);
                //print("move2 Penalty");
            }
            else if(Current_Hero.transform.position == previous_move3)
            {
                AddReward(-7f);
                //print("move3 Penalty");
            }
            else if(Current_Hero.transform.position == previous_move4)
            {
                AddReward(-6f);
                //print("move4 Penalty");
            }
            else if(Current_Hero.transform.position == previous_move5)
            {
                AddReward(-5f);
                //print("move4 Penalty");
            }            

            BaseEnemy Current_Enemy = UnitManager.Instance.SelectedEnemy;

            float enemyX = Current_Enemy.transform.position.x;
            float enemyY = Current_Enemy.transform.position.y;

            Set_One_Hot_Grid_To_False();
            Set_One_Hot_Grid_To_True((int)enemyX, (int)enemyY); //Enemy
            Set_One_Hot_Grid_To_True((int)currentX, (int)currentY); //Hero

            //print($"ACTION: {currentX}, {currentY}");
            //print($"THIS.TRANSFORM: {this.transform.position}");
        }
        
        if(UnitManager.Instance.CanEscape)
        {
            if(Current_Hero.transform.position == UnitManager.Instance.EscapeExit) //targetEnemy2.transform.position) 
            {
                AddReward(30f);
                print(Current_Hero.transform.position);
                Destroy(Current_Hero.gameObject);
                print("I Escaped");
                UnitManager.Instance.EscapeCount += 1;
                if (UnitManager.Instance.EscapeCount + UnitManager.Instance.DeadHeroes == 3) 
                {
                    this.EndEpisode();
                    UnitManager.Instance.ResetGame();
                }

                won_game = true;

                this.EndEpisode();
                UnitManager.Instance.ResetGame();
            }
            else if(Current_Hero.transform.position == previous_move1)
            {
                AddReward(-9f);
                //print("-1f Penalty");
            }
            else if(Current_Hero.transform.position == previous_move2)
            {
                AddReward(-8f);
                //print("move2 Penalty");
            }
            else if(Current_Hero.transform.position == previous_move3)
            {
                AddReward(-7f);
                //print("move3 Penalty");
            }
            else if(Current_Hero.transform.position == previous_move4)
            {
                AddReward(-6f);
                //print("move4 Penalty");
            }
            else if(Current_Hero.transform.position == previous_move5)
            {
                AddReward(-5f);
                //print("move4 Penalty");
            }

            //print($"ESCAPE EXIT: {UnitManager.Instance.EscapeExit}");
            float ExitX = UnitManager.Instance.EscapeExit.x;
            float ExitY = UnitManager.Instance.EscapeExit.y;
            Set_One_Hot_Grid_To_False();

            if(UnitManager.Instance.EscapeExit != dummy_location)
            {
                Set_One_Hot_Grid_To_True((int)ExitX, (int)ExitY);
            }
            Set_One_Hot_Grid_To_True((int)currentX, (int)currentY);

            
            //print($"ExitPortal: {UnitManager.Instance.portal.transform.position}");
            //print($"Enemy2 Portal Position {targetEnemy2.transform.position}");
        }

        AddReward(-0.01f);
                
        number_of_moves++;
        GameManager.Instance.RL_total_moves++;
        //GameManager.Instance.RL_total_moves++;
        print($"Number of moves: {number_of_moves}");

        if(UnitManager.Instance.SelectedHeroes[2] != null)
        {
            GameManager.Instance.ChangeState(GameState.Heroes3Turn);
        }
        else if(UnitManager.Instance.SelectedEnemy != null)
        {
            GameManager.Instance.ChangeState(GameState.EnemiesTurn);
        }
        else if(UnitManager.Instance.SelectedHeroes[0] != null)
        {
            GameManager.Instance.ChangeState(GameState.HeroesTurn);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.Heroes2Turn);
        }

    }

/*         if((UnitManager.Instance.CanEscape) && ) 
        {
            AddReward(25f);
            print(Current_Hero.transform.position);
            Destroy(Current_Hero.gameObject);
            print("I Escaped");
            UnitManager.Instance.EscapeCount += 1;
            if (UnitManager.Instance.EscapeCount + UnitManager.Instance.DeadHeroes == 3) 
            {
                GameManager.Instance.ChangeState(GameState.EnemiesTurn);
            }
            this.EndEpisode();
            UnitManager.Instance.ResetGame();
        }
        //GameManager.Instance.ChangeState(GameState.EnemiesTurn);
    } */

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKeyDown(KeyCode.W))
        {
            discreteActionsOut[0] = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            discreteActionsOut[0] = 3;
        }
    }

    public void Create_One_Hot_Grid()
    {
        // Create a 2D array of boolean values
        EnemyGrid = new int[width, height];

        // Populate the grid with boolean values (for demonstration, we'll set all values to true)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                EnemyGrid[x, y] = 0; // Set all values to true (you can modify this as needed)
            }
        }
    }

    public void Set_One_Hot_Grid_To_False()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                EnemyGrid[x, y] = 0;
            }
        }
    }

    public void Set_One_Hot_Grid_To_True(int xIndex, int yIndex)
    {
        EnemyGrid[xIndex, yIndex] = 1;
    }





    public void MakeMove()
    {
        RequestDecision();
        //GameManager.Instance.ChangeState(GameState.EnemiesTurn);
    }
}

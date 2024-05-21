# Dungeon Escape
 76100 Final Project: Dungeon Escape Game in Unity 2023.2.17f1

Using Minimax, Nash, A*, MonteCarlo Search, and ML to control agents in a dungeon environment to kill the dragon and find the exit.

Install Unity using Version 2023.2.17f1

## Overview

**Dungeon Escape** is an immersive AI-driven Unity game where multiple knight agents attempt to escape a dungeon while being pursued by a formidable dragon. Each knight must acquire a key to unlock the dungeon door leading to their freedom. The game showcases various AI strategies, with each knight agent implementing different AI techniques: Reinforcement Learning, Nash Equilibrium, Minimax, Monte Carlo Search, and A*. The dragon, controlled by its own AI, aims to thwart the knights' escape.

## Goals

- **Escape the Dungeon:** The primary objective for each knight agent is to escape the dungeon by acquiring a key and unlocking the exit door.
- **Avoid or Defeat the Dragon:** Knights can choose to defeat the dragon to make their escape easier or avoid it entirely.
  
## Team Member Roles

### Anthony Williams
- **Agent Development:**
  - A* Algorithm
  - Monte Carlo Search Trees
- **Heuristic-Based Simulations:**
  - Implement heuristics to guide simulations and improve decision-making.
- **Decision Making/Machine Learning:**
  - Integrate machine learning techniques for agent decision-making.
- **Unity Environment:**
  - Build and design the game environment within Unity.
- **Trials:**
  - Conduct trials to test agent performance.
- **Visualization Metrics:**
  - Create visual representations of performance metrics.

### Fourcan Abdullah
- **Adversarial Search Development:**
  - Implement Minimax algorithm for knight agents.
  - Develop collaboration agent using Nash Equilibrium.
- **Dragon AI:**
  - Implement Minimax algorithm for the dragon agent.
- **Pathfinding:**
  - Use A* algorithm for agents to find and exit the portal.
- **Unity Environment:**
  - Assist in building and designing the game environment.
- **Heuristic Search:**
  - Integrate heuristic search methods for enhanced decision-making.
- **Visualization Metrics:**
  - Develop visualizations to represent performance data.

### Eugene Sajor
- **Reinforcement Learning Agent:**
  - Develop a deep reinforcement learning agent for knight behavior.
- **Unity Environment:**
  - Contribute to the construction and design of the game environment.
- **Training and Trials:**
  - Run training sessions and trials to refine agent behavior.
- **Evaluation Metrics:**
  - Create and implement metrics to evaluate agent performance.
- **Visualization Metrics:**
  - Design visual tools to showcase evaluation metrics.

## Game Mechanics

1. **Knights and Dragon:**
   - Each knight agent has a unique AI strategy.
   - The dragon agent operates to prevent knights from escaping.
2. **Objective:**
   - Knights must find a key to unlock the exit door and escape.
3. **Strategies:**
   - Knights may collaborate, defeat the dragon, or avoid it.
4. **Environment:**
   - A dungeon environment designed in Unity.

## AI Techniques

- **A Star Search Algorithm:** Pathfinding and shortest path calculation for knights.
- **Monte Carlo Search Trees:** Simulation-based decision-making for knights.
- **Minimax Algorithm:** Adversarial strategy for knights and dragon.
- **Nash Equilibrium:** Collaborative strategy for knights.
- **Reinforcement Learning:** Adaptive learning and behavior modeling for knights.

# Project Setup Instructions

## 1. Create a new 2D project

## 2. Install NuGetForUnity package
1. Open the project.
2. Go to **Window -> Package Manager**.
3. Click the **+** sign.
4. Select **Install package from Git URL**.
5. Paste this link into the prompt: `https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity`.

## 3. Download and install the ML-Agents package
1. Go to this link: [ML-Agents Release 21](https://github.com/Unity-Technologies/ml-agents/releases/tag/release_21).
2. Download **Source code (zip)** at the bottom of the page.
3. Unzip the file.
4. Open the project.
5. Go to **Window -> Package Manager**.
6. Click the **+** sign.
7. Select **Install package from disk**.
8. Navigate to the unzipped file.
9. Go to the folder **com.unity.ml-agents**.
10. Select **package.json** to install.

## 4. Install C5 and Newtonsoft.Json packages via NuGet
1. In the project, go to **NuGet**.
2. Search for **Newtonsoft.Json**.
3. Install **Newtonsoft.Json**.
4. Search for **C5**.
5. Install **C5**.

## 5. Handle errors
1. If any errors occur, comment out the files with those errors.

## 6. Set the slider in Snorelax2 prefab
1. Locate the Snorelax2 prefab in **Prefabs -> Units -> Heroes**.
2. Set the slider to **5**.
                                
### Locations of Algorithms:
1. Anthony:
  - MonteCarloSearch, A* Algorithms are in **Assets -> _scripts -> AO Algo**
2. Fourcan: 
  - A*, Hero Nash + Minimax, Enemy Minimax Algorithms are in **Assets -> _Scripts -> Units**
3. Eugene:
  - ML implementation is in **Assets -> _Scripts -> Units -> Heroes**


By Anthony Williams, Fourcan Adullah, and Eugene Sajor

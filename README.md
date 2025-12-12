# Procedural-AI-Chunked-World
Procedural chunk-based world generation + Runtime NavMesh + State Machine AI (Unity project).
# Procedural Chunked Worlds + AI
Author: Muhammad Kamil Khan  

## Overview
This Unity project demonstrates a streaming procedural generation system with runtime NavMesh rebaking and an advanced enemy AI (Patrol → FOV detection → Chase → Search). Suitable as a demo/prototype for research into procedural environments and AI navigation.

## Features
- Chunk-based deterministic PCG (seeded per chunk)
- Random interior walls & obstacles (configurable densities)
- Chunk streaming (create/destroy as player moves)
- Runtime NavMesh rebuild via NavMeshComponents
- Enemy AI with perception & state machine
- Enemy spawn happens after NavMesh bake to ensure valid navigation

## Requirements
- Unity 2022.x (2022.3 LTS recommended)
- NavMeshComponents package (Install via Package Manager / Git URL: `https://github.com/Unity-Technologies/NavMeshComponents.git`) OR import the NavMeshComponents folder into `Assets/`


## How to run
1. Open project in Unity.  
2. Open scene: `Auto_AI` (or `MainScene`).  
3. In **ChunkManager** inspector:
   - Assign `LevelGenerator`, `ChunksRoot` (should be present), `Player`, and `NavMeshSurfaceRoot`.  
   - Assign `EnemyPrefab`.  
4. In **LevelGenerator** inspector:
   - Assign `FloorPrefab`, `WallPrefab`, `ObstaclePrefab`. Tune `obstacleDensity` and `wallDensity`.  
5. Press **Play**. Move the player to generate nearby chunks & observe enemies.

## Useful files
- `Assets/Scripts/LevelGenerator.cs`  
- `Assets/Scripts/ChunkManager.cs`  
- `Assets/Scripts/EnemyAI.cs`  
- `Assets/Scripts/EnemyAlign.cs`

## Controls
- WASD / arrow keys to move (your Player controller)
- 'O' and 'P' to rotate player
- Move to edge of chunks to see streaming behavior

## Demo video
[link to recorded demo] https://www.youtube.com/watch?v=yVf1r7poS2M


## Contact
Muhammad Kamil Khan — [kamilkhan6850@gmail.com]  

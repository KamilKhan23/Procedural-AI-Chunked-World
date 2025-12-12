# Procedural-AI-Chunked-World
Procedural chunk-based world generation + Runtime NavMesh + State Machine AI (Unity project).
# Procedural Chunked Worlds + AI
Author: Muhammad Kamil Khan  

#  Procedural AI Chunked World  
**A Research-Oriented Prototype Combining Procedural Content Generation (PCG) and Adaptive AI Navigation in Unity**

[![Demo Video](https://img.youtube.com/vi/yVf1r7poS2M/0.jpg)](https://www.youtube.com/watch?v=yVf1r7poS2M)

---

## Demo Video
**Watch the full demonstration here:**  
https://www.youtube.com/watch?v=yVf1r7poS2M

---

##  Project Overview
This project is a **procedural AI simulation framework** created in Unity.  
It showcases how **infinite, dynamically generated environments** can interact with **adaptive AI agents** through runtime NavMesh updates and autonomous behavior systems.

The system was developed as part of a **graduate research portfolio** for applications to MS/PhD programs in Computational Media, Game AI, and Procedural Content Generation (PCG).

---

##  Core Features

###  **1. Chunk-Based Procedural World Generation**
- Infinite world streaming  
- Deterministic generation with seed control  
- Difficulty-based obstacle density  
- Random obstacles + interior walls  
- Lightweight chunk system (~10×8 tiles each)

###  **2. AI Agent Behavior (State Machine)**
AI enemies exhibit multi-state behavior:
- **Patrol** random NavMesh points  
- **Perception system** (field of view + distance)  
- **Chase** when the player is detected  
- **Search** behavior when line-of-sight is lost  
- Fully dynamic — AI adapts to newly generated chunks  

###  **3. Runtime NavMesh Generation**
- NavMesh baked at runtime for each chunk  
- Updates automatically as chunks load/unload  
- Ensures AI remains navigable in a changing world  

###  **4. Player Controller**
- Smooth WASD + 'O' and 'P' for player rotation 
- Relative movement  
- Simplified capsule-based controller  

---

##  Tech Stack

| Component | Technology |
|----------|------------|
| Engine | Unity 2022.3 LTS |
| AI Navigation | Unity NavMesh Components |
| PCG | Custom Chunk Generator |
| Scripting | C# |
| Platform | Windows |

---

## 📁 Project Structure
│── AILevelDesigner/ # PCG scripts & chunk generator
│── Player/ # Player controller & camera logic
│── Enemy/ # AI behavior scripts + perception
│── Prefabs/ # Tiles, walls, obstacles, enemies
│── Scenes/ # Aouto_AI scene



---

##  How It Works (High-Level)
1. The **Chunk Manager** tracks the player position.  
2. When the player approaches the edge of the loaded area, a **new chunk** is generated.  
3. Walls + obstacles are placed procedurally.  
4. A **runtime NavMesh** is baked for that chunk.  
5. **AI enemies** spawn based on difficulty and available nav-space.  
6. Enemies begin their **patrol/Search/Chase state machine**, reacting to the player.  

---

##  Research Relevance  
This prototype demonstrates concepts relevant to academic research in:

- Procedural Content Generation (PCG)  
- Runtime NavMesh synthesis  
- Adaptive autonomous agents  
- Game AI simulation  
- Emergent world–agent interaction  
- Dynamic difficulty + PCG metrics (future work)

---

##  Future Extensions (Planned)
- Learning-based enemy decision-making (RL or Utility AI)  
- Difficulty modeling using PCG metrics  
- Adaptive chunk generation based on player performance  
- Visual debugging tools for PCG and AI behavior  
- Multi-agent cooperation/competition  

---

##  Author  
**Muhammad Kamil Khan**  
BS Computer Systems Engineering – UET Peshawar  
US Citizen • Game AI & Procedural Systems Researcher  

LinkedIn: *[add link here](https://www.linkedin.com/in/muhammad-kamil-khan-0196a3271/)*  
Email: *kamilkhan6850@gmail.com*  

---


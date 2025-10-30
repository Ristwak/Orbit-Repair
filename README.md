# 🛰️ ORBIT REPAIR VR

**Orbit Repair VR** is a hands-on virtual reality simulation that puts players in the role of an astronaut performing an emergency satellite repair mission outside a space station.  
Blending education and immersion, it offers a cinematic, interactive journey through the challenges of orbital repair — teaching players timing, precision, and the importance of procedure in space.

---

## 🎮 Gameplay Overview

- The mission begins **inside a spacecraft**, where the player activates the repair system.
- The sequence of tasks follows a realistic space repair protocol:
  1. **Press the Button** to initialize the mission.
  2. **Pick up the Tool** (screwdriver) to prepare for EVA.
  3. **Pull the Lever** to open the airlock hatch.
  4. **Exit into Space** and locate the malfunctioning satellite.
  5. **Repair the SparkBox** using the tool.
  6. **Complete the Mission** and return safely.

Each interaction unlocks the next stage, guiding the player through logical progression and interactive storytelling.  
The environment includes sound cues, visual feedback, and narrations that make every action feel authentic.

---

## ✨ Key Features

- **Sequential Mission Flow** – Complete each task step by step: button → tool → lever → repair.  
- **Immersive VR Interactions** – Built using the XR Interaction Toolkit, compatible with hand tracking and controllers.  
- **Physics-Based Environment** – Objects respond realistically within the space setting.  
- **Mouse Interaction Mode** – Allows PC testing without a VR headset.  
- **Dynamic Narration** – Mission prompts and success lines managed by a centralized audio system.  
- **Cinematic Scene Transitions** – Smooth fade-in/out effects using a loading screen manager.  
- **Mission Completion Logic** – Spark effects, mission audio, and automatic scene restart integrated via scripts.

---

## 🧩 Gameplay Flow

### 🪐 **Scene 1 – Space Station Interior**
1. **Button Activation:**  
   The player presses a glowing button to begin the mission.  
   - Narration plays, system lights activate, and suit HUD appears.
   - Tool pickup becomes available.

2. **Tool Collection:**  
   The screwdriver on the workstation can be grabbed or touched.  
   - The tool disappears from the table.  
   - An identical version appears in the player’s hand.  
   - Lever activation becomes available.

3. **Lever Interaction:**  
   The player pulls the lever to open the airlock hatch.  
   - Animation and sound play.  
   - The scene transitions to the next environment: **Outer Space**.

---

### 🌌 **Scene 2 – Outer Space Repair**
1. The player floats near the **SparkBox**, where a continuous spark effect simulates a damaged circuit.  
2. When the player touches the box with the **screwdriver**, the following occurs:
   - Short delay simulates repair work.  
   - Sparks fade out gradually.  
   - “Mission Complete” narration plays via the `AudioManager`.  
   - A few seconds later, the game restarts for replay.

---

## 🛠️ Technical Overview

| Component | Description |
|------------|-------------|
| **Engine** | Unity 2022.3 LTS (URP) |
| **Framework** | XR Interaction Toolkit |
| **Interaction Mode** | Hand tracking & controller support |
| **Testing Mode** | Mouse-based collider interaction |
| **Scenes** | `Orbit Repair` (station) & `Outer Space` (EVA repair zone) |

---

## 🧠 Educational Objectives

- Demonstrate **procedure-based learning** through realistic VR tasks.  
- Teach players **sequencing, precision, and safety** in a high-risk space environment.  
- Foster understanding of **satellite repair concepts** and **EVA mission steps**.  
- Encourage critical thinking and situational awareness through experiential learning.

---

## 📜 Script Summary

| Script | Function |
|--------|-----------|
| `OrbitRepairMenuUI.cs` | Manages main menu and game start logic |
| `SuitUpButton.cs` | Handles button press animation and activation sequence |
| `ToolPickupEquipper.cs` | Manages pickup logic, activates in-hand tool, unlocks lever |
| `LeverToSceneLoader.cs` | Controls lever pull and transitions to the next scene |
| `SparkController.cs` | Manages spark particle system, mission completion audio, and restart logic |
| `AudioManager.cs` | Central audio system for music, narration, and mission cues |
| `OrbitRepairGameManager.cs` | Tracks mission states and transitions |
| `OrbitRepairSequenceDirector.cs` | Coordinates phase unlocking between tasks |
| `LoadingScreen.cs` | Handles fade transitions during scene load |

---

## 🧭 Mission Flow Summary

> Press the button → pick up the tool → pull the lever → step into space → repair the SparkBox → hear the success cue → restart mission.

---

## 📦 Requirements

- **Unity Version:** 2022.3 LTS or later  
- **Packages Needed:**
  - XR Interaction Toolkit  
  - TextMeshPro  
  - OpenXR Plugin  
- **Hardware:**  
  - Meta Quest / HTC Vive / OpenXR-compatible headset  
  - Optional: PC mouse support for debugging

---

## 🗺️ Roadmap

- Add **zero-gravity movement mechanics** with limited thrust controls  
- Introduce **repair time pressure** (oxygen or timer)  
- Expand **SparkBox system** into multi-step repairs  
- Add **voice-guided AI companion** for instructions  
- Localize the experience for multiple languages  

---

## 👨‍🚀 Credits

Developed, designed, scripted, and produced by  
**🎖️ Ristwak Pandey**  

> I am the sole developer and rightful owner of this game and hold complete authority over its design, logic, code, visuals, and distribution.  

---

## 📜 License

This project is copyrighted under **Ristwak Pandey**.  
All rights reserved.  
Unauthorized reproduction, modification, or distribution of this project or its assets is strictly prohibited.

---

## 🪐 Closing Note

> *Orbit Repair VR* isn’t just a game — it’s an interactive learning experience that brings players face-to-face with the challenges of working in space.  
Through curiosity, precision, and calm under pressure, the player becomes not just a gamer — but a true orbital engineer.

---

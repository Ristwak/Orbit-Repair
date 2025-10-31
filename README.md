# 🛰️ ORBIT REPAIR VR

**Orbit Repair VR** is a fully interactive virtual reality simulation where players perform an emergency satellite repair outside a space station.  
It combines cinematic storytelling, physics-based interaction, and real-time narration to recreate the challenges of a real astronaut’s Extra-Vehicular Activity (EVA).

---

## 🎮 Gameplay Overview

The experience begins **inside the spacecraft**, where the player follows a guided repair protocol.  
Each mission step builds upon the last — ensuring players understand **order, precision, and timing**.

### 🔧 Mission Steps

1. **Press the Button** to initiate the repair protocol.  
2. **Suit Up** and activate the helmet HUD.  
3. **Pick Up the Tool** (screwdriver) to prepare for EVA.  
4. **Pull the Lever** to open the airlock hatch.  
5. **Exit into Space** and approach the damaged satellite.  
6. **Repair the SparkBox** to restore satellite power.  
7. **Hear the Victory Cue** and automatically restart for replay.

Each interaction unlocks the next stage, guided by **narration, ambient cues, and visual feedback**.

---

## ✨ Key Features (2025 Edition)

| Feature | Description |
|----------|-------------|
| 🖐️ **Hand + Ray Interactions** | All objects can be interacted with using **hand tracking** or **controller rays**. |
| 🔩 **XRGrabInteractable Integration** | Tools, buttons, and levers now support both direct grabs and ray-based interaction. |
| ⚙️ **Improved Collision Logic** | Optimized triggers ensure reliable detection in low-gravity environments. |
| 🎧 **Persistent Audio System** | `AudioManager` now uses **DontDestroyOnLoad**, allowing cross-scene narration and music continuity. |
| ⏱️ **Single Global Timer** | Centralized mission timer shared across all scripts to handle time-based success/failure. |
| 🔇 **Play-Once Narration** | Each narration or music clip plays only once per session — no repetition or looping. |
| ⚡ **Enhanced Spark System** | New collision-driven SparkBox logic that triggers mission completion audio before restart. |
| 🖱️ **Mouse & PC Test Mode** | All interactions can be simulated via mouse input for PC debugging. |

---

## 🧩 Gameplay Flow

### 🪐 **Scene 1 – Space Station Interior**

1. **Suit-Up Button Press**
   - Pressable via hand or ray.  
   - Plays “Welcome to Mission” narration.  
   - Unlocks tool pickup.

2. **Tool Pickup**
   - Grab the screwdriver using XRGrabInteractable.  
   - The tool disappears from the table and appears in the player’s hand.  
   - Unlocks the lever for airlock activation.

3. **Lever Activation**
   - Pull the lever to open the airlock.  
   - Scene transitions to **Outer Space** using the `LoadingScreen` fade.

---

### 🌌 **Scene 2 – Outer Space Repair**

1. **Approach the SparkBox**
   - Sparks continuously emit to simulate a damaged circuit.

2. **Repair Sequence**
   - When the tool touches the SparkBox:
     - Sparks fade after a short delay.  
     - “Mission Complete” narration plays (cross-scene).  
     - Scene restarts after narration ends.

---

## 🧠 Educational Objectives

- Demonstrate **procedure-based learning** in a realistic space environment.  
- Teach **sequencing, precision, and safety** in high-risk operations.  
- Promote understanding of **satellite repair** and **EVA procedures**.  
- Encourage **focus, timing, and calm under pressure**.

---

## 🛠️ Technical Overview

| Component | Description |
|------------|-------------|
| **Engine** | Unity 2022.3 LTS (URP) |
| **Framework** | XR Interaction Toolkit + OpenXR |
| **Input Modes** | Hand tracking, VR controllers, or mouse (debug) |
| **Scenes** | `Orbit Repair` (interior) & `Outer Space` (EVA repair zone) |
| **Audio System** | Persistent, non-repeating, cross-scene `AudioManager` |
| **Physics** | Trigger-based collision system |
| **Persistence** | AudioManager marked as `DontDestroyOnLoad` |
| **Testing** | Mouse-click simulation for all major actions |

---

## 📜 Updated Script Summary

| Script | Purpose |
|--------|----------|
| **`AudioManager.cs`** | Centralized system for all audio cues, ensuring no repetition; persistent across scenes. |
| **`GameTimer.cs`** | Global timer accessible to all mission scripts for countdown-based logic. |
| **`OrbitRepairMenuUI.cs`** | Handles main menu, mission start, and time initialization. |
| **`SuitUpButton.cs`** | Pressable with hand or ray; animates button and activates next mission stage. |
| **`ToolPickupEquipper.cs`** | Enables realistic tool grabbing, activates in-hand tool, and unlocks lever. |
| **`LeverToSceneLoader.cs`** | Controls lever pulling animation and scene transitions. |
| **`OrbitRepairSequenceDirector.cs`** | Manages mission order (Button → Tool → Lever → Space). |
| **`OrbitRepairGameManager.cs`** | Oversees game state, success/failure, and game-over UI logic. |
| **`SparkController.cs`** | Handles spark deactivation, win narration, and delayed restart. |
| **`LoadingScreen.cs`** | Smooth fade transition during scene loading. |

---

## 🧭 Mission Flow Summary

> **Press the Button → Pick up the Tool → Pull the Lever → Repair the SparkBox → Hear the Mission Complete narration → Restart automatically**

---

## 🎧 Audio System Enhancements

| Feature | Behavior |
|----------|-----------|
| 🎵 **Menu Music** | Loops continuously on main menu only. |
| 🔈 **Narration Clips** | Play once per session and do not repeat. |
| 🚀 **Cross-Scene Audio** | Win/Fail narrations persist across scene loads. |
| 🎚️ **Music Ducking** | Music volume lowers automatically during narration playback. |
| 🔊 **Forced Narration Mode** | `PlayNarrationForce()` overrides all audio for game-ending cues. |

---

## 📦 Requirements

- **Unity Version:** 2022.3 LTS or later  
- **Packages Required:**
  - XR Interaction Toolkit (v3.x or newer)
  - OpenXR Plugin  
  - TextMeshPro  
- **Hardware:**
  - Meta Quest / HTC Vive / OpenXR-compatible headset  
  - Optional: mouse and keyboard for testing

---

## 👨‍🚀 Credits

Developed, scripted, and produced by  
**🎖️ Ristwak Pandey**

> I am the sole developer and rightful owner of this project and hold full authority over its logic, code, visuals, and distribution.

---

## 📜 License

This project is copyrighted under **Ristwak Pandey**.  
All rights reserved.  
Unauthorized reproduction, modification, or distribution of this project or its assets is strictly prohibited.

---

## 🪐 Closing Note

> *Orbit Repair VR* is not just a simulation — it’s a journey through the precision and patience of real orbital engineering.  
Through immersive learning and guided interaction, players experience what it truly means to repair the future, one bolt at a time.

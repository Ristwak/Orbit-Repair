# Suit Up VR

**Suit Up VR** is an interactive virtual reality learning experience where players learn how astronauts prepare for a space mission by correctly assembling a space suit. The game blends education and immersive VR gameplay, guiding players step by step through donning each part of the suit under a time limit.

---

## 🎮 Gameplay Overview

- The player starts in a spacecraft preparation environment.
- The mission is to equip all space suit parts in the correct order:
  1. Skin Cooling Suit
  2. Main Suit
  3. Shoes
  4. Left Hand Glove
  5. Right Hand Glove
  6. Backpack
  7. Helmet
- Each suit component has narration explaining its purpose.
- Players must complete the sequence **within a time limit** (default 4 minutes).
- Completing the sequence successfully triggers a mission complete narration and a Game Over (success) panel.
- Running out of time ends the mission with failure.

---

## ✨ Features

- **Step-by-step interactive dressing** of a space suit
- **Narrated instructions** for each component
- **Countdown timer** for added challenge
- **Mission success and failure states**
- **Game Over panel** with Restart and Quit options
- **VR-ready UI** with tracked device raycasting
- **Hand gesture-based locomotion** for immersive movement

---

## 🧑‍🏫 Educational Goals

- Teach the correct sequence of donning a space suit
- Explain the function of each suit component
- Highlight the importance of timing and procedure in astronaut safety
- Provide an engaging introduction to astronaut training

---

## 🛠️ Project Structure

- **Scripts**
  - `MainMenuUI.cs` → Handles menu, game panels, restart/quit
  - `SuitMeshAttach.cs` → Core gameplay logic, suit assembly, timer
  - `AudioManager.cs` → Background music and narration
  - `HandGesturePlayerMove.cs` → Player locomotion with hand gestures
- **UI**
  - Main Menu panel
  - About panel
  - Game Over panel
  - Timer display
- **Audio**
  - Narration clips for each step
  - Intro and mission complete lines
  - Background music and SFX

---

## 📦 Requirements

- **Unity 2022.3 LTS** or later
- **XR Interaction Toolkit**
- **TextMeshPro**
- VR headset (Meta Quest, HTC Vive, or other OpenXR compatible)

---

## 🚀 Getting Started

1. Clone this repository.
2. Open the project in Unity.
3. Install and enable the **XR Interaction Toolkit** package.
4. Connect your VR headset and ensure OpenXR is enabled in Project Settings.
5. Press **Play** and use VR controllers or hand gestures to interact.

---

## 🗺️ Roadmap

- Add mini-challenges in zero gravity after suiting up
- Expand narration with quizzes and reflections
- Improve hand gesture locomotion with physics-based collisions
- Add more detailed astronaut environment assets
- Localize narration for different languages

---

## 📜 License

I, **Ristwak Pandey**, hold all rights to this game as I am the sole developer. I have full authority over the design, development, and distribution of this project.

# UR3-Augmented-Reality-Teleoperation-System-for-Robotic-Art

<p align="center">
  <video src="https://media.githubusercontent.com/media/YOUR_GITHUB_USERNAME/AR-Painter/main/showcase-videos/assembly/assembly1fast.mp4" width="100%" style="max-width: 800px;" autoplay loop muted playsinline controls></video>
</p>

---

## 📊 Project Overview

[![Project Type](https://img.shields.io/badge/Project-Robotics%20Capstone-blue.svg)](#)
[![Unity Version](https://img.shields.io/badge/Unity-6000.2.10f1%20(6.2)-black.svg)](https://unity.com/)
[![SDK](https://img.shields.io/badge/Meta%20XR-Core%20%26%20Interaction-lightgrey.svg)](https://developer.oculus.com/)
[![Robot](https://img.shields.io/badge/Robot-UR3-red.svg)](https://www.universal-robots.com/)
[![Protocol](https://img.shields.io/badge/Protocol-XML--RPC%20%2F%20TCP%2FIP-orange.svg)](#)

[cite_start]**AR Painter** is an advanced robotic teleoperation system that combines Augmented Reality (AR Class 2 Display) with collaborative robotics[cite: 202, 217]. [cite_start]Built as a final group project for the Bachelor's Degree in Robotic Engineering at the University of Alicante, the system allows users to sketch paths on a virtual 3D canvas using a **Meta Quest 3** headset[cite: 221, 222, 235]. [cite_start]These hand-drawn air gestures are translated in real-time into spatial coordinates, causing a physical **Universal Robots UR3 collaborative arm** to replicate the artwork onto a real whiteboard[cite: 235, 237, 258].

[cite_start]This framework yields massive potential for assistive accessibility (enabling individuals with severe motor disabilities to engage in artistic activities) and teleoperation within hazardous environments where a direct human presence is impossible[cite: 206, 207].

---

## 🔌 Hardware Setup & Connection Architecture

[cite_start]The system relies on a decentralized multi-device network layout designed to distribute computational loads[cite: 289]. [cite_start]Because direct APIs do not exist to link Meta ecosystems with Universal Robots, an intermediary communication bridge was custom-developed[cite: 271, 272]:


### 🧠 Core Subsystems Breakdown:
1. **AR Environment Side (PC 1)**: Runs the real-time Unity interface[cite: 236]. It leverages the host PC’s hardware (CPU, GPU, RAM) rather than running standalone on the headset, dramatically improving processing power and speeding up code deployment via **Meta Horizon Link**[cite: 283, 285].
2. **Central Server Bridge (PC 2)**: Centralizes cross-platform traffic by initializing a multi-threaded **TCP/IP socket server** to intercept string instructions from Unity, and a local **XML-RPC server** to handle structural requests from the robot[cite: 300, 318].
3. **Actuator Side (UR3 Arm)**: Connects to PC 2 via Ethernet IPv4[cite: 288]. The Polyscope controller queries the XML-RPC server natively to parse motion data array poses (`x, y, z`)[cite: 138, 150].

---

## 📁 Codebase & Component Breakdown

<details>
<summary>📦 <b>Click here to expand the detailed code and script architecture</b></summary>

### 🎮 Unity Modules (Assets/Scripts)
* **`Pincel.cs` (Brush Script)**: The mathematical core of the interface[cite: 364]. It tracks input triggers, enforces drawing boundary checks (ensuring points are discarded if drawn outside the canvas) [cite: 92, 94], maps spatial vectors to flat drawing planes [cite: 95], and commands the native Unity `LineRenderer` component to draw visible ink paths[cite: 98, 100].
* **`Comunicacion.cs`**: Instantiates the TCP/IP socket client upon startup[cite: 106]. It packages Vector3 and Quaternion game positions into parsed text strings formatted as:  
  `"<Action_Command>/<X_Coord>/<Y_Coord>/<Z_Coord>/"`[cite: 111, 112]. 
  It works asynchronously to prevent the main Unity rendering thread from freezing[cite: 108].

### 🐍 Python Server Modules
* **`pc_server.py`**: Runs on the central computer[cite: 118]. Listens for incoming TCP/IP strings from Unity, reformats coordinate streams, and hosts the active XML-RPC function definitions[cite: 118].
* **`ur_lib.py`**: The official protocol helper provided by Universal Robots to bridge native Python methods with XML data formatting[cite: 164].

### 🤖 Robot Controller Scripts
* **`XMLR_PC.script` / `.urp`**: Program uploaded to the UR3 TP[cite: 119, 198]. Loops actively to poll the server for state cues via the XML-RPC layer, executing linear path interpolations (`MoveL`) across target coordinate clusters[cite: 138, 142].

</details>

---

## 🎮 Interface Controls & Interaction

Interaction is mapped natively using the **Meta Quest Touch Pro** input actions framework[cite: 314, 354]:

* **Grip Buttons (Left/Right Hand)**: Grabs, drags, and repositions interactive 3D assets in space, such as the easel or brush tool[cite: 74, 346].
* **Right Index Trigger**: Draws paths on the canvas[cite: 77, 350]. It behaves like classic desktop software (e.g., MS Paint)—sampling an initial tracking point on press and a final target point on release to dramatically minimize raw data congestion over the network[cite: 79, 80].
* **Button A**: Instantly clears all visual vectors and flushes point coordinate tables[cite: 75].
* **Button X**: Submits the generated vector points from Unity over to the UR3 server queue for drawing execution[cite: 76].
* **Color Buckets**: Touching the Blue, Red, Green, or Yellow buckets shifts the visual brush hue for that complete drawing queue[cite: 68, 193].

---

## 🚀 Quick Start & Execution Guide

The environment can be ran using a **Single PC setup** (using Loopback Local IPs) or a **Dual PC setup** (recommended for performance)[cite: 159, 169].

### 1. Requirements & Dependencies
* **Unity 6.2 (6000.2.10f1)** containing the following SDK packages: Meta XR Core SDK, Meta XR Interaction SDK, OpenXR Plugin, and XR Plugin Management[cite: 163].
* **Python 3.x** installed on the server device.
* **A calibrated end-effector mechanical assembly**: The UR3 tool center point (TCP) tool attachment must incorporate a passive compression spring mechanism[cite: 131, 155]. This absorbs tolerance offsets against the board and maintains uniform pressure without damaging the marker tip[cite: 23, 141].

### 2. Startup Sequence
1. Connect both computers (if using the dual configuration) to the exact same network gateway[cite: 167]. Configure your Ethernet IPv4 interfaces to link the UR3 controller[cite: 171].
2. Define your **Feature Plane Workspace** on the UR3 pendant by moving the physical robot tool parallel across the whiteboard to capture the origin, X-axis direction, and Y-axis vector orientations[cite: 132, 135].
3. Open the UR3 code program, verify it points to the correct Server Bridge IP, and press **Play**[cite: 172, 173].
4. Connect the Meta Quest 3 via a Link Cable, navigate inside the headset settings, and boot **Meta Horizon Link**[cite: 174, 175].
5. Boot up your terminal window on PC 2 and run the Python bridge:
   ```bash
   python pc_server.py

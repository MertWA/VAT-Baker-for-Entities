# 📦 VAT Baker for Unity Entities (DOTS)

[![Asset Store Link](https://img.shields.io/badge/🔥_NEWEST_VERSION-SOON! Available_on_Asset_Store-green)](https://github.com/MertWA/VAT-Baker-for-Entities)

![Unity 6](https://img.shields.io/badge/Unity-6-000000?style=flat&logo=unity&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-blue.svg)

## ✨ Overview
**MWA VAT Baker** is a lightweight and efficient tool designed to bake **Skinned Mesh Renderer** animations into **Vertex Animation Textures (VAT)**.

Fully compatible with **Unity 6** and the latest **DOTS/ECS** workflow, this tool allows you to bypass the performance overhead of the traditional Animator component.

### Key Features
* **One-Click Baking:** Simple Editor Window to generate required assets.
* **Complete Data:** Bakes both **Position** and **Normal** data into high-precision EXR textures.
* **Ready-to-Use Shader:** Includes a custom **URP Shader Graph** to decode and play the animations on the GPU.
* **ECS Friendly:** Designed specifically to work with `RenderMesh` and DOTS instancing.

---

## 🛠️ Requirements
* **Unity Version:** Unity 6 (Recommended) or Unity 2022.3 LTS+.
* **Render Pipeline:** Universal Render Pipeline (URP).
* **Packages:** Entities (1.0+), Universal RP.
* **Input:** A `GameObject` with a **SkinnedMeshRenderer** and an **AnimationClip**.
  
---

## 🛠️ Installation

1. Download the repository or clone it.
2. Copy the `MWA_VATBaker.cs` script into an `Editor` folder in your Unity project (e.g., `Assets/Scripts/Editor/`).
3. Copy the `MWA_Vat_Shader.shadergraph` into your project.

---

## 🚀 How to Use

### 1. Baking the Animation
1. Open the tool via the Unity menu: **Tools > VAT Baker V4 (Normals + Color)**.
2. Assign the **Model** (GameObject with SkinnedMeshRenderer) and the **Animation** (AnimationClip).
3. Set the **Frame Count** (resolution of the animation).
4. Click **Bake**.

> **Output:** The tool will create a folder named `Assets/MWA_VATOutput_` containing:
> * `PositionTexture.exr`
> * `NormalTexture.exr`
> * `BakedMesh.asset` (Mesh with UV2 channel mapping vertex indices).

### 2. Setting up the Material
1. Create a new **Material** and select the Shader Graph: `Shader Graphs/Vat_Shader` (or wherever you placed the graph).
2. Assign the generated textures to the material slots:
   * **AnimPositions** $\rightarrow$ `PositionTexture.exr`
   * **AnimNormals** $\rightarrow$ `NormalTexture.exr`
3. Set the **TargetFPS** or **Frame Count** property in the material to match the `Frame Count` you used during baking.

### 3. Animating in DOTS/ECS
To animate the entities, you simply need to drive the **Time** input in the material properties.

* Since the shader uses the Mesh's **UV2** coordinates and the Textures to offset vertices, you don't need `SkinningComponents`.
* Use a **Hybrid Renderer** approach or `RenderMeshUtility` to assign this Mesh and Material to your Entities.
* Update the Material Property Block for `Time` (or a custom time variable) via a System to play the animation.

---

## 📄 Technical Details
* **Texture Format:** RGBAFloat (EXR) for high precision vertex positioning.
* **Filter Mode:** Point (Nearest Neighbor) is strictly enforced to prevent data interpolation artifacts.
* **Normal Encoding:** Normals are remapped from `[-1, 1]` to `[0, 1]` range for texture storage.

---

## 🤝 License
This project is open-source and free to use in personal or commercial projects.

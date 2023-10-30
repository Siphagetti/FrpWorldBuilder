# FrpWorldBuilder

## Table of Contents

1. [Project Overview](#project-overview)
2. [Code Structure](#code-structure)
   - [GameManager](#gamemanager)
   - [ServiceManager](#servicemanager)
   - [Logger](#logger)
   - [Language](#language)
   - [Prefab](#prefab)
   - [Save](#save)
   - [Hierarchy](#hierarchy)

## Project Overview

Welcome to the GitHub repository for Your Project Name. This project is designed to [briefly describe the main purpose of your project and the problem it aims to solve]. It utilizes the Unity game engine and includes various scripts and components to achieve its goals.

## Code Structure

In this section, we'll explore the key components and scripts that make up the project's codebase. Each script and component is described below.

### GameManager

The `GameManager` script serves as the core of the project, responsible for managing the game's main loop and high-level logic. It performs the following tasks:

- Initializes essential systems and services.
- Handles input events and user interactions.
- Manages game state transitions.

### ServiceManager

The `ServiceManager` script acts as a central repository for various services used throughout the project. It offers the following functionalities:

- Manages service instances and provides access to essential features.
- Included services cover areas such as language localization, data management, and more.

### Logger

The `Logger` script provides a structured way to log messages of different types, such as track, info, warning, error, and fatal. Key features include:

- Centralized logging for a consistent user experience.
- The ability to log messages with localization support.
- Integration with UI components for displaying log messages.

### Language

The `Language` namespace encompasses language localization and internationalization services. The key components are:

- `LanguageService`: Manages the localization of in-game text and offers the ability to change the language dynamically.
- `LanguagePackage`: A data structure that holds a list of localized messages.

### Prefab

The `Prefab` namespace focuses on prefab and asset bundle management within the project. It includes the following components:

- `PrefabDragManager`: Handles drag-and-drop functionality for prefabs and asset bundles.
- `CategoryComponent`: Manages categories and asset bundles for organized content.
- `ThumbnailsComponent`: Handles the creation and display of thumbnail images for prefabs.

### Save

The `Save` namespace provides functionality for saving and loading game data. It offers:

- `SaveManager`: Manages save and load operations for game progress and data storage.
- `SavableObject`: A base class for objects that can be saved and loaded.

### Hierarchy

The `Hierarchy` namespace deals with the hierarchical structure of game objects and scenes. Key components include:

- `HierarchyManager`: Manages the hierarchy of game objects and scenes, including their creation, deletion, and organization.

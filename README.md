# FrpWorldBuilder

This project is a Unity-based game/application with modular components organized into various namespaces for easy management and maintenance. Below, you'll find an overview of the project structure and details about each namespace and class.

## Table of Contents

- [Hierarchy Namespace](#hierarchy-namespace)
  - [HierarchyManager Class](#hierarchymanager-class)
  - [PrefabDragManager Class](#prefabdragmanager-class)
  - [HierarchyElement Class](#hierarchyelement-class)

- [Prefab Namespace](#prefab-namespace)
  - [Prefab Class](#prefab-class)
  - [PrefabDTO Class](#prefabdto-class)
  - [PrefabRepository Class](#prefabrepository-class)
  - [IPrefabService Interface](#iprefabservice-interface)
  - [PrefabService Class](#prefabservice-class)
  - [Thumbnail Class](#thumbnail-class)
  
- [Services Namespace](#services-namespace)
  - [IBaseService Interface](#ibaseservice-interface)
  - [ServiceManager Class](#servicemanager-class)

- [Save Namespace](#save-namespace)
  - [SavableObject Class](#savableobject-class)
  - [SaveManager Class](#savemanager-class)
  - [SaveData Class](#savedata-class)

- [UserInterface Namespace](#userinterface-namespace)
  - [UI_Logger_WorldBuilding Class](#ui_logger_worldbuilding-class)

- [Language Namespace](#language-namespace)
  - [LocalizedText Class](#localizedtext-class)

# Hierarchy Namespace

The `Hierarchy` namespace includes classes responsible for managing the game's hierarchy system.

## HierarchyManager Class

Manages the hierarchy of objects in the game. It allows users to organize objects into groups and provides methods for selecting, deselecting, and deleting objects.

## PrefabDragManager Class

Handles the drag-and-drop functionality of prefabs in the game. It allows users to drag prefabs from the UI onto the scene.

## HierarchyElement Class

Represents an element in the hierarchy. When clicked, it can be selected for manipulation in the scene.

# Prefab Namespace

The `Prefab` namespace contains classes related to the game's prefabs.

## Prefab Class

Represents a game object prefab. It includes methods for initializing, updating, and managing prefabs in the scene.

## PrefabDTO Class

A data transfer object (DTO) used for serializing prefab data to JSON for saving and loading scenes.

## PrefabRepository Class

Manages a collection of prefabs, categorized into asset bundles. It provides methods for adding, removing, and retrieving prefabs.

## IPrefabService Interface

An interface for the prefab service, defining methods for working with prefabs.

## PrefabService Class

Implements the `IPrefabService` interface and is responsible for managing the loading and importing of asset bundles and prefabs.

## Thumbnail Class

Handles the drag-and-drop functionality of UI thumbnails, making it easier to add prefabs to the scene from the UI.

# Services Namespace

The `Services` namespace includes classes for managing various game services.

## IBaseService Interface

An interface for defining common service methods that other services can implement.

## ServiceManager Class

Manages all game services, providing access to different services throughout the game.

# Save Namespace

The `Save` namespace contains classes for saving and loading game data.

## SavableObject Class

A base class for objects that can be saved and loaded. It handles the serialization of data to JSON.

## SaveManager Class

Manages the saving and loading of game data. It keeps track of saved files and provides methods for saving and loading game states.

## SaveData Class

A data structure for saving game data, including a file name and the serialized JSON data.

# UserInterface Namespace

The `UserInterface` namespace is dedicated to classes responsible for the game's user interface.

## UI_Logger_WorldBuilding Class

Manages the user interface for logging in the world-building aspect of the game. It provides methods for logging messages with different log types and displaying them in the user interface.

# Language Namespace

The `Language` namespace contains classes for handling localized text in the user interface.

## LocalizedText Class

Manages localized text in the game's user interface. It automatically updates text components with localized content based on the current language. Provides methods for subscribing to text updates and fetching localized text from the language service.

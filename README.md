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

Manages the hierarchy of objects in the game, providing functionality for organizing objects into groups, creating and deleting groups, and adding, moving, and deleting hierarchy elements. The `HierarchyManager` class is responsible for controlling the hierarchy in the game's user interface.

### Serialized Fields

- **Hierarchy Content:** A reference to the transform that serves as the parent for all hierarchy elements.
- **Hierarchy Group Button Prefab:** The prefab used to create buttons for group headings in the hierarchy.
- **Hierarchy Group Container Prefab:** The prefab used to create containers for groups that hold hierarchy elements.
- **Hierarchy Element Prefab:** The prefab used to create individual hierarchy elements.
- **New Group Input Field:** The input field used for entering a new group name.
- **New Group Button:** A button used to trigger the creation of a new group.
- **Hierarchy Panel Toggle Button:** A button used to toggle the visibility of the hierarchy panel in the user interface.

### Constants

- **Group Button Prefix:** A constant string prefix for group buttons.
- **Group Container Prefix:** A constant string prefix for group containers.

### Dictionaries

- **Hierarchy Groups:** A dictionary that maps group buttons to their corresponding group containers.
- **Hierarchy Items:** A dictionary that maps group containers to lists of hierarchy elements within each group.

### Functions

- **Awake():** Called when the script instance is being loaded. It sets up click listeners for the "New Group" button.

- **CreateNewGroup():** Initiates the creation of a new group. Displays the input field for entering the group name.

- **NewGroupCoroutine():** A coroutine for creating a new group with user input. Monitors user input and creates a new group with the provided name, checking for duplicates.

- **CreateGroup(string groupName):** Creates a new group in the hierarchy. Instantiates a group button and a group container, sets up their properties, and handles their visibility.

- **DeleteGroup(GameObject groupButton):** Deletes a group from the hierarchy, triggering a confirmation popup before deletion.

- **DeleteGroupCoroutine(GameObject groupButton):** A coroutine for handling the deletion of a group. Displays a confirmation popup and removes the group button and container.

- **LoadHierarchyElement(Prefab.Prefab prefab, string groupName):** Loads a hierarchy element into a specified group. Creates a group if it doesn't exist and adds the element to it.

- **AddHierarchyElement(Prefab.Prefab prefab):** Adds a hierarchy element to the root of the hierarchy.

- **ChangeHierarchyElementGroup(Transform element, GameObject targetGroup):** Moves a hierarchy element to a different group or container within the hierarchy. Handles group and container hierarchy adjustments.

- **DeleteHierarchyElement(GameObject element, GameObject group = null):** Deletes a hierarchy element, triggering a confirmation popup before deletion. The optional `group` parameter specifies the group in which the element is contained.

- **DeleteElementCoroutine(GameObject element, GameObject group = null):** A coroutine for handling the deletion of a hierarchy element. Displays a confirmation popup, removes the element, and refreshes the hierarchy visibility.

- **GroupContainerRefresh(GameObject container):** A coroutine to refresh the visibility of a group container, ensuring it updates correctly in the hierarchy.

- **ResetHierarchy():** Clears the dictionaries, resetting the hierarchy structure.

The `HierarchyManager` class plays a crucial role in managing the organization and structure of the game's hierarchy system, providing various functions for group creation, element loading, and element management. It also facilitates group and element deletion with user confirmation.


## PrefabDragManager Class

Manages the drag-and-drop functionality of prefabs in the game. The `PrefabDragManager` class allows users to click and drag prefabs from the UI onto the scene. It handles the selection, dragging, and deselection of prefab objects.

### Properties

- **CamDist:** Represents the camera distance for the currently selected prefab while dragging.

- **_isDragging:** A boolean flag indicating whether a prefab is currently being dragged.

- **SelectedObject:** A static reference to the currently selected prefab.

- **_mouseOffset:** A vector representing the mouse offset when dragging a prefab.

- **targetLayer:** A layer mask used for raycasting to detect objects in the scene.

### Functions

- **Update():** Handles mouse input for prefab dragging by calling `HandleMouseInput()`.

- **HandleMouseInput():** Manages the mouse input for prefab dragging, including starting, stopping, and moving the selected prefab.

- **StopDragging():** Stops the dragging of the selected prefab, resetting its position and visibility.

- **MoveSelectedObjectWithCursor():** Moves the selected prefab object with the cursor's position.

- **HandleMouseClick():** Detects mouse clicks and initiates prefab selection or deselection based on raycasting.

- **SelectObject(GameObject obj):** Selects a prefab object for dragging. Handles mouse offset, visibility, and panel hiding.

- **DeselectObject():** Deselects the currently selected prefab, resetting visibility and dragging state.

- **SetSelectedObject(GameObject obj):** Sets the selected object, enabling it to be moved and handling mouse offset, visibility, and dragging state.

- **Awake():** Initializes the `PrefabDragManager`, sets it as the `Thumbnail`'s drag manager, and retrieves the initial vertical position (`_yPos`) of the panel.

### Shine Mesh Functions

- **ShineMesh():** Makes the currently selected prefab mesh renderer visible.

- **DontShineMesh():** Hides the currently selected prefab mesh renderer.

### Hide or Reveal Panel Functions

- **HidePanel():** Hides the panel containing the prefab. Initiates the hiding animation.

- **RevealPanel():** Reveals the hidden prefab panel. Initiates the revealing animation.

- **HidePanelCoroutine():** Coroutine to smoothly hide the prefab panel using animations.

- **RevealPanelCoroutine():** Coroutine to smoothly reveal the hidden prefab panel using animations.

The `PrefabDragManager` class is responsible for enabling the drag-and-drop functionality of prefabs in the game. It allows users to interact with prefabs, move them around the scene, and provides a user-friendly interface for prefab selection and deselection.

## HierarchyElement Class

The `HierarchyElement` class represents an element in the game's hierarchy system. It allows users to interact with and select prefab objects for manipulation in the scene.

### Key Functions and Features

- **OnPointerClick(PointerEventData eventData):**
  - Responds to mouse click events on the hierarchy element. When the left mouse button is clicked, it communicates with the `PrefabDragManager` to select the corresponding prefab object for manipulation.

The `HierarchyElement` class simplifies the process of selecting and interacting with prefab elements within the game's hierarchy system.

For more details about the specific function and its implementation, please refer to the source code and comments in the class itself.

---

**Note:** The `HierarchyElement` class is designed to handle user interactions with hierarchy elements and trigger actions based on mouse click events.

# Prefab Namespace

The `Prefab` namespace contains classes related to the game's prefabs.

## Prefab Class

The `Prefab` class is responsible for managing game object prefabs in the Unity-based game/application. It includes features for initialization, rendering, scaling, and more.

### Key Functions and Features

- **Initialize():**
  - Initializes the prefab with provided data and asset bundle information, ensuring that the prefab's mesh data is combined, textures are readable, and materials are created.

- **CreateWrapper(Vector3 createPos):**
  - Creates a wrapper for the prefab at the specified position and returns it. The wrapper acts as a container for the prefab and allows for custom rendering.

- **UpdateTransform():**
  - Updates the stored transform information of the prefab to reflect the current state in the scene.

### Editor Functions

- **MakeTexturesReadable():**
  - An editor function to make the textures of prefabs readable. Accessible from the Unity editor's menu, this function ensures that textures can be accessed and modified within the editor.

- **MakeMeshesReadable():**
  - An editor function to make the meshes of prefabs readable. Accessible from the Unity editor's menu, this function ensures that mesh data can be accessed and modified within the editor.

### PrefabDTO Class

- The `Prefab` class uses the `PrefabDTO` (Data Transfer Object) to store information about prefabs, including their GUID, name, asset bundle, hierarchy group name, and transformation. This simplifies the storage and serialization of prefab data.

### SerializableTransform Class

- The `SerializableTransform` class is used to store and manage the transformation data of prefabs in a serializable format.

The `Prefab` class plays a pivotal role in managing the behavior and properties of game object prefabs within the game, including their rendering, scaling, and initialization.

For more details about specific methods and properties, please refer to the source code and comments in the class itself.

---

**Note:** Functions marked as "Editor Functions" are designed for use in the Unity editor environment and aid in asset management and configuration.


## PrefabDTO Class

The `PrefabDTO` class is a data transfer object (DTO) used for serializing prefab data to JSON for saving and loading scenes. It encapsulates essential information about a prefab, allowing it to be easily stored, transferred, and recreated within the game.

### Properties

- `guid` (string): A unique identifier for the prefab.
- `prefabName` (string): The name of the prefab.
- `assetBundleName` (string): The name of the asset bundle to which the prefab belongs.
- `hierarchyGroupName` (string): The name of the hierarchy group to which the prefab is assigned.
- `transform` (SerializableTransform): A serialized representation of the prefab's transform data, including position, rotation, and scale.

### Constructors

- `PrefabDTO()`: Default constructor for creating an empty `PrefabDTO` instance.
- `PrefabDTO(PrefabDTO other)`: A copy constructor that initializes a new `PrefabDTO` instance with the values from an existing `PrefabDTO`.

### Key Methods

- `Equals(object obj)`: Overrides the equality comparison for `PrefabDTO` objects. It compares all fields (excluding the `transform`) to determine if two `PrefabDTO` objects are equal.

- `GetHashCode()`: Overrides the hashing function for `PrefabDTO` objects, ensuring consistent and unique hash codes based on the object's properties.

The `PrefabDTO` class serves as a lightweight data container for prefab information, making it easy to save, load, and transfer prefab data between scenes or during serialization/deserialization processes.

For more details about the specific properties and methods, please refer to the source code and comments in the class itself.

---

**Note:** The `PrefabDTO` class is crucial for serializing and deserializing prefab data, allowing for efficient storage and retrieval of prefab properties during gameplay.


## PrefabRepository Class

The `PrefabRepository` class is responsible for managing and organizing prefabs within asset bundles by category. It provides methods to add, remove, and retrieve prefabs, as well as access asset bundles by category.

### Properties

- `_assetBundles` (private Dictionary): A dictionary that stores prefabs categorized by asset bundle and, further, by category.

### Constructors

- `PrefabRepository(IEnumerable<string> categories)`: Initializes a `PrefabRepository` instance with a list of categories. Each category can contain multiple asset bundles and their associated prefabs.

### Key Methods

- `AddAssetBundle(string category, string bundleName, Prefab[] value)`: Adds an asset bundle with its associated prefabs to a specific category.

- `AddCategory(string category)`: Adds a new category to the repository. A category is used to organize related prefabs.

- `RemoveCategory(string category)`: Removes a category and all its associated asset bundles and prefabs from the repository.

- `RemoveAssetBundle(string bundleName)`: Removes an asset bundle from all categories in the repository.

- `RemoveAssetBundle(string category, string bundleName)`: Removes an asset bundle from a specific category.

- `GetPrefab(string bundleName, string prefabName)`: Retrieves a specific prefab by its name from any asset bundle in the repository.

- `GetPrefab(string category, string bundleName, string prefabName)`: Retrieves a specific prefab by its name from a specific category and asset bundle.

- `GetPrefabs(string bundleName)`: Retrieves all prefabs within a specific asset bundle from the repository.

- `GetPrefabs(string category, string bundleName)`: Retrieves all prefabs within a specific asset bundle from a specific category.

- `GetAllPrefabs(string category)`: Retrieves all prefabs within a specific category, combining the prefabs from different asset bundles.

- `GetAssetBundles(string category)`: Retrieves a dictionary of all asset bundles in a specific category. The dictionary associates each asset bundle with an array of prefabs.

The `PrefabRepository` class is a crucial component for managing, organizing, and accessing prefabs in different categories and asset bundles. It simplifies the retrieval of prefabs based on category, asset bundle, and prefab name.

For more details about the specific methods and their usage, please refer to the source code and comments in the class itself.

---

**Note:** The `PrefabRepository` class plays a central role in structuring and organizing the prefabs in the game, making it easier to manage and retrieve them based on various criteria.


## IPrefabService Interface

The `IPrefabService` interface defines a set of methods and properties related to prefab management within the application. This interface is designed to provide a consistent way of interacting with prefabs, including creating and deleting categories, asset bundles, and importing prefabs.

### Methods and Properties

- `GetRepository()`: Returns a `PrefabRepository` instance, which is responsible for managing and organizing prefabs by category and asset bundle.

- `NewCategory(string categoryName)`: Creates a new category for organizing prefabs. Categories are used to group related prefabs together.

- `DeleteCategory(string category)`: Deletes an existing category, including all associated asset bundles and prefabs.

- `DeleteAssetBundle(string category, string bundleName)`: Deletes a specific asset bundle from a given category.

- `GetCategories()`: Retrieves a list of all available categories in the repository.

- `ImportAssetBundle(string category)`: Imports an asset bundle into a specified category asynchronously. It returns a `Task` that provides a response containing the asset bundle name and associated prefabs.

- `LoadPrefabs(Transform parent, List<PrefabDTO> prefabsData)`: Loads a list of prefabs into the game world. It takes a parent `Transform` and a list of prefab data as parameters and returns a list of loaded `Prefab` instances.

The `IPrefabService` interface serves as an abstraction for managing prefabs, allowing for flexibility and consistency when working with different aspects of prefab management, including categories, asset bundles, and loading prefabs into the game.

For more information on how to use these methods, please refer to the code that implements this interface.

---

**Note:** This interface defines the contract for managing prefabs within the application and can be implemented in classes that handle prefab-related operations. It provides a high-level API for prefab management.

## PrefabService Class

The `PrefabService` class is responsible for managing the import, deletion, and loading of prefabs within the application. It interacts with asset bundles, categories, and the prefab repository to provide a high-level interface for prefab management.

### Fields

- `subFolderName`: Represents the name of the subfolder where asset bundles are stored.

- `rootFolderPath`: Defines the root folder path where asset bundles and categories are organized.

- `prefabRepo`: An instance of the `PrefabRepository` class that manages prefab data.

### Constructor

- `PrefabService()`: Initializes the `PrefabService` by specifying the subfolder name and root folder path. It ensures the root folder exists and loads all existing prefabs.

### Public Methods

- `GetCategories()`: Retrieves a list of category names by scanning subfolders within the root folder.

- `ImportAssetBundle(string category)`: Allows the user to import an asset bundle into a specified category. It opens a file dialog, copies the asset bundle to the category folder, and loads the asset bundle into the repository.

- `DeleteCategory(string category)`: Deletes a category, including all associated asset bundles. It also removes the category from the repository.

- `DeleteAssetBundle(string category, string bundleName)`: Deletes a specific asset bundle within a category. This action includes removing the asset bundle from the repository.

- `NewCategory(string categoryName)`: Creates a new category folder within the root folder to organize prefabs.

- `LoadPrefabs(Transform parent, List<PrefabDTO> prefabsData)`: Loads prefabs into the game world based on prefab data. It creates prefab wrappers and sets their properties, including hierarchy group assignments.

### Private Methods

- `LoadAllPrefabs()`: Loads all existing prefabs by scanning category folders and invoking `LoadAssetBundlesInCategory()` for each category.

- `LoadAssetBundlesInCategory(string category)`: Loads asset bundles within a specific category by scanning the category folder and invoking `LoadAssetBundle()` for each asset bundle.

- `LoadAssetBundle(string category, string bundlePath)`: Loads assets from an asset bundle, initializes prefab components, and adds them to the repository. It handles the loading of prefabs from asset bundles.

The `PrefabService` class provides a central hub for managing prefab-related operations, making it easier to organize and import asset bundles, create and delete categories, and load prefabs into the scene.

For more details on how to use the methods in this class, please refer to the code that implements the `IPrefabService` interface and the associated classes.

---

**Note:** The `PrefabService` class is designed to simplify prefab management tasks by offering a user-friendly interface for users to interact with asset bundles and prefabs.

## Thumbnail Class

The `Thumbnail` class is responsible for managing the UI representation of prefabs within the application. It enables users to drag and place prefabs into the game world, and also allows for managing the visibility of the UI element while dragging.

### Properties

- `OwnerUIPanelRect`: A `RectTransform` property that represents the owner UI panel's RectTransform. It allows the class to control the position of the UI element.

- `dragManager`: A property representing an instance of the `PrefabDragManager` class, which manages prefab dragging operations.

### Fields

- `_image`: A `RawImage` field representing the UI image associated with the prefab.

- `_text`: A `TMPro.TMP_Text` field representing the text associated with the prefab.

- `_prefab`: A field representing the GameObject prefab to be managed by the `Thumbnail` class.

- `_parentTransform`: A reference to the parent transform of the UI element.

- `_initialSiblingIndex`: An integer field to keep track of the initial sibling index of the UI element.

- `_mouseOffset`: A Vector3 field representing the offset for dragging the UI element from where it's held.

- `_hideUI`: A boolean flag to hide the UI element while dragging the prefab.

- `_spawnedPrefab`: A GameObject representing the spawned prefab that follows the cursor while dragging.

- `_prefabSpawnPos`: A Vector3 field representing the position where the spawned prefab is placed when hidden.

### Methods

- `OnBeginDrag(PointerEventData eventData)`: Called when a drag operation begins. It prepares the UI element for dragging, creates a spawned prefab at an invisible position, and calculates the mouse offset.

- `OnDrag(PointerEventData eventData)`: Called while a drag operation is in progress. It controls the position of the UI element based on cursor movement and manages the visibility of the UI element.

- `OnEndDrag(PointerEventData eventData)`: Called when a drag operation ends. It handles UI element visibility, destruction of the spawned prefab, and restoring the UI element's initial state.

- `SetPrefab(GameObject prefab)`: An internal method used to set the prefab that the `Thumbnail` class manages.

# Services Namespace

The `Services` namespace includes classes for managing various game services.

## IBaseService Interface

The `IBaseService` interface is a base interface that signifies a class's intention to serve as a service. It doesn't include any specific methods or properties and is meant to be a generic marker interface for classes that provide various services within the application.

As a marker interface, `IBaseService` is a starting point for defining more specific service interfaces or classes that can extend its functionality as needed. Classes that implement this interface are expected to fulfill a service-oriented role but are not restricted by any predefined contract.

This base interface provides a foundation for organizing and categorizing service-related classes in your application.

---

**Note:** The `IBaseService` interface, while not specifying any members, plays a role in categorizing and structuring classes that provide services within your application. More specific service interfaces or classes can extend its functionality to define the contract for particular services.

## ServiceManager Class

The `ServiceManager` class is a central component of your application responsible for managing various services. It provides a way to register, access, and update services used throughout your application. Services are classes that perform specific functions or provide features, such as language localization, prefab management, and scene-related services.

### Initialization

When your application starts, the `ServiceManager` is responsible for initializing default services. This ensures that essential services are available and ready for use. The class is designed to be a singleton, allowing only one instance to exist throughout the application's lifetime.

### Registering Services

Services can be registered with the `ServiceManager` using the `AddService<T>` method. This method associates a specific service type with an instance of that service. The manager ensures that no duplicate services of the same type are added.

### Accessing Services

You can retrieve services from the `ServiceManager` using the `GetService<T>` method. This method returns the registered instance of a service based on its type. If a service of the specified type doesn't exist, it returns a default value, typically `null`.

### Updating Services

The `ServiceManager` also provides a way to update services. You can use the `UpdateService<T>` method to replace or update the instance of a registered service with a new one.

### Getting All Services

If needed, you can retrieve all registered services with the `GetAll` method. This can be useful for iterating through all available services, inspecting their state, or performing batch operations.

The `ServiceManager` class plays a vital role in managing and organizing the services required for your application's functionality. By using this central service manager, you can easily maintain, access, and update services, ensuring the smooth operation of your application.

---

**Note:** The `ServiceManager` class is an essential part of structuring your application, making it easy to manage and access various services used by your application components. It ensures that services are correctly initialized, preventing duplicates, and provides a flexible way to access and update them.


# Save Namespace

The `Save` namespace contains classes for saving and loading game data.

## SavableObject Class

The `SavableObject` class is a fundamental component in your application that allows objects to be saved and loaded in a structured manner. It is part of the save system and provides an abstract base for objects that need to be saved and loaded. 

### Key-Based Data Management

- Objects derived from `SavableObject` are associated with a unique key. This key is essential for identifying and managing data specific to each object. The key can be augmented with an optional key modifier to handle different data for objects of the same class.

- Objects are registered with the `SaveManager`, which ensures that they can be saved and loaded at the appropriate times. This is typically done when constructing or initializing an object and unregistering when the object is destroyed.

### Saving Data

- To save data for a `SavableObject`, you can override the `Save` method. This method serializes the object into JSON data and adds it to the `SaveManager`. The data is saved in a structured list for later retrieval. This method returns a `Task` for asynchronous handling.

### Loading Data

- The `Load` method is overridden to load data from the save JSON via the `SaveManager`. The method looks for the relevant data in the saved list, deserializes it, and populates the object's fields. The loading process is asynchronous and returns a `Task`.

### Data Serialization

- Data is serialized into JSON using Unity's `JsonUtility` class for saving and deserializing. This ensures data compatibility and easy data transformation.

### Equality and Hash Code

- The `Equals` method checks for equality based on the object's unique key. Two `SavableObject` instances are considered equal if their keys match, regardless of case. The `GetHashCode` method generates a hash code based on the key for use in hash-based collections.

- The key-based approach is an effective way to manage, save, and load data for objects in a unified and organized manner.

---

**Note:** The `SavableObject` class serves as a foundation for implementing a structured and efficient save system for your application. By inheriting from this class and overriding the `Save` and `Load` methods, you can manage the saving and loading of data for various objects seamlessly.


## SaveManager Class

The `SaveManager` class is a crucial component in your application that enables saving and loading game data efficiently. It acts as a central manager for handling various objects that need to be saved and loaded.

### Save Data Management

- The `SaveManager` class provides a structured approach to save and load data. It allows the registration of objects that derive from `SavableObject`, which are equipped to save and load their specific data.

- The `RegisterSavable` method is used to register an object for saving and loading. It associates the object with methods to save and load its data. Multiple objects can be registered.

- The `UnregisterSavable` method is used to remove a registered object from the save and load process.

### Saving Data

- The `Save` method initiates the saving process for all registered objects. It calls the registered `SaveDelegate` methods for each object. These methods handle the serialization of the object's data and add it to a structured list for saving. The saving process is asynchronous and handled by tasks.

- Data is saved to JSON format using Unity's `JsonUtility` class. The saved JSON data is written to a file within a specified folder, with each save having its own filename.

### Loading Data

- The `Load` method allows loading data from previously saved files. It retrieves the saved JSON data, deserializes it, and calls the registered `LoadDelegate` methods for each registered object. These methods populate the object's fields with the loaded data. The loading process is asynchronous and handled by tasks.

- Objects are loaded based on a specified save file name. By default, the last used save file is loaded.

### Data Serialization

- Data is serialized into JSON format using Unity's `JsonUtility`. This provides a standardized and efficient way to represent game data.

### Asynchronous Handling

- The `SaveManager` uses asynchronous tasks for both saving and loading operations. This ensures that these operations do not block the main thread, allowing for smooth gameplay and interaction.

### Save Folder

- A dedicated save folder within the streaming assets directory is used to store the saved JSON files. The folder is created if it does not exist.

- File I/O operations ensure that saved data is persisted across game sessions.

### Singleton Pattern

- The `SaveManager` follows the singleton pattern, allowing for a single instance throughout the application.

---

**Note:** The `SaveManager` class simplifies and organizes the process of saving and loading game data, allowing for efficient data management and a seamless player experience. By registering objects as savable entities and handling the saving and loading processes, this class streamlines game data persistence.


## SaveData Struct

The `SaveData` struct is a fundamental data structure used by the `SaveManager` to manage and store saved data. It represents a single piece of data that is saved to a JSON file.

### Properties

- `key`: This property represents a unique key that is used to access a service's data within the JSON file. It serves as an identifier for the saved data, allowing for efficient retrieval.

- `data`: The `data` property stores the actual saved data. This data is converted to JSON format before being saved to the file. The content of `data` is specific to the type of data being saved, whether it's related to game progress, user preferences, or any other relevant information.

### Constructor

- The `SaveData` struct has a constructor that takes two parameters: `key` and `data`. When creating a `SaveData` instance, you provide the unique key and the data to be saved.

---

**Note:** The `SaveData` struct is a simple yet essential structure for managing saved data in your application. It encapsulates both the identifier (key) and the serialized data, making it possible to efficiently save and load various types of game-related information.


# UserInterface Namespace

The `UserInterface` namespace is dedicated to classes responsible for the game's user interface.

## UI_Logger_WorldBuilding Class

The `UI_Logger_WorldBuilding` class is an essential component that facilitates logging and user interface interactions within the world-building context of your application.

### Log Display

- This class is responsible for displaying logs and messages in the user interface. It acts as a bridge between the application's internal logging system and the visual representation of logs to the user.

- The class includes a reference to a `Transform` called `_logContainer` and a `GameObject` called `_logPrefab`. The `_logContainer` defines where log messages are displayed, and `_logPrefab` is used as a template for creating log entries.

### Log Type Handling

- The class provides different text colors for various log types, including track, info, warning, error, and fatal. These colors enhance the user's understanding of the log messages, making it easier to differentiate between different types of information.

- The `Log` method allows you to display log messages with the appropriate color based on their log type. The switch statement routes log messages to the corresponding methods for different log types.

### Log Creation

- The `CreateTextBox` method generates log entries with the specified text and color. It also ensures that the log display area accommodates the new log entry's height to prevent overlap with previous entries.

- The log message's text and color are configured using the Text Mesh Pro (TMP) component, providing flexibility and visual appeal.

### User Interface Interaction

- The `UI_Logger_WorldBuilding` class extends the `ILoggerUI` interface, allowing it to seamlessly integrate with other user interface elements.

- The class creates log entries as game objects within the `_logContainer`. Each log entry contains a TMP text component for text rendering. When a new log entry is added, the log display area's size dynamically adjusts to accommodate it, ensuring a smooth user experience.

---

**Note:** The `UI_Logger_WorldBuilding` class enhances user engagement by providing a visually appealing and organized way to display log messages in the context of world building. This facilitates debugging and provides essential feedback to the user.


# Language Namespace

The `Language` namespace contains classes for handling localized text in the user interface.

## ILanguageService Interface

The `ILanguageService` interface is a crucial part of your application's language localization and management system. It defines the contract that language services must adhere to, providing methods and events for language-related operations.

### Language Localization

- The interface includes a method named `GetLocalizedText`, which allows implementations to retrieve localized text using a provided key. It supports the localization of text with dynamic arguments by accepting an array of objects as parameters.

- The `GetLocalizedValue` method, which returns an `IEnumerator`, is designed for asynchronous loading of localized values. It enables efficient handling of language data loading, particularly when dealing with large language files.

### Language Change Notifications

- To notify components in your application about language changes, the `ILanguageService` defines a delegate and an event called `LanguageChangeAction`. This event can be subscribed to by other parts of your application to react to changes in the selected language.

- The `Subscribe` and `Unsubscribe` methods are used for adding and removing subscribers to the `LanguageChangeAction` event. Components interested in language change notifications can subscribe and unsubscribe accordingly.

### Language Management

- The `ChangeLanguage` method allows switching the current application language. Implementations of this interface can manage and facilitate language changes based on user preferences or application requirements.

---

**Note:** The `ILanguageService` interface plays a central role in your application's multi-language support, providing the necessary abstractions for language localization, change notifications, and language management. Implementations of this interface enable your application to be more accessible and user-friendly for speakers of different languages.

## LanguageService Class

The `LanguageService` class is a crucial component in your application that facilitates language localization and management. It enables the presentation of content in multiple languages and provides support for dynamic text with parameters.

### Supported Languages

- The `LanguageService` class supports multiple languages, represented by an enumeration named `Language`. In the provided example, English (EN) and Turkish (TR) are included. You can extend this enumeration to include additional languages.

### Localized Messages

- The class uses the `LocalizedMessage` struct to represent localized messages, consisting of a unique key and the associated text. These localized messages are organized in a package called `LanguagePackage`.

- A `LanguagePackage` is a container for localized messages and is loaded based on the selected language. It is responsible for providing localized text based on the unique keys.

### Event for Language Change

- The `LanguageService` class defines an event `OnLanguageChange` of type `LanguageChangeAction`. This event allows other parts of your application to subscribe and respond to language changes.

### Current Language Management

- The current language is managed through a serialized field `language`, which is initially set to the default language (English in this example). The class provides a method to change the current language using the `ChangeLanguage` function.

### Localized Text Retrieval

- The `GetLocalizedText` method retrieves localized text for a given key. It supports dynamic text with parameters and allows you to format the localized text with provided arguments. If a message is not found, it returns an empty string or another appropriate default value.

- This method initiates asynchronous loading of localized values to ensure efficient handling, especially with large language files. It leverages Unity's coroutine system for asynchronous operations.

### Language Change Event Handling

- You can subscribe and unsubscribe from the language change event using the `Subscribe` and `Unsubscribe` methods. These allow components in your application to react to language changes dynamically.

### Asynchronous Localization Data Loading

- The `GetLocalizedValue` method provides an asynchronous way to obtain localized values. It waits until the language package is loaded and then retrieves the text associated with the specified key. If the key is not found, it yields "Key not found."

### Loading and Saving Localization Data

- The `LanguageService` class extends the `SavableObject` class, allowing it to be saved and loaded. This enables preserving the selected language across application sessions.

- The `LoadLocalizedStrings` method loads localized strings based on the current language from a JSON file. This method is called when the language changes.

---

**Note:** The `LanguageService` class is essential for creating a multilingual application. By providing abstractions for localized text retrieval, language change events, and data loading, it simplifies the process of supporting multiple languages and ensures a user-friendly experience for diverse audiences.

## LocalizedText Class

The `LocalizedText` class plays a vital role in your application's language localization system. It allows you to dynamically update the text of UI elements with localized content.

### Text Component Binding

- The class is designed to be attached to Unity's Text Mesh Pro (TMP) `TMP_Text` components. It binds to the text component of the GameObject to which it is attached.

- A serialized field `_key` is used to specify the unique key associated with the localized text. This key is used to fetch the appropriate localized text from the language service.

### Text Update on Language Change

- In the `Start` method, the class retrieves the `TMP_Text` component and calls the `UpdateText` method to set the initial text content based on the selected language.

- The `UpdateText` method initiates an asynchronous text update through the `UpdateTextCoroutine`. It subscribes to the language change event provided by the `ILanguageService`, ensuring that text updates occur when the application's language is changed.

- The `UpdateTextCoroutine` method retrieves the localized text for the specified key asynchronously. It waits for the result of the asynchronous operation and updates the text component with the new localized content.

- If the text component is not null and the asynchronous operation returns a string, the text component's text is updated with the localized text.

### Unsubscribing from Language Changes

- To prevent memory leaks and ensure efficient event handling, the class unsubscribes from the language change event in the `OnDestroy` method. This is essential to release resources and maintain clean event subscriptions.

---

**Note:** The `LocalizedText` class simplifies the process of updating text components with localized content. By binding to a specific key and subscribing to language changes, it allows for real-time language updates in your application's user interface.

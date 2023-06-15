Fog Yeah! Thank you for purchasing Fog Yeah: The Ultimate Shaderless Fog Solution!

There are a number of Tutorial scenes and Demos available in the Examples Folder.
If you have any questions, feature requests or if you have found a bug then please send me an email at:
	irishjohngaming@gmail.com
- Or contact me directly on Discord:
	JohnDong#6317

Quick Start Guide
	Fog Yeah was created with a view of ease of use. 

	2D:
		To create an initial 'playground' where you can play around with all the features safely:
			Create a new scene.
			Drag the prefab 'FogSystem' from FogYeah / Prefabs into the heirarchy.
			Finally, Click the 'Generate Tiles' button in the component of the FogSystem. 

			You should now see the generated fog of war. 
			Now you are ready to add an effector. 
			Effectors are transforms that can reveal the fog of war. These can be moving or static objects. 
		
			Create a new game object, reset its transform or set its position to 0,0,0 and change its tag to 'Player'.
		
			If no effectors are specified in the FogSystem Effectors array, the Fog System can generate them at startup by finding any tagged 'Player' objects in the scene.
			If you would not like this to happen, disable the 'Auto Create Effector From Players' option in the FogSystem.

			Now you can run your scene and move around your player gameobject using the editor to see the fog disappearing around them.

			This concludes the Quick Start Guide. All the features available in the FogSystem have tooltips associated with them. Hover each feature to read more about what they do. 

	3D: 
		Some slight differences are needed to set up for a 3D scenario.
			Create a new scene.
			Create a Terrain.
			Drag the prefab 'FogSystem' from FogYeah / Prefabs into the heirarchy.
			Click the Switch to 3D Mode button, if it is not already in 3D mode. 
			Assign the variable "Terrain Map To Cover" under "General 3D Setup Requirements" > "3D Settings" by dragging the Terrain onto it.
			Finally, Click the 'Generate Tiles' button in the component of the FogSystem. 

			The rest is exactly the same as the 2D variation. Here is the rest of the steps for your convinience:
				You should now see the generated fog of war. 
				Now you are ready to add an effector. 
				Effectors are transforms that can reveal the fog of war. These can be moving or static objects. 
		
				Create a new game object, reset its transform or set its position to 0,0,0 and change its tag to 'Player'.
		
				If no effectors are specified in the FogSystem Effectors array, the Fog System can generate them at startup by finding any tagged 'Player' objects in the scene.
				If you would not like this to happen, disable the 'Auto Create Effector From Players' option in the FogSystem.

				Now you can run your scene and move around your player gameobject using the editor to see the fog disappearing around them.

				This concludes the Quick Start Guide. All the features available in the FogSystem have tooltips associated with them. Hover each feature to read more about what they do. 

Functional Documentation
	Core Systems and Functions
		Fog
			Generation and Removal
				The fog system provides two buttons that are intended to be used after you have added your images, configured your placement options and sizes. 
				The 'Generate Fog' button will create your Fog Tiles and place the map if it is not added already. 
				For best results, generate this in a new scene, customise and re-generate to your hearts content, and then bring the 3 items (FogSystem, FogImageTileCanvas and MapCanvas) into the scene you wish to use them in. 
				
				The 'Remove Fog' button will remove the fog from the scene. Like an Undo feature. 
				This button is especially useful for tweaking, as you can change some colors, remove the old fog and re-generate in very little time. 
			
			Fog Regrowth
				See General setup > Regrowth Mode

		StartMode
			The start mode determines when the fog system will begin working at runtime.
			Awake
				Most common use cases will use this mode. This will begin the runtime fog activity on awake.
			Start
				This mode should be used if you wish to use Event Hooks. See EventHooks, and the event hook example (12_Under_Fog_Extension_Example).
			Enable
				This will make the fog system initialise when the gameobject or component first becomes enabled.
			Manual
				This mode will prevent the fog system from activating automatically, and will instead become active only after calling FogSystem.Instance.Initialise();

		Effectors
			Effectors is the name given to any transform that can 'effect' or reveal the Fog. 
			There are two types of effectors, 'Normal/Dynamic' effectors [known as simply 'effector'] and Static Effectors. 
			These are moving effectors. Functionally, there is no difference between these two effectors. For more information - see the Code documentation.
			Here is a brief overview of the most important differences between the two:
			Normal Effectors
				These are 'Moving' effectors, or effectors that Can move. A change in movement of any of these effectors, will cause the fog system to refresh/recalculate and redraw effected areas.
			Static Effectors
				These are 'Not Moving' effectors. This can be used for Buildings or other static elements that you would like to reveal an area. 
				A movement in one of these effectors will -NOT- cause a refresh of the fog system.
				Static effectors should only be assigned if the effector cannot or will not move. 
				NB. If a static gameobject is assigned as an Effector, it will automatically be added to the effector list as a Static Effector.

		EventHooks
			Events are in place that can be hooked in order to add custom behavior to Fog Yeah.
			These are the events currently available:
				OnStateChangedForFogItemIDs
					This method is called every time Fog Yeah has actioned fog tiles.
					For example, when the Fog is changing from fully visible to semi-transparent.
					This event will populate a list of Fog Tile ID's, and the state of which these tiles have changed to.

				OnFogSystemHasStarted
					This hook should only be used if you know you can hook to it before the initialisation of the Fog System. I.E. when you are controling the Start Mode, or activation. 
					It is best to use this with StartModes other than Awake. 
					This event will fire once the fog system has finished initialisation, and has started to action fog items.

				Usage: 
				    FogSystem.Instance.EventHooks.OnStateChangedForFogItemIDs += ActionStateChangesToFogSystem;
					FogSystem.Instance.EventHooks.OnFogSystemHasStarted += StartTrackingItems;

			For an example of Event Hooks in use, see the example scene: 12_Under_Fog_Extension_Example

	General Setup 2D
		Map Axis
			This determines how the map and fog system will be drawn.
			Most commonly, in 3d setups, you should choose XZ.
			Adversly, in 2d setups, you should choose XY. 
			Especially if you are using Orthographic cameras.

			Please note that this is just for the initial Generation of the fog/map. 
			The canvasses are created in World space by default, so you can move them around as you please. 

		Map Sprite
			This is the sprite that your map will display. 
			NB. The map sprite will be added into the map canvas. Native size must be used. See Limitations.

		Regrowth Mode
			There are 3 different 'Modes' to fog regrowth.
				1. Regrowing
					The fog will reappear as origonally placed when an effector moves past it.

				2. Shrouding
					The fog that was visited, but is no longer in vision range of an effector will remain semi-transparent, or shrouded after an effector moves past it. 
					This is the slowest of the 3 methods. Although the operation is threaded, the operation requires time to compute opacity.
					The amount of opacity shown in a 'shrouded' element is driven by the settings > Shroud amount. 

				3. None
					The fog will not reappear after an effector moves past it. 
					This is the fastest of the 3 methods and can be used with an larger amount of effectors. 

		Shroud Amount
			This is the amount of opacity that will remain after a fog tile becomes shrouded. 
			For more information on shrouding, see General setup > Regrowth Mode > Shrouding.

		Fade Speed
			This is the speed in seconds that the system will wait between each iteration. 
			NB. This value is also dependant on the speed it takes for your fog to update. 
			As this system is not using a shader, large amounts of effectors can cause this value to seem like its 'expanding'.
			The real cause here is not that the value is changing behind the scenes, but that the working thread does not finish within the delay timeframe. 
			Consider increasing this value only if you have alot of effectors and you have large sight ranges. 

		Refresh Delay
			This is the delay in seconds between each fog change search iteration. 
			Between each search, this value is used to delay the next search. 
			A larger value will increase performance, but may slow down fog regrowth or shrouding.

	General Setup 3D
		Fog Tile Prefab
			This is the 3D Prefab - Some default examples of these can be found in FogYeah > Prefabs > Dependants > FogTiles - prefixed with a 3D
			Ensure that the model you are using has a Transparent Material assigned if you wish to use Shrouding

        Terrain Map To Cover
			This is the in-scene terrain that you would like to cover in fog.

        Regrowth Mode
			As above - No different from 2D variation.

        Shroud Amount
			As above - No different from 2D variation.
        
		Fade Speed
			As above - No different from 2D variation.
        
		Refresh Delay
			As above - No different from 2D variation.
        
		Use Colors From List 
			Choose this option to change the colors of the assigned Fog Tile Prefab at runtime. 
			This works slightly different from the 2D variant as the color is applied to the instance at runtime, rather than the material in the prefab.
			This is to protect the prefab from unwanted color changes, and to prevent the need for physical materials.
			Changes to this can be made if requested.

        Fog Tile Color Variations
			[Linked to above checkbox - used if checked, ignored if false.]
			These are the colors that the system will generate the fog using.
			At runtime Awake, these colors will be applied to each prefab randomly.

        Randomise Color HSV
			This option will apply random colors to the tiles at runtime Awake.

        Random Scattering
			As above - No different from 2D variation.

        Randomise Rotation
			As above - No different from 2D variation.

        Grid Size
			As above - No different from 2D variation.

	Appearance
		This section is filled with customise options for the appearance of the fog tiles during generation.
		Fog Tile Color Variations
			These allows you to add as many colors as you would like in order to manipulate each tile randomly based on these colors.
			These colors will be painted randomly onto the images you choose in Fog Tile Image Variations below. 

		Fog Tile Image Variations
			These allow you to specify as many images as you would like to be randomly chosen and tiled to fill the Map Sprite chosen.
			For best results:
				- Use pure white images with transparency if you want to use Fog Tile Color Variations as your primary colors.
				- Or use your colored images with one variation of white and the system won't change the color.
			If you want to use your own tiles, we recommend you create 200x200 pixel images, with between 3-5 in the Placement Options > Density setting. Then add them to a sprite atlas, as in the UI/...tiles Folder. 
			For more information on sprite atlases, please use the Unity Documentation.

		Randomise Color
			This will paint your images using random colors from RandomHSV. 
			This setting overrides the Fog Tile Color Variations but does not clear them out in settings. 

	Placement Options
		This section controls how to grid of fog tiles will be placed. 
		The default values in these settings should be good in most cases using a 4k map size. 
		Most importantly, these values should be tweaked so that the Grid Size and Density are as low as possible, but with the desired effect. 

		Random Scattering
			Scattering of the tiles randomises in what order they will be placed. 
			Disabling this option is useful if you want your tiles to appear as they are stacked like a spread out line of playing cards.

		Randomise Rotation
			This will rotate each tile randomly.
			Disabling this option is useful if you want to use Pixel art or some tiles that appear better when rotated the same way. 

		Grid Size
			The fog system will generate a square grid to cover the Map Image, regardless of the shape of the Map Image. See Limitations.
			This value will tell the system to generate a grid of Grid Size x Grid Size.
			e.g. A grid size of 50 will cause 2500 tiles to be created as 50x50 = 2500.

		Density
			This is the amount of overlap the grid should use when placing the tiles.
			For smooth edges on your fog, consider increasing this value. 
			Please see Limitations for this as the higher the density, the more slowdown occurs. 

		NB. A higher Grid Size and Density can be used to cover 'holes' in your fog, but will cause computations to slow down.

		Limitations
		Approximitly 30 effectors can be used with the Shrouding effect currently.
		A large Grid size and Density size can be slow to generate.
		A large amount of effectors can cause the shrouding effect to take a long time computationally.
		The fog system works with Native Size images. Resizing your image and regenerating the fog way cause unwanted effects. 

Code Documentation
	The code is commented in full but here is a quick overview of how the system ties together. 
	All processing is done by threads kicked off by checkers in the Late Update of FogSystem. 
	No other update functions are used. 
	No overlapping of threads is possible in the system. 
	Errors in the system will result in the fog system being 'Deactivated'. This is controlled by a boolean in FogSystem.cs, FogSystemActive.

	FogSystem.cs
		This is the core of the system.
		It houses the 'Generate fog' and 'Remove fog' buttons used by the Editor Script and also the Awake methods and LateUpdate that drive updates and utilise the other classes in the system. 

	Generators
		FogGenerator.cs
			This is the base class for all generators.
			This is used by the FogSystem to generate or remove fog based off the current selected mode.

		FogGenerator2DMapSize.cs
			This is the abstracted Map Size element of the generators.
			This uses its own method to create fogs from Tiny to Massive.

		FogGenerator2DMapSprite.cs
			This uses a map sprite as the base to create fog around.

		FogGenerator3D.cs
			This is the terrain covering fog generator. 

	Settings
		These classes are described in the component that can be manipulated to change how the fog is generated, and how the system performs.
		Most of these fields are self explanatory and serialised with a tooltip for further information.
		The settings for each Mode are created in line with the class name for each modes' generator.

	FogSystemValidator.cs
		This validates operations and ensures no errors will happen in generation / deletion / execution.

	FogItem.cs : IFogItem.cs
		FogItems are 'Fog Tiles'.
		These are the in memory components that keeps track of each fog tile. 

	FogItem3D.cs : IFogItem.cs
		FogItem3Ds are '3D Fog Tiles'
		To the fog system, these are no different from their 2D counterpart, but they have different implementations of the same methods.
		For example, where FogTile uses Images opacity to 'grey' out tiles, the FogTile3D uses a Mesh Renderer's color opacity value.

	FogSystemEffector.cs
		This is the class file that handles all the Line of Sight and actions for each effector. 

	FogSystemSearchHelper.cs
		This class scans for changes in the fog tiles.
		It creates lists that the ActionHelper can act upon.
		This script contains threading.

	FogSystemActionHelper.cs
		This class acts upon tiles that require action. 
		i.e. Smoothly changing a tile from Transparent to Darkest.
		This class also contains threading.

FAQ
Q.	What is the quickest way I can start playing around with this.
A.	Open a new scene and drag the FogSystem prefab into it. 
	Click the Generate Fog button in the FogSystem component.
	Place a gameobject tagged 'Player' into the scene and hit Start.

Q.	Why is my map bigger than I expected.
A.	The system will set the map to the Native Size it was created in.  
	You will need to use some image editing software to bring the size of the image down. (Always make a backup!)

Q.	I want to use the map already in the scene, Why is there two maps?
A.	Please replace the map image in the settings with your sprite, and use the canvas provided by the generator. 

Q.	Can I place this on a wall? Or table?
A.	Yes, The map & Fog are created in world space, so you can place them wherever you want.

Q.	Is the fog of war visible from the back of the map?
A.	The fog of war will effect both sides of the map. 

Q.	How do I remove this asset?
A.	Simply delete the FogYeah folder from your solution.

Q.	Are animated images supported?
A.	Animated images such as .gifs are not supported yet. I hope to work on this in future, and if possible, add it to this package in a free update.

Q.  Is HDRP supported?
A.	Yes, Same workflow in both HDRP + URP.

Q.	Are 2D and 3D project structures supported?
A.	Yes, Same workflow for both, but change the Axis setting. As with all settings, Mouse over the setting title for a help. See the 2D demonstration in the examples for an orthographic camera, 2D setup. 

Q.	How does the system perform?
A.	See for yourself in the profiler. Very low garbage is generated and cleanup happens almost immediatly and it has minimal impact on the CPU. 

Q.  So i am using the asset with PhotonPUN2, but your system doesnt seem to pickup the player as an effector once its spawned. 
A.  As your spawning your effector after Start, you'll need to add it programmatically to the FogSystem once it spawns. For an example of this, check the effectors being added at runtime in the sample scene. Also, near the end of the Tutorial #1 video.

Q. Does you system only look for objects tagged player on start? 
A. Yes there is a feature that can automatically will do that for you - You can turn that off if you don't want it to work like that. You can add pre-existing gameobjects the Effector array. 

Q. Also what is the component i would add to the player. from the demo examples i can only see an example script. 
A. There is no component/mono to add to anything. Just use the FogSystem.Instance.Add (The transform you want to add) to add an Effector.

Q. Is there an official effector script? 
A. As above, I don't like having alot of mono's on a gameobject, so its designed with a way to add any transform programatically as above :) 
	If you prefer to add it using a component, just throw the FogSystem.Instance.Add(this.transform) into a startmethod on a mono and it'll add it when instantiated.

Q. You also state 3D in your description on the asset but it looks to be a 2D plane in a world canvas. 
A. 3D Camera support or 2D Orthographic support is what I mean by this. I have clarified in the asset description (*Thank you!)

Q. My game is first person shooter so doesn't seem to be 3D, unless I am missing something.
A. Yes, its meant more for Maps and things like that, its not somethign that will fog your vision at a certain distance or something like that. The 3d part is that you can place that map anywhere in your game.

Q. I want the Fog to cover 8192 X 8192
A. By default, Unity maxes out images at around 2K size. You can override this in the project view by selecting the sprite and going to the settings.

Q. I would like for some items to be able to see further than the player.
A. Each effector has individual Ranges for how much they can reveal. See the readme for Effectors.

Q. I would like to change the vision range of the player if they have some equipment.
A. You can change the effector sight range as you wish at runtime. 
	You can take a Remove/Add style approach by removing the effector, and then re-adding it with the new sightrange if you wish. 
	The Effector array is also accessable from the fog system instance singleton, so you can modify it as you wish using this either.

Q. I wish to create a 2D Map of certain size without having to use the Map Image.
A. Change the setting under 2D Settings > Placement > Use Map Sprite or Map Size to 'Map Size'. 
	Now you can change the X/Y values in Map Size to the height and width of your choosing. 

Q. I wish to have objects under the fog hide and unhide.
A. There is an example of integrating using Fog Yeah's event hook system included. Please see 12_Under_Fog_Extension_Example

ChangeLog
2.3.0
- Improved UI interfaces.
- Added version number to the upper left of logo.
- Added tabs system and removed the dropdowns from the editor UI.
- Reorganised & refactored some scripts for quality of life.

2.2.0
- Small Hotfix for issue with new EventHook example scene.

2.1.0
- Added EventHooks
	- Public events have been added to allow for ease of integration with various use cases.
	- New example scene displaying the new event hook system: 12_Under_Fog_Extension_Example.
- More performance improvements.
- Updated documentation and cleaned up some code comments.
- Added a StartMode feature.
	- Choose when to activate the Fog System, on Awake, Start, Enable or Manually.
- Refactored in some areas.
- Fixed a bug with the 3D Fog of War Regrowth mode settings. 
	- Prefiously, the Regrowth mode for 3D fog was being taken from the 2D common settings.
	- Now all settings have been consolodated for common settings correctly.

2.0.0:
- Massive refactoring of the system. Please revisit scenes / prefabs using the fog system and verify the settings are still correct.
	- Moved every mode into their own dedicated class. 
	- Seperated all generators and their settings.
	- Abstracted the generator logic and implemented base class.
	- Moved all settings and generators into their own folders.
	- Cleaned up all editor sections.
	- Moved all serialised field sections to their own classes.
	- Moved MapSize mode into its own seperated section from 2D Map Sprite.
- Fixed a bug in the 2D Map Size mode that prevented very tiny maps from being possible.
	- Recreated the 2D Map Size mode using a more rigid process.
	- Created a different style of density to be applied in Map Size mode.
	- Added more safety around the map size method.
- Added a remove map sprite button to the Map Sprite method.
- Fixed a bug with the 2D Map Sprite method where the Map Sprite could not be found, but the fog system was expected. 

1.3.0:
- Added a Vector 2 'Map Size' Option.
	- You can now create any size fog system in your game. 
	- Instead of using the Image system, you can now choose 'Map Size' from the 2D Placement Options.
	- This will disregard the Map Sprite and instead, create a grid with the size, density and grid size you provide.
	- This allows for any size map to be generated, without having to resize the map image out of band.
	- It also allows for rectangular shapes.
	- Compatible with XY and XZ 2D modes.

- Updated documentation
- Some small refactorings done to code.
- Repositioned some editor sections.
- Added a new Example scene to show off the new Map Size option.

1.2.0:
- Added "3D Mode"
	- Added 3D Fog Item support
	- Added 3D fog of war support for Terrains
	- Added "3D Mode" Editor UI and Toggle
	- Added one example scene covering a terrain in 3D fog of war
	- Added infrastructure for 3D Mesh coverage
	- Added 3D Fog effector example materials

- Added 3D / 2D region markers in fogsystem
- Moved and seperated Settings structures for both 2D and 3D

- Seperated more code functions related to different modes
- Improved 2D performance in some scenarios
- Cleaned up some awake functions of the Fog System
- Removed need for manual updates to some settings
- Improved thread safety of Searcher and Actioner
- Moved Effector code

- Updated documentation & comments on code
- Added more infrastructure to enable further customisation
- Added some backwards compatability modifiers

- Improved Editor UI

1.1.0:
- Added support for smaller fog tiles
- Added support for floating points as effector sight ranges
- Updated readme FAQ.

1.0.0:
- Initial Release
- Features included:
	General
	- Automatic Map and Fog Generation
	- Regrowth modes:
		- Shrouding
		- Regrowing
		- None
	- Shroud Amount
	- Refresh delays with Multithreading

	Appearance
	- Fog tile Color variations
	- Fog tile Image variations
	- A Random Color (From HSV) Button

	Placement Options
	- Scattering (Remove unwanted visible tiling in smaller maps)
	- Random Rotation
	- Grid size
	- Density

	Setup Options
	- Auto create effectors at startup from gameobjects tagged "Player"
	
	Effectors
	- Create example effectors
	- An effector array where you can specify the effectors already in the scene visually

	Buttons
	- Generate Fog Tiles - creates the map (if not already added) and the fog tiles
	- Remove Fog Tiles - a general 'undo' button that removes all things generated by the Fog

	Code
	- Multi threading where available
	- Members to add and remove effectors at runtime
	- Singleton instance for ease of use i.e.
		- FogSystem.Instance.AddEffector(gameObject.transform, 1000);
		- FogSystem.Instance.RemoveEffector(gameObject.transform);
		etc
	- Extensive documentation & comments
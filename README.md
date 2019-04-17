# HexfallDemo

<h4> Unity Version: 2018.3.12f1</h4>

<h4>Platform: Android </h4>

A demo project.

Core mechanics: %100 working..

Grid system: %100 working..

Scoring: %100 working..

Bomb hexagon: %99 working..(Small issue: If another match happens right after a bomb is created, the game will lower the timer by 1.)

Game over logic: There's a small bug on this issue. Sometimes algorithm will pick wrong 3 tiles as potential matches. This happens on 1-2 certain patterns only. Other than that it's also working perfectly.

<b><h2> Class References </b></h2>


<b>GridManager.cs</b>: Responsible for creating and populating the grid with tiles. Selecting tiles, rotating and game over logic happens here.

<b>Tile.cs</b>: Class for a single tile. Holds the row and column numbers as well as the color info. Have some useful functions in it such as switching places with another tile.

<b>Bomb.cs</b>: Derives from Tile.cs. Holds a few useful info for bombs only. Has a Tick method in it that counts down the timer. 

<b>TileGroup.cs</b>: The group of tiles selected when user touches the screen. Holds 3 tiles as well as some useful functions for the group.

<b>TileMatrix.cs</b>: Holds the actual grid information and references to all the tile objects. Responsible for matching, collapsing columns when a match happens and all kind of useful things happen on the grid.

<b>UIController.cs</b>: A very basic controller for ui operations. Displaying score, restarting the game, opening the menu etc.

<b>Helpers.cs</b>: Contains some useful mathematical operations that can be used anywhere.

<b>CustomInputManager.cs</b>: Contains input logic used in the game. Fires 2 events. OnScreenTouch and OnScreenDrag. Calculates the swipe direction made by the user aswell.

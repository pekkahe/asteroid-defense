# Asteroid Defense
Asteroid Defense is a [Unity](https://unity.com/) exercise project that simulates an asteroid defense system for a space station.

The space station is located at the center of the screen and the asteroids, which are spawned offscreen, are floating in space around it.
The station radar scans new asteroids periodically for their locations and velocities. 

After scanning an asteroid, it immediately decides whether the trajectory of the asteroid comes within a given safety distance to the space station, and if it does, shoots a torpedo to blow it up.

The assumptions and requirements of the simulation:

- The space station has a fixed position. 
- The safety distance is a fixed value.
- The torpedo launcher can instantly shoot in any direction. 
- Torpedoes have a fixed speed and cannot propel themselves.
- There is no gravitational force, therefore every object must move at a constant velocity once created.
- The user must be allowed to spawn asteroids interactively.

For video and screenshots of the game, please visit [pekkahellsten.com](https://pekkahellsten.com/).

## Controls
Hold the left mouse button to spawn an asteroid.

Drag and release to launch the asteroid.

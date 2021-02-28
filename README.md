# Asteroid Defense
Asteroid Defense is a small [Unity](https://unity.com/) exercise project that simulates an asteroid defense system.

## Details
The defense system of the space station consists of a radar and a torpedo launcher. The outer space is full of debris and rocks floating around, but we don't know about them until they show up on the radar. 

Every once in a while, the radar detects some object and it tells us its exact position and velocity at that moment. We immediately have to decide whether the trajectory of the object comes within a given safety distance to the space station, and if it does, shoot a torpedo right away to take it down. 

Assumptions and requirements: 

- The space station has a fixed position. 
- The safety distance is a fixed value. 
- The torpedo launcher can instantly shoot in any direction. 
- Torpedoes have a fixed speed (S) and cannot propel themselves. 
- There is no gravitational force, therefore every object must move at a constant velocity once created. 

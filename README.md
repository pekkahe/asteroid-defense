# Asteroid Defense
Asteroid Defense is a small [Unity](https://unity.com/) exercise project that simulates an asteroid defense system.

## Assignment details
You have to program the defense system of a space station, which consists of a radar and a torpedo launcher. The outer space is full of debris and rocks floating around, but we don't know about them until they show up on the radar. Every once in a while, the radar detects some object and it tells us its exact position and velocity at that moment. We immediately have to decide whether the trajectory of the object comes within a given safety distance to the space station, and if it does, shoot a torpedo right away to take it down. 

Assumptions and requirements: 

- The space station has a fixed position. 
- The safety distance is a fixed value. 
- The torpedo launcher can instantly shoot in any direction. 
- Torpedoes have a fixed speed (S) and cannot propel themselves. 
- There is no gravitational force, therefore every object must move at a constant velocity once created. 
- The torpedo launcher logic must contain methods with the following signatures (it's okay to extend the return values to contain more information if necessary): 
- Return true if the object is bound to cross the safety zone, false otherwise: 
bool DoesTrajectoryEnterSafetyZone(Vector3 objectPosition, Vector3 objectVelocity); 
- Return the velocity needed to hit the target (with the given current position and velocity) considering that torpedoes travel with speed S:
Vector3 CalculateTorpedoVelocity(Vector3 targetPosition, Vector3 targetVelocity);
 
Implement a simulation that runs and displays the above scene. Allow the user to spawn debris interactively (preferably being able to control both P and V at will in some manner). For bonus points try to signal somehow in advance whether it's possible to shoot down the object before it enters the safety zone. You can use any platform of your choice, but if it's not obvious how to run it (e.g. Unity3D or WebGL), then please provide build instructions.
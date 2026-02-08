# Last Light

"Last Light" is an immersive first-person survival horror experience developed in Unity that challenges players to navigate a atmospheric and dangerous world.
## 🛠️ Tech Stack
Engine: Unity 2022.3 LTS (or newer)

Language: C#

Version Control: Git / GitHub

## 🕹️ Controls

## Controls

| Category     | Key / Input        | Action                                           |
|--------------|--------------------|--------------------------------------------------|
| Movement     | W, A, S, D         | Walk / Run                                       |
|              | Spacebar           | Jump                                             |
|              | Mouse              | Look around                                      |
| Combat       | Left Click         | Attack (Shoot Gun / Swing Axe)                   |
|              | R                  | Reload Gun                                       |
|              | 1                  | Equip Gun                                        |
|              | 2                  | Equip Axe                                        |
| Interaction  | E                  | Interact (Pickup items, open doors, talk to NPC) |
| System       | ESC                | Pause Game                                       |


## 📋 The Game Plan (Gameplay Loop)

### Level 1: House Escape

1. **Wake Up** 
   - Cinematic camera rise
   - Objective: "Exit the house from the main gate"

2. **Find Axe** 
   - Located in outside storeroom
   - Objective: "Find the Axe"

3. **Gather Wood** 
   - Chop 10 trees
   - Each tree drops 1 log
   - Objective: "Gather Wood (0/10)"

4. **Trade with NPC** 
   - Give 10 logs
   - Receive Gun
   - Gun auto-equips
   - Objective: "Trade logs with NPC"

5. **Kill Zombies** 
   - Kill 10 zombies
   - 10th zombie drops Key
   - Objective: "Kill 10 Zombies (0/10)"

6. **Reach Fire Tower** 
   - Use Key to unlock door
   - Objective: "Find the Fire Tower"

7. **Enter Portal** 
   - At top of tower
   - Loads BossScene
   - Objective: "Enter the Portal"

### Level 2: Boss Arena

8. **Boss Fight** 
   - Teleport to arena
   - Fog disabled
   - Zombie spawning stops
   - Boss activates
   - Objective: "Kill the Boss Zombie"

9. **Victory** 
   - Boss defeated
   - Victory screen
   - Game end

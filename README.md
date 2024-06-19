Welcome to the 2D Unity Tower Defense Demo! This is a short demo showcasing a basic tower defense game built using Unity. The project includes essential features such as enemy pathfinding, tower placement, and upgradeable towers.


## Introduction

This project is a simple 2D tower defense game developed in Unity. The primary objective is to prevent enemies from reaching the end of the path by strategically placing and upgrading towers.

## Features

- **Enemy Pathfinding**: Enemies follow a predetermined path from start to finish.
- **Tower Placement**: Players can place towers anywhere on the map, except directly on the enemy path
- **Upgradeable Towers**: Towers can be upgraded to enhance their capabilities. 
- **Various Enemy Types**: Different enemies with unique attributes.
- **Basic UI**: Interface to manage game elements like health, currency, and tower upgrades.
### Stat Management:
This Tower Defense game is built on a robust and extensible stat and buff system, enabling dynamic and flexible gameplay enhancements. The core architecture revolves around a StatManager and BuffManager, which together facilitate seamless stat modifications for projectiles, turrets, and enemies.
**StatManager**: 

 - Manages various stats (e.g., health, damage, speed) for game objects.
 - Provides methods to add, update, and retrieve stats, ensuring consistent and centralized stat handling.
 - Serialized Stats: Uses serialized dictionaries to store stats, allowing for easy customization and scalability through the Unity Editor.
   
**Buff System**:
- BuffManager: Handles the application of temporary and permanent buffs. Buffs can modify stats through additive, multiplicative, or set operations.
- Timed Buffs: Supports buffs with durations, applying their effects for a specified time before automatically removing them.
- Event-Driven: Utilizes events to notify other systems when buffs are applied or removed, promoting a decoupled and flexible architecture.

## Play Now!
If you would like to play directly without the use of the Unity Editor, you can play it directly on ([itch here](https://la-jer.itch.io/tower-defense-demo))!



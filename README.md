<div align="center">

# Sword Hero | Madbox Senior Game Developer Assignment

![Madbox](https://img.shields.io/badge/Madbox-Senior%20Game%20Developer%20Assignment-FF6B6B?style=for-the-badge)
![Unity](https://img.shields.io/badge/Unity-2022.3.17f1-000000?style=for-the-badge&logo=unity)
![Time](https://img.shields.io/badge/Development-~13%20Hours-4ECDC4?style=for-the-badge)

</div>

---

## Summary

Mobile game prototype founded on clean architecture and advanced patterns such as MVC, VContainer DI, and Object Pooling, utilizing Addressables asset management.

### Table of Contents
1. Assignment Overview
2. Architecture Approach
3. Development Phases
4. Technical Challenges
5. Next Steps

---

## Assignment Overview

### Core Features

**Character Movement**
- MVC touch-based joystick system with cross-platform input abstraction
- Supports both mobile touch and desktop mouse through IJoystickService interface

**Enemy Spawning & Behavior**
- Object pooling system using Repository pattern for memory optimization
- Follow-player behavior with collision-based damage mechanics

**Auto-Attack System**
- Closest target detection with radius-based enemy selection
- Use Case pattern for coordinating attack timing and damage application

**Three Weapon Types**
- ScriptableObject-based weapon configurations affecting player stats
- Dynamic weapon switching with animation controller integration

**Addressables Asset Management**
- Custom asset loading pipeline with progress tracking
- Asset caching system preventing duplicate loads

### Bonus Features

**Enemy Variants System**
- Multiple enemy prefab variants configurable through PoolableRecipe ScriptableObjects
- Random enemy type selection during spawn operations

**In-Game UI**
- Health bar system with real-time updates
- Gold collection and shop interface with purchase validation

**Loot Economics**
- Enemy death events trigger gold rewards through MessagePipe
- Configurable gold values per enemy type

**Extraction Point Shop**
- Trigger-based zone detection for shop access
- Game pause functionality during shopping phases

### Architecture Approach

**MVC Pattern Implementation**
- Every major system (Pawn, Joystick, GameLoop, Weapons) implements strict Model-View-Controller separation for maintainability.
- Models contain pure business logic and state management without Unity-specific dependencies enabling easy unit testing.
- Controllers orchestrate between Models and Views while handling dependency injection and lifecycle management through VContainer integration.

**Dependency Injection Strategy**
- VContainer chosen over Zenject for superior performance characteristics and native Unity integration capabilities.
- Service registration occurs in GameLifetimeScope with proper lifetime management for singleton services and transient components.
- Constructor injection pattern enforced throughout codebase ensuring explicit dependency declaration and improved testability.

**Event-Driven Communication**
- MessagePipe integration provides zero-allocation event publishing with automatic subscriber management and type-safe message handling.
- Use Cases handle complex multi-step operations requiring coordination between multiple systems and services.
- Event publishing used for lightweight notifications while Use Cases manage stateful operations requiring transaction-like behavior.

---

## Development Phases (Time Breakdown)

**Architecture Foundation (~1h)**

I started by designing a solid, scalable architecture that would showcase both technical skills and mobile game development best practices. I chose MVC combined with dependency injection because of their strong synergy and my comfort with these patterns. The goal was to demonstrate not just what I could build, but how I approach architectural decisions for production mobile games.

**Addressables System (~1.5h)**

Asset management was my first priority, knowing it's critical for scalable mobile games. I noticed from playing Pocket Champs that Madbox likely uses Addressables extensively, so I built a comprehensive loading system with progress tracking. This foundation proved essential for everything that followed.

**Input & Movement (~2h)**

I leveraged an existing joystick architecture from a recent project. This allowed rapid implementation while maintaining clean separation of concerns. The cross-platform input abstraction was straightforward once the foundation was solid.

**Object Pooling Challenge (~3h)**

The most complex part was integrating VContainer's automatic lifecycle management with manual pooling requirements. The fundamental conflict between DI container behavior and pooling semantics required a custom intermediary system. I adapted an existing factory-pattern solution, using Claude Code AI assistance to optimize the integration. If I were to redo this, I'd consider limiting VContainer to UI systems and using a different approach for gameplay objects.

**Rapid Feature Development (~2h)**

Once the pooling system worked, scaling became remarkably fast. Enemy movement took about 15 minutes, the weapon system under 45 minutes, and health/damage systems about 30 minutes. The architecture paid off here - adding new features became trivial.

**Game Loop & Polish (+3h)**

I implemented basic state management rather than a full state machine, given time constraints and the focused scope. The shop system and pause functionality were the final pieces to create a coherent gameplay loop. I dedicated additional time beyond the initial 10-hour estimate to refactor and polish the services, as I felt delivering a solid, production-quality project was a must.

---

## Technical Challenges

### The VContainer + Object Pooling Dilemma

This was where I hit my major architectural wall. I had committed to both dependency injection for clean architecture and object pooling for performance, but they fundamentally don't play well together. VContainer wants to control object lifecycles automatically (create once, dispose once), while Pooling demands the opposite (create once, spawn/despawn multiple times with manual control).

The tension was immediate. How do you inject dependencies into an object that gets recycled? When does initialization happen? Who owns the lifecycle? I realized I was dealing with two competing paradigms that needed a bridge.

My solution was creating an intermediary system; a RepositoryFactory pattern that acts as a translator between VContainer's world and the pooling world. Pooled objects get their Initialize/Dispose calls manually at spawn/despawn moments, while still benefiting from constructor injection for services.

Looking back, this taught me an important lesson about architectural decisions. Sometimes the "clean" solution (pure DI everywhere) conflicts with the "performant" solution (pooling). The real skill is finding the compromise that gives you both benefits. Though honestly, for a future project, I'd probably keep VContainer for UI systems and use a simpler approach for gameplay objects to avoid this complexity entirely.

### Addressables Asset Management

The challenge was building something that felt production-ready, not just functional. I needed progress tracking for loading screens, caching to prevent duplicate loads, and a clean interface that could handle any asset type. The AddressableCatalog ScriptableObject became my designer-friendly solution, letting you configure what loads when without touching code.

What took longer than expected was getting the threading right. Asset loading can't block the main thread, but progress updates need to reach the UI safely. The IProgress<float> pattern solved this, though it required careful handling of the async operations.

---

## Next Steps

Looking at what I've built, there are several areas where I'd expand the game if I continued development:

**Ranged Combat**: Right now everything is melee-based. 

**State Machine and Menus**: The current game loop is basic. A proper state machine would handle main menus, actual pause, and game state transitions more elegantly.

**Visual Effects and Feedback**: The game feels functional but lacks impact. Adding particle effects, screen shake, and visual feedback would make it more satisfying.

**Enhanced UI**: The current UI is minimal. A proper inventory system, character stats display, and more polished interface would improve the player experience.

**Sound Integration**: I didn't implement any audio system. Adding sound effects, background music, and audio feedback would dramatically improve game feel.

**VContainer Architecture Review**: As I mentioned in the challenges section, I'd reconsider using VContainer for gameplay objects. The pooling conflicts taught me that sometimes simpler is better for performance-critical systems.
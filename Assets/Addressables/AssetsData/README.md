# Unity Addressables System - Complete Guide

This document explains Unity's Addressables system, what each file in this folder does, and how to use them effectively for your phased RPG project.

## 📁 Folder Structure Overview

```
AddressableAssetsData/
├── AddressableAssetSettings.asset      # Main configuration file
├── DefaultObject.asset                # Default settings reference
├── AssetGroups/                       # Asset organization
│   ├── Built In Data.asset            # Unity's built-in assets
│   ├── Default Local Group.asset      # Your game assets
│   └── Schemas/                       # Group behavior definitions
├── AssetGroupTemplates/               # Templates for new groups
│   └── Packed Assets.asset            # Template for bundled assets
└── DataBuilders/                      # Build mode configurations
    ├── BuildScriptFastMode.asset      # Fast iteration (development)
    ├── BuildScriptVirtualMode.asset   # Virtual mode (fastest)
    ├── BuildScriptPackedMode.asset    # Production builds
    └── BuildScriptPackedPlayMode.asset # Test packed builds
```

## 🎯 What is Unity Addressables?

Unity Addressables is a content management system that allows you to:

- **Load assets by address** instead of direct references
- **Stream content** from local or remote sources
- **Reduce memory footprint** by loading/unloading assets dynamically
- **Update content** without rebuilding the entire game
- **Organize assets** into logical groups for efficient delivery

### Traditional vs Addressables

```csharp
// ❌ Traditional - Direct reference, always loaded
public GameObject enemyPrefab;
var enemy = Instantiate(enemyPrefab);

// ✅ Addressables - Load on demand, memory efficient
public AssetReference enemyPrefabRef;
var enemyPrefab = await Addressables.LoadAssetAsync<GameObject>(enemyPrefabRef);
var enemy = Instantiate(enemyPrefab);
```

## 📄 File Breakdown

### 1. AddressableAssetSettings.asset
**The brain of the system** - Contains global configuration:

```yaml
Key Settings:
- m_DefaultGroup: Which group new assets go to
- m_OptimizeCatalogSize: Minimize catalog file size
- m_BuildRemoteCatalog: Enable remote content updates
- m_maxConcurrentWebRequests: Download performance tuning
- m_BuildAddressablesWithPlayerBuild: Auto-build with game
```

**What it controls:**
- Default asset group assignment
- Build behavior and optimization
- Remote content delivery settings
- Catalog generation options

### 2. AssetGroups/ - Asset Organization

#### Default Local Group.asset
**Your main asset container** - Currently contains:
- `UI Atlas` - UI texture atlas
- `LongSword` - Weapon prefab
- Other game assets

**Group Properties:**
- **Local delivery** - Assets included in build
- **Bundled together** - Loaded as a unit
- **Fast access** - No download required

#### Built In Data.asset
**Unity's system assets** - Contains:
- Built-in shaders and resources
- Default Unity materials
- System sprites and textures

### 3. Schemas/ - Behavior Definitions

Schemas define HOW groups behave:

#### BundledAssetGroupSchema.asset
```yaml
Controls:
- Bundle packing strategy
- Compression settings
- Asset dependencies
- Loading behavior
```

#### ContentUpdateGroupSchema.asset
```yaml
Controls:
- How content updates work
- Version management
- Static vs dynamic content
```

### 4. DataBuilders/ - Build Modes

#### BuildScriptFastMode.asset
**Development mode** - Fastest iteration:
- Assets loaded directly from project
- No bundles created
- Instant changes in play mode

#### BuildScriptVirtualMode.asset
**Simulation mode** - Test without building:
- Simulates addressable loading
- No actual bundles
- Perfect for testing logic

#### BuildScriptPackedMode.asset
**Production mode** - Real bundles:
- Creates actual asset bundles
- Optimized for shipping
- True loading behavior

#### BuildScriptPackedPlayMode.asset
**Test mode** - Production simulation:
- Uses packed bundles in editor
- Test final loading behavior
- Verify bundle dependencies

### 5. AssetGroupTemplates/
**Templates for creating new groups** with predefined settings:
- Bundle configuration
- Schema assignments
- Naming conventions

## 🛠️ How to Use for Your RPG Project

### Current Setup Analysis

Your project already has:
✅ **UI Atlas** - Addressable UI textures  
✅ **LongSword** - Addressable weapon prefab  
✅ **Basic configuration** - Ready to use  

### Recommended Asset Organization

Create additional groups for your RPG:

```
Asset Groups Strategy:
├── UI Assets (fast loading)
│   ├── UI Atlas
│   ├── Menu backgrounds
│   └── HUD elements
├── Heroes (character data)
│   ├── Hero prefabs
│   ├── Character models
│   └── Animations
├── Enemies (combat content)
│   ├── Enemy prefabs (Bee, etc.)
│   ├── AI behaviors
│   └── Combat effects
├── Weapons (equipment)
│   ├── LongSword
│   ├── GreatSword
│   ├── CurvedSword
│   └── Weapon effects
└── Environment (world content)
    ├── Level prefabs
    ├── Environment assets
    └── Lighting setups
```

### Integration with Your Asset Management System

Your custom `AddressableAssetLoader` works perfectly with this setup:

```csharp
// Create catalog entries for your addressable assets
AddressableCatalog entries:
- id: "hero_warrior", reference: HeroPrefabRef, loadOnStartup: true
- id: "enemy_bee", reference: BeePrefabRef, loadOnStartup: false  
- id: "weapon_longsword", reference: LongSwordRef, loadOnStartup: false
- id: "ui_atlas", reference: UIAtlasRef, loadOnStartup: true
```

## 🎮 Workflow for Your RPG

### 1. Making Assets Addressable

**In Unity Editor:**
1. Select your asset (prefab, texture, etc.)
2. Check "Addressable" in inspector
3. Assign a meaningful address name
4. Choose appropriate group

**Example for your project:**
```
Bee.prefab → Address: "enemy_bee" → Group: "Enemies"
Hero.prefab → Address: "hero_warrior" → Group: "Heroes"  
CurvedSword.prefab → Address: "weapon_curved_sword" → Group: "Weapons"
```

### 2. Development Workflow

```csharp
// Phase 1: Development (Fast Mode)
// - Quick iteration
// - Instant asset changes
// - No bundle building

// Phase 2: Testing (Packed Play Mode)  
// - Test final loading behavior
// - Verify dependencies
// - Performance testing

// Phase 3: Production (Packed Mode)
// - Optimized bundles
// - Real loading times
// - Shipping ready
```

### 3. Using with Your Asset Management System

```csharp
// Your AddressableCatalog setup
[CreateAssetMenu]
public class RPGAddressableCatalog : AddressableCatalog
{
    // Pre-configured entries for your RPG
    private void Reset()
    {
        // Add default RPG entries
        AddEntry("ui_atlas", uiAtlasRef, loadOnStartup: true);
        AddEntry("hero_warrior", heroRef, loadOnStartup: false);
        AddEntry("enemy_bee", beeRef, loadOnStartup: false);
    }
}

// Usage in game code
public class RPGGameManager
{
    private readonly IAssetLoader _assetLoader;
    
    public async UniTask LoadLevel(string levelName)
    {
        // Preload essential assets
        await _assetLoader.PreloadStartupAssetsAsync();
        
        // Load level-specific content
        var levelPrefab = await _assetLoader.LoadAssetAsync<GameObject>(levelRef);
        var hero = await _assetLoader.InstantiateAsync(heroRef);
    }
}
```

## 🚀 Build Modes Explained

### Development Phase
```csharp
Use: BuildScriptFastMode
Benefits:
- Instant asset changes
- No build time
- Fast iteration
- Direct project access
```

### Testing Phase  
```csharp
Use: BuildScriptPackedPlayMode
Benefits:
- Test bundle loading
- Verify dependencies  
- Check loading times
- Simulate production
```

### Production Phase
```csharp
Use: BuildScriptPackedMode
Benefits:
- Optimized bundles
- Compressed assets
- Real loading behavior
- Shipping ready
```

## 📊 Performance Considerations

### Memory Management
```csharp
// ✅ Good practice - Release when done
var asset = await Addressables.LoadAssetAsync<Texture>("ui_background");
// Use asset...
Addressables.Release(asset); // Free memory

// ✅ Your asset loader handles this automatically
_assetLoader.Release(asset); // Tracked and cleaned up
```

### Loading Strategy
```csharp
// ✅ Preload essential assets at startup
await _assetLoader.PreloadStartupAssetsAsync(); // UI, core systems

// ✅ Load on-demand for gameplay
var enemy = await _assetLoader.LoadAssetAsync<GameObject>(enemyRef); // When needed

// ✅ Batch related assets in same group
Group "Level1": [Enemies, Weapons, Environment] // Load together
```

## 🎯 Next Steps for Your RPG

1. **Organize existing assets** into logical groups:
   - Move Hero.prefab, Bee.prefab, weapons to appropriate groups
   - Create catalog entries for each

2. **Configure your AddressableCatalog**:
   - Add entries for all addressable assets
   - Mark UI and core assets as `loadOnStartup: true`
   - Mark gameplay assets as `loadOnStartup: false`

3. **Test loading workflow**:
   - Use Fast Mode for development
   - Switch to Packed Play Mode for testing
   - Build final bundles with Packed Mode

4. **Integrate with VContainer**:
   - Register your asset loader in GameLifetimeScope
   - Use build callbacks for automatic preloading
   - Inject IAssetLoader into game systems

## 🔧 Common Commands

```csharp
// Window → Asset Management → Addressables → Groups
// View and organize your addressable assets

// Window → Asset Management → Addressables → Profiles  
// Configure build and load paths

// Window → Asset Management → Addressables → Build
// Build addressable content for testing/shipping
```

Perfect for your phased RPG project - organize assets by gameplay phase, preload essentials, and load content dynamically as players progress! 🎮
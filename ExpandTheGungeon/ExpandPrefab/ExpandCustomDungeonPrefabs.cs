﻿using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using ExpandTheGungeon.ExpandUtilities;
using System.Reflection;
using MonoMod.RuntimeDetour;
using ExpandTheGungeon.ItemAPI;
using ExpandTheGungeon.ExpandDungeonFlows;

namespace ExpandTheGungeon.ExpandPrefab {

    public class ExpandCustomDungeonPrefabs : DungeonDatabase {

        public static Dungeon GetOrLoadByNameHook(Func<string, Dungeon>orig, string name) {
            switch (name.ToLower()) {
                case "base_space":
                    DebugTime.RecordStartTime();
                    DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
                    return SpaceDungeon(GetOrLoadByName_Orig("Base_ResourcefulRat"));
                case "base_jungle":
                    DebugTime.RecordStartTime();
                    DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
                    return JungleDungeon(GetOrLoadByName_Orig("Base_ResourcefulRat"));
                case "base_belly":
                    DebugTime.RecordStartTime();
                    DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
                    return BellyDungeon(GetOrLoadByName_Orig("Base_ResourcefulRat"));
                case "base_west":
                    DebugTime.RecordStartTime();
                    DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
                    return WestDungeon(GetOrLoadByName_Orig("Base_Gungeon"));
                case "base_phobos":
                    DebugTime.RecordStartTime();
                    DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
                    return PhobosDungeon(GetOrLoadByName_Orig("Base_Gungeon"));
                default:
                    return orig(name);
            }
        }
                
        public static Dungeon GetOrLoadByName_Orig(string name) {
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("dungeons/" + name.ToLower());
            DebugTime.RecordStartTime();
            Dungeon component = assetBundle.LoadAsset<GameObject>(name).GetComponent<Dungeon>();
            DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
            assetBundle = null;
            return component;
        }

        public static Hook getOrLoadByName_Hook;
        public static Hook dungeonStartHook;

        public void DungeonStart_Hook(Action<ItemDB>orig, ItemDB self) {
            List<WeightedGameObject> collection;
            if (self.ModLootPerFloor.TryGetValue("ANY", out collection)) {
                GameManager.Instance.Dungeon.baseChestContents.defaultItemDrops.elements.AddRange(collection);
            }
            string dungeonFloorName = GameManager.Instance.Dungeon.DungeonFloorName;
            if (!string.IsNullOrEmpty(dungeonFloorName) && dungeonFloorName.StartsWith("#")) {
                string key = dungeonFloorName.Substring(1, dungeonFloorName.IndexOf('_') - 1);
                if (self.ModLootPerFloor.TryGetValue(key, out collection)) {
                    GameManager.Instance.Dungeon.baseChestContents.defaultItemDrops.elements.AddRange(collection);
                }
            }
        }

        public static void InitCustomGameLevelDefinitions(AssetBundle braveResources) {
            if (ExpandStats.debugMode) { Debug.Log("[ExpandTheGungeon] Installing DungeonDatabase.GetOrLoadByName Hook..."); }
            getOrLoadByName_Hook = new Hook(
                typeof(DungeonDatabase).GetMethod("GetOrLoadByName", BindingFlags.Static | BindingFlags.Public),
                typeof(ExpandCustomDungeonPrefabs).GetMethod("GetOrLoadByNameHook", BindingFlags.Static | BindingFlags.Public)
            );

            if (ExpandStats.debugMode) { Debug.Log("[ExpandTheGungeon] Installing ItemDB.DungeonStart Hook..."); }
            dungeonStartHook = new Hook(
                typeof(ItemDB).GetMethod("DungeonStart", BindingFlags.Instance | BindingFlags.Public),
                typeof(ExpandCustomDungeonPrefabs).GetMethod("DungeonStart_Hook", BindingFlags.Instance | BindingFlags.Public),
                typeof(ItemDB)
            );
            
            ReInitFloorDefinitions(GameManager.Instance);
        }
        

        public static void ReInitFloorDefinitions(GameManager gameManager) {
            if (gameManager) {
                bool SpaceEntryExists = false;
                if (gameManager.customFloors != null) {
                    foreach (GameLevelDefinition definition in gameManager.customFloors) {
                        if (!gameManager) { return; }
                        switch (definition.dungeonSceneName.ToLower()) {
                            case "tt_jungle":
                                definition.priceMultiplier = 1.20000005f;
                                definition.secretDoorHealthMultiplier = 1;
                                definition.enemyHealthMultiplier = 1.33329999f;
                                definition.damageCap = 300;
                                definition.bossDpsCap = 42;
                                definition.flowEntries = new List<DungeonFlowLevelEntry>(0);
                                definition.predefinedSeeds = new List<int>(0);
                                break;
                            case "tt_belly":
                                definition.priceMultiplier = 1.39999998f;
                                definition.secretDoorHealthMultiplier = 1;
                                definition.enemyHealthMultiplier = 1.66659999f;
                                definition.damageCap = 300;
                                definition.bossDpsCap = 60;
                                definition.flowEntries = new List<DungeonFlowLevelEntry>(0);
                                definition.predefinedSeeds = new List<int>(0);
                                break;
                            case "tt_west":
                                definition.priceMultiplier = 2;
                                definition.secretDoorHealthMultiplier = 1;
                                definition.enemyHealthMultiplier = 2.1f;
                                definition.damageCap = 300;
                                definition.bossDpsCap = 78;
                                definition.flowEntries = new List<DungeonFlowLevelEntry>(0);
                                definition.predefinedSeeds = new List<int>(0);
                                break;
                            case "tt_phobos":
                                definition.priceMultiplier = 1.4f;
                                definition.secretDoorHealthMultiplier = 1;
                                definition.enemyHealthMultiplier = 1.7f;
                                definition.damageCap = 300;
                                definition.bossDpsCap = 60;
                                definition.flowEntries = new List<DungeonFlowLevelEntry>(0);
                                definition.predefinedSeeds = new List<int>(0);
                                break;
                            case "tt_space":
                                SpaceEntryExists = true;
                                break;
                        }
                    }
                    if (!SpaceEntryExists) {
                        gameManager.customFloors.Add(
                            new GameLevelDefinition() {
                                dungeonSceneName = "tt_space",
                                dungeonPrefabPath = "Base_Space",
                                priceMultiplier = 2,
                                secretDoorHealthMultiplier = 1,
                                enemyHealthMultiplier = 2.1f,
                                damageCap = 300,
                                bossDpsCap = 78,
                                flowEntries = new List<DungeonFlowLevelEntry>(0),
                                predefinedSeeds = new List<int>(0)
                            }
                        );
                    }
                }
            }
        }


        public static Dungeon SpaceDungeon(Dungeon dungeon) {
            Dungeon MinesDungeonPrefab = GetOrLoadByName_Orig("Base_Mines");
            Dungeon RatDungeonPrefab = GetOrLoadByName_Orig("Base_ResourcefulRat");
            Dungeon FinalScenarioPilotPrefab = GetOrLoadByName_Orig("FinalScenario_Pilot");
            Dungeon FinalScenarioBulletPrefab = GetOrLoadByName_Orig("FinalScenario_Bullet");

            DungeonMaterial FinalScenario_MainMaterial = UnityEngine.Object.Instantiate(FinalScenarioPilotPrefab.roomMaterialDefinitions[0]);
            FinalScenario_MainMaterial.supportsPits = true;
            FinalScenario_MainMaterial.doPitAO = false;
            // FinalScenario_MainMaterial.pitsAreOneDeep = true;
            FinalScenario_MainMaterial.useLighting = true;
            // FinalScenario_MainMaterial.supportsLavaOrLavalikeSquares = true;
            FinalScenario_MainMaterial.lightPrefabs.elements[0].rawGameObject = MinesDungeonPrefab.roomMaterialDefinitions[0].lightPrefabs.elements[0].rawGameObject;
            FinalScenario_MainMaterial.roomFloorBorderGrid = MinesDungeonPrefab.roomMaterialDefinitions[0].roomFloorBorderGrid;
            FinalScenario_MainMaterial.pitLayoutGrid = MinesDungeonPrefab.roomMaterialDefinitions[0].pitLayoutGrid;
            FinalScenario_MainMaterial.pitBorderFlatGrid = MinesDungeonPrefab.roomMaterialDefinitions[0].pitBorderFlatGrid;

            DungeonTileStampData m_CanyonStampData = ScriptableObject.CreateInstance<DungeonTileStampData>();
            m_CanyonStampData.name = "ENV_SPACE_STAMP_DATA";
            m_CanyonStampData.tileStampWeight = 0;
            m_CanyonStampData.spriteStampWeight = 0;
            m_CanyonStampData.objectStampWeight = 1;
            m_CanyonStampData.stamps = new TileStampData[0];
            m_CanyonStampData.spriteStamps = new SpriteStampData[0];
            m_CanyonStampData.objectStamps = RatDungeonPrefab.stampData.objectStamps;
            m_CanyonStampData.SymmetricFrameChance = 0.25f;
            m_CanyonStampData.SymmetricCompleteChance = 0.6f;

            dungeon.gameObject.name = "Base_Space";            
            dungeon.contentSource = ContentSource.CONTENT_UPDATE_03;
            dungeon.DungeonSeed = 0;
            dungeon.DungeonFloorName = "An Alien Place.";
            dungeon.DungeonShortName = "An Alien Place.";
            dungeon.DungeonFloorLevelTextOverride = "Unknown location...";
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.NONE;
            dungeon.debugSettings = new DebugDungeonSettings() {
                RAPID_DEBUG_DUNGEON_ITERATION_SEEKER = false,
                RAPID_DEBUG_DUNGEON_ITERATION = false,
                RAPID_DEBUG_DUNGEON_COUNT = 50,
                GENERATION_VIEWER_MODE = false,
                FULL_MINIMAP_VISIBILITY = false,
                COOP_TEST = false,
                DISABLE_ENEMIES = false,
                DISABLE_LOOPS = false,
                DISABLE_SECRET_ROOM_COVERS = false,
                DISABLE_OUTLINES = false,
                WALLS_ARE_PITS = false
            };
            dungeon.PatternSettings = new SemioticDungeonGenSettings() {
                flows = new List<DungeonFlow>() { complex_flow_test.Complex_Flow_Test() },
                mandatoryExtraRooms = new List<ExtraIncludedRoomData>(0),
                optionalExtraRooms = new List<ExtraIncludedRoomData>(0),
                MAX_GENERATION_ATTEMPTS = 250,
                DEBUG_RENDER_CANVASES_SEPARATELY = false
            };            
            dungeon.ForceRegenerationOfCharacters = false;
            dungeon.ActuallyGenerateTilemap = true;
            dungeon.decoSettings = new TilemapDecoSettings {
                standardRoomVisualSubtypes = new WeightedIntCollection {
                    elements = new WeightedInt[] {
                        new WeightedInt() {
                            annotation = "pilot final",
                            value = 0,
                            weight = 1,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "pilot final2",
                            value = 1,
                            weight = 1,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "shop",
                            value = 2,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "pilot final2",
                            value = 3,
                            weight = 1,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "pilot final3",
                            value = 4,
                            weight = 1,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "pilot final4",
                            value = 5,
                            weight = 1,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "pilot final5",
                            value = 6,
                            weight = 1,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        }
                    }
                },
                decalLayerStyle = MinesDungeonPrefab.decoSettings.decalLayerStyle,
                decalSize = MinesDungeonPrefab.decoSettings.decalSize,
                decalSpacing = MinesDungeonPrefab.decoSettings.decalSpacing,
                decalExpansion = MinesDungeonPrefab.decoSettings.decalExpansion,
                patternLayerStyle = MinesDungeonPrefab.decoSettings.patternLayerStyle,
                patternSize = MinesDungeonPrefab.decoSettings.patternSize,
                patternSpacing = MinesDungeonPrefab.decoSettings.patternSpacing,
                patternExpansion = MinesDungeonPrefab.decoSettings.patternExpansion,
                decoPatchFrequency = MinesDungeonPrefab.decoSettings.decoPatchFrequency,
                ambientLightColor = MinesDungeonPrefab.decoSettings.ambientLightColor,
                ambientLightColorTwo = MinesDungeonPrefab.decoSettings.ambientLightColorTwo,
                lowQualityAmbientLightColor = MinesDungeonPrefab.decoSettings.lowQualityAmbientLightColor,
                lowQualityAmbientLightColorTwo = MinesDungeonPrefab.decoSettings.lowQualityAmbientLightColorTwo,
                lowQualityCheapLightVector = MinesDungeonPrefab.decoSettings.lowQualityCheapLightVector,
                UsesAlienFXFloorColor = MinesDungeonPrefab.decoSettings.UsesAlienFXFloorColor,
                AlienFXFloorColor = MinesDungeonPrefab.decoSettings.AlienFXFloorColor,
                generateLights = MinesDungeonPrefab.decoSettings.generateLights,
                lightCullingPercentage = MinesDungeonPrefab.decoSettings.lightCullingPercentage,
                lightOverlapRadius = MinesDungeonPrefab.decoSettings.lightOverlapRadius,
                nearestAllowedLight = MinesDungeonPrefab.decoSettings.nearestAllowedLight,
                minLightExpanseWidth = MinesDungeonPrefab.decoSettings.minLightExpanseWidth,
                lightHeight = MinesDungeonPrefab.decoSettings.lightHeight,
                lightCookies = MinesDungeonPrefab.decoSettings.lightCookies,
                debug_view = false
            };
                        
            dungeon.tileIndices = new TileIndices() {
                tilesetId = GlobalDungeonData.ValidTilesets.SPACEGEON,
                dungeonCollection = FinalScenarioPilotPrefab.tileIndices.dungeonCollection,
                dungeonCollectionSupportsDiagonalWalls = false,
                aoTileIndices = FinalScenarioBulletPrefab.tileIndices.aoTileIndices,
                placeBorders = true,
                placePits = false,
                chestHighWallIndices = new List<TileIndexVariant>() {
                    new TileIndexVariant() {
                        index = 41,
                        likelihood = 0.5f,
                        overrideLayerIndex = 0,
                        overrideIndex = 0
                    }
                },
                decalIndexGrid = null,
                patternIndexGrid = FinalScenarioBulletPrefab.tileIndices.patternIndexGrid,
                globalSecondBorderTiles = new List<int>(0),
                edgeDecorationTiles = null
            };
            dungeon.roomMaterialDefinitions = new DungeonMaterial[] {
                FinalScenario_MainMaterial,
                FinalScenario_MainMaterial,
                FinalScenario_MainMaterial,
                FinalScenario_MainMaterial,
                FinalScenario_MainMaterial,
                FinalScenario_MainMaterial,
                FinalScenario_MainMaterial
            };
            dungeon.dungeonWingDefinitions = new DungeonWingDefinition[0];
            dungeon.pathGridDefinitions = new List<TileIndexGrid>() { MinesDungeonPrefab.pathGridDefinitions[0] };
            dungeon.dungeonDustups = new DustUpVFX() {
                runDustup = MinesDungeonPrefab.dungeonDustups.runDustup,
                waterDustup = MinesDungeonPrefab.dungeonDustups.waterDustup,
                additionalWaterDustup = MinesDungeonPrefab.dungeonDustups.additionalWaterDustup,
                rollNorthDustup = MinesDungeonPrefab.dungeonDustups.rollNorthDustup,
                rollNorthEastDustup = MinesDungeonPrefab.dungeonDustups.rollNorthEastDustup,
                rollEastDustup = MinesDungeonPrefab.dungeonDustups.rollEastDustup,
                rollSouthEastDustup = MinesDungeonPrefab.dungeonDustups.rollSouthEastDustup,
                rollSouthDustup = MinesDungeonPrefab.dungeonDustups.rollSouthDustup,
                rollSouthWestDustup = MinesDungeonPrefab.dungeonDustups.rollSouthWestDustup,
                rollWestDustup = MinesDungeonPrefab.dungeonDustups.rollWestDustup,
                rollNorthWestDustup = MinesDungeonPrefab.dungeonDustups.rollNorthWestDustup,
                rollLandDustup = MinesDungeonPrefab.dungeonDustups.rollLandDustup
            };
            dungeon.damageTypeEffectMatrix = MinesDungeonPrefab.damageTypeEffectMatrix;
            dungeon.stampData = m_CanyonStampData;
            dungeon.UsesCustomFloorIdea = false;
            dungeon.FloorIdea = new RobotDaveIdea() {
                ValidEasyEnemyPlaceables = new DungeonPlaceable[0],
                ValidHardEnemyPlaceables = new DungeonPlaceable[0],
                UseWallSawblades = false,
                UseRollingLogsVertical = true,
                UseRollingLogsHorizontal = true,
                UseFloorPitTraps = false,
                UseFloorFlameTraps = true,
                UseFloorSpikeTraps = true,
                UseFloorConveyorBelts = true,
                UseCaveIns = true,
                UseAlarmMushrooms = false,
                UseChandeliers = true,
                UseMineCarts = false,
                CanIncludePits = false
            };
            dungeon.PlaceDoors = true;
            dungeon.doorObjects = MinesDungeonPrefab.doorObjects;
            dungeon.oneWayDoorObjects = MinesDungeonPrefab.oneWayDoorObjects;
            dungeon.oneWayDoorPressurePlate = MinesDungeonPrefab.oneWayDoorPressurePlate;
            dungeon.phantomBlockerDoorObjects = MinesDungeonPrefab.phantomBlockerDoorObjects;
            dungeon.UsesWallWarpWingDoors = false;
            dungeon.baseChestContents = MinesDungeonPrefab.baseChestContents;
            dungeon.SecretRoomSimpleTriggersFacewall = new List<GameObject>() { MinesDungeonPrefab.SecretRoomSimpleTriggersFacewall[0] };
            dungeon.SecretRoomSimpleTriggersSidewall = new List<GameObject>() { MinesDungeonPrefab.SecretRoomSimpleTriggersSidewall[0] };
            dungeon.SecretRoomComplexTriggers = new List<ComplexSecretRoomTrigger>(0);
            dungeon.SecretRoomDoorSparkVFX = MinesDungeonPrefab.SecretRoomDoorSparkVFX;
            dungeon.SecretRoomHorizontalPoofVFX = MinesDungeonPrefab.SecretRoomHorizontalPoofVFX;
            dungeon.SecretRoomVerticalPoofVFX = MinesDungeonPrefab.SecretRoomVerticalPoofVFX;
            dungeon.sharedSettingsPrefab = MinesDungeonPrefab.sharedSettingsPrefab;
            dungeon.NormalRatGUID = string.Empty;
            dungeon.BossMasteryTokenItemId = CustomMasterRounds.GtlichFloorMasterRoundID;
            dungeon.UsesOverrideTertiaryBossSets = false;
            dungeon.OverrideTertiaryRewardSets = new List<TertiaryBossRewardSet>(0);
            dungeon.defaultPlayerPrefab = MinesDungeonPrefab.defaultPlayerPrefab;
            dungeon.StripPlayerOnArrival = false;
            dungeon.SuppressEmergencyCrates = false;
            dungeon.SetTutorialFlag = false;
            dungeon.PlayerIsLight = true;
            dungeon.PlayerLightColor = MinesDungeonPrefab.PlayerLightColor;
            dungeon.PlayerLightIntensity = 4;
            dungeon.PlayerLightRadius = 4;
            dungeon.PrefabsToAutoSpawn = new GameObject[0];            
            dungeon.musicEventName = "Play_MUS_Dungeon_Rat_Theme_01";
            
            FinalScenarioPilotPrefab = null;
            RatDungeonPrefab = null;
            MinesDungeonPrefab = null;

            return dungeon;
        }

        public static Dungeon JungleDungeon(Dungeon dungeon) {
            AssetBundle expandSharedAuto1 = ResourceManager.LoadAssetBundle(ExpandTheGungeon.ModAssetBundleName);
            AssetBundle braveResources = ResourceManager.LoadAssetBundle("brave_resources_001");
            Dungeon MinesDungeonPrefab = GetOrLoadByName_Orig("Base_Mines");
            Dungeon GungeonPrefab = GetOrLoadByName_Orig("Base_Gungeon");
            Dungeon SewersPrefab = GetOrLoadByName_Orig("Base_Sewer");
            Dungeon CastlePrefab = GetOrLoadByName_Orig("Base_Castle");

            DungeonMaterial Jungle_Woods = ScriptableObject.CreateInstance<DungeonMaterial>();
            Jungle_Woods.wallShards = GungeonPrefab.roomMaterialDefinitions[0].wallShards;
            Jungle_Woods.bigWallShards = GungeonPrefab.roomMaterialDefinitions[0].bigWallShards;
            Jungle_Woods.bigWallShardDamageThreshold = 10;
            Jungle_Woods.fallbackVerticalTileMapEffects = GungeonPrefab.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            Jungle_Woods.fallbackHorizontalTileMapEffects = GungeonPrefab.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            Jungle_Woods.pitfallVFXPrefab = null;
            Jungle_Woods.UsePitAmbientVFX = false;
            Jungle_Woods.AmbientPitVFX = new List<GameObject>(0);
            Jungle_Woods.PitVFXMinCooldown = 5;
            Jungle_Woods.PitVFXMaxCooldown = 30;
            Jungle_Woods.ChanceToSpawnPitVFXOnCooldown = 1;
            Jungle_Woods.stampFailChance = 0.2f;
            Jungle_Woods.overrideTableTable = null;
            Jungle_Woods.supportsPits = false;
            Jungle_Woods.doPitAO = true;
            Jungle_Woods.pitsAreOneDeep = false;
            Jungle_Woods.supportsDiagonalWalls = false;
            Jungle_Woods.supportsUpholstery = false;
            Jungle_Woods.carpetIsMainFloor = false;
            Jungle_Woods.carpetGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "JungleAssets/carpetGrid1") };
            Jungle_Woods.supportsChannels = false;
            Jungle_Woods.minChannelPools = 0;
            Jungle_Woods.maxChannelPools = 3;
            Jungle_Woods.channelTenacity = 0.75f;
            Jungle_Woods.channelGrids = new TileIndexGrid[0];
            Jungle_Woods.supportsLavaOrLavalikeSquares = false;
            Jungle_Woods.lavaGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "JungleAssets/lavaGrid") };
            Jungle_Woods.supportsIceSquares = false;
            Jungle_Woods.iceGrids = new TileIndexGrid[0];
            Jungle_Woods.roomFloorBorderGrid = null;
            Jungle_Woods.roomCeilingBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "JungleAssets/roomCeilingBorderGrid");
            Jungle_Woods.pitLayoutGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "JungleAssets/pitLayoutGrid");
            Jungle_Woods.pitBorderRaisedGrid = null;
            Jungle_Woods.additionalPitBorderFlatGrid = null;
            Jungle_Woods.pitBorderFlatGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "JungleAssets/outerCeilingBorderGrid");
            Jungle_Woods.outerCeilingBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "JungleAssets/outerCeilingBorderGrid");
            Jungle_Woods.floorSquareDensity = 0.05f;
            Jungle_Woods.floorSquares = new TileIndexGrid[0];
            Jungle_Woods.usesFacewallGrids = false;
            Jungle_Woods.facewallGrids = new FacewallIndexGridDefinition[] {
                new FacewallIndexGridDefinition() {
                    grid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "JungleAssets/faceWallGrid1"),
                    minWidth = 3,
                    maxWidth = 8,
                    hasIntermediaries = false,
                    minIntermediaryBuffer = 4,
                    maxIntermediaryBuffer = 6,
                    minIntermediaryLength = 1,
                    maxIntermediaryLength = 3,
                    topsMatchBottoms = true,
                    middleSectionSequential = false,
                    canExistInCorners = true,
                    forceEdgesInCorners = true,
                    canAcceptWallDecoration = false,
                    canAcceptFloorDecoration = false,
                    forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    canBePlacedInExits = false,
                    chanceToPlaceIfPossible = 0.15f,
                    perTileFailureRate = 0.05f
                },
                new FacewallIndexGridDefinition() {
                    grid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "JungleAssets/faceWallGrid2"),
                    minWidth = 3,
                    maxWidth = 8,
                    hasIntermediaries = false,
                    minIntermediaryBuffer = 4,
                    maxIntermediaryBuffer = 6,
                    minIntermediaryLength = 1,
                    maxIntermediaryLength = 3,
                    topsMatchBottoms = true,
                    middleSectionSequential = false,
                    canExistInCorners = true,
                    forceEdgesInCorners = true,
                    canAcceptWallDecoration = false,
                    canAcceptFloorDecoration = false,
                    forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    canBePlacedInExits = false,
                    chanceToPlaceIfPossible = 0.15f,
                    perTileFailureRate = 0.05f
                },
                new FacewallIndexGridDefinition() {
                    grid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "JungleAssets/faceWallGrid3"),
                    minWidth = 3,
                    maxWidth = 8,
                    hasIntermediaries = false,
                    minIntermediaryBuffer = 4,
                    maxIntermediaryBuffer = 6,
                    minIntermediaryLength = 1,
                    maxIntermediaryLength = 3,
                    topsMatchBottoms = true,
                    middleSectionSequential = false,
                    canExistInCorners = true,
                    forceEdgesInCorners = true,
                    canAcceptWallDecoration = false,
                    canAcceptFloorDecoration = false,
                    forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    canBePlacedInExits = false,
                    chanceToPlaceIfPossible = 0.15f,
                    perTileFailureRate = 0.05f
                },
            };
            Jungle_Woods.usesInternalMaterialTransitions = false;
            Jungle_Woods.usesProceduralMaterialTransitions = false;
            Jungle_Woods.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            Jungle_Woods.secretRoomWallShardCollections = new List<GameObject>(0);
            Jungle_Woods.overrideStoneFloorType = false;
            Jungle_Woods.overrideFloorType = CellVisualData.CellFloorType.Stone;
            Jungle_Woods.useLighting = true;
            Jungle_Woods.lightPrefabs = new WeightedGameObjectCollection() {
               elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = ExpandPrefabs.JungleLight,
                       weight = 1,                       
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]                       
                   }
               }
            };
            Jungle_Woods.facewallLightStamps = CastlePrefab.roomMaterialDefinitions[0].facewallLightStamps;
            Jungle_Woods.sidewallLightStamps = CastlePrefab.roomMaterialDefinitions[0].sidewallLightStamps;
            Jungle_Woods.usesDecalLayer = false;
            Jungle_Woods.decalIndexGrid = null;
            Jungle_Woods.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            Jungle_Woods.decalSize = 1;
            Jungle_Woods.decalSpacing = 1;
            Jungle_Woods.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            Jungle_Woods.patternSpacing = 1;
            Jungle_Woods.patternSize = 1;
            Jungle_Woods.patternIndexGrid = null;
            Jungle_Woods.forceEdgesDiagonal = false;
            Jungle_Woods.exteriorFacadeBorderGrid = null;
            Jungle_Woods.facadeTopGrid = null;
            Jungle_Woods.bridgeGrid = null;


            DungeonMaterial Jungle_Bamboo = ScriptableObject.CreateInstance<DungeonMaterial>();
            Jungle_Bamboo.wallShards = GungeonPrefab.roomMaterialDefinitions[0].wallShards;
            Jungle_Bamboo.bigWallShards = GungeonPrefab.roomMaterialDefinitions[0].bigWallShards;
            Jungle_Bamboo.bigWallShardDamageThreshold = 10;
            Jungle_Bamboo.fallbackVerticalTileMapEffects = GungeonPrefab.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            Jungle_Bamboo.fallbackHorizontalTileMapEffects = GungeonPrefab.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            Jungle_Bamboo.pitfallVFXPrefab = null;
            Jungle_Bamboo.UsePitAmbientVFX = false;
            Jungle_Bamboo.AmbientPitVFX = new List<GameObject>(0);
            Jungle_Bamboo.PitVFXMinCooldown = 5;
            Jungle_Bamboo.PitVFXMaxCooldown = 30;
            Jungle_Bamboo.ChanceToSpawnPitVFXOnCooldown = 1;
            Jungle_Bamboo.stampFailChance = 0.2f;
            Jungle_Bamboo.overrideTableTable = null;
            Jungle_Bamboo.supportsPits = false;
            Jungle_Bamboo.doPitAO = true;
            Jungle_Bamboo.pitsAreOneDeep = false;
            Jungle_Bamboo.supportsDiagonalWalls = false;
            Jungle_Bamboo.supportsUpholstery = false;
            Jungle_Bamboo.carpetIsMainFloor = false;
            Jungle_Bamboo.carpetGrids = Jungle_Woods.carpetGrids;
            Jungle_Bamboo.supportsChannels = false;
            Jungle_Bamboo.minChannelPools = 0;
            Jungle_Bamboo.maxChannelPools = 3;
            Jungle_Bamboo.channelTenacity = 0.75f;
            Jungle_Bamboo.channelGrids = new TileIndexGrid[0];
            Jungle_Bamboo.supportsLavaOrLavalikeSquares = true;
            Jungle_Bamboo.lavaGrids = Jungle_Woods.lavaGrids;
            Jungle_Bamboo.supportsIceSquares = false;
            Jungle_Bamboo.iceGrids = new TileIndexGrid[0];
            Jungle_Bamboo.roomFloorBorderGrid = null;
            Jungle_Bamboo.roomCeilingBorderGrid = Jungle_Woods.roomCeilingBorderGrid;                        
            Jungle_Bamboo.pitLayoutGrid = Jungle_Woods.pitLayoutGrid;
            Jungle_Bamboo.pitBorderFlatGrid = Jungle_Woods.outerCeilingBorderGrid;
            Jungle_Bamboo.pitBorderRaisedGrid = null;
            Jungle_Bamboo.additionalPitBorderFlatGrid = null;
            Jungle_Bamboo.outerCeilingBorderGrid = Jungle_Woods.outerCeilingBorderGrid;
            Jungle_Bamboo.floorSquareDensity = 0.05f;
            Jungle_Bamboo.floorSquares = new TileIndexGrid[0];
            Jungle_Bamboo.usesFacewallGrids = false;
            Jungle_Bamboo.facewallGrids = new FacewallIndexGridDefinition[] {
                new FacewallIndexGridDefinition() {
                    grid = Jungle_Woods.facewallGrids[0].grid,
                    minWidth = 3,
                    maxWidth = 8,
                    hasIntermediaries = false,
                    minIntermediaryBuffer = 4,
                    maxIntermediaryBuffer = 6,
                    minIntermediaryLength = 1,
                    maxIntermediaryLength = 3,
                    topsMatchBottoms = true,
                    middleSectionSequential = false,
                    canExistInCorners = true,
                    forceEdgesInCorners = true,
                    canAcceptWallDecoration = false,
                    canAcceptFloorDecoration = false,
                    forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    canBePlacedInExits = false,
                    chanceToPlaceIfPossible = 0.15f,
                    perTileFailureRate = 0.05f
                },
                new FacewallIndexGridDefinition() {
                    grid = Jungle_Woods.facewallGrids[1].grid,
                    minWidth = 3,
                    maxWidth = 8,
                    hasIntermediaries = false,
                    minIntermediaryBuffer = 4,
                    maxIntermediaryBuffer = 6,
                    minIntermediaryLength = 1,
                    maxIntermediaryLength = 3,
                    topsMatchBottoms = true,
                    middleSectionSequential = false,
                    canExistInCorners = true,
                    forceEdgesInCorners = true,
                    canAcceptWallDecoration = false,
                    canAcceptFloorDecoration = false,
                    forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    canBePlacedInExits = false,
                    chanceToPlaceIfPossible = 0.15f,
                    perTileFailureRate = 0.05f
                },
                new FacewallIndexGridDefinition() {
                    grid = Jungle_Woods.facewallGrids[2].grid,
                    minWidth = 3,
                    maxWidth = 8,
                    hasIntermediaries = false,
                    minIntermediaryBuffer = 4,
                    maxIntermediaryBuffer = 6,
                    minIntermediaryLength = 1,
                    maxIntermediaryLength = 3,
                    topsMatchBottoms = true,
                    middleSectionSequential = false,
                    canExistInCorners = true,
                    forceEdgesInCorners = true,
                    canAcceptWallDecoration = false,
                    canAcceptFloorDecoration = false,
                    forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    canBePlacedInExits = false,
                    chanceToPlaceIfPossible = 0.15f,
                    perTileFailureRate = 0.05f
                },
            };
            Jungle_Bamboo.usesInternalMaterialTransitions = false;
            Jungle_Bamboo.usesProceduralMaterialTransitions = false;
            Jungle_Bamboo.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            Jungle_Bamboo.secretRoomWallShardCollections = new List<GameObject>(0);
            Jungle_Bamboo.overrideStoneFloorType = false;
            Jungle_Bamboo.overrideFloorType = CellVisualData.CellFloorType.Stone;
            Jungle_Bamboo.useLighting = true;
            Jungle_Bamboo.lightPrefabs = new WeightedGameObjectCollection() {
               elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = ExpandPrefabs.JungleLight,
                       weight = 1,                       
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]                       
                   }
               }
            };
            Jungle_Bamboo.facewallLightStamps = CastlePrefab.roomMaterialDefinitions[0].facewallLightStamps;
            Jungle_Bamboo.sidewallLightStamps = CastlePrefab.roomMaterialDefinitions[0].sidewallLightStamps;
            Jungle_Bamboo.usesDecalLayer = false;
            Jungle_Bamboo.decalIndexGrid = null;
            Jungle_Bamboo.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            Jungle_Bamboo.decalSize = 1;
            Jungle_Bamboo.decalSpacing = 1;
            Jungle_Bamboo.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            Jungle_Bamboo.patternSpacing = 1;
            Jungle_Bamboo.patternSize = 1;
            Jungle_Bamboo.patternIndexGrid = null;
            Jungle_Bamboo.forceEdgesDiagonal = false;
            Jungle_Bamboo.exteriorFacadeBorderGrid = null;
            Jungle_Bamboo.facadeTopGrid = null;
            Jungle_Bamboo.bridgeGrid = null;
 
                        
            DungeonTileStampData m_JungleStampData = ScriptableObject.CreateInstance<DungeonTileStampData>();
            m_JungleStampData.name = "ENV_JUNGLE_STAMP_DATA";
            m_JungleStampData.tileStampWeight = 1;
            m_JungleStampData.spriteStampWeight = 0;
            m_JungleStampData.objectStampWeight = 1.5f;
            m_JungleStampData.stamps = new TileStampData[0];
            m_JungleStampData.spriteStamps = new SpriteStampData[0];
            m_JungleStampData.objectStamps = GungeonPrefab.stampData.objectStamps;
            m_JungleStampData.SymmetricFrameChance = 0.5f;
            m_JungleStampData.SymmetricCompleteChance = 0.25f;

            dungeon.gameObject.name = "Base_Jungle";
            dungeon.contentSource = ContentSource.CONTENT_UPDATE_03;
            dungeon.DungeonSeed = 0;
            dungeon.DungeonFloorName = "Deep in the Jungle";
            dungeon.DungeonShortName = "Deep in the Jungle";
            dungeon.DungeonFloorLevelTextOverride = "An Ancient Place...";
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.NONE;
            dungeon.debugSettings = new DebugDungeonSettings() {
                RAPID_DEBUG_DUNGEON_ITERATION_SEEKER = false,
                RAPID_DEBUG_DUNGEON_ITERATION = false,
                RAPID_DEBUG_DUNGEON_COUNT = 50,
                GENERATION_VIEWER_MODE = false,
                FULL_MINIMAP_VISIBILITY = false,
                COOP_TEST = false,
                DISABLE_ENEMIES = false,
                DISABLE_LOOPS = false,
                DISABLE_SECRET_ROOM_COVERS = false,
                DISABLE_OUTLINES = false,
                WALLS_ARE_PITS = false
            };
            dungeon.PatternSettings = new SemioticDungeonGenSettings() {
                flows = new List<DungeonFlow>() { f1b_jungle_flow_01.F1b_Jungle_Flow_01(), f1b_jungle_flow_02.F1b_Jungle_Flow_02() },
                mandatoryExtraRooms = new List<ExtraIncludedRoomData>(0),
                optionalExtraRooms = new List<ExtraIncludedRoomData>(0),
                MAX_GENERATION_ATTEMPTS = 250,
                DEBUG_RENDER_CANVASES_SEPARATELY = false
            };

            

            dungeon.ForceRegenerationOfCharacters = false;
            dungeon.ActuallyGenerateTilemap = true;
            dungeon.decoSettings = new TilemapDecoSettings {
                standardRoomVisualSubtypes = new WeightedIntCollection {
                    elements = new WeightedInt[] {
                        new WeightedInt() {
                            annotation = "woods",
                            value = 0,
                            weight = 0.5f,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "the boo",
                            value = 1,
                            weight = 0.5f,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "shop",
                            value = 2,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "unused",
                            value = 3,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "unused",
                            value = 4,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        }
                    }
                },
                decalLayerStyle = TilemapDecoSettings.DecoStyle.NONE,
                decalSize = 3,
                decalSpacing = 1,
                decalExpansion = 0,
                patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE,
                patternSize = 3,
                patternSpacing = 3,
                patternExpansion = 0,
                decoPatchFrequency = 0.01f,
                ambientLightColor = new Color(0.827336f, 0.866108f, 0.885294f, 1),
                ambientLightColorTwo = new Color(0.72549f, 0.764706f, 0.784314f, 1),
                lowQualityAmbientLightColor = new Color(1, 1, 1, 1),
                lowQualityAmbientLightColorTwo = new Color(1, 1, 1, 1),
                lowQualityCheapLightVector = new Vector4(1, 0, -1, 0),
                UsesAlienFXFloorColor = false,
                AlienFXFloorColor = new Color(0, 0, 0, 1),
                generateLights = true,
                lightCullingPercentage = 0.2f,
                lightOverlapRadius = 8,
                nearestAllowedLight = 12,
                minLightExpanseWidth = 2,
                lightHeight = -2,
                lightCookies = new Texture2D[0],
                debug_view = false
            };
            
            dungeon.tileIndices = new TileIndices() {
                tilesetId = GlobalDungeonData.ValidTilesets.JUNGLEGEON,
                dungeonCollection = braveResources.LoadAsset<GameObject>("TallGrassStrip").GetComponent<tk2dTiledSprite>().Collection,
                dungeonCollectionSupportsDiagonalWalls = false,
                aoTileIndices = new AOTileIndices() {
                    AOFloorTileIndex = 0,
                    AOBottomWallBaseTileIndex = 1,
                    AOBottomWallTileRightIndex = 2,
                    AOBottomWallTileLeftIndex = 3,
                    AOBottomWallTileBothIndex = 4,
                    AOTopFacewallRightIndex = 6,
                    AOTopFacewallLeftIndex = 5,
                    AOTopFacewallBothIndex = 7,
                    AOFloorWallLeft = 5,
                    AOFloorWallRight = 6,
                    AOFloorWallBoth = 7,
                    AOFloorPizzaSliceLeft = 8,
                    AOFloorPizzaSliceRight = 9,
                    AOFloorPizzaSliceBoth = 10,
                    AOFloorPizzaSliceLeftWallRight = 11,
                    AOFloorPizzaSliceRightWallLeft = 12,
                    AOFloorWallUpAndLeft = 13,
                    AOFloorWallUpAndRight = 14,
                    AOFloorWallUpAndBoth = 15,
                    AOFloorDiagonalWallNortheast = -1,
                    AOFloorDiagonalWallNortheastLower = -1,
                    AOFloorDiagonalWallNortheastLowerJoint = -1,
                    AOFloorDiagonalWallNorthwest = -1,
                    AOFloorDiagonalWallNorthwestLower = -1,
                    AOFloorDiagonalWallNorthwestLowerJoint = -1,
                    AOBottomWallDiagonalNortheast = -1,
                    AOBottomWallDiagonalNorthwest = -1
                },
                placeBorders = true,
                placePits = false,
                chestHighWallIndices = new List<TileIndexVariant>() {
                    new TileIndexVariant() {
                        index = 41,
                        likelihood = 0.5f,
                        overrideLayerIndex = 0,
                        overrideIndex = 0
                    }
                },
                decalIndexGrid = null,
                patternIndexGrid = null,
                globalSecondBorderTiles = new List<int>(0),
                edgeDecorationTiles = null
            };

            dungeon.roomMaterialDefinitions = new DungeonMaterial[] { Jungle_Woods, Jungle_Bamboo, Jungle_Woods, Jungle_Woods, Jungle_Woods };
            dungeon.dungeonWingDefinitions = new DungeonWingDefinition[0];
            dungeon.pathGridDefinitions = new List<TileIndexGrid>() { MinesDungeonPrefab.pathGridDefinitions[0] };
            dungeon.dungeonDustups = new DustUpVFX() {
                runDustup = GungeonPrefab.dungeonDustups.runDustup,
                waterDustup = GungeonPrefab.dungeonDustups.waterDustup,
                additionalWaterDustup = GungeonPrefab.dungeonDustups.additionalWaterDustup,
                rollNorthDustup = GungeonPrefab.dungeonDustups.rollNorthDustup,
                rollNorthEastDustup = GungeonPrefab.dungeonDustups.rollNorthEastDustup,
                rollEastDustup = GungeonPrefab.dungeonDustups.rollEastDustup,
                rollSouthEastDustup = GungeonPrefab.dungeonDustups.rollSouthEastDustup,
                rollSouthDustup = GungeonPrefab.dungeonDustups.rollSouthDustup,
                rollSouthWestDustup = GungeonPrefab.dungeonDustups.rollSouthWestDustup,
                rollWestDustup = GungeonPrefab.dungeonDustups.rollWestDustup,
                rollNorthWestDustup = GungeonPrefab.dungeonDustups.rollNorthWestDustup,
                rollLandDustup = GungeonPrefab.dungeonDustups.rollLandDustup
            };
            dungeon.damageTypeEffectMatrix = GungeonPrefab.damageTypeEffectMatrix;
            dungeon.stampData = m_JungleStampData;
            dungeon.UsesCustomFloorIdea = false;
            dungeon.FloorIdea = new RobotDaveIdea() {
                ValidEasyEnemyPlaceables = new DungeonPlaceable[0],
                ValidHardEnemyPlaceables = new DungeonPlaceable[0],
                UseWallSawblades = false,
                UseRollingLogsVertical = false,
                UseRollingLogsHorizontal = false,
                UseFloorPitTraps = false,
                UseFloorFlameTraps = false,
                UseFloorSpikeTraps = false,
                UseFloorConveyorBelts = false,
                UseCaveIns = false,
                UseAlarmMushrooms = false,
                UseChandeliers = false,
                UseMineCarts = false,
                CanIncludePits = true
            };
            dungeon.PlaceDoors = true;
            dungeon.doorObjects = ExpandPrefabs.Jungle_Doors;
            dungeon.oneWayDoorObjects = GungeonPrefab.oneWayDoorObjects;
            dungeon.oneWayDoorPressurePlate = GungeonPrefab.oneWayDoorPressurePlate;
            dungeon.phantomBlockerDoorObjects = GungeonPrefab.phantomBlockerDoorObjects;
            dungeon.UsesWallWarpWingDoors = false;
            dungeon.baseChestContents = GungeonPrefab.baseChestContents;
            dungeon.SecretRoomSimpleTriggersFacewall = new List<GameObject>() { SewersPrefab.SecretRoomSimpleTriggersFacewall[0] };
            dungeon.SecretRoomSimpleTriggersSidewall = new List<GameObject>() { SewersPrefab.SecretRoomSimpleTriggersSidewall[0] };
            dungeon.SecretRoomComplexTriggers = new List<ComplexSecretRoomTrigger>(0);
            dungeon.SecretRoomDoorSparkVFX = SewersPrefab.SecretRoomDoorSparkVFX;
            dungeon.SecretRoomHorizontalPoofVFX = SewersPrefab.SecretRoomHorizontalPoofVFX;
            dungeon.SecretRoomVerticalPoofVFX = SewersPrefab.SecretRoomVerticalPoofVFX;
            dungeon.sharedSettingsPrefab = GungeonPrefab.sharedSettingsPrefab;
            dungeon.NormalRatGUID = string.Empty;
            dungeon.BossMasteryTokenItemId = -1;
            dungeon.UsesOverrideTertiaryBossSets = false;
            dungeon.OverrideTertiaryRewardSets = new List<TertiaryBossRewardSet>(0);
            dungeon.defaultPlayerPrefab = GungeonPrefab.defaultPlayerPrefab;
            dungeon.StripPlayerOnArrival = false;
            dungeon.SuppressEmergencyCrates = false;
            dungeon.SetTutorialFlag = false;
            dungeon.PlayerIsLight = false;
            dungeon.PlayerLightColor = new Color(1, 1, 1, 1);
            dungeon.PlayerLightIntensity = 3;
            dungeon.PlayerLightRadius = 5;
            dungeon.PrefabsToAutoSpawn = new GameObject[0];
            dungeon.musicEventName = string.Empty;

            expandSharedAuto1 = null;
            braveResources = null;
            MinesDungeonPrefab = null;
            GungeonPrefab = null;

            return dungeon;
        }

        public static Dungeon BellyDungeon(Dungeon dungeon) {
            AssetBundle expandSharedAuto1 = ResourceManager.LoadAssetBundle(ExpandTheGungeon.ModAssetBundleName);
            AssetBundle sharedAssets = ResourceManager.LoadAssetBundle("shared_auto_001");
            AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            Dungeon MinesDungeonPrefab = GetOrLoadByName_Orig("Base_Mines");
            Dungeon GungeonPrefab = GetOrLoadByName_Orig("Base_Gungeon");
            Dungeon SewersPrefab = GetOrLoadByName_Orig("Base_Sewer");
            Dungeon AbbeyPrefab = GetOrLoadByName_Orig("Base_Cathedral");
                        
            DungeonMaterial BellyMaterial = ScriptableObject.CreateInstance<DungeonMaterial>();
            BellyMaterial.name = "Belly";
            BellyMaterial.wallShards = GungeonPrefab.roomMaterialDefinitions[0].wallShards;
            BellyMaterial.bigWallShards = GungeonPrefab.roomMaterialDefinitions[0].bigWallShards;
            BellyMaterial.bigWallShardDamageThreshold = 10;
            BellyMaterial.fallbackVerticalTileMapEffects = GungeonPrefab.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            BellyMaterial.fallbackHorizontalTileMapEffects = GungeonPrefab.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            BellyMaterial.pitfallVFXPrefab = SewersPrefab.roomMaterialDefinitions[0].pitfallVFXPrefab;
            BellyMaterial.UsePitAmbientVFX = false;
            BellyMaterial.AmbientPitVFX = new List<GameObject>(0);
            BellyMaterial.PitVFXMinCooldown = 5;
            BellyMaterial.PitVFXMaxCooldown = 30;
            BellyMaterial.ChanceToSpawnPitVFXOnCooldown = 1;
            BellyMaterial.stampFailChance = 0.2f;
            BellyMaterial.overrideTableTable = null;
            BellyMaterial.supportsPits = true;
            BellyMaterial.doPitAO = false; // was True
            BellyMaterial.pitsAreOneDeep = false;
            BellyMaterial.supportsDiagonalWalls = false;
            BellyMaterial.supportsUpholstery = false;
            BellyMaterial.carpetIsMainFloor = false;
            BellyMaterial.supportsChannels = false;
            BellyMaterial.minChannelPools = 0;
            BellyMaterial.maxChannelPools = 3;
            BellyMaterial.channelTenacity = 0.75f;
            BellyMaterial.channelGrids = new TileIndexGrid[0];
            BellyMaterial.supportsLavaOrLavalikeSquares = false;
            BellyMaterial.carpetGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "BellyAssets/carpetGrid1") };
            BellyMaterial.lavaGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "BellyAssets/lavaGrid") };
            BellyMaterial.supportsIceSquares = false;
            BellyMaterial.iceGrids = new TileIndexGrid[0];
            BellyMaterial.roomFloorBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "BellyAssets/roomFloorBorderGrid");
            BellyMaterial.roomCeilingBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "BellyAssets/roomCeilingBorderGrid");
            BellyMaterial.pitLayoutGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "BellyAssets/pitLayoutGrid");
            BellyMaterial.pitBorderFlatGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "BellyAssets/pitBorderFlatGrid");
            BellyMaterial.pitBorderRaisedGrid = null;
            BellyMaterial.additionalPitBorderFlatGrid = null;
            BellyMaterial.outerCeilingBorderGrid = null;
            BellyMaterial.floorSquareDensity = 0.05f;
            BellyMaterial.floorSquares = new TileIndexGrid[0];
            BellyMaterial.usesFacewallGrids = false;
            BellyMaterial.facewallGrids = new FacewallIndexGridDefinition[] {
                new FacewallIndexGridDefinition() {
                    grid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "BellyAssets/faceWallGrid1"),
                    minWidth = 3,
                    maxWidth = 20,
                    hasIntermediaries = true,
                    minIntermediaryBuffer = 4,
                    maxIntermediaryBuffer = 6,
                    minIntermediaryLength = 1,
                    maxIntermediaryLength = 3,
                    topsMatchBottoms = true,
                    middleSectionSequential = false,
                    canExistInCorners = false,
                    forceEdgesInCorners = false,
                    canAcceptWallDecoration = false,
                    canAcceptFloorDecoration = true,
                    forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    canBePlacedInExits = true,
                    chanceToPlaceIfPossible = 0.15f,
                    perTileFailureRate = 0.05f
                }
            };
            BellyMaterial.usesInternalMaterialTransitions = false;
            BellyMaterial.usesProceduralMaterialTransitions = false;
            BellyMaterial.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            BellyMaterial.secretRoomWallShardCollections = new List<GameObject>(0);
            BellyMaterial.overrideStoneFloorType = false;
            BellyMaterial.overrideFloorType = CellVisualData.CellFloorType.Stone;
            BellyMaterial.useLighting = true;
            BellyMaterial.lightPrefabs = new WeightedGameObjectCollection() {
                elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = ExpandPrefabs.BellyLight,
                       weight = 1,
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]
                   }
               }
            };
            BellyMaterial.facewallLightStamps = GungeonPrefab.roomMaterialDefinitions[0].facewallLightStamps;
            BellyMaterial.sidewallLightStamps = GungeonPrefab.roomMaterialDefinitions[0].sidewallLightStamps;
            BellyMaterial.usesDecalLayer = false;
            BellyMaterial.decalIndexGrid = null;
            BellyMaterial.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            BellyMaterial.decalSize = 1;
            BellyMaterial.decalSpacing = 1;
            BellyMaterial.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            BellyMaterial.patternSpacing = 1;
            BellyMaterial.patternSize = 1;
            BellyMaterial.patternIndexGrid = null;
            BellyMaterial.forceEdgesDiagonal = false;
            BellyMaterial.exteriorFacadeBorderGrid = null;
            BellyMaterial.facadeTopGrid = null;
            BellyMaterial.bridgeGrid = null;


            DungeonTileStampData m_BellyStampData = ScriptableObject.CreateInstance<DungeonTileStampData>();
            m_BellyStampData.name = "ENV_BELLY_STAMP_DATA";
            m_BellyStampData.tileStampWeight = 1;
            m_BellyStampData.spriteStampWeight = 0;
            m_BellyStampData.objectStampWeight = 1;
            m_BellyStampData.stamps = new TileStampData[0];
            m_BellyStampData.spriteStamps = new SpriteStampData[0];
            m_BellyStampData.objectStamps = new ObjectStampData[] {
                new ObjectStampData() {
                    width = 1,
                    height = 1,
                    relativeWeight = 1,
                    placementRule = DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL,
                    occupySpace = DungeonTileStampData.StampSpace.OBJECT_SPACE,
                    stampCategory = DungeonTileStampData.StampCategory.NATURAL,
                    preferredIntermediaryStamps = 2,
                    intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.SKELETON,
                    requiresForcedMatchingStyle = false,
                    opulence = Opulence.PLAIN,
                    roomTypeData = new List<StampPerRoomPlacementSettings>(0),
                    indexOfSymmetricPartner = -1,
                    preventRoomRepeats = false,
                    objectReference = sharedAssets.LoadAsset<GameObject>("Big_Skull_001")
                },
                new ObjectStampData() {
                    width = 1,
                    height = 1,
                    relativeWeight = 1,
                    placementRule = DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL,
                    occupySpace = DungeonTileStampData.StampSpace.OBJECT_SPACE,
                    stampCategory = DungeonTileStampData.StampCategory.NATURAL,
                    preferredIntermediaryStamps = 2,
                    intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.SKELETON,
                    requiresForcedMatchingStyle = false,
                    opulence = Opulence.PLAIN,
                    roomTypeData = new List<StampPerRoomPlacementSettings>(0),
                    indexOfSymmetricPartner = -1,
                    preventRoomRepeats = false,
                    objectReference = sharedAssets2.LoadAsset<GameObject>("Big_Skull_002")
                },
                new ObjectStampData() {
                    width = 1,
                    height = 1,
                    relativeWeight = 1,
                    placementRule = DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL,
                    occupySpace = DungeonTileStampData.StampSpace.OBJECT_SPACE,
                    stampCategory = DungeonTileStampData.StampCategory.NATURAL,
                    preferredIntermediaryStamps = 2,
                    intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.SKELETON,
                    requiresForcedMatchingStyle = false,
                    opulence = Opulence.PLAIN,
                    roomTypeData = new List<StampPerRoomPlacementSettings>(0),
                    indexOfSymmetricPartner = -1,
                    preventRoomRepeats = false,
                    objectReference = sharedAssets2.LoadAsset<GameObject>("Big_Skull_003")
                },
                new ObjectStampData() {
                    width = 2,
                    height = 1,
                    relativeWeight = 1,
                    placementRule = DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL,
                    occupySpace = DungeonTileStampData.StampSpace.OBJECT_SPACE,
                    stampCategory = DungeonTileStampData.StampCategory.NATURAL,
                    preferredIntermediaryStamps = 2,
                    intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.SKELETON,
                    requiresForcedMatchingStyle = false,
                    opulence = Opulence.PLAIN,
                    roomTypeData = new List<StampPerRoomPlacementSettings>(0),
                    indexOfSymmetricPartner = -1,
                    preventRoomRepeats = false,
                    objectReference = sharedAssets.LoadAsset<GameObject>("Skull_Pile_001")
                },
                new ObjectStampData() {
                    width = 1,
                    height = 2,
                    relativeWeight = 1,
                    placementRule = DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL,
                    occupySpace = DungeonTileStampData.StampSpace.OBJECT_SPACE,
                    stampCategory = DungeonTileStampData.StampCategory.NATURAL,
                    preferredIntermediaryStamps = 4,
                    intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.SKELETON,
                    requiresForcedMatchingStyle = false,
                    opulence = Opulence.PLAIN,
                    roomTypeData = new List<StampPerRoomPlacementSettings>(0),
                    indexOfSymmetricPartner = -1,
                    preventRoomRepeats = false,
                    objectReference = sharedAssets.LoadAsset<GameObject>("Skeleton_Left_Sit_Corner")
                },
                new ObjectStampData() {
                    width = 1,
                    height = 2,
                    relativeWeight = 1,
                    placementRule = DungeonTileStampData.StampPlacementRule.BELOW_LOWER_FACEWALL,
                    occupySpace = DungeonTileStampData.StampSpace.OBJECT_SPACE,
                    stampCategory = DungeonTileStampData.StampCategory.NATURAL,
                    preferredIntermediaryStamps = 4,
                    intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.SKELETON,
                    requiresForcedMatchingStyle = false,
                    opulence = Opulence.PLAIN,
                    roomTypeData = new List<StampPerRoomPlacementSettings>(0),
                    indexOfSymmetricPartner = -1,
                    preventRoomRepeats = false,
                    objectReference = sharedAssets2.LoadAsset<GameObject>("Skeleton_Right_Sit_Corner")
                },
            };

            // 
            m_BellyStampData.SymmetricFrameChance = 0.1f;
            m_BellyStampData.SymmetricCompleteChance = 0.1f;
            dungeon.gameObject.name = "Base_Belly";
            dungeon.contentSource = ContentSource.CONTENT_UPDATE_03;
            dungeon.DungeonSeed = 0;
            dungeon.DungeonFloorName = "Inside the Beast";
            dungeon.DungeonShortName = "Inside the Beast";
            dungeon.DungeonFloorLevelTextOverride = "A Disgusting Place...";
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.NONE;
            dungeon.debugSettings = new DebugDungeonSettings() {
                RAPID_DEBUG_DUNGEON_ITERATION_SEEKER = false,
                RAPID_DEBUG_DUNGEON_ITERATION = false,
                RAPID_DEBUG_DUNGEON_COUNT = 50,
                GENERATION_VIEWER_MODE = false,
                FULL_MINIMAP_VISIBILITY = false,
                COOP_TEST = false,
                DISABLE_ENEMIES = false,
                DISABLE_LOOPS = false,
                DISABLE_SECRET_ROOM_COVERS = false,
                DISABLE_OUTLINES = false,
                WALLS_ARE_PITS = false
            };

            dungeon.PatternSettings = new SemioticDungeonGenSettings() {
                flows = new List<DungeonFlow>() { f2b_belly_flow_01.F2b_Belly_Flow_01() },
                mandatoryExtraRooms = new List<ExtraIncludedRoomData>(0),
                optionalExtraRooms = new List<ExtraIncludedRoomData>(0),
                MAX_GENERATION_ATTEMPTS = 250,
                DEBUG_RENDER_CANVASES_SEPARATELY = false
            };
            dungeon.ForceRegenerationOfCharacters = false;
            dungeon.ActuallyGenerateTilemap = true;
            dungeon.decoSettings = new TilemapDecoSettings {
                standardRoomVisualSubtypes = new WeightedIntCollection {
                    elements = new WeightedInt[] {
                        new WeightedInt() {
                            annotation = "belly",
                            value = 0,
                            weight = 1f,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "unused",
                            value = 1,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "shop",
                            value = 2,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "unused",
                            value = 3,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "unused",
                            value = 4,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        }
                    }
                },
                decalLayerStyle = TilemapDecoSettings.DecoStyle.NONE,
                decalSize = 3,
                decalSpacing = 1,
                decalExpansion = 0,
                patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE,
                patternSize = 3,
                patternSpacing = 3,
                patternExpansion = 0,
                decoPatchFrequency = 0.01f,
                ambientLightColor = new Color(0.925355f, 1f, 0.661765f, 1),
                ambientLightColorTwo = new Color(0.92549f, 1f, 0.662745f, 1),
                lowQualityAmbientLightColor = new Color(1, 1, 1, 1),
                lowQualityAmbientLightColorTwo = new Color(1, 1, 1, 1),
                lowQualityCheapLightVector = new Vector4(1, 0, -1, 0),
                UsesAlienFXFloorColor = false,
                AlienFXFloorColor = new Color(0, 0, 0, 1),
                generateLights = true,
                lightCullingPercentage = 0.2f,
                lightOverlapRadius = 8,
                nearestAllowedLight = 12,
                minLightExpanseWidth = 2,
                lightHeight = -2,
                lightCookies = new Texture2D[0],
                debug_view = false
            };
            
            dungeon.tileIndices = new TileIndices() {
                tilesetId = GlobalDungeonData.ValidTilesets.BELLYGEON,
                dungeonCollection = ExpandPrefabs.ENV_Tileset_Belly.GetComponent<tk2dSpriteCollectionData>(),
                dungeonCollectionSupportsDiagonalWalls = false,
                aoTileIndices = new AOTileIndices() {
                    AOFloorTileIndex = 0,
                    AOBottomWallBaseTileIndex = 1,
                    AOBottomWallTileRightIndex = 2,
                    AOBottomWallTileLeftIndex = 3,
                    AOBottomWallTileBothIndex = 4,
                    AOTopFacewallRightIndex = 6,
                    AOTopFacewallLeftIndex = 5,
                    AOTopFacewallBothIndex = 7,
                    AOFloorWallLeft = 5,
                    AOFloorWallRight = 6,
                    AOFloorWallBoth = 7,
                    AOFloorPizzaSliceLeft = 8,
                    AOFloorPizzaSliceRight = 9,
                    AOFloorPizzaSliceBoth = 10,
                    AOFloorPizzaSliceLeftWallRight = 11,
                    AOFloorPizzaSliceRightWallLeft = 12,
                    AOFloorWallUpAndLeft = 13,
                    AOFloorWallUpAndRight = 14,
                    AOFloorWallUpAndBoth = 15,
                    AOFloorDiagonalWallNortheast = -1,
                    AOFloorDiagonalWallNortheastLower = -1,
                    AOFloorDiagonalWallNortheastLowerJoint = -1,
                    AOFloorDiagonalWallNorthwest = -1,
                    AOFloorDiagonalWallNorthwestLower = -1,
                    AOFloorDiagonalWallNorthwestLowerJoint = -1,
                    AOBottomWallDiagonalNortheast = -1,
                    AOBottomWallDiagonalNorthwest = -1
                },
                placeBorders = true,
                placePits = false,
                chestHighWallIndices = new List<TileIndexVariant>() {
                    new TileIndexVariant() {
                        index = 41,
                        likelihood = 0.5f,
                        overrideLayerIndex = 0,
                        overrideIndex = 0
                    }
                },
                decalIndexGrid = null,
                patternIndexGrid = null,
                globalSecondBorderTiles = new List<int>(0),
                edgeDecorationTiles = null
            };
            
            dungeon.roomMaterialDefinitions = new DungeonMaterial[] {
                BellyMaterial,
                BellyMaterial,
                BellyMaterial,
                BellyMaterial,
                sharedAssets2.LoadAsset<DungeonMaterial>("Boss_Cathedral_StainedGlass_Lights")
            };
            dungeon.dungeonWingDefinitions = new DungeonWingDefinition[0];
            dungeon.pathGridDefinitions = new List<TileIndexGrid>() { MinesDungeonPrefab.pathGridDefinitions[0] };
            dungeon.dungeonDustups = new DustUpVFX() {
                runDustup = GungeonPrefab.dungeonDustups.runDustup,
                waterDustup = GungeonPrefab.dungeonDustups.waterDustup,
                additionalWaterDustup = GungeonPrefab.dungeonDustups.additionalWaterDustup,
                rollNorthDustup = GungeonPrefab.dungeonDustups.rollNorthDustup,
                rollNorthEastDustup = GungeonPrefab.dungeonDustups.rollNorthEastDustup,
                rollEastDustup = GungeonPrefab.dungeonDustups.rollEastDustup,
                rollSouthEastDustup = GungeonPrefab.dungeonDustups.rollSouthEastDustup,
                rollSouthDustup = GungeonPrefab.dungeonDustups.rollSouthDustup,
                rollSouthWestDustup = GungeonPrefab.dungeonDustups.rollSouthWestDustup,
                rollWestDustup = GungeonPrefab.dungeonDustups.rollWestDustup,
                rollNorthWestDustup = GungeonPrefab.dungeonDustups.rollNorthWestDustup,
                rollLandDustup = GungeonPrefab.dungeonDustups.rollLandDustup
            };
            dungeon.damageTypeEffectMatrix = GungeonPrefab.damageTypeEffectMatrix;
            dungeon.stampData = m_BellyStampData;
            dungeon.UsesCustomFloorIdea = false;
            dungeon.FloorIdea = new RobotDaveIdea() {
                ValidEasyEnemyPlaceables = new DungeonPlaceable[0],
                ValidHardEnemyPlaceables = new DungeonPlaceable[0],
                UseWallSawblades = false,
                UseRollingLogsVertical = false,
                UseRollingLogsHorizontal = false,
                UseFloorPitTraps = false,
                UseFloorFlameTraps = false,
                UseFloorSpikeTraps = false,
                UseFloorConveyorBelts = false,
                UseCaveIns = false,
                UseAlarmMushrooms = false,
                UseChandeliers = false,
                UseMineCarts = false,
                CanIncludePits = true
            };
            dungeon.PlaceDoors = true;
            dungeon.doorObjects = ExpandPrefabs.Belly_Doors;
            dungeon.oneWayDoorObjects = AbbeyPrefab.oneWayDoorObjects;
            dungeon.oneWayDoorPressurePlate = AbbeyPrefab.oneWayDoorPressurePlate;
            dungeon.phantomBlockerDoorObjects = AbbeyPrefab.phantomBlockerDoorObjects;
            dungeon.UsesWallWarpWingDoors = false;
            dungeon.baseChestContents = AbbeyPrefab.baseChestContents;
            dungeon.SecretRoomSimpleTriggersFacewall = new List<GameObject>() { SewersPrefab.SecretRoomSimpleTriggersFacewall[0] };
            dungeon.SecretRoomSimpleTriggersSidewall = new List<GameObject>() { SewersPrefab.SecretRoomSimpleTriggersSidewall[0] };
            dungeon.SecretRoomComplexTriggers = new List<ComplexSecretRoomTrigger>(0);
            dungeon.SecretRoomDoorSparkVFX = GungeonPrefab.SecretRoomDoorSparkVFX;
            dungeon.SecretRoomHorizontalPoofVFX = GungeonPrefab.SecretRoomHorizontalPoofVFX;
            dungeon.SecretRoomVerticalPoofVFX = GungeonPrefab.SecretRoomVerticalPoofVFX;
            dungeon.sharedSettingsPrefab = AbbeyPrefab.sharedSettingsPrefab;
            dungeon.NormalRatGUID = string.Empty;
            dungeon.BossMasteryTokenItemId = -1;
            dungeon.UsesOverrideTertiaryBossSets = false;
            dungeon.OverrideTertiaryRewardSets = new List<TertiaryBossRewardSet>(0);
            dungeon.defaultPlayerPrefab = AbbeyPrefab.defaultPlayerPrefab;
            dungeon.StripPlayerOnArrival = false;
            dungeon.SuppressEmergencyCrates = false;
            dungeon.SetTutorialFlag = false;
            dungeon.PlayerIsLight = false;
            dungeon.PlayerLightColor = new Color(1, 1, 1, 1);
            dungeon.PlayerLightIntensity = 3;
            dungeon.PlayerLightRadius = 5;
            dungeon.PrefabsToAutoSpawn = new GameObject[0];
            dungeon.musicEventName = AbbeyPrefab.musicEventName;

            expandSharedAuto1 = null;
            sharedAssets = null;
            sharedAssets2 = null;
            MinesDungeonPrefab = null;
            GungeonPrefab = null;
            AbbeyPrefab = null;
            
            return dungeon;
        }

        public static Dungeon WestDungeon(Dungeon dungeon) {
            AssetBundle expandSharedAuto1 = ResourceManager.LoadAssetBundle(ExpandTheGungeon.ModAssetBundleName);
            AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            Dungeon CastlePrefab = GetOrLoadByName_Orig("Base_Castle");
            

            DungeonMaterial West_Canyon = ScriptableObject.CreateInstance<DungeonMaterial>();
            West_Canyon.name = "West_Canyon";
            West_Canyon.wallShards = dungeon.roomMaterialDefinitions[0].wallShards;
            West_Canyon.bigWallShards = dungeon.roomMaterialDefinitions[0].bigWallShards;
            West_Canyon.bigWallShardDamageThreshold = 10;
            West_Canyon.fallbackVerticalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            West_Canyon.fallbackHorizontalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            West_Canyon.pitfallVFXPrefab = null;
            West_Canyon.UsePitAmbientVFX = false;
            West_Canyon.AmbientPitVFX = new List<GameObject>(0);
            West_Canyon.PitVFXMinCooldown = 5;
            West_Canyon.PitVFXMaxCooldown = 30;
            West_Canyon.ChanceToSpawnPitVFXOnCooldown = 1;
            West_Canyon.stampFailChance = 0.2f;
            West_Canyon.overrideTableTable = null;
            West_Canyon.supportsPits = true;
            West_Canyon.doPitAO = true;
            West_Canyon.pitsAreOneDeep = false;
            West_Canyon.supportsDiagonalWalls = false;
            West_Canyon.supportsUpholstery = false;
            West_Canyon.carpetIsMainFloor = false;
            West_Canyon.carpetGrids = new TileIndexGrid[0];
            West_Canyon.supportsChannels = false;
            West_Canyon.minChannelPools = 0;
            West_Canyon.maxChannelPools = 3;            
            West_Canyon.channelTenacity = 0.75f;
            West_Canyon.channelGrids = new TileIndexGrid[0];
            West_Canyon.supportsLavaOrLavalikeSquares = false;
            West_Canyon.lavaGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/Canyon/lavaGrid") };
            West_Canyon.supportsIceSquares = false;
            West_Canyon.iceGrids = new TileIndexGrid[0];
            West_Canyon.roomFloorBorderGrid = null;
            West_Canyon.roomCeilingBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/Canyon/roomCeilingBorderGrid");
            West_Canyon.pitLayoutGrid = null;
            West_Canyon.pitBorderRaisedGrid = null;
            West_Canyon.additionalPitBorderFlatGrid = null;
            West_Canyon.pitBorderFlatGrid = null;
            West_Canyon.outerCeilingBorderGrid = null;
            West_Canyon.floorSquareDensity = 0f;
            West_Canyon.floorSquares = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/Canyon/floorGrid1") };
            West_Canyon.usesFacewallGrids = false;
            West_Canyon.facewallGrids = new FacewallIndexGridDefinition[0];
            West_Canyon.usesInternalMaterialTransitions = false;
            West_Canyon.usesProceduralMaterialTransitions = false;
            West_Canyon.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            West_Canyon.secretRoomWallShardCollections = new List<GameObject>(0);
            West_Canyon.overrideStoneFloorType = false;
            West_Canyon.overrideFloorType = CellVisualData.CellFloorType.Stone;
            West_Canyon.useLighting = true;
            West_Canyon.lightPrefabs = new WeightedGameObjectCollection() {
               elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = ExpandPrefabs.WestLight,
                       weight = 1,                       
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]                       
                   }
               }
            };
            West_Canyon.facewallLightStamps = dungeon.roomMaterialDefinitions[0].facewallLightStamps;
            West_Canyon.sidewallLightStamps = dungeon.roomMaterialDefinitions[0].sidewallLightStamps;
            West_Canyon.usesDecalLayer = false;
            West_Canyon.decalIndexGrid = null;
            West_Canyon.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            West_Canyon.decalSize = 1;
            West_Canyon.decalSpacing = 1;
            West_Canyon.usesPatternLayer = false;
            West_Canyon.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            West_Canyon.patternSpacing = 1;
            West_Canyon.patternSize = 1;
            West_Canyon.patternIndexGrid = null;
            West_Canyon.forceEdgesDiagonal = true;
            West_Canyon.exteriorFacadeBorderGrid = null;
            West_Canyon.facadeTopGrid = null;
            West_Canyon.bridgeGrid = null;

            DungeonMaterial West_Wood_Interior = ScriptableObject.CreateInstance<DungeonMaterial>();
            West_Wood_Interior.name = "West_Wood_Interior";
            West_Wood_Interior.wallShards = dungeon.roomMaterialDefinitions[0].wallShards;
            West_Wood_Interior.bigWallShards = dungeon.roomMaterialDefinitions[0].bigWallShards;
            West_Wood_Interior.bigWallShardDamageThreshold = 10;
            West_Wood_Interior.fallbackVerticalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            West_Wood_Interior.fallbackHorizontalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            West_Wood_Interior.pitfallVFXPrefab = null;
            West_Wood_Interior.UsePitAmbientVFX = false;
            West_Wood_Interior.AmbientPitVFX = new List<GameObject>(0);
            West_Wood_Interior.PitVFXMinCooldown = 5;
            West_Wood_Interior.PitVFXMaxCooldown = 30;
            West_Wood_Interior.ChanceToSpawnPitVFXOnCooldown = 1;
            West_Wood_Interior.stampFailChance = 0.2f;
            West_Wood_Interior.overrideTableTable = null;
            West_Wood_Interior.supportsPits = false;
            West_Wood_Interior.doPitAO = true;
            West_Wood_Interior.pitsAreOneDeep = false;
            West_Wood_Interior.supportsDiagonalWalls = false;
            West_Wood_Interior.supportsUpholstery = true;
            West_Wood_Interior.carpetIsMainFloor = false;
            West_Wood_Interior.carpetGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/WoodInterior/carpetGrid1") };
            West_Wood_Interior.supportsChannels = false;
            West_Wood_Interior.minChannelPools = 0;
            West_Wood_Interior.maxChannelPools = 3;            
            West_Wood_Interior.channelTenacity = 0.75f;
            West_Wood_Interior.channelGrids = new TileIndexGrid[0];
            West_Wood_Interior.supportsLavaOrLavalikeSquares = false;
            West_Wood_Interior.lavaGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/WoodInterior/lavaGrid") };
            West_Wood_Interior.supportsIceSquares = false;
            West_Wood_Interior.iceGrids = new TileIndexGrid[0];
            West_Wood_Interior.roomFloorBorderGrid = null;
            West_Wood_Interior.roomCeilingBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/WoodInterior/roomCeilingBorderGrid");
            West_Wood_Interior.roomCeilingBorderGrid.topCapIndices.indices[0] = 575;
            West_Wood_Interior.pitLayoutGrid = null;
            West_Wood_Interior.pitBorderRaisedGrid = null;
            West_Wood_Interior.additionalPitBorderFlatGrid = null;
            West_Wood_Interior.pitBorderFlatGrid = null;
            West_Wood_Interior.outerCeilingBorderGrid = null;
            West_Wood_Interior.floorSquareDensity = 0f;
            West_Wood_Interior.floorSquares = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/WoodInterior/floorGrid1") };
            West_Wood_Interior.usesFacewallGrids = false;
            West_Wood_Interior.facewallGrids = new FacewallIndexGridDefinition[0];
            West_Wood_Interior.usesInternalMaterialTransitions = false;
            West_Wood_Interior.usesProceduralMaterialTransitions = false;
            West_Wood_Interior.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            West_Wood_Interior.secretRoomWallShardCollections = new List<GameObject>(0);
            West_Wood_Interior.overrideStoneFloorType = false;
            West_Wood_Interior.overrideFloorType = CellVisualData.CellFloorType.Stone;
            West_Wood_Interior.useLighting = true;
            West_Wood_Interior.lightPrefabs = new WeightedGameObjectCollection() {
               elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = CastlePrefab.roomMaterialDefinitions[0].lightPrefabs.elements[0].rawGameObject,
                       weight = 1,                       
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]                       
                   }
               }
            };
            West_Wood_Interior.facewallLightStamps = dungeon.roomMaterialDefinitions[0].facewallLightStamps;
            West_Wood_Interior.sidewallLightStamps = dungeon.roomMaterialDefinitions[0].sidewallLightStamps;
            West_Wood_Interior.usesDecalLayer = false;
            West_Wood_Interior.decalIndexGrid = null;
            West_Wood_Interior.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            West_Wood_Interior.decalSize = 1;
            West_Wood_Interior.decalSpacing = 1;
            West_Wood_Interior.usesPatternLayer = false;
            West_Wood_Interior.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            West_Wood_Interior.patternSpacing = 1;
            West_Wood_Interior.patternSize = 1;
            West_Wood_Interior.patternIndexGrid = null;
            West_Wood_Interior.forceEdgesDiagonal = false;
            West_Wood_Interior.exteriorFacadeBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/WoodInterior/exteriorFacadeBorderGrid");
            West_Wood_Interior.facadeTopGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/WoodInterior/facadeTopGrid");
            West_Wood_Interior.bridgeGrid = null;
            

            DungeonMaterial West_Red_Interior = ScriptableObject.CreateInstance<DungeonMaterial>();
            West_Red_Interior.name = "West_Red_Interior";
            West_Red_Interior.wallShards = dungeon.roomMaterialDefinitions[0].wallShards;
            West_Red_Interior.bigWallShards = dungeon.roomMaterialDefinitions[0].bigWallShards;
            West_Red_Interior.bigWallShardDamageThreshold = 10;
            West_Red_Interior.fallbackVerticalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            West_Red_Interior.fallbackHorizontalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            West_Red_Interior.pitfallVFXPrefab = null;
            West_Red_Interior.UsePitAmbientVFX = false;
            West_Red_Interior.AmbientPitVFX = new List<GameObject>(0);
            West_Red_Interior.PitVFXMinCooldown = 5;
            West_Red_Interior.PitVFXMaxCooldown = 30;
            West_Red_Interior.ChanceToSpawnPitVFXOnCooldown = 1;
            West_Red_Interior.stampFailChance = 0.2f;
            West_Red_Interior.overrideTableTable = null;
            West_Red_Interior.supportsPits = false;
            West_Red_Interior.doPitAO = true;
            West_Red_Interior.pitsAreOneDeep = false;
            West_Red_Interior.supportsDiagonalWalls = false;
            West_Red_Interior.supportsUpholstery = true;
            West_Red_Interior.carpetIsMainFloor = false;
            West_Red_Interior.carpetGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/RedInterior/carpetGrid1") };
            West_Red_Interior.supportsChannels = false;
            West_Red_Interior.minChannelPools = 0;
            West_Red_Interior.maxChannelPools = 3;            
            West_Red_Interior.channelTenacity = 0.75f;
            West_Red_Interior.channelGrids = new TileIndexGrid[0];
            West_Red_Interior.supportsLavaOrLavalikeSquares = false;
            West_Red_Interior.lavaGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/RedInterior/lavaGrid") };
            West_Red_Interior.supportsIceSquares = false;
            West_Red_Interior.iceGrids = new TileIndexGrid[0];
            West_Red_Interior.roomFloorBorderGrid = null;
            West_Red_Interior.roomCeilingBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/RedInterior/roomCeilingBorderGrid");
            West_Red_Interior.roomCeilingBorderGrid.topCapIndices.indices[0] = 580;
            West_Red_Interior.pitLayoutGrid = null;
            West_Red_Interior.pitBorderRaisedGrid = null;
            West_Red_Interior.additionalPitBorderFlatGrid = null;
            West_Red_Interior.pitBorderFlatGrid = null;
            West_Red_Interior.outerCeilingBorderGrid = null;
            West_Red_Interior.floorSquareDensity = 0f;
            West_Red_Interior.floorSquares = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/RedInterior/floorGrid1") };
            West_Red_Interior.usesFacewallGrids = false;
            West_Red_Interior.facewallGrids = new FacewallIndexGridDefinition[0];
            West_Red_Interior.usesInternalMaterialTransitions = false;
            West_Red_Interior.usesProceduralMaterialTransitions = false;
            West_Red_Interior.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            West_Red_Interior.secretRoomWallShardCollections = new List<GameObject>(0);
            West_Red_Interior.overrideStoneFloorType = false;
            West_Red_Interior.overrideFloorType = CellVisualData.CellFloorType.Stone;
            West_Red_Interior.useLighting = true;
            West_Red_Interior.lightPrefabs = new WeightedGameObjectCollection() {
               elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = CastlePrefab.roomMaterialDefinitions[0].lightPrefabs.elements[0].rawGameObject,
                       weight = 1,                       
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]                       
                   }
               }
            };
            West_Red_Interior.facewallLightStamps = dungeon.roomMaterialDefinitions[0].facewallLightStamps;
            West_Red_Interior.sidewallLightStamps = dungeon.roomMaterialDefinitions[0].sidewallLightStamps;
            West_Red_Interior.usesDecalLayer = false;
            West_Red_Interior.decalIndexGrid = null;
            West_Red_Interior.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            West_Red_Interior.decalSize = 1;
            West_Red_Interior.decalSpacing = 1;
            West_Red_Interior.usesPatternLayer = false;
            West_Red_Interior.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            West_Red_Interior.patternSpacing = 1;
            West_Red_Interior.patternSize = 1;
            West_Red_Interior.patternIndexGrid = null;
            West_Red_Interior.forceEdgesDiagonal = false;
            West_Red_Interior.exteriorFacadeBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/RedInterior/exteriorFacadeBorderGrid");
            West_Red_Interior.facadeTopGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "WestAssets/RedInterior/facadeTopGrid");
            West_Red_Interior.bridgeGrid = null;
            



            dungeon.gameObject.name = "Base_West";
            dungeon.contentSource = ContentSource.CONTENT_UPDATE_03;
            dungeon.DungeonSeed = 0;
            dungeon.DungeonFloorName = "Old West";
            dungeon.DungeonShortName = "Old West";
            dungeon.DungeonFloorLevelTextOverride = "Old Western";
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.NONE;
            dungeon.debugSettings = new DebugDungeonSettings() {
                RAPID_DEBUG_DUNGEON_ITERATION_SEEKER = false,
                RAPID_DEBUG_DUNGEON_ITERATION = false,
                RAPID_DEBUG_DUNGEON_COUNT = 50,
                GENERATION_VIEWER_MODE = false,
                FULL_MINIMAP_VISIBILITY = false,
                COOP_TEST = false,
                DISABLE_ENEMIES = false,
                DISABLE_LOOPS = false,
                DISABLE_SECRET_ROOM_COVERS = false,
                DISABLE_OUTLINES = false,
                WALLS_ARE_PITS = false
            };

            dungeon.PatternSettings.flows = new List<DungeonFlow>() { f4c_west_flow_01.F4c_West_Flow_01() };
            dungeon.PatternSettings.MAX_GENERATION_ATTEMPTS = 1;
            dungeon.ForceRegenerationOfCharacters = false;
            dungeon.ActuallyGenerateTilemap = true;
            dungeon.decoSettings = new TilemapDecoSettings {
                standardRoomVisualSubtypes = new WeightedIntCollection {
                    elements = new WeightedInt[] {
                        new WeightedInt() {
                            annotation = "canyon",
                            value = 0,
                            weight = 1,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "interior wood",
                            value = 1,
                            weight = 1,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "interior red",
                            value = 2,
                            weight = 1,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "unused",
                            value = 3,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "unused",
                            value = 4,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        },
                        new WeightedInt() {
                            annotation = "unused",
                            value = 5,
                            weight = 0,
                            additionalPrerequisites = new DungeonPrerequisite[0]
                        }
                    }
                },
                decalLayerStyle = TilemapDecoSettings.DecoStyle.NONE,
                decalSize = 3,
                decalSpacing = 1,
                decalExpansion = 0,
                patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE,
                patternSize = 3,
                patternSpacing = 3,
                patternExpansion = 0,
                decoPatchFrequency = 0.01f,
                ambientLightColor = new Color(0.81532f, 0.786505f, 0.905882f, 1),
                ambientLightColorTwo = new Color(0.76826f, 0.724221f, 0.905882f, 1),
                lowQualityAmbientLightColor = new Color(0.946203f, 0.941987f, 0.977941f, 1),
                lowQualityAmbientLightColorTwo = new Color(0.985294f, 0.943724f, 0.891112f, 1),
                lowQualityCheapLightVector = new Vector4(1, 0, -1, 0),
                UsesAlienFXFloorColor = false,
                AlienFXFloorColor = new Color(0, 0, 0, 1),
                generateLights = true,
                lightCullingPercentage = 0.2f,
                lightOverlapRadius = 8,
                nearestAllowedLight = 12,
                minLightExpanseWidth = 2,
                lightHeight = -2,
                lightCookies = new Texture2D[0],
                debug_view = false
            };

            dungeon.tileIndices = new TileIndices() {
                tilesetId = GlobalDungeonData.ValidTilesets.WESTGEON,
                dungeonCollection = ExpandPrefabs.ENV_Tileset_West.GetComponent<tk2dSpriteCollectionData>(),
                dungeonCollectionSupportsDiagonalWalls = false,
                aoTileIndices = new AOTileIndices() {
                    AOFloorTileIndex = 0,
                    AOBottomWallBaseTileIndex = 1,
                    AOBottomWallTileRightIndex = 2,
                    AOBottomWallTileLeftIndex = 3,
                    AOBottomWallTileBothIndex = 4,
                    AOTopFacewallRightIndex = 6,
                    AOTopFacewallLeftIndex = 5,
                    AOTopFacewallBothIndex = 7,
                    AOFloorWallLeft = 5,
                    AOFloorWallRight = 6,
                    AOFloorWallBoth = 7,
                    AOFloorPizzaSliceLeft = 8,
                    AOFloorPizzaSliceRight = 9,
                    AOFloorPizzaSliceBoth = 10,
                    AOFloorPizzaSliceLeftWallRight = 11,
                    AOFloorPizzaSliceRightWallLeft = 12,
                    AOFloorWallUpAndLeft = 13,
                    AOFloorWallUpAndRight = 14,
                    AOFloorWallUpAndBoth = 15,
                    AOFloorDiagonalWallNortheast = 42,
                    AOFloorDiagonalWallNortheastLower = 64,
                    AOFloorDiagonalWallNortheastLowerJoint = 86,
                    AOFloorDiagonalWallNorthwest = 43,
                    AOFloorDiagonalWallNorthwestLower = 65,
                    AOFloorDiagonalWallNorthwestLowerJoint = 87,
                    AOBottomWallDiagonalNortheast = -1,
                    AOBottomWallDiagonalNorthwest = -1
                },
                placeBorders = true,
                placePits = false,
                chestHighWallIndices = new List<TileIndexVariant>() {
                    new TileIndexVariant() {
                        index = 41,
                        likelihood = 0.5f,
                        overrideLayerIndex = 0,
                        overrideIndex = 0
                    }
                },
                decalIndexGrid = null,
                patternIndexGrid = null,
                globalSecondBorderTiles = new List<int>(0),
                edgeDecorationTiles = null
            };
            
            dungeon.roomMaterialDefinitions = new DungeonMaterial[] {
                West_Canyon,
                West_Wood_Interior,
                West_Red_Interior,
                West_Canyon,
                West_Canyon,
                West_Canyon,
                sharedAssets2.LoadAsset<DungeonMaterial>("Boss_Cathedral_StainedGlass_Lights")
            };

            dungeon.dungeonWingDefinitions = new DungeonWingDefinition[0];
            
            ObjectStampData[] m_GungeonObjectStampData = dungeon.stampData.objectStamps;

            dungeon.stampData = ScriptableObject.CreateInstance<DungeonTileStampData>();
            dungeon.stampData.name = "ENV_WEST_STAMP_DATA";
            dungeon.stampData.tileStampWeight = 1;
            dungeon.stampData.spriteStampWeight = 0;
            dungeon.stampData.objectStampWeight = 1;
            dungeon.stampData.stamps = new TileStampData[0];
            dungeon.stampData.spriteStamps = new SpriteStampData[0];
            dungeon.stampData.objectStamps = m_GungeonObjectStampData;
            dungeon.stampData.SymmetricFrameChance = 0.1f;
            dungeon.stampData.SymmetricCompleteChance = 0.1f;
            dungeon.UsesCustomFloorIdea = false;
            // dungeon.FloorIdea
            dungeon.PlaceDoors = true;
            dungeon.doorObjects = ExpandPrefabs.West_Doors;
            // dungeon.lockedDoorObjects = ???
            // dungeon.oneWayDoorObjects = Gungeon One Ways
            // dungeon.oneWayDoorPressurePlate = Gungeon Pressure Plate
            // dungeon.UsesWallWarpWingDoors = false; // Will allow this for now
            // dungeon.WarpWingDoorPrefab = Gungeon Warp Wing Door Prefab
            // dungeon.baseChestContents = Gungeon Base Contents
            // dungeon.SecretRoomDoorSparkVFX = Gungeon door sparks
            // dungeon.SecretRoomHorizontalPoofVFX = Gungeon poof vfx
            // dungeon.SecretRoomVerticalPoofVFX = Gungoen vertical poof vfx
            // dungeon.sharedSettingsPrefab = Common shared settings. Doesn't need to be redefined.
            dungeon.BossMasteryTokenItemId = -1;
            dungeon.UsesOverrideTertiaryBossSets = false;
            dungeon.OverrideTertiaryRewardSets = new List<TertiaryBossRewardSet>(0);
            // dungeon.defaultPlayerPrefab = Convict
            dungeon.StripPlayerOnArrival = false;
            dungeon.SuppressEmergencyCrates = false;
            dungeon.SetTutorialFlag = false;
            dungeon.PlayerIsLight = false;
            dungeon.PlayerLightColor = Color.white;
            dungeon.PlayerLightIntensity = 3;
            dungeon.PlayerLightRadius = 5;
            // dungeon.musicEventName = string.Empty;
            dungeon.musicEventName = "Play_MUS_Dungeon_Rat_Theme_01";

            expandSharedAuto1 = null;
            CastlePrefab = null;
            sharedAssets2 = null;

            return dungeon;
        }

        public static Dungeon PhobosDungeon(Dungeon dungeon) {
            AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            AssetBundle expandSharedAuto1 = ResourceManager.LoadAssetBundle(ExpandTheGungeon.ModAssetBundleName);
            Dungeon NakatomiPrefab = GetOrLoadByName_Orig("Base_Nakatomi");
            Dungeon SewersPrefab = GetOrLoadByName_Orig("Base_Sewer");
            Dungeon CastlePrefab = GetOrLoadByName_Orig("Base_Castle");
            Dungeon CatacombsPrefab = GetOrLoadByName_Orig("Base_Catacombs");


            DungeonMaterial m_PhobosBlue = ScriptableObject.CreateInstance<DungeonMaterial>();
            m_PhobosBlue.name = "Phobos_Blue";
            m_PhobosBlue.wallShards = CastlePrefab.roomMaterialDefinitions[0].wallShards;
            m_PhobosBlue.bigWallShards = new WeightedGameObjectCollection() { elements = new List<WeightedGameObject>(0) };
            m_PhobosBlue.bigWallShardDamageThreshold = 10;
            m_PhobosBlue.fallbackVerticalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            m_PhobosBlue.fallbackHorizontalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            m_PhobosBlue.pitfallVFXPrefab = SewersPrefab.roomMaterialDefinitions[0].pitfallVFXPrefab;
            m_PhobosBlue.UsePitAmbientVFX = false;
            m_PhobosBlue.AmbientPitVFX = new List<GameObject>(0);
            m_PhobosBlue.PitVFXMinCooldown = 5;
            m_PhobosBlue.PitVFXMaxCooldown = 30;
            m_PhobosBlue.ChanceToSpawnPitVFXOnCooldown = 1;
            m_PhobosBlue.stampFailChance = 0.2f;
            m_PhobosBlue.overrideTableTable = null;
            m_PhobosBlue.supportsPits = true;
            m_PhobosBlue.doPitAO = true;
            m_PhobosBlue.pitsAreOneDeep = false;
            m_PhobosBlue.supportsDiagonalWalls = false;
            m_PhobosBlue.supportsUpholstery = false;
            m_PhobosBlue.carpetIsMainFloor = false;
            m_PhobosBlue.carpetGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Blue/carpetGrid") };
            m_PhobosBlue.supportsChannels = false;
            m_PhobosBlue.minChannelPools = 0;
            m_PhobosBlue.maxChannelPools = 3;
            m_PhobosBlue.channelTenacity = 0.75f;
            m_PhobosBlue.channelGrids = new TileIndexGrid[0];
            m_PhobosBlue.supportsLavaOrLavalikeSquares = false;
            m_PhobosBlue.lavaGrids = new TileIndexGrid[0];
            m_PhobosBlue.supportsIceSquares = false;
            m_PhobosBlue.iceGrids = new TileIndexGrid[0];
            m_PhobosBlue.roomFloorBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Blue/roomFloorBorderGrid");
            m_PhobosBlue.roomCeilingBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Blue/roomCeilingBorderGrid");
            m_PhobosBlue.pitLayoutGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Blue/pitLayoutGrid");
            m_PhobosBlue.pitBorderFlatGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Blue/pitBorderFlatGrid");
            m_PhobosBlue.pitBorderRaisedGrid = null;
            m_PhobosBlue.additionalPitBorderFlatGrid = null;
            m_PhobosBlue.outerCeilingBorderGrid = null;
            m_PhobosBlue.floorSquareDensity = 0.05f;
            m_PhobosBlue.floorSquares = new TileIndexGrid[0];
            m_PhobosBlue.usesFacewallGrids = false;
            m_PhobosBlue.facewallGrids = CatacombsPrefab.roomMaterialDefinitions[0].facewallGrids;
            m_PhobosBlue.usesInternalMaterialTransitions = false;
            m_PhobosBlue.usesProceduralMaterialTransitions = false;
            m_PhobosBlue.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            m_PhobosBlue.secretRoomWallShardCollections = new List<GameObject>(0);
            m_PhobosBlue.overrideStoneFloorType = false;
            m_PhobosBlue.overrideFloorType = CellVisualData.CellFloorType.Stone;
            m_PhobosBlue.useLighting = true;
            m_PhobosBlue.lightPrefabs = new WeightedGameObjectCollection() {
                elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = ExpandPrefabs.PhobosLight,
                       weight = 1,
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]
                   }
               }
            };
            m_PhobosBlue.facewallLightStamps = dungeon.roomMaterialDefinitions[0].facewallLightStamps;
            m_PhobosBlue.sidewallLightStamps = dungeon.roomMaterialDefinitions[0].sidewallLightStamps;
            m_PhobosBlue.usesDecalLayer = false;
            m_PhobosBlue.decalIndexGrid = null;
            m_PhobosBlue.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            m_PhobosBlue.decalSize = 1;
            m_PhobosBlue.decalSpacing = 1;
            m_PhobosBlue.usesPatternLayer = false;
            m_PhobosBlue.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            m_PhobosBlue.patternSpacing = 1;
            m_PhobosBlue.patternSize = 1;
            m_PhobosBlue.patternIndexGrid = null;
            m_PhobosBlue.forceEdgesDiagonal = false;
            m_PhobosBlue.exteriorFacadeBorderGrid = null;
            m_PhobosBlue.facadeTopGrid = null;
            m_PhobosBlue.bridgeGrid = null;

            DungeonMaterial m_PhobosGrey = ScriptableObject.CreateInstance<DungeonMaterial>();
            m_PhobosGrey.name = "Phobos_Grey";
            m_PhobosGrey.wallShards = CastlePrefab.roomMaterialDefinitions[0].wallShards;
            m_PhobosGrey.bigWallShards = new WeightedGameObjectCollection() { elements = new List<WeightedGameObject>(0) };
            m_PhobosGrey.bigWallShardDamageThreshold = 10;
            m_PhobosGrey.fallbackVerticalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            m_PhobosGrey.fallbackHorizontalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            m_PhobosGrey.pitfallVFXPrefab = SewersPrefab.roomMaterialDefinitions[0].pitfallVFXPrefab;
            m_PhobosGrey.UsePitAmbientVFX = false;
            m_PhobosGrey.AmbientPitVFX = new List<GameObject>(0);
            m_PhobosGrey.PitVFXMinCooldown = 5;
            m_PhobosGrey.PitVFXMaxCooldown = 30;
            m_PhobosGrey.ChanceToSpawnPitVFXOnCooldown = 1;
            m_PhobosGrey.stampFailChance = 0.2f;
            m_PhobosGrey.overrideTableTable = null;
            m_PhobosGrey.supportsPits = true;
            m_PhobosGrey.doPitAO = true;
            m_PhobosGrey.pitsAreOneDeep = false;
            m_PhobosGrey.supportsDiagonalWalls = false;
            m_PhobosGrey.supportsUpholstery = false;
            m_PhobosGrey.carpetIsMainFloor = false;
            m_PhobosGrey.carpetGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Grey/carpetGrid") };
            m_PhobosGrey.supportsChannels = false;
            m_PhobosGrey.minChannelPools = 0;
            m_PhobosGrey.maxChannelPools = 3;
            m_PhobosGrey.channelTenacity = 0.75f;
            m_PhobosGrey.channelGrids = new TileIndexGrid[0];
            m_PhobosGrey.supportsLavaOrLavalikeSquares = false;
            m_PhobosGrey.lavaGrids = new TileIndexGrid[0];
            m_PhobosGrey.supportsIceSquares = false;
            m_PhobosGrey.iceGrids = new TileIndexGrid[0];
            m_PhobosGrey.roomFloorBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Grey/roomFloorBorderGrid");
            m_PhobosGrey.roomCeilingBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Grey/roomCeilingBorderGrid");

            m_PhobosGrey.pitLayoutGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Grey/pitLayoutGrid");
            m_PhobosGrey.pitBorderFlatGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Grey/pitBorderFlatGrid");
            m_PhobosGrey.pitBorderRaisedGrid = null;
            m_PhobosGrey.additionalPitBorderFlatGrid = null;
            m_PhobosGrey.outerCeilingBorderGrid = null;
            m_PhobosGrey.floorSquareDensity = 0.05f;
            m_PhobosGrey.floorSquares = new TileIndexGrid[0];
            m_PhobosGrey.usesFacewallGrids = false;
            m_PhobosGrey.facewallGrids = CatacombsPrefab.roomMaterialDefinitions[0].facewallGrids;
            m_PhobosGrey.usesInternalMaterialTransitions = false;
            m_PhobosGrey.usesProceduralMaterialTransitions = false;
            m_PhobosGrey.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            m_PhobosGrey.secretRoomWallShardCollections = new List<GameObject>(0);
            m_PhobosGrey.overrideStoneFloorType = false;
            m_PhobosGrey.overrideFloorType = CellVisualData.CellFloorType.Stone;
            m_PhobosGrey.useLighting = true;
            m_PhobosGrey.lightPrefabs = new WeightedGameObjectCollection() {
                elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = ExpandPrefabs.PhobosLight,
                       weight = 1,
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]
                   }
               }
            };
            m_PhobosGrey.facewallLightStamps = dungeon.roomMaterialDefinitions[0].facewallLightStamps;
            m_PhobosGrey.sidewallLightStamps = dungeon.roomMaterialDefinitions[0].sidewallLightStamps;
            m_PhobosGrey.usesDecalLayer = false;
            m_PhobosGrey.decalIndexGrid = null;
            m_PhobosGrey.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            m_PhobosGrey.decalSize = 1;
            m_PhobosGrey.decalSpacing = 1;
            m_PhobosGrey.usesPatternLayer = false;
            m_PhobosGrey.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            m_PhobosGrey.patternSpacing = 1;
            m_PhobosGrey.patternSize = 1;
            m_PhobosGrey.patternIndexGrid = null;
            m_PhobosGrey.forceEdgesDiagonal = false;
            m_PhobosGrey.exteriorFacadeBorderGrid = null;
            m_PhobosGrey.facadeTopGrid = null;
            m_PhobosGrey.bridgeGrid = null;

            DungeonMaterial m_PhobosGold = ScriptableObject.CreateInstance<DungeonMaterial>();
            m_PhobosGold.name = "Phobos_Gold";
            m_PhobosGold.wallShards = CastlePrefab.roomMaterialDefinitions[0].wallShards;
            m_PhobosGold.bigWallShards = new WeightedGameObjectCollection() { elements = new List<WeightedGameObject>(0) };
            m_PhobosGold.bigWallShardDamageThreshold = 10;
            m_PhobosGold.fallbackVerticalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            m_PhobosGold.fallbackHorizontalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            m_PhobosGold.pitfallVFXPrefab = SewersPrefab.roomMaterialDefinitions[0].pitfallVFXPrefab;
            m_PhobosGold.UsePitAmbientVFX = false;
            m_PhobosGold.AmbientPitVFX = new List<GameObject>(0);
            m_PhobosGold.PitVFXMinCooldown = 5;
            m_PhobosGold.PitVFXMaxCooldown = 30;
            m_PhobosGold.ChanceToSpawnPitVFXOnCooldown = 1;
            m_PhobosGold.stampFailChance = 0.2f;
            m_PhobosGold.overrideTableTable = null;
            m_PhobosGold.supportsPits = true;
            m_PhobosGold.doPitAO = true;
            m_PhobosGold.pitsAreOneDeep = false;
            m_PhobosGold.supportsDiagonalWalls = false;
            m_PhobosGold.supportsUpholstery = false;
            m_PhobosGold.carpetIsMainFloor = false;
            m_PhobosGold.carpetGrids = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Gold/carpetGrid") };
            m_PhobosGold.supportsChannels = false;
            m_PhobosGold.minChannelPools = 0;
            m_PhobosGold.maxChannelPools = 3;
            m_PhobosGold.channelTenacity = 0.75f;
            m_PhobosGold.channelGrids = new TileIndexGrid[0];
            m_PhobosGold.supportsLavaOrLavalikeSquares = false;
            m_PhobosGold.lavaGrids = new TileIndexGrid[0];
            m_PhobosGold.supportsIceSquares = false;
            m_PhobosGold.iceGrids = new TileIndexGrid[0];
            m_PhobosGold.roomFloorBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Gold/roomFloorBorderGrid");
            m_PhobosGold.roomCeilingBorderGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Gold/roomCeilingBorderGrid");
            m_PhobosGold.pitLayoutGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Gold/pitLayoutGrid");
            m_PhobosGold.pitBorderFlatGrid = ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Gold/pitBorderFlatGrid");
            m_PhobosGold.pitBorderRaisedGrid = null;
            m_PhobosGold.additionalPitBorderFlatGrid = null;
            m_PhobosGold.outerCeilingBorderGrid = null;
            m_PhobosGold.floorSquareDensity = 0.05f;
            m_PhobosGold.floorSquares = new TileIndexGrid[] { ExpandUtility.DeserializeTileIndexGridFromAssetBundle(expandSharedAuto1, "PhobosAssets/Phobos_Gold/floorSquares") };
            m_PhobosGold.usesFacewallGrids = false;
            m_PhobosGold.facewallGrids = CatacombsPrefab.roomMaterialDefinitions[0].facewallGrids;
            m_PhobosGold.usesInternalMaterialTransitions = false;
            m_PhobosGold.usesProceduralMaterialTransitions = false;
            m_PhobosGold.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            m_PhobosGold.secretRoomWallShardCollections = new List<GameObject>(0);
            m_PhobosGold.overrideStoneFloorType = false;
            m_PhobosGold.overrideFloorType = CellVisualData.CellFloorType.Stone;
            m_PhobosGold.useLighting = true;
            m_PhobosGold.lightPrefabs = new WeightedGameObjectCollection() {
                elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = ExpandPrefabs.PhobosLight,
                       weight = 1,
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]
                   }
               }
            };
            m_PhobosGold.facewallLightStamps = dungeon.roomMaterialDefinitions[0].facewallLightStamps;
            m_PhobosGold.sidewallLightStamps = dungeon.roomMaterialDefinitions[0].sidewallLightStamps;
            m_PhobosGold.usesDecalLayer = false;
            m_PhobosGold.decalIndexGrid = null;
            m_PhobosGold.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            m_PhobosGold.decalSize = 1;
            m_PhobosGold.decalSpacing = 1;
            m_PhobosGold.usesPatternLayer = false;
            m_PhobosGold.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            m_PhobosGold.patternSpacing = 1;
            m_PhobosGold.patternSize = 1;
            m_PhobosGold.patternIndexGrid = null;
            m_PhobosGold.forceEdgesDiagonal = false;
            m_PhobosGold.exteriorFacadeBorderGrid = null;
            m_PhobosGold.facadeTopGrid = null;
            m_PhobosGold.bridgeGrid = null;

            /*DungeonMaterial m_PhobosStainedGlassLights = ScriptableObject.CreateInstance<DungeonMaterial>();
            m_PhobosStainedGlassLights.name = "Boss_Cathedral_StainedGlass_Lights";
            m_PhobosStainedGlassLights.wallShards = CastlePrefab.roomMaterialDefinitions[0].wallShards;
            m_PhobosStainedGlassLights.bigWallShards = new WeightedGameObjectCollection() { elements = new List<WeightedGameObject>(0) };
            m_PhobosStainedGlassLights.bigWallShardDamageThreshold = 10;
            m_PhobosStainedGlassLights.fallbackVerticalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackVerticalTileMapEffects;
            m_PhobosStainedGlassLights.fallbackHorizontalTileMapEffects = dungeon.roomMaterialDefinitions[0].fallbackHorizontalTileMapEffects;
            m_PhobosStainedGlassLights.pitfallVFXPrefab = null;
            m_PhobosStainedGlassLights.UsePitAmbientVFX = false;
            m_PhobosStainedGlassLights.AmbientPitVFX = new List<GameObject>(0);
            m_PhobosStainedGlassLights.PitVFXMinCooldown = 5;
            m_PhobosStainedGlassLights.PitVFXMaxCooldown = 30;
            m_PhobosStainedGlassLights.ChanceToSpawnPitVFXOnCooldown = 1;
            m_PhobosStainedGlassLights.stampFailChance = 0.2f;
            m_PhobosStainedGlassLights.overrideTableTable = null;
            m_PhobosStainedGlassLights.supportsPits = true;
            m_PhobosStainedGlassLights.doPitAO = true;
            m_PhobosStainedGlassLights.pitsAreOneDeep = false;
            m_PhobosStainedGlassLights.supportsDiagonalWalls = false;
            m_PhobosStainedGlassLights.supportsUpholstery = false;
            m_PhobosStainedGlassLights.carpetIsMainFloor = false;
            m_PhobosStainedGlassLights.carpetGrids = new TileIndexGrid[0];
            m_PhobosStainedGlassLights.supportsChannels = false;
            m_PhobosStainedGlassLights.minChannelPools = 0;
            m_PhobosStainedGlassLights.maxChannelPools = 3;
            m_PhobosStainedGlassLights.channelTenacity = 0.75f;
            m_PhobosStainedGlassLights.channelGrids = new TileIndexGrid[0];
            m_PhobosStainedGlassLights.supportsLavaOrLavalikeSquares = false;
            m_PhobosStainedGlassLights.lavaGrids = new TileIndexGrid[0];
            m_PhobosStainedGlassLights.supportsIceSquares = false;
            m_PhobosStainedGlassLights.iceGrids = new TileIndexGrid[0];
            m_PhobosStainedGlassLights.roomFloorBorderGrid = null;
            m_PhobosStainedGlassLights.roomCeilingBorderGrid = null;
            m_PhobosStainedGlassLights.pitLayoutGrid = null;
            m_PhobosStainedGlassLights.pitBorderFlatGrid = null;
            m_PhobosStainedGlassLights.pitBorderRaisedGrid = null;
            m_PhobosStainedGlassLights.additionalPitBorderFlatGrid = null;
            m_PhobosStainedGlassLights.outerCeilingBorderGrid = null;
            m_PhobosStainedGlassLights.floorSquareDensity = 0.05f;
            m_PhobosStainedGlassLights.floorSquares = new TileIndexGrid[0];
            m_PhobosStainedGlassLights.usesFacewallGrids = false;
            m_PhobosStainedGlassLights.facewallGrids = new FacewallIndexGridDefinition[0];
            m_PhobosStainedGlassLights.usesInternalMaterialTransitions = false;
            m_PhobosStainedGlassLights.usesProceduralMaterialTransitions = false;
            m_PhobosStainedGlassLights.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            m_PhobosStainedGlassLights.secretRoomWallShardCollections = new List<GameObject>(0);
            m_PhobosStainedGlassLights.overrideStoneFloorType = false;
            m_PhobosStainedGlassLights.overrideFloorType = CellVisualData.CellFloorType.Stone;
            m_PhobosStainedGlassLights.useLighting = true;
            m_PhobosStainedGlassLights.lightPrefabs = new WeightedGameObjectCollection() {
                elements = new List<WeightedGameObject>() {
                   new WeightedGameObject() {
                       rawGameObject = ExpandPrefab.PhobosLight2,
                       weight = 1,
                       forceDuplicatesPossible = false,
                       pickupId = -1,
                       additionalPrerequisites = new DungeonPrerequisite[0]
                   }
               }
            };            
            m_PhobosStainedGlassLights.facewallLightStamps = new List<LightStampData>() {
                new LightStampData() {
                    width = 1,
                    height = 2,
                    relativeWeight = 1,
                    placementRule = DungeonTileStampData.StampPlacementRule.ON_LOWER_FACEWALL,
                    occupySpace = DungeonTileStampData.StampSpace.WALL_SPACE,
                    stampCategory = DungeonTileStampData.StampCategory.MUNDANE,
                    preferredIntermediaryStamps = 0,
                    intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    requiresForcedMatchingStyle = false,
                    opulence = Opulence.FINE,
                    roomTypeData = new List<StampPerRoomPlacementSettings>(0),
                    indexOfSymmetricPartner = -1,
                    preventRoomRepeats = false,
                    objectReference = null,
                    CanBeTopWallLight = true,
                    CanBeCenterLight = true,
                    FallbackIndex = 0
                }
            };
            m_PhobosStainedGlassLights.sidewallLightStamps = new List<LightStampData>() {
                new LightStampData() {
                    width = 1,
                    height = 2,
                    relativeWeight = 1,
                    placementRule = DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS,
                    occupySpace = DungeonTileStampData.StampSpace.WALL_SPACE,
                    stampCategory = DungeonTileStampData.StampCategory.MUNDANE,
                    preferredIntermediaryStamps = 0,
                    intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    requiresForcedMatchingStyle = false,
                    opulence = Opulence.FINE,
                    roomTypeData = new List<StampPerRoomPlacementSettings>(0),
                    indexOfSymmetricPartner = -1,
                    preventRoomRepeats = false,
                    objectReference = null,
                    CanBeTopWallLight = true,
                    CanBeCenterLight = true,
                    FallbackIndex = 0
                }
            };
            m_PhobosStainedGlassLights.usesDecalLayer = false;
            m_PhobosStainedGlassLights.decalIndexGrid = null;
            m_PhobosStainedGlassLights.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            m_PhobosStainedGlassLights.decalSize = 1;
            m_PhobosStainedGlassLights.decalSpacing = 1;
            m_PhobosStainedGlassLights.usesPatternLayer = false;
            m_PhobosStainedGlassLights.patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE;
            m_PhobosStainedGlassLights.patternSpacing = 1;
            m_PhobosStainedGlassLights.patternSize = 1;
            m_PhobosStainedGlassLights.patternIndexGrid = null;
            m_PhobosStainedGlassLights.forceEdgesDiagonal = false;
            m_PhobosStainedGlassLights.exteriorFacadeBorderGrid = null;
            m_PhobosStainedGlassLights.facadeTopGrid = null;
            m_PhobosStainedGlassLights.bridgeGrid = null;*/

            


            dungeon.gameObject.name = "Base_Phobos";
            dungeon.DungeonSeed = 0;
            dungeon.DungeonShortName = "Phobos";
            dungeon.DungeonFloorName = "Phobos";
            dungeon.DungeonFloorLevelTextOverride = "An unknown place.";
            dungeon.debugSettings = new DebugDungeonSettings() {
                RAPID_DEBUG_DUNGEON_ITERATION_SEEKER = false,
                RAPID_DEBUG_DUNGEON_ITERATION = false,
                RAPID_DEBUG_DUNGEON_COUNT = 50,
                GENERATION_VIEWER_MODE = false,
                FULL_MINIMAP_VISIBILITY = false,
                COOP_TEST = false,
                DISABLE_ENEMIES = false,
                DISABLE_LOOPS = true,
                DISABLE_SECRET_ROOM_COVERS = false,
                DISABLE_OUTLINES = false,
                WALLS_ARE_PITS = false,
            };
            dungeon.PatternSettings = new SemioticDungeonGenSettings() {
                flows = new List<DungeonFlow>() { FlowDatabase.GetOrLoadByName("Complex_Flow_Test") },
                mandatoryExtraRooms = new List<ExtraIncludedRoomData>(0),
                optionalExtraRooms = new List<ExtraIncludedRoomData>(0),
                MAX_GENERATION_ATTEMPTS = 250,
                DEBUG_RENDER_CANVASES_SEPARATELY = false,
            };
            dungeon.ForceRegenerationOfCharacters = false;
            dungeon.ActuallyGenerateTilemap = true;
            dungeon.decoSettings = new TilemapDecoSettings() {
                standardRoomVisualSubtypes = new WeightedIntCollection() {
                    elements = new WeightedInt[] {
                      new WeightedInt() {
                          annotation = "blue plate",
                          value = 0,
                          weight = 0.3f,
                          additionalPrerequisites = new DungeonPrerequisite[0]
                      },
                      new WeightedInt() {
                          annotation = "grey plate",
                          value = 1,
                          weight = 0.4f,
                          additionalPrerequisites = new DungeonPrerequisite[0]
                      },
                      new WeightedInt() {
                          annotation = "shop",
                          value = 2,
                          weight = 0,
                          additionalPrerequisites = new DungeonPrerequisite[0]
                      },
                      new WeightedInt() {
                          annotation = "gold plate",
                          value = 3,
                          weight = 0.3f,
                          additionalPrerequisites = new DungeonPrerequisite[0]
                      },
                  }
                },
                decalLayerStyle = TilemapDecoSettings.DecoStyle.NONE,
                decalSize = 3,
                decalSpacing = 1,
                decalExpansion = 0,
                patternLayerStyle = TilemapDecoSettings.DecoStyle.NONE,
                patternSize = 3,
                patternSpacing = 3,
                patternExpansion = 0,
                decoPatchFrequency = 0.01f,
                ambientLightColor = new Color(0.875462f, 0.882353f, 0.810986f, 1),
                ambientLightColorTwo = new Color(0.87451f, 0.882353f, 0.811765f, 1),
                lowQualityAmbientLightColor = Color.white,
                lowQualityAmbientLightColorTwo = Color.white,
                lowQualityCheapLightVector = new Vector4(1, 0, -1, 0),
                UsesAlienFXFloorColor = false,
                AlienFXFloorColor = Color.black,
                generateLights = true,
                lightCullingPercentage = 0.2f,
                lightOverlapRadius = 8,
                nearestAllowedLight = 12,
                minLightExpanseWidth = 2,
                lightHeight = -2,
                lightCookies = new Texture2D[0],
                debug_view = false
            };
            dungeon.tileIndices = new TileIndices() {
                tilesetId = GlobalDungeonData.ValidTilesets.PHOBOSGEON,
                dungeonCollection = ExpandPrefabs.ENV_Tileset_Phobos.GetComponent<tk2dSpriteCollectionData>(),
                dungeonCollectionSupportsDiagonalWalls = false,
                aoTileIndices = new AOTileIndices() {
                    AOFloorTileIndex = 0,
                    AOBottomWallBaseTileIndex = 1,
                    AOBottomWallTileRightIndex = 2,
                    AOBottomWallTileLeftIndex = 3,
                    AOBottomWallTileBothIndex = 4,
                    AOTopFacewallRightIndex = 6,
                    AOTopFacewallLeftIndex = 5,
                    AOTopFacewallBothIndex = 7,
                    AOFloorWallLeft = 5,
                    AOFloorWallRight = 6,
                    AOFloorWallBoth = 7,
                    AOFloorPizzaSliceLeft = 8,
                    AOFloorPizzaSliceRight = 9,
                    AOFloorPizzaSliceBoth = 10,
                    AOFloorPizzaSliceLeftWallRight = 11,
                    AOFloorPizzaSliceRightWallLeft = 12,
                    AOFloorWallUpAndLeft = 13,
                    AOFloorWallUpAndRight = 14,
                    AOFloorWallUpAndBoth = 15,
                    AOFloorDiagonalWallNortheast = -1,
                    AOFloorDiagonalWallNortheastLower = -1,
                    AOFloorDiagonalWallNortheastLowerJoint = -1,
                    AOFloorDiagonalWallNorthwest = -1,
                    AOFloorDiagonalWallNorthwestLower = -1,
                    AOFloorDiagonalWallNorthwestLowerJoint = -1,
                    AOBottomWallDiagonalNortheast = -1,
                    AOBottomWallDiagonalNorthwest = -1,
                },
                placeBorders = true,
                placePits = false,
                chestHighWallIndices = new List<TileIndexVariant>() {
                    new TileIndexVariant() {
                        index = 41,
                        likelihood = 0.5f,
                        overrideLayerIndex = 0,
                        overrideIndex = 0
                    }
                },
                decalIndexGrid = null,
                patternIndexGrid = null,
                globalSecondBorderTiles = new List<int>(0),
                edgeDecorationTiles = null,
            };
            dungeon.roomMaterialDefinitions = new DungeonMaterial[] {
                m_PhobosBlue,
                m_PhobosGrey,
                m_PhobosBlue,
                m_PhobosGold,
                m_PhobosBlue,
                sharedAssets2.LoadAsset<DungeonMaterial>("Boss_Cathedral_StainedGlass_Lights"),
                m_PhobosBlue,
            };
            dungeon.dungeonWingDefinitions = new DungeonWingDefinition[0];
            dungeon.pathGridDefinitions = new List<TileIndexGrid>(0);
            // dungeon.dungeonDustups
            dungeon.damageTypeEffectMatrix = new DamageTypeEffectMatrix() {
                definitions = new List<DamageTypeEffectDefinition>() {
                    new DamageTypeEffectDefinition() {
                        name = "Fire",
                        damageType = CoreDamageTypes.Magic,
                        wallDecals = new VFXPool() { type = VFXPoolType.None, effects = new VFXComplex[0] },
                    },
                    new DamageTypeEffectDefinition() {
                        name = "Ice",
                        damageType = CoreDamageTypes.Fire,
                        wallDecals = new VFXPool() { type = VFXPoolType.None, effects = new VFXComplex[0] },
                    },
                    new DamageTypeEffectDefinition() {
                        name = "Rubel",
                        damageType = CoreDamageTypes.Ice,
                        wallDecals = new VFXPool() { type = VFXPoolType.None, effects = new VFXComplex[0] },
                    },
                    new DamageTypeEffectDefinition() {
                        name = "Water",
                        damageType = CoreDamageTypes.Poison,
                        wallDecals = new VFXPool() { type = VFXPoolType.None, effects = new VFXComplex[0] },
                    },
                }
            };
            
            dungeon.stampData = ScriptableObject.CreateInstance<DungeonTileStampData>();
            dungeon.stampData.name = "ENV_PHOBOS_STAMP_DATA";
            dungeon.stampData.tileStampWeight = 1;
            dungeon.stampData.spriteStampWeight = 0;
            dungeon.stampData.objectStampWeight = 1;
            dungeon.stampData.stamps = new TileStampData[0];
            dungeon.stampData.spriteStamps = new SpriteStampData[0];
            dungeon.stampData.objectStamps = SewersPrefab.stampData.objectStamps; // Original prefab had no object stamps setup. Using Sewers for now.
            // dungeon.stampData.SymmetricFrameChance = 0.25f;
            // dungeon.stampData.SymmetricCompleteChance = 0.5f;
            // Using settings from Sewers for now.
            dungeon.stampData.SymmetricFrameChance = 0.5f;
            dungeon.stampData.SymmetricCompleteChance = 0.25f;
            dungeon.UsesCustomFloorIdea = false;
            dungeon.FloorIdea = new RobotDaveIdea() {
                ValidEasyEnemyPlaceables = new DungeonPlaceable[0],
                ValidHardEnemyPlaceables = new DungeonPlaceable[0],
                UseWallSawblades = false,
                UseRollingLogsVertical = false,
                UseRollingLogsHorizontal = false,
                UseFloorPitTraps = false,
                UseFloorFlameTraps = false,
                UseFloorSpikeTraps = false,
                UseFloorConveyorBelts = false,
                UseCaveIns = false,
                UseAlarmMushrooms = false,
                UseMineCarts = false,
                UseChandeliers = false,
                CanIncludePits = true
            };
            dungeon.doorObjects = NakatomiPrefab.alternateDoorObjectsNakatomi;
            dungeon.lockedDoorObjects = null;
            // Use Gungeon's existing oneWayDoor/pressure plate/phantomBlockers
            // dungeon.oneWayDoorObjects 
            // dungeon.oneWayDoorPressurePlate
            // dungeon.phantomBlockerDoorObjects
            dungeon.WarpWingDoorPrefab = null;
            // dungeon.baseChestContents
            dungeon.SecretRoomSimpleTriggersFacewall = SewersPrefab.SecretRoomSimpleTriggersFacewall;
            dungeon.SecretRoomSimpleTriggersSidewall = SewersPrefab.SecretRoomSimpleTriggersSidewall;
            dungeon.SecretRoomComplexTriggers = new List<ComplexSecretRoomTrigger>(0);
            // dungeon.SecretRoomDoorSparkVFX
            // dungeon.SecretRoomHorizontalPoofVFX
            // dungeon.SecretRoomVerticalPoofVFX
            // dungeon.sharedSettingsPrefab
            dungeon.StripPlayerOnArrival = false;
            dungeon.SuppressEmergencyCrates = false;
            dungeon.SetTutorialFlag = false;
            dungeon.PlayerIsLight = true;
            dungeon.PlayerLightColor = new Color(0.985294f, 0.719884f, 0.383975f, 1);
            dungeon.PlayerLightIntensity = 3.8f;
            dungeon.PlayerLightRadius = 4.5f;
            dungeon.musicEventName = "Play_MUS_sewer_theme_01";

            sharedAssets2 = null;
            expandSharedAuto1 = null;
            NakatomiPrefab = null;
            SewersPrefab = null;
            CastlePrefab = null;
            CatacombsPrefab = null;

            return dungeon;
        }
    }
}


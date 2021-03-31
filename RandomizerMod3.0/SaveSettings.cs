using System;
using System.Linq;
using System.Collections.Generic;
using Modding;
using RandomizerMod.Actions;
using SereCore;
using RandomizerMod.Randomization;
using static RandomizerMod.LogHelper;
using static RandomizerMod.Randomization.Randomizer;

namespace RandomizerMod
{
    public class SaveSettings : BaseSettings
    {
        /*
         * UNLISTED BOOLS
         * rescuedSly is used in room randomizer to control when Sly appears in the shop, separately from when the door is unlocked
         */


        private SerializableStringDictionary _itemPlacements = new SerializableStringDictionary();
        private SerializableIntDictionary _orderedLocations = new SerializableIntDictionary();
        public SerializableStringDictionary _transitionPlacements = new SerializableStringDictionary();
        private SerializableIntDictionary _variableCosts = new SerializableIntDictionary();
        private SerializableIntDictionary _shopCosts = new SerializableIntDictionary();
        private SerializableIntDictionary _additiveCounts = new SerializableIntDictionary();

        private SerializableBoolDictionary _obtainedItems = new SerializableBoolDictionary();
        private SerializableBoolDictionary _obtainedLocations = new SerializableBoolDictionary();
        private SerializableBoolDictionary _obtainedTransitions = new SerializableBoolDictionary();

        /// <remarks>item, location</remarks>
        public (string, string)[] ItemPlacements => _itemPlacements.Select(pair => (pair.Key, pair.Value)).ToArray();

        public int MaxOrder => _orderedLocations.Count;

        public (string, int)[] VariableCosts => _variableCosts.Select(pair => (pair.Key, pair.Value)).ToArray();
        public (string, int)[] ShopCosts => _shopCosts.Select(pair => (pair.Key, pair.Value)).ToArray();

        public bool RandomizeTransitions => RandomizeAreas || RandomizeRooms;

        public bool FreeLantern => !(DarkRooms || RandomizeKeys);
        public SaveSettings()
        {
            AfterDeserialize += () =>
            {
                RandomizerAction.CreateActions(ItemPlacements, this);
            };
        }

        public int JijiHintCounter
        {
            get => GetInt(0);
            set => SetInt(value);
        }
        public int QuirrerHintCounter
        {
            get => GetInt(0);
            set => SetInt(value);
        }

        public bool AllBosses
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool AllSkills
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool AllCharms
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool CharmNotch
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool Grubfather
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool Jiji
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool Quirrel
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool ItemDepthHints
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool EarlyGeo
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool LeverSkips
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool NPCItemDialogue
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool ExtraPlatforms
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool Randomizer
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeAreas
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeRooms
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool ConnectAreas
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool SlyCharm
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeDreamers
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeSkills
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeCharms
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeKeys
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeGeoChests
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeMaskShards
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeVesselFragments
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeCharmNotches
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizePaleOre
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeRancidEggs
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool RandomizeRelics
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeMaps
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeStags
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeGrubs
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeWhisperingRoots
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        
        public bool RandomizeRocks
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeBossGeo
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        
        public bool RandomizeSoulTotems
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        
        public bool RandomizePalaceTotems
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        
        public bool RandomizeLoreTablets
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeLifebloodCocoons
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeGrimmkinFlames
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeBossEssence
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool DuplicateMajorItems
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeClawPieces
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool CursedNail
        {
            get => GetBool(false);
            set => SetBool(value);
        }
        public bool CursedPogo
        {
            get => GetBool(false);
            set => SetBool(value);
        }


        internal bool GetRandomizeByPool(string pool)
        {
            switch (pool)
            {
                case "Dreamer":
                    return RandomizeDreamers;
                case "Skill":
                    return RandomizeSkills;
                case "SplitClaw":
                    return RandomizeClawPieces;
                case "Charm":
                    return RandomizeCharms;
                case "Key":
                    return RandomizeKeys;
                case "Mask":
                    return RandomizeMaskShards;
                case "Vessel":
                    return RandomizeVesselFragments;
                case "Ore":
                    return RandomizePaleOre;
                case "Notch":
                    return RandomizeCharmNotches;
                case "Geo":
                    return RandomizeGeoChests;
                case "Egg":
                    return RandomizeRancidEggs;
                case "Relic":
                    return RandomizeRelics;
                case "Map":
                    return RandomizeMaps;
                case "Stag":
                    return RandomizeStags;
                case "Grub":
                    return RandomizeGrubs;
                case "Root":
                    return RandomizeWhisperingRoots;
                case "Rock":
                    return RandomizeRocks;
                case "Soul":
                    return RandomizeSoulTotems;
                case "PalaceSoul":
                    return RandomizePalaceTotems;
                case "Lore":
                    return RandomizeLoreTablets;
                case "Lifeblood":
                    return RandomizeLifebloodCocoons;
                case "Flame":
                    return RandomizeGrimmkinFlames;
                case "Essence_Boss":
                    return RandomizeBossEssence;
                case "Boss_Geo":
                    return RandomizeBossGeo;
                case "CursedNail":
                    return CursedNail;
                case "CursedPogo":
                    return CursedPogo;
                default:
                    return false;
            }
        }


        public bool CreateSpoilerLog
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool Cursed
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeFocus
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeStartItems
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool RandomizeStartLocation
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        // The following settings names are referenced in Benchwarp. Please do not change!
        public string StartName
        {
            get => GetString("King's Pass");
            set => SetString(value);
        }

        public string StartSceneName
        {
            get => GetString("Tutorial_01");
            set => SetString(value);
        }

        public string StartRespawnMarkerName
        {
            get => GetString("Randomizer Respawn Marker");
            set => SetString(value);
        }

        public int StartRespawnType
        {
            get => GetInt(0);
            set => SetInt(value);
        }

        public int StartMapZone
        {
            get => GetInt((int)GlobalEnums.MapZone.KINGS_PASS);
            set => SetInt(value);
        }
        // End Benchwarp block.

        public bool ShadeSkips
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool AcidSkips
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool SpikeTunnels
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool MildSkips
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool SpicySkips
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool FireballSkips
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool DarkRooms
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public int Seed
        {
            get => GetInt(-1);
            set => SetInt(value);
        }

        public void ResetPlacements()
        {
            _itemPlacements = new SerializableStringDictionary();
            _orderedLocations = new SerializableIntDictionary();
            _transitionPlacements = new SerializableStringDictionary();
            _variableCosts = new SerializableIntDictionary();
            _shopCosts = new SerializableIntDictionary();
            _additiveCounts = new SerializableIntDictionary();

            _obtainedItems = new SerializableBoolDictionary();
            _obtainedLocations = new SerializableBoolDictionary();
            _obtainedTransitions = new SerializableBoolDictionary();
        }

        public void AddItemPlacement(string item, string location)
        {
            _itemPlacements[item] = location;
        }

        public void AddOrderedLocation(string location, int order)
        {
            _orderedLocations[location] = order;
        }

        public int GetLocationOrder(string location)
        {
            return _orderedLocations[location];
        }

        public string GetNthLocation(int n)
        {
            return _orderedLocations.FirstOrDefault(kvp => kvp.Value == n).Key;
        }

        public string[] GetNthLocationItems(int n)
        {
            string location = GetNthLocation(n);
            return ItemPlacements.Where(pair => pair.Item2 == location).Select(pair => pair.Item1).ToArray();
        }

        public string GetItemPlacedAt(string location)
        {
            foreach (var ilp in _itemPlacements)
            {
                if (ilp.Value == location)
                {
                    return ilp.Key;
                }
            }
            return "";
        }
        
        public void AddTransitionPlacement(string entrance, string exit)
        {
            _transitionPlacements[entrance] = exit;
        }

        public void AddNewCost(string item, int cost)
        {
            _variableCosts[item] = cost;
        }

        public void AddShopCost(string item, int cost)
        {
            _shopCosts[item] = cost;
        }

        public int GetShopCost(string item)
        {
            return _shopCosts[item];
        }


        public void MarkItemFound(string item)
        {
            _obtainedItems[item] = true;
        }

        public bool CheckItemFound(string item)
        {
            if (!_obtainedItems.TryGetValue(item, out bool found)) return false;
            return found;
        }

        public string[] GetItemsFound()
        {
            return _obtainedItems.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
        }

        public int GetNumLocations()
        {
            return _orderedLocations.Count + _shopCosts.Count - 5;
        }

        public HashSet<string> GetPlacedItems()
        {
            return new HashSet<string>(ItemPlacements.Select(pair => pair.Item1));
        }

        public void MarkLocationFound(string location)
        {
            _obtainedLocations[location] = true;
        }

        public bool CheckLocationFound(string location)
        {
            if (!_obtainedLocations.TryGetValue(location, out bool found)) return false;
            return found;
        }

        public string[] GetLocationsFound()
        {
            return _obtainedLocations.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
        }

        public void MarkTransitionFound(string transition)
        {
            _obtainedTransitions[transition] = true;
        }

        public bool CheckTransitionFound(string transition)
        {
            if (!_obtainedTransitions.TryGetValue(transition, out bool found)) return false;
            return found;
        }

        public string[] GetTransitionsFound()
        {
            return _obtainedTransitions.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
        }

        // Returns the actual item that will be obtained by picking up the given item; these may differ
        // if the pickup is part of an additive group.
        public string GetEffectiveItem(string item)
        {
            var additiveSet = LogicManager.AdditiveItemSets.FirstOrDefault(set => set.Contains(item));
            if (additiveSet != null)
            {
                var count = Math.Min(GetAdditiveCount(item), additiveSet.Length - 1);
                item = additiveSet[count];
            }
            return item;
        }

        public int GetAdditiveCount(string item)
        {
            string[] additiveSet = LogicManager.AdditiveItemSets.FirstOrDefault(set => set.Contains(item));
            if (additiveSet is null) return 0;
            if (!_additiveCounts.TryGetValue(additiveSet[0], out int count))
            {
                _additiveCounts.Add(additiveSet[0], 0);
                count = 0;
            }
            return count;
        }

        public void IncrementAdditiveCount(string item)
        {
            string[] additiveSet = LogicManager.AdditiveItemSets.FirstOrDefault(set => set.Contains(item));
            if (additiveSet is null) return;
            if (!_additiveCounts.ContainsKey(additiveSet[0]))
            {
                _additiveCounts.Add(additiveSet[0], 0);
            }
            _additiveCounts[additiveSet[0]]++;
        }
    }


    public class GlobalSettings : BaseSettings
    {
        public bool NPCItemDialogue
        {
            get => GetBool(true);
            set => SetBool(value);
        }
    }
}

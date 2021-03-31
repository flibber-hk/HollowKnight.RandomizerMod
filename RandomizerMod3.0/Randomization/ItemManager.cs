﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using static RandomizerMod.LogHelper;
using System.Security.Policy;

namespace RandomizerMod.Randomization
{
    public class ItemManager
    {
        internal ProgressionManager pm;
        private static VanillaManager vm { get { return VanillaManager.Instance; } }

        public static Dictionary<string, string> nonShopItems;
        public static Dictionary<string, List<string>> shopItems;
        public static Dictionary<string, int> locationOrder; // the order in which a location was first removed from the pool (filled with progression, moved to standby, first shop item, etc). The order of an item will be the order of its location.
        public static List<string> duplicatedItems;

        public static HashSet<string> recentProgression;

        private List<string> unplacedLocations;
        private List<string> unplacedItems;
        private List<string> unplacedProgression;
        private List<string> standbyLocations;
        private List<string> standbyItems;
        private List<string> standbyProgression;

        private Queue<bool> progressionFlag;
        internal Queue<string> updateQueue;
        private bool delinearizeShops; // if there are 12 or fewer shop items, we do not add shops back into randomization after they've been filled once, until the end of second pass
        public bool normalFillShops; // if there are fewer than 5 shop items, we do not include shops in randomization at all, until the end of second pass

        private HashSet<string> reachableLocations;
        public HashSet<string> randomizedLocations;
        public HashSet<string> randomizedItems;
        public HashSet<string> allLocations;

        public int availableCount => reachableLocations.Intersect(unplacedLocations).Count();

        public bool anyLocations => unplacedLocations.Any();
        public bool anyItems => unplacedItems.Any();
        public bool canGuess => unplacedProgression.Any(i => LogicManager.GetItemDef(i).itemCandidate);
        internal ItemManager(Random rnd)
        {
            // takes approximately .004s to construct

            pm = new ProgressionManager(
                RandomizerState.InProgress
                ); ;

            nonShopItems = new Dictionary<string, string>();
            shopItems = new Dictionary<string, List<string>>();
            locationOrder = new Dictionary<string, int>();

            unplacedLocations = new List<string>();
            unplacedItems = new List<string>();
            unplacedProgression = new List<string>();
            standbyLocations = new List<string>();
            standbyItems = new List<string>();
            standbyProgression = new List<string>();
            recentProgression = new HashSet<string>();

            progressionFlag = new Queue<bool>();
            updateQueue = new Queue<string>();

            foreach (string shopName in LogicManager.ShopNames)
            {
                shopItems.Add(shopName, new List<string>());
            }

            randomizedItems = GetRandomizedItems();
            randomizedLocations = GetRandomizedLocations();
            List<string> items = randomizedItems.ToList();
            List<string> locations = randomizedLocations.ToList();
            randomizedLocations = new HashSet<string>(locations);
            allLocations = new HashSet<string>(LogicManager.ItemNames);
            allLocations.UnionWith(LogicManager.ShopNames);

            while (locations.Any())
            {
                string l = locations[rnd.Next(locations.Count)];
                unplacedLocations.Add(l);
                locations.Remove(l);
            }

            while (items.Any())
            {
                string i = items[rnd.Next(items.Count)];

                if (RandomizerMod.Instance.Settings.Cursed)
                {
                    if (LogicManager.GetItemDef(i).majorItem) i = items[rnd.Next(items.Count)];
                }

                if (!LogicManager.GetItemDef(i).progression)
                {
                    unplacedItems.Add(i);
                    progressionFlag.Enqueue(false);
                }
                else
                {
                    unplacedProgression.Add(i);
                    progressionFlag.Enqueue(true);
                }
                items.Remove(i);
            }

            reachableLocations = new HashSet<string>();
            vm.Setup(this);

            foreach (string item in Randomizer.startItems) unplacedItems.Remove(item);
            foreach (string item in Randomizer.startProgression)
            {
                unplacedProgression.Remove(item);
                pm.Add(item);
                UpdateReachableLocations(item);
            }

            int shopItemCount = unplacedItems.Count + unplacedProgression.Count - unplacedLocations.Count + 5;
            normalFillShops = shopItemCount >= 5;
            delinearizeShops = shopItemCount > 12;
            if (!normalFillShops)
            {
                LogWarn("Entering randomization with insufficient items to fill all shops.");
                foreach (string s in LogicManager.ShopNames) unplacedLocations.Remove(s);
            }
        }

        private static HashSet<string> GetRandomizedItems() // not suitable outside randomizer, because it can't compute duplicate items
        {
            HashSet<string> items = new HashSet<string>();

            if (RandomizerMod.Instance.Settings.RandomizeDreamers) items.UnionWith(LogicManager.GetItemsByPool("Dreamer"));
            if (RandomizerMod.Instance.Settings.RandomizeSkills) items.UnionWith(LogicManager.GetItemsByPool("Skill"));
            if (RandomizerMod.Instance.Settings.RandomizeCharms) items.UnionWith(LogicManager.GetItemsByPool("Charm"));
            if (RandomizerMod.Instance.Settings.RandomizeKeys) items.UnionWith(LogicManager.GetItemsByPool("Key"));
            if (RandomizerMod.Instance.Settings.RandomizeMaskShards) items.UnionWith(LogicManager.GetItemsByPool("Mask"));
            if (RandomizerMod.Instance.Settings.RandomizeVesselFragments) items.UnionWith(LogicManager.GetItemsByPool("Vessel"));
            if (RandomizerMod.Instance.Settings.RandomizePaleOre) items.UnionWith(LogicManager.GetItemsByPool("Ore"));
            if (RandomizerMod.Instance.Settings.RandomizeCharmNotches) items.UnionWith(LogicManager.GetItemsByPool("Notch"));
            if (RandomizerMod.Instance.Settings.RandomizeGeoChests) items.UnionWith(LogicManager.GetItemsByPool("Geo"));
            if (RandomizerMod.Instance.Settings.RandomizeRancidEggs) items.UnionWith(LogicManager.GetItemsByPool("Egg"));
            if (RandomizerMod.Instance.Settings.RandomizeRelics) items.UnionWith(LogicManager.GetItemsByPool("Relic"));
            if (RandomizerMod.Instance.Settings.RandomizeMaps) items.UnionWith(LogicManager.GetItemsByPool("Map"));
            if (RandomizerMod.Instance.Settings.RandomizeStags) items.UnionWith(LogicManager.GetItemsByPool("Stag"));
            if (RandomizerMod.Instance.Settings.RandomizeGrubs) items.UnionWith(LogicManager.GetItemsByPool("Grub"));
            if (RandomizerMod.Instance.Settings.RandomizeWhisperingRoots) items.UnionWith(LogicManager.GetItemsByPool("Root"));
            if (RandomizerMod.Instance.Settings.RandomizeRocks) items.UnionWith(LogicManager.GetItemsByPool("Rock"));
            if (RandomizerMod.Instance.Settings.RandomizeSoulTotems) items.UnionWith(LogicManager.GetItemsByPool("Soul"));
            if (RandomizerMod.Instance.Settings.RandomizePalaceTotems) items.UnionWith(LogicManager.GetItemsByPool("PalaceSoul"));
            if (RandomizerMod.Instance.Settings.RandomizeLoreTablets) items.UnionWith(LogicManager.GetItemsByPool("Lore"));
            if (RandomizerMod.Instance.Settings.RandomizeLifebloodCocoons) items.UnionWith(LogicManager.GetItemsByPool("Cocoon"));
            if (RandomizerMod.Instance.Settings.RandomizeGrimmkinFlames) items.UnionWith(LogicManager.GetItemsByPool("Flame"));
            if (RandomizerMod.Instance.Settings.RandomizeBossEssence) items.UnionWith(LogicManager.GetItemsByPool("Essence_Boss"));
            if (RandomizerMod.Instance.Settings.RandomizeBossGeo) items.UnionWith(LogicManager.GetItemsByPool("Boss_Geo"));
            if (RandomizerMod.Instance.Settings.RandomizeFocus) items.UnionWith(LogicManager.GetItemsByPool("Cursed"));
            if (RandomizerMod.Instance.Settings.CursedNail) items.UnionWith(LogicManager.GetItemsByPool("CursedNail"));
            if (RandomizerMod.Instance.Settings.CursedPogo) items.UnionWith(LogicManager.GetItemsByPool("CursedPogo"));

            if (RandomizerMod.Instance.Settings.RandomizeClawPieces && RandomizerMod.Instance.Settings.RandomizeSkills)
            {
                items.UnionWith(LogicManager.GetItemsByPool("SplitClaw"));
                items.Remove("Mantis_Claw");
            }

            if (RandomizerMod.Instance.Settings.Cursed)
            {
                items.Remove("Shade_Soul");
                items.Remove("Descending_Dark");
                items.Remove("Abyss_Shriek");

                int i = 0;

                List<string> iterate = items.ToList();
                foreach (string item in iterate)
                {
                    switch (LogicManager.GetItemDef(item).pool)
                    {
                        case "Mask":
                        case "Vessel":
                        case "Ore":
                        case "Notch":
                        case "Geo":
                        case "Egg":
                        case "Relic":
                        case "Rock":
                        case "Soul":
                        case "PalaceSoul":
                        case "Lore":
                            items.Remove(item);
                            items.Add("1_Geo_(" + i + ")");
                            i++;
                            break;
                    }
                }

            }

            if (RandomizerMod.Instance.Settings.DuplicateMajorItems)
            {
                duplicatedItems = new List<string>();
                foreach (string majorItem in LogicManager.ItemNames.Where(_item => LogicManager.GetItemDef(_item).majorItem).ToList())
                {
                    if (Randomizer.startItems.Contains(majorItem)) continue;
                    if (RandomizerMod.Instance.Settings.Cursed && (majorItem == "Vengeful_Spirit" || majorItem == "Desolate_Dive" || majorItem == "Howling_Wraiths")) continue;
                    if (RandomizerMod.Instance.Settings.RandomizeClawPieces && majorItem.EndsWith("Mantis_Claw")) continue;
                    if (majorItem.EndsWith("_Mantis_Claw")) continue;
                    duplicatedItems.Add(majorItem);
                }
                if (RandomizerMod.Instance.Settings.RandomizeClawPieces) duplicatedItems.Add("Dupe_Mantis_Claw");
            }

            return items;
        }

        public static HashSet<string> GetRandomizedLocations()
        {
            HashSet<string> locations = new HashSet<string>();
            if (RandomizerMod.Instance.Settings.RandomizeDreamers) locations.UnionWith(LogicManager.GetItemsByPool("Dreamer"));
            if (RandomizerMod.Instance.Settings.RandomizeSkills) locations.UnionWith(LogicManager.GetItemsByPool("Skill"));
            if (RandomizerMod.Instance.Settings.RandomizeCharms) locations.UnionWith(LogicManager.GetItemsByPool("Charm"));
            if (RandomizerMod.Instance.Settings.RandomizeKeys) locations.UnionWith(LogicManager.GetItemsByPool("Key"));
            if (RandomizerMod.Instance.Settings.RandomizeMaskShards) locations.UnionWith(LogicManager.GetItemsByPool("Mask"));
            if (RandomizerMod.Instance.Settings.RandomizeVesselFragments) locations.UnionWith(LogicManager.GetItemsByPool("Vessel"));
            if (RandomizerMod.Instance.Settings.RandomizePaleOre) locations.UnionWith(LogicManager.GetItemsByPool("Ore"));
            if (RandomizerMod.Instance.Settings.RandomizeCharmNotches) locations.UnionWith(LogicManager.GetItemsByPool("Notch"));
            if (RandomizerMod.Instance.Settings.RandomizeGeoChests) locations.UnionWith(LogicManager.GetItemsByPool("Geo"));
            if (RandomizerMod.Instance.Settings.RandomizeRancidEggs) locations.UnionWith(LogicManager.GetItemsByPool("Egg"));
            if (RandomizerMod.Instance.Settings.RandomizeRelics) locations.UnionWith(LogicManager.GetItemsByPool("Relic"));
            if (RandomizerMod.Instance.Settings.RandomizeMaps) locations.UnionWith(LogicManager.GetItemsByPool("Map"));
            if (RandomizerMod.Instance.Settings.RandomizeStags) locations.UnionWith(LogicManager.GetItemsByPool("Stag"));
            if (RandomizerMod.Instance.Settings.RandomizeGrubs) locations.UnionWith(LogicManager.GetItemsByPool("Grub"));
            if (RandomizerMod.Instance.Settings.RandomizeWhisperingRoots) locations.UnionWith(LogicManager.GetItemsByPool("Root"));
            if (RandomizerMod.Instance.Settings.RandomizeRocks) locations.UnionWith(LogicManager.GetItemsByPool("Rock"));
            if (RandomizerMod.Instance.Settings.RandomizeSoulTotems) locations.UnionWith(LogicManager.GetItemsByPool("Soul"));
            if (RandomizerMod.Instance.Settings.RandomizePalaceTotems) locations.UnionWith(LogicManager.GetItemsByPool("PalaceSoul"));
            if (RandomizerMod.Instance.Settings.RandomizeLoreTablets) locations.UnionWith(LogicManager.GetItemsByPool("Lore"));
            if (RandomizerMod.Instance.Settings.RandomizeLifebloodCocoons) locations.UnionWith(LogicManager.GetItemsByPool("Cocoon"));
            if (RandomizerMod.Instance.Settings.RandomizeGrimmkinFlames) locations.UnionWith(LogicManager.GetItemsByPool("Flame"));
            if (RandomizerMod.Instance.Settings.RandomizeBossEssence) locations.UnionWith(LogicManager.GetItemsByPool("Essence_Boss"));
            if (RandomizerMod.Instance.Settings.RandomizeBossGeo) locations.UnionWith(LogicManager.GetItemsByPool("Boss_Geo"));
            if (RandomizerMod.Instance.Settings.RandomizeFocus) locations.UnionWith(LogicManager.GetItemsByPool("Cursed"));
            
            // Adding *three* new locations to KP throws off the balance a bit. Put 3 more items in shops instead.
            // if (RandomizerMod.Instance.Settings.CursedNail) locations.UnionWith(LogicManager.GetItemsByPool("CursedNail"));

            if (RandomizerMod.Instance.Settings.RandomizeClawPieces && RandomizerMod.Instance.Settings.RandomizeSkills)
            {
                locations.UnionWith(LogicManager.GetItemsByPool("SplitClaw"));
                locations.Remove("Mantis_Claw");
            }

            locations = new HashSet<string>(locations.Where(item => LogicManager.GetItemDef(item).type != ItemType.Shop));
            locations.UnionWith(LogicManager.ShopNames);

            return locations;
        }

        public void ResetReachableLocations()
        {
            reachableLocations = new HashSet<string>(
                randomizedLocations.Union(vm.progressionLocations).Where(val => pm.CanGet(val))
            );
        }

        public void UpdateReachableLocations(string newThing = null)
        {
            if (newThing != null)
            {
                pm.Add(newThing);
                updateQueue.Enqueue(newThing);
            }

            HashSet<string> potentialLocations;
            HashSet<string> potentialTransitions = new HashSet<string>();

            while (updateQueue.Any())
            {
                potentialLocations = LogicManager.GetLocationsByProgression(recentProgression);
                if (RandomizerMod.Instance.Settings.RandomizeTransitions)
                {
                    potentialTransitions = LogicManager.GetTransitionsByProgression(recentProgression);
                }
                recentProgression = new HashSet<string>();

                string item = updateQueue.Dequeue();
                foreach (string location in potentialLocations)
                {
                    if (pm.CanGet(location))
                    {
                        reachableLocations.Add(location);
                        if (vm.progressionLocations.Contains(location)) vm.UpdateVanillaLocations(location);
                    }
                }
                
                if (RandomizerMod.Instance.Settings.RandomizeTransitions)
                {
                    if (TransitionManager.transitionPlacements.TryGetValue(item, out string transition1) && !pm.Has(transition1))
                    {
                        pm.Add(transition1);
                        updateQueue.Enqueue(transition1);
                    }
                    foreach (string transition in potentialTransitions)
                    {
                        if (!pm.Has(transition) && pm.CanGet(transition))
                        {
                            pm.Add(transition);
                            updateQueue.Enqueue(transition);
                            if (TransitionManager.transitionPlacements.TryGetValue(transition, out string transition2) && !pm.Has(transition2))
                            {
                                pm.Add(transition2);
                                updateQueue.Enqueue(transition2);
                            }
                        }
                    }
                }
            }
        }

        public string FindNextLocation(ProgressionManager _pm = null)
        {
            if (_pm != null) pm = _pm;
            return unplacedLocations.FirstOrDefault(location => pm.CanGet(location));
        }
        public string NextLocation(bool checkLogic = true)
        {
            return unplacedLocations.First(location => !checkLogic || reachableLocations.Contains(location));
        }
        public string NextItem(bool checkFlag = true)
        {
            if (checkFlag && progressionFlag.Any() && progressionFlag.Dequeue() && unplacedProgression.Any()) return unplacedProgression.First();
            if (unplacedItems.Any()) return unplacedItems.First();
            if (unplacedProgression.Any()) return unplacedProgression.First();
            if (standbyItems.Any()) return standbyItems.First();
            if (standbyProgression.Any()) return standbyProgression.First();
            throw new IndexOutOfRangeException();
        }
        public string GuessItem()
        {
            return unplacedProgression.First(item => LogicManager.GetItemDef(item).itemCandidate);
        }
        public string ForceItem()
        {
            Queue<string> progressionQueue = new Queue<string>();
            List<string> tempProgression = new List<string>();

            void UpdateTransitions()
            {
                foreach (string transition in LogicManager.GetTransitionsByProgression(pm.tempItems))
                {
                    if (!pm.Has(transition) && pm.CanGet(transition))
                    {
                        tempProgression.Add(transition);
                        progressionQueue.Enqueue(transition);
                        pm.Add(transition);
                        if (TransitionManager.transitionPlacements.TryGetValue(transition, out string transition2))
                        {
                            tempProgression.Add(transition2);
                            progressionQueue.Enqueue(transition2);
                            pm.Add(transition2);
                        }
                    }
                }
            }
            bool CheckForNewLocations()
            {
                foreach (string location in LogicManager.GetLocationsByProgression(pm.tempItems))
                {
                    if (randomizedLocations.Contains(location) && !reachableLocations.Contains(location) && pm.CanGet(location))
                    {
                        return true;
                    }
                }
                return false;
            }

            for (int i = 0; i < unplacedProgression.Count; i++)
            {
                bool found = false;
                string item = unplacedProgression[i];
                pm.AddTemp(item);
                if (CheckForNewLocations()) found = true;
                else if (RandomizerMod.Instance.Settings.RandomizeTransitions)
                {
                    UpdateTransitions();
                    while (progressionQueue.Any())
                    {
                        progressionQueue.Dequeue();
                        UpdateTransitions();
                        found = found || CheckForNewLocations();
                    }
                }
                pm.RemoveTempItems();
                if (found)
                {
                    return item;
                }
            }
            return null;
        }
        public void Delinearize(Random rand)
        {
            if (RandomizerMod.Instance.Settings.Cursed) return;

            // add back shops for rare consideration for late progression
            if (delinearizeShops && unplacedProgression.Count > 0 && rand.Next(8) == 0)
            {
                unplacedLocations.Insert(rand.Next(unplacedLocations.Count), LogicManager.ShopNames[rand.Next(LogicManager.ShopNames.Length)]);
            }

            // release junk item paired with location from standby for rerandomization, assuming there are enough standby locations for all standby progression items. Note location order is not reset
            if (standbyLocations.Count > standbyProgression.Count && standbyItems.Any() && rand.Next(2) == 0)
            {
                int index = rand.Next(standbyLocations.Count);
                string location = standbyLocations[index];
                string item = standbyItems[0];
                standbyLocations.RemoveAt(index);
                standbyItems.RemoveAt(0);
                unplacedItems.Add(item);
                unplacedLocations.Insert(rand.Next(unplacedLocations.Count), location);
            }
        }

        public void TransferStandby()
        {
            standbyItems.AddRange(unplacedItems);
            unplacedItems = new List<string>();
            unplacedItems.AddRange(standbyProgression);
            unplacedItems.AddRange(unplacedProgression);
            unplacedItems.AddRange(standbyItems);

            standbyLocations.AddRange(unplacedLocations);
            unplacedLocations = standbyLocations;
        }

        public void PlaceItem(string item, string location)
        {
            if (shopItems.ContainsKey(location)) shopItems[location].Add(item);
            else nonShopItems.Add(location, item);
            UpdateOrder(location);
            unplacedLocations.Remove(location);
            if (LogicManager.GetItemDef(item).progression)
            {
                unplacedProgression.Remove(item);
                UpdateReachableLocations(item);
            }
            else unplacedItems.Remove(item);

            if (LogicManager.GetItemsByPool("Grub").Contains(item))
            {
                pm.AddGrubLocation(location);
            }
            else if (LogicManager.GetItemsByPool("Root").Contains(item))
            {
                pm.AddEssenceLocation(location, LogicManager.GetItemDef(item).geo);
            }
            else if (LogicManager.GetItemsByPool("Flame").Contains(item))
            {
                pm.AddFlameLocation(location);
            }
        }

        public void PlaceItemFromStandby(string item, string location)
        {
            if (shopItems.ContainsKey(location)) shopItems[location].Add(item);
            else nonShopItems.Add(location, item);
            UpdateOrder(location);
            unplacedLocations.Remove(location);
            unplacedItems.Remove(item);
        }

        public void PlaceProgressionToStandby(string item)
        {
            unplacedProgression.Remove(item);
            standbyProgression.Add(item);
            UpdateReachableLocations(item);
        }

        public void PlaceJunkItemToStandby(string item, string location)
        {
            standbyItems.Add(item);
            standbyLocations.Add(location);
            UpdateOrder(location);
            unplacedItems.Remove(item);
            unplacedLocations.Remove(location);
        }

        public void UpdateOrder(string location)
        {
            if (!locationOrder.ContainsKey(location)) locationOrder[location] = locationOrder.Count + 1;
        }

        // debugging stuff

        public void LogLocationStatus(string loc)
        {
            if (unplacedLocations.Contains(loc)) RandomizerMod.Instance.Log($"{loc} unfilled.");
            else if (nonShopItems.ContainsKey(loc)) RandomizerMod.Instance.Log($"{loc} filled with {nonShopItems[loc]}");
            else Log($"{loc} not found.");
        }

        private void LogDataConflicts()
        {
            string stuff = pm.ListObtainedProgression();
            foreach (string _item in stuff.Split(','))
            {
                string item = _item.Trim();
                if (string.IsNullOrEmpty(item)) continue;
                if (!nonShopItems.ContainsValue(item) && !standbyProgression.Contains(item))
                {
                    if (LogicManager.ShopNames.All(shop => !shopItems[shop].Contains(item)))
                    {
                        LogWarn("Found " + item + " in inventory, unable to trace origin.");
                    }
                }
            }
        }
    }
}

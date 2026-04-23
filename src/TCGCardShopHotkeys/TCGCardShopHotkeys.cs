using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TCGCardShopHotkeys
{
    [BepInPlugin("copilot.tcgcardshop.hotkeys", "TCG Card Shop Hotkeys", "1.0.0")]
    public class TCGCardShopHotkeysPlugin : BaseUnityPlugin
    {
        private const int ExpBoost = 100000;
        private const float MoneyBoost = 100000f;

        private MethodInfo _queueEventMethod;
        private ConstructorInfo _addCoinEventCtor;
        private ConstructorInfo _addShopExpEventCtor;
        private ConstructorInfo _addFameEventCtor;
        private ConstructorInfo _moneyUpdatedEventCtor;

        private FieldInfo _playerInstanceField;
        private FieldInfo _coinAmountField;
        private FieldInfo _coinAmountDoubleField;
        private FieldInfo _shopExpPointField;
        private FieldInfo _famePointField;
        private FieldInfo _playerGradeInProgressListField;
        private FieldInfo _playerCurrentGradeSubmitSetField;

        private FieldInfo _gameDataGradeInProgressListField;
        private FieldInfo _gameDataCurrentGradeSubmitSetField;

        private FieldInfo _gameManagerInstanceField;
        private FieldInfo _gameManagerDataField;
        private FieldInfo _gameDataInstanceField;
        private FieldInfo _gradeSetDayPassedField;
        private FieldInfo _gradeSetMinutePassedField;

        private Type _endDayReportScreenType;
        private MethodInfo _endDayOpenScreenMethod;
        private MethodInfo _endDayOnPressMethod;
        private object _endDayInstance;

        private MethodInfo _workerManagerNextDayMethod;
        private MethodInfo _shelfManagerNextDayMethod;
        private MethodInfo _lightManagerNextDayMethod;

        private Harmony _harmony;

        private static ManualLogSource _staticLogger;
        private static bool _perfectGradeRandomHookActive;

        private static FieldInfo _gradeInProgressListStaticField;
        private static FieldInfo _gradeSubmitCardDataListField;
        private static FieldInfo _cardGradeField;

        private static bool _expensiveBoosterMode = false;
        private static FieldInfo _rolledCardDataListField;
        private static FieldInfo _secondaryRolledCardDataListField;
        private static FieldInfo _cardMonsterTypeField;
        private static FieldInfo _cardBorderTypeField;
        private static FieldInfo _cardIsFoilField;
        private static FieldInfo _cardIsDestinyField;
        private static FieldInfo _cardExpansionTypeField;
        private static MethodInfo _getShownMonsterListMethod;
        private static MethodInfo _getCardMarketPriceMethod;

        private bool _warnedNoMoneyPath;
        private bool _warnedNoNextDayPath;

        private void Awake()
        {
            _staticLogger = Logger;
            CacheGameApi();
            SetupHarmonyPatches();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                if (TryAddExperience(out var result))
                {
                    Logger.LogInfo($"F8: +{ExpBoost} EXP shop/joueur appliqué ({result}).");
                }
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                if (TryAddMoney(out var result))
                {
                    Logger.LogInfo($"F9: +{(int)MoneyBoost} appliqué ({result}).");
                }
                else
                {
                    if (!_warnedNoMoneyPath)
                    {
                        Logger.LogWarning("F9: impossible de trouver un chemin compatible pour modifier l'argent. Active la console BepInEx pour plus de logs.");
                        _warnedNoMoneyPath = true;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                if (TryNextDay(out var result))
                {
                    Logger.LogInfo($"F10: passage au jour suivant ({result}).");
                }
                else
                {
                    if (!_warnedNoNextDayPath)
                    {
                        Logger.LogWarning("F10: impossible de trouver un chemin compatible pour passer au jour suivant. Active la console BepInEx pour plus de logs.");
                        _warnedNoNextDayPath = true;
                    }
                }
            }
        }

        private void CacheGameApi()
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => string.Equals(a.GetName().Name, "Assembly-CSharp", StringComparison.OrdinalIgnoreCase));
            if (asm == null)
            {
                Logger.LogWarning("Assembly-CSharp introuvable au démarrage, tentative au premier raccourci.");
                return;
            }

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            var cEventType = asm.GetType("CEvent");
            var eventManagerType = asm.GetType("CEventManager");
            _queueEventMethod = eventManagerType?.GetMethod("QueueEvent", flags, null, new[] { cEventType }, null);

            var addCoinType = asm.GetType("CEventPlayer_AddCoin");
            _addCoinEventCtor = addCoinType?.GetConstructor(new[] { typeof(float), typeof(bool) });

            var addShopExpType = asm.GetType("CEventPlayer_AddShopExp");
            _addShopExpEventCtor = addShopExpType?.GetConstructor(new[] { typeof(int), typeof(bool) });

            var addFameType = asm.GetType("CEventPlayer_AddFame");
            _addFameEventCtor = addFameType?.GetConstructor(new[] { typeof(int), typeof(bool) });

            var moneyUpdatedType = asm.GetType("CEventPlayer_OnMoneyCurrencyUpdated");
            _moneyUpdatedEventCtor = moneyUpdatedType?.GetConstructor(Type.EmptyTypes);

            var playerDataType = asm.GetType("CPlayerData");
            _playerInstanceField = playerDataType?.GetField("m_Instance", flags);
            _coinAmountField = playerDataType?.GetField("m_CoinAmount", flags);
            _coinAmountDoubleField = playerDataType?.GetField("m_CoinAmountDouble", flags);
            _shopExpPointField = playerDataType?.GetField("m_ShopExpPoint", flags);
            _famePointField = playerDataType?.GetField("m_FamePoint", flags);
            _playerGradeInProgressListField = playerDataType?.GetField("m_GradeCardInProgressList", flags);
            _playerCurrentGradeSubmitSetField = playerDataType?.GetField("m_CurrentGradeCardSubmitSet", flags);

            _gradeInProgressListStaticField = playerDataType?.GetField("m_GradeCardInProgressList", flags);

            var gradeCardSubmitSetType = asm.GetType("GradeCardSubmitSet");
            _gradeSubmitCardDataListField = gradeCardSubmitSetType?.GetField("m_CardDataList", flags);

            var cardDataType = asm.GetType("CardData");
            _cardGradeField = cardDataType?.GetField("cardGrade", flags);
            _cardMonsterTypeField = cardDataType?.GetField("monsterType", flags);
            _cardBorderTypeField = cardDataType?.GetField("borderType", flags);
            _cardIsFoilField = cardDataType?.GetField("isFoil", flags);
            _cardIsDestinyField = cardDataType?.GetField("isDestiny", flags);
            _cardExpansionTypeField = cardDataType?.GetField("expansionType", flags);

            var cardOpeningType = asm.GetType("CardOpeningSequence");
            _rolledCardDataListField = cardOpeningType?.GetField("m_RolledCardDataList", flags);
            _secondaryRolledCardDataListField = cardOpeningType?.GetField("m_SecondaryRolledCardDataList", flags);

            var inventoryBaseType = asm.GetType("InventoryBase");
            _getShownMonsterListMethod = inventoryBaseType?.GetMethod("GetShownMonsterList", flags, null, new[] { _cardExpansionTypeField?.FieldType }, null);

            _getCardMarketPriceMethod = playerDataType?.GetMethod("GetCardMarketPrice", flags, null, new[] { cardDataType }, null);

            var gameDataType = asm.GetType("CGameData");
            _gameDataGradeInProgressListField = gameDataType?.GetField("m_GradeCardInProgressList", flags);
            _gameDataCurrentGradeSubmitSetField = gameDataType?.GetField("m_CurrentGradeCardSubmitSet", flags);
            _gameDataInstanceField = gameDataType?.GetField("instance", flags);

            var gameManagerType = asm.GetType("CGameManager");
            _gameManagerInstanceField = gameManagerType?.GetField("m_Instance", flags);
            _gameManagerDataField = gameManagerType?.GetField("m_GameData", flags);

            _gradeSetDayPassedField = gradeCardSubmitSetType?.GetField("m_DayPassed", flags);
            _gradeSetMinutePassedField = gradeCardSubmitSetType?.GetField("m_MinutePassed", flags);

            _endDayReportScreenType = asm.GetType("EndOfDayReportScreen");
            _endDayOpenScreenMethod = _endDayReportScreenType?.GetMethod("OpenScreen", flags, null, Type.EmptyTypes, null);
            _endDayOnPressMethod = _endDayReportScreenType?.GetMethod("OnPressGoNextDay", flags, null, Type.EmptyTypes, null);

            var workerManagerType = asm.GetType("WorkerManager");
            _workerManagerNextDayMethod = workerManagerType?.GetMethod("OnPressGoNextDay", flags, null, Type.EmptyTypes, null);

            var shelfManagerType = asm.GetType("ShelfManager");
            _shelfManagerNextDayMethod = shelfManagerType?.GetMethod("OnPressGoNextDay", flags, null, Type.EmptyTypes, null);

            var lightManagerType = asm.GetType("LightManager");
            _lightManagerNextDayMethod = lightManagerType?.GetMethod("GoNextDay", flags, null, Type.EmptyTypes, null);
        }

        private void SetupHarmonyPatches()
        {
            if (_harmony != null)
            {
                return;
            }

            _harmony = new Harmony("copilot.tcgcardshop.hotkeys.harmony");
            _harmony.PatchAll(typeof(TCGCardShopHotkeysPlugin).Assembly);
        }

        private bool TryAddMoney(out string route)
        {
            if (_queueEventMethod == null || _addCoinEventCtor == null)
            {
                CacheGameApi();
            }

            try
            {
                if (_queueEventMethod != null && _addCoinEventCtor != null)
                {
                    var addCoinEvent = _addCoinEventCtor.Invoke(new object[] { MoneyBoost, false });
                    _queueEventMethod.Invoke(null, new[] { addCoinEvent });
                    QueueMoneyUpdatedEvent();
                    route = "CEventPlayer_AddCoin via CEventManager.QueueEvent";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"F9 event path failed: {ex.Message}");
            }

            try
            {
                var playerInstance = _playerInstanceField?.GetValue(null);
                if (playerInstance != null && _coinAmountField != null && _coinAmountDoubleField != null)
                {
                    var currentFloat = Convert.ToSingle(_coinAmountField.GetValue(playerInstance));
                    var currentDouble = Convert.ToDouble(_coinAmountDoubleField.GetValue(playerInstance));

                    _coinAmountField.SetValue(playerInstance, currentFloat + MoneyBoost);
                    _coinAmountDoubleField.SetValue(playerInstance, currentDouble + MoneyBoost);

                    QueueMoneyUpdatedEvent();
                    route = "CPlayerData.m_CoinAmount/m_CoinAmountDouble";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"F9 fallback path failed: {ex.Message}");
            }

            route = string.Empty;
            return false;
        }

        private bool TryAddExperience(out string route)
        {
            if (_queueEventMethod == null || _addShopExpEventCtor == null || _addFameEventCtor == null)
            {
                CacheGameApi();
            }

            try
            {
                if (_queueEventMethod != null && _addShopExpEventCtor != null && _addFameEventCtor != null)
                {
                    var addShopExpEvent = _addShopExpEventCtor.Invoke(new object[] { ExpBoost, false });
                    var addFameEvent = _addFameEventCtor.Invoke(new object[] { ExpBoost, false });

                    _queueEventMethod.Invoke(null, new[] { addShopExpEvent });
                    _queueEventMethod.Invoke(null, new[] { addFameEvent });

                    route = "CEventPlayer_AddShopExp + CEventPlayer_AddFame";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"F8 event path failed: {ex.Message}");
            }

            try
            {
                var playerInstance = _playerInstanceField?.GetValue(null);
                if (playerInstance != null && _shopExpPointField != null && _famePointField != null)
                {
                    var currentExp = Convert.ToInt32(_shopExpPointField.GetValue(playerInstance));
                    var currentFame = Convert.ToInt32(_famePointField.GetValue(playerInstance));

                    _shopExpPointField.SetValue(playerInstance, currentExp + ExpBoost);
                    _famePointField.SetValue(playerInstance, currentFame + ExpBoost);

                    route = "CPlayerData.m_ShopExpPoint/m_FamePoint";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"F8 fallback path failed: {ex.Message}");
            }

            route = string.Empty;
            return false;
        }

        private bool TryNextDay(out string route)
        {
            if (_endDayOnPressMethod == null)
            {
                CacheGameApi();
            }

            var primedCount = PrimeGradingForNextDay();

            try
            {
                _endDayOpenScreenMethod?.Invoke(null, Array.Empty<object>());

                if (_endDayInstance == null && _endDayReportScreenType != null)
                {
                    _endDayInstance = Resources.FindObjectsOfTypeAll(_endDayReportScreenType).FirstOrDefault();
                }

                if (_endDayInstance != null && _endDayOnPressMethod != null)
                {
                    _endDayOnPressMethod.Invoke(_endDayInstance, Array.Empty<object>());
                    route = $"EndOfDayReportScreen.OnPressGoNextDay + gradingPrimed:{primedCount}";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"F10 end-day screen path failed: {ex.Message}");
            }

            try
            {
                if (_workerManagerNextDayMethod != null)
                {
                    _workerManagerNextDayMethod.Invoke(null, Array.Empty<object>());
                    route = $"WorkerManager.OnPressGoNextDay + gradingPrimed:{primedCount}";
                    return true;
                }

                if (_shelfManagerNextDayMethod != null)
                {
                    _shelfManagerNextDayMethod.Invoke(null, Array.Empty<object>());
                    route = $"ShelfManager.OnPressGoNextDay + gradingPrimed:{primedCount}";
                    return true;
                }

                if (_lightManagerNextDayMethod != null)
                {
                    _lightManagerNextDayMethod.Invoke(null, Array.Empty<object>());
                    route = $"LightManager.GoNextDay + gradingPrimed:{primedCount}";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"F10 fallback path failed: {ex.Message}");
            }

            route = string.Empty;
            return false;
        }

        private int PrimeGradingForNextDay()
        {
            try
            {
                if (_playerGradeInProgressListField == null || _gradeSetMinutePassedField == null)
                {
                    return 0;
                }

                var listObj = _playerGradeInProgressListField.GetValue(null);
                if (listObj is not IList list)
                {
                    return 0;
                }

                var primed = 0;
                for (var i = 0; i < list.Count; i++)
                {
                    var gradeSet = list[i];
                    if (gradeSet == null)
                    {
                        continue;
                    }

                    var minutePassed = Convert.ToSingle(_gradeSetMinutePassedField.GetValue(gradeSet));
                    if (minutePassed < 540f)
                    {
                        _gradeSetMinutePassedField.SetValue(gradeSet, 540f);
                        primed++;
                    }
                }

                return primed;
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Prime grading failed: {ex.Message}");
                return 0;
            }
        }

        private int IncrementGradingProgressOneDay()
        {
            try
            {
                if (_playerInstanceField == null || _gradeSetDayPassedField == null)
                {
                    return 0;
                }

                var playerInstance = _playerInstanceField.GetValue(null);
                if (playerInstance == null)
                {
                    return 0;
                }

                var updatedCount = 0;
                var processedSets = new HashSet<object>(ReferenceEqualityComparer.Instance);

                updatedCount += IncrementGradingFromRoot(
                    playerInstance,
                    _playerCurrentGradeSubmitSetField,
                    _playerGradeInProgressListField,
                    processedSets);

                var gameManagerInstance = _gameManagerInstanceField?.GetValue(null);
                var gameDataFromManager = gameManagerInstance != null ? _gameManagerDataField?.GetValue(gameManagerInstance) : null;
                if (gameDataFromManager != null)
                {
                    updatedCount += IncrementGradingFromRoot(
                        gameDataFromManager,
                        _gameDataCurrentGradeSubmitSetField,
                        _gameDataGradeInProgressListField,
                        processedSets);
                }

                var gameDataInstance = _gameDataInstanceField?.GetValue(null);
                if (gameDataInstance != null)
                {
                    updatedCount += IncrementGradingFromRoot(
                        gameDataInstance,
                        _gameDataCurrentGradeSubmitSetField,
                        _gameDataGradeInProgressListField,
                        processedSets);
                }

                if (updatedCount == 0)
                {
                    updatedCount += IncrementGradingBySceneScan(processedSets);
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Grading day increment failed: {ex.Message}");
                return 0;
            }
        }

        private int IncrementGradingFromRoot(object root, FieldInfo currentSetField, FieldInfo inProgressListField, HashSet<object> processedSets)
        {
            if (root == null || _gradeSetDayPassedField == null)
            {
                return 0;
            }

            var updated = 0;

            var currentSet = currentSetField?.GetValue(root);
            if (TryIncrementGradeSet(currentSet, processedSets, out var updatedCurrent))
            {
                updated += updatedCurrent;
                currentSetField?.SetValue(root, currentSet);
            }

            var listObj = inProgressListField?.GetValue(root);
            if (listObj is IList list)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var entry = list[index];
                    if (TryIncrementGradeSet(entry, processedSets, out var inc))
                    {
                        list[index] = entry;
                        updated += inc;
                    }
                }
            }
            else if (listObj is IEnumerable enumerable)
            {
                foreach (var entry in enumerable)
                {
                    if (TryIncrementGradeSet(entry, processedSets, out var inc))
                    {
                        updated += inc;
                    }
                }
            }

            return updated;
        }

        private bool TryIncrementGradeSet(object gradeSet, HashSet<object> processedSets, out int updated)
        {
            updated = 0;
            if (gradeSet == null)
            {
                return false;
            }

            if (!gradeSet.GetType().IsValueType && processedSets != null && !processedSets.Add(gradeSet))
            {
                return false;
            }

            var dayPassed = Convert.ToInt32(_gradeSetDayPassedField.GetValue(gradeSet));
            _gradeSetDayPassedField.SetValue(gradeSet, dayPassed + 1);

            if (_gradeSetMinutePassedField != null)
            {
                var minutePassed = Convert.ToSingle(_gradeSetMinutePassedField.GetValue(gradeSet));
                _gradeSetMinutePassedField.SetValue(gradeSet, minutePassed + 1440f);
            }

            updated = 1;
            return true;
        }

        private int IncrementGradingBySceneScan(HashSet<object> processedSets)
        {
            if (_gradeSetDayPassedField == null)
            {
                return 0;
            }

            var total = 0;
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var behaviour in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
            {
                if (behaviour == null)
                {
                    continue;
                }

                var type = behaviour.GetType();
                foreach (var field in type.GetFields(flags))
                {
                    var value = field.GetValue(behaviour);
                    if (value == null)
                    {
                        continue;
                    }

                    if (string.Equals(field.FieldType.Name, "GradeCardSubmitSet", StringComparison.Ordinal))
                    {
                        if (TryIncrementGradeSet(value, processedSets, out var inc))
                        {
                            field.SetValue(behaviour, value);
                            total += inc;
                        }
                    }
                    else if (value is IList list && field.FieldType.IsGenericType && string.Equals(field.FieldType.GetGenericArguments()[0].Name, "GradeCardSubmitSet", StringComparison.Ordinal))
                    {
                        for (var i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            if (TryIncrementGradeSet(item, processedSets, out var inc))
                            {
                                list[i] = item;
                                total += inc;
                            }
                        }
                    }
                }
            }

            return total;
        }

        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

            public new bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        private void QueueMoneyUpdatedEvent()
        {
            if (_queueEventMethod == null || _moneyUpdatedEventCtor == null)
            {
                return;
            }

            try
            {
                var evt = _moneyUpdatedEventCtor.Invoke(Array.Empty<object>());
                _queueEventMethod.Invoke(null, new[] { evt });
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Money update event failed: {ex.Message}");
            }
        }

        private static void BeforeRestockDayStarted()
        {
            _perfectGradeRandomHookActive = true;
            ForceAllInProgressCardGradesToZero();
        }

        private static void AfterRestockDayStarted()
        {
            _perfectGradeRandomHookActive = false;
        }

        private static void ForceAllInProgressCardGradesToZero()
        {
            try
            {
                if (_gradeInProgressListStaticField == null || _gradeSubmitCardDataListField == null || _cardGradeField == null)
                {
                    return;
                }

                var listObj = _gradeInProgressListStaticField.GetValue(null);
                if (listObj is not IEnumerable gradingSets)
                {
                    return;
                }

                foreach (var gradeSet in gradingSets)
                {
                    if (gradeSet == null)
                    {
                        continue;
                    }

                    var cardListObj = _gradeSubmitCardDataListField.GetValue(gradeSet);
                    if (cardListObj is not IEnumerable cards)
                    {
                        continue;
                    }

                    foreach (var card in cards)
                    {
                        if (card == null)
                        {
                            continue;
                        }

                        _cardGradeField.SetValue(card, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                _staticLogger?.LogDebug($"Perfect grade prepare failed: {ex.Message}");
            }
        }

        private static void ApplyExpensiveBoosterCards(object cardOpeningSequenceInstance, bool isSecondaryRolledData)
        {
            if (!_expensiveBoosterMode || cardOpeningSequenceInstance == null)
            {
                return;
            }

            try
            {
                var listField = isSecondaryRolledData ? _secondaryRolledCardDataListField : _rolledCardDataListField;
                if (listField == null)
                {
                    return;
                }

                if (listField.GetValue(cardOpeningSequenceInstance) is not IList cardList)
                {
                    return;
                }

                for (var index = 0; index < cardList.Count; index++)
                {
                    var card = cardList[index];
                    if (card == null)
                    {
                        continue;
                    }

                    MaximizeCardValue(card);
                    cardList[index] = card;
                }
            }
            catch (Exception ex)
            {
                _staticLogger?.LogDebug($"Expensive booster patch failed: {ex.Message}");
            }
        }

        private static void MaximizeCardValue(object card)
        {
            if (_cardMonsterTypeField == null || _cardBorderTypeField == null || _cardIsFoilField == null || _cardIsDestinyField == null || _cardExpansionTypeField == null || _getShownMonsterListMethod == null || _getCardMarketPriceMethod == null)
            {
                return;
            }

            var expansion = _cardExpansionTypeField.GetValue(card);
            if (expansion == null)
            {
                return;
            }

            var expansionName = expansion.ToString() ?? string.Empty;
            var isGhostExpansion = string.Equals(expansionName, "Ghost", StringComparison.OrdinalIgnoreCase);
            var isDestinyExpansion = string.Equals(expansionName, "Destiny", StringComparison.OrdinalIgnoreCase);

            _cardIsFoilField.SetValue(card, true);

            var borderType = _cardBorderTypeField.FieldType;
            var targetBorder = Enum.Parse(borderType, isGhostExpansion ? "Base" : "FullArt", ignoreCase: true);
            _cardBorderTypeField.SetValue(card, targetBorder);

            var shownMonsterListObj = _getShownMonsterListMethod.Invoke(null, new[] { expansion });
            if (shownMonsterListObj is not IEnumerable shownMonsterList)
            {
                return;
            }

            object bestMonster = null;
            var bestIsDestiny = isDestinyExpansion;
            var bestPrice = -1f;

            foreach (var monster in shownMonsterList)
            {
                if (monster == null)
                {
                    continue;
                }

                _cardMonsterTypeField.SetValue(card, monster);

                if (isGhostExpansion)
                {
                    _cardIsDestinyField.SetValue(card, false);
                    var nonDestinyPrice = Convert.ToSingle(_getCardMarketPriceMethod.Invoke(null, new[] { card }));
                    if (nonDestinyPrice > bestPrice)
                    {
                        bestPrice = nonDestinyPrice;
                        bestMonster = monster;
                        bestIsDestiny = false;
                    }

                    _cardIsDestinyField.SetValue(card, true);
                    var destinyPrice = Convert.ToSingle(_getCardMarketPriceMethod.Invoke(null, new[] { card }));
                    if (destinyPrice > bestPrice)
                    {
                        bestPrice = destinyPrice;
                        bestMonster = monster;
                        bestIsDestiny = true;
                    }
                }
                else
                {
                    _cardIsDestinyField.SetValue(card, isDestinyExpansion);
                    var price = Convert.ToSingle(_getCardMarketPriceMethod.Invoke(null, new[] { card }));
                    if (price > bestPrice)
                    {
                        bestPrice = price;
                        bestMonster = monster;
                        bestIsDestiny = isDestinyExpansion;
                    }
                }
            }

            if (bestMonster != null)
            {
                _cardMonsterTypeField.SetValue(card, bestMonster);
                _cardIsDestinyField.SetValue(card, bestIsDestiny);
            }
        }

        [HarmonyPatch]
        private static class RestockDayStartedPatch
        {
            private static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("RestockManager");
                var eventType = AccessTools.TypeByName("CEventPlayer_OnDayStarted");
                return eventType == null ? null : AccessTools.Method(type, "OnDayStarted", new[] { eventType });
            }

            private static void Prefix()
            {
                BeforeRestockDayStarted();
            }

            private static void Postfix()
            {
                AfterRestockDayStarted();
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.Range), new[] { typeof(int), typeof(int) })]
        private static class PerfectGradeRandomPatch
        {
            private static bool Prefix(int minInclusive, int maxExclusive, ref int __result)
            {
                if (!_perfectGradeRandomHookActive)
                {
                    return true;
                }

                if (minInclusive == 0 && maxExclusive == 100)
                {
                    __result = 0;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch]
        private static class ExpensiveBoosterPatch
        {
            private static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("CardOpeningSequence");
                var collectionPackType = AccessTools.TypeByName("ECollectionPackType");
                if (type == null || collectionPackType == null)
                {
                    return null;
                }

                return AccessTools.Method(type, "GetPackContent", new[] { typeof(bool), typeof(int), typeof(bool), collectionPackType });
            }

            private static void Postfix(object __instance, bool isSecondaryRolledData)
            {
                ApplyExpensiveBoosterCards(__instance, isSecondaryRolledData);
            }
        }
    }
}

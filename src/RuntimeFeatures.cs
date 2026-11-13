// Only __BongoClaim_ members are transplanted into the installed game's types.
// Its inventory, purchase callback and Steam stat APIs remain the owners of state.
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace BongoCat {
    public partial class ShopItem {
        public bool __BongoClaim_CanOpenOwned_v1() {
            return !_waitingForServer && _chestExchanger && !Steam.ChestExchanger.__BongoClaim_IsBusy_v1()
                && SteamTools.Interface.IsReady
                && SteamTools.Game.Inventory.Achievement_Token.GetTotalQuantity() > 0
                && SteamTools.Game.Inventory.ManualToken.GetTotalQuantity() > 0
                && _chestExchanger.__BongoClaim_CanExchange_v1() && CanBuy();
        }
    }
    public partial class Shop {
        public bool __BongoClaim_OpenOwned_v1(bool emote) {
            if (_isEmoteShop != emote || _openingChest || !_shopItem) return false;
            long quantity = emote ? SteamTools.Game.Inventory.Emote_Chest_Token.GetTotalQuantity() : SteamTools.Game.Inventory.Chest_Token.GetTotalQuantity();
            if (quantity <= 0 || !_shopItem.__BongoClaim_CanOpenOwned_v1()) return false;
            _shopItem.Buy(); return true;
        }
        public bool __BongoClaim_IsOwnedType_v1(bool emote) { return _isEmoteShop == emote; }
    }
    public partial class Pets {
        public int __BongoClaim_GetScore_v1() { return Current; }
        public bool __BongoClaim_SetScore_v1(int score) {
            if (!_init || score < 0 || _totalSpent < 0 || score > 2147483646 - _totalSpent) return false;
            _currentGained = score + _totalSpent;
            UpdateStats();
            Heathen.SteamworksIntegration.API.StatsAndAchievements.Client.StoreStats();
            return true;
        }
    }
    public partial class SettingsManager {
        private static GameObject __BongoClaim_NormalTokensRow_v1, __BongoClaim_EmoteTokensRow_v1, __BongoClaim_ScoreRow_v1;
        private static bool __BongoClaim_EditingScore_v1;
        private static string __BongoClaim_ScoreBuffer_v1;
        private static float __BongoClaim_NextRefresh_v1;
        private static GameObject __BongoClaim_ExchangeRow_v1;
        private static GameObject __BongoClaim_ExchangeIcon_v1;
        private static bool __BongoClaim_ThemeApplied_v1;
        private static float __BongoClaim_NextThemeProbe_v1;
        private static Component __BongoClaim_ThemeIconColor_v1;
        private static bool __BongoClaim_ThemeSelected_v1;
        private static bool __BongoClaim_AutoExchange_v1;
        private static bool __BongoClaim_ExchangeInFlight_v1;
        private static float __BongoClaim_NextExchange_v1;
        private static object[] __BongoClaim_ExchangeItems_v1;
        private static long[] __BongoClaim_ExchangeAmounts_v1;
        private static string __BongoClaim_ExchangeStatus_v1 = "DESACTIVADO";
        private static void __BongoClaim_SetLabel_v1(GameObject row, string text) {
            if (!row) return;
            var label = row.transform.Find("Content/Label").GetComponent<TMP_Text>();
            if (label.text != text) label.text = text;
        }
        private static void __BongoClaim_ClaimNormal_v1() { __BongoClaim_ClaimOwned_v1(false); }
        private static void __BongoClaim_ClaimEmote_v1() { __BongoClaim_ClaimOwned_v1(true); }
        private static void __BongoClaim_ClaimOwned_v1(bool emote) {
            if (__BongoClaim_EditingScore_v1) return;
            var shops = UnityEngine.Object.FindObjectsOfType<Shop>(true);
            foreach (var shop in shops) {
                if (shop && shop.__BongoClaim_IsOwnedType_v1(emote)) { shop.__BongoClaim_OpenOwned_v1(emote); break; }
            }
            __BongoClaim_NextRefresh_v1 = 0;
        }
        private static Type __BongoClaim_FindExchangeType_v1() {
            foreach (MonoBehaviour candidate in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true))
                if (candidate && candidate.GetType().FullName == "BongoCat.ItemExchange") return candidate.GetType();
            return null;
        }
        private static void __BongoClaim_EditScore_v1() {
            if (!Pets.Instance || !__BongoClaim_ScoreRow_v1) return;
            __BongoClaim_EditingScore_v1 = !__BongoClaim_EditingScore_v1;
            __BongoClaim_ScoreBuffer_v1 = "";
            __BongoClaim_NextRefresh_v1 = 0;
            __BongoClaim_SetLabel_v1(__BongoClaim_ScoreRow_v1, "Escribe la puntuación y pulsa Intro para guardar");
        }
        private static string __BongoClaim_ExchangeLabel_v1() {
            return "🔄 Intercambio automático de emojis duplicados: " + __BongoClaim_ExchangeStatus_v1;
        }
        private static void __BongoClaim_UpdateExchangeIconState_v1() {
            if (__BongoClaim_ExchangeIcon_v1) {
                var labelObject = __BongoClaim_ExchangeIcon_v1.transform.Find("Label");
                if (labelObject) {
                    var label = labelObject.GetComponent<TMP_Text>();
                    if (label) {
                        label.text = __BongoClaim_AutoExchange_v1 ? "♥ ON" : "♡ OFF";
                        label.color = __BongoClaim_AutoExchange_v1 ? new Color(0.22f, 0.82f, 0.48f, 1f) : new Color(0.58f, 0.61f, 0.65f, 1f);
                        label.fontSize = 10f;
                    }
                }
            }
            if (__BongoClaim_ThemeIconColor_v1 && __BongoClaim_ThemeSelected_v1 != __BongoClaim_AutoExchange_v1) {
                __BongoClaim_FindMethod_v1(__BongoClaim_ThemeIconColor_v1.GetType(), "SetSelected", 1).Invoke(__BongoClaim_ThemeIconColor_v1, new object[] { __BongoClaim_AutoExchange_v1 });
                __BongoClaim_ThemeSelected_v1 = __BongoClaim_AutoExchange_v1;
            }
        }
        private static void __BongoClaim_ToggleExchange_v1() {
            __BongoClaim_AutoExchange_v1 = !__BongoClaim_AutoExchange_v1;
            __BongoClaim_NextExchange_v1 = Time.unscaledTime + 3f;
            __BongoClaim_ExchangeStatus_v1 = __BongoClaim_AutoExchange_v1 ? "ACTIVO | esperando" : (__BongoClaim_ExchangeInFlight_v1 ? "DESACTIVADO | terminando intercambio" : "DESACTIVADO");
            __BongoClaim_UpdateExchangeIconState_v1();
            __BongoClaim_SetLabel_v1(__BongoClaim_ExchangeRow_v1, __BongoClaim_ExchangeLabel_v1());
            UnityEngine.Debug.Log("[Bongo Cat Auto Chest] Emoji auto-exchange " + (__BongoClaim_AutoExchange_v1 ? "ON" : "OFF"));
        }
        private static Component __BongoClaim_FindThemedSibling_v1(Transform parent, Type themedType, GameObject exclude) {
            if (parent == null || themedType == null) return null;
            Component fallback = null;
            foreach (MonoBehaviour candidate in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true))
                if (candidate && candidate.gameObject != exclude && candidate.GetType() == themedType) {
                    if (candidate.transform.parent == parent) return candidate;
                    if (fallback == null) fallback = candidate;
                }
            return fallback;
        }
        private static void __BongoClaim_CopyThemeFields_v1(Component source, Component destination, string[] fields) {
            if (!source || !destination) return;
            foreach (string name in fields) {
                FieldInfo from = source.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo to = destination.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (from != null && to != null) to.SetValue(destination, from.GetValue(source));
            }
        }
        private static bool __BongoClaim_RefreshTheme_v1(Component themed) {
            if (!themed) return false;
            Type managerType = typeof(Shop).Assembly.GetType("BongoCat.UiThemes.UiThemeManager");
            Type componentType = typeof(Shop).Assembly.GetType("BongoCat.UiThemes.ThemedComponent");
            if (managerType == null || componentType == null) return false;
            var instanceProperty = managerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            var shownProperty = managerType.GetProperty("Shown", BindingFlags.Public | BindingFlags.Instance);
            object manager = instanceProperty == null ? null : instanceProperty.GetValue(null, null);
            if (manager == null || shownProperty == null || shownProperty.GetValue(manager, null) == null) return false;
            MethodInfo redraw = managerType.GetMethod("Redraw", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { componentType }, null);
            if (redraw == null) return false;
            redraw.Invoke(manager, new object[] { themed });
            return true;
        }
        private static void __BongoClaim_ApplyNativeTheme_v1(Component arrow, GameObject icon, Button button, TMP_Text label) {
            if (!arrow || !icon || !button || !label) return;
            if (__BongoClaim_ThemeApplied_v1) {
                if (__BongoClaim_ThemeIconColor_v1 && __BongoClaim_ThemeSelected_v1 != __BongoClaim_AutoExchange_v1) {
                    __BongoClaim_FindMethod_v1(__BongoClaim_ThemeIconColor_v1.GetType(), "SetSelected", 1).Invoke(__BongoClaim_ThemeIconColor_v1, new object[] { __BongoClaim_AutoExchange_v1 });
                    __BongoClaim_ThemeSelected_v1 = __BongoClaim_AutoExchange_v1;
                }
                return;
            }
            if (Time.unscaledTime < __BongoClaim_NextThemeProbe_v1) return;
            __BongoClaim_NextThemeProbe_v1 = Time.unscaledTime + 1f;
            bool wasActive = icon.activeSelf;
            icon.SetActive(false);
            try {
            Type gameAssembly = typeof(Shop).Assembly.GetType("BongoCat.UiThemes.ThemedButton");
            Type iconThemeType = typeof(Shop).Assembly.GetType("BongoCat.UiThemes.ThemedIconButton");
            Component source = gameAssembly == null ? null : arrow.GetComponent(gameAssembly);
            if (!source && gameAssembly != null) source = __BongoClaim_FindThemedSibling_v1(arrow.transform.parent, gameAssembly, icon);
            if (source) {
                Component target = icon.GetComponent(gameAssembly);
                if (!target) target = icon.AddComponent(gameAssembly);
                __BongoClaim_CopyThemeFields_v1(source, target, new string[] { "_normalSprite", "_highlightedSprite", "_pressedSprite", "_selectedSprite", "_disabledSprite", "_iconDefaultColor", "_iconHoverColor", "_iconSelectedColor" });
                FieldInfo sourceIconField = source.GetType().GetField("_iconColor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo targetIconField = target.GetType().GetField("_iconColor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                Component sourceIcon = sourceIconField == null ? null : sourceIconField.GetValue(source) as Component;
                if (sourceIcon && targetIconField != null) {
                    Type iconColorType = sourceIcon.GetType();
                    Component targetIcon = icon.GetComponent(iconColorType);
                    if (!targetIcon) targetIcon = icon.AddComponent(iconColorType);
                    __BongoClaim_CopyThemeFields_v1(sourceIcon, targetIcon, new string[] { "DefaultColor", "HoverColor", "SelectedColor" });
                    FieldInfo graphic = iconColorType.GetField("_icon", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (graphic != null) graphic.SetValue(targetIcon, label);
                    FieldInfo buttonField = iconColorType.GetField("_button", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (buttonField != null) buttonField.SetValue(targetIcon, button);
                    targetIconField.SetValue(target, targetIcon);
                    __BongoClaim_ThemeIconColor_v1 = targetIcon;
                }
                if (!wasActive) return;
                icon.SetActive(true);
                if (!__BongoClaim_RefreshTheme_v1(target)) return;
                __BongoClaim_ThemeApplied_v1 = true;
                if (__BongoClaim_ThemeIconColor_v1) {
                    __BongoClaim_FindMethod_v1(__BongoClaim_ThemeIconColor_v1.GetType(), "SetSelected", 1).Invoke(__BongoClaim_ThemeIconColor_v1, new object[] { __BongoClaim_AutoExchange_v1 });
                    __BongoClaim_ThemeSelected_v1 = __BongoClaim_AutoExchange_v1;
                }
                return;
            }
            if (iconThemeType == null) return;
            source = arrow.GetComponent(iconThemeType);
            if (!source) source = __BongoClaim_FindThemedSibling_v1(arrow.transform.parent, iconThemeType, icon);
            if (source) {
                Component target = icon.GetComponent(iconThemeType);
                if (!target) target = icon.AddComponent(iconThemeType);
                __BongoClaim_CopyThemeFields_v1(source, target, new string[] { "_normalColor", "_highlightedColor", "_pressedColor", "_selectedColor", "_disabledColor" });
                if (!wasActive) return;
                icon.SetActive(true);
                if (!__BongoClaim_RefreshTheme_v1(target)) return;
                __BongoClaim_ThemeApplied_v1 = true;
            }
            } finally {
                icon.SetActive(wasActive);
            }
        }
        private static void __BongoClaim_EnsureExchangeIcon_v1(object exchange) {
            var component = exchange as Component;
            if (!component) return;
            Type exchangeType = exchange.GetType();
            FieldInfo arrowField = __BongoClaim_FindField_v1(exchangeType, "_fillDuplicatesFill", false);
            var arrow = arrowField == null ? null : arrowField.GetValue(exchange) as Component;
            if (!arrow || !arrow.transform.parent) return;
            var parent = arrow.transform.parent;
            var existing = parent.Find("BongoClaim_AutoEmojiExchange");
            if (existing) __BongoClaim_ExchangeIcon_v1 = existing.gameObject;
            if (!__BongoClaim_ExchangeIcon_v1) {
                var icon = new GameObject("BongoClaim_AutoEmojiExchange", typeof(RectTransform));
                __BongoClaim_ThemeApplied_v1 = false;
                __BongoClaim_NextThemeProbe_v1 = 0f;
                var rect = icon.GetComponent<RectTransform>();
                icon.transform.SetParent(parent, false);
                var arrowRect = arrow.GetComponent<RectTransform>();
                rect.anchorMin = arrowRect.anchorMin; rect.anchorMax = arrowRect.anchorMax; rect.pivot = arrowRect.pivot;
                rect.sizeDelta = new Vector2(Math.Max(24f, arrowRect.sizeDelta.x), Math.Max(24f, arrowRect.sizeDelta.y));
                rect.anchoredPosition = new Vector2(arrowRect.anchoredPosition.x + (arrowRect.sizeDelta.x + 4f) * 4f, arrowRect.anchoredPosition.y);
                var image = icon.AddComponent<Image>(); image.color = Color.white;
                var button = icon.AddComponent<Button>(); button.onClick.AddListener(__BongoClaim_ToggleExchange_v1);
                button.targetGraphic = image;
                var labelObject = new GameObject("Label", typeof(RectTransform)); labelObject.transform.SetParent(icon.transform, false);
                var labelRect = labelObject.GetComponent<RectTransform>(); labelRect.anchorMin = new Vector2(0f, 0f); labelRect.anchorMax = new Vector2(1f, 1f); labelRect.sizeDelta = new Vector2(0f, 0f);
                var label = labelObject.AddComponent<TextMeshProUGUI>(); label.text = "↻"; label.alignment = TextAlignmentOptions.Center; label.fontSize = 25f; label.raycastTarget = false;
                __BongoClaim_ExchangeIcon_v1 = icon;
            }
            bool visible = (bool)exchangeType.GetProperty("IsVisible", BindingFlags.Public | BindingFlags.Instance).GetValue(exchange, null);
            int desiredIndex = arrow.transform.GetSiblingIndex() + 4;
            if (__BongoClaim_ExchangeIcon_v1.transform.GetSiblingIndex() != desiredIndex) __BongoClaim_ExchangeIcon_v1.transform.SetSiblingIndex(desiredIndex);
            var placedRect = __BongoClaim_ExchangeIcon_v1.GetComponent<RectTransform>();
            var sourceRect = arrow.GetComponent<RectTransform>();
            placedRect.anchorMin = sourceRect.anchorMin; placedRect.anchorMax = sourceRect.anchorMax; placedRect.pivot = sourceRect.pivot;
            placedRect.sizeDelta = new Vector2(Math.Max(40f, sourceRect.sizeDelta.x), Math.Max(24f, sourceRect.sizeDelta.y));
            placedRect.anchoredPosition = new Vector2(sourceRect.anchoredPosition.x + (sourceRect.sizeDelta.x + 4f) * 4f, sourceRect.anchoredPosition.y);
            __BongoClaim_ExchangeIcon_v1.SetActive(visible);
            var iconLabel = __BongoClaim_ExchangeIcon_v1.transform.Find("Label").GetComponent<TMP_Text>();
            var iconImage = __BongoClaim_ExchangeIcon_v1.GetComponent<Image>();
            float iconHeight = placedRect.sizeDelta.y;
            var iconButton = __BongoClaim_ExchangeIcon_v1.GetComponent<Button>();
            __BongoClaim_ApplyNativeTheme_v1(arrow, __BongoClaim_ExchangeIcon_v1, iconButton, iconLabel);
            __BongoClaim_UpdateExchangeIconState_v1();
        }
        private static FieldInfo __BongoClaim_FindField_v1(Type type, string name, bool isStatic) {
            return type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | (isStatic ? BindingFlags.Static : BindingFlags.Instance));
        }
        private static object __BongoClaim_ReadField_v1(object target, string name) {
            FieldInfo field = __BongoClaim_FindField_v1(target.GetType(), name, false);
            if (field == null) throw new MissingFieldException(target.GetType().FullName, name);
            return field.GetValue(target);
        }
        private static MethodInfo __BongoClaim_FindMethod_v1(Type type, string name, int parameters) {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                if (method.Name == name && method.GetParameters().Length == parameters) return method;
            throw new MissingMethodException(type.FullName, name);
        }
        private static object __BongoClaim_ResolveEmoteGroup_v1(Type groupType) {
            if (groupType == null || !groupType.IsEnum) throw new InvalidOperationException("The game's emoji exchange group is unavailable");
            string[] candidates = new string[] { "Emotes", "Emote", "Emojis", "Emoji" };
            foreach (string name in candidates) if (Enum.IsDefined(groupType, name)) return Enum.Parse(groupType, name);
            throw new InvalidOperationException("The game has no emoji exchange category");
        }
        private static bool __BongoClaim_ExchangeCanFill_v1(object exchange, object group) {
            object[] args = new object[] { group, null };
            return (bool)__BongoClaim_FindMethod_v1(exchange.GetType(), "CanFillExchangeWithDuplicates", 2).Invoke(exchange, args);
        }
        private static void __BongoClaim_RememberExchangeSlots_v1(object exchange) {
            IList slots = (IList)__BongoClaim_ReadField_v1(exchange, "_exchangeSlots");
            __BongoClaim_ExchangeItems_v1 = new object[slots.Count];
            __BongoClaim_ExchangeAmounts_v1 = new long[slots.Count];
            for (int i = 0; i < slots.Count; i++) {
                object slot = slots[i];
                __BongoClaim_ExchangeItems_v1[i] = __BongoClaim_ReadField_v1(slot, "ItemSlotted");
                __BongoClaim_ExchangeAmounts_v1[i] = Convert.ToInt64(__BongoClaim_ReadField_v1(slot, "_slottedAmount"));
            }
        }
        private static bool __BongoClaim_ExchangeSlotsUnchanged_v1(object exchange) {
            if (__BongoClaim_ExchangeItems_v1 == null) return false;
            IList slots = (IList)__BongoClaim_ReadField_v1(exchange, "_exchangeSlots");
            if (slots.Count != __BongoClaim_ExchangeItems_v1.Length) return false;
            for (int i = 0; i < slots.Count; i++) {
                object slot = slots[i];
                if (!System.Object.ReferenceEquals(__BongoClaim_ExchangeItems_v1[i], __BongoClaim_ReadField_v1(slot, "ItemSlotted")) ||
                    __BongoClaim_ExchangeAmounts_v1[i] != Convert.ToInt64(__BongoClaim_ReadField_v1(slot, "_slottedAmount"))) return false;
            }
            return true;
        }
        private static void __BongoClaim_ClearOwnedExchangeSlots_v1(object exchange) {
            if (!__BongoClaim_ExchangeSlotsUnchanged_v1(exchange)) return;
            IList slots = (IList)__BongoClaim_ReadField_v1(exchange, "_exchangeSlots");
            foreach (object slot in slots)
                __BongoClaim_FindMethod_v1(slot.GetType(), "SetItem", 2).Invoke(slot, new object[] { null, 1L });
            __BongoClaim_FindMethod_v1(exchange.GetType(), "UpdateInteractable", 0).Invoke(exchange, null);
            __BongoClaim_ExchangeItems_v1 = null;
            __BongoClaim_ExchangeAmounts_v1 = null;
        }
        private static void __BongoClaim_StartEmojiExchange_v1(object exchange, Type exchangeType, object group) {
            if ((bool)exchangeType.GetProperty("HasItemsSlotted", BindingFlags.Public | BindingFlags.Instance).GetValue(exchange, null)) {
                __BongoClaim_ExchangeStatus_v1 = "ACTIVO | espacios manuales intactos";
                return;
            }
            FieldInfo tabsField = __BongoClaim_FindField_v1(exchangeType, "EmoteTabs", true);
            if (tabsField == null) throw new MissingFieldException(exchangeType.FullName, "EmoteTabs");
            int[] tabs = (int[])tabsField.GetValue(null);
            if (tabs == null || tabs.Length == 0) throw new InvalidOperationException("No emoji exchange tab is available");
            __BongoClaim_FindMethod_v1(exchangeType, "OpenTab", 1).Invoke(exchange, new object[] { tabs[0] });
            if (!__BongoClaim_ExchangeCanFill_v1(exchange, group)) {
                __BongoClaim_ExchangeStatus_v1 = "ACTIVO | no hay duplicados";
                return;
            }
            __BongoClaim_FindMethod_v1(exchangeType, "SlotDuplicates", 0).Invoke(exchange, null);
            __BongoClaim_RememberExchangeSlots_v1(exchange);
            __BongoClaim_FindMethod_v1(exchangeType, "UpdateInteractable", 0).Invoke(exchange, null);
            int possible = Convert.ToInt32(__BongoClaim_ReadField_v1(exchange, "_possibleExchanges"));
            if (possible <= 0 || !__BongoClaim_ExchangeSlotsUnchanged_v1(exchange)) {
                __BongoClaim_ClearOwnedExchangeSlots_v1(exchange);
                __BongoClaim_ExchangeStatus_v1 = "ACTIVO | no hay duplicados";
                return;
            }
            MethodInfo routineMethod = __BongoClaim_FindMethod_v1(exchangeType, "ExchangeRoutine", 2);
            IList items = (IList)Activator.CreateInstance(routineMethod.GetParameters()[0].ParameterType);
            IList slots = (IList)__BongoClaim_ReadField_v1(exchange, "_exchangeSlots");
            foreach (object slot in slots) {
                object item = __BongoClaim_ReadField_v1(slot, "ItemSlotted");
                if (item != null) items.Add(item);
            }
            if (items.Count == 0) {
                __BongoClaim_ClearOwnedExchangeSlots_v1(exchange);
                __BongoClaim_ExchangeStatus_v1 = "ACTIVO | no hay duplicados";
                return;
            }
            IEnumerator routine = (IEnumerator)routineMethod.Invoke(exchange, new object[] { items, possible });
            MethodInfo startCoroutine = exchangeType.GetMethod("StartCoroutine", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IEnumerator) }, null);
            if (startCoroutine == null) throw new MissingMethodException(exchangeType.FullName, "StartCoroutine");
            startCoroutine.Invoke(exchange, new object[] { routine });
            __BongoClaim_ExchangeInFlight_v1 = true;
            __BongoClaim_ExchangeStatus_v1 = "ACTIVO | intercambiando";
        }
        private static void __BongoClaim_TickEmojiExchange_v1() {
            if (__BongoClaim_ExchangeRow_v1)
                __BongoClaim_SetLabel_v1(__BongoClaim_ExchangeRow_v1, __BongoClaim_ExchangeLabel_v1());
            try {
                Type visibleType = __BongoClaim_FindExchangeType_v1();
                FieldInfo visibleField = visibleType == null ? null : __BongoClaim_FindField_v1(visibleType, "Instance", true);
                object visibleExchange = visibleField == null ? null : visibleField.GetValue(null);
                if (visibleExchange != null) __BongoClaim_EnsureExchangeIcon_v1(visibleExchange);
            } catch (Exception ex) {
                if (Time.unscaledTime >= __BongoClaim_NextThemeProbe_v1) {
                    __BongoClaim_NextThemeProbe_v1 = Time.unscaledTime + 30f;
                    Exception cause = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
                    UnityEngine.Debug.Log("[Bongo Cat Auto Chest] Could not apply the exchange icon theme: " + cause.ToString());
                }
            }
            if (!SteamTools.Interface.IsReady) return;
            try {
                if (__BongoClaim_ExchangeInFlight_v1) {
                    Type pendingType = __BongoClaim_FindExchangeType_v1();
                    FieldInfo pendingField = pendingType == null ? null : __BongoClaim_FindField_v1(pendingType, "Instance", true);
                    object pendingExchange = pendingField == null ? null : pendingField.GetValue(null);
                    if (pendingExchange != null && !(bool)__BongoClaim_ReadField_v1(pendingExchange, "_isExchanging") && !Steam.ChestExchanger.__BongoClaim_IsBusy_v1()) {
                        __BongoClaim_ClearOwnedExchangeSlots_v1(pendingExchange);
                        __BongoClaim_ExchangeInFlight_v1 = false;
                        __BongoClaim_NextExchange_v1 = Time.unscaledTime + 3f;
                        __BongoClaim_ExchangeStatus_v1 = __BongoClaim_AutoExchange_v1 ? "ACTIVO | esperando" : "DESACTIVADO";
                    }
                }
                if (!__BongoClaim_AutoExchange_v1 || __BongoClaim_ExchangeInFlight_v1 || Time.unscaledTime < __BongoClaim_NextExchange_v1) return;
                __BongoClaim_NextExchange_v1 = Time.unscaledTime + 3f;
                if (Steam.ChestExchanger.__BongoClaim_IsBusy_v1()) { __BongoClaim_ExchangeStatus_v1 = "ACTIVO | esperando al inventario"; return; }
                Type exchangeType = __BongoClaim_FindExchangeType_v1();
                if (exchangeType == null) { __BongoClaim_ExchangeStatus_v1 = "ACTIVO | abre la colección"; return; }
                FieldInfo instanceField = __BongoClaim_FindField_v1(exchangeType, "Instance", true);
                object exchange = instanceField == null ? null : instanceField.GetValue(null);
                if (exchange == null) { __BongoClaim_ExchangeStatus_v1 = "ACTIVO | abre la colección"; return; }
                Type groupType = __BongoClaim_FindMethod_v1(exchangeType, "CanFillExchangeWithDuplicates", 2).GetParameters()[0].ParameterType;
            if (groupType == null || !groupType.IsEnum) throw new InvalidOperationException("The game's emoji exchange group is unavailable");
                object group = __BongoClaim_ResolveEmoteGroup_v1(groupType);
                __BongoClaim_StartEmojiExchange_v1(exchange, exchangeType, group);
            } catch (Exception ex) {
                try {
                    if (!__BongoClaim_ExchangeInFlight_v1 && !Steam.ChestExchanger.__BongoClaim_IsBusy_v1()) {
                        Type failedType = __BongoClaim_FindExchangeType_v1();
                        FieldInfo instanceField = failedType == null ? null : __BongoClaim_FindField_v1(failedType, "Instance", true);
                        object failedExchange = instanceField == null ? null : instanceField.GetValue(null);
                        if (failedExchange != null) __BongoClaim_ClearOwnedExchangeSlots_v1(failedExchange);
                    }
                } catch { }
                __BongoClaim_AutoExchange_v1 = false;
                __BongoClaim_ExchangeInFlight_v1 = false;
                string error = ex.Message;
                if (error.Length > 42) error = error.Substring(0, 42);
                __BongoClaim_ExchangeStatus_v1 = "DESACTIVADO | " + error;
                __BongoClaim_SetLabel_v1(__BongoClaim_ExchangeRow_v1, __BongoClaim_ExchangeLabel_v1());
                UnityEngine.Debug.Log("[Bongo Cat Auto Chest] Emoji exchange stopped: " + error);
            }
        }
        public void __BongoClaim_CreateTokenActions_v1() {
            var autoStart = UnityEngine.Object.FindObjectOfType<SetAutoStart>();
            if (!autoStart || !ShowChestPopup) return;
            var anchor = autoStart.__BongoClaim_GetRow_v1();
            var parent = anchor.transform.parent;
            if (parent.Find("BongoClaim_NormalTokens")) return;
            int index = anchor.transform.GetSiblingIndex() + 4;
            __BongoClaim_NormalTokensRow_v1 = PlayerPrefsToggle.__BongoClaim_CreateActionRow_v1(ShowChestPopup, parent, "BongoClaim_NormalTokens", "Fichas normales: cargando", "Abre una ficha normal que ya tengas, incluso durante la cuenta atrás. F11 abre una mientras el menú está abierto. No genera fichas nuevas.", __BongoClaim_ClaimNormal_v1, index);
            __BongoClaim_EmoteTokensRow_v1 = PlayerPrefsToggle.__BongoClaim_CreateActionRow_v1(ShowChestPopup, parent, "BongoClaim_EmoteTokens", "Fichas de emoji: cargando", "Abre una ficha de emoji que ya tengas, incluso durante la cuenta atrás. Insert abre una mientras el menú está abierto.", __BongoClaim_ClaimEmote_v1, index + 1);
            __BongoClaim_ScoreRow_v1 = PlayerPrefsToggle.__BongoClaim_CreateActionRow_v1(ShowChestPopup, parent, "BongoClaim_EditScore", "Editar puntuación de mascotas", "Pulsa y escribe un número entero no negativo. Intro guarda y Escape cancela. Retroceso borra. F10 también activa la edición.", __BongoClaim_EditScore_v1, index + 2);
            __BongoClaim_ExchangeRow_v1 = PlayerPrefsToggle.__BongoClaim_CreateActionRow_v1(ShowChestPopup, parent, "BongoClaim_EmojiExchange", __BongoClaim_ExchangeLabel_v1(), "Pulsa para activar o detener el intercambio automático de emojis duplicados. Intenta un intercambio cada 3 segundos y respeta los espacios que hayas rellenado manualmente.", __BongoClaim_ToggleExchange_v1, index + 3);
            __BongoClaim_NextRefresh_v1 = 0;
        }
        private static void __BongoClaim_RefreshToken_v1(GameObject row, string name, long quantity, bool ready) {
            if (!row) return;
            __BongoClaim_SetLabel_v1(row, ready ? name + ": " + quantity.ToString() + " | reclamar" : name + ": cargando");
            row.GetComponent<Button>().interactable = ready && quantity > 0 && !Steam.ChestExchanger.__BongoClaim_IsBusy_v1() && !__BongoClaim_EditingScore_v1;
        }
        public static bool __BongoClaim_FeatureTick_v1(bool menuOpen) {
            __BongoClaim_TickEmojiExchange_v1();
            bool editing = __BongoClaim_EditingScore_v1;
            if (!menuOpen) { __BongoClaim_EditingScore_v1 = false; __BongoClaim_NextRefresh_v1 = 0; }
            if (menuOpen && Application.isFocused && !Helper.InputFieldCache.FocusingInputField) {
                if (!__BongoClaim_EditingScore_v1 && Input.GetKeyDown(KeyCode.F11)) __BongoClaim_ClaimNormal_v1();
                if (!__BongoClaim_EditingScore_v1 && Input.GetKeyDown(KeyCode.Insert)) __BongoClaim_ClaimEmote_v1();
                if (!__BongoClaim_EditingScore_v1 && Input.GetKeyDown(KeyCode.F10)) __BongoClaim_EditScore_v1();
                else if (__BongoClaim_EditingScore_v1) {
                    if (Input.GetKeyDown(KeyCode.Escape)) { __BongoClaim_EditingScore_v1 = false; __BongoClaim_NextRefresh_v1 = 0; }
                    else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                        int value;
                        if (Int32.TryParse(__BongoClaim_ScoreBuffer_v1, out value) && Pets.Instance && Pets.Instance.__BongoClaim_SetScore_v1(value)) {
                            __BongoClaim_EditingScore_v1 = false; __BongoClaim_NextRefresh_v1 = 0;
                        } else { __BongoClaim_ScoreBuffer_v1 = ""; __BongoClaim_SetLabel_v1(__BongoClaim_ScoreRow_v1, "Puntuación incorrecta; vuelve a escribirla"); }
                    } else {
                        bool changed = false;
                        if (Input.GetKeyDown(KeyCode.Backspace) && __BongoClaim_ScoreBuffer_v1.Length > 0) { __BongoClaim_ScoreBuffer_v1 = __BongoClaim_ScoreBuffer_v1.Substring(0, __BongoClaim_ScoreBuffer_v1.Length - 1); changed = true; }
                        for (int digit = 0; digit <= 9; digit++) {
                            if ((Input.GetKeyDown((KeyCode)(48 + digit)) || Input.GetKeyDown((KeyCode)(256 + digit))) && __BongoClaim_ScoreBuffer_v1.Length < 10) { __BongoClaim_ScoreBuffer_v1 += ((char)('0' + digit)).ToString(); changed = true; }
                        }
                        if (changed) __BongoClaim_SetLabel_v1(__BongoClaim_ScoreRow_v1, "Puntuación: " + __BongoClaim_ScoreBuffer_v1 + "_ (Intro)");
                    }
                }
            }
            if (menuOpen && Time.unscaledTime >= __BongoClaim_NextRefresh_v1) {
                __BongoClaim_NextRefresh_v1 = Time.unscaledTime + 0.5f;
                bool ready = SteamTools.Interface.IsReady;
                __BongoClaim_RefreshToken_v1(__BongoClaim_NormalTokensRow_v1, "Fichas normales", ready ? SteamTools.Game.Inventory.Chest_Token.GetTotalQuantity() : 0, ready);
                __BongoClaim_RefreshToken_v1(__BongoClaim_EmoteTokensRow_v1, "Fichas de emoji", ready ? SteamTools.Game.Inventory.Emote_Chest_Token.GetTotalQuantity() : 0, ready);
                if (!__BongoClaim_EditingScore_v1 && Pets.Instance) __BongoClaim_SetLabel_v1(__BongoClaim_ScoreRow_v1, "Mascotas: " + Pets.Instance.__BongoClaim_GetScore_v1().ToString() + " | editar");
            }
            return menuOpen && (editing || __BongoClaim_EditingScore_v1);
        }
    }
}
namespace Steam {
    public partial class ChestExchanger {
        public static bool __BongoClaim_IsBusy_v1() { return IsExchanging; }
        public bool __BongoClaim_CanExchange_v1() { return _exchange != null && _exchange.CanExchange(); }
    }
}

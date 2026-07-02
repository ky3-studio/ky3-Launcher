using Launcher.Model.Metadata.Food;
using Launcher.UI.Xaml.Control.AutoSuggestBox;
using System.Collections.Frozen;

namespace Launcher.ViewModel.Wiki;

internal static class FoodFilter
{
    private static readonly FrozenDictionary<string, string> EffectLabelToIcon = new Dictionary<string, string>
    {
        [SH.ViewWikiFoodEffectRecoveryHp] = "UI_Buff_Item_Recovery_HpAdd",
        [SH.ViewWikiFoodEffectRecoveryHpAll] = "UI_Buff_Item_Recovery_HpAddAll",
        [SH.ViewWikiFoodEffectRevive] = "UI_Buff_Item_Recovery_Revive",
        [SH.ViewWikiFoodEffectAtkAdd] = "UI_Buff_Item_Atk_Add",
        [SH.ViewWikiFoodEffectCritRate] = "UI_Buff_Item_Atk_CritRate",
        [SH.ViewWikiFoodEffectDefAdd] = "UI_Buff_Item_Def_Add",
        [SH.ViewWikiFoodEffectSPAdd] = "UI_Buff_Item_Other_SPAdd",
        [SH.ViewWikiFoodEffectSPReduce] = "UI_Buff_Item_Other_SPReduceConsume",
        [SH.ViewWikiFoodEffectClimate] = "UI_Buff_Item_Climate_Heat",
        [SH.ViewWikiFoodEffectAdventure] = "UI_Buff_Item_Adventure",
        [SH.ViewWikiFoodEffectSpecial] = "UI_Buff_Item_SpecialEffect",
    }.ToFrozenDictionary();

    public static Predicate<Food>? Compile(SearchData? searchData)
    {
        return searchData is { FilterTokens.Count: > 0 } ? Compile(searchData.FilterTokens) : default;
    }

    public static Predicate<Food> Compile(IEnumerable<SearchToken> input)
    {
        ILookup<SearchTokenKind, string> lookup = input.ToLookup(token => token.Kind, token => token.Value);
        return food => DoFilter(lookup, food);
    }

    private static bool DoFilter(ILookup<SearchTokenKind, string> lookup, Food food)
    {
        List<bool> matches = [];

        foreach ((SearchTokenKind kind, IEnumerable<string> tokens) in lookup)
        {
            switch (kind)
            {
                case SearchTokenKind.ItemQuality:
                    matches.Add(tokens.Any(t => t.StartsWith($"{food.Rank}")));
                    break;
                case SearchTokenKind.FoodEffect:
                    matches.Add(tokens.Any(label => EffectLabelToIcon.TryGetValue(label, out string? icon) && icon == food.EffectIcon));
                    break;
                case SearchTokenKind.Food:
                    matches.Add(tokens.Contains(food.Name));
                    break;
                case SearchTokenKind.FoodCategory:
                    matches.Add(food.IsSpecialDish);
                    break;
                default:
                    matches.Add(false);
                    break;
            }
        }

        return matches.Count > 0 && matches.All(r => r);
    }
}

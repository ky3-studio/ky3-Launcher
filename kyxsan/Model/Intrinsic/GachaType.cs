namespace kyxsan.Model.Intrinsic;

[ExtendedEnum]
internal enum GachaType
{
    [LocalizationKey(nameof(SH.ModelIntrinsicGachaTypeNovice))]
    Novice = 100,

    [LocalizationKey(nameof(SH.ModelIntrinsicGachaTypeStandard))]
    Standard = 200,

    [LocalizationKey(nameof(SH.ModelIntrinsicGachaTypeAvatarEvent))]
    AvatarEvent = 301,

    [LocalizationKey(nameof(SH.ModelIntrinsicGachaTypeWeaponEvent))]
    WeaponEvent = 302,

    [LocalizationKey(nameof(SH.ModelIntrinsicGachaTypeAvatarEvent2))]
    AvatarEvent2 = 400,

    [LocalizationKey(nameof(SH.ModelIntrinsicGachaTypeChronicled))]
    Chronicled = 500,
}

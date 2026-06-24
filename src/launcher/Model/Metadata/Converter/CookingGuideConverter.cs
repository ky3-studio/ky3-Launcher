using kyxsan.UI.Xaml.Data.Converter;
using kyxsan.Web.Endpoint.kyxsan;

namespace kyxsan.Model.Metadata.Converter;

internal sealed partial class CookingGuideConverter : ValueConverter<string, Uri>
{
    public static Uri IconNameToUri(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return default!;
        }

        return StaticResourcesEndpoints.StaticRaw("CookingGuide", name).ToUri();
    }

    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}

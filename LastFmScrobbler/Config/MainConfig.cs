using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using SemVer;
using SiraUtil.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace LastFmScrobbler.Config
{
    public class MainConfig
    {
        [NonNullable, UseConverter(typeof(VersionConverter))]
        public Version Version { get; set; } = new Version("0.0.0");

        public string? SessionName { get; set; } = null;
        
        public string? SessionKey  { get; set; } = null;
    }
}
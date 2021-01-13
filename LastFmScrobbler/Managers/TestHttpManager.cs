using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public class TestHttpManager : IInitializable
    {
        [Inject] private SiraLog _log = null!;
        [Inject] private LastFmClient m = null!;

        public void Initialize()
        {
            var t = m.Authorize();
        }
    }
}
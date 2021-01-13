using SiraUtil.Tools;
using Zenject;

#pragma warning disable 8618, 649
// Disables warning: fields are assigned with Zenject.

namespace LastFmScrobbler.Managers
{
    public class TestHttpManager : IInitializable
    {
        [Inject] private SiraLog _log;
        [Inject] private LastFmManager m;

        public void Initialize()
        {
            var t = m.Authorize();
        }
    }
}
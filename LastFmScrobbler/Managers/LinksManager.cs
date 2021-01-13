using UnityEngine;

#pragma warning disable 8618, 649
// Disables warning: fields are assigned with Zenject.

namespace LastFmScrobbler.Managers
{
    public interface ILinksManager
    {
        public void OpenLink(string url);
    }

    public class LinksManager : ILinksManager
    {
        public void OpenLink(string url)
        {
            Application.OpenURL(url);
        }
    }
}
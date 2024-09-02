namespace DuksGames.Argon.Interaction
{
    public interface IOverlayEnable
    {
        void ShowHide(bool shouldShow, string overlayName);
        bool IsOverlayEnabled(string overlayName);
    }

}
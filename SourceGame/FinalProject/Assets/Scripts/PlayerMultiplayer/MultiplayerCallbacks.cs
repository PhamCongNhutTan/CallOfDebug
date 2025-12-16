using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class MultiplayerCallbacks : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)EVENT_CODE.START_FIRE)
        {
            MultiplayerManager.Instance.startTiming = true;
        }
    }
}


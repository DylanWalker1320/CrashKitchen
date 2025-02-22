using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    // public Transform XRRoot;

    // public override void OnNetworkSpawn()
    // {
    //     if (IsOwner)
    //     {
    //         XRRoot = GameObject.FindObjectOfType<XROrigin>().transform;
    //         transform.SetParent(XRRoot);
    //         transform.localPosition = Vector3.zero; // Reset position
    //     }
    // }
}

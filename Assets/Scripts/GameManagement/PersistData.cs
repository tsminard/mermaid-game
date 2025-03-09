using UnityEngine;

// script to handle persisting data between scenes
public class PersistData : MonoBehaviour
{
    // Siren interaction variables
    int sirenInteractionNumber; // indicates what level of familiarity with the siren
    SirenTypes siren; // indicates which siren we are currently talking to 

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // GETTERS + SETTERS
    // this should probably be the majority of this class
    public void setSirenInteractionNumber(int sirenInteractionNumber) { this.sirenInteractionNumber = sirenInteractionNumber; }

    public int getSirenInteractionNumber() { return sirenInteractionNumber;  }

    public void setSiren(SirenTypes siren) { this.siren = siren;  }

    public SirenTypes getSiren() { return siren;  }
}

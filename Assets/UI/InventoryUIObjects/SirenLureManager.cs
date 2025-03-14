using UnityEngine;
using System.Collections.Generic;

// class to handle correlating sirens with their lures 
public class SirenLureManager : MonoBehaviour
{
    private static Dictionary<SirenTypes, LureNote[]> sirenToLure = new Dictionary<SirenTypes, LureNote[]>();
    private static bool areLuresBuilt = false; 
    // build all siren / lure combos manually since we shouldn't randomly generate them
    // im calling this from an external class (tabbedLureUIController) to ensure that this is created BEFORE the LureInventorySlots are created 
    public static void buildSirenLures()
    {
        if (!areLuresBuilt)
        {
            // set all siren lures to 5 notes for now
            LureNote[] orcaNotes = new LureNote[] { new LureNote(KeyCode.V),
            new LureNote(KeyCode.Z),
            new LureNote(KeyCode.C),
            new LureNote(KeyCode.X),
            new LureNote(KeyCode.Z) };
            sirenToLure.Add(SirenTypes.Orca, orcaNotes);

            LureNote[] seaAngelNotes = new LureNote[] { new LureNote(KeyCode.V),
            new LureNote(KeyCode.C),
            new LureNote(KeyCode.V),
            new LureNote(KeyCode.C),
            new LureNote(KeyCode.Z) };
            sirenToLure.Add(SirenTypes.Sea_Angel, seaAngelNotes);

            LureNote[] leopardSharkNotes = new LureNote[] { new LureNote(KeyCode.Z),
            new LureNote(KeyCode.X),
            new LureNote(KeyCode.Z),
            new LureNote(KeyCode.X),
            new LureNote(KeyCode.Z) };
            sirenToLure.Add(SirenTypes.Leopard_Shark, leopardSharkNotes);

            LureNote[] morayNotes = new LureNote[] {new LureNote(KeyCode.Z),
            new LureNote(KeyCode.Z),
            new LureNote(KeyCode.V),
            new LureNote(KeyCode.Z),
            new LureNote(KeyCode.Z) };
            sirenToLure.Add(SirenTypes.Moray, morayNotes);

            areLuresBuilt = true;
        }
    }

    // HELPER METHODS
    // getters + setters
    public static LureNote[] getLureBySiren(SirenTypes sirenType)
    {
        LureNote[] lureNotes;
        sirenToLure.TryGetValue(sirenType, out lureNotes);
        if (lureNotes != null) return lureNotes;
        else
        {
            Debug.Log("Could not find lure notes for siren " + sirenType.ToString());
            return null;
        }

    }
}

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WaitingPlayerList : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI List;
    [SerializeField] private PlayerCount Count;

    private readonly string Space = "     ";

    private string[] Names;

    public void Display(string[] names)
    {
        Count.SetCount(names.Length);
        Names = names;

        string text = string.Empty;
        for (int i = 0; i < names.Length; i++)
        {
            text += names[i];
            text += Space;
        }
        List.text = text;
    }

    public bool IsTheSame(string[] names)
    {
        if (Names == null && names == null) return true;
        else if (Names == null) return false;
        else if (Names.Length != names.Length) return false;
        else
        {
            for (int i = 0; i < Names.Length; i++)
                if (Names[i] != names[i]) return false;
        }
        return true;
    }
}

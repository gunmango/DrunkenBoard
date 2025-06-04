using UnityEngine;

public class PaletteRadioButton : SimpleRadioButton
{
    [SerializeField] private EPlayerColor playerColor;
    
    public EPlayerColor PlayerColor => playerColor;
}

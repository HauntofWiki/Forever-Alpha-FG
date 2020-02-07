using UnityEngine.Rendering;

public static class Constants
{
    //prefab path constants
    public const string PrefabPlayer = "Prefabs/Characters/Player";
    
    //Universal game constants
    public const int MaxGameClock = 99;
    public const int MaxMeter = 100;
    public const int ExMeterCost = 25;
    public const int SuperMeterCost = 50;

    //Max distance 2 characters can be on the screen on the X axis
    public const float MaxDistance = 4.0f;
    //Max Size of the stage
    public const float MaxStage = 8.0f;
    //Minimum value characters can be on the Y axis
    public const float Floor = 0.0f;
    //A buffer for handling with the positions float value
    public const float FloorBuffer = 0.1f;
}
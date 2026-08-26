using UnityEngine;

public class FishScore : MonoBehaviour
{
    public enum FishSize
    {
        Small,
        Medium,
        Large
    }

    public FishSize size;

    // Returns a different score depending on the fish's size
    public int Score
    {
        get
        {
            switch (size)
            {
                case FishSize.Small:
                    return 10;

                case FishSize.Medium:
                    return 25;

                case FishSize.Large:
                    return 50;

                default:
                    return 0;
            }
        }
    }
}
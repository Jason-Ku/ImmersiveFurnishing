using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreParams
{
    public float wallScore;
    public float groupScore;
    public float spread;
    public float messiness;

    public ScoreParams(float wallScore, float groupScore, float spread, float messiness)
    {
        this.wallScore = wallScore;
        this.groupScore = groupScore;
        this.spread = spread;
        this.messiness = messiness;
    }
}

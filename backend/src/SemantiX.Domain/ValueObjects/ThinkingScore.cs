namespace SemantiX.Domain.ValueObjects;

public sealed class ThinkingScore
{
    public float Score { get; }           // 0-100 ümumi bal
    public float SpeedFactor { get; }     // Sürət amili
    public float LogicFactor { get; }     // Məntiqi keçid amili
    public float ConsistencyFactor { get; } // Ardıcıllıq amili

    public ThinkingScore(float speed, float logic, float consistency)
    {
        SpeedFactor = Math.Clamp(speed, 0f, 100f);
        LogicFactor = Math.Clamp(logic, 0f, 100f);
        ConsistencyFactor = Math.Clamp(consistency, 0f, 100f);
        Score = (SpeedFactor * 0.3f + LogicFactor * 0.4f + ConsistencyFactor * 0.3f);
    }

    public string Grade => Score switch
    {
        >= 80 => "🧠 Dahi",
        >= 60 => "⚡ Sürətli Düşünən",
        >= 40 => "🎯 Məntiqli",
        >= 20 => "🐢 Tədricən",
        _ => "🎲 Şanslı"
    };
}

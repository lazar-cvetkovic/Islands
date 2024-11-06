public static class HexMetrics
{
    /// <summary>
    /// Outer radius of the hex tile
    /// </summary>
    public const float OuterRadius = 1f;

    /// <summary>
    /// Inner radius of the of the hex tile. Set to sqrt(3)/2
    /// </summary>
    public const float InnerRadius = OuterRadius * 0.866025404f;

    /// <summary>
    /// Scale factor for height
    /// </summary>
    public const float HeightScale = 0.1f;
}

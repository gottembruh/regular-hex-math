namespace RegularHexMath
{
    /// <summary>
    /// The typical layouts and orientations for hex grids
    /// </summary>
    public enum HCellType : byte
    {
        /// <summary>
        /// Horizontal layout shoves odd rows right [odd-r]
        /// </summary>
        OddH,

        /// <summary>
        /// Horizontal layout shoves even rows right [even-r]
        /// </summary>
        EvenH,

        /// <summary>
        /// Vertical layout shoves odd columns down [odd-q]
        /// </summary>
        OddV,

        /// <summary>
        /// Vertical layout shoves even columns down [even-q]
        /// </summary>
        EvenV,
    }
}
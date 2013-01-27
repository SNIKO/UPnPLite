
namespace SV.UPnPLite
{
    /// <summary>
    ///     Stores an ordered pair of integers, which specify a <see cref="Size.Height"/> and <see cref="Size.Width"/>.
    /// </summary>
    public struct Size
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Size"/> structure from the specified dimensions.
        /// </summary>
        /// <param name="width">
        ///     The width component of the new <see cref="Size"/>.
        /// </param>
        /// <param name="height">
        ///     The height component of the new <see cref="Size"/>.
        /// </param>
        public Size(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        ///     Gets or sets the horizontal component of this <see cref="Size"/> structure.
        /// </summary>
        public int Width;

        /// <summary>
        ///     Gets or sets the vertical component of this <see cref="Size"/> structure.
        /// </summary>
        public int Height;
    }
}

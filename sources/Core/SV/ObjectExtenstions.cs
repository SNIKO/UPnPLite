
namespace SV
{
    using System;

    public static class ObjectExtenstions
    {
        public static void EnsureNotNull(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
        }
    }
}

using System;

namespace RemoteDispatcher.Annotations
{
    /// <summary>
    ///     Represents the description for the class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class DescriptionAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="DescriptionAttribute"/>.
        /// </summary>
        public DescriptionAttribute()
        { }

        /// <summary>
        ///     Initializes a new instance of <see cref="DescriptionAttribute"/>.
        /// </summary>
        /// <param name="description">The description to set.</param>
        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
    }
}
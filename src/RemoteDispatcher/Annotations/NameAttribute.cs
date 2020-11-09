using System;

namespace RemoteDispatcher.Annotations
{
    /// <summary>
    ///     Represents the name for the class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NameAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="NameAttribute"/>.
        /// </summary>
        public NameAttribute()
        { }

        /// <summary>
        ///     Initializes a new instance of <see cref="NameAttribute"/>.
        /// </summary>
        /// <param name="name">The name to set.</param>
        public NameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}
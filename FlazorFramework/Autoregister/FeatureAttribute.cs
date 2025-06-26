using System;

namespace Framework.Flazor.Autoregister
{
    /// <summary>
    /// Marks a class representing a Fluxor state object so a corresponding feature can be auto-registered for it.
    /// </summary>
    /// <remarks>
    /// AllowMultiple is false - it does not make sense to permit multiple attributes on a single class.
    /// Inherited is false - if the feature was inherited, different states might be created with the same feature name.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FeatureAttribute : Attribute
    {
        public FeatureAttribute()
        {
        }

        public FeatureAttribute(string featureName)
        {
            if (string.IsNullOrWhiteSpace(featureName))
            {
                throw new ArgumentException($"'{nameof(featureName)}' cannot be null or whitespace.", nameof(featureName));
            }

            FeatureName = featureName;
        }

        public string FeatureName { get; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StatsdClient
{
    internal class MetricSerializer
    {
        private static readonly string[] EmptyStringArray = new string[0];
        private readonly string _prefix;
        private readonly string[] _constantTags;

        internal MetricSerializer(string prefix, string[] constantTags)
        {
            _prefix = string.IsNullOrEmpty(prefix) ? string.Empty : prefix + ".";
            _constantTags = constantTags;
        }

        public static string ConcatTags(string[] constantTags, string[] tags)
        {
            // avoid dealing with null arrays
            constantTags = constantTags ?? EmptyStringArray;
            tags = tags ?? EmptyStringArray;

            if (constantTags.Length == 0 && tags.Length == 0)
            {
                return string.Empty;
            }

            var allTags = constantTags.Concat(tags);
            string concatenatedTags = string.Join(",", allTags);
            return $"|#{concatenatedTags}";
        }

        public SerializedMetric Serialize<T>(MetricType metricType, string name, T value, double sampleRate = 1.0, string[] tags = null)
        {
            return new SerializedMetric(Metric.GetCommand(metricType, _prefix, name, value, sampleRate, _constantTags, tags));
        }

        public abstract class Metric
        {
            private static readonly Dictionary<MetricType, string> _commandToUnit = new Dictionary<MetricType, string>
                                                                {
                                                                    { MetricType.Counting, "c" },
                                                                    { MetricType.Timing, "ms" },
                                                                    { MetricType.Gauge, "g" },
                                                                    { MetricType.Histogram, "h" },
                                                                    { MetricType.Distribution, "d" },
                                                                    { MetricType.Meter, "m" },
                                                                    { MetricType.Set, "s" },
                                                                };

            public static string GetCommand<T>(MetricType metricType, string prefix, string name, T value, double sampleRate, string[] constantTags, string[] tags)
            {
                string full_name = prefix + name;
                string unit = _commandToUnit[metricType];
                var allTags = ConcatTags(constantTags, tags);

                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}:{1}|{2}{3}{4}",
                    full_name,
                    value,
                    unit,
                    sampleRate == 1.0 ? string.Empty : string.Format(CultureInfo.InvariantCulture, "|@{0}", sampleRate),
                    allTags);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuksGames.Tools
{
    public class StarterWorkTicketData
    {
        public string name;
        public string playableId => this.name;
        public int playableType; // psuedo enum 
        public bool allowsInterrupts;
        public string commandBehaviourType;
        public string signalFilters;

        // defines the constant value for the case where the filter_signal_type is constant_val
        public float signalConstantValue;

        public string customInfo;

        // slash delimited path in hierarchy to obj that should own the audio source
        public string audioObjectPath;

        // target paths
        public string[] targets;

        // name of animation clip
        public string animAction;

        // name of the audio clip
        public string audioClipName;

        public bool loopAudio;

        public bool audioAlwaysForwards;

        // should enable messages apply to children
        public bool applyToChildren;

        public bool overTime;
        public string overTimeFunction;
        public bool runIndefinitely;
        public bool pickUpFromLastState;

        public float totalRangeSeconds;
        public float periodSeconds;
        public float broadcastIntervalSeconds;
        public float lowValue;
        public float highValue;
        public bool shouldClampFunction;
        public string outroBehaviour;
        public float outroSpeedMultiplier;
        public float outroThreshold;
        public float outroDestinationValue;
        public float outroDestinationValueB;

        // screen overlay
        public string overlayName;
        public bool overlayHasDuration;

        // camera shake
        public float shakeDuration;
        public float shakeDisplacementDistance;

        public bool shouldPlayAfter;
        public string playAfter;
        public string playAfterStor;
        public float playAfterAdditionalDelay;
        public bool playAfterDeferToLatest;

        public string headlineText;
        public float headlineDisplaySeconds;

        public string messageBusType;

        // composite command
        public string[] commandNames;
        public bool isSequential;

        // wait seconds
        public float waitSeconds;

        // cutscene
        public string camera;
        public bool isCancellable;
    }


    public static class _SWTExtensions
    {
        public static bool IsAnimationClipRequired(this StarterWorkTicketData _this)
        {
            return _this.playableType == 1  // Animation 
                || _this.playableType == 2  // Looping Animation
                || _this.playableType == 12; // Cutscene
        }

        public static bool HasTargets(this StarterWorkTicketData _this)
        {
            if (_this.targets == null) { return false; }
            return _this.targets.Length > 0;
        }

        public static IEnumerable<string> GetTargetNames(this StarterWorkTicketData _this)
        {
            return _this.targets.Select(path =>
            {
                var split = path.Split(_SWTExtensions.PathSeparator);
                return split[split.Length - 1];
            });
        }

        public static IEnumerable<string> GetTargetPaths(this StarterWorkTicketData _this)
        {
            return _this.targets.Select(path => path.Replace(_SWTExtensions.PathSeparator, "/"));
        }

        public static string Dump(this StarterWorkTicketData _this)
        {
            return _this.GetType().GetFields()
                .Select(info => (info.Name, Value: info.GetValue(_this) ?? "(null)"))
                .Aggregate(
                    new StringBuilder(),
                    (sb, pair) =>
                    {
                        if (pair.Value.GetType().IsArray)
                        {
                            sb.Append($"{pair.Name}: [");
                            if (pair.Value is IEnumerable enumeration)
                                foreach (var item in enumeration)
                                {
                                    sb.AppendLine($"'{item.ToString()}'");
                                }

                            return sb.AppendLine("]");
                        }

                        return sb.AppendLine($"{pair.Name}: {pair.Value.ToString()} ");
                    },
                    sb => sb.ToString());
        }

        public static string PathSeparator => "~~&&&~~"; // no one will ever use such a strange sequence in their object names
    }

}
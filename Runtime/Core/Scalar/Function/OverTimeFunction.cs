using System.Linq;
using DuksGames.Argon.Utils;
using UnityEngine;

namespace DuksGames.Argon.Core
{

    [System.Serializable]
    public struct OverTimeFunctionIngredients
    {
        public OverTimeFunctionParameters Parameters;
        public OverTimeFunctionType Type;
    }

    [System.Serializable]
    public struct OverTimeFunctionParameters
    {
        /// <summary>
        /// For a linear function: the y value at x=0
        /// </summary>
        public float LeftValue;

        /// <summary>
        /// For a linear function: the y value at x=PeriodSeconds
        /// </summary>
        public float RightValue;

        /// <summary>
        /// Meaningless parameter for function classes (and TODO: can we splice / purge this var out of this struct)
        ///  This is used to calculate the ping count (divide by TickInterval) and also for High-then-low overtime commands.
        ///    So, all of this is symptomatic of an awkward fit: this is a handy grab-bag parameter object--and convenient for conveying the data from blender
        ///      but confusing when you're trying to see how these function classes work.
        /// </summary>
        public float TotalRangeSeconds;

        /// <summary>
        /// For a linear function: the value p such that f(p) = RightValue.
        ///  For periodic functions: the period width. 
        /// </summary>
        public float PeriodSeconds;

        /// <summary>
        /// Meaningless for function classes. Represents the time in seconds between broadcasts.
        /// </summary>
        public float TickIntervalSeconds;

        /// <summary>
        ///  If true, clamp the output of the function between LeftValue and RightValue. 
        /// </summary>
        public bool ClampOutput;
    }

    public static class OverTimeFunctionParametersExtensions
    {
        public static float LesserValue(this OverTimeFunctionParameters parameters)
        {
            return parameters.LeftValue > parameters.RightValue ? parameters.RightValue : parameters.LeftValue;
        }
        public static float GreaterValue(this OverTimeFunctionParameters parameters)
        {
            return parameters.LeftValue > parameters.RightValue ? parameters.LeftValue : parameters.RightValue;
        }
    }

    // Must match the equivalent enum in the blender script
    public enum OverTimeFunctionType
    {
        StartValueEndValue,
        SawTooth,
        Linear
    }

    public static class FunctionFactory
    {

        public static AbstractFunction CreateAndInit(OverTimeFunctionIngredients ingredients)
        {
            var f = FunctionFactory.CreateFrom(ingredients.Type);
            f.Setup(ingredients.Parameters);
            return ModifiedFunctionFactory.CreateModified(f, new FunctionOutputModifierSettings
            {
                ShouldClamp = ingredients.Parameters.ClampOutput,
                ClampLower = ingredients.Parameters.LesserValue(),
                ClampUpper = ingredients.Parameters.GreaterValue()
            });
        }

        static AbstractFunction CreateFrom(OverTimeFunctionType functionType)
        {
            switch (functionType)
            {
                case OverTimeFunctionType.SawTooth:
                    return new SawToothFunction();
                case OverTimeFunctionType.Linear:
                    return new LinearFunction();
                default:
                    return new LinearFunction();
                    // throw new System.ArgumentException($"{functionType} not supported"); // WANT but test
            }
        }

        public static FunctionBounds BoundsFrom(OverTimeFunctionParameters parameters)
        {
            return new FunctionBounds
            {
                Upper = parameters.RightValue,
                Lower = parameters.LeftValue,
            };
        }
    }

    public class FunctionBounds
    {
        public float Upper;
        public float Lower;

        public float GetSize() { return this.Upper - this.Lower; }
    }


    public abstract class AbstractFunction
    {
        public OverTimeFunctionParameters Parameters;

        public abstract float GetSignal(float time);

        public virtual void Setup(OverTimeFunctionParameters parameters)
        {
            this.Parameters = parameters;
        }
    }

    public class LinearFunction : AbstractFunction
    {
        public override float GetSignal(float time)
        {
            return time * (this.Parameters.RightValue - this.Parameters.LeftValue) / this.Parameters.PeriodSeconds + this.Parameters.LeftValue;
        }

    }

    public abstract class PeriodicFunction : AbstractFunction
    {
    }

    public class SawToothFunction : PeriodicFunction
    {

        public override float GetSignal(float time)
        {
            return time % this.Parameters.PeriodSeconds / this.Parameters.PeriodSeconds * (this.Parameters.RightValue - this.Parameters.LeftValue) + this.Parameters.LeftValue;
        }

    }

    public class ClampFunction : AbstractFunction
    {
        public float Upper;
        public float Low;

        public override float GetSignal(float time)
        {
            return Mathf.Clamp(time, this.Low, this.Upper);
        }

    }

    public class CompositeFunction : AbstractFunction
    {
        public AbstractFunction[] Functions;

        public override float GetSignal(float time)
        {
            foreach (var f in this.Functions)
                time = f.GetSignal(time);

            return time;
        }

    }
}
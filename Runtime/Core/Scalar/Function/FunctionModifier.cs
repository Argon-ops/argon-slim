
namespace DuksGames.Argon.Core
{
    public struct FunctionOutputModifierSettings
    {
        public bool ShouldClamp;
        public float ClampLower;
        public float ClampUpper;
    }

    public static class ModifiedFunctionFactory
    {
        public static AbstractFunction CreateModified(
            AbstractFunction baseFunction,
            FunctionOutputModifierSettings settings)
        {

            if (settings.ShouldClamp)
            {
                var clamp = new ClampFunction
                {
                    Upper = settings.ClampUpper,
                    Low = settings.ClampLower
                };
                var composite = new CompositeFunction
                {
                    Functions = new AbstractFunction[] {
                        baseFunction,
                        clamp
                    }
                };
                return composite;
            }

            return baseFunction;
        }
    }



}
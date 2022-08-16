global using static HavokActorTool.Common.RuntimeConfig;

namespace HavokActorTool.Common
{
    public class RuntimeConfig
    {
        public static DelegatePrint Print { get; set; } = CLI.Print;
        public static DelegateBoolInput BoolInput { get; set; } = CLI.BoolInput;
        public static DelegateInput Input { get; set; } = CLI.Input;
    }
}

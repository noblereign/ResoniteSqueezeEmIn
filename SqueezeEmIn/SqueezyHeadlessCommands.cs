#if RESONITE_HEADLESS
using HarmonyLib;
using FrooxEngine.Headless;

namespace SqueezeEmIn {
	class SqueezyHeadlessCommands {
		internal static void InitHeadlessCommands(Harmony harmony) {
			var target = typeof(HeadlessCommands).GetMethod(nameof(HeadlessCommands.SetupCommonCommands));
			var postfix = typeof(SqueezyHeadlessCommands).GetMethod(nameof(SetupSqueezeCommands));

			harmony.Patch(target, postfix: new HarmonyMethod(method: postfix));
		}

		public static void SetupSqueezeCommands(CommandHandler handler) {
			SqueezeEmIn.Msg("Registering new console commands");
			handler.RegisterCommand(new GenericCommand("squeeze", "Allow somebody to bypass the user limit", "<user> <true/false>", async (h, world, args) => {
				(bool, string) result = await SqueezyCommandHandler.RunSqueezeCommand(args);
				if (result.Item1) {
					SqueezeEmIn.Msg(result.Item2);
				} else {
					SqueezeEmIn.Warn(result.Item2);
				}
			}));
		}
	}
}
#endif

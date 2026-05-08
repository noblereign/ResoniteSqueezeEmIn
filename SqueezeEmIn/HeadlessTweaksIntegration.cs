#if RESONITE_HEADLESS
using HeadlessTweaks;
using SkyFrost.Base;

// heavily based on https://github.com/GrandtheUK/HeadlessAllowList/blob/main/HeadlessAllowList/ChatCommands.cs
namespace SqueezeEmIn;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
internal class HeadlessTweaksIntegration {
	internal static void InitHeadlessTweaks() {
		MessageCommands.RegisterCommands(typeof(HeadlessTweaksIntegration));
	}

	[MessageCommands.Command("squeeze", "Allow a user to bypass the user limit", "Moderation", PermissionLevel.Moderator, usage: "[user] [true/false]")]
	internal static async void SqueezeCommand(UserMessages userMessages, Message msg, string[] args) {
		if (args.Length < 2) {
			_ = userMessages.SendTextMessage("Usage: /squeeze [user] [true/false]");
			return;
		}

		(bool, string) result = await SqueezyCommandHandler.RunSqueezeCommand(args.ToList());
		if (result.Item1) {
			SqueezeEmIn.Msg(result.Item2);
		} else {
			SqueezeEmIn.Warn(result.Item2);
		}
	}
}
#endif

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using FrooxEngine;

using HarmonyLib;
using ResoniteModLoader;

namespace SqueezeEmIn;
//More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class SqueezeEmIn : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.0"; //Changing the version here updates it in all locations needed
	public override string Name => "SqueezeEmIn";
	public override string Author => "Noble";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/noblereign/ResoniteSqueezeEmIn";

	public static ModConfiguration config;

	[AutoRegisterConfigKey]
	private static ModConfigurationKey<bool> Enabled = new("Enabled", "Enables the mod.", () => true);

	[AutoRegisterConfigKey]
	private static ModConfigurationKey<string> UserIDsCommaSeperated = new("User IDs", "A comma-seperated list of User IDs to always let into your sessions, regardless of user cap.", () => "");

	[AutoRegisterConfigKey]
	private static ModConfigurationKey<List<string>> UserIDsArray = new(
		"User ID Array", "An array of User IDs to always let into your sessions, regardless of user cap.\n\n(Array is mainly intended for headlesses, as it'll look much cleaner in the config file.)", () => []
	);

	public override void OnEngineInit() {
		config = GetConfiguration();
		config.Save(true);

		Harmony harmony = new("dog.glacier.SqueezeEmIn");
		harmony.PatchAll();

		#if RESONITE_HEADLESS
		SqueezyHeadlessCommands.InitHeadlessCommands(harmony);
		Engine.Current.OnReady += () => { // https://github.com/GrandtheUK/HeadlessAllowList/blob/main/HeadlessAllowList/HeadlessAllowList.cs#L52-L61
			IEnumerable<ResoniteModBase> mods = ModLoader.Mods();
			if (mods.Any(mod => mod.Name == "HeadlessTweaks")) {
				HeadlessTweaksIntegration.InitHeadlessTweaks();
				Msg("Added command to HeadlessTweaks");
			} else {
				Warn("HeadlessTweaks not found. Chat commands unavailable");
			}
		};
		#endif
	}

	public static bool SqueezeUser(string userId, bool allowSqueeze) {
		List<string> userIdArrayList = SqueezeEmIn.config.GetValue(SqueezeEmIn.UserIDsArray) ?? [];
		if (allowSqueeze) {
			if (userIdArrayList.Contains(userId)) return false;
			userIdArrayList.Add(userId);
		} else {
			if (!userIdArrayList.Contains(userId)) return false;
			userIdArrayList.RemoveAll(item => item == userId);
		}
		config?.Set(UserIDsArray, userIdArrayList);
		config?.Save();
		return true;
	}

	[HarmonyPatch(typeof(World), nameof(World.VerifyJoinRequest), MethodType.Async)]
	public class World_VerifyJoinRequest_Patch {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod) {
			var lookFor = AccessTools.PropertyGetter(typeof(World), nameof(World.HasFreeUserSpots));
			
			var customMethod = AccessTools.Method(typeof(World_VerifyJoinRequest_Patch), nameof(CustomHasFreeUserSpotsCheck));

			var stateMachineType = originalMethod.DeclaringType;
			var connectionField = AccessTools.Field(stateMachineType, "connection");

			bool patched = false;

			foreach (var instruction in instructions) {
				// Does the instruction call the function we want to replace?
				bool isTargetMethod = instruction.Calls(lookFor);

				if (!patched && isTargetMethod) {
					// 1. The original code JUST pushed 'World' onto the stack.
					// Stack: [ ..., World ]

					// 2. Push the state machine instance ('this') onto the stack.
					yield return new(OpCodes.Ldarg_0);
					// Stack: [ ..., World, StateMachineInstance ]

					// 3. Load the 'connection' field from the state machine instance.
					yield return new(OpCodes.Ldfld, connectionField);
					// Stack: [ ..., World, SessionConnection ]

					// 4. Call our custom method! It consumes BOTH arguments and returns a bool, 
					// keeping the IL stack perfectly balanced for the upcoming branch instruction.
					yield return new(OpCodes.Call, customMethod);

					patched = true;
					Msg("Ready to squeeze 'em in!");
				} else {
					// this isn't what we're looking for... keep walking bro
					yield return instruction;
				}
			}

			if (!patched) {
				Warn("Patch was never applied! A Resonite update might've broken the mod </3 No squeezing will happen today </3");
			}
		}

		public static bool CustomHasFreeUserSpotsCheck(World world, SessionConnection connection) {
			if (config.GetValue(Enabled)) {
				string userIdStringList = config.GetValue(UserIDsCommaSeperated) ?? "";
				List<string> userIdArrayList = config.GetValue(UserIDsArray) ?? [];

				if (!string.IsNullOrWhiteSpace(connection.UserID) && (userIdArrayList.Contains(connection.UserID) || userIdStringList.Split(',').Select(str => str.Trim()).Contains(connection.UserID))) {
					return true; // this person is a VIP!!! SQUEEZE EM IN!!!
				}
			}

			return world.HasFreeUserSpots;
		}
	}
}

#if RESONITE_HEADLESS
using FrooxEngine;
using SkyFrost.Base;

namespace SqueezeEmIn {
	class SqueezyCommandHandler {
		public async static Task<string> TryGetUserId(string value, bool allowAnyUserId = true, bool onlyLookupContacts = false) { // https://github.com/New-Project-Final-Final-WIP/HeadlessTweaks/blob/master/HeadlessTweaks/MsgCommands/Utilities.cs#L26
			if (allowAnyUserId && IdUtil.GetOwnerType(value.ToUpper()) == OwnerType.User) return value;

			var contact = Engine.Current.Cloud.Contacts.FindContact((Contact f) => f.ContactUsername.Equals(value, StringComparison.InvariantCultureIgnoreCase));

			if (contact != null)
				return contact.ContactUserId;

			if (onlyLookupContacts) return null;

			var user = await Engine.Current.Cloud.Users.GetUserByName(value);
			if (user.IsOK)
				return user.Entity.Id;

			return null;
		}

		async public static Task<(bool success, string message)> RunSqueezeCommand(List<string> args) {
			if (args.Count != 2) {
				return (false, "Please include a user and whether to allow them through or not");
			}

			var user = args[0];
			var permission = args[1];

			var userId = await TryGetUserId(user);

			if (userId == null) {
				return (false, $"Could not find user '{user}', use a user id to override this check");
			}

			if (bool.TryParse(permission, out bool doSqueeze)) {

				bool succeeded = SqueezeEmIn.SqueezeUser(userId, doSqueeze);

				if (succeeded) {
					return (true, doSqueeze ? $"{userId} can now bypass maximum user limits on this headless" : $"{userId} has been removed from the squeezing list");
				} else {
					return (false, doSqueeze ? $"{userId} is already on the squeezing list!" : $"{userId} isn't on the squeezing list yet!");
				}
			} else {
				return (false, "Second argument invalid! Use 'true' or 'false' please.");
			}
		}
	}
}
#endif

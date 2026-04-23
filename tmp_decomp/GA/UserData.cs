using Unity.XGamingRuntime;

namespace GA;

public struct UserData
{
	public XUserHandle UserHandle;

	public XUserLocalId LocalId;

	public ulong UserXUID;

	public string UserGamertag;

	public bool UserIsGuest;

	public XblPermissionCheckResult CanPlayMultiplayer;

	public ulong[] AvoidList;

	public ulong[] MuteList;

	public byte[] ImageBuffer;

	public XblContextHandle XblContext;
}

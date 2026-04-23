using System;
using System.Collections.Generic;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;
using UnityEngine;

namespace GA;

internal class GamecoreUserManager
{
	public enum UserOpResult
	{
		Success,
		NoDefaultUser,
		ResolveUserIssueRequired,
		UnclearedVetoes,
		UnknownError
	}

	private enum State
	{
		Initializing,
		GetContext,
		WaitForAddingUser,
		GetBasicInfo,
		InitializeNetwork,
		GrabAchievements,
		UserDisplayImage,
		ReturnMuteList,
		ReturnAvoidList,
		UserPermissionsCheck,
		WaitForNextTask,
		Error,
		Idle,
		End
	}

	public delegate void AddUserCompletedDelegate(UserOpResult result);

	public List<UserData> UserDataList = new List<UserData>();

	private State m_State = State.Idle;

	private UserData m_CurrentUserData;

	private AddUserCompletedDelegate m_CurrentCompletionDelegate;

	private XUserChangeRegistrationToken m_CallbackRegistrationToken;

	public UserData CurrentUserData => m_CurrentUserData;

	public event EventHandler<XUserChangeEvent> UsersChanged;

	public event EventHandler<UserData> UserInfoUpdated;

	public GamecoreUserManager()
	{
		SDK.XUserRegisterForChangeEvent(UserChangeEventCallback, out m_CallbackRegistrationToken);
	}

	~GamecoreUserManager()
	{
		if (UserDataList != null)
		{
			foreach (UserData userData in UserDataList)
			{
				if (userData.XblContext != null)
				{
					SDK.XBL.XblContextCloseHandle(userData.XblContext);
				}
				if (userData.UserHandle != null)
				{
					SDK.XUserCloseHandle(userData.UserHandle);
				}
			}
			UserDataList.Clear();
		}
		SDK.XUserUnregisterForChangeEvent(m_CallbackRegistrationToken);
	}

	public void Update()
	{
		switch (m_State)
		{
		case State.GetContext:
			GetUserContext();
			break;
		case State.GetBasicInfo:
			GetBasicInfo();
			break;
		case State.UserDisplayImage:
			m_State = State.WaitForNextTask;
			GetUserImage();
			break;
		case State.ReturnMuteList:
			m_State = State.WaitForNextTask;
			GetMuteList();
			break;
		case State.ReturnAvoidList:
			m_State = State.WaitForNextTask;
			GetAvoidList();
			break;
		case State.UserPermissionsCheck:
			m_State = State.WaitForNextTask;
			GetUserMultiplayerPermissions();
			break;
		case State.Error:
			m_CurrentCompletionDelegate(UserOpResult.UnknownError);
			m_CurrentCompletionDelegate = null;
			m_State = State.Idle;
			break;
		case State.End:
			UserDataList.Add(m_CurrentUserData);
			m_CurrentCompletionDelegate(UserOpResult.Success);
			m_CurrentCompletionDelegate = null;
			m_State = State.Idle;
			break;
		case State.WaitForAddingUser:
		case State.InitializeNetwork:
		case State.GrabAchievements:
		case State.WaitForNextTask:
		case State.Idle:
			break;
		}
	}

	public bool AddDefaultUserSilently(AddUserCompletedDelegate completionDelegate)
	{
		if (m_State != State.Idle)
		{
			return false;
		}
		m_State = State.WaitForAddingUser;
		m_CurrentUserData = default(UserData);
		m_CurrentCompletionDelegate = completionDelegate;
		SDK.XUserAddAsync(XUserAddOptions.AddDefaultUserSilently, delegate(int hresult, XUserHandle userHandle)
		{
			if (hresult == 0 && userHandle != null)
			{
				Debug.Log("[GamecoreUserManager] AddUser complete " + hresult + " user handle " + userHandle.GetHashCode());
				UserOpResult userId = GetUserId(userHandle);
				switch (userId)
				{
				case UserOpResult.ResolveUserIssueRequired:
					ResolveSigninIssueWithUI(userHandle, m_CurrentCompletionDelegate);
					break;
				default:
					m_CurrentCompletionDelegate(userId);
					break;
				case UserOpResult.Success:
					m_State = State.GetContext;
					break;
				}
			}
			else if (hresult == -1994108666)
			{
				m_State = State.Idle;
				m_CurrentCompletionDelegate(UserOpResult.NoDefaultUser);
			}
			else
			{
				m_State = State.Idle;
				m_CurrentCompletionDelegate(UserOpResult.UnknownError);
			}
		});
		return true;
	}

	public bool AddUserWithUI(AddUserCompletedDelegate completionDelegate)
	{
		if (m_State != State.Idle)
		{
			return false;
		}
		m_State = State.WaitForAddingUser;
		m_CurrentUserData = default(UserData);
		m_CurrentCompletionDelegate = completionDelegate;
		SDK.XUserAddAsync(XUserAddOptions.None, delegate(int hresult, XUserHandle userHandle)
		{
			if (hresult == 0 && userHandle != null)
			{
				Debug.Log("[GamecoreUserManager] AddUserWithUI complete " + hresult + " user handle " + userHandle.GetHashCode());
				UserOpResult userId = GetUserId(userHandle);
				switch (userId)
				{
				case UserOpResult.ResolveUserIssueRequired:
					ResolveSigninIssueWithUI(userHandle, m_CurrentCompletionDelegate);
					break;
				default:
					m_CurrentCompletionDelegate(userId);
					break;
				case UserOpResult.Success:
					m_State = State.GetContext;
					break;
				}
			}
			else if (userHandle != null)
			{
				ResolveSigninIssueWithUI(userHandle, m_CurrentCompletionDelegate);
			}
			else
			{
				Debug.LogError("[GamecoreUserManager] Got empty user handle back from AddUserWithUI.");
				m_State = State.Idle;
				m_CurrentCompletionDelegate(UserOpResult.UnknownError);
			}
		});
		return true;
	}

	private void ResolveSigninIssueWithUI(XUserHandle userHandle, AddUserCompletedDelegate completionDelegate)
	{
		SDK.XUserResolveIssueWithUiUtf16Async(userHandle, null, delegate(int resolveHResult)
		{
			if (resolveHResult == 0)
			{
				GetUserId(userHandle);
				m_State = State.GetContext;
			}
			else
			{
				completionDelegate(UserOpResult.UnclearedVetoes);
				m_State = State.Idle;
			}
		});
	}

	private UserOpResult GetUserId(XUserHandle userHandle)
	{
		ulong userId;
		int num = SDK.XUserGetId(userHandle, out userId);
		if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(num))
		{
			m_CurrentUserData.UserHandle = userHandle;
			m_CurrentUserData.UserXUID = userId;
			return UserOpResult.Success;
		}
		if (num == -1994108670)
		{
			return UserOpResult.ResolveUserIssueRequired;
		}
		return UserOpResult.UnknownError;
	}

	private void GetBasicInfo()
	{
		int hr = SDK.XUserGetGamertag(m_CurrentUserData.UserHandle, XUserGamertagComponent.Classic, out m_CurrentUserData.UserGamertag);
		if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
		{
			hr = SDK.XUserGetIsGuest(m_CurrentUserData.UserHandle, out m_CurrentUserData.UserIsGuest);
		}
		if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
		{
			hr = SDK.XUserGetLocalId(m_CurrentUserData.UserHandle, out m_CurrentUserData.LocalId);
			foreach (UserData userData in UserDataList)
			{
				if (userData.LocalId.Value == m_CurrentUserData.LocalId.Value)
				{
					m_State = State.End;
					return;
				}
			}
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				m_State = State.End;
			}
			else
			{
				Debug.LogError("[GamecoreUserManager] Failed to grab gamertag and guest status, hresult: " + hr);
				m_State = State.End;
			}
			this.UserInfoUpdated?.Invoke(this, m_CurrentUserData);
		}
		else
		{
			Debug.LogError("[GamecoreUserManager] Failed to grab gamertag, hresult: " + hr);
			m_State = State.End;
		}
	}

	private void GetUserImage()
	{
		SDK.XUserGetGamerPictureAsync(m_CurrentUserData.UserHandle, XUserGamerPictureSize.Large, delegate(int hresult, byte[] buffer)
		{
			if (hresult == 0)
			{
				m_CurrentUserData.ImageBuffer = buffer;
				m_State = State.End;
			}
			else
			{
				Debug.LogError("[GamecoreUserManager] Failed to grab image, hresult: " + hresult);
				m_State = State.Error;
			}
		});
	}

	private void GetMuteList()
	{
		SDK.XBL.XblPrivacyGetMuteListAsync(m_CurrentUserData.XblContext, delegate(int hresult, ulong[] xuids)
		{
			if (hresult == 0)
			{
				m_CurrentUserData.MuteList = xuids;
				m_State = State.ReturnAvoidList;
			}
			else
			{
				m_State = State.Error;
			}
		});
	}

	private void GetAvoidList()
	{
		SDK.XBL.XblPrivacyGetAvoidListAsync(m_CurrentUserData.XblContext, delegate(int hresult, ulong[] xuids)
		{
			if (hresult == 0)
			{
				m_CurrentUserData.AvoidList = xuids;
				m_State = State.UserPermissionsCheck;
			}
			else
			{
				m_State = State.Error;
			}
		});
	}

	private void GetUserMultiplayerPermissions()
	{
		SDK.XBL.XblPrivacyCheckPermissionAsync(m_CurrentUserData.XblContext, XblPermission.PlayMultiplayer, m_CurrentUserData.UserXUID, delegate(int hresult, XblPermissionCheckResult result)
		{
			if (hresult == 0)
			{
				m_CurrentUserData.CanPlayMultiplayer = result;
				m_State = State.End;
			}
			else
			{
				Debug.LogError("[GamecoreUserManager] Failed to get user multiplayer permission");
				m_State = State.Error;
			}
		});
	}

	private void GetUserContext()
	{
		if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(SDK.XBL.XblContextCreateHandle(m_CurrentUserData.UserHandle, out m_CurrentUserData.XblContext)) && m_CurrentUserData.XblContext != null)
		{
			Debug.Log("[GamecoreUserManager] Success XBL and Context");
			m_State = State.GetBasicInfo;
		}
		else
		{
			Debug.LogError("[GamecoreUserManager] Error creating context");
			m_State = State.Error;
		}
	}

	private void UserChangeEventCallback(IntPtr _, XUserLocalId userLocalId, XUserChangeEvent eventType)
	{
		if (eventType == XUserChangeEvent.SignedOut)
		{
			Debug.LogWarning("[GamecoreUserManager] User logging out");
			foreach (UserData userData in UserDataList)
			{
				if (userData.LocalId.Value == userLocalId.Value)
				{
					UserDataList.Remove(userData);
					break;
				}
			}
		}
		if (eventType != XUserChangeEvent.SignedInAgain)
		{
			this.UsersChanged?.Invoke(this, eventType);
		}
	}
}

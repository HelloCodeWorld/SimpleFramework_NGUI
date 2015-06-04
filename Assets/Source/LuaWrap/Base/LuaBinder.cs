﻿using System;

public static class LuaBinder
{
	public static void Bind(IntPtr L)
	{
		objectWrap.Register(L);
		ObjectWrap.Register(L);
		TypeWrap.Register(L);
		DelegateWrap.Register(L);
		IEnumeratorWrap.Register(L);
		EnumWrap.Register(L);
		StringWrap.Register(L);
		MsgPacketWrap.Register(L);
		AnimationBlendModeWrap.Register(L);
		AnimationClipWrap.Register(L);
		AnimationStateWrap.Register(L);
		AnimationWrap.Register(L);
		ApplicationWrap.Register(L);
		AssetBundleWrap.Register(L);
		AsyncOperationWrap.Register(L);
		AudioClipWrap.Register(L);
		AudioSourceWrap.Register(L);
		BehaviourWrap.Register(L);
		BlendWeightsWrap.Register(L);
		BoxColliderWrap.Register(L);
		CameraClearFlagsWrap.Register(L);
		CameraWrap.Register(L);
		CharacterControllerWrap.Register(L);
		ColliderWrap.Register(L);
		ComponentWrap.Register(L);
		com_junfine_simpleframework_BaseLuaWrap.Register(L);
		com_junfine_simpleframework_ByteBufferWrap.Register(L);
		com_junfine_simpleframework_ConstWrap.Register(L);
		com_junfine_simpleframework_iooWrap.Register(L);
		com_junfine_simpleframework_LuaHelperWrap.Register(L);
		com_junfine_simpleframework_manager_NetworkManagerWrap.Register(L);
		com_junfine_simpleframework_manager_PanelManagerWrap.Register(L);
		com_junfine_simpleframework_manager_ResourceManagerWrap.Register(L);
		com_junfine_simpleframework_manager_TimerManagerWrap.Register(L);
		com_junfine_simpleframework_UIWrapGridWrap.Register(L);
		com_junfine_simpleframework_UtilWrap.Register(L);
		DebuggerWrap.Register(L);
		GameObjectWrap.Register(L);
		InputWrap.Register(L);
		KeyCodeWrap.Register(L);
		LightTypeWrap.Register(L);
		LightWrap.Register(L);
		LuaEnumTypeWrap.Register(L);
		MaterialWrap.Register(L);
		MeshColliderWrap.Register(L);
		MeshRendererWrap.Register(L);
		MonoBehaviourWrap.Register(L);
		MotionWrap.Register(L);
		ParticleAnimatorWrap.Register(L);
		ParticleEmitterWrap.Register(L);
		ParticleRendererWrap.Register(L);
		PhysicsWrap.Register(L);
		PlaneWrap.Register(L);
		PlayModeWrap.Register(L);
		QualitySettingsWrap.Register(L);
		QueueModeWrap.Register(L);
		RendererWrap.Register(L);
		RenderSettingsWrap.Register(L);
		RenderTextureWrap.Register(L);
		ScreenWrap.Register(L);
		SkinnedMeshRendererWrap.Register(L);
		SleepTimeoutWrap.Register(L);
		SpaceWrap.Register(L);
		SphereColliderWrap.Register(L);
		TextureWrap.Register(L);
		TimeWrap.Register(L);
		TouchPhaseWrap.Register(L);
		TrackedReferenceWrap.Register(L);
		TransformWrap.Register(L);
		UIEventListenerWrap.Register(L);
		UIGridWrap.Register(L);
		UILabelWrap.Register(L);
		UIPanelWrap.Register(L);
		UIRectWrap.Register(L);
		UIWidgetContainerWrap.Register(L);
		UIWidgetWrap.Register(L);
		YieldInstructionWrap.Register(L);
	}
}

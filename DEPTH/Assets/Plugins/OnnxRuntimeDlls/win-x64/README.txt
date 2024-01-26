These dll files have to be in DEPTH/Assets/Plugins/OnnxRuntimeDlls/win-x64
They are in the nuget package files (.nupkg), get them from
	https://www.nuget.org/packages/Microsoft.ML.OnnxRuntime.Managed/
		[THE NUPKG FILE]/lib/netstandard1.1/*.dll
	https://www.nuget.org/packages/Microsoft.ML.OnnxRuntime.Gpu/
		[THE NUPKG FILE]/runtimes/win-x64/native/*.dll

	From Microsoft.ML.OnnxRuntime.Gpu
		onnxruntime.dll
		onnxruntime_providers_shared.dll
		onnxruntime_providers_cuda.dll
		onnxruntime_providers_tensorrt.dll (i don't think that this is needed)

	From Microsoft.ML.OnnxRuntime.Managed
		Microsoft.ML.OnnxRuntime.dll

Refer to OnnxRuntimeDepthModelBehavior.cs


-----------------------------------------------------------------------------------

MODIFYING THE RENDERING PIPELINE IN UNITY IF YOU DO NOT HAVE A MIXED-REALITY HEADSET

If you do not have a Mixed-Reality (MR) viewer compatible with this experience, you can modify the rendering pipeline to recompile shaders and DepthPlane visualization.

Currently the project is set up to support the Varjo XR3 MR viewer in the 'MainWS' scene, in fact you can see the supporting camera in the unity object hierarchy, which is called XRRig.

In the 'MainWS_clone' scene, support for the MR viewer is not present if you do not have a viewer, but to run the project and display the DepthMap correctly, we recommend recompiling without the High Definition Rendering Pipeline (HDRP) in Unity.

To do this, follow the steps below:
From Unity's Tools menu at the top, select: Edit -> Prpject Settigs.
In the window that opens, from the menu on the left, select 'XR Plug-in Management' and disable 'Varjo':
[XR Plug-in Management screen with Varjo disabled].

Still in the project settings, select the Graphics item and change 'Writable rendering pipeline settings' to 'None':
[Graphics Settings screen].


Next, Select the game object 'DepthModel' from the hierarchy of game objects in Unity, and in the inspector select the material.
In the drop-down menu of 'Shader' select 'Standard'.


Done!
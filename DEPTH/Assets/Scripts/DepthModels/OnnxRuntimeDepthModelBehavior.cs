#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
//#define USING_ONNX_RUNTIME

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Microsoft.ML.OnnxRuntime;
//using Microsoft.ML.OnnxRuntime.Gpu;
using Microsoft.ML.OnnxRuntime.Tensors;

/*
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

I think it would work in the linux build if you get the .so files in linux-64 directory

Used https://github.com/lewiji/godot-midas-depth/blob/master/src/Inference/InferImageDepth.cs as reference
	MIT License

	Copyright (c) 2022 Lewis James

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/

public class OnnxRuntimeDepthModel : DepthModel {
	public string ModelType {get; private set;}

	private InferenceSession _infsession;
	private int _width, _height;
	private string _inputname;
	private int _outwidth, _outheight;

	private RenderTexture _rt;
	private float[] _output;

	public OnnxRuntimeDepthModel(string onnxpath, string modelType, string provider=null, int gpuid=0, string settings=null) {
		//param settings: used for TVM and OpenVINO

		ModelType = modelType;
		if (settings == null) settings = "";
		if (provider == null) provider = "default";

		Debug.Log($"OnnxRuntimeDepthModel(): using the provider {provider}");

		SessionOptions sessionOptions = new SessionOptions();
		switch (provider.ToLower()) {
		case "":
		case "default":
			Debug.Log("OnnxRuntime may not use GPU. Try other GPU execution provider.");
			break;
		
		case "cuda":
			Debug.Log($"Using gpuid={gpuid}");
			sessionOptions = SessionOptions.MakeSessionOptionWithCudaProvider(gpuid);
			break;

		case "openvino":
			Debug.Log($"settings (default empty string): \"{settings}\"");
			sessionOptions.AppendExecutionProvider_OpenVINO();
			break;

		case "directml": //Not tested
			Debug.Log($"Using gpuid={gpuid}");
			sessionOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
			sessionOptions.AppendExecutionProvider_DML(gpuid);
			break;

		case "tvm": //Not tested
			/*
			These `settings` crashes the program:
				"input_shapes:[1 3 256 256]"
				"executer:graph, input_names:0, input_shapes:[1 3 256 256]"
			*/
			//settings = "input_names:0, input_shapes:[1 3 256 256]";
			Debug.Log($"settings: \"{settings}\"");
			sessionOptions = SessionOptions.MakeSessionOptionWithTvmProvider(settings);
			break;

		case "rocm": //Not tested
			Debug.Log($"Using gpuid={gpuid}");
			sessionOptions = SessionOptions.MakeSessionOptionWithRocmProvider(gpuid);
			break;
		
		default:
			Debug.LogError($"Unknown provider: {provider}");
			break;
		}

		try {
			_infsession = new InferenceSession(onnxpath, sessionOptions);
		}
		catch (OnnxRuntimeException exc) {
			Debug.LogWarning($"OnnxRuntimeException, provider: {provider} => {exc}");
			throw new InvalidOperationException();
		}
		
		foreach (KeyValuePair<string, NodeMetadata> item in _infsession.InputMetadata) {
			_inputname = item.Key;
			_width = item.Value.Dimensions[2];
			_height = item.Value.Dimensions[3];
		} //only 1
		foreach (KeyValuePair<string, NodeMetadata> item in _infsession.OutputMetadata) {
			_outwidth = item.Value.Dimensions[1];
			_outheight = item.Value.Dimensions[2];
		} //only 1

		_rt = new RenderTexture(_width, _height, 16);
	}

	public Depth Run(Texture inputTexture) {
		int w = _width;
		int h = _height;

		if (w != _rt.width || h != _rt.height) {
			_rt.Release();
			_rt = new RenderTexture(w, h, 16);
		}

		int length = w * h;

		Graphics.Blit(inputTexture, _rt);

		Texture2D tex = new Texture2D(w, h);
		RenderTexture.active = _rt;
		tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
		RenderTexture.active = null;
		UnityEngine.GameObject.Destroy(tex);

		var rawdata = tex.GetRawTextureData();

		float[] rfloats = new float[length];
		float[] gfloats = new float[length];
		float[] bfloats = new float[length];

		for (int i = 0; i < length; i++) {
			int row = h - (i/w) - 1;
			int col = (i%w);

			int k = row * w + col;

			rfloats[k] = (float) rawdata[i*4 + 0] / 255;
			gfloats[k] = (float) rawdata[i*4 + 1] / 255;
			bfloats[k] = (float) rawdata[i*4 + 2] / 255;
			//a = rawdata[i*4 + 3];
		}
		
		var dimensions = new ReadOnlySpan<int>(new []{1, 3, h, w});
		var t1 = new DenseTensor<float>(dimensions);
		for (var j = 0; j < _height; j++) {
			if (j >= h) continue;

			for (var i = 0; i < w; i++) {
				if (i >= w) continue;

				var index = j * w + i;
				t1[0, 0, j, i] = rfloats[index];
				t1[0, 1, j, i] = gfloats[index];
				t1[0, 2, j, i] = bfloats[index];
			}
		}

		var inputs = new List<NamedOnnxValue>() {
			NamedOnnxValue.CreateFromTensor<float>(_inputname, t1)
		};

		using var results = _infsession?.Run(inputs);
		float[] output = results?.First().AsEnumerable<float>().ToArray();
		results?.Dispose();

		float max = output.Max();
		float min = output.Min();
		
		for (int i = 0; i < output.Length; i++)
			output[i] = (output[i] - min) / (max - min); 

		return new Depth(output, _outwidth, _outheight);
	}

	public void Dispose() {
		_infsession?.Dispose();
		_infsession = null;

		_rt?.Release();
		_rt = null;
	}

	public void PrintMetadata() {
		/* For debug */

		foreach (var mItem in new Dictionary<string, IReadOnlyDictionary<string, NodeMetadata>> {{"InputMetadata", _infsession.InputMetadata}, {"OutputMetadata", _infsession.OutputMetadata}}) {
			Debug.Log($"************************{mItem.Key}");
			foreach (KeyValuePair<string, NodeMetadata> item in mItem.Value) { //only 1
				Debug.Log("+++++" + item.Key + ": ");
				
				var v = item.Value;
				Debug.Log($"Dimensions:{v.Dimensions}"); //[1, 3, 384, 384]
				Debug.Log($"Dimensions.Length:{v.Dimensions.Length}");
				foreach (var e in v.Dimensions) Debug.Log(e);

				Debug.Log($"ElementType:{v.ElementType}");
				Debug.Log($"IsTensor:{v.IsTensor}");
				Debug.Log($"OnnxValueType:{v.OnnxValueType}");

				Debug.Log($"SymbolicDimensions:{v.SymbolicDimensions }");
				Debug.Log($"SymbolicDimensions.Length:{v.SymbolicDimensions.Length}");
				foreach (var e in v.SymbolicDimensions) Debug.Log(e);
			}
		}

		Debug.Log("************************MODELMETADATA");
		var mm = _infsession.ModelMetadata;
		foreach (KeyValuePair<string, string> item in mm.CustomMetadataMap)
			Debug.Log(item.Key + ": " + item.Value);
		Debug.Log($"Description:{mm.Description}");
		Debug.Log($"Domain:{mm.Domain}");
		Debug.Log($"GraphDescription:{mm.GraphDescription}");
		Debug.Log($"GraphName:{mm.GraphName}");
		Debug.Log($"ProducerName:{mm.ProducerName}");
		Debug.Log($"Version:{mm.Version}");
	}
}

#else

public class OnnxRuntimeDepthModel : DepthModel {
	public OnnxRuntimeDepthModel(string s1, string s2, int i1) {
		Debug.LogError("This should not be shown.");
	}
}

#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

//See Mqcs.cs
using Mdict = System.Collections.Generic.Dictionary<string, string>;
using PtypePname = System.Tuple<string, string>;
//using Handler = System.Action<Mdict, byte[]>;
using Handler = System.Action<System.Collections.Generic.Dictionary<string, string>, byte[]>;
//using Handlers = System.Collections.Generic.Dictionary<PtypePname, Handler>;
using Handlers = System.Collections.Generic.Dictionary<System.Tuple<string, string>, System.Action<System.Collections.Generic.Dictionary<string, string>, byte[]>>;

public class ZmqTexInputs : TexInputs {
	public bool IsConnected => _isConnected;

	private IDepthMesh _dmesh;

	private const float _timeout = 2;
	private const int _failTolerance = 3;

	private MQ _mq;
	private DepthMapType _dtype;

	private bool _isHandshaking = false;
	private bool _isConnected = false;
	private int _consecutiveFails = 0;

	private bool _noUpdate = false;
	private int _noUpdateBuffer = 0; //When _noUpdate is True, decrease this until it reaches 0, and when it is 0 the tex will not update.

	private Texture _tex;
	private Depth _depth;

	public ZmqTexInputs(IDepthMesh dmesh, int port, string multimediaPath=null) {
		_dmesh = dmesh;

		Debug.Log($"ZmqTexInputs(): port: {port}");
		_mq = new MQ(new Handlers {
			{new PtypePname("RES", "ERROR"), OnResError},
			{new PtypePname("RES", "HANDSHAKE_IMAGE_AND_DEPTH"), OnResHandshakeImageAndDepth},
			{new PtypePname("RES", "IMAGE_AND_DEPTH"), OnResImageAndDepth},

			{new PtypePname("RES", "IMAGE_AND_DEPTH_REQUEST_PLAY"), OnResImageAndDepthRequestPlay},
			{new PtypePname("RES", "IMAGE_AND_DEPTH_REQUEST_PAUSE"), OnResImageAndDepthRequestPause},
			{new PtypePname("RES", "IMAGE_AND_DEPTH_REQUEST_STOP"), OnResImageAndDepthRequestStop},
		});
		_mq.Connect(port);
		Handshake();

		if (multimediaPath != null && _isConnected)
			RequestPlay(multimediaPath); //Play the multimedia
	}

	private void Handshake() {
		Debug.Log("Handshaking...");
		_isHandshaking = true;

		bool success;
		success = _mq.Send(
			@$"
			ptype=REQ
			pname=HANDSHAKE_IMAGE_AND_DEPTH
			
			pversion={MQ.Pversion}
			client_program=DepthViewer
			client_program_version={DepthFileUtils.Version}
			!HEADEREND"
		);

		if (success) {
			success = _mq.Receive();
			if (!success)
				Debug.LogWarning("The server did not respond.");
		}
		else
			Debug.LogWarning("Failed to send!");

		//Cleanup when it failed
		if (!success) {
			Debug.LogWarning("Handshake failure.");
			_isConnected = false;
		}
		//Whether it is actually connected is determined in OnResHandshake...()

		_isHandshaking = false;
	}

	public void UpdateTex() {
		if (_noUpdate && _noUpdateBuffer <= 0)
			return;

		if (!_isConnected) {
			UITextSet.StatusText.text = "Not connected...";
			return;
		}

		//Request Image & Depth
		byte[] tosend = Utils.EncodeAscii(
			$@"
			ptype=REQ
			pname=IMAGE_AND_DEPTH
			!HEADEREND"
		);

		bool success;
		success = _mq.Send(tosend);
		if (success) _mq.Receive(); //This may set `_depth`

		if (!success) {
			Debug.LogWarning("The server did not respond.");
			_consecutiveFails++;

			if (_tex != null)
				UnityEngine.Object.Destroy(_tex);
			_tex = null;
			_depth = null;
		}

		//`_consecutiveFails` can be increased elsewhere (OnResError)
		if (_consecutiveFails > _failTolerance) {
			Debug.Log($"ZmqTexInputs.UpdateTex(): Disconnecting after {_consecutiveFails} failures.");
			_isConnected = false;

			if (_tex != null)
				UnityEngine.Object.Destroy(_tex);
			_tex = null;
			_depth = null;
		}

		if (_depth != null) {
			_dmesh.SetScene(_depth, _tex);
			UITextSet.StatusText.text = "Received!";

			if (_noUpdate && _noUpdateBuffer > 0) //See the definition for these fields
				_noUpdateBuffer--;
		}
		else {
			UITextSet.StatusText.text = "Skipping.";
		}
		_depth = null; //Indicates that there is now new depth to update
	}

	private void OnResHandshakeImageAndDepth(Mdict mdict, byte[] _) {
		Debug.Log(MQ.ReconstructHeader(mdict));

		try {
			_dtype = (DepthMapType) Enum.Parse(typeof (DepthMapType), mdict["depth_map_type"]);
			Debug.Log($"Handshake success. Depth map type: {_dtype}");
			_isConnected = true;
		}
		catch (Exception exc) {
			Debug.LogError($"Failed to parse: {exc}");
			Debug.LogWarning("Handshake failure.");
			_isConnected = false;
		}
	}

	private void OnResImageAndDepth(Mdict mdict, byte[] data) {
		string status = mdict["status"];
		switch (status) {
		case "not_avaliable":
			Debug.LogWarning("ZmqTexInputs.OnResImageAndDepth(): Server sent \"not_availblle\"");
			_consecutiveFails++;
			return;
		case "not_modified":
			return;

		case "new":
			break; //Process below

		default:
			Debug.LogWarning($"ZmqTexInputs.OnResImageAndDepth(): Unknown status {status}");
			_consecutiveFails++;
			return;
		}

		int lenImage = 0;
		int lenDepth = 0;
		try {
			lenImage = int.Parse(mdict["len_image"]);
			lenDepth = int.Parse(mdict["len_depth"]);
		}
		catch (Exception exc) {
			Debug.LogWarning($"Failed to parse: {exc}");
			_consecutiveFails++;
			return;
		}

		if (data.Length != lenImage + lenDepth) {
			Debug.LogWarning($"data.Length={data.Length} does not match (lenImage + lenDepth)={lenImage + lenDepth}");
			if (data.Length < lenImage + lenDepth)
				return;
		}

		byte[] imageBytes = new byte[lenImage];
		byte[] depthBytes = new byte[lenDepth];

		Array.Copy(data, 0, imageBytes, 0, imageBytes.Length);
		Array.Copy(data, lenImage, depthBytes, 0, depthBytes.Length);

		if (_tex != null)
			UnityEngine.Object.Destroy(_tex);
		_tex = Utils.LoadImage(imageBytes);
		_depth = DepthFileUtils.ReadPgmOrPfm(depthBytes, _dtype);
	}

	private void OnResError(Mdict mdict, byte[] data) {
		string errorMsg = Utils.DecodeAscii(data);
		Debug.LogWarning($"The server responded with the error message: {errorMsg}");
		_consecutiveFails++;

		if (_isHandshaking) {
			Debug.LogWarning("Handshake failure.");
			_isConnected = false;
		}
	}

	public void RequestPlay(string path, bool isImage=false) {
		Debug.Log($"ZmqTexInputs: Requesting {path}");

		_mq.Send(
			@$"
			ptype=REQ
			pname=IMAGE_AND_DEPTH_REQUEST_PLAY
			!HEADEREND" + '\n',
			path
		);
		_mq.Receive();

		_noUpdate = isImage;
		_noUpdateBuffer = 2; //Not in effect when _noUpdate == false
		_dmesh.ShouldUpdateDepth = isImage;
	}

	public void RequestPause() {
		_mq.Send(
			@$"
			ptype=REQ
			pname=IMAGE_AND_DEPTH_REQUEST_PAUSE
			!HEADEREND"
		);
		_mq.Receive();
	}

	public void RequestStop() {
		_mq.Send(
			@$"
			ptype=REQ
			pname=IMAGE_AND_DEPTH_REQUEST_STOP
			!HEADEREND"
		);
		_mq.Receive();
	}

	private void OnResImageAndDepthRequestPlay(Mdict mdict, byte[] data) {}
	private void OnResImageAndDepthRequestPause(Mdict mdict, byte[] data) {}
	private void OnResImageAndDepthRequestStop(Mdict mdict, byte[] data) {}

	public void Dispose() {
		if (_isConnected) {
			RequestStop();
			_isConnected = false;
		}
		_mq?.Dispose();
		_mq = null;
	}

    public void UpdateTexCustom(Texture2D texture)
    {
        throw new NotImplementedException();
    }
}

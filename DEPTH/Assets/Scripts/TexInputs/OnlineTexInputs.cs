using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class OnlineTexInputs : TexInputs {
	private OnlineTex _otex;

	private DepthModel _dmodel;
	private IDepthMesh _dmesh;
    Depth depths;
    private Texture2D _currentTex;
	private float _lastTime;
	private int updateCounter;
	private int limiterDepth;

	public OnlineTexInputs(DepthModel dmodel, IDepthMesh dmesh, OnlineTex otex) {
		_dmodel = dmodel;
		_dmesh = dmesh;
        updateCounter = 0;
        _lastTime = 0;
		limiterDepth = 10;
        depths = null;
        _otex = otex;

		if (!_otex.Supported) {
			Debug.LogError("!_otex.Supported");
			return;
		}

		_otex.StartRendering();
	}

    //TODO: aggiornare questa funzione per chiarmare UpdateTex da SocketOnline

    public void UpdateTex()
    {
        if (_otex == null || !_otex.Supported)
            return;

        Texture texture = _otex.GetTex();
        if (texture == null)
        {
            Debug.LogError("Couldn't get the texture");
            return;
        }

        float time = _otex.LastTime;
        if (time == _lastTime) return; //not changed
        _lastTime = time;

        if (_dmodel == null) return;

        if (updateCounter % limiterDepth == 0)
        {
            depths = _dmodel.Run(texture);
            _dmesh.SetScene(depths, texture);
        }
        else
        {
            _dmesh.SetTexture(texture);
        }


        //      if (updateCounter % limiterDepth == 0)
        //{
        //	updateCounter = 0;
        //	depths = _dmodel.Run(texture);
        //	_dmesh.SetScene(depths, texture);
        //}
        //else
        //{
        //	_dmesh.SetTexture(texture);
        //}
        updateCounter += 1;

        //_dmesh.SetTexture(texture);
        //_dmesh.SetScene(depths, texture);

    }

    public void UpdateTexCustom(Texture2D texture)
    {
        //Texture texture = _otex.GetTex();


        if (_dmodel == null) return;


        if (updateCounter % limiterDepth == 0)
        {
            depths = _dmodel.Run(texture);
            _dmesh.SetScene(depths, texture);
        }
        else
        {
            _dmesh.SetTexture(texture);
        }

        updateCounter += 1;


    }

    //public void UpdateTexSocket(Texture texture)
    //{

    //    float time = _otex.LastTime;
    //    if (time == _lastTime) return; //not changed
    //    _lastTime = time;

    //    if (_dmodel == null) return;

    //    Depth depths = _dmodel.Run(texture);
    //    _dmesh.SetScene(depths, texture);
    //}

    public void Dispose() {
		_otex.Dispose();
		_otex = null;
	}

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class raymarchcamera : SceneViewFilter
{
    #region material shader
    //------ Material component
    [SerializeField]
    private Shader _shader;

    public Material _raymarchMaterial
    {
        get
        {
            if (!_raymarchMat && _shader)
            {
                _raymarchMat = new Material(_shader);
                _raymarchMat.hideFlags = HideFlags.HideAndDontSave;
            }
            return _raymarchMat;
        }
    }

    private Material _raymarchMat;

    //------Camera component
    public Camera _camera
    {
        get
        {
            if (!_cam)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }

    private Camera _cam;
    #endregion
    #region push coords
    private void PushCoords(RenderTexture source, RenderTexture destination) {

        RenderTexture.active = destination;
        GL.PushMatrix();
        GL.LoadOrtho();
        _raymarchMaterial.SetPass(0);
        GL.Begin(GL.QUADS);

        //BL
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f);
        //BR
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);
        //TR
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);
        //TL
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }
    #endregion
    #region frustum
    //------ Frustum 
    private Matrix4x4 CamFrustum(Camera cam)
    {

        Matrix4x4 frustum = Matrix4x4.identity;
        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 TL = (-Vector3.forward - goRight + goUp);
        Vector3 TR = (-Vector3.forward + goRight + goUp);
        Vector3 BR = (-Vector3.forward + goRight - goUp);
        Vector3 BL = (-Vector3.forward - goRight - goUp);

        frustum.SetRow(0, TL);
        frustum.SetRow(1, TR);
        frustum.SetRow(2, BR);
        frustum.SetRow(3, BL);

        return frustum;
    }
    #endregion

    public float _accuracy = 0.01f;
    public float _maxDistance = 200;
    public float _boxround;
    public float _smooth1;
    public float _smooth2;
    public float _LightInt = 1;
    public float _ShadowIntensity = 0.5f;
    public float _penumbra = 0.4f;

    public int _maxIterations = 200;

    public Vector2 _shadowDistance = new Vector2(0.1f, 10f); 

    public Transform _directionalLight;

    public Vector3 _modInterval;

    public Color _mainColor, _LightColor;

    public Vector4 _sphere1, _sphere2, _box1; 



    //----- Interfaccia con lo shader
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_raymarchMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }

        _raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(_camera));
        _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);

        _raymarchMaterial.SetFloat("_accuracy", _accuracy);
        _raymarchMaterial.SetFloat("_maxDistance", _maxDistance);
        _raymarchMaterial.SetFloat("_smooth1", _smooth1);
        _raymarchMaterial.SetFloat("_smooth2", _smooth2);
        _raymarchMaterial.SetFloat("_LightInt", _LightInt);
        _raymarchMaterial.SetFloat("_ShadowIntensity", _ShadowIntensity);
        _raymarchMaterial.SetFloat("_penumbra", _penumbra);

        _raymarchMaterial.SetInt("_maxIterations", _maxIterations);

        _raymarchMaterial.SetVector("_shadowDistance", _shadowDistance);
        _raymarchMaterial.SetVector("_LightDir", _directionalLight ? _directionalLight.forward : Vector3.down);
        _raymarchMaterial.SetVector("_modInterval", _modInterval);
        _raymarchMaterial.SetVector("_mainColor", _mainColor);
        _raymarchMaterial.SetVector("_LightColor", _LightColor);
        _raymarchMaterial.SetVector("_mainColor", _mainColor);
        _raymarchMaterial.SetVector("_sphere1", _sphere1);
        _raymarchMaterial.SetVector("_sphere2", _sphere2);
        _raymarchMaterial.SetVector("_box1", _box1);

        _raymarchMaterial.SetTexture("_MainTex", source);

        PushCoords(source, destination);
    }


}

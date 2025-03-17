Shader "Custom/AlwaysVisible"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0, 0, 1) // Change color here
    }
    SubShader
    {
        Tags { "Queue"="Overlay" } // Renders on top of everything
        Pass
        {
            ZWrite Off    // Prevents depth testing (always visible)
            Blend SrcAlpha OneMinusSrcAlpha // Allows transparency
            Color [_Color]
        }
    }
}

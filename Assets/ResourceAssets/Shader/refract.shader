// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33396,y:32613,varname:node_3138,prsc:2|alpha-7426-OUT,clip-6071-A,refract-6247-OUT;n:type:ShaderForge.SFN_Slider,id:3566,x:32424,y:32757,ptovrint:False,ptlb:node_3566,ptin:_node_3566,varname:node_3566,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4273504,max:1;n:type:ShaderForge.SFN_Multiply,id:8101,x:32810,y:32660,varname:node_8101,prsc:2|A-9612-RGB,B-6555-A,C-3566-OUT;n:type:ShaderForge.SFN_VertexColor,id:6555,x:32503,y:32602,varname:node_6555,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:9612,x:32482,y:32447,ptovrint:False,ptlb:node_9612,ptin:_node_9612,varname:node_9612,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8689fac7742124e4ebd4c379ee337bcc,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ComponentMask,id:6247,x:32996,y:32711,varname:node_6247,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-8101-OUT;n:type:ShaderForge.SFN_Tex2d,id:6071,x:32903,y:32466,ptovrint:False,ptlb:node_6071,ptin:_node_6071,varname:node_6071,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:7426,x:33119,y:32479,varname:node_7426,prsc:2,v1:0;proporder:3566-6071-9612;pass:END;sub:END;*/

Shader "Shader Forge/refract" {
    Properties {
        _node_3566 ("node_3566", Range(0, 1)) = 0.4273504
        _node_6071 ("node_6071", 2D) = "white" {}
        _node_9612 ("node_9612", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float _node_3566;
            uniform sampler2D _node_9612; uniform float4 _node_9612_ST;
            uniform sampler2D _node_6071; uniform float4 _node_6071_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _node_9612_var = tex2D(_node_9612,TRANSFORM_TEX(i.uv0, _node_9612));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (_node_9612_var.rgb*i.vertexColor.a*_node_3566).rg;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float4 _node_6071_var = tex2D(_node_6071,TRANSFORM_TEX(i.uv0, _node_6071));
                clip(_node_6071_var.a - 0.5);
////// Lighting:
                float3 finalColor = 0;
                return fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _node_6071; uniform float4 _node_6071_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _node_6071_var = tex2D(_node_6071,TRANSFORM_TEX(i.uv0, _node_6071));
                clip(_node_6071_var.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

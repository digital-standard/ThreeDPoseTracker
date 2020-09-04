Shader "Unlit/CastShadow"
{
	Properties
	{
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white" { }
		_ShadowIntensity("Shadow Intensity", Range(0.0, 5.0)) = 1.0 // 大きくするほど影が暗くなる
		// _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5 // 半透明物体の場合、アルファカットオフを可変にはせずに...
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0 // カットオフをトグルで切り替え、オンならば固定値0.001でカットオフするケースが多いようです
	}
		CGINCLUDE
			// 影係数の取得および緩和に使う
			// 1 - (1 - 影係数) * _ShadowIntensity を最終的な影係数とすることで、影が暗くなりすぎるのを防ぐ
//#define FADED_SHADOW_ATTENUATION(i) (1.0 - (1.0 - UNITY_SHADOW_ATTENUATION(i, i.worldPos)) * _ShadowIntensity)
#define FADED_SHADOW_ATTENUATION(i) (1.0 - (1.0 - UNITY_SHADOW_ATTENUATION(i, i.worldPos)) * _ShadowIntensity)
			ENDCG
			SubShader
		{
			// Queueが「AlphaTest」になっていましたが、おそらく妥当な方法だと思います
			// 本来は「AlphaTest」はアルファを基に完全透明・完全不透明を切り替えるタイプの不透明オブジェクトに
			// 用いるもので、半透明オブジェクトは「Transparent」を使うべきかと思いますが、そうしてしまうと
			// 影機能も使えなくなってしまうようです
			// 半透明オブジェクトを正しくアルファ合成するには他の不透明オブジェクトよりも後に描画させたいところですが
			// 「AlphaTest」なら一般的な不透明オブジェクトの「Geometry」よりは後になるそうなので、描画不具合も
			// 起こりにくいのではないでしょうか
			Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }
			// シャドウキャスティングパス
			// LightModeが「ShadowCaster」のパスがないと、影を落とす・受けるのいずれも行われなくなるようです
			// コード末尾にFallbackを記述しておくことで他のシェーダーに処理を代替させることもできますが、
			// 今回は半透明の特殊なケースですので、自前で用意することにしました
			// なお、旧バージョンでは「ShadowCollector」パスが影を受ける処理を担当していたそうですが
			// 現在は廃止されたようです
			Pass
			{
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }
				ZWrite On ZTest LEqual
				CGPROGRAM
				#define UNITY_STANDARD_USE_DITHER_MASK
				#define UNITY_STANDARD_USE_SHADOW_UVS
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityStandardShadow.cginc"
				struct VertexOutput
				{
					V2F_SHADOW_CASTER_NOPOS
					float2 tex: TEXCOORD1;
				};
				void vert(VertexInput v, out VertexOutput o, out float4 opos: SV_POSITION)
				{
					TRANSFER_SHADOW_CASTER_NOPOS(o, opos)
					o.tex = v.uv0;
				}
				half4 frag(VertexOutput i, UNITY_VPOS_TYPE vpos : VPOS) : SV_Target
				{
					// シャドウマップへの描画は原理的に完全透明か完全不透明でないとなりません
					// そこで、Unity組み込みのディザリング用テクスチャをアルファに基づいて参照することで
					// アルファが小さいほど高確率で透明、大きいほど不透明となるような動作をさせることができます
					// できあがるシャドウマップにはディザリングによるノイズが乗っていますが、ソフトシャドウモードなら
					// 後で近傍比率フィルタリングがかかるので、影に関してはさほど見た目は悪くないと思われます
					// 半透明部分のエッジにノイズが目立つかもしれませんが、これはポストエフェクトなどでごまかす...?
					half alpha = tex2D(_MainTex, i.tex.xy).a * _Color.a;
					alpha = tex3D(_DitherMaskLOD, float3(vpos.xy * 0.25, alpha * 0.9375)).a;
					// 場合によっては下記1行をコメントアウトして、Quad全面を完全描画させた方が良好な結果になるかと思います
					//clip(alpha - 0.01); // ディザリングの結果アルファ0.01未満になればフラグメントを破棄
					SHADOW_CASTER_FRAGMENT(i)
				}

				ENDCG
			}
			// フォワードレンダリングベースパス
			Pass
			{
				Name "ForwardBase"
				Tags { "LightMode" = "ForwardBase" }
				Blend SrcAlpha OneMinusSrcAlpha // 一般的なアルファ合成方式を使用
				ZWrite On ZTest LEqual
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_fwdbase
				#pragma multi_compile _ UNITY_UI_ALPHACLIP // アルファクリップがオン・オフの場合でマルチコンパイル
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				sampler2D _MainTex;
				fixed4 _Color;
				float _ShadowIntensity;
				struct v2f
				{
					float4 pos: SV_POSITION; // UNITY_TRANSFER_SHADOW内ではクリッピング座標の名前が「pos」であることが前提になっているため、そのようにする(ご質問者さんのコードでは、すでにそうなっていました)
					float2 uv: TEXCOORD0;
					UNITY_SHADOW_COORDS(1) // TEXCOORD1を影情報伝達に使用する
					float3 worldPos: TEXCOORD2; // TEXCOORD2をワールド座標伝達に使用する
				};
				v2f vert(appdata_full v) // 頂点入力データの形式として、appdata_tanに代わってappdata_fullを使う
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord.xy;
					UNITY_TRANSFER_SHADOW(o, v.texcoord1); // texcoord1にライトマップ用UVが格納されているので、それを使って影情報の計算を行う
					o.worldPos = mul(unity_ObjectToWorld, v.vertex); // worldPosに頂点のワールド座標をセットする
					return o;
				}
				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 color = tex2D(_MainTex, i.uv) * _Color;
					#ifdef UNITY_UI_ALPHACLIP
						clip(color.a - 0.001); // アルファ0.001未満は完全透明と見なし、フラグメントを破棄してレンダリングしない
					#endif
					color.rgb *= FADED_SHADOW_ATTENUATION(i); // 影係数はアルファには作用させない
					return color;
				}
				ENDCG

			}

						// フォワードレンダリング加算パス
						Pass
						{
							Name "ForwardAdd"
							Tags { "LightMode" = "ForwardAdd" }
							Blend DstColor Zero // 普通は加算パスでは照明効果をどんどん加算していくかと思いますが、今回は影をどんどん乗算していくことにしました
							ZWrite Off
							CGPROGRAM

							#pragma vertex vert
							#pragma fragment frag
							#pragma fragmentoption ARB_fog_exp2
							#pragma fragmentoption ARB_precision_hint_fastest
							#pragma multi_compile_fwdadd_fullshadows
							#pragma multi_compile _ UNITY_UI_ALPHACLIP
							#include "UnityCG.cginc"
							#include "AutoLight.cginc"
							sampler2D _MainTex;
							fixed4 _Color;
							float _ShadowIntensity;
							// v2f、vertはベースパスと同じ(CGINCLUDEやcgincを使って共通コードをまとめることも可能かと思います)
							struct v2f
							{
								float4 pos: SV_POSITION;
								float2 uv: TEXCOORD0;
								UNITY_SHADOW_COORDS(1)
								float3 worldPos: TEXCOORD2;
							};
							v2f vert(appdata_full v)
							{
								v2f o;
								o.pos = UnityObjectToClipPos(v.vertex);
								o.uv = v.texcoord.xy;
								UNITY_TRANSFER_SHADOW(o, v.texcoord1);
								o.worldPos = mul(unity_ObjectToWorld, v.vertex);
								return o;
							}
							fixed4 frag(v2f i) : SV_Target
							{
								float alpha = tex2D(_MainTex, i.uv).a * _Color.a;
								#ifdef UNITY_UI_ALPHACLIP
									clip(alpha - 0.001); // アルファ0.001未満は完全透明と見なし、フラグメントを破棄してレンダリングしない
								#endif
								float attenuation = FADED_SHADOW_ATTENUATION(i);
								return fixed4(attenuation, attenuation, attenuation, 1.0);
							}
							ENDCG

						}
		}
}
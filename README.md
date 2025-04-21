1. **Unity Editor**  
   - 버전: 2022.3.47f1  
   - (ProjectSettings/ProjectVersion.txt 에 `m_EditorVersion` 으로도 확인 가능)


2. **Package Manager**  
   - Window → Package Manager → In Project → Resolve(⟳)  
   - Packages/manifest.json 에 기록된 dependencies:  

   "dependencies": {
    "com.unity.ai.navigation": "1.1.5",
    "com.unity.barracuda": "https://github.com/Unity-Technologies/barracuda-release.git",
    "com.unity.collab-proxy": "2.5.1",
    "com.unity.feature.development": "1.0.1",
    "com.unity.textmeshpro": "3.0.7",
    "com.unity.timeline": "1.7.6",
    "com.unity.ugui": "1.0.0",
    "com.unity.visualscripting": "1.9.4",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.cloth": "1.0.0",
    "com.unity.modules.director": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.terrain": "1.0.0",
    "com.unity.modules.terrainphysics": "1.0.0",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.umbra": "1.0.0",
    "com.unity.modules.unityanalytics": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestassetbundle": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.unitywebrequestwww": "1.0.0",
    "com.unity.modules.vehicles": "1.0.0",
    "com.unity.modules.video": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.wind": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
  }

![image](https://github.com/user-attachments/assets/6ed597de-090e-46cd-871c-bcb2a1a896f7)


## Onboarding 절차  
1. `git pull` 로 최신 커밋(특히 Packages/*.json) 가져오기  
2. Unity Hub 에서 프로젝트 열기  
3. Window → Package Manager → In Project → Resolve(⟳)

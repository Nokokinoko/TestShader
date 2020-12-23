# TestShader
## SnowScene
Using shaders, let the snow fall within the specified range.  
Snow shakes randomly.  
Using the inner product of the object's normal vector and the world's upward vector, we change the color of the texture to white, so that snow appears to be piled up.  
  
シェーダを使って、特定範囲内に雪を降らせます。  
雪はランダムに揺れ落ちます。  
オブジェクトの法線ベクトルとワールドの上方向ベクトルの内積を使って、テクスチャの色を白に変動させることで雪が積もっているように見せています。  
  
![Snow](https://github.com/Nokokinoko/TestShader/blob/master/gif/Snow.gif)  
  
## WaveScene
Draw the object group like a wave like using Perlin Noise.  
It is realized by manipulating UV coordinates of the mesh object.  
  
パーリンノイズを用いてオブジェクト群を波打つように描画しています。  
メッシュオブジェクトのUV座標を操作することで実現しています。  
  
(参考：[setchi/Unity-GPGPU-Sandbox](https://github.com/setchi/Unity-GPGPU-Sandbox))  
  
![Wave](https://github.com/Nokokinoko/TestShader/blob/master/gif/Wave.gif)  
  
## ParticleScene
Particle using mesh object vertex information.  
Writing position information with fragment shader, reading position information with vertex shader and rendering.  
  
メッシュオブジェクトの頂点情報を用いたパーティクル。  
フラグメントシェーダで位置情報を書き込み、バーテックスシェーダで位置情報を読み込んで描画しています。  
  
(参考：[setchi/Unity-GPGPU-Sandbox](https://github.com/setchi/Unity-GPGPU-Sandbox))  
  
![Particle](https://github.com/Nokokinoko/TestShader/blob/master/gif/Particle.gif)  

# Dependency
Unity 2017.4.8f1

# Used Asset
FreeLowPolyPack by BrokenVector  
LowPolyBuildingsLite by A3D

# License
This software is released under the MIT License, see LICENSE.

# Authors
SYOTA TSUDA  
https://whale-no12.work/

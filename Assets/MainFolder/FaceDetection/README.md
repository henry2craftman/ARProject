# Plane Detection
<img src="/Images/FaceDetectionApp.jpg" width="25%" height="30%" title="px(픽셀) 크기 설정" alt="FaceDetectionApp"></img>

<h2 id="table-of-contents">📝 Table of Contents</h2>

- [Architecture](#architecture)
- [Folder Structure](#folder-structure)
- [Videos](#videos)


<h2 id="architecture">🌠 Architecture</h2>
<img src="/Images/faceDetection.png" width="30%" height="30%" title="px(픽셀) 크기 설정" alt="Plane Detection"></img>

- AR Face Manager: AR Camera Manager로 부터 받은 이미지 정보에서 얼굴을 탐지
- FacialFeature.cs: AR Face Manager의 얼굴 정보에서 468개의 얼굴 특징점의 정보를 기반으로 얼굴에 Mesh를 생성
  - Video Manager: 얼굴에 생성된 Mesh에 Video를 재생(RenderTexture를 Albedo로가진 Mateirl을 사용) 

<h2 id="folder-structure">🌠 Folder Structure</h2>

<h2 id="videos">🌠 Videos</h3>


https://github.com/henry2craftman/ARProject/assets/141684228/40a685c8-9a70-4043-8d09-d898561fe037





[목차로 돌아가기](#table-of-contents)

# MZ.Xray.Prism

### 1. 프로젝트 개요

#### 시연 영상
프로젝트 동작 예시를 영상으로 확인할 수 있습니다.  
[![MZ.Xray.Prism 시연 영상](https://img.youtube.com/vi/uLTVRcM9iZM/0.jpg)](https://www.youtube.com/watch?v=uLTVRcM9iZM)  
> 클릭하면 YouTube로 이동하여 전체 영상을 재생합니다.

#### 목적
- 생산 라인/연구 장비 등에서 X-ray 영상을 자동으로 분석해 위험 물품을 검출

#### 특징
- 실시간 영상 스트림 처리(프레임 파이프라인)
- AI 추론(객체 탐지) 모듈화
- 결과 저장(DB)
- 룰 기반 검사 로직(Calibration & LUT Curve)

#### 환경 요구 사항
- OS: Windows 10/11, .NET 8 LTS
- IDE: Visual Studio 2022, Desktop development with .NET 
- AI : ONNX Runtime(CUDA)
- GPU: CUDA 12.x, cuDNN 9.x
- Library : WPF, Prism, OpenCV-Sharp, Serilog,  LiveChartCore

#### 기능 
- 로그인 및 회원가입 기능
- 실환경 Xray형식과 동일한 파일 네트워크 통신 수행
   - 실환경은 (unsafe)Pointer 형식임으로 해당 데이터를 네트워크 소켓으로 전송하여 실시간 처리로 구성
- 하단(Footer) 버튼 기능 설명
   - 픽커(Picker) : 기능 제어를 위한 버튼, 해당 버튼이 On/Off에 따라 화면에서 하단 버튼이 보이는 여부를 확인
   - 시작/정지(Play/Stop) : Xray 제어 수행 (시작/정지)
   - 이전/이후(Prev/Next) : 1 Frame 이전, 이후 이동
   - 색상 : Gray, Color, 유기물(Organic), 무기물(Inorganic), 금속(Metal) 표현
   - 줌 : 화면의 중앙을 기점으로 확대/축소
   - 필터 : 밝기(Bright), 대조(Contrast) 및 필터 초기화
   - 인공지능 : On/Off를 통해 UI 에 표현
   - Zeffect : 물성 분석 알고리즘을 통해 해당 범위를 색상으로 표현
   - 캡쳐 : ui 화면 캡쳐 수행
   - 설정 : Footer 버튼 (보이기/숨기기)
- 상단(Menu) 버튼 설명
   - 색상 정보(Material) : 이중에너지에 대한 색상값을 수치화하여 표현하기 위함
   - 색상 곡선(LUT Curve) : 화면에 표기된 Xray 색상을 그래프를 통해 제어 수행
   - 인공지능 카테고리 관리(AI) : 인공지능 모델의 카테고리 정보 수정 및 제어 가능
   - 보고서(Report) : 해당 모델이 예측된 결과를 기간별로 검색이 가능하며, PDF 파일을 통해 분석 결과 표현
   - 이미지 저장소(Image Storage) : 저장된 이미지 목록을 검색 및 확인 가능
   - 로그 저장소(Log Storage) : 로그 정보 저장
   - 태마 색상(theme) : White/Dark 태마 설정
   - 사용자 정보 : 현 사용자가 소프트웨어 사용 시간 표현

### 2. 아키텍처

```text
MZ.Xray.Prism/
├─ Application/
├─ Producer/
├─ UnitTest/
├─ .github/
│  └─ workflows/
```

- Application : 운영 UI, 시각화, 설정 관리, 실시간 상태 모니터링(Prism MVVM)
- Producer : 입력→전처리→데이터 송신, 가상의 장비(Detector)의 송신부 구현
- UnitTest : 핵심 알고리즘/서비스 단위 테스트
- .github/workflows(CI/CD) : 빌드/테스트 자동화 워크플로우(Git Action)

### 3. 엔티티 

```
User (1) ──── (1) UserSetting ── (N) UserButton
   │
   ├──── (1) Calibration
   ├──── (1) Filter
   ├──── (1) Material ─── (N) MaterialControl
   ├──── (N) ZeffectControl
   └──── (N) CurveControl

AIOption (1) ─── (N) Category

Image (1) ─── (N) ObjectDetection
```



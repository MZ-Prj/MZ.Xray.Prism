# MZ.Xray.Prism

### 1. 프로젝트 개요

#### 목적
- 생산 라인/연구 장비 등에서 X-ray 영상을 자동으로 분석해 위험 물품을 검출

#### 특징
- 실시간 영상 스트림 처리(프레임 파이프라인)
- AI 추론(객체 탐지) 모듈화
- 결과 저장(DB)
- 룰 기반 검사 로직(Calibration & LUT Curve)

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
- .github/workflows(CI/CD) : 빌드/테스트/릴리스 자동화 워크플로우(Git Action)

### 4. 의존성 및 환경 요구 사항
- OS: Windows 10/11 (운영 UI 기준), .NET 8 LTS
- IDE: Visual Studio 2022 최신, Desktop development with .NET 
- AI : ONNX Runtime(CUDA)
- GPU: CUDA 12.x, cuDNN 9.x
- Library : OpenCV-Sharp, Serilog/NLog, Prism, LiveChartCore

### 5. 구성(설정 파일 스키마)
<!-- TODO -->

### 6. 테스트 전략(Unit)
- VisionBase : 모듈 아키텍쳐로써 이미지 처리 단위 테스트
- Infrastructure : 데이터베이스의 Service 처리 단위 테스트
- AI : Library에 따라 모델 로드 및 예측 결과 검증

### 7. 기능 설

# PhotoSelector2 - File Mover

이 앱은 파일 이름 기반으로 파일을 이동시키는 기능을 제공합니다.

## 기능 설명

파일 목록을 입력받아 확장자를 제외한 파일명이 일치하는 파일들을 지정한 원본 폴더에서 찾아 자동으로 생성되는 "selected" 폴더로 이동시킵니다.

## 사용 방법

1. **원본 폴더 (Source Folder)**: 
   - "Select" 버튼을 클릭하여 파일 탐색기로 폴더를 선택합니다
   - 선택한 폴더 경로가 자동으로 표시됩니다
   - 예: `C:\Users\Photos\Wedding`

2. **대상 폴더 (Destination Folder)**: 
   - 자동으로 원본 폴더 아래 "selected" 폴더가 생성됩니다
   - 예: 원본이 `C:\Users\Photos\Wedding`이면 대상은 `C:\Users\Photos\Wedding\selected`
   - 폴더가 없으면 자동으로 생성됩니다

3. **파일 이름 목록 (File Names)**: 이동시킬 파일 이름을 입력합니다
   - 쉼표(,)로 구분하여 여러 파일을 입력할 수 있습니다
   - 확장자 포함/제외 모두 가능합니다
 - 예시:
     ```
     251103 Portrait-0402.jpg, 251103 Portrait-0412.jpg, 251103 Portrait-0083
     ```

4. **파일 이동 실행 (Move Files)**: 버튼을 클릭하여 파일 이동을 실행합니다

## 결과 확인

실행 후 결과 섹션에서 다음 정보를 확인할 수 있습니다:

- **성공**: 이동된 파일 개수
- **이동된 파일 목록**: 실제로 이동된 파일들의 이름
- **실패/누락 파일**: 찾을 수 없거나 이미 대상 폴더에 존재하는 파일들

## 주요 특징

- **폴더 탐색기 지원**: Windows 탐색기를 통해 폴더를 쉽게 선택할 수 있습니다

- **자동 대상 폴더 생성**: 원본 폴더 아래 "selected" 폴더가 자동으로 생성되어 파일이 정리됩니다

- **확장자 무관**: 입력 시 확장자가 있든 없든 파일명으로 매칭합니다
  - `photo-001.jpg` 입력 → `photo-001.jpg`, `photo-001.png` 모두 찾음
  - `photo-001` 입력 → `photo-001.jpg`, `photo-001.png` 모두 찾음

- **중복 방지**: 대상 폴더에 이미 같은 이름의 파일이 있으면 이동하지 않고 스킵합니다

- **대소문자 구분 없음**: 파일명 비교 시 대소문자를 구분하지 않습니다

## 예제

**작업 전 폴더 구조:**
```
C:\Photos\Wedding\
├── 251103 Portrait-0001.jpg
├── 251103 Portrait-0002.jpg
├── 251103 Portrait-0402.jpg
├── 251103 Portrait-0412.jpg
├── 251103 Portrait-0083.jpg
└── ... (기타 파일들)
```

**앱 사용:**
1. Source Folder 선택: `C:\Photos\Wedding`
2. File Names 입력:
   ```
   251103 Portrait-0402.jpg, 251103 Portrait-0412.jpg, 251103 Portrait-0083.jpg
   ```
3. "Move Files" 클릭

**작업 후 폴더 구조:**
```
C:\Photos\Wedding\
├── 251103 Portrait-0001.jpg
├── 251103 Portrait-0002.jpg
├── ... (기타 파일들)
└── selected\
    ├── 251103 Portrait-0402.jpg
    ├── 251103 Portrait-0412.jpg
    └── 251103 Portrait-0083.jpg
```

## 기술 스택

- .NET 9.0
- .NET MAUI
- CommunityToolkit.Maui (FolderPicker 기능)
- Cross-platform: Android, iOS, macOS Catalyst, Windows

## 코드 구조

- `MainPage.xaml`: UI 레이아웃
- `MainPage.xaml.cs`: UI 이벤트 핸들러 및 FolderPicker 로직
- `Services/FileManagerService.cs`: 파일 이동 로직
- `MauiProgram.cs`: CommunityToolkit.Maui 초기화

## 주의사항

- 파일 이동은 복사가 아닌 **이동(move)** 작업입니다
- 원본 폴더에서 파일이 사라지고 selected 폴더로 이동됩니다
- 실행 전 백업을 권장합니다
- 파일 시스템 권한이 필요합니다
- Windows에서 폴더 선택 대화상자를 통해 안전하게 폴더를 선택할 수 있습니다

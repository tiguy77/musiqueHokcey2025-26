#ifndef MyAppVersion
  #define MyAppVersion "2.0.2"
#endif

#define MyAppName "Aréna DJ"
#define MyAppPublisher "Aréna DJ"
#define MyAppExeName "MusiqueHockey.exe"

[Setup]
AppId={{A8E42D7C-1D64-4BD9-8C7B-28A62E47A6D2}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={localappdata}\Programs\Arena DJ
DefaultGroupName=Aréna DJ
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
OutputDir=..\dist\installer
OutputBaseFilename=ArenaDJ-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#MyAppExeName}
SetupLogging=yes
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
Name: "desktopicon"; Description: "Créer un raccourci sur le Bureau"; GroupDescription: "Raccourcis :"; Flags: unchecked

[Files]
Source: "..\dist\windows-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Aréna DJ"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\Aréna DJ"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Lancer Aréna DJ"; Flags: nowait postinstall skipifsilent

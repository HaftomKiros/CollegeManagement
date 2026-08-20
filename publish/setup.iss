[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName=College Management System
AppVersion=1.0.0
AppPublisher=ECC-DoA Wukro St. Mary College
DefaultDirName={autopf}\College Management System
DefaultGroupName=College Management System
OutputDir=C:\Temp\CollegeSetup
OutputBaseFilename=CollegeManagement_Setup_v1.0.0
Compression=lzma2/max
SolidCompression=yes
Uninstallable=yes
UninstallDisplayName=College Management System

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a Desktop shortcut"; Flags: unchecked

[Files]
Source: "CollegeManagement\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\College Management System";           Filename: "{app}\CollegeManagementWPF.exe"
Name: "{group}\Uninstall College Management System"; Filename: "{uninstallexe}"
Name: "{autodesktop}\College Management System";     Filename: "{app}\CollegeManagementWPF.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\CollegeManagementWPF.exe"; Description: "Launch College Management System"; Flags: nowait postinstall skipifsilent

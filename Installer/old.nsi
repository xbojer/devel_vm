Name "Beta Manager Installer"
OutFile "Installer_Beta_Manager.exe"
InstallDir "$PROFILE\Beta Manager"
!include 'LogicLib.nsh'
!include 'Registry.nsh'
!include 'FileFunc.nsh'
!include "MUI2.nsh"
!include WinMessages.nsh
!include x64.nsh
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_LANGUAGE "Polish"
RequestExecutionLevel admin
Var PROGRAM_FILES
Var BM_VERSION

!define StrTrimNewLines "!insertmacro StrTrimNewLines"
!define StrStr "!insertmacro StrStr"

!macro StrTrimNewLines ResultVar String
  Push "${String}"
  Call StrTrimNewLines
  Pop "${ResultVar}"
!macroend
Function StrTrimNewLines
  Exch $R0
  Push $R1
  Push $R2

  ;Initialize trim counter
  StrCpy $R1 0

  loop:
  ;Subtract to get "String"'s last characters
  IntOp $R1 $R1 - 1

  ;Verify if they are either $\r or $\n
  StrCpy $R2 $R0 1 $R1
  ${If} $R2 == `$\r`
  ${OrIf} $R2 == `$\n`
    Goto loop
  ${EndIf}

  ;Trim characters (if needed)
  IntOp $R1 $R1 + 1
  ${If} $R1 < 0
    StrCpy $R0 $R0 $R1
  ${EndIf}


  ;Return output to user
  Pop $R2
  Pop $R1
  Exch $R0
FunctionEnd
!macro StrStr ResultVar String SubString
  Push `${String}`
  Push `${SubString}`
  Call StrStr
  Pop `${ResultVar}`
!macroend
Function StrStr
/*After this point:
  ------------------------------------------
  $R0 = SubString (input)
  $R1 = String (input)
  $R2 = SubStringLen (temp)
  $R3 = StrLen (temp)
  $R4 = StartCharPos (temp)
  $R5 = TempStr (temp)*/

  ;Get input from user
  Exch $R0
  Exch
  Exch $R1
  Push $R2
  Push $R3
  Push $R4
  Push $R5

  ;Get "String" and "SubString" length
  StrLen $R2 $R0
  StrLen $R3 $R1
  ;Start "StartCharPos" counter
  StrCpy $R4 0

  ;Loop until "SubString" is found or "String" reaches its end
  ${Do}
    ;Remove everything before and after the searched part ("TempStr")
    StrCpy $R5 $R1 $R2 $R4

    ;Compare "TempStr" with "SubString"
    ${IfThen} $R5 == $R0 ${|} ${ExitDo} ${|}
    ;If not "SubString", this could be "String"'s end
    ${IfThen} $R4 >= $R3 ${|} ${ExitDo} ${|}
    ;If not, continue the loop
    IntOp $R4 $R4 + 1
  ${Loop}

/*After this point:
  ------------------------------------------
  $R0 = ResultVar (output)*/

  ;Remove part before "SubString" on "String" (if there has one)
  StrCpy $R0 $R1 `` $R4

  ;Return output to user
  Pop $R5
  Pop $R4
  Pop $R3
  Pop $R2
  Pop $R1
  Exch $R0
FunctionEnd

Section "Begin"

	SetOutPath "$INSTDIR"

	${If} ${RunningX64}
		DetailPrint "Maszyna 64 bitowa"
		SetRegView 64
		StrCpy $PROGRAM_FILES $PROGRAMFILES64
	${Else}
		StrCpy $PROGRAM_FILES $PROGRAMFILES
	${EndIf}

# PRZYGOTOWANIE DO INSTALACJI

	${GetParameters} $R0
	${if} $R0 == "/UPDATE_VBOX"
			${registry::Read} "HKEY_LOCAL_MACHINE\SOFTWARE\Oracle\VirtualBox" "VersionExt" $R1 $R2
			${If} ${FileExists} "\\10.11.11.200\instale\Devel\VirtualBox-$R1.exe"
				DetailPrint "Wersja VirtualBox'a jest aktualna."
				Quit
			${Else}
				DetailPrint "Jest nowa wersja VirtualBox'a. Aktualizujê do najnowszej wersji ..."
				DetailPrint "Uruchamianie instalatora VirtualBox ..."
				nsExec::Exec "\\10.11.11.200\instale\Devel\VirtualBox.exe"
				Delete $TEMP\VirtualBox.exe
				goto exit_program
			${EndIf}
	${EndIf}
# Sprawdz czy sa stare smieci w c:\users\fotka\Devel_VM
	${If} ${FileExists} "C:\Users\fotka\Devel_VM"
		DetailPrint "Znaleziono star? instalacj?. Usuwam ..."
		RMDir /r "C:\Users\fotka\Devel_VM"
		messageBox MB_OK "Po zako?czeniu instalacji usu? stary program Beta Manager z sekcji Autostart!"
	${EndIf}

	# Sprawdz czy VirtualBox jest zainstalowany, jezeli nie to zainstaluj
	IfFileExists "$PROGRAM_FILES\Oracle\VirtualBox\VirtualBox.exe" +6 InstallVBox
	InstallVBox:
		DetailPrint "Kopiowanie VirtualBox'a ..."
		CopyFiles '\\10.11.11.200\instale\Devel\VirtualBox.exe' "$TEMP\VirtualBox.exe"
		DetailPrint "Uruchamianie instalatora VirtualBox ..."
		nsExec::Exec "$TEMP\VirtualBox.exe"
		Delete $TEMP\VirtualBox.exe
	DetailPrint "VirtualBox jest ju? zainstalowany."

# Sprawdz czy istenieje user vbox, jezeli nie utworz
	nsExec::ExecToStack "$SYSDIR\net.exe user"
	Pop $0
	Pop $1
	${StrStr} $R0 $1 "vbox"
	${if} $R0 == ""
		DetailPrint "Tworze usera 'vbox' ..."
		nsExec::Exec "$SYSDIR\net.exe user vbox /add 123qwe"
	${Else}
		DetailPrint "User 'vbox' istnieje."
	${EndIf}

	DetailPrint "Sprawdzanie czy u?ytkownik 'vbox' jest ukryty ..."
	${registry::KeyExists} "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList" $R1
	${if} $R1 == "-1"
		DetailPrint "Ukrywanie u?ytkownika 'vbox' ..."
		${registry::CreateKey} "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList" $R1
		${registry::Write} "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList" "vbox" "0" "REG_DWORD" $R1
	${Else}
		DetailPrint "Klucz istenieje. Sprawdzam czy u?ytkownika ma poprawn? warto?? widoczno?ci ..."
		${registry::Read} "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList" "vbox" $R1 $R2
		${if} $R1 == "0"
			DetailPrint "U?ytkownik jest ju? ukryty."
		${Else}
			DetailPrint "U?ytkownik nie jest ukryty. Zmieniam warto?? ..."
			${registry::Write} "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList" "vbox" "0" "REG_DWORD" $R1
		${EndIf}
	${EndIf}


# Sprawdz czy katalog do kodu istnieje, jezeli nie utworz
	${if} ${FileExists} "C:\Devel\*.*"
		DetailPrint "Katalog C:\Devel istnieje ..."
		nsExec::ExecToStack "$SYSDIR\net.exe share"
		Pop $0
		Pop $1
		${StrStr} $R0 $1 "Devel"
		${if} $R0 == ""
			DetailPrint "Katalog nie jest udost?pniony w sieci dla usera 'vbox'. Udost?pniam ..."
			nsExec::Exec "$SYSDIR\net.exe share DEVEL=C:\Devel /GRANT:vbox,FULL"
		${Else}
			DetailPrint "Katalog jest ju? udost?pniony w sieci."
		${EndIf}
	${Else}
		CreateDirectory C:\Devel
		DetailPrint "Udostepniam katalog c:\Devel w sieci dla usera 'vbox' ..."
		nsExec::Exec "$SYSDIR\net.exe share DEVEL=C:\Devel /GRANT:vbox,FULL"
	${EndIf}

# Sprawdz czy odpowiednie porty sa pootwierane, jezeli nie otworz
# HTTP
	nsExec::ExecToStack "$SYSDIR\netsh advfirewall firewall show rule name=http"
	Pop $0
	Pop $1
	${StrStr} $R0 $1 "http"
	${if} $R0 == ""
		DetailPrint "Port 80 jest zamkni?ty. Otwieram ..."
		nsExec::ExecToStack "$SYSDIR\netsh advfirewall firewall add rule name=http localport=80 dir=in action=allow protocol=TCP"
	${Else}
		DetailPrint "Port 80 jest ju? otwarty."
	${EndIf}

	nsExec::ExecToStack "$SYSDIR\netsh advfirewall firewall show rule name=ssh"
	Pop $0
	Pop $1
	${StrStr} $R0 $1 "ssh"
	${if} $R0 == ""
		DetailPrint "Port 10022 jest zamkni?ty. Otwieram ..."
		nsExec::ExecToStack "$SYSDIR\netsh advfirewall firewall add rule name=ssh localport=10022 dir=in action=allow protocol=TCP"
	${Else}
		DetailPrint "Port 10022 jest ju? otwarty."
	${EndIf}

# INSTALACJA MANAGERA

	IfFileExists "$TEMP\wget.exe" file_exists fetch_again
		DetailPrint "Pobieram program 'wget' ... "
		fetch_again:
		InetLoad::load "http://users.ugent.be/~bpuype/cgi-bin/fetch.pl?dl=wget/wget-1.10.2.exe" "$TEMP\wget.exe"
			Pop $R0
			${ifNot} $R0 == "OK"
				DetailPrint "B??d podczas pobierania! Robie kolejn? pr?b? ..."
				IntOp $0 $0 + 1
				${ifNot} $0 == "3"
					goto fetch_again
				${Else}
					MessageBox MB_OK "Nie uda?o si? pobra? programu 'wget'. Przerywam!"
					Quit
				${EndIf}
			${Else}
				DetailPrint "Pobranie programu 'wget' zako?czone sukcesem!"
			${EndIf}

	file_exists:
	${GetParameters} $R0
	${if} $R0 = ""
		StrCpy $BM_VERSION "2_0_0_0"
	${Else}
		StrCpy $BM_VERSION $R0
	${EndIf}

		nsExec::ExecToLog "$TEMP\wget.exe -nv --no-check-certificate https://raw.github.com/xbojer/devel_vm/master/Devel_VM/publish/Application%20Files/Beta_Manager_$BM_VERSION/filelist.txt -O $TEMP\filelist.txt"
		Pop $R0
		StrCmp $R0 "0" +3
			MessageBox MB_OK "Nie uda?o si? pobra? listy wymaganych plik?w. Przerywam!"
			Quit

	ClearErrors
	FileOpen $0 "$TEMP\filelist.txt" r
	GetTempFileName $R0
	FileOpen $1 $R0 w

	loop:
		FileRead $0 $2
		IfErrors done
		${StrTrimNewLines} $3 $2
		DetailPrint "Pobieranie $3 ..."
		nsExec::ExecToLog '$TEMP\wget.exe -nv --no-check-certificate https://raw.github.com/xbojer/devel_vm/master/Devel_VM/publish/Application%20Files/Beta_Manager_$BM_VERSION/$3 -O "$INSTDIR\$3"'
		Pop $R0
		Goto loop

	done:
		FileClose $0
		FileClose $1
		Delete "$TEMP\filelist.txt"
		Delete $R0

	DetailPrint "Tworzenie deinstalatora..."
		WriteUninstaller "$INSTDIR\Uninstall.exe"
	CreateDirectory "$SMPROGRAMS\Beta Manager"
	DetailPrint "Tworzenie skr?t?w..."
		createShortCut "$SMPROGRAMS\Beta Manager\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
		createShortCut "$SMPROGRAMS\Beta Manager\Beta Manager.lnk" "$INSTDIR\Beta_Manager.exe" "" "$INSTDIR\beta.ico"
	DetailPrint "Dodanie wpisu do rejestru ..."
		WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "BetaManager" "$INSTDIR\Beta_Manager.exe"

exit_program:
	DetailPrint "Restartuje aplikacje Beta_Manager ..."
		Exec '"$INSTDIR\Beta_Manager.exe" /r'
		Quit

SectionEnd

Section "Uninstall"

	${If} ${RunningX64}
		DetailPrint "Maszyna 64 bitowa"
		SetRegView 64
		StrCpy $PROGRAM_FILES $PROGRAMFILES64
	${Else}
		StrCpy $PROGRAM_FILES $PROGRAMFILES
	${EndIf}

	DetailPrint "Zatrzymanie aplikacji Beta Manager ..."
		Processes::KillProcess "Beta_Manager.exe"

	DetailPrint "Usuni?cie plik?w aplikacji ..."
		RMDir /r "$SMPROGRAMS\Beta Manager"
		RMDir /r "$INSTDIR"
		RMDir "$INSTDIR"
	DetailPrint "Usuni?cie wpisu w rejestrze ..."
		${registry::DeleteValue} "HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run" "BetaManager" $R0
		${if} $R0 == "-1"
			DetailPrint "B??d podczas usuwania warto?ci 'HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run BetaManager'."
		${EndIf}
		DetailPrint "Usuwam u?ytkownika vbox ..."
		nsExec::Exec "$SYSDIR\net.exe user vbox /DELETE"
	DetailPrint "Usuwam zale?no?ci zwi?zane z userem 'vbox' ..."
		${registry::DeleteValue} "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList" "vbox" $R0
		${if} $R0 == "-1"
			DetailPrint "B??d podczas usuwania warto?ci 'HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList vbox'."
		${EndIf}
	DetailPrint "Usuwanie udzia?u sieciowego ..."
		nsExec::Exec "$SYSDIR\net.exe share DEVEL /DELETE"
	Quit
SectionEnd

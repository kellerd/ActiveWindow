GetMyWindow() {
WinGetActiveTitle, awt
return awt
}
GetPid() {
WinGet awt, PID
return awt
}
GetProcessName() {
WinGet awt, ProcessName
return awt
}
GetId() {
WinGet awt, ID
return awt
}
GetTransparent() {
MouseGetPos,,, MouseWin
WinGet, TransColor, TransColor, ahk_id %MouseWin% 
return TransColor
}
GetMyMouse(){
	MouseGetPos,x,y
	return x . "|" . y
}

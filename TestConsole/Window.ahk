GetMyWindow(awt) {
WinGetActiveTitle, awt
return awt
}
GetMyMouse(x,y){
	MouseGetPos,x,y
	return x . "|" . y
}

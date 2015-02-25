WatchWindowTitle:
WinGetActiveTitle, awt
return
WatchCursor:
MouseGetPos, xpos, ypos 
return

Init:
	SetTimer, WatchWindowTitle, 1000
	SetTimer, WatchCursor, 1000
return 
WatchCursor:
MouseGetPos, xpos, ypos 
return

#Persistent

Init:
	SetTimer, WatchCursor, 1000
return 
<?php
//Code written by JrMasterModelBuilder.

//This block helps prevent browser caching problems.
header("Cache-Control: no-cache");
header("Expires: -1");
header("Content-Type: text/plain");

//Get progress data if available.
if(file_exists("savegame.txt"))
{
	echo file_get_contents("savegame.txt");
}
else
{
	echo "&userID=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX&username=XXXXX&state=&total=&record=&rank=0&outof=0&domainname=localhost&";
}
?>
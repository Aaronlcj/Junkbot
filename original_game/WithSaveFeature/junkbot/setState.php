<?php
//Code written by JrMasterModelBuilder.
//This file relies on the .htaccess file to redirect calls to .asp files to .php files.

//This block helps prevent browser caching problems.
header("Cache-Control: no-cache");
header("Expires: -1");
header("Content-Type: text/plain");

//This block gets the variables from the game.
$state = isset($_POST['state']) ? $_POST['state'] : "";
$total = isset($_POST['total']) ? $_POST['total'] : "";
$record = isset($_POST['record']) ? $_POST['record'] : "";
$rank = isset($_POST['rank']) ? $_POST['rank'] : "";
$outof = isset($_POST['outof']) ? $_POST['outof'] : "";

//Make sure data is passed.
if($state == "" && $total == "" && $record == "" && $rank == "" && $outof == "")
{
	exit();
}

//This block calls the variables from above and puts then into a string.
//Constant variables are also added here.
$savestate = "&userID=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX&username=XXXXX&state=".$state."&total=".$total."&record=".$record."&rank=".$rank."&outof=".$outof."&domainname=localhost&";

//This block calls the string above and writes it to savegame.txt overwriting the previous data.
$writefile = "savegame.txt";
$fh = fopen($writefile, 'w');
fwrite($fh, $savestate); 
fclose($fh);
?>
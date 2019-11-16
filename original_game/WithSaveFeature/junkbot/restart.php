<?php
if(isset($_GET['confirm']) && $_GET['confirm'] == "true")
{
	$savestate="&userID=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX&username=XXXXX&state=&total=&record=&rank=0&outof=0&domainname=localhost&";
	$writefile="savegame.txt";
	$fh=fopen($writefile, 'w');
	fwrite($fh, $savestate); 
	fclose($fh);
	?>
<html>
<head>
<title>Restart Game</title>
</head>
<body>
Game restarted.
</body>
</html>
	<?php
}
else
{
	?>
<html>
<head>
<title>Restart Game</title>
</head>
<body>
<a href="?confirm=true">Confirm Restart?</a>
</body>
</html>
	<?php
}
?>
// Variables
$GameConnections = new SimSet();

// Functions
function getConnectionCount()  	
{
  return $GameConnections.getCount(); 	
}

function getConnection(%i)
{ 
  return $GameConnections.getObject(%i); 
}

function getIsServer()
{ 
  return $ServerLocal;
}
	
function newConnection(%newClient)
{ 
  $GameConnections.add(%newClient);
}

// Server Function
function onClientConnected(%newClient)
{
  // Record the connection id
  newConnection(%newClient);
}

// This error always shows up, let's use a dummy fix (I hate console errors!)
// common/gameScripts/client/message.cs (13): Unable to find function onServerMessage
// This is the actual function call
// onServerMessage(detag(%msgString));
function onServerMessage()
{
}

function onConnect()
{
}

function setLobbyName()
{
  %name = lobbyName.getText();
  %server = getConnection(0);
  %client = getConnection(1);
  if(getIsServer())
    commandToClient(%client, 'receiveMessage', %text);
  else
    commandToServer('receiveMessage', %text);
}

function clientCmdReceiveMessage(%text)
{
  netText.setText(%text);
}

function serverCmdReceiveMessage(%client, %text)
{
  netText.setText(%text);
}

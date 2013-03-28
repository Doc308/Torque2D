//-----------------------------------------------------------------------------
// Copyright (c) 2013 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------
function NetworkToy::create()
{
   // Network Prefs
   $pref::Master0 = "2:master.garagegames.com:28002";
   $pref::Net::LagThreshold = 400;
   $pref::Net::Port = 28000;
   $pref::Server::RegionMask = 2;
   $pref::Net::RegionMask = 2;
   
   $pref::Server::Name = "T2D Server";
   $pref::Player::Name = "T2D Player";
   
   // Set up networking
   setNetPort(0);
   
   // Simple network scripts
   exec("./scripts/simplechat.cs");
   exec("./scripts/simpleSetup.cs");
   
   // Gui Related scripts
   exec("./scripts/guiProfiles.cs");
   exec("./scripts/messageBox.cs");
   exec("./scripts/chatGui.cs");
   exec("./scripts/startServerGui.cs");
   exec("./scripts/joinServerGui.cs");
   
   // TGB Network structure
   exec("./scripts/client/chatClient.cs");
   exec("./scripts/client/client.cs");
   exec("./scripts/client/message.cs");
   exec("./scripts/client/serverConnection.cs");
   
   //exec("./scripts/server/banlist.cs"); //Nothing in this file
   exec("./scripts/server/chatServer.cs");
   exec("./scripts/server/clientConnection.cs");
   exec("./scripts/server/kickban.cs");
   exec("./scripts/server/message.cs");
   exec("./scripts/server/server.cs");
   
   // Load and configure the Network Menu
   Sandbox.add( TamlRead("./gui/NetworkMenu.gui.taml") );
   // Load and configure the Start Server Menu
   Sandbox.add( TamlRead("./gui/startServer.gui.taml") );
   // Load and configure the Join Server Menu
   Sandbox.add( TamlRead("./gui/joinServer.gui.taml") );
   // Load and configure the Chat Menu
   Sandbox.add( TamlRead("./gui/chatGui.gui.taml") );
   // Load and configure the Waiting for server Menu
   Sandbox.add( TamlRead("./gui/waitingForServer.gui.taml") );
   // Load and configure the MessageBokOk gui
   Sandbox.add( TamlRead("./gui/messageBoxOk.gui.taml") );
   
   Canvas.pushDialog(NetworkMenu);
}

function NetworkToy::reset()
{
}

function NetworkToy::destroy()
{
}

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

function SpriterToy::create( %this )
{
    echo("@@@@@@@ Creating Spriter @@@@@@@");
    $SpriterToy = Sandbox.ActiveToy;
    exec("./scripts/Spriter.cs");
    exec("./scripts/SpriterData.cs");
    exec("./scripts/SpriterEntity.cs");
    
    %object = new Sprite();
    
    //Load in the SCML file(s)
    %dir = getCurrentDirectory(); // Limit search to program directory hub
    %file = findFirstFile(%dir @ "/*.SCML");
    
    if(isFile(%file))
      %Spriter = Spriter::create();
      
    %SpriterData = %Spriter.load(%file);
    
    $SpriterEntity = %Spriter.createEntity(0);

    SpriterEntity::attachToScene($SpriterEntity);
    
    // Set the sandbox drag mode availability.
    Sandbox.allowManipulation( pan );
    
    // Set the manipulation mode.
    Sandbox.useManipulation( pan );
    
    // Reset the toy.
    SpriterToy.reset();
}


//-----------------------------------------------------------------------------

function SpriterToy::destroy( %this )
{
}

//-----------------------------------------------------------------------------

function SpriterToy::reset( %this )
{
    // Clear the scene.
    SandboxScene.clear();
}

//-----------------------------------------------------------------------------
/* Sprite Creation for Sprite module
function SpriterToy::createStaticSprite( %this )
{    
    // Create the sprite.
    %object = new Sprite();
    
    // Always try to configure a scene-object prior to adding it to a scene for best performance.

    // Set the position.
    %object.Position = "-25 0";
        
    // If the size is to be square then we can simply pass a single value.
    // This applies to any 'Vector2' engine type.
    %object.Size = 40;
    
    // Set the sprite to use an image.  This is known as "static" image mode.
    %object.Image = "ToyAssets:Tiles";
    
    // We don't really need to do this as the frame is set to zero by default.
    %object.Frame = 0;
        
    // Add the sprite to the scene.
    SandboxScene.add( %object );    
}*/

function NoiseToy::create(%this)
{
    // Load scripts.
    exec( "./scripts/Noise.cs" );
   
    // Set the sandbox drag mode availability.
    Sandbox.allowManipulation( pan );
    Sandbox.allowManipulation( pull );
    
    // Set the manipulation mode.
    Sandbox.useManipulation( pan );
    
    // Configure the toy.
    NoiseToy.InterpolateMode = "None";
    NoiseToy.Points = 10;
    NoiseToy.SizeX = 10;
    
    // Add the configuration options.
    addSelectionOption( "None,Linear,Cosine", "Interpolate Mode", 3, "setInterpolateMode", true, "Sets the interpolation mode." );
    addNumericOption("Size X", 1, 100, 1, "setSizeX", NoiseToy.SizeX, true, "Sets the horizontal size." );
    
    // Reset the toy.
    %this.reset();
}

function NoiseToy::destroy(%this)
{
}

function NoiseToy::reset(%this)
{
    // Clear the scene.
    SandboxScene.clear();
    
    // Create the background.
    %this.createBackground();
}

function NoiseToy::setInterpolateMode(%this, %value)
{
   NoiseToy.InterpolateMode = %value;
}

function NoiseToy::createBackground(%this)
{
   //Create the CompositeSprite
   %object = new CompositeSprite();
   %object.setBatchLayout("rect");
	%object.setDefaultSpriteStride(4, 4);
	%object.setDefaultSpriteSize(4, 4);
	
	//create the coordinates needed for placement
	%this.createPolyList();
	
	//loop through along x
	for(%i=0;%i<=%this.SizeX;%i++)
	{
	   %posX = %i; //x position just follows our loop
	   %posY = $noiseArray[%i]; //y position stored based on x position array value
	   
	   //loop through to fill below our height value
	   for(%j=0;%posY>=0;%j++)
	   {
	      %pos = %posX SPC %posY;
	      %object.addSprite(%pos);
	      %object.setSpriteImage("ToyAssets:Blocks", 5);
	      %posY--; //decrement our height value
	   }
	}
   SandboxScene.add(%object);
}

function NoiseToy::setSizeX(%this, %val)
{
   %this.SizeX = %val;
}

function NoiseToy::createNoInterpolate(%this, %x)
{
   return %x;
}

function NoiseToy::createLinearInterpolate(%this, %a, %b, %x)
{
   %result = %a*(1-%x) + %b*%x;
   return %result;
}

function NoiseToy::createCosineInterpolate(%this, %a, %b, %x)
{
   %ft = %x * 3.1415927;
   %f = (1 - mCos(%ft)) * 0.5;
   
   %result = %a*(1-%f) + %b*%f;
   return %result;
}

//Create the coordinates for sprite placement
function NoiseToy::createPolyList(%this)
{
   %x = %this.SizeX;
   
   //seed each function with our size
   //first generate random noise
   %this.createNoise(%x);
   
   //smooth out the noise
   %this.Smooth(%noise, %x);
   
   //interpolate the smooth noise
   %this.Interpolate(%smooth, %x);
   
   //prepare for CompositeSprite integration
   %this.Prepare(%smooth, %x);
}

function NoiseToy::createNoise(%this, %x)
{
   //Start our first point at 0,0
   $noiseArray[0] = "0";
   
   //loop through along the horizontal axis
   for(%i=1;%i<%x;%i++)
   {
      %rand = getRandomF(0,1); //Gets a floating random between 0 & 1
      $noiseArray[%i] = %rand; //assign the coordinate to the array
   }
   //add in the last point at 0
   $noiseArray[%x] = "0";
}

function NoiseToy::Smooth(%this, %noise, %x)
{
   //loop through along x
   for(%i=1;%i<%x;%i++)
   {
      //transition between the values to smooth out the points
      %point = $noiseArray[%i]/2 + $noiseArray[%i-1]/4 + $noiseArray[%i+1]/4;
      $noiseArray[%i] = %point; // Set up coordinates based on the horizontal value and smoothed point
   }
}

function NoiseToy::Interpolate(%this, %smooth, %x)
{
   //loop through along x
   for(%i=1;%i<%x;%i++)
   {
      %rand = getRandomF(0,1); //Gets a floating random between 0 & 1
      // Create the appropriate Interpolation
      // Note that on Linear and Cosine we compare to the previous point
      switch$( NoiseToy.InterpolateMode )
      {
         case "None":
           %point = %this.createNoInterpolate($noiseArray[%i]);
            
         case "Linear":
           %point = %this.createLinearInterpolate($noiseArray[%i-1], $noiseArray[%i], %rand);
            
         case "Cosine":
           %point = %this.createCosineInterpolate($noiseArray[%i-1], $noiseArray[%i], %rand);
      }
      $noiseArray[%i] = %point; // Set up coordinates based on the horizontal value and interpolated point
   }
}

function NoiseToy::Prepare(%this, %interpolate, %x)
{
   for(%i=0;%i<=%x;%i++)
   {
      //Round the value of our y * 10 (to get height)
      %y = mRound($noiseArray[%i]*10);
      $noiseArray[%i] = %y; //set our coordinate
   }
}

// Class to represent spriter entities
function SpriterEntity::create(%entity, %data)
{
   %this = new ScriptObject()
   {
      class = "SpriterEntity";
      x = 0;
		y = 0;
		data = %data;
      entity = %entity;
   };
   %this.spriteSet = new SimSet();
   return %this;
}

function SpriterEntity::addSprite(%this, %sprite)
{
   %this.spriteSet.add(%sprite);
}

function SpriterEntity::attachToScene(%this)
{
   // Get the entity data from the spriter data structure
   %ent = %this.data.entitiesSet.getObject(%this.entity);
   
   %anim = %ent.animationSet.getObject(0);
   
   if (%anim > 0)
   {
      %factor = 0.05;
      
      // Find key 0 of the mainline animation
      echo("maineline count: " @ %anim.mainline.keySet.getCount());
      %key = %anim.mainline.keySet.getObject(0);
      
      // Load each object into a sprite and set its attributes.
      %maxObjects = %key.objectsSet.getCount();
      
      for (%i = 0; %i < %maxObjects; %i++)
      {
         // For each object in the mainline, determine if it is an object reference
         // (persistant object) or an object (transient object).
         
         %object = %key.objectsSet.getObject(%i);
         
         if (%object.reference)
         {
            // For all persistant objects:
            // Look up the timeline and key to find the object data
            %timeline = %anim.timelineSet.getObject(%object.timeline);
            %tkey = %timeline.keySet.getObject(0);
            %obj = %tkey.objectsSet.getObject(0);
            
            // Look up the image size
            %mapSize = %this.data.imageSize[%obj.folder, %obj.file];
            %mapName = %this.data.assetName[%obj.folder, %obj.file];
            echo("name: " @ %mapName);
            
            echo("bitmapSize =" SPC %mapSize);
            %bitmapCenter = (getWord(%mapSize, 0) * 0.5) SPC (getWord(%mapSize, 1) * 0.5);
            echo("bitmapCenter =" SPC %bitmapCenter);
            %pivotOffset = ((getWord(%mapSize, 0) * %obj.pivot_x) - getWord(%bitmapCenter, 0)) SPC (getWord(%mapSize, 1) * %obj.pivot_y - getWord(%bitmapCenter, 1));
            echo("pivotOffset =" SPC %pivotOffset);
            
            // Get the relitive position
            %this.relitivePosition[%i] = %obj.x SPC %obj.y;
            
            // Rotate/scale the sprite
            %rad = mDegToRad(%sprite.Rotation);
            %cos = mCos(%rad);
            %sin = mSin(%rad);
            %x = getWord(%pivotOffset, 0);
            %y = getWord(%pivotOffset, 1);
            
            %pivotOffsetAdjustment = (%x - ((%x * %cos) - (%y * %sin))) SPC (%y - ((%y * %cos) + (%x * %sin)));
            
            // Calculate the position
            %obj.x = (%obj.x - getWord(%pivotOffset, 0) + getWord(%pivotOffsetAdjustment, 0)) * %factor;
            %obj.y = (%obj.y - getWord(%pivotOffset, 1) + getWord(%pivotOffsetAdjustment, 1)) * -%factor;
            
            echo("Object (" @ %obj.x @ ", " @ %obj.y @ ")");
            
            // Create the sprite
            %sprite = new Sprite();
            %sprite.setBodyType( static );
            %sprite.Image = %mapName;
            %sprite.Angle = -%obj.angle;
            echo("angle =" SPC -%obj.angle);
            %sprite.Position = %obj.x SPC %obj.y;
            echo("Position (" @ %sprite.Position @ ")");
            
            %sprite.size = (getWord(%mapSize, 0) * %factor) SPC (getWord(%mapSize, 1) * %factor);
            
            //SandboxScene.add(%sprite);
            
            echo("Image size =" SPC %mapSize);
            echo("Sprite size =" SPC %sprite.size);

            %this.spriteSet.add(%sprite);
         }
         else
         {
            // For all transient objects:
            // Look up the image
            %mapSize = %data.imageSize[%object.folder, %object.file];
            %mapName = %data.assetName[%obj.folder, %obj.file];

            // Get the relitive position
            %this.relx[%i] = %object.x;
            %this.rely[%i] = %object.y;

            // Calculate the position
            %object.x = (%this.x - %object.x) * %factor;
            %object.y = (%this.y - %object.y) * %factor;
            
            echo("Object (" @ %object.x @ ", " @ %object.y @ ")");         

            // Create the sprite
            %sprite = new Sprite();
            %sprite.Image = %mapName;
            %sprite.Position = %object.x SPC %object.y; 

            // Rotate/scale the sprite
            %sprite.Angle = -%object.angle;
            %size = %sprite.size;
            %sprite.size = (getWord(%size, 0) * %object.x_scale) SPC (getWord(%size, 1) * %object.y_scale);

            %this.spriteSet.add(%sprite);
         }
      }
   }
   %n = %this.spriteSet.getCount();
   
   for (%i = 0; %i < %n; %i++)
   {
      %obj = %this.spriteSet.getObject(%i);
      SandboxScene.add(%obj);
      //%this.spriteSet.getObject(%i).attachToScene();
   }
}

function SpriterEntity::moveTo(%this, %x, %y)
{
   %this.x = %x;
   %this.y = %y;
}

function SpriterEntity::loadKeyFrame(%this, %n)
{
   %anim = %this.anim;
   
   if (%anim)
   {
      %factor = 0.05;   
      
      // Find key 0 of the mainline animation
      echo(%anim.mainline.keySet.getCount());
      %key = %anim.mainline.keySet.getObject(%n);

      // Load each object into a sprite and set its attributes.
      %maxObjects = %key.objectsSet.getCount();

      for (%i = 0; %i < %maxObjects; %i++)
      {
         // For each object in the mainline, determine if it is an object reference
         // (persistant object) or an object (transient object).

         %object = %key.objectsSet.getObject(%i);
         
         if (%object.reference)
         {
            // For all persistant objects:
            // Look up the timeline and key to find the object data
            %timeline = %anim.timelineSet.getObject(%object.timeline);
            %tkey = %timeline.keySet.getObject(0);
            %obj = %tkey.objectsSet.getObject(0);

            // Look up the image map
            %mapSize = %this.data.imageSize[%obj.folder, %obj.file];
            %mapName = %this.data.assetName[%obj.folder, %obj.file];

            echo("bitmapSize =" SPC %mapSize);
            %bitmapCenter = (getWord(%mapSize, 0) * 0.5) SPC (getWord(%mapSize, 1) * 0.5);
            echo("bitmapCenter =" SPC %bitmapCenter);
            %pivotOffset = ((getWord(%mapSize, 0) * %obj.pivot_x) - getWord(%bitmapCenter, 0)) SPC (getWord(%mapSize, 1) * %obj.pivot_y - getWord(%bitmapCenter, 1));
            echo("pivotOffset =" SPC %pivotOffset);

            // Get the relitive position
            %this.relitivePosition[%i] = %obj.x SPC %obj.y;

            // Rotate/scale the sprite
            
            %rad = mDegToRad(%sprite.Rotation);
            %cos = mCos(%rad);
            %sin = mSin(%rad);
            %x = getWord(%pivotOffset, 0);
            %y = getWord(%pivotOffset, 1);

            %pivotOffsetAdjustment = (%x - ((%x * %cos) - (%y * %sin))) SPC (%y - ((%y * %cos) + (%x * %sin)));

            // Calculate the position
            %obj.x = (%obj.x - getWord(%pivotOffset, 0) + getWord(%pivotOffsetAdjustment, 0)) * %factor;
            %obj.y = (%obj.y - getWord(%pivotOffset, 1) + getWord(%pivotOffsetAdjustment, 1)) * -%factor;

            echo("Object (" @ %obj.x @ ", " @ %obj.y @ ")");

            // Create the sprite
            %sprite = new Sprite();
            %sprite.Image = %mapName;
            %sprite.Angle = -%obj.angle;
            echo("angle =" SPC -%obj.angle);
            %sprite.Position = %obj.x SPC %obj.y;
            echo("Position (" @ %sprite.Position @ ")");

            //%sprite.rotateAroundPoint(%pivotOffset, %obj.angle);
            
            //%sprite.size = (getWord(%size, 0) * %obj.x_scale) SPC (getWord(%size, 1) * %obj.y_scale);

            %sprite.size = (getWord(%mapSize, 0) * %factor) SPC (getWord(%mapSize, 1) * %factor);

            echo("Image size =" SPC %mapSize);
            echo("Sprite size =" SPC %sprite.size);

            %this.spriteSet.add(%sprite);
         }
         else
         {
            // For all transient objects:
            // Look up the image map
            %mapSize = %data.imageSize[%object.folder, %object.file];
            %mapName = %this.data.assetName[%obj.folder, %obj.file];

            // Get the relitive position
            %this.relx[%i] = %object.x;
            %this.rely[%i] = %object.y;

            // Calculate the position
            %object.x = (%this.x - %object.x) * %factor;
            %object.y = (%this.y - %object.y) * %factor;
            
            echo("Object (" @ %object.x @ ", " @ %object.y @ ")");         
            
            // Create the sprite
            %sprite = new Sprite();
            %sprite.Image = %mapName;
            %sprite.Postion = %object.x SPC %object.y; 

            // Rotate/scale the sprite
            %sprite.Angle = -%object.angle;
            %size = %sprite.size;
            %sprite.size = (getWord(%size, 0) * %object.x_scale) SPC (getWord(%size, 1) * %object.y_scale);

            %this.spriteSet.add(%sprite);
         }
      }

      // Look up the next key frame to set the timer
      %next_key = %anim.mainline.keySet.getObject(%n + 1);
      
      if (%next_key)
         %this.setTimerOn((%next_key.time - %key.time)/100);
      else
         %this.setTimerOn((%anim.length - %key.time)/100);

      %this.key = %n;
   }
}

function SpriterEntity::setAnim(%this, %anim)
{   
   // Find the requested animation
   if (isInt(%anim))
      %this.anim = %this.entity.animationSet.getObject(%anim);
   else
      %this.anim = %this.entity.animationSet.findObjectByInternalName(%anim);

   %this.loadKeyFrame(0);
}

function SpriterEntity::onTimer(%this)
{
   echo("Got Timer");

   %this.loadKeyFrame(%this.key + 1);
}

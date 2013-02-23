// Class to represent spriter entities
function SpriterEntity::create(%entity, %data)
{
   %this = new SceneObject()
   {
      x = 0;
		y = 0;
		data = %data;
      entity = %entity;
   };
   %this.SpriteSet = new SimSet();
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
      echo(%anim.mainline.keySet.getCount());
      %key = %anim.mainline.keySet.getObject(%n); // What is %n??
      
      // Load each object into a sprite and set its attributes.
      %max_objects = %key.objectsSet.getCount();
      
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
            
            // Look up the image
            %map = %this.data.imageMap[%obj.folder, %obj.file]; // Legacy format, should be Image
            
            // Get the bitmap size (LEGACY CALLS!! Verify)
            %bitmapSize = %map.getSrcBitmapSize();
            echo("bitmapSize =" SPC %bitmapSize);
            %bitmapCenter = (getWord(%bitmapSize, 0) * 0.5) SPC (getWord(%bitmapSize, 1) * 0.5);
            echo("bitmapCenter =" SPC %bitmapCenter);
            %pivotOffset = ((getWord(%bitmapSize, 0) * %obj.pivot_x) - getWord(%bitmapCenter, 0)) SPC (getWord(%bitmapSize, 1) * %obj.pivot_y - getWord(%bitmapCenter, 1));
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
            %sprite.Image = %map;
            %sprite.Rotation = -%obj.angle;
            echo("angle =" SPC -%obj.angle);
            %sprite.Position = %obj.x SPC %obj.y;
            echo("Position (" @ %sprite.Position @ ")");
            
            //%sprite.rotateAroundPoint(%pivotOffset, %obj.angle);
            //%sprite.size = (getWord(%size, 0) * %obj.x_scale) SPC (getWord(%size, 1) * %obj.y_scale);
            
            %sprite.size = (getWord(%bitmapSize, 0) * %factor) SPC (getWord(%bitmapSize, 1) * %factor);
            
            echo("Image size =" SPC %map.getSrcBitmapSize());
            echo("Sprite size =" SPC %sprite.size);
            
            %this.spriteSet.add(%sprite);
         }
         else
         {
            // For all transient objects:
            // Look up the image
            %map = %data.imageMap[%object.folder, %object.file];

            // Get the relitive position
            %this.relx[%i] = %object.x;
            %this.rely[%i] = %object.y;

            // Calculate the position
            %object.x = (%this.x - %object.x) * %factor;
            %object.y = (%this.y - %object.y) * %factor;
            
            echo("Object (" @ %object.x @ ", " @ %object.y @ ")");         

            // Create the sprite
            %sprite = new Sprite();
            %sprite.Image = %map;
            %sprite.Postion = %object.x SPC %object.y; 

            // Rotate/scale the sprite
            %sprite.Rotation = -%object.angle;
            %size = %sprite.size;
            %sprite.size = (getWord(%size, 0) * %object.x_scale) SPC (getWord(%size, 1) * %object.y_scale);

            %this.spriteSet.add(%sprite);
         }
      }
   }
   %n = %this.spriteSet.getCount();
   
   for (%i = 0; %i < %n; %i++)
   {
      %this.spriteSet.getObject(%i).addToScene();
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
      %max_objects = %key.objectsSet.getCount();

      for (%i = 0; %i < %max_objects; %i++)
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
            %map = %this.data.imageMap[%obj.folder, %obj.file];

            //   Get the bitmap size
            %bitmapSize = %map.getSrcBitmapSize();
            echo("bitmapSize =" SPC %bitmapSize);
            %bitmapCenter = (getWord(%bitmapSize, 0) * 0.5) SPC (getWord(%bitmapSize, 1) * 0.5);
            echo("bitmapCenter =" SPC %bitmapCenter);
            %pivotOffset = ((getWord(%bitmapSize, 0) * %obj.pivot_x) - getWord(%bitmapCenter, 0)) SPC (getWord(%bitmapSize, 1) * %obj.pivot_y - getWord(%bitmapCenter, 1));
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
            %sprite.Image = %map;
            %sprite.Rotation = -%obj.angle;
            echo("angle =" SPC -%obj.angle);
            %sprite.Position = %obj.x SPC %obj.y;
            echo("Position (" @ %sprite.Position @ ")");

            //%sprite.rotateAroundPoint(%pivotOffset, %obj.angle);
            
            //%sprite.size = (getWord(%size, 0) * %obj.x_scale) SPC (getWord(%size, 1) * %obj.y_scale);

            %sprite.size = (getWord(%bitmapSize, 0) * %factor) SPC (getWord(%bitmapSize, 1) * %factor);

            echo("Image size =" SPC %map.getSrcBitmapSize());
            echo("Sprite size =" SPC %sprite.size);

            %this.spriteSet.add(%sprite);
         }
         else
         {
            // For all transient objects:
            // Look up the image map
            %map = %data.imageMap[%object.folder, %object.file];

            // Get the relitive position
            %this.relx[%i] = %object.x;
            %this.rely[%i] = %object.y;

            // Calculate the position
            %object.x = (%this.x - %object.x) * %factor;
            %object.y = (%this.y - %object.y) * %factor;
            
            echo("Object (" @ %object.x @ ", " @ %object.y @ ")");         
            
            // Create the sprite
            %sprite = new Sprite();
            %sprite.Image = %map;
            %sprite.Postion = %object.x SPC %object.y; 

            // Rotate/scale the sprite
            %sprite.Rotation = -%object.angle;
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
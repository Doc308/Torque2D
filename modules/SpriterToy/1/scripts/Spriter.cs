function Spriter::create()
{
   %this = new ScriptObject()
   {
      class = "Spriter";
      currentData = null;
   };
   %this.dataSet = new SimSet();
   
   return %this;
}

function isInt(%str)
{
   return (%str $= %str * 1);
}

function Spriter::load(%this, %file)
{
   %data = %this.dataSet.findObjectByInternalName(%file); 
   echo("data first set at: " @ %data);
   
   if (!%data)
   {
      %data = SpriterData::create(%file);
      %this.dataSet.add(%data);
      %this.currentData = %data;
   }
   
   return %data;
}

function Spriter::attachToScene(%this)
{
   %count = %this.objCount;
   for (%a = 0; %a < %count; %a++)
   {
      SandboxScene.add(%this.object[%a]);
   }
}

// Spriter::createEntity() creates an instance of a SpriterEntity object
// with the given entity name or number, animation number, and the
// specified SpriterData object.  If no data object is given, the last one
// accessed will be reused.  If no animation number is given, 0 will be used.
function Spriter::createEntity(%this, %entity, %data)
{
   if (!%data)
     %data = %this.currentData;
     
   %entity = SpriterEntity::create(%entity, %data);
   
   return %entity;
}
// Spriter Data, will load the SCML file and prepare it for use
function SpriterData::create(%file)
{
   //Create the initial Script Object to house our data
   %this = new ScriptObject()
   {
      class = "SpriterData";
      file = %file;
		line = "";
		scml_version = "0";
		generator = "Spriter";
		generator_version = "1.0";
		folderSet = null;
		entitySet = null;
   };
   %this.entitiesSet = new SimSet();
   
   if(%this.load(%file))
     %this.setInternalName(%file);
   
   return %this;
}

// Populate all necessary SpriterData structures
function SpriterData::load(%this, %file)
{
   %result = false;
   
   %this.xml = new SimXMLDocument();
   %xml = %this.xml;

   if (%xml.loadFile(%file))
   {
      if(%xml.pushFirstChildElement("spriter_data"))
      {
         //Retrieve metadata about the program that created this SCML file
         if (%xml.attributeExists("scml_version"))
           %SpriterData.scml_version = %xml.attributeF32("scml_version");
        
         if (%xml.attributeExists("generator"))
           %SpriterData.generator = %xml.attribute("generator");
         
         if (%xml.attributeExists("generator_version"))
           %SpriterData.generator_version = %xml.attribute("generator_version");
         
         %this.loadFolders();
         %this.loadEntities();
         
         if(%this.folderSet | %this.entitySet)
           %result = true;
      }
   }
   //%xml.delete();
   else
   {
     echo("Invalid file, or incorrect path!");   
   }
   
   return %result;
}

// Read the xml tags representing folders and their attributes.
function SpriterData::loadFolders(%this)
{
   %xml = %this.xml;
   // Attempt to read any folder data from the SCML file
   %folder = %xml.pushFirstChildElement("folder");
   
   if(%folder)
     %folderSet = new SimSet();
   else
     %folderSet = null;
   
   if(%folderSet)
   {  
      while(%folder)
      {
         %folderObject = SpriterFolder::create();
         
         if (%folderObject)
         {
            %attribute = %xml.firstAttribute();
         
            while (%attribute !$= "")
            {
               switch$(%attribute)
               {
                  case "id":
                     %folderObject.id = %xml.attributeS32(%attribute);
                     //echo(%attribute SPC "=" SPC %folderObject.id);
                  case "name":
                     %folderObject.name = %xml.attribute(%attribute);
                     //echo(%attribute SPC "=" SPC %folderObject.name);
                  default:
                     echo("Unknown attribute in folder element");
               }
               %attribute = %xml.nextAttribute();
            }
            %folderObject.fileSet = %this.loadFiles(%folderObject.id);
            %folderSet.add(%folderObject);
         }
         %folder = %xml.nextSiblingElement("folder");
      }
   }
   %xml.popElement();
   return %folderSet;
}

// Read the xml tags representing files and their attributes.
function SpriterData::loadFiles(%this, %folderId)
{
   %xml = %this.xml;
   %set = null;
   
   %file = %xml.pushFirstChildElement("file");
   
   if(%file)
   {
      %set = new SimSet();
      while(%file)
      {
         %fileObject = SpriterFile::create();
         if (%fileObject)
         {
            %attribute = %xml.firstAttribute();
            while (%attribute !$= "")
            {
               switch$(%attribute)
               {
                  case "type":
                     %fileObject.type = %xml.attribute(%attribute);
                     //echo(%attribute SPC "=" SPC %fileObject.type);
                  case "id":
                     %fileObject.id = %xml.attributeS32(%attribute);
                     //echo(%attribute SPC "=" SPC %fileObject.id);
                  case "name":
                     %fileObject.name = %xml.attribute(%attribute);
                     //echo(%attribute SPC "=" SPC %fileObject.name);
                  case "width":
                     %fileObject.width = %xml.attributeS32(%attribute);
                     //echo(%attribute SPC "=" SPC %fileObject.width);
                  case "height":
                     %fileObject.height = %xml.attributeS32(%attribute);
                     //echo(%attribute SPC "=" SPC %fileObject.height);
                  case "pivot_x":
                     %fileObject.pivot_x = %xml.attributeF32(%attribute);
                     //echo(%attribute SPC "=" SPC %fileObject.pivot_x);
                  case "pivot_y":
                     %fileObject.pivot_y = %xml.attributeF32(%attribute);
                     //echo(%attribute SPC "=" SPC %fileObject.pivot_y);
                  default:
                     echo("Unknown attribute in file element");
               }
               %attribute = %xml.nextAttribute();
            }
            // Use the path of the SCML file, attach a /, load up the images into TAML
            %path = filePath(%this.file) @ "/" @ %fileObject.name;
            if(isFile(%path))
            {      
               echo("File path " @ %path);
               
               // Create a new Image Asset
               %asset = new ImageAsset();
               
               %asset.AssetName = fileBase(%path);
               %asset.setImageFile(%path);
               
               // Additional spriter specific data
               %asset.type = %fileObject.type;
               %asset.id = %fileObject.id;
               %asset.name = %fileObject.name;
               %asset.pivot = %fileObject.pivot_x SPC %fileObject.pivot_y;
               
               %tamlFile = filePath(%path) @ "/" @ fileBase(%path) @ ".asset.taml";
               //%this.objTamlFile[%folderId, %fileObject.id] = %tamlFile;
               TamlWrite(%asset, %tamlFile);
               
               // Define the new Taml Asset to the Database
               AssetDatabase.addSingleDeclaredAsset($SpriterToy, %tamlFile);
               
               
               // Query the Database for our asset
               %query = new AssetQuery();
               %count = AssetDatabase.findAssetName(%query, fileBase(%path));
               %newAsset = %query.getAsset(0);
               %obj = AssetDatabase.acquireAsset(%newAsset);
               %asset.size = %obj.getImageSize();
               echo("Size: " @ %obj.getImageSize());
               AssetDatabase.releaseAsset(%newAsset);
               
               // Store the full information into an array for access
               %this.image[%folderId, %fileObject.id] = %asset;
            }
            
            %set.add(%fileObject);
         }
         %file = %xml.nextSiblingElement("file");
      }
   }
   %xml.popElement();
   return %set;
}

// Read the xml tags representing entities and their attributes.
function SpriterData::loadEntities(%this)
{
   %xml = %this.xml;
   %set = null;
   
   %entity = %xml.pushFirstChildElement("entity");
   if (%entity)
   {
      %set = new SimSet();
      while (%entity)
      {
         %entityObject = SpriterEnt::create();
         if (%entityObject)
         {
            %attribute = %xml.firstAttribute();
            while (%attribute !$= "")
            {
               switch$(%attribute)
               {
                  case "id":
                     %entityObject.id = %xml.attributeS32(%attribute);
                     //   echo(%attribute SPC "=" SPC %entityObject.id);
                  case "name":
                     %entityObject.name = %xml.attribute(%attribute);
                     //   echo(%attribute SPC "=" SPC %entityObject.name);
                  default:
                     echo("Unknown attribute in entity element");
               }
               %attribute = %xml.nextAttribute();
            }
            %this.entitiesSet.add(%entityObject);
            %entityObject.animationSet = %this.loadAnimations();
            %set.add(%entityObject);
         }
         %entity = %xml.nextSiblingElement("entity");
      }
      %xml.popElement();
   }
   return %set;
}

// Read the xml tags representing animations and their attributes.
function SpriterData::loadAnimations(%this)
{
   %xml = %this.xml;
   %set = null;
   
   %anim = %xml.pushFirstChildElement("animation");
   if (%anim)
   {
      %set = new SimSet();
      while (%anim)
      {
         %animObject = SpriterAnimation::create();
         if(%animObject)
         {
            %attribute = %xml.firstAttribute();
            while (%attribute !$= "")
            {
               switch$(%attribute)
               {
                  case "id":
                     %animObject.id = %xml.attributeS32(%attribute);
                     //echo("animation" SPC %attribute SPC "=" SPC %animObject.id);
                  case "name":
                     %animObject.name = %xml.attribute(%attribute);
                     //echo(%attribute SPC "=" SPC %animObject.name);
                  case "length":
                     %animObject.length = %xml.attributeS32(%attribute);
                     //echo(%attribute SPC "=" SPC %animObject.length);
                  case "looping":
                     %animObject.looping = %xml.attributeS32(%attribute);
                     //echo(%attribute SPC "=" SPC %animObject.looping);
                  default:
                     echo("Unknown attribute in animation element");
               }
               %attribute = %xml.nextAttribute();
            }
            %animObject.mainline = %this.loadMainline();
            %animObject.timelineSet = %this.loadTimelines();
            
            %animObject.setInternalName(%animObject.name);
            %set.add(%animObject);
         }
         %anim = %xml.nextSiblingElement("animation");
      }
      %xml.popElement();
   }
   return %set;
}

// Read the xml tags representing the mainline and its attributes.
function SpriterData::loadMainline(%this)
{
   %xml = %this.xml;
   %set = null;
   
   %mainline = %xml.pushFirstChildElement("mainline");
   if (%mainline)
   {
      %mainlineObject = SpriterMainline::create();
      if (%mainlineObject)
      {
         %mainlineObject.keySet = %this.loadKeys();
         echo(%mainlineObject.keySet.getCount() SPC "keys in mainline");
      }
      %xml.popElement();
   }
   return %mainlineObject;
}

// Read the xml tags representing timelines and their attributes.
function SpriterData::loadTimelines(%this)
{
   %xml = %this.xml;
   %set = null;
   
   %timeline = %xml.pushFirstChildElement("timeline");
   if (%timeline)
   {
      %set = new SimSet();
       while (%timeline)
       {
          %timelineObject = SpriterTimeline::create();
          if (%timelineObject)
          {
             %attribute = %xml.firstAttribute();
             while (%attribute !$= "")
             {
                switch$(%attribute)
               {
                  case "id":
                     %timelineObject.id = %xml.attributeS32(%attribute);
                     //echo("timeline" SPC %attribute SPC "=" SPC %timelineObject.id);
                  case "name":
                     %timelineObject.name = %xml.attribute(%attribute);
                     //echo("timeline" SPC %attribute SPC "=" SPC %timelineObject.name);
                  default:
                     echo("Unknown attribute in timeline element");
               }
               %attribute = %xml.nextAttribute();
             }
             %timelineObject.keySet = %this.loadKeys();
             %set.add(%timelineObject);
          }
          %timeline = %xml.nextSiblingElement("timeline");
       }
       %xml.popElement();
   }
   return %set;
}

// Read the xml tags representing the key frames and their attributes.
function SpriterData::loadKeys(%this)
{
   %xml = %this.xml;
   %set = null;
   
   %key = %xml.pushFirstChildElement("key");
   if (%key)
   {
      %set = new SimSet();
      while (%key)
      {
         %keyObject = SpriterKey::create();
         if(%keyObject)
         {
            %attribute = %xml.firstAttribute();
            while (%attribute !$= "")
            {
               switch$(%attribute)
               {
                  case "id":
                     %keyObject.id = %xml.attributeS32(%attribute);
                     //echo(%attribute SPC "=" SPC %keyObject.id);
                  case "time":
                     %keyObject.time = %xml.attribute(%attribute);
                     //echo(%attribute SPC "=" SPC %keyObject.time);
                  case "curve_type":
                     %keyObject.curve_type = %xml.attribute(%attribute);
                     //echo(%attribute SPC "=" SPC %keyObject.curve_type);
                  case "spin":
                     %keyObject.spin = %xml.attribute(%attribute);
                     //echo(%attribute SPC "=" SPC %keyObject.spin);
                  default:
                     echo("Unknown attribute in key element");
               }
               %attribute = %xml.nextAttribute();
            }
            %keyObject.objectsSet = %this.loadObjects();
            %set.add(%keyObject);
         }
         %key = %xml.nextSiblingElement("key");
      }
      %xml.popElement();
   }
   return %set;
}

// Read the xml tags representing the objects and object references and their attributes.
function SpriterData::loadObjects(%this)
{
   %xml = %this.xml;
   %set = null;
   %index = 0;
   
   %object = %xml.pushChildElement(%index);
   if (%object)
   {
      %set = new SimSet();
      while (%object)
      {
         %objectObject = SpriterKey::create();
         if (%objectObject)
         {
            //echo("text =" SPC %xml.elementValue());
            //echo("index =" SPC %index);
            
            if (%xml.elementValue() $= "object_ref")
            {
               %objectObject.reference = true;
               %attribute = %xml.firstAttribute();
               while (%attribute !$= "")
               {
                  switch$(%attribute)
                  {
                     case "id":
                        %objectObject.id = %xml.attributeS32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.id);
                     case "timeline":
                        %objectObject.timeline = %xml.attributeS32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.timeline);
                     case "key":
                        %objectObject.key = %xml.attributeS32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.key);
                     case "z_index":
                        %objectObject.z_index = %xml.attributeS32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.z_index);
                     case "object_type":
                        %objectObject.object_type = %xml.attribute(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.object_type);
                     default:
                        echo("Unknown attribute in key element");
                  }
                  %set.add(%objectObject);
                  %attribute = %xml.nextAttribute();
               }
            }
            else if (%xml.elementValue() $= "object")
            {
               %objectObject.reference = false;
               %attribute = %xml.firstAttribute();
               while (%attribute !$= "")
               {
                  switch$(%attribute)
                  {
                     case "folder":
                        %objectObject.folder = %xml.attributeS32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.folder);
                     case "file":
                        %objectObject.file = %xml.attributeS32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.file);
                     case "x":
                        %objectObject.x = %xml.attributeF32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.x);
                     case "y":
                        %objectObject.y = %xml.attributeF32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.y);
                     case "pivot_x":
                        %objectObject.pivot_x = %xml.attributeF32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.pivot_x);
                     case "pivot_y":
                        %objectObject.pivot_y = %xml.attributeF32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.pivot_y);
                     case "angle":
                        %objectObject.angle = %xml.attributeF32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.angle);
                     case "scale_x":
                        %objectObject.scale_x = %xml.attributeF32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.scale_x);
                     case "scale_y":
                        %objectObject.scale_y = %xml.attributeF32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.scale_y);
                     case "a":
                        %objectObject.a = %xml.attributeF32(%attribute);
                        //echo(%attribute SPC "=" SPC %objectObject.a);
                     default:
                        echo("Unknown attribute in key element");
                  }
                  %set.add(%objectObject);
                  %attribute = %xml.nextAttribute();
               }
            }
         }
         %xml.popElement();
         %object = %xml.pushChildElement(%index++);
      }
   }
   return %set;
}

// Class to represent Spriter folders
function SpriterFolder::create()
{
   %this = new ScriptObject()
   {
      class = SpriterFolder;
      fileSet = null;
      id = 0;
      name = "";
   };
   return %this;
}

function SpriterFile::create()
{
   %this = new ScriptObject()
   {
      class = SpriterFile;
      type = "";
      id = 0;
		name = "";
		width = 0;
		height = 0;
		pivot_x = 0.0;
		pivot_y = 1.0;
   };
   return %this;
}

function SpriterEnt::create()
{
   %this = new ScriptObject()
   {
      class = SpriterEnt;
      id = 0;
		name = "";
		animationSet = null;
   };
   return %this;
}

function SpriterAnimation::create()
{
   %this = new ScriptObject()
   {
      class = SpriterAnimation;
      id = 0;
		name = "";
		length = 0;
		looping = false;
		mainline = null;
		timelineSet = null;
   };
   return %this;
}

// Class to represent spriter mainlines
function SpriterMainline::create()
{
   %this = new ScriptObject()
   {
      class = SpriterMainline;
      keySet = null;
   };
   return %this;
}

// Class to represent spriter timelines
function SpriterTimeline::create()
{
   %this = new ScriptObject()
   {
      class = SpriterTimeline;
      id = 0;
		name = "";
		object_type = "sprite";
		variable_type = "string";
		usage = "display";
		keySet = null;
   };
   return %this;
}

// Class to represent spriter animation key
function SpriterKey::create()
{
   %this = new ScriptObject()
   {
      class = SpriterKey;
      id = 0;
		time = 0;
		spin = 1;
		objectSet = null;
   };
   return %this;
}

// Class to represent spriter objects and object references
function SpriterObject::create()
{
   %this = new ScriptObject()
   {
      class = SpriterObject;
      reference = false;
	   
	   // Object reference fields
		id = 0;
		timeline = 0;
		key = 0;
		z_index = 0;
		object_type = "sprite";
		
		// Object fields
		folder = "";
		file = "";
		imageMap = null;
		x = 0.0;
		y = 0.0;
		pivot_x = 0;
		pivot_y = 0;
		angle = 0.0;
		scale_x = 1.0;
		scale_y = 1.0;
		a = 1.0;
   };
   return %this;
}
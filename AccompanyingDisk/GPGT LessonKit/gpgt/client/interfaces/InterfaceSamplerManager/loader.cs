//------------------------------------------------------
// Copyright 2000-2006, GarageGames.com, Inc.
// Written by Ed Maurina, Hall Of Worlds, LLC
//------------------------------------------------------
echo("\n\c3--------- Loading Interfaces Sampler ---------");
//--------------------------------------------------------------------------
// loader.cs
//--------------------------------------------------------------------------
//
// This is the loader file.  It has the responsibility of finding all the 
// currently existing interface samples and creating menus to load them with.
//

//--------------------------------------------------------------------------
// Profiles
//--------------------------------------------------------------------------
if (!isObject(gsLabel)) new GuiControlProfile (ifcsLabel) {
	border      = 0;
	fontType    = "Tahoma";
	fontSize    = 14;
	fontColor   = "32 32 255";
	justify		= right;
};

if (!isObject(gsBorder)) new GuiControlProfile (ifcsBorder) {
	border				= 2;
	borderThickeness    = 1;
	borderColor         = "0 0 0";
};




$SampleInterfacesSelector::ButtonFormatPrefix = "<just:center><color:5efa20><font:Arial Bold:18>";
$SampleInterfacesSelector::ButtonNamePrefix  = "interfaceChoice";
$SampleInterfacesSelector::ButtonCount       = 10;

//--------------------------------------------------------------------------
// Scripts
//--------------------------------------------------------------------------
// onAdd() - When this control is added, we want to build a list of entries
//           that can be loaded.
function SampleInterfacesSelector::onAdd( %this ) 
{
   %this.inSample = false;

   // In order to allow for distribution of this kit w/o .cs files,
   // The auto-loader code does the following:
   // 1. Attempt to load DSO files ONLY.
   // 2. Failing that, try the same for CS files.
   //
   // Note: One drawback of doing this is the fact that newly added CS files
   // will not be seen until you clean away all DSO files.  Of course,
   // this also keeps folks from 'extending' the free version of this kit, so
   // the tradeoff is probably fair.
   //
   %searchPattern = "*/ifcs*.cs.dso";
   %tmp = findFirstFile(%searchPattern);
   if("" $= %tmp) %searchPattern = "*/ifcs*.cs";

   //// 1. Build list of samplers (source files).
   //
   %this.guiTotalSamples=0;

   %tmp = findFirstFile(%searchPattern);
   %tmp = strReplace( %tmp , ".dso", "" );

   %this.interfaceSampleLoader[%this.guiTotalSamples] = %tmp;

   while("" !$= %this.interfaceSampleLoader[%this.guiTotalSamples]) 
   {
         %this.guiTotalSamples++;

         %tmp = findNextFile(%searchPattern);
         %tmp = strReplace( %tmp , ".dso", "" );

         %this.interfaceSampleLoader[%this.guiTotalSamples] = %tmp;
   }

   // 2. Sort the list of samplers (source files).
   //
   // We want to sort our list, but there is no method for sorting
   // scripted arrays. So, instead of writing our own code, let's
   // cheat and use a GUI control as a proxy to do the work.
   // The GuiTextListCtrl can sort entries alphabetically, and
   // allows us to retrieve elements by row, making it ideal for the 
   // job.
   if (%this.guiTotalSamples > 1) 
   {
      %tempControl = new GuiTextListCtrl();
      for(%count=0; %count < %this.guiTotalSamples ; %count++) 
      {
         %tempControl.addRow( 0 , %this.interfaceSampleLoader[%count] );
      }

      %tempControl.sort( 0 , true );

      for(%count=0; %count < %this.guiTotalSamples ; %count++) 
      {
         %this.interfaceSampleLoader[%count] = %tempControl.getRowText( %count );
      }

      %tempControl.delete();
   }

   // 3. Build list of button names and target interfaces.
   //
   // In this step, we create two additional arrays. One of button text,
   // and, a second of the control associated with that button.  As long as 
   // the rules have been followed for the creation of samplers, this
   // will work.  That is, each sampler loader file must have the same 
   // name as the control it loads.
   //
   // To simply the jobs, we'll take the paths we have and put them through the 
   // following process:
   // 1. Copy to temporary variable.
   // 2. convert all '/' to spaces
   // 3. Replace all '.cs' with NULL strings.
   // 4. Set control name to last word is this string.
   // 5. Copy control name to button name.
   // 6. Replace 'gs' with NULL string.
   if(%this.guiTotalSamples) 
   {
      for(%count=0; %count < %this.guiTotalSamples ; %count++) 
      {
         %tmpStr = %this.interfaceSampleLoader[%count];
         %tmpStr = strReplace( %tmpStr , "/" , " "  );
         %tmpStr = strReplace( %tmpStr , "_" , ""  );
         %tmpStr = strReplace( %tmpStr , ".cs" , "" );

         %this.interfaceSampleControlName[%count] =
            getWord( %tmpStr , getWordCount( %tmpStr ) - 1 );

         %this.interfaceSampleControlName[%count] =
            strReplace( %this.interfaceSampleControlName[%count] , "-" , "" );

         %this.interfaceSampleButtonText[%count] =
            strReplace( getWord( %tmpStr , getWordCount( %tmpStr ) - 1 ) , "ifcs" , "" );
         %this.interfaceSampleButtonText[%count] =
            strReplace( %this.interfaceSampleButtonText[%count] , "-" , " " );

         //echo("Loader found .... ", %this.interfaceSampleLoader[%count]);
         //echo("Button text  .... ", %this.interfaceSampleButtonText[%count]);
         //echo("Control name .... ", %this.interfaceSampleControlName[%count]);
      }
   }

   // 4. Load the samples.
   //
   // In this last step, we iterate over our list of samples and load them.
   if(%this.guiTotalSamples) 
   {
      for(%count=0; %count < %this.guiTotalSamples ; %count++) 
      {
         exec( %this.interfaceSampleLoader[%count] );
      }
   }
}

function SampleInterfacesSelector::loadSamples( %this ) {
}

function SampleInterfacesSelector::onWake( %this ) 
{
   if( %this.inSample ) {
      %this.inSample = false;
   } else {
      %this.currentIndex = 0;
   }
   %this.flipPage( 0 , false);
}

function SampleInterfacesSelector::flipPage( %this , %direction , %normalFlip ) 
{   
   echo("\c3%normalFlip => ", %normalFlip);
   if( %normalFlip ) 
   {
      if( %direction) {
         echo("Right");

         %nextIndex = %this.currentIndex + $SampleInterfacesSelector::ButtonCount;
         %this.currentIndex = ( %nextIndex < %this.guiTotalSamples ) ?
            %nextIndex : %this.currentIndex;

      } else {
         echo("Left");

         %nextIndex = %this.currentIndex - $SampleInterfacesSelector::ButtonCount;
         if(%nextIndex < 0 ) {
            %this.currentIndex = 0;
            Canvas.setContent(%this.Parent);
            return;
         } 
         %this.currentIndex = %nextIndex;
      }

   }

   echo("\c3%this.currentIndex == ", %this.currentIndex);

   for(%count = 0; %count < $SampleInterfacesSelector::ButtonCount; %count++) 
   {
      %tmpIndex = %count + %this.currentIndex;

      if( %tmpIndex < %this.guiTotalSamples ) 
      {
         echo("\c3", $SampleInterfacesSelector::ButtonNamePrefix @ %count);
         %button = ($SampleInterfacesSelector::ButtonNamePrefix @ %count).getID();
         %MLText = %button.getObject(0);
         %MLText.setText($SampleInterfacesSelector::ButtonFormatPrefix @ 
            %this.interfaceSampleButtonText[%tmpIndex] );
         %button.setActive(1);
         %button.command = "Canvas.setContent(" @ 
            %this.interfaceSampleControlName[%tmpIndex] @ ");SampleInterfacesSelector.inSample=true;";
         InterfacesRight.setActive(true);

         // Un-hook placeholder buttons
         if( "Placeholder" $= %this.interfaceSampleButtonText[%tmpIndex] ) {
            %button.command="";
            %button.setActive(0);
         }
      } else {
         %button = ($SampleInterfacesSelector::ButtonNamePrefix @ %count).getID();
         %MLText = %button.getObject(0);
         %MLText.setText($SampleInterfacesSelector::ButtonFormatPrefix @ "--");
         %button.setActive(0);
         %button.command = "";
         InterfacesRight.setActive(false);
      }
   }
}

//--------------------------------------------------------------------------
// Load Interface Definitions
//--------------------------------------------------------------------------


//--------------------------------------------------------------------------
// loadSampler.cs
//--------------------------------------------------------------------------
exec("./SampleInterfacesSelector.gui");


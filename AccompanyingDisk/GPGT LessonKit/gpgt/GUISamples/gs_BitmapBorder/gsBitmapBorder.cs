//------------------------------------------------------
// Copyright 2000-2006, GarageGames.com, Inc.
// Written by Ed Maurina, Hall Of Worlds, LLC
//------------------------------------------------------
echo("\c3--------- Loading GuiBitmapBorder Samples ---------");
//--------------------------------------------------------------------------
// gsBitmapBorder.cs
//--------------------------------------------------------------------------
//--------------------------------------------------------------------------
// 
// In this file, we demonstrate, the GuiBitmapBorder:
//

//--------------------------------------------------------------------------
// Profiles
//--------------------------------------------------------------------------
if(!isObject(gsUglyBorder)) new GuiControlProfile(gsUglyBorder)
{
	hasBitmapArray = true;
	bitmap = "./uglyborder";
};

//--------------------------------------------------------------------------
// Scripts
//--------------------------------------------------------------------------


//--------------------------------------------------------------------------
// Load Interface Definition
//--------------------------------------------------------------------------
exec("./gsBitmapBorder.gui");


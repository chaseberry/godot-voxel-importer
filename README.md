godot-voxel-importer
====================
A voxel file importer for Godot

https://github.com/ephtracy/voxel-model
https://ephtracy.github.io/

Import Types
------------

### Object as Mesh

### Object as Mesh Library

### Objects as Mesh Library

### Objects as Meshes

### Objects as Mesh Libraries

### Packed Scene

Import Options
--------------

### Scale
What scale of each voxel during the import process. A scale of one means each voxel is 1x1x1 in Godot World Space.

### Include Invisible
When checked, objects layers that are marked invisible will be included in the output model.
When unchecked, object layers that are marked invisible will not be included in the output model.

### Set Origin At Bottom
When checked, will set the origin of the model to the center on the X/Z axis, and the bottom of the Y axis. 
When un-checked, will set the origin of the model to the center of the imported object.

### Ignore Transforms
When checked, model offsets from each other in object mode will be ignored, '0-ing' out the center of each one
When unchecked, models will keep their offsets from each other 

### Apply Materials
When checked, material settings from the voxel file will be used in the output model.
When unchecked, the output model will use a flat material colored by the voxel colors.

### Merge All Frames
When checked, all frames from the imported object will be used to generate the output model.
When unchecked, only the first frame from the object will be used to generate the output model.

### Output Directory
When using the 'Objects as Meshes' or 'Objects as Mesh Libraries' this is the output directory for the 2nd through Nth
imported object. The primary object will be treated as a normal Godot Import

### Output Header
When using the 'Objects as Meshes' or 'Objects as Mesh Libraries' this is the header applied to each output file.

### Packed Scene Logic
How to handle the creation of a packed scene.
1. Smart Objects - 
2. First Key Frame - 
3. Merge Key Frames - 
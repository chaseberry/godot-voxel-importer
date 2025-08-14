godot-voxel-importer
====================
A voxel file importer for Godot

https://github.com/ephtracy/voxel-model
https://ephtracy.github.io/

Import Types
------------

### Object as Mesh
Imports the object as a single Mesh. If the model contains more than one object, they are merged together.

### Object as Mesh Library
Imports the object as a Mesh Library, with each frame being a Mesh3 in the library. Each object is merged together at
their closest frame index. For example, if a model has two objects, one with frames 1, 3 and the other with frames 2, 3,
the final library will have 3 frames [1, 2], [1, 2], [3, 3].

### Objects as Mesh Library
Imports each separate object as a Mesh in a MeshLibrary

### Objects as Meshes


### Objects as Mesh Libraries

### Packed Scene

Import Options
--------------

### Scale
The scale of each voxel during the import process. A scale of `1` means each voxel is `1x1x1` in Godot World Space.

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
1. Smart Objects - If the object only has 1 Frame, create a Mesh Instance, if the object has multiple frames, use an
   AnimatableMesh.
2. First Key Frame - Use the first frame of each object for the output model
3. Merge Key Frames - Combing all frames of each object for the output model
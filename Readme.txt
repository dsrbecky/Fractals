Requirements:
 - .NET Framework 2.0

General features: 
 - saving and loading of formulas
 - formulas are compiled and can be changed at run-time
 - colorization formulas
 - caching of results in a tree (for a long time)
 - rendering and calculation each have its own thread

Graphics features:
 - zoom in (left mouse button)
 - zoom out (right mouse button)
 - pan (middle mouse button)
 - rotate (mouse wheel)
 - caching of bitmaps in a queue (for a short time)
 - automatic adjustment of quality on movement to obtain desired frames per second
 - continuous improvement of quality when there is no movement
 - several levels of anti-aliasing
 - areas with details are enhanced first (ie they are preferred to plain areas)
 - only visible fragments are calculated and rendered (works for rotated images too)


TODO aka planed features:
 - refactor the two years old code
 - port from GDI to OpenGL
 - add cache compression
 - release on SourceForge!
These shaders where made for the game Oberon's Court. You can support the game by sharing the shaders or just checking up on progress occasionaly. You can do this on twitter @littlechicken01 or on facebook http://facebook.com/oberonscourt or slidedb  http://www.slidedb.com/games/oberons-court


You are free to use these shaders as you see fit, please don't distribute  them commercially as-is, but you can use them in your indie game however you want,even if its a commercial game.   

The shaders are mostly undocumented, uncommented and a mess. But they are exactly how I use them to create the textureless look of Oberon's Court (which I call pure3D).
A tutorial will be available shortly on http://blog.littlechicken.nl. 

The shaders where made with strumpy shader or shaderforge for unity3d, which I very much recommend any unity3D developer purchases. 



On getting your models to work with the shaders.

most shaders in the package require:

a) a planar map, to create the color gradient, (direction of the uvs determines the direction of the gradient)
c) vertex color data, usually a radiosity solution baked in from 3dsmax or maya
d) some tinkering to work.. 

The most important shader, as seen in all the screenshots is called Oberon_landscape_shadows, it's a shadow enabled shader optimized for mobile, combining color gradients, masked fresnel and vertex radiosity with an unlit shadow compatible lighting model. 



Enjoy, and sorry the tutorial isn't up yet.
Kind regards,
Tomas Sala

To use this in your project:

- Add mojoshader*.c and mojoshader*.h to your project.
- Compile mojoshader*.c
- If you don't have a C99-compliant compiler, like Microsoft Visual Studio,
  you'll need to compile the .c files as C++ to get them to build.
- If you don't have cmake to generate mojoshader_version.h, you can either
  add a blank file with that name, or add MOJOSHADER_NO_VERSION_INCLUDE to
  your preprocessor definitions.

// end of README.txt ...



MonoGame Build Instructions
---------------------------

Windows:

 - You need CMake to generate the projects.
 - First run `cmake -G "Visual Studio 11"` to generate projects for 32bit.
 - Open the solution and unload the finderrors, test, testoutput, and testparse projects.
 - Select and build the lemon and mojoshader projects under the MinSizeRel config.
 - Copy the 32bit lemon.exe parser generator from the MinSizeRel folder to a safe place.
 - Clean and run `cmake -G "Visual Studio 11 Win64"` to generate 64bit projects.
 - Open the solution and unload the finderrors, test, testoutput, testparse, and lemon projects.
 - Copy the 32bit lemon.exe to the MinSizeRel folder.
 - Select and build the mojoshader project under the MinSizeRel config.
 

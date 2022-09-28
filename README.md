Thimbleweed Park Explorer
===================
![Screenshot](https://quickandeasysoftware.net/wp/wp-content/uploads/2018/05/ThimbleweedParkExplorer.png)

An explorer/viewer/dumper tool for Thimbleweed Park.

Made as a test project for relearning C# so the code will be wonky.

It uses the following nuget packages:
* NAudio
* NAudio Vorbis
* ObjectListView
* WindowsAPICodePack-Core
* WindowsAPICodePack-Shell
* Newtonsoft.Json
* BCnEncoder

http://quickandeasysoftware.net

Forked to add support for the recently released "Return to Monkey Island" ([Official website](https://returntomonkeyisland.com))
Status:
The files can be read, the contents can be saved to disk. 

Since the game uses longer keys for the decryption algorithm, the keys are extracted from the game's executable the first time a resource file is opened.
